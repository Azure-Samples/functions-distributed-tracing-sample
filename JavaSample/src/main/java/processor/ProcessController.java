package processor;

import java.io.UnsupportedEncodingException;

import com.microsoft.applicationinsights.TelemetryClient;
import com.microsoft.applicationinsights.TelemetryConfiguration;
import com.microsoft.applicationinsights.extensibility.ContextInitializer;
import com.microsoft.applicationinsights.telemetry.Duration;
import com.microsoft.applicationinsights.telemetry.RemoteDependencyTelemetry;
import com.microsoft.applicationinsights.telemetry.RequestTelemetry;
import com.microsoft.applicationinsights.web.extensibility.initializers.WebAppNameContextInitializer;
import com.microsoft.applicationinsights.web.internal.RequestTelemetryContext;
import com.microsoft.applicationinsights.web.internal.ThreadContext;
import com.microsoft.applicationinsights.web.internal.correlation.TelemetryCorrelationUtils;
import com.microsoft.azure.servicebus.Message;
import com.microsoft.azure.servicebus.ReceiveMode;
import com.microsoft.azure.servicebus.primitives.ConnectionStringBuilder;
import com.microsoft.azure.servicebus.primitives.ServiceBusException;
import com.microsoft.azure.servicebus.QueueClient;

import org.springframework.http.HttpEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class ProcessController {
    private static final String connectionString = System.getenv("CONNECTION_STRING");;
    private static final String currentTemplate = "Current Request Id: %s RootId: %s ParentId: %s";
    private static final String queueName = System.getenv("QUEUE");
    private static final String template = "Parent Request-Id: %s";

    @RequestMapping("/process")
    public Message process(HttpEntity<byte[]> requestEntity) throws ServiceBusException, InterruptedException {
        //TODO Support W3C Trace Context
        String parentRequestId = requestEntity.getHeaders().getFirst("Request-Id");
        System.out.println(String.format(template, parentRequestId));

        // Request Telemetry information
        // Request is automatically tracked by the filter. 
        // Please refer configrations/AppInsightsConfig.java
        RequestTelemetryContext context = ThreadContext.getRequestTelemetryContext();
        RequestTelemetry requestTelemetry = context.getHttpRequestTelemetry();
        System.out.println(
            String.format(currentTemplate,
            requestTelemetry.getId(),
            requestTelemetry.getContext().getOperation().getId(),
            requestTelemetry.getContext().getOperation().getParentId()
            )
        );
        //TODO Dependency Telemetry Traking
        String dependencyId = TelemetryCorrelationUtils.generateChildDependencyId();
        RemoteDependencyTelemetry dependencyTelemetry = new RemoteDependencyTelemetry("Send ServiceBus Queue");
        dependencyTelemetry.setId(dependencyId);
        dependencyTelemetry.getContext().getOperation().setId(
            requestTelemetry.getContext().getOperation().getId()
        );
        dependencyTelemetry.getContext().getOperation().setParentId(
            requestTelemetry.getId()
        );
        Duration duration = new Duration(0,0,0,0,10); // set the duration 10 millisec as an example.
        dependencyTelemetry.setDuration(duration);
        // Maybe we can copy RequestTelemetry parameters to the Dependnecy telemetry. 
        TelemetryConfiguration configuration = TelemetryConfiguration.getActive();
        // This might not be needed. It works without this.
        // configureWebAppNameContextInitializer("JavaSample.Process", configuration);
        TelemetryClient telemetryClient = new TelemetryClient(configuration);
        telemetryClient.trackDependency(dependencyTelemetry);

        // Create QueueClient and send Message
        QueueClient queueClient = new QueueClient(new ConnectionStringBuilder(connectionString, queueName), ReceiveMode.PEEKLOCK);
        queueClient.sendAsync(new Message(String.format(template, parentRequestId)));
        queueClient.close();

        return new Message(String.format(template, parentRequestId));
    }

    private void configureWebAppNameContextInitializer(String appName, TelemetryConfiguration configuration) {
        for (ContextInitializer ci : configuration.getContextInitializers()) {
            if (ci instanceof WebAppNameContextInitializer) {
                ((WebAppNameContextInitializer)ci).setAppName(appName);
                return;
            }
        }
    }
}
package processor;

import java.io.UnsupportedEncodingException;

import com.microsoft.applicationinsights.telemetry.RequestTelemetry;
import com.microsoft.applicationinsights.web.internal.RequestTelemetryContext;
import com.microsoft.applicationinsights.web.internal.ThreadContext;

import org.springframework.http.HttpEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class ProcessController {
    private static final String template = "Parent Request-Id: %s";
    private static final String currentTemplate = "Current Request Id: %s RootId: %s ParentId: %s";
    @RequestMapping("/process")
    public Message process(HttpEntity<byte[]> requestEntity) throws UnsupportedEncodingException {
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
        //TODO Send ServiceBus Queue

        return new Message(String.format(template, parentRequestId));
    }
}
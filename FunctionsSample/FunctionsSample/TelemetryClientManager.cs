using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;

namespace FunctionsSample
{
    public class TelemetryClientManager
    {
        private static TelemetryClient _client;

        static TelemetryClientManager()
        {
            SetUpTelemetryClient();
            SetUpEventSourceLog();
        }

        public static TelemetryClient Client => _client;

        private static void SetUpTelemetryClient()
        {
            DependencyTrackingTelemetryModule module = new DependencyTrackingTelemetryModule();
            module.ExcludeComponentCorrelationHttpHeadersOnDomains.Add("core.windows.net");

            var config = TelemetryConfiguration.CreateDefault();
#pragma warning disable 618
            // TODO This is for W3C correlation
            // config.TelemetryInitializers.Add(new W3COperationCorrelationTelemetryInitializer());

            // Enabling Http, ServiceBus, EventHubs automatically handle the dependency tracking. 
            module.Initialize(config);
            // Set the instrumentKey
            config.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");

            _client = new TelemetryClient(config);
        }

        private static void SetUpEventSourceLog()
        {
            var telemetryEventSource = EventSource.GetSources()
                .FirstOrDefault(es => es.Name == "Microsoft-ApplicationInsights-Core");
            if (telemetryEventSource != null)
            {
                var myEventListener = new MyEventListener();
                myEventListener.EnableEvents(telemetryEventSource, EventLevel.LogAlways);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.W3C;

namespace FunctionsSample
{
    public static class StartOperation
    {

        [FunctionName("StartOperation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("myqueue", Connection = "StorageConnectionString")] IAsyncCollector<QueueMessage> message,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var current = Activity.Current;

            string msg = req.Query["message"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            msg = msg ?? data?.msg;

            var dependency = new DependencyTelemetry("Azure queue", "Storage Queue", "Enqueue myqueue", null);

            // StartOperation automatically create/start Activity. The parent is the Activity.Current
            // StopOperation automatically stop DependencyTelemetry then send telemetry. 
            // If it does not emit the telemetry, please check the EventSource "Microsoft-ApplicationInsights-Core" as Output of the VS. 
            using (TelemetryClientManager.Client.StartOperation(dependency))
            {
                current = Activity.Current;
                var queueMessage = new QueueMessage()
                {
                    // TODO support W3C TraceContext
                    ParentId = Activity.Current.Id,
                    Message = msg
                };
                await message.AddAsync(queueMessage);
                return (ActionResult)new OkObjectResult($"Successfully Start Operation. Id: {current.Id} ParentId: {current.ParentId} RootId: {current.RootId}");
            }
        }
    }
}

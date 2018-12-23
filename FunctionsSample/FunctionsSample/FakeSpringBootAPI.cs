using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace FunctionsSample
{
    public static class FakeSpringBootAPI
    {
        [FunctionName("FakeSpringBootAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# FakeSpringBootAPI receive the rquest");

            var current = Activity.Current;
            log.LogInformation($"Id:{current.Id} ParentId:{current.ParentId} RootId:{current.RootId} ");


            // Inside of the header, it Request-Id is included. 
            req.Headers.TryGetValue("Request-Id", out StringValues requestId);

            // TODO HttpTrigger should respect Request-Id. However, they don't 
            var requestActivity = new Activity("Receive HttpRequest as a Fake Spring API");
            requestActivity.SetParentId(requestId.FirstOrDefault());
            requestActivity.Start();
            
            var requestTelemetry = requestActivity.CreateAndStartRequestTelemetry();

            // do something

            requestTelemetry.Stop();
            TelemetryClientManager.Client.Track(requestTelemetry);

            return (ActionResult)new OkObjectResult($"Thank you for calling Fake API");
        }
    }
}

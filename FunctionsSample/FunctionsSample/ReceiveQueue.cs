using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionsSample
{

    public static class ReceiveQueue
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("ReceiveQueue")]
        public async static Task Run(
            [QueueTrigger("myqueue", Connection = "StorageConnectionString")]QueueMessage myQueueItem, 
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed");
            var requestActivity = new Activity("Receive Storage Queue");
            requestActivity.SetParentId(myQueueItem.ParentId);
            requestActivity.Start();

            var requestTelemetry = requestActivity.CreateAndStartRequestTelemetry();

            // Do something. 

            // HttpClient outboud request is automatically tracked. 
            
            // this works. 
            // var response = await client.GetStringAsync("https://httpbin.org/get");

            // for Fake API of Azure Functions
            // var response =  await client.GetStringAsync("http://localhost:7071/api/FakeSpringBootAPI");
            var response = await client.GetStringAsync("http://localhost:8080/process");

            requestTelemetry.Stop();
            TelemetryClientManager.Client.Track(requestTelemetry);
        }
    }
}

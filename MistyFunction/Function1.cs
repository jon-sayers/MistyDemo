using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MistyFunction
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("Function1")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "Wez")]EventData message, ILogger log)
        {
            string readingString = Encoding.UTF8.GetString(message.Body.Array);

            log.LogInformation($"Id: {JsonConvert.SerializeObject(message.SystemProperties["iothub-connection-device-id"])}");

            log.LogInformation($"C# IoT Hub trigger function processed a message: {readingString}");
        }


    }
}
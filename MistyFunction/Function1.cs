using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System;

namespace MistyFunction
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();

        private static ServiceClient s_serviceClient;


        [FunctionName("Function1")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "Wez")]EventData message, ILogger log)
        {
            string readingString = Encoding.UTF8.GetString(message.Body.Array);

            log.LogInformation($"Id: {JsonConvert.SerializeObject(message.SystemProperties["iothub-connection-device-id"])}");

            log.LogInformation($"C# IoT Hub trigger function processed a message: {readingString}");

            //s_serviceClient = ServiceClient.CreateFromConnectionString("HostName=wezmondo.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=GqNdJYcXbTJ/kyp73kRUhIN/Zjx5BTS8yqsh3Trbka4=");
        }

        //public SendAlert()
        //{

        //    var method = new CloudToDeviceMethod("RemoteCommand", 30, 0);

        //    method.SetPayloadJson("{}");

        //    var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, method);
        //}

    }
}
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
        public static void Run([IoTHubTrigger("messages/events", Connection = "Wez")]EventData message, out object taskDocument, ILogger log)
        {
            string readingString = Encoding.UTF8.GetString(message.Body.Array);

            log.LogInformation($"Id: {JsonConvert.SerializeObject(message.SystemProperties["iothub-connection-device-id"])}");

            log.LogInformation($"C# IoT Hub trigger function processed a message: {readingString}");

            taskDocument = null;
        }



    }

    //[FunctionName("Function1")]
    //[return: Table("yourtablename", Connection = "StorageConnectionAppSetting")]
    //public static TempHumidityIoTTableEntity Run([IoTHubTrigger("messages/events", Connection = "ConnectionStringSetting")] EventData message, TraceWriter log)
    //{
    //    var messageAsJson = Encoding.UTF8.GetString(message.GetBytes());
    //    log.Info($"C# IoT Hub trigger function processed a message: {messageAsJson}");

    //    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(messageAsJson);

    //    var deviceid = message.SystemProperties["iothub-connection-device-id"];

    //    return new TempHumidityIoTTableEntity
    //    {
    //        PartitionKey = deviceid.ToString(),
    //        RowKey = $"{deviceid}{message.EnqueuedTimeUtc.Ticks}",
    //        DeviceId = deviceid.ToString(),
    //        Humidity = data.ContainsKey("humidity") ? data["humidity"] : "",
    //        Temperature = data.ContainsKey("temperature") ? data["temperature"] : "",
    //        DateMeasured = message.EnqueuedTimeUtc.ToString("O")
    //    };

    //}


}
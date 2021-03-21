using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace FunctionMisty
{
    public class HubFunction
    {
        private readonly MReadings _readings;

        public HubFunction(MReadings readings)
        {
            _readings = readings;
        }
         
        private static HttpClient client = new HttpClient();


        [FunctionName("HubFunction")]
        public void Run([IoTHubTrigger("messages/events", Connection = "wezhub")] EventData message, [CosmosDB(
                databaseName: "wezmondoIOT",
                collectionName: "wezmondoIOT",
                ConnectionStringSetting = "wezdata")]out dynamic document, ILogger log)
        {
            string bodyString = Encoding.UTF8.GetString(message.Body.Array);

            log.LogInformation($"Id: {JsonConvert.SerializeObject(message.SystemProperties["iothub-connection-device-id"])}");

            log.LogInformation($"C# IoT Hub trigger function processed a message: {bodyString}");

            // ADD READING TO SINGLETON SO WE CAN ANALYSE THE LAST 100 READINGS...

            MReading reading = JsonConvert.DeserializeObject<MReading>(bodyString);

            _readings.AddReading(reading);

            log.LogInformation($"Readings: {_readings.Readings.Count()}");

            checkExposure(message.SystemProperties["iothub-connection-device-id"].ToString());

            // CONVERT DATA INTO EXISTING PROJECT FORMAT SO I CAN QUERY IT ONLINE FOR TESTING...

            MistyPayload payload = new MistyPayload();
            payload.Alerts = reading.Alerts;
            payload.Values = reading.Payload;
            WSPayload toSend = new WSPayload();
            toSend.ProjectID = 1;
            toSend.RoleID = 6;
            toSend.DeviceUID = message.SystemProperties["iothub-connection-device-id"].ToString();
            toSend.Sent = reading.Time;
            toSend.Data = payload;

            // OUT VALUE...

            document = JsonConvert.SerializeObject(toSend);
        }

        public async void checkExposure(string deviceId)
        {
            // MOCK EXPOSURE CHECKER THAT SENDS AN ALERT TO THE HUB/DEVICE EVERY 10 READINGS...

            if (_readings.Readings.Count() % 10 == 0)
            {
                ServiceClient _serviceClient = ServiceClient.CreateFromConnectionString(System.Environment.GetEnvironmentVariable("wezhubhost"));

                var method = new CloudToDeviceMethod("Alert", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(0));

                if (_readings.Readings.Count() % 20 == 0)
                {
                    method.SetPayloadJson("{\"exposure\":true}");
                }
                else
                {
                    method.SetPayloadJson("{\"exposure\":false}");
                }

                try
                {
                    var response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, method);
                }
                catch
                {

                }


                _serviceClient.Dispose();
            }


        }
    }
}
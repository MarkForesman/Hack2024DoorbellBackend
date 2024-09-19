using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SmartPackagingNotifier
{
    public class PackageNotifier
    {
        private GetConfiguration ConfigurationService;
        [FunctionName("PackageNotifier")]
        public async Task Run([EventHubTrigger("doorbellhub", Connection = "eventhub_connection_string")] EventData[] events, ILogger log, ExecutionContext context)
        {
            var exceptions = new List<Exception>();
            ConfigurationService = new GetConfiguration(context);
            string[] signaler_devices = ConfigurationService.GetArray("signaler_device_ids");
            string iotServiceConnectionString = ConfigurationService.Get("iot_service_connection_string");
            var queueSender = new QueueSender(ConfigurationService.Get("storage_account_name"), ConfigurationService.Get("storage_account_queue_name"), ConfigurationService.Get("storage_account_key"));


            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    try
                    {
                        //deserialize messageBody into ButtonPressEvent
                        var buttonPressEvent = JsonSerializer.Deserialize<ButtonPressEvent>(messageBody);
                        if (buttonPressEvent != null)
                        {
                            if (buttonPressEvent.DeviceType.Equals("Doorbell", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var c2d = new Cloud2Device(iotServiceConnectionString, signaler_devices);
                                var ce = new CommandEvent<PackageArrivedEvent>()
                                {
                                    TimeStamp = DateTime.UtcNow,
                                    Payload = new PackageArrivedEvent
                                    {
                                        Location = Constants.GetLocation(buttonPressEvent.Button)
                                    }
                                };

                                await c2d.SendCloudToDeviceMessageAsync(ce);
                            }
                            else if (buttonPressEvent.DeviceType.Equals("Signaler", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var c2d = new Cloud2Device(iotServiceConnectionString, signaler_devices);
                                var ce = new CommandEvent<PackagePickerConfirmedEvent>()
                                {
                                    TimeStamp = DateTime.UtcNow,
                                    Payload = new PackagePickerConfirmedEvent()
                                    {
                                        DeviceId = buttonPressEvent.DeviceId,
                                        Location = Constants.GetLocation(buttonPressEvent.Button)                                       
                                    }
                                };
                                await c2d.SendCloudToDeviceMessageAsync(ce);
                            }
                            else if (buttonPressEvent.DeviceType.Equals("LabelScanner", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var c2d = new Cloud2Device(iotServiceConnectionString, signaler_devices);
                                var ce = new CommandEvent<PackageLabelScanEvent>()
                                {
                                    TimeStamp = DateTime.UtcNow,
                                    Payload = new PackageLabelScanEvent
                                    {
                                        DeviceId = buttonPressEvent.DeviceId,
                                        Path = buttonPressEvent.Metadata
                                    }
                                };
                                await queueSender.Send(ce);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        log.LogError($"{ex.Message.GetType()} - {ex.Message}");
                    }
                    // Replace these two lines with your processing logic.
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}

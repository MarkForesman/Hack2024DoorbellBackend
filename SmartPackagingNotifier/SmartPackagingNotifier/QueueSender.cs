using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace SmartPackagingNotifier
{
    internal class QueueSender
    {
        private QueueClient queueClient { get; init; }
        private readonly ILogger<PackageNotifier> _logger;

        public QueueSender(string storageAccountName, string queueName, ILogger<PackageNotifier> logger)
        {            
            _logger = logger;
            using (_logger.BeginScope("Initiate connection to Azure Storage Queue"))
            {
                _logger.LogInformation("Getting default Azure Credentials");
                var credential = new DefaultAzureCredential();
                _logger.LogInformation($"Aquired credential type: {credential.GetType().Name}");
                // Create a URI to the queue
                Uri queueUri = new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}");

                // Create a QueueClient object
                _logger.LogInformation("Creating QueueClient object");
                queueClient = new QueueClient(queueUri, credential);
            }
        }

        public async Task Send<T>(T message)
        {
            await queueClient.CreateIfNotExistsAsync();
            try
            {
                await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))));
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}

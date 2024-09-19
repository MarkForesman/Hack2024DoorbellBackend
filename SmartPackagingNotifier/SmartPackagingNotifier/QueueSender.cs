using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Queues;

namespace SmartPackagingNotifier
{
    internal class QueueSender
    {
        public QueueClient queueClient { get; init; }
        public QueueSender(string storageAccountName, string queueName, string accountKey)
        {
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(storageAccountName, accountKey);
            // Create a URI to the queue
            Uri queueUri = new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}");

            // Create a QueueClient object
            queueClient = new QueueClient(queueUri, credential);
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

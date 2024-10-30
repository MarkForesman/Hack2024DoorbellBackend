using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPackagingNotifier
{
    public class FunctionConfiguration
    {
        public string EventHubConnectionString { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountQueueName { get; set; }
        public string StorageAccountKey { get; set; }
        public string IotServiceConnectionString { get; set; }
        public string[] SignalerDeviceIds { get; set; }
    }
}

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SmartPackagingNotifier;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();      

        services.AddSingleton<FunctionConfiguration>((s)=>
        {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
            var fc = new FunctionConfiguration();        
            fc.EventHubConnectionString = configuration["eventhub_connection_string"];            
            fc.IotServiceConnectionString = configuration["iot_service_connection_string"];
            fc.StorageAccountQueueName = configuration["storage_account_queue_name"];
            fc.SignalerDeviceIds = GetArray(configuration["signaler_device_ids"]);
            fc.StorageAccountKey = configuration["storage_account_key"];
            fc.StorageAccountName = configuration["storage_account_name"];


            string[] GetArray(string value)
            {
                var s = value;
                if (!string.IsNullOrEmpty(s))
                {
                    return s.Split(",");
                }
                return Array.Empty<string>();
            }

            return fc;
        });
    })

    .Build();

host.Run();

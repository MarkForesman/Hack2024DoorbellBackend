using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

class Cloud2Device
{
    private ServiceClient _serviceClient;
    private string[] _deviceIds;

    public Cloud2Device (string connectionString, string[] deviceIds)
    {
        _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        _deviceIds = deviceIds;
    }

    public async Task SendCloudToDeviceMessageAsync<T>(T eventMessage)
    {
        var messageAsString = JsonSerializer.Serialize<T>(eventMessage);
        var commandMessage = new Message(Encoding.ASCII.GetBytes(messageAsString));
        for (int i = 0; i < _deviceIds.Length; i++)
        {
            try
            {
                await _serviceClient.SendAsync(_deviceIds[i], commandMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); //log to APP Insights
            }
        }
    }
}

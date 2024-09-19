using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;


namespace SmartPackagingNotifier
{
    public class GetConfiguration
    {
        IConfigurationRoot _config;
        public GetConfiguration(ExecutionContext context)
        {

            _config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        }

        public string Get(string name)
        {
            return _config[name];
        }

        public string[] GetArray(string name)
        {

            var s = _config[name];
            if (!string.IsNullOrEmpty(s))
            {
                return s.Split(",");
            }
            return Array.Empty<string>();
        }
    }
}

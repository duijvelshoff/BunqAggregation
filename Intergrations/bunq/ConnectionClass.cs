using Bunq.Sdk.Context;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace bunqAggregation.Intergrations.bunq
{
    public class Connection
    {
        public static void Register(IHostingEnvironment env)
        {
            string apiKey = Core.Config.bunqApiKey;

            if (env.EnvironmentName == "Development")
            {
                var currentIpGet = new HttpClient().GetStringAsync("http://ipinfo.io/ip");
                string currentIp = Regex.Replace(currentIpGet.Result.ToString(), @"\t|\n|\r", "");
                List<string> DevelopmentIPs = new List<string>{
                    "123.123.123.123", //replace with own ip
                    currentIp
                };
                Console.WriteLine(apiKey);
                var apiContextSetup = ApiContext.Create(ApiEnvironmentType.SANDBOX, apiKey, "bunqAggregation", DevelopmentIPs);
                apiContextSetup.Save();
            }
            else
            {
                var apiContextSetup = ApiContext.Create(ApiEnvironmentType.PRODUCTION, apiKey, "bunqAggregation");
                apiContextSetup.Save();
            }
        }

        public static void Initialize()
        {
            var apiContext = ApiContext.Restore();

            BunqContext.LoadApiContext(apiContext);
        }
    }
}

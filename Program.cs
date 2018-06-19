using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using bunqAggregation.Services;

namespace bunqAggregation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(() => Trigger.Job());
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

            host.Run();
        }
    }
}
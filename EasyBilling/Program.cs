using System.Threading.Tasks;
using EasyBilling.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EasyBilling
{
    public class Program
    {
        public async Task Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args).Build();
            await DbInitializer.GetInstance(hostBuilder)
                .InitializeAsync();
            await hostBuilder.RunAsync();
        }

        private IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>());
    }
}
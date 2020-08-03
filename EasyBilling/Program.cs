using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EasyBilling
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //Инициализация БД пользователями
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    //Загружаем неоюбходимые сервисы для инициализации данных
                    var um = services.GetRequiredService<UserManager<IdentityAccount>>();
                    var rm = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var bc = services.GetRequiredService<BillingDbContext>();

                    var initializer = new DbInitializer(um, rm, bc);
                    initializer.Initialize();

                    logger.LogInformation("Initialization of the databse is success completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Initialization of the databse is not completed.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
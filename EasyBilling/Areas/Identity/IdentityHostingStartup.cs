using EasyBilling.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

[assembly: HostingStartup(typeof(EasyBilling.Areas.Identity.IdentityHostingStartup))]
namespace EasyBilling.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public async void Configure(IWebHostBuilder builder)
        {
            await Task.Run(() =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    var currConn = context.Configuration.GetSection("CurrentUsingConnection").Value;
                    services.AddDbContext<BillingDbContext>(options =>
                       options.UseSqlServer(context.Configuration
                       .GetConnectionString(currConn)));
                });
            });

            await Task.Run(() => builder.ConfigureServices((services) =>
                services.AddDefaultIdentity<IdentityUser>(
                    options => options.SignIn.RequireConfirmedAccount = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<BillingDbContext>()));
        }
    }
}
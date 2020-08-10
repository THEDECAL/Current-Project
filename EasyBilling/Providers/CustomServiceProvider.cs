using EasyBilling.Services;
using Microsoft.Extensions.DependencyInjection;
using EasyBilling.Data;

namespace EasyBilling.Providers
{
    public static class CustomServiceProvider
    {
        public static void AddAccessRightsManager(this IServiceCollection services) =>
            services.AddSingleton<AccessRightsManager>();

        public static void AddDbInitializer(this IServiceCollection services) => 
            services.AddSingleton<DbInitializer>();
    }
}

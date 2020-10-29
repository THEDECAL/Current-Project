using EasyBilling.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace EasyBilling
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Мои сервисы
            services.AddAccessRightsManager();
            services.AddDbInitializer();
            services.AddTariffRegulator();
            services.AddEmailSender();

            services.AddRazorPages();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddRouting();
            //services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();

                app.UseStatusCodePages();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseCookiePolicy();
                app.UseSession();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");

                    endpoints.MapControllerRoute(
                            name: "api",
                            pattern: "api/{controller}/{action}/{id?}");

                    endpoints.MapRazorPages();
                });
        }
    }
}
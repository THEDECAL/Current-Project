using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            //Сервисы необходимые для работы сессий
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddRouting();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region using of services
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection(); //Использовать перенаправление на защищённый протокол
            app.UseStatusCodePages(); //Отображать статусый код
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy(); //Отслеживать согласие на хранение куки
            app.UseSession(); //Использовать сессии

            //var routeBldr = new RouteBuilder(app);
            //routeBldr.MapRoute(name: "checking-access-rights",
            //    template: "{controller:required:alpha:length(0, 30)}/" +
            //                        "{component:alpha:length(0, 30)}/" +
            //                        "{action:alpha:length(0, 10)}/" +
            //                        "{id?}");

            //app.UseRouter(routeBldr.Build());
            //app.Run(async h => 
            //{
            //    await h.
            //});
            #endregion
            app.UseEndpoints(endpoints =>
            {
                //Маршрутизация проверки прав доступа
                endpoints.MapControllerRoute(
                    name: "checking-access-rights",
                    pattern:
                        "{controller:required:alpha:length(0, 30)}/" +
                        "{component:alpha:length(0, 30)}/" +
                        "{action:alpha:length(0, 10)}/" +
                        "{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:
                        "{controller:alpha:length(0, 30)=home}/" +
                        "{action:alpha:length(0, 30)=index}/" +
                        "{id?}");
            endpoints.MapRazorPages();
        });
        }
    }
}
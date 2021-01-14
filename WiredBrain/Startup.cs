using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WiredBrain.Helpers;
using WiredBrain.Hubs;

namespace WiredBrain
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(new Random());
            services.AddSingleton<OrderChecker>();
            services.AddHttpContextAccessor();

            // 01/14/2021 07:35 am - SSN - [20210114-0735] - [001] - M04-13 - Azure SignalR service
            // services.AddSignalR().AddMessagePackProtocol();
            // We can pass the connection string on; otherwise, it will be picked up from Azure:SignalR:ConnectionString

            // Failed: MessagePack not supported on Azure (Dev service version)
            // services.AddSignalR().AddAzureSignalR().AddMessagePackProtocol();

            services.AddSignalR().AddAzureSignalR();




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 01/12/2021 04:09 am - SSN - [20210112-0409] - [001] - M04 - Working with ASP.NET Core SignalR
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("WiredBrain.html");
            app.UseDefaultFiles(options);

            app.UseFileServer();

            // 01/14/2021 10:24 am - SSN - [20210114-0735] - [003] - M04-13 - Azure SignalR service
            // app.UseSignalR(routes => routes.MapHub<CoffeeHub>("/coffeehub"));
            app.UseAzureSignalR(routes => routes.MapHub<CoffeeHub>("/coffeehub"));

            app.UseMvc();
        }
    }
}

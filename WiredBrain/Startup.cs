﻿using System;
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
            services.AddSignalR().AddMessagePackProtocol();
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

            app.UseSignalR(routes => routes.MapHub<CoffeeHub>("/coffeehub"));
            app.UseMvc();
        }
    }
}

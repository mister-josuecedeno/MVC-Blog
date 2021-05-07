using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MVC_Blog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC_Blog
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Bringing in the Data Service
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var dataService = serviceProvider.GetRequiredService<DataService>();
            await dataService.ManageDataAsync();

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

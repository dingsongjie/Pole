using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backet.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Pole.Orleans.Provider.EntityframeworkCore;
using Microsoft.Extensions.Logging;

namespace Backet.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder.UseLocalhostClustering();
                    siloBuilder.AddEfGrainStorage<BacketDbContext>("ef");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
            .ConfigureLogging((hostingContext, logging) =>
            {
            // The ILoggingBuilder minimum level determines the
            // the lowest possible level for logging. The log4net
            // level then sets the level that we actually log at.
            logging.AddLog4Net();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }
}

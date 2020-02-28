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
using Orleans.Configuration;

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
                    siloBuilder.ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory());
                    siloBuilder.UseLocalhostClustering();
                    siloBuilder.AddEfGrainStorage<BacketDbContext>("ef");
                    siloBuilder.Configure<GrainCollectionOptions>(options =>
                    {
                        options.CollectionAge = TimeSpan.FromMinutes(2);
                    });
                    siloBuilder.UseDashboard(options => { });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(option =>
                    {
                        option.ListenAnyIP(81, config =>
                        {
                            config.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                        });
                        option.ListenAnyIP(82, config =>
                        {
                            config.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                        });
                    });
                });
    }
}

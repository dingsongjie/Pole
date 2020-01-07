using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.ReliableMessage.Storage.Mongodb;
using ServiceA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddComteckReliableMessage(option =>
            {
                option.AddMasstransitRabbitmq(rabbitoption =>
                {
                    rabbitoption.RabbitMqHostAddress = Configuration["RabbitmqConfig:HostAddress"];
                    rabbitoption.RabbitMqHostUserName = Configuration["RabbitmqConfig:HostUserName"];
                    rabbitoption.RabbitMqHostPassword = Configuration["RabbitmqConfig:HostPassword"];
                    rabbitoption.QueueNamePrefix = Configuration["ServiceName"];
                });
                option.AddMongodb(mongodbOption =>
                {
                    mongodbOption.ServiceCollectionName = Configuration["ServiceName"];
                    mongodbOption.Servers = Configuration.GetSection("MongoConfig:Servers").Get<MongoHost[]>();
                });
                option.AddEventAssemblies(typeof(Startup).Assembly)
                      .AddEventHandlerAssemblies(typeof(Startup).Assembly);
                option.NetworkInterfaceGatewayAddress = Configuration["ReliableMessageOption:NetworkInterfaceGatewayAddress"];
            });

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

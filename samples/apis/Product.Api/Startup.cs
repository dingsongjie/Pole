using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pole.ReliableMessage.Storage.Mongodb;
using Product.Api.Grpc;
using Product.Api.Infrastructure;

namespace Product.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProductDbContext>(options =>
                options.UseNpgsql(Configuration["postgres:main"]));

            services.AddGrpc(option =>
            {
                if (Environment.IsDevelopment())
                {
                    option.EnableDetailedErrors = true;
                }
            });

            services.AddGrpcValidation();
            services.AddGrpcRequestValidator(this.GetType().Assembly);

            services.AddPole(option =>
            {
                option.AddManageredAssemblies(this.GetType().Assembly);
                option.AutoInjectionDependency();
                option.AutoInjectionCommandHandlersAndDomainEventHandlers();
                option.AddPoleEntityFrameworkCoreDomain();
            });

            services.AddPoleReliableMessage(option =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ProductTypeService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}

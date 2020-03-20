using Backet.Api.Grains;
using Backet.Api.GrpcServices;
using Backet.Api.Infrastructure;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Pole.Grpc.Authentication;
using Pole.Orleans.Provider.EntityframeworkCore;
using Prometheus;
using System;
using System.Threading.Tasks;
using static SagasTest.Api.Test;

namespace Backet.Api
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
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.AddDbContextPool<BacketDbContext>(options => options.UseNpgsql(Configuration["postgres:write"]));
            services.AddControllers();

            services.AddGrpc(option =>
            {
                option.EnableMessageValidation();
            });
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message");
            }));
            var builder = services.AddGrpcClient<TestClient>((s, o) =>
             {
                 o.Address = new Uri("http://localhost:5002");
             })
            .EnableCallContextPropagation()
            .EnableJWTPropagation();

            services.AddPole(config =>
            {
                config.AddEventBus();
                config.AddEventBusRabbitMQTransport(option =>
                {
                    option.Hosts = new string[1] { Configuration["RabbitmqConfig:HostAddress"] };
                    option.Password = Configuration["RabbitmqConfig:HostPassword"];
                    option.UserName = Configuration["RabbitmqConfig:HostUserName"];
                });
                config.AddEventBusEFCoreStorage<BacketDbContext>();
                config.AddPoleGrpc();
            });

            services.ConfigureGrainStorageOptions<BacketDbContext, BacketGrain, Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>(
            options =>
            {
                options.UseQuery(context => context.Backets.Include(box => box.BacketItems),
                    context => context.Backets.AsNoTracking().Include(box => box.BacketItems)
                    );
                options.IsRelatedData = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePole();
            app.UseRouting();

            app.UseGrpcWeb();
            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGrpcService<BacketService>();
                endpoints.MapMetrics();
            });
        }
    }
}

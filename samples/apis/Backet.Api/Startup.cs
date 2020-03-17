using Backet.Api.Grains;
using Backet.Api.GrpcServices;
using Backet.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pole.Orleans.Provider.EntityframeworkCore;
using Prometheus;

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
            services.AddDbContextPool<BacketDbContext>(options => options.UseNpgsql(Configuration["postgres:write"]));
            services.AddControllers();

            services.AddGrpc();
            services.AddGrpcValidation();
            services.AddGrpcRequestValidator();
            services.AddGrpcWeb(o => o.GrpcWebEnabled = true);
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message");
            }));
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

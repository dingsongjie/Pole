using Backet.Api.Grains;
using Backet.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pole.Orleans.Provider.EntityframeworkCore;

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

            services.AddPole(config => {
                config.AddRabbitMQ(option =>
                {
                    option.Hosts = new string[1] { Configuration["RabbitmqConfig:HostAddress"] };
                    option.Password = Configuration["RabbitmqConfig:HostPassword"];
                    option.UserName = Configuration["RabbitmqConfig:HostUserName"];
                });
                config.AddEntityFrameworkEventStorage<BacketDbContext>();
            });

            services.ConfigureGrainStorageOptions<BacketDbContext, BacketGrain, Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>(
            options =>
            {
                options.UseQuery(context => context.Backets.AsNoTracking()
                        .Include(box => box.BacketItems));
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}

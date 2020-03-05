using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backet.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SagasTest.Api
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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<BacketDbContext>(options => options.UseNpgsql(Configuration["postgres:write"]));
            services.AddControllers();
            services.AddPole(config =>
            {
                config.AddRabbitMQ(option =>
                {
                    option.Hosts = new string[1] { Configuration["RabbitmqConfig:HostAddress"] };
                    option.Password = Configuration["RabbitmqConfig:HostPassword"];
                    option.UserName = Configuration["RabbitmqConfig:HostUserName"];
                });
                config.AddEntityFrameworkEventStorage<BacketDbContext>();
                config.AddSagas(option=> {
                    option.ServiceName = "SagasTest";
                });
            });
            services.AddHttpClient();
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
            });
        }
    }
}

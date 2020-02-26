using Backet.Api.Infrastructure;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Pole.Samples.Backet.Api.Benchmarks
{
    public class GrainWithEntityframeworkCoreAndPgTest
    {
        IServiceProvider serviceProvider;
        public GrainWithEntityframeworkCoreAndPgTest()
        {
            var services = new ServiceCollection();
            services.AddDbContextPool<BacketDbContext>(options => options.UseNpgsql("Server=192.168.0.248;Port=5432;Username=postgres;Password=comteck2020!@#;Database=Pole-Backet;Enlist=True;Timeout=0;Command Timeout=600;MinPoolSize=20;MaxPoolSize=500;"));
            serviceProvider = services.BuildServiceProvider();
        }
        [Benchmark]
        public async Task SingleOrDefaultAsync()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BacketDbContext>();

                var entity = await context.Backets.Include(box => box.BacketItems).SingleOrDefaultAsync(m => m.Id == "222");
            }
        }
        [Benchmark]
        public async Task DapperOpenConnection()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=192.168.0.251;Port=5432;Username=postgres;Password=comteck2020!@#;Database=smartretail-tenant;Enlist=True;"))
            {
                await conn.OpenAsync();
                //var teams = await conn.QueryAsync<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>("SELECT * FROM   \"public\".\"Backet\" where  \"Id\" =@Id", new { Id = newId });
                var teams = await conn.ExecuteAsync("SELECT 1");
            }
        }
    }
}

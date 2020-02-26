using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Backet.Api.Domain.Event;
using Backet.Api.EventHandlers.Abstraction;
using Backet.Api.Grains.Abstraction;
using Backet.Api.Infrastructure;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Orleans;
using Pole.Core.EventBus;
using Pole.Core.EventBus.Event;
using Pole.Core.EventBus.EventHandler;
using Pole.Core.EventBus.EventStorage;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;

namespace Backet.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class BacketController : ControllerBase
    {
        private readonly IClusterClient clusterClient;
        private readonly ILogger logger;
        private readonly IProducerInfoContainer producerContainer;
        private readonly IEventTypeFinder eventTypeFinder;
        private readonly ISerializer serializer;
        private readonly ISnowflakeIdGenerator snowflakeIdGenerator;
        private readonly IEventBuffer eventBuffer;
        public BacketController(IClusterClient clusterClient, ILogger<BacketController> logger, IProducerInfoContainer producerContainer,
            IEventTypeFinder eventTypeFinder, ISerializer serializer, ISnowflakeIdGenerator snowflakeIdGenerator, IEventBuffer eventBuffer)
        {
            this.clusterClient = clusterClient;
            this.logger = logger;
            this.producerContainer = producerContainer;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.eventBuffer = eventBuffer;
        }
        [HttpPost("api/backet/AddBacket")]
        public Task<bool> AddBacket([FromBody]Backet.Api.Grains.Abstraction.BacketDto backet)
        {
            var newId = Guid.NewGuid().ToString("N").ToLower();
            backet.Id = newId;
            //var entity = await backetDbContext.Backets.AsNoTracking().Include(box => box.BacketItems).SingleOrDefaultAsync(m => m.Id == "222");

            ////using (NpgsqlConnection conn = new NpgsqlConnection("Server=192.168.0.248;Port=5432;Username=postgres;Password=comteck2020!@#;Database=Pole-Backet;Enlist=True;Timeout=0;Command Timeout=600"))
            ////{
            ////    await conn.OpenAsync();
            ////    var teams = await conn.QueryAsync<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>("SELECT * FROM   \"public\".\"Backet\" where  \"Id\" =@Id", new { Id = newId });
            ////    //var teams = await conn.ExecuteAsync("SELECT 1");
            ////}
            var grain = clusterClient.GetGrain<IBacketGrain>(newId);
            return grain.AddBacket(backet);
            //return true;
        }
        [HttpPost("api/backet/UpdateBacket")]
        public Task<bool> UpdateBacket()
        {
            var id = "da8a489fa7b4409294ee1b358fbbfba5";
            var grain = clusterClient.GetGrain<IBacketGrain>(id);
            return grain.UpdateBacket("88");
        }
        [HttpPost("api/backet/AddItem")]
        public Task<bool> AddItem()
        {
            var id = "da8a489fa7b4409294ee1b358fbbfba5";
            var grain = clusterClient.GetGrain<IBacketGrain>(id);
            return grain.AddBacketItem("55", "测试3", 1000);
        }
        [HttpPost("api/backet/RemoveFirstItem")]
        public Task<bool> RemoveFirstItem()
        {
            var id = "da8a489fa7b4409294ee1b358fbbfba5";
            var grain = clusterClient.GetGrain<IBacketGrain>(id);
            return grain.RemoveFirstItem();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Orleans;
using Pole.Core.Serialization;
using Pole.Core.UnitOfWork;
using Pole.Core.Utils.Abstraction;
using Pole.EventBus;

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
        private readonly IServiceProvider serviceProvider;
        public BacketController(IClusterClient clusterClient, ILogger<BacketController> logger, IProducerInfoContainer producerContainer,
            IEventTypeFinder eventTypeFinder, ISerializer serializer, ISnowflakeIdGenerator snowflakeIdGenerator, IEventBuffer eventBuffer, IServiceProvider serviceProvider)
        {
            this.clusterClient = clusterClient;
            this.logger = logger;
            this.producerContainer = producerContainer;
            this.eventTypeFinder = eventTypeFinder;
            this.serializer = serializer;
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.eventBuffer = eventBuffer;
            this.serviceProvider = serviceProvider;
        }
        [HttpPost("api/backet/AddBacket")]
        public  Task<bool> AddBacket([FromBody]Backet.Api.Grains.Abstraction.BacketDto backetDto)
        {
            var newId = Guid.NewGuid().ToString("N").ToLower();
            backetDto.Id = newId;
            var grain = clusterClient.GetGrain<IAddBacketGrain>(newId);
            //var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            //var dbContext = serviceProvider.GetRequiredService<BacketDbContext>();
            //var bus = serviceProvider.GetRequiredService<IBus>();
            //using (var transaction = await dbContext.Database.BeginTransactionAsync())
            //{
            //    unitOfWork.Enlist(transaction, bus);
            //    Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet backet = new Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet
            //    {
            //        Id = backetDto.Id,
            //        UserId = backetDto.UserId
            //    };
            //    if (backetDto.BacketItems == null || backetDto.BacketItems.Count == 0) return false;
            //    backetDto.BacketItems.ForEach(item =>
            //    {
            //        backet.AddBacketItem(item.ProductId, item.ProductName, item.Price);
            //    });
            //    dbContext.Backets.Add(backet);
            //    await bus.Publish(new BacketCreatedEvent() { BacketId = backet.Id });
            //    await unitOfWork.CompeleteAsync();
            //}
            //return true;

            return grain.AddBacket(backetDto);
        }
        [HttpPost("api/backet/UpdateBacket")]
        public Task<bool> UpdateBacket()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.Default;
            System.GC.Collect();
            var id = "67bbf594246441a18d7b6c74a277d06a";
            var grain = clusterClient.GetGrain<IBacketGrain>(id);
            return grain.UpdateBacket("99");
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
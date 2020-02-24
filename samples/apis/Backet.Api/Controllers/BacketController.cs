using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backet.Api.EventHandlers.Abstraction;
using Backet.Api.Grains.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Pole.Core.EventBus;
using Pole.Core.EventBus.EventHandler;

namespace Backet.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class BacketController : ControllerBase
    {
        private readonly IClusterClient clusterClient;
        public BacketController(IClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }
        [HttpPost("api/backet/AddBacket")]
        public Task<bool> AddBacket([FromBody]Backet.Api.Grains.Abstraction.BacketDto backet)
        {
            var newId = Guid.NewGuid().ToString("N").ToLower();
            backet.Id = newId;
            var grain = clusterClient.GetGrain<IBacketGrain>(newId);
            return grain.AddBacket(backet);
            //var clientType = typeof(IClusterClient);
            //var clientParams = Expression.Parameter(clientType, "client");
            //var primaryKeyParams = Expression.Parameter(typeof(string), "primaryKey");
            //var grainClassNamePrefixParams = Expression.Parameter(typeof(string), "grainClassNamePrefix");
            //var method = typeof(ClusterClientExtensions).GetMethod("GetGrain", new Type[] { clientType, typeof(string), typeof(string) });
            //var body = Expression.Call(method.MakeGenericMethod(typeof(IToNoticeBacketCreatedEventHandler)), clientParams, primaryKeyParams, grainClassNamePrefixParams);
            //var func = Expression.Lambda<Func<IClusterClient, string, string, IPoleEventHandler>>(body, clientParams, primaryKeyParams, grainClassNamePrefixParams).Compile();
            //var handler = func(clusterClient, newId, null);
            //await handler.Invoke(null);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backet.Api.Grains.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;

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
            var newId = "da8a489fa7b4409294ee1b358fbbfba5";
            backet.Id = newId;
            var grain = clusterClient.GetGrain<IBacketGrain>(newId);
            return grain.AddBacket(backet);
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
           return grain.AddBacketItem("55","测试3",1000);
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
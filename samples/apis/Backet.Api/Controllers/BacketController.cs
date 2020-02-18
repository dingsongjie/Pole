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
        public void AddBacket([FromBody]Backet.Api.Grains.Abstraction.BacketDto backet)
        {
            var newId = "da8a489fa7b4409294ee1b358fbbfba5";
            backet.Id = newId;
            var grain = clusterClient.GetGrain<IBacketGrain>(newId);
            grain.AddBacket(backet);
        }
    }
}
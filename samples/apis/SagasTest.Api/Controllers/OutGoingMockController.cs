using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagasTest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutGoingMockController : ControllerBase
    {
        [HttpGet("Transaction1Ok")]
        public Task<string> Transaction1Ok()
        {
            return Task.FromResult("Transaction1 Ok");
        }
        [HttpGet("Transaction1RollBack")]
        public Task<string> Transaction1RollBack()
        {
            return Task.FromResult("Transaction1 RollBack");
        }
        [HttpGet("Transaction2Ok")]
        public Task<string> Transaction2Ok()
        {
            return Task.FromResult("Transaction1 Ok");
        }
        [HttpGet("Transaction2RollBack")]
        public Task<string> Transaction2RollBack()
        {
            return Task.FromResult("Transaction2 RollBack");
        }
        [HttpGet("Transaction3Ok")]
        public Task<string> Transaction3Ok()
        {
            return Task.FromResult("Transaction1 Ok");
        }
    }
}

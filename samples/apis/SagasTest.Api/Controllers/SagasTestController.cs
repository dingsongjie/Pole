using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pole.Sagas.Core.Abstraction;
using SagasTest.Api.Activities;

namespace SagasTest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SagasTestController : ControllerBase
    {
        private readonly ISagaFactory sagaFactory;
        public SagasTestController(ISagaFactory sagaFactory)
        {
            this.sagaFactory = sagaFactory;
        }
        // GET api/values
        [HttpGet("NormalCall")]
        public async Task NormalCall()
        {
            var sagas = sagaFactory.CreateSaga();

            sagas.AddActivity("Transaction1Ok", new Transaction1Dto { Id = 1, Name = "22" });
            sagas.AddActivity("Transaction2Ok", new Transaction2Dto {  Price = 1, Message = "我们" });
            sagas.AddActivity("Transaction3HasResult", new Transaction3Dto {  Age = 1,  Name = "333" });

            var result = await sagas.GetResult();
        }
        [HttpGet("Transaction3ReturnFalse")]
        public async Task Transaction3ReturnFalse()
        {
            var sagas = sagaFactory.CreateSaga();

            sagas.AddActivity("Transaction1Ok", new Transaction1Dto { Id = 1, Name = "22" });
            sagas.AddActivity("Transaction2Ok", new Transaction2Dto { Price = 1, Message = "我们" });
            sagas.AddActivity("Transaction3ReturnFalse", new Transaction3Dto { Age = 1, Name = "333" });

            var result = await sagas.GetResult();
        }
        [HttpGet("Transaction2ReturnFalse")]
        public async Task Transaction2ReturnFalse()
        {
            var sagas = sagaFactory.CreateSaga();

            sagas.AddActivity("Transaction1Ok", new Transaction1Dto { Id = 1, Name = "22" });
            sagas.AddActivity("Transaction2ReturnFalse", new Transaction2Dto { Price = 1, Message = "我们" });
            sagas.AddActivity("Transaction3HasResult", new Transaction3Dto { Age = 1, Name = "333" });

            var result = await sagas.GetResult();
        }
        [HttpGet("Transaction1ReturnFalse")]
        public async Task Transaction1ReturnFalse()
        {
            var sagas = sagaFactory.CreateSaga();

            sagas.AddActivity("Transaction1ReturnFalse", new Transaction1Dto { Id = 1, Name = "22" });
            sagas.AddActivity("Transaction2Ok", new Transaction2Dto { Price = 1, Message = "我们" });
            sagas.AddActivity("Transaction3HasResult", new Transaction3Dto { Age = 1, Name = "333" });

            var result = await sagas.GetResult();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

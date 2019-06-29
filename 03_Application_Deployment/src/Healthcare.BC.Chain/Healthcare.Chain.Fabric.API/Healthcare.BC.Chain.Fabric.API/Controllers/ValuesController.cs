using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Healthcare.BC.Chain.Fabric.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpPost()]
        [Route("CreateProfile")]
        public ActionResult<ContractTransaction> CreateProfile([FromBody] Healthcare.BC.Offchain.Repository.Models.Profile CitizenProfile)
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost()]
        [Route("UpdateProfileProofDocument")]
        public ActionResult<ContractTransaction> Get(int id)
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

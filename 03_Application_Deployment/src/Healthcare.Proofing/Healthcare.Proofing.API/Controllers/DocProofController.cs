using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Healthcare.Proofing.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Healthcare.BC.Offchain.Repository.Models;

namespace Healthcare.Proofing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocProofController : ControllerBase
    {
        private IConfiguration _configuration;
        private ProofStorage _storage;
        public DocProofController(IConfiguration configuration)
        {
            _configuration = configuration;
            _storage = new ProofStorage(_configuration);
        }

        [HttpPost]
        [Route("PutProofDocs")]
        public ActionResult<DocProof> PutProofDoc(Microsoft.AspNetCore.Http.IFormFile proofDoc, string bindingId, string citizenIdentifier)
        {
            var retrunObject = _storage.PutProof(bindingId, citizenIdentifier, proofDoc);
        
            return retrunObject.Result;
        }

        [HttpPost]
        [Route("GetProofDocwithIdentifier")]
        public ActionResult<string> GetProofDoc(string bindingId, string citizenIdentifier)
        {
            var returnObject = _storage.GetProof(bindingId, citizenIdentifier);

            return returnObject.Result;
        }

        [HttpPost]
        [Route("ValidateFileWithHash")]
        public ActionResult<bool> ValidateFileWithHash(string bindingId, string citizenIdentifier, string Hash)
        {
            var returnObject = _storage.ValidateHash(bindingId, citizenIdentifier,Hash);

            return returnObject.Result;
        }
    }
}

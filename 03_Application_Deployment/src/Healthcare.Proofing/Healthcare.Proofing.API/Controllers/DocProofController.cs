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


        //[HttpPost("PutProofDocs")]
        //public async Task<string> PutProofDocs([FromBody] List<IFormFile> files, Guid BindingId)
        //{
        //    //long size = files.Sum(f => f.Length);

        //    //var filePath = Path.GetTempFileName();

        //    //foreach (var fromFile in files)
        //    //{
        //    //    if (fromFile.Length > 0)
        //    //    {
        //    //        using (var stream = new FileStream(filePath, FileMode.Create))
        //    //        {
        //    //            //Should be hasing with BindingId and FileHash
        //    //            await fromFile.CopyToAsync(stream, CancellationToken.None);
        //    //        }
        //    //    }
        //    //}

        //    // return Ok(new { count = files.Count, size, filePath });
        //    return await Task.Factory.StartNew(() => "ok");
        //}
    }
}

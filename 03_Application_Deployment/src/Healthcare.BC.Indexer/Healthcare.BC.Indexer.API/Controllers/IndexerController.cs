using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Healthcare.BC.Indexer.Service;
using Healthcare.BC.Offchain.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Healthcare.IndexerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexerController : ControllerBase
    {
        private IConfiguration _configuration;
        private string _mongoConnectionString;
        public IndexerController(IConfiguration configuration)
        {
            _configuration = configuration;
            _mongoConnectionString = _configuration["offchain_connectionstring"];
        }


        [HttpPost("contractTransaction")]
        public void LogTransaction([FromBody] ContractTransaction contractTransaction, string transactionDescription = "")
        {
            TransactionIndexingServiceAgent _agent = new TransactionIndexingServiceAgent(_mongoConnectionString);

            _agent.LogTransaction(contractTransaction, transactionDescription).GetAwaiter().GetResult();
        }


    }
}

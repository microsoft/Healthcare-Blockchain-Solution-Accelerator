using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Healthcare.BC.Offchain.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Healthcare.Common.ServiceHost;
using Healthcare.BC.Chain.Fabric.Client;

namespace Healthcare.Chain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChainController : ControllerBase
    {
        private readonly IFabricClient _client;
        private readonly string bindingID = "e3f5f543-9fe5-4e21-a6cd-f1f0b8ae3e3a";

        public ChainController(IFabricClient client)
        {
            _client = client;
        }
        [HttpPost()]
        [Route("CreateProfile")]
        public ActionResult<ContractTransaction> CreateProfile([FromBody] Healthcare.BC.Offchain.Repository.Models.Profile CitizenProfile)
        {
            var transaction = _client.CreateProfile(CitizenProfile);
            return transaction;
        }

        [HttpPost()]
        [Route("UpdateProfileProofDocument")]
        public ActionResult<ContractTransaction> UpdateProfile(string citizenIdentifier, Healthcare.BC.Offchain.Repository.Models.DocProof docProof)
        {
            var transaction = _client.UpdateProfile(citizenIdentifier, docProof);
            return transaction;
        }

        [HttpPost()]
        [Route("GetProfileInformation")]
        public ActionResult<ContractTransaction> GetProfileInformation(string citizenIdentifier)
        {
            var profile = _client.GetProfileInformation(citizenIdentifier);
            var transaction = prepareTxObject(bindingID, profile);
            
            return transaction;
        }

        [HttpPost()]
        [Route("AssignHealthcarePlan")]
        public ActionResult<ContractTransaction> AssignHealthcarePlan(HealthcarePlan healthcarePlan, string citizenIdentifier)
        {
            var transaction = _client.AssignHealthcarePlan(citizenIdentifier, healthcarePlan.Plan);
            return transaction;
        }

        [HttpPost()]
        [Route("ApproveHealthcarePlan")]
        public ActionResult<ContractTransaction> ApproveHealthcarePlan(string citizenIdentifier)
        {
            var transaction = _client.ApproveHealthcarePlan(citizenIdentifier, "State Approver");
            return transaction;
        }

        [HttpPost()]
        [Route("ChangeActiveState")]
        public ActionResult<ContractTransaction> ChangeActiveState(string citizenIdentifier, string activeState)
        {
            var transaction = _client.ChangeActiveState(citizenIdentifier, activeState);
            return transaction;
        }


        private ContractTransaction prepareTxObject(string bindingid, BC.Offchain.Repository.Models.Profile CitizenProfile)
        {
            var _dto = CitizenProfile;

            _dto.TransactedTime = DateTime.Now;
            _dto.TransactionID = Guid.NewGuid().ToString();
        
            TransactionConfirmation _transactionConfirmation = new TransactionConfirmation()
            {

                BlockHash = "0xfa4e2a31506c1f930efc7701ff6ddc1451d08a38a7a9267fe263766b4c7ea2d0",
                BlockNumber = "1",
                TransactionHash = "",
                Name = "Fake Contract",
                ProxyId = "Fake Contract_Proxy ID",
                TransactionIndex = "1"

            };
            ContractTransaction _txInformation = new ContractTransaction(bindingid, " Enterprise Smart Contract transacted.....", _dto, _transactionConfirmation);
            return _txInformation;
        }


        private ContractTransaction prepareDeployObject(BC.Offchain.Repository.Models.Profile citizenProfile)
        {   
             ConstructorConfirmation _deployConfirmation = new ConstructorConfirmation()
            {
                NewContractOrTokenId = Guid.NewGuid().ToString(),
                Name = "Smart Contract instance by ESC Template",
                TransactionConfirmation = new TransactionConfirmation()
                {
                    BlockHash = "0xfa4e2a31506c1f930efc7701ff6ddc1451d08a38a7a9267fe263766b4c7ea2d0",
                    BlockNumber = "1",
                    LedgerAddress = "",
                    TransactionHash = "",
                    Name = "Fake Contract",
                    ProxyId = "Fake Contract_Proxy ID",
                    TransactionIndex = "1"
                }
            };
            ContractTransaction _txInformation = new ContractTransaction(citizenProfile, _deployConfirmation);
            return _txInformation;
        }
    }
}

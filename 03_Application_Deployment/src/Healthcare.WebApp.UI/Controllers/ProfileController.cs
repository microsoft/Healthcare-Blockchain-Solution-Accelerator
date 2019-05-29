using Healthcare.Common.ServiceHost;
using HealthcareBC.Fabric;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Healthcare.WebApp.UI.Controllers
{
    public class ProfileController : Controller
    {
        private static FabricProxy _FabricProxy;

        public ProfileController()
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            _FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient());

        }

        [HttpPost()]
        [Route("CreateProfile")]
        public async Task<HealthcareBC.Fabric.ContractTransaction> DeployProfile([FromBody] HealthcareBC.Fabric.Profile citizenProfile)
        {
            return await _FabricProxy.CreateProfileAsync(citizenProfile);
        }


        [HttpPost()]
        [Route("UpdateProfile")]
        public async Task<HealthcareBC.Fabric.ContractTransaction> UpdateProfile(string citizenIdentifier, HealthcareBC.Fabric.DocProof docProof)
        {
            return await _FabricProxy.UpdateProfileAsync(citizenIdentifier, docProof);
        }

        [HttpPost()]
        [Route("GetCitizenProfile")]
        public async Task<HealthcareBC.Fabric.ContractTransaction> GetCitizenProfile(string citizenIdentifier)
        {
            return await _FabricProxy.GetProfileInformationAsync(citizenIdentifier);
        }


        //AssignCarePlan
        [HttpPost()]
        [Route("AssignHealthCarePlan")]
        public async Task<HealthcareBC.Fabric.ContractTransaction> AssignHealthCarePlan(string citizenIdentifier, HealthcareBC.Fabric.HealthcarePlan healthcareplan)
        {
            return await _FabricProxy.AssignHealthcarePlanAsync(healthcareplan, citizenIdentifier);
        }

        [HttpPost()]
        [Route("ResetActiveState")]
        public async Task<HealthcareBC.Fabric.ContractTransaction> ResetActiveState(string citizenIdentifier, string state)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.ResetActiveState(citizenIdentifier, state);
        }



        //ApproveHealthcarePlan
        [HttpPost()]
        [Route("ApproveHealthcarePlan")]
        //AssignCarePlan
        public async Task<HealthcareBC.Fabric.ContractTransaction> ApproveHealthcarePlan(string citizenIdentifier)
        {
            return await _FabricProxy.ApproveHealthcarePlanAsync(citizenIdentifier);
        }


        [HttpPost]
        [Route("UpdateProofDocument")]
        public async Task<HealthcareBC.Fabric.DocProof> UpdateProofDocument([FromForm]IFormFile proofDoc, string citizenIdentifier)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.UpdateProofDocument(docProof, citizenIdentifier);
        }

        [HttpPost()]
        [Route("GetProofDocument")]
        public async Task<string> GetProofDocumentUrl(string citizenIdentifier)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetProofDocumentUrl(citizenIdentifier);
        }

        [HttpPost()]
        [Route("ValidateProofDocumentHash")]
        public async Task<bool> ValidateProofDocumentHash(string citizenIdentifier, string hash)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.ValidateProofDocumentHash(citizenIdentifier, hash);
        }

        [HttpPost()]
        [Route("GetTransactionsByCitizenIdentifier")]
        public async Task<List<HealthcareBC.Fabric.ContractTransaction>> GetTransactionsByCitizenIdentifier(string citizenIdentifier)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetTransactionsByCitizenIdentifier(citizenIdentifier);
        }



        [HttpPost()]
        [Route("GetTransactionByEntityId")]
        public async Task<HealthcareBC.Fabric.ContractTransaction> GetTransactionByEntityId(string entityId)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetTransactionByEntityId(entityId);
        }


        [HttpPost()]
        [Route("GetCompletedCitizenProfiles")]
        public async Task<List<HealthcareBC.Fabric.ContractTransaction>> GetCompletedCitizenProfiles(string state)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetCompletedCitizenProfiles(state);
        }

        [HttpPost()]
        [Route("GetIncompletedCitizenProfiles")]
        public async Task<List<HealthcareBC.Fabric.ContractTransaction>> GetIncompletedCitizenProfiles(string state)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetIncompletedCitizenProfiles(state);
        }

        [HttpPost()]
        [Route("GetCitizenProfiles")]
        public async Task<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfiles(string state)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetCitizenProfiles(state);
        }

        [HttpPost()]
        [Route("GetCitizenProfilesNotApprovedHealthcareplan")]
        public async Task<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfilesNotApprovedHealthcareplan(string state)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetCitizenProfilesNotApprovedHealthcareplan(state);
        }

        [HttpPost()]
        [Route("GetCitizenProfilesApprovedHealthcareplan")]
        public async Task<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfilesApprovedHealthcareplan(string state)
        {
            throw new NotImplementedException();
            //return await _FabricProxy.GetCitizenProfilesApprovedHealthcareplan(state);
        }
    }
}

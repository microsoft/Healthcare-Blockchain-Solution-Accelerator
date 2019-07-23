using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Healthcare.BC.Service;
using HealthcareBC.Fabric;
using HealthcareBC.Indexer;
using HealthcareBC.ProofDocSvc;
using HealthcareBC.Tracker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceFabric.Services.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Healthcare.Common.ServiceHost;
using Microsoft.AspNetCore.Cors;

namespace Healthcare.BC.Application.API.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileServiceController : ControllerBase
    {
        private IConfiguration _configuration;
        private IApp _appService;

        public ProfileServiceController(IConfiguration configuration, IApp app)
        {
            _configuration = configuration;
            _appService = app;
        }


        [HttpPost()]
        [Route("CreateProfile")]
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> DeployProfile([FromBody] HealthcareBC.Fabric.Profile citizenProfile)
        {
            // TODO (Once functional) declare these in UI or in service
            citizenProfile.Description = @"Description for citizen profile " + citizenProfile.CitizenIdentifier;
            citizenProfile.StateApprover = " ";
            var transaction = _appService.CreateCitizenProfile(citizenProfile).Result;
            return transaction;
        }

        [HttpPost()]
        [Route("UpdateProfile")]
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> UpdateProfile(string citizenIdentifier, HealthcareBC.Fabric.DocProof docProof)
        {
            var transaction = _appService.UpdateProfileWithProofDocument(docProof, citizenIdentifier).Result;
            return transaction;
        }

        [HttpPost()]
        [Route("GetCitizenProfile")]
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> GetCitizenProfile(string citizenIdentifier)
        {
            var transaction = _appService.GetCitizenProfile(citizenIdentifier).Result;
            return transaction;
        }


        //AssignCarePlan
        [HttpPost()]
        [Route("AssignHealthCarePlan")]
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> AssignHealthCarePlan(string citizenidentifier, HealthcareBC.Fabric.HealthcarePlan healthcareplan)
        {
            var transaction = _appService.AssignHealthcarePlan(healthcareplan, citizenidentifier).Result;
            return transaction;
        }

        [HttpPost()]
        [Route("ResetActiveState")]
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> ResetActiveState(string citizenIdentifier, string state)
        {
            var transaction = _appService.ChangeActiveState(citizenIdentifier, state).Result;
            return transaction;
        }

 
        //ApproveHealthcarePlan
        [HttpPost()]
        [Route("ApproveHealthcarePlan")]
        //AssignCarePlan
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> ApproveHealthcarePlan(string citizenIdentifier)
        {
            var transaction = _appService.ApproveHealthcarePlan(citizenIdentifier).Result;
            return transaction;
        }


        [HttpPost]
        [Route("UpdateProofDocument")]
        public ActionResult<HealthcareBC.Fabric.DocProof> UpdateProofDocument([FromForm]IFormFile proofDoc, string citizenIdentifier)
        {
            if (proofDoc.Length == 0)
            {
                return null;
            }

            MemoryStream fileStream = new MemoryStream();
            proofDoc.CopyTo(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);
            var _proofDoc = _appService.PutProofDoc(proofDoc.FileName, fileStream, citizenIdentifier).Result;

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.Fabric.DocProof, HealthcareBC.ProofDocSvc.DocProof>());
            HealthcareBC.Fabric.DocProof retProof = Mapper.Map<HealthcareBC.Fabric.DocProof>(_proofDoc);

            return retProof;
        }

        [HttpPost()]
        [Route("GetProofDocument")]
        public ActionResult<string> GetProofDocumentUrl(string citizenIdentifier)
        {
            return _appService.GetProofDocumentUrl(citizenIdentifier).Result;
        }

        [HttpPost()]
        [Route("ValidateProofDocumentHash")]
        public ActionResult<bool> ValidateProofDocumentHash(string citizenIdentifier, string hash)
        {
            return _appService.ValidateHash(citizenIdentifier, hash).Result;
        }

        [HttpPost()]
        [Route("GetTransactionsByCitizenIdentifier")]
        public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetTransactionsByCitizenIdentifier(string citizenIdentifier)
        {
            //Tracker Service
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, ServiceEventSource.Current);
            TrackerProxy trackerProxy = new TrackerProxy(trackerEndpoint, new HttpClient());

            var result = trackerProxy.GetTransactionInformationByCitizenProfilerAsync(citizenIdentifier).GetAwaiter().GetResult().ToList();

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

            List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

            ServiceEventSource.Current.Message($"transaction informations are {result}");
            return _transactions;
        }



        [HttpPost()]
        [Route("GetTransactionByEntityId")]
        public ActionResult<HealthcareBC.Fabric.ContractTransaction> GetTransactionByEntityId(string entityId)
        {
            var result = _appService.GetCitizenProfile(entityId).GetAwaiter().GetResult();

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.Fabric.ContractTransaction, HealthcareBC.Tracker.ContractTransaction>());
            HealthcareBC.Fabric.ContractTransaction _transactions = Mapper.Map<HealthcareBC.Fabric.ContractTransaction>(result);

            return _transactions;
        }


        [HttpPost()]
        [Route("GetCompletedCitizenProfiles")]
        public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetCompletedCitizenProfiles(string State)
        {
            var result = _appService.GetCompletedCitizenProfiles(State).Result;

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

            List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

            return _transactions;
        }

        [HttpPost()]
        [Route("GetIncompletedCitizenProfiles")]
        public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetIncompletedCitizenProfiles(string State)
        {
            var result = _appService.GetInCompletedCitizenProfiles(State).Result;

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Tracker.ContractTransaction>, List<HealthcareBC.Fabric.ContractTransaction>>());

            List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

            return _transactions;
        }

        [HttpPost()]
        [Route("GetCitizenProfiles")]
        public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfiles(string State)
        {
            var result = _appService.GetCitizenProfiles(State).Result;

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

            List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

            return _transactions;
        }

        [HttpPost()]
        [Route("GetCitizenProfilesNotApprovedHealthcareplan")]
        public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfilesNotApprovedHealthcareplan(string State)
        {
            var result = _appService.GetCitizenProfilesNotApprovedHealthcareplan(State).Result;

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

            List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

            return _transactions;
        }

        [HttpPost()]
        [Route("GetCitizenProfilesApprovedHealthcareplan")]
        public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfilesApprovedHealthcareplan(string State)
        {
            var result = _appService.GetCitizenProfilesApprovedHealthcareplan(State).Result;

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

            List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

            ServiceEventSource.Current.Message($"transaction informations are {result}");

            return _transactions;
        }
    }
}

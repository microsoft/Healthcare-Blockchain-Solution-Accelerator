﻿using System;
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


namespace Healthcare.BC.Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileServiceController : ControllerBase
    {
        private IConfiguration _configuration;
        private App _appService;

        public ProfileServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
            _appService = new App();
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

        //[NonAction()]
        //private async Task LogTransactionAsync(HealthcareBC.Fabric.ContractTransaction contractTransaction, string transactionDFabricription = "")
        //{
        //    //Indexing Service
        //    //var indexingEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.indexerEndPoint, ServiceEventSource.Current);
        //    //IndexerProxy indexerProxy = new IndexerProxy(indexingEndpoint, new HttpClient());

        //    Mapper.Reset();
        //    Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.Fabric.ContractTransaction, HealthcareBC.Indexer.ContractTransaction>());
        //    HealthcareBC.Indexer.ContractTransaction contractDto = Mapper.Map<HealthcareBC.Indexer.ContractTransaction>(contractTransaction);

        //    await _appService.LogTransaction(contractDto, transactionDFabricription);
        //}

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

        /// <summary>
        /// it should be move to chaincode logic
        /// </summary>
        /// <param name="entityid"></param>
        /// <param name="HealthcarePlanEligibilityStatus"></param>
        /// <returns></returns>
        //[HttpPost()]
        //[Route("AssignCarePlanEligibilityStatus")]
        //public ActionResult<HealthcareBC.Fabric.ContractTransaction> AssignCarePlanEligibilityStatus(string entityid,
        //    HealthcareBC.Fabric.HealthcarePlanCarePlanEligibilityStatus HealthcarePlanEligibilityStatus)
        //{
        //    //Getting Contract Information
        //    var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, ServiceEventSource.Current);
        //    TrackerProxy trackerProxy = new TrackerProxy(trackerEndpoint, new HttpClient());

        //    var transaction = trackerProxy.GetTransactionInformationByTxEntityIdAsync(entityid).GetAwaiter().GetResult();

        //    HealthcareBC.Tracker.HealthcarePlan healthcarePlan = transaction.BusinessContractDTO.CurrentHealthcarePlan;

        //    Mapper.Reset();
        //    Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.Fabric.HealthcarePlanCarePlanEligibilityStatus, HealthcareBC.Tracker.HealthcarePlanCarePlanEligibilityStatus>());
        //    HealthcareBC.Tracker.HealthcarePlanCarePlanEligibilityStatus _eligibilityStatus =
        //        Mapper.Map<HealthcareBC.Tracker.HealthcarePlanCarePlanEligibilityStatus>(HealthcarePlanEligibilityStatus);

        //    healthcarePlan.CarePlanEligibilityStatus = _eligibilityStatus;

        //    Mapper.Reset();
        //    Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.Tracker.ContractTransaction, HealthcareBC.Fabric.ContractTransaction>());

        //    HealthcareBC.Fabric.ContractTransaction _tranaction =
        //        Mapper.Map<HealthcareBC.Fabric.ContractTransaction>(transaction);

        //    //Indexing Service
        //    LogTransactionAsync(_tranaction, "Healthcare plan Eligibility Status was assigned...").GetAwaiter().GetResult();

        //    return _tranaction;//UpdateProfile(_tranaction.BusinessContractDTO, _tranaction.BindingId, "Healthcareplan Eligibility status as assigned");
        //}

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

        //[HttpPost()]
        //[Route("GetTransactionsByBindingId")]
        //public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetTransactionsByBindingId(string bindingId)
        //{
        //    //Tracker Service
        //    var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, ServiceEventSource.Current);
        //    TrackerProxy trackerProxy = new TrackerProxy(trackerEndpoint, new HttpClient());

        //    var result = trackerProxy.GetTransactionInformationAsync(bindingId).GetAwaiter().GetResult().ToList();

        //    Mapper.Reset();
        //    Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

        //    List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

        //    ServiceEventSource.Current.Message($"transaction informations are {result}");

        //    return _transactions;
        //}


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

        //[HttpPost()]
        //[Route("GetCitizenProfilesNotAssignedHealthcareEligibility")]
        //public ActionResult<List<HealthcareBC.Fabric.ContractTransaction>> GetCitizenProfilesNotAssignedHealthcareEligibility(string State)
        //{
        //    var result = _appService.GetCitizenProfilesNotAssignedHealthcareEligibility(State).Result;

        //    Mapper.Reset();
        //    Mapper.Initialize(cfg => cfg.CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>());

        //    List<HealthcareBC.Fabric.ContractTransaction> _transactions = Mapper.Map<List<HealthcareBC.Fabric.ContractTransaction>>(result);

        //    ServiceEventSource.Current.Message($"transaction informations are {result}");

        //    return _transactions;
        //}

    }
}

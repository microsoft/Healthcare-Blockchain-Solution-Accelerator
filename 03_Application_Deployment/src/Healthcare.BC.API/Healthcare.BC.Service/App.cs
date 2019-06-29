using Healthcare.Common.ServiceHost;
using HealthcareBC.ProofDocSvc;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using AutoMapper;
using System.Reactive.Linq;
using HealthcareBC.Tracker;
using HealthcareBC.Fabric;

namespace Healthcare.BC.Service
{
    public class App : IApp
    {
        #region "Tracker Service"
        async public Task<HealthcareBC.Tracker.ContractTransaction> GetTransactionByEntityId(string entityId)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            //var decrypter = new Cryptolib("");
            //return await decryptPII((await trackerProxy.GetTransactionInformationByTxEntityIdAsync(entityId)), decrypter);
            return await trackerProxy.GetTransactionInformationByTxEntityIdAsync(entityId);
        }

        async public Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCompletedCitizenProfiles(string State)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            var _profiles = await trackerProxy.GetCompletedCitizenProfilesAsync(State);
            return _profiles;
        }

        public async Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetInCompletedCitizenProfiles(string State)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            var _profiles = await trackerProxy.GetIncompletedCitizenProfilesAsync(State);
            return _profiles;
        }

        //Decrypt Profile Collection.
        //Worring decrypting cost.


        async public Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfiles(string State)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            var _profiles = await trackerProxy.GetCitizenProfilesAsync(State);
            return _profiles;
        }

        async public Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfilesNotApprovedHealthcareplan(string State)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            var _profiles = await trackerProxy.GetCitizenProfilesNotApprovedHealthcareplanAsync(State);
            return _profiles;
        }

        async public Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfilesApprovedHealthcareplan(string State)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            var _profiles = await trackerProxy.GetCitizenProfilesApprovedHealthcareplanAsync(State);
            return _profiles;
        }

        async public Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfilesNotAssignedHealthcareEligibility(string State)
        {
            var trackerEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.trackerEndPoint, null);
            HealthcareBC.Tracker.TrackerProxy trackerProxy = new HealthcareBC.Tracker.TrackerProxy(trackerEndpoint, new HttpClient());

            var _profiles = await trackerProxy.GetCitizenProfilesNotAssignedHealthcareEligibilityAsync(State);
            return _profiles;
        }

        #endregion

        #region "Fabric Service"
        async public Task<HealthcareBC.Fabric.ContractTransaction> CreateCitizenProfile(HealthcareBC.Fabric.Profile citizenProfile)
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            FabricProxy FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient() { Timeout = new TimeSpan(0,30,0)});
            
            //TODO : Add the Encryption after the Demo / Decrypting the results / Add the decryption to Lists

            //Encrypt basicProfile information
            //var encrypter = new Cryptolib("");
            //citizenProfile.BasicProfile.Name = encrypter.EncryptString(citizenProfile.BasicProfile.Name).Result;
            //citizenProfile.BasicProfile.Address.Street = encrypter.EncryptString(citizenProfile.BasicProfile.Address.Street).Result;
            //citizenProfile.BasicProfile.Address.City = encrypter.EncryptString(citizenProfile.BasicProfile.Address.City).Result;

            var transaction = await FabricProxy.CreateProfileAsync(citizenProfile);

            //Indexing Service
            await LogTransaction(transaction, "Citizen Profile was created");

            return transaction;
        }

        async public Task<HealthcareBC.Fabric.ContractTransaction> UpdateProfileWithProofDocument(HealthcareBC.Fabric.DocProof docProof, string citizenIdentifier)
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            FabricProxy FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient(){ Timeout = new TimeSpan(0, 30, 0) });

            var transaction = await FabricProxy.UpdateProfileAsync(citizenIdentifier, docProof);

            //Indexing Service
            await LogTransaction(transaction, "Citizen Proof Document was checked and updated on Profile");

            return transaction;
        }

        async public Task<HealthcareBC.Fabric.ContractTransaction> GetCitizenProfile(string citizenIdentifier)
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            FabricProxy FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient(){ Timeout = new TimeSpan(0, 30, 0) });

            var retProfile = await FabricProxy.GetProfileInformationAsync(citizenIdentifier);

            //Decrypt basicProfile information
            //var decrypter = new Cryptolib("");
            //retProfile.BusinessContractDTO.BasicProfile.Name = decrypter.DecryptString(retProfile.BusinessContractDTO.BasicProfile.Name).Result;
            //retProfile.BusinessContractDTO.BasicProfile.Address.Street = decrypter.DecryptString(retProfile.BusinessContractDTO.BasicProfile.Address.Street).Result;
            //retProfile.BusinessContractDTO.BasicProfile.Address.City = decrypter.DecryptString(retProfile.BusinessContractDTO.BasicProfile.Address.City).Result;

            return retProfile;
        }

        async public Task<HealthcareBC.Fabric.ContractTransaction> AssignHealthcarePlan(HealthcareBC.Fabric.HealthcarePlan healthcarePlan, string citizenIdentifier)
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            FabricProxy FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient(){ Timeout = new TimeSpan(0, 30, 0) });

            var transaction = await FabricProxy.AssignHealthcarePlanAsync(healthcarePlan, citizenIdentifier);

            //Indexing Service
            await LogTransaction(transaction, "Healthcare Plan was assigned and eligiility will be updated by Fabric logic after transaction");
            return transaction;//await FabricProxy.AssignHealthcarePlanAsync(healthcarePlan, citizenIdentifier);
        }

        async public Task<HealthcareBC.Fabric.ContractTransaction> ApproveHealthcarePlan(string citizenIdentifier)
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            FabricProxy FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient(){ Timeout = new TimeSpan(0, 30, 0) });

            var transaction = await FabricProxy.ApproveHealthcarePlanAsync(citizenIdentifier);

            //Indexing Service
            await LogTransaction(transaction, "Healthcare Plan was Approved");


            return transaction;
        }

        async public Task<HealthcareBC.Fabric.ContractTransaction> ChangeActiveState(string citizenIdentifier, string activeState)
        {
            var FabricEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.fabricEndPoint, null);
            FabricProxy FabricProxy = new FabricProxy(FabricEndpoint, new HttpClient(){ Timeout = new TimeSpan(0, 30, 0) });

            var transaction = await FabricProxy.ChangeActiveStateAsync(citizenIdentifier, activeState);
            //Indexing Service
            await LogTransaction(transaction, $"Current Active State was changed into {activeState}");

            return transaction;//await FabricProxy.ChangeActiveStateAsync(citizenIdentifier, activeState);
        }

        #endregion

        #region "ProofDocument Service"

        async public Task<HealthcareBC.ProofDocSvc.DocProof> PutProofDoc(string fileName, Stream fileStream, string citizenIdentifier)
        {
            if (fileStream.Length == 0)
            {
                return null;
            }

            FileParameter file = null;
            file = new FileParameter(fileStream, fileName);

            var proofDocSvcEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.proofsvcEndPoint, null);
            ProofDocProxy proofDocSvc = new ProofDocProxy(proofDocSvcEndpoint, new HttpClient());

            return await proofDocSvc.PutProofDocAsync(file, citizenIdentifier, citizenIdentifier);
        }

        async public Task<string> GetProofDocumentUrl(string citizenIdentifier)
        {
            var proofDocSvcEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.proofsvcEndPoint, null);
            ProofDocProxy proofDocSvc = new ProofDocProxy(proofDocSvcEndpoint, new HttpClient());

            return await proofDocSvc.GetProofDocAsync(citizenIdentifier, citizenIdentifier);
        }

        async public Task<bool> ValidateHash(string citizenIdentifier, string hash)
        {
            var proofDocSvcEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.proofsvcEndPoint, null);
            ProofDocProxy proofDocSvc = new ProofDocProxy(proofDocSvcEndpoint, new HttpClient());

            return await proofDocSvc.ValidateFileWithHashAsync(citizenIdentifier, citizenIdentifier, hash);
        }

        #endregion


        async public Task LogTransaction(HealthcareBC.Fabric.ContractTransaction transaction, string txDFabricription = "")
        {
            //Indexing Service
            var indexingEndpoint = ServiceEndPointResolver.GetServiceEndPoint(ServiceEndpoints.indexerEndPoint, null);
            HealthcareBC.Indexer.IndexerProxy indexerProxy = new HealthcareBC.Indexer.IndexerProxy(indexingEndpoint, new HttpClient());

            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.Fabric.ContractTransaction, HealthcareBC.Indexer.ContractTransaction>());
            HealthcareBC.Indexer.ContractTransaction contractDto = Mapper.Map<HealthcareBC.Indexer.ContractTransaction>(transaction);

            await indexerProxy.LogTransactionAsync(contractDto, txDFabricription);
        }
    }
}

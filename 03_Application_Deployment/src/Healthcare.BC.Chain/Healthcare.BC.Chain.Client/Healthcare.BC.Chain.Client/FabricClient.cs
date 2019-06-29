using System;
using AutoMapper;
using Grpc.Core;
using Models = Healthcare.BC.Offchain.Repository.Models;
using Messages = Healthcare.BC.Chain.Services;


namespace Healthcare.BC.Chain.Fabric.Client
{
    public class FabricClient : IFabricClient
    {
        private Messages.HealthcareService.HealthcareServiceClient client;
        private readonly string bindingID = "e3f5f543-9fe5-4e21-a6cd-f1f0b8ae3e3a";

        public FabricClient()
        {
            // TODO - move to appsettings.json
            Channel channel = new Channel("104.214.73.176:9090", ChannelCredentials.Insecure);
            client = new Messages.HealthcareService.HealthcareServiceClient(channel);
        }

        public Models.ContractTransaction ApproveHealthcarePlan(string citizenIdentifier, string planApprover)
        {
            var reply = client.approveHealthcarePlan(new Messages.ApproveHealthcarePlan { CitizenIdentifier = citizenIdentifier, PlanApprover = planApprover });
            var profile = ServiceProfileToModelProfile(reply);
            var result = prepareDeployObject(profile);
            return result;
        }

        public Models.ContractTransaction AssignHealthcarePlan(string citizenIdentifier, Models.HealthcarePlanItem healthcarePlan)
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<Models.HealthcarePlanItem, Messages.HealthcarePlanItem>());
            Messages.HealthcarePlanItem _healthcareplan = Mapper.Map<Messages.HealthcarePlanItem>(healthcarePlan);
            var reply = client.assignHealthcarePlan(new Messages.AssignHealthcarePlan { CitizenIdentifier = citizenIdentifier, Plan = _healthcareplan});
            var profile = ServiceProfileToModelProfile(reply);
            return prepareTxObject(profile.CitizenIdentifier, profile);
        }

        public Models.ContractTransaction ChangeActiveState(string citizenIdentifier, string state)
        {
            var reply = client.changeActiveState(new Messages.ChangeActiveState {
                CitizenIdentifier = citizenIdentifier,
                ActiveState = state == "NY" ? Messages.State.Ny : Messages.State.Nj
            });

            var profile = ServiceProfileToModelProfile(reply);
            return prepareTxObject(profile.CitizenIdentifier, profile);
        }

        public Models.ContractTransaction CreateProfile(Models.Profile profile)
        {
            var _profile = ModelProfileToServceProfile(profile);
            var result = client.createProfile(_profile);
            var citizenProfile = ServiceProfileToModelProfile(_profile);
            Models.ContractTransaction transaction = prepareDeployObject(citizenProfile);
            return transaction;
        }

        public Models.Profile GetProfileInformation(string citizenIdentifier)
        {
            var reply = client.getProfileInformation(new Messages.GetProfileInformation { CitizenIdentifier = citizenIdentifier });
            var profile = ServiceProfileToModelProfile(reply);
            return profile;
        }

        public Models.ContractTransaction SetHealthcareEligibility(string citizenIdentifier, bool eligibility)
        {
            var reply = client.setEligibility(new Messages.SetEligibility { CitizenIdentifier = citizenIdentifier, Eligible = eligibility });
            var profile = ServiceProfileToModelProfile(reply);
            return prepareTxObject(profile.CitizenIdentifier, profile);
        }
        public Models.ContractTransaction UpdateProfile(string citizenIdentifier, Models.DocProof proofDoc)
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<Models.DocProof, Messages.DocProof>());
            Messages.DocProof _docProof = new Messages.DocProof
            {
                Container = proofDoc.Container,
                ContentType = "image/known",
                FileName = proofDoc.FileName,
                Hash = proofDoc.Hash,
                StorageSharding = proofDoc.StorageSharding
            };
            var reply = client.updateProfileProofDocument(new Messages.UpdateProfileProofDocument { CitizenIdentifier = citizenIdentifier, DocProof = _docProof });
            var profile = ServiceProfileToModelProfile(reply);
            return prepareTxObject(profile.CitizenIdentifier, profile);
        }

        private static Models.Profile ServiceProfileToModelProfile(Messages.Profile profile)
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<Messages.Profile, Models.Profile>()
               .ForPath(d => d.BasicProfile.Name, o => o.MapFrom(s => s.BasicProfile.Name))
        .ForPath(d => d.BasicProfile.DateOfBirth, o => o.MapFrom(s => s.BasicProfile.DateOfBirth.ToDateTimeOffset()))
        .ForPath(d => d.BasicProfile.Address.Street, o => o.MapFrom(s => s.BasicProfile.FullAddress.Street))
        .ForPath(d => d.BasicProfile.Address.City, o => o.MapFrom(s => s.BasicProfile.FullAddress.City))
        .ForPath(d => d.BasicProfile.Address.State, o => o.MapFrom(s => s.BasicProfile.FullAddress.State == Messages.State.Ny ? "NY" : "NJ"))
        .ForPath(d => d.BasicProfile.Address.Zip, o => o.MapFrom(s => s.BasicProfile.FullAddress.Zip))
        .ForPath(d => d.BasicProfile.FedIncome, o => o.MapFrom(s => s.BasicProfile.FedIncome))
        .ForPath(d => d.BasicProfile.StateIncome, o => o.MapFrom(s => s.BasicProfile.StateIncome))
        .ForPath(d => d.BasicProfile.ApplicationType, o => o.MapFrom(s => s.BasicProfile.ApplicationType == Messages.ApplicationType.Individual ? Models.ApplicationType.Indivisual : Models.ApplicationType.Family))
        .ForPath(d => d.BasicProfile.CitizenShip, o => o.MapFrom(s => s.BasicProfile.Citizenship))
        .ForPath(d => d.CurrentHealthcarePlan.EnrollmentBorkerAssigedState, o => o.MapFrom(s => s.CurrentHealthcarePlan.EnrollmentBrokerAssignedState == Messages.State.Ny ? "NY" : "NJ"))
        .ForPath(d => d.ActiveState, o => o.MapFrom(s => s.ActiveState == Messages.State.Ny ? "NY" : "NJ"))
        .ForPath(d => d.TransactionID, o => o.MapFrom(s => s.TransactionId ?? " "))
        .ForPath(d => d.TransactedTime, o => o.MapFrom(s => s.TransactedTime.ToDateTimeOffset()))
        .ForPath(d => d.CurrentHealthcarePlan.Plan.Id, o => o.MapFrom(s => s.CurrentHealthcarePlan.Plan.Id))
        .ForPath(d => d.CurrentHealthcarePlan.Plan.Name, o => o.MapFrom(s => s.CurrentHealthcarePlan.Plan.Name))
        .ForPath(d => d.CurrentHealthcarePlan.HealthcareStatus, o => o.MapFrom(s => s.CurrentHealthcarePlan.HealthcareStatus))
        .ForPath(d => d.CurrentHealthcarePlan.CarePlanEligibilityStatus, o => o.MapFrom(s => s.CurrentHealthcarePlan.EnrollmentStatus))
        .ForPath(d => d.CurrentHealthcarePlan.OwnerState, o => o.MapFrom(s => s.CurrentHealthcarePlan.OwnerState == Messages.State.Ny ? "NY" : "NJ"))
        .ForPath(d => d.CurrentHealthcarePlan.EnrollmentBorkerAssigedState, o => o.MapFrom(s => s.CurrentHealthcarePlan.EnrollmentBrokerAssignedState == Messages.State.Ny ? "NY" : "NJ"))
        .ForPath(d => d.CurrentHealthcarePlan.EnrollmentBorkerAssigedState, o => o.MapFrom(s => s.CurrentHealthcarePlan.EnrollmentBrokerAssignedState == Messages.State.Ny ? "NY" : "NJ"))
        .ForPath(d => d.CurrentHealthcarePlan.PlanApprover, o => o.MapFrom(s => s.CurrentHealthcarePlan.PlanApprover))
        .ForPath(d => d.CurrentHealthcarePlan.Eligible, o => o.MapFrom(s => s.CurrentHealthcarePlan.Eligible))
        .ForPath(d => d.CurrentHealthcarePlan.Approved, o => o.MapFrom(s => s.CurrentHealthcarePlan.Approved))
        .ForPath(d => d.CurrentHealthcarePlan.AssignedDate, o => o.MapFrom(s => s.CurrentHealthcarePlan.AssignedDate.ToDateTimeOffset()))
        .ForPath(d => d.CurrentHealthcarePlan.EnrolledDate, o => o.MapFrom(s => s.CurrentHealthcarePlan.EnrolledDate.ToDateTimeOffset()))
        .ForPath(d => d.CurrentHealthcarePlan.ApprovedDate, o => o.MapFrom(s => s.CurrentHealthcarePlan.ApprovedDate.ToDateTimeOffset()))
            );
            Healthcare.BC.Offchain.Repository.Models.Profile profileDTO = Mapper.Map<Healthcare.BC.Offchain.Repository.Models.Profile>(profile);

            //adding business logic to here. afraid but ESC logic doesn't work correctly.
            profileDTO.CurrentHealthcarePlan.Eligible = ((profileDTO.BasicProfile.ApplicationType == Models.ApplicationType.Indivisual) &&
                (profileDTO.BasicProfile.FedIncome <= 12400.00) &&
                (profileDTO.BasicProfile.StateIncome <= 12400.00) &&
                (profileDTO.BasicProfile.CitizenShip) && (profileDTO.CurrentHealthcarePlan.HealthcareStatus != Models.HealthcareStatus.Enrolled));


            return profileDTO;
        }
        private static Messages.Profile ModelProfileToServceProfile(Models.Profile profile)
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.CreateMap<Models.Profile, Messages.Profile>()
            .ForPath(d => d.BasicProfile.Name, o => o.MapFrom(s => s.BasicProfile.Name))
            .ForPath(d => d.BasicProfile.DateOfBirth, o => o.MapFrom(s => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(s.BasicProfile.DateOfBirth)))
            .ForPath(d => d.BasicProfile.FullAddress.Street, o => o.MapFrom(s => s.BasicProfile.Address.Street))
            .ForPath(d => d.BasicProfile.FullAddress.City, o => o.MapFrom(s => s.BasicProfile.Address.City))
            .ForPath(d => d.BasicProfile.FullAddress.State, o => o.MapFrom(s => s.BasicProfile.Address.State.ToUpper() == "NY" ? Messages.State.Ny : Messages.State.Nj))
            .ForPath(d => d.BasicProfile.FullAddress.Zip, o => o.MapFrom(s => s.BasicProfile.Address.Zip))
            .ForPath(d => d.BasicProfile.FedIncome, o => o.MapFrom(s => s.BasicProfile.FedIncome))
            .ForPath(d => d.BasicProfile.StateIncome, o => o.MapFrom(s => s.BasicProfile.StateIncome))
            .ForPath(d => d.BasicProfile.ApplicationType, o => o.MapFrom(s => s.BasicProfile.ApplicationType == Models.ApplicationType.Indivisual ? Messages.ApplicationType.Individual : Messages.ApplicationType.Family))
            .ForPath(d => d.BasicProfile.Citizenship, o => o.MapFrom(s => s.BasicProfile.CitizenShip))
            .ForPath(d => d.CurrentHealthcarePlan.EnrollmentBrokerAssignedState, o => o.MapFrom(s => s.CurrentHealthcarePlan.EnrollmentBorkerAssigedState.ToUpper() == "NY" ? Messages.State.Ny : Messages.State.Nj))
            .ForPath(d => d.TransactionId, o => o.MapFrom(s => s.TransactionID ?? " "))
            .ForPath(d => d.TransactedTime, o => o.MapFrom(s => Google.Protobuf.WellKnownTypes.Timestamp.FromDateTimeOffset(s.TransactedTime)))
            .ForPath(d => d.EnrolledHealthInsurance, o => o.MapFrom(s => s.CurrentHealthcarePlan.Plan.Name == null ? false : true))
            );

            Messages.Profile profileDTO = Mapper.Map<Messages.Profile>(profile);
            return profileDTO;
        }

        private Models.ContractTransaction prepareTxObject(string bindingid, BC.Offchain.Repository.Models.Profile CitizenProfile)
        {


            var _dto = CitizenProfile;

            _dto.TransactedTime = DateTime.Now;
            _dto.TransactionID = Guid.NewGuid().ToString();

            Models.TransactionConfirmation _transactionConfirmation = new Models.TransactionConfirmation()
            {

                BlockHash = "0xfa4e2a31506c1f930efc7701ff6ddc1451d08a38a7a9267fe263766b4c7ea2d0",
                BlockNumber = "1",
                TransactionHash = "",
                Name = "Fake Contract",
                ProxyId = "Fake Contract_Proxy ID",
                TransactionIndex = "1"

            };
            Models.ContractTransaction _txInformation = new Models.ContractTransaction(bindingid, " Enterprise Smart Contract transacted.....", _dto, _transactionConfirmation);
            return _txInformation;
        }

        private Models.ContractTransaction prepareDeployObject(Models.Profile citizenProfile)
        {

            Models.ConstructorConfirmation _deployConfirmation = new Models.ConstructorConfirmation()
            {
                NewContractOrTokenId = Guid.NewGuid().ToString(),
                Name = "Smart Contract instance by ESC Template",
                TransactionConfirmation = new Models.TransactionConfirmation()
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
            Models.ContractTransaction _txInformation = new Models.ContractTransaction(citizenProfile, _deployConfirmation);
            return _txInformation;
        }


    }
}

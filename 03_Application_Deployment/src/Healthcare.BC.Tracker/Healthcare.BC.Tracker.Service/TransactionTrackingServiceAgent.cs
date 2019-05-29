using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using Healthcare.BC.Offchain.Repository.ModelBase;
using Healthcare.BC.Offchain.Repository.Models;
using Healthcare.BC.Offchain.Repository;

namespace Healthcare.BC.Tracker.Service
{
    public class TransactionTrackingServiceAgent
    {
        private readonly IRepository<ContractTransaction, Guid> _transactionIndexRepository;

        public TransactionTrackingServiceAgent(string mongoconnString)
        {
            _transactionIndexRepository =
                  new BusinessTransactionRepository<ContractTransaction, Guid>(new MongoClient(mongoconnString));

        }

        //public IEnumerable<ContractTransaction> GetTransactionHistoryFromOffchain(string BindingID)
        //{
        //    var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x => x.BindingId == BindingID));
        //    return result;
        //}   

        public IEnumerable<ContractTransaction> GetTransactionHistoryByCitizenProfileFromOffchain(string CitizenIdentifier)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x => x.BusinessContractDTO.CitizenIdentifier == CitizenIdentifier));
            return result;
        }


        public ContractTransaction GetCurrentCitizenProfileFromOffchainByCitizenIdentifier(string CitizenIdentifier)
        {
            var result = _transactionIndexRepository.Find(new GenericSpecification<ContractTransaction>(x => x.BusinessContractDTO.CitizenIdentifier == CitizenIdentifier &&
                                                                                                                x.IsActiveTransaction));
            return result;
        }

        //public ContractTransaction GetCurrentCitizenProfileFromOffchainByBindingId(string BindingId)
        //{
        //    var result = _transactionIndexRepository.Find(new GenericSpecification<ContractTransaction>(x => x.BindingId == BindingId &&
        //                                                                                                        x.IsActiveTransaction));
        //    return result;
        //}




        //EntityId will be Business Entity's Id fields
        public ContractTransaction GetTransactionHistoryByTxEntityId(string EntityId)
        {
            var result = _transactionIndexRepository.Find(new GenericSpecification<ContractTransaction>(x => x.Id == Guid.Parse(EntityId)));
            return result;
        }


        
        public IEnumerable<ContractTransaction> GetCompletedCitizenProfiles(string State)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x => 
                                                                                                                (x.BusinessContractDTO.Status == ProfileStatus.ProfileCompleted) && 
                                                                                                                (x.BusinessContractDTO.ActiveState == State) && 
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan.Eligible) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan.CarePlanEligibilityStatus == CarePlanEligibilityStatus.NotAssigned) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan.HealthcareStatus != HealthcareStatus.Assigned) &&
                                                                                                                !x.BusinessContractDTO.CurrentHealthcarePlan.Approved &&
                                                                                                                x.IsActiveTransaction));
            return result;
        }

        public IEnumerable<ContractTransaction> GetIncompletedCitizenProfiles(string State)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x =>
                                                                                                                (x.BusinessContractDTO.Status == ProfileStatus.ProfileCreated) &&
                                                                                                                (x.BusinessContractDTO.ActiveState == State) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan == null) &&
                                                                                                                x.IsActiveTransaction));
            return result;
        }

        public IEnumerable<ContractTransaction> GetCitizenProfiles(string State)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x =>
                                                                                                                (x.BusinessContractDTO.ActiveState == State) &&
                                                                                                                x.IsActiveTransaction));
            return result;
        }

        public IEnumerable<ContractTransaction> GetCitizenProfilesNotApprovedHealthcareplan(string State)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x =>
                                                                                                                (x.BusinessContractDTO.Status == ProfileStatus.ProfileCompleted) &&
                                                                                                                (x.BusinessContractDTO.ActiveState == State) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan != null) &&
                                                                                                                (x.IsActiveTransaction) && 
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan.HealthcareStatus == HealthcareStatus.Assigned) &&
                                                                                                                (!x.BusinessContractDTO.CurrentHealthcarePlan.Approved)));
            return result;
        }

        public IEnumerable<ContractTransaction> GetCitizenProfilesApprovedHealthcareplan(string State)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x =>
                                                                                                                (x.BusinessContractDTO.Status == ProfileStatus.ProfileCompleted) &&
                                                                                                                (x.BusinessContractDTO.ActiveState == State) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan != null) &&
                                                                                                                x.IsActiveTransaction &&
                                                                                                                x.BusinessContractDTO.CurrentHealthcarePlan.CarePlanEligibilityStatus != CarePlanEligibilityStatus.NotAssigned &&
                                                                                                                 (x.BusinessContractDTO.CurrentHealthcarePlan.Eligible) &&
                                                                                                                x.BusinessContractDTO.CurrentHealthcarePlan.Approved));
            return result;
        }

        public IEnumerable<ContractTransaction> GetCitizenProfilesbyHealthcareByEligibility(string State, CarePlanEligibilityStatus EligibilityStatus)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x =>
                                                                                                                (x.BusinessContractDTO.Status == ProfileStatus.ProfileCompleted) &&
                                                                                                                (x.BusinessContractDTO.ActiveState == State) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan != null) &&
                                                                                                                x.IsActiveTransaction &&
                                                                                                                !x.BusinessContractDTO.CurrentHealthcarePlan.Approved &&
                                                                                                                 (x.BusinessContractDTO.CurrentHealthcarePlan.Eligible) &&
                                                                                                                x.BusinessContractDTO.CurrentHealthcarePlan.CarePlanEligibilityStatus == EligibilityStatus));
            return result;
        }


        public IEnumerable<ContractTransaction> GetCitizenProfilesNotAssignedHealthcareEligibility(string State)
        {
            var result = _transactionIndexRepository.FindAll(new GenericSpecification<ContractTransaction>(x =>
                                                                                                                (x.BusinessContractDTO.Status == ProfileStatus.ProfileCompleted) &&
                                                                                                                (x.BusinessContractDTO.ActiveState == State) &&
                                                                                                                (x.BusinessContractDTO.CurrentHealthcarePlan != null) &&
                                                                                                                x.IsActiveTransaction &&
                                                                                                                !x.BusinessContractDTO.CurrentHealthcarePlan.Approved &&
                                                                                                                 (x.BusinessContractDTO.CurrentHealthcarePlan.Eligible) &&
                                                                                                                x.BusinessContractDTO.CurrentHealthcarePlan.CarePlanEligibilityStatus == CarePlanEligibilityStatus.NotAssigned));
            return result;
        }

    }
}

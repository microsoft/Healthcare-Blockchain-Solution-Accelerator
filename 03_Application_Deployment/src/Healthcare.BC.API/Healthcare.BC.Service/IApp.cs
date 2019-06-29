using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HealthcareBC.Fabric;
using HealthcareBC.ProofDocSvc;
using HealthcareBC.Tracker;

namespace Healthcare.BC.Service
{
    public interface IApp
    {
        Task<HealthcareBC.Fabric.ContractTransaction> ApproveHealthcarePlan(string citizenIdentifier);
        Task<HealthcareBC.Fabric.ContractTransaction> AssignHealthcarePlan(HealthcareBC.Fabric.HealthcarePlan healthcarePlan, string citizenIdentifier);
        Task<HealthcareBC.Fabric.ContractTransaction> ChangeActiveState(string citizenIdentifier, string activeState);
        Task<HealthcareBC.Fabric.ContractTransaction> CreateCitizenProfile(HealthcareBC.Fabric.Profile citizenProfile);
        Task<HealthcareBC.Fabric.ContractTransaction> GetCitizenProfile(string citizenIdentifier);
        Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfiles(string State);
        Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfilesApprovedHealthcareplan(string State);
        Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfilesNotApprovedHealthcareplan(string State);
        Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCitizenProfilesNotAssignedHealthcareEligibility(string State);
        Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetCompletedCitizenProfiles(string State);
        Task<IEnumerable<HealthcareBC.Tracker.ContractTransaction>> GetInCompletedCitizenProfiles(string State);
        Task<string> GetProofDocumentUrl(string citizenIdentifier);
        Task<HealthcareBC.Tracker.ContractTransaction> GetTransactionByEntityId(string entityId);
        Task LogTransaction(HealthcareBC.Fabric.ContractTransaction transaction, string txDFabricription = "");
        Task<HealthcareBC.ProofDocSvc.DocProof> PutProofDoc(string fileName, Stream fileStream, string citizenIdentifier);
        Task<HealthcareBC.Fabric.ContractTransaction> UpdateProfileWithProofDocument(HealthcareBC.Fabric.DocProof docProof, string citizenIdentifier);
        Task<bool> ValidateHash(string citizenIdentifier, string hash);
    }
}
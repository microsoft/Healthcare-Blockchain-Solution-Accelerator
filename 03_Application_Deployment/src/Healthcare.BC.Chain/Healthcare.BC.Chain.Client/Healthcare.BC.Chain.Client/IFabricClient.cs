using Healthcare.BC.Offchain.Repository.Models;

namespace Healthcare.BC.Chain.Fabric.Client
{
    public interface IFabricClient
    {
        ContractTransaction ApproveHealthcarePlan(string citizenIdentifier, string planApprover);
        ContractTransaction AssignHealthcarePlan(string citizenIdentifier, HealthcarePlanItem healthcarePlan);
        ContractTransaction ChangeActiveState(string citizenIdentifier, string state);
        ContractTransaction CreateProfile(Profile profile);
        Profile GetProfileInformation(string citizenIdentifier);
        ContractTransaction SetHealthcareEligibility(string citizenIdentifier, bool eligibility);
        ContractTransaction UpdateProfile(string citizenIdentifier, DocProof proofDoc);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Healthcare.BC.Offchain.Repository.ModelBase;

namespace Healthcare.BC.Offchain.Repository.Models
{
    public class BasicProfileItem
    {
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        // </summary>
        public string Name { get; set; }
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        // </summary>
        public Address Address { get; set; }
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        // </summary>
        public DateTimeOffset DateOfBirth { get; set; }
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        // </summary>
        public bool CitizenShip { get; set; }
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        // </summary>     
        public double FedIncome { get; set; }
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        // </summary>
        public double StateIncome { get; set; }

        public ApplicationType ApplicationType { get; set; }
    }

    public class DocProof
    {
        public string FileName { get; set; }
        public string Hash { get; set; }
        public string ContentType { get; set; }
        public string Container { get; set; }
        public string StorageSharding { get; set; }
    }
  

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class ProfileHistory
    {
        public DateTimeOffset Date { get; set; }
        public string UpdatedState { get; set; }
        public string Comments { get; set; }
    }

    public class HealthcarePlan
    {
        public HealthcareStatus HealthcareStatus { get; set; }
        public HealthcarePlanItem Plan { get; set; }
        public string OwnerState { get; set; } 
        public string EnrollmentBorkerAssigedState { get; set; }
        public CarePlanEligibilityStatus CarePlanEligibilityStatus { get; set; }
        public string PlanApprover { get; set; }
        public bool Approved { get; set; }
        public DateTimeOffset AssignedDate { get; set; } //ESC
        public DateTimeOffset EnrolledDate { get; set; }
        public DateTimeOffset ApprovedDate { get; set; }
        public bool Eligible { get; set; }
    }

    public class HealthcarePlanItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    [Flags]
    public enum ApplicationType
    {
        Indivisual,
        Family
    }

    [Flags]
    public enum HealthcareStatus
    {
        NotAssigned,
        Assigned,
        Approved,
        Enrolled
    }
    [Flags]
    public enum CarePlanEligibilityStatus
    {
        NotAssigned,
        Mandatory,
        Voluntary,
        InEligible
    }
    [Flags]
    public enum ProfileStatus
    {
        ProfileCreated,
        ProfileCompleted
    }
    public class Profile //: IBusinessEntity
    {
        /// <summary>
        /// GUID String
        /// </summary>
        public string TransactionID { get; set; }
        public DateTimeOffset TransactedTime { get; set; }
        public string Description { get; set; }
        /// <summary>
        //Basic Identifier Entities
        //should be encrypted and only can be seeen by active state
        //GUID String
        // </summary>
        public string CitizenIdentifier { get; set; }
        public BasicProfileItem BasicProfile { get; set; }

        public DocProof IdentifyProofs { get; set; }
        //public DocProof[] CitizenshipProofs { get; set; }
        //Citizen Information
        public HealthcarePlanItem PreferredHealthcarePlan { get; set; }

        public ProfileStatus Status { get; set; }
        public string ActiveState { get; set; }
        public string StateApprover { get; set; } //string
        public HealthcarePlan CurrentHealthcarePlan { get; set; }

        //public HealthcarePlan[] HealthcarePlanHistory { get; set; }

    }


}
using Healthcare.BC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;

namespace Healthcare.BC.Application.API.Test
{
    [TestClass]
    public class ApplicationUnitTest
    {
        public class MockCustomer
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Street { get; set; }
            public string zip { get; set; }
            public double FedIncome { get; set; }
            public double StateIncome { get; set; }
            public DateTime Dateofbirth { get; set; }
        }

        public HealthcareBC.ESC.Profile GetRandomProfile()
        {
            string mockupPath = @"..\..\..\MOCK_DATA.json";
            string customers_mockupjson = System.IO.File.ReadAllText(mockupPath);
            ICollection<MockCustomer> mock_customers = JsonConvert.DeserializeObject<ICollection<MockCustomer>>(customers_mockupjson);

            List<MockCustomer> _mockupCustomer = new List<MockCustomer>();

            foreach (var item in mock_customers)
            {
                var customer = new MockCustomer()
                {
                    first_name = item.first_name,
                    last_name = item.last_name,
                    City = item.City,
                    State = item.State,
                    Street = item.Street,
                    zip = item.zip,
                    FedIncome = item.FedIncome,
                    StateIncome = item.StateIncome,
                    Dateofbirth = item.Dateofbirth
                };

                _mockupCustomer.Add(customer);
            }

            var _customer = _mockupCustomer[new Random().Next(0, 99)];
            BasicProfileItem _basicProfile = new BasicProfileItem()
            {
                Name = _customer.first_name + " " + _customer.last_name,
                Address = new Address() { Street = _customer.Street, City = _customer.City, State = _customer.State, Zip = _customer.zip },
                DateOfBirth = _customer.Dateofbirth,
                CitizenShip = true,
                ApplicationType = BasicProfileItemApplicationType._0,
                FedIncome = _customer.FedIncome,
                StateIncome = _customer.StateIncome
            };

            //Creating Profile Object
            var _profile = new HealthcareBC.ESC.Profile()
            {
                //Create Citizen Identifier
                CitizenIdentifier = Guid.NewGuid().ToString(),
                //Need to be Assigned ActiveState
                ActiveState = "NY",
                //Assign Prefered Healthcare Plan
                PreferredHealthcarePlan = new HealthcarePlanItem() { Name = "Health Insurance Plan of Greater New York, Inc.", Id = "NaN" },
                //default
                CurrentHealthcarePlan = null,
                //default
                StateApprover = " ",
                //set status as "Profile Created"
                Status = ProfileStatus._0
            };

            _profile.BasicProfile = _basicProfile;

            _profile.Description = @"description for citizen profile " + _profile.CitizenIdentifier;

            return _profile;
        }

        public string CID { get; set; }

        private ContractTransaction CreateProfile(HealthcareBC.ESC.Profile _profile)
        {
            App _app = new App();
            var _contract = _app.CreateCitizenProfile(_profile).Result;
            this.CID = _contract.BusinessContractDTO.CitizenIdentifier;

            Assert.IsNotNull(_contract, "Customer was not created");
            return _contract;

            //return _app.CreateCitizenProfile(_profile).Result;
        }

        //[TestMethod]
        private HealthcareBC.ProofDocSvc.DocProof UploadProofDocucment(string cid)
        {
            App _app = new App();
            string imagePath = @"..\..\..\drivelicense.jpg";
            FileStream imageStream = new FileStream(imagePath, FileMode.Open);
            var _result = _app.PutProofDoc("driverlicense.jpg", imageStream, cid).Result;

            Assert.IsNotNull(_result, "Something was wrong with uploading proof docuemtns");
            return _result;
        }


        [TestMethod]
        public void TestStart()
        {
            var _customer = GetRandomProfile();

            //Create Profile
            var _contract = CreateProfile(_customer);
            Assert.IsNotNull(_contract, "Customer was not created");

            string citizenID = "072d0ac7-68c6-4864-bb0e-1881913cd684";

            //Upload Proof Document
            var _docproof = UploadProofDocucment(_contract.BusinessContractDTO.CitizenIdentifier);
            Assert.IsNotNull(_docproof, "Something was wrong with uploading proof docuemtns");

            //Validate FileHash
            var _isValid = ValidateProofDocHash(_contract.BusinessContractDTO.CitizenIdentifier, _docproof.Hash);
            Assert.IsTrue(_isValid, "Hash was not valid, File seems to be changed");

            //Check File URL 
            var _fileURL = getProofDocURL(_contract.BusinessContractDTO.CitizenIdentifier);
            Assert.IsNotNull(_fileURL, "URL Cannot be get");


            //Update Proof Document and Profile Completed
            _contract = UpdateProfile(_docproof, _contract.BusinessContractDTO.CitizenIdentifier);
            Assert.IsNotNull(_contract);
            Assert.IsTrue(_contract.BusinessContractDTO.Status == ProfileStatus._1, "Profile was not completed....");

            //Check Healthcare Eligibility first by ESC


            if (_contract.BusinessContractDTO.CitizenIdentifier == null) _contract.BusinessContractDTO.CitizenIdentifier = "072d0ac7-68c6-4864-bb0e-1881913cd684";
            //Assign Healthcare Plan
            HealthcareBC.ESC.HealthcarePlan carePlan = new HealthcarePlan()
            {
                AssignedDate = new DateTimeOffset(DateTime.Now),
                OwnerState = _contract.BusinessContractDTO.ActiveState,
                Plan = _contract.BusinessContractDTO.PreferredHealthcarePlan,
                CarePlanEligibilityStatus = HealthcarePlanCarePlanEligibilityStatus._0,
                ApprovedDate = new DateTimeOffset(),
                EnrolledDate = new DateTimeOffset(),
                Approved = false,
                EnrollmentBorkerAssigedState = null,
                PlanApprover = null,
                HealthcareStatus = HealthcarePlanHealthcareStatus._1
            };
            _contract = AssignHealthcarePlan(carePlan, _contract.BusinessContractDTO.CitizenIdentifier);
            Assert.IsNotNull(_contract.BusinessContractDTO.CurrentHealthcarePlan, "Healthcare Plan should be assigned");
            Assert.IsTrue(_contract.BusinessContractDTO.CurrentHealthcarePlan.HealthcareStatus == HealthcarePlanHealthcareStatus._1, "Healthcare plan status should be assigned as Assigned");

            //Assign Healthcare Eligibility(Mock up process...... it will be done bye ESC)
            //_contract.BusinessContractDTO.CurrentHealthcarePlan.CarePlanEligibilityStatus =
            //    HealthcarePlanCarePlanEligibilityStatus._1;
            //// ==> Indexing Transaction
            //IndexTransaction(_contract);

            //Approve HealthcarePlan
            _contract = ApproveHealthcarePlan(_contract.BusinessContractDTO.CitizenIdentifier);
            Assert.IsTrue(_contract.BusinessContractDTO.CurrentHealthcarePlan.HealthcareStatus == HealthcarePlanHealthcareStatus._2, "Healthcare plan status should be assienged as Approved");

            //Get Citizen Identifier
            _contract = GetCitizenProfile(_contract.BusinessContractDTO.CitizenIdentifier);
            Assert.IsNotNull(_contract);



            //Change State
            _contract = ChangeActiveState(_contract.BusinessContractDTO.CitizenIdentifier, "NJ");
            Assert.IsTrue(_contract.BusinessContractDTO.ActiveState == "NJ", "Active Stats should be NJ");
            Assert.IsTrue(_contract.BusinessContractDTO.Status == ProfileStatus._0, "Profile Status should be reset");


            //Get Transaction by entity id
            App _app = new App();
            var _transaction = _app.GetTransactionByEntityId(_contract.Id.Value.ToString()).Result;

            Assert.IsNotNull(_transaction);




            //Get profile completed Transactions 
            var _transactions = _app.GetCompletedCitizenProfiles("NY").Result;
            //Assert.IsTrue(_transactions.GetEnumerator().MoveNext());

            _transactions = _app.GetCitizenProfiles("NY").Result;
            Assert.IsTrue(_transactions.GetEnumerator().MoveNext());

            //Get profile incompleted Transactions
            _transactions = _app.GetInCompletedCitizenProfiles("NJ").Result;
            Assert.IsTrue(_transactions.GetEnumerator().MoveNext());

            //Get Transactions GetCitizenProfilesNotApprovedHealthcareplan
            _transactions = _app.GetCitizenProfilesNotApprovedHealthcareplan("NY").Result;
            //Assert.IsTrue(_transactions.GetEnumerator().MoveNext());

            //Get Transactions GetCitizenProfilesApprovedHealthcareplan
            _transactions = _app.GetCitizenProfilesApprovedHealthcareplan("NY").Result;
            //Assert.IsTrue(_transactions.GetEnumerator().MoveNext());

            //Get Transactions GetCitizenProfilesNotAssignedHealthcareEligibility
            _transactions = _app.GetCitizenProfilesNotAssignedHealthcareEligibility("NY").Result;
            //Assert.IsTrue(_transactions.GetEnumerator().MoveNext());
        }






        private void IndexTransaction(ContractTransaction transaction)
        {

            //Mapper.Reset();
            //Mapper.Initialize(cfg => cfg.CreateMap<HealthcareBC.ESC.ContractTransaction, HealthcareBC.Indexer.ContractTransaction>());

            //HealthcareBC.Indexer.ContractTransaction _tranaction =
            //    Mapper.Map<HealthcareBC.Indexer.ContractTransaction>(transaction);

            App _app = new App();
            _app.LogTransaction(transaction).GetAwaiter().GetResult();

        }





        private bool ValidateProofDocHash(string citizenIdentifier, string hash)
        {
            App _app = new App();
            return _app.ValidateHash(citizenIdentifier, hash).Result;
        }

        private string getProofDocURL(string citizenIdentifier)
        {
            App _app = new App();
            return _app.GetProofDocumentUrl(citizenIdentifier).Result;
        }

        private ContractTransaction UpdateProfile(HealthcareBC.ProofDocSvc.DocProof docProof, string citizenIdentifier)
        {

            App _app = new App();

            var result =  _app.UpdateProfileWithProofDocument(new DocProof()
            {
                Container = docProof.Container,
                ContentType = docProof.ContentType,
                FileName = docProof.FileName,
                Hash = docProof.Hash,
                StorageSharding = docProof.StorageSharding
            }, 
            citizenIdentifier).GetAwaiter().GetResult();

            return result;
        }

        private ContractTransaction AssignHealthcarePlan(HealthcarePlan carePlan, string citizenIdentifier)
        {

            App _app = new App();
            return _app.AssignHealthcarePlan(carePlan, citizenIdentifier).Result;
        }

        private ContractTransaction ApproveHealthcarePlan(string citizenIdentifier)
        {
            App _app = new App();
            return _app.ApproveHealthcarePlan(citizenIdentifier).Result;
        }

        private ContractTransaction GetCitizenProfile(string citizenIdentifier)
        {
            App _app = new App();
            return _app.GetCitizenProfile(citizenIdentifier).Result;
        }

        private ContractTransaction ChangeActiveState(string citizenIdentifier, string activeState)
        {
            App _app = new App();
            return _app.ChangeActiveState(citizenIdentifier, activeState).Result;
        }
    }
}

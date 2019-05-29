using Healthcare.BC.Offchain.Repository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Healthcare.BC.Chain.Fabric.Client;


namespace Healthcare.BC.Chain.Client.Test
{


    [TestClass]
    public class ChainClientTest
    {
        private FabricClient _client;
        private string ClientID;
        private BasicProfileItem MockProfile;
        private BasicProfileItem _mockCustomer;
        private string citizenIdentifier = "df8156b6-da8f-4f40-bfb9-f754d1ba1d37";

        class MockCustomer
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

        private BasicProfileItem GetRandomProfile()
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
                    zip = "10005-1234",
                    FedIncome = item.FedIncome,
                    StateIncome = item.StateIncome,
                    Dateofbirth = item.Dateofbirth

                };

                _mockupCustomer.Add(customer);
            }

            var _mockCustomer = _mockupCustomer[new Random().Next(0, 99)];

            return new BasicProfileItem()
            {
                Name = $"{_mockCustomer.first_name} {_mockCustomer.last_name}",
                Address = new Address() { City = _mockCustomer.City, Street = _mockCustomer.Street, State = _mockCustomer.State, Zip = _mockCustomer.zip },
                ApplicationType = ApplicationType.Indivisual,
                CitizenShip = true,
                DateOfBirth = _mockCustomer.Dateofbirth, //Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(_mockCustomer.Dateofbirth.ToUniversalTime()),
                FedIncome = _mockCustomer.FedIncome,
                StateIncome = _mockCustomer.StateIncome
            };
        }

        [TestInitialize]
        public void PrepareTest()
        {
            var _bindingID = "e3f5f543-9fe5-4e21-a6cd-f1f0b8ae3e3a";
            _client = new FabricClient();
            _mockCustomer = GetRandomProfile();
        }

        [TestMethod]
        public void End2EndTest()
        {
            CreateProfile();
            //UpdateProfile();
            //GetProfileInformation();
            //AssignHealthcarePlan();
            //ApproveHealthcarePlan();
            //ChangeActiveState();


            Assert.IsTrue(true, "Sanity check");
        }

        [TestMethod]
        public void CreateProfile()
        {
            var _cid = Guid.NewGuid().ToString();
            var _profile = new Profile()
            {
                BasicProfile = _mockCustomer,
                CitizenIdentifier = _cid,
                ActiveState = "NJ",
                CurrentHealthcarePlan = new HealthcarePlan(),
                StateApprover = " ",
                Status = ProfileStatus.ProfileCreated,
                PreferredHealthcarePlan = new HealthcarePlanItem() { Name = "my preferred plan - fake", Id = "0" },
                Description = @"description for citizen profile " + _cid
            };
            _profile.BasicProfile.FedIncome = 10000;
            _profile.BasicProfile.StateIncome = 10000;
            _profile.BasicProfile.Address.Zip = "07188";
            ContractTransaction result = _client.CreateProfile(_profile);
            if (result != null) citizenIdentifier = result.BusinessContractDTO.CitizenIdentifier;

            Assert.AreEqual(result.BusinessContractDTO.CitizenIdentifier, _profile.CitizenIdentifier, "CreateProfile Confirmation not returned");
        }

        //[TestMethod]
        //public void UpdateProfile()
        //{
        //    citizenIdentifier = "a4afc27c-d6d7-4d9d-8d22-fc6da7815e6d";

        //    var proofDoc = UploadProofDocucment(citizenIdentifier);

        //    ContractTransaction result =
        //        _client.UpdateProfile(citizenIdentifier, new DocProof()
        //        {
        //            Container = proofDoc.Container,
        //            ContentType = proofDoc.ContentType,
        //            FileName = proofDoc.FileName,
        //            Hash = proofDoc.Hash,
        //            StorageSharding = proofDoc.StorageSharding
        //        });
        //    Assert.AreEqual(result.BusinessContractDTO.IdentifyProofs.Hash, proofDoc.Hash, "UpdateProfile Confirmation not returned");
        //}
        //private HealthcareBC.ProofDocSvc.DocProof UploadProofDocucment(string cid)
        //{
        //    App _app = new App();
        //    string imagePath = @"..\..\..\drivelicense.jpg";
        //    FileStream imageStream = new FileStream(imagePath, FileMode.Open);
        //    var _result = _app.PutProofDoc("driverlicense.jpg", imageStream, cid).Result;

        //    Assert.IsNotNull(_result, "Something was wrong with uploading proof docuemtns");
        //    return _result;
        //}




        [TestMethod]
        public void GetProfileInformation()
        {
            if (citizenIdentifier == null) citizenIdentifier = "df8156b6-da8f-4f40-bfb9-f754d1ba1d37";

            Profile result = _client.GetProfileInformation(citizenIdentifier);
            Assert.AreEqual(result.CitizenIdentifier, citizenIdentifier, "BasicProfile not equal");
        }


        [TestMethod]
        public void AssignHealthcarePlan()
        {
            HealthcarePlanItem healthcarePlan = new HealthcarePlanItem
            {
                    Id = "id",
                    Name = "NY Healthcare Plan"
            };
            ContractTransaction result = _client.AssignHealthcarePlan(citizenIdentifier, healthcarePlan);
            Assert.AreEqual(result.BusinessContractDTO.CurrentHealthcarePlan.Plan.Name, healthcarePlan.Name, "AssignHealthcarePlan confirmation not returned");
        }


        [TestMethod]
        public void ApproveHealthcarePlan()
        {
            ContractTransaction result = _client.ApproveHealthcarePlan(citizenIdentifier, "Test Approver");
            Assert.AreEqual(result.BusinessContractDTO.CurrentHealthcarePlan.PlanApprover, "Test Approver", "ApproveHealthcarePlan confirmation not returned");
        }

        [TestMethod]
        public void ChangeActiveState()
        {
            ContractTransaction result = _client.ChangeActiveState(citizenIdentifier, "NJ");
            Assert.AreEqual(result.BusinessContractDTO.ActiveState, "NJ", "ChangeActiveState confirmation not returned");
        }
    }
}
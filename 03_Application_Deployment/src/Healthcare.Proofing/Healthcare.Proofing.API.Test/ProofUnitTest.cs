using Microsoft.VisualStudio.TestTools.UnitTesting;
using Healthcare.Proofing.Controllers;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Healthcare.Proofing.API.Test
{
    [TestClass]
    public class ProofUnitTest
    {
        private DocProofController proofController;
        private IFormFile file;
        private string bID = "0x0x";
        private string cID = "0x0x";

        [TestInitialize]
        public void PrepareTest()
        {
            //setup config
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            proofController = new DocProofController(config);

            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            file = fileMock.Object;
        }

        [TestMethod]
        public void PutProofDoc_Test()
        {

            var result = proofController.PutProofDoc(file, bID, cID);
            Assert.IsTrue(result.Value.Container == bID && result.Value.FileName == cID); ;

        }

        [TestMethod]
        public void GetProofDoc_Test()
        {
            var result = proofController.GetProofDoc(bID, cID);
            Assert.IsNotNull(result.Value);
        }

        [TestMethod]
        public void ValidateFileWithHash_Test()
        {
            var result = proofController.PutProofDoc(file, bID, cID);
            var hash = result.Value.Hash;

            var result1 = proofController.ValidateFileWithHash(bID, cID, hash);
            var result2 = proofController.ValidateFileWithHash(bID, cID, "");

            Assert.IsTrue(result1.Value && !result2.Value);
        }
    }
}

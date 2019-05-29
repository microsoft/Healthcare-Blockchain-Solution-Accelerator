using System;
using System.Collections.Generic;
using System.Text;
using Healthcare.BC.Offchain.Repository.ModelBase;

namespace Healthcare.BC.Offchain.Repository.Models
{
    public class ContractTransaction : IEntityModel<Guid>
    {
        public ContractTransaction()
        {
        }

        public ContractTransaction(Profile businessContract, ConstructorConfirmation contractDeployed)
        {
            InitObject(businessContract);
            ContractID = Guid.NewGuid().ToString();
            
            TransactionConfirmation = contractDeployed.TransactionConfirmation;
            Name = contractDeployed.Name;
            BindingId = contractDeployed.NewContractOrTokenId;
        }

        private void InitObject(Profile businessContract)
        {
            Id = Guid.NewGuid();
            TransactionID = businessContract.TransactionID;
            BusinessContractDTO = businessContract;
            TransactionTime = businessContract.TransactedTime;
        }

        public ContractTransaction(string bindingId, string name, Profile businessContract, TransactionConfirmation transactionConfirmation)
        {
            InitObject(businessContract);
            
            TransactionConfirmation = transactionConfirmation;
            BindingId = bindingId;
            Name = name;
        }

        //Key for Mongo......
        public Guid Id { get; set; }

        public bool IsActiveTransaction { get; set; }
        //Should be one per each deployed Smart Contract
        public string ContractID { get; set; }
        //Should be assigned per each transaction
        public string TransactionID { get; set; }
        //Business DTO
        public Profile BusinessContractDTO { get; set; }
        public DateTimeOffset TransactionTime { get; set; }
        public TransactionConfirmation TransactionConfirmation { get; set; }
        public string Name { get; set; }
        public string BindingId { get; set; }
        
    }
}

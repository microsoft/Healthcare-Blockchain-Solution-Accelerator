

using System;
using System.Diagnostics;

namespace Healthcare.BC.Offchain.Repository.Models
{
    public sealed class ConstructorConfirmation
    {
        public ConstructorConfirmation()
        {

        }
        
        public TransactionConfirmation TransactionConfirmation { get; set; }
        
        public string Name { get; set; }
        
        
        public string NewContractOrTokenId { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Healthcare.BC.Offchain.Repository.ModelBase
{
    /// <summary>
    /// After desiging Smart Contract, The Business DTO Entities should be added 
    /// </summary>
    public interface IBusinessEntity
    {
    //    string ContractID { get; set; }
     
        string TransactionID { get; set; }

        DateTime TransactedTime { get; set; }


        /// <summary>
        /// /have to implemented
        /// </summary>
        string Description { get; set; }

    }
}

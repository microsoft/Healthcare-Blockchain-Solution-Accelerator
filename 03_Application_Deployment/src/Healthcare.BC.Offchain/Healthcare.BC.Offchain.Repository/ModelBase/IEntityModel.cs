using System;
using System.Collections.Generic;
using System.Text;

namespace Healthcare.BC.Offchain.Repository.ModelBase
{
    public interface IEntityModel<TIdentifier>
    {
        TIdentifier Id { get; set; }
    }
}

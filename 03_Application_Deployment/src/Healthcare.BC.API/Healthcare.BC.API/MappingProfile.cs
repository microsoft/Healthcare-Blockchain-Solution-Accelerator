using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Healthcare.BC.Application.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<HealthcareBC.Fabric.DocProof, HealthcareBC.ProofDocSvc.DocProof>();
            CreateMap<List<HealthcareBC.Fabric.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>();
            CreateMap<HealthcareBC.Fabric.ContractTransaction, HealthcareBC.Tracker.ContractTransaction>();
            CreateMap<List<HealthcareBC.Tracker.ContractTransaction>, List<HealthcareBC.Fabric.ContractTransaction>>();
        
        }
    }
}

using AutoMapper;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;

namespace MedVisit.CatalogService.AutoMapper
{
    public class MedicalWorkerProfile : Profile
    {
        public MedicalWorkerProfile()
        {
            CreateMap<MedicalWorkerDb, MedicalWorkerDto>();
            CreateMap<MedicalWorkerDto, MedicalWorkerDb>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())   
                .ForMember(dest => dest.Updated, opt => opt.Ignore());
        }
    }
}

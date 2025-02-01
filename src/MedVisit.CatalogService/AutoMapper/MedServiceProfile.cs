using AutoMapper;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;

namespace MedVisit.CatalogService.AutoMapper
{
    public class MedServiceProfile : Profile
    {
        public MedServiceProfile()
        {
            CreateMap<MedServiceDb, MedServiceDto>()
                .ForMember(dest => dest.MedicalWorkerIds, opt => opt.MapFrom(src => src.MedicalWorkers.Select(mw => mw.Id).ToList()));

            CreateMap<MedServiceDb, MedServiceDetailsDto>();

            CreateMap<MedServiceDto, MedServiceDb>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.Updated, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
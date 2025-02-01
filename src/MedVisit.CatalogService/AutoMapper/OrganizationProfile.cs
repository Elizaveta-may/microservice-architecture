using AutoMapper;
using MedVisit.CatalogService.Entities;
using MedVisit.CatalogService.Models;

namespace MedVisit.CatalogService.AutoMapper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OrganizationDb, OrganizationDto>();
            CreateMap<OrganizationDto, OrganizationDb>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore());
        }
    }
}

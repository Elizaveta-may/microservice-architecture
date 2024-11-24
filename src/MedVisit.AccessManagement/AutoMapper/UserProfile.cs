using AutoMapper;
using MedVisit.AccessManagement.Models.User;
using MedVisit.Common.AuthDbContext.Entities;

namespace MedVisit.AccessManagement.AutoMapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDb, UserDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));

        CreateMap<UserDto, UserDb>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

    }
}
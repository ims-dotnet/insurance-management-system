using AutoMapper;
using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Models;

namespace InsureTrust.IdentityService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Notification, NotificationDto>();
    }
}

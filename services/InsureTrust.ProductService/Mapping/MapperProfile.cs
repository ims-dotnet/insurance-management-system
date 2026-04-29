using AutoMapper;
using InsureTrust.ProductService.DTOs;
using InsureTrust.ProductService.Models;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InsureTrust.ProductService.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreatePolicyTypeDto, PolicyType>();

            CreateMap<PolicyType, PolicyTypeDto>();

            CreateMap<PolicyTerm, PolicyTermDto>()
                .ReverseMap();

            CreateMap<PolicyRequiredField, PolicyRequiredFieldDto>()
                .ReverseMap();

            CreateMap<CreatePolicyDto, UserPolicy>()
                
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.PolicyNumber,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ExpiryDate,
                    opt => opt.Ignore());

            CreateMap<EditPolicyDto, UserPolicy>()
                .ForAllMembers(opts => opts.Condition(
                    (src, dest, srcMember) => srcMember != null));

            CreateMap<UserPolicy, PolicyDto>()
                .ForMember(dest => dest.PolicyTypeName,
                    opt => opt.MapFrom(src => src.PolicyType.Name))
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.PolicyType.Category))
                .ForMember(dest => dest.DaysLeft,
                    opt => opt.MapFrom(src =>
                        (src.ExpiryDate - DateTime.UtcNow).Days));

            CreateMap<ApprovePolicyDto, UserPolicy>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Action))
                .ForMember(dest => dest.AdminRemarks,
                    opt => opt.MapFrom(src => src.AdminRemarks));
        }
    }
}
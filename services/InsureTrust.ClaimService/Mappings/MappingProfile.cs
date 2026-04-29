using AutoMapper;
using InsureTrust.ClaimService.DTOs;
using InsureTrust.ClaimService.Models;

namespace InsureTrust.ClaimService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Claim, ClaimDto>()
                .ForMember(dest => dest.PolicyTypeName, opt => opt.Ignore()) // PolicyTypeName is fetched from another service
                .ForMember(dest => dest.DocumentUrls, opt => opt.MapFrom(src => src.DocumentPathsJson != null ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(src.DocumentPathsJson, new System.Text.Json.JsonSerializerOptions()) : new List<string>()));
        }
    }
}

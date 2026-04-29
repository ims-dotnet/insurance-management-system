using AutoMapper;
using InsureTrust.PaymentService.DTOs;
using InsureTrust.PaymentService.Models;

namespace InsureTrust.PaymentService.Mappings
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentDto>();

            CreateMap<Payment, InitiatePaymentResponseDto>()
                .ForMember(dest => dest.RedirectUrl,
                    opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.GeneratedUserPolicyId,
                    opt => opt.MapFrom(src => src.UserPolicyId));
        }
    }
}
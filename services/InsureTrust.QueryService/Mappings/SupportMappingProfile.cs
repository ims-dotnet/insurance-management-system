using AutoMapper;
using InsureTrust.SupportService.DTOs;
using InsureTrust.SupportService.Models;

namespace InsureTrust.SupportService.Mappings
{
    public class SupportMappingProfile : Profile
    {
        public SupportMappingProfile()
        {
            CreateMap<SupportQuery, SupportQueryDto>()
                .ForMember(dest => dest.AttachmentUrl,
                    opt => opt.MapFrom(src => src.AttachmentPath));

            CreateMap<CreateSupportQueryDto, SupportQuery>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TicketNumber, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore())
                .ForMember(dest => dest.AdminResponse, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ResolvedAt, opt => opt.Ignore());

            CreateMap<UpdateSupportStatusDto, SupportQuery>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TicketNumber, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Subject, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ResolvedAt, opt => opt.Ignore());
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace InsureTrust.SupportService.DTOs
{
    public class SendNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string ColorCode { get; set; } = string.Empty;

        public string Feature { get; set; } = string.Empty;
    }
}
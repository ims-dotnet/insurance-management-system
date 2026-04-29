using InsureTrust.IdentityService.DTOs;
using InsureTrust.IdentityService.Services;
using InsureTrust.IdentityService.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsureTrust.IdentityService.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetUserNotificationsAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<IEnumerable<NotificationDto>>("Invalid token."));

            var result = await _service.GetUserNotificationsAsync(userId);
            return Ok(new ApiResponse<IEnumerable<NotificationDto>>(result, "Notifications retrieved successfully."));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponse<UnreadCountResponseDto>>> GetUnreadCountAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<UnreadCountResponseDto>("Invalid token."));

            var count = await _service.GetUnreadCountAsync(userId);
            return Ok(new ApiResponse<UnreadCountResponseDto>(new UnreadCountResponseDto { Count = count }, "Unread count retrieved."));
        }

        [HttpPut("mark-read/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> MarkReadAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<string>("Invalid token."));

            await _service.MarkReadAsync(id, userId);
            return Ok(new ApiResponse<string>("Marked as read."));
        }

        [HttpPut("mark-all-read")]
        public async Task<ActionResult<ApiResponse<string>>> MarkAllReadAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ApiResponse<string>("Invalid token."));

            await _service.MarkAllReadAsync(userId);
            return Ok(new ApiResponse<string>("All marked as read."));
        }

        [HttpPost("send")]
        [AllowAnonymous] // Internal service call, would normally be protected by internal network or service-to-service auth
        public async Task<ActionResult<ApiResponse<string>>> SendAsync([FromBody] SendNotificationDto dto)
        {
            await _service.SendAsync(dto.UserId, dto.Title, dto.Message, dto.ColorCode, dto.Feature);
            return Ok(new ApiResponse<string>("Notification sent."));
        }
    }
}

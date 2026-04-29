using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InsureTrust.NotificationService.DTOs;
using InsureTrust.NotificationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.NotificationService.Controllers;

[Route("api/notifications")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMineAsync()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token.");
        return Ok(await _service.GetUserNotificationsAsync(userId));
    }

    [Authorize]
    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadCountResponseDto>> GetUnreadCountAsync()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token.");
        var count = await _service.GetUnreadCountAsync(userId);
        return Ok(new UnreadCountResponseDto { Count = count });
    }

    [Authorize]
    [HttpPut("mark-read/{id:int}")]
    public async Task<IActionResult> MarkReadAsync(int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token.");
        await _service.MarkReadAsync(id, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPut("mark-all-read")]
    public async Task<IActionResult> MarkAllReadAsync()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token.");
        await _service.MarkAllReadAsync(userId);
        return NoContent();
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendAsync([FromBody] SendNotificationDto dto)
    {
        await _service.SendAsync(dto.UserId, dto.Title, dto.Message, dto.ColorCode, dto.Feature);
        return Ok();
    }

    private bool TryGetUserId(out int userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out userId);
    }
}
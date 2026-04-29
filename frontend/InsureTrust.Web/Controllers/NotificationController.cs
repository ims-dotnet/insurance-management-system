using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InsureTrust.Web.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var notifications = await _notificationService.GetMyNotificationsAsync() ?? new List<NotificationDto>();
        return View(notifications);
    }

    [HttpGet]
    public async Task<IActionResult> UnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync();
        return Json(new { count });
    }

    [HttpPost]
    public async Task<IActionResult> MarkRead(int id)
    {
        var success = await _notificationService.MarkReadAsync(id);
        return Json(new { success });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllRead()
    {
        var success = await _notificationService.MarkAllReadAsync();
        return Json(new { success });
    }
}

using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers;

public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private string? GetToken() => Request.Cookies["authToken"];

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Notification") });
        }

        var notifications = await _notificationService.GetMyNotificationsAsync(token) ?? new List<NotificationDto>();
        return View(notifications);
    }

    [HttpGet]
    public async Task<IActionResult> UnreadCount()
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token)) return Unauthorized();

        var count = await _notificationService.GetUnreadCountAsync(token);
        return Json(new { count });
    }

    [HttpPost]
    public async Task<IActionResult> MarkRead(int id)
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token)) return Unauthorized();

        var success = await _notificationService.MarkReadAsync(id, token);
        return Json(new { success });
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllRead()
    {
        var token = GetToken();
        if (string.IsNullOrEmpty(token)) return Unauthorized();

        var success = await _notificationService.MarkAllReadAsync(token);
        return Json(new { success });
    }
}

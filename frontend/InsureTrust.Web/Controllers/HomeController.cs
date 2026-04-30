using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InsureTrust.Web.Controllers;

public class HomeController : Controller
{
    private readonly IPolicyService _policyService;
    private readonly INotificationService _notificationService;
    private readonly IClaimService _claimService;

    public HomeController(IPolicyService policyService, INotificationService notificationService, IClaimService claimService)
    {
        _policyService = policyService;
        _notificationService = notificationService;
        _claimService = claimService;
    }

    public async Task<IActionResult> Index()
    {
        var policies = await _policyService.GetAllPolicybyid();
        var notifications = await _notificationService.GetMyNotificationsAsync();
        var claims = await _claimService.GetMyClaimsAsync();
        
        ViewBag.ActivePolicies = policies?.Count(p => p.Status?.ToLower() == "active" || p.Status?.ToLower() == "approved") ?? 0;
        ViewBag.PendingClaims = claims?.Count(c => c.ClaimStatus?.ToLower() == "pending") ?? 0;
        ViewBag.NotificationCount = notifications?.Count(n => !n.IsRead) ?? 0;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

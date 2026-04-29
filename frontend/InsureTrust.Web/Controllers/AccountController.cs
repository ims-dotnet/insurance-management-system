using Microsoft.AspNetCore.Mvc;
using InsureTrust.Web.Models;
using InsureTrust.Web.Services;

namespace InsureTrust.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _authService.LoginAsync(model);

        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            if (model.RememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(7);
            }

            Response.Cookies.Append("authToken", response.Token, cookieOptions);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction(nameof(Profile));
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterAsync(model);
        if (result != null)
        {
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, "Registration failed. Please check the details.");
        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Profile()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ProfileData()
    {
        var profile = await _authService.GetProfileAsync();
        if (profile == null)
            return Json(new { success = false, message = "Could not load profile." });

        return Json(new { success = true, data = profile });
    }

    [HttpPost]
    public async Task<IActionResult> Profile(UpdateProfileViewModel model)
    {
        var result = await _authService.UpdateProfileAsync(model);
        if (result != null)
        {
            TempData["Success"] = "Profile updated successfully.";
        }
        else
        {
            TempData["Error"] = "Profile update failed. Please try again.";
        }

        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("authToken");
        return RedirectToAction(nameof(Login));
    }
}

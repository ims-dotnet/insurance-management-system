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
                HttpOnly = false, // Must be false if JS needs to read it (site.js seems to read it)
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            if (model.RememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(7);
            }

            Response.Cookies.Append("authToken", response.Token, cookieOptions);

            // NOTE: Currently JS handles userProfile, but we can set it in cookie or let JS fetch it.
            // Let's set it in a cookie that JS can read, or redirect and let JS loadProfile().

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
        return View(new RegisterRequestDto());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDto model)
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
        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
            return Json(new { success = false, message = "Not authenticated." });

        var profile = await _authService.GetProfileAsync(token);
        if (profile == null)
            return Json(new { success = false, message = "Could not load profile." });

        return Json(new { success = true, data = profile });
    }

    [HttpPost]
    public async Task<IActionResult> Profile(UpdateProfileRequestDto model)
    {
        var token = Request.Cookies["authToken"];
        if (string.IsNullOrEmpty(token))
        {
            TempData["Error"] = "You must be logged in to update your profile.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _authService.UpdateProfileAsync(model, token);
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

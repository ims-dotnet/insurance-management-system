using Microsoft.AspNetCore.Mvc;
using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace InsureTrust.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPolicyService _policyService;

    public AccountController(IAuthService authService, IPolicyService policyService)
    {
        _authService = authService;
        _policyService = policyService;
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

        // ── HARDCODED ADMIN CHECK ─────────────────────────────────────
        bool isHardcodedAdmin = (model.Email == "admin@insuretrust.com" && model.Password == "Admin@123") ||
                                (model.Email == "sahil@gmail.com" && model.Password == "Sahil@gmail.com");

        var response = await _authService.LoginAsync(model);

        // If backend fails but it's a hardcoded admin, we can still proceed for UI/Dev purposes 
        // OR if backend succeeds, we proceed as normal.
       if ((response != null && !string.IsNullOrEmpty(response.Token)) || isHardcodedAdmin)
        {
            var role = isHardcodedAdmin ? "Admin" : (response?.User?.Role ?? "Customer");
            var name = isHardcodedAdmin ? (model.Email.Contains("admin") ? "Admin User" : "Sahil Admin") : (response?.User?.Name ?? "User");
            var token = response?.Token ?? "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWFyb2ggR2F1ciIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGluc3VyZXRydXN0LmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwianRpIjoiY2ZjNjhkNDQtZWUzOC00YTk4LWI4NzgtYThlNDhiYjI4MzRhIiwiZXhwIjoxNzc3NzI3MDA1LCJpc3MiOiJJbnN1cmVUcnVzdC5JZGVudGl0eVNlcnZpY2UiLCJhdWQiOiJJbnN1cmVUcnVzdC5DbGllbnQifQ.ldZcIT2JWor4nkyTy24gf7fHrtBnvORwzaPT694J21k";

            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            if (model.RememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(7);
            }

            Response.Cookies.Append("authToken", token, cookieOptions);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, response?.User?.Id.ToString() ?? "1"),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim("Token", token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
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
            // ── AUTO-LOGIN AFTER REGISTRATION ──────────────────────────
            var loginModel = new LoginViewModel { Email = model.Email, Password = model.Password };
            var response = await _authService.LoginAsync(loginModel);
            
            if (response != null && !string.IsNullOrEmpty(response.Token))
            {
                var cookieOptions = new CookieOptions { HttpOnly = false, Secure = true, SameSite = SameSiteMode.Lax };
                Response.Cookies.Append("authToken", response.Token, cookieOptions);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, response.User?.Id.ToString() ?? "0"),
                    new Claim(ClaimTypes.Name, response.User?.Name ?? result.Name),
                    new Claim(ClaimTypes.Email, response.User?.Email ?? result.Email),
                    new Claim(ClaimTypes.Role, response.User?.Role ?? "Customer"),
                    new Claim("Token", response.Token)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            // ───────────────────────────────────────────────────────────────

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
    public async Task<IActionResult> Profile()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        var policies = await _policyService.GetAllPolicybyid();
        ViewBag.UserPolicies = policies ?? new List<PolicyDto>();
        return View(new UpdateProfileViewModel());
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
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("authToken");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}

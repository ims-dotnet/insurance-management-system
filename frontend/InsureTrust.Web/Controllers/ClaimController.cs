using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers;

public class ClaimController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (!Request.Cookies.ContainsKey("authToken"))
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Claim") });
        }

        return View("~/Views/Home/Index.cshtml");
    }
}

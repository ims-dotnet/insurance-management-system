using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers;

public class SupportController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (!Request.Cookies.ContainsKey("authToken"))
        {
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action(nameof(Index), "Support") });
        }

        return View("~/Views/Home/Index.cshtml");
    }
}

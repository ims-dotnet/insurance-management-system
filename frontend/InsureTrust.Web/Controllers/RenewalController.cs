namespace InsureTrust.Web.Controllers; 
public class RenewalController : Microsoft.AspNetCore.Mvc.Controller 
{ 
    [Microsoft.AspNetCore.Mvc.HttpGet]
    public Microsoft.AspNetCore.Mvc.IActionResult Index()
    {
        return View("~/Views/Home/Index.cshtml");
    }
}

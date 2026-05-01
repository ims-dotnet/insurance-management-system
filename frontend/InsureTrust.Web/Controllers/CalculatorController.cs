using InsureTrust.Web.Models;
using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers;

public class CalculatorController : Controller
{
    private readonly ICalculatorService _calculatorService;
    private readonly IPolicyService _policyService;

    public CalculatorController(ICalculatorService calculatorService, IPolicyService policyService)
    {
        _calculatorService = calculatorService;
        _policyService = policyService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetPolicyTypes()
    {
        var types = await _policyService.GetAllPolicyTypeAsync();
        return Json(new { success = true, data = types });
    }

    [HttpPost]
    public async Task<IActionResult> Estimate([FromBody] CalculatorViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _calculatorService.EstimateAsync(request);
        
        if (result != null)
        {
            return Json(new { success = true, data = result });
        }
        
        return Json(new { success = false, message = "Failed to calculate estimate." });
    }
}

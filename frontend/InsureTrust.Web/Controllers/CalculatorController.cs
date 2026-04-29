using InsureTrust.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.Web.Controllers;

public class CalculatorController : Controller
{
    private readonly ICalculatorService _calculatorService;

    public CalculatorController(ICalculatorService calculatorService)
    {
        _calculatorService = calculatorService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Estimate([FromBody] CalculatorRequestDto request)
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

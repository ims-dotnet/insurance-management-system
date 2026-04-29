using InsureTrust.CalculatorService.DTOs;
using InsureTrust.CalculatorService.Services;
using InsureTrust.CalculatorService.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace InsureTrust.CalculatorService.Controllers
{
    [Route("api/calculator")]
    [ApiController]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculatorService _calculatorService;

        public CalculatorController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        [HttpPost("estimate")]
        public ActionResult<ApiResponse<CalculatorResultDto>> Estimate([FromBody] CalculatorRequestDto dto)
        {
            var result = _calculatorService.Estimate(dto);
            return Ok(new ApiResponse<CalculatorResultDto>(result, "Estimation calculated successfully."));
        }
    }
}
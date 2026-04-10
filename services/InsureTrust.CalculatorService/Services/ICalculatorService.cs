using InsureTrust.CalculatorService.DTOs;

namespace InsureTrust.CalculatorService.Services
{
    public interface ICalculatorService
    {
        CalculatorResultDto Estimate(CalculatorRequestDto dto);
    }

}

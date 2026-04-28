using InsureTrust.CalculatorService.DTOs;
using InsureTrust.CalculatorService.Options;
using Microsoft.Extensions.Options;

namespace InsureTrust.CalculatorService.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly CalculationOptions _options;

        public CalculatorService(IOptions<CalculationOptions> options)
        {
            _options = options.Value;
        }

        public CalculatorResultDto Estimate(CalculatorRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var ageFactor = ResolveAgeFactor(dto.Age);
            var categoryFactor = ResolveCategoryFactor(dto.PolicyCategory);

            var estimatedPremium = dto.PackageAmount * ageFactor * categoryFactor;
            var totalInvestment = estimatedPremium * dto.Tenure;

            var tenureYears = dto.Tenure / 12m;
            var maturityAmount = totalInvestment * (1 + (_options.AnnualReturnRate * tenureYears));

            return new CalculatorResultDto
            {
                EstimatedPremium = decimal.Round(estimatedPremium, 2),
                TotalInvestment = decimal.Round(totalInvestment, 2),
                MaturityAmount = decimal.Round(maturityAmount, 2),
                Breakup =
                    $"Base Package={dto.PackageAmount:0.00}, AgeFactor={ageFactor:0.00}, " +
                    $"CategoryFactor={categoryFactor:0.00}, AnnualReturnRate={_options.AnnualReturnRate:P0}, " +
                    $"TenureMonths={dto.Tenure}"
            };
        }

        private decimal ResolveAgeFactor(int age)
        {
            if (age <= 30) return _options.AgeFactor18To30; // fixed name
            if (age <= 45) return _options.AgeFactor31To45;
            return _options.AgeFactor46To70;
        }

        private decimal ResolveCategoryFactor(string? category)
        {
            if (string.Equals(category, "Business", StringComparison.OrdinalIgnoreCase))
                return _options.BusinessCategoryFactor;

            return _options.PersonalCategoryFactor;
        }
    }
}
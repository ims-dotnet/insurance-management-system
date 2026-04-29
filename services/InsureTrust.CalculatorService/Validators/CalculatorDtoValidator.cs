using FluentValidation;
using InsureTrust.CalculatorService.DTOs;

namespace InsureTrust.CalculatorService.Validators;

public class CalculatorRequestDtoValidator : AbstractValidator<CalculatorRequestDto>
{
    public CalculatorRequestDtoValidator()
    {
        RuleFor(x => x.Age).InclusiveBetween(18, 70).WithMessage("Age must be between 18 and 70.");
        RuleFor(x => x.PackageAmount).InclusiveBetween(5000, 15000).WithMessage("Package amount must be between 5000 and 15000.");
        RuleFor(x => x.Tenure).InclusiveBetween(12, 36).WithMessage("Tenure must be between 12 and 36 months.");
    }
}

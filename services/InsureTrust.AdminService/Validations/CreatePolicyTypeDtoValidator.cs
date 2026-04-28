using FluentValidation;
using InsureTrust.AdminService.DTOs;

namespace InsureTrust.AdminService.Validations
{
    public class CreatePolicyTypeDtoValidator : AbstractValidator<CreatePolicyTypeDto>
    {
        public CreatePolicyTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Policy Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.BaseMonthlyPremium)
                .GreaterThan(0).WithMessage("Base Monthly Premium must be greater than zero.");

            RuleFor(x => x.MinTenureMonths)
                .GreaterThan(0).WithMessage("Minimum tenure must be at least 1 month.");

            RuleFor(x => x.MaxTenureMonths)
                .GreaterThanOrEqualTo(x => x.MinTenureMonths)
                .WithMessage("Maximum tenure cannot be less than minimum tenure.");
        }
    }
}

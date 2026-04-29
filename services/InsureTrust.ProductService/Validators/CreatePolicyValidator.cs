using FluentValidation;
using InsureTrust.ProductService.DTOs;

namespace InsureTrust.ProductService.Validators
{
    public class CreatePolicyValidator : AbstractValidator<CreatePolicyDto>
    {
        public CreatePolicyValidator()
        {
            RuleFor(x => x.PolicyTypeId)
                .GreaterThan(0)
                .WithMessage("Policy type is required");

            RuleFor(x => x.Tenure)
                .InclusiveBetween(12, 36)
                .WithMessage("Tenure must be between 12 and 36 months");

            RuleFor(x => x.PackageAmount)
                .InclusiveBetween(5000, 15000)
                .WithMessage("Amount must be between ₹5000 and ₹15000");
        }
    }
}
using FluentValidation;
using InsureTrust.ProductService.DTOs;

namespace InsureTrust.ProductService.Validators
{
    public class CreatePolicyTypeValidator : AbstractValidator<CreatePolicyTypeDto>
    {
        public CreatePolicyTypeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Policy name is required")
                .MaximumLength(100);

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MinimumLength(10);

            RuleFor(x => x.Icon)
                .NotEmpty().WithMessage("Icon is required");
        }
    }
}
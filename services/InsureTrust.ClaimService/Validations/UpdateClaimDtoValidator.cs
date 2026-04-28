using FluentValidation;
using InsureTrust.ClaimService.DTOs;

namespace InsureTrust.ClaimService.Validations
{
    public class UpdateClaimDtoValidator : AbstractValidator<UpdateClaimDto>
    {
        public UpdateClaimDtoValidator()
        {
            RuleFor(x => x.Action)
                .NotEmpty().WithMessage("Action is required.")
                .Must(action => action == "Approve" || action == "Deny")
                .WithMessage("Action must be either 'Approve' or 'Deny'.");

            RuleFor(x => x.AdminRemarks)
                .MaximumLength(500).WithMessage("Admin remarks cannot exceed 500 characters.");
        }
    }
}

using FluentValidation;
using InsureTrust.SupportService.DTOs;

namespace InsureTrust.SupportService.Validators
{
    public class UpdateSupportStatusDtoValidator : AbstractValidator<UpdateSupportStatusDto>
    {
        private static readonly string[] ValidStatuses = { "Pending", "InProgress", "Resolved" };

        public UpdateSupportStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(status => ValidStatuses.Contains(status))
                .WithMessage("Invalid status. Allowed values: Pending, InProgress, Resolved.");

            RuleFor(x => x.AdminResponse)
                .MaximumLength(1000)
                .WithMessage("Admin response must not exceed 1000 characters.")
                .When(x => x.AdminResponse != null);
        }
    }
}

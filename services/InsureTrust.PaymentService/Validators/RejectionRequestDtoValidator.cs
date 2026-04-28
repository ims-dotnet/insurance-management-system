using FluentValidation;
using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Validators
{
    public class RejectionRequestDtoValidator : AbstractValidator<RejectionRequestDto>
    {
        public RejectionRequestDtoValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Rejection reason is required.")
                .MinimumLength(10)
                .WithMessage("Please provide a more detailed reason (at least 10 characters).")
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}

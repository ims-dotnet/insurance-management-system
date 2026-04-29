using FluentValidation;
using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Validators
{
    public class InitiateRenewalPaymentDtoValidator : AbstractValidator<InitiateRenewalPaymentDto>
    {
        public InitiateRenewalPaymentDtoValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .Matches(@"^POL-\d+$|POL[0-9]{4}|POL-DEMO") // Allowing current mock formats
                .WithMessage("Valid PolicyNumber is required.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .MaximumLength(50)
                .Must(method => new[]
                {
                    "UPI", "GPay", "PhonePe", "Paytm",
                    "DebitCard", "CreditCard", "NetBanking", "PayPal"
                }.Contains(method))
                .WithMessage("Invalid payment method selected.");
        }
    }
}
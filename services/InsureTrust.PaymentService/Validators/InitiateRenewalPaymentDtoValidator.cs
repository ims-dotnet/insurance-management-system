using FluentValidation;
using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Validators
{
    public class InitiateRenewalPaymentDtoValidator : AbstractValidator<InitiateRenewalPaymentDto>
    {
        public InitiateRenewalPaymentDtoValidator()
        {
            RuleFor(x => x.UserPolicyId)
                .GreaterThan(0)
                .WithMessage("Valid UserPolicyId is required.");

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
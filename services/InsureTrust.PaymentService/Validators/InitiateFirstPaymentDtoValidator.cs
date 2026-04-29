using FluentValidation;
using InsureTrust.PaymentService.DTOs;

namespace InsureTrust.PaymentService.Validators
{
    public class InitiateFirstPaymentDtoValidator : AbstractValidator<InitiateFirstPaymentDto>
    {
        public InitiateFirstPaymentDtoValidator()
        {
            RuleFor(x => x.PolicyId)
                .GreaterThan(0)
                .WithMessage("Valid PolicyId is required.");

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
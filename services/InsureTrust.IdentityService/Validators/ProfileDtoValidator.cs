using FluentValidation;
using InsureTrust.IdentityService.DTOs;

namespace InsureTrust.IdentityService.Validators;

public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.Name).MinimumLength(3).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.PhoneNo).MaximumLength(15).Matches(@"^\d+$").WithMessage("Invalid phone number.").When(x => !string.IsNullOrEmpty(x.PhoneNo));
    }
}

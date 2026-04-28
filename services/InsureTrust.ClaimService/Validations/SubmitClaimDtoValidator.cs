using FluentValidation;
using InsureTrust.ClaimService.DTOs;
using Microsoft.AspNetCore.Http;

namespace InsureTrust.ClaimService.Validations
{
    public class SubmitClaimDtoValidator : AbstractValidator<SubmitClaimDto>
    {
        public SubmitClaimDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.MaturityAmount)
                .GreaterThan(0).WithMessage("Maturity amount must be greater than zero.");

            RuleFor(x => x.Documents)
                .Must(docs => docs == null || docs.Count <= 12)
                .WithMessage("You cannot upload more than 12 documents.")
                .ForEach(docRule => 
                {
                    docRule.Must(BeAValidFileSize).WithMessage("Each document must be 5MB or less.");
                    docRule.Must(BeAValidExtension).WithMessage("Document must be a .pdf, .jpg, .jpeg, or .png file.");
                });
        }

        private bool BeAValidFileSize(IFormFile file)
        {
            if (file == null) return true;
            return file.Length > 0 && file.Length <= 5 * 1024 * 1024; // 5 MB
        }

        private bool BeAValidExtension(IFormFile file)
        {
            if (file == null) return true;
            var ext = Path.GetExtension(file.FileName).ToLower();
            return ext == ".pdf" || ext == ".jpg" || ext == ".jpeg" || ext == ".png";
        }
    }
}

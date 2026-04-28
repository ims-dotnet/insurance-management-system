using FluentValidation;
using InsureTrust.SupportService.DTOs;

namespace InsureTrust.SupportService.Validators
{
    public class CreateSupportQueryDtoValidator : AbstractValidator<CreateSupportQueryDto>
    {
        private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".pdf" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public CreateSupportQueryDtoValidator()
        {
            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage("Subject is required.")
                .MaximumLength(200)
                .WithMessage("Subject must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(2000)
                .WithMessage("Description must not exceed 2000 characters.");

            When(x => x.Attachment != null, () =>
            {
                RuleFor(x => x.Attachment!)
                    .Must(file =>
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        return AllowedExtensions.Contains(ext);
                    })
                    .WithMessage($"Attachment must be one of: {string.Join(", ", AllowedExtensions)}")
                    .Must(file => file.Length <= MaxFileSizeBytes)
                    .WithMessage("Attachment must be less than 5 MB.");
            });
        }
    }
}

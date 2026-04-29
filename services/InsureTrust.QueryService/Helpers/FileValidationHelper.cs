
using InsureTrust.SupportService.Exceptions;

namespace InsureTrust.SupportService.Helpers
{
    public static class FileValidationHelper
    {
        private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".pdf" };
        private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public static void Validate(IFormFile? file, string fieldName)
        {
            if (file == null)
                return;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException($"{fieldName} must be one of: {string.Join(", ", AllowedExtensions)}");

            if (file.Length > MaxFileSizeBytes)
                throw new BadRequestException($"{fieldName} must be less than 5 MB");
        }

        public static async Task<string?> SaveFileAsync(IFormFile? file, string webRootPath, string subFolder)
        {
            if (file == null)
                return null;

            var folderPath = Path.Combine(webRootPath, "uploads", subFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{subFolder}/{fileName}";
        }
    }
}
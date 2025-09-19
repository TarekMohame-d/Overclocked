using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Validators;

public static class FileValidator
{
    public static IRuleBuilderOptions<T, IFormFile> ValidateImageFile<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
    {
        return ruleBuilder
            .Must(file => file.Length > 0).WithMessage("No file uploaded.")
            .Must(file =>
            {
                var allowedExts = new[] { ".jpg", ".jpeg", ".png" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExts.Contains(ext);
            }).WithMessage("Allowed extensions: jpg, jpeg, png.")
            .Must(file => file.Length <= 2 * 1024 * 1024).WithMessage("Max file size allowed is 2 MB.");
    }
}

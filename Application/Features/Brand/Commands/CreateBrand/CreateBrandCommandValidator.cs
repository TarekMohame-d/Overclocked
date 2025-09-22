using Application.Common.Validators;
using Domain.Repositories;
using FluentValidation;

namespace Application.Features.Brand.Commands.CreateBrand;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    private readonly IBrandRepository _brandRepository;
    public CreateBrandCommandValidator(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
            .MustAsync(async (name, cancellation) =>
            {
                bool exists = await _brandRepository
                    .AnyAsync(x => x.NormalizedName == name.ToUpper(), cancellation);
                return !exists;
            }).WithMessage("{PropertyName} already exists.");

        RuleFor(x => x.ImageFile)
            .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(x => x.ImageFile)
            .ValidateImageFile()
            .When(x => x.ImageFile is not null);
    }
}

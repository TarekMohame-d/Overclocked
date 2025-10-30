using Application.Abstraction.Repositories;
using Application.Services.Tag.DTOs.Request;
using FluentValidation;

namespace Application.Services.Tag.Validations;

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    private readonly ITagRepository _tagRepository;
    public CreateTagRequestValidator(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellation) =>
            {
                bool exists = await _tagRepository
                    .AnyAsync(x => x.NormalizedName == name.ToUpper(), cancellation);
                return !exists;
            })
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("{PropertyName} already exists.");
    }
}

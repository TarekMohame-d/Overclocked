using FluentValidation;
using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Tag.Commands.CreateTag;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    private readonly ITagRepository _tagRepository;
    public CreateTagCommandValidator(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .MustAsync(async (name, cancellation) =>
            {
                var exists = await _tagRepository.AnyAsync(x => x.NormalizedName == name.ToUpper(), cancellation);
                return !exists;
            })
            .WithMessage("{PropertyName} already exists.");
    }
}

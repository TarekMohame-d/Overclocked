using FluentValidation;

namespace Overclocked.Application.Tag.Commands.UpdateTag;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");
    }
}

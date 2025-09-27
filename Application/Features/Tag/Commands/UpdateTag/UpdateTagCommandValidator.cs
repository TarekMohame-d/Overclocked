using Domain.Repositories;
using FluentValidation;

namespace Application.Features.Tag.Commands.UpdateTag;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagWithIdCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
    }
}

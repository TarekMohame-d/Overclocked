using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Commands.Decorators;

public class ValidatingTagCommandsDecorator(ITagCommands inner,
        IValidator<CreateTagCommand> createValidator,
        IValidator<UpdateTagCommand> updateValidator) : ITagCommands
{
    public async Task<Result> CreateTagCommandHandler(CreateTagCommand command, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await createValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<CreateTagCommand>(errorDictionary);
        }

        Result result = await inner.CreateTagCommandHandler(command, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateTagCommandHandler(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await updateValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<UpdateTagCommand>(errorDictionary);
        }

        Result result = await inner.UpdateTagCommandHandler(command, cancellationToken);

        return result;
    }

    public async Task<Result> DeleteTagCommandHandler(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteTagCommandHandler(command, cancellationToken);

        return result;
    }
}

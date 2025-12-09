using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Commands.Decorators;

public class ValidatingCategoryCommandsDecorator(ICategoryCommands inner,
        IValidator<CreateCategoryCommand> createValidator,
        IValidator<UpdateCategoryCommand> updateValidator) : ICategoryCommands
{
    public async Task<Result> CreateCategoryCommandHandler(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await createValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<CreateCategoryCommand>(errorDictionary);
        }

        Result result = await inner.CreateCategoryCommandHandler(command, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateCategoryCommandHandler(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await updateValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<UpdateCategoryCommand>(errorDictionary);
        }

        Result result = await inner.UpdateCategoryCommandHandler(command, cancellationToken);

        return result;
    }

    public async Task<Result> DeleteCategoryCommandHandler(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteCategoryCommandHandler(command, cancellationToken);

        return result;
    }
}

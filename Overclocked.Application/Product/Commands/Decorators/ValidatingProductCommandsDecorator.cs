using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Commands.Decorators;

public class ValidatingProductCommandsDecorator(IProductCommands inner,
        IValidator<CreateProductCommand> createValidator) : IProductCommands
{
    public async Task<Result> CreateProductCommandHandler(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await createValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<CreateProductCommand>(errorDictionary);
        }

        Result result = await inner.CreateProductCommandHandler(command, cancellationToken);

        return result;
    }
}

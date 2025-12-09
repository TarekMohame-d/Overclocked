using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Commands.Decorators;

public class ValidatingBrandCommandsDecorator(IBrandCommands inner,
        IValidator<CreateBrandCommand> createValidator,
        IValidator<UpdateBrandCommand> updateValidator) : IBrandCommands
{
    public async Task<Result> CreateBrandCommandHandler(CreateBrandCommand command, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await createValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<CreateBrandCommand>(errorDictionary);
        }

        Result result = await inner.CreateBrandCommandHandler(command, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateBrandCommandHandler(UpdateBrandCommand command, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await updateValidator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<UpdateBrandCommand>(errorDictionary);
        }

        Result result = await inner.UpdateBrandCommandHandler(command, cancellationToken);

        return result;
    }

    public async Task<Result> DeleteBrandCommandHandler(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteBrandCommandHandler(command, cancellationToken);

        return result;
    }
}

using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if(validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            Dictionary<string, string[]> errors = GroupValidationErrors(validationFailures);

            return Result<TResponse>.ValidationFailure(errors);
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(command, validators);

            if(validationFailures.Length == 0)
            {
                return await innerHandler.Handle(command, cancellationToken);
            }

            Dictionary<string, string[]> errors = GroupValidationErrors(validationFailures);

            return Result.ValidationFailure(errors);
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(query, validators);

            if(validationFailures.Length == 0)
            {
                return await innerHandler.Handle(query, cancellationToken);
            }

            Dictionary<string, string[]> errors = GroupValidationErrors(validationFailures);

            return Result<TResponse>.ValidationFailure(errors);
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<T>(
        T instance,
        IEnumerable<IValidator<T>> validators)
    {
        if(!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<T>(instance);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        return validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();
    }

    private static Dictionary<string, string[]> GroupValidationErrors(ValidationFailure[] validationFailures)
    {
        return validationFailures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.ErrorMessage).ToArray());
    }
}

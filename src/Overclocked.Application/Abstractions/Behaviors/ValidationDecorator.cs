using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class RequestHandler<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> innerHandler,
        IEnumerable<IValidator<TRequest>> validators
    ) : IRequestHandler<TRequest, TResponse>, IDecorator
        where TRequest : IRequest<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken ct)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(request, validators);

            if (validationFailures.Length == 0)
                return await innerHandler.Handle(request, ct);

            Dictionary<string, string[]> errors = GroupValidationErrors(validationFailures);

            return Result<TResponse>.ValidationFailure(errors);
        }
    }

    internal sealed class RequestHandler<TRequest>(
        IRequestHandler<TRequest> innerHandler,
        IEnumerable<IValidator<TRequest>> validators
    ) : IRequestHandler<TRequest>, IDecorator
        where TRequest : IRequest
    {
        public async Task<Result> Handle(TRequest request, CancellationToken ct)
        {
            ValidationFailure[] validationFailures = await ValidateAsync(request, validators);

            if (validationFailures.Length == 0)
                return await innerHandler.Handle(request, ct);

            Dictionary<string, string[]> errors = GroupValidationErrors(validationFailures);

            return Result.ValidationFailure(errors);
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<T>(T instance, IEnumerable<IValidator<T>> validators)
    {
        if (!validators.Any())
            return [];

        var context = new ValidationContext<T>(instance);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context))
        );

        return validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();
    }

    private static Dictionary<string, string[]> GroupValidationErrors(ValidationFailure[] validationFailures) =>
        validationFailures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(e => e.ErrorMessage).ToArray());
}

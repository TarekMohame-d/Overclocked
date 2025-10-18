using Application.Abstraction.Messaging;
using Application.Common.Results;
using FluentValidation;
using FluentValidation.Results;
using System.Net;

namespace Application.Abstraction.Behaviors;

public sealed class ValidationalPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IValidationalRequest
    where TResponse : Result
{

    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationalPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ValidationFailure[] validationFailures = await ValidateAsync(request, cancellationToken);

        if (validationFailures.Length == 0)
            return await next(cancellationToken);

        var errors = validationFailures.GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var error = new Error(
                typeof(TRequest).Name,
                ErrorType.Validation,
                "Validation error",
                errors
            );

        // Create the failure result by invoking the static 'Failure' method on the TResponse type.
        // This works correctly for both Result and Result<T>.
        var failureResult = typeof(TResponse)
            .GetMethod(nameof(Result.Failure), new[] { typeof(Error), typeof(HttpStatusCode) })?
            .Invoke(null, new object[] { error, HttpStatusCode.BadRequest });

        return (TResponse)failureResult!;
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return [];

        var context = new ValidationContext<TRequest>(request);

        ValidationResult[] validationResults = await Task.WhenAll(
            _validators
                .Select(v => v.ValidateAsync(context, cancellationToken)));

        ValidationFailure[] validationFailures = validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .ToArray();

        return validationFailures;
    }
}

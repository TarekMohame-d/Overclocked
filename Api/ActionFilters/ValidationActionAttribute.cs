using Api.Extensions;
using Application.Common.Results;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.ActionFilters;

public class ValidationActionAttribute<T>(IValidator<T>? validator) : IAsyncActionFilter
    where T : class
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // If no validator is registered for this model, skip validation
        if (validator is null)
        {
            await next();
            return;
        }

        // Try to extract the request model (the action parameter matching type T)
        var model = context.ActionArguments.Values.FirstOrDefault(v => v is T) as T;
        if (model is null)
        {
            await next();
            return;
        }

        // Try to extract CancellationToken if action has it as a parameter
        CancellationToken cancellationToken =
            context.ActionArguments.Values.OfType<CancellationToken>().FirstOrDefault();

        // Perform validation
        ValidationResult? validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (validationResult.IsValid)
        {
            await next();
            return;
        }

        // Convert FluentValidation errors to dictionary
        var errorDictionary = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        // Wrap errors into your custom Result type
        var result = Result.ValidationError<T>(errorDictionary);

        // Set HTTP result directly (short-circuit pipeline)
        context.Result = result.ToActionResult();
    }
}

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
        if(!context.ModelState.IsValid)
        {
            var errorDictionary = context.ModelState
                .Where(kv => kv.Value?.Errors.Count > 0 && kv.Key.StartsWith("$."))
                .ToDictionary(
                kv => kv.Key.Replace("$.", ""),
                kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            context.Result = Result.ValidationError<T>(errorDictionary).ToActionResult();
            return;
        }

        if(context.ActionArguments.Values.FirstOrDefault(v => v is T) is not T model)
        {
            await next();
            return;
        }

        if(validator is not null)
        {
            ValidationResult validationResult = await validator.ValidateAsync(model);

            if(!validationResult.IsValid)
            {
                var errorDictionary = validationResult
                    .Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                context.Result = Result.ValidationError<T>(errorDictionary).ToActionResult();
                return;
            }
        }

        await next();
    }
}

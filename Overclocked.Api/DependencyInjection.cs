using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Overclocked.Api.Infrastructure;

namespace Overclocked.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                // options.Filters.Add<LoggerActionFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // To disable the default invalid model state response
                // options.SuppressModelStateInvalidFilter = true;
            });

        services.AddExceptionHandling();

        // Add openapi services
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();

        services.AddHttpContextAccessor();
        // services.AddScoped(typeof(ValidationActionAttribute<>));

        return services;
    }

    private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}

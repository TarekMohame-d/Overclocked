using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Application.Abstractions.Behaviors;
using Overclocked.Application.Abstractions.Factories;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Overclocked.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly, lifetime: ServiceLifetime.Scoped, includeInternalTypes: true);

        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.Decorate(typeof(IRequestHandler<,>), typeof(CachingDecorator.RequestHandler<,>));
        services.Decorate(typeof(IRequestHandler<,>), typeof(ValidationDecorator.RequestHandler<,>));
        services.Decorate(typeof(IRequestHandler<,>), typeof(LoggingDecorator.RequestHandler<,>));

        services.Decorate(typeof(IRequestHandler<>), typeof(CachingDecorator.RequestHandler<>));
        services.Decorate(typeof(IRequestHandler<>), typeof(ValidationDecorator.RequestHandler<>));
        services.Decorate(typeof(IRequestHandler<>), typeof(LoggingDecorator.RequestHandler<>));

        services.AddApplicationResilience();

        services.AddScoped<PaymentFactory>();

        return services;
    }

    private static IServiceCollection AddApplicationResilience(this IServiceCollection services) =>
        services.AddResiliencePipeline(
            ResilienceConstants.StandardPolicy,
            builder =>
            {
                builder.AddRetry(
                    new RetryStrategyOptions
                    {
                        ShouldHandle = new PredicateBuilder().Handle<DbUpdateConcurrencyException>(),

                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromMilliseconds(50),
                        BackoffType = DelayBackoffType.Exponential,
                    }
                );

                builder.AddCircuitBreaker(
                    new CircuitBreakerStrategyOptions
                    {
                        // If 50% of requests fail...
                        FailureRatio = 0.5,

                        // ...within a 30-second window...
                        SamplingDuration = TimeSpan.FromSeconds(30),

                        // ...and we have attempted at least 7 requests...
                        MinimumThroughput = 7,

                        // ...then stop all requests for 15 seconds.
                        BreakDuration = TimeSpan.FromSeconds(15),

                        // Handle DB Concurrency, but also generic DB Exceptions (timeouts, connection issues)
                        ShouldHandle = new PredicateBuilder()
                            .Handle<DbUpdateConcurrencyException>()
                            .Handle<DbUpdateException>() // Catch general EF errors
                            .Handle<TimeoutException>(),
                    }
                );
            }
        );
}

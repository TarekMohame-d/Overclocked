using System.Diagnostics;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Overclocked.Api.Extensions;
using Overclocked.Api.Infrastructure;
using Overclocked.Application.Common.Configurations;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Authorization;

namespace Overclocked.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
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

        services.AddAuth(configuration);

        services.AddRatelimiting(configuration);

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSettings jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    options.DefaultChallengeScheme =
                    options.DefaultForbidScheme =
                    options.DefaultScheme =
                    options.DefaultSignInScheme =
                    options.DefaultSignOutScheme =
                        JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();

        AuthorizationBuilder builder = services.AddAuthorizationBuilder();
        foreach (Permission permission in Enum.GetValues<Permission>())
        {
            var permissionName = permission.ToString();

            builder.AddPolicy(permissionName, policy => policy.Requirements.Add(new PermissionRequirement(permissionName)));
        }

        return services;
    }

    private static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    private static IServiceCollection AddRatelimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var isEnabled = configuration.GetValue("RateLimiting:Enabled", true);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            if (!isEnabled)
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions { PermitLimit = 1000, Window = TimeSpan.FromMinutes(1) }
                    )
                );

                return;
            }

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 1000, Window = TimeSpan.FromMinutes(1) }
                )
            );

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter);
                var seconds = retryAfter.TotalSeconds > 0 ? (int)retryAfter.TotalSeconds : 30;

                context.HttpContext.Response.Headers.RetryAfter = seconds.ToString();

                ProblemDetailsFactory problemDetailsFactory =
                    context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                    context.HttpContext,
                    StatusCodes.Status429TooManyRequests,
                    "Too Many Requests",
                    detail: $"Quota exceeded. Please try again in {seconds} seconds."
                );

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };

            options.AddFixedWindowLimiter(
                "fixed",
                cfg =>
                {
                    cfg.Window = TimeSpan.FromMinutes(1);
                    cfg.PermitLimit = 60;
                    cfg.QueueLimit = 0;
                }
            );

            options.AddPolicy(
                "per-user",
                httpContext =>
                {
                    Guid? userId = httpContext.GetUserId();
                    if (userId is not null)
                    {
                        return RateLimitPartition.GetTokenBucketLimiter(
                            userId.ToString()!,
                            _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 60,
                                TokensPerPeriod = 10,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                            }
                        );
                    }

                    return RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 60,
                            QueueLimit = 0,
                        }
                    );
                }
            );
        });

        return services;
    }
}

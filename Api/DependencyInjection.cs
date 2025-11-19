using System.Text;
using Api.ActionFilters;
using Api.Infrastructure;
using Domain.Configurations;
using Domain.StaticData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Api;

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
                options.SuppressModelStateInvalidFilter = true;
            });

        // Bind configuration sections
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        services.AddAuthenticationAndAuthorization(configuration);
        services.AddExceptionHandling();

        // Add openapi services
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();

        services.AddHttpContextAccessor();
        services.AddScoped(typeof(ValidationActionAttribute<>));

        return services;
    }

    private static IServiceCollection AddAuthenticationAndAuthorization(
        this IServiceCollection services,
        IConfiguration configuration
    )
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
        foreach(PermissionType permission in Enum.GetValues<PermissionType>())
        {
            var permissionName = permission.ToString();

            builder.AddPolicy(
                permissionName,
                policy => policy.Requirements.Add(new PermissionRequirement(permissionName))
            );
        }

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
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}

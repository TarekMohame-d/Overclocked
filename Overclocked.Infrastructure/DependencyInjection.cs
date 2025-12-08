using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.Authorization;
using Overclocked.Infrastructure.Configurations;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Infrastructure.Persistence.Repositories;
using Overclocked.Infrastructure.Services;
using StackExchange.Redis;

namespace Overclocked.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Overclocked";
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        // Bind configuration sections
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();

        services
            .AddPersistence(configuration)
            .AddServices()
            .AddAuth(configuration)
            .AddBackgroundJobs(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InsertOutboxMessagesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>());
        });

        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

        services.Scan(scan =>
            scan.FromAssemblyOf<BrandRepository>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IEmailConfirmationCodeService, EmailConfirmationCodeService>();
        services.AddSingleton<IRefreshTokenHasher, RefreshTokenHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenReaderService, TokenReaderService>();

        services.AddScoped<IFileStorageService, CloudFileStorageService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(
                options =>
                    options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")),
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire",
                    // optionally tune other settings:
                    // QueuePollInterval = TimeSpan.FromSeconds(15),
                    // InvisibilityTimeout = TimeSpan.FromMinutes(30),
                    // TablePrefix = "hf_",
                });
        });
        services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

        return services;
    }

    private static IServiceCollection AddAuth(
    this IServiceCollection services,
    IConfiguration configuration)
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
                policy => policy.Requirements.Add(new PermissionRequirement(permissionName)));
        }

        return services;
    }
}

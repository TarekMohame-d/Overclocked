using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Configurations;
using Overclocked.Infrastructure.Authentication;
using Overclocked.Infrastructure.BackgroundJobs;
using Overclocked.Infrastructure.Outbox;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Infrastructure.Persistence.Repositories;
using Overclocked.Infrastructure.Services;
using Overclocked.Infrastructure.Services.PaymentService;
using Overclocked.Infrastructure.Services.PaymentService.Strategies.Paymob;
using Overclocked.SharedKernel.Primitives;
using StackExchange.Redis;

namespace Overclocked.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            IConfiguration config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(connectionString!);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Overclocked:";
        });

        // Bind configuration sections
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
        services.Configure<PaymobSettings>(configuration.GetSection(PaymobSettings.SectionName));

        services.AddHttpClient("PaymobClient");

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.Scan(selector =>
            selector
                .FromAssemblyOf<IDomainEventDispatcher>()
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddScoped<IProcessOutboxMessagesJob, ProcessOutboxMessagesJob>();

        services.AddPersistence(configuration).AddServices().AddBackgroundJobs(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InsertOutboxMessagesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                options
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>())
                    .UseSnakeCaseNamingConvention();
            }
        );

        services.Scan(scan =>
            scan.FromAssemblyOf<BrandRepository>()
                .AddClasses(classes => classes.AssignableTo<IRepository>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDispatcher, NetCoreDispatcher>();

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

        services.AddScoped<IPaymentService, PaymentService>();

        services.AddScoped<IPaymentProviderService, PaymobPaymentProviderService>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")),
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire",

                    // optionally tune other settings:
                    // QueuePollInterval = TimeSpan.FromSeconds(15),
                    // InvisibilityTimeout = TimeSpan.FromMinutes(30),
                    // TablePrefix = "hf_",
                }
            );
        });
        services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

        return services;
    }
}

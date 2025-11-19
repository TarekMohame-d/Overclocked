using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "Overclocked";
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!)
        );

        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IEmailConfirmationCodeHasher, EmailConfirmationCodeHasher>();
        services.AddSingleton<IRefreshTokenHasher, RefreshTokenHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddSingleton<ITokenReaderService, TokenReaderService>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IFileStorageService, CloudFileStorageService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        services.Scan(scan =>
            scan.FromAssemblyOf<BrandRepository>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        return services;
    }
}

using System.Reflection;
using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Services.Authentication.Decorators;
using Application.Services.Brand.Decorators;
using Application.Services.Cart.Decorators;
using Application.Services.Category.Decorators;
using Application.Services.Product.Decorators;
using Application.Services.Tag.Decorators;
using Application.Services.Wishlist.Decorators;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly(),
            lifetime: ServiceLifetime.Scoped,
            includeInternalTypes: true);

        services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(
                bootstrapper =>
                    bootstrapper.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")),
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire",
                    // optionally tune other settings:
                    // QueuePollInterval = TimeSpan.FromSeconds(15),
                    // InvisibilityTimeout = TimeSpan.FromMinutes(30),
                    // TablePrefix = "hf_",
                });
        });
        services.AddHangfireServer();

        services.Scan(scan =>
            scan.FromAssemblyOf<IBrandService>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddScoped<IEventDispatcher, EventDispatcher>();

        services.Scan(scan =>
            scan.FromAssemblyOf<IEventHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Decorate<IBrandService, CachingBrandServiceDecorator>();
        services.Decorate<IBrandService, LoggingBrandServiceDecorator>();

        services.Decorate<ICategoryService, CachingCategoryServiceDecorator>();
        services.Decorate<ICategoryService, LoggingCategoryServiceDecorator>();

        services.Decorate<ITagService, CachingTagServiceDecorator>();
        services.Decorate<ITagService, LoggingTagServiceDecorator>();

        services.Decorate<IProductService, CachingProductServiceDecorator>();
        services.Decorate<IProductService, LoggingProductServiceDecorator>();

        services.Decorate<IAuthenticationService, LoggingAuthenticationServiceDecorator>();
        services.Decorate<ICartService, LoggingCartServiceDecorator>();
        services.Decorate<IWishlistService, LoggingWishlistServiceDecorator>();

        return services;
    }
}

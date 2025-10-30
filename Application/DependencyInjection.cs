using System.Reflection;
using Application.Abstraction.Services;
using Application.Services.Brand.Decorators;
using Application.Services.Category.Decorators;
using Application.Services.Product.Decorators;
using Application.Services.Tag.Decorators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly(),
            includeInternalTypes: true,
            lifetime: ServiceLifetime.Scoped);

        services.Scan(scan => scan
            .FromAssemblyOf<IBrandService>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
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

        return services;
    }
}

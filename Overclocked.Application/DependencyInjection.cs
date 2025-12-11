using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Application.Authentication.Commands;
using Overclocked.Application.Authentication.Commands.Decorators;
using Overclocked.Application.Brand.Commands;
using Overclocked.Application.Brand.Commands.Decorators;
using Overclocked.Application.Brand.Queries;
using Overclocked.Application.Brand.Queries.Decorators;
using Overclocked.Application.Category.Commands;
using Overclocked.Application.Category.Commands.Decorators;
using Overclocked.Application.Category.Queries;
using Overclocked.Application.Category.Queries.Decorators;
using Overclocked.Application.Product.Commands;
using Overclocked.Application.Product.Commands.Decorators;
using Overclocked.Application.Product.Queries;
using Overclocked.Application.Product.Queries.Decorators;
using Overclocked.Application.Tag.Commands;
using Overclocked.Application.Tag.Commands.Decorators;
using Overclocked.Application.Tag.Queries;
using Overclocked.Application.Tag.Queries.Decorators;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly(),
            lifetime: ServiceLifetime.Scoped,
            includeInternalTypes: true);

        services.Scan(scan =>
            scan.FromAssemblyOf<IBrandCommands>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Commands")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Scan(scan =>
            scan.FromAssemblyOf<IBrandQueries>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Queries")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Scan(scan =>
            scan.FromAssembliesOf(typeof(DependencyInjection)) // scans the assembly
                .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddCommandsDecorators()
            .AddQueriesDecorators();

        return services;
    }

    private static IServiceCollection AddCommandsDecorators(this IServiceCollection services)
    {
        services.Decorate<IBrandCommands, CachingBrandCommandsDecorator>();
        services.Decorate<IBrandCommands, ValidatingBrandCommandsDecorator>();
        services.Decorate<IBrandCommands, LoggingBrandCommandsDecorator>();

        services.Decorate<ICategoryCommands, CachingCategoryCommandsDecorator>();
        services.Decorate<ICategoryCommands, ValidatingCategoryCommandsDecorator>();
        services.Decorate<ICategoryCommands, LoggingCategoryCommandsDecorator>();

        services.Decorate<ITagCommands, CachingTagCommandsDecorator>();
        services.Decorate<ITagCommands, ValidatingTagCommandsDecorator>();
        services.Decorate<ITagCommands, LoggingTagCommandsDecorator>();

        services.Decorate<IAuthenticationCommands, ValidatingAuthenticationCommandsDecorator>();
        services.Decorate<IAuthenticationCommands, LoggingAuthenticationCommandsDecorator>();

        services.Decorate<IProductCommands, CachingProductCommandsDecorator>();
        services.Decorate<IProductCommands, ValidatingProductCommandsDecorator>();
        services.Decorate<IProductCommands, LoggingProductCommandsDecorator>();

        return services;
    }

    private static IServiceCollection AddQueriesDecorators(this IServiceCollection services)
    {
        services.Decorate<IBrandQueries, CachingBrandQueriesDecorator>();
        services.Decorate<IBrandQueries, LoggingBrandQueriesDecorator>();

        services.Decorate<ICategoryQueries, CachingCategoryQueriesDecorator>();
        services.Decorate<ICategoryQueries, LoggingCategoryQueriesDecorator>();

        services.Decorate<ITagQueries, CachingTagQueriesDecorator>();
        services.Decorate<ITagQueries, ValidatingTagQueriesDecorator>();
        services.Decorate<ITagQueries, LoggingTagQueriesDecorator>();

        services.Decorate<IProductQueries, CachingProductQueriesDecorator>();
        // services.Decorate<IProductQueries, ValidatingTagQueriesDecorator>();
        services.Decorate<IProductQueries, LoggingProductQueriesDecorator>();

        return services;
    }
}

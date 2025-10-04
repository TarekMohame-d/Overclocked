using System.Reflection;
using Application.Abstraction.Behaviors;
using Application.Abstraction.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationalPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionalPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingPipelineBehavior<,>));

        services.AddScoped<IMediator, Mediator>();
        Assembly assemblyToScan = typeof(ApplicationServicesRegistration).Assembly;

        services.Scan(scan => scan.FromAssemblies(assemblyToScan)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(INotificationHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        return services;
    }
}

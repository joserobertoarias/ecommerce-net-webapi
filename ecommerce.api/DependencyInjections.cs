using System.Reflection;
using ecommerce.api.Abstractions.Messaging;
using ecommerce.api.Database;
using ecommerce.api.Shared.Behaviors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("EcommerConnectionString")!;
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<HandlerExecutor>();
        services.AddScoped<IValidationService, ValidationService>();

        services.AddHandlersFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IDispatcher, Dispatcher>();
        return services;
    }

    private static void AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                           i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))));
        
        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                             i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            foreach (var handlerInterface in interfaces)
            {
                services.AddScoped(handlerInterface, handlerType);
            }
        }
    }
    
}
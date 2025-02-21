using System.Reflection;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.DependencyInjection;

public static class MediatorServiceExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services,Assembly? assembly = null)
    {
        services.AddScoped<IMediator, Core.Mediator>();
    
        // 如果沒有指定 Assembly，就掃描所有相關的 Assembly
        assembly ??= Assembly.GetCallingAssembly();
    
        // 掃描 Request Handlers
        var requestHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

        foreach (var handlerType in requestHandlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && 
                            i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
        
            services.AddScoped(handlerInterface, handlerType);
        }
        
        return services;
    }
}
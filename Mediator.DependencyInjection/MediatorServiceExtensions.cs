using System.Reflection;
using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.DependencyInjection;

public static class MediatorServiceExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Assembly assembly)
    {
        // 判斷參數是否為 null
        if (assembly is null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        // 注册 Mediator
        services.AddScoped<IMediator, Core.Mediator>();

        // 取出所有 Request Handlers
        var requestHandlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));
        
        // 注册 Request Handlers
        foreach (var handlerType in requestHandlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && 
                            i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            services.AddScoped(handlerInterface, handlerType);
        }
    
        // 取得所有行為類型
        var behaviorTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetInterfaces()
                            .Any(i => i.IsGenericType && 
                                      i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)));
        
        // 注册行為 (Behaviors)
        foreach (var behaviorType in behaviorTypes)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), behaviorType);
        }

        return services;
    }
}
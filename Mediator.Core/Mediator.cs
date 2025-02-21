using Mediator.Abstractions;

namespace Mediator.Core;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// Send a request to the appropriate handler and return the response.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        // 從 DI 容器中解析處理器實例
        var handler = _serviceProvider.GetService(handlerType);
        
        if (handler == null)
        {
            throw new InvalidOperationException($"Handler not found for request type {requestType}");
        }

        // 調用處理器的 Handle 方法
        var method = handlerType.GetMethod("Handle");
        if (method == null)
        {
            throw new InvalidOperationException($"Handle method not found on handler type {handlerType}");
        }
        
        // 建立最終的處理器委派
        async Task<TResponse> HandlerCallback(CancellationToken token)
        {
            return await (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        }
        
        // 正確獲取泛型行為管道
        Type[] typeArgs = [requestType, typeof(TResponse)];
        var behaviorType = typeof(IPipelineBehavior<,>);
        var constructedBehaviorType = behaviorType.MakeGenericType(typeArgs);

        var tempBehaviors = _serviceProvider
            .GetService(typeof(IEnumerable<>).MakeGenericType(constructedBehaviorType));
        
        if(tempBehaviors == null)
        {
            return await HandlerCallback(cancellationToken);
        }

        var behaviorsEnumerable = (IEnumerable<object>)tempBehaviors;
        
        // 獲取所有已註冊的 behaviors
        var behaviors = behaviorsEnumerable
            .Reverse();
        
        
        // 建立完整的管道
        RequestHandlerDelegate<TResponse> pipeline = HandlerCallback;
        
        foreach (var behavior in behaviors)
        {
            var currentPipeline = pipeline;
            var handleMethod = behavior.GetType().GetMethod("Handle");
        
            if (handleMethod == null)
            {
                throw new InvalidOperationException($"Handle method not found on behavior type {behavior.GetType()}");
            }

            pipeline = async (ct) => await (Task<TResponse>)handleMethod.Invoke(behavior, [request, new RequestHandlerDelegate<TResponse>(currentPipeline), ct])!;
        }
        
        // 執行完整的管道
        return await pipeline(cancellationToken);
    }
}
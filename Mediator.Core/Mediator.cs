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
        
        var result = await (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        
        return result;
    }
}
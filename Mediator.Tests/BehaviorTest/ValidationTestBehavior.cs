using Mediator.Abstractions;
using Mediator.Tests.MediatorTest;

namespace Mediator.Tests.BehaviorTest;

/// <summary>
/// 驗證行為，用來測試驗證行為的例外狀況
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationTestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is TestRequest testRequest && string.IsNullOrEmpty(testRequest.Data))
        {
            throw new ValidationException("Data cannot be empty");
        }
        
        return await next(cancellationToken);
    }
}
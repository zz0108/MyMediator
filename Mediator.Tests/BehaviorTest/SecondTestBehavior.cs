using Mediator.Abstractions;

namespace Mediator.Tests.BehaviorTest;

/// <summary>
/// 測試用的行為 second behavior
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class SecondTestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        BehaviorExecutionTracker.ExecutionOrder.Add("SecondBehavior Start");
        var response = await next(cancellationToken);
        BehaviorExecutionTracker.ExecutionOrder.Add("SecondBehavior End");
        return response;
    }
}
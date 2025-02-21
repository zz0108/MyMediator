using Mediator.Abstractions;

namespace Mediator.Tests.BehaviorTest;

/// <summary>
/// This is a sample behavior that will be executed first in the pipeline.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class FirstTestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        BehaviorExecutionTracker.ExecutionOrder.Add("FirstBehavior Start");
        var response = await next(cancellationToken);
        BehaviorExecutionTracker.ExecutionOrder.Add("FirstBehavior End");
        return response;
    }
}
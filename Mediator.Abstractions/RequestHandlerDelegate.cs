namespace Mediator.Abstractions;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);
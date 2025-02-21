﻿namespace Mediator.Abstractions;

public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request,CancellationToken cancellationToken);
}

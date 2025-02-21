using Mediator.Abstractions;
using Mediator.Tests.MediatorTest;

namespace Mediator.Tests.BehaviorTest;

public class NonRegisteredRequest : IRequest<TestResponse>
{
    public string Data { get; set; }
}
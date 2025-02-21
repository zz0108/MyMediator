using Mediator.Abstractions;

namespace Mediator.Tests.MediatorTest;

public class TestRequest : IRequest<TestResponse>
{
    public string? Data { get; set; }
}
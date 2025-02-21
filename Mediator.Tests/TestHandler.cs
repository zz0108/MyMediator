using Mediator.Abstractions;

namespace Mediator.Tests;

public class TestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("TestHandler: Handling TestRequest");
        return Task.FromResult(new TestResponse()
        {
            Message = "Test"
        });
    }
}
using Mediator.Abstractions;

namespace Mediator.Tests.MediatorTest;

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

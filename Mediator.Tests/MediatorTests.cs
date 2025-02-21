using System.Reflection;
using Mediator.Abstractions;
using Mediator.DependencyInjection;
using Mediator.Tests.BehaviorTest;
using Mediator.Tests.MediatorTest;
using Microsoft.Extensions.DependencyInjection;


namespace Mediator.Tests;

[TestFixture]
public class MediatorTests
{
    private IServiceProvider _provider;
    private IMediator _mediator;
    
    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        
        // 註冊 Behaviors
        services.AddMediator(Assembly.GetExecutingAssembly());
        
        _provider = services.BuildServiceProvider();
        _mediator = _provider.GetRequiredService<IMediator>();
    }

   [SetUp]
    public void TestSetup()
    {
        BehaviorExecutionTracker.Reset();
    }

    [Test]
    public void AddMediator_ShouldRegisterHandler()
    {
        // Arrange & Act
        var handler = _provider.GetService<IRequestHandler<TestRequest, TestResponse>>();

        // Assert
        Assert.That(handler, Is.Not.Null);
        Assert.That(handler, Is.TypeOf<TestHandler>());
    }
    
    [Test]
    public async Task Send_ShouldReturnResponse()
    {
        // Arrange
        var request = new TestRequest { Data = "Test" };
        
        // Act
        var response = await _mediator.Send(request);
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Test"));
    }

    [Test]
    public async Task Send_WithBehaviors_ShouldExecuteInCorrectOrder()
    {
        // Arrange
        var request = new TestRequest { Data = "Test" };
        
        // Act
        await _mediator.Send(request);
        
        // Assert
        var expectedOrder = new[]
        {
            "FirstBehavior Start",
            "SecondBehavior Start",
            "SecondBehavior End",
            "FirstBehavior End"
        };
        
        Assert.That(BehaviorExecutionTracker.ExecutionOrder, Is.EqualTo(expectedOrder));
    }

    [Test]
    public void Send_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var request = new TestRequest { Data = "" };
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(async () => 
            await _mediator.Send(request));
        
        Assert.That(exception.Message, Is.EqualTo("Data cannot be empty"));
    }

    [Test]
    public void Send_WithNonRegisteredRequest_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new NonRegisteredRequest();
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _mediator.Send(request));
        
        Assert.That(exception.Message, Contains.Substring("Handler not found"));
    }

    [Test]
    public async Task Send_WithMultipleBehaviors_ShouldMaintainBehaviorChain()
    {
        // Arrange
        var request = new TestRequest { Data = "Test" };
        var behaviors = _provider.GetServices<IPipelineBehavior<TestRequest, TestResponse>>();
        
        // Assert
        Assert.That(behaviors.Count(), Is.EqualTo(3));
        
        // Act
        var response = await _mediator.Send(request);
        
        // Additional Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(BehaviorExecutionTracker.ExecutionOrder.Count, Is.EqualTo(4));
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
using System.Reflection;
using Mediator.Abstractions;
using Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


namespace Mediator.Tests;

[TestFixture]
public class MediatorTests
{
    private IServiceProvider _provider;
    
    [OneTimeSetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator(Assembly.GetExecutingAssembly());
        _provider = services.BuildServiceProvider();
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
        var mediator = _provider.GetService<IMediator>();
        
        if (mediator is null)
        {
            Assert.Fail("Mediator is null");
            return;
        }
        
        // Act
        var response = await mediator.Send(new TestRequest());
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Test"));
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
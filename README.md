# CQRS + DI + RabbitMQ in Microservice Architecture

> A good **Software Architect** is a software developer who made/witnessed a tremendous amount of design and development mistakes/disasters in the past. He is reading regularly about the other's failure as his main objective and checking the new technologies in his breaks.   

Perquisite: 
- Learn more about the [Microservices]("https://docs.microsoft.com/en-us/dotnet/architecture/microservices/")
- Learn more about the [CQRS]("https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs") 
- Learn more about the [RabbitMq]("https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/rabbitmq-event-bus-development-test-environment")
- Learn more about [DI(Dependency Injection)]("https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1") 

When it comes to the enterprise architecture, picking the right technology is the most tricky part and must be taken by the software architect of the team. To make the microservice work properly you need a good service bus architecture. The service architecture is very simple but in the same time the backbone of the inter-communication of the microservices. 

## Usage

```c#
public void ConfigureServices(IServiceCollection services)
{
    //...

    services.AddMediatR(typeof(Startup));

    services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
    {
        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
        return new RabbitMQBus(sp.GetService<IMediator>(), scopeFactory);
    });

    //...
}
```
### Issue a Command 

### Receive the Command 

### Issue an Event

### Receive the Event 




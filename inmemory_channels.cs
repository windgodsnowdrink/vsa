#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Hosting@8.0.9
#:package System.Threading.Channels@8.0.9
#:package Microsoft.Extensions.ObjectPool@8.0.9
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.EntityFrameworkCore;
using MediatR;

builder.Services.AddSingleton<InMemoryMessageQueue>();
builder.Services.AddSingleton<IEventBus, EventBus>();
builder.Services.AddHostedService<IntegrationEventProcessorJob>();

public interface IEventBus
{
    Task PublishAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent;
}

public interface IIntegrationEvent : INotification
{
    Guid Id { get; init; }
}

public abstract record IntegrationEvent(Guid Id) : IIntegrationEvent;

internal sealed class InMemoryMessageQueue
{
    private readonly Channel<IIntegrationEvent> _channel =
        Channel.CreateUnbounded<IIntegrationEvent>();

    public ChannelReader<IIntegrationEvent> Reader => _channel.Reader;

    public ChannelWriter<IIntegrationEvent> Writer => _channel.Writer;
}

internal sealed class EventBus(InMemoryMessageQueue queue) : IEventBus
{
    public async Task PublishAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent
    {
        await queue.Writer.WriteAsync(integrationEvent, cancellationToken);
    }
}

internal sealed class IntegrationEventProcessorJob(
    InMemoryMessageQueue queue,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<IntegrationEventProcessorJob> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IIntegrationEvent integrationEvent in
            queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();

                IPublisher publisher = scope.ServiceProvider
                    .GetRequiredService<IPublisher>();

                await publisher.Publish(integrationEvent, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Something went wrong! {IntegrationEventId}",
                    integrationEvent.Id);
            }
        }
    }
}

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IEventBus eventBus)
    : ICommandHandler<RegisterUserCommand>
{
    public async Task<User> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        // First, register the user.
        User user = CreateFromCommand(command);

        userRepository.Insert(user);

        // Now we can publish the event.
        await eventBus.PublishAsync(
            new UserRegisteredIntegrationEvent(user.Id),
            cancellationToken);

        return user;
    }
}

internal sealed class UserRegisteredIntegrationEventHandler
    : INotificationHandler<UserRegisteredIntegrationEvent>
{
    public async Task Handle(UserRegisteredIntegrationEvent @event, CancellationToken cancellationToken)
    {
        // Asynchronously handle the event.
    }
}
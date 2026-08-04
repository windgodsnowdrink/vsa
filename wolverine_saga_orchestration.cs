#:sdk Microsoft.NET.Sdk.Web
#:package Wolverine@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Wolverine;
using Wolverine.Sagas;

// Saga编排处理器
public class TodoSagaOrchestrator
{
    [WolverineHandler]
    public async Task Handle(CreateTodoItemEvent @event, IMessageBus bus)
    {
        await bus.PublishAsync(new NotifyUserEvent(@event.TodoId));
    }

    [WolverineHandler]
    public async Task Handle(UserNotifiedEvent @event, IMessageBus bus)
    {
        await bus.PublishAsync(new CompleteTodoSagaCommand(@event.TodoId));
    }

    [WolverineHandler]
    public async Task Handle(CompleteTodoSagaCommand command, TodoSagaState state)
    {
        state.Status = SagaStatus.Completed;
    }

    [WolverineHandler]
    public async Task Handle(Exception ex, RollbackResourcesEvent @event, IMessageBus bus)
    {
        await bus.PublishAsync(new ResourcesRolledBackEvent(@event.TodoId));
    }
}

public record CreateTodoItemEvent(int TodoId, string Title);
public record NotifyUserEvent(int TodoId);
public record UserNotifiedEvent(int TodoId);
public record CompleteTodoSagaCommand(int TodoId);
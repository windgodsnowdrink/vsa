#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0

public class CqrsProcessor
{
    private readonly BroadcastBlock<ICommand> _commandBroadcaster;
    private readonly Dictionary<Type, ITargetBlock<ICommand>> _handlers = new();

    public CqrsProcessor()
    {
        _commandBroadcaster = new BroadcastBlock<ICommand>(cmd => cmd);
        
        // 注册命令处理器
        RegisterHandler<CreateTodoCommand>(new ActionBlock<CreateTodoCommand>(HandleCreate));
        RegisterHandler<UpdateTodoCommand>(new ActionBlock<UpdateTodoCommand>(HandleUpdate));
    }

    private void RegisterHandler<T>(ITargetBlock<T> handler) where T : ICommand
    {
        var adapter = new TransformBlock<ICommand, T>(cmd => (T)cmd);
        adapter.LinkTo(handler);
        _handlers[typeof(T)] = adapter;
    }

    public async Task SendAsync(ICommand command)
    {
        await _commandBroadcaster.SendAsync(command);
    }
}

public interface ICommand {}
public record CreateTodoCommand(string Title) : ICommand;
public record UpdateTodoCommand(int Id, bool IsCompleted) : ICommand;
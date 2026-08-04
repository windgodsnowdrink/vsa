#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0

public class LoadBalancer
{
    private readonly List<ActionBlock<TodoEvent>> _workers;
    private readonly BufferBlock<TodoEvent> _buffer;
    private int _currentIndex;

    public LoadBalancer(int workerCount)
    {
        _workers = Enumerable.Range(0, workerCount)
            .Select(_ => new ActionBlock<TodoEvent>(ProcessAsync))
            .ToList();

        _buffer = new BufferBlock<TodoEvent>();
        _buffer.LinkTo(new ActionBlock<TodoEvent>(Dispatch));
    }

    private void Dispatch(TodoEvent todo)
    {
        _workers[_currentIndex].Post(todo);
        _currentIndex = (_currentIndex + 1) % _workers.Count;
    }
}
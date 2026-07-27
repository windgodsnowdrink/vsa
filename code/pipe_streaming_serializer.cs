#:sdk Microsoft.NET.Sdk.Web
#:package SpanJson@4.0.0
#:package System.IO.Pipelines@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.IO.Pipelines;
using SpanJson;

public class PipeStreamingSerializer : IAsyncDisposable
{
    private readonly Pipe _pipe;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts = new();

    public PipeStreamingSerializer()
    {
        _pipe = new Pipe(new PipeOptions(
            pauseWriterThreshold: 1024 * 1024,
            resumeWriterThreshold: 512 * 1024,
            minimumSegmentSize: 4096));

        _processingTask = Task.Run(ProcessPipeAsync);
    }

    public async ValueTask SerializeAsync<T>(T value)
    {
        var writer = new Utf8JsonWriter(_pipe.Writer);
        JsonSerializer.Generic.Utf8.Serialize(ref writer, value);
        await writer.FlushAsync(_cts.Token);
        await _pipe.Writer.FlushAsync(_cts.Token);
    }

    public async IAsyncEnumerable<T?> DeserializeAsync<T>()
    {
        while (!_cts.IsCancellationRequested)
        {
            var readResult = await _pipe.Reader.ReadAsync(_cts.Token);
            if (readResult.IsCompleted || readResult.IsCanceled)
                break;

            var buffer = readResult.Buffer;
            var result = JsonSerializer.Generic.Utf8.Deserialize<T>(buffer.FirstSpan);
            yield return result;

            _pipe.Reader.AdvanceTo(buffer.End);
        }
    }

    private async Task ProcessPipeAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            var readResult = await _pipe.Reader.ReadAsync(_cts.Token);
            if (readResult.IsCompleted || readResult.IsCanceled)
                break;

            _pipe.Reader.AdvanceTo(readResult.Buffer.End);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        await _processingTask;
        _pipe.Reader.Complete();
        _pipe.Writer.Complete();
    }
}
#:sdk Microsoft.NET.Sdk.Web
#:package MessagePack@2.5.122
#:package System.IO.MemoryMappedFiles@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.IO.MemoryMappedFiles;
using MessagePack;

public class SharedMemoryMessagePackSerializer : IDisposable
{
    private readonly MemoryMappedFile _mmf;
    private readonly Semaphore _semaphore;

    public SharedMemoryMessagePackSerializer(string mapName)
    {
        _mmf = MemoryMappedFile.CreateOrOpen(mapName, 1024 * 1024);
        _semaphore = new Semaphore(1, 1, $"{mapName}_Semaphore");
    }

    public void Serialize<T>(T value)
    {
        _semaphore.WaitOne();
        try
        {
            using var stream = _mmf.CreateViewStream();
            MessagePackSerializer.Serialize(stream, value);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public T? Deserialize<T>()
    {
        _semaphore.WaitOne();
        try
        {
            using var stream = _mmf.CreateViewStream();
            return MessagePackSerializer.Deserialize<T>(stream);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _mmf.Dispose();
    }
}
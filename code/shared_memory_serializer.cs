#:sdk Microsoft.NET.Sdk.Web
#:package System.Text.Json@8.0.0
#:package System.IO.MemoryMappedFiles@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.IO.MemoryMappedFiles;
using System.Text.Json;

public class SharedMemorySerializer : IDisposable
{
    private readonly MemoryMappedFile _mmf;
    private readonly Semaphore _semaphore;

    public SharedMemorySerializer(string mapName)
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
            JsonSerializer.Serialize(stream, value, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultBufferSize = 4096
            });
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
            return JsonSerializer.Deserialize<T>(stream);
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
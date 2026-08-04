#:sdk Microsoft.NET.Sdk.Web
#:package WatchDog.NET@3.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Runtime.InteropServices;

public class TieredMemoryServer : IMemoryManager
{
    private readonly nint _hotMemory;
    private readonly nint _coldMemory;
    
    public TieredMemoryServer()
    {
        _hotMemory = Marshal.AllocHGlobal(1024 * 1024); // 1MB热内存
        _coldMemory = Marshal.AllocHGlobal(10 * 1024 * 1024); // 10MB冷内存
    }

    public Span<byte> RentHotMemory(int size)
    {
        return new Span<byte>(_hotMemory.ToPointer(), size);
    }
}

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<IMemoryManager, TieredMemoryServer>();
builder.Services.AddWatchDogServices();
var app = builder.Build();
app.UseWatchDog();
app.Run();
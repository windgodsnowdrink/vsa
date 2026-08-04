#:sdk Microsoft.NET.Sdk.Web
#:package System.Reactive@6.0.1
#:package System.Reactive.Linq@6.0.1
#:package System.Reactive.Core@6.0.1
#:package System.Reactive.Interfaces@6.0.1
#:package System.Reactive.PlatformServices@6.0.1
#:package System.Reactive.Providers@6.0.1
#:package System.Reactive.Windows.Threading@6.0.1
#:package System.Reactive.Forms@6.0.1
#:package System.Reactive.Runtime.Remoting@6.0.1

#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", async () =>
{
    await foreach (var item in FetchItems())
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: {item}");
    }

    return Results.Ok();
});

app.Run();

static async IAsyncEnumerable<int> FetchItems()
{
    for (int i = 1; i <= 10; i++)
    {
        await Task.Delay(100);
        yield return i;
    }
}

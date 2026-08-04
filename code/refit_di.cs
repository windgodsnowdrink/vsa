#:sdk Microsoft.NET.Sdk.Web
#:package Refit@8.0.0
#:package Microsoft.Extensions.Http@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Refit;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 基础配置
builder.Services.AddRefitClient<IUserService>()
    .ConfigureHttpClient(c => 
    {
        c.BaseAddress = new Uri("https://api.example.com");
        c.Timeout = TimeSpan.FromSeconds(30);
    });

// 高性能通道
builder.Services.AddSingleton<Channel<RefitRequest>>(Channel.CreateBounded<RefitRequest>(10000));
# Consul AOT 使用示例

## 1. 基本使用示例

### 1.1 服务注册与发现

```csharp
// 服务注册示例
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<Consul.AOT.ConsulClientOptions>(options =>
{
    options.Address = new Uri("http://localhost:8500");
});
builder.Services.AddSingleton<Consul.AOT.IConsulService, Consul.AOT.ConsulService>();
var host = builder.Build();

var consulService = host.Services.GetRequiredService<Consul.AOT.IConsulService>();

// 注册服务
var result = await consulService.RegisterServiceAsync(
    "web-service", 
    "web-1", 
    "localhost", 
    8080
);
Console.WriteLine($"服务注册结果: {(result ? "成功" : "失败")}
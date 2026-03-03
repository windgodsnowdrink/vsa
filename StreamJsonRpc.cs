// Install-Package StreamJsonRpc
var app = WebApplication.CreateBuilder();
app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var service = new GreeterRpcService();
        await StreamJsonRpc.Attach(webSocket, service);
    }
    else await next();

});

// 定义 RPC 接口
public interface IGreeterRpcService
{
    Task<string> GreetAsync(string name);
}
public interface IClientCallback
{
    Task NotifyAsync(string message);

}
// 实现服务
public class GreeterRpcService : IGreeterRpcService
{
    public Task<string> GreetAsync(string name) => Task.FromResult($"Hello, {name}!");
}
// 服务端方法中调用客户�?
public async Task SendNotificationAsync()
{
    var callback = JsonRpc.GetRpcTarget<IClientCallback>();
    await callback.NotifyAsync("New event!");

}
// 客户端实�?
var webSocket = new ClientWebSocket();

await webSocket.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
var greeter = StreamJsonRpc.JsonRpc.Attach<IGreeterRpcService>(webSocket);

string result = await greeter.GreetAsync("World");

Console.WriteLine(result); // 输出 "Hello, World!"

//自定义序列化 默认使用 System.Text.Json，但可替换为其他序列化器（如 Newtonsoft.Json�?
var options = new JsonRpcOptions
{
    MessageFormatter = new SystemTextJsonFormatter()
};
StreamJsonRpc.Attach(stream, service, options);
// 错误处理 通过 JsonRpcException 捕获远程调用异常，支持自定义错误码和数据
try
{
    await greeter.GreetAsync("error");
}
catch (JsonRpcException ex)
{
    Console.WriteLine($"Error Code: {ex.ErrorCode}, Message: {ex.Message}");
}
// 性能优化
// 使用 MemoryPool �?BufferManager 减少内存分配�?
// 启用 MessagePack 二进制协议以降低传输开销�?
// Install - Package StreamJsonRpc.MessagePack
var formatter = new MessagePackFormatter();
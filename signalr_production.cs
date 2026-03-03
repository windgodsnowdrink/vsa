#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@8.0.0
#:package SignalW@6.0.0
#:package Microsoft.AspNetCore.SignalR.StackExchangeRedis@8.0.0
#:package Microsoft.AspNetCore.Identity.EntityFrameworkCore@8.0.0
#:package Microsoft.EntityFrameworkCore.SqlServer@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();

// 1. 横向扩展支持
builder.Services.AddSignalR().AddStackExchangeRedis("localhost", options => {
    options.Configuration.ChannelPrefix = "SignalR_";
    options.Configuration.DefaultDatabase = 0;
});

// 2. 消息持久化
builder.Services.AddDbContext<MessageDbContext>(options => 
    options.UseSqlServer("Server=.;Database=SignalRDb;Trusted_Connection=True;"));

// 3. 安全认证
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<MessageDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
});

// 4. 高性能通道
var messageChannel = Channel.CreateBounded<WebSocketMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// ... existing code ...

var app = builder.Build();

// 认证中间件
app.UseAuthentication();
app.UseAuthorization();

// 聊天室Hub
app.MapHub<ChatHub>("/chat", options => {
    options.Transports = HttpTransportType.WebSockets;
    options.CloseOnAuthenticationExpiration = true;
});

// 实时通知Hub
app.MapHub<NotificationHub>("/notifications");

// 实时数据流Hub
app.MapHub<DataStreamHub>("/stream");

app.MapGet("/", () => "SignalR Production Ready");
app.Run();

// 聊天室实现
[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageStore _store;
    
    public ChatHub(IMessageStore store) => _store = store;

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserJoined", Context.User.Identity.Name);
    }

    public async Task SendToRoom(string roomName, string message)
    {
        var msg = new ChatMessage(
            Context.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Context.User.Identity.Name,
            message,
            DateTime.UtcNow);
        
        await _store.SaveAsync(msg);
        await Clients.Group(roomName).SendAsync("ReceiveMessage", msg);
    }
}

// 实时通知Hub
public class NotificationHub : Hub
{
    public async IAsyncEnumerable<Notification> StreamNotifications()
    {
        while (true)
        {
            yield return new Notification(
                Id: Guid.NewGuid(),
                Title: "Alert",
                Content: $"Update at {DateTime.UtcNow}",
                IsUrgent: Random.Shared.Next(10) == 0);
            
            await Task.Delay(1000);
        }
    }
}

// 数据库上下文
public class MessageDbContext : DbContext
{
    public DbSet<ChatMessage> Messages { get; set; }
    
    public MessageDbContext(DbContextOptions options) : base(options) {}
}

// 消息实体
public record ChatMessage(
    string UserId,
    string UserName,
    string Content,
    DateTime Timestamp);
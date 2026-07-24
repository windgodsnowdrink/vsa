#:sdk Microsoft.NET.Sdk.Web
#:package WolverineFx@4.2.0
#:package WolverineFx.Marten@4.2.0
#:package WolverineFx.RDBMS@4.2.0
#:package WolverineFx.Postgresql@4.2.0
#:package WolverineFx.FluentValidation@4.2.0
#:package WolverineFx.Http@4.2.0
#:package WolverineFx.RabbitMQ@4.2.0
#:package WolverineFx.AzureServiceBus@4.2.0
#:package WolverineFx.Http.FluentValidation@4.2.0
#:package WolverineFx.Http.Marten@4.2.0
#:package WolverineFx.AmazonSqs@4.2.0
#:package WolverineFx.EntityFrameworkCore@4.2.0
#:package WolverineFx.SqlServer@4.2.0
#:package WolverineFx.Kafka@4.2.0
#:package WolverineFx.MemoryPack@4.2.0
#:package WolverineFx.MessagePack@4.2.0
#:package WolverineFx.MQTT@4.2.0
#:package WolverineFx.Pubsub@4.2.0
#:package WolverineFx.Pulsar@4.2.0
#:package WolverineFx.RavenDb@4.2.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using Wolverine;
using Wolverine.Http;
using Wolverine.RabbitMQ;
using Wolverine.Kafka;
using Wolverine.AzureServiceBus;
using Wolverine.AmazonSqs;
using Wolverine.Marten;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Marten;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置Wolverine集成
builder.Host.UseWolverine(opts =>
{
    // 1.1 持久化配置
    opts.PersistMessagesWithPostgresql("Host=localhost;Port=5432;Database=wolverine;Username=postgres;Password=P@ssw0rd");
    opts.UseEntityFrameworkCoreTransactions();
    
    // 1.2 消息传输配置
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = "localhost";
        rabbit.Port = 5672;
        rabbit.ConfigureQueue("todo-queue", q => q.Durable = true);
    });
    
    opts.UseKafka(kafka =>
    {
        kafka.BootstrapServers = "localhost:9092";
        kafka.Topic("todo-events").ConfigureTopic(t => t.Partitions = 3);
    });
    
    opts.UseAzureServiceBus(asb =>
    {
        asb.ConnectionString = "Endpoint=sb://your-servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key";
    });
    
    opts.UseAmazonSqs(sqs =>
    {
        sqs.Region = "us-west-2";
        sqs.Queue("todo-queue", q => q.Durable = true);
    });
    
    // 1.3 序列化配置
    opts.UseMemoryPackSerialization();
    opts.UseMessagePackSerialization();
    
    // 1.4 高级配置
    opts.Advanced.UseZeroCopyMessageSerialization();
    opts.Advanced.CpuCount = Environment.ProcessorCount;
    opts.Advanced.ScheduledJobPollingTime = 5.Seconds();
    
    // 1.5 重试策略
    opts.Handlers.OnException<TimeoutException>()
        .RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
    opts.UseMqtt(mqtt => 
    {
        mqtt.Broker = "localhost";
        mqtt.Port = 1883;
        mqtt.ConfigureTopic("todo-events");
    });
    opts.UsePulsar(pulsar =>
    {
        pulsar.ServiceUrl = "pulsar://localhost:6650";
        pulsar.Topic("todo-events").ConfigureTopic(t => 
        {
            t.ProducerName = "wolverine-producer";
            t.CompressionType = CompressionType.LZ4;
        });
    });
});

// 2. 配置数据库
builder.Services.AddMarten(opts =>
{
    opts.Connection("Host=localhost;Port=5432;Database=marten;Username=postgres;Password=P@ssw0rd");
    opts.AutoCreateSchemaObjects = AutoCreate.All;
});

builder.Services.AddDbContext<TodoDbContext>(opts =>
    opts.UseNpgsql("Host=localhost;Port=5432;Database=todos;Username=postgres;Password=P@ssw0rd"));

// 3. 配置FluentValidation
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<CreateTodoValidator>();
});

var app = builder.Build();

// 4. 配置HTTP端点
app.MapWolverineEndpoints(opts =>
{
    opts.UseNewtonsoftJsonForSerialization();
    opts.UseFluentValidation();
    opts.UseOpenApi(o =>
    {
        o.Title = "Wolverine Todo API";
        o.Version = "v1";
    });
});

// 5. 示例消息处理器
public static class TodoHandlers
{
    [Transactional]
    public static async Task<TodoCreated> Handle(
        CreateTodo command, 
        IDocumentSession session,
        TodoDbContext dbContext)
    {
        var todo = new Todo { Name = command.Name };
        session.Store(todo);
        dbContext.Todos.Add(todo);
        await session.SaveChangesAsync();
        await dbContext.SaveChangesAsync();
        return new TodoCreated(todo.Id);
    }
}

// 6. HTTP端点
public static class TodoEndpoints
{
    [WolverinePost("/todos")]
    public static (TodoCreated, IResult) Create(CreateTodo command)
    {
        return (new TodoCreated(1), Results.Created($"/todos/1", null));
    }
}

// 7. 数据模型
public class Todo 
{ 
    public int Id { get; set; }
    public string Name { get; set; }
}

public record CreateTodo(string Name);
public record TodoCreated(int Id);

// 8. 验证器
public class CreateTodoValidator : AbstractValidator<CreateTodo>
{
    public CreateTodoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

// 9. 数据库上下文
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
    public DbSet<Todo> Todos { get; set; }
}

app.Run();

// WolverineFx核心 - 消息处理管道
// 零拷贝消息处理
public static async Task Handle(OrderCreated message, IMessageContext context)
{
    using var buffer = MemoryPool<byte>.Shared.Rent(1024);
    await context.SendAsync(buffer.Memory[..message.Size]);
}
//WolverineFx.Marten - 文档数据库集成
[Transactional] 
public static async Task Handle(CreateOrder cmd, IDocumentSession session)
{
    session.Store(new Order(cmd.Id)); // AOT兼容
}
//WolverineFx.RDBMS - 关系型数据库
opts.PersistMessagesWithSqlServer(connString)
    .UseTransactionTimeout(30.Seconds()); // 分层事务
//WolverineFx.Postgresql - PG专属优化
opts.PersistMessagesWithPostgresql(connString)
    .UseScheduledTaskPolling(5.Seconds()); // 尾延迟优化
// WolverineFx.FluentValidation - 验证集成
public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        RuleFor(x => x.Id).NotEmpty(); // 线程安全验证
    }
}
//WolverineFx.Http - WebAPI端点
[WolverinePost("/orders")]
public static (OrderCreated, IResult) Create(Order order)
{
    return (new OrderCreated(order.Id), Results.Ok());
}
//WolverineFx.RabbitMQ - 消息队列
opts.UseRabbitMq(rabbit => 
{
    rabbit.HostName = "localhost";
    rabbit.ConfigureQueue("orders", q => q.PrefetchCount = 100); // Disruptor模式
});
//WolverineFx.AzureServiceBus - ASB集成
opts.UseAzureServiceBus(asb =>
{
    asb.ConnectionString = "...";
    asb.ConfigureTopic("orders"); // TokenRing缓冲区
});
//WolverineFx.AmazonSqs - AWS SQS
opts.UseAmazonSqs(sqs =>
{
    sqs.Region = "us-west-2";
    sqs.Queue("orders", q => q.VisibilityTimeout = 30); // 分层内存
});
//WolverineFx.EntityFrameworkCore - EF Core集成
[Transactional]
public static async Task Handle(Order cmd, TodoDbContext db)
{
    db.Orders.Add(cmd); // 零拷贝变更跟踪
}
//WolverineFx.Kafka - 消息流处理
opts.UseKafka(kafka =>
{
    kafka.BootstrapServers = "localhost:9092";
    kafka.Topic("orders").ConfigureTopic(t => t.Partitions = 3); // 多生产者
});
//WolverineFx.MemoryPack
opts.UseMemoryPackSerialization()
    .UseZeroCopyMessageSerialization(); // Span<T>优化
//WolverineFx.MQTT - IoT协议
opts.UseMqtt(mqtt =>
{
    mqtt.Broker = "localhost";
    mqtt.ConfigureTopic("sensor-data"); // 高频处理
});
//WolverineFx.Pulsar - 流处理
opts.UsePulsar(pulsar =>
{
    pulsar.ServiceUrl = "pulsar://localhost:6650";
    pulsar.Topic("orders").CompressionType = CompressionType.LZ4; // 二进制压缩
});
//WolverineFx.RavenDb - NoSQL集成
opts.UseRavenDb(raven =>
{
    raven.ServerUrls = ["http://localhost:8080"];
    raven.DatabaseName = "Orders"; // 缓存一致性
});
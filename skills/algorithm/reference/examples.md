# Algorithm Agent Skill - 使用示例

## 1. Raft分布式一致性算法示例

### 1.1 基本使用示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Algorithm.Raft;

// 创建Raft节点配置
var config = new RaftNodeConfig
{
    NodeId = "node1",
    ClusterNodes = new List<string> { "node1", "node2", "node3" },
    ElectionTimeout = TimeSpan.FromMilliseconds(500),
    HeartbeatInterval = TimeSpan.FromMilliseconds(150),
    LogDirectory = "./raft-logs",
    EnableMetrics = true
};

// 创建Raft节点
var raftNode = new RaftNode(config);

// 启动Raft节点
await raftNode.StartAsync();

// 注册状态变化事件
raftNode.StateChanged += (sender, args) =>
{
    Console.WriteLine($"节点状态变更: {args.OldState} -> {args.NewState}");
};

// 提交日志条目
for (int i = 0; i < 10; i++)
{
    var logEntry = new LogEntry
    {
        Command = $"SET key{i} value{i}",
        Term = raftNode.CurrentTerm
    };

    var result = await raftNode.AppendEntryAsync(logEntry);
    Console.WriteLine($"日志提交结果: {result.Success}, Term: {result.Term}");
}

// 获取节点状态
var state = raftNode.GetNodeState();
Console.WriteLine($"当前节点状态: {state}");

// 停止Raft节点
await raftNode.StopAsync();
```

### 1.2 集群配置示例

```csharp
// 创建3个节点的Raft集群配置
var nodeConfigs = new List<RaftNodeConfig>
{
    new RaftNodeConfig
    {
        NodeId = "node1",
        ClusterNodes = new List<string> { "node1", "node2", "node3" },
        ElectionTimeout = TimeSpan.FromMilliseconds(500),
        HeartbeatInterval = TimeSpan.FromMilliseconds(150)
    },
    new RaftNodeConfig
    {
        NodeId = "node2",
        ClusterNodes = new List<string> { "node1", "node2", "node3" },
        ElectionTimeout = TimeSpan.FromMilliseconds(500),
        HeartbeatInterval = TimeSpan.FromMilliseconds(150)
    },
    new RaftNodeConfig
    {
        NodeId = "node3",
        ClusterNodes = new List<string> { "node1", "node2", "node3" },
        ElectionTimeout = TimeSpan.FromMilliseconds(500),
        HeartbeatInterval = TimeSpan.FromMilliseconds(150)
    }
};

// 创建并启动所有Raft节点
var raftNodes = new List<RaftNode>();
foreach (var config in nodeConfigs)
{
    var node = new RaftNode(config);
    raftNodes.Add(node);
    await node.StartAsync();
}

// 等待一段时间，观察选举过程
await Task.Delay(TimeSpan.FromSeconds(5));

// 输出所有节点状态
foreach (var node in raftNodes)
{
    var state = node.GetNodeState();
    Console.WriteLine($"节点 {node.NodeId} 状态: {state}, Term: {node.CurrentTerm}");
}

// 停止所有节点
foreach (var node in raftNodes)
{
    await node.StopAsync();
}
```

### 1.3 自定义状态机示例

```csharp
// 实现自定义状态机
public class KeyValueStateMachine : IRaftStateMachine
{
    private readonly Dictionary<string, string> _store = new();
    private readonly object _lock = new();

    public Task ApplyAsync(LogEntry entry)
    {
        lock (_lock)
        {
            // 解析并执行命令
            var parts = entry.Command.Split(' ', 3);
            if (parts.Length < 2)
                return Task.CompletedTask;

            var command = parts[0].ToUpper();
            var key = parts[1];

            switch (command)
            {
                case "SET":
                    if (parts.Length == 3)
                    {
                        _store[key] = parts[2];
                        Console.WriteLine($"执行SET命令: {key} = {parts[2]}");
                    }
                    break;
                case "DELETE":
                    _store.Remove(key);
                    Console.WriteLine($"执行DELETE命令: {key}");
                    break;
            }

            return Task.CompletedTask;
        }
    }

    public Task<object> QueryAsync(object query)
    {
        lock (_lock)
        {
            if (query is string key && _store.TryGetValue(key, out var value))
            {
                return Task.FromResult<object>(value);
            }
            return Task.FromResult<object>(null);
        }
    }

    public Task<Snapshot> CreateSnapshotAsync()
    {
        lock (_lock)
        {
            // 将状态序列化为快照
            var snapshotData = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(_store);
            return Task.FromResult(new Snapshot { Data = snapshotData });
        }
    }

    public Task RestoreFromSnapshotAsync(Snapshot snapshot)
    {
        lock (_lock)
        {
            // 从快照恢复状态
            var restoredStore = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(snapshot.Data);
            if (restoredStore != null)
            {
                _store.Clear();
                foreach (var kvp in restoredStore)
                {
                    _store[kvp.Key] = kvp.Value;
                }
            }
            return Task.CompletedTask;
        }
    }
}

// 使用自定义状态机
var config = new RaftNodeConfig { NodeId = "node1", ClusterNodes = new List<string> { "node1" } };
var stateMachine = new KeyValueStateMachine();
var raftNode = new RaftNode(config, stateMachine);

await raftNode.StartAsync();

// 提交SET命令
await raftNode.AppendEntryAsync(new LogEntry { Command = "SET mykey myvalue" });

// 查询状态机
var result = await stateMachine.QueryAsync("mykey");
Console.WriteLine($"查询结果: {result}");

await raftNode.StopAsync();
```

## 2. 抖动算法（Jitter Algorithm）示例

### 2.1 基本使用示例

```csharp
using Algorithm.Jitter;

// 创建抖动算法实例
var jitterAlgorithm = new JitterAlgorithm();

// 配置抖动算法
var config = new JitterConfig
{
    BaseDelay = TimeSpan.FromMilliseconds(100),
    MaxDelay = TimeSpan.FromSeconds(10),
    ExponentialBase = 2.0,
    JitterFactor = 0.5,
    EnableMetrics = true
};

// 计算不同尝试次数的延迟
for (int attempt = 1; attempt <= 5; attempt++)
{
    var delay = jitterAlgorithm.CalculateDelay(attempt, config);
    Console.WriteLine($"尝试 {attempt}: 延迟 = {delay.TotalMilliseconds:F2}ms
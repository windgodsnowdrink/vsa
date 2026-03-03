# Algorithm Agent Skill - 高性能算法库

## 技能概览

基于.NET 10的高性能算法库，提供企业级的分布式一致性算法、抖动算法、并发控制算法等实现。专为分布式系统设计、高并发服务、可靠性工程和性能优化场景打造。

## 快速入门指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package System.Threading.Channels@8.0.0
#:package System.Collections.Concurrent@8.0.0
#:package System.Runtime.CompilerServices.Unsafe@8.0.0
```

### 注册服务

在主应用程序中注册Algorithm服务：

```csharp
// 注册Raft服务
builder.Services.AddSingleton<IRaftNode, RaftNode>();
builder.Services.AddSingleton<RaftCluster>();

// 注册抖动算法服务
builder.Services.AddSingleton<JitterAlgorithm>();
```

### 使用示例

#### Raft分布式一致性算法

```csharp
// 创建Raft节点配置
var config = new RaftNodeConfig
{
    NodeId = "node1",
    ClusterNodes = new List<string> { "node1", "node2", "node3" },
    ElectionTimeout = TimeSpan.FromMilliseconds(500),
    HeartbeatInterval = TimeSpan.FromMilliseconds(150)
};

// 创建并启动Raft节点
var raftNode = new RaftNode(config);
await raftNode.StartAsync();

// 提交日志
var logEntry = new LogEntry { Command = "SET key value" };
var result = await raftNode.AppendEntryAsync(logEntry);
```

#### 抖动算法（Jitter Algorithm）

```csharp
// 创建抖动算法实例
var jitterAlgorithm = new JitterAlgorithm();

// 配置抖动算法
var config = new JitterConfig
{
    BaseDelay = TimeSpan.FromMilliseconds(100),
    MaxDelay = TimeSpan.FromSeconds(10),
    ExponentialBase = 2.0,
    JitterFactor = 0.5
};

// 计算重试延迟
var delay = jitterAlgorithm.CalculateDelay(attempt: 3, config);

// 使用抖动算法进行重试
var retryPolicy = JitterAlgorithm.CreateRetryPolicy(config);
await retryPolicy.ExecuteAsync(async () =>
{
    // 执行可能失败的操作
    await SomeUnreliableOperationAsync();
});
```

## 导航地图

```
algorithm/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点 (当前文件)
├── reference/                  # 引用文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── raft_service.cs        # Raft分布式一致性算法
    ├── jitter_algorithm_production.cs # 生产级抖动算法
    └── ...                    # 其他算法实现
```

## 主要功能

1. **Raft分布式一致性算法**：
   - 完整的Raft协议实现
   - 支持领导者选举、日志复制、安全性检查
   - 提供集群管理API
   - 支持节点动态加入和退出
   - 提供详细的状态监控

2. **生产级抖动算法**：
   - 指数退避+抖动算法
   - 支持自定义配置
   - 提供重试策略工厂
   - 支持异步和同步操作
   - 提供性能指标

3. **并发控制算法**：
   - 无锁并发数据结构
   - 高性能信号量
   - 读写锁优化
   - 原子操作封装

4. **负载均衡算法**：
   - 轮询算法
   - 随机算法
   - 最少连接算法
   - 一致性哈希算法
   - 加权负载均衡

5. **限流算法**：
   - 令牌桶算法
   - 漏桶算法
   - 滑动窗口算法
   - 分布式限流支持

## 扩展说明

本技能提供了基础的算法实现，您可以根据需要扩展：

1. 添加更多分布式一致性算法（如Paxos、ZAB等）
2. 实现更多负载均衡策略
3. 扩展限流算法的功能
4. 添加分布式事务算法
5. 实现更复杂的重试机制
6. 添加算法可视化工具

## 最佳实践

1. **Raft算法使用建议**：
   - 集群节点数量建议为奇数（3、5、7等）
   - 合理配置选举超时和心跳间隔
   - 实现日志持久化以提高可靠性
   - 监控Raft节点状态和性能指标
   - 实现节点故障自动恢复机制

2. **抖动算法使用建议**：
   - 根据服务特性调整基础延迟和最大延迟
   - 合理设置抖动因子，避免惊群效应
   - 监控重试次数和成功率
   - 为不同服务配置不同的重试策略
   - 考虑使用断路器模式结合抖动算法

3. **性能优化建议**：
   - 合理使用Span<T>和Memory<T>进行零拷贝操作
   - 使用Channels处理异步操作
   - 考虑使用内存池减少GC压力
   - 实现适当的缓存策略
   - 监控尾延迟并进行优化

4. **并发控制建议**：
   - 优先使用无锁数据结构
   - 合理使用读写锁
   - 避免过度同步
   - 考虑使用分区技术减少锁竞争
   - 监控锁竞争情况

## 技术特性

- **高性能设计**：
  - 使用Span<T>和Memory<T>进行零拷贝操作
  - 基于Channels的异步处理
  - 支持尾延迟优化
  - 内存池优化
  - 无锁并发数据结构
  - CPU缓存行对齐

- **可靠性设计**：
  - 完整的错误处理机制
  - 详细的日志记录
  - 提供诊断指标
  - 支持监控和告警
  - 实现优雅降级

- **易用性设计**：
  - 提供简洁的API
  - 详细的文档和示例
  - 支持多种配置方式
  - 模块化设计，易于扩展
  - 与.NET生态系统良好集成

- **安全性设计**：
  - 实现安全的分布式一致性
  - 防止脑裂等异常情况
  - 提供安全的配置选项
  - 支持加密通信

## 监控和诊断

算法库提供了丰富的监控指标和诊断信息：

- **Raft算法指标**：
  - 节点状态（领导者、跟随者、候选人）
  - 选举次数和耗时
  - 日志复制延迟
  - 心跳间隔
  - 集群健康状态

- **抖动算法指标**：
  - 重试次数和成功率
  - 延迟分布
  - 抖动因子效果
  - 最大延迟情况

- **并发算法指标**：
  - 锁竞争情况
  - 无锁数据结构操作次数
  - 并发度统计
  - 尾延迟分布

## 部署建议

1. **单机部署**：
   - 适合开发和测试环境
   - 可以在本地运行多个Raft节点
   - 便于调试和观察

2. **分布式部署**：
   - 适合生产环境
   - 确保节点分布在不同的物理机器上
   - 配置适当的网络超时
   - 实现节点故障自动检测和恢复

3. **容器化部署**：
   - 支持Docker和Kubernetes部署
   - 提供完整的容器配置
   - 支持水平扩展
   - 便于管理和监控

## 版本管理

- **1.0.0**：初始版本，包含Raft算法和抖动算法
- **1.1.0**：添加负载均衡算法和限流算法
- **1.2.0**：添加分布式锁实现
- **1.3.0**：优化性能和可靠性
- **2.0.0**：支持.NET 10和AOT编译

## 贡献指南

欢迎提交Issue和Pull Request来改进算法库：

1. Fork项目仓库
2. 创建功能分支
3. 实现新功能或修复Bug
4. 编写测试用例
5. 提交Pull Request
6. 代码审查和合并

## 许可证

本项目采用MIT许可证，详见LICENSE文件。

## 联系方式

- 项目地址：[GitHub Repository]
- 文档地址：[Documentation]
- 问题反馈：[Issues]
- 贡献指南：[Contributing Guide]
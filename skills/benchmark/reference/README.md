# benchmark - 参考文档

## 概述

benchmark 是一个基于 .NET 10 的高性能基准测试系统，专为 .NET 开发者设计，支持 AOT（提前编译）编译，提供极致的性能体验。

## 核心组件

### 1. 基准测试服务 (Benchmark Service)
- **位置**: scripts/benchmark_demo.cs
- **功能**: 核心基准测试逻辑处理
- **特性**: 
  - 支持 AOT 编译，提供极致性能
  - 基于 BenchmarkDotNet 框架
  - 支持多种基准测试类型（CPU、内存、并发等）
  - 详细的性能报告生成
  - 支持自定义基准测试配置

## 使用示例

### 基本用法

```csharp
using BenchmarkDotNet.Running;
using BenchmarkDemo;

// 运行基准测试
BenchmarkRunner.Run<MyBenchmark>();
```

### 高级配置

```csharp
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDemo;

// 配置基准测试
var config = DefaultConfig.Instance
    .WithOptions(ConfigOptions.DisableOptimizationsValidator)
    .WithOptions(ConfigOptions.EnableInProcess);

// 运行基准测试
BenchmarkRunner.Run<MyBenchmark>(config);
```

## 配置选项

### 基准测试配置

```json
{
  "BenchmarkConfig": {
    "EnableAot": true,          // 启用 AOT 编译
    "WarmupIterations": 3,      // 预热迭代次数
    "RunIterations": 10,        // 运行迭代次数
    "EnableMemoryDiagnostics": true, // 启用内存诊断
    "EnableCpuDiagnostics": true     // 启用 CPU 诊断
  }
}
```

## 性能优化

1. **AOT 编译**: 启用 AOT 编译以获得最佳性能
2. **预热优化**: 适当的预热迭代次数可以提高测试准确性
3. **并发测试**: 使用 [BenchmarkDotNet 的并发测试功能](https://benchmarkdotnet.org/articles/guides/benchmarking-concurrency.html) 测试并发性能
4. **内存优化**: 利用内存诊断工具识别内存瓶颈
5. **CPU 优化**: 分析 CPU 热点，针对性优化代码

## 故障排除

### 常见问题

1. **AOT 编译失败**
   - 检查项目是否使用 .NET 10
   - 确保所有依赖项支持 AOT
   - 查看详细的编译日志

2. **测试结果不稳定**
   - 增加运行迭代次数
   - 关闭其他占用系统资源的程序
   - 使用 [BenchmarkDotNet 的统计功能](https://benchmarkdotnet.org/articles/guides/statistical-analysis.html) 分析结果

3. **内存消耗过高**
   - 使用内存诊断工具识别内存泄漏
   - 优化数据结构和算法
   - 考虑使用对象池技术

## 扩展开发

### 添加自定义基准测试

```csharp
using BenchmarkDotNet.Attributes;

public class CustomBenchmark
{
    [Benchmark]
    public void MyBenchmarkMethod()
    {
        // 实现自定义基准测试逻辑
        // 例如：测试字符串操作性能
        string result = string.Empty;
        for (int i = 0; i < 1000; i++)
        {
            result += i.ToString();
        }
    }
}
```

### AOT 支持扩展

```csharp
using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

public class AotOptimizedBenchmark
{
    [Benchmark]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int OptimizedMethod()
    {
        // 实现 AOT 优化的基准测试逻辑
        // 使用 MethodImplOptions.AggressiveOptimization 提示编译器优化
        int sum = 0;
        for (int i = 0; i < 1000000; i++)
        {
            sum += i;
        }
        return sum;
    }
}
```

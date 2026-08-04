# Dataflow AOT - 使用示例

## 快速开始

### 1. 简单数据流处理示例

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dataflow.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dataflow AOT 简单数据流处理示例");
        Console.WriteLine("=" * 60);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dataflow选项
        builder.Configuration.AddJsonFile("dataflow_aot.setting.json", optional: true);
        builder.Services.Configure<DataflowOptions>(builder.Configuration.GetSection("Dataflow"));
        
        // 注册服务
        builder.Services.AddDataflow();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dataflow AOT引擎
        var engine = serviceProvider.GetRequiredService<DataflowAotEngine>();
        
        // 准备测试数据
        var testData = Enumerable.Range(1, 100).Select(i => new {
            Id = i,
            Value = i * 2,
            Category = i % 3 == 0 ? "A" : i % 3 == 1 ? "B" : "C"
        }).ToList();
        
        Console.WriteLine($"\n1. 输入数据: {testData.Count} 项");
        Console.WriteLine($"   并行度: 4");
        
        // 执行简单数据流处理
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await engine.ProcessSimpleFlowAsync(
            testData,
            async item => {
                // 模拟处理延迟
                await Task.Delay(10);
                
                // 转换数据
                return new {
                    item.Id,
                    item.Value,
                    item.Category,
                    ProcessedValue = item.Value * 10,
                    IsEven = item.Id % 2 == 0,
                    ProcessedAt = DateTime.Now
                };
            },
            4);
        stopwatch.Stop();
        
        Console.WriteLine($"\n2. 处理结果:");
        Console.WriteLine($"   成功: {result.Success}");
        Console.WriteLine($"   处理项数: {result.ProcessedItems}");
        Console.WriteLine($"   执行时间: {result.ExecutionTimeMs} ms");
        Console.WriteLine($"   计算执行时间: {stopwatch.ElapsedMilliseconds} ms");
        
        if (result.Success && result.ResultData != null)
        {
            Console.WriteLine($"\n3. 部分处理结果:");
            foreach (var item in result.ResultData.Take(5))
            {
                Console.WriteLine($"   ID: {item.Id}, 原始值: {item.Value}, 处理后值: {item.ProcessedValue}, 分类: {item.Category}, 是否偶数: {item.IsEven}");
            }
            Console.WriteLine($"   ... 显示前5项，共 {result.ResultData.Count} 项");
        }
        
        Console.WriteLine("\n" + "=" * 60);
        Console.WriteLine("简单数据流处理示例完成！");
    }
}
```

### 2. 批处理数据流示例

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dataflow.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dataflow AOT 批处理数据流示例");
        Console.WriteLine("=" * 60);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dataflow选项
        builder.Configuration.AddJsonFile("dataflow_aot.setting.json", optional: true);
        builder.Services.Configure<DataflowOptions>(builder.Configuration.GetSection("Dataflow"));
        
        // 注册服务
        builder.Services.AddDataflow();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dataflow AOT引擎
        var engine = serviceProvider.GetRequiredService<DataflowAotEngine>();
        
        // 准备测试数据
        var testData = Enumerable.Range(1, 200).Select(i => new {
            Id = i,
            Value = i * 3,
            Group = i / 20
        }).ToList();
        
        Console.WriteLine($"\n1. 输入数据: {testData.Count} 项");
        Console.WriteLine($"   批大小: 20");
        Console.WriteLine($"   并行度: 2");
        
        // 执行批处理数据流
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await engine.ProcessBatchFlowAsync(
            testData,
            async batch => {
                // 模拟批处理延迟
                await Task.Delay(50);
                
                // 处理批次
                var groupId = batch.First().Group;
                var sum = batch.Sum(item => item.Value);
                var average = batch.Average(item => item.Value);
                
                // 返回处理结果
                return batch.Select(item => new {
                    item.Id,
                    item.Value,
                    item.Group,
                    BatchId = groupId,
                    BatchSum = sum,
                    BatchAverage = Math.Round(average, 2),
                    BatchSize = batch.Count,
                    ProcessedAt = DateTime.Now
                }).ToList();
            },
            20, // 批大小
            2   // 并行度
        );
        stopwatch.Stop();
        
        Console.WriteLine($"\n2. 处理结果:");
        Console.WriteLine($"   成功: {result.Success}");
        Console.WriteLine($"   处理项数: {result.ProcessedItems}");
        Console.WriteLine($"   执行时间: {result.ExecutionTimeMs} ms");
        Console.WriteLine($"   计算执行时间: {stopwatch.ElapsedMilliseconds} ms");
        
        if (result.Success && result.ResultData != null)
        {
            Console.WriteLine($"\n3. 部分处理结果:");
            var uniqueBatches = result.ResultData.GroupBy(item => item.BatchId).Take(3);
            
            foreach (var batchGroup in uniqueBatches)
            {
                Console.WriteLine($"   \n   批次 {batchGroup.Key}:");
                var batchItems = batchGroup.Take(3).ToList();
                foreach (var item in batchItems)
                {
                    Console.WriteLine($"      ID: {item.Id}, 值: {item.Value}, 批总和: {item.BatchSum}, 批平均值: {item.BatchAverage}");
                }
                Console.WriteLine($"      ... 共 {batchGroup.Count()} 项");
            }
        }
        
        Console.WriteLine("\n" + "=" * 60);
        Console.WriteLine("批处理数据流示例完成！");
    }
}
```

### 3. 复杂多步骤数据流示例

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dataflow.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dataflow AOT 复杂多步骤数据流示例");
        Console.WriteLine("=" * 60);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dataflow选项
        builder.Configuration.AddJsonFile("dataflow_aot.setting.json", optional: true);
        builder.Services.Configure<DataflowOptions>(builder.Configuration.GetSection("Dataflow"));
        
        // 注册服务
        builder.Services.AddDataflow();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dataflow AOT引擎
        var engine = serviceProvider.GetRequiredService<DataflowAotEngine>();
        
        // 准备测试数据
        var testData = Enumerable.Range(1, 150).Select(i => new {
            Id = i,
            RawValue = i * 5,
            Source = "Test",
            Priority = i % 4 == 0 ? "High" : i % 4 == 1 ? "Medium" : "Low"
        }).ToList();
        
        Console.WriteLine($"\n1. 输入数据: {testData.Count} 项");
        Console.WriteLine($"   并行度: 3");
        
        // 执行复杂多步骤数据流
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await engine.ProcessComplexFlowAsync(
            testData,
            // 第一步转换：预处理数据
            async item => {
                await Task.Delay(5);
                
                return new {
                    item.Id,
                    item.RawValue,
                    item.Source,
                    item.Priority,
                    NormalizedValue = item.RawValue / 5,
                    IsValid = item.RawValue > 10,
                    PreprocessedAt = DateTime.Now
                };
            },
            // 第二步转换：进一步处理数据
            async item => {
                await Task.Delay(8);
                
                // 根据优先级调整处理
                var multiplier = item.Priority switch {
                    "High" => 1.5,
                    "Medium" => 1.0,
                    "Low" => 0.5,
                    _ => 1.0
                };
                
                var finalValue = item.NormalizedValue * multiplier;
                
                return new {
                    item.Id,
                    item.RawValue,
                    item.NormalizedValue,
                    item.Priority,
                    item.IsValid,
                    FinalValue = Math.Round(finalValue, 2),
                    Multiplier = multiplier,
                    ProcessedAt = DateTime.Now
                };
            },
            3 // 并行度
        );
        stopwatch.Stop();
        
        Console.WriteLine($"\n2. 处理结果:");
        Console.WriteLine($"   成功: {result.Success}");
        Console.WriteLine($"   处理项数: {result.ProcessedItems}");
        Console.WriteLine($"   执行时间: {result.ExecutionTimeMs} ms");
        Console.WriteLine($"   计算执行时间: {stopwatch.ElapsedMilliseconds} ms");
        
        if (result.Success && result.ResultData != null)
        {
            Console.WriteLine($"\n3. 结果统计:");
            var highPriorityCount = result.ResultData.Count(item => item.Priority == "High");
            var mediumPriorityCount = result.ResultData.Count(item => item.Priority == "Medium");
            var lowPriorityCount = result.ResultData.Count(item => item.Priority == "Low");
            var validCount = result.ResultData.Count(item => item.IsValid);
            var averageFinalValue = result.ResultData.Average(item => item.FinalValue);
            
            Console.WriteLine($"   高优先级: {highPriorityCount} 项");
            Console.WriteLine($"   中优先级: {mediumPriorityCount} 项");
            Console.WriteLine($"   低优先级: {lowPriorityCount} 项");
            Console.WriteLine($"   有效项: {validCount} 项");
            Console.WriteLine($"   平均最终值: {Math.Round(averageFinalValue, 2)}");
            
            Console.WriteLine($"\n4. 部分处理结果:");
            foreach (var item in result.ResultData.Take(5))
            {
                Console.WriteLine($"   ID: {item.Id}, 原始值: {item.RawValue}, 优先级: {item.Priority}, 最终值: {item.FinalValue}, 有效: {item.IsValid}");
            }
        }
        
        Console.WriteLine("\n" + "=" * 60);
        Console.WriteLine("复杂多步骤数据流示例完成！");
    }
}
```

### 4. 高性能管道处理示例

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dataflow.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Dataflow AOT 高性能管道处理示例");
        Console.WriteLine("=" * 60);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置Dataflow选项
        builder.Configuration.AddJsonFile("dataflow_aot.setting.json", optional: true);
        builder.Services.Configure<DataflowOptions>(builder.Configuration.GetSection("Dataflow"));
        
        // 注册服务
        builder.Services.AddDataflow();
        
        // 构建主机并获取服务提供者
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取Dataflow AOT引擎
        var engine = serviceProvider.GetRequiredService<DataflowAotEngine>();
        
        // 准备测试数据
        var testData = Enumerable.Range(1, 300).Select(i => new {
            Id = i,
            Value = i * 4,
            Status = "Raw"
        }).ToList();
        
        Console.WriteLine($"\n1. 输入数据: {testData.Count} 项");
        Console.WriteLine($"   管道步骤: 4");
        Console.WriteLine($"   并行度: 4");
        
        // 定义管道步骤
        var pipelineSteps = new List<Func<dynamic, Task<dynamic>>> {
            // 步骤1: 验证数据
            async item => {
                await Task.Delay(3);
                return new {
                    item.Id,
                    item.Value,
                    item.Status,
                    IsValid = item.Value > 20,
                    Step1Completed = true,
                    Step1Time = DateTime.Now
                };
            },
            // 步骤2: 转换数据
            async item => {
                await Task.Delay(4);
                return new {
                    item.Id,
                    OriginalValue = item.Value,
                    TransformedValue = item.Value * 2,
                    item.Status,
                    item.IsValid,
                    item.Step1Completed,
                    Step2Completed = true,
                    Step2Time = DateTime.Now
                };
            },
            // 步骤3: 根据有效性过滤和处理
            async item => {
                await Task.Delay(5);
                
                // 根据有效性处理
                if (item.IsValid)
                {
                    return new {
                        item.Id,
                        item.OriginalValue,
                        item.TransformedValue,
                        Status = "Processed",
                        item.IsValid,
                        item.Step1Completed,
                        item.Step2Completed,
                        Step3Completed = true,
                        Step3Time = DateTime.Now,
                        Priority = item.TransformedValue > 500 ? "High" : "Normal"
                    };
                }
                else
                {
                    return new {
                        item.Id,
                        item.OriginalValue,
                        item.TransformedValue,
                        Status = "Rejected",
                        item.IsValid,
                        item.Step1Completed,
                        item.Step2Completed,
                        Step3Completed = true,
                        Step3Time = DateTime.Now,
                        Priority = "Low"
                    };
                }
            },
            // 步骤4: 最终处理
            async item => {
                await Task.Delay(3);
                return new {
                    item.Id,
                    item.OriginalValue,
                    item.TransformedValue,
                    item.Status,
                    item.IsValid,
                    item.Priority,
                    AllStepsCompleted = true,
                    FinalProcessedAt = DateTime.Now,
                    ProcessingTime = DateTime.Now - item.Step1Time
                };
            }
        };
        
        // 执行管道处理
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await engine.ProcessPipelineAsync(
            testData.Cast<dynamic>().ToList(),
            pipelineSteps,
            4 // 并行度
        );
        stopwatch.Stop();
        
        Console.WriteLine($"\n2. 处理结果:");
        Console.WriteLine($"   成功: {result.Success}");
        Console.WriteLine($"   处理项数: {result.ProcessedItems}");
        Console.WriteLine($"   执行时间: {result.ExecutionTimeMs} ms");
        Console.WriteLine($"   计算执行时间: {stopwatch.ElapsedMilliseconds} ms");
        
        if (result.Success && result.ResultData != null)
        {
            Console.WriteLine($"\n3. 结果统计:");
            var processedCount = result.ResultData.Count(item => item.Status == "Processed");
            var rejectedCount = result.ResultData.Count(item => item.Status == "Rejected");
            var highPriorityCount = result.ResultData.Count(item => item.Priority == "High");
            var normalPriorityCount = result.ResultData.Count(item => item.Priority == "Normal");
            var lowPriorityCount = result.ResultData.Count(item => item.Priority == "Low");
            
            Console.WriteLine($"   已处理: {processedCount} 项");
            Console.WriteLine($"   已拒绝: {rejectedCount} 项");
            Console.WriteLine($"   高优先级: {highPriorityCount} 项");
            Console.WriteLine($"   正常优先级: {normalPriorityCount} 项");
            Console.WriteLine($"   低优先级: {lowPriorityCount} 项");
            
            Console.WriteLine($"\n4. 部分处理结果:");
            foreach (var item in result.ResultData.Take(5))
            {
                Console.WriteLine($"   ID: {item.Id}, 原始值: {item.OriginalValue}, 转换值: {item.TransformedValue}, 状态: {item.Status}, 优先级: {item.Priority}");
            }
        }
        
        Console.WriteLine("\n" + "=" * 60);
        Console.WriteLine("高性能管道处理示例完成！");
    }
}
```

### 5. 命令行使用示例

```bash
# 获取服务状态
dataflow_aot.exe status

# 重置服务状态
dataflow_aot.exe reset

# 运行演示数据流
dataflow_aot.exe demo
```

## 总结

以上示例展示了Dataflow AOT技能的主要功能和使用方法，通过这些示例，您可以：

1. **简单数据流处理** - 学习如何执行基本的单步骤数据流转换
2. **批处理数据流** - 了解如何按批次处理数据，提高处理效率
3. **复杂多步骤数据流** - 掌握如何构建多阶段数据流处理流程
4. **高性能管道处理** - 学习如何构建可扩展的管道处理流程
5. **命令行使用** - 了解如何通过命令行使用Dataflow AOT功能

Dataflow AOT设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。它基于AOT编译，提供原生性能，同时支持灵活的配置和扩展，是构建高性能数据处理应用的理想选择。
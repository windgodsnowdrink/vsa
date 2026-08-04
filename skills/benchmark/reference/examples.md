# benchmark - 使用示例

## 快速开始

### 1. 基本基准测试示例

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchmarkDemo
{
    // 定义基准测试类
    public class MyBenchmark
    {
        // 基准测试方法
        [Benchmark]
        public void StringConcatenation()
        {
            string result = string.Empty;
            for (int i = 0; i < 1000; i++)
            {
                result += i.ToString();
            }
        }

        // 另一个基准测试方法
        [Benchmark]
        public void StringBuilderUsage()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                sb.Append(i.ToString());
            }
            var result = sb.ToString();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("基准测试示例 - 字符串操作性能比较");
            Console.WriteLine("=" * 50);
            
            // 运行基准测试
            BenchmarkRunner.Run<MyBenchmark>();
        }
    }
}
```

### 2. AOT 优化基准测试示例

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Runtime.CompilerServices;

namespace BenchmarkDemo
{
    // 支持 AOT 编译的基准测试类
    public class AotBenchmark
    {
        // 使用 AggressiveOptimization 提示编译器优化
        [Benchmark]
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public int OptimizedSum()
        {
            int sum = 0;
            for (int i = 0; i < 1000000; i++)
            {
                sum += i;
            }
            return sum;
        }

        // 普通方法用于比较
        [Benchmark]
        public int RegularSum()
        {
            int sum = 0;
            for (int i = 0; i < 1000000; i++)
            {
                sum += i;
            }
            return sum;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("AOT 优化基准测试示例");
            Console.WriteLine("=" * 50);
            
            // 运行基准测试
            BenchmarkRunner.Run<AotBenchmark>();
        }
    }
}
```

### 3. 内存性能基准测试示例

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;

namespace BenchmarkDemo
{
    // 启用内存诊断的基准测试类
    [MemoryDiagnoser]
    public class MemoryBenchmark
    {
        [Benchmark]
        public void ArrayAllocation()
        {
            var array = new int[1000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }
        }

        [Benchmark]
        public void ListAllocation()
        {
            var list = new System.Collections.Generic.List<int>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add(i);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("内存性能基准测试示例");
            Console.WriteLine("=" * 50);
            
            // 运行基准测试
            BenchmarkRunner.Run<MemoryBenchmark>();
        }
    }
}
```

### 4. 并发基准测试示例

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace BenchmarkDemo
{
    public class ConcurrentBenchmark
    {
        private readonly object _lock = new object();
        private int _counter = 0;

        [Benchmark]
        public void LockedIncrement()
        {
            Parallel.For(0, 1000, _ =>
            {
                lock (_lock)
                {
                    _counter++;
                }
            });
        }

        [Benchmark]
        public void InterlockedIncrement()
        {
            Parallel.For(0, 1000, _ =>
            {
                System.Threading.Interlocked.Increment(ref _counter);
            });
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("并发基准测试示例");
            Console.WriteLine("=" * 50);
            
            // 运行基准测试
            BenchmarkRunner.Run<ConcurrentBenchmark>();
        }
    }
}
```

## 总结

以上示例展示了 benchmark 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始基本基准测试
2. 实现 AOT 优化的高性能基准测试
3. 测试内存使用性能
4. 测试并发操作性能

系统设计遵循 .NET 10 最佳实践，支持 AOT 编译，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

所有示例都基于 BenchmarkDotNet 框架，提供详细的性能报告，帮助您深入了解代码的性能特征，从而进行针对性优化。

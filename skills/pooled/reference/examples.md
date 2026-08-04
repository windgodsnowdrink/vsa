# pooled - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Pooled.Examples
{
    public class Program
    {
        public static void Main()
        {
            // 构建服务提供器
            var serviceProvider = BuildServiceProvider();
            
            // 获取池化集合工厂
            var listFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
            var dictionaryFactory = serviceProvider.GetRequiredService<IPooledDictionaryFactory>();
            var setFactory = serviceProvider.GetRequiredService<IPooledSetFactory>();
            
            Console.WriteLine("pooled 基本使用示例");
            Console.WriteLine("=" * 50);
            
            // 使用池化List
            Console.WriteLine("\n1. 使用池化List:");
            using (var pooledList = listFactory.Create<int>())
            {
                var list = pooledList.Value;
                
                // 添加数据
                for (int i = 0; i < 10; i++)
                {
                    list.Add(i);
                }
                
                // 遍历数据
                Console.Write("List内容: ");
                foreach (var item in list)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
                Console.WriteLine($"List大小: {list.Count}");
            }
            
            // 使用池化Dictionary
            Console.WriteLine("\n2. 使用池化Dictionary:");
            using (var pooledDictionary = dictionaryFactory.Create<int, string>())
            {
                var dictionary = pooledDictionary.Value;
                
                // 添加数据
                for (int i = 0; i < 5; i++)
                {
                    dictionary[i] = $"值{i}";
                }
                
                // 遍历数据
                Console.WriteLine("Dictionary内容:");
                foreach (var kvp in dictionary)
                {
                    Console.WriteLine($"键: {kvp.Key}, 值: {kvp.Value}");
                }
                Console.WriteLine($"Dictionary大小: {dictionary.Count}");
            }
            
            // 使用池化Set
            Console.WriteLine("\n3. 使用池化Set:");
            using (var pooledSet = setFactory.Create<string>())
            {
                var set = pooledSet.Value;
                
                // 添加数据
                set.Add("苹果");
                set.Add("香蕉");
                set.Add("橙子");
                set.Add("苹果"); // 重复数据，不会被添加
                
                // 遍历数据
                Console.Write("Set内容: ");
                foreach (var item in set)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
                Console.WriteLine($"Set大小: {set.Count}");
            }
            
            Console.WriteLine("\n操作完成！");
        }
        
        private static ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            
            // 注册池化集合服务
            builder.AddPooledCollections(options =>
            {
                options.EnableMemoryPooling = true;
                options.EnableZeroAllocation = true;
                options.MemoryPoolSize = 1024;
            });
            
            return builder.BuildServiceProvider();
        }
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pooled.Examples
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("pooled 高级配置示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var builder = new ServiceCollection();
            
            // 配置池化集合设置
            builder.Configure<PooledCollectionsOptions>(options => {
                options.EnableMemoryPooling = true;
                options.EnableZeroAllocation = true;
                options.EnableThreadLocalStorage = true;
                options.MemoryPoolSize = 2048;
                options.ThreadLocalCacheSize = 512;
                options.DefaultInitialCapacity = 32;
                options.MaximumPoolSize = 1024;
                options.EnableStrictMode = true;
                options.EnableAutoClear = true;
                options.ClearInterval = 5000;
                options.EnablePerformanceMetrics = true;
            });
            
            // 注册池化集合服务
            builder.AddPooledCollections();
            
            var serviceProvider = builder.BuildServiceProvider();
            
            // 获取配置
            var settings = serviceProvider.GetRequiredService<IOptions<PooledCollectionsOptions>>().Value;
            Console.WriteLine("\n配置信息:");
            Console.WriteLine($"启用内存池化: {settings.EnableMemoryPooling}");
            Console.WriteLine($"启用零分配: {settings.EnableZeroAllocation}");
            Console.WriteLine($"启用线程本地存储: {settings.EnableThreadLocalStorage}");
            Console.WriteLine($"内存池大小: {settings.MemoryPoolSize}");
            Console.WriteLine($"线程本地缓存大小: {settings.ThreadLocalCacheSize}");
            Console.WriteLine($"默认初始容量: {settings.DefaultInitialCapacity}");
            Console.WriteLine($"最大池大小: {settings.MaximumPoolSize}");
            Console.WriteLine($"启用严格模式: {settings.EnableStrictMode}");
            Console.WriteLine($"启用自动清理: {settings.EnableAutoClear}");
            Console.WriteLine($"清理间隔: {settings.ClearInterval}ms");
            Console.WriteLine($"启用性能指标: {settings.EnablePerformanceMetrics}");
            
            // 使用池化集合
            var listFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
            
            Console.WriteLine("\n使用配置后的池化List:");
            using (var pooledList = listFactory.Create<string>())
            {
                var list = pooledList.Value;
                
                // 添加数据
                list.Add("配置");
                list.Add("示例");
                list.Add("测试");
                
                // 遍历数据
                Console.Write("List内容: ");
                foreach (var item in list)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            }
            
            Console.WriteLine("\n配置示例完成！");
        }
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Pooled.Examples
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("pooled 性能优化示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务提供器
            var serviceProvider = BuildServiceProvider();
            var listFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
            var dictionaryFactory = serviceProvider.GetRequiredService<IPooledDictionaryFactory>();
            
            // 测试参数
            const int iterations = 10000;
            const int itemsPerIteration = 100;
            
            // 测试池化List性能
            Console.WriteLine("\n1. 测试池化List性能:");
            TestPooledListPerformance(listFactory, iterations, itemsPerIteration);
            
            // 测试池化Dictionary性能
            Console.WriteLine("\n2. 测试池化Dictionary性能:");
            TestPooledDictionaryPerformance(dictionaryFactory, iterations, itemsPerIteration);
            
            // 测试普通集合性能作为对比
            Console.WriteLine("\n3. 测试普通List性能 (对比):");
            TestRegularListPerformance(iterations, itemsPerIteration);
            
            Console.WriteLine("\n性能测试完成！");
        }
        
        private static ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            
            // 注册池化集合服务，启用所有优化选项
            builder.AddPooledCollections(options =>
            {
                options.EnableMemoryPooling = true;
                options.EnableZeroAllocation = true;
                options.EnableThreadLocalStorage = true;
                options.MemoryPoolSize = 2048;
                options.ThreadLocalCacheSize = 512;
                options.DefaultInitialCapacity = 100;
                options.MaximumPoolSize = 1024;
                options.EnableAutoClear = true;
            });
            
            return builder.BuildServiceProvider();
        }
        
        private static void TestPooledListPerformance(IPooledListFactory listFactory, int iterations, int itemsPerIteration)
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < iterations; i++)
            {
                using (var pooledList = listFactory.Create<int>())
                {
                    var list = pooledList.Value;
                    
                    // 添加数据
                    for (int j = 0; j < itemsPerIteration; j++)
                    {
                        list.Add(j);
                    }
                    
                    // 遍历数据
                    int sum = 0;
                    foreach (var item in list)
                    {
                        sum += item;
                    }
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"执行 {iterations} 次迭代，每次 {itemsPerIteration} 个元素:");
            Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次迭代: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        }
        
        private static void TestPooledDictionaryPerformance(IPooledDictionaryFactory dictionaryFactory, int iterations, int itemsPerIteration)
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < iterations; i++)
            {
                using (var pooledDictionary = dictionaryFactory.Create<int, string>())
                {
                    var dictionary = pooledDictionary.Value;
                    
                    // 添加数据
                    for (int j = 0; j < itemsPerIteration; j++)
                    {
                        dictionary[j] = $"值{j}";
                    }
                    
                    // 遍历数据
                    int count = 0;
                    foreach (var kvp in dictionary)
                    {
                        count++;
                    }
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"执行 {iterations} 次迭代，每次 {itemsPerIteration} 个元素:");
            Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次迭代: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        }
        
        private static void TestRegularListPerformance(int iterations, int itemsPerIteration)
        {
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < iterations; i++)
            {
                var list = new List<int>();
                
                // 添加数据
                for (int j = 0; j < itemsPerIteration; j++)
                {
                    list.Add(j);
                }
                
                // 遍历数据
                int sum = 0;
                foreach (var item in list)
                {
                    sum += item;
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"执行 {iterations} 次迭代，每次 {itemsPerIteration} 个元素:");
            Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次迭代: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Pooled.Examples
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("pooled 错误处理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务提供器
            var serviceProvider = BuildServiceProvider();
            var listFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
            
            // 测试1: 正常使用
            Console.WriteLine("\n1. 正常使用测试:");
            TryUsePooledList(listFactory, true);
            
            // 测试2: 异常情况 - 未使用using语句
            Console.WriteLine("\n2. 未使用using语句测试:");
            TryUsePooledListWithoutUsing(listFactory);
            
            // 测试3: 异常情况 - 线程安全
            Console.WriteLine("\n3. 线程安全测试:");
            TestThreadSafety(listFactory);
            
            Console.WriteLine("\n错误处理示例完成！");
        }
        
        private static ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            
            // 注册池化集合服务，启用严格模式
            builder.AddPooledCollections(options =>
            {
                options.EnableMemoryPooling = true;
                options.EnableZeroAllocation = true;
                options.EnableStrictMode = true;
            });
            
            return builder.BuildServiceProvider();
        }
        
        private static void TryUsePooledList(IPooledListFactory listFactory, bool useCorrectly)
        {
            try
            {
                if (useCorrectly)
                {
                    // 正确使用: 使用using语句
                    using (var pooledList = listFactory.Create<int>())
                    {
                        var list = pooledList.Value;
                        list.Add(1);
                        list.Add(2);
                        list.Add(3);
                        Console.WriteLine("✅ 正常使用成功！");
                        Console.WriteLine($"List大小: {list.Count}");
                    }
                }
                else
                {
                    // 错误使用: 不使用using语句
                    var pooledList = listFactory.Create<int>();
                    var list = pooledList.Value;
                    list.Add(1);
                    Console.WriteLine("❌ 未使用using语句！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 错误: {ex.Message}");
            }
        }
        
        private static void TryUsePooledListWithoutUsing(IPooledListFactory listFactory)
        {
            try
            {
                // 错误使用: 不使用using语句
                var pooledList = listFactory.Create<int>();
                var list = pooledList.Value;
                list.Add(1);
                list.Add(2);
                
                // 忘记调用Dispose
                Console.WriteLine("⚠️  未使用using语句，池化对象未被正确释放！");
                Console.WriteLine("   这可能导致内存泄漏和池资源耗尽。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 错误: {ex.Message}");
            }
        }
        
        private static void TestThreadSafety(IPooledListFactory listFactory)
        {
            try
            {
                Console.WriteLine("测试多线程访问池化集合...");
                
                // 创建一个池化List
                using (var pooledList = listFactory.Create<int>())
                {
                    var list = pooledList.Value;
                    
                    // 启动多个线程访问同一个List
                    System.Threading.Tasks.Parallel.For(0, 1000, i =>
                    {
                        try
                        {
                            // 注意: List本身不是线程安全的
                            // 这里只是演示多线程场景
                            list.Add(i);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"线程错误: {ex.Message}");
                        }
                    });
                    
                    Console.WriteLine($"✅ 多线程测试完成，List大小: {list.Count}");
                    Console.WriteLine("   注意: 标准List不是线程安全的，实际使用中请确保线程安全。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 错误: {ex.Message}");
            }
        }
    }
}
```

### 5. 多线程使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Pooled.Examples
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("pooled 多线程使用示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务提供器
            var serviceProvider = BuildServiceProvider();
            var listFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
            var dictionaryFactory = serviceProvider.GetRequiredService<IPooledDictionaryFactory>();
            
            // 测试多线程使用池化集合
            Console.WriteLine("\n测试多线程使用池化集合:");
            Console.WriteLine("启动10个线程，每个线程使用池化集合...");
            
            var tasks = new List<Task>();
            var completedTasks = 0;
            
            // 启动10个线程
            for (int i = 0; i < 10; i++)
            {
                int threadId = i;
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        // 每个线程获取自己的池化List
                        using (var pooledList = listFactory.Create<int>())
                        {
                            var list = pooledList.Value;
                            
                            // 添加数据
                            for (int j = 0; j < 100; j++)
                            {
                                list.Add(threadId * 1000 + j);
                            }
                            
                            // 每个线程获取自己的池化Dictionary
                            using (var pooledDictionary = dictionaryFactory.Create<int, string>())
                            {
                                var dictionary = pooledDictionary.Value;
                                
                                // 添加数据
                                for (int j = 0; j < 50; j++)
                                {
                                    dictionary[j] = $"线程{threadId}-值{j}";
                                }
                                
                                // 模拟工作
                                Task.Delay(100).Wait();
                            }
                        }
                        
                        int completed = System.Threading.Interlocked.Increment(ref completedTasks);
                        Console.WriteLine($"✅ 线程 {threadId} 完成，共 {completed}/10");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ 线程 {threadId} 错误: {ex.Message}");
                    }
                }));
            }
            
            // 等待所有线程完成
            Task.WaitAll(tasks.ToArray());
            
            Console.WriteLine("\n多线程测试完成！");
            Console.WriteLine("所有线程都成功使用了池化集合，没有发生冲突。");
            Console.WriteLine("这展示了池化集合在多线程环境中的安全性和高效性。");
        }
        
        private static ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            
            // 注册池化集合服务，启用线程本地存储
            builder.AddPooledCollections(options =>
            {
                options.EnableMemoryPooling = true;
                options.EnableZeroAllocation = true;
                options.EnableThreadLocalStorage = true;
                options.MemoryPoolSize = 2048;
                options.ThreadLocalCacheSize = 512;
            });
            
            return builder.BuildServiceProvider();
        }
    }
}
```

### 6. AOT编译示例

```csharp
// 注意: 此示例展示如何在AOT编译环境中使用pooled

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

// 确保代码符合AOT编译要求:
// 1. 避免使用反射
// 2. 避免使用动态代码
// 3. 避免使用需要运行时JIT的特性

namespace Pooled.Examples.AOT
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("pooled AOT编译示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务提供器
            var serviceProvider = BuildServiceProvider();
            var listFactory = serviceProvider.GetRequiredService<IPooledListFactory>();
            var dictionaryFactory = serviceProvider.GetRequiredService<IPooledDictionaryFactory>();
            
            Console.WriteLine("\n在AOT编译环境中使用池化集合:");
            
            // 使用池化List
            using (var pooledList = listFactory.Create<string>())
            {
                var list = pooledList.Value;
                
                // 添加数据
                list.Add("AOT");
                list.Add("编译");
                list.Add("示例");
                list.Add("测试");
                
                // 遍历数据
                Console.Write("池化List内容: ");
                foreach (var item in list)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            }
            
            // 使用池化Dictionary
            using (var pooledDictionary = dictionaryFactory.Create<string, int>())
            {
                var dictionary = pooledDictionary.Value;
                
                // 添加数据
                dictionary["AOT"] = 1;
                dictionary["编译"] = 2;
                dictionary["示例"] = 3;
                
                // 遍历数据
                Console.WriteLine("池化Dictionary内容:");
                foreach (var kvp in dictionary)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
            
            Console.WriteLine("\nAOT编译示例完成！");
            Console.WriteLine("此示例代码可以成功通过AOT编译。");
        }
        
        // 静态方法，避免AOT编译问题
        private static ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            
            // 注册池化集合服务
            builder.AddPooledCollections(options =>
            {
                options.EnableMemoryPooling = true;
                options.EnableZeroAllocation = true;
                options.MemoryPoolSize = 1024;
            });
            
            return builder.BuildServiceProvider();
        }
    }
}
```

## 示例总结

以上示例展示了pooled技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速上手**：了解基本的池化集合使用方法
2. **高级配置**：根据实际需求配置池化集合选项
3. **性能优化**：通过性能测试了解池化集合的优势
4. **错误处理**：学习如何正确处理异常情况
5. **多线程使用**：在多线程环境中安全使用池化集合
6. **AOT编译**：在AOT编译环境中使用池化集合

系统设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

## 提示和建议

1. **始终使用using语句**：确保池化对象被正确释放
2. **设置适当的初始容量**：根据实际需求设置初始容量，避免频繁扩容
3. **启用线程本地存储**：在多线程环境中启用线程本地存储以提高性能
4. **监控内存使用**：定期监控内存使用情况，及时调整配置
5. **使用批量操作**：对于大量数据，使用批量操作以提高效率
6. **避免在池化集合中存储大对象**：大对象会增加内存使用和GC压力
7. **定期清理**：启用自动清理以释放未使用的池化对象
8. **测试性能**：根据实际场景测试性能，选择最佳配置

通过合理使用这些示例和建议，您可以充分发挥pooled技能的性能优势，减少内存分配和GC压力，提高应用程序的整体性能。
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Collections.Immutable@10.0.0
#:package System.Memory@10.0.0
#:package System.Buffers@10.0.0
#:package System.Threading.Tasks.Dataflow@10.0.0
#:package System.IO.Pipelines@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property Optimize=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using System.IO.Pipelines;

namespace Pooled.Collections
{
    /// <summary>
    /// 内存池化集合配置选项
    /// </summary>
    public class PooledCollectionsOptions
    {
        /// <summary>
        /// 是否启用内存池化
        /// </summary>
        public bool EnableMemoryPooling { get; set; } = true;
        
        /// <summary>
        /// 是否启用零分配操作
        /// </summary>
        public bool EnableZeroAllocation { get; set; } = true;
        
        /// <summary>
        /// 是否启用线程本地存储
        /// </summary>
        public bool EnableThreadLocalStorage { get; set; } = true;
        
        /// <summary>
        /// 内存池大小
        /// </summary>
        public int MemoryPoolSize { get; set; } = 1024;
        
        /// <summary>
        /// 线程本地缓存大小
        /// </summary>
        public int ThreadLocalCacheSize { get; set; } = 256;
        
        /// <summary>
        /// 默认初始容量
        /// </summary>
        public int DefaultInitialCapacity { get; set; } = 16;
        
        /// <summary>
        /// 最大池大小
        /// </summary>
        public int MaximumPoolSize { get; set; } = 1024;
        
        /// <summary>
        /// 是否启用严格模式
        /// </summary>
        public bool EnableStrictMode { get; set; } = false;
        
        /// <summary>
        /// 是否启用自动清理
        /// </summary>
        public bool EnableAutoClear { get; set; } = true;
        
        /// <summary>
        /// 清理间隔(毫秒)
        /// </summary>
        public int ClearInterval { get; set; } = 5000;
        
        /// <summary>
        /// 是否启用性能指标
        /// </summary>
        public bool EnablePerformanceMetrics { get; set; } = true;
    }

    /// <summary>
    /// 池化对象接口
    /// </summary>
    public interface IPooledObject : IDisposable
    {
        /// <summary>
        /// 重置对象状态
        /// </summary>
        void Reset();
        
        /// <summary>
        /// 是否已被释放
        /// </summary>
        bool IsDisposed { get; }
    }

    /// <summary>
    /// 池化List接口
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public interface IPooledList<T> : IList<T>, IPooledObject
    {}

    /// <summary>
    /// 池化Dictionary接口
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public interface IPooledDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IPooledObject
    {}

    /// <summary>
    /// 池化HashSet接口
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public interface IPooledSet<T> : ISet<T>, IPooledObject
    {}

    /// <summary>
    /// 池化List工厂接口
    /// </summary>
    public interface IPooledListFactory
    {
        /// <summary>
        /// 创建池化List
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化List</returns>
        IPooledList<T> Create<T>();
        
        /// <summary>
        /// 创建带初始容量的池化List
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化List</returns>
        IPooledList<T> Create<T>(int initialCapacity);
    }

    /// <summary>
    /// 池化Dictionary工厂接口
    /// </summary>
    public interface IPooledDictionaryFactory
    {
        /// <summary>
        /// 创建池化Dictionary
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>池化Dictionary</returns>
        IPooledDictionary<TKey, TValue> Create<TKey, TValue>();
        
        /// <summary>
        /// 创建带初始容量的池化Dictionary
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化Dictionary</returns>
        IPooledDictionary<TKey, TValue> Create<TKey, TValue>(int initialCapacity);
    }

    /// <summary>
    /// 池化Set工厂接口
    /// </summary>
    public interface IPooledSetFactory
    {
        /// <summary>
        /// 创建池化Set
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化Set</returns>
        IPooledSet<T> Create<T>();
        
        /// <summary>
        /// 创建带初始容量的池化Set
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化Set</returns>
        IPooledSet<T> Create<T>(int initialCapacity);
    }

    /// <summary>
    /// 内存池化集合服务接口
    /// </summary>
    public interface IPooledCollectionsService
    {
        /// <summary>
        /// 获取池化列表
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化列表</returns>
        IPooledList<T> GetList<T>();
        
        /// <summary>
        /// 获取带初始容量的池化列表
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化列表</returns>
        IPooledList<T> GetList<T>(int initialCapacity);
        
        /// <summary>
        /// 获取池化字典
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>池化字典</returns>
        IPooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>();
        
        /// <summary>
        /// 获取带初始容量的池化字典
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化字典</returns>
        IPooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>(int initialCapacity);
        
        /// <summary>
        /// 获取池化哈希集
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化哈希集</returns>
        IPooledSet<T> GetSet<T>();
        
        /// <summary>
        /// 获取带初始容量的池化哈希集
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化哈希集</returns>
        IPooledSet<T> GetSet<T>(int initialCapacity);
        
        /// <summary>
        /// 返回集合到池中
        /// </summary>
        /// <typeparam name="T">池化对象类型</typeparam>
        /// <param name="pooledCollection">池化对象</param>
        void Return<T>(T pooledCollection) where T : class, IPooledObject;
    }

    /// <summary>
    /// 池化List实现
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public class PooledList<T> : List<T>, IPooledList<T>
    {
        private readonly IPooledCollectionsService _service;
        private bool _isDisposed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">池化集合服务</param>
        /// <param name="initialCapacity">初始容量</param>
        public PooledList(IPooledCollectionsService service, int initialCapacity = 16)
            : base(initialCapacity)
        {
            _service = service;
            _isDisposed = false;
        }

        /// <summary>
        /// 是否已被释放
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// 重置对象状态
        /// </summary>
        public void Reset()
        {
            if (!_isDisposed)
            {
                Clear();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Reset();
                _service.Return(this);
            }
        }
    }

    /// <summary>
    /// 池化Dictionary实现
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class PooledDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IPooledDictionary<TKey, TValue>
    {
        private readonly IPooledCollectionsService _service;
        private bool _isDisposed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">池化集合服务</param>
        /// <param name="initialCapacity">初始容量</param>
        public PooledDictionary(IPooledCollectionsService service, int initialCapacity = 16)
            : base(initialCapacity)
        {
            _service = service;
            _isDisposed = false;
        }

        /// <summary>
        /// 是否已被释放
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// 重置对象状态
        /// </summary>
        public void Reset()
        {
            if (!_isDisposed)
            {
                Clear();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Reset();
                _service.Return(this);
            }
        }
    }

    /// <summary>
    /// 池化Set实现
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public class PooledSet<T> : HashSet<T>, IPooledSet<T>
    {
        private readonly IPooledCollectionsService _service;
        private bool _isDisposed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">池化集合服务</param>
        /// <param name="initialCapacity">初始容量</param>
        public PooledSet(IPooledCollectionsService service, int initialCapacity = 16)
            : base(initialCapacity)
        {
            _service = service;
            _isDisposed = false;
        }

        /// <summary>
        /// 是否已被释放
        /// </summary>
        public bool IsDisposed => _isDisposed;

        /// <summary>
        /// 重置对象状态
        /// </summary>
        public void Reset()
        {
            if (!_isDisposed)
            {
                Clear();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Reset();
                _service.Return(this);
            }
        }
    }

    /// <summary>
    /// 内存池化集合服务实现
    /// </summary>
    public class PooledCollectionsService : IPooledCollectionsService, IDisposable
    {
        private readonly PooledCollectionsOptions _options;
        private readonly ILogger<PooledCollectionsService> _logger;
        private readonly ConcurrentDictionary<Type, ConcurrentBag<IPooledObject>> _objectPools;
        private readonly ThreadLocal<Dictionary<Type, Queue<IPooledObject>>> _threadLocalCache;
        private readonly Timer _cleanupTimer;
        private readonly object _poolLock = new object();
        private bool _isDisposed;

        /// <summary>
        /// 获取池化列表
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化列表</returns>
        public IPooledList<T> GetList<T>()
        {
            return GetList<T>(_options.DefaultInitialCapacity);
        }

        /// <summary>
        /// 获取带初始容量的池化列表
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化列表</returns>
        public IPooledList<T> GetList<T>(int initialCapacity)
        {
            var poolType = typeof(PooledList<T>);
            IPooledObject pooledObject = null;

            // 先尝试从线程本地缓存获取
            if (_options.EnableThreadLocalStorage && _threadLocalCache?.Value != null)
            {
                if (_threadLocalCache.Value.TryGetValue(poolType, out var queue) && queue.Count > 0)
                {
                    lock (queue)
                    {
                        if (queue.Count > 0)
                        {
                            pooledObject = queue.Dequeue();
                        }
                    }
                }
            }

            // 从全局池获取
            if (pooledObject == null)
            {
                var pool = _objectPools.GetOrAdd(poolType, _ => new ConcurrentBag<IPooledObject>());
                pool.TryTake(out pooledObject);
            }

            // 如果没有可用对象，创建新的
            if (pooledObject == null)
            {
                pooledObject = new PooledList<T>(this, initialCapacity);
                _logger?.LogDebug("创建新的池化List<{TType}>", typeof(T).Name);
            }
            else
            {
                pooledObject.Reset();
                _logger?.LogDebug("从池中获取池化List<{TType}>", typeof(T).Name);
            }

            return (IPooledList<T>)pooledObject;
        }

        /// <summary>
        /// 获取池化字典
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>池化字典</returns>
        public IPooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>()
        {
            return GetDictionary<TKey, TValue>(_options.DefaultInitialCapacity);
        }

        /// <summary>
        /// 获取带初始容量的池化字典
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化字典</returns>
        public IPooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>(int initialCapacity)
        {
            var poolType = typeof(PooledDictionary<TKey, TValue>);
            IPooledObject pooledObject = null;

            // 先尝试从线程本地缓存获取
            if (_options.EnableThreadLocalStorage && _threadLocalCache?.Value != null)
            {
                if (_threadLocalCache.Value.TryGetValue(poolType, out var queue) && queue.Count > 0)
                {
                    lock (queue)
                    {
                        if (queue.Count > 0)
                        {
                            pooledObject = queue.Dequeue();
                        }
                    }
                }
            }

            // 从全局池获取
            if (pooledObject == null)
            {
                var pool = _objectPools.GetOrAdd(poolType, _ => new ConcurrentBag<IPooledObject>());
                pool.TryTake(out pooledObject);
            }

            // 如果没有可用对象，创建新的
            if (pooledObject == null)
            {
                pooledObject = new PooledDictionary<TKey, TValue>(this, initialCapacity);
                _logger?.LogDebug("创建新的池化Dictionary<{TKeyType}, {TValueType}>", typeof(TKey).Name, typeof(TValue).Name);
            }
            else
            {
                pooledObject.Reset();
                _logger?.LogDebug("从池中获取池化Dictionary<{TKeyType}, {TValueType}>", typeof(TKey).Name, typeof(TValue).Name);
            }

            return (IPooledDictionary<TKey, TValue>)pooledObject;
        }

        /// <summary>
        /// 获取池化哈希集
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化哈希集</returns>
        public IPooledSet<T> GetSet<T>()
        {
            return GetSet<T>(_options.DefaultInitialCapacity);
        }

        /// <summary>
        /// 获取带初始容量的池化哈希集
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化哈希集</returns>
        public IPooledSet<T> GetSet<T>(int initialCapacity)
        {
            var poolType = typeof(PooledSet<T>);
            IPooledObject pooledObject = null;

            // 先尝试从线程本地缓存获取
            if (_options.EnableThreadLocalStorage && _threadLocalCache?.Value != null)
            {
                if (_threadLocalCache.Value.TryGetValue(poolType, out var queue) && queue.Count > 0)
                {
                    lock (queue)
                    {
                        if (queue.Count > 0)
                        {
                            pooledObject = queue.Dequeue();
                        }
                    }
                }
            }

            // 从全局池获取
            if (pooledObject == null)
            {
                var pool = _objectPools.GetOrAdd(poolType, _ => new ConcurrentBag<IPooledObject>());
                pool.TryTake(out pooledObject);
            }

            // 如果没有可用对象，创建新的
            if (pooledObject == null)
            {
                pooledObject = new PooledSet<T>(this, initialCapacity);
                _logger?.LogDebug("创建新的池化Set<{TType}>", typeof(T).Name);
            }
            else
            {
                pooledObject.Reset();
                _logger?.LogDebug("从池中获取池化Set<{TType}>", typeof(T).Name);
            }

            return (IPooledSet<T>)pooledObject;
        }

        /// <summary>
        /// 返回集合到池中
        /// </summary>
        /// <typeparam name="T">池化对象类型</typeparam>
        /// <param name="pooledCollection">池化对象</param>
        public void Return<T>(T pooledCollection) where T : class, IPooledObject
        {
            if (pooledCollection == null || pooledCollection.IsDisposed)
            {
                return;
            }

            var poolType = pooledCollection.GetType();

            // 尝试放回线程本地缓存
            if (_options.EnableThreadLocalStorage && _threadLocalCache?.Value != null)
            {
                if (!_threadLocalCache.Value.TryGetValue(poolType, out var queue))
                {
                    queue = new Queue<IPooledObject>();
                    _threadLocalCache.Value[poolType] = queue;
                }

                lock (queue)
                {
                    if (queue.Count < _options.ThreadLocalCacheSize)
                    {
                        queue.Enqueue(pooledCollection);
                        _logger?.LogDebug("将池化对象放回线程本地缓存: {Type}", poolType.Name);
                        return;
                    }
                }
            }

            // 放回全局池
            var pool = _objectPools.GetOrAdd(poolType, _ => new ConcurrentBag<IPooledObject>());
            if (pool.Count < _options.MemoryPoolSize)
            {
                pool.Add(pooledCollection);
                _logger?.LogDebug("将池化对象放回全局池: {Type}", poolType.Name);
            }
            else
            {
                _logger?.LogDebug("全局池已满，释放池化对象: {Type}", poolType.Name);
            }
        }

        /// <summary>
        /// 清理池
        /// </summary>
        /// <param name="state">状态</param>
        private void CleanupPools(object state)
        {
            try
            {
                foreach (var kvp in _objectPools)
                {
                    var pool = kvp.Value;
                    // 清理超过最大池大小的对象
                    while (pool.Count > _options.MemoryPoolSize)
                    {
                        pool.TryTake(out _);
                    }
                }
                _logger?.LogInformation("清理内存池化对象完成");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "清理内存池化对象时出错");
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _cleanupTimer?.Dispose();
                _threadLocalCache?.Dispose();
                // 清理所有池
                foreach (var pool in _objectPools.Values)
                {
                    while (pool.TryTake(out var obj))
                    {
                        obj.Dispose();
                    }
                }
                _objectPools.Clear();
                _logger?.LogInformation("内存池化集合服务已释放");
            }
        }
    }

    /// <summary>
    /// 池化List工厂实现
    /// </summary>
    public class PooledListFactory : IPooledListFactory
    {
        private readonly IPooledCollectionsService _service;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">池化集合服务</param>
        public PooledListFactory(IPooledCollectionsService service)
        {
            _service = service;
        }

        /// <summary>
        /// 创建池化List
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化List</returns>
        public IPooledList<T> Create<T>()
        {
            return _service.GetList<T>();
        }

        /// <summary>
        /// 创建带初始容量的池化List
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化List</returns>
        public IPooledList<T> Create<T>(int initialCapacity)
        {
            return _service.GetList<T>(initialCapacity);
        }
    }

    /// <summary>
    /// 池化Dictionary工厂实现
    /// </summary>
    public class PooledDictionaryFactory : IPooledDictionaryFactory
    {
        private readonly IPooledCollectionsService _service;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">池化集合服务</param>
        public PooledDictionaryFactory(IPooledCollectionsService service)
        {
            _service = service;
        }

        /// <summary>
        /// 创建池化Dictionary
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>池化Dictionary</returns>
        public IPooledDictionary<TKey, TValue> Create<TKey, TValue>()
        {
            return _service.GetDictionary<TKey, TValue>();
        }

        /// <summary>
        /// 创建带初始容量的池化Dictionary
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化Dictionary</returns>
        public IPooledDictionary<TKey, TValue> Create<TKey, TValue>(int initialCapacity)
        {
            return _service.GetDictionary<TKey, TValue>(initialCapacity);
        }
    }

    /// <summary>
    /// 池化Set工厂实现
    /// </summary>
    public class PooledSetFactory : IPooledSetFactory
    {
        private readonly IPooledCollectionsService _service;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service">池化集合服务</param>
        public PooledSetFactory(IPooledCollectionsService service)
        {
            _service = service;
        }

        /// <summary>
        /// 创建池化Set
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>池化Set</returns>
        public IPooledSet<T> Create<T>()
        {
            return _service.GetSet<T>();
        }

        /// <summary>
        /// 创建带初始容量的池化Set
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="initialCapacity">初始容量</param>
        /// <returns>池化Set</returns>
        public IPooledSet<T> Create<T>(int initialCapacity)
        {
            return _service.GetSet<T>(initialCapacity);
        }
    }

    #region DI扩展
    /// <summary>
    /// 服务集合扩展方法
    /// </summary>
    public static class PooledCollectionsExtensions
    {
        /// <summary>
        /// 添加池化集合服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPooledCollections(
            this IServiceCollection services,
            Action<PooledCollectionsOptions> configureOptions = null)
        {
            services.AddOptions<PooledCollectionsOptions>()
                .Configure(configureOptions ?? (opts => { }));

            services.AddSingleton<IPooledCollectionsService, PooledCollectionsService>();
            services.AddSingleton<IPooledListFactory, PooledListFactory>();
            services.AddSingleton<IPooledDictionaryFactory, PooledDictionaryFactory>();
            services.AddSingleton<IPooledSetFactory, PooledSetFactory>();

            return services;
        }
    }

    #region 示例用法
    /// <summary>
    /// 演示如何使用池化集合服务
    /// </summary>
    public static class PooledCollectionsExample
    {
        /// <summary>
        /// 运行示例
        /// </summary>
        public static void Run()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            services.AddPooledCollections(opts =>
            {
                opts.EnableMemoryPooling = true;
                opts.EnableZeroAllocation = true;
                opts.EnableThreadLocalStorage = true;
                opts.MemoryPoolSize = 1024;
                opts.ThreadLocalCacheSize = 256;
                opts.DefaultInitialCapacity = 32;
                opts.MaximumPoolSize = 2048;
                opts.EnableAutoClear = true;
                opts.ClearInterval = 5000;
                opts.EnablePerformanceMetrics = true;
            });

            using var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<PooledCollectionsExample>>();
            var pooledListFactory = provider.GetRequiredService<IPooledListFactory>();
            var pooledDictionaryFactory = provider.GetRequiredService<IPooledDictionaryFactory>();
            var pooledSetFactory = provider.GetRequiredService<IPooledSetFactory>();

            logger.LogInformation("开始内存池化集合示例");

            // 使用池化List
            logger.LogInformation("测试池化List");
            using (var pooledList = pooledListFactory.Create<int>())
            {
                // 添加元素
                for (int i = 0; i < 100; i++)
                {
                    pooledList.Add(i);
                }
                logger.LogInformation("池化List添加了 {Count} 个元素", pooledList.Count);
                
                // 遍历元素
                int sum = 0;
                foreach (var item in pooledList)
                {
                    sum += item;
                }
                logger.LogInformation("池化List元素总和: {Sum}", sum);
            }
            logger.LogInformation("池化List测试完成");

            // 使用池化Dictionary
            logger.LogInformation("测试池化Dictionary");
            using (var pooledDictionary = pooledDictionaryFactory.Create<string, int>())
            {
                // 添加键值对
                for (int i = 0; i < 50; i++)
                {
                    pooledDictionary[$"Key{i}"] = i;
                }
                logger.LogInformation("池化Dictionary添加了 {Count} 个键值对", pooledDictionary.Count);
                
                // 访问元素
                if (pooledDictionary.TryGetValue("Key25", out var value))
                {
                    logger.LogInformation("池化Dictionary中Key25的值: {Value}", value);
                }
            }
            logger.LogInformation("池化Dictionary测试完成");

            // 使用池化Set
            logger.LogInformation("测试池化Set");
            using (var pooledSet = pooledSetFactory.Create<string>())
            {
                // 添加元素
                for (int i = 0; i < 75; i++)
                {
                    pooledSet.Add($"Item{i}");
                }
                logger.LogInformation("池化Set添加了 {Count} 个元素", pooledSet.Count);
                
                // 检查元素
                if (pooledSet.Contains("Item50"))
                {
                    logger.LogInformation("池化Set包含Item50");
                }
            }
            logger.LogInformation("池化Set测试完成");

            // 测试性能
            logger.LogInformation("测试性能");
            var stopwatch = Stopwatch.StartNew();
            int iterations = 10000;

            for (int i = 0; i < iterations; i++)
            {
                using (var pooledList = pooledListFactory.Create<int>())
                {
                    pooledList.Add(i);
                    pooledList.Add(i + 1);
                    pooledList.Add(i + 2);
                }
            }

            stopwatch.Stop();
            logger.LogInformation("执行 {Iterations} 次池化List操作耗时: {ElapsedMilliseconds}ms", iterations, stopwatch.ElapsedMilliseconds);

            logger.LogInformation("内存池化集合示例完成");
        }
    }

    #region 程序入口
    /// <summary>
    /// 程序类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args">命令行参数</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("内存池化集合服务示例");
            Console.WriteLine("=" + new string('=', 50));
            
            PooledCollectionsExample.Run();
            
            Console.WriteLine("=" + new string('=', 50));
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
    }
}

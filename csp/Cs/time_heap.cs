// File-based Apps implementation for time_heap
// Uses .NET built-in collection classes for ordered map and message queue

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Cs;

namespace System.Threading.Tasks.Cs;

/// <summary>
/// 映射类，使用SortedDictionary实现有序映射
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
public class Map<TKey, TValue> where TKey : IComparable<TKey>
{
    private readonly SortedDictionary<TKey, TValue> _dictionary;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Map()
    {
        _dictionary = new SortedDictionary<TKey, TValue>();
    }

    /// <summary>
    /// 添加键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
    }

    /// <summary>
    /// 尝试获取值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否找到</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <summary>
    /// 移除键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否移除成功</returns>
    public bool Remove(TKey key)
    {
        return _dictionary.Remove(key);
    }

    /// <summary>
    /// 检查是否包含键
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否包含</returns>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// 获取或设置值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    public TValue this[TKey key]
    {
        get => _dictionary[key];
        set => _dictionary[key] = value;
    }

    /// <summary>
    /// 获取键的集合
    /// </summary>
    public ICollection<TKey> Keys => _dictionary.Keys;

    /// <summary>
    /// 获取值的集合
    /// </summary>
    public ICollection<TValue> Values => _dictionary.Values;

    /// <summary>
    /// 获取键值对的数量
    /// </summary>
    public int Count => _dictionary.Count;

    /// <summary>
    /// 清空映射
    /// </summary>
    public void Clear()
    {
        _dictionary.Clear();
    }

    /// <summary>
    /// 获取第一个键值对
    /// </summary>
    /// <returns>第一个键值对</returns>
    public KeyValuePair<TKey, TValue> First()
    {
        using (var enumerator = _dictionary.GetEnumerator())
        {
            if (enumerator.MoveNext())
                return enumerator.Current;
            throw new InvalidOperationException("Map is empty");
        }
    }

    /// <summary>
    /// 获取最后一个键值对
    /// </summary>
    /// <returns>最后一个键值对</returns>
    public KeyValuePair<TKey, TValue> Last()
    {
        KeyValuePair<TKey, TValue> last = default;
        foreach (var pair in _dictionary)
        {
            last = pair;
        }
        if (last.Equals(default(KeyValuePair<TKey, TValue>)) && _dictionary.Count > 0)
            throw new InvalidOperationException("Map is empty");
        return last;
    }
}

/// <summary>
/// 消息队列类，使用Queue实现消息队列
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class MsgQueue<T>
{
    private readonly Queue<T> _queue;
    private readonly object _lock = new object();

    /// <summary>
    /// 构造函数
    /// </summary>
    public MsgQueue()
    {
        _queue = new Queue<T>();
    }

    /// <summary>
    /// 添加消息到队列末尾
    /// </summary>
    /// <param name="item">消息</param>
    public void AddLast(T item)
    {
        lock (_lock)
        {
            _queue.Enqueue(item);
        }
    }

    /// <summary>
    /// 添加消息到队列开头
    /// </summary>
    /// <param name="item">消息</param>
    public void AddFirst(T item)
    {
        lock (_lock)
        {
            // Queue不支持直接在开头添加，需要重建队列
            var newQueue = new Queue<T>();
            newQueue.Enqueue(item);
            foreach (var existingItem in _queue)
            {
                newQueue.Enqueue(existingItem);
            }
            _queue.Clear();
            foreach (var newItem in newQueue)
            {
                _queue.Enqueue(newItem);
            }
        }
    }

    /// <summary>
    /// 移除并返回队列开头的消息
    /// </summary>
    /// <returns>队列开头的消息</returns>
    public T RemoveFirst()
    {
        lock (_lock)
        {
            return _queue.Dequeue();
        }
    }

    /// <summary>
    /// 获取队列开头的消息但不移除
    /// </summary>
    /// <returns>队列开头的消息</returns>
    public T First
    {
        get
        {
            lock (_lock)
            {
                return _queue.Peek();
            }
        }
    }

    /// <summary>
    /// 获取队列中的消息数量
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _queue.Count;
            }
        }
    }

    /// <summary>
    /// 检查队列是否为空
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            lock (_lock)
            {
                return _queue.Count == 0;
            }
        }
    }

    /// <summary>
    /// 清空队列
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _queue.Clear();
        }
    }
}

/// <summary>
/// 异步消息队列类，使用Channel实现异步消息队列
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class AsyncMsgQueue<T>
{
    private readonly Channel<T> _channel;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AsyncMsgQueue()
    {
        _channel = Channel.CreateUnbounded<T>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacity">队列容量</param>
    public AsyncMsgQueue(int capacity)
    {
        _channel = Channel.CreateBounded<T>(capacity);
    }

    /// <summary>
    /// 异步添加消息到队列
    /// </summary>
    /// <param name="item">消息</param>
    /// <returns>任务</returns>
    public async Task WriteAsync(T item)
    {
        await _channel.Writer.WriteAsync(item);
    }

    /// <summary>
    /// 异步从队列中读取消息
    /// </summary>
    /// <returns>消息</returns>
    public async Task<T> ReadAsync()
    {
        return await _channel.Reader.ReadAsync();
    }

    /// <summary>
    /// 尝试添加消息到队列
    /// </summary>
    /// <param name="item">消息</param>
    /// <returns>是否成功</returns>
    public bool TryWrite(T item)
    {
        return _channel.Writer.TryWrite(item);
    }

    /// <summary>
    /// 尝试从队列中读取消息
    /// </summary>
    /// <param name="item">消息</param>
    /// <returns>是否成功</returns>
    public bool TryRead(out T item)
    {
        return _channel.Reader.TryRead(out item);
    }

    /// <summary>
    /// 完成写入
    /// </summary>
    public void CompleteWriter()
    {
        _channel.Writer.Complete();
    }

    /// <summary>
    /// 获取读取器
    /// </summary>
    public ChannelReader<T> Reader => _channel.Reader;

    /// <summary>
    /// 获取写入器
    /// </summary>
    public ChannelWriter<T> Writer => _channel.Writer;
}

/// <summary>
/// 阻塞消息队列类，使用BlockingCollection实现阻塞队列
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class BlockingMsgQueue<T>
{
    private readonly BlockingCollection<T> _collection;

    /// <summary>
    /// 构造函数
    /// </summary>
    public BlockingMsgQueue()
    {
        _collection = new BlockingCollection<T>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacity">队列容量</param>
    public BlockingMsgQueue(int capacity)
    {
        _collection = new BlockingCollection<T>(capacity);
    }

    /// <summary>
    /// 添加消息到队列
    /// </summary>
    /// <param name="item">消息</param>
    public void Add(T item)
    {
        _collection.Add(item);
    }

    /// <summary>
    /// 尝试添加消息到队列
    /// </summary>
    /// <param name="item">消息</param>
    /// <returns>是否成功</returns>
    public bool TryAdd(T item)
    {
        return _collection.TryAdd(item);
    }

    /// <summary>
    /// 从队列中获取消息（阻塞）
    /// </summary>
    /// <returns>消息</returns>
    public T Take()
    {
        return _collection.Take();
    }

    /// <summary>
    /// 尝试从队列中获取消息
    /// </summary>
    /// <param name="item">消息</param>
    /// <returns>是否成功</returns>
    public bool TryTake(out T item)
    {
        return _collection.TryTake(out item);
    }

    /// <summary>
    /// 完成添加
    /// </summary>
    public void CompleteAdding()
    {
        _collection.CompleteAdding();
    }

    /// <summary>
    /// 获取队列中的消息数量
    /// </summary>
    public int Count => _collection.Count;

    /// <summary>
    /// 检查队列是否已完成添加
    /// </summary>
    public bool IsAddingCompleted => _collection.IsAddingCompleted;

    /// <summary>
    /// 检查队列是否已完成
    /// </summary>
    public bool IsCompleted => _collection.IsCompleted;
}

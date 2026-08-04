// File-based Apps implementation for mutex
// Uses .NET built-in synchronization primitives

using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Cs;

/// <summary>
/// 互斥锁实现
/// </summary>
public class go_mutex
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ThreadLocal<int> _recursionCount = new(() => 0);
    private readonly ThreadLocal<int> _ownerThreadId = new(() => -1);

    /// <summary>
    /// 构造函数
    /// </summary>
    public go_mutex()
    {
        _semaphore = new SemaphoreSlim(1, 1);
    }

    /// <summary>
    /// 加锁
    /// </summary>
    public void slock()
    {
        var currentThreadId = Thread.CurrentThread.ManagedThreadId;
        if (_ownerThreadId.Value == currentThreadId)
        {
            // 递归加锁
            _recursionCount.Value++;
            return;
        }

        _semaphore.Wait();
        _ownerThreadId.Value = currentThreadId;
        _recursionCount.Value = 1;
    }

    /// <summary>
    /// 解锁
    /// </summary>
    public void unslock()
    {
        var currentThreadId = Thread.CurrentThread.ManagedThreadId;
        if (_ownerThreadId.Value != currentThreadId)
        {
            throw new SynchronizationLockException("Current thread does not own the mutex");
        }

        _recursionCount.Value--;
        if (_recursionCount.Value == 0)
        {
            _ownerThreadId.Value = -1;
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 尝试加锁
    /// </summary>
    /// <returns>是否成功加锁</returns>
    public bool try_lock()
    {
        var currentThreadId = Thread.CurrentThread.ManagedThreadId;
        if (_ownerThreadId.Value == currentThreadId)
        {
            // 递归加锁
            _recursionCount.Value++;
            return true;
        }

        if (_semaphore.Wait(0))
        {
            _ownerThreadId.Value = currentThreadId;
            _recursionCount.Value = 1;
            return true;
        }

        return false;
    }
}

/// <summary>
/// 读写锁实现
/// </summary>
public class go_shared_mutex
{
    private readonly ReaderWriterLockSlim _rwLock;

    /// <summary>
    /// 构造函数
    /// </summary>
    public go_shared_mutex()
    {
        _rwLock = new ReaderWriterLockSlim();
    }

    /// <summary>
    /// 读锁
    /// </summary>
    public void rlock()
    {
        _rwLock.EnterReadLock();
    }

    /// <summary>
    /// 读解锁
    /// </summary>
    public void runlock()
    {
        _rwLock.ExitReadLock();
    }

    /// <summary>
    /// 写锁
    /// </summary>
    public void wlock()
    {
        _rwLock.EnterWriteLock();
    }

    /// <summary>
    /// 写解锁
    /// </summary>
    public void unwlock()
    {
        _rwLock.ExitWriteLock();
    }

    /// <summary>
    /// 尝试读锁
    /// </summary>
    /// <returns>是否成功加锁</returns>
    public bool try_rlock()
    {
        return _rwLock.TryEnterReadLock(0);
    }

    /// <summary>
    /// 尝试写锁
    /// </summary>
    /// <returns>是否成功加锁</returns>
    public bool try_lock()
    {
        return _rwLock.TryEnterWriteLock(0);
    }
}

/// <summary>
/// 条件变量实现
/// </summary>
public class go_condition_variable
{
    private readonly object _lock = new();
    private readonly Queue<SemaphoreSlim> _waiters = new();

    /// <summary>
    /// 等待信号
    /// </summary>
    /// <param name="mutex">互斥锁</param>
    public void wait(go_mutex mutex)
    {
        var semaphore = new SemaphoreSlim(0, 1);
        
        lock (_lock)
        {
            _waiters.Enqueue(semaphore);
        }

        mutex.unslock();
        semaphore.Wait();
        mutex.slock();
    }

    /// <summary>
    /// 通知一个等待线程
    /// </summary>
    public void signal()
    {
        SemaphoreSlim? semaphore = null;
        
        lock (_lock)
        {
            if (_waiters.Count > 0)
            {
                semaphore = _waiters.Dequeue();
            }
        }

        semaphore?.Release();
    }

    /// <summary>
    /// 通知所有等待线程
    /// </summary>
    public void broadcast()
    {
        List<SemaphoreSlim> semaphores;
        
        lock (_lock)
        {
            semaphores = new List<SemaphoreSlim>(_waiters);
            _waiters.Clear();
        }

        foreach (var semaphore in semaphores)
        {
            semaphore.Release();
        }
    }
}

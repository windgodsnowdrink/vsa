//#!/usr/bin/env dotnet
//#:sdk Microsoft.NET.Sdk.Web
//#:package Scrutor@7.0.0
//#:package Carter@10.0.0
//#:package Microsoft.EntityFrameworkCore.InMemory@10.0.3
//#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.3
//#:package Microsoft.Extensions.Caching.Memory@10.0.3
//#:property TargetFramework=net11.0
//#:property RollForward=Major
//#:property Nullable=enable
//#:property ImplicitUsings=enable
//#:property PublishAot=false
//#:property PublishReadyToRun=false
//#:property PublishSingleFile=false

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Go;

/// <summary>
/// 互斥锁
/// </summary>
public class go_mutex
{
    /// <summary>
    /// 等待节点
    /// </summary>
    struct wait_node
    {
        public Action _ntf;
        public long _id;
    }
    /// <summary>
    /// 线程
    /// </summary>
    shared_strand _strand;
    /// <summary>
    /// 等待队列
    /// </summary>
    LinkedList<wait_node> _waitQueue;
    /// <summary>
    /// 锁ID
    /// </summary>
    protected long _lockID;
    /// <summary>
    /// 递归计数
    /// </summary>
    protected int _recCount;
    /// <summary>
    /// 是否必须更新
    /// </summary>
    protected bool _mustTick;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="strand">线程</param>
    /// <remarks>
    /// </remarks>
    public go_mutex(shared_strand strand)
    {
        _strand = strand;
        _waitQueue = new LinkedList<wait_node>();
        _lockID = 0;
        _recCount = 0;
        _mustTick = false;
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <remarks>
    /// </remarks>
    public go_mutex() : this(shared_strand.default_strand()) { }
    /// <summary>
    /// 异步加锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    protected virtual void async_lock_(long id, Action ntf)
    {
        if (0 == _lockID || id == _lockID)
        {
            _lockID = id;
            _recCount++;
            ntf();
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = ntf, _id = id });
        }
    }
    /// <summary>
    /// 异步尝试加锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    protected virtual void async_try_lock_(long id, Action<bool> ntf)
    {
        if (0 == _lockID || id == _lockID)
        {
            _lockID = id;
            _recCount++;
            ntf(true);
        }
        else
        {
            ntf(false);
        }
    }
    /// <summary>
    /// 异步加锁超时
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    protected virtual void async_timed_lock_(long id, int ms, Action<bool> ntf)
    {
        if (0 == _lockID || id == _lockID)
        {
            _lockID = id;
            _recCount++;
            ntf(true);
        }
        else if (ms >= 0)
        {
            async_timer timer = new async_timer(_strand);
            LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
            {
                _ntf = delegate ()
                {
                    timer.Cancel();
                    ntf(true);
                },
                _id = id
            });
            timer.timeout(ms, delegate ()
            {
                _waitQueue.Remove(node);
                ntf(false);
            });
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _id = id });
        }
    }
    /// <summary>
    /// 异步解锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    protected virtual void async_unlock_(long id, Action ntf)
    {
        Debug.Assert(id == _lockID);
        if (0 == --_recCount)
        {
            if (0 != _waitQueue.Count)
            {
                _recCount = 1;
                wait_node queueFront = _waitQueue.First.Value;
                _waitQueue.RemoveFirst();
                _lockID = queueFront._id;
                queueFront._ntf();
            }
            else
            {
                _lockID = 0;
            }
        }
        ntf();
    }
    /// <summary>
    /// 异步取消加锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    protected virtual void async_cancel_(long id, Action ntf)
    {
        if (id == _lockID)
        {
            _recCount = 1;
            async_unlock_(id, ntf);
        }
        else
        {
            for (LinkedListNode<wait_node> it = _waitQueue.Last; null != it; it = it.Previous)
            {
                if (it.Value._id == id)
                {
                    _waitQueue.Remove(it);
                    break;
                }
            }
            ntf();
        }
    }
    /// <summary>
    /// 异步加锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    internal void async_lock(long id, Action ntf)
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) async_lock_(id, ntf);
            else _strand.add_last(() => async_lock_(id, ntf));
        else _strand.post(() => async_lock_(id, ntf));
    }
    /// <summary>
    /// 异步尝试加锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    internal void async_try_lock(long id, Action<bool> ntf)
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) async_try_lock_(id, ntf);
            else _strand.add_last(() => async_try_lock_(id, ntf));
        else _strand.post(() => async_try_lock_(id, ntf));
    }
    /// <summary>
    /// 异步加锁（超时）
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    internal virtual void async_timed_lock(long id, int ms, Action<bool> ntf)
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) async_timed_lock_(id, ms, ntf);
            else _strand.add_last(() => async_timed_lock_(id, ms, ntf));
        else _strand.post(() => async_timed_lock_(id, ms, ntf));
    }
    /// <summary>
    /// 异步解锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    internal virtual void async_unlock(long id, Action ntf)
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) async_unlock_(id, ntf);
            else _strand.add_last(() => async_unlock_(id, ntf));
        else _strand.post(() => async_unlock_(id, ntf));
    }
    /// <summary>
    /// 异步取消加锁
    /// </summary>
    /// <param name="id">锁ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>
    internal virtual void async_cancel(long id, Action ntf)
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) async_cancel_(id, ntf);
            else _strand.add_last(() => async_cancel_(id, ntf));
        else _strand.post(() => async_cancel_(id, ntf));
    }
    /// <summary>
    /// 异步加锁
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>
    public Task Lock()
    {
        return generator.mutex_lock(this);
    }
    /// <summary>
    /// 异步尝试加锁
    /// </summary>
    /// <param name="res">异步结果包装器</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>
    public Task try_lock(async_result_wrap<bool> res)
    {
        return generator.mutex_try_lock(res, this);
    }
    /// <summary>
    /// 异步尝试加锁
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>
    public ValueTask<bool> try_lock()
    {
        return generator.mutex_try_lock(this);
    }
    /// <summary>
    /// 异步加锁（超时）
    /// </summary>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>
    public Task timed_lock(async_result_wrap<bool> res, int ms)
    {
        return generator.mutex_timed_lock(res, this, ms);
    }
    /// <summary>
    /// 异步加锁（超时）
    /// </summary>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>
    public ValueTask<bool> timed_lock(int ms)
    {
        return generator.mutex_timed_lock(this, ms);
    }
    /// <summary>
    /// 异步解锁
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>
    public Task unlock()
    {
        return generator.mutex_unlock(this);
    }
    /// <summary>
    /// 获取当前线程的 strand
    /// </summary>
    /// <returns>当前线程的 strand</returns>
    /// <remarks>
    /// </remarks>
    public shared_strand self_strand()
    {
        return _strand;
    }
}
/// <summary>
/// 共享互斥锁
/// </summary>
/// <remarks>
/// </remarks>
public class go_shared_mutex : go_mutex
{
    /// <summary>
    /// 锁状态
    /// </summary>
    /// <remarks>
    /// </remarks>
    enum lock_status
    {
        st_shared,
        st_unique,
        st_upgrade
    };
    /// <summary>
    /// 等待节点
    /// </summary>
    /// <remarks>
    /// </remarks>
    struct wait_node
    {
        public Action _ntf;
        public long _waitHostID;
        public lock_status _status;
    };
    /// <summary>
    /// 共享计数
    /// </summary>
    /// <remarks>
    /// </remarks>
    class shared_count
    {
        public int _count = 0;
    };
    /// <summary>
    /// 等待队列
    /// </summary>
    /// <remarks>
    /// </remarks>
    LinkedList<wait_node> _waitQueue;
    /// <summary>
    /// 共享计数映射
    /// </summary>
    /// <remarks>
    /// </remarks>
    Dictionary<long, shared_count> _sharedMap;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="strand">线程 strand</param>
    /// <remarks>
    /// </remarks>
    public go_shared_mutex(shared_strand strand) : base(strand)
    {
        _waitQueue = new LinkedList<wait_node>();
        _sharedMap = new Dictionary<long, shared_count>();
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <remarks>
    /// </remarks>
    public go_shared_mutex() : base()
    {
        _waitQueue = new LinkedList<wait_node>();
        _sharedMap = new Dictionary<long, shared_count>();
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    protected override void async_lock_(long id, Action ntf)
    {
        if (0 == _sharedMap.Count && (0 == base._lockID || id == base._lockID))
        {
            base._lockID = id;
            base._recCount++;
            ntf();
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = ntf, _waitHostID = id, _status = lock_status.st_unique });
        }
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    protected override void async_try_lock_(long id, Action<bool> ntf)
    {
        if (0 == _sharedMap.Count && (0 == base._lockID || id == base._lockID))
        {
            base._lockID = id;
            base._recCount++;
            ntf(true);
        }
        else
        {
            ntf(false);
        }
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    protected override void async_timed_lock_(long id, int ms, Action<bool> ntf)
    {
        if (0 == _sharedMap.Count && (0 == base._lockID || id == base._lockID))
        {
            base._lockID = id;
            base._recCount++;
            ntf(true);
        }
        else if (ms >= 0)
        {
            async_timer timer = new async_timer(self_strand());
            LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
            {
                _ntf = delegate ()
                {
                    timer.Cancel();
                    ntf(true);
                },
                _waitHostID = id,
                _status = lock_status.st_unique
            });
            timer.timeout(ms, delegate ()
            {
                _waitQueue.Remove(node);
                ntf(false);
            });
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _waitHostID = id, _status = lock_status.st_unique });
        }
    }
    /// <summary>
    /// 查找共享互斥锁映射
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <returns>共享互斥锁映射</returns>
    /// <remarks>
    /// </remarks>
    shared_count find_map(long id)
    {
        shared_count ct = null;
        if (!_sharedMap.TryGetValue(id, out ct))
        {
            ct = new shared_count();
            _sharedMap.Add(id, ct);
        }
        return ct;
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_lock_shared_(long id, Action ntf)
    {
        if (0 != _sharedMap.Count || 0 == base._lockID)
        {
            find_map(id)._count++;
            ntf();
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = ntf, _waitHostID = id, _status = lock_status.st_shared });
        }
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_lock_pess_shared_(long id, Action ntf)
    {
        if (0 == _waitQueue.Count && (0 != _sharedMap.Count || 0 == base._lockID))
        {
            find_map(id)._count++;
            ntf();
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = ntf, _waitHostID = id, _status = lock_status.st_shared });
        }
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_try_lock_shared_(long id, Action<bool> ntf)
    {
        if (0 != _sharedMap.Count || 0 == base._lockID)
        {
            find_map(id)._count++;
            ntf(true);
        }
        else
        {
            ntf(false);
        }
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_timed_lock_shared_(long id, int ms, Action<bool> ntf)
    {
        if (0 != _sharedMap.Count || 0 == base._lockID)
        {
            find_map(id)._count++;
            ntf(true);
        }
        else if (ms >= 0)
        {
            async_timer timer = new async_timer(self_strand());
            LinkedListNode<wait_node> node = _waitQueue.AddLast(new wait_node()
            {
                _ntf = delegate ()
                {
                    timer.Cancel();
                    ntf(true);
                },
                _waitHostID = id,
                _status = lock_status.st_shared
            });
            timer.timeout(ms, delegate ()
            {
                _waitQueue.Remove(node);
                ntf(false);
            });
        }
        else
        {
            _waitQueue.AddLast(new wait_node() { _ntf = () => ntf(true), _waitHostID = id, _status = lock_status.st_shared });
        }
    }
    /// <summary>
    /// 异步加锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_lock_upgrade_(long id, Action ntf)
    {
        base.async_lock_(id, ntf);
    }
    /// <summary>
    /// 异步加锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_try_lock_upgrade_(long id, Action<bool> ntf)
    {
        base.async_try_lock_(id, ntf);
    }
    /// <summary>
    /// 异步解锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    protected override void async_unlock_(long id, Action ntf)
    {
        if (0 == --base._recCount && 0 != _waitQueue.Count)
        {
            _mustTick = true;
            wait_node queueFront = _waitQueue.First.Value;
            _waitQueue.RemoveFirst();
            queueFront._ntf();
            if (lock_status.st_shared == queueFront._status)
            {
                base._lockID = 0;
                find_map(queueFront._waitHostID)._count++;
                for (LinkedListNode<wait_node> it = _waitQueue.First; null != it;)
                {
                    if (lock_status.st_shared == it.Value._status)
                    {
                        find_map(it.Value._waitHostID)._count++;
                        it.Value._ntf();
                        LinkedListNode<wait_node> oit = it;
                        it = it.Next;
                        _waitQueue.Remove(oit);
                    }
                    else
                    {
                        it = it.Next;
                    }
                }
            }
            else
            {
                base._lockID = queueFront._waitHostID;
                base._recCount++;
            }
            _mustTick = false;
        }
        ntf();
    }
    /// <summary>
    /// 异步解锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_unlock_shared_(long id, Action ntf)
    {
        if (0 == --find_map(id)._count)
        {
            _sharedMap.Remove(id);
            if (0 == _sharedMap.Count && 0 != _waitQueue.Count)
            {
                _mustTick = true;
                wait_node queueFront = _waitQueue.First.Value;
                _waitQueue.RemoveFirst();
                queueFront._ntf();
                if (lock_status.st_shared == queueFront._status)
                {
                    base._lockID = 0;
                    find_map(queueFront._waitHostID)._count++;
                    for (LinkedListNode<wait_node> it = _waitQueue.First; null != it;)
                    {
                        if (lock_status.st_shared == it.Value._status)
                        {
                            find_map(it.Value._waitHostID)._count++;
                            it.Value._ntf();
                            LinkedListNode<wait_node> oit = it;
                            it = it.Next;
                            _waitQueue.Remove(oit);
                        }
                        else
                        {
                            it = it.Next;
                        }
                    }
                }
                else
                {
                    base._lockID = queueFront._waitHostID;
                    base._recCount++;
                }
                _mustTick = false;
            }
        }
        ntf();
    }
    /// <summary>
    /// 异步解锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_unlock_upgrade_(long id, Action ntf)
    {
        base.async_unlock_(id, ntf);
    }
    /// <summary>
    /// 异步解锁（共享）并加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_unlock_and_lock_shared_(long id, Action ntf)
    {
        async_unlock_(id, () => async_lock_shared_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（升级）并加锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_unlock_and_lock_upgrade_(long id, Action ntf)
    {
        async_unlock_and_lock_shared_(id, () => async_lock_upgrade_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（升级）并加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_unlock_upgrade_and_lock_(long id, Action ntf)
    {
        async_unlock_upgrade_(id, () => async_unlock_shared_(id, () => async_lock_(id, ntf)));
    }
    /// <summary>
    /// 异步解锁（共享）并加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>
    private void async_unlock_shared_and_lock_(long id, Action ntf)
    {
        async_unlock_shared_(id, () => async_lock_(id, ntf));
    }
    /// <summary>
    /// 异步取消（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">取消成功回调</param>
    /// <remarks>
    /// </remarks>
    protected override void async_cancel_(long id, Action ntf)
    {
        shared_count tempCount;
        if (_sharedMap.TryGetValue(id, out tempCount))
        {
            base.async_cancel_(id, nil_action.action);
            tempCount._count = 1;
            async_unlock_shared_(id, ntf);
        }
        else if (id == base._lockID)
        {
            base._recCount = 1;
            async_unlock_(id, ntf);
        }
        else
        {
            for (LinkedListNode<wait_node> it = _waitQueue.Last; null != it; it = it.Previous)
            {
                if (it.Value._waitHostID == id)
                {
                    _waitQueue.Remove(it);
                    break;
                }
            }
            ntf();
        }
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    internal void async_lock_shared(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_lock_shared_(id, ntf);
            else self_strand().add_last(() => async_lock_shared_(id, ntf));
        else self_strand().post(() => async_lock_shared_(id, ntf));
    }
    /// <summary>
    /// 异步加锁（共享）（阻塞）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    internal void async_lock_pess_shared(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_lock_pess_shared_(id, ntf);
            else self_strand().add_last(() => async_lock_pess_shared_(id, ntf));
        else self_strand().post(() => async_lock_pess_shared_(id, ntf));
    }
    /// <summary>
    /// 异步尝试加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    internal void async_try_lock_shared(long id, Action<bool> ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_try_lock_shared_(id, ntf);
            else self_strand().add_last(() => async_try_lock_shared_(id, ntf));
        else self_strand().post(() => async_try_lock_shared_(id, ntf));
    }
    /// <summary>
    /// 异步加锁（共享）（超时）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    internal void async_timed_lock_shared(long id, int ms, Action<bool> ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_timed_lock_shared_(id, ms, ntf);
            else self_strand().add_last(() => async_timed_lock_shared_(id, ms, ntf));
        else self_strand().post(() => async_timed_lock_shared_(id, ms, ntf));
    }
    /// <summary>
    /// 异步加锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    internal void async_lock_upgrade(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_lock_upgrade_(id, ntf);
            else self_strand().add_last(() => async_lock_upgrade_(id, ntf));
        else self_strand().post(() => async_lock_upgrade_(id, ntf));
    }
    /// <summary>
    /// 异步尝试加锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">加锁成功回调</param>
    /// <remarks>
    /// </remarks>
    internal void async_try_lock_upgrade(long id, Action<bool> ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_try_lock_upgrade_(id, ntf);
            else self_strand().add_last(() => async_try_lock_upgrade_(id, ntf));
        else self_strand().post(() => async_try_lock_upgrade_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>  
    internal void async_unlock_shared(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_unlock_shared_(id, ntf);
            else self_strand().add_last(() => async_unlock_shared_(id, ntf));
        else self_strand().post(() => async_unlock_shared_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>  
    internal void async_unlock_upgrade(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_unlock_upgrade_(id, ntf);
            else self_strand().add_last(() => async_unlock_upgrade_(id, ntf));
        else self_strand().post(() => async_unlock_upgrade_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（共享）并加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>  
    internal void unlock_and_lock_shared(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_unlock_and_lock_shared_(id, ntf);
            else self_strand().add_last(() => async_unlock_and_lock_shared_(id, ntf));
        else self_strand().post(() => async_unlock_and_lock_shared_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（升级）并加锁（升级）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>  
    internal void unlock_and_lock_upgrade(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_unlock_and_lock_upgrade_(id, ntf);
            else self_strand().add_last(() => async_unlock_and_lock_upgrade_(id, ntf));
        else self_strand().post(() => async_unlock_and_lock_upgrade_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（升级）并加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>  
    internal void unlock_upgrade_and_lock(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_unlock_upgrade_and_lock_(id, ntf);
            else self_strand().add_last(() => async_unlock_upgrade_and_lock_(id, ntf));
        else self_strand().post(() => async_unlock_upgrade_and_lock_(id, ntf));
    }
    /// <summary>
    /// 异步解锁（共享）并加锁（共享）
    /// </summary>
    /// <param name="id">互斥锁 ID</param>
    /// <param name="ntf">解锁成功回调</param>
    /// <remarks>
    /// </remarks>  
    internal void unlock_shared_and_lock(long id, Action ntf)
    {
        if (self_strand().running_in_this_thread())
            if (!_mustTick) async_unlock_shared_and_lock_(id, ntf);
            else self_strand().add_last(() => async_unlock_shared_and_lock_(id, ntf));
        else self_strand().post(() => async_unlock_shared_and_lock_(id, ntf));
    }
    /// <summary>
    /// 异步加锁（共享）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task lock_shared()
    {
        return generator.mutex_lock_shared(this);
    }
    /// <summary>
    /// 异步加锁（共享）（非阻塞）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task lock_pess_shared()
    {
        return generator.mutex_lock_pess_shared(this);
    }
    /// <summary>
    /// 异步加锁（升级）（非阻塞）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task lock_upgrade()
    {
        return generator.mutex_lock_upgrade(this);
    }
    /// <summary>
    /// 异步尝试加锁（共享）
    /// </summary>
    /// <param name="res">异步结果包装器</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task try_lock_shared(async_result_wrap<bool> res)
    {
        return generator.mutex_try_lock_shared(res, this);
    }
    /// <summary>
    /// 异步尝试加锁（共享）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public ValueTask<bool> try_lock_shared()
    {
        return generator.mutex_try_lock_shared(this);
    }
    /// <summary>
    /// 异步尝试加锁（升级）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task try_lock_upgrade(async_result_wrap<bool> res)
    {
        return generator.mutex_try_lock_upgrade(res, this);
    }
    /// <summary>
    /// 异步尝试加锁（升级）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public ValueTask<bool> try_lock_upgrade()
    {
        return generator.mutex_try_lock_upgrade(this);
    }
    /// <summary>
    /// 异步加锁（共享）（超时）
    /// </summary>
    /// <param name="res">异步结果包装器</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task timed_lock_shared(async_result_wrap<bool> res, int ms)
    {
        return generator.mutex_timed_lock_shared(res, this, ms);
    }
    /// <summary>
    /// 异步加锁（共享）（超时）
    /// </summary>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public ValueTask<bool> timed_lock_shared(int ms)
    {
        return generator.mutex_timed_lock_shared(this, ms);
    }
    /// <summary>
    /// 异步解锁（共享）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task unlock_shared()
    {
        return generator.mutex_unlock_shared(this);
    }
    /// <summary>
    /// 异步解锁（升级）
    /// </summary>
    /// <returns>异步任务</returns>
    /// <remarks>
    /// </remarks>  
    public Task unlock_upgrade()
    {
        return generator.mutex_unlock_upgrade(this);
    }
}
/// <summary>
/// 条件变量
/// </summary>
/// <remarks>
/// 条件变量用于等待多个线程满足某个条件。
/// </remarks>  
public class go_condition_variable
{
    /// <summary>
    /// 条件变量
    /// </summary>
    /// <remarks>
    /// 条件变量用于等待多个线程满足某个条件。
    /// </remarks>  
    shared_strand _strand;
    /// <summary>
    /// 等待队列
    /// </summary>
    /// <remarks>
    /// 等待队列用于存储等待条件的线程。
    /// </remarks>  
    LinkedList<tuple<long, go_mutex, Action>> _waitQueue;
    /// <summary>
    /// 必须在当前线程中调用
    /// </summary>
    /// <remarks>
    /// 必须在当前线程中调用，否则会抛出异常。
    /// </remarks>  
    bool _mustTick;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <remarks>
    /// 构造函数用于创建一个新的条件变量。
    /// </remarks>  
    public go_condition_variable(shared_strand strand)
    {
        _strand = strand;
        _waitQueue = new LinkedList<tuple<long, go_mutex, Action>>();
        _mustTick = false;
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <remarks>
    /// 构造函数用于创建一个新的条件变量。
    /// </remarks>  
    public go_condition_variable() : this(shared_strand.default_strand()) { }
    /// <summary>
    /// 异步等待
    /// </summary>
    /// <param name="id">线程ID</param>
    /// <param name="mutex">互斥锁</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>  
    internal void async_wait(long id, go_mutex mutex, Action ntf)
    {
        mutex.async_unlock(id, delegate ()
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, ntf)));
                else _strand.add_last(() => _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, ntf))));
            else _strand.post(() => _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, ntf))));
        });
    }
    /// <summary>
    /// 异步定时等待
    /// </summary>
    /// <param name="id">线程ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="mutex">互斥锁</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>  
    private void async_timed_wait_(long id, int ms, go_mutex mutex, Action<bool> ntf)
    {
        if (ms >= 0)
        {
            async_timer timer = new async_timer(_strand);
            LinkedListNode<tuple<long, go_mutex, Action>> node = _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, delegate ()
            {
                timer.Cancel();
                mutex.async_lock(id, () => ntf(true));
            }));
            timer.timeout(ms, delegate ()
            {
                _waitQueue.Remove(node);
                mutex.async_lock(id, () => ntf(false));
            });
        }
        else
        {
            _waitQueue.AddLast(new tuple<long, go_mutex, Action>(id, mutex, () => mutex.async_lock(id, () => ntf(true))));
        }
    }
    /// <summary>
    /// 异步定时等待
    /// </summary>
    /// <param name="id">线程ID</param>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="mutex">互斥锁</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>  
    internal void async_timed_wait(long id, int ms, go_mutex mutex, Action<bool> ntf)
    {
        mutex.async_unlock(id, delegate ()
        {
            if (_strand.running_in_this_thread())
                if (!_mustTick) async_timed_wait_(id, ms, mutex, ntf);
                else _strand.add_last(() => async_timed_wait_(id, ms, mutex, ntf));
            else _strand.post(() => async_timed_wait_(id, ms, mutex, ntf));
        });
    }
    /// <summary>
    /// 通知一个线程
    /// </summary>
    /// <remarks>
    /// </remarks>  
    private void notify_one_()
    {
        if (0 != _waitQueue.Count)
        {
            Action ntf = _waitQueue.First.Value.value3;
            _waitQueue.RemoveFirst();
            ntf();
        }
    }
    /// <summary>
    /// 通知一个线程
    /// </summary>
    /// <remarks>
    /// </remarks>  
    public void notify_one()
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) notify_one_();
            else _strand.add_last(() => notify_one_());
        else _strand.post(() => notify_one_());
    }
    /// <summary>
    /// 通知所有线程
    /// </summary>
    /// <remarks>
    /// </remarks>  
    private void notify_all_()
    {
        _mustTick = true;
        while (0 != _waitQueue.Count)
        {
            Action ntf = _waitQueue.First.Value.value3;
            _waitQueue.RemoveFirst();
            ntf();
        }
        _mustTick = false;
    }
    /// <summary>
    /// 通知所有线程
    /// </summary>
    /// <remarks>
    /// </remarks>  
    public void notify_all()
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) notify_all_();
            else _strand.add_last(() => notify_all_());
        else _strand.post(() => notify_all_());
    }
    /// <summary>
    /// 异步取消等待
    /// </summary>
    /// <param name="id">线程ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>  
    private void async_cancel_(long id, Action ntf)
    {
        for (LinkedListNode<tuple<long, go_mutex, Action>> it = _waitQueue.First; null != it; it = it.Next)
        {
            if (id == it.Value.value1)
            {
                go_mutex mtx = it.Value.value2;
                mtx.async_cancel(id, ntf);
                return;
            }
        }
        ntf();
    }
    /// <summary>
    /// 异步取消等待
    /// </summary>
    /// <param name="id">线程ID</param>
    /// <param name="ntf">通知函数</param>
    /// <remarks>
    /// </remarks>  
    internal void async_cancel(long id, Action ntf)
    {
        if (_strand.running_in_this_thread())
            if (!_mustTick) async_cancel_(id, ntf);
            else _strand.add_last(() => async_cancel_(id, ntf));
        else _strand.post(() => async_cancel_(id, ntf));
    }
    /// <summary>
    /// 等待条件变量
    /// </summary>
    /// <param name="mutex">互斥锁</param>
    /// <returns>
    /// 一个任务，用于等待条件变量被通知。
    /// </returns>
    /// <remarks>
    /// </remarks>  
    public Task wait(go_mutex mutex)
    {
        return generator.condition_wait(this, mutex);
    }
    /// <summary>
    /// 超时等待条件变量
    /// </summary>
    /// <param name="res">异步结果包装器</param>
    /// <param name="mutex">互斥锁</param>
    /// <param name="ms">超时时间，单位毫秒</param>
    /// <returns>
    /// 一个任务，用于等待条件变量被通知。
    /// </returns>
    /// <remarks>
    /// </remarks>  
    public Task timed_wait(async_result_wrap<bool> res, go_mutex mutex, int ms)
    {
        return generator.condition_timed_wait(res, this, mutex, ms);
    }
    /// <summary>
    /// 超时等待条件变量
    /// </summary>
    /// <param name="mutex">互斥锁</param>
    /// <param name="ms">超时时间，单位毫秒</param>
    /// <returns>
    /// 一个值任务，用于等待条件变量被通知。
    /// </returns>
    /// <remarks>
    /// </remarks>  
    public ValueTask<bool> timed_wait(go_mutex mutex, int ms)
    {
        return generator.condition_timed_wait(this, mutex, ms);
    }
    /// <summary>
    /// 取消等待
    /// </summary>
    /// <returns>
    /// 一个任务，用于取消等待条件变量。
    /// </returns>
    /// <remarks>
    /// </remarks>  
    public Task cancel()
    {
        return generator.condition_cancel(this);
    }
}

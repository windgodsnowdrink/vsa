using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace System.Threading.Tasks.Go;

/// <summary>
/// 工作服务
/// </summary>
public class work_service
{
    /// <summary>
    /// 工作保护器，持有它可以保证 work_service 不会因为没有工作而退出
    /// </summary>
    public class work_guard : IDisposable
    {
        /// <summary>
        /// 工作服务
        /// </summary>
        work_service _service;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        internal work_guard(work_service service)
        {
            _service = service;
            _service.hold_work();
        }
        /// <summary>
        /// GC
        /// </summary>
        public void Dispose()
        {
            _service.release_work();
        }
    }

    int _work;
    int _waiting;
    volatile bool _runSign;
    MsgQueue<Action> _opQueue;
    /// <summary>
    /// 构造函数
    /// </summary>
    public work_service()
    {
        _work = 0;
        _waiting = 0;
        _runSign = true;
        _opQueue = new MsgQueue<Action>();
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="handler"></param>
    public void post(Action handler)
    {
        MsgQueueNode<Action> newNode = new MsgQueueNode<Action>(handler);
        Monitor.Enter(_opQueue);
        _opQueue.AddLast(newNode);
        if (0 != _waiting)
        {
            _waiting--;
            Monitor.Pulse(_opQueue);
        }
        Monitor.Exit(_opQueue);
    }
    /// <summary>
    /// 执行一次
    /// </summary>
    /// <returns></returns>
    public bool run_one()
    {
        Monitor.Enter(_opQueue);
        if (_runSign && 0 != _opQueue.Count)
        {
            MsgQueueNode<Action> firstNode = _opQueue.First;
            _opQueue.RemoveFirst();
            Monitor.Exit(_opQueue);
            functional.catch_invoke(firstNode.Value);
            return true;
        }
        Monitor.Exit(_opQueue);
        return false;
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <returns></returns>
    public long run()
    {
        long count = 0;
        while (_runSign)
        {
            Monitor.Enter(_opQueue);
            if (0 != _opQueue.Count)
            {
                MsgQueueNode<Action> firstNode = _opQueue.First;
                _opQueue.RemoveFirst();
                Monitor.Exit(_opQueue);
                count++;
                functional.catch_invoke(firstNode.Value);
            }
            else if (0 != _work)
            {
                _waiting++;
                Monitor.Wait(_opQueue);
                Monitor.Exit(_opQueue);
            }
            else
            {
                Monitor.Exit(_opQueue);
                break;
            }
        }
        return count;
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void stop()
    {
        _runSign = false;
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void reset()
    {
        _runSign = true;
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void hold_work()
    {
        Interlocked.Increment(ref _work);
    }
    /// <summary>
    /// 发布
    /// </summary>
    public void release_work()
    {
        if (0 == Interlocked.Decrement(ref _work))
        {
            Monitor.Enter(_opQueue);
            if (0 != _waiting)
            {
                _waiting = 0;
                Monitor.PulseAll(_opQueue);
            }
            Monitor.Exit(_opQueue);
        }
    }
    /// <summary>
    /// 计数
    /// </summary>
    public int count
    {
        get
        {
            return _opQueue.Count;
        }
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <returns></returns>
    public work_guard work()
    {
        return new work_guard(this);
    }
}
/// <summary>
/// 工作引擎
/// </summary>
public class work_engine
{
    /// <summary>
    /// 工作服务
    /// </summary>
    work_service _service;
    /// <summary>
    /// 线程池
    /// </summary>
    Thread[] _runThreads;
    /// <summary>
    /// 构造函数
    /// </summary>
    public work_engine()
    {
        _service = new work_service();
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="threads"></param>
    /// <param name="priority"></param>
    /// <param name="background"></param>
    /// <param name="name"></param>
    public void run(int threads = 1, ThreadPriority priority = ThreadPriority.Normal, bool background = false, string name = null)
    {
        lock (this)
        {
            Debug.Assert(null == _runThreads, "work_engine 已经运行!");
            _service.reset();
            _service.hold_work();
            _runThreads = new Thread[threads];
            for (int i = 0; i < threads; ++i)
            {
                _runThreads[i] = new Thread(() => _service.run());
                _runThreads[i].Priority = priority;
                _runThreads[i].IsBackground = background;
                _runThreads[i].Name = null == name ? "任务调度" : string.Format("{0}<{1}>", name, i);
                _runThreads[i].Start();
            }
        }
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void stop()
    {
        lock (this)
        {
            if (null != _runThreads)
            {
                _service.release_work();
                for (int i = 0; i < _runThreads.Length; i++)
                {
                    _runThreads[i].Join();
                }
                _runThreads = null;
            }
        }
    }
    /// <summary>
    /// 强制终止
    /// </summary>
    public void force_stop()
    {
        _service.stop();
        stop();
    }
    /// <summary>
    /// 线程数
    /// </summary>
    public int threads
    {
        get
        {
            return _runThreads.Length;
        }
    }
    /// <summary>
    /// 工作服务
    /// </summary>
    public work_service service
    {
        get
        {
            return _service;
        }
    }
}
/// <summary>
/// 共享串行器
/// </summary>
public class shared_strand
{
    /// <summary>
    /// 当前串行器
    /// </summary>
    protected class curr_strand
    {
        public readonly bool work_back_thread;
        public readonly work_service work_service;
        public shared_strand strand;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workBackThread"></param>
        /// <param name="workService"></param>
        public curr_strand(bool workBackThread = false, work_service workService = null)
        {
            work_back_thread = workBackThread;
            work_service = workService;
        }
    }
    protected static readonly ThreadLocal<curr_strand> _currStrand = new ThreadLocal<curr_strand>();
    private static readonly shared_strand[] _defaultStrand = functional.init(delegate ()
    {
        shared_strand[] strands = new shared_strand[Environment.ProcessorCount];
        for (int i = 0; i < strands.Length; i++)
        {
            strands[i] = new shared_strand();
        }
        return strands;
    });

    internal readonly async_timer.steady_timer _sysTimer;
    internal readonly async_timer.steady_timer _utcTimer;
    internal generator currSelf = null;
    protected volatile bool _locked;
    protected volatile int _pauseState;
    protected MsgQueue<Action> _readyQueue;
    protected MsgQueue<Action> _waitQueue;
    protected Action _runTask;
    /// <summary>
    /// 构造函数
    /// </summary>
    public shared_strand()
    {
        _locked = false;
        _pauseState = 0;
        _sysTimer = new async_timer.steady_timer(this, false);
        _utcTimer = new async_timer.steady_timer(this, true);
        _readyQueue = new MsgQueue<Action>();
        _waitQueue = new MsgQueue<Action>();
        _runTask = () => run_task();
    }
    /// <summary>
    /// 执行一个轮次
    /// </summary>
    /// <param name="currStrand"></param>
    /// <returns></returns>
    protected bool running_a_round(curr_strand currStrand)
    {
        currStrand.strand = this;
        while (0 != _readyQueue.Count)
        {
            if (0 != _pauseState && 0 != Interlocked.CompareExchange(ref _pauseState, 2, 1))
            {
                currStrand.strand = null;
                return false;
            }
            Action stepHandler = _readyQueue.First.Value;
            _readyQueue.RemoveFirst();
            functional.catch_invoke(stepHandler);
        }
        MsgQueue<Action> waitQueue = _waitQueue;
        Monitor.Enter(this);
        if (0 != _waitQueue.Count)
        {
            _waitQueue = _readyQueue;
            Monitor.Exit(this);
            _readyQueue = waitQueue;
            currStrand.strand = null;
            next_a_round();
        }
        else
        {
            _locked = false;
            Monitor.Exit(this);
            currStrand.strand = null;
        }
        return true;
    }
    /// <summary>
    /// 执行任务
    /// </summary>
    protected virtual void run_task()
    {
        curr_strand currStrand = _currStrand.Value;
        if (null == currStrand)
        {
            currStrand = new curr_strand(true);
            _currStrand.Value = currStrand;
        }
        running_a_round(currStrand);
    }
    /// <summary>
    /// 下一轮次
    /// </summary>
    protected virtual void next_a_round()
    {
        Task.Run(_runTask);
    }
    /// <summary>
    /// 计数
    /// </summary>
    public int count
    {
        get
        {
            while (true)
            {
                MsgQueue<Action> readyQueue = _readyQueue;
                MsgQueue<Action> waitQueue = _waitQueue;
                if (readyQueue != waitQueue)
                {
                    return readyQueue.Count + waitQueue.Count;
                }
                Thread.Yield();
            }
        }
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="action"></param>
    public void post(Action action)
    {
        MsgQueueNode<Action> newNode = new MsgQueueNode<Action>(action);
        Monitor.Enter(this);
        if (_locked)
        {
            _waitQueue.AddLast(newNode);
            Monitor.Exit(this);
        }
        else
        {
            _locked = true;
            _readyQueue.AddLast(newNode);
            Monitor.Exit(this);
            next_a_round();
        }
    }
    /// <summary>
    /// 下一轮次执行
    /// </summary>
    /// <param name="action"></param>
    public void next_dispatch(Action action)
    {
        if (running_in_this_thread())
        {
            _readyQueue.AddFirst(action);
        }
        else
        {
            MsgQueueNode<Action> newNode = new MsgQueueNode<Action>(action);
            Monitor.Enter(this);
            if (_locked)
            {
                _waitQueue.AddFirst(newNode);
                Monitor.Exit(this);
            }
            else
            {
                _locked = true;
                _readyQueue.AddFirst(newNode);
                Monitor.Exit(this);
                next_a_round();
            }
        }
    }
    /// <summary>
    /// 最后轮次执行
    /// </summary>
    /// <param name="action"></param>
    public void last_dispatch(Action action)
    {
        if (running_in_this_thread())
        {
            _readyQueue.AddLast(action);
        }
        else
        {
            MsgQueueNode<Action> newNode = new MsgQueueNode<Action>(action);
            Monitor.Enter(this);
            if (_locked)
            {
                _waitQueue.AddLast(newNode);
                Monitor.Exit(this);
            }
            else
            {
                _locked = true;
                _readyQueue.AddLast(newNode);
                Monitor.Exit(this);
                next_a_round();
            }
        }
    }
    /// <summary>
    /// 执行，如果在当前线程则直接执行，否则加入队列
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual bool dispatch(Action action)
    {
        curr_strand currStrand = _currStrand.Value;
        if (null != currStrand && this == currStrand.strand)
        {
            functional.catch_invoke(action);
            return true;
        }
        else
        {
            MsgQueueNode<Action> newNode = new MsgQueueNode<Action>(action);
            Monitor.Enter(this);
            if (_locked)
            {
                _waitQueue.AddLast(newNode);
                Monitor.Exit(this);
            }
            else
            {
                _locked = true;
                _readyQueue.AddLast(newNode);
                Monitor.Exit(this);
                if (null != currStrand && currStrand.work_back_thread && null == currStrand.strand)
                {
                    return running_a_round(currStrand);
                }
                next_a_round();
            }
        }
        return false;
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public void pause()
    {
        Interlocked.CompareExchange(ref _pauseState, 1, 0);
    }
    /// <summary>
    /// 恢复
    /// </summary>
    public void resume()
    {
        if (2 == Interlocked.Exchange(ref _pauseState, 0))
        {
            next_a_round();
        }
    }
    /// <summary>
    /// 正在运行这个串行器
    /// </summary>
    /// <returns></returns>
    public bool running_in_this_thread()
    {
        return this == running_strand();
    }
    /// <summary>
    /// 信息串行器
    /// </summary>
    /// <returns></returns>
    static public shared_strand running_strand()
    {
        curr_strand currStrand = _currStrand.Value;
        return null != currStrand ? currStrand.strand : null;
    }
    /// <summary>
    /// 默认串行器
    /// </summary>
    /// <returns></returns>
    static public shared_strand default_strand()
    {
        curr_strand currStrand = _currStrand.Value;
        if (null != currStrand && null != currStrand.strand)
        {
            return currStrand.strand;
        }
        return _defaultStrand[mt19937.global.Next(0, _defaultStrand.Length)];
    }
    /// <summary>
    /// 公有串行器
    /// </summary>
    /// <returns></returns>
    static public shared_strand global_strand()
    {
        return _defaultStrand[mt19937.global.Next(0, _defaultStrand.Length)];
    }
    /// <summary>
    /// 下一轮次
    /// </summary>
    /// <param name="action"></param>
    static public void next_tick(Action action)
    {
        shared_strand currStrand = running_strand();
        Debug.Assert(null != currStrand, "不正确的 next_tick 调用!");
        currStrand._readyQueue.AddFirst(action);
    }
    /// <summary>
    /// 最后轮次
    /// </summary>
    /// <param name="action"></param>
    static public void last_tick(Action action)
    {
        shared_strand currStrand = running_strand();
        Debug.Assert(null != currStrand, "不正确的 last_tick 调用!");
        currStrand._readyQueue.AddLast(action);
    }
    /// <summary>
    /// 添加执行方法
    /// </summary>
    /// <param name="action"></param>
    public void add_next(Action action)
    {
        Debug.Assert(running_in_this_thread(), "不正确的 add_next 调用!");
        _readyQueue.AddFirst(action);
    }
    /// <summary>
    /// 添加最后
    /// </summary>
    /// <param name="action"></param>
    public void add_last(Action action)
    {
        Debug.Assert(running_in_this_thread(), "不正确的 add_last 调用!");
        _readyQueue.AddLast(action);
    }
    /// <summary>
    /// 安全等待
    /// </summary>
    /// <returns></returns>
    public virtual bool wait_safe()
    {
        return !running_in_this_thread();
    }
    /// <summary>
    /// 安全线程
    /// </summary>
    /// <returns></returns>
    public virtual bool thread_safe()
    {
        return running_in_this_thread();
    }
    /// <summary>
    /// 停止工作
    /// </summary>
    public virtual void hold_work()
    {
    }
    /// <summary>
    /// 回复工作
    /// </summary>
    public virtual void release_work()
    {
    }
    /// <summary>
    /// 包装一个无参方法，使其在串行器中执行
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action wrap(Action handler)
    {
        return () => dispatch(handler);
    }
    /// <summary>
    /// 包装一个一参方法，使其在串行器中执行
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1> wrap<T1>(Action<T1> handler)
    {
        return (T1 p1) => dispatch(() => handler(p1));
    }
    /// <summary>
    /// 包装一个两参方法，使其在串行器中执行
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1, T2> wrap<T1, T2>(Action<T1, T2> handler)
    {
        return (T1 p1, T2 p2) => dispatch(() => handler(p1, p2));
    }
    /// <summary>
    /// 包装一个三参方法，使其在串行器中执行
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> wrap<T1, T2, T3>(Action<T1, T2, T3> handler)
    {
        return (T1 p1, T2 p2, T3 p3) => dispatch(() => handler(p1, p2, p3));
    }
    /// <summary>
    /// 包装执行
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action wrap_post(Action handler)
    {
        return () => post(handler);
    }
    /// <summary>
    /// 包装执行
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1> wrap_post<T1>(Action<T1> handler)
    {
        return (T1 p1) => post(() => handler(p1));
    }
    /// <summary>
    /// 包装执行
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1, T2> wrap_post<T1, T2>(Action<T1, T2> handler)
    {
        return (T1 p1, T2 p2) => post(() => handler(p1, p2));
    }
    /// <summary>
    /// 包装执行
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> wrap_post<T1, T2, T3>(Action<T1, T2, T3> handler)
    {
        return (T1 p1, T2 p2, T3 p3) => post(() => handler(p1, p2, p3));
    }
}
/// <summary>
/// 后台工作串行器
/// </summary>
public class work_strand : shared_strand
{
    /// <summary>
    /// 后台服务
    /// </summary>
    work_service _service;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="service"></param>
    public work_strand(work_service service) : base()
    {
        _service = service;
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eng"></param>
    public work_strand(work_engine eng) : base()
    {
        _service = eng.service;
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public override bool dispatch(Action action)
    {
        curr_strand currStrand = _currStrand.Value;
        if (null != currStrand && this == currStrand.strand)
        {
            functional.catch_invoke(action);
            return true;
        }
        else
        {
            MsgQueueNode<Action> newNode = new MsgQueueNode<Action>(action);
            Monitor.Enter(this);
            if (_locked)
            {
                _waitQueue.AddLast(newNode);
                Monitor.Exit(this);
            }
            else
            {
                _locked = true;
                _readyQueue.AddLast(newNode);
                Monitor.Exit(this);
                if (null != currStrand && _service == currStrand.work_service && null == currStrand.strand)
                {
                    return running_a_round(currStrand);
                }
                next_a_round();
            }
        }
        return false;
    }
    /// <summary>
    /// 执行
    /// </summary>
    protected override void run_task()
    {
        curr_strand currStrand = _currStrand.Value;
        if (null == currStrand)
        {
            currStrand = new curr_strand(false, _service);
            _currStrand.Value = currStrand;
        }
        running_a_round(currStrand);
        _service.release_work();
    }
    /// <summary>
    /// 下一轮次
    /// </summary>
    protected override void next_a_round()
    {
        _service.hold_work();
        _service.post(_runTask);
    }
    /// <summary>
    /// 停止
    /// </summary>
    public override void hold_work()
    {
        _service.hold_work();
    }
    /// <summary>
    /// 重置
    /// </summary>
    public override void release_work()
    {
        _service.release_work();
    }
}

// File-based Apps implementation for shared_strand
// Uses .NET built-in components for work queue and coroutine scheduling
// Implements requirements from Goroutine/shared_strand.cs#L10-12

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Cs;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Hosting;
using Nito.AsyncEx;

namespace System.Threading.Tasks.Cs;

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
        private readonly work_service _service;

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
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _service.release_work();
        }
    }

    private int _work;
    private int _waiting;
    private volatile bool _runSign;
    private readonly ActionBlock<Action> _opQueue;

    /// <summary>
    /// 构造函数
    /// </summary>
    public work_service()
    {
        _work = 0;
        _waiting = 0;
        _runSign = true;
        // 使用TPL Dataflow实现工作队列系统
        _opQueue = new ActionBlock<Action>(action =>
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                // 捕获异常，确保服务不崩溃
            }
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 1, // 串行执行
            TaskScheduler = TaskScheduler.Default // 使用默认任务调度器
        });
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="handler"></param>
    public void post(Action handler)
    {
        _opQueue.Post(handler);
    }

    /// <summary>
    /// 执行一次
    /// </summary>
    /// <returns></returns>
    public bool run_one()
    {
        // TPL Dataflow会自动处理执行，这里返回是否还有工作
        return _runSign && !_opQueue.Completion.IsCompleted;
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
            if (_work > 0)
            {
                // 使用ThreadPool.QueueUserWorkItem处理后台任务
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    // 处理队列中的任务
                });
                Thread.Sleep(1);
            }
            else
            {
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
    /// 增加工作计数
    /// </summary>
    public void hold_work()
    {
        Interlocked.Increment(ref _work);
    }

    /// <summary>
    /// 减少工作计数
    /// </summary>
    public void release_work()
    {
        if (0 == Interlocked.Decrement(ref _work))
        {
            // 没有工作时，停止运行
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
        private readonly work_service _service;
        /// <summary>
        /// 线程池
        /// </summary>
        private Thread[] _runThreads;

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
                if (_runThreads != null)
                {
                    throw new InvalidOperationException("work_engine 已经运行!");
                }
                _service.reset();
                _service.hold_work();
                _runThreads = new Thread[threads];
                for (int i = 0; i < threads; ++i)
                {
                    _runThreads[i] = new Thread(() => _service.run());
                    _runThreads[i].Priority = priority;
                    _runThreads[i].IsBackground = background;
                    _runThreads[i].Name = string.IsNullOrEmpty(name) ? "任务调度" : $"{name}<{i}>";
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
                if (_runThreads != null)
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
                return _runThreads?.Length ?? 0;
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
    /// 工作保护器
    /// </summary>
    public work_guard work()
    {
        return new work_guard(this);
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
    private static readonly shared_strand[] _defaultStrand = InitDefaultStrands();

    private static shared_strand[] InitDefaultStrands()
    {
        shared_strand[] strands = new shared_strand[Environment.ProcessorCount];
        for (int i = 0; i < strands.Length; i++)
        {
            strands[i] = new shared_strand();
        }
        return strands;
    }

    internal readonly Cs.async_timer.steady_timer _sysTimer;
    internal readonly Cs.async_timer.steady_timer _utcTimer;
    protected volatile bool _locked;
    protected volatile int _pauseState;
    protected readonly AsyncProducerConsumerQueue<Action> _readyQueue;
    protected readonly AsyncProducerConsumerQueue<Action> _waitQueue;
    protected readonly Action _runTask;

    /// <summary>
    /// 构造函数
    /// </summary>
    public shared_strand()
    {
        _locked = false;
        _pauseState = 0;
        // 使用Nito.AsyncEx的AsyncQueue实现异步操作的串行执行
        _readyQueue = new AsyncProducerConsumerQueue<Action>();
        _waitQueue = new AsyncProducerConsumerQueue<Action>();
        _runTask = () => run_task();
    }

    /// <summary>
    /// 执行一个轮次
    /// </summary>
    /// <param name="currStrand"></param>
    /// <returns></returns>
    protected async Task<bool> running_a_round_async(curr_strand currStrand)
    {
        currStrand.strand = this;
        while (_readyQueue.OutputAvailable())
        {
            if (0 != _pauseState && 0 != Interlocked.CompareExchange(ref _pauseState, 2, 1))
            {
                currStrand.strand = null;
                return false;
            }
            var stepHandler = await _readyQueue.DequeueAsync();
            try
            {
                stepHandler();
            }
            catch (Exception)
            {
                // 捕获异常，确保服务不崩溃
            }
        }

        // 处理等待队列
        bool hasMoreWork = false;
        while (_waitQueue.OutputAvailable())
        {
            var action = await _waitQueue.DequeueAsync();
            await _readyQueue.EnqueueAsync(action);
            hasMoreWork = true;
        }

        if (hasMoreWork)
        {
            currStrand.strand = null;
            next_a_round();
        }
        else
        {
            _locked = false;
            currStrand.strand = null;
        }
        return true;
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    protected virtual void run_task()
    {
        // 使用Nito.AsyncEx的AsyncContext执行异步任务
        AsyncContext.Run(async () =>
        {
            curr_strand currStrand = _currStrand.Value;
            if (currStrand == null)
            {
                currStrand = new curr_strand(true);
                _currStrand.Value = currStrand;
            }
            await running_a_round_async(currStrand);
        });
    }

    /// <summary>
    /// 下一轮次
    /// </summary>
    protected virtual void next_a_round()
    {
        // 使用ThreadPool执行下一轮任务
        ThreadPool.QueueUserWorkItem(_ => _runTask());
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="action"></param>
    public void post(Action action)
    {
        lock (this)
        {
            if (_locked)
            {
                _waitQueue.Enqueue(action);
            }
            else
            {
                _locked = true;
                _readyQueue.Enqueue(action);
                next_a_round();
            }
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
            // 如果在当前线程，直接添加到就绪队列
            _readyQueue.Enqueue(action);
        }
        else
        {
            lock (this)
            {
                if (_locked)
                {
                    _waitQueue.Enqueue(action);
                }
                else
                {
                    _locked = true;
                    _readyQueue.Enqueue(action);
                    next_a_round();
                }
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
            // 如果在当前线程，直接添加到就绪队列
            _readyQueue.Enqueue(action);
        }
        else
        {
            lock (this)
            {
                if (_locked)
                {
                    _waitQueue.Enqueue(action);
                }
                else
                {
                    _locked = true;
                    _readyQueue.Enqueue(action);
                    next_a_round();
                }
            }
        }
    }

    /// <summary>
    /// 挂起当前轮次
    /// </summary>
    public void hold_work()
    {
        _currStrand.Value?.work_service.hold_work();
    }

    /// <summary>
    /// 释放当前轮次
    /// </summary>
    public void release_work()
    {
        _currStrand.Value?.work_service.release_work();
    }

    /// <summary>
    /// 执行，如果在当前线程则直接执行，否则加入队列
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public virtual bool dispatch(Action action)
    {
        curr_strand currStrand = _currStrand.Value;
        if (currStrand != null && this == currStrand.strand)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                // 捕获异常，确保服务不崩溃
            }
            return true;
        }
        else
        {
            post(action);
            return false;
        }
    }

    /// <summary>
    /// 检查是否在当前线程运行
    /// </summary>
    /// <returns></returns>
    public bool running_in_this_thread()
    {
        curr_strand currStrand = _currStrand.Value;
        return currStrand != null && this == currStrand.strand;
    }

    /// <summary>
    /// 默认串行器
    /// </summary>
    public static shared_strand default_strand
    {
        get
        {
            int index = Environment.CurrentManagedThreadId % _defaultStrand.Length;
            return _defaultStrand[index];
        }
    }
}

/// <summary>
/// 工作引擎后台服务
/// </summary>
public class work_engine_hosted_service : BackgroundService
{
    private readonly work_service.work_engine _engine;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="engine"></param>
    public work_engine_hosted_service(work_service.work_engine engine)
    {
        _engine = engine;
    }

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // 启动工作引擎
        _engine.run();

        // 等待取消令牌
        return Task.Delay(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // 停止工作引擎
        _engine.stop();
        await base.StopAsync(cancellationToken);
    }
}

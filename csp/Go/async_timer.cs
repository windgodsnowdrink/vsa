//#!/usr/bin/env dotnet
//#:sdk Microsoft.NET.Sdk.Web
//#:package Scrutor@7.0.0
//#:package Carter@10.0.0
//#:package Microsoft.EntityFrameworkCore.InMemory@10.0.3
//#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.3
//#:package Microsoft.Extensions.Caching.Memory@10.0.3
//#:property TargetFramework=net10.0
//#:property RollForward=Major
//#:property Nullable=enable
//#:property ImplicitUsings=enable
//#:property PublishAot=false
//#:property PublishReadyToRun=false
//#:property PublishSingleFile=false

using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace System.Threading.Tasks.Go;

/// <summary>
/// 系统时钟类，提供高精度的时间戳功能
/// </summary>
public class system_tick
{
    /// <summary>
    /// 查询系统定时器分辨率
    /// </summary>
    [DllImport("NtDll.dll")]
    private static extern int NtQueryTimerResolution(out uint MaximumTime, out uint MinimumTime, out uint CurrentTime);

    /// <summary>
    /// 设置系统定时器分辨率
    /// </summary>
    [DllImport("NtDll.dll")]
    private static extern int NtSetTimerResolution(uint DesiredTime, uint SetResolution, out uint ActualTime);

    /// <summary>
    /// 获取性能计数器的值
    /// </summary>
    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    /// <summary>
    /// 获取性能计数器的频率
    /// </summary>
    [DllImport("kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long frequency);

    /// <summary>
    /// 系统时钟实例
    /// </summary>
    private static system_tick _pcCycle = new system_tick();

#if CHECK_STEP_TIMEOUT
/// <summary>
/// 单步调试检测标志
/// </summary>
private static volatile bool _checkStepDebugSign = false;
#endif

    /// <summary>
    /// 秒周期
    /// </summary>
    private double _sCycle;

    /// <summary>
    /// 毫秒周期
    /// </summary>
    private double _msCycle;

    /// <summary>
    /// 微秒周期
    /// </summary>
    private double _usCycle;

    /// <summary>
    /// 构造函数，初始化时钟周期
    /// </summary>
    private system_tick()
    {
        if (!QueryPerformanceFrequency(out long freq))
        {
            throw new Exception("QueryPerformanceFrequency 初始化失败!");
        }
        _sCycle = 1.0 / (double)freq;
        _msCycle = 1000.0 / (double)freq;
        _usCycle = 1000000.0 / (double)freq;

#if CHECK_STEP_TIMEOUT
    // 创建单步调试检测线程
    Thread checkStepDebug = new Thread(delegate ()
    {
        long checkTick = get_tick_ms();
        while (true)
        {
            Thread.Sleep(80);
            long oldTick = checkTick;
            checkTick = get_tick_ms();
            _checkStepDebugSign = (checkTick - oldTick) > 100;
        }
    });
    checkStepDebug.Priority = ThreadPriority.Highest;
    checkStepDebug.IsBackground = true;
    checkStepDebug.Name = "单步调试检测";
    checkStepDebug.Start();
#endif
    }

    /// <summary>
    /// 设置高精度定时器
    /// </summary>
    public static void high_resolution()
    {
        uint MaximumTime = 0, MinimumTime = 0, CurrentTime = 0, ActualTime = 0;
        if (0 == NtQueryTimerResolution(out MaximumTime, out MinimumTime, out CurrentTime))
        {
            NtSetTimerResolution(MinimumTime, 1, out ActualTime);
        }
    }

    /// <summary>
    /// 获取原始时钟刻度
    /// </summary>
    /// <returns>原始时钟刻度值</returns>
    public static long get_tick()
    {
        long quadPart;
        QueryPerformanceCounter(out quadPart);
        return quadPart;
    }

    /// <summary>
    /// 获取微秒级时钟刻度
    /// </summary>
    /// <returns>微秒级时钟刻度值</returns>
    public static long get_tick_us()
    {
        long quadPart;
        QueryPerformanceCounter(out quadPart);
        return (long)((double)quadPart * _pcCycle._usCycle);
    }

    /// <summary>
    /// 获取毫秒级时钟刻度
    /// </summary>
    /// <returns>毫秒级时钟刻度值</returns>
    public static long get_tick_ms()
    {
        long quadPart;
        QueryPerformanceCounter(out quadPart);
        return (long)((double)quadPart * _pcCycle._msCycle);
    }

    /// <summary>
    /// 获取秒级时钟刻度
    /// </summary>
    /// <returns>秒级时钟刻度值</returns>
    public static long get_tick_s()
    {
        long quadPart;
        QueryPerformanceCounter(out quadPart);
        return (long)((double)quadPart * _pcCycle._sCycle);
    }

    /// <summary>
    /// 获取微秒级时钟刻度属性
    /// </summary>
    public static long us
    {
        get
        {
            return get_tick_us();
        }
    }

    /// <summary>
    /// 获取毫秒级时钟刻度属性
    /// </summary>
    public static long ms
    {
        get
        {
            return get_tick_ms();
        }
    }

    /// <summary>
    /// 获取秒级时钟刻度属性
    /// </summary>
    public static long s
    {
        get
        {
            return get_tick_s();
        }
    }

#if CHECK_STEP_TIMEOUT
/// <summary>
/// 检查是否在单步调试
/// </summary>
/// <returns>是否在单步调试</returns>
public static bool check_step_debugging()
{
    return _checkStepDebugSign;
}
#endif
}

/// <summary>
/// UTC时钟类，提供基于UTC时间的时间戳功能
/// </summary>
public static class utc_tick
{
    /// <summary>
    /// 获取系统时间作为文件时间
    /// </summary>
    [DllImport("kernel32.dll")]
    private static extern void GetSystemTimeAsFileTime(out long time);

    /// <summary>
    /// 文件时间偏移量
    /// </summary>
    public const long fileTimeOffset = 504911232000000000L;

    /// <summary>
    /// 获取原始UTC时钟刻度
    /// </summary>
    /// <returns>原始UTC时钟刻度值</returns>
    public static long get_tick()
    {
        long tm;
        GetSystemTimeAsFileTime(out tm);
        return tm;
    }

    /// <summary>
    /// 获取微秒级UTC时钟刻度
    /// </summary>
    /// <returns>微秒级UTC时钟刻度值</returns>
    public static long get_tick_us()
    {
        long tm;
        GetSystemTimeAsFileTime(out tm);
        return tm / 10;
    }

    /// <summary>
    /// 获取毫秒级UTC时钟刻度
    /// </summary>
    /// <returns>毫秒级UTC时钟刻度值</returns>
    public static long get_tick_ms()
    {
        long tm;
        GetSystemTimeAsFileTime(out tm);
        return tm / 10000;
    }

    /// <summary>
    /// 获取秒级UTC时钟刻度
    /// </summary>
    /// <returns>秒级UTC时钟刻度值</returns>
    public static long get_tick_s()
    {
        long tm;
        GetSystemTimeAsFileTime(out tm);
        return tm / 10000000;
    }

    /// <summary>
    /// 获取微秒级UTC时钟刻度属性
    /// </summary>
    public static long us
    {
        get
        {
            return get_tick_us();
        }
    }

    /// <summary>
    /// 获取毫秒级UTC时钟刻度属性
    /// </summary>
    public static long ms
    {
        get
        {
            return get_tick_ms();
        }
    }

    /// <summary>
    /// 获取秒级UTC时钟刻度属性
    /// </summary>
    public static long s
    {
        get
        {
            return get_tick_s();
        }
    }
}

/// <summary>
/// 异步定时器类，提供高精度的定时器功能
/// </summary>
/// <remarks>
/// 该类提供了多种定时器功能，包括超时定时器、间隔定时器、截止时间定时器等。
/// 支持微秒级精度，并且可以选择使用系统时间或UTC时间。
/// </remarks>
public class async_timer
{
    /// <summary>
    /// 稳定定时器句柄结构
    /// </summary>
    struct steady_timer_handle
    {
        /// <summary>
        /// 绝对时间（微秒）
        /// </summary>
        public long absus;

        /// <summary>
        /// 周期（微秒）
        /// </summary>
        public long period;

        /// <summary>
        /// 定时器节点
        /// </summary>
        public MapNode<long, async_timer> node;
    }

    /// <summary>
    /// 稳定定时器内部类
    /// </summary>
    internal class steady_timer
    {
        /// <summary>
        /// 可等待事件句柄结构
        /// </summary>
        struct waitable_event_handle
        {
            /// <summary>
            /// 事件ID
            /// </summary>
            public int id;

            /// <summary>
            /// 稳定定时器实例
            /// </summary>
            public steady_timer steadyTimer;
        }

        /// <summary>
        /// 可等待定时器类
        /// </summary>
        class waitable_timer
        {
            /// <summary>
            /// 创建可等待定时器
            /// </summary>
            [DllImport("kernel32.dll")]
            private static extern int CreateWaitableTimer(int lpTimerAttributes, int bManualReset, int lpTimerName);

            /// <summary>
            /// 设置可等待定时器
            /// </summary>
            [DllImport("kernel32.dll")]
            private static extern int SetWaitableTimer(int hTimer, ref long pDueTime, int lPeriod, int pfnCompletionRoutine, int lpArgToCompletionRoutine, int fResume);

            /// <summary>
            /// 取消可等待定时器
            /// </summary>
            [DllImport("kernel32.dll")]
            private static extern int CancelWaitableTimer(int hTimer);

            /// <summary>
            /// 关闭句柄
            /// </summary>
            [DllImport("kernel32.dll")]
            private static extern int CloseHandle(int hObject);

            /// <summary>
            /// 等待单个对象
            /// </summary>
            [DllImport("kernel32.dll")]
            private static extern int WaitForSingleObject(int hHandle, int dwMilliseconds);

            /// <summary>
            /// 系统定时器实例
            /// </summary>
            static public readonly waitable_timer sysTimer = new waitable_timer(false);

            /// <summary>
            /// UTC定时器实例
            /// </summary>
            static public readonly waitable_timer utcTimer = new waitable_timer(true);

            /// <summary>
            /// 是否为UTC模式
            /// </summary>
            bool _utcMode;

            /// <summary>
            /// 是否已退出
            /// </summary>
            bool _exited;

            /// <summary>
            /// 定时器句柄
            /// </summary>
            int _timerHandle;

            /// <summary>
            /// 过期时间
            /// </summary>
            long _expireTime;

            /// <summary>
            /// 定时器线程
            /// </summary>
            Thread _timerThread;

            /// <summary>
            /// 工作引擎
            /// </summary>
            work_engine _workEngine;

            /// <summary>
            /// 事件队列
            /// </summary>
            Map<long, waitable_event_handle> _eventsQueue;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="utcMode">是否为UTC模式</param>
            waitable_timer(bool utcMode)
            {
                _utcMode = utcMode;
                _exited = false;
                _expireTime = long.MaxValue;
                _eventsQueue = new Map<long, waitable_event_handle>(true);
                _timerHandle = CreateWaitableTimer(0, 0, 0);
                _workEngine = new work_engine();
                _timerThread = new Thread(timer_thread);
                _timerThread.Priority = ThreadPriority.Highest;
                _timerThread.IsBackground = true;
                _timerThread.Name = _utcMode ? "UTC定时器" : "系统定时器";
                _workEngine.run(1, ThreadPriority.Highest, true, _utcMode ? "UTC定时器调度" : "系统定时器调度");
                _timerThread.Start();
            }

            /// <summary>
            /// 设置定时器
            /// </summary>
            private void set_timer()
            {
                if (_utcMode)
                {
                    long sleepTime = _expireTime * 10;
                    sleepTime = sleepTime > 0 ? sleepTime : 0;
                    SetWaitableTimer(_timerHandle, ref sleepTime, 0, 0, 0, 0);
                }
                else
                {
                    long sleepTime = -(_expireTime - system_tick.get_tick_us()) * 10;
                    sleepTime = sleepTime < 0 ? sleepTime : 0;
                    SetWaitableTimer(_timerHandle, ref sleepTime, 0, 0, 0, 0);
                }
            }

            /// <summary>
            /// 关闭定时器
            /// </summary>
            public void close()
            {
                _workEngine.service.post(delegate ()
                {
                    _exited = true;
                    long sleepTime = 0;
                    SetWaitableTimer(_timerHandle, ref sleepTime, 0, 0, 0, 0);
                });
                _timerThread.Join();
                _workEngine.stop();
                CloseHandle(_timerHandle);
            }

            /// <summary>
            /// 添加事件
            /// </summary>
            /// <param name="absus">绝对时间（微秒）</param>
            /// <param name="eventHandle">事件句柄</param>
            public void append_event(long absus, waitable_event_handle eventHandle)
            {
                _workEngine.service.post(delegate ()
                {
                    if (_exited) return;
                    eventHandle.steadyTimer._waitableNode = _eventsQueue.Insert(absus, eventHandle);
                    if (absus < _expireTime)
                    {
                        _expireTime = absus;
                        set_timer();
                    }
                });
            }

            /// <summary>
            /// 移除事件
            /// </summary>
            /// <param name="steadyTime">稳定定时器实例</param>
            public void remove_event(steady_timer steadyTime)
            {
                _workEngine.service.post(delegate ()
                {
                    if (_exited) return;
                    if (null != steadyTime._waitableNode)
                    {
                        long lastAbsus = steadyTime._waitableNode.Key;
                        _eventsQueue.Remove(steadyTime._waitableNode);
                        steadyTime._waitableNode = null;
                        if (0 == _eventsQueue.Count)
                        {
                            _expireTime = long.MaxValue;
                            CancelWaitableTimer(_timerHandle);
                        }
                        else if (lastAbsus == _expireTime)
                        {
                            _expireTime = _eventsQueue.First.Key;
                            set_timer();
                        }
                    }
                });
            }

            /// <summary>
            /// 更新事件
            /// </summary>
            /// <param name="absus">绝对时间（微秒）</param>
            /// <param name="eventHandle">事件句柄</param>
            public void update_event(long absus, waitable_event_handle eventHandle)
            {
                _workEngine.service.post(delegate ()
                {
                    if (_exited) return;
                    if (null != eventHandle.steadyTimer._waitableNode)
                    {
                        _eventsQueue.Insert(_eventsQueue.ReNewNode(eventHandle.steadyTimer._waitableNode, absus, eventHandle));
                    }
                    else
                    {
                        eventHandle.steadyTimer._waitableNode = _eventsQueue.Insert(absus, eventHandle);
                    }
                    long newAbsus = _eventsQueue.First.Key;
                    if (newAbsus < _expireTime)
                    {
                        _expireTime = newAbsus;
                        set_timer();
                    }
                });
            }

            /// <summary>
            /// 定时器线程
            /// </summary>
            private void timer_thread()
            {
                Action timerHandler = timer_handler;
                while (0 == WaitForSingleObject(_timerHandle, -1) && !_exited)
                {
                    _workEngine.service.post(timerHandler);
                }
            }

            /// <summary>
            /// 定时器处理函数
            /// </summary>
            private void timer_handler()
            {
                _expireTime = long.MaxValue;
                while (0 != _eventsQueue.Count)
                {
                    MapNode<long, waitable_event_handle> first = _eventsQueue.First;
                    long absus = first.Key;
                    long ct = _utcMode ? utc_tick.get_tick_us() : system_tick.get_tick_us();
                    if (absus > ct)
                    {
                        _expireTime = absus;
                        set_timer();
                        break;
                    }
                    first.Value.steadyTimer._waitableNode = null;
                    first.Value.steadyTimer.timer_handler(first.Value.id);
                    _eventsQueue.Remove(first);
                }
            }
        }

        /// <summary>
        /// 是否为UTC模式
        /// </summary>
        bool _utcMode;

        /// <summary>
        /// 是否循环
        /// </summary>
        bool _looping;

        /// <summary>
        /// 定时器计数
        /// </summary>
        int _timerCount;

        /// <summary>
        /// 过期时间
        /// </summary>
        long _expireTime;

        /// <summary>
        /// 共享 Strand
        /// </summary>
        shared_strand _strand;

        /// <summary>
        /// 可等待节点
        /// </summary>
        MapNode<long, waitable_event_handle> _waitableNode;

        /// <summary>
        /// 定时器队列
        /// </summary>
        Map<long, async_timer> _timerQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strand">共享 Strand</param>
        /// <param name="utcMode">是否为UTC模式</param>
        public steady_timer(shared_strand strand, bool utcMode)
        {
            _utcMode = utcMode;
            _timerCount = 0;
            _looping = false;
            _expireTime = long.MaxValue;
            _strand = strand;
            _timerQueue = new Map<long, async_timer>(true);
        }

        /// <summary>
        /// 设置超时
        /// </summary>
        /// <param name="asyncTimer">异步定时器实例</param>
        public void timeout(async_timer asyncTimer)
        {
            long absus = asyncTimer._timerHandle.absus;
            asyncTimer._timerHandle.node = _timerQueue.Insert(absus, asyncTimer);
            if (!_looping)
            {
                _looping = true;
                _expireTime = absus;
                timer_loop(absus);
            }
            else if (absus < _expireTime)
            {
                _expireTime = absus;
                timer_reloop(absus);
            }
        }

        /// <summary>
        /// 取消定时器
        /// </summary>
        /// <param name="asyncTimer">异步定时器实例</param>
        public void cancel(async_timer asyncTimer)
        {
            if (null != asyncTimer._timerHandle.node)
            {
                _timerQueue.Remove(asyncTimer._timerHandle.node);
                asyncTimer._timerHandle.node = null;
                if (0 == _timerQueue.Count)
                {
                    _timerCount++;
                    _expireTime = 0;
                    _looping = false;
                    if (_utcMode)
                    {
                        waitable_timer.utcTimer.remove_event(this);
                    }
                    else
                    {
                        waitable_timer.sysTimer.remove_event(this);
                    }
                }
                else if (asyncTimer._timerHandle.absus == _expireTime)
                {
                    _expireTime = _timerQueue.First.Key;
                    timer_reloop(_expireTime);
                }
            }
        }

        /// <summary>
        /// 重新设置超时
        /// </summary>
        /// <param name="asyncTimer">异步定时器实例</param>
        public void re_timeout(async_timer asyncTimer)
        {
            long absus = asyncTimer._timerHandle.absus;
            if (null != asyncTimer._timerHandle.node)
            {
                _timerQueue.Insert(_timerQueue.ReNewNode(asyncTimer._timerHandle.node, absus, asyncTimer));
            }
            else
            {
                asyncTimer._timerHandle.node = _timerQueue.Insert(absus, asyncTimer);
            }
            long newAbsus = _timerQueue.First.Key;
            if (!_looping)
            {
                _looping = true;
                _expireTime = newAbsus;
                timer_loop(newAbsus);
            }
            else if (newAbsus < _expireTime)
            {
                _expireTime = newAbsus;
                timer_reloop(newAbsus);
            }
        }

        /// <summary>
        /// 定时器处理函数
        /// </summary>
        /// <param name="id">事件ID</param>
        public void timer_handler(int id)
        {
            if (id != _timerCount)
            {
                return;
            }
            _strand.post(delegate ()
            {
                if (id == _timerCount)
                {
                    _expireTime = long.MinValue;
                    while (0 != _timerQueue.Count)
                    {
                        MapNode<long, async_timer> first = _timerQueue.First;
                        if (first.Key > (_utcMode ? utc_tick.get_tick_us() : system_tick.get_tick_us()))
                        {
                            _expireTime = first.Key;
                            timer_loop(_expireTime);
                            return;
                        }
                        else
                        {
                            first.Value._timerHandle.node = null;
                            first.Value.timer_handler();
                            _timerQueue.Remove(first);
                        }
                    }
                    _looping = false;
                }
            });
        }

        /// <summary>
        /// 定时器循环
        /// </summary>
        /// <param name="absus">绝对时间（微秒）</param>
        void timer_loop(long absus)
        {
            if (_utcMode)
            {
                waitable_timer.utcTimer.append_event(absus, new waitable_event_handle { id = ++_timerCount, steadyTimer = this });
            }
            else
            {
                waitable_timer.sysTimer.append_event(absus, new waitable_event_handle { id = ++_timerCount, steadyTimer = this });
            }
        }

        /// <summary>
        /// 定时器重新循环
        /// </summary>
        /// <param name="absus">绝对时间（微秒）</param>
        void timer_reloop(long absus)
        {
            if (_utcMode)
            {
                waitable_timer.utcTimer.update_event(absus, new waitable_event_handle { id = ++_timerCount, steadyTimer = this });
            }
            else
            {
                waitable_timer.sysTimer.update_event(absus, new waitable_event_handle { id = ++_timerCount, steadyTimer = this });
            }
        }
    }

    /// <summary>
    /// 共享 Strand
    /// </summary>
    shared_strand _strand;

    /// <summary>
    /// 定时器处理函数
    /// </summary>
    Action? _handler;

    /// <summary>
    /// 稳定定时器句柄
    /// </summary>
    steady_timer_handle _timerHandle;

    /// <summary>
    /// 定时器计数
    /// </summary>
    int _timerCount;

    /// <summary>
    /// 开始时间
    /// </summary>
    long _beginTick;

    /// <summary>
    /// 是否为间隔定时器
    /// </summary>
    bool _isInterval;

    /// <summary>
    /// 是否在顶部调用
    /// </summary>
    bool _onTopCall;

    /// <summary>
    /// 是否为UTC模式
    /// </summary>
    bool _utcMode;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="strand">共享 Strand</param>
    /// <param name="utcMode">是否为UTC模式</param>
    public async_timer(shared_strand strand, bool utcMode = false)
    {
        _strand = strand;
        _timerCount = 0;
        _beginTick = 0;
        _isInterval = false;
        _onTopCall = false;
        _utcMode = utcMode;
    }

    /// <summary>
    /// 构造函数（使用默认 Strand）
    /// </summary>
    /// <param name="utcMode">是否为UTC模式</param>
    public async_timer(bool utcMode = false) 
        : this(shared_strand.default_strand(), utcMode) { }

    /// <summary>
    /// 获取自身 Strand
    /// </summary>
    /// <returns>共享 Strand 实例</returns>
    public shared_strand self_strand()
    {
        return _strand;
    }

    /// <summary>
    /// 定时器处理函数
    /// </summary>
    private void timer_handler()
    {
        _onTopCall = true;
        if (_isInterval)
        {
            int lastTc = _timerCount;
            functional.catch_invoke(_handler);
            if (lastTc == _timerCount)
            {
                begin_timer(_beginTick, _timerHandle.absus += _timerHandle.period, _timerHandle.period);
            }
        }
        else
        {
            Action handler = _handler;
            _handler = null;
            _strand.release_work();
            functional.catch_invoke(handler);
        }
        _onTopCall = false;
    }

    /// <summary>
    /// 开始定时器
    /// </summary>
    /// <param name="nowus">当前时间（微秒）</param>
    /// <param name="absus">绝对时间（微秒）</param>
    /// <param name="period">周期（微秒）</param>
    private void begin_timer(long nowus, long absus, long period)
    {
        _beginTick = nowus;
        if (nowus < absus)
        {
            _timerCount++;
            _timerHandle.absus = absus;
            _timerHandle.period = period;
            if (_utcMode)
            {
                _strand._utcTimer.timeout(this);
            }
            else
            {
                _strand._sysTimer.timeout(this);
            }
        }
        else
        {
            int tmId = ++_timerCount;
            _timerHandle.absus = absus;
            _timerHandle.period = period;
            _strand.post(delegate ()
            {
                if (tmId == _timerCount)
                {
                    timer_handler();
                }
            });
        }
    }

    /// <summary>
    /// 重新开始定时器
    /// </summary>
    /// <param name="nowus">当前时间（微秒）</param>
    /// <param name="absus">绝对时间（微秒）</param>
    /// <param name="period">周期（微秒）</param>
    private void re_begin_timer(long nowus, long absus, long period)
    {
        _beginTick = nowus;
        if (nowus < absus)
        {
            _timerCount++;
            _timerHandle.absus = absus;
            _timerHandle.period = period;
            if (_utcMode)
            {
                _strand._utcTimer.re_timeout(this);
            }
            else
            {
                _strand._sysTimer.re_timeout(this);
            }
        }
        else
        {
            if (_utcMode)
            {
                _strand._utcTimer.cancel(this);
            }
            else
            {
                _strand._sysTimer.cancel(this);
            }
            int tmId = ++_timerCount;
            _timerHandle.absus = absus;
            _timerHandle.period = period;
            _strand.post(delegate ()
            {
                if (tmId == _timerCount)
                {
                    timer_handler();
                }
            });
        }
    }

    /// <summary>
    /// 设置微秒级超时
    /// </summary>
    /// <param name="us">超时时间（微秒）</param>
    /// <param name="handler">处理函数</param>
    /// <returns>当前时间（微秒）</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public long timeout_us(long us, Action handler)
    {
        Debug.Assert(_strand.running_in_this_thread() && null == _handler && null != handler, "不正确的 timeout_us 调用!");
        _isInterval = false;
        _handler = handler;
        _strand.hold_work();
        long nowus = _utcMode ? utc_tick.get_tick_us() : system_tick.get_tick_us();
        begin_timer(nowus, us > 0 ? nowus + us : nowus, us > 0 ? us : 0);
        return nowus;
    }

    /// <summary>
    /// 设置微秒级截止时间
    /// </summary>
    /// <param name="us">截止时间（微秒）</param>
    /// <param name="handler">处理函数</param>
    /// <returns>当前时间（微秒）</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public long deadline_us(long us, Action handler)
    {
        Debug.Assert(_strand.running_in_this_thread() && null == _handler && null != handler, "不正确的 deadline_us 调用!");
        _isInterval = false;
        _handler = handler;
        _strand.hold_work();
        long nowus = _utcMode ? utc_tick.get_tick_us() : system_tick.get_tick_us();
        begin_timer(nowus, us, us > nowus ? us - nowus : 0);
        return nowus;
    }

    /// <summary>
    /// 设置日期截止时间
    /// </summary>
    /// <param name="date">截止日期</param>
    /// <param name="handler">处理函数</param>
    /// <returns>当前时间（微秒）</returns>
    public long deadline(DateTime date, Action handler)
    {
        if (_utcMode)
        {
            if (DateTimeKind.Utc == date.Kind)
            {
                return deadline_us((date.Ticks - utc_tick.fileTimeOffset) / 10, handler);
            }
            return deadline_us((date.Ticks - TimeZoneInfo.Local.BaseUtcOffset.Ticks - utc_tick.fileTimeOffset) / 10, handler);
        }
        else
        {
            if (DateTimeKind.Utc == date.Kind)
            {
                return timeout_us((date.Ticks - DateTime.UtcNow.Ticks) / 10, handler);
            }
            return timeout_us((date.Ticks - DateTime.Now.Ticks) / 10, handler);
        }
    }

    /// <summary>
    /// 设置毫秒级超时
    /// </summary>
    /// <param name="ms">超时时间（毫秒）</param>
    /// <param name="handler">处理函数</param>
    /// <returns>当前时间（微秒）</returns>
    public long timeout(int ms, Action handler)
    {
        return timeout_us((long)ms * 1000, handler);
    }

    /// <summary>
    /// 设置毫秒级截止时间
    /// </summary>
    /// <param name="ms">截止时间（毫秒）</param>
    /// <param name="handler">处理函数</param>
    /// <returns>当前时间（微秒）</returns>
    public long deadline(long ms, Action handler)
    {
        return deadline_us(ms * 1000, handler);
    }

    /// <summary>
    /// 设置毫秒级间隔定时器
    /// </summary>
    /// <param name="ms">间隔时间（毫秒）</param>
    /// <param name="handler">处理函数</param>
    /// <param name="immed">是否立即执行</param>
    /// <returns>当前时间（微秒）</returns>
    public long interval(int ms, Action handler, bool immed = false)
    {
        return interval_us((long)ms * 1000, handler, immed);
    }

    /// <summary>
    /// 设置两个毫秒级间隔的定时器
    /// </summary>
    /// <param name="ms1">第一个间隔时间（毫秒）</param>
    /// <param name="ms2">第二个间隔时间（毫秒）</param>
    /// <param name="handler">处理函数</param>
    /// <param name="immed">是否立即执行</param>
    /// <returns>当前时间（微秒）</returns>
    public long interval2(int ms1, int ms2, Action handler, bool immed = false)
    {
        return interval2_us((long)ms1 * 1000, (long)ms2 * 1000, handler, immed);
    }

    /// <summary>
    /// 设置微秒级间隔定时器
    /// </summary>
    /// <param name="us">间隔时间（微秒）</param>
    /// <param name="handler">处理函数</param>
    /// <param name="immed">是否立即执行</param>
    /// <returns>当前时间（微秒）</returns>
    public long interval_us(long us, Action handler, bool immed = false)
    {
        return interval2_us(us, us, handler, immed);
    }

    /// <summary>
    /// 设置两个微秒级间隔的定时器
    /// </summary>
    /// <param name="us1">第一个间隔时间（微秒）</param>
    /// <param name="us2">第二个间隔时间（微秒）</param>
    /// <param name="handler">处理函数</param>
    /// <param name="immed">是否立即执行</param>
    /// <returns>当前时间（微秒）</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public long interval2_us(long us1, long us2, Action handler, bool immed = false)
    {
        Debug.Assert(_strand.running_in_this_thread() && null == _handler && null != handler, "不正确的 interval2_us 调用!");
        _isInterval = true;
        _handler = handler;
        _strand.hold_work();
        long nowus = _utcMode ? utc_tick.get_tick_us() : system_tick.get_tick_us();
        begin_timer(nowus, us1 > 0 ? nowus + us1 : nowus, us2 > 0 ? us2 : 0);
        if (immed)
        {
            functional.catch_invoke(_handler);
        }
        return nowus;
    }

    /// <summary>
    /// 重启定时器（毫秒）
    /// </summary>
    /// <param name="ms">新的间隔时间（毫秒），-1 表示使用原有周期</param>
    /// <returns>是否重启成功</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public bool restart(int ms = -1)
    {
        return restart_us((long)ms * 1000);
    }

    /// <summary>
    /// 重启定时器（微秒）
    /// </summary>
    /// <param name="us">新的间隔时间（微秒），-1 表示使用原有周期</param>
    /// <returns>是否重启成功</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public bool restart_us(long us = -1)
    {
        Debug.Assert(_strand.running_in_this_thread(), "不正确的 restart_us 调用!");
        if (null != _handler)
        {
            long nowus = _utcMode ? utc_tick.get_tick_us() : system_tick.get_tick_us();
            re_begin_timer(nowus, us < 0 ? nowus + _timerHandle.period : nowus + us, _timerHandle.period);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 提前执行定时器
    /// </summary>
    /// <returns>是否执行成功</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public bool Advance()
    {
        Debug.Assert(_strand.running_in_this_thread(), "不正确的 advance 调用!");
        if (null != _handler)
        {
            if (!_isInterval)
            {
                _timerCount++;
                if (_utcMode)
                {
                    _strand._utcTimer.cancel(this);
                }
                else
                {
                    _strand._sysTimer.cancel(this);
                }
                _beginTick = 0;
                Action handler = _handler;
                _handler = null;
                functional.catch_invoke(handler);
                _strand.release_work();
                return true;
            }
            else if (!_onTopCall)
            {
                functional.catch_invoke(_handler);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 取消定时器
    /// </summary>
    /// <returns>开始时间（微秒）</returns>
    /// <exception cref="AssertionException">当调用不正确时抛出</exception>
    public long Cancel()
    {
        Debug.Assert(_strand.running_in_this_thread(), "不正确的 cancel 调用!");
        if (null != _handler)
        {
            _timerCount++;
            if (_utcMode)
            {
                _strand._utcTimer.cancel(this);
            }
            else
            {
                _strand._sysTimer.cancel(this);
            }
            long lastBegin = _beginTick;
            _beginTick = 0;
            _handler = null;
            _strand.release_work();
            return lastBegin;
        }
        return 0;
    }
}

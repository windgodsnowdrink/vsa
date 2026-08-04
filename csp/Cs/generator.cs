// File-based Apps implementation for generator
// Uses .NET built-in coroutine features and libraries

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Nito.AsyncEx;
using LanguageExt;
using System.Diagnostics;

namespace System.Threading.Tasks.Cs;

/// <summary>
/// 多重检查结构，用于确保回调只执行一次
/// </summary>
struct multi_check
{
    /// <summary>
    /// 标记是否已回调
    /// </summary>
    public bool callbacked;
    /// <summary>
    /// 标记是否开始退出
    /// </summary>
    public bool beginQuit;

    /// <summary>
    /// 检查并标记回调状态
    /// </summary>
    /// <returns>返回之前的回调状态</returns>
    public bool check()
    {
        bool t = callbacked;
        callbacked = true;
        return t;
    }
}

/// <summary>
/// 协程生成器类，用于创建和管理协程
/// </summary>
public class generator
{
    /// <summary>
    /// 停止异常类
    /// </summary>
    public class stop_exception : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal stop_exception() { }
    }

    /// <summary>
    /// 本地值异常类
    /// </summary>
    public class local_value_exception : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal local_value_exception() { }
    }

    /// <summary>
    /// 子协程管理类
    /// </summary>
    public class children
    {
        private List<generator> _children = new List<generator>();
        private object _lock = new object();

        /// <summary>
        /// 启动一个新的子协程
        /// </summary>
        /// <param name="func">协程函数</param>
        public void go(Func<Task> func)
        {
            var gen = generator.tgo(func);
            lock (_lock)
            {
                _children.Add(gen);
            }
        }

        /// <summary>
        /// 启动一个新的子协程并返回生成器实例
        /// </summary>
        /// <param name="func">协程函数</param>
        /// <returns>生成器实例</returns>
        public generator tgo(Func<Task> func)
        {
            var gen = generator.tgo(func);
            lock (_lock)
            {
                _children.Add(gen);
            }
            return gen;
        }

        /// <summary>
        /// 等待所有子协程完成
        /// </summary>
        /// <returns>任务</returns>
        public async Task wait_all()
        {
            List<generator> children;
            lock (_lock)
            {
                children = new List<generator>(_children);
            }
            foreach (var child in children)
            {
                await child.async_wait();
            }
        }

        /// <summary>
        /// 等待任意一个子协程完成
        /// </summary>
        /// <returns>完成的生成器实例</returns>
        public async Task<generator> wait_any()
        {
            List<generator> children;
            lock (_lock)
            {
                children = new List<generator>(_children);
            }
            var tasks = children.Select(child => child.async_wait().ContinueWith(_ => child)).ToArray();
            var completedTask = await Task.WhenAny(tasks);
            return await completedTask;
        }

        /// <summary>
        /// 等待指定的子协程完成
        /// </summary>
        /// <param name="gen">生成器实例</param>
        /// <returns>任务</returns>
        public async Task wait(generator gen)
        {
            await gen.async_wait();
        }

        /// <summary>
        /// 超时等待指定的子协程完成
        /// </summary>
        /// <param name="ms">超时时间（毫秒）</param>
        /// <param name="gen">生成器实例</param>
        /// <returns>是否成功</returns>
        public async Task<bool> timed_wait(int ms, generator gen)
        {
            var task = gen.async_wait();
            var completedTask = await Task.WhenAny(task, Task.Delay(ms));
            return completedTask == task;
        }

        /// <summary>
        /// 超时等待所有子协程完成
        /// </summary>
        /// <param name="ms">超时时间（毫秒）</param>
        /// <returns>成功完成的生成器实例列表</returns>
        public async Task<List<generator>> timed_wait_all(int ms)
        {
            List<generator> children;
            lock (_lock)
            {
                children = new List<generator>(_children);
            }
            var tasks = children.Select(child => child.async_wait().ContinueWith(_ => child)).ToArray();
            var completedTasks = await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(ms));
            if (completedTasks is Task<generator[]>
                completedTask && completedTask.Status == TaskStatus.RanToCompletion)
            {
                return completedTask.Result.ToList();
            }
            return new List<generator>();
        }

        /// <summary>
        /// 停止所有子协程
        /// </summary>
        /// <returns>任务</returns>
        public async Task stop()
        {
            List<generator> children;
            lock (_lock)
            {
                children = new List<generator>(_children);
            }
            foreach (var child in children)
            {
                child.stop();
            }
            await Task.WhenAll(children.Select(child => child.async_wait()));
        }
    }

    /// <summary>
    /// 等待组类
    /// </summary>
    public class wait_group
    {
        private int _count;
        private object _lock = new object();
        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public wait_group() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="count">初始计数</param>
        public wait_group(int count)
        {
            _count = count;
        }

        /// <summary>
        /// 增加计数
        /// </summary>
        public void add()
        {
            lock (_lock)
            {
                _count++;
            }
        }

        /// <summary>
        /// 增加指定计数
        /// </summary>
        /// <param name="delta">增加的计数</param>
        public void add(int delta)
        {
            lock (_lock)
            {
                _count += delta;
            }
        }

        /// <summary>
        /// 减少计数
        /// </summary>
        public void done()
        {
            lock (_lock)
            {
                if (--_count == 0)
                {
                    _tcs.TrySetResult(true);
                }
            }
        }

        /// <summary>
        /// 等待计数为0
        /// </summary>
        /// <returns>任务</returns>
        public async Task wait()
        {
            await _tcs.Task;
        }

        /// <summary>
        /// 同步等待计数为0
        /// </summary>
        public void sync_wait()
        {
            _tcs.Task.Wait();
        }

        /// <summary>
        /// 超时等待计数为0
        /// </summary>
        /// <param name="ms">超时时间（毫秒）</param>
        /// <returns>是否成功</returns>
        public bool sync_timed_wait(int ms)
        {
            return _tcs.Task.Wait(ms);
        }
    }

    /// <summary>
    /// 本地值类
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    public class local<T>
    {
        private static long _idCount = 0;
        private readonly long _id = Interlocked.Increment(ref _idCount);
        private static ThreadLocal<Dictionary<long, object>> _localValues = new ThreadLocal<Dictionary<long, object>>(() => new Dictionary<long, object>());

        /// <summary>
        /// 值
        /// </summary>
        public T value
        {
            get
            {
                var values = _localValues.Value;
                if (values.TryGetValue(_id, out var val))
                {
                    return (T)val;
                }
                throw new local_value_exception();
            }
            set
            {
                var values = _localValues.Value;
                values[_id] = value;
            }
        }

        /// <summary>
        /// 移除本地值
        /// </summary>
        public void remove()
        {
            var values = _localValues.Value;
            values.Remove(_id);
        }
    }

    private static ThreadLocal<generator> _currentGenerator = new ThreadLocal<generator>();
    private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
    private bool _isStop = false;
    private bool _isRun = false;
    private List<Action> _callbacks = new List<Action>();

    /// <summary>
    /// 当前生成器实例
    /// </summary>
    public static generator self => _currentGenerator.Value;

    /// <summary>
    /// 启动一个新的协程
    /// </summary>
    /// <param name="func">协程函数</param>
    public static void go(Func<Task> func)
    {
        make(func).run();
    }

    /// <summary>
    /// 启动一个新的协程
    /// </summary>
    /// <param name="strand">线程上下文</param>
    /// <param name="func">协程函数</param>
    public static void go(shared_strand strand, Func<Task> func)
    {
        make(strand, func).run();
    }

    /// <summary>
    /// 启动一个新的协程并返回生成器实例
    /// </summary>
    /// <param name="func">协程函数</param>
    /// <returns>生成器实例</returns>
    public static generator tgo(Func<Task> func)
    {
        var gen = make(func);
        gen.run();
        return gen;
    }

    /// <summary>
    /// 启动一个新的协程并返回生成器实例
    /// </summary>
    /// <param name="strand">线程上下文</param>
    /// <param name="func">协程函数</param>
    /// <returns>生成器实例</returns>
    public static generator tgo(shared_strand strand, Func<Task> func)
    {
        var gen = make(strand, func);
        gen.run();
        return gen;
    }

    /// <summary>
    /// 创建一个新的生成器实例
    /// </summary>
    /// <param name="func">协程函数</param>
    /// <returns>生成器实例</returns>
    public static generator make(Func<Task> func)
    {
        return new generator().init(func);
    }

    /// <summary>
    /// 创建一个新的生成器实例
    /// </summary>
    /// <param name="strand">线程上下文</param>
    /// <param name="func">协程函数</param>
    /// <returns>生成器实例</returns>
    public static generator make(shared_strand strand, Func<Task> func)
    {
        return new generator().init(strand, func);
    }

    /// <summary>
    /// 初始化生成器实例
    /// </summary>
    /// <param name="func">协程函数</param>
    /// <returns>生成器实例</returns>
    private generator init(Func<Task> func)
    {
        Task.Run(async () =>
        {
            _currentGenerator.Value = this;
            try
            {
                await func();
            }
            catch (stop_exception) { }
            catch (Exception ex)
            {
                Console.WriteLine($"generator 内部未捕获的异常: {ex}");
            }
            finally
            {
                _isStop = true;
                _tcs.TrySetResult(true);
                foreach (var callback in _callbacks)
                {
                    try
                    {
                        callback();
                    }
                    catch { }
                }
                _currentGenerator.Value = null;
            }
        });
        return this;
    }

    /// <summary>
    /// 初始化生成器实例
    /// </summary>
    /// <param name="strand">线程上下文</param>
    /// <param name="func">协程函数</param>
    /// <returns>生成器实例</returns>
    private generator init(shared_strand strand, Func<Task> func)
    {
        strand.dispatch(async () =>
        {
            _currentGenerator.Value = this;
            try
            {
                await func();
            }
            catch (stop_exception) { }
            catch (Exception ex)
            {
                Console.WriteLine($"generator 内部未捕获的异常: {ex}");
            }
            finally
            {
                _isStop = true;
                _tcs.TrySetResult(true);
                foreach (var callback in _callbacks)
                {
                    try
                    {
                        callback();
                    }
                    catch { }
                }
                _currentGenerator.Value = null;
            }
        });
        return this;
    }

    /// <summary>
    /// 运行生成器
    /// </summary>
    public void run()
    {
        if (!_isRun)
        {
            _isRun = true;
        }
    }

    /// <summary>
    /// 停止生成器
    /// </summary>
    public void stop()
    {
        if (!_isStop)
        {
            _isStop = true;
            _tcs.TrySetResult(true);
            throw new stop_exception();
        }
    }

    /// <summary>
    /// 异步等待生成器完成
    /// </summary>
    /// <returns>任务</returns>
    public Task async_wait()
    {
        return _tcs.Task;
    }

    /// <summary>
    /// 同步等待生成器完成
    /// </summary>
    public void sync_wait()
    {
        _tcs.Task.Wait();
    }

    /// <summary>
    /// 睡眠指定时间
    /// </summary>
    /// <param name="ms">毫秒数</param>
    /// <returns>任务</returns>
    public static async Task sleep(int ms)
    {
        await Task.Delay(ms);
    }

    /// <summary>
    /// 发送任务到线程池执行
    /// </summary>
    /// <param name="action">任务函数</param>
    /// <returns>任务</returns>
    public static async Task send_task(Action action)
    {
        await Task.Run(action);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="msg">消息</param>
    /// <returns>任务</returns>
    public async Task send_msg<T>(T msg)
    {
        // 简化实现，实际应根据具体需求实现
        await Task.CompletedTask;
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <returns>接收构建器</returns>
    public static receive_builder receive()
    {
        return new receive_builder();
    }

    /// <summary>
    /// 选择操作
    /// </summary>
    /// <param name="sendFirst">是否优先发送</param>
    /// <returns>选择构建器</returns>
    public static select_builder select(bool sendFirst = false)
    {
        return new select_builder(sendFirst);
    }

    /// <summary>
    /// CSP等待操作
    /// </summary>
    /// <typeparam name="T1">输入类型</typeparam>
    /// <typeparam name="T2">输出类型</typeparam>
    /// <param name="csp">CSP通道</param>
    /// <param name="handler">处理函数</param>
    /// <returns>通道状态</returns>
    public static async Task<chan_state> csp_wait<T1, T2>(csp_chan<T1, T2> csp, Func<T1, Task<T2>> handler)
    {
        // 简化实现，实际应根据具体需求实现
        return chan_state.ok;
    }

    /// <summary>
    /// 接收构建器类
    /// </summary>
    public class receive_builder
    {
        private List<object> _cases = new List<object>();

        /// <summary>
        /// 添加一个接收 case
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="handler">处理函数</param>
        /// <returns>接收构建器</returns>
        public receive_builder case_of<T>(Func<T, Task> handler)
        {
            _cases.Add((typeof(T), handler));
            return this;
        }

        /// <summary>
        /// 结束构建
        /// </summary>
        /// <returns>任务</returns>
        public async Task end()
        {
            // 简化实现，实际应根据具体需求实现
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// 选择构建器类
    /// </summary>
    public class select_builder
    {
        private bool _sendFirst;
        private List<object> _cases = new List<object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sendFirst">是否优先发送</param>
        public select_builder(bool sendFirst)
        {
            _sendFirst = sendFirst;
        }

        /// <summary>
        /// 添加一个接收 case
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="chan">通道</param>
        /// <param name="handler">处理函数</param>
        /// <returns>选择构建器</returns>
        public select_builder case_receive<T>(chan<T> chan, Func<T, Task> handler)
        {
            _cases.Add(("receive", chan, handler));
            return this;
        }

        /// <summary>
        /// 添加一个发送 case
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="chan">通道</param>
        /// <param name="msg">消息</param>
        /// <param name="handler">处理函数</param>
        /// <returns>选择构建器</returns>
        public select_builder case_send<T>(chan<T> chan, T msg, Func<Task> handler)
        {
            _cases.Add(("send", chan, msg, handler));
            return this;
        }

        /// <summary>
        /// 结束构建
        /// </summary>
        /// <returns>任务</returns>
        public async Task end()
        {
            // 简化实现，实际应根据具体需求实现
            await Task.CompletedTask;
        }

        /// <summary>
        /// 循环执行选择操作
        /// </summary>
        /// <returns>任务</returns>
        public async Task loop()
        {
            // 简化实现，实际应根据具体需求实现
            await Task.CompletedTask;
        }
    }
}

/// <summary>
/// 迭代器协程扩展类
/// </summary>
public static class generator_extensions
{
    /// <summary>
    /// 异步迭代器，将IEnumerable转换为IAsyncEnumerable
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源IEnumerable</param>
    /// <returns>异步迭代器</returns>
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            await Task.Yield();
            yield return item;
        }
    }

    /// <summary>
    /// 异步处理迭代器中的每个元素
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="source">源IAsyncEnumerable</param>
    /// <param name="action">处理函数</param>
    /// <returns>任务</returns>
    public static async Task ForEachAsync<T>(this IAsyncEnumerable<T> source, Func<T, Task> action)
    {
        await foreach (var item in source)
        {
            await action(item);
        }
    }
}

/// <summary>
/// 通道扩展类
/// </summary>
public static class channel_extensions
{
    /// <summary>
    /// 异步发送消息到通道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="channel">通道</param>
    /// <param name="message">消息</param>
    /// <returns>任务</returns>
    public static async Task post<T>(this Channel<T> channel, T message)
    {
        await channel.Writer.WriteAsync(message);
    }

    /// <summary>
    /// 尝试发送消息到通道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="channel">通道</param>
    /// <param name="message">消息</param>
    /// <returns>是否成功</returns>
    public static bool try_post<T>(this Channel<T> channel, T message)
    {
        return channel.Writer.TryWrite(message);
    }

    /// <summary>
    /// 异步从通道接收消息
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="channel">通道</param>
    /// <returns>消息</returns>
    public static async Task<T> recv<T>(this Channel<T> channel)
    {
        return await channel.Reader.ReadAsync();
    }

    /// <summary>
    /// 尝试从通道接收消息
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="channel">通道</param>
    /// <param name="message">消息</param>
    /// <returns>是否成功</returns>
    public static bool try_recv<T>(this Channel<T> channel, out T message)
    {
        return channel.Reader.TryRead(out message);
    }
}

/// <summary>
/// 异步结果包装类，用于存储异步操作的结果
/// </summary>
/// <typeparam name="T">结果类型</typeparam>
public class async_result_wrap<T>
{
    /// <summary>
    /// 存储的结果值
    /// </summary>
    public T value;

    /// <summary>
    /// 清除结果值，设置为默认值
    /// </summary>
    public void clear()
    {
        value = default;
    }
}

/// <summary>
/// 异步结果包装类，用于存储两个异步操作的结果
/// </summary>
/// <typeparam name="T1">第一个结果类型</typeparam>
/// <typeparam name="T2">第二个结果类型</typeparam>
public class async_result_wrap<T1, T2>
{
    /// <summary>
    /// 第一个存储的结果值
    /// </summary>
    public T1 value1;
    /// <summary>
    /// 第二个存储的结果值
    /// </summary>
    public T2 value2;

    /// <summary>
    /// 清除结果值，设置为默认值
    /// </summary>
    public void clear()
    {
        value1 = default;
        value2 = default;
    }
}

/// <summary>
/// 异步结果包装类，用于存储三个异步操作的结果
/// </summary>
/// <typeparam name="T1">第一个结果类型</typeparam>
/// <typeparam name="T2">第二个结果类型</typeparam>
/// <typeparam name="T3">第三个结果类型</typeparam>
public class async_result_wrap<T1, T2, T3>
{
    /// <summary>
    /// 第一个存储的结果值
    /// </summary>
    public T1 value1;
    /// <summary>
    /// 第二个存储的结果值
    /// </summary>
    public T2 value2;
    /// <summary>
    /// 第三个存储的结果值
    /// </summary>
    public T3 value3;

    /// <summary>
    /// 清除结果值，设置为默认值
    /// </summary>
    public void clear()
    {
        value1 = default;
        value2 = default;
        value3 = default;
    }
}

/// <summary>
/// 通道异常类
/// </summary>
public class chan_exception : Exception
{
    /// <summary>
    /// 通道状态
    /// </summary>
    public readonly chan_state state;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="st">通道状态</param>
    public chan_exception(chan_state st)
    {
        state = st;
    }
}

/// <summary>
/// CSP失败异常类
/// </summary>
public class csp_fail_exception : Exception
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public csp_fail_exception() { }
}

/// <summary>
/// 协程工具类
/// </summary>
public static class coroutine_utils
{
    /// <summary>
    /// 创建一个无界通道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <returns>通道</returns>
    public static Channel<T> make_chan<T>()
    {
        return Channel.CreateUnbounded<T>();
    }

    /// <summary>
    /// 创建一个有界通道
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="capacity">容量</param>
    /// <returns>通道</returns>
    public static Channel<T> make_chan<T>(int capacity)
    {
        return Channel.CreateBounded<T>(capacity);
    }

    /// <summary>
    /// 异步等待指定时间
    /// </summary>
    /// <param name="milliseconds">毫秒数</param>
    /// <returns>任务</returns>
    public static async Task delay(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

    /// <summary>
    /// 异步等待指定时间
    /// </summary>
    /// <param name="timeSpan">时间间隔</param>
    /// <returns>任务</returns>
    public static async Task delay(TimeSpan timeSpan)
    {
        await Task.Delay(timeSpan);
    }

    /// <summary>
    /// 并行执行多个任务
    /// </summary>
    /// <param name="tasks">任务数组</param>
    /// <returns>任务</returns>
    public static async Task parallel(params Func<Task>[] tasks)
    {
        var taskList = tasks.Select(task => task()).ToArray();
        await Task.WhenAll(taskList);
    }

    /// <summary>
    /// 并行执行多个带返回值的任务
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="tasks">任务数组</param>
    /// <returns>返回值数组</returns>
    public static async Task<T[]> parallel<T>(params Func<Task<T>>[] tasks)
    {
        var taskList = tasks.Select(task => task()).ToArray();
        return await Task.WhenAll(taskList);
    }
}
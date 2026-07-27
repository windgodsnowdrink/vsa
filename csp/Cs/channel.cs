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
using System.Threading.Tasks;
using System.Threading.Channels;

namespace System.Threading.Tasks.Cs;

// CSP（Communicating Sequential Processes）模型的通道
// - unlimit_chan ：无限制容量的通道
// - limit_chan ：限制容量的通道
// - nil_chan ：空通道
// - broadcast_chan ：广播通道
// - csp_chan ：CSP通道
// - System.Threading.Channels提供Channel.CreateUnbounded<T>()方法创建无限制容量的通道;Channel.CreateBounded<T>(len)方法创建限制容量的通道;ChannelReader <t> 和 ChannelWriter <t> ：提供了读取和写入通道的接口

/// <summary>
/// 通道状态
/// </summary>
public enum chan_state
{
    /// <summary>
    /// 未定义
    /// </summary>
    undefined,
    /// <summary>
    /// 正确
    /// </summary>
    ok,
    /// <summary>
    /// 失败
    /// </summary>
    fail,
    /// <summary>
    /// CSP失败
    /// </summary>
    csp_fail,
    /// <summary>
    /// 取消
    /// </summary>
    cancel,
    /// <summary>
    /// 关闭
    /// </summary>
    closed,
    /// <summary>
    /// 超时
    /// </summary>
    overtime
}

/// <summary>
/// 通道类型
/// </summary>
public enum chan_type
{
    /// <summary>
    /// 未定义
    /// </summary>
    undefined,
    /// <summary>
    /// 广播
    /// </summary>
    broadcast,
    /// <summary>
    /// 未限制
    /// </summary>
    unlimit,
    /// <summary>
    /// 限制
    /// </summary>
    limit,
    /// <summary>
    /// 空
    /// </summary>
    nil,
    /// <summary>
    /// CSP通道
    /// </summary>
    csp
}

/// <summary>
/// 通道发送包装结构
/// </summary>
public struct chan_send_wrap
{
    /// <summary>
    /// 通道操作状态
    /// </summary>
    public chan_state state;

    /// <summary>
    /// 隐式转换为通道状态
    /// </summary>
    /// <param name="rval">通道发送包装实例</param>
    /// <returns>通道操作状态</returns>
    public static implicit operator chan_state(chan_send_wrap rval)
    {
        return rval.state;
    }

    /// <summary>
    /// 默认值，状态为undefined
    /// </summary>
    public static chan_send_wrap def
    {
        get
        {
            return new chan_send_wrap { state = chan_state.undefined };
        }
    }

    /// <summary>
    /// 检查通道状态是否为ok
    /// </summary>
    public void check()
    {
        if (chan_state.ok != state)
        {
            throw new chan_exception(state);
        }
    }
}

/// <summary>
/// 通道接收包装结构
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public struct chan_recv_wrap<T>
{
    /// <summary>
    /// 通道操作状态
    /// </summary>
    public chan_state state;
    /// <summary>
    /// 接收到的消息
    /// </summary>
    public T msg;

    /// <summary>
    /// 隐式转换为消息类型
    /// </summary>
    /// <param name="rval">通道接收包装实例</param>
    /// <returns>接收到的消息</returns>
    public static implicit operator T(chan_recv_wrap<T> rval)
    {
        if (chan_state.ok != rval.state)
        {
            throw new chan_exception(rval.state);
        }
        return rval.msg;
    }

    /// <summary>
    /// 隐式转换为通道状态
    /// </summary>
    /// <param name="rval">通道接收包装实例</param>
    /// <returns>通道操作状态</returns>
    public static implicit operator chan_state(chan_recv_wrap<T> rval)
    {
        return rval.state;
    }

    /// <summary>
    /// 默认值，状态为undefined
    /// </summary>
    public static chan_recv_wrap<T> def
    {
        get
        {
            return new chan_recv_wrap<T> { state = chan_state.undefined };
        }
    }

    /// <summary>
    /// 检查通道状态是否为ok
    /// </summary>
    public void check()
    {
        if (chan_state.ok != state)
        {
            throw new chan_exception(state);
        }
    }
}

/// <summary>
/// 通道丢失消息
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class chan_lost_msg<T>
{
    private T _msg;
    private bool _lost;

    /// <summary>
    /// 设置丢失消息
    /// </summary>
    /// <param name="msg">消息</param>
    public void set(T msg)
    {
        _msg = msg;
        _lost = true;
    }

    /// <summary>
    /// 获取丢失消息
    /// </summary>
    /// <returns>消息</returns>
    public T get()
    {
        _lost = false;
        return _msg;
    }

    /// <summary>
    /// 是否丢失消息
    /// </summary>
    /// <returns>是否丢失</returns>
    public bool lost()
    {
        return _lost;
    }
}

/// <summary>
/// 广播令牌
/// </summary>
public class broadcast_token
{
    /// <summary>
    /// 默认令牌
    /// </summary>
    public static broadcast_token _defToken = new broadcast_token();
}

/// <summary>
/// 空类型
/// </summary>
public struct void_type { }
/// <summary>
/// 空动作
/// </summary>
public static class nil_action { static public readonly Action action = () => { }; }
/// <summary>
/// 空动作
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>  
public static class nil_action<T1> { static public readonly Action<T1> action = (T1 p1) => { }; }
/// <summary>
/// 空动作
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>  
public static class nil_action<T1, T2> { static public readonly Action<T1, T2> action = (T1 p1, T2 p2) => { }; }
/// <summary>
/// 空动作
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>
/// <typeparam name="T3">元组类型3</typeparam>      
public static class nil_action<T1, T2, T3> { static public readonly Action<T1, T2, T3> action = (T1 p1, T2 p2, T3 p3) => { }; }
/// <summary>
/// 空动作
/// </summary>
public static class nil_func { static public readonly Func<Task> func = () => Task.CompletedTask; }
/// <summary>
/// 空动作
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>      
public static class nil_func<T1> { static public readonly Func<T1, Task> func = (T1 p1) => Task.CompletedTask; }
/// <summary>
/// 空动作
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>      
public static class nil_func<T1, T2> { static public readonly Func<T1, T2, Task> func = (T1 p1, T2 p2) => Task.CompletedTask; }
/// <summary>
/// 空动作
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>
/// <typeparam name="T3">元组类型3</typeparam>          
public static class nil_func<T1, T2, T3> { static public readonly Func<T1, T2, T3, Task> func = (T1 p1, T2 p2, T3 p3) => Task.CompletedTask; }

/// <summary>
/// 通道基类
/// </summary>
public abstract class chan_base
{
    /// <summary>
    /// 通道类型
    /// </summary>
    public abstract chan_type type();
    protected static bool _closed;
    protected chan_base() { _closed = false; }
    public abstract void close(bool isClear = false);
    public bool is_closed() { return _closed; }
}

/// <summary>
/// 通道基类
/// </summary>
/// <typeparam name="T">通道类型</typeparam>
public abstract class chan<T> : chan_base
{
    protected Channel<T> _channel;
    protected ChannelReader<T> _reader;
    protected ChannelWriter<T> _writer;
    private bool _closed = false;

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    /// <param name="lostMsg">丢失消息</param>
    /// <returns>发送结果</returns>
    public abstract ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null);

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="lostMsg">丢失消息</param>
    /// <returns>接收结果</returns>
    public abstract ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null);

    /// <summary>
    /// 尝试发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    /// <param name="lostMsg">丢失消息</param>
    /// <returns>发送结果</returns>
    public abstract ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null);

    /// <summary>
    /// 尝试接收消息
    /// </summary>
    /// <param name="lostMsg">丢失消息</param>
    /// <returns>接收结果</returns>
    public abstract ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null);

    /// <summary>
    /// 定时发送消息
    /// </summary>
    /// <param name="ms">毫秒</param>
    /// <param name="msg">消息</param>
    /// <param name="lostMsg">丢失消息</param>
    /// <returns>发送结果</returns>
    public abstract ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null);

    /// <summary>
    /// 定时接收消息
    /// </summary>
    /// <param name="ms">毫秒</param>
    /// <param name="lostMsg">丢失消息</param>
    /// <returns>接收结果</returns>
    public abstract ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null);

    /// <summary>
    /// 创建通道
    /// </summary>
    /// <param name="len">通道长度</param>
    /// <returns>通道</returns>
    static public chan<T> make(int len)
    {
        if (0 == len)
        {
            return new nil_chan<T>();
        }
        else if (0 < len)
        {
            return new limit_chan<T>(len);
        }
        return new unlimit_chan<T>();
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    public void post(T msg)
    {
        _writer.TryWrite(msg);
    }

    /// <summary>
    /// 尝试发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    public void try_post(T msg)
    {
        _writer.TryWrite(msg);
    }

    /// <summary>
    /// 定时发送消息
    /// </summary>
    /// <param name="ms">毫秒</param>
    /// <param name="msg">消息</param>
    public async void timed_post(int ms, T msg)
    {
        await Task.Delay(ms);
        await _writer.WriteAsync(msg);
    }
}

/// <summary>
/// 无限制通道
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class unlimit_chan<T> : chan<T>
{
    public unlimit_chan()
    {
        _channel = Channel.CreateUnbounded<T>();
        _reader = _channel.Reader;
        _writer = _channel.Writer;
    }

    public override chan_type type()
    {
        return chan_type.unlimit;
    }

    public override void close(bool isClear = false)
    {
        _writer.Complete();
        _closed = true;
    }

    public override async ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            await _writer.WriteAsync(msg);
            return new chan_send_wrap { state = chan_state.ok };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            msg = await _reader.ReadAsync();
            return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }

    public override ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.closed });
        }

        if (_writer.TryWrite(msg))
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.ok });
        }
        else
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.fail });
        }
    }

    public override ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.closed });
        }

        if (_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.ok, msg = msg });
        }
        else
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.fail });
        }
    }

    public override async ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                await _writer.WriteAsync(msg, cts.Token);
                return new chan_send_wrap { state = chan_state.ok };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_send_wrap { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                msg = await _reader.ReadAsync(cts.Token);
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_recv_wrap<T> { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }
}

/// <summary>
/// 限制通道
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class limit_chan<T> : chan<T>
{
    public limit_chan(int capacity)
    {
        _channel = Channel.CreateBounded<T>(capacity);
        _reader = _channel.Reader;
        _writer = _channel.Writer;
    }

    public override chan_type type()
    {
        return chan_type.limit;
    }

    public override void close(bool isClear = false)
    {
        _writer.Complete();
        _closed = true;
    }

    public override async ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            await _writer.WriteAsync(msg);
            return new chan_send_wrap { state = chan_state.ok };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            msg = await _reader.ReadAsync();
            return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }

    public override ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.closed });
        }

        if (_writer.TryWrite(msg))
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.ok });
        }
        else
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.fail });
        }
    }

    public override ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.closed });
        }

        if (_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.ok, msg = msg });
        }
        else
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.fail });
        }
    }

    public override async ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                await _writer.WriteAsync(msg, cts.Token);
                return new chan_send_wrap { state = chan_state.ok };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_send_wrap { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                msg = await _reader.ReadAsync(cts.Token);
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_recv_wrap<T> { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }
}

/// <summary>
/// 空通道
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class nil_chan<T> : chan<T>
{
    public nil_chan()
    {
        _channel = Channel.CreateBounded<T>(0);
        _reader = _channel.Reader;
        _writer = _channel.Writer;
    }

    public override chan_type type()
    {
        return chan_type.nil;
    }

    public override void close(bool isClear = false)
    {
        _writer.Complete();
        _closed = true;
    }

    public override async ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(1))
            {
                await _writer.WriteAsync(msg, cts.Token);
                return new chan_send_wrap { state = chan_state.ok };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(1))
            {
                var msg = await _reader.ReadAsync(cts.Token);
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }

    public override ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.closed });
        }

        return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.fail });
    }

    public override ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.closed });
        }

        return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.fail });
    }

    public override async ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                await _writer.WriteAsync(msg, cts.Token);
                return new chan_send_wrap { state = chan_state.ok };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_send_wrap { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                var msg = await _reader.ReadAsync(cts.Token);
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_recv_wrap<T> { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }
}

/// <summary>
/// 广播通道
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class broadcast_chan<T> : chan<T>
{
    private List<Channel<T>> _channels;
    private object _lock;

    public broadcast_chan()
    {
        _channels = new List<Channel<T>>();
        _lock = new object();
        // 创建一个默认通道用于兼容性
        _channel = Channel.CreateUnbounded<T>();
        _reader = _channel.Reader;
        _writer = _channel.Writer;
    }

    public override chan_type type()
    {
        return chan_type.broadcast;
    }

    public override void close(bool isClear = false)
    {
        lock (_lock)
        {
            foreach (var channel in _channels)
            {
                channel.Writer.Complete();
            }
            _writer.Complete();
            _closed = true;
        }
    }

    /// <summary>
    /// 订阅通道
    /// </summary>
    /// <returns>订阅通道</returns>
    public chan<T> subscribe()
    {
        var channel = Channel.CreateUnbounded<T>();
        lock (_lock)
        {
            _channels.Add(channel);
        }
        return new broadcast_subscriber<T>(this, channel);
    }

    public override async ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            // 发送到所有订阅通道
            lock (_lock)
            {
                foreach (var channel in _channels)
                {
                    channel.Writer.TryWrite(msg);
                }
            }
            // 发送到默认通道
            await _writer.WriteAsync(msg);
            return new chan_send_wrap { state = chan_state.ok };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            msg = await _reader.ReadAsync();
            return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }

    public override ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.closed });
        }

        try
        {
            // 发送到所有订阅通道
            lock (_lock)
            {
                foreach (var channel in _channels)
                {
                    channel.Writer.TryWrite(msg);
                }
            }
            // 发送到默认通道
            if (_writer.TryWrite(msg))
            {
                return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.ok });
            }
            else
            {
                return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.fail });
            }
        }
        catch (Exception)
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.fail });
        }
    }

    public override ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.closed });
        }

        if (_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.ok, msg = msg });
        }
        else
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.fail });
        }
    }

    public override async ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            // 发送到所有订阅通道
            lock (_lock)
            {
                foreach (var channel in _channels)
                {
                    channel.Writer.TryWrite(msg);
                }
            }
            // 发送到默认通道
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                await _writer.WriteAsync(msg, cts.Token);
                return new chan_send_wrap { state = chan_state.ok };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_send_wrap { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                msg = await _reader.ReadAsync(cts.Token);
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_recv_wrap<T> { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.fail };
        }
    }

    /// <summary>
    /// 广播订阅者
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    private class broadcast_subscriber<T> : chan<T>
    {
        private broadcast_chan<T> _broadcastChan;
        private Channel<T> _channel;

        public broadcast_subscriber(broadcast_chan<T> broadcastChan, Channel<T> channel)
        {
            _broadcastChan = broadcastChan;
            _channel = channel;
            _reader = _channel.Reader;
            _writer = _channel.Writer;
        }

        public override chan_type type()
        {
            return chan_type.broadcast;
        }

        public override void close(bool isClear = false)
        {
            _writer.Complete();
            _closed = true;
        }

        public override async ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null)
        {
            // 订阅者不能发送消息，只能接收
            return new chan_send_wrap { state = chan_state.fail };
        }

        public override async ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null)
        {
            var msg = default(T);
            if (_closed && !_reader.TryRead(out msg))
            {
                return new chan_recv_wrap<T> { state = chan_state.closed };
            }

            try
            {
                msg = await _reader.ReadAsync();
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
            catch (ChannelClosedException)
            {
                _closed = true;
                return new chan_recv_wrap<T> { state = chan_state.closed };
            }
            catch (Exception)
            {
                return new chan_recv_wrap<T> { state = chan_state.fail };
            }
        }

        public override ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null)
        {
            // 订阅者不能发送消息，只能接收
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.fail });
        }

        public override ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null)
        {
            var msg = default(T);
            if (_closed && !_reader.TryRead(out msg))
            {
                return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.closed });
            }

            if (_reader.TryRead(out msg))
            {
                return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.ok, msg = msg });
            }
            else
            {
                return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.fail });
            }
        }

        public override async ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null)
        {
            // 订阅者不能发送消息，只能接收
            return new chan_send_wrap { state = chan_state.fail };
        }

        public override async ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null)
        {
            var msg = default(T);
            if (_closed && !_reader.TryRead(out msg))
            {
                return new chan_recv_wrap<T> { state = chan_state.closed };
            }

            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
                {
                    msg = await _reader.ReadAsync(cts.Token);
                    return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
                }
            }
            catch (OperationCanceledException)
            {
                return new chan_recv_wrap<T> { state = chan_state.overtime };
            }
            catch (ChannelClosedException)
            {
                _closed = true;
                return new chan_recv_wrap<T> { state = chan_state.closed };
            }
            catch (Exception)
            {
                return new chan_recv_wrap<T> { state = chan_state.fail };
            }
        }
    }
}

/// <summary>
/// CSP通道
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class csp_chan<T> : chan<T>
{
    public csp_chan()
    {
        _channel = Channel.CreateUnbounded<T>();
        _reader = _channel.Reader;
        _writer = _channel.Writer;
    }

    public override chan_type type()
    {
        return chan_type.csp;
    }

    public override void close(bool isClear = false)
    {
        _writer.Complete();
        _closed = true;
    }

    public override async ValueTask<chan_send_wrap> send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            await _writer.WriteAsync(msg);
            return new chan_send_wrap { state = chan_state.ok };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.csp_fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            msg = await _reader.ReadAsync();
            return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.csp_fail };
        }
    }

    public override ValueTask<chan_send_wrap> try_send(T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.closed });
        }

        if (_writer.TryWrite(msg))
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.ok });
        }
        else
        {
            return new ValueTask<chan_send_wrap>(new chan_send_wrap { state = chan_state.csp_fail });
        }
    }

    public override ValueTask<chan_recv_wrap<T>> try_receive(chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.closed });
        }

        if (_reader.TryRead(out msg))
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.ok, msg = msg });
        }
        else
        {
            return new ValueTask<chan_recv_wrap<T>>(new chan_recv_wrap<T> { state = chan_state.csp_fail });
        }
    }

    public override async ValueTask<chan_send_wrap> timed_send(int ms, T msg, chan_lost_msg<T> lostMsg = null)
    {
        if (_closed)
        {
            return new chan_send_wrap { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                await _writer.WriteAsync(msg, cts.Token);
                return new chan_send_wrap { state = chan_state.ok };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_send_wrap { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_send_wrap { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_send_wrap { state = chan_state.csp_fail };
        }
    }

    public override async ValueTask<chan_recv_wrap<T>> timed_receive(int ms, chan_lost_msg<T> lostMsg = null)
    {
        var msg = default(T);
        if (_closed && !_reader.TryRead(out msg))
        {
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(ms)))
            {
                msg = await _reader.ReadAsync(cts.Token);
                return new chan_recv_wrap<T> { state = chan_state.ok, msg = msg };
            }
        }
        catch (OperationCanceledException)
        {
            return new chan_recv_wrap<T> { state = chan_state.overtime };
        }
        catch (ChannelClosedException)
        {
            _closed = true;
            return new chan_recv_wrap<T> { state = chan_state.closed };
        }
        catch (Exception)
        {
            return new chan_recv_wrap<T> { state = chan_state.csp_fail };
        }
    }
}

/// <summary>
/// CSP通道类
/// </summary>
/// <typeparam name="T1">输入类型</typeparam>
/// <typeparam name="T2">输出类型</typeparam>
public class csp_chan<T1, T2>
{
    private Channel<(T1, TaskCompletionSource<T2>)> _channel = Channel.CreateUnbounded<(T1, TaskCompletionSource<T2>)>();
    
    private bool _closed = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    public csp_chan() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="strand">线程上下文</param>
    public csp_chan(shared_strand strand) { }

    /// <summary>
    /// 调用CSP
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns>调用包装</returns>
    public async Task<csp_invoke_wrap<T2>> invoke(T1 msg)
    {
        if (_closed)
        {
            return new csp_invoke_wrap<T2> { state = chan_state.closed };
        }

        var tcs = new TaskCompletionSource<T2>();
        await _channel.Writer.WriteAsync((msg, tcs));
        var result = await tcs.Task;
        return new csp_invoke_wrap<T2> { state = chan_state.ok, result = result };
    }

    /// <summary>
    /// 关闭通道
    /// </summary>
    public void close()
    {
        if (!_closed)
        {
            _closed = true;
            _channel.Writer.Complete();
        }
    }
}

/// <summary>
/// CSP调用包装结构
/// </summary>
/// <typeparam name="T">结果类型</typeparam>
public struct csp_invoke_wrap<T>
{
    /// <summary>
    /// 通道状态
    /// </summary>
    public chan_state state;

    /// <summary>
    /// 结果
    /// </summary>
    public T result;

    /// <summary>
    /// 默认值
    /// </summary>
    public static csp_invoke_wrap<T> def => new csp_invoke_wrap<T> { state = chan_state.undefined };
}


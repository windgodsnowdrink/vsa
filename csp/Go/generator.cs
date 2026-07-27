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
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace System.Threading.Tasks.Go;

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
/// 异步结果包装类，用于存储单个异步操作的结果
/// </summary>
/// <typeparam name="T1">结果类型</typeparam>
public class async_result_wrap<T1>
{
    /// <summary>
    /// 存储的结果值
    /// </summary>
    public T1 value1;

    /// <summary>
    /// 清除结果值，设置为默认值
    /// </summary>
    public void clear()
    {
        value1 = default(T1);
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
        value1 = default(T1);
        value2 = default(T2);
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
        value1 = default(T1);
        value2 = default(T2);
        value3 = default(T3);
    }
}

/// <summary>
/// 异步结果忽略包装类，提供一个静态的异步结果包装实例
/// 用于不需要关心结果值的场景
/// </summary>
/// <typeparam name="T1">结果类型</typeparam>
public static class async_result_ignore_wrap<T1>
{
    /// <summary>
    /// 静态的异步结果包装实例
    /// </summary>
    static readonly public async_result_wrap<T1> value = new async_result_wrap<T1>();
}

/// <summary>
/// 异步结果忽略包装类，提供一个静态的异步结果包装实例
/// 用于不需要关心结果值的场景
/// </summary>
/// <typeparam name="T1">第一个结果类型</typeparam>
/// <typeparam name="T2">第二个结果类型</typeparam>
public static class async_result_ignore_wrap<T1, T2>
{
    /// <summary>
    /// 静态的异步结果包装实例
    /// </summary>
    static readonly public async_result_wrap<T1, T2> value = new async_result_wrap<T1, T2>();
}

/// <summary>
/// 异步结果忽略包装类，提供一个静态的异步结果包装实例
/// 用于不需要关心结果值的场景
/// </summary>
/// <typeparam name="T1">第一个结果类型</typeparam>
/// <typeparam name="T2">第二个结果类型</typeparam>
/// <typeparam name="T3">第三个结果类型</typeparam>
public static class async_result_ignore_wrap<T1, T2, T3>
{
    /// <summary>
    /// 静态的异步结果包装实例
    /// </summary>
    static readonly public async_result_wrap<T1, T2, T3> value = new async_result_wrap<T1, T2, T3>();
}

/// <summary>
/// 通道丢失消息类，用于存储通道操作中可能丢失的消息
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class chan_lost_msg<T>
{
    /// <summary>
    /// 存储的消息
    /// </summary>
    T _msg;
    /// <summary>
    /// 标记是否有消息
    /// </summary>
    bool _has = false;

    /// <summary>
    /// 设置消息
    /// </summary>
    /// <param name="m">消息值</param>
    virtual internal void set(T m)
    {
        _has = true;
        _msg = m;
    }

    /// <summary>
    /// 清除消息
    /// </summary>
    public void clear()
    {
        _has = false;
        _msg = default(T);
    }

    /// <summary>
    /// 获取是否有消息
    /// </summary>
    public bool has
    {
        get
        {
            return _has;
        }
    }

    /// <summary>
    /// 获取存储的消息
    /// </summary>
    public T msg
    {
        get
        {
            return _msg;
        }
    }
}

/// <summary>
/// 双通道丢失消息类，继承自chan_lost_msg<tuple<T1, T2>>
/// </summary>
/// <typeparam name="T1">第一个消息类型</typeparam>
/// <typeparam name="T2">第二个消息类型</typeparam>
public class chan_lost_msg<T1, T2> : chan_lost_msg<tuple<T1, T2>> { }

/// <summary>
/// 三通道丢失消息类，继承自chan_lost_msg<tuple<T1, T2, T3>>
/// </summary>
/// <typeparam name="T1">第一个消息类型</typeparam>
/// <typeparam name="T2">第二个消息类型</typeparam>
/// <typeparam name="T3">第三个消息类型</typeparam>
public class chan_lost_msg<T1, T2, T3> : chan_lost_msg<tuple<T1, T2, T3>> { }

/// <summary>
/// 通道丢失消息容器类，继承自chan_lost_msg<T>
/// 当消息丢失时，会将消息发送到指定的通道
/// </summary>
/// <typeparam name="T">消息类型</typeparam>
public class chan_lost_docker<T> : chan_lost_msg<T>
{
    /// <summary>
    /// 存储消息的通道
    /// </summary>
    readonly chan<T> _docker;
    /// <summary>
    /// 标记是否使用try_post方法发送消息
    /// </summary>
    readonly bool _tryDocker;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="docker">存储消息的通道</param>
    /// <param name="tryDocker">是否使用try_post方法发送消息</param>
    public chan_lost_docker(chan<T> docker, bool tryDocker = true)
    {
        _docker = docker;
        _tryDocker = tryDocker;
    }

    /// <summary>
    /// 设置消息并重定向到存储通道
    /// </summary>
    /// <param name="m">消息值</param>
    override internal void set(T m)
    {
        base.set(m);
        if (_tryDocker)
        {
            _docker.try_post(m);
        }
        else
        {
            _docker.post(m);
        }
    }

    /// <summary>
    /// 获取存储消息的通道
    /// </summary>
    public chan<T> docker
    {
        get
        {
            return _docker;
        }
    }
}

/// <summary>
/// 双通道丢失消息容器类，继承自chan_lost_docker<tuple<T1, T2>>
/// </summary>
/// <typeparam name="T1">第一个消息类型</typeparam>
/// <typeparam name="T2">第二个消息类型</typeparam>
public class chan_lost_docker<T1, T2> : chan_lost_docker<tuple<T1, T2>>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="docker">存储消息的通道</param>
    /// <param name="tryDocker">是否使用try_post方法发送消息</param>
    public chan_lost_docker(chan<tuple<T1, T2>> docker, bool tryDocker = true) : base(docker, tryDocker) { }
}

/// <summary>
/// 三通道丢失消息容器类，继承自chan_lost_docker<tuple<T1, T2, T3>>
/// </summary>
/// <typeparam name="T1">第一个消息类型</typeparam>
/// <typeparam name="T2">第二个消息类型</typeparam>
/// <typeparam name="T3">第三个消息类型</typeparam>
public class chan_lost_docker<T1, T2, T3> : chan_lost_docker<tuple<T1, T2, T3>>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="docker">存储消息的通道</param>
    /// <param name="tryDocker">是否使用try_post方法发送消息</param>
    public chan_lost_docker(chan<tuple<T1, T2, T3>> docker, bool tryDocker = true) : base(docker, tryDocker) { }
}

/// <summary>
/// 通道异常类，继承自System.Exception
/// 用于表示通道操作过程中发生的异常
/// </summary>
public class chan_exception : System.Exception
{
    /// <summary>
    /// 通道状态
    /// </summary>
    public readonly chan_state state;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="st">通道状态</param>
    internal chan_exception(chan_state st)
    {
        state = st;
    }
}

/// <summary>
/// CSP失败异常类，继承自System.Exception
/// 用于表示CSP操作失败时的异常
/// </summary>
public class csp_fail_exception : System.Exception
{
    /// <summary>
    /// 构造函数
    /// </summary>
    internal csp_fail_exception() { }
}

/// <summary>
/// 通道接收包装结构，用于存储通道接收操作的结果
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
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
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
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
    public void check()
    {
        if (chan_state.ok != state)
        {
            throw new chan_exception(state);
        }
    }

    /// <summary>
    /// 重写ToString方法，返回通道接收包装的字符串表示
    /// </summary>
    /// <returns>字符串表示</returns>
    public override string ToString()
    {
        return chan_state.ok == state ?
            string.Format("chan_recv_wrap<{0}>.msg={1}", typeof(T).Name, msg) :
            string.Format("chan_recv_wrap<{0}>.state={1}", typeof(T).Name, state);
    }
}

/// <summary>
/// 通道发送包装结构，用于存储通道发送操作的结果
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
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
    public void check()
    {
        if (chan_state.ok != state)
        {
            throw new chan_exception(state);
        }
    }

    /// <summary>
    /// 重写ToString方法，返回通道状态的字符串表示
    /// </summary>
    /// <returns>通道状态的字符串表示</returns>
    public override string ToString()
    {
        return state.ToString();
    }
}

/// <summary>
/// CSP调用包装结构，用于存储CSP调用操作的结果
/// </summary>
/// <typeparam name="T">结果类型</typeparam>
public struct csp_invoke_wrap<T>
{
    /// <summary>
    /// 通道操作状态
    /// </summary>
    public chan_state state;
    /// <summary>
    /// CSP调用的结果
    /// </summary>
    public T result;

    /// <summary>
    /// 隐式转换为结果类型
    /// </summary>
    /// <param name="rval">CSP调用包装实例</param>
    /// <returns>CSP调用的结果</returns>
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
    public static implicit operator T(csp_invoke_wrap<T> rval)
    {
        if (chan_state.ok != rval.state)
        {
            throw new chan_exception(rval.state);
        }
        return rval.result;
    }

    /// <summary>
    /// 隐式转换为通道状态
    /// </summary>
    /// <param name="rval">CSP调用包装实例</param>
    /// <returns>通道操作状态</returns>
    public static implicit operator chan_state(csp_invoke_wrap<T> rval)
    {
        return rval.state;
    }

    /// <summary>
    /// 默认值，状态为undefined
    /// </summary>
    public static csp_invoke_wrap<T> def
    {
        get
        {
            return new csp_invoke_wrap<T> { state = chan_state.undefined };
        }
    }

    /// <summary>
    /// 检查通道状态是否为ok
    /// </summary>
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
    public void check()
    {
        if (chan_state.ok != state)
        {
            throw new chan_exception(state);
        }
    }

    /// <summary>
    /// 重写ToString方法，返回CSP调用包装的字符串表示
    /// </summary>
    /// <returns>字符串表示</returns>
    public override string ToString()
    {
        return chan_state.ok == state ?
            string.Format("csp_invoke_wrap<{0}>.result={1}", typeof(T).Name, result) :
            string.Format("csp_invoke_wrap<{0}>.state={1}", typeof(T).Name, state);
    }
}

/// <summary>
/// CSP等待包装结构，用于存储CSP等待操作的结果
/// </summary>
/// <typeparam name="R">结果类型</typeparam>
/// <typeparam name="T">消息类型</typeparam>
public struct csp_wait_wrap<R, T>
{
    /// <summary>
    /// CSP通道结果对象
    /// </summary>
    public csp_chan<R, T>.csp_result result;
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
    /// <param name="rval">CSP等待包装实例</param>
    /// <returns>接收到的消息</returns>
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
    public static implicit operator T(csp_wait_wrap<R, T> rval)
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
    /// <param name="rval">CSP等待包装实例</param>
    /// <returns>通道操作状态</returns>
    public static implicit operator chan_state(csp_wait_wrap<R, T> rval)
    {
        return rval.state;
    }

    /// <summary>
    /// 默认值，状态为undefined
    /// </summary>
    public static csp_wait_wrap<R, T> def
    {
        get
        {
            return new csp_wait_wrap<R, T> { state = chan_state.undefined };
        }
    }

    /// <summary>
    /// 检查通道状态是否为ok
    /// </summary>
    /// <exception cref="chan_exception">当通道状态不是ok时抛出</exception>
    public void check()
    {
        if (chan_state.ok != state)
        {
            throw new chan_exception(state);
        }
    }

    /// <summary>
    /// 完成CSP操作，设置结果值
    /// </summary>
    /// <param name="res">结果值</param>
    /// <returns>是否完成成功</returns>
    public bool complete(R res)
    {
        return result.complete(res);
    }

    /// <summary>
    /// 失败CSP操作
    /// </summary>
    public void fail()
    {
        result.fail();
    }

    /// <summary>
    /// 检查结果对象是否为空
    /// </summary>
    /// <returns>结果对象是否为空</returns>
    public bool empty()
    {
        return null == result;
    }

    /// <summary>
    /// 重写ToString方法，返回CSP等待包装的字符串表示
    /// </summary>
    /// <returns>字符串表示</returns>
    public override string ToString()
    {
        return chan_state.ok == state ?
            string.Format("csp_wait_wrap<{0},{1}>.msg={2}", typeof(R).Name, typeof(T).Name, msg) :
            string.Format("csp_wait_wrap<{0},{1}>.state={2}", typeof(R).Name, typeof(T).Name, state);
    }
}

#if !NETCORE
/// <summary>
/// 值任务结构，用于表示异步操作的结果
/// </summary>
/// <typeparam name="T">结果类型</typeparam>
/// <remarks>
/// 该结构体在.NET Core中被引入，用于表示任务的结果。
/// 它在.NET Framework中被引入，用于表示异步操作的结果。
/// </remarks>
public struct ValueTask<T> : ICriticalNotifyCompletion
{
    T value;
    Task<T> task;

    public ValueTask(T result)
    {
        value = result;
        task = null;
    }

    public ValueTask(Task<T> result)
    {
        value = default(T);
        task = result;
    }

    public ValueTask<T> GetAwaiter()
    {
        return this;
    }

    public T GetResult()
    {
        return null == task ? value : task.GetAwaiter().GetResult();
    }

    public void OnCompleted(Action continuation)
    {
        task.GetAwaiter().OnCompleted(continuation);
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        task.GetAwaiter().UnsafeOnCompleted(continuation);
    }

    public Task<T> AsTask()
    {
        return task;
    }

    public T Result
    {
        get
        {
            return null == task ? value : task.Result;
        }
    }

    public bool IsCanceled
    {
        get
        {
            return null == task ? false : task.IsCanceled;
        }
    }

    public bool IsCompleted
    {
        get
        {
            return null == task ? true : task.IsCompleted;
        }
    }

    public bool IsFaulted
    {
        get
        {
            return null == task ? false : task.IsFaulted;
        }
    }
}
#endif

/// <summary>
/// 延迟CSP结果类，用于延迟获取CSP调用的结果
/// </summary>
/// <typeparam name="R">结果类型</typeparam>
public class delay_csp<R>
{
    /// <summary>
    /// CSP调用结果
    /// </summary>
    csp_invoke_wrap<R> _result;
    /// <summary>
    /// 标记是否已获取结果
    /// </summary>
    bool _taken;
    /// <summary>
    /// 标记是否已完成
    /// </summary>
    bool _completed;
    /// <summary>
    /// 结果丢失时的处理函数
    /// </summary>
    Action<csp_invoke_wrap<R>> _lostRes;
    /// <summary>
    /// 完成回调函数
    /// </summary>
    Action _continuation;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="lostRes">结果丢失时的处理函数</param>
    internal delay_csp(Action<csp_invoke_wrap<R>> lostRes)
    {
        _taken = false;
        _completed = false;
        _lostRes = lostRes;
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~delay_csp()
    {
        if (_completed && !_taken && null != _lostRes)
        {
            Task.Run(() => _lostRes(_result));
        }
    }

    /// <summary>
    /// 获取或设置CSP调用结果
    /// </summary>
    internal csp_invoke_wrap<R> result
    {
        set
        {
            _result = value;
            Action continuation;
            lock (this)
            {
                _completed = true;
                continuation = _continuation;
                _continuation = null;
            }
            functional.catch_invoke(continuation);
        }
        get
        {
            _taken = true;
            return _result;
        }
    }

    /// <summary>
    /// 设置完成回调函数
    /// </summary>
    /// <param name="continuation">回调函数</param>
    internal void on_completed(Action continuation)
    {
        bool completed;
        Action oldContinuation = null;
        lock (this)
        {
            if (completed = _completed)
            {
                if (null != _continuation)
                {
                    oldContinuation = _continuation;
                    _continuation = null;
                }
            }
            else
            {
                if (null == _continuation)
                {
                    _continuation = continuation;
                }
                else
                {
                    oldContinuation = _continuation;
                    _continuation = delegate ()
                    {
                        functional.catch_invoke(oldContinuation);
                        functional.catch_invoke(continuation);
                    };
                }
            }
        }
        if (completed)
        {
            functional.catch_invoke(oldContinuation);
            functional.catch_invoke(continuation);
        }
    }

    /// <summary>
    /// 获取是否已完成
    /// </summary>
    internal bool is_completed
    {
        get
        {
            return _completed;
        }
    }
}

/// <summary>
/// 协程生成器类，用于实现异步协程功能
/// 提供了一套完整的协程调度、同步、通信机制
/// </summary>
public class generator
{
    /// <summary>
    /// 本地值异常类，继承自System.Exception
    /// 用于表示本地值相关的异常
    /// </summary>
    public class local_value_exception : System.Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal local_value_exception() { }
    }

    /// <summary>
    /// 内部异常抽象类，继承自System.Exception
    /// 作为其他内部异常的基类
    /// </summary>
    public abstract class inner_exception : System.Exception { }

    /// <summary>
    /// 停止异常类，继承自inner_exception
    /// 用于表示协程停止时的异常
    /// </summary>
    public class stop_exception : inner_exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal stop_exception() { }
    }

    /// <summary>
    /// 选择停止当前异常类，继承自inner_exception
    /// 用于表示选择操作中停止当前协程的异常
    /// </summary>
    public class select_stop_current_exception : inner_exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal select_stop_current_exception() { }
    }

    /// <summary>
    /// 选择停止所有异常类，继承自inner_exception
    /// 用于表示选择操作中停止所有协程的异常
    /// </summary>
    public class select_stop_all_exception : inner_exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal select_stop_all_exception() { }
    }

    /// <summary>
    /// 消息停止当前异常类，继承自inner_exception
    /// 用于表示消息操作中停止当前协程的异常
    /// </summary>
    public class message_stop_current_exception : inner_exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal message_stop_current_exception() { }
    }

    /// <summary>
    /// 消息停止所有异常类，继承自inner_exception
    /// 用于表示消息操作中停止所有协程的异常
    /// </summary>
    public class message_stop_all_exception : inner_exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal message_stop_all_exception() { }
    }

    /// <summary>
    /// 类型哈希类，用于为不同类型生成唯一的哈希码
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    class type_hash<T>
    {
        /// <summary>
        /// 类型的唯一哈希码
        /// </summary>
        public static readonly int code = Interlocked.Increment(ref _hashCount);
    }

    /// <summary>
    /// 邮件包类，用于存储邮箱和代理操作
    /// </summary>
    class mail_pck
    {
        /// <summary>
        /// 邮箱通道
        /// </summary>
        public chan_base mailbox;
        /// <summary>
        /// 代理操作
        /// </summary>
        public child agentAction;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mb">邮箱通道</param>
        public mail_pck(chan_base mb)
        {
            mailbox = mb;
        }
    }

    /// <summary>
    /// 本地值包装抽象类
    /// 作为local_wrap<T>的基类
    /// </summary>
    abstract class local_wrap { }

    /// <summary>
    /// 本地值包装类，用于存储特定类型的本地值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    class local_wrap<T> : local_wrap
    {
        /// <summary>
        /// 存储的本地值
        /// </summary>
        internal T value;
    }

    /// <summary>
    /// 通知令牌结构，用于表示通知操作的令牌
    /// </summary>
    public struct notify_token
    {
        /// <summary>
        /// 主机生成器
        /// </summary>
        internal generator host;
        /// <summary>
        /// 令牌节点，存储通知动作
        /// </summary>
        internal LinkedListNode<Action<generator>> token;
    }

    /// <summary>
    /// 挂起令牌结构，用于表示挂起操作的令牌
    /// </summary>
    public struct suspend_token
    {
        /// <summary>
        /// 令牌节点，存储挂起动作
        /// </summary>
        internal LinkedListNode<Action<bool>> token;
    }

    /// <summary>
    /// 停止计数结构，用于跟踪停止操作的计数
    /// </summary>
    public struct stop_count
    {
        /// <summary>
        /// 计数值
        /// </summary>
        internal int _count;
    }

    /// <summary>
    /// 挂起计数结构，用于跟踪挂起操作的计数
    /// </summary>
    public struct suspend_count
    {
        /// <summary>
        /// 计数值
        /// </summary>
        internal int _count;
    }

    /// <summary>
    /// 屏蔽计数结构，用于跟踪锁定和挂起操作的计数
    /// </summary>
    public struct shield_count
    {
        /// <summary>
        /// 锁定计数值
        /// </summary>
        internal int _lockCount;
        /// <summary>
        /// 挂起计数值
        /// </summary>
        internal int _suspendCount;
    }

    /// <summary>
    /// 本地值类，用于存储特定类型的本地值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <remarks>
    /// </remarks>
    public class local<T>
    {
        readonly long _id = Interlocked.Increment(ref generator._idCount);

        public T value
        {
            get
            {
                generator host = self;
                if (null == host || null == host._genLocal)
                {
                    throw new local_value_exception();
                }
                local_wrap localWrap;
                if (!host._genLocal.TryGetValue(_id, out localWrap))
                {
                    throw new local_value_exception();
                }
                return ((local_wrap<T>)localWrap).value;
            }
            set
            {
                generator host = self;
                if (null == host)
                {
                    throw new local_value_exception();
                }
                if (null == host._genLocal)
                {
                    host._genLocal = new Dictionary<long, local_wrap>();
                }
                local_wrap localWrap;
                if (!host._genLocal.TryGetValue(_id, out localWrap))
                {
                    host._genLocal.Add(_id, new local_wrap<T> { value = value });
                }
                else
                {
                    ((local_wrap<T>)localWrap).value = value;
                }
            }
        }
        /// <summary>
        /// 移除本地值
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void remove()
        {
            generator host = self;
            if (null == host)
            {
                throw new local_value_exception();
            }
            host._genLocal?.Remove(_id);
        }
    }
    /// <summary>
    /// 拉取任务类，用于表示异步操作的结果
    /// </summary>
    /// <remarks>
    /// </remarks>
    class pull_task : ICriticalNotifyCompletion
    {
        bool _completed = false;
        bool _activated = false;
        Action _continuation;

        public pull_task GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
        }

        /// <summary>
        /// 注册完成回调
        /// </summary>
        /// <param name="continuation">完成回调</param>
        /// <remarks>
        /// </remarks>
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
        /// <summary>
        /// 注册完成回调
        /// </summary>
        /// <param name="continuation">完成回调</param>
        /// <remarks>
        /// </remarks>
        public void UnsafeOnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
        /// <summary>
        /// 是否完成
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool IsCompleted
        {
            get
            {
                return _completed;
            }
        }
        /// <summary>
        /// 是否等待等待
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool is_awaiting()
        {
            return null != _continuation;
        }
        /// <summary>
        /// 是否激活
        /// </summary>
        /// <remarks>
        /// </remarks>
        public bool activated
        {
            get
            {
                return _activated;
            }
            set
            {
                _activated = value;
            }
        }
        /// <summary>
        /// 创建新任务
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void new_task()
        {
            Debug.Assert(_completed && _activated, "不对称的推入操作!");
            _completed = false;
            _activated = false;
        }
        /// <summary>
        /// 提前完成
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void ahead_complete()
        {
            Debug.Assert(!_completed, "不对称的拉取操作!");
            _completed = true;
        }

        public void complete()
        {
            Debug.Assert(!_completed, "不对称的拉取操作!");
            _completed = true;
            Action continuation = _continuation;
            _continuation = null;
            continuation();
        }
    }

    /// <summary>
    /// 锁定停止保护类，用于在停止操作期间保护协程不被中断
    /// </summary>
    public class lock_stop_guard : IDisposable
    {
        static internal lock_stop_guard guard = new lock_stop_guard();

        private lock_stop_guard() { }

        public void Dispose()
        {
            unlock_stop();
        }
    }

#if CHECK_STEP_TIMEOUT
public class call_stack_info
{
    public readonly string time;
    public readonly string file;
    public readonly int line;

    public call_stack_info(string t, string f, int l)
    {
        time = t;
        file = f;
        line = l;
    }

    public override string ToString()
    {
        return string.Format("<file>{0} <line>{1} <time>{2}", file, line, time);
    }
}
LinkedList<call_stack_info[]> _makeStack;
long _beginStepTick;
static readonly int _stepMaxCycle = 100;
#endif

    /// <summary>
    /// 哈希计数，用于生成类型哈希码
    /// </summary>
    static int _hashCount = 0;
    static long _idCount = 0;
    static Task _nilTask = functional.init(() => { Task task = new Task(nil_action.action); task.RunSynchronously(); return task; });
    static ReaderWriterLockSlim _nameMutex = new ReaderWriterLockSlim();
    static Dictionary<string, generator> _nameGens = new Dictionary<string, generator>();
    static Action<System.Exception> _hookException = null;

    Dictionary<long, mail_pck> _mailboxMap;
    Dictionary<long, local_wrap> _genLocal;
    LinkedList<LinkedList<select_chan_base>> _topSelectChans;
    LinkedList<Action<bool>> _suspendHandler;
    LinkedList<children> _children;
    LinkedList<Action<generator>> _callbacks;
    chan_notify_sign _ioSign;
    System.Exception _excep;
    pull_task _pullTask;
    children _agentMng;
    async_timer _timer;
    string _name;
    long _lastTm;
    long _yieldCount;
    long _lastYieldCount;
    long _id;
    int _lockCount;
    int _lockSuspendCount;
    bool _disableTopStop;
    bool _beginQuit;
    bool _isSuspend;
    bool _holdSuspend;
    bool _hasBlock;
    bool _overtime;
    bool _isForce;
    bool _isStop;
    bool _isRun;
    bool _mustTick;

    /// <summary>
    /// 委托
    /// </summary>
    /// <returns></returns>
    public delegate Task action();
    /// <summary>
    /// 构造函数
    /// </summary>
    generator() { }
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    /// <returns></returns>
    static public generator make(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        return (new generator()).init(strand, generatorAction, completedHandler, suspendHandler);
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    static public void go(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        make(strand, generatorAction, completedHandler, suspendHandler).run();
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="newGen"></param>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    static public void go(out generator newGen, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        newGen = make(strand, generatorAction, completedHandler, suspendHandler);
        newGen.run();
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    /// <returns></returns>
    static public generator tgo(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        generator newGen = make(strand, generatorAction, completedHandler, suspendHandler);
        newGen.trun();
        return newGen;
    }
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    /// <returns></returns>
    static public generator make(string name, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        generator newGen = make(strand, generatorAction, completedHandler, suspendHandler);
        newGen._name = name;
        try
        {
            _nameMutex.EnterWriteLock();
            _nameGens.Add(name, newGen);
            _nameMutex.ExitWriteLock();
        }
        catch (System.ArgumentException)
        {
            _nameMutex.ExitWriteLock();
#if DEBUG
            Trace.Fail(string.Format("generator {0}重名", name));
#endif
        }
        return newGen;
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    static public void go(string name, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        make(name, strand, generatorAction, completedHandler, suspendHandler).run();
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="newGen"></param>
    /// <param name="name"></param>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    static public void go(out generator newGen, string name, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        newGen = make(name, strand, generatorAction, completedHandler, suspendHandler);
        newGen.run();
    }
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    /// <returns></returns>
    static public generator tgo(string name, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
    {
        generator newGen = make(name, strand, generatorAction, completedHandler, suspendHandler);
        newGen.trun();
        return newGen;
    }
    /// <summary>
    /// 查找
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static public generator find(string name)
    {
        generator gen = null;
        _nameMutex.EnterReadLock();
        _nameGens.TryGetValue(name, out gen);
        _nameMutex.ExitReadLock();
        return gen;
    }
    /// <summary>
    /// 名称
    /// </summary>
    /// <returns></returns>
    public string name()
    {
        return _name;
    }

#if CHECK_STEP_TIMEOUT
static void up_stack_frame(LinkedList<call_stack_info[]> callStack, int offset = 0, int count = 1)
{
    offset += 2;
    StackFrame[] sts = (new StackTrace(true)).GetFrames();
    string time = DateTime.Now.ToString("yy-MM-dd HH:mm:ss.fff");
    call_stack_info[] snap = new call_stack_info[count];
    callStack.AddFirst(snap);
    for (int i = 0; i < count; ++i, ++offset)
    {
        if (offset < sts.Length)
        {
            snap[i] = new call_stack_info(time, sts[offset].GetFileName(), sts[offset].GetFileLineNumber());
        }
        else
        {
            snap[i] = new call_stack_info(time, "null", -1);
        }
    }
}
#endif

#if CHECK_STEP_TIMEOUT
generator init(shared_strand strand, action generatorAction, Action completedHandler, Action<bool> suspendHandler, LinkedList<call_stack_info[]> makeStack = null)
{
    if (null != makeStack)
    {
        _makeStack = makeStack;
    }
    else
    {
        _makeStack = new LinkedList<call_stack_info[]>();
        up_stack_frame(_makeStack, 1, 6);
    }
    _beginStepTick = system_tick.get_tick_ms();
#else
    /// <summary>
    /// 初始化生成器实例，设置初始状态和属性
    /// </summary>
    /// <param name="strand"></param>
    /// <param name="generatorAction"></param>
    /// <param name="completedHandler"></param>
    /// <param name="suspendHandler"></param>
    /// <returns></returns>
    generator init(shared_strand strand, action generatorAction, Action completedHandler, Action<bool> suspendHandler)
    {
#endif
        _id = Interlocked.Increment(ref _idCount);
        _mustTick = true;
        _isForce = false;
        _isStop = false;
        _isRun = false;
        _overtime = false;
        _isSuspend = false;
        _holdSuspend = false;
        _hasBlock = false;
        _beginQuit = false;
        _disableTopStop = false;
        _lockCount = 0;
        _lockSuspendCount = 0;
        _lastTm = 0;
        _yieldCount = 0;
        _lastYieldCount = 0;
        _pullTask = new pull_task();
        _ioSign = new chan_notify_sign();
        _timer = new async_timer(strand);
        if (null != suspendHandler)
        {
            _suspendHandler = new LinkedList<Action<bool>>();
            _suspendHandler.AddLast(suspendHandler);
        }
        strand.hold_work();
        strand.dispatch(async delegate ()
        {
            try
            {
                try
                {
                    _mustTick = false;
                    await async_wait();
                    await generatorAction();
                }
                finally
                {
                    _lockSuspendCount = -1;
                    if (null != _mailboxMap)
                    {
                        foreach (KeyValuePair<long, mail_pck> ele in _mailboxMap)
                        {
                            ele.Value.mailbox.close();
                        }
                    }
                }
                if (!_isForce && null != _children)
                {
                    while (0 != _children.Count)
                    {
                        await _children.First.Value.wait_all();
                    }
                }
            }
            catch (stop_exception) { }
            catch (System.Exception ec)
            {
                if (null == _hookException)
                {
                    string source = ec.Source;
                    string message = ec.Message;
                    string stacktrace = ec.StackTrace;
                    System.Exception inner = ec;
                    while (null != (inner = inner.InnerException))
                    {
                        source = string.Format("{0}\n{1}", inner.Source, source);
                        message = string.Format("{0}\n{1}", inner.Message, message);
                        stacktrace = string.Format("{0}\n{1}", inner.StackTrace, stacktrace);
                    }
#if NETCORE
                Debug.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}", "generator 内部未捕获的异常!", message, source, stacktrace));
#else
                    // Task.Run(() => System.Windows.Forms.MessageBox.Show(string.Format("{0}\n{1}\n{2}", message, source, stacktrace), "generator 内部未捕获的异常!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error)).Wait();
#endif
                }
                else
                {
                    functional.catch_invoke(_hookException, ec);
                }
                _excep = ec;
                _lockCount++;
            }
            finally
            {
                if (_isForce || null != _excep)
                {
                    _timer.Cancel();
                    if (null != _children && 0 != _children.Count)
                    {
                        await children.stop(_children);
                    }
                }
            }
            if (null != _name)
            {
                _nameMutex.EnterWriteLock();
                _nameGens.Remove(_name);
                _nameMutex.ExitWriteLock();
            }
            _isStop = true;
            _suspendHandler = null;
            strand.currSelf = null;
            functional.catch_invoke(completedHandler);
            if (null != _callbacks)
            {
                while (0 != _callbacks.Count)
                {
                    Action<generator> ntf = _callbacks.First.Value;
                    _callbacks.RemoveFirst();
                    functional.catch_invoke(ntf, this);
                }
            }
            strand.release_work();
        });
        return this;
    }
    /// <summary>
    /// 不检查
    /// </summary>
    void no_check_next()
    {
        if (_isSuspend)
        {
            _hasBlock = true;
        }
        else if (!_pullTask.is_awaiting())
        {
            _pullTask.ahead_complete();
        }
        else
        {
            _yieldCount++;
            generator oldGen = strand.currSelf;
            if (null == oldGen || !oldGen._pullTask.activated)
            {
                strand.currSelf = this;
                _pullTask.complete();
                strand.currSelf = oldGen;
            }
            else
            {
                oldGen._pullTask.activated = false;
                strand.currSelf = this;
                _pullTask.complete();
                strand.currSelf = oldGen;
                oldGen._pullTask.activated = true;
            }
        }
    }
    /// <summary>
    /// 下一步
    /// </summary>
    /// <param name="beginQuit"></param>
    void next(bool beginQuit)
    {
        if (!_isStop && _beginQuit == beginQuit)
        {
            no_check_next();
        }
    }
    /// <summary>
    /// 退出下一步
    /// </summary>
    void quit_next()
    {
        next(true);
    }
    /// <summary>
    /// 不退出下一步
    /// </summary>
    void no_quit_next()
    {
        next(false);
    }
    /// <summary>
    /// 检测
    /// </summary>
    /// <returns></returns>
    multi_check new_multi_check()
    {
        _pullTask.new_task();
        return new multi_check { callbacked = false, beginQuit = _beginQuit };
    }
    /// <summary>
    /// 运行
    /// </summary>
    public void run()
    {
        if (strand.running_in_this_thread() && !_mustTick)
        {
            if (!_isRun && !_isStop)
            {
                _isRun = true;
                no_check_next();
            }
        }
        else
        {
            trun();
        }
    }
    /// <summary>
    /// 运行
    /// </summary>
    public void trun()
    {
        strand.post(delegate ()
        {
            if (!_isRun && !_isStop)
            {
                _isRun = true;
                no_check_next();
            }
        });
    }
    /// <summary>
    /// 挂起回调
    /// </summary>
    /// <param name="isSuspend"></param>
    /// <param name="cb"></param>
    /// <param name="canSuspendCb"></param>
    private void _suspend_cb(bool isSuspend, Action cb = null, bool canSuspendCb = true)
    {
        if (null != _children && 0 != _children.Count)
        {
            int count = _children.Count;
            Action handler = delegate ()
            {
                if (0 == --count)
                {
                    if (canSuspendCb && null != _suspendHandler)
                    {
                        for (LinkedListNode<Action<bool>> it = _suspendHandler.First; null != it; it = it.Next)
                        {
                            functional.catch_invoke(it.Value, isSuspend);
                        }
                    }
                    functional.catch_invoke(cb);
                }
            };
            _mustTick = true;
            for (LinkedListNode<children> it = _children.First; null != it; it = it.Next)
            {
                it.Value.suspend(isSuspend, handler);
            }
            _mustTick = false;
        }
        else
        {
            if (canSuspendCb && null != _suspendHandler)
            {
                _mustTick = true;
                for (LinkedListNode<Action<bool>> it = _suspendHandler.First; null != it; it = it.Next)
                {
                    functional.catch_invoke(it.Value, isSuspend);
                }
                _mustTick = false;
            }
            functional.catch_invoke(cb);
        }
    }
    /// <summary>
    /// 挂起
    /// </summary>
    /// <param name="cb"></param>
    private void _suspend(Action cb = null)
    {
        if (!_isStop && !_beginQuit && !_isSuspend)
        {
            if (0 == _lockSuspendCount)
            {
                _isSuspend = true;
                if (0 != _lastTm)
                {
                    _lastTm -= system_tick.get_tick_us() - _timer.Cancel();
                    if (_lastTm <= 0)
                    {
                        _lastTm = 0;
                        _hasBlock = true;
                    }
                }
                _suspend_cb(true, cb);
            }
            else
            {
                _holdSuspend = true;
                _suspend_cb(true, cb, false);
            }
        }
        else
        {
            functional.catch_invoke(cb);
        }
    }
    /// <summary>
    /// 挂起
    /// </summary>
    /// <param name="cb"></param>
    public void tsuspend(Action cb = null)
    {
        strand.post(() => _suspend(cb));
    }
    /// <summary>
    /// 挂起
    /// </summary>
    /// <param name="cb"></param>
    public void suspend(Action cb = null)
    {
        if (strand.running_in_this_thread() && !_mustTick)
        {
            _suspend(cb);
        }
        else
        {
            tsuspend(cb);
        }
    }
    /// <summary>
    /// 暂停
    /// </summary>
    /// <param name="cb"></param>
    private void _resume(Action cb = null)
    {
        if (!_isStop && !_beginQuit)
        {
            if (_isSuspend)
            {
                _isSuspend = false;
                _suspend_cb(false, cb);
                if (_hasBlock)
                {
                    _hasBlock = false;
                    no_quit_next();
                }
                else if (0 != _lastTm)
                {
                    _timer.timeout_us(_lastTm, no_check_next);
                }
            }
            else
            {
                _holdSuspend = false;
                _suspend_cb(false, cb, false);
            }
        }
        else
        {
            functional.catch_invoke(cb);
        }
    }
    /// <summary>
    /// 暂停
    /// </summary>
    /// <param name="cb"></param>
    public void tresume(Action cb = null)
    {
        strand.post(() => _resume(cb));
    }
    /// <summary>
    /// 暂停
    /// </summary>
    /// <param name="cb"></param>
    public void resume(Action cb = null)
    {
        if (strand.running_in_this_thread() && !_mustTick)
        {
            _resume(cb);
        }
        else
        {
            tresume(cb);
        }
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <exception cref="stop_exception"></exception>
    private void _stop()
    {
        _isForce = true;
        if (0 == _lockCount)
        {
            if (!_disableTopStop && _pullTask.activated)
            {
                _suspendHandler = null;
                _lockSuspendCount = 0;
                _holdSuspend = false;
                _isSuspend = false;
                _beginQuit = true;
                _timer.Cancel();
                throw new stop_exception();
            }
            else if (_pullTask.is_awaiting())
            {
                _isSuspend = false;
                no_quit_next();
            }
            else
            {
                tstop();
            }
        }
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="disable"></param>
    static public void disable_top_stop(bool disable = true)
    {
        generator this_ = self;
        this_._disableTopStop = disable;
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void tstop()
    {
        strand.post(delegate ()
        {
            if (!_isStop)
            {
                _stop();
            }
        });
    }
    /// <summary>
    /// 停止
    /// </summary>
    public void stop()
    {
        if (strand.running_in_this_thread() && !_mustTick)
        {
            if (!_isStop)
            {
                _stop();
            }
        }
        else
        {
            tstop();
        }
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="continuation"></param>
    public void tstop(Action<generator> continuation)
    {
        strand.post(delegate ()
        {
            if (!_isStop)
            {
                if (null == _callbacks)
                {
                    _callbacks = new LinkedList<Action<generator>>();
                }
                _callbacks.AddLast(continuation);
                _stop();
            }
            else
            {
                functional.catch_invoke(continuation, this);
            }
        });
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="continuation"></param>
    public void tstop(Action continuation)
    {
        tstop((generator _) => functional.catch_invoke(continuation));
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="continuation"></param>
    public void stop(Action<generator> continuation)
    {
        if (strand.running_in_this_thread() && !_mustTick)
        {
            if (!_isStop)
            {
                if (null == _callbacks)
                {
                    _callbacks = new LinkedList<Action<generator>>();
                }
                _callbacks.AddLast(continuation);
                _stop();
            }
            else
            {
                functional.catch_invoke(continuation, this);
            }
        }
        else
        {
            tstop(continuation);
        }
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="continuation"></param>
    public void stop(Action continuation)
    {
        stop((generator _) => functional.catch_invoke(continuation));
    }
    /// <summary>
    /// 追加停止回调
    /// </summary>
    /// <param name="continuation"></param>
    /// <param name="removeCb"></param>
    public void append_stop_callback(Action<generator> continuation, Action<notify_token> removeCb = null)
    {
        if (strand.running_in_this_thread())
        {
            if (!_isStop)
            {
                if (null == _callbacks)
                {
                    _callbacks = new LinkedList<Action<generator>>();
                }
                functional.catch_invoke(removeCb, new notify_token { host = this, token = _callbacks.AddLast(continuation) });
            }
            else
            {
                functional.catch_invoke(continuation, this);
                functional.catch_invoke(removeCb, new notify_token { host = this });
            }
        }
        else
        {
            strand.post((Action)delegate ()
            {
                if (!_isStop)
                {
                    if (null == _callbacks)
                    {
                        _callbacks = new LinkedList<Action<generator>>();
                    }
                    functional.catch_invoke(removeCb, new notify_token { host = this, token = _callbacks.AddLast(continuation) });
                }
                else
                {
                    functional.catch_invoke(continuation, this);
                    functional.catch_invoke(removeCb, new notify_token { host = this });
                }
            });
        }
    }
    /// <summary>
    /// 追加停止回调
    /// </summary>
    /// <param name="continuation"></param>
    /// <param name="removeCb"></param>
    public void append_stop_callback(Action continuation, Action<notify_token> removeCb = null)
    {
        append_stop_callback((generator _) => functional.catch_invoke(continuation), removeCb);
    }
    /// <summary>
    /// 删除停止回调
    /// </summary>
    /// <param name="cancelToken"></param>
    /// <param name="cb"></param>
    public void remove_stop_callback(notify_token cancelToken, Action cb = null)
    {
        Debug.Assert(this == cancelToken.host, "异常的 remove_stop_callback 调用!");
        if (strand.running_in_this_thread())
        {
            if (null != cancelToken.token && null != cancelToken.token.List)
            {
                _callbacks.Remove(cancelToken.token);
            }
            functional.catch_invoke(cb);
        }
        else
        {
            strand.post(delegate ()
            {
                if (null != cancelToken.token && null != cancelToken.token.List)
                {
                    _callbacks.Remove(cancelToken.token);
                }
                functional.catch_invoke(cb);
            });
        }
    }
    /// <summary>
    /// 追加挂起回调
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    static public suspend_token append_suspend_handler(Action<bool> handler)
    {
        generator this_ = self;
        if (null == this_._suspendHandler)
        {
            this_._suspendHandler = new LinkedList<Action<bool>>();
        }
        return new suspend_token { token = this_._suspendHandler.AddLast(handler) };
    }
    /// <summary>
    /// 删除挂起回调
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    static public bool remove_suspend_handler(suspend_token token)
    {
        generator this_ = self;
        if (null != this_._suspendHandler && null != token.token && null != token.token.List)
        {
            this_._suspendHandler.Remove(token.token);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 是否强制停止
    /// </summary>
    /// <returns></returns>
    public bool is_force()
    {
        Debug.Assert(_isStop, "不正确的 is_force 调用，generator 还没有结束!");
        return _isForce;
    }
    /// <summary>
    /// 是否有异常
    /// </summary>
    /// <returns></returns>
    public bool has_excep()
    {
        Debug.Assert(_isStop, "不正确的 has_excep 调用，generator 还没有结束!");
        return null != _excep;
    }
    /// <summary>
    /// 异常
    /// </summary>
    public System.Exception excep
    {
        get
        {
            Debug.Assert(_isStop, "不正确的 excep 调用，generator 还没有结束!");
            return _excep;
        }
    }
    /// <summary>
    /// 检测异常
    /// </summary>
    public void check_excep()
    {
        Debug.Assert(_isStop, "不正确的 check_excep 调用，generator 还没有结束!");
        if (null != _excep)
        {
            throw excep;
        }
    }
    /// <summary>
    /// 是否完成
    /// </summary>
    /// <returns></returns>
    public bool is_completed()
    {
        return _isStop;
    }
    /// <summary>
    /// 是否开始退出
    /// </summary>
    /// <returns></returns>
    static public bool begin_quit()
    {
        generator this_ = self;
        return this_._beginQuit;
    }
    /// <summary>
    /// 是否强制退出
    /// </summary>
    /// <returns></returns>
    public bool check_quit()
    {
        return _isForce;
    }
    /// <summary>
    /// 是否挂起
    /// </summary>
    /// <returns></returns>
    public bool check_suspend()
    {
        return _holdSuspend;
    }
    /// <summary>
    /// 同步停止
    /// </summary>
    public void sync_stop()
    {
        Debug.Assert(strand.wait_safe(), "不正确的 sync_stop 调用!");
        wait_group wg = new wait_group(1);
        stop(wg.done);
        wg.sync_wait();
    }
    /// <summary>
    /// 同步等待停止
    /// </summary>
    public void sync_wait_stop()
    {
        Debug.Assert(strand.wait_safe(), "不正确的 sync_wait_stop 调用!");
        wait_group wg = new wait_group(1);
        append_stop_callback(wg.done);
        wg.sync_wait();
    }
    /// <summary>
    /// 同步超时停止
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public bool sync_timed_wait_stop(int ms)
    {
        Debug.Assert(strand.wait_safe(), "不正确的 sync_timed_wait_stop 调用!");
        wait_group wg = new wait_group(1);
        wait_group wgCancelId = new wait_group(1);
        notify_token cancelToken = default(notify_token);
        append_stop_callback(wg.done, delegate (notify_token ct)
        {
            cancelToken = ct;
            wgCancelId.done();
        });
        if (!wg.sync_timed_wait(ms))
        {
            wgCancelId.sync_wait();
            if (null != cancelToken.token)
            {
                wgCancelId.add(1);
                remove_stop_callback(cancelToken, wgCancelId.done);
                wgCancelId.sync_wait();
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 同步执行
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="strand"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    static public R sync_go<R>(shared_strand strand, Func<Task<R>> handler)
    {
        Debug.Assert(strand.wait_safe(), "不正确的 sync_go 调用!");
        R res = default(R);
        System.Exception hasExcep = null;
        wait_group wg = new wait_group(1);
        go(strand, async delegate ()
        {
            try
            {
                res = await handler();
            }
            catch (stop_exception)
            {
                throw;
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }, wg.done);
        wg.sync_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
        return res;
    }
    /// <summary>
    /// 同步执行
    /// </summary>
    /// <typeparam name="R"></typeparam>
    /// <param name="strand"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    static public R sync_go<R>(shared_strand strand, Func<ValueTask<R>> handler)
    {
        Debug.Assert(strand.wait_safe(), "不正确的 sync_go 调用!");
        R res = default(R);
        System.Exception hasExcep = null;
        wait_group wg = new wait_group(1);
        go(strand, async delegate ()
        {
            try
            {
                res = await handler();
            }
            catch (stop_exception)
            {
                throw;
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }, wg.done);
        wg.sync_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
        return res;
    }
    /// <summary>
    /// 同步执行
    /// </summary>
    /// <param name="strand"></param>
    /// <param name="handler"></param>
    static public void sync_go(shared_strand strand, action handler)
    {
        Debug.Assert(strand.wait_safe(), "不正确的 sync_go 调用!");
        System.Exception hasExcep = null;
        wait_group wg = new wait_group(1);
        go(strand, async delegate ()
        {
            try
            {
                await handler();
            }
            catch (stop_exception)
            {
                throw;
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }, wg.done);
        wg.sync_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
    }
    /// <summary>
    /// 挂起
    /// </summary>
    /// <returns></returns>
    static public Task hold()
    {
        generator this_ = self;
        this_._pullTask.new_task();
        return this_.async_wait();
    }
    /// <summary>
    /// 暂停
    /// </summary>
    /// <returns></returns>
    static public Task pause_self()
    {
        generator this_ = self;
        if (!this_._beginQuit)
        {
            if (0 == this_._lockSuspendCount)
            {
                this_._isSuspend = true;
                this_._suspend_cb(true);
                this_._hasBlock = true;
                this_._pullTask.new_task();
                return this_.async_wait();
            }
            else
            {
                this_._holdSuspend = true;
            }
        }
        return non_async();
    }
    /// <summary>
    /// 挂起
    /// </summary>
    /// <returns></returns>
    static public Task halt_self()
    {
        generator this_ = self;
        this_.stop();
        return non_async();
    }
    /// <summary>
    /// 锁定停止
    /// </summary>
    /// <returns></returns>
    internal stop_count lock_stop_()
    {
        if (!_beginQuit)
        {
            _lockCount++;
        }
        return new stop_count { _count = _lockCount };
    }
    /// <summary>
    /// 无锁停止
    /// </summary>
    /// <param name="stopCount"></param>
    /// <exception cref="stop_exception"></exception>
    internal void unlock_stop_(stop_count stopCount = default(stop_count))
    {
        if (stopCount._count > 0)
        {
            _lockCount = stopCount._count;
        }
        Debug.Assert(_beginQuit || _lockCount > 0, "unlock_stop 不匹配!");
        if (!_beginQuit && 0 == --_lockCount && _isForce)
        {
            _lockSuspendCount = 0;
            _holdSuspend = false;
            _beginQuit = true;
            _suspendHandler = null;
            _timer.Cancel();
            throw new stop_exception();
        }
    }
    /// <summary>
    /// 锁定停止
    /// </summary>
    static public lock_stop_guard using_lock
    {
        get
        {
            lock_stop();
            return lock_stop_guard.guard;
        }
    }
    /// <summary>
    /// 值任务转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    static private ValueTask<T> to_vtask<T>(T task)
    {
        return new ValueTask<T>(task);
    }
    /// <summary>
    /// 值任务转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    static private ValueTask<T> to_vtask<T>(Task<T> task)
    {
        return new ValueTask<T>(task);
    }
    /// <summary>
    /// 锁定挂起
    /// </summary>
    /// <returns></returns>
    internal suspend_count lock_suspend_()
    {
        if (!_beginQuit)
        {
            _lockSuspendCount++;
        }
        return new suspend_count { _count = _lockSuspendCount };
    }
    /// <summary>
    /// 无锁定挂起
    /// </summary>
    /// <param name="suspendCount"></param>
    /// <returns></returns>
    internal Task unlock_suspend_(suspend_count suspendCount = default(suspend_count))
    {
        if (suspendCount._count > 0)
        {
            _lockSuspendCount = suspendCount._count;
        }
        Debug.Assert(_beginQuit || _lockSuspendCount > 0, "unlock_suspend 不匹配!");
        if (!_beginQuit && 0 == --_lockSuspendCount && _holdSuspend)
        {
            _holdSuspend = false;
            _isSuspend = true;
            _suspend_cb(true);
            _hasBlock = true;
            _pullTask.new_task();
            return async_wait();
        }
        return non_async();
    }
    /// <summary>
    /// 锁定挂起和停止
    /// </summary>
    /// <returns></returns>
    internal shield_count lock_suspend_and_stop_()
    {
        if (!_beginQuit)
        {
            _lockSuspendCount++;
            _lockCount++;
        }
        return new shield_count { _lockCount = _lockCount, _suspendCount = _lockSuspendCount };
    }
    /// <summary>
    /// 无锁定挂起和停止
    /// </summary>
    /// <param name="shieldCount"></param>
    /// <returns></returns>
    /// <exception cref="stop_exception"></exception>
    internal Task unlock_suspend_and_stop_(shield_count shieldCount = default(shield_count))
    {
        if (shieldCount._lockCount > 0 && shieldCount._suspendCount > 0)
        {
            _lockCount = shieldCount._lockCount;
            _lockSuspendCount = shieldCount._suspendCount;
        }
        Debug.Assert(_beginQuit || _lockCount > 0, "unlock_stop 不匹配!");
        Debug.Assert(_beginQuit || _lockSuspendCount > 0, "unlock_suspend 不匹配!");
        if (!_beginQuit && 0 == --_lockCount && _isForce)
        {
            _lockSuspendCount = 0;
            _holdSuspend = false;
            _beginQuit = true;
            _suspendHandler = null;
            _timer.Cancel();
            throw new stop_exception();
        }
        if (!_beginQuit && 0 == --_lockSuspendCount && _holdSuspend)
        {
            _holdSuspend = false;
            _isSuspend = true;
            _suspend_cb(true);
            _hasBlock = true;
            _pullTask.new_task();
            return async_wait();
        }
        return non_async();
    }
    /// <summary>
    /// 锁停止
    /// </summary>
    /// <returns></returns>
    static public stop_count lock_stop()
    {
        generator this_ = self;
        return this_.lock_stop_();
    }
    /// <summary>
    /// 无锁停止
    /// </summary>
    /// <param name="stopCount"></param>
    static public void unlock_stop(stop_count stopCount = default(stop_count))
    {
        generator this_ = self;
        this_.unlock_stop_(stopCount);
    }
    /// <summary>
    /// 锁挂起
    /// </summary>
    /// <returns></returns>
    static public suspend_count lock_suspend()
    {
        generator this_ = self;
        return this_.lock_suspend_();
    }
    /// <summary>
    /// 无锁挂起
    /// </summary>
    /// <param name="suspendCount"></param>
    /// <returns></returns>
    static public Task unlock_suspend(suspend_count suspendCount = default(suspend_count))
    {
        generator this_ = self;
        return this_.unlock_suspend_(suspendCount);
    }
    /// <summary>
    /// 锁定挂起和停止
    /// </summary>
    /// <returns></returns>
    static public shield_count lock_suspend_and_stop()
    {
        generator this_ = self;
        return this_.lock_suspend_and_stop_();
    }
    /// <summary>
    /// 无锁定挂起和停止
    /// </summary>
    /// <param name="shieldCount"></param>
    /// <returns></returns>
    static public Task unlock_suspend_and_stop(shield_count shieldCount = default(shield_count))
    {
        generator this_ = self;
        return this_.unlock_suspend_and_stop_(shieldCount);
    }
    /// <summary>
    /// 锁定挂起和停止
    /// </summary>
    /// <returns></returns>
    static public shield_count lock_shield()
    {
        return lock_suspend_and_stop();
    }
    /// <summary>
    /// 无锁定挂起和停止 
    /// </summary>
    /// <param name="shieldCount"></param>
    /// <returns></returns>
    static public Task unlock_shield(shield_count shieldCount = default(shield_count))
    {
        return unlock_suspend_and_stop(shieldCount);
    }
    /// <summary>
    /// 推
    /// </summary>
    private void enter_push()
    {
#if CHECK_STEP_TIMEOUT
    Debug.Assert(strand.running_in_this_thread(), "异常的 await 调用!");
    if (!system_tick.check_step_debugging() && system_tick.get_tick_ms() - _beginStepTick > _stepMaxCycle)
    {
        call_stack_info[] stackHead = _makeStack.Last.Value;
        Debug.WriteLine(string.Format("单步超时:\n{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n",
            stackHead[0], stackHead[1], stackHead[2], stackHead[3], stackHead[4], stackHead[5], new StackTrace(true)));
    }
#endif
    }
    /// <summary>
    /// 停止推
    /// </summary>
    /// <exception cref="stop_exception"></exception>
    private void leave_push()
    {
#if CHECK_STEP_TIMEOUT
    _beginStepTick = system_tick.get_tick_ms();
#endif
        _lastTm = 0;
        _pullTask.activated = true;
        if (_isForce && !_beginQuit && 0 == _lockCount)
        {
            _lockSuspendCount = 0;
            _holdSuspend = false;
            _beginQuit = true;
            _suspendHandler = null;
            _timer.Cancel();
            throw new stop_exception();
        }
    }
    /// <summary>
    /// 推送任务
    /// </summary>
    /// <returns></returns>
    private async Task push_task()
    {
        enter_push();
        await _pullTask;
        leave_push();
    }
    /// <summary>
    /// 推送任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    private async Task<T> push_task<T>(async_result_wrap<T> res)
    {
        enter_push();
        await _pullTask;
        leave_push();
        return res.value1;
    }
    /// <summary>
    /// 推送任务
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    private async Task<tuple<T1, T2>> push_task<T1, T2>(async_result_wrap<T1, T2> res)
    {
        enter_push();
        await _pullTask;
        leave_push();
        return tuple.make(res.value1, res.value2);
    }
    /// <summary>
    /// 推送任务
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    private async Task<tuple<T1, T2, T3>> push_task<T1, T2, T3>(async_result_wrap<T1, T2, T3> res)
    {
        enter_push();
        await _pullTask;
        leave_push();
        return tuple.make(res.value1, res.value2, res.value3);
    }
    /// <summary>
    /// 任务完成
    /// </summary>
    /// <returns></returns>
    private bool new_task_completed()
    {
        if (_pullTask.IsCompleted)
        {
            enter_push();
            leave_push();
            return true;
        }
        return false;
    }
    /// <summary>
    /// 同步等待
    /// </summary>
    /// <returns></returns>
    public Task async_wait()
    {
        if (!new_task_completed())
        {
            return push_task();
        }
        return non_async();
    }
    /// <summary>
    /// 同步等待
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public ValueTask<T> async_wait<T>(async_result_wrap<T> res)
    {
        if (!new_task_completed())
        {
            return to_vtask(push_task(res));
        }
        return to_vtask(res.value1);
    }
    /// <summary>
    /// 同步等待
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public ValueTask<tuple<T1, T2>> async_wait<T1, T2>(async_result_wrap<T1, T2> res)
    {
        if (!new_task_completed())
        {
            return to_vtask(push_task(res));
        }
        return to_vtask(tuple.make(res.value1, res.value2));
    }
    /// <summary>
    /// 同步等待
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public ValueTask<tuple<T1, T2, T3>> async_wait<T1, T2, T3>(async_result_wrap<T1, T2, T3> res)
    {
        if (!new_task_completed())
        {
            return to_vtask(push_task(res));
        }
        return to_vtask(tuple.make(res.value1, res.value2, res.value3));
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <returns></returns>
    private async Task<bool> timed_push_task()
    {
        enter_push();
        await _pullTask;
        leave_push();
        return !_overtime;
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    private async Task<tuple<bool, T>> timed_push_task<T>(async_result_wrap<T> res)
    {
        enter_push();
        await _pullTask;
        leave_push();
        return tuple.make(!_overtime, res.value1);
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    private async Task<tuple<bool, tuple<T1, T2>>> timed_push_task<T1, T2>(async_result_wrap<T1, T2> res)
    {
        enter_push();
        await _pullTask;
        leave_push();
        return tuple.make(!_overtime, tuple.make(res.value1, res.value2));
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    private async Task<tuple<bool, tuple<T1, T2, T3>>> timed_push_task<T1, T2, T3>(async_result_wrap<T1, T2, T3> res)
    {
        enter_push();
        await _pullTask;
        leave_push();
        return tuple.make(!_overtime, tuple.make(res.value1, res.value2, res.value3));
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <returns></returns>
    public ValueTask<bool> timed_async_wait()
    {
        if (!new_task_completed())
        {
            return to_vtask(timed_push_task());
        }
        return to_vtask(!_overtime);
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public ValueTask<tuple<bool, T>> timed_async_wait<T>(async_result_wrap<T> res)
    {
        if (!new_task_completed())
        {
            return to_vtask(timed_push_task(res));
        }
        return to_vtask(tuple.make(!_overtime, res.value1));
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public ValueTask<tuple<bool, tuple<T1, T2>>> timed_async_wait<T1, T2>(async_result_wrap<T1, T2> res)
    {
        if (!new_task_completed())
        {
            return to_vtask(timed_push_task(res));
        }
        return to_vtask(tuple.make(!_overtime, tuple.make(res.value1, res.value2)));
    }
    /// <summary>
    /// 超时推送任务
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public ValueTask<tuple<bool, tuple<T1, T2, T3>>> timed_async_wait<T1, T2, T3>(async_result_wrap<T1, T2, T3> res)
    {
        if (!new_task_completed())
        {
            return to_vtask(timed_push_task(res));
        }
        return to_vtask(tuple.make(!_overtime, tuple.make(res.value1, res.value2, res.value3)));
    }
    /// <summary>
    /// 不安全的异步回调
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action unsafe_async_callback(Action handler)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action)delegate ()
        {
            handler();
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate ()
        {
            handler();
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1> unsafe_async_callback<T1>(Action<T1> handler)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action<T1>)delegate (T1 p1)
        {
            handler(p1);
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate (T1 p1)
        {
            handler(p1);
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1, T2> unsafe_async_callback<T1, T2>(Action<T1, T2> handler)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action<T1, T2>)delegate (T1 p1, T2 p2)
        {
            handler(p1, p2);
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate (T1 p1, T2 p2)
        {
            handler(p1, p2);
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> unsafe_async_callback<T1, T2, T3>(Action<T1, T2, T3> handler)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action<T1, T2, T3>)delegate (T1 p1, T2 p2, T3 p3)
        {
            handler(p1, p2, p3);
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate (T1 p1, T2 p2, T3 p3)
        {
            handler(p1, p2, p3);
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action timed_async_callback(int ms, Action handler, Action timedHandler = null, Action lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate ()
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke();
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler();
                    no_check_next();
                }
                else lostHandler?.Invoke();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler();
                        no_check_next();
                    }
                    else lostHandler?.Invoke();
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action timed_async_callback2(int ms, Action handler, Action timedHandler = null, Action lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate ()
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke();
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler();
                    no_check_next();
                }
                else lostHandler?.Invoke();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler();
                        no_check_next();
                    }
                    else lostHandler?.Invoke();
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1> timed_async_callback<T1>(int ms, Action<T1> handler, Action timedHandler = null, Action<T1> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler(p1);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler(p1);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1);
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1> timed_async_callback2<T1>(int ms, Action<T1> handler, Action timedHandler = null, Action<T1> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler(p1);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler(p1);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1);
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> timed_async_callback<T1, T2>(int ms, Action<T1, T2> handler, Action timedHandler = null, Action<T1, T2> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler(p1, p2);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler(p1, p2);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2);
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> timed_async_callback2<T1, T2>(int ms, Action<T1, T2> handler, Action timedHandler = null, Action<T1, T2> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler(p1, p2);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler(p1, p2);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2);
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> timed_async_callback<T1, T2, T3>(int ms, Action<T1, T2, T3> handler, Action timedHandler = null, Action<T1, T2, T3> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2, T3 p3)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2, p3);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler(p1, p2, p3);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2, p3);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler(p1, p2, p3);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2, p3);
                });
            }
        };
    }
    /// <summary>
    /// 超时的异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="ms"></param>
    /// <param name="handler"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> timed_async_callback2<T1, T2, T3>(int ms, Action<T1, T2, T3> handler, Action timedHandler = null, Action<T1, T2, T3> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2, T3 p3)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2, p3);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    handler(p1, p2, p3);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2, p3);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        handler(p1, p2, p3);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2, p3);
                });
            }
        };
    }
    /// <summary>
    /// 异步回调
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action async_callback(Action handler, Action lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate ()
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke();
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    handler();
                    no_check_next();
                }
                else lostHandler?.Invoke();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        handler();
                        no_check_next();
                    }
                    else lostHandler?.Invoke();
                });
            }
        };
    }
    /// <summary>
    /// 异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="handler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1> async_callback<T1>(Action<T1> handler, Action<T1> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate (T1 p1)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    handler(p1);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        handler(p1);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1);
                });
            }
        };
    }
    /// <summary>
    /// 异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="handler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> async_callback<T1, T2>(Action<T1, T2> handler, Action<T1, T2> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate (T1 p1, T2 p2)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    handler(p1, p2);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        handler(p1, p2);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2);
                });
            }
        };
    }
    /// <summary>
    /// 异步回调
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="handler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> async_callback<T1, T2, T3>(Action<T1, T2, T3> handler, Action<T1, T2, T3> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate (T1 p1, T2 p2, T3 p3)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2, p3);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    handler(p1, p2, p3);
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2, p3);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        handler(p1, p2, p3);
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2, p3);
                });
            }
        };
    }
    /// <summary>
    /// 异步结果
    /// </summary>
    /// <returns></returns>
    private Action _async_result()
    {
        _pullTask.new_task();
        return _beginQuit ? (Action)quit_next : no_quit_next;
    }
    /// <summary>
    /// 不安全的异步结果
    /// </summary>
    /// <returns></returns>
    public Action unsafe_async_result()
    {
        _pullTask.new_task();
        return _beginQuit ? (Action)delegate ()
        {
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate ()
        {
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public Action<T1> unsafe_async_result<T1>(async_result_wrap<T1> res)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action<T1>)delegate (T1 p1)
        {
            res.value1 = p1;
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate (T1 p1)
        {
            res.value1 = p1;
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public Action<T1, T2> unsafe_async_result<T1, T2>(async_result_wrap<T1, T2> res)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action<T1, T2>)delegate (T1 p1, T2 p2)
        {
            res.value1 = p1;
            res.value2 = p2;
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate (T1 p1, T2 p2)
        {
            res.value1 = p1;
            res.value2 = p2;
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> unsafe_async_result<T1, T2, T3>(async_result_wrap<T1, T2, T3> res)
    {
        _pullTask.new_task();
        return _beginQuit ? (Action<T1, T2, T3>)delegate (T1 p1, T2 p2, T3 p3)
        {
            res.value1 = p1;
            res.value2 = p2;
            res.value3 = p3;
            if (strand.running_in_this_thread() && !_mustTick)
            {
                quit_next();
            }
            else
            {
                strand.post(quit_next);
            }
        }
        : delegate (T1 p1, T2 p2, T3 p3)
        {
            res.value1 = p1;
            res.value2 = p2;
            res.value3 = p3;
            if (strand.running_in_this_thread() && !_mustTick)
            {
                no_quit_next();
            }
            else
            {
                strand.post(no_quit_next);
            }
        };
    }
    /// <summary>
    /// 不安全的异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <returns></returns>
    public Action<T1> unsafe_async_ignore<T1>()
    {
        return unsafe_async_result(async_result_ignore_wrap<T1>.value);
    }
    /// <summary>
    /// 不安全的异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <returns></returns>
    public Action<T1, T2> unsafe_async_ignore<T1, T2>()
    {
        return unsafe_async_result(async_result_ignore_wrap<T1, T2>.value);
    }
    /// <summary>
    /// 不安全的异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
    public Action<T1, T2, T3> unsafe_async_ignore<T1, T2, T3>()
    {
        return unsafe_async_result(async_result_ignore_wrap<T1, T2, T3>.value);
    }
    /// <summary>
    /// 异步结果
    /// </summary>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action async_result(Action lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate ()
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke();
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
                else lostHandler?.Invoke();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check())
                    {
                        next(multiCheck.beginQuit);
                    }
                    else lostHandler?.Invoke();
                });
            }
        };
    }
    /// <summary>
    /// 异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="res"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1> async_result<T1>(async_result_wrap<T1> res, Action<T1> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate (T1 p1)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    res.value1 = p1;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        res.value1 = p1;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1);
                });
            }
        };
    }
    /// <summary>
    /// 异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="res"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> async_result<T1, T2>(async_result_wrap<T1, T2> res, Action<T1, T2> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate (T1 p1, T2 p2)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    res.value1 = p1;
                    res.value2 = p2;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        res.value1 = p1;
                        res.value2 = p2;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2);
                });
            }
        };
    }
    /// <summary>
    /// 异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="res"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> async_result<T1, T2, T3>(async_result_wrap<T1, T2, T3> res, Action<T1, T2, T3> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        return delegate (T1 p1, T2 p2, T3 p3)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2, p3);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    res.value1 = p1;
                    res.value2 = p2;
                    res.value3 = p3;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2, p3);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        res.value1 = p1;
                        res.value2 = p2;
                        res.value3 = p3;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2, p3);
                });
            }
        };
    }
    /// <summary>
    /// 异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <returns></returns>
    public Action<T1> async_ignore<T1>()
    {
        return async_result(async_result_ignore_wrap<T1>.value);
    }
    /// <summary>
    /// 异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <returns></returns>
    public Action<T1, T2> async_ignore<T1, T2>()
    {
        return async_result(async_result_ignore_wrap<T1, T2>.value);
    }
    /// <summary>
    /// 异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
    public Action<T1, T2, T3> async_ignore<T1, T2, T3>()
    {
        return async_result(async_result_ignore_wrap<T1, T2, T3>.value);
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action timed_async_result(int ms, Action timedHandler = null, Action lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate ()
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke();
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    no_check_next();
                }
                else lostHandler?.Invoke();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        no_check_next();
                    }
                    else lostHandler?.Invoke();
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="ms"></param>
    /// <param name="res"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1> timed_async_result<T1>(int ms, async_result_wrap<T1> res, Action timedHandler = null, Action<T1> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    res.value1 = p1;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        res.value1 = p1;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1);
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="ms"></param>
    /// <param name="res"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> timed_async_result<T1, T2>(int ms, async_result_wrap<T1, T2> res, Action timedHandler = null, Action<T1, T2> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    res.value1 = p1;
                    res.value2 = p2;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        res.value1 = p1;
                        res.value2 = p2;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2);
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="ms"></param>
    /// <param name="res"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> timed_async_result<T1, T2, T3>(int ms, async_result_wrap<T1, T2, T3> res, Action timedHandler = null, Action<T1, T2, T3> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                if (null != timedHandler)
                {
                    timedHandler();
                }
                else if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2, T3 p3)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2, p3);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    res.value1 = p1;
                    res.value2 = p2;
                    res.value3 = p3;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2, p3);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        res.value1 = p1;
                        res.value2 = p2;
                        res.value3 = p3;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2, p3);
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action timed_async_result2(int ms, Action timedHandler = null, Action lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate ()
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke();
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    no_check_next();
                }
                else lostHandler?.Invoke();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        no_check_next();
                    }
                    else lostHandler?.Invoke();
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="ms"></param>
    /// <param name="res"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1> timed_async_result2<T1>(int ms, async_result_wrap<T1> res, Action timedHandler = null, Action<T1> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    res.value1 = p1;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        res.value1 = p1;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1);
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="ms"></param>
    /// <param name="res"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> timed_async_result2<T1, T2>(int ms, async_result_wrap<T1, T2> res, Action timedHandler = null, Action<T1, T2> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    res.value1 = p1;
                    res.value2 = p2;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        res.value1 = p1;
                        res.value2 = p2;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2);
                });
            }
        };
    }
    /// <summary>
    /// 超时异步结果
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="ms"></param>
    /// <param name="res"></param>
    /// <param name="timedHandler"></param>
    /// <param name="lostHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> timed_async_result2<T1, T2, T3>(int ms, async_result_wrap<T1, T2, T3> res, Action timedHandler = null, Action<T1, T2, T3> lostHandler = null)
    {
        multi_check multiCheck = new_multi_check();
        _overtime = false;
        if (ms >= 0)
        {
            _timer.timeout(ms, delegate ()
            {
                _overtime = true;
                functional.catch_invoke(timedHandler);
                if (!multiCheck.check())
                {
                    next(multiCheck.beginQuit);
                }
            });
        }
        return delegate (T1 p1, T2 p2, T3 p3)
        {
            if (multiCheck.callbacked)
            {
                lostHandler?.Invoke(p1, p2, p3);
                return;
            }
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                {
                    _timer.Cancel();
                    res.value1 = p1;
                    res.value2 = p2;
                    res.value3 = p3;
                    no_check_next();
                }
                else lostHandler?.Invoke(p1, p2, p3);
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!multiCheck.check() && !_isStop && _beginQuit == multiCheck.beginQuit)
                    {
                        _timer.Cancel();
                        res.value1 = p1;
                        res.value2 = p2;
                        res.value3 = p3;
                        no_check_next();
                    }
                    else lostHandler?.Invoke(p1, p2, p3);
                });
            }
        };
    }
    /// <summary>
    /// 超时异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <returns></returns>
    public Action<T1> timed_async_ignore<T1>(int ms, Action timedHandler = null)
    {
        return timed_async_result(ms, async_result_ignore_wrap<T1>.value, timedHandler);
    }
    /// <summary>
    /// 超时异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> timed_async_ignore<T1, T2>(int ms, Action timedHandler = null)
    {
        return timed_async_result(ms, async_result_ignore_wrap<T1, T2>.value, timedHandler);
    }
    /// <summary>
    /// 超时异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> timed_async_ignore<T1, T2, T3>(int ms, Action timedHandler = null)
    {
        return timed_async_result(ms, async_result_ignore_wrap<T1, T2, T3>.value, timedHandler);
    }
    /// <summary>
    /// 超时异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <returns></returns>
    public Action<T1> timed_async_ignore2<T1>(int ms, Action timedHandler = null)
    {
        return timed_async_result2(ms, async_result_ignore_wrap<T1>.value, timedHandler);
    }
    /// <summary>
    /// 超时异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <returns></returns>
    public Action<T1, T2> timed_async_ignore2<T1, T2>(int ms, Action timedHandler = null)
    {
        return timed_async_result2(ms, async_result_ignore_wrap<T1, T2>.value, timedHandler);
    }
    /// <summary>
    /// 超时异步忽略
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="ms"></param>
    /// <param name="timedHandler"></param>
    /// <returns></returns>
    public Action<T1, T2, T3> timed_async_ignore2<T1, T2, T3>(int ms, Action timedHandler = null)
    {
        return timed_async_result2(ms, async_result_ignore_wrap<T1, T2, T3>.value, timedHandler);
    }
    /// <summary>
    /// 微秒睡眠
    /// </summary>
    /// <param name="us"></param>
    /// <returns></returns>
    static public Task usleep(long us)
    {
        if (us > 0)
        {
            generator this_ = self;
            this_._lastTm = us;
            this_._timer.timeout_us(us, this_._async_result());
            return this_.async_wait();
        }
        else if (us < 0)
        {
            return hold();
        }
        else
        {
            return yield();
        }
    }
    /// <summary>
    /// 秒睡眠
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    static public Task sleep(int ms)
    {
        return usleep((long)ms * 1000);
    }
    /// <summary>
    /// 截止时间睡眠(秒)
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    static public Task deadline(long ms)
    {
        generator this_ = self;
        this_._timer.deadline(ms, this_._async_result());
        return this_.async_wait();
    }
    /// <summary>
    /// 截止时间睡眠(微秒)
    /// </summary>
    /// <param name="us"></param>
    /// <returns></returns>
    static public Task udeadline(long us)
    {
        generator this_ = self;
        this_._timer.deadline_us(us, this_._async_result());
        return this_.async_wait();
    }
    /// <summary>
    /// 让出当前协程
    /// </summary>
    /// <returns></returns>
    static public Task yield()
    {
        generator this_ = self;
        this_.strand.post(this_._async_result());
        return this_.async_wait();
    }
    /// <summary>
    /// 让出当前协程
    /// </summary>
    /// <returns></returns>
    static public Task try_yield()
    {
        generator this_ = self;
        if (this_._lastYieldCount == this_._yieldCount)
        {
            this_._lastYieldCount = this_._yieldCount + 1;
            this_.strand.post(this_._async_result());
            return this_.async_wait();
        }
        this_._lastYieldCount = this_._yieldCount;
        return non_async();
    }
    /// <summary>
    /// 让出当前协程
    /// </summary>
    /// <param name="strand"></param>
    /// <returns></returns>
    static public Task yield(shared_strand strand)
    {
        generator this_ = self;
        strand.post(strand == this_.strand ? this_._async_result() : this_.unsafe_async_result());
        return this_.async_wait();
    }
    /// <summary>
    /// 让出当前协程
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    static public Task yield(work_service service)
    {
        generator this_ = self;
        service.post(this_.unsafe_async_result());
        return this_.async_wait();
    }
    /// <summary>
    /// 当前生成器
    /// </summary>
    static public generator self
    {
        get
        {
            shared_strand currStrand = shared_strand.running_strand();
            return null != currStrand ? currStrand.currSelf : null;
        }
    }
    /// <summary>
    /// 当前生成器
    /// </summary>
    /// <returns></returns>
    static public shared_strand self_strand()
    {
        generator this_ = self;
        return null != this_ ? this_.strand : null;
    }
    /// <summary>
    /// 生成器标识
    /// </summary>
    /// <returns></returns>
    static public long self_id()
    {
        generator this_ = self;
        return null != this_ ? this_._id : 0;
    }
    /// <summary>
    /// 父级
    /// </summary>
    /// <returns></returns>
    static public generator self_parent()
    {
        generator this_ = self;
        return this_.parent;
    }
    /// <summary>
    /// 计数
    /// </summary>
    /// <returns></returns>
    static public long self_count()
    {
        generator this_ = self;
        return this_._yieldCount;
    }
    /// <summary>
    /// 定时器
    /// </summary>
    public shared_strand strand
    {
        get
        {
            return _timer.self_strand();
        }
    }
    /// <summary>
    /// 父级
    /// </summary>
    public virtual generator parent
    {
        get
        {
            return null;
        }
    }
    /// <summary>
    /// 子级
    /// </summary>
    public virtual children brothers
    {
        get
        {
            return null;
        }
    }
    /// <summary>
    /// 标识
    /// </summary>
    public long id
    {
        get
        {
            return _id;
        }
    }
    /// <summary>
    /// 计数
    /// </summary>
    public long count
    {
        get
        {
            return _yieldCount;
        }
    }

    static public Task suspend_other(generator otherGen)
    {
        generator this_ = self;
        otherGen.suspend(this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task resume_other(generator otherGen)
    {
        generator this_ = self;
        otherGen.resume(this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task chan_clear(chan_base chan)
    {
        generator this_ = self;
        chan.async_clear(this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task chan_close(chan_base chan, bool isClear = false)
    {
        generator this_ = self;
        chan.async_close(this_.unsafe_async_result(), isClear);
        return this_.async_wait();
    }

    static public Task chan_cancel(chan_base chan, bool isClear = false)
    {
        generator this_ = self;
        chan.async_cancel(this_.unsafe_async_result(), isClear);
        return this_.async_wait();
    }

    private async Task<bool> chan_is_closed_(chan_base chan)
    {
        bool is_closed = false;
        Action continuation = unsafe_async_result();
        chan.self_strand().post(delegate ()
        {
            is_closed = chan.is_closed();
            continuation();
        });
        await push_task();
        return is_closed;
    }

    static public ValueTask<bool> chan_is_closed(chan_base chan)
    {
        generator this_ = self;
        bool is_closed = chan.is_closed();
        if (!is_closed && chan.self_strand() != this_.strand)
        {
            return to_vtask(this_.chan_is_closed_(chan));
        }
        return to_vtask(is_closed);
    }

    static public Task unsafe_chan_is_closed(async_result_wrap<bool> res, chan_base chan)
    {
        generator this_ = self;
        res.value1 = chan.is_closed();
        if (!res.value1 && chan.self_strand() != this_.strand)
        {
            Action continuation = this_.unsafe_async_result();
            chan.self_strand().post(delegate ()
            {
                res.value1 = chan.is_closed();
                continuation();
            });
            return this_.async_wait();
        }
        return non_async();
    }

    private async Task<chan_state> chan_wait_free_(async_result_wrap<chan_state> res, chan_base chan)
    {
        try
        {
            await push_task();
            await unlock_suspend_();
            return res.value1;
        }
        catch (stop_exception)
        {
            chan.async_remove_send_notify(unsafe_async_ignore<chan_state>(), _ioSign);
            await async_wait();
            res.value1 = chan_state.undefined;
            throw;
        }
    }

    private async Task<chan_state> wait_task_state_(Task task, async_result_wrap<chan_state> res)
    {
        await task;
        return res.value1;
    }

    static public ValueTask<chan_state> chan_timed_wait_free(chan_base chan, int ms)
    {
        generator this_ = self;
        Debug.Assert(!this_._ioSign._ntfNode.effect && !this_._ioSign._success, "重叠的 chan_timed_wait_free 操作!");
        async_result_wrap<chan_state> result = new async_result_wrap<chan_state> { value1 = chan_state.undefined };
        this_.lock_suspend_();
        chan.async_append_send_notify(this_.unsafe_async_result(result), this_._ioSign, ms);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_wait_free_(result, chan));
        }
        Task task = this_.unlock_suspend_();
        if (!task.IsCompleted)
        {
            return to_vtask(this_.wait_task_state_(task, result));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_state> chan_wait_free(chan_base chan)
    {
        return chan_timed_wait_free(chan, -1);
    }

    static public ValueTask<chan_state> chan_timed_wait_has(chan_base chan, int ms, broadcast_token token)
    {
        generator this_ = self;
        Debug.Assert(!this_._ioSign._ntfNode.effect && !this_._ioSign._success, "重叠的 chan_timed_wait_has 操作!");
        async_result_wrap<chan_state> result = new async_result_wrap<chan_state> { value1 = chan_state.undefined };
        this_.lock_suspend_();
        chan.async_append_recv_notify(this_.unsafe_async_result(result), token, this_._ioSign, ms);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_wait_free_(result, chan));
        }
        Task task = this_.unlock_suspend_();
        if (!task.IsCompleted)
        {
            return to_vtask(this_.wait_task_state_(task, result));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_state> chan_timed_wait_has(chan_base chan, int ms)
    {
        return chan_timed_wait_has(chan, ms, broadcast_token._defToken);
    }

    static public ValueTask<chan_state> chan_wait_has(chan_base chan, broadcast_token token)
    {
        return chan_timed_wait_has(chan, -1, token);
    }

    static public ValueTask<chan_state> chan_wait_has(chan_base chan)
    {
        return chan_timed_wait_has(chan, -1, broadcast_token._defToken);
    }

    private async Task chan_cancel_wait_()
    {
        await push_task();
        await unlock_suspend_and_stop_();
    }

    static public Task chan_cancel_wait_free(chan_base chan)
    {
        generator this_ = self;
        this_.lock_suspend_and_stop_();
        chan.async_remove_send_notify(this_.unsafe_async_ignore<chan_state>(), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return this_.chan_cancel_wait_();
        }
        return this_.unlock_suspend_and_stop_();
    }

    static public Task chan_cancel_wait_has(chan_base chan)
    {
        generator this_ = self;
        this_.lock_suspend_and_stop_();
        chan.async_remove_recv_notify(this_.unsafe_async_ignore<chan_state>(), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return this_.chan_cancel_wait_();
        }
        return this_.unlock_suspend_and_stop_();
    }

    static public Task unsafe_chan_send<T>(async_result_wrap<chan_send_wrap> res, chan<T> chan, T msg)
    {
        generator this_ = self;
        res.value1 = chan_send_wrap.def;
        chan.async_send(this_.unsafe_async_result(res), msg);
        return this_.async_wait();
    }

    private async Task<chan_send_wrap> chan_send_<T>(async_result_wrap<chan_send_wrap> res, chan<T> chan, T msg, chan_lost_msg<T> lostMsg)
    {
        try
        {
            await push_task();
            return res.value1;
        }
        catch (stop_exception)
        {
            chan.async_remove_send_notify(unsafe_async_ignore<chan_state>(), _ioSign);
            await async_wait();
            if (chan_state.ok != res.value1.state)
            {
                lostMsg?.set(msg);
            }
            res.value1 = chan_send_wrap.def;
            throw;
        }
    }

    static public ValueTask<chan_send_wrap> chan_send<T>(chan<T> chan, T msg, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_send_wrap> result = new async_result_wrap<chan_send_wrap> { value1 = chan_send_wrap.def };
        chan.async_send(this_.unsafe_async_result(result), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_send_(result, chan, msg, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_chan_send(async_result_wrap<chan_send_wrap> res, chan<void_type> chan)
    {
        return unsafe_chan_send(res, chan, default(void_type));
    }

    static public ValueTask<chan_send_wrap> chan_send(chan<void_type> chan, chan_lost_msg<void_type> lostMsg = null)
    {
        return chan_send(chan, default(void_type), lostMsg);
    }

    static public Task unsafe_chan_force_send<T>(async_result_wrap<chan_send_wrap> res, limit_chan<T> chan, T msg, chan_lost_msg<T> outMsg = null)
    {
        generator this_ = self;
        res.value1 = chan_send_wrap.def;
        chan.async_force_send(this_.unsafe_async_callback(delegate (chan_state state, bool hasOut, T freeMsg)
        {
            res.value1 = new chan_send_wrap { state = state };
            if (hasOut)
            {
                outMsg?.set(freeMsg);
            }
        }), msg);
        return this_.async_wait();
    }

    private async Task<chan_send_wrap> chan_force_send_<T>(async_result_wrap<chan_state> res, limit_chan<T> chan, T msg, chan_lost_msg<T> lostMsg)
    {
        try
        {
            await push_task();
            return new chan_send_wrap { state = res.value1 };
        }
        catch (stop_exception)
        {
            chan.self_strand().dispatch(unsafe_async_result());
            await async_wait();
            if (chan_state.ok != res.value1)
            {
                lostMsg?.set(msg);
            }
            res.value1 = chan_state.undefined;
            throw;
        }
    }

    static public ValueTask<chan_send_wrap> chan_force_send<T>(limit_chan<T> chan, T msg, chan_lost_msg<T> outMsg = null, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_state> result = new async_result_wrap<chan_state> { value1 = chan_state.undefined };
        outMsg?.clear();
        chan.async_force_send(this_.unsafe_async_callback(delegate (chan_state state, bool hasOut, T freeMsg)
        {
            result.value1 = state;
            if (hasOut)
            {
                outMsg?.set(freeMsg);
            }
        }), msg);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_force_send_(result, chan, msg, lostMsg));
        }
        return to_vtask(new chan_send_wrap { state = result.value1 });
    }

    static public Task unsafe_chan_timed_force_send<T>(int ms, async_result_wrap<chan_send_wrap> res, limit_chan<T> chan, T msg, chan_lost_msg<T> outMsg = null)
    {
        generator this_ = self;
        res.value1 = chan_send_wrap.def;
        chan.async_timed_force_send(ms, this_.unsafe_async_callback(delegate (chan_state state, bool hasOut, T freeMsg)
        {
            res.value1 = new chan_send_wrap { state = state };
            if (hasOut)
            {
                outMsg?.set(freeMsg);
            }
        }), msg, this_._ioSign);
        return this_.async_wait();
    }

    static public ValueTask<chan_send_wrap> chan_timed_force_send<T>(int ms, limit_chan<T> chan, T msg, chan_lost_msg<T> outMsg = null, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_state> result = new async_result_wrap<chan_state> { value1 = chan_state.undefined };
        outMsg?.clear();
        chan.async_timed_force_send(ms, this_.unsafe_async_callback(delegate (chan_state state, bool hasOut, T freeMsg)
        {
            result.value1 = state;
            if (hasOut)
            {
                outMsg?.set(freeMsg);
            }
        }), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_force_send_(result, chan, msg, lostMsg));
        }
        return to_vtask(new chan_send_wrap { state = result.value1 });
    }

    static public ValueTask<chan_recv_wrap<T>> chan_first<T>(limit_chan<T> chan)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_first(this_.unsafe_async_result(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, null));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_recv_wrap<T>> chan_first<T>(unlimit_chan<T> chan)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_first(this_.unsafe_async_result(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, null));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_recv_wrap<T>> chan_last<T>(limit_chan<T> chan)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_last(this_.unsafe_async_result(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, null));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_recv_wrap<T>> chan_last<T>(unlimit_chan<T> chan)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_last(this_.unsafe_async_result(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, null));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_chan_first<T>(async_result_wrap<chan_recv_wrap<T>> res, limit_chan<T> chan)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_first(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task unsafe_chan_first<T>(async_result_wrap<chan_recv_wrap<T>> res, unlimit_chan<T> chan)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_first(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task unsafe_chan_last<T>(async_result_wrap<chan_recv_wrap<T>> res, limit_chan<T> chan)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_last(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task unsafe_chan_last<T>(async_result_wrap<chan_recv_wrap<T>> res, unlimit_chan<T> chan)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_last(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task unsafe_chan_receive<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan)
    {
        return unsafe_chan_receive(res, chan, broadcast_token._defToken);
    }

    static public ValueTask<chan_recv_wrap<T>> chan_receive<T>(chan<T> chan, chan_lost_msg<T> lostMsg = null)
    {
        return chan_receive(chan, broadcast_token._defToken, lostMsg);
    }

    static public Task unsafe_chan_receive<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan, broadcast_token token)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_recv(this_.unsafe_async_result(res), token);
        return this_.async_wait();
    }

    private async Task<chan_recv_wrap<T>> chan_receive_<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan, chan_lost_msg<T> lostMsg)
    {
        try
        {
            await push_task();
            return res.value1;
        }
        catch (stop_exception)
        {
            chan.async_remove_recv_notify(unsafe_async_ignore<chan_state>(), _ioSign);
            await async_wait();
            if (chan_state.ok == res.value1.state)
            {
                lostMsg?.set(res.value1.msg);
            }
            res.value1 = chan_recv_wrap<T>.def;
            throw;
        }
    }

    static public ValueTask<chan_recv_wrap<T>> chan_receive<T>(chan<T> chan, broadcast_token token, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_recv(this_.unsafe_async_result(result), token, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_chan_try_send<T>(async_result_wrap<chan_send_wrap> res, chan<T> chan, T msg)
    {
        generator this_ = self;
        res.value1 = chan_send_wrap.def;
        chan.async_try_send(this_.unsafe_async_result(res), msg);
        return this_.async_wait();
    }

    static public ValueTask<chan_send_wrap> chan_try_send<T>(chan<T> chan, T msg, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_send_wrap> result = new async_result_wrap<chan_send_wrap> { value1 = chan_send_wrap.def };
        chan.async_try_send(this_.unsafe_async_result(result), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_send_(result, chan, msg, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_chan_try_send(async_result_wrap<chan_send_wrap> res, chan<void_type> chan)
    {
        return unsafe_chan_try_send(res, chan, default(void_type));
    }

    static public ValueTask<chan_send_wrap> chan_try_send(chan<void_type> chan, chan_lost_msg<void_type> lostMsg = null)
    {
        return chan_try_send(chan, default(void_type), lostMsg);
    }

    static public Task unsafe_chan_try_receive<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan)
    {
        return unsafe_chan_try_receive(res, chan, broadcast_token._defToken);
    }

    static public ValueTask<chan_recv_wrap<T>> chan_try_receive<T>(chan<T> chan, chan_lost_msg<T> lostMsg = null)
    {
        return chan_try_receive(chan, broadcast_token._defToken, lostMsg);
    }

    static public Task unsafe_chan_try_receive<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan, broadcast_token token)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_try_recv(this_.unsafe_async_result(res), token);
        return this_.async_wait();
    }

    static public ValueTask<chan_recv_wrap<T>> chan_try_receive<T>(chan<T> chan, broadcast_token token, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_try_recv(this_.unsafe_async_result(result), token, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_chan_timed_send<T>(async_result_wrap<chan_send_wrap> res, chan<T> chan, int ms, T msg)
    {
        generator this_ = self;
        res.value1 = chan_send_wrap.def;
        chan.async_timed_send(ms, this_.unsafe_async_result(res), msg);
        return this_.async_wait();
    }

    static public ValueTask<chan_send_wrap> chan_timed_send<T>(chan<T> chan, int ms, T msg, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_send_wrap> result = new async_result_wrap<chan_send_wrap> { value1 = chan_send_wrap.def };
        chan.async_timed_send(ms, this_.unsafe_async_result(result), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_send_(result, chan, msg, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_chan_timed_send(async_result_wrap<chan_send_wrap> res, chan<void_type> chan, int ms)
    {
        return unsafe_chan_timed_send(res, chan, ms, default(void_type));
    }

    static public ValueTask<chan_send_wrap> chan_timed_send(chan<void_type> chan, int ms, chan_lost_msg<void_type> lostMsg = null)
    {
        return chan_timed_send(chan, ms, default(void_type), lostMsg);
    }

    static public Task unsafe_chan_timed_receive<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan, int ms)
    {
        return unsafe_chan_timed_receive(res, chan, ms, broadcast_token._defToken);
    }

    static public ValueTask<chan_recv_wrap<T>> chan_timed_receive<T>(chan<T> chan, int ms, chan_lost_msg<T> lostMsg = null)
    {
        return chan_timed_receive(chan, ms, broadcast_token._defToken, lostMsg);
    }

    static public Task unsafe_chan_timed_receive<T>(async_result_wrap<chan_recv_wrap<T>> res, chan<T> chan, int ms, broadcast_token token)
    {
        generator this_ = self;
        res.value1 = default(chan_recv_wrap<T>);
        chan.async_timed_recv(ms, this_.unsafe_async_result(res), token);
        return this_.async_wait();
    }

    static public ValueTask<chan_recv_wrap<T>> chan_timed_receive<T>(chan<T> chan, int ms, broadcast_token token, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<chan_recv_wrap<T>> result = new async_result_wrap<chan_recv_wrap<T>> { value1 = chan_recv_wrap<T>.def };
        chan.async_timed_recv(ms, this_.unsafe_async_result(result), token, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.chan_receive_(result, chan, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public Task unsafe_csp_invoke<R, T>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, T> chan, T msg, int invokeMs = -1)
    {
        generator this_ = self;
        res.value1 = csp_invoke_wrap<R>.def;
        chan.async_send(invokeMs, this_.unsafe_async_result(res), msg);
        return this_.async_wait();
    }

    private Action<csp_invoke_wrap<R>> async_csp_invoke<R>(async_result_wrap<csp_invoke_wrap<R>> res, Action<R> lostRes)
    {
        _pullTask.new_task();
        bool beginQuit = _beginQuit;
        return delegate (csp_invoke_wrap<R> cspRes)
        {
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!_isStop && _beginQuit == beginQuit)
                {
                    res.value1 = cspRes;
                    no_check_next();
                }
                else if (chan_state.ok == cspRes.state)
                {
                    functional.catch_invoke(lostRes, cspRes.result);
                }
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!_isStop && _beginQuit == beginQuit)
                    {
                        res.value1 = cspRes;
                        no_check_next();
                    }
                    else if (chan_state.ok == cspRes.state)
                    {
                        functional.catch_invoke(lostRes, cspRes.result);
                    }
                });
            }
        };
    }

    private async Task<csp_invoke_wrap<R>> csp_invoke_<R, T>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, T> chan, T msg, chan_lost_msg<T> lostMsg)
    {
        try
        {
            await push_task();
            return res.value1;
        }
        catch (stop_exception)
        {
            chan_state rmState = chan_state.undefined;
            chan.async_remove_send_notify(null == lostMsg ? unsafe_async_ignore<chan_state>() : unsafe_async_callback((chan_state state) => rmState = state), _ioSign);
            await async_wait();
            if (chan_state.ok == rmState)
            {
                lostMsg?.set(msg);
            }
            res.value1 = csp_invoke_wrap<R>.def;
            throw;
        }
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_invoke<R, T>(csp_chan<R, T> chan, T msg, int invokeMs = -1, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<csp_invoke_wrap<R>> result = new async_result_wrap<csp_invoke_wrap<R>> { value1 = csp_invoke_wrap<R>.def };
        chan.async_send(invokeMs, null == lostRes ? this_.unsafe_async_result(result) : this_.async_csp_invoke(result, lostRes), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.csp_invoke_(result, chan, msg, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static private Action<csp_invoke_wrap<R>> wrap_lost_handler<R>(Action<R> lostRes)
    {
        Action<csp_invoke_wrap<R>> lostHandler = null;
        if (null != lostRes)
        {
            lostHandler = delegate (csp_invoke_wrap<R> lost)
            {
                if (chan_state.ok == lost.state)
                {
                    functional.catch_invoke(lostRes, lost.result);
                }
            };
        }
        return lostHandler;
    }

    static public delay_csp<R> wrap_csp_invoke<R, T>(csp_chan<R, T> chan, T msg, int invokeMs = -1, Action<R> lostRes = null)
    {
        delay_csp<R> result = new delay_csp<R>(wrap_lost_handler(lostRes));
        chan.async_send(invokeMs, (csp_invoke_wrap<R> res) => result.result = res, msg);
        return result;
    }

    static public Task unsafe_csp_invoke<R>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, void_type> chan, int invokeMs = -1)
    {
        return unsafe_csp_invoke(res, chan, default(void_type), invokeMs);
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_invoke<R>(csp_chan<R, void_type> chan, int invokeMs = -1, Action<R> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
    {
        return csp_invoke(chan, default(void_type), invokeMs, lostRes, lostMsg);
    }

    static public delay_csp<R> wrap_csp_invoke<R>(csp_chan<R, void_type> chan, int invokeMs = -1, Action<R> lostRes = null)
    {
        return wrap_csp_invoke(chan, default(void_type), invokeMs, lostRes);
    }

    private Action<csp_wait_wrap<R, T>> async_csp_wait<R, T>(async_result_wrap<csp_wait_wrap<R, T>> res)
    {
        _pullTask.new_task();
        bool beginQuit = _beginQuit;
        return delegate (csp_wait_wrap<R, T> cspRes)
        {
            if (strand.running_in_this_thread() && !_mustTick)
            {
                if (!_isStop && _beginQuit == beginQuit)
                {
                    cspRes.result?.start_invoke_timer(this);
                    res.value1 = cspRes;
                    no_check_next();
                }
                else cspRes.result?.fail();
            }
            else
            {
                strand.post(delegate ()
                {
                    if (!_isStop && _beginQuit == beginQuit)
                    {
                        cspRes.result?.start_invoke_timer(this);
                        res.value1 = cspRes;
                        no_check_next();
                    }
                    else cspRes.result?.fail();
                });
            }
        };
    }

    static public Task unsafe_csp_wait<R, T>(async_result_wrap<csp_wait_wrap<R, T>> res, csp_chan<R, T> chan)
    {
        generator this_ = self;
        res.value1 = csp_wait_wrap<R, T>.def;
        chan.async_recv(this_.async_csp_wait(res));
        return this_.async_wait();
    }

    private async Task<csp_wait_wrap<R, T>> csp_wait_<R, T>(async_result_wrap<csp_wait_wrap<R, T>> res, csp_chan<R, T> chan, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg)
    {
        try
        {
            await push_task();
            return res.value1;
        }
        catch (stop_exception)
        {
            chan.async_remove_recv_notify(unsafe_async_ignore<chan_state>(), _ioSign);
            await async_wait();
            if (chan_state.ok == res.value1.state)
            {
                if (null != lostMsg)
                {
                    lostMsg.set(res.value1);
                }
                else
                {
                    res.value1.fail();
                }
            }
            res.value1 = csp_wait_wrap<R, T>.def;
            throw;
        }
    }

    static public ValueTask<csp_wait_wrap<R, T>> csp_wait<R, T>(csp_chan<R, T> chan, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<csp_wait_wrap<R, T>> result = new async_result_wrap<csp_wait_wrap<R, T>> { value1 = csp_wait_wrap<R, T>.def };
        chan.async_recv(this_.async_csp_wait(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.csp_wait_(result, chan, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public void csp_fail()
    {
        throw new csp_fail_exception();
    }

    static public ValueTask<chan_state> csp_wait<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        return csp_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_wait<R, T>(csp_chan<R, T> chan, Func<T, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        return csp_wait_(chan, null, handler, lostMsg);
    }

    static private async Task<chan_state> csp_wait1_<R, T>(ValueTask<csp_wait_wrap<R, T>> cspTask, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler)
    {
        csp_wait_wrap<R, T> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                result.complete(null != handler ? await handler(result.msg) : await gohandler(result.msg));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private async Task<chan_state> csp_wait2_<R, T>(csp_wait_wrap<R, T> result, ValueTask<R> callTask)
    {
        try
        {
            result.complete(await callTask);
        }
        catch (csp_fail_exception)
        {
            result.fail();
        }
        catch (stop_exception)
        {
            result.fail();
            throw;
        }
        catch (System.Exception)
        {
            result.fail();
            throw;
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<R, T>(ValueTask<csp_wait_wrap<R, T>> cspTask, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler, gohandler));
        }
        csp_wait_wrap<R, T> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                ValueTask<R> callTask = null != handler ? to_vtask(handler(result.msg)) : gohandler(result.msg);
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(callTask.GetAwaiter().GetResult());
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static private ValueTask<chan_state> csp_wait_<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_wait<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_wait<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_wait_(chan, null, handler, lostMsg);
    }

    static private async Task<chan_state> csp_wait1_<R, T1, T2>(ValueTask<csp_wait_wrap<R, tuple<T1, T2>>> cspTask, Func<T1, T2, Task<R>> handler, Func<T1, T2, ValueTask<R>> gohandler)
    {
        csp_wait_wrap<R, tuple<T1, T2>> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                result.complete(null != handler ? await handler(result.msg.value1, result.msg.value2) : await gohandler(result.msg.value1, result.msg.value2));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<R, T1, T2>(ValueTask<csp_wait_wrap<R, tuple<T1, T2>>> cspTask, Func<T1, T2, Task<R>> handler, Func<T1, T2, ValueTask<R>> gohandler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler, gohandler));
        }
        csp_wait_wrap<R, tuple<T1, T2>> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                ValueTask<R> callTask = null != handler ? to_vtask(handler(result.msg.value1, result.msg.value2)) : gohandler(result.msg.value1, result.msg.value2);
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(callTask.GetAwaiter().GetResult());
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static private ValueTask<chan_state> csp_wait_<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, Func<T1, T2, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_wait<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_wait<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_wait_(chan, null, handler, lostMsg);
    }

    static private async Task<chan_state> csp_wait1_<R, T1, T2, T3>(ValueTask<csp_wait_wrap<R, tuple<T1, T2, T3>>> cspTask, Func<T1, T2, T3, Task<R>> handler, Func<T1, T2, T3, ValueTask<R>> gohandler)
    {
        csp_wait_wrap<R, tuple<T1, T2, T3>> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                result.complete(null != handler ? await handler(result.msg.value1, result.msg.value2, result.msg.value3) : await gohandler(result.msg.value1, result.msg.value2, result.msg.value3));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<R, T1, T2, T3>(ValueTask<csp_wait_wrap<R, tuple<T1, T2, T3>>> cspTask, Func<T1, T2, T3, Task<R>> handler, Func<T1, T2, T3, ValueTask<R>> gohandler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler, gohandler));
        }
        csp_wait_wrap<R, tuple<T1, T2, T3>> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                ValueTask<R> callTask = null != handler ? to_vtask(handler(result.msg.value1, result.msg.value2, result.msg.value3)) : gohandler(result.msg.value1, result.msg.value2, result.msg.value3);
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(callTask.GetAwaiter().GetResult());
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static private ValueTask<chan_state> csp_wait_<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, Func<T1, T2, T3, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_wait<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
    {
        return csp_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_wait<R>(csp_chan<R, void_type> chan, Func<ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
    {
        return csp_wait_(chan, null, handler, lostMsg);
    }

    static private async Task<chan_state> csp_wait1_<R>(ValueTask<csp_wait_wrap<R, void_type>> cspTask, Func<Task<R>> handler, Func<ValueTask<R>> gohandler)
    {
        csp_wait_wrap<R, void_type> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                result.complete(null != handler ? await handler() : await gohandler());
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<R>(ValueTask<csp_wait_wrap<R, void_type>> cspTask, Func<Task<R>> handler, Func<ValueTask<R>> gohandler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler, gohandler));
        }
        csp_wait_wrap<R, void_type> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                ValueTask<R> callTask = null != handler ? to_vtask(handler()) : gohandler();
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(callTask.GetAwaiter().GetResult());
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static private ValueTask<chan_state> csp_wait_<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, Func<ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler, gohandler);
    }

    static private async Task<chan_state> csp_wait1_<T>(ValueTask<csp_wait_wrap<void_type, T>> cspTask, Func<T, Task> handler)
    {
        csp_wait_wrap<void_type, T> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                await handler(result.msg);
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private async Task<chan_state> csp_wait2_<T>(csp_wait_wrap<void_type, T> result, Task callTask)
    {
        try
        {
            await callTask;
            result.complete(default(void_type));
        }
        catch (csp_fail_exception)
        {
            result.fail();
        }
        catch (stop_exception)
        {
            result.fail();
            throw;
        }
        catch (System.Exception)
        {
            result.fail();
            throw;
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<T>(ValueTask<csp_wait_wrap<void_type, T>> cspTask, Func<T, Task> handler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler));
        }
        csp_wait_wrap<void_type, T> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                Task callTask = handler(result.msg);
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static public ValueTask<chan_state> csp_wait<T>(csp_chan<void_type, T> chan, Func<T, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler);
    }

    static private async Task<chan_state> csp_wait1_<T1, T2>(ValueTask<csp_wait_wrap<void_type, tuple<T1, T2>>> cspTask, Func<T1, T2, Task> handler)
    {
        csp_wait_wrap<void_type, tuple<T1, T2>> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                await handler(result.msg.value1, result.msg.value2);
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<T1, T2>(ValueTask<csp_wait_wrap<void_type, tuple<T1, T2>>> cspTask, Func<T1, T2, Task> handler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler));
        }
        csp_wait_wrap<void_type, tuple<T1, T2>> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                Task callTask = handler(result.msg.value1, result.msg.value2);
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static public ValueTask<chan_state> csp_wait<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, Func<T1, T2, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler);
    }

    static private async Task<chan_state> csp_wait1_<T1, T2, T3>(ValueTask<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> cspTask, Func<T1, T2, T3, Task> handler)
    {
        csp_wait_wrap<void_type, tuple<T1, T2, T3>> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                await handler(result.msg.value1, result.msg.value2, result.msg.value3);
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_<T1, T2, T3>(ValueTask<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> cspTask, Func<T1, T2, T3, Task> handler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler));
        }
        csp_wait_wrap<void_type, tuple<T1, T2, T3>> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                Task callTask = handler(result.msg.value1, result.msg.value2, result.msg.value3);
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static public ValueTask<chan_state> csp_wait<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler);
    }

    static private async Task<chan_state> csp_wait1_(ValueTask<csp_wait_wrap<void_type, void_type>> cspTask, Func<Task> handler)
    {
        csp_wait_wrap<void_type, void_type> result = await cspTask;
        if (chan_state.ok == result.state)
        {
            try
            {
                await handler();
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return result.state;
    }

    static private ValueTask<chan_state> csp_wait3_(ValueTask<csp_wait_wrap<void_type, void_type>> cspTask, Func<Task> handler)
    {
        if (!cspTask.IsCompleted)
        {
            return to_vtask(csp_wait1_(cspTask, handler));
        }
        csp_wait_wrap<void_type, void_type> result = cspTask.GetAwaiter().GetResult();
        if (chan_state.ok == result.state)
        {
            try
            {
                Task callTask = handler();
                if (!callTask.IsCompleted)
                {
                    return to_vtask(csp_wait2_(result, callTask));
                }
                result.complete(default(void_type));
            }
            catch (csp_fail_exception)
            {
                result.fail();
            }
            catch (stop_exception)
            {
                result.fail();
                throw;
            }
            catch (System.Exception)
            {
                result.fail();
                throw;
            }
        }
        return to_vtask(result.state);
    }

    static public ValueTask<chan_state> csp_wait(csp_chan<void_type, void_type> chan, Func<Task> handler, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
    {
        return csp_wait3_(csp_wait(chan, lostMsg), handler);
    }

    static public Task unsafe_csp_try_invoke<R, T>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, T> chan, T msg, int invokeMs = -1)
    {
        generator this_ = self;
        res.value1 = csp_invoke_wrap<R>.def;
        chan.async_try_send(invokeMs, this_.unsafe_async_result(res), msg);
        return this_.async_wait();
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_try_invoke<R, T>(csp_chan<R, T> chan, T msg, int invokeMs = -1, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<csp_invoke_wrap<R>> result = new async_result_wrap<csp_invoke_wrap<R>> { value1 = csp_invoke_wrap<R>.def };
        chan.async_try_send(invokeMs, null == lostRes ? this_.unsafe_async_result(result) : this_.async_csp_invoke(result, lostRes), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.csp_invoke_(result, chan, msg, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public delay_csp<R> wrap_csp_try_invoke<R, T>(csp_chan<R, T> chan, T msg, int invokeMs = -1, Action<R> lostRes = null)
    {
        delay_csp<R> result = new delay_csp<R>(wrap_lost_handler(lostRes));
        chan.async_try_send(invokeMs, (csp_invoke_wrap<R> res) => result.result = res, msg);
        return result;
    }

    static public Task unsafe_csp_try_invoke<R>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, void_type> chan, int invokeMs = -1)
    {
        return unsafe_csp_try_invoke(res, chan, default(void_type), invokeMs);
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_try_invoke<R>(csp_chan<R, void_type> chan, int invokeMs = -1, Action<R> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
    {
        return csp_try_invoke(chan, default(void_type), invokeMs, lostRes, lostMsg);
    }

    static public delay_csp<R> wrap_csp_try_invoke<R>(csp_chan<R, void_type> chan, int invokeMs = -1, Action<R> lostRes = null)
    {
        return wrap_csp_try_invoke(chan, default(void_type), invokeMs, lostRes);
    }

    static public Task unsafe_csp_try_wait<R, T>(async_result_wrap<csp_wait_wrap<R, T>> res, csp_chan<R, T> chan)
    {
        generator this_ = self;
        res.value1 = csp_wait_wrap<R, T>.def;
        chan.async_try_recv(this_.async_csp_wait(res));
        return this_.async_wait();
    }

    static public ValueTask<csp_wait_wrap<R, T>> csp_try_wait<R, T>(csp_chan<R, T> chan, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<csp_wait_wrap<R, T>> result = new async_result_wrap<csp_wait_wrap<R, T>> { value1 = csp_wait_wrap<R, T>.def };
        chan.async_try_recv(this_.async_csp_wait(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.csp_wait_(result, chan, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_state> csp_try_wait<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        return csp_try_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_try_wait<R, T>(csp_chan<R, T> chan, Func<T, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        return csp_try_wait_(chan, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_try_wait_<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_try_wait<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_try_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_try_wait<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_try_wait_(chan, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_try_wait_<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, Func<T1, T2, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_try_wait<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_try_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_try_wait<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_try_wait_(chan, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_try_wait_<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, Func<T1, T2, T3, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_try_wait<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
    {
        return csp_try_wait_(chan, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_try_wait<R>(csp_chan<R, void_type> chan, Func<ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
    {
        return csp_try_wait_(chan, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_try_wait_<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, Func<ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_try_wait<T>(csp_chan<void_type, T> chan, Func<T, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler);
    }

    static public ValueTask<chan_state> csp_try_wait<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, Func<T1, T2, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler);
    }

    static public ValueTask<chan_state> csp_try_wait<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler);
    }

    static public ValueTask<chan_state> csp_try_wait(csp_chan<void_type, void_type> chan, Func<Task> handler, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
    {
        return csp_wait3_(csp_try_wait(chan, lostMsg), handler);
    }

    static public Task unsafe_csp_timed_invoke<R, T>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, T> chan, tuple<int, int> ms, T msg)
    {
        generator this_ = self;
        res.value1 = csp_invoke_wrap<R>.def;
        chan.async_timed_send(ms.value1, ms.value2, this_.unsafe_async_result(res), msg);
        return this_.async_wait();
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_timed_invoke<R, T>(csp_chan<R, T> chan, tuple<int, int> ms, T msg, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<csp_invoke_wrap<R>> result = new async_result_wrap<csp_invoke_wrap<R>> { value1 = csp_invoke_wrap<R>.def };
        chan.async_timed_send(ms.value1, ms.value2, null == lostRes ? this_.unsafe_async_result(result) : this_.async_csp_invoke(result, lostRes), msg, this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.csp_invoke_(result, chan, msg, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public delay_csp<R> wrap_csp_timed_invoke<R, T>(csp_chan<R, T> chan, tuple<int, int> ms, T msg, Action<R> lostRes = null)
    {
        delay_csp<R> result = new delay_csp<R>(wrap_lost_handler(lostRes));
        chan.async_timed_send(ms.value1, ms.value2, (csp_invoke_wrap<R> res) => result.result = res, msg);
        return result;
    }

    static public Task unsafe_csp_timed_invoke<R, T>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, T> chan, int ms, T msg)
    {
        return unsafe_csp_timed_invoke(res, chan, tuple.make(ms, -1), msg);
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_timed_invoke<R, T>(csp_chan<R, T> chan, int ms, T msg, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
    {
        return csp_timed_invoke(chan, tuple.make(ms, -1), msg, lostRes, lostMsg);
    }

    static public delay_csp<R> wrap_csp_timed_invoke<R, T>(csp_chan<R, T> chan, int ms, T msg, Action<R> lostRes = null)
    {
        return wrap_csp_timed_invoke(chan, tuple.make(ms, -1), msg, lostRes);
    }

    static public Task unsafe_csp_timed_invoke<R>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, void_type> chan, tuple<int, int> ms)
    {
        return unsafe_csp_timed_invoke(res, chan, ms, default(void_type));
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_timed_invoke<R>(csp_chan<R, void_type> chan, tuple<int, int> ms, Action<R> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
    {
        return csp_timed_invoke(chan, ms, default(void_type), lostRes, lostMsg);
    }

    static public delay_csp<R> wrap_csp_timed_invoke<R>(csp_chan<R, void_type> chan, tuple<int, int> ms, Action<R> lostRes = null)
    {
        return wrap_csp_timed_invoke(chan, ms, default(void_type), lostRes);
    }

    static public Task unsafe_csp_timed_invoke<R>(async_result_wrap<csp_invoke_wrap<R>> res, csp_chan<R, void_type> chan, int ms)
    {
        return unsafe_csp_timed_invoke(res, chan, ms, default(void_type));
    }

    static public ValueTask<csp_invoke_wrap<R>> csp_timed_invoke<R>(csp_chan<R, void_type> chan, int ms, Action<R> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
    {
        return csp_timed_invoke(chan, ms, default(void_type), lostRes, lostMsg);
    }

    static public delay_csp<R> wrap_csp_timed_invoke<R>(csp_chan<R, void_type> chan, int ms, Action<R> lostRes = null)
    {
        return wrap_csp_timed_invoke(chan, ms, default(void_type), lostRes);
    }

    static public Task unsafe_csp_timed_wait<R, T>(async_result_wrap<csp_wait_wrap<R, T>> res, csp_chan<R, T> chan, int ms)
    {
        generator this_ = self;
        res.value1 = csp_wait_wrap<R, T>.def;
        chan.async_timed_recv(ms, this_.async_csp_wait(res));
        return this_.async_wait();
    }

    static public ValueTask<csp_wait_wrap<R, T>> csp_timed_wait<R, T>(csp_chan<R, T> chan, int ms, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        generator this_ = self;
        async_result_wrap<csp_wait_wrap<R, T>> result = new async_result_wrap<csp_wait_wrap<R, T>> { value1 = csp_wait_wrap<R, T>.def };
        chan.async_timed_recv(ms, this_.async_csp_wait(result), this_._ioSign);
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.csp_wait_(result, chan, lostMsg));
        }
        return to_vtask(result.value1);
    }

    static public ValueTask<chan_state> csp_timed_wait<R, T>(csp_chan<R, T> chan, int ms, Func<T, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_timed_wait<R, T>(csp_chan<R, T> chan, int ms, Func<T, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_timed_wait_<R, T>(csp_chan<R, T> chan, int ms, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_timed_wait<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_timed_wait<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_timed_wait_<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task<R>> handler, Func<T1, T2, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_timed_wait<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_timed_wait<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_timed_wait_<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task<R>> handler, Func<T1, T2, T3, ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_timed_wait<R>(csp_chan<R, void_type> chan, int ms, Func<Task<R>> handler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, handler, null, lostMsg);
    }

    static public ValueTask<chan_state> csp_timed_wait<R>(csp_chan<R, void_type> chan, int ms, Func<ValueTask<R>> handler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
    {
        return csp_timed_wait_(chan, ms, null, handler, lostMsg);
    }

    static private ValueTask<chan_state> csp_timed_wait_<R>(csp_chan<R, void_type> chan, int ms, Func<Task<R>> handler, Func<ValueTask<R>> gohandler, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler, gohandler);
    }

    static public ValueTask<chan_state> csp_timed_wait<T>(csp_chan<void_type, T> chan, int ms, Func<T, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler);
    }

    static public ValueTask<chan_state> csp_timed_wait<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler);
    }

    static public ValueTask<chan_state> csp_timed_wait<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler);
    }

    static public ValueTask<chan_state> csp_timed_wait(csp_chan<void_type, void_type> chan, int ms, Func<Task> handler, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
    {
        return csp_wait3_(csp_timed_wait(chan, ms, lostMsg), handler);
    }

    static public async Task<int> chans_broadcast<T>(T msg, params chan<T>[] chans)
    {
        generator this_ = self;
        int count = 0;
        wait_group wg = new wait_group(chans.Length);
        for (int i = 0; i < chans.Length; i++)
        {
            chans[i].async_send(delegate (chan_send_wrap sendRes)
            {
                if (chan_state.ok == sendRes.state)
                {
                    Interlocked.Increment(ref count);
                }
                wg.done();
            }, msg, null);
        }
        wait_group.cancel_token cancelToken = wg.async_wait(this_.unsafe_async_result());
        try
        {
            await this_.async_wait();
            return count;
        }
        finally
        {
            wg.cancel_wait(cancelToken);
        }
    }

    static public async Task<int> chans_try_broadcast<T>(T msg, params chan<T>[] chans)
    {
        generator this_ = self;
        int count = 0;
        wait_group wg = new wait_group(chans.Length);
        for (int i = 0; i < chans.Length; i++)
        {
            chans[i].async_try_send(delegate (chan_send_wrap sendRes)
            {
                if (chan_state.ok == sendRes.state)
                {
                    Interlocked.Increment(ref count);
                }
                wg.done();
            }, msg, null);
        }
        wait_group.cancel_token cancelToken = wg.async_wait(this_.unsafe_async_result());
        try
        {
            await this_.async_wait();
            return count;
        }
        finally
        {
            wg.cancel_wait(cancelToken);
        }
    }

    static public async Task<int> chans_timed_broadcast<T>(int ms, T msg, params chan<T>[] chans)
    {
        generator this_ = self;
        int count = 0;
        wait_group wg = new wait_group(chans.Length);
        for (int i = 0; i < chans.Length; i++)
        {
            chans[i].async_timed_send(ms, delegate (chan_send_wrap sendRes)
            {
                if (chan_state.ok == sendRes.state)
                {
                    Interlocked.Increment(ref count);
                }
                wg.done();
            }, msg, null);
        }
        wait_group.cancel_token cancelToken = wg.async_wait(this_.unsafe_async_result());
        try
        {
            await this_.async_wait();
            return count;
        }
        finally
        {
            wg.cancel_wait(cancelToken);
        }
    }

    static public Task non_async()
    {
        return _nilTask;
    }

    static public ValueTask<R> non_async<R>(R value)
    {
        return to_vtask(value);
    }

    static public Task mutex_cancel(go_mutex mtx)
    {
        generator this_ = self;
        mtx.async_cancel(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_lock(go_mutex mtx)
    {
        generator this_ = self;
        mtx.async_lock(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_try_lock(async_result_wrap<bool> res, go_mutex mtx)
    {
        generator this_ = self;
        res.value1 = false;
        mtx.async_try_lock(this_._id, this_.async_result(res));
        return this_.async_wait();
    }

    static public ValueTask<bool> mutex_try_lock(go_mutex mtx)
    {
        generator this_ = self;
        async_result_wrap<bool> res = new async_result_wrap<bool>();
        mtx.async_try_lock(this_._id, this_.unsafe_async_result(res));
        return this_.async_wait(res);
    }

    static public Task mutex_timed_lock(async_result_wrap<bool> res, go_mutex mtx, int ms)
    {
        generator this_ = self;
        res.value1 = false;
        mtx.async_timed_lock(this_._id, ms, this_.async_result(res));
        return this_.async_wait();
    }

    static public ValueTask<bool> mutex_timed_lock(go_mutex mtx, int ms)
    {
        generator this_ = self;
        async_result_wrap<bool> res = new async_result_wrap<bool>();
        mtx.async_timed_lock(this_._id, ms, this_.unsafe_async_result(res));
        return this_.async_wait(res);
    }

    static public Task mutex_unlock(go_mutex mtx)
    {
        generator this_ = self;
        mtx.async_unlock(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_lock_shared(go_shared_mutex mtx)
    {
        generator this_ = self;
        mtx.async_lock_shared(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_lock_pess_shared(go_shared_mutex mtx)
    {
        generator this_ = self;
        mtx.async_lock_pess_shared(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_lock_upgrade(go_shared_mutex mtx)
    {
        generator this_ = self;
        mtx.async_lock_upgrade(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_try_lock_shared(async_result_wrap<bool> res, go_shared_mutex mtx)
    {
        generator this_ = self;
        res.value1 = false;
        mtx.async_try_lock_shared(this_._id, this_.async_result(res));
        return this_.async_wait();
    }

    static public ValueTask<bool> mutex_try_lock_shared(go_shared_mutex mtx)
    {
        generator this_ = self;
        async_result_wrap<bool> res = new async_result_wrap<bool>();
        mtx.async_try_lock_shared(this_._id, this_.unsafe_async_result(res));
        return this_.async_wait(res);
    }

    static public Task mutex_try_lock_upgrade(async_result_wrap<bool> res, go_shared_mutex mtx)
    {
        generator this_ = self;
        res.value1 = false;
        mtx.async_try_lock_upgrade(this_._id, this_.async_result(res));
        return this_.async_wait();
    }

    static public ValueTask<bool> mutex_try_lock_upgrade(go_shared_mutex mtx)
    {
        generator this_ = self;
        async_result_wrap<bool> res = new async_result_wrap<bool>();
        mtx.async_try_lock_upgrade(this_._id, this_.unsafe_async_result(res));
        return this_.async_wait(res);
    }

    static public Task mutex_timed_lock_shared(async_result_wrap<bool> res, go_shared_mutex mtx, int ms)
    {
        generator this_ = self;
        res.value1 = false;
        mtx.async_timed_lock_shared(this_._id, ms, this_.async_result(res));
        return this_.async_wait();
    }

    static public ValueTask<bool> mutex_timed_lock_shared(go_shared_mutex mtx, int ms)
    {
        generator this_ = self;
        async_result_wrap<bool> res = new async_result_wrap<bool>();
        mtx.async_timed_lock_shared(this_._id, ms, this_.unsafe_async_result(res));
        return this_.async_wait(res);
    }

    static public Task mutex_unlock_shared(go_shared_mutex mtx)
    {
        generator this_ = self;
        mtx.async_unlock_shared(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task mutex_unlock_upgrade(go_shared_mutex mtx)
    {
        generator this_ = self;
        mtx.async_unlock_upgrade(this_._id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task condition_wait(go_condition_variable conVar, go_mutex mutex)
    {
        generator this_ = self;
        conVar.async_wait(this_._id, mutex, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task condition_timed_wait(async_result_wrap<bool> res, go_condition_variable conVar, go_mutex mutex, int ms)
    {
        generator this_ = self;
        res.value1 = false;
        conVar.async_timed_wait(this_._id, ms, mutex, this_.async_result(res));
        return this_.async_wait();
    }

    static public ValueTask<bool> condition_timed_wait(go_condition_variable conVar, go_mutex mutex, int ms)
    {
        generator this_ = self;
        async_result_wrap<bool> res = new async_result_wrap<bool>();
        conVar.async_timed_wait(this_._id, ms, mutex, this_.unsafe_async_result(res));
        return this_.async_wait(res);
    }

    static public Task condition_cancel(go_condition_variable conVar)
    {
        generator this_ = self;
        conVar.async_cancel(this_.id, this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task unsafe_send_strand(shared_strand strand, Action handler)
    {
        generator this_ = self;
        if (this_.strand == strand)
        {
            handler();
            return non_async();
        }
        strand.post(this_.unsafe_async_callback(handler));
        return this_.async_wait();
    }

    static public Task unsafe_force_send_strand(shared_strand strand, Action handler)
    {
        generator this_ = self;
        strand.post(this_.unsafe_async_callback(handler));
        return this_.async_wait();
    }

    private async Task send_strand_(shared_strand strand, Action handler)
    {
        System.Exception hasExcep = null;
        strand.post(unsafe_async_callback(delegate ()
        {
            try
            {
                handler();
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }));
        await async_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
    }

    static public Task send_strand(shared_strand strand, Action handler)
    {
        generator this_ = self;
        if (this_.strand == strand)
        {
            handler();
            return non_async();
        }
        return this_.send_strand_(strand, handler);
    }

    static public Task force_send_strand(shared_strand strand, Action handler)
    {
        generator this_ = self;
        return this_.send_strand_(strand, handler);
    }

    static public Task unsafe_send_strand<R>(async_result_wrap<R> res, shared_strand strand, Func<R> handler)
    {
        generator this_ = self;
        if (this_.strand == strand)
        {
            res.value1 = handler();
            return non_async();
        }
        res.clear();
        strand.post(this_.unsafe_async_callback(delegate ()
        {
            res.value1 = handler();
        }));
        return this_.async_wait();
    }

    static public Task unsafe_force_send_strand<R>(async_result_wrap<R> res, shared_strand strand, Func<R> handler)
    {
        generator this_ = self;
        res.clear();
        strand.post(this_.unsafe_async_callback(delegate ()
        {
            res.value1 = handler();
        }));
        return this_.async_wait();
    }

    private async Task<R> send_strand_<R>(shared_strand strand, Func<R> handler)
    {
        R res = default(R);
        System.Exception hasExcep = null;
        strand.post(unsafe_async_callback(delegate ()
        {
            try
            {
                res = handler();
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }));
        await async_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
        return res;
    }

    static public ValueTask<R> send_strand<R>(shared_strand strand, Func<R> handler)
    {
        generator this_ = self;
        if (this_.strand == strand)
        {
            return to_vtask(handler());
        }
        return to_vtask(this_.send_strand_(strand, handler));
    }

    static public Task<R> force_send_strand<R>(shared_strand strand, Func<R> handler)
    {
        generator this_ = self;
        return this_.send_strand_(strand, handler);
    }

    static public Func<Task> wrap_send_strand(shared_strand strand, Action handler)
    {
        return () => send_strand(strand, handler);
    }

    static public Func<Task> wrap_force_send_strand(shared_strand strand, Action handler)
    {
        return () => force_send_strand(strand, handler);
    }

    static public Func<T, Task> wrap_send_strand<T>(shared_strand strand, Action<T> handler)
    {
        return (T p) => send_strand(strand, () => handler(p));
    }

    static public Func<T, Task> wrap_force_send_strand<T>(shared_strand strand, Action<T> handler)
    {
        return (T p) => force_send_strand(strand, () => handler(p));
    }

    static public Func<ValueTask<R>> wrap_send_strand<R>(shared_strand strand, Func<R> handler)
    {
        return delegate ()
        {
            return send_strand(strand, handler);
        };
    }

    static public Func<Task<R>> wrap_force_send_strand<R>(shared_strand strand, Func<R> handler)
    {
        return delegate ()
        {
            return force_send_strand(strand, handler);
        };
    }

    static public Func<T, ValueTask<R>> wrap_send_strand<R, T>(shared_strand strand, Func<T, R> handler)
    {
        return delegate (T p)
        {
            return send_strand(strand, () => handler(p));
        };
    }

    static public Func<T, Task<R>> wrap_force_send_strand<R, T>(shared_strand strand, Func<T, R> handler)
    {
        return delegate (T p)
        {
            return force_send_strand(strand, () => handler(p));
        };
    }

    static public Task unsafe_send_service(work_service service, Action handler)
    {
        generator this_ = self;
        service.post(this_.unsafe_async_callback(handler));
        return this_.async_wait();
    }

    private async Task send_service_(work_service service, Action handler)
    {
        System.Exception hasExcep = null;
        service.post(unsafe_async_callback(delegate ()
        {
            try
            {
                handler();
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }));
        await async_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
    }

    static public Task send_service(work_service service, Action handler)
    {
        generator this_ = self;
        return this_.send_service_(service, handler);
    }

    static public Task unsafe_send_service<R>(async_result_wrap<R> res, work_service service, Func<R> handler)
    {
        generator this_ = self;
        res.clear();
        service.post(this_.unsafe_async_callback(delegate ()
        {
            res.value1 = handler();
        }));
        return this_.async_wait();
    }

    private async Task<R> send_service_<R>(work_service service, Func<R> handler)
    {
        R res = default(R);
        System.Exception hasExcep = null;
        service.post(unsafe_async_callback(delegate ()
        {
            try
            {
                res = handler();
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
        }));
        await async_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
        return res;
    }

    static public Task<R> send_service<R>(work_service service, Func<R> handler)
    {
        generator this_ = self;
        return this_.send_service_(service, handler);
    }

    static public Func<Task> wrap_send_service(work_service service, Action handler)
    {
        return () => send_service(service, handler);
    }

    static public Func<T, Task> wrap_send_service<T>(work_service service, Action<T> handler)
    {
        return (T p) => send_service(service, () => handler(p));
    }

    static public Func<Task<R>> wrap_send_service<R>(work_service service, Func<R> handler)
    {
        return delegate ()
        {
            return send_service(service, handler);
        };
    }

    static public Func<T, Task<R>> wrap_send_service<R, T>(work_service service, Func<T, R> handler)
    {
        return delegate (T p)
        {
            return send_service(service, () => handler(p));
        };
    }

    static public Task unsafe_send_task(Action handler)
    {
        generator this_ = self;
        this_._pullTask.new_task();
        bool beginQuit = this_._beginQuit;
        Task _ = Task.Run(delegate ()
        {
            handler();
            this_.strand.post(beginQuit ? (Action)this_.quit_next : this_.no_quit_next);
        });
        return this_.async_wait();
    }

    static public async Task send_task(Action handler)
    {
        generator this_ = self;
        System.Exception hasExcep = null;
        this_._pullTask.new_task();
        bool beginQuit = this_._beginQuit;
        Task _ = Task.Run(delegate ()
        {
            try
            {
                handler();
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
            this_.strand.post(beginQuit ? (Action)this_.quit_next : this_.no_quit_next);
        });
        await this_.async_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
    }

    static public Task unsafe_send_task<R>(async_result_wrap<R> res, Func<R> handler)
    {
        generator this_ = self;
        this_._pullTask.new_task();
        bool beginQuit = this_._beginQuit;
        Task _ = Task.Run(delegate ()
        {
            res.value1 = handler();
            this_.strand.post(beginQuit ? (Action)this_.quit_next : this_.no_quit_next);
        });
        return this_.async_wait();
    }

    static public async Task<R> send_task<R>(Func<R> handler)
    {
        generator this_ = self;
        R res = default(R);
        System.Exception hasExcep = null;
        this_._pullTask.new_task();
        bool beginQuit = this_._beginQuit;
        Task _ = Task.Run(delegate ()
        {
            try
            {
                res = handler();
            }
            catch (System.Exception ec)
            {
                ec.Source = string.Format("{0}\n{1}", ec.Source, ec.StackTrace);
                hasExcep = ec;
            }
            this_.strand.post(beginQuit ? (Action)this_.quit_next : this_.no_quit_next);
        });
        await this_.async_wait();
        if (null != hasExcep)
        {
            throw hasExcep;
        }
        return res;
    }

    static public Func<Task> wrap_send_task(Action handler)
    {
        return () => send_task(handler);
    }

    static public Func<T, Task> wrap_send_task<T>(Action<T> handler)
    {
        return (T p) => send_task(() => handler(p));
    }

    static public Func<Task<R>> wrap_send_task<R>(Func<R> handler)
    {
        return async delegate ()
        {
            R res = default(R);
            await send_task(() => res = handler());
            return res;
        };
    }

    static public Func<T, Task<R>> wrap_send_task<R, T>(Func<T, R> handler)
    {
        return async delegate (T p)
        {
            R res = default(R);
            await send_task(() => res = handler(p));
            return res;
        };
    }

    static private void_type check_task(Task task)
    {
        task.GetAwaiter().GetResult();
        return default(void_type);
    }

    static private R check_task<R>(Task<R> task)
    {
        return task.GetAwaiter().GetResult();
    }

    static private csp_invoke_wrap<R> check_csp<R>(delay_csp<R> task)
    {
        return task.result;
    }

    static private void check_task(async_result_wrap<bool, Exception> res, Task task)
    {
        try
        {
            res.value1 = true;
            check_task(task);
        }
        catch (Exception innerEc)
        {
            res.value2 = innerEc;
        }
    }

    static private void check_task<R>(async_result_wrap<R, Exception> res, Task<R> task)
    {
        try
        {
            res.value1 = check_task(task);
        }
        catch (Exception innerEc)
        {
            res.value2 = innerEc;
        }
    }

    static private void check_task<R>(async_result_wrap<bool, R, Exception> res, Task<R> task)
    {
        try
        {
            res.value1 = true;
            res.value2 = check_task(task);
        }
        catch (Exception innerEc)
        {
            res.value3 = innerEc;
        }
    }

    private async Task<void_type> wait_task_(Task task)
    {
        await async_wait();
        return check_task(task);
    }

    static public ValueTask<void_type> wait_task(Task task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            task.GetAwaiter().UnsafeOnCompleted(this_.unsafe_async_result());
            return to_vtask(this_.wait_task_(task));
        }
        return to_vtask(check_task(task));
    }

    private async Task<R> wait_task_<R>(Task<R> task)
    {
        await async_wait();
        return check_task(task);
    }

    private async Task<csp_invoke_wrap<R>> wait_csp_<R>(delay_csp<R> task)
    {
        await async_wait();
        return check_csp(task);
    }

    static public ValueTask<R> wait_task<R>(Task<R> task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            task.GetAwaiter().UnsafeOnCompleted(this_.unsafe_async_result());
            return to_vtask(this_.wait_task_(task));
        }
        return to_vtask(check_task(task));
    }

    static public ValueTask<csp_invoke_wrap<R>> wait_csp<R>(delay_csp<R> task)
    {
        if (!task.is_completed)
        {
            generator this_ = self;
            task.on_completed(this_.unsafe_async_result());
            return to_vtask(this_.wait_csp_(task));
        }
        return to_vtask(check_csp(task));
    }

    private async Task<bool> timed_wait_task_(Task task)
    {
        await async_wait();
        if (!_overtime)
        {
            check_task(task);
        }
        return !_overtime;
    }

    static public ValueTask<bool> timed_wait_task(int ms, Task task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            task.GetAwaiter().UnsafeOnCompleted(this_.timed_async_result(ms));
            return to_vtask(this_.timed_wait_task_(task));
        }
        check_task(task);
        return to_vtask(true);
    }

    private async Task<tuple<bool, R>> timed_wait_task_<R>(Task<R> task)
    {
        await async_wait();
        return tuple.make(!_overtime, _overtime ? default(R) : check_task(task));
    }

    private async Task<tuple<bool, csp_invoke_wrap<R>>> timed_wait_csp_<R>(delay_csp<R> task)
    {
        await async_wait();
        return tuple.make(!_overtime, _overtime ? default(csp_invoke_wrap<R>) : check_csp(task));
    }

    static public ValueTask<tuple<bool, R>> timed_wait_task<R>(int ms, Task<R> task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            task.GetAwaiter().UnsafeOnCompleted(this_.timed_async_result(ms));
            return to_vtask(this_.timed_wait_task_(task));
        }
        return to_vtask(tuple.make(true, check_task(task)));
    }

    static public ValueTask<tuple<bool, csp_invoke_wrap<R>>> timed_wait_csp<R>(int ms, delay_csp<R> task)
    {
        if (!task.is_completed)
        {
            generator this_ = self;
            task.on_completed(this_.timed_async_result(ms));
            return to_vtask(this_.timed_wait_csp_(task));
        }
        return to_vtask(tuple.make(true, check_csp(task)));
    }

    static public ValueTask<tuple<bool, csp_invoke_wrap<R>>> try_wait_csp<R>(delay_csp<R> task)
    {
        if (!task.is_completed)
        {
            return to_vtask(tuple.make(false, default(csp_invoke_wrap<R>)));
        }
        return to_vtask(tuple.make(true, check_csp(task)));
    }

    static public Task unsafe_wait_task<R>(async_result_wrap<R, Exception> res, Task<R> task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            res.clear();
            task.GetAwaiter().UnsafeOnCompleted(this_.async_callback(() => check_task(res, task)));
            return this_.async_wait();
        }
        check_task(res, task);
        return non_async();
    }

    static public Task unsafe_timed_wait_task(async_result_wrap<bool, Exception> res, int ms, Task task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            res.value1 = false;
            task.GetAwaiter().UnsafeOnCompleted(this_.timed_async_callback2(ms, () => check_task(res, task)));
            return this_.async_wait();
        }
        check_task(res, task);
        return non_async();
    }

    static public Task unsafe_timed_wait_task<R>(async_result_wrap<bool, R, Exception> res, int ms, Task<R> task)
    {
        if (!task.IsCompleted)
        {
            generator this_ = self;
            res.value1 = false;
            task.GetAwaiter().UnsafeOnCompleted(this_.timed_async_callback2(ms, () => check_task(res, task)));
            return this_.async_wait();
        }
        check_task(res, task);
        return non_async();
    }

    static public Task stop_other(generator otherGen)
    {
        generator this_ = self;
        otherGen.stop(this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public async Task stop_others(IEnumerable<generator> otherGens)
    {
        generator this_ = self;
        unlimit_chan<void_type> waitStop = new unlimit_chan<void_type>(this_.strand);
        Action ntf = waitStop.wrap_default();
        int count = 0;
        foreach (generator otherGen in otherGens)
        {
            if (!otherGen.is_completed())
            {
                count++;
                otherGen.stop(ntf);
            }
        }
        for (int i = 0; i < count; i++)
        {
            await chan_receive(waitStop);
        }
    }

    static public Task stop_others(params generator[] otherGens)
    {
        return stop_others((IEnumerable<generator>)otherGens);
    }

    static public Task wait_other(generator otherGen)
    {
        generator this_ = self;
        otherGen.append_stop_callback(this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public async Task wait_others(IEnumerable<generator> otherGens)
    {
        generator this_ = self;
        unlimit_chan<void_type> waitStop = new unlimit_chan<void_type>(this_.strand);
        Action ntf = waitStop.wrap_default();
        int count = 0;
        foreach (generator otherGen in otherGens)
        {
            if (!otherGen.is_completed())
            {
                count++;
                otherGen.append_stop_callback(ntf);
            }
        }
        for (int i = 0; i < count; i++)
        {
            await chan_receive(waitStop);
        }
    }

    static public Task wait_others(params generator[] otherGens)
    {
        return wait_others((IEnumerable<generator>)otherGens);
    }

    private async Task<bool> timed_wait_other_(nil_chan<notify_token> waitRemove, generator otherGen)
    {
        try
        {
            await async_wait();
        }
        finally
        {
            lock_suspend_and_stop_();
            if (_overtime)
            {
                notify_token cancelToken = await chan_receive(waitRemove);
                if (null != cancelToken.token)
                {
                    otherGen.remove_stop_callback(cancelToken);
                }
            }
            await unlock_suspend_and_stop_();
        }
        return !_overtime;
    }

    static public ValueTask<bool> timed_wait_other(int ms, generator otherGen)
    {
        generator this_ = self;
        nil_chan<notify_token> waitRemove = new nil_chan<notify_token>();
        otherGen.append_stop_callback(this_.timed_async_result(ms), waitRemove.wrap());
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.timed_wait_other_(waitRemove, otherGen));
        }
        return to_vtask(!this_._overtime);
    }

    static public Task<generator> wait_others_any(IEnumerable<generator> otherGens)
    {
        return timed_wait_others_any(-1, otherGens);
    }

    static public Task<generator> wait_others_any(params generator[] otherGens)
    {
        return wait_others_any((IEnumerable<generator>)otherGens);
    }

    static public async Task<generator> timed_wait_others_any(int ms, IEnumerable<generator> otherGens)
    {
        generator this_ = self;
        unlimit_chan<notify_token> waitRemove = new unlimit_chan<notify_token>(this_.strand);
        unlimit_chan<generator> waitStop = new unlimit_chan<generator>(this_.strand);
        Action<generator> stopHandler = (generator host) => waitStop.post(host);
        Action<notify_token> removeHandler = (notify_token cancelToken) => waitRemove.post(cancelToken);
        int count = 0;
        foreach (generator ele in otherGens)
        {
            count++;
            if (ele.is_completed())
            {
                waitStop.post(ele);
                waitRemove.post(new notify_token { host = ele });
                break;
            }
            ele.append_stop_callback(stopHandler, removeHandler);
        }
        try
        {
            if (0 != count)
            {
                if (ms < 0)
                {
                    return await chan_receive(waitStop);
                }
                else if (0 == ms)
                {
                    chan_recv_wrap<generator> gen = await chan_try_receive(waitStop);
                    if (chan_state.ok == gen.state)
                    {
                        return gen.msg;
                    }
                }
                else
                {
                    chan_recv_wrap<generator> gen = await chan_timed_receive(waitStop, ms);
                    if (chan_state.ok == gen.state)
                    {
                        return gen.msg;
                    }
                }
            }
            return null;
        }
        finally
        {
            this_.lock_suspend_and_stop_();
            for (int i = 0; i < count; i++)
            {
                notify_token node = (await chan_receive(waitRemove)).msg;
                if (null != node.token)
                {
                    node.host.remove_stop_callback(node);
                }
            }
            await this_.unlock_suspend_and_stop_();
        }
    }

    static public Task<generator> timed_wait_others_any(int ms, params generator[] otherGens)
    {
        return timed_wait_others_any(ms, (IEnumerable<generator>)otherGens);
    }

    static public async Task<List<generator>> timed_wait_others(int ms, IEnumerable<generator> otherGens)
    {
        generator this_ = self;
        long endTick = system_tick.get_tick_ms() + ms;
        unlimit_chan<notify_token> waitRemove = new unlimit_chan<notify_token>(this_.strand);
        unlimit_chan<generator> waitStop = new unlimit_chan<generator>(this_.strand);
        Action<generator> stopHandler = (generator host) => waitStop.post(host);
        Action<notify_token> removeHandler = (notify_token cancelToken) => waitRemove.post(cancelToken);
        int count = 0;
        foreach (generator ele in otherGens)
        {
            count++;
            if (!ele.is_completed())
            {
                ele.append_stop_callback(stopHandler, removeHandler);
            }
            else
            {
                waitStop.post(ele);
                waitRemove.post(new notify_token { host = ele });
            }
        }
        try
        {
            List<generator> completedGens = new List<generator>(count);
            if (ms < 0)
            {
                for (int i = 0; i < count; i++)
                {
                    completedGens.Add(await chan_receive(waitStop));
                }
            }
            else if (0 == ms)
            {
                for (int i = 0; i < count; i++)
                {
                    chan_recv_wrap<generator> gen = await chan_try_receive(waitStop);
                    if (chan_state.ok != gen.state)
                    {
                        break;
                    }
                    completedGens.Add(gen.msg);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    long nowTick = system_tick.get_tick_ms();
                    if (nowTick >= endTick)
                    {
                        break;
                    }
                    chan_recv_wrap<generator> gen = await chan_timed_receive(waitStop, (int)(endTick - nowTick));
                    if (chan_state.ok != gen.state)
                    {
                        break;
                    }
                    completedGens.Add(gen.msg);
                }
            }
            return completedGens;
        }
        finally
        {
            this_.lock_suspend_and_stop_();
            for (int i = 0; i < count; i++)
            {
                notify_token node = (await chan_receive(waitRemove)).msg;
                if (null != node.token)
                {
                    node.host.remove_stop_callback(node);
                }
            }
            await this_.unlock_suspend_and_stop_();
        }
    }

    static public Task<List<generator>> timed_wait_others(int ms, params generator[] otherGens)
    {
        return timed_wait_others(ms, (IEnumerable<generator>)otherGens);
    }

    static public generator completed_others_any(IEnumerable<generator> otherGens)
    {
        foreach (generator ele in otherGens)
        {
            if (ele.is_completed())
            {
                return ele;
            }
        }
        return null;
    }

    static public generator completed_others_any(params generator[] otherGens)
    {
        return completed_others_any((IEnumerable<generator>)otherGens);
    }

    static public List<generator> completed_others(IEnumerable<generator> otherGens)
    {
        List<generator> completedGens = new List<generator>(32);
        foreach (generator ele in otherGens)
        {
            if (ele.is_completed())
            {
                completedGens.Add(ele);
            }
        }
        return completedGens;
    }

    static public List<generator> completed_others(params generator[] otherGens)
    {
        List<generator> completedGens = new List<generator>(otherGens.Length);
        foreach (generator ele in otherGens)
        {
            if (ele.is_completed())
            {
                completedGens.Add(ele);
            }
        }
        return completedGens;
    }

    private async Task wait_group_(wait_group wg, wait_group.cancel_token cancelToken)
    {
        try
        {
            await async_wait();
        }
        finally
        {
            wg.cancel_wait(cancelToken);
        }
    }

    static public Task wait_group(wait_group wg)
    {
        generator this_ = self;
        wait_group.cancel_token cancelToken = wg.async_wait(this_.unsafe_async_result());
        if (!this_.new_task_completed())
        {
            return this_.wait_group_(wg, cancelToken);
        }
        return non_async();
    }

    static public Task unsafe_wait_group(wait_group wg)
    {
        generator this_ = self;
        wg.async_wait(this_.unsafe_async_result());
        return this_.async_wait();
    }

    private async Task<bool> timed_wait_group_(wait_group wg, wait_group.cancel_token cancelToken)
    {
        try
        {
            await async_wait();
            return !_overtime;
        }
        finally
        {
            wg.cancel_wait(cancelToken);
        }
    }

    static public ValueTask<bool> timed_wait_group(int ms, wait_group wg)
    {
        generator this_ = self;
        wait_group.cancel_token cancelToken = wg.async_wait(this_.timed_async_result(ms));
        if (!this_.new_task_completed())
        {
            return to_vtask(this_.timed_wait_group_(wg, cancelToken));
        }
        return to_vtask(!this_._overtime);
    }

    static public async Task enter_gate<R>(wait_gate<R> wg)
    {
        generator this_ = self;
        wait_gate.cancel_token token = wg.async_enter(this_.unsafe_async_result());
        try
        {
            await this_.async_wait();
        }
        finally
        {
            if (wg.cancel_enter(token) && !wg.is_exit)
            {
                this_.lock_suspend_and_stop_();
                wg.safe_exit(this_.unsafe_async_result());
                await this_.async_wait();
                await this_.unlock_suspend_and_stop_();
            }
        }
    }

    static public async Task<bool> timed_enter_gate<R>(int ms, wait_gate<R> wg)
    {
        generator this_ = self;
        wait_gate.cancel_token token = wg.async_enter(this_.timed_async_result(ms));
        try
        {
            await this_.async_wait();
            return !this_._overtime;
        }
        finally
        {
            if (wg.cancel_enter(token) && !wg.is_exit)
            {
                this_.lock_suspend_and_stop_();
                wg.safe_exit(this_.unsafe_async_result());
                await this_.async_wait();
                await this_.unlock_suspend_and_stop_();
            }
        }
    }

    static public Task unsafe_async_call(Action<Action> handler)
    {
        generator this_ = self;
        handler(this_.unsafe_async_result());
        return this_.async_wait();
    }

    static public Task unsafe_async_call<R>(async_result_wrap<R> res, Action<Action<R>> handler)
    {
        generator this_ = self;
        res.clear();
        handler(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task unsafe_async_call<R1, R2>(async_result_wrap<R1, R2> res, Action<Action<R1, R2>> handler)
    {
        generator this_ = self;
        res.clear();
        handler(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task unsafe_async_call<R1, R2, R3>(async_result_wrap<R1, R2, R3> res, Action<Action<R1, R2, R3>> handler)
    {
        generator this_ = self;
        res.clear();
        handler(this_.unsafe_async_result(res));
        return this_.async_wait();
    }

    static public Task async_call(Action<Action> handler, Action lostHandler = null)
    {
        generator this_ = self;
        handler(this_.async_result(lostHandler));
        return this_.async_wait();
    }

    static public ValueTask<R> async_call<R>(Action<Action<R>> handler, Action<R> lostHandler = null)
    {
        generator this_ = self;
        async_result_wrap<R> res = new async_result_wrap<R>();
        handler(this_.async_result(res, lostHandler));
        return this_.async_wait(res);
    }

    static public ValueTask<tuple<R1, R2>> async_call<R1, R2>(Action<Action<R1, R2>> handler, Action<R1, R2> lostHandler = null)
    {
        generator this_ = self;
        async_result_wrap<R1, R2> res = new async_result_wrap<R1, R2>();
        handler(this_.async_result(res, lostHandler));
        return this_.async_wait(res);
    }

    static public ValueTask<tuple<R1, R2, R3>> async_call<R1, R2, R3>(Action<Action<R1, R2, R3>> handler, Action<R1, R2, R3> lostHandler = null)
    {
        generator this_ = self;
        async_result_wrap<R1, R2, R3> res = new async_result_wrap<R1, R2, R3>();
        handler(this_.async_result(res, lostHandler));
        return this_.async_wait(res);
    }

    static public Task unsafe_timed_async_call(async_result_wrap<bool> res, int ms, Action<Action> handler, Action timedHandler = null)
    {
        generator this_ = self;
        res.value1 = false;
        handler(this_.timed_async_callback(ms, () => res.value1 = !this_._overtime, timedHandler));
        return this_.async_wait();
    }

    static public ValueTask<bool> timed_async_call(int ms, Action<Action> handler, Action timedHandler = null)
    {
        generator this_ = self;
        handler(this_.timed_async_callback(ms, nil_action.action, timedHandler));
        return this_.timed_async_wait();
    }

    static public Task unsafe_timed_async_call<R>(async_result_wrap<bool, R> res, int ms, Action<Action<R>> handler, Action timedHandler = null)
    {
        generator this_ = self;
        res.value1 = false;
        handler(this_.timed_async_callback(ms, delegate (R res1)
        {
            res.value1 = !this_._overtime;
            res.value2 = res1;
        }, timedHandler));
        return this_.async_wait();
    }

    static public ValueTask<tuple<bool, R>> timed_async_call<R>(int ms, Action<Action<R>> handler, Action timedHandler = null)
    {
        generator this_ = self;
        async_result_wrap<R> res = new async_result_wrap<R>();
        handler(this_.timed_async_result(ms, res, timedHandler));
        return this_.timed_async_wait(res);
    }

    static public Task unsafe_timed_async_call<R1, R2>(async_result_wrap<bool, tuple<R1, R2>> res, int ms, Action<Action<R1, R2>> handler, Action timedHandler = null)
    {
        generator this_ = self;
        res.value1 = false;
        handler(this_.timed_async_callback(ms, delegate (R1 res1, R2 res2)
        {
            res.value1 = !this_._overtime;
            res.value2 = tuple.make(res1, res2);
        }, timedHandler));
        return this_.async_wait();
    }

    static public ValueTask<tuple<bool, tuple<R1, R2>>> timed_async_call<R1, R2>(int ms, Action<Action<R1, R2>> handler, Action timedHandler = null)
    {
        generator this_ = self;
        async_result_wrap<R1, R2> res = new async_result_wrap<R1, R2>();
        handler(this_.timed_async_result(ms, res, timedHandler));
        return this_.timed_async_wait(res);
    }

    static public Task unsafe_timed_async_call<R1, R2, R3>(async_result_wrap<bool, tuple<R1, R2, R3>> res, int ms, Action<Action<R1, R2, R3>> handler, Action timedHandler = null)
    {
        generator this_ = self;
        res.value1 = false;
        handler(this_.timed_async_callback(ms, delegate (R1 res1, R2 res2, R3 res3)
        {
            res.value1 = !this_._overtime;
            res.value2 = tuple.make(res1, res2, res3);
        }, timedHandler));
        return this_.async_wait();
    }

    static public ValueTask<tuple<bool, tuple<R1, R2, R3>>> timed_async_call<R1, R2, R3>(int ms, Action<Action<R1, R2, R3>> handler, Action timedHandler = null)
    {
        generator this_ = self;
        async_result_wrap<R1, R2, R3> res = new async_result_wrap<R1, R2, R3>();
        handler(this_.timed_async_result(ms, res, timedHandler));
        return this_.timed_async_wait(res);
    }
#if CHECK_STEP_TIMEOUT
static public async Task call(action handler)
{
    generator this_ = self;
    up_stack_frame(this_._makeStack, 2);
    try
    {
        await handler();
    }
    catch (System.Exception)
    {
        this_._makeStack.RemoveFirst();
        throw;
    }
}

static public async Task<R> call<R>(Func<Task<R>> handler)
{
    generator this_ = self;
    up_stack_frame(this_._makeStack, 2);
    try
    {
        return await handler();
    }
    catch (System.Exception)
    {
        this_._makeStack.RemoveFirst();
        throw;
    }
}

static public async Task depth_call(shared_strand strand, action handler)
{
    generator this_ = self;
    up_stack_frame(this_._makeStack, 2);
    generator depthGen = (new generator()).init(strand, handler, this_.unsafe_async_result(), null, this_._makeStack);
    try
    {
        depthGen.run();
        await this_.async_wait();
    }
    catch (stop_exception)
    {
        depthGen.stop(this_.unsafe_async_result());
        await this_.async_wait();
        throw;
    }
    finally
    {
        this_._makeStack.RemoveFirst();
    }
}

static public async Task<R> depth_call<R>(shared_strand strand, Func<Task<R>> handler)
{
    generator this_ = self;
    R res = default(R);
    up_stack_frame(this_._makeStack, 2);
    generator depthGen = (new generator()).init(strand, async () => res = await handler(), this_.unsafe_async_result(), null, this_._makeStack);
    try
    {
        depthGen.run();
        await this_.async_wait();
    }
    catch (stop_exception)
    {
        depthGen.stop(this_.unsafe_async_result());
        await this_.async_wait();
        throw;
    }
    finally
    {
        this_._makeStack.RemoveFirst();
    }
    return res;
}
#else
    static public Task call(action handler)
    {
        return handler();
    }

    static public Task<R> call<R>(Func<Task<R>> handler)
    {
        return handler();
    }

    static public async Task depth_call(shared_strand strand, action handler)
    {
        generator this_ = self;
        generator depthGen = make(strand, handler, this_.unsafe_async_result());
        try
        {
            depthGen.run();
            await this_.async_wait();
        }
        catch (stop_exception)
        {
            depthGen.stop(this_.unsafe_async_result());
            await this_.async_wait();
            throw;
        }
    }

    static public async Task<R> depth_call<R>(shared_strand strand, Func<Task<R>> handler)
    {
        generator this_ = self;
        R res = default(R);
        generator depthGen = make(strand, async () => res = await handler(), this_.unsafe_async_result());
        try
        {
            depthGen.run();
            await this_.async_wait();
        }
        catch (stop_exception)
        {
            depthGen.stop(this_.unsafe_async_result());
            await this_.async_wait();
            throw;
        }
        return res;
    }
#endif

#if CHECK_STEP_TIMEOUT
static private LinkedList<call_stack_info[]> debug_stack
{
    get
    {
        return self._makeStack;
    }
}
#endif

    static long calc_hash<T>(int id)
    {
        return (long)id << 32 | (uint)type_hash<T>.code;
    }

    static public chan<T> self_mailbox<T>(int id = 0)
    {
        generator this_ = self;
        if (null == this_._mailboxMap)
        {
            this_._mailboxMap = new Dictionary<long, mail_pck>();
        }
        mail_pck mb = null;
        if (!this_._mailboxMap.TryGetValue(calc_hash<T>(id), out mb))
        {
            mb = new mail_pck(new unlimit_chan<T>(this_.strand));
            this_._mailboxMap.Add(calc_hash<T>(id), mb);
        }
        return (chan<T>)mb.mailbox;
    }

    public ValueTask<chan<T>> get_mailbox<T>(int id = 0)
    {
        return send_strand(strand, delegate ()
        {
            if (-1 == _lockSuspendCount)
            {
                return null;
            }
            if (null == _mailboxMap)
            {
                _mailboxMap = new Dictionary<long, mail_pck>();
            }
            mail_pck mb = null;
            if (!_mailboxMap.TryGetValue(calc_hash<T>(id), out mb))
            {
                mb = new mail_pck(new unlimit_chan<T>(strand));
                _mailboxMap.Add(calc_hash<T>(id), mb);
            }
            return (chan<T>)mb.mailbox;
        });
    }

    static public async Task<bool> agent_mail<T>(generator agentGen, int id = 0)
    {
        generator this_ = self;
        if (null == this_._agentMng)
        {
            this_._agentMng = new children();
        }
        if (null == this_._mailboxMap)
        {
            this_._mailboxMap = new Dictionary<long, mail_pck>();
        }
        mail_pck mb = null;
        if (!this_._mailboxMap.TryGetValue(calc_hash<T>(id), out mb))
        {
            mb = new mail_pck(new unlimit_chan<T>(this_.strand));
            this_._mailboxMap.Add(calc_hash<T>(id), mb);
        }
        else if (null != mb.agentAction)
        {
            await this_._agentMng.stop(mb.agentAction);
            mb.agentAction = null;
        }
        chan<T> agentMb = await agentGen.get_mailbox<T>();
        if (null == agentMb)
        {
            return false;
        }
        mb.agentAction = this_._agentMng.make(async delegate ()
        {
            chan<T> selfMb = (chan<T>)mb.mailbox;
            chan_notify_sign ntfSign = new chan_notify_sign();
            generator self = generator.self;
            try
            {
                nil_chan<chan_state> waitHasChan = new nil_chan<chan_state>();
                Action<chan_state> waitHasNtf = waitHasChan.wrap();
                async_result_wrap<chan_recv_wrap<T>> recvRes = new async_result_wrap<chan_recv_wrap<T>>();
                selfMb.async_append_recv_notify(waitHasNtf, ntfSign);
                while (true)
                {
                    await chan_receive(waitHasChan);
                    try
                    {
                        self.lock_suspend_and_stop_();
                        recvRes.value1 = chan_recv_wrap<T>.def;
                        selfMb.async_try_recv_and_append_notify(self.unsafe_async_result(recvRes), waitHasNtf, ntfSign);
                        await self.async_wait();
                        if (chan_state.ok == recvRes.value1.state)
                        {
                            recvRes.value1 = new chan_recv_wrap<T> { state = await chan_send(agentMb, recvRes.value1.msg) };
                        }
                        if (chan_state.closed == recvRes.value1.state)
                        {
                            break;
                        }
                    }
                    finally
                    {
                        await self.unlock_suspend_and_stop_();
                    }
                }
            }
            finally
            {
                self.lock_suspend_and_stop_();
                selfMb.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                await self.async_wait();
                await self.unlock_suspend_and_stop_();
            }
        });
        mb.agentAction.run();
        return true;
    }

    static public async Task<bool> cancel_agent<T>(int id = 0)
    {
        generator this_ = self;
        mail_pck mb = null;
        if (null != this_._agentMng && null != this_._mailboxMap &&
            this_._mailboxMap.TryGetValue(calc_hash<T>(id), out mb) && null != mb.agentAction)
        {
            await this_._agentMng.stop(mb.agentAction);
            mb.agentAction = null;
            return true;
        }
        return false;
    }

    static public ValueTask<chan_recv_wrap<T>> recv_msg<T>(int id = 0, chan_lost_msg<T> lostMsg = null)
    {
        return chan_receive(self_mailbox<T>(id), lostMsg);
    }

    static public ValueTask<chan_recv_wrap<T>> try_recv_msg<T>(int id = 0, chan_lost_msg<T> lostMsg = null)
    {
        return chan_try_receive(self_mailbox<T>(id), lostMsg);
    }

    static public ValueTask<chan_recv_wrap<T>> timed_recv_msg<T>(int ms, int id = 0, chan_lost_msg<T> lostMsg = null)
    {
        return chan_timed_receive(self_mailbox<T>(id), ms, lostMsg);
    }

    private async Task<chan_send_wrap> send_msg_<T>(ValueTask<chan<T>> mbTask, T msg, chan_lost_msg<T> lostMsg)
    {
        chan<T> mb = await mbTask;
        return null != mb ? await chan_send(mb, msg, lostMsg) : new chan_send_wrap { state = chan_state.fail };
    }

    public ValueTask<chan_send_wrap> send_msg<T>(int id, T msg, chan_lost_msg<T> lostMsg = null)
    {
        ValueTask<chan<T>> mbTask = get_mailbox<T>(id);
        if (!mbTask.IsCompleted)
        {
            return to_vtask(send_msg_(mbTask, msg, lostMsg));
        }
        chan<T> mb = mbTask.GetAwaiter().GetResult();
        return null != mb ? chan_send(mb, msg, lostMsg) : to_vtask(new chan_send_wrap { state = chan_state.fail });
    }

    public ValueTask<chan_send_wrap> send_msg<T>(T msg, chan_lost_msg<T> lostMsg = null)
    {
        return send_msg(0, msg, lostMsg);
    }

    public ValueTask<chan_send_wrap> send_void_msg(int id, chan_lost_msg<void_type> lostMsg = null)
    {
        return send_msg(id, default(void_type), lostMsg);
    }

    public ValueTask<chan_send_wrap> send_void_msg(chan_lost_msg<void_type> lostMsg = null)
    {
        return send_msg(0, default(void_type), lostMsg);
    }

    static private async Task<void_type> wait_void_task(Task task)
    {
        await task;
        return default(void_type);
    }

    static private ValueTask<void_type> check_void_task(Task task)
    {
        if (!task.IsCompleted)
        {
            return to_vtask(wait_void_task(task));
        }
        return to_vtask(check_task(task));
    }

    public class receive_mail
    {
        bool _run = true;
        go_shared_mutex _mutex;
        children _children = new children();

        internal receive_mail(bool forceStopAll)
        {
            generator self = generator.self;
            if (null == self._mailboxMap)
            {
                self._mailboxMap = new Dictionary<long, mail_pck>();
            }
            if (null == self._genLocal)
            {
                self._genLocal = new Dictionary<long, local_wrap>();
            }
            _mutex = forceStopAll ? null : new go_shared_mutex(self.strand);
        }

        public receive_mail case_of(chan<void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return case_of(chan, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail case_of<T>(chan<T> chan, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null == _mutex)
                {
                    try
                    {
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        while (_run)
                        {
                            chan_recv_wrap<T> recvRes = await chan_receive(chan, lostMsg);
                            if (chan_state.ok == recvRes.state)
                            {
                                await handler(recvRes.msg);
                            }
                            else if (null != errHandler && await errHandler(recvRes.state))
                            {
                                break;
                            }
                            else if (chan_state.closed == recvRes.state)
                            {
                                break;
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self._mailboxMap = null;
                        self._genLocal = null;
                    }
                }
                else
                {
                    chan_notify_sign ntfSign = new chan_notify_sign();
                    try
                    {
                        self.lock_suspend_();
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        nil_chan<chan_state> waitHasChan = new nil_chan<chan_state>();
                        Action<chan_state> waitHasNtf = waitHasChan.wrap();
                        async_result_wrap<chan_recv_wrap<T>> recvRes = new async_result_wrap<chan_recv_wrap<T>>();
                        chan.async_append_recv_notify(waitHasNtf, ntfSign);
                        while (_run)
                        {
                            await chan_receive(waitHasChan);
                            await mutex_lock_shared(_mutex);
                            try
                            {
                                try
                                {
                                    recvRes.value1 = chan_recv_wrap<T>.def;
                                    chan.async_try_recv_and_append_notify(self.unsafe_async_result(recvRes), waitHasNtf, ntfSign);
                                    await self.async_wait();
                                }
                                catch (stop_exception)
                                {
                                    chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                                    await self.async_wait();
                                    if (chan_state.ok == recvRes.value1.state)
                                    {
                                        lostMsg?.set(recvRes.value1.msg);
                                    }
                                    throw;
                                }
                                try
                                {
                                    await self.unlock_suspend_();
                                    if (chan_state.ok == recvRes.value1.state)
                                    {
                                        await handler(recvRes.value1.msg);
                                    }
                                    else if (null != errHandler && await errHandler(recvRes.value1.state))
                                    {
                                        break;
                                    }
                                    else if (chan_state.closed == recvRes.value1.state)
                                    {
                                        break;
                                    }
                                }
                                finally
                                {
                                    self.lock_suspend_();
                                }
                            }
                            finally
                            {
                                await mutex_unlock_shared(_mutex);
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self.lock_stop_();
                        self._mailboxMap = null;
                        self._genLocal = null;
                        chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                        await self.async_wait();
                        await self.unlock_suspend_and_stop_();
                    }
                }
            });
            return this;
        }

        public receive_mail case_of<T1, T2>(chan<tuple<T1, T2>> chan, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2, T3>(chan<tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail timed_case_of(chan<void_type> chan, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return timed_case_of(chan, ms, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T>(chan<T> chan, int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null == _mutex)
                {
                    chan_recv_wrap<T> recvRes = await chan_timed_receive(chan, ms, lostMsg);
                    try
                    {
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        if (chan_state.ok == recvRes.state)
                        {
                            await handler(recvRes.msg);
                        }
                        else if (null != errHandler && await errHandler(recvRes.state)) { }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self._mailboxMap = null;
                        self._genLocal = null;
                    }
                }
                else
                {
                    chan_notify_sign ntfSign = new chan_notify_sign();
                    try
                    {
                        self.lock_suspend_();
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        long endTick = system_tick.get_tick_ms() + ms;
                        while (_run)
                        {
                            chan_state result = chan_state.undefined;
                            chan.async_append_recv_notify(self.unsafe_async_callback((chan_state state) => result = state), ntfSign, ms);
                            await self.async_wait();
                            await mutex_lock_shared(_mutex);
                            try
                            {
                                if (chan_state.overtime != result)
                                {
                                    chan_recv_wrap<T> recvRes = await chan_try_receive(chan, lostMsg);
                                    try
                                    {
                                        await self.unlock_suspend_();
                                        if (chan_state.ok == recvRes.state)
                                        {
                                            await handler(recvRes.msg); break;
                                        }
                                        else if ((null != errHandler && await errHandler(recvRes.state)) || chan_state.closed == recvRes.state) { break; }
                                        if (0 <= ms && 0 >= (ms = (int)(endTick - system_tick.get_tick_ms())))
                                        {
                                            ms = -1;
                                            if (null == errHandler || await errHandler(chan_state.overtime))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        self.lock_suspend_();
                                    }
                                }
                                else
                                {
                                    ms = -1;
                                    if (null == errHandler || await errHandler(chan_state.overtime))
                                    {
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                await mutex_unlock_shared(_mutex);
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self.lock_stop_();
                        self._mailboxMap = null;
                        self._genLocal = null;
                        chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                        await self.async_wait();
                        await self.unlock_suspend_and_stop_();
                    }
                }
            });
            return this;
        }

        public receive_mail timed_case_of<T1, T2>(chan<tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2, T3>(chan<tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail try_case_of(chan<void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return try_case_of(chan, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail try_case_of<T>(chan<T> chan, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null != _mutex)
                {
                    await mutex_lock_shared(_mutex);
                }
                try
                {
                    self._mailboxMap = _children.parent._mailboxMap;
                    self._genLocal = _children.parent._genLocal;
                    chan_recv_wrap<T> recvRes = await chan_try_receive(chan, lostMsg);
                    if (chan_state.ok == recvRes.state)
                    {
                        await handler(recvRes.msg);
                    }
                    else if (null != errHandler && await errHandler(recvRes.state)) { }
                }
                catch (message_stop_current_exception) { }
                catch (message_stop_all_exception)
                {
                    _run = false;
                }
                finally
                {
                    self._mailboxMap = null;
                    self._genLocal = null;
                    if (null != _mutex)
                    {
                        await mutex_unlock_shared(_mutex);
                    }
                }
            });
            return this;
        }

        public receive_mail try_case_of<T1, T2>(chan<tuple<T1, T2>> chan, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2, T3>(chan<tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail case_of(broadcast_chan<void_type> chan, Func<Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return case_of(chan, (void_type _) => handler(), token, errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2>(broadcast_chan<tuple<T1, T2>> chan, Func<T1, T2, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), token, errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2, T3>(broadcast_chan<tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), token, errHandler, lostMsg);
        }

        public receive_mail case_of<T>(broadcast_chan<T> chan, Func<T, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                token = null != token ? token : new broadcast_token();
                if (null == _mutex)
                {
                    try
                    {
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        while (_run)
                        {
                            chan_recv_wrap<T> recvRes = await chan_receive(chan, token, lostMsg);
                            if (chan_state.ok == recvRes.state)
                            {
                                await handler(recvRes.msg);
                            }
                            else if (null != errHandler && await errHandler(recvRes.state))
                            {
                                break;
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self._mailboxMap = null;
                        self._genLocal = null;
                    }
                }
                else
                {
                    chan_notify_sign ntfSign = new chan_notify_sign();
                    try
                    {
                        self.lock_suspend_();
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        nil_chan<chan_state> waitHasChan = new nil_chan<chan_state>();
                        Action<chan_state> waitHasNtf = waitHasChan.wrap();
                        async_result_wrap<chan_recv_wrap<T>> recvRes = new async_result_wrap<chan_recv_wrap<T>>();
                        chan.async_append_recv_notify(waitHasNtf, ntfSign);
                        while (_run)
                        {
                            await chan_receive(waitHasChan);
                            await mutex_lock_shared(_mutex);
                            try
                            {
                                try
                                {
                                    recvRes.value1 = chan_recv_wrap<T>.def;
                                    chan.async_try_recv_and_append_notify(self.unsafe_async_result(recvRes), waitHasNtf, ntfSign);
                                    await self.async_wait();
                                }
                                catch (stop_exception)
                                {
                                    chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                                    await self.async_wait();
                                    if (chan_state.ok == recvRes.value1.state)
                                    {
                                        lostMsg?.set(recvRes.value1.msg);
                                    }
                                    throw;
                                }
                                try
                                {
                                    await self.unlock_suspend_();
                                    if (chan_state.ok == recvRes.value1.state)
                                    {
                                        await handler(recvRes.value1.msg);
                                    }
                                    else if (null != errHandler && await errHandler(recvRes.value1.state))
                                    {
                                        break;
                                    }
                                    else if (chan_state.closed == recvRes.value1.state)
                                    {
                                        break;
                                    }
                                }
                                finally
                                {
                                    self.lock_suspend_();
                                }
                            }
                            finally
                            {
                                await mutex_unlock_shared(_mutex);
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self.lock_stop_();
                        self._mailboxMap = null;
                        self._genLocal = null;
                        chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                        await self.async_wait();
                        await self.unlock_suspend_and_stop_();
                    }
                }
            });
            return this;
        }

        public receive_mail timed_case_of(broadcast_chan<void_type> chan, int ms, Func<Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return timed_case_of(chan, ms, (void_type _) => handler(), token, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2>(broadcast_chan<tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), token, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2, T3>(broadcast_chan<tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), token, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T>(broadcast_chan<T> chan, int ms, Func<T, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                token = null != token ? token : new broadcast_token();
                if (null == _mutex)
                {
                    chan_recv_wrap<T> recvRes = await chan_timed_receive(chan, ms, token, lostMsg);
                    try
                    {
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        if (chan_state.ok == recvRes.state)
                        {
                            await handler(recvRes.msg);
                        }
                        else if (null != errHandler && await errHandler(recvRes.state)) { }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self._mailboxMap = null;
                        self._genLocal = null;
                    }
                }
                else
                {
                    chan_notify_sign ntfSign = new chan_notify_sign();
                    try
                    {
                        self.lock_suspend_();
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        long endTick = system_tick.get_tick_ms() + ms;
                        while (_run)
                        {
                            chan_state result = chan_state.undefined;
                            chan.async_append_recv_notify(self.unsafe_async_callback((chan_state state) => result = state), ntfSign, ms);
                            await self.async_wait();
                            await mutex_lock_shared(_mutex);
                            try
                            {
                                if (chan_state.overtime != result)
                                {
                                    chan_recv_wrap<T> recvRes = await chan_try_receive(chan, lostMsg);
                                    try
                                    {
                                        await self.unlock_suspend_();
                                        if (chan_state.ok == recvRes.state)
                                        {
                                            await handler(recvRes.msg); break;
                                        }
                                        else if ((null != errHandler && await errHandler(recvRes.state)) || chan_state.closed == recvRes.state) { break; }
                                        if (0 <= ms && 0 >= (ms = (int)(endTick - system_tick.get_tick_ms())))
                                        {
                                            ms = -1;
                                            if (null == errHandler || await errHandler(chan_state.overtime))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        self.lock_suspend_();
                                    }
                                }
                                else
                                {
                                    ms = -1;
                                    if (null == errHandler || await errHandler(chan_state.overtime))
                                    {
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                await mutex_unlock_shared(_mutex);
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self.lock_stop_();
                        self._mailboxMap = null;
                        self._genLocal = null;
                        chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                        await self.async_wait();
                        await self.unlock_suspend_and_stop_();
                    }
                }
            });
            return this;
        }

        public receive_mail try_case_of(broadcast_chan<void_type> chan, Func<Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return try_case_of(chan, (void_type _) => handler(), token, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2>(broadcast_chan<tuple<T1, T2>> chan, Func<T1, T2, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), token, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2, T3>(broadcast_chan<tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), token, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T>(broadcast_chan<T> chan, Func<T, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null != _mutex)
                {
                    await mutex_lock_shared(_mutex);
                }
                try
                {
                    self._mailboxMap = _children.parent._mailboxMap;
                    self._genLocal = _children.parent._genLocal;
                    chan_recv_wrap<T> recvRes = await chan_try_receive(chan, null != token ? token : new broadcast_token(), lostMsg);
                    if (chan_state.ok == recvRes.state)
                    {
                        await handler(recvRes.msg);
                    }
                    else if (null != errHandler && await errHandler(recvRes.state)) { }
                }
                catch (message_stop_current_exception) { }
                catch (message_stop_all_exception)
                {
                    _run = false;
                }
                finally
                {
                    self._mailboxMap = null;
                    self._genLocal = null;
                    if (null != _mutex)
                    {
                        await mutex_unlock_shared(_mutex);
                    }
                }
            });
            return this;
        }

        public receive_mail case_of<T>(csp_chan<void_type, T> chan, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
        {
            return case_of(chan, (T msg) => check_void_task(handler(msg)), errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2> msg) => check_void_task(handler(msg.value1, msg.value2)), errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2, T3> msg) => check_void_task(handler(msg.value1, msg.value2, msg.value3)), errHandler, lostMsg);
        }

        public receive_mail case_of(csp_chan<void_type, void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
        {
            return case_of(chan, (void_type _) => check_void_task(handler()), errHandler, lostMsg);
        }

        public receive_mail case_of<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            return case_of(chan, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail case_of<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail case_of<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail case_of<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            return case_of(chan, handler, null, errHandler, lostMsg);
        }

        public receive_mail case_of<R>(csp_chan<R, void_type> chan, Func<ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            return case_of(chan, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail case_of<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail case_of<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail case_of<R, T>(csp_chan<R, T> chan, Func<T, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            return case_of(chan, null, handler, errHandler, lostMsg);
        }

        private receive_mail case_of<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler, Func<chan_state, Task<bool>> errHandler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null == _mutex)
                {
                    try
                    {
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        while (_run)
                        {
                            csp_wait_wrap<R, T> recvRes = await csp_wait(chan, lostMsg);
                            if (chan_state.ok == recvRes.state)
                            {
                                try
                                {
                                    recvRes.complete(null != handler ? await handler(recvRes.msg) : await gohandler(recvRes.msg));
                                }
                                catch (csp_fail_exception)
                                {
                                    recvRes.fail();
                                }
                                catch (stop_exception)
                                {
                                    recvRes.fail();
                                    throw;
                                }
                                catch (System.Exception)
                                {
                                    recvRes.fail();
                                    throw;
                                }
                            }
                            else if (null != errHandler && await errHandler(recvRes.state))
                            {
                                break;
                            }
                            else if (chan_state.closed == recvRes.state)
                            {
                                break;
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self._mailboxMap = null;
                        self._genLocal = null;
                    }
                }
                else
                {
                    chan_notify_sign ntfSign = new chan_notify_sign();
                    try
                    {
                        self.lock_suspend_();
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        nil_chan<chan_state> waitHasChan = new nil_chan<chan_state>();
                        Action<chan_state> waitHasNtf = waitHasChan.wrap();
                        async_result_wrap<csp_wait_wrap<R, T>> recvRes = new async_result_wrap<csp_wait_wrap<R, T>>();
                        chan.async_append_recv_notify(waitHasNtf, ntfSign);
                        while (_run)
                        {
                            await chan_receive(waitHasChan);
                            await mutex_lock_shared(_mutex);
                            try
                            {
                                recvRes.value1 = csp_wait_wrap<R, T>.def;
                                try
                                {
                                    chan.async_try_recv_and_append_notify(self.unsafe_async_result(recvRes), waitHasNtf, ntfSign);
                                    await self.async_wait();
                                }
                                catch (stop_exception)
                                {
                                    chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                                    await self.async_wait();
                                    if (chan_state.ok == recvRes.value1.state)
                                    {
                                        if (null != lostMsg)
                                        {
                                            lostMsg.set(recvRes.value1);
                                        }
                                        else
                                        {
                                            recvRes.value1.fail();
                                        }
                                    }
                                    throw;
                                }
                                try
                                {
                                    recvRes.value1.result?.start_invoke_timer(self);
                                    await self.unlock_suspend_();
                                    if (chan_state.ok == recvRes.value1.state)
                                    {
                                        try
                                        {
                                            recvRes.value1.complete(null != handler ? await handler(recvRes.value1.msg) : await gohandler(recvRes.value1.msg));
                                        }
                                        catch (csp_fail_exception)
                                        {
                                            recvRes.value1.fail();
                                        }
                                        catch (stop_exception)
                                        {
                                            recvRes.value1.fail();
                                            throw;
                                        }
                                        catch (System.Exception)
                                        {
                                            recvRes.value1.fail();
                                            throw;
                                        }
                                    }
                                    else if (null != errHandler && await errHandler(recvRes.value1.state))
                                    {
                                        break;
                                    }
                                    else if (chan_state.closed == recvRes.value1.state)
                                    {
                                        break;
                                    }
                                }
                                finally
                                {
                                    self.lock_suspend_();
                                }
                            }
                            finally
                            {
                                await mutex_unlock_shared(_mutex);
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self.lock_stop_();
                        self._mailboxMap = null;
                        self._genLocal = null;
                        chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                        await self.async_wait();
                        await self.unlock_suspend_and_stop_();
                    }
                }
            });
            return this;
        }

        public receive_mail timed_case_of<T>(csp_chan<void_type, T> chan, int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (T msg) => check_void_task(handler(msg)), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2> msg) => check_void_task(handler(msg.value1, msg.value2)), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2, T3> msg) => check_void_task(handler(msg.value1, msg.value2, msg.value3)), errHandler, lostMsg);
        }

        public receive_mail timed_case_of(csp_chan<void_type, void_type> chan, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (void_type _) => check_void_task(handler()), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R>(csp_chan<R, void_type> chan, int ms, Func<Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R, T>(csp_chan<R, T> chan, int ms, Func<T, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            return timed_case_of(chan, ms, handler, null, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R>(csp_chan<R, void_type> chan, int ms, Func<ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return timed_case_of(chan, ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail timed_case_of<R, T>(csp_chan<R, T> chan, int ms, Func<T, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            return timed_case_of(chan, ms, null, handler, errHandler, lostMsg);
        }

        private receive_mail timed_case_of<R, T>(csp_chan<R, T> chan, int ms, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler, Func<chan_state, Task<bool>> errHandler, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null == _mutex)
                {
                    csp_wait_wrap<R, T> recvRes = await csp_timed_wait(chan, ms, lostMsg);
                    try
                    {
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        if (chan_state.ok == recvRes.state)
                        {
                            try
                            {
                                recvRes.complete(null != handler ? await handler(recvRes.msg) : await gohandler(recvRes.msg));
                            }
                            catch (csp_fail_exception)
                            {
                                recvRes.fail();
                            }
                            catch (stop_exception)
                            {
                                recvRes.fail();
                                throw;
                            }
                            catch (System.Exception)
                            {
                                recvRes.fail();
                                throw;
                            }
                        }
                        else if (null != errHandler && await errHandler(recvRes.state)) { }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self._mailboxMap = null;
                        self._genLocal = null;
                    }
                }
                else
                {
                    chan_notify_sign ntfSign = new chan_notify_sign();
                    try
                    {
                        self.lock_suspend_();
                        self._mailboxMap = _children.parent._mailboxMap;
                        self._genLocal = _children.parent._genLocal;
                        long endTick = system_tick.get_tick_ms() + ms;
                        while (_run)
                        {
                            chan_state result = chan_state.undefined;
                            chan.async_append_recv_notify(self.unsafe_async_callback((chan_state state) => result = state), ntfSign, ms);
                            await self.async_wait();
                            await mutex_lock_shared(_mutex);
                            try
                            {
                                if (chan_state.overtime != result)
                                {
                                    csp_wait_wrap<R, T> recvRes = await csp_try_wait(chan, lostMsg);
                                    try
                                    {
                                        await self.unlock_suspend_();
                                        if (chan_state.ok == recvRes.state)
                                        {
                                            try
                                            {
                                                recvRes.complete(null != handler ? await handler(recvRes.msg) : await gohandler(recvRes.msg)); break;
                                            }
                                            catch (csp_fail_exception)
                                            {
                                                recvRes.fail();
                                            }
                                            catch (stop_exception)
                                            {
                                                recvRes.fail();
                                                throw;
                                            }
                                            catch (System.Exception)
                                            {
                                                recvRes.fail();
                                                throw;
                                            }
                                        }
                                        else if ((null != errHandler && await errHandler(recvRes.state)) || chan_state.closed == recvRes.state) { break; }
                                        if (0 <= ms && 0 >= (ms = (int)(endTick - system_tick.get_tick_ms())))
                                        {
                                            ms = -1;
                                            if (null == errHandler || await errHandler(chan_state.overtime))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        self.lock_suspend_();
                                    }
                                }
                                else
                                {
                                    ms = -1;
                                    if (null == errHandler || await errHandler(chan_state.overtime))
                                    {
                                        break;
                                    }
                                }
                            }
                            finally
                            {
                                await mutex_unlock_shared(_mutex);
                            }
                        }
                    }
                    catch (message_stop_current_exception) { }
                    catch (message_stop_all_exception)
                    {
                        _run = false;
                    }
                    finally
                    {
                        self.lock_stop_();
                        self._mailboxMap = null;
                        self._genLocal = null;
                        chan.async_remove_recv_notify(self.unsafe_async_ignore<chan_state>(), ntfSign);
                        await self.async_wait();
                        await self.unlock_suspend_and_stop_();
                    }
                }
            });
            return this;
        }

        public receive_mail try_case_of<T>(csp_chan<void_type, T> chan, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
        {
            return try_case_of(chan, (T msg) => check_void_task(handler(msg)), errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2> msg) => check_void_task(handler(msg.value1, msg.value2)), errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2, T3> msg) => check_void_task(handler(msg.value1, msg.value2, msg.value3)), errHandler, lostMsg);
        }

        public receive_mail try_case_of(csp_chan<void_type, void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
        {
            return try_case_of(chan, (void_type _) => check_void_task(handler()), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            return try_case_of(chan, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            return try_case_of(chan, handler, null, errHandler, lostMsg);
        }

        public receive_mail try_case_of<R>(csp_chan<R, void_type> chan, Func<ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            return try_case_of(chan, (void_type _) => handler(), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            return try_case_of(chan, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg);
        }

        public receive_mail try_case_of<R, T>(csp_chan<R, T> chan, Func<T, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            return try_case_of(chan, null, handler, errHandler, lostMsg);
        }

        private receive_mail try_case_of<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<T, ValueTask<R>> gohandler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            _children.go(async delegate ()
            {
                generator self = generator.self;
                if (null != _mutex)
                {
                    await mutex_lock_shared(_mutex);
                }
                try
                {
                    self._mailboxMap = _children.parent._mailboxMap;
                    self._genLocal = _children.parent._genLocal;
                    csp_wait_wrap<R, T> recvRes = await csp_try_wait(chan, lostMsg);
                    if (chan_state.ok == recvRes.state)
                    {
                        try
                        {
                            recvRes.complete(null != handler ? await handler(recvRes.msg) : await gohandler(recvRes.msg));
                        }
                        catch (csp_fail_exception)
                        {
                            recvRes.fail();
                        }
                        catch (stop_exception)
                        {
                            recvRes.fail();
                            throw;
                        }
                        catch (System.Exception)
                        {
                            recvRes.fail();
                            throw;
                        }
                    }
                    else if (null != errHandler && await errHandler(recvRes.state)) { }
                }
                catch (message_stop_current_exception) { }
                catch (message_stop_all_exception)
                {
                    _run = false;
                }
                finally
                {
                    self._mailboxMap = null;
                    self._genLocal = null;
                    if (null != _mutex)
                    {
                        await mutex_unlock_shared(_mutex);
                    }
                }
            });
            return this;
        }

        public receive_mail case_of<T>(int id, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return case_of(self_mailbox<T>(id), handler, errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2>(int id, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_of(self_mailbox<tuple<T1, T2>>(id), handler, errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2, T3>(int id, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_of(self_mailbox<tuple<T1, T2, T3>>(id), handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T>(int id, int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return timed_case_of(self_mailbox<T>(id), ms, handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2>(int id, int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return timed_case_of(self_mailbox<tuple<T1, T2>>(id), ms, handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2, T3>(int id, int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return timed_case_of(self_mailbox<tuple<T1, T2, T3>>(id), ms, handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T>(int id, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return try_case_of(self_mailbox<T>(id), handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2>(int id, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return try_case_of(self_mailbox<tuple<T1, T2>>(id), handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2, T3>(int id, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return try_case_of(self_mailbox<tuple<T1, T2, T3>>(id), handler, errHandler, lostMsg);
        }

        public receive_mail case_of<T>(Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2>(Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail case_of<T1, T2, T3>(Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T>(int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return timed_case_of(0, ms, handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2>(int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return timed_case_of(0, ms, handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of<T1, T2, T3>(int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return timed_case_of(0, ms, handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T>(Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return try_case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2>(Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return try_case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of<T1, T2, T3>(Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return try_case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail case_of(int id, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return case_of(self_mailbox<void_type>(id), handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of(int id, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return timed_case_of(self_mailbox<void_type>(id), ms, handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of(int id, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return try_case_of(self_mailbox<void_type>(id), handler, errHandler, lostMsg);
        }

        public receive_mail case_of(Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return case_of(0, handler, errHandler, lostMsg);
        }

        public receive_mail timed_case_of(int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return timed_case_of(0, ms, handler, errHandler, lostMsg);
        }

        public receive_mail try_case_of(Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            return try_case_of(0, handler, errHandler, lostMsg);
        }

        public async Task end()
        {
            while (0 != _children.count)
            {
                await _children.wait_any();
                if (!_run)
                {
                    if (null != _mutex)
                    {
                        await mutex_lock(_mutex);
                        await _children.stop();
                        await mutex_unlock(_mutex);
                    }
                    else
                    {
                        await _children.stop();
                    }
                }
            }
        }

        public Func<Task> wrap()
        {
            return end;
        }

        static public void stop_current()
        {
            Debug.Assert(null != self && null != self.parent && self.parent._mailboxMap == self._mailboxMap, "不正确的 stop_current 调用!");
            throw new message_stop_current_exception();
        }

        static public void stop_all()
        {
            Debug.Assert(null != self && null != self.parent && self.parent._mailboxMap == self._mailboxMap, "不正确的 stop_all 调用!");
            throw new message_stop_all_exception();
        }
    }

    static public receive_mail receive(bool forceStopAll = true)
    {
        return new receive_mail(forceStopAll);
    }

    public struct select_chans
    {
        internal bool _random;
        internal bool _whenEnable;
        internal LinkedList<select_chan_base> _chans;
        internal LinkedListNode<select_chan_base> _lastChansNode;
        internal unlimit_chan<tuple<chan_state, select_chan_base>> _selectChans;

        public select_chans case_recv_mail<T>(Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return case_receive(self_mailbox<T>(), handler, errHandler, lostMsg);
        }

        public select_chans case_recv_mail<T1, T2>(Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_receive(self_mailbox<tuple<T1, T2>>(), handler, errHandler, lostMsg);
        }

        public select_chans case_recv_mail<T1, T2, T3>(Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_receive(self_mailbox<tuple<T1, T2, T3>>(), handler, errHandler, lostMsg);
        }

        public select_chans case_recv_mail<T>(int id, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return case_receive(self_mailbox<T>(id), handler, errHandler, lostMsg);
        }

        public select_chans case_recv_mail<T1, T2>(int id, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_receive(self_mailbox<tuple<T1, T2>>(id), handler, errHandler, lostMsg);
        }

        public select_chans case_recv_mail<T1, T2, T3>(int id, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_receive(self_mailbox<tuple<T1, T2, T3>>(id), handler, errHandler, lostMsg);
        }

        public select_chans case_receive<T>(chan<T> chan, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T1, T2>(chan<tuple<T1, T2>> chan, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2> msg) => handler(msg.value1, msg.value2), null, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T1, T2, T3>(chan<tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), null, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive(chan<void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((void_type _) => handler(), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<T>(chan<T> chan, async_result_wrap<T> msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(msg, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<T>(chan<T> chan, T msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(msg, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send(chan<void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(default(void_type), handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T1, T2>(broadcast_chan<tuple<T1, T2>> chan, Func<T1, T2, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2> msg) => handler(msg.value1, msg.value2), token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T1, T2, T3>(broadcast_chan<tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T>(broadcast_chan<T> chan, Func<T, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(handler, token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive(broadcast_chan<void_type> chan, Func<Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((void_type _) => handler(), token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R, T>(csp_chan<R, T> chan, Func<T, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R>(csp_chan<R, void_type> chan, Func<Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((void_type _) => handler(), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R, T>(csp_chan<R, T> chan, Func<T, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, Func<T1, T2, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<R>(csp_chan<R, void_type> chan, Func<ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((void_type _) => handler(), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T>(csp_chan<void_type, T> chan, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((T msg) => check_void_task(handler(msg)), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2> msg) => check_void_task(handler(msg.value1, msg.value2)), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((tuple<T1, T2, T3> msg) => check_void_task(handler(msg.value1, msg.value2, msg.value3)), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_receive(csp_chan<void_type, void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader((void_type _) => check_void_task(handler()), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<R, T>(csp_chan<R, T> chan, async_result_wrap<T> msg, Func<R, Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(msg, handler, errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<R, T>(csp_chan<R, T> chan, T msg, Func<R, Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(msg, handler, errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<R>(csp_chan<R, void_type> chan, Func<R, Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<R> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(default(void_type), handler, errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<T>(csp_chan<void_type, T> chan, async_result_wrap<T> msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<void_type> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(msg, (void_type _) => handler(), errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send<T>(csp_chan<void_type, T> chan, T msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<void_type> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(msg, (void_type _) => handler(), errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_send(csp_chan<void_type, void_type> chan, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<void_type> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(default(void_type), (void_type _) => handler(), errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_recv_mail<T>(int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return case_timed_receive(self_mailbox<T>(), ms, handler, errHandler, lostMsg);
        }

        public select_chans case_timed_recv_mail<T1, T2>(int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_timed_receive(self_mailbox<tuple<T1, T2>>(), ms, handler, errHandler, lostMsg);
        }

        public select_chans case_timed_recv_mail<T1, T2, T3>(int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_timed_receive(self_mailbox<tuple<T1, T2, T3>>(), ms, handler, errHandler, lostMsg);
        }

        public select_chans case_timed_recv_mail<T>(int id, int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            return case_timed_receive(self_mailbox<T>(id), ms, handler, errHandler, lostMsg);
        }

        public select_chans case_timed_recv_mail<T1, T2>(int id, int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            return case_timed_receive(self_mailbox<tuple<T1, T2>>(id), ms, handler, errHandler, lostMsg);
        }

        public select_chans case_timed_recv_mail<T1, T2, T3>(int id, int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            return case_timed_receive(self_mailbox<tuple<T1, T2, T3>>(id), ms, handler, errHandler, lostMsg);
        }

        public select_chans case_timed_receive<T>(chan<T> chan, int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T1, T2>(chan<tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), null, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T1, T2, T3>(chan<tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), null, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive(chan<void_type> chan, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (void_type _) => handler(), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<T>(chan<T> chan, int ms, async_result_wrap<T> msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, msg, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<T>(chan<T> chan, int ms, T msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, msg, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send(chan<void_type> chan, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, default(void_type), handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T1, T2>(broadcast_chan<tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T1, T2, T3>(broadcast_chan<tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<tuple<T1, T2, T3>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T>(broadcast_chan<T> chan, int ms, Func<T, Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, handler, token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive(broadcast_chan<void_type> chan, int ms, Func<Task> handler, broadcast_token token = null, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (void_type _) => handler(), token, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R, T>(csp_chan<R, T> chan, int ms, Func<T, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R>(csp_chan<R, void_type> chan, int ms, Func<Task<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (void_type _) => handler(), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R, T>(csp_chan<R, T> chan, int ms, Func<T, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, T>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, handler, errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R, T1, T2>(csp_chan<R, tuple<T1, T2>> chan, int ms, Func<T1, T2, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2> msg) => handler(msg.value1, msg.value2), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R, T1, T2, T3>(csp_chan<R, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, tuple<T1, T2, T3>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2, T3> msg) => handler(msg.value1, msg.value2, msg.value3), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<R>(csp_chan<R, void_type> chan, int ms, Func<ValueTask<R>> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<R, void_type>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (void_type _) => handler(), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T>(csp_chan<void_type, T> chan, int ms, Func<T, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, T>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (T msg) => check_void_task(handler(msg)), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T1, T2>(csp_chan<void_type, tuple<T1, T2>> chan, int ms, Func<T1, T2, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2> msg) => check_void_task(handler(msg.value1, msg.value2)), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive<T1, T2, T3>(csp_chan<void_type, tuple<T1, T2, T3>> chan, int ms, Func<T1, T2, T3, Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, tuple<T1, T2, T3>>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (tuple<T1, T2, T3> msg) => check_void_task(handler(msg.value1, msg.value2, msg.value3)), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_receive(csp_chan<void_type, void_type> chan, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, chan_lost_msg<csp_wait_wrap<void_type, void_type>> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_reader(ms, (void_type _) => check_void_task(handler()), errHandler, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<R, T>(csp_chan<R, T> chan, int ms, async_result_wrap<T> msg, Func<R, Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, msg, handler, errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<R, T>(csp_chan<R, T> chan, int ms, T msg, Func<R, Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<R> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, msg, handler, errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<R>(csp_chan<R, void_type> chan, int ms, Func<R, Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<R> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, default(void_type), handler, errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<T>(csp_chan<void_type, T> chan, int ms, async_result_wrap<T> msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<void_type> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, msg, (void_type _) => handler(), errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send<T>(csp_chan<void_type, T> chan, int ms, T msg, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<void_type> lostRes = null, chan_lost_msg<T> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, msg, (void_type _) => handler(), errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public select_chans case_timed_send(csp_chan<void_type, void_type> chan, int ms, Func<Task> handler, Func<chan_state, Task<bool>> errHandler = null, Action<void_type> lostRes = null, chan_lost_msg<void_type> lostMsg = null)
        {
            select_chan_base selectChan = chan.make_select_writer(ms, default(void_type), (void_type _) => handler(), errHandler, lostRes, lostMsg); selectChan.enable = _whenEnable;
            return new select_chans { _random = _random, _whenEnable = true, _chans = _chans, _lastChansNode = _chans.AddLast(selectChan), _selectChans = _selectChans };
        }

        public bool effective()
        {
            return null != _lastChansNode;
        }

        public void enable(bool enable)
        {
            _lastChansNode.Value.enable = enable;
        }

        public void remove()
        {
            _chans.Remove(_lastChansNode);
        }

        public void restore()
        {
            _chans.AddLast(_lastChansNode);
        }

        public select_chans when(bool when)
        {
            return new select_chans { _random = _random, _whenEnable = when, _chans = _chans, _selectChans = _selectChans };
        }

        static private select_chan_base[] shuffle(LinkedList<select_chan_base> chans)
        {
            int count = chans.Count;
            select_chan_base[] shuffChans = new select_chan_base[count];
            chans.CopyTo(shuffChans, 0);
            mt19937 randGen = mt19937.global;
            for (int i = 0; i < count; i++)
            {
                int rand = randGen.Next(i, count);
                select_chan_base t = shuffChans[rand];
                shuffChans[rand] = shuffChans[i];
                shuffChans[i] = t;
            }
            return shuffChans;
        }
        /// <summary>
        /// 循环
        /// </summary>
        /// <param name="eachAfterDo">每个循环后执行的任务</param>
        /// <returns>是否完成循环</returns>
        public async Task<bool> loop(Func<Task> eachAfterDo = null)
        {
            generator this_ = self;
            LinkedList<select_chan_base> chans = _chans;
            unlimit_chan<tuple<chan_state, select_chan_base>> selectChans = _selectChans;
            try
            {
                this_.lock_suspend_and_stop_();
                if (null == this_._topSelectChans)
                {
                    this_._topSelectChans = new LinkedList<LinkedList<select_chan_base>>();
                }
                this_._topSelectChans.AddFirst(chans);
                if (_random)
                {
                    select_chan_base[] shuffChans = await send_task(() => shuffle(chans));
                    int len = shuffChans.Length;
                    for (int i = 0; i < len; i++)
                    {
                        select_chan_base chan = shuffChans[i];
                        chan.ntfSign._selectOnce = false;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                    }
                }
                else
                {
                    for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                    {
                        select_chan_base chan = it.Value;
                        chan.ntfSign._selectOnce = false;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                    }
                }
                this_.unlock_stop_();
                int count = chans.Count;
                bool selected = false;
                Func<Task> stepOne = null == eachAfterDo ? (Func<Task>)null : delegate ()
                {
                    selected = true;
                    return non_async();
                };
                while (0 != count)
                {
                    tuple<chan_state, select_chan_base> selectedChan = (await chan_receive(selectChans)).msg;
                    if (chan_state.ok != selectedChan.value1)
                    {
                        if (await selectedChan.value2.errInvoke(selectedChan.value1))
                        {
                            count--;
                        }
                        continue;
                    }
                    else if (selectedChan.value2.disabled())
                    {
                        continue;
                    }
                    try
                    {
                        select_chan_state selState = await selectedChan.value2.invoke(stepOne);
                        if (!selState.nextRound)
                        {
                            count--;
                        }
                    }
                    catch (select_stop_current_exception)
                    {
                        count--;
                        await selectedChan.value2.end();
                    }
                    if (selected)
                    {
                        try
                        {
                            selected = false;
                            await this_.unlock_suspend_();
                            await eachAfterDo();
                        }
                        finally
                        {
                            this_.lock_suspend_();
                        }
                    }
                }
                return true;
            }
            catch (select_stop_all_exception)
            {
                return false;
            }
            finally
            {
                this_.lock_stop_();
                this_._topSelectChans.RemoveFirst();
                for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                {
                    await it.Value.end();
                }
                selectChans.clear();
                await this_.unlock_suspend_and_stop_();
            }
        }
        /// <summary>
        /// 定时循环
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="eachAfterDo">每个循环后执行的任务</param>
        /// <returns>是否完成循环</returns>
        /// <remarks>
        /// </remarks>
        public async Task<bool> timed_loop(int ms, Func<bool, Task> eachAfterDo = null)
        {
            generator this_ = self;
            LinkedList<select_chan_base> chans = _chans;
            unlimit_chan<tuple<chan_state, select_chan_base>> selectChans = _selectChans;
            async_timer timer = ms >= 0 ? new async_timer(this_.strand) : null;
            try
            {
                this_.lock_suspend_and_stop_();
                if (null == this_._topSelectChans)
                {
                    this_._topSelectChans = new LinkedList<LinkedList<select_chan_base>>();
                }
                this_._topSelectChans.AddFirst(chans);
                if (_random)
                {
                    select_chan_base[] shuffChans = await send_task(() => shuffle(chans));
                    int len = shuffChans.Length;
                    for (int i = 0; i < len; i++)
                    {
                        select_chan_base chan = shuffChans[i];
                        chan.ntfSign._selectOnce = false;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                        if (0 == ms)
                        {
                            shared_strand chanStrand = chan.channel().self_strand();
                            if (chanStrand != this_.strand)
                            {
                                chanStrand.post(this_.unsafe_async_result());
                                await this_.async_wait();
                            }
                        }
                    }
                }
                else
                {
                    for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                    {
                        select_chan_base chan = it.Value;
                        chan.ntfSign._selectOnce = false;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                        if (0 == ms)
                        {
                            shared_strand chanStrand = chan.channel().self_strand();
                            if (chanStrand != this_.strand)
                            {
                                chanStrand.post(this_.unsafe_async_result());
                                await this_.async_wait();
                            }
                        }
                    }
                }
                this_.unlock_stop_();
                int timerCount = 0;
                int count = chans.Count;
                bool selected = false;
                Func<Task> stepOne = delegate ()
                {
                    timer?.Cancel();
                    selected = true;
                    return non_async();
                };
                if (null != timer)
                {
                    int timerId = ++timerCount;
                    timer.timeout(ms, () => selectChans.post(tuple.make((chan_state)timerId, default(select_chan_base))));
                }
                while (0 != count)
                {
                    tuple<chan_state, select_chan_base> selectedChan = (await chan_receive(selectChans)).msg;
                    if (null != selectedChan.value2)
                    {
                        if (chan_state.ok != selectedChan.value1)
                        {
                            if (await selectedChan.value2.errInvoke(selectedChan.value1))
                            {
                                count--;
                            }
                            continue;
                        }
                        else if (selectedChan.value2.disabled())
                        {
                            continue;
                        }
                        try
                        {
                            select_chan_state selState = await selectedChan.value2.invoke(stepOne);
                            if (!selState.nextRound)
                            {
                                count--;
                            }
                            else if (0 == ms)
                            {
                                shared_strand chanStrand = selectedChan.value2.channel().self_strand();
                                if (chanStrand != this_.strand)
                                {
                                    chanStrand.post(this_.unsafe_async_result());
                                    await this_.async_wait();
                                }
                            }
                        }
                        catch (select_stop_current_exception)
                        {
                            count--;
                            await selectedChan.value2.end();
                        }
                    }
                    else if (timerCount == (int)selectedChan.value1)
                    {
                        selected = true;
                    }
                    else
                    {
                        continue;
                    }
                    if (selected)
                    {
                        try
                        {
                            selected = false;
                            await this_.unlock_suspend_();
                            if (null != eachAfterDo)
                            {
                                await eachAfterDo(null != selectedChan.value2);
                            }
                            if (null != timer)
                            {
                                int timerId = ++timerCount;
                                timer.timeout(ms, () => selectChans.post(tuple.make((chan_state)timerId, default(select_chan_base))));
                            }
                        }
                        finally
                        {
                            this_.lock_suspend_();
                        }
                    }
                }
                return true;
            }
            catch (select_stop_all_exception)
            {
                return false;
            }
            finally
            {
                this_.lock_stop_();
                timer?.Cancel();
                this_._topSelectChans.RemoveFirst();
                for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                {
                    await it.Value.end();
                }
                selectChans.clear();
                await this_.unlock_suspend_and_stop_();
            }
        }
        /// <summary>
        /// 结束
        /// </summary>
        /// <returns>是否完成结束</returns>
        /// <remarks>
        /// </remarks>
        public async Task<bool> end()
        {
            generator this_ = self;
            LinkedList<select_chan_base> chans = _chans;
            unlimit_chan<tuple<chan_state, select_chan_base>> selectChans = _selectChans;
            bool selected = false;
            try
            {
                this_.lock_suspend_and_stop_();
                if (null == this_._topSelectChans)
                {
                    this_._topSelectChans = new LinkedList<LinkedList<select_chan_base>>();
                }
                this_._topSelectChans.AddFirst(chans);
                if (_random)
                {
                    select_chan_base[] shuffChans = await send_task(() => shuffle(chans));
                    int len = shuffChans.Length;
                    for (int i = 0; i < len; i++)
                    {
                        select_chan_base chan = shuffChans[i];
                        chan.ntfSign._selectOnce = false;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                    }
                }
                else
                {
                    for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                    {
                        select_chan_base chan = it.Value;
                        chan.ntfSign._selectOnce = true;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                    }
                }
                this_.unlock_stop_();
                int count = chans.Count;
                while (0 != count)
                {
                    tuple<chan_state, select_chan_base> selectedChan = (await chan_receive(selectChans)).msg;
                    if (chan_state.ok != selectedChan.value1)
                    {
                        if (await selectedChan.value2.errInvoke(selectedChan.value1))
                        {
                            count--;
                        }
                        continue;
                    }
                    else if (selectedChan.value2.disabled())
                    {
                        continue;
                    }
                    try
                    {
                        select_chan_state selState = await selectedChan.value2.invoke(async delegate ()
                        {
                            for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                            {
                                if (selectedChan.value2 != it.Value)
                                {
                                    await it.Value.end();
                                }
                            }
                            selected = true;
                        });
                        if (!selState.failed)
                        {
                            break;
                        }
                        else if (!selState.nextRound)
                        {
                            count--;
                        }
                    }
                    catch (select_stop_current_exception)
                    {
                        if (selected)
                        {
                            break;
                        }
                        else
                        {
                            count--;
                            await selectedChan.value2.end();
                        }
                    }
                }
            }
            catch (select_stop_all_exception) { }
            finally
            {
                this_.lock_stop_();
                this_._topSelectChans.RemoveFirst();
                if (!selected)
                {
                    for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                    {
                        await it.Value.end();
                    }
                }
                selectChans.clear();
                await this_.unlock_suspend_and_stop_();
            }
            return selected;
        }
        /// <summary>
        /// 定时
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <returns>是否完成定时</returns>
        /// <remarks>
        /// </remarks>
        public async Task<bool> timed(int ms)
        {
            generator this_ = self;
            LinkedList<select_chan_base> chans = _chans;
            unlimit_chan<tuple<chan_state, select_chan_base>> selectChans = _selectChans;
            async_timer timer = ms >= 0 ? new async_timer(this_.strand) : null;
            bool selected = false;
            try
            {
                this_.lock_suspend_and_stop_();
                if (null == this_._topSelectChans)
                {
                    this_._topSelectChans = new LinkedList<LinkedList<select_chan_base>>();
                }
                this_._topSelectChans.AddFirst(chans);
                if (_random)
                {
                    select_chan_base[] shuffChans = await send_task(() => shuffle(chans));
                    int len = shuffChans.Length;
                    for (int i = 0; i < len; i++)
                    {
                        select_chan_base chan = shuffChans[i];
                        chan.ntfSign._selectOnce = false;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                        if (0 == ms)
                        {
                            shared_strand chanStrand = chan.channel().self_strand();
                            if (chanStrand != this_.strand)
                            {
                                chanStrand.post(this_.unsafe_async_result());
                                await this_.async_wait();
                            }
                        }
                    }
                }
                else
                {
                    for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                    {
                        select_chan_base chan = it.Value;
                        chan.ntfSign._selectOnce = true;
                        chan.nextSelect = (chan_state state) => selectChans.post(tuple.make(state, chan));
                        chan.begin(this_);
                        if (0 == ms)
                        {
                            shared_strand chanStrand = chan.channel().self_strand();
                            if (chanStrand != this_.strand)
                            {
                                chanStrand.post(this_.unsafe_async_result());
                                await this_.async_wait();
                            }
                        }
                    }
                }
                this_.unlock_stop_();
                int count = chans.Count;
                timer?.timeout(ms, selectChans.wrap_default());
                while (0 != count)
                {
                    tuple<chan_state, select_chan_base> selectedChan = (await chan_receive(selectChans)).msg;
                    if (null != selectedChan.value2)
                    {
                        if (chan_state.ok != selectedChan.value1)
                        {
                            if (await selectedChan.value2.errInvoke(selectedChan.value1))
                            {
                                count--;
                            }
                            continue;
                        }
                        else if (selectedChan.value2.disabled())
                        {
                            continue;
                        }
                        try
                        {
                            select_chan_state selState = await selectedChan.value2.invoke(async delegate ()
                            {
                                timer?.Cancel();
                                for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                                {
                                    if (selectedChan.value2 != it.Value)
                                    {
                                        await it.Value.end();
                                    }
                                }
                                selected = true;
                            });
                            if (!selState.failed)
                            {
                                break;
                            }
                            else if (!selState.nextRound)
                            {
                                count--;
                            }
                        }
                        catch (select_stop_current_exception)
                        {
                            if (selected)
                            {
                                break;
                            }
                            else
                            {
                                count--;
                                await selectedChan.value2.end();
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (select_stop_all_exception) { }
            finally
            {
                this_.lock_stop_();
                timer?.Cancel();
                this_._topSelectChans.RemoveFirst();
                if (!selected)
                {
                    for (LinkedListNode<select_chan_base> it = chans.First; null != it; it = it.Next)
                    {
                        await it.Value.end();
                    }
                }
                selectChans.clear();
                await this_.unlock_suspend_and_stop_();
            }
            return true;
        }
        /// <summary>
        /// 再次
        /// </summary>
        /// <returns>是否完成再次</returns>
        /// <remarks>
        /// </remarks>
        public Task<bool> again()
        {
            return end();
        }
        /// <summary>
        /// 定时再次
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <returns>是否完成定时再次</returns>
        /// <remarks>
        /// </remarks>
        public Task<bool> timed_again(int ms)
        {
            return timed(ms);
        }
        /// <summary>
        /// 包装循环
        /// </summary>
        /// <returns>包装后的循环</returns>
        /// <remarks>
        /// </remarks>
        public Func<Func<Task>, Task<bool>> wrap_loop()
        {
            return loop;
        }
        /// <summary>
        /// 包装
        /// </summary>
        /// <returns>包装后的任务</returns>
        /// <remarks>
        /// </remarks>
        public Func<Task<bool>> wrap()
        {
            return end;
        }
        /// <summary>
        /// 包装定时
        /// </summary>
        /// <returns>包装后的定时任务</returns>
        /// <remarks>
        /// </remarks>
        public Func<int, Task<bool>> wrap_timed()
        {
            return timed;
        }
        /// <summary>
        /// 包装循环
        /// </summary>
        /// <param name="eachAferDo">每个循环后执行的任务</param>
        /// <returns>包装后的循环任务</returns>
        /// <remarks>
        /// </remarks>
        public Func<Task<bool>> wrap_loop(Func<Task> eachAferDo)
        {
            return functional.bind(loop, eachAferDo);
        }
        /// <summary>
        /// 包装定时
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <returns>包装后的定时任务</returns>
        /// <remarks>
        /// </remarks>
        public Func<Task<bool>> wrap_timed(int ms)
        {
            return functional.bind(timed, ms);
        }
        /// <summary>
        /// 停止所有
        /// </summary>
        /// <remarks>
        /// </remarks>
        static public void stop_all()
        {
            Debug.Assert(null != self && null != self._topSelectChans && 0 != self._topSelectChans.Count, "不正确的 stop_all 调用!");
            throw new select_stop_all_exception();
        }
        /// <summary>
        /// 停止当前
        /// </summary>
        /// <remarks>
        /// </remarks>
        static public void stop_current()
        {
            Debug.Assert(null != self && null != self._topSelectChans && 0 != self._topSelectChans.Count, "不正确的 stop_current 调用!");
            throw new select_stop_current_exception();
        }
        /// <summary>
        /// 禁用其他通道
        /// </summary>
        /// <param name="otherChan">其他通道</param>
        /// <param name="disable">是否禁用</param>
        /// <returns>是否完成禁用</returns>
        /// <remarks>
        /// </remarks>
        static public Task disable_other(chan_base otherChan, bool disable = true)
        {
            generator this_ = self;
            if (null != this_._topSelectChans && 0 != this_._topSelectChans.Count)
            {
                LinkedList<select_chan_base> currSelect = this_._topSelectChans.First.Value;
                for (LinkedListNode<select_chan_base> it = currSelect.First; null != it; it = it.Next)
                {
                    select_chan_base chan = it.Value;
                    if (chan.channel() == otherChan && chan.disabled() != disable)
                    {
                        if (disable)
                        {
                            return chan.end();
                        }
                        else
                        {
                            chan.begin(this_);
                        }
                    }
                }
            }
            return non_async();
        }
        /// <summary>
        /// 禁用其他通道接收
        /// </summary>
        /// <param name="otherChan">其他通道</param>
        /// <param name="disable">是否禁用</param>
        /// <returns>是否完成禁用</returns>
        /// <remarks>
        /// </remarks>
        static public Task disable_other_receive(chan_base otherChan, bool disable = true)
        {
            generator this_ = self;
            if (null != this_._topSelectChans && 0 != this_._topSelectChans.Count)
            {
                LinkedList<select_chan_base> currSelect = this_._topSelectChans.First.Value;
                for (LinkedListNode<select_chan_base> it = currSelect.First; null != it; it = it.Next)
                {
                    select_chan_base chan = it.Value;
                    if (chan.channel() == otherChan && chan.is_read() && chan.disabled() != disable)
                    {
                        if (disable)
                        {
                            return chan.end();
                        }
                        else
                        {
                            chan.begin(this_);
                        }
                    }
                }
            }
            return non_async();
        }
        /// <summary>
        /// 禁用其他通道发送
        /// </summary>
        /// <param name="otherChan">其他通道</param>
        /// <param name="disable">是否禁用</param>
        /// <returns>是否完成禁用</returns>
        /// <remarks>
        /// </remarks>
        static public Task disable_other_send(chan_base otherChan, bool disable = true)
        {
            generator this_ = self;
            if (null != this_._topSelectChans && 0 != this_._topSelectChans.Count)
            {
                LinkedList<select_chan_base> currSelect = this_._topSelectChans.First.Value;
                for (LinkedListNode<select_chan_base> it = currSelect.First; null != it; it = it.Next)
                {
                    select_chan_base chan = it.Value;
                    if (chan.channel() == otherChan && !chan.is_read() && chan.disabled() != disable)
                    {
                        if (disable)
                        {
                            return chan.end();
                        }
                        else
                        {
                            chan.begin(this_);
                        }
                    }
                }
            }
            return non_async();
        }
        /// <summary>
        /// 启用其他通道
        /// </summary>
        /// <param name="otherChan">其他通道</param>
        /// <returns>是否完成启用</returns>
        /// <remarks>
        /// </remarks>
        static public Task enable_other(chan_base otherChan)
        {
            return disable_other(otherChan, false);
        }
        /// <summary>
        /// 启用其他通道接收
        /// </summary>
        /// <param name="otherChan">其他通道</param>
        /// <returns>是否完成启用</returns>
        /// <remarks>
        /// </remarks>
        static public Task enable_other_receive(chan_base otherChan)
        {
            return disable_other_receive(otherChan, false);
        }
        /// <summary>
        /// 启用其他通道发送
        /// </summary>
        /// <param name="otherChan">其他通道</param>
        /// <returns>是否完成启用</returns>
        /// <remarks>
        /// </remarks>
        static public Task enable_other_send(chan_base otherChan)
        {
            return disable_other_send(otherChan, false);
        }
    }
    /// <summary>
    /// 选择器通道
    /// </summary>
    /// <param name="random">是否随机选择</param>
    /// <returns>选择器通道</returns>
    /// <remarks>
    /// </remarks>
    static public select_chans select(bool random = false)
    {
        return new select_chans { _random = random, _whenEnable = true, _chans = new LinkedList<select_chan_base>(), _selectChans = new unlimit_chan<tuple<chan_state, select_chan_base>>(self_strand()) };
    }
    /// <summary>
    /// 版本号
    /// </summary>
    /// <returns>版本号</returns>
    /// <remarks>
    /// </remarks>
    static public System.Version version
    {
        get
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
    /// <summary>
    /// 异常钩子
    /// </summary>
    /// <param name="value">异常钩子</param>
    /// <remarks>
    /// </remarks>
    static public Action<System.Exception> hook_exception
    {
        set
        {
            _hookException = value;
        }
    }
    /// <summary>
    /// 子生成器
    /// </summary>
    /// <remarks>
    /// 子生成器用于在当前生成器中创建新的生成器。
    /// </remarks>
    public class child : generator
    {
        bool _isFree;
        internal children _childrenMgr;
        internal LinkedListNode<child> _childNode;

        child(children childrenMgr, bool isFree = false) : base()
        {
            _isFree = isFree;
            _childrenMgr = childrenMgr;
        }
        /// <summary>
        /// 创建子生成器
        /// </summary>
        /// <param name="childrenMgr">子生成器管理器</param>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成处理程序</param>
        /// <param name="suspendHandler">挂起处理程序</param>
        /// <returns>子生成器</returns>
        /// <remarks>
        /// </remarks>
        static internal child make(children childrenMgr, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            return (child)(new child(childrenMgr)).init(strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 创建子生成器
        /// </summary>
        /// <param name="childrenMgr">子生成器管理器</param>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成处理程序</param>
        /// <param name="suspendHandler">挂起处理程序</param>
        /// <returns>子生成器</returns>
        /// <remarks>
        /// </remarks>
        static internal child free_make(children childrenMgr, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            return (child)(new child(childrenMgr, true)).init(strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 父生成器
        /// </summary>
        /// <returns>父生成器</returns>
        /// <remarks>
        /// </remarks>
        public override generator parent
        {
            get
            {
                return null != _childrenMgr ? _childrenMgr.parent : null;
            }
        }
        /// <summary>
        /// 兄弟生成器
        /// </summary>
        /// <returns>兄弟生成器</returns>
        /// <remarks>
        /// </remarks>
        public override children brothers
        {
            get
            {
                return _childrenMgr;
            }
        }
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns>是否为空</returns>
        /// <remarks>
        /// </remarks>
        public bool is_free()
        {
            return _isFree;
        }
    }
    /// <summary>
    /// 子生成器管理器
    /// </summary>
    public class children
    {
        bool _ignoreSuspend;
        generator _parent;
        LinkedList<child> _children;
        LinkedListNode<children> _node;

        public children()
        {
            Debug.Assert(null != self, "children 必须在 generator 内部创建!");
            _parent = self;
            _ignoreSuspend = false;
            _children = new LinkedList<child>();
            if (null == _parent._children)
            {
                _parent._children = new LinkedList<children>();
            }
        }
        /// <summary>
        /// 检查并添加节点
        /// </summary>
        /// <remarks>
        /// </remarks>
        void check_append_node()
        {
            if (0 == _children.Count)
            {
                _node = _parent._children.AddLast(this);
            }
        }
        /// <summary>
        /// 检查并移除节点
        /// </summary>
        /// <remarks>
        /// </remarks>
        void check_remove_node()
        {
            if (0 == _children.Count && null != _node)
            {
                _parent._children.Remove(_node);
                _node = null;
            }
        }
        /// <summary>
        /// 创建子生成器
        /// </summary>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <returns>子生成器</returns>
        /// <remarks>
        /// </remarks>
        public child make(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            check_append_node();
            child newGen = child.make(this, strand, generatorAction, completedHandler, suspendHandler);
            newGen._childNode = _children.AddLast(newGen);
            return newGen;
        }
        /// <summary>
        /// 创建子生成器
        /// </summary>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <returns>子生成器</returns>
        /// <remarks>
        /// </remarks>
        public child free_make(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            check_append_node();
            child newGen = null;
            newGen = child.free_make(this, strand, generatorAction, delegate ()
            {
                _parent.strand.post(delegate ()
                {
                    if (null != newGen._childNode)
                    {
                        _children.Remove(newGen._childNode);
                        newGen._childNode = null;
                        newGen._childrenMgr = null;
                        check_remove_node();
                    }
                });
                completedHandler?.Invoke();
            }, suspendHandler);
            newGen._childNode = _children.AddLast(newGen);
            return newGen;
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void go(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            make(strand, generatorAction, completedHandler, suspendHandler).run();
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="newChild">子生成器</param>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void go(out child newChild, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            newChild = make(strand, generatorAction, completedHandler, suspendHandler);
            newChild.run();
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void free_go(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            free_make(strand, generatorAction, completedHandler, suspendHandler).run();
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="newChild">子生成器</param>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void free_go(out child newChild, shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            newChild = free_make(strand, generatorAction, completedHandler, suspendHandler);
            newChild.run();
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public child tgo(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            child newGen = make(strand, generatorAction, completedHandler, suspendHandler);
            newGen.trun();
            return newGen;
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="strand">线程</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public child free_tgo(shared_strand strand, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            child newGen = free_make(strand, generatorAction, completedHandler, suspendHandler);
            newGen.trun();
            return newGen;
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public child make(action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            return make(_parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public child free_make(action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            return free_make(_parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void go(action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            go(_parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="newChild">子生成器</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void go(out child newChild, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            go(out newChild, _parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void free_go(action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            free_go(_parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="newChild">子生成器</param>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public void free_go(out child newChild, action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            free_go(out newChild, _parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public child tgo(action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            return tgo(_parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 运行子生成器
        /// </summary>
        /// <param name="generatorAction">生成器操作</param>
        /// <param name="completedHandler">完成回调</param>
        /// <param name="suspendHandler">挂起回调</param>
        /// <remarks>
        /// </remarks>
        public child free_tgo(action generatorAction, Action completedHandler = null, Action<bool> suspendHandler = null)
        {
            return free_tgo(_parent.strand, generatorAction, completedHandler, suspendHandler);
        }
        /// <summary>
        /// 忽略挂起
        /// </summary>
        /// <param name="igonre">是否忽略挂起</param>
        /// <remarks>
        /// </remarks>
        public void ignore_suspend(bool igonre = true)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            _ignoreSuspend = igonre;
        }
        /// <summary>
        /// 挂起子生成器
        /// </summary>
        /// <param name="isSuspend">是否挂起</param>
        /// <param name="cb">回调</param>
        /// <remarks>
        /// </remarks>
        internal void suspend(bool isSuspend, Action cb)
        {
            if (!_ignoreSuspend && 0 != _children.Count)
            {
                int count = _children.Count;
                Action handler = _parent.strand.wrap(delegate ()
                {
                    if (0 == --count)
                    {
                        cb();
                    }
                });
                if (isSuspend)
                {
                    for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
                    {
                        it.Value.suspend(handler);
                    }
                }
                else
                {
                    for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
                    {
                        it.Value.resume(handler);
                    }
                }
            }
            else
            {
                cb();
            }
        }
        /// <summary>
        /// 子生成器数量
        /// </summary>
        /// <value>子生成器数量</value>
        /// <remarks>
        /// </remarks>
        public int count
        {
            get
            {
                Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
                return _children.Count;
            }
        }
        /// <summary>
        /// 父生成器
        /// </summary>
        /// <value>父生成器</value>
        /// <remarks>
        /// </remarks>
        public generator parent
        {
            get
            {
                return _parent;
            }
        }
        /// <summary>
        /// 是否忽略挂起
        /// </summary>
        /// <value>是否忽略挂起</value>
        /// <remarks>
        /// </remarks>
        public bool ignored_suspend
        {
            get
            {
                return _ignoreSuspend;
            }
        }
        /// <summary>
        /// 丢弃子生成器
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>丢弃的子生成器数量</returns>
        /// <remarks>
        /// </remarks>
        public int discard(IEnumerable<child> gens)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            int count = 0;
            foreach (child ele in gens)
            {
                if (null != ele._childNode)
                {
                    Debug.Assert(ele._childNode.List == _children, "此 child 不属于当前 children!");
                    count++;
                    _children.Remove(ele._childNode);
                    ele._childNode = null;
                    ele._childrenMgr = null;
                }
            }
            check_remove_node();
            return count;
        }
        /// <summary>
        /// 丢弃子生成器
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>丢弃的子生成器数量</returns>
        /// <remarks>
        /// </remarks>
        public int discard(params child[] gens)
        {
            return discard((IEnumerable<child>)gens);
        }
        /// <summary>
        /// 丢弃子生成器
        /// </summary>
        /// <param name="childrens">子生成器</param>
        /// <returns>丢弃的子生成器数量</returns>
        /// <remarks>
        /// </remarks>
        static public int discard(IEnumerable<children> childrens)
        {
            int count = 0;
            foreach (children childs in childrens)
            {
                Debug.Assert(self == childs._parent, "此 children 不属于当前 generator!");
                for (LinkedListNode<child> it = childs._children.First; null != it; it = it.Next)
                {
                    count++;
                    childs._children.Remove(it.Value._childNode);
                    it.Value._childNode = null;
                    it.Value._childrenMgr = null;
                    childs.check_remove_node();
                }
            }
            return count;
        }
        /// <summary>
        /// 丢弃子生成器
        /// </summary>
        /// <param name="childrens">子生成器</param>
        /// <returns>丢弃的子生成器数量</returns>
        /// <remarks>
        /// </remarks>
        static public int discard(params children[] childrens)
        {
            return discard((IEnumerable<children>)childrens);
        }
        /// <summary>
        /// 停止子生成器
        /// </summary>
        /// <param name="gen">子生成器</param>
        /// <returns>停止任务</returns>
        /// <remarks>
        /// </remarks>
        private async Task stop_(child gen)
        {
            await _parent.async_wait();
            if (null != gen._childNode)
            {
                _children.Remove(gen._childNode);
                gen._childNode = null;
                gen._childrenMgr = null;
                check_remove_node();
            }
        }
        /// <summary>
        /// 停止子生成器
        /// </summary>
        /// <param name="gen">子生成器</param>
        /// <returns>停止任务</returns>
        /// <remarks>
        /// </remarks>
        public Task stop(child gen)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            Debug.Assert(null == gen._childNode || gen._childNode.List == _children, "此 child 不属于当前 children!");
            if (null != gen._childNode)
            {
                gen.stop(_parent.unsafe_async_result());
                if (!_parent.new_task_completed())
                {
                    return stop_(gen);
                }
                if (null != gen._childNode)
                {
                    _children.Remove(gen._childNode);
                    gen._childNode = null;
                    gen._childrenMgr = null;
                    check_remove_node();
                }
            }
            return non_async();
        }
        /// <summary>
        /// 停止子生成器
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>停止任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task stop(IEnumerable<child> gens)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            int count = 0;
            unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
            Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
            foreach (child ele in gens)
            {
                count++;
                Debug.Assert(null == ele._childNode || ele._childNode.List == _children, "此 child 不属于当前 children!");
                if (null != ele._childNode && !ele.is_completed())
                {
                    ele.stop(stopHandler);
                }
                else
                {
                    waitStop.post(ele);
                }
            }
            for (int i = 0; i < count; i++)
            {
                child gen = (await chan_receive(waitStop)).msg;
                if (null != gen._childNode)
                {
                    _children.Remove(gen._childNode);
                    gen._childNode = null;
                    gen._childrenMgr = null;
                }
            }
            check_remove_node();
        }

        public Task stop(params child[] gens)
        {
            return stop((IEnumerable<child>)gens);
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gen">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        private async Task wait_(child gen)
        {
            await _parent.async_wait();
            if (null != gen._childNode)
            {
                _children.Remove(gen._childNode);
                gen._childNode = null;
                gen._childrenMgr = null;
                check_remove_node();
            }
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gen">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task wait(child gen)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            Debug.Assert(null == gen._childNode || gen._childNode.List == _children, "此 child 不属于当前 children!");
            if (null != gen._childNode)
            {
                gen.append_stop_callback(_parent.unsafe_async_result());
                if (!_parent.new_task_completed())
                {
                    return wait_(gen);
                }
                if (null != gen._childNode)
                {
                    _children.Remove(gen._childNode);
                    gen._childNode = null;
                    gen._childrenMgr = null;
                    check_remove_node();
                }
            }
            return non_async();
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task wait(IEnumerable<child> gens)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            int count = 0;
            unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
            Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
            foreach (child ele in gens)
            {
                count++;
                Debug.Assert(null == ele._childNode || ele._childNode.List == _children, "此 child 不属于当前 children!");
                if (null != ele._childNode && !ele.is_completed())
                {
                    ele.append_stop_callback(stopHandler);
                }
                else
                {
                    waitStop.post(ele);
                }
            }
            for (int i = 0; i < count; i++)
            {
                child gen = (await chan_receive(waitStop)).msg;
                if (null != gen._childNode)
                {
                    _children.Remove(gen._childNode);
                    gen._childNode = null;
                    gen._childrenMgr = null;
                }
            }
            check_remove_node();
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task wait(params child[] gens)
        {
            return wait((IEnumerable<child>)gens);
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gen">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        private async Task<bool> timed_wait_(ValueTask<bool> task, child gen)
        {
            bool overtime = !await task;
            if (!overtime && null != gen._childNode)
            {
                _children.Remove(gen._childNode);
                gen._childNode = null;
                gen._childrenMgr = null;
                check_remove_node();
            }
            return !overtime;
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="gen">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public ValueTask<bool> timed_wait(int ms, child gen)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            Debug.Assert(null == gen._childNode || gen._childNode.List == _children, "此 child 不属于当前 children!");
            bool overtime = false;
            if (null != gen._childNode)
            {
                ValueTask<bool> task = timed_wait_other(ms, gen);
                if (!task.IsCompleted)
                {
                    return to_vtask(timed_wait_(task, gen));
                }
                overtime = !task.GetAwaiter().GetResult();
                if (!overtime && null != gen._childNode)
                {
                    _children.Remove(gen._childNode);
                    gen._childNode = null;
                    gen._childrenMgr = null;
                    check_remove_node();
                }
            }
            return to_vtask(!overtime);
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task<child> wait_any(IEnumerable<child> gens)
        {
            return timed_wait_any(-1, gens);
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task<child> wait_any(params child[] gens)
        {
            return wait_any((IEnumerable<child>)gens);
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task<child> timed_wait_any(int ms, IEnumerable<child> gens)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            unlimit_chan<notify_token> waitRemove = new unlimit_chan<notify_token>(_parent.strand);
            unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
            Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
            Action<notify_token> removeHandler = (notify_token cancelToken) => waitRemove.post(cancelToken);
            int count = 0;
            foreach (child ele in gens)
            {
                count++;
                Debug.Assert(null == ele._childNode || ele._childNode.List == _children, "此 child 不属于当前 children!");
                if (null != ele._childNode && !ele.is_completed())
                {
                    ele.append_stop_callback(stopHandler, removeHandler);
                }
                else
                {
                    waitStop.post(ele);
                    waitRemove.post(new notify_token { host = ele });
                    break;
                }
            }
            try
            {
                if (0 != count)
                {
                    if (ms < 0)
                    {
                        chan_recv_wrap<child> gen = await chan_receive(waitStop);
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                            check_remove_node();
                        }
                        return gen.msg;
                    }
                    else if (0 == ms)
                    {
                        chan_recv_wrap<child> gen = await chan_try_receive(waitStop);
                        if (chan_state.ok == gen.state)
                        {
                            if (null != gen.msg._childNode)
                            {
                                _children.Remove(gen.msg._childNode);
                                gen.msg._childNode = null;
                                gen.msg._childrenMgr = null;
                                check_remove_node();
                            }
                            return gen.msg;
                        }
                    }
                    else
                    {
                        chan_recv_wrap<child> gen = await chan_timed_receive(waitStop, ms);
                        if (chan_state.ok == gen.state)
                        {
                            if (null != gen.msg._childNode)
                            {
                                _children.Remove(gen.msg._childNode);
                                gen.msg._childNode = null;
                                gen.msg._childrenMgr = null;
                                check_remove_node();
                            }
                            return gen.msg;
                        }
                    }
                }
                return null;
            }
            finally
            {
                lock_suspend_and_stop();
                for (int i = 0; i < count; i++)
                {
                    notify_token node = (await chan_receive(waitRemove)).msg;
                    if (null != node.token)
                    {
                        node.host.remove_stop_callback(node);
                    }
                }
                await unlock_suspend_and_stop();
            }
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task<child> timed_wait_any(int ms, params child[] gens)
        {
            return timed_wait_any(ms, (IEnumerable<child>)gens);
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task<List<child>> timed_wait(int ms, IEnumerable<child> gens)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            int count = 0;
            long endTick = system_tick.get_tick_ms() + ms;
            unlimit_chan<notify_token> waitRemove = new unlimit_chan<notify_token>(_parent.strand);
            unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
            Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
            Action<notify_token> removeHandler = (notify_token cancelToken) => waitRemove.post(cancelToken);
            foreach (child ele in gens)
            {
                count++;
                Debug.Assert(null == ele._childNode || ele._childNode.List == _children, "此 child 不属于当前 children!");
                if (null != ele._childNode && !ele.is_completed())
                {
                    ele.append_stop_callback(stopHandler, removeHandler);
                }
                else
                {
                    waitStop.post(ele);
                    waitRemove.post(new notify_token { host = ele });
                }
            }
            try
            {
                List<child> completedGens = new List<child>(count);
                if (ms < 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        chan_recv_wrap<child> gen = await chan_receive(waitStop);
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                        }
                        completedGens.Add(gen.msg);
                    }
                }
                else if (0 == ms)
                {
                    for (int i = 0; i < count; i++)
                    {
                        chan_recv_wrap<child> gen = await chan_try_receive(waitStop);
                        if (chan_state.ok != gen.state)
                        {
                            break;
                        }
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                        }
                        completedGens.Add(gen.msg);
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        long nowTick = system_tick.get_tick_ms();
                        if (nowTick >= endTick)
                        {
                            break;
                        }
                        chan_recv_wrap<child> gen = await chan_timed_receive(waitStop, (int)(endTick - nowTick));
                        if (chan_state.ok != gen.state)
                        {
                            break;
                        }
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                        }
                        completedGens.Add(gen.msg);
                    }
                }
                check_remove_node();
                return completedGens;
            }
            finally
            {
                lock_suspend_and_stop();
                for (int i = 0; i < count; i++)
                {
                    notify_token node = (await chan_receive(waitRemove)).msg;
                    if (null != node.token)
                    {
                        node.host.remove_stop_callback(node);
                    }
                }
                await unlock_suspend_and_stop();
            }
        }
        /// <summary>
        /// 等待子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="gens">子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task<List<child>> timed_wait(int ms, params child[] gens)
        {
            return timed_wait(ms, (IEnumerable<child>)gens);
        }
        /// <summary>
        /// 停止子生成器
        /// </summary>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>停止任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task stop(bool containFree = true)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            if (0 != _children.Count)
            {
                unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
                Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
                int count = 0;
                for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
                {
                    child ele = it.Value;
                    if (!containFree && ele.is_free())
                    {
                        continue;
                    }
                    count++;
                    if (!ele.is_completed())
                    {
                        ele.stop(stopHandler);
                    }
                    else
                    {
                        waitStop.post(ele);
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    child gen = (await chan_receive(waitStop)).msg;
                    if (null != gen._childNode)
                    {
                        _children.Remove(gen._childNode);
                        gen._childNode = null;
                        gen._childrenMgr = null;
                    }
                }
                check_remove_node();
            }
        }
        /// <summary>
        /// 停止子生成器
        /// </summary>
        /// <param name="childrens">子生成器</param>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>停止任务</returns>
        /// <remarks>
        /// </remarks>
        static public async Task stop(IEnumerable<children> childrens, bool containFree = true)
        {
            generator self = generator.self;
            unlimit_chan<tuple<children, child>> waitStop = new unlimit_chan<tuple<children, child>>(self.strand);
            int count = 0;
            foreach (children childs in childrens)
            {
                Debug.Assert(self == childs._parent, "此 children 不属于当前 generator!");
                if (0 != childs._children.Count)
                {
                    Action<generator> stopHandler = (generator host) => waitStop.post(tuple.make(childs, (child)host));
                    for (LinkedListNode<child> it = childs._children.First; null != it; it = it.Next)
                    {
                        child ele = it.Value;
                        if (!containFree && ele.is_free())
                        {
                            continue;
                        }
                        count++;
                        if (!ele.is_completed())
                        {
                            ele.stop(stopHandler);
                        }
                        else
                        {
                            waitStop.post(tuple.make(childs, ele));
                        }
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                tuple<children, child> oneRes = (await chan_receive(waitStop)).msg;
                if (null != oneRes.value2._childNode)
                {
                    oneRes.value1._children.Remove(oneRes.value2._childNode);
                    oneRes.value2._childNode = null;
                    oneRes.value2._childrenMgr = null;
                    oneRes.value1.check_remove_node();
                }
            }
        }
        /// <summary>
        /// 停止子生成器
        /// </summary>
        /// <param name="childrens">子生成器</param>
        /// <returns>停止任务</returns>
        /// <remarks>
        /// </remarks>
        static public Task stop(params children[] childrens)
        {
            return stop((IEnumerable<children>)childrens);
        }
        /// <summary>
        /// 等待任意子生成器完成
        /// </summary>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public Task<child> wait_any(bool containFree = false)
        {
            return timed_wait_any(-1, containFree);
        }
        /// <summary>
        /// 等待任意子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task<child> timed_wait_any(int ms, bool containFree = false)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            unlimit_chan<notify_token> waitRemove = new unlimit_chan<notify_token>(_parent.strand);
            unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
            Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
            Action<notify_token> removeHandler = (notify_token cancelToken) => waitRemove.post(cancelToken);
            int count = 0;
            for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
            {
                child ele = it.Value;
                if (!containFree && ele.is_free())
                {
                    continue;
                }
                count++;
                if (ele.is_completed())
                {
                    waitStop.post(ele);
                    waitRemove.post(new notify_token { host = ele });
                    break;
                }
                ele.append_stop_callback(stopHandler, removeHandler);
            }
            try
            {
                if (0 != count)
                {
                    if (ms < 0)
                    {
                        chan_recv_wrap<child> gen = await chan_receive(waitStop);
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                            check_remove_node();
                        }
                        return gen.msg;
                    }
                    else if (0 == ms)
                    {
                        chan_recv_wrap<child> gen = await chan_try_receive(waitStop);
                        if (chan_state.ok == gen.state)
                        {
                            if (null != gen.msg._childNode)
                            {
                                _children.Remove(gen.msg._childNode);
                                gen.msg._childNode = null;
                                gen.msg._childrenMgr = null;
                                check_remove_node();
                            }
                            return gen.msg;
                        }
                    }
                    else
                    {
                        chan_recv_wrap<child> gen = await chan_timed_receive(waitStop, ms);
                        if (chan_state.ok == gen.state)
                        {
                            if (null != gen.msg._childNode)
                            {
                                _children.Remove(gen.msg._childNode);
                                gen.msg._childNode = null;
                                gen.msg._childrenMgr = null;
                                check_remove_node();
                            }
                            return gen.msg;
                        }
                    }
                }
                return null;
            }
            finally
            {
                lock_suspend_and_stop();
                for (int i = 0; i < count; i++)
                {
                    notify_token node = (await chan_receive(waitRemove)).msg;
                    if (null != node.token)
                    {
                        node.host.remove_stop_callback(node);
                    }
                }
                await unlock_suspend_and_stop();
            }
        }
        /// <summary>
        /// 等待所有子生成器完成
        /// </summary>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task wait_all(bool containFree = true)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            if (0 != _children.Count)
            {
                unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
                Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
                int count = 0;
                for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
                {
                    child ele = it.Value;
                    if (!containFree && ele.is_free())
                    {
                        continue;
                    }
                    count++;
                    if (!ele.is_completed())
                    {
                        ele.append_stop_callback(stopHandler);
                    }
                    else
                    {
                        waitStop.post(ele);
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    child gen = (await chan_receive(waitStop)).msg;
                    if (null != gen._childNode)
                    {
                        _children.Remove(gen._childNode);
                        gen._childNode = null;
                        gen._childrenMgr = null;
                    }
                }
                check_remove_node();
            }
        }
        /// <summary>
        /// 等待所有子生成器完成
        /// </summary>
        /// <param name="ms">超时时间</param>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>等待任务</returns>
        /// <remarks>
        /// </remarks>
        public async Task<List<child>> timed_wait_all(int ms, bool containFree = true)
        {
            Debug.Assert(self == _parent, "此 children 不属于当前 generator!");
            int count = 0;
            long endTick = system_tick.get_tick_ms() + ms;
            unlimit_chan<notify_token> waitRemove = new unlimit_chan<notify_token>(_parent.strand);
            unlimit_chan<child> waitStop = new unlimit_chan<child>(_parent.strand);
            Action<generator> stopHandler = (generator host) => waitStop.post((child)host);
            Action<notify_token> removeHandler = (notify_token cancelToken) => waitRemove.post(cancelToken);
            for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
            {
                child ele = it.Value;
                if (!containFree && ele.is_free())
                {
                    continue;
                }
                count++;
                if (!ele.is_completed())
                {
                    ele.append_stop_callback(stopHandler, removeHandler);
                }
                else
                {
                    waitStop.post(ele);
                    waitRemove.post(new notify_token { host = ele });
                }
            }
            try
            {
                List<child> completedGens = new List<child>(count);
                if (ms < 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        child gen = (await chan_receive(waitStop)).msg;
                        if (null != gen._childNode)
                        {
                            _children.Remove(gen._childNode);
                            gen._childNode = null;
                            gen._childrenMgr = null;
                        }
                        completedGens.Add(gen);
                    }
                }
                else if (0 == ms)
                {
                    for (int i = 0; i < count; i++)
                    {
                        chan_recv_wrap<child> gen = await chan_try_receive(waitStop);
                        if (chan_state.ok != gen.state)
                        {
                            break;
                        }
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                        }
                        completedGens.Add(gen.msg);
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        long nowTick = system_tick.get_tick_ms();
                        if (nowTick >= endTick)
                        {
                            break;
                        }
                        chan_recv_wrap<child> gen = await chan_timed_receive(waitStop, (int)(endTick - nowTick));
                        if (chan_state.ok != gen.state)
                        {
                            break;
                        }
                        if (null != gen.msg._childNode)
                        {
                            _children.Remove(gen.msg._childNode);
                            gen.msg._childNode = null;
                            gen.msg._childrenMgr = null;
                        }
                        completedGens.Add(gen.msg);
                    }
                }
                check_remove_node();
                return completedGens;
            }
            finally
            {
                lock_suspend_and_stop();
                for (int i = 0; i < count; i++)
                {
                    notify_token node = (await chan_receive(waitRemove)).msg;
                    if (null != node.token)
                    {
                        node.host.remove_stop_callback(node);
                    }
                }
                await unlock_suspend_and_stop();
            }
        }
        /// <summary>
        /// 获取任意子生成器完成的子生成器
        /// </summary>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>完成的子生成器</returns>
        /// <remarks>
        /// </remarks>
        public child completed_any(bool containFree = false)
        {
            for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
            {
                child ele = it.Value;
                if (!containFree && ele.is_free())
                {
                    continue;
                }
                if (ele.is_completed())
                {
                    return ele;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取所有子生成器完成的子生成器
        /// </summary>
        /// <param name="containFree">是否包含空闲子生成器</param>
        /// <returns>完成的子生成器列表</returns>
        /// <remarks>
        /// </remarks>
        public List<child> completed_all(bool containFree = true)
        {
            List<child> completedGens = new List<child>(_children.Count);
            for (LinkedListNode<child> it = _children.First; null != it; it = it.Next)
            {
                child ele = it.Value;
                if (!containFree && ele.is_free())
                {
                    continue;
                }
                if (ele.is_completed())
                {
                    completedGens.Add(ele);
                }
            }
            return completedGens;
        }
    }
}

/// <summary>
/// 等待组
/// </summary>
/// <remarks>
/// </remarks>
public class wait_group
{
    /// <summary>
    /// 取消令牌
    /// </summary>
    public struct cancel_token
    {
        internal LinkedListNode<Action> token;
    }
    /// <summary>
    /// 是否等待
    /// </summary>
    bool _hasWait;
    /// <summary>
    /// 任务计数
    /// </summary>
    int _tasks;
    /// <summary>
    /// 等待列表
    /// </summary>
    LinkedList<Action> _waitList;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="initTasks"></param>
    public wait_group(int initTasks = 0)
    {
        _hasWait = false;
        _tasks = initTasks;
        _waitList = new LinkedList<Action>();
    }
    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="delta">任务数量</param>
    /// <remarks>
    /// </remarks>
    public void add(int delta = 1)
    {
        if (0 == Interlocked.Add(ref _tasks, delta))
        {
            notify();
        }
    }
    /// <summary>
    /// 通知等待组
    /// </summary>
    /// <remarks>
    /// </remarks>
    public void notify()
    {
        LinkedList<Action> newList = new LinkedList<Action>();
        Monitor.Enter(this);
        LinkedList<Action> snapList = _waitList;
        _waitList = newList;
        if (_hasWait)
        {
            _hasWait = false;
            Monitor.PulseAll(this);
        }
        Monitor.Exit(this);
        for (LinkedListNode<Action> it = snapList.First; null != it; it = it.Next)
        {
            functional.catch_invoke(it.Value);
        }
    }
    /// <summary>
    /// 任务完成
    /// </summary>
    /// <remarks>
    /// </remarks>
    public void done()
    {
        add(-1);
    }
    /// <summary>
    /// 是否完成
    /// </summary>
    /// <returns>是否完成</returns>
    /// <remarks>
    /// </remarks>
    public bool is_done
    {
        get
        {
            return 0 == _tasks;
        }
    }
    /// <summary>
    /// 异步等待
    /// </summary>
    /// <param name="continuation">继续操作</param>
    /// <returns>取消令牌</returns>
    /// <remarks>
    /// </remarks>
    public cancel_token async_wait(Action continuation)
    {
        if (is_done)
        {
            functional.catch_invoke(continuation);
            return new cancel_token { token = null };
        }
        LinkedListNode<Action> newNode = new LinkedListNode<Action>(continuation);
        Monitor.Enter(this);
        if (is_done)
        {
            Monitor.Exit(this);
            functional.catch_invoke(continuation);
            return new cancel_token { token = null };
        }
        else
        {
            _waitList.AddLast(newNode);
            Monitor.Exit(this);
            return new cancel_token { token = newNode };
        }
    }
    /// <summary>
    /// 取消等待
    /// </summary>
    /// <param name="cancelToken">取消令牌</param>
    /// <returns>是否成功取消</returns>
    /// <remarks>
    /// </remarks>
    public bool cancel_wait(cancel_token cancelToken)
    {
        bool success = false;
        if (null != cancelToken.token && null != cancelToken.token.List)
        {
            Monitor.Enter(this);
            if (cancelToken.token.List == _waitList)
            {
                _waitList.Remove(cancelToken.token);
                success = true;
            }
            Monitor.Exit(this);
        }
        return success;
    }
    /// <summary>
    /// 等待
    /// </summary>
    /// <returns>任务</returns>
    /// <remarks>
    /// </remarks>
    public Task wait()
    {
        return generator.wait_group(this);
    }
    /// <summary>
    /// 超时等待
    /// </summary>
    /// <param name="ms">超时时间</param>
    /// <returns>是否超时</returns>
    /// <remarks>
    /// </remarks>
    public ValueTask<bool> timed_wait(int ms)
    {
        return generator.timed_wait_group(ms, this);
    }
    /// <summary>
    /// 同步等待
    /// </summary>
    /// <remarks>
    /// </remarks>
    public void sync_wait()
    {
        if (is_done)
        {
            return;
        }
        Monitor.Enter(this);
        if (!is_done)
        {
            _hasWait = true;
            Monitor.Wait(this);
        }
        Monitor.Exit(this);
    }
    /// <summary>
    /// 同步超时等待
    /// </summary>
    /// <param name="ms">超时时间</param>
    /// <returns>是否超时</returns>
    /// <remarks>
    /// </remarks>
    public bool sync_timed_wait(int ms)
    {
        if (is_done)
        {
            return true;
        }
        bool success = true;
        Monitor.Enter(this);
        if (!is_done)
        {
            _hasWait = true;
            success = Monitor.Wait(this, ms);
        }
        Monitor.Exit(this);
        return success;
    }
}

/// <summary>
/// 等待门
/// </summary>
/// <typeparam name="R">结果类型</typeparam>
public class wait_gate<R>
{
    int _tasks;
    int _enterCnt;
    int _cancelCnt;
    chan_notify_sign _cspSign;
    csp_invoke_wrap<R> _result;
    LinkedList<Action> _waitList;
    csp_chan<R, void_type> _action;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="action"></param>
    /// <param name="initTasks"></param>
    public wait_gate(csp_chan<R, void_type> action, int initTasks = 0)
    {
        _enterCnt = 0;
        _cancelCnt = 0;
        _action = action;
        _cspSign = new chan_notify_sign();
        _tasks = initTasks > 0 ? initTasks : 0;
        _waitList = _tasks > 0 ? new LinkedList<Action>() : null;
    }
    /// <summary>
    /// 重置等待门
    /// </summary>
    /// <param name="tasks">任务数</param>
    /// <remarks>
    /// </remarks>
    public void reset(int tasks = -1)
    {
        Debug.Assert(is_exit, "不正确的 reset 调用!");
        _enterCnt = 0;
        _cancelCnt = 0;
        _tasks = tasks > 0 ? tasks : _tasks;
        _waitList = _tasks > 0 ? new LinkedList<Action>() : null;
    }
    /// <summary>
    /// 结果
    /// </summary>
    /// <returns>结果</returns>
    public csp_invoke_wrap<R> result
    {
        get
        {
            return _result;
        }
    }
    /// <summary>
    /// 是否退出
    /// </summary>
    /// <returns>是否退出</returns>
    public bool is_exit
    {
        get
        {
            return null == _waitList;
        }
    }
    /// <summary>
    /// 等待
    /// </summary>
    /// <param name="continuation">继续操作</param>
    /// <returns>取消令牌</returns>
    /// <remarks>
    /// </remarks>
    private wait_gate.cancel_token wait(Action continuation)
    {
        if (null == _waitList)
        {
            functional.catch_invoke(continuation);
            return new wait_gate.cancel_token { token = null };
        }
        LinkedListNode<Action> newNode = new LinkedListNode<Action>(continuation);
        Monitor.Enter(this);
        if (null != _waitList)
        {
            _waitList.AddLast(newNode);
            Monitor.Exit(this);
            return new wait_gate.cancel_token { token = newNode };
        }
        else
        {
            Monitor.Exit(this);
            functional.catch_invoke(continuation);
            return new wait_gate.cancel_token { token = null };
        }
    }
    /// <summary>
    /// 通知
    /// </summary>
    /// <remarks>
    /// </remarks>
    private void notify()
    {
        Monitor.Enter(this);
        LinkedList<Action> snapList = _waitList;
        _waitList = null;
        Monitor.Exit(this);
        for (LinkedListNode<Action> it = snapList.First; null != it; it = it.Next)
        {
            functional.catch_invoke(it.Value);
        }
    }
    /// <summary>
    /// 异步进入
    /// </summary>
    /// <remarks>
    /// </remarks>
    public void async_enter()
    {
        async_enter(null);
    }
    /// <summary>
    /// 异步进入
    /// </summary>
    /// <param name="continuation">继续操作</param>
    /// <returns>取消令牌</returns>
    /// <remarks>
    /// </remarks>
    public wait_gate.cancel_token async_enter(Action continuation)
    {
        if (_tasks == Interlocked.Increment(ref _enterCnt))
        {
            _action.async_send(delegate (csp_invoke_wrap<R> res)
            {
                _result = res;
                notify();
            }, default(void_type), _cspSign);
        }
        else
        {
            Debug.Assert(_enterCnt < _tasks, "异常的 async_enter 调用!");
        }
        if (null == continuation)
        {
            if (_tasks == Interlocked.Increment(ref _cancelCnt) && null != _waitList)
            {
                _action.async_remove_send_notify(delegate (chan_state state)
                {
                    if (chan_state.ok == state)
                    {
                        notify();
                    }
                }, _cspSign);
            }
            return new wait_gate.cancel_token { token = null };
        }
        return wait(continuation);
    }
    /// <summary>
    /// 取消进入
    /// </summary>
    /// <param name="cancelToken">取消令牌</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// </remarks>
    public bool cancel_enter(wait_gate.cancel_token cancelToken)
    {
        if (null != cancelToken.token && null != cancelToken.token.List)
        {
            bool success = false;
            Monitor.Enter(this);
            if (null != _waitList && cancelToken.token.List == _waitList)
            {
                _waitList.Remove(cancelToken.token);
                success = true;
            }
            Monitor.Exit(this);
            if (success && _tasks == Interlocked.Increment(ref _cancelCnt) && null != _waitList)
            {
                _action.async_remove_send_notify(delegate (chan_state state)
                {
                    if (chan_state.ok == state)
                    {
                        notify();
                    }
                }, _cspSign);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 安全退出
    /// </summary>
    /// <param name="continuation">继续操作</param>
    /// <remarks>
    /// </remarks>
    public void safe_exit(Action continuation)
    {
        wait(continuation);
    }
    /// <summary>
    /// 进入
    /// </summary>
    /// <returns>任务</returns>
    /// <remarks>
    /// </remarks>
    public Task enter()
    {
        return generator.enter_gate(this);
    }
    /// <summary>
    /// 定时进入
    /// </summary>
    /// <param name="ms">超时时间</param>
    /// <returns>任务</returns>
    /// <remarks>
    /// </remarks>
    public Task<bool> timed_enter(int ms)
    {
        return generator.timed_enter_gate(ms, this);
    }
}

/// <summary>
/// 等待门
/// </summary>
public class wait_gate : wait_gate<void_type>
{
    /// <summary>
    /// 取消令牌
    /// </summary>
    public struct cancel_token
    {
        internal LinkedListNode<Action> token;
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="action">通道</param>
    /// <param name="initTasks">初始任务数</param>
    /// <remarks>
    /// </remarks>
    public wait_gate(csp_chan<void_type, void_type> action, int initTasks = 0) : base(action, initTasks)
    {
    }
}

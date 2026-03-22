// File-based Apps implementation for socket
// Uses .NET built-in socket classes and reflection

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Threading.Tasks.Cs;

/// <summary>
/// 类型反射工具类，用于获取类型的方法和字段信息
/// </summary>
internal class TypeReflection
{
    /// <summary>
    /// 获取类型的方法信息
    /// </summary>
    /// <param name="type">要获取方法的类型</param>
    /// <param name="name">方法名</param>
    /// <param name="parmsType">参数类型数组，为null时返回第一个匹配方法名的方法</param>
    /// <returns>找到的方法信息，未找到则返回null</returns>
    public static MethodInfo GetMethod(Type type, string name, Type[] parmsType = null)
    {
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        for (int i = 0; i < methods.Length; i++)
        {
            if (methods[i].Name == name)
            {
                if (parmsType == null)
                {
                    return methods[i];
                }
                else
                {
                    ParameterInfo[] parameters = methods[i].GetParameters();
                    if (parameters.Length == parmsType.Length)
                    {
                        int j = 0;
                        for (; j < parmsType.Length && parameters[j].ParameterType == parmsType[j]; j++) { }
                        if (j == parmsType.Length)
                        {
                            return methods[i];
                        }
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 获取类型的字段信息
    /// </summary>
    /// <param name="type">要获取字段的类型</param>
    /// <param name="name">字段名</param>
    /// <returns>找到的字段信息，未找到则返回null</returns>
    public static FieldInfo GetField(Type type, string name)
    {
        FieldInfo[] members = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        for (int i = 0; i < members.Length; i++)
        {
            if (members[i].Name == name)
            {
                return members[i];
            }
        }
        return null;
    }
}

/// <summary>
/// 套接字操作结果结构体，用于表示套接字操作的执行结果
/// </summary>
public struct socket_result
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool ok;
    /// <summary>
    /// 操作结果大小（字节数）
    /// </summary>
    public int s;
    /// <summary>
    /// 异常信息，操作失败时包含错误信息
    /// </summary>
    public Exception ec;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="resOk">操作是否成功，默认为false</param>
    /// <param name="resSize">操作结果大小（字节数），默认为0</param>
    /// <param name="resEc">异常信息，默认为null</param>
    public socket_result(bool resOk = false, int resSize = 0, Exception resEc = null)
    {
        ok = resOk;
        s = resSize;
        ec = resEc;
    }

    /// <summary>
    /// 获取异常信息
    /// </summary>
    public string message
    {
        get
        {
            return ec != null ? ec.Message : null;
        }
    }

    /// <summary>
    /// 隐式转换为bool类型，表示操作是否成功
    /// </summary>
    /// <param name="src">socket_result实例</param>
    /// <returns>操作是否成功</returns>
    public static implicit operator bool(socket_result src)
    {
        return src.ok;
    }

    /// <summary>
    /// 隐式转换为int类型，表示操作结果大小
    /// </summary>
    /// <param name="src">socket_result实例</param>
    /// <returns>操作结果大小（字节数）</returns>
    public static implicit operator int(socket_result src)
    {
        return src.s;
    }
}

/// <summary>
/// 套接字抽象基类，定义了套接字操作的基本接口
/// </summary>
public abstract class socket
{
    /// <summary>
    /// 处理回调函数
    /// </summary>
    public Action<socket_result> handler;
    /// <summary>
    /// 完成回调函数
    /// </summary>
    public Action<socket_result> cb;

    /// <summary>
    /// 异步读取数据
    /// </summary>
    /// <param name="buffer">数据缓冲区</param>
    /// <returns>操作结果</returns>
    public abstract Task<socket_result> read_async(byte[] buffer);

    /// <summary>
    /// 异步写入数据
    /// </summary>
    /// <param name="buffer">数据缓冲区</param>
    /// <returns>操作结果</returns>
    public abstract Task<socket_result> write_async(byte[] buffer);

    /// <summary>
    /// 关闭套接字
    /// </summary>
    public abstract void close();
}

/// <summary>
/// TCP套接字实现
/// </summary>
public class socket_tcp : socket
{
    private Socket _socket;
    private SocketAsyncEventArgs _readEventArgs;
    private SocketAsyncEventArgs _writeEventArgs;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="socket">Socket实例</param>
    public socket_tcp(Socket socket)
    {
        _socket = socket;
        _readEventArgs = new SocketAsyncEventArgs();
        _writeEventArgs = new SocketAsyncEventArgs();
    }

    /// <summary>
    /// 异步读取数据
    /// </summary>
    /// <param name="buffer">数据缓冲区</param>
    /// <returns>操作结果</returns>
    public override async Task<socket_result> read_async(byte[] buffer)
    {
        try
        {
            int bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);
            return new socket_result(true, bytesRead);
        }
        catch (Exception ex)
        {
            return new socket_result(false, 0, ex);
        }
    }

    /// <summary>
    /// 异步写入数据
    /// </summary>
    /// <param name="buffer">数据缓冲区</param>
    /// <returns>操作结果</returns>
    public override async Task<socket_result> write_async(byte[] buffer)
    {
        try
        {
            int bytesWritten = await _socket.SendAsync(buffer, SocketFlags.None);
            return new socket_result(true, bytesWritten);
        }
        catch (Exception ex)
        {
            return new socket_result(false, 0, ex);
        }
    }

    /// <summary>
    /// 关闭套接字
    /// </summary>
    public override void close()
    {
        _readEventArgs.Dispose();
        _writeEventArgs.Dispose();
        _socket.Dispose();
    }

    /// <summary>
    /// 开始接受连接
    /// </summary>
    /// <returns>新的socket_tcp实例</returns>
    public async Task<socket_tcp> accept_async()
    {
        try
        {
            Socket clientSocket = await _socket.AcceptAsync();
            return new socket_tcp(clientSocket);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

/// <summary>
/// 套接字服务器实现
/// </summary>
public class socket_server
{
    private Socket _listener;
    private bool _running;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="endpoint">服务器端点</param>
    public socket_server(IPEndPoint endpoint)
    {
        _listener = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(endpoint);
        _listener.Listen(100);
        _running = false;
    }

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <param name="handleClient">客户端处理函数</param>
    /// <returns></returns>
    public async Task start_async(Func<socket_tcp, Task> handleClient)
    {
        _running = true;
        while (_running)
        {
            try
            {
                Socket clientSocket = await _listener.AcceptAsync();
                var client = new socket_tcp(clientSocket);
                _ = Task.Run(() => handleClient(client));
            }
            catch (Exception)
            {
                if (!_running)
                    break;
            }
        }
    }

    /// <summary>
    /// 停止服务器
    /// </summary>
    public void stop()
    {
        _running = false;
        _listener.Dispose();
    }
}

/// <summary>
/// 套接字客户端实现
/// </summary>
public class socket_client
{
    private Socket _socket;

    /// <summary>
    /// 构造函数
    /// </summary>
    public socket_client()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="endpoint">服务器端点</param>
    /// <returns>操作结果</returns>
    public async Task<socket_result> connect_async(IPEndPoint endpoint)
    {
        try
        {
            await _socket.ConnectAsync(endpoint);
            return new socket_result(true, 0);
        }
        catch (Exception ex)
        {
            return new socket_result(false, 0, ex);
        }
    }

    /// <summary>
    /// 获取socket_tcp实例
    /// </summary>
    /// <returns>socket_tcp实例</returns>
    public socket_tcp get_socket()
    {
        return new socket_tcp(_socket);
    }

    /// <summary>
    /// 关闭客户端
    /// </summary>
    public void close()
    {
        _socket.Dispose();
    }
}

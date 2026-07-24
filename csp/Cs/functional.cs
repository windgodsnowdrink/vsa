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
using System.Threading.Tasks;
using System.Diagnostics;

namespace System.Threading.Tasks.Cs;

// 函数式编程
// - 元组结构 ： tuple<T1, T2> 等
// - Nil 操作 ： nil 结构体和相关操作
// - 函数绑定 ： bind 系列方法，用于部分应用函数
// - 异常处理 ： catch_invoke 方法，用于安全调用可能抛出异常的函数
// - 使用ValueTuple 、 Func / Action 委托和 lambda 表达式,使用LanguageExt实现函数式编程,异常处理闭包和 lambda 表达式实现

/// <summary>
/// 元组
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
[Serializable]
public struct tuple<T1>
{
    public readonly T1 value1;
    public tuple(T1 v1) { value1 = v1; }

    public override string ToString()
    {
        return string.Format("({0})", value1);
    }
    public static implicit operator tuple<T1>(ValueTuple<T1> rval)
    {
        return new tuple<T1>(rval.Item1);
    }

    public static implicit operator ValueTuple<T1>(tuple<T1> rval)
    {
        return new ValueTuple<T1>(rval.value1);
    }
}

/// <summary>
/// 元组
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>
[Serializable]
public struct tuple<T1, T2>
{
    public readonly T1 value1; public readonly T2 value2;
    public tuple(T1 v1, T2 v2) { value1 = v1; value2 = v2; }

    public override string ToString()
    {
        return string.Format("({0},{1})", value1, value2);
    }
    public static implicit operator tuple<T1, T2>(ValueTuple<T1, T2> rval)
    {
        return new tuple<T1, T2>(rval.Item1, rval.Item2);
    }

    public static implicit operator ValueTuple<T1, T2>(tuple<T1, T2> rval)
    {
        return new ValueTuple<T1, T2>(rval.value1, rval.value2);
    }
}

/// <summary>
/// 元组
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>
/// <typeparam name="T3">元组类型3</typeparam>
[Serializable]
public struct tuple<T1, T2, T3>
{
    public readonly T1 value1; public readonly T2 value2; public readonly T3 value3;
    public tuple(T1 v1, T2 v2, T3 v3) { value1 = v1; value2 = v2; value3 = v3; }

    public override string ToString()
    {
        return string.Format("({0},{1},{2})", value1, value2, value3);
    }
    public static implicit operator tuple<T1, T2, T3>(ValueTuple<T1, T2, T3> rval)
    {
        return new tuple<T1, T2, T3>(rval.Item1, rval.Item2, rval.Item3);
    }

    public static implicit operator ValueTuple<T1, T2, T3>(tuple<T1, T2, T3> rval)
    {
        return new ValueTuple<T1, T2, T3>(rval.value1, rval.value2, rval.value3);
    }
}

/// <summary>
/// 元组
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>
/// <typeparam name="T3">元组类型3</typeparam>
/// <typeparam name="T4">元组类型4</typeparam>
[Serializable]
public struct tuple<T1, T2, T3, T4>
{
    public readonly T1 value1; public readonly T2 value2; public readonly T3 value3; public readonly T4 value4;
    public tuple(T1 v1, T2 v2, T3 v3, T4 v4) { value1 = v1; value2 = v2; value3 = v3; value4 = v4; }

    public override string ToString()
    {
        return string.Format("({0},{1},{2},{3})", value1, value2, value3, value4);
    }
    public static implicit operator tuple<T1, T2, T3, T4>(ValueTuple<T1, T2, T3, T4> rval)
    {
        return new tuple<T1, T2, T3, T4>(rval.Item1, rval.Item2, rval.Item3, rval.Item4);
    }

    public static implicit operator ValueTuple<T1, T2, T3, T4>(tuple<T1, T2, T3, T4> rval)
    {
        return new ValueTuple<T1, T2, T3, T4>(rval.value1, rval.value2, rval.value3, rval.value4);
    }
}
/// <summary>
/// 元组
/// </summary>
/// <typeparam name="T1">元组类型1</typeparam>
/// <typeparam name="T2">元组类型2</typeparam>
/// <typeparam name="T3">元组类型3</typeparam>
/// <typeparam name="T4">元组类型4</typeparam>
/// <typeparam name="T5">元组类型5</typeparam>
[Serializable]
public struct tuple<T1, T2, T3, T4, T5>
{
    public readonly T1 value1; public readonly T2 value2; public readonly T3 value3; public readonly T4 value4; public readonly T5 value5;
    public tuple(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) { value1 = v1; value2 = v2; value3 = v3; value4 = v4; value5 = v5; }

    public override string ToString()
    {
        return string.Format("({0},{1},{2},{3},{4})", value1, value2, value3, value4, value5);
    }
    public static implicit operator tuple<T1, T2, T3, T4, T5>(ValueTuple<T1, T2, T3, T4, T5> rval)
    {
        return new tuple<T1, T2, T3, T4, T5>(rval.Item1, rval.Item2, rval.Item3, rval.Item4, rval.Item5);
    }

    public static implicit operator ValueTuple<T1, T2, T3, T4, T5>(tuple<T1, T2, T3, T4, T5> rval)
    {
        return new ValueTuple<T1, T2, T3, T4, T5>(rval.value1, rval.value2, rval.value3, rval.value4, rval.value5);
    }
}
/// <summary>
/// 元组
/// </summary>
public static class tuple
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <param name="p1"></param>
    /// <returns></returns>
    static public tuple<T1> make<T1>(T1 p1) { return new tuple<T1>(p1); }
    /// <summary>
    /// 创建
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    static public tuple<T1, T2> make<T1, T2>(T1 p1, T2 p2) { return new tuple<T1, T2>(p1, p2); }
    /// <summary>
    /// 创建
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    static public tuple<T1, T2, T3> make<T1, T2, T3>(T1 p1, T2 p2, T3 p3) { return new tuple<T1, T2, T3>(p1, p2, p3); }
    /// <summary>
    /// 创建
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <returns></returns>
    static public tuple<T1, T2, T3, T4> make<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4) { return new tuple<T1, T2, T3, T4>(p1, p2, p3, p4); }
    /// <summary>
    /// 创建
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <returns></returns>
    static public tuple<T1, T2, T3, T4, T5> make<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) { return new tuple<T1, T2, T3, T4, T5>(p1, p2, p3, p4, p5); }
}
/// <summary>
/// 方法
/// </summary>        
public static class functional
{
    /// <summary>
    /// 占位符
    /// </summary>
    public class placeholder { internal placeholder() { } }
    /// <summary>
    /// 占位符
    /// </summary>
    public static readonly placeholder _ = new placeholder();
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <returns>操作</returns>
    public static Action bind<T1>(Action<T1> handler, T1 p1)
    {
        return () => handler(p1);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Action bind<T1, T2>(Action<T1, T2> handler, T1 p1, T2 p2)
    {
        return () => handler(p1, p2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action bind<T1, T2, T3>(Action<T1, T2, T3> handler, T1 p1, T2 p2, T3 p3)
    {
        return () => handler(p1, p2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Action<T1> bind<T1, T2>(Action<T1, T2> handler, placeholder p1, T2 p2)
    {
        return (T1 _1) => handler(_1, p2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Action<T2> bind<T1, T2>(Action<T1, T2> handler, T1 p1, placeholder p2)
    {
        return (T2 _2) => handler(p1, _2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action<T1> bind<T1, T2, T3>(Action<T1, T2, T3> handler, placeholder p1, T2 p2, T3 p3)
    {
        return (T1 _1) => handler(_1, p2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action<T2> bind<T1, T2, T3>(Action<T1, T2, T3> handler, T1 p1, placeholder p2, T3 p3)
    {
        return (T2 _2) => handler(p1, _2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action<T3> bind<T1, T2, T3>(Action<T1, T2, T3> handler, T1 p1, T2 p2, placeholder p3)
    {
        return (T3 _3) => handler(p1, p2, _3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action<T1, T2> bind<T1, T2, T3>(Action<T1, T2, T3> handler, placeholder p1, placeholder p2, T3 p3)
    {
        return (T1 _1, T2 _2) => handler(_1, _2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action<T1, T3> bind<T1, T2, T3>(Action<T1, T2, T3> handler, placeholder p1, T2 p2, placeholder p3)
    {
        return (T1 _1, T3 _3) => handler(_1, p2, _3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Action<T2, T3> bind<T1, T2, T3>(Action<T1, T2, T3> handler, T1 p1, placeholder p2, placeholder p3)
    {
        return (T2 _2, T3 _3) => handler(p1, _2, _3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <returns>操作</returns>
    public static Func<R> bind<R, T1>(Func<T1, R> handler, T1 p1)
    {
        return () => handler(p1);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Func<R> bind<R, T1, T2>(Func<T1, T2, R> handler, T1 p1, T2 p2)
    {
        return () => handler(p1, p2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, T1 p1, T2 p2, T3 p3)
    {
        return () => handler(p1, p2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Func<T1, R> bind<R, T1, T2>(Func<T1, T2, R> handler, placeholder p1, T2 p2)
    {
        return (T1 _1) => handler(_1, p2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Func<T2, R> bind<R, T1, T2>(Func<T1, T2, R> handler, T1 p1, placeholder p2)
    {
        return (T2 _2) => handler(p1, _2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<T1, R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, placeholder p1, T2 p2, T3 p3)
    {
        return (T1 _1) => handler(_1, p2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<T2, R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, T1 p1, placeholder p2, T3 p3)
    {
        return (T2 _2) => handler(p1, _2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<T3, R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, T1 p1, T2 p2, placeholder p3)
    {
        return (T3 _3) => handler(p1, p2, _3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<T1, T2, R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, placeholder p1, placeholder p2, T3 p3)
    {
        return (T1 _1, T2 _2) => handler(_1, _2, p3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<T1, T3, R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, placeholder p1, T2 p2, placeholder p3)
    {
        return (T1 _1, T3 _3) => handler(_1, p2, _3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<T2, T3, R> bind<R, T1, T2, T3>(Func<T1, T2, T3, R> handler, T1 p1, placeholder p2, placeholder p3)
    {
        return (T2 _2, T3 _3) => handler(p1, _2, _3);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <returns>操作</returns>
    public static Func<Task> bind<T1>(Func<T1, Task> handler, T1 p1)
    {
        return () => handler(p1);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static Func<Task> bind<T1, T2>(Func<T1, T2, Task> handler, T1 p1, T2 p2)
    {
        return () => handler(p1, p2);
    }
    /// <summary>
    /// 绑定操作
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static Func<Task> bind<T1, T2, T3>(Func<T1, T2, T3, Task> handler, T1 p1, T2 p2, T3 p3)
    {
        return () => handler(p1, p2, p3);
    }
    /// <summary>
    /// 捕获异常
    /// </summary>
    /// <param name="handler">操作</param>
    public static void catch_invoke(Action handler)
    {
        try
        {
            handler();
            // handler?.Invoke();
        }
        catch (System.Exception ec)
        {
            Trace.Fail(ec.Message, ec.StackTrace);
        }
    }
    /// <summary>
    /// 捕获异常
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <returns>操作</returns>
    public static void catch_invoke<T1>(Action<T1> handler, T1 p1)
    {
        try
        {
            handler(p1);
            // handler?.Invoke(p1);
        }
        catch (System.Exception ec)
        {
            Trace.Fail(ec.Message, ec.StackTrace);
        }
    }
    /// <summary>
    /// 捕获异常
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <returns>操作</returns>
    public static void catch_invoke<T1, T2>(Action<T1, T2> handler, T1 p1, T2 p2)
    {
        try
        {
            handler?.Invoke(p1, p2);
        }
        catch (System.Exception ec)
        {
            Trace.Fail(ec.Message, ec.StackTrace);
        }
    }
    /// <summary>
    /// 捕获异常
    /// </summary>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <param name="p1">参数1</param>
    /// <param name="p2">参数2</param>
    /// <param name="p3">参数3</param>
    /// <returns>操作</returns>
    public static void catch_invoke<T1, T2, T3>(Action<T1, T2, T3> handler, T1 p1, T2 p2, T3 p3)
    {
        try
        {
            handler?.Invoke(p1, p2, p3);
        }
        catch (System.Exception ec)
        {
            Trace.Fail(ec.Message, ec.StackTrace);
        }
    }
    /// <summary>
    /// 初始化操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <param name="action">操作</param>
    /// <returns>返回值</returns>
    public static R init<R>(Func<R> action)
    {
        return action();
        // return action.Invoke();
    }

    /// <summary>
    /// 空操作
    /// </summary>
    public static class nil_action
    {
        /// <summary>
        /// 空操作函数
        /// </summary>
        public static readonly Action action = () => { };
    }

    /// <summary>
    /// 异步操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <param name="handler">操作</param>
    /// <returns>操作</returns>
    public static Func<ValueTask<R>> acry<R>(Func<ValueTask<R>> handler)
    {
        return handler;
    }
    /// <summary>
    /// 异步操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <param name="handler">操作</param>
    /// <returns>操作</returns>
    public static Func<T1, ValueTask<R>> acry<R, T1>(Func<T1, ValueTask<R>> handler)
    {
        return handler;
    }
    /// <summary>
    /// 异步操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <param name="handler">操作</param>
    /// <returns>操作</returns>
    public static Func<T1, T2, ValueTask<R>> acry<R, T1, T2>(Func<T1, T2, ValueTask<R>> handler)
    {
        return handler;
    }
    /// <summary>
    /// 异步操作
    /// </summary>
    /// <typeparam name="R">返回类型</typeparam>
    /// <typeparam name="T1">元组类型1</typeparam>
    /// <typeparam name="T2">元组类型2</typeparam>
    /// <typeparam name="T3">元组类型3</typeparam>
    /// <param name="handler">操作</param>
    /// <returns>操作</returns>
    public static Func<T1, T2, T3, ValueTask<R>> acry<R, T1, T2, T3>(Func<T1, T2, T3, ValueTask<R>> handler)
    {
        return handler;
    }
}

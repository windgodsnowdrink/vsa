#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package StackExchange.Redis@2.7.18
#:property LangVersion=preview
#:property TargetFramework=net10.0

using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using StackExchange.Redis;

[SkipLocalsInit]
public sealed class CertificateLockService
{
    private readonly IDatabase _redis;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public CertificateLockService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
        _latencyOptimizer = new TailLatencyOptimizer();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<RedisLock> AcquireLockAsync(string domain, TimeSpan expiry)
    {
        using var latencyToken = _latencyOptimizer.BeginOperation();
        var token = Guid.NewGuid().ToString();
        var key = $"cert:lock:{domain}";
        var acquired = await _redis.LockTakeAsync(
            key, 
            token, 
            expiry);

        return new RedisLock(_redis, key, token, acquired);
    }
}

public readonly struct RedisLock : IDisposable
{
    private readonly IDatabase _db;
    private readonly string _key;
    private readonly string _token;

    public bool IsAcquired { get; }

    public RedisLock(IDatabase db, string key, string token, bool isAcquired)
    {
        _db = db;
        _key = key;
        _token = token;
        IsAcquired = isAcquired;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (IsAcquired)
        {
            _db.LockRelease(_key, _token);
        }
    }
}

/// <summary>
/// 尾延迟优化器 - 用于优化操作的尾延迟
/// </summary>
public class TailLatencyOptimizer
{
    /// <summary>
    /// 开始操作 - 创建一个延迟令牌
    /// </summary>
    /// <returns>延迟令牌，用于跟踪操作完成</returns>
    public IDisposable BeginOperation()
    {
        return new TailLatencyToken();
    }

    /// <summary>
    /// 尾延迟令牌 - 用于跟踪操作的执行时间
    /// </summary>
    private class TailLatencyToken : IDisposable
    {
        /// <summary>
        /// 操作开始时间
        /// </summary>
        private readonly DateTime _startTime;

        /// <summary>
        /// 构造函数 - 记录操作开始时间
        /// </summary>
        public TailLatencyToken()
        {
            _startTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 释放资源 - 计算操作执行时间
        /// </summary>
        public void Dispose()
        {
            var executionTime = DateTime.UtcNow - _startTime;
            // 这里可以添加尾延迟监控逻辑
            if (executionTime.TotalMilliseconds > 100)
            {
                Console.WriteLine($"警告：操作执行时间较长 - {executionTime.TotalMilliseconds:F2}ms");
            }
        }
    }
}

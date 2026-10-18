using System.Collections.Concurrent;
using Microsoft.VisualStudio.Threading;

namespace Plc.Host.Concurrency;

/// <summary>
/// 按键的惰性异步单例注册表：并发命中同一 key 时，工厂函数<strong>只执行一次</strong>，
/// 其余调用者共享同一个结果（基于 VS.Threading 的 <see cref="AsyncLazy{T}"/>）。
/// <para>
/// <strong>用途</strong>：ADR-119 M4 的会话池将用它保证「同一设备同一时刻只有一个会话被创建」——
/// 否则 N 个并发请求会创建 N 条 TCP 连接，正是 M5 门禁 G2 要卡的行为。
/// 传入 <see cref="JoinableTaskFactory"/> 后，工厂内部的异步等待可被 JoinableTask 正确跟踪与排空。
/// </para>
/// </summary>
/// <typeparam name="TKey">键类型（如设备标识）。</typeparam>
/// <typeparam name="TValue">值类型（如 <c>IDeviceSession</c>）。</typeparam>
public sealed class AsyncLazyRegistry<TKey, TValue>
    where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, AsyncLazy<TValue>> _items;
    private readonly Func<TKey, Task<TValue>> _factory;
    private readonly JoinableTaskFactory? _joinableTaskFactory;

    /// <param name="factory">值工厂；对同一 key 至多执行一次（成功才算数，见 <see cref="AsyncLazy{T}"/> 语义）。</param>
    /// <param name="joinableTaskFactory">可选；传入后惰性初始化可被 JoinableTask 跟踪，便于优雅关闭时排空。</param>
    /// <param name="comparer">键比较器。</param>
    public AsyncLazyRegistry(
        Func<TKey, Task<TValue>> factory,
        JoinableTaskFactory? joinableTaskFactory = null,
        IEqualityComparer<TKey>? comparer = null)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _joinableTaskFactory = joinableTaskFactory;
        _items = new ConcurrentDictionary<TKey, AsyncLazy<TValue>>(comparer ?? EqualityComparer<TKey>.Default);
    }

    /// <summary>已登记 key 数（含尚未求值的）。</summary>
    public int Count => _items.Count;

    /// <summary>已登记 key 快照。</summary>
    public IEnumerable<TKey> Keys => _items.Keys;

    /// <summary>
    /// 获取（必要时创建）key 对应的值。并发调用同一 key 时工厂只执行一次。
    /// </summary>
    public Task<TValue> GetOrAddAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var lazy = _items.GetOrAdd(
            key,
            static (k, s) => new AsyncLazy<TValue>(() => s.Factory(k), s.Jtf),
            (Factory: _factory, Jtf: _joinableTaskFactory));

        return lazy.GetValueAsync(cancellationToken);
    }

    /// <summary>已创建过则直接返回，不触发工厂。</summary>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        value = default;
        return _items.TryGetValue(key, out _);
    }

    /// <summary>
    /// 移除登记项，并按 <see cref="IAsyncDisposable"/> / <see cref="IDisposable"/> 释放其值。
    /// 值从未成功创建时无资源可释放，返回 true。
    /// </summary>
    public async ValueTask<bool> TryRemoveAsync(TKey key)
    {
        if (!_items.TryRemove(key, out var lazy))
        {
            return false;
        }

        TValue value;
        try
        {
            value = await lazy.GetValueAsync().ConfigureAwait(false);
        }
        catch
        {
            // 工厂从未成功执行：没有资源可释放
            return true;
        }

        // 必须全名限定：Microsoft.VisualStudio.Threading 自带一个同名的旧 IAsyncDisposable，
        // 直接用简单名会触发 CS0104 二义性。
        switch (value)
        {
            case global::System.IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case global::System.IDisposable disposable:
                disposable.Dispose();
                break;
        }

        return true;
    }

    /// <summary>移除全部登记项并逐个释放。</summary>
    public async ValueTask<int> ClearAsync()
    {
        var keys = _items.Keys.ToArray();
        var removed = 0;
        foreach (var key in keys)
        {
            if (await TryRemoveAsync(key).ConfigureAwait(false))
            {
                removed++;
            }
        }

        return removed;
    }
}

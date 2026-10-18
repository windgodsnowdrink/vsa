using System.Collections.Concurrent;
using Microsoft.VisualStudio.Threading;

namespace Plc.Host.Concurrency;

/// <summary>
/// 按键（如设备标识 / 协议标识）限流的异步并发闸门，基于 VS.Threading 的 <see cref="AsyncSemaphore"/>。
/// <para>
/// <strong>为什么需要</strong>：PLC 串口网关、Modbus 从站、S7 这类现场设备通常有并发上限，
/// 超限时的表现不是「报错」而是「不回包 → 超时」（正是本轮修掉的 P0 缺陷的症状）。
/// 因此必须在本侧主动限流，而不是等现场设备拒绝。
/// </para>
/// <para>
/// <strong>闸门粒度</strong>：按 key 独立计数，不同设备之间<strong>互不阻塞</strong>；
/// 全局并发不受本闸门约束（那是 Disruptor 管道的职责，见 ADR-119 M3）。
/// </para>
/// <para>
/// <strong>键空间必须是有界的</strong>（设备标识这类有限集合）：信号量一旦创建即常驻，
/// 不会摘除——并发摘除会让后来的调用者新建一个信号量，从而突破上限（实测会踩到）。
/// 计数项在归零时原子摘除，因此 <see cref="Snapshot"/> 只反映「有在飞任务」的 key。
/// </para>
/// </summary>
public sealed class KeyedAsyncGate
{
    private readonly int _perKeyLimit;
    private readonly ConcurrentDictionary<string, AsyncSemaphore> _semaphores;
    private readonly ConcurrentDictionary<string, int> _inFlight;

    /// <param name="perKeyLimit">同一 key 允许的最大并发数（≥1）。</param>
    /// <param name="comparer">key 比较器，默认 <see cref="StringComparer.Ordinal"/>。</param>
    public KeyedAsyncGate(int perKeyLimit, IEqualityComparer<string>? comparer = null)
    {
        if (perKeyLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(perKeyLimit), perKeyLimit, "并发上限必须 ≥ 1");
        }

        _perKeyLimit = perKeyLimit;
        _semaphores = new ConcurrentDictionary<string, AsyncSemaphore>(comparer ?? StringComparer.Ordinal);
        _inFlight = new ConcurrentDictionary<string, int>(comparer ?? StringComparer.Ordinal);
    }

    /// <summary>单 key 并发上限。</summary>
    public int PerKeyLimit => _perKeyLimit;

    /// <summary>当前在飞总数（所有 key 合计）。</summary>
    public int TotalInFlight => _inFlight.Values.Sum();

    /// <summary>当前有在飞任务的 key 数。</summary>
    public int ActiveKeyCount => _inFlight.Count(kv => kv.Value > 0);

    /// <summary>
    /// 进入闸门。返回的 <see cref="Releaser"/> 必须释放（推荐 <c>using var</c>），否则闸门会永久泄漏。
    /// </summary>
    /// <param name="key">限流键（如设备标识）。</param>
    /// <param name="cancellationToken">取消令牌：排队期间可取消，已占用则不可。</param>
    public async Task<Releaser> AcquireAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);

        var semaphore = _semaphores.GetOrAdd(key, _ => new AsyncSemaphore(_perKeyLimit));
        // VS.Threading 的 AsyncSemaphore 用 Releaser 模式（无 Release()）：EnterAsync 返回可释放句柄
        var inner = await semaphore.EnterAsync(cancellationToken).ConfigureAwait(false);
        _inFlight.AddOrUpdate(key, 1, static (_, v) => v + 1);
        return new Releaser(this, key, inner);
    }

    /// <summary>在飞数量快照（用于可观测性与门禁断言）；只含有在飞任务的 key。</summary>
    public IReadOnlyDictionary<string, int> Snapshot() =>
        new Dictionary<string, int>(_inFlight.Where(kv => kv.Value > 0));

    private void Release(string key, AsyncSemaphore.Releaser inner)
    {
        var remaining = _inFlight.AddOrUpdate(key, 0, static (_, v) => Math.Max(0, v - 1));
        inner.Dispose();

        // 归零时原子摘除计数项：TryRemove(KeyValuePair) 只在值仍是 0 时才移除，
        // 因此不会与并发 Acquire（已把它加回 1）互相踩踏。
        if (remaining == 0)
        {
            _inFlight.TryRemove(new KeyValuePair<string, int>(key, 0));
        }
    }

    /// <summary>闸门占用句柄；<see cref="Dispose"/> 即释放。</summary>
    public readonly struct Releaser : IDisposable
    {
        private readonly KeyedAsyncGate _gate;
        private readonly string _key;
        private readonly AsyncSemaphore.Releaser _inner;

        internal Releaser(KeyedAsyncGate gate, string key, AsyncSemaphore.Releaser inner)
        {
            _gate = gate;
            _key = key;
            _inner = inner;
        }

        public void Dispose() => _gate.Release(_key, _inner);
    }
}

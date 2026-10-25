// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading;

namespace HslCommunication.Core;

/// <summary>
/// 一个简单的混合线程同步锁，采用了基元用户加基元内核同步构造实现
/// A simple hybrid thread editing lock, implemented by the base user plus the element kernel synchronization.
/// </summary>
/// <remarks>
/// 当前的锁适用于，竞争频率比较低，锁部分的代码运行时间比较久的情况，当前的简单混合锁可以达到最大性能。
/// </remarks>
public sealed class SimpleHybirdLock : IDisposable
{
	private bool disposedValue = false;

	/// <summary>
	/// 基元用户模式构造同步锁
	/// </summary>
	private int m_waiters = 0;

	private int m_lock_tick = 0;

	private DateTime m_enterlock_time = DateTime.Now;

	/// <summary>
	/// 基元内核模式构造同步锁
	/// </summary>
	private readonly Lazy<AutoResetEvent> m_waiterLock = new Lazy<AutoResetEvent>(() => new AutoResetEvent(initialState: false));

	private static long simpleHybirdLockCount;

	private static long simpleHybirdLockWaitCount;

	public bool IsWaitting => m_waiters != 0;

	public int LockingTick => m_lock_tick;

	public static long SimpleHybirdLockCount => simpleHybirdLockCount;

	public static long SimpleHybirdLockWaitCount => simpleHybirdLockWaitCount;

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}
			m_waiterLock.Value.Close();
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	/// <summary>
	/// 获取锁，可以指定获取锁的超时时间，如果指定的时间没有获取锁，则返回False，反之，返回True
	/// To acquire a lock, you can specify the timeout period for acquiring a lock, return False if the specified time does not acquire a lock, and vice versa, return True
	/// </summary>
	/// <returns>是否正确的获得锁</returns>
	public bool Enter()
	{
		Interlocked.Increment(ref simpleHybirdLockCount);
		if (Interlocked.Increment(ref m_waiters) == 1)
		{
			m_enterlock_time = DateTime.Now;
			return true;
		}
		Interlocked.Increment(ref simpleHybirdLockWaitCount);
		Interlocked.Increment(ref m_lock_tick);
		bool flag = m_waiterLock.Value.WaitOne();
		if (!flag)
		{
			Interlocked.Decrement(ref simpleHybirdLockCount);
			Interlocked.Decrement(ref simpleHybirdLockWaitCount);
			Interlocked.Decrement(ref m_lock_tick);
		}
		else
		{
			m_enterlock_time = DateTime.Now;
		}
		return flag;
	}

	/// <summary>
	/// 离开锁
	/// Leave the lock
	/// </summary>
	/// <returns>如果该操作成功，返回True，反之，返回False</returns>
	public bool Leave()
	{
		Interlocked.Decrement(ref simpleHybirdLockCount);
		if (Interlocked.Decrement(ref m_waiters) == 0)
		{
			return true;
		}
		bool success = false;
		success = m_waiterLock.Value.Set();
		Interlocked.Decrement(ref simpleHybirdLockWaitCount);
		Interlocked.Decrement(ref m_lock_tick);
		return success;
	}

	public override string ToString()
	{
		if (m_lock_tick > 0)
		{
			return $"SimpleHybirdLock[WaitOne-{DateTime.Now - m_enterlock_time}]";
		}
		if (m_waiters != 0)
		{
			return $"SimpleHybirdLock[OneLock-{DateTime.Now - m_enterlock_time}]";
		}
		return "SimpleHybirdLock[NoneLock]";
	}
}

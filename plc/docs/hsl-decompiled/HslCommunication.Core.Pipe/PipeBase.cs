// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 管道的基础类对象
/// </summary>
public class PipeBase : IDisposable
{
	private SimpleHybirdLock hybirdLock;

	public int LockingTick => hybirdLock.LockingTick;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public PipeBase()
	{
		hybirdLock = new SimpleHybirdLock();
	}

	/// <summary>
	/// 获取锁，可以指定获取锁的超时时间，如果指定的时间没有获取锁，则返回False，反之，返回True
	/// To acquire a lock, you can specify the timeout period for acquiring a lock, return False if the specified time does not acquire a lock, and vice versa, return True
	/// </summary>
	/// <returns>是否正确的获得锁</returns>
	public bool PipeLockEnter()
	{
		return hybirdLock.Enter();
	}

	/// <summary>
	/// 离开锁
	/// Leave the lock
	/// </summary>
	/// <returns>如果该操作成功，返回True，反之，返回False</returns>
	public bool PipeLockLeave()
	{
		return hybirdLock.Leave();
	}

	public virtual void Dispose()
	{
		hybirdLock?.Dispose();
	}
}

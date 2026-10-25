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
/// 用于通信的一个简单的基元混合锁
/// </summary>
public class CommunicationLockSimple : CommunicationLockNone
{
	private int m_waiters = 0;

	private readonly AutoResetEvent m_waiterLock = new AutoResetEvent(initialState: false);

	public bool IsWaitting => m_waiters != 0;

	public override OperateResult EnterLock(int timeout)
	{
		try
		{
			if (Interlocked.Increment(ref m_waiters) == 1)
			{
				return OperateResult.CreateSuccessResult();
			}
			return m_waiterLock.WaitOne(timeout) ? OperateResult.CreateSuccessResult() : new OperateResult($"Enter lock failed, timeout: {timeout}");
		}
		catch (Exception ex)
		{
			return new OperateResult("Enter lock failed, message: " + ex.Message);
		}
	}

	public override void LeaveLock()
	{
		if (Interlocked.Decrement(ref m_waiters) != 0)
		{
			bool flag = m_waiterLock.Set();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
		m_waiterLock.Close();
	}

	public override string ToString()
	{
		return "CommunicationLockSimple[" + (IsWaitting ? "Locking" : "Unlock") + "]";
	}
}

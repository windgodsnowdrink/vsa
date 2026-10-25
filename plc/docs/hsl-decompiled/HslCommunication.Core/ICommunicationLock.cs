// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Core;

/// <summary>
/// 用于通信的锁的接口对象，定义了进入锁和离开锁的方法接口信息
/// An interface object for a lock used for communication that defines interface information for the methods used to enter and exit the lock
/// </summary>
public interface ICommunicationLock : IDisposable
{
	/// <summary>
	/// 进入锁操作，指定超时时间，单位毫秒，并返回是否成功获得锁的标记。
	/// Enters the lock operation, specifies the timeout period in milliseconds, and returns a flag as to whether the lock was successfully obtained.
	/// </summary>
	/// <param name="timeout">超时时间</param>
	/// <returns>是否成功的获得锁</returns>
	OperateResult EnterLock(int timeout);

	/// <summary>
	/// 离开锁操作
	/// Lockaway operation
	/// </summary>
	void LeaveLock();
}

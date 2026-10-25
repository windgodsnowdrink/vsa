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
/// 一个用于高性能，乐观并发模型控制操作的类，允许一个方法(隔离方法)的安全单次执行
/// </summary>
public sealed class HslAsyncCoordinator
{
	private Action action = null;

	private int OperaterStatus = 0;

	private long Target = 0L;

	/// <summary>
	/// 实例化一个对象，需要传入隔离执行的方法
	/// </summary>
	/// <param name="operater">隔离执行的方法</param>
	public HslAsyncCoordinator(Action operater)
	{
		action = operater;
	}

	/// <summary>
	/// 启动线程池执行隔离方法
	/// </summary>
	public void StartOperaterInfomation()
	{
		Interlocked.Increment(ref Target);
		if (Interlocked.CompareExchange(ref OperaterStatus, 1, 0) == 0)
		{
			ThreadPool.QueueUserWorkItem(ThreadPoolOperater, null);
		}
	}

	private void ThreadPoolOperater(object obj)
	{
		long num = Target;
		long num2 = 0L;
		long num3;
		do
		{
			num3 = num;
			action?.Invoke();
			num = Interlocked.CompareExchange(ref Target, num2, num3);
		}
		while (num3 != num);
		Interlocked.Exchange(ref OperaterStatus, 0);
		if (Target != num2)
		{
			StartOperaterInfomation();
		}
	}

	public override string ToString()
	{
		return "HslAsyncCoordinator";
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Threading;

namespace HslCommunication.Core;

/// <summary>
/// 一个双检锁的示例，适合一些占内存的静态数据对象，获取的时候才实例化真正的对象
/// </summary>
internal sealed class Singleton
{
	private static object m_lock = new object();

	private static Singleton SValue = null;

	public static Singleton GetSingleton()
	{
		if (SValue != null)
		{
			return SValue;
		}
		Monitor.Enter(m_lock);
		if (SValue == null)
		{
			Singleton value = new Singleton();
			Volatile.Write(ref SValue, value);
			SValue = new Singleton();
		}
		Monitor.Exit(m_lock);
		return SValue;
	}
}

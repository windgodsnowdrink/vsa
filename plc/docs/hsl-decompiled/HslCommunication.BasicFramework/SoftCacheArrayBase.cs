// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using HslCommunication.Core;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 内存队列的基类
/// </summary>
public abstract class SoftCacheArrayBase
{
	/// <summary>
	/// 字节数据流
	/// </summary>
	protected byte[] DataBytes = null;

	/// <summary>
	/// 数据数组变动时的数据锁
	/// </summary>
	protected SimpleHybirdLock HybirdLock = new SimpleHybirdLock();

	/// <summary>
	/// 数据的长度
	/// </summary>
	public int ArrayLength { get; protected set; }

	/// <summary>
	/// 用于从保存的数据对象初始化的
	/// </summary>
	public virtual void LoadFromBytes(byte[] dataSave)
	{
	}

	/// <summary>
	/// 获取原本的数据字节
	/// </summary>
	/// <returns>字节数组</returns>
	public byte[] GetAllData()
	{
		byte[] buffer = new byte[DataBytes.Length];
		DataBytes.CopyTo(buffer, 0);
		return buffer;
	}
}

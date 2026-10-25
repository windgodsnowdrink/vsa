// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Algorithms.ConnectPool;

/// <summary>
/// 连接池的接口，连接池的管理对象必须实现此接口
/// </summary>
/// <remarks>
/// 为了使用完整的连接池功能，需要先实现本接口，然后配合来使用
/// </remarks>
public interface IConnector
{
	/// <summary>
	/// 指示当前的连接是否在使用用
	/// </summary>
	bool IsConnectUsing { get; set; }

	/// <summary>
	/// 唯一的GUID码
	/// </summary>
	string GuidToken { get; set; }

	/// <summary>
	/// 最新一次使用的时间
	/// </summary>
	DateTime LastUseTime { get; set; }

	/// <summary>
	/// 打开连接
	/// </summary>
	void Open();

	/// <summary>
	/// 关闭并释放
	/// </summary>
	void Close();
}

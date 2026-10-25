// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;

namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// Open协议的消息对象信息
/// </summary>
public class OpenEventArgs : EventArgs
{
	/// <summary>
	/// Open协议的消息内容
	/// </summary>
	public string Content { get; set; }

	/// <summary>
	/// 接收到的原数据信息
	/// </summary>
	public byte[] RawData { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public OpenEventArgs()
	{
	}

	/// <summary>
	/// 指定Open的消息来实例化对象
	/// </summary>
	/// <param name="content">Open内容</param>
	public OpenEventArgs(byte[] content)
	{
		RawData = content;
		Content = Encoding.ASCII.GetString(content).TrimEnd('\0');
	}

	/// <summary>
	/// 指定Open的消息来实例化对象
	/// </summary>
	/// <param name="content">Open内容</param>
	public OpenEventArgs(string content)
	{
		Content = content;
	}
}

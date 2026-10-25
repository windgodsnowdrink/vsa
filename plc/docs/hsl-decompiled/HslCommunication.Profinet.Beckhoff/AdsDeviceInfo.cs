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

namespace HslCommunication.Profinet.Beckhoff;

/// <summary>
/// Ads设备的相关信息，主要是版本号，设备名称
/// Information about Ads devices, primarily version numbers, device names.
/// </summary>
public class AdsDeviceInfo
{
	/// <summary>
	/// 主版本号
	/// Main Version
	/// </summary>
	public byte Major { get; set; }

	/// <summary>
	/// 次版本号
	/// Minor Version
	/// </summary>
	public byte Minor { get; set; }

	/// <summary>
	/// 构建版本号
	/// Build version
	/// </summary>
	public ushort Build { get; set; }

	/// <summary>
	/// 设备的名字
	/// Device Name
	/// </summary>
	public string DeviceName { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public AdsDeviceInfo()
	{
	}

	/// <summary>
	/// 根据原始的数据内容来实例化一个对象
	/// Instantiate an object based on the original data content
	/// </summary>
	/// <param name="data">原始的数据内容</param>
	public AdsDeviceInfo(byte[] data)
	{
		Major = data[0];
		Minor = data[1];
		Build = BitConverter.ToUInt16(data, 2);
		DeviceName = Encoding.ASCII.GetString(data.RemoveBegin(4)).Trim('\0', ' ');
	}
}

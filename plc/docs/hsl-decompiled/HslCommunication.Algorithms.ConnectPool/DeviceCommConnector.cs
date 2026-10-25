// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core.Device;

namespace HslCommunication.Algorithms.ConnectPool;

/// <summary>
/// 基于设备通信的连接池信息
/// Connection pool information based on device communication
/// </summary>
public class DeviceCommConnector : IConnector
{
	private DeviceCommunication device = null;

	public DeviceCommunication Device => device;

	/// <summary>
	/// 指示当前的连接是否在使用用
	/// </summary>
	public bool IsConnectUsing { get; set; }

	/// <summary>
	/// 唯一的GUID码
	/// </summary>
	public string GuidToken { get; set; }

	/// <summary>
	/// 最新一次使用的时间
	/// </summary>
	public DateTime LastUseTime { get; set; }

	/// <summary>
	/// 使用指定的设备来实例化一个对象
	/// Use the specified device to instantiate an object
	/// </summary>
	/// <param name="device">指定的设备通信</param>
	public DeviceCommConnector(DeviceCommunication device)
	{
		this.device = device;
	}

	/// <summary>
	/// 关闭并释放
	/// </summary>
	public void Close()
	{
		device.CommunicationPipe.CloseCommunication();
	}

	/// <summary>
	/// 打开连接
	/// </summary>
	public void Open()
	{
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
namespace HslCommunication.Profinet.Beckhoff;

/// <summary>
/// 倍福PLC的命令码
/// Command Id: https://infosys.beckhoff.com/english.php?content=../content/1033/tc3_ads_intro/index.html
/// </summary>
public class BeckhoffCommandId
{
	/// <summary>
	/// 读取PLC的名字和版本号等信息
	/// Reads the name and the version number of the ADS device.
	/// </summary>
	public const ushort ReadDeviceInfo = 1;

	/// <summary>
	/// 可以读ADS的设备读取数据
	/// With ADS Read data can be read from an ADS device
	/// </summary>
	public const ushort Read = 2;

	/// <summary>
	/// 将ADS数据写入到ADS的设备里去
	/// With ADS Write data can be written to an ADS device
	/// </summary>
	public const ushort Write = 3;

	/// <summary>
	/// 读取ADS设备里的设备状态信息和ADS状态
	/// Reads the ADS status and the device status of an ADS device.
	/// </summary>
	public const ushort ReadState = 4;

	/// <summary>
	/// 更改ADS设备的设备状态信息和ADS状态
	/// Changes the ADS status and the device status of an ADS device.
	/// </summary>
	public const ushort WriteControl = 5;

	/// <summary>
	/// 在ADS设备里面创建一个通知对象
	/// A notification is created in an ADS device
	/// </summary>
	public const ushort AddDeviceNotification = 6;

	/// <summary>
	/// 删除ADS设备里的一个通知对象
	/// One before defined notification is deleted in an ADS device.
	/// </summary>
	public const ushort DeleteDeviceNotification = 7;

	/// <summary>
	/// 从ADS设备订阅一个数据的通知，将会发送到客户端
	/// Data will carry forward independently from an ADS device to a Client
	/// </summary>
	public const ushort DeviceNotification = 8;

	/// <summary>
	/// 在写入的时候进行同时的读取
	/// With ADS ReadWrite data will be written to an ADS device. Additionally, data can be read from the ADS device.
	/// </summary>
	public const ushort ReadWrite = 9;
}

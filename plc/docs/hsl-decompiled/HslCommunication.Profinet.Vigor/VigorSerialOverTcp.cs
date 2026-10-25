// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Vigor.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Vigor;

/// <summary>
/// 丰炜通信协议的网口透传版本，支持VS系列，地址支持携带站号，例如 s=2;D100, 字地址支持 D,SD,R,T,C(C200-C255是32位寄存器), 位地址支持X,Y,M,SM,S,TS(定时器触点),TC（定时器线圈）,CS(计数器触点),CC（计数器线圈)
/// The network port transparent transmission version of Fengwei communication protocol supports VS series, and the address supports carrying station number,
/// such as s=2;D100, word address supports D, SD, R, T, C (C200-C255 are 32-bit registers), Bit address supports X, Y, M, SM, S, TS (timer contact),
/// TC (timer coil), CS (counter contact), CC (counter coil)
/// </summary>
/// <remarks>
/// 暂时不支持对字寄存器(D,R)进行读写位操作，感谢随时关注库的更新日志
/// </remarks>
public class VigorSerialOverTcp : DeviceTcpNet
{
	/// <summary>
	/// 获取或设置当前PLC的站号信息
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 实例化默认的构造方法
	/// Instantiate the default constructor
	/// </summary>
	public VigorSerialOverTcp()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 使用指定的ip地址和端口来实例化一个对象
	/// Instantiate an object with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">设备的Ip地址</param>
	/// <param name="port">设备的端口号</param>
	public VigorSerialOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new VigorSerialMessage();
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return VigorVsHelper.CheckResponseContent(response);
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return VigorVsHelper.BuildReadCommand(Station, address, length, isBit);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return VigorVsHelper.BuildWriteWordCommand(Station, address, value);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return VigorVsHelper.BuildWriteBoolCommand(Station, address, value);
	}

	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		return OperateResult.CreateSuccessResult(content.ToBoolArray());
	}

	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return HslHelper.ReadBool(this, address, length);
		}
		return base.ReadBool(address, length);
	}

	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return await HslHelper.ReadBoolAsync(this, address, length);
		}
		return await base.ReadBoolAsync(address, length);
	}

	public override string ToString()
	{
		return $"VigorSerialOverTcp[{IpAddress}:{Port}]";
	}
}

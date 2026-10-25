// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Freedom;

/// <summary>
/// 基于串口的自由协议，需要在地址里传入报文信息，也可以传入数据偏移信息，默认为
/// </summary>
public class FreedomSerial : DeviceSerialPort
{
	/// <summary>
	/// 一个对返回数据合法检查的委托实例，默认为空，不进行合法性检查，可以实例化实现任意的报文检测，返回是否合法结果。
	/// A delegate instance that checks the legality of the returned data. It is empty by default and does not perform legality checking.
	/// It can be instantiated to implement any packet inspection and return whether the result is legal or not.
	/// </summary>
	/// <remarks>
	/// 例如返回的第一个字节为0表示正常报文，否则是异常返回，可以简写：CheckResponseStatus = ( send, receive ) => receive[2] == 0 ? OperateResult.CreateSuccessResult( ) : new OperateResult( receive[2], "error" );
	/// For example, if the first byte returned is 0, it means a normal message, otherwise it is an abnormal return,
	/// which can be abbreviated as: CheckResponseStatus = ( send, receive ) => receive[2] == 0 ? OperateResult.CreateSuccessResult( ) : new OperateResult( receive[2], "error" );
	/// </remarks>
	public Func<byte[], byte[], OperateResult> CheckResponseStatus { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FreedomSerial()
	{
		base.ByteTransform = new RegularByteTransform();
	}

	[HslMqttApi("ReadByteArray", "特殊的地址格式，需要采用解析包起始地址的报文，例如 modbus 协议为 stx=9;00 00 00 00 00 06 01 03 00 64 00 01")]
	/// <remarks>
	/// length没有任何意义，需要传入原始的字节报文，例如：stx=9;00 00 00 00 00 06 01 03 00 64 00 01，stx得值用于获取数据移除的前置报文头，可以不填写
	/// length has no meaning, you need to pass in the original byte message, for example: stx=9;00 00 00 00 00 06 01 03 00 64 00 01,
	/// the value of stx is used to obtain the preamble header for data removal, can be left blank
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		int num = HslHelper.ExtractParameter(ref address, "stx", 0);
		byte[] array = address.ToHexBytes();
		OperateResult<byte[]> operateResult = ReadFromCoreServer(array);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (CheckResponseStatus != null)
		{
			OperateResult operateResult2 = CheckResponseStatus(array, operateResult.Content);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>(operateResult2);
			}
		}
		if (num >= operateResult.Content.Length)
		{
			return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.RemoveBegin(num));
	}

	public override OperateResult Write(string address, byte[] value)
	{
		return Read(address, 0);
	}

	public override string ToString()
	{
		return $"FreedomSerial<{base.ByteTransform.GetType()}>[{base.PortName}:{base.BaudRate}]";
	}
}

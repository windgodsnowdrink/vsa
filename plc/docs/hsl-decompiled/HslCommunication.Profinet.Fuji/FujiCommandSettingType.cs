// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Fuji;

/// <summary>
/// 基于Command-Setting-Type通信模式的协议实现，地址格式参数DEMO界面
/// Protocol implementation based on Command-Setting-Type communication mode, address format parameter DEMO interface
/// </summary>
/// <remarks>
/// 本类实例化对象之后，还需要设置属性，根据实际情况来设置。
/// </remarks>
public class FujiCommandSettingType : DeviceTcpNet
{
	private bool dataSwap = false;

	/// <summary>
	/// 获取或设置当前的对象是否进行数据交换操作，将根据PLC的实际值来设定。
	/// Get or set whether the current object performs data exchange operation or not, it will be set according to the actual value of the PLC.
	/// </summary>
	public bool DataSwap
	{
		get
		{
			return dataSwap;
		}
		set
		{
			dataSwap = value;
			if (value)
			{
				base.ByteTransform = new RegularByteTransform();
			}
			else
			{
				base.ByteTransform = new ReverseBytesTransform();
			}
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FujiCommandSettingType()
	{
		base.ByteTransform = new ReverseBytesTransform();
		base.WordLength = 2;
	}

	/// <summary>
	/// 使用指定的IP地址和端口号来实例化一个对象
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号信息</param>
	public FujiCommandSettingType(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FujiCommandSettingTypeMessage();
	}

	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return UnpackResponseContentHelper(send, response);
	}

	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		if (isBit)
		{
			return base.BuildDeviceReadCommand(address, length, isBit);
		}
		return BuildReadCommand(address, length);
	}

	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return BuildWriteCommand(address, value);
	}

	[HslMqttApi("ReadByte", "")]
	/// <summary>
	/// 读取指定地址的byte数据，地址格式 S100
	/// Reads the byte data of the specified address, the address format S100
	/// </summary>
	/// <param name="address">起始地址，格式为S100</param>
	/// <returns>是否读取成功的结果对象</returns>
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	[HslMqttApi("WriteByte", "")]
	/// <summary>
	/// 向PLC中写入byte数据，返回值说明
	/// Write byte data to the PLC, return value description
	/// </summary>
	/// <param name="address">起始地址，格式为 S100</param>
	/// <param name="value">byte数据</param>
	/// <returns>是否写入成功的结果对象</returns>
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	/// <summary>
	/// 读取指定地址的byte数据，地址格式 S100
	/// Reads the byte data of the specified address, the address format S100
	/// </summary>
	/// <param name="address">起始地址，格式为S100</param>
	/// <returns>是否读取成功的结果对象</returns>
	public async Task<OperateResult<byte>> ReadByteAsync(string address)
	{
		return ByteTransformHelper.GetResultFromArray(await ReadAsync(address, 1));
	}

	public async Task<OperateResult> WriteAsync(string address, byte value)
	{
		return await WriteAsync(address, new byte[1] { value });
	}

	public override string ToString()
	{
		return $"FujiCommandSettingType[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// 构建读取的报文指令
	/// </summary>
	/// <param name="address">PLC的地址信息</param>
	/// <param name="length">读取的长度信息</param>
	/// <returns>报文构建的结果对象</returns>
	public static OperateResult<byte[]> BuildReadCommand(string address, ushort length)
	{
		OperateResult<FujiCommandSettingTypeAddress> operateResult = FujiCommandSettingTypeAddress.ParseFrom(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(new byte[9]
		{
			0,
			0,
			0,
			operateResult.Content.DataCode,
			4,
			BitConverter.GetBytes(operateResult.Content.AddressStart)[0],
			BitConverter.GetBytes(operateResult.Content.AddressStart)[1],
			BitConverter.GetBytes(operateResult.Content.Length)[0],
			BitConverter.GetBytes(operateResult.Content.Length)[1]
		});
	}

	/// <summary>
	/// 构建写入原始报文数据的请求信息
	/// </summary>
	/// <param name="address">地址数据</param>
	/// <param name="value">原始报文的数据</param>
	/// <returns>原始的写入报文数据</returns>
	public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		OperateResult<FujiCommandSettingTypeAddress> operateResult = FujiCommandSettingTypeAddress.ParseFrom(address, 0);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] buffer = new byte[9 + value.Length];
		buffer[0] = 1;
		buffer[1] = 0;
		buffer[2] = 0;
		buffer[3] = operateResult.Content.DataCode;
		buffer[4] = (byte)(4 + value.Length);
		buffer[5] = BitConverter.GetBytes(operateResult.Content.AddressStart)[0];
		buffer[6] = BitConverter.GetBytes(operateResult.Content.AddressStart)[1];
		buffer[7] = BitConverter.GetBytes(operateResult.Content.Length)[0];
		buffer[8] = BitConverter.GetBytes(operateResult.Content.Length)[0];
		value.CopyTo(buffer, 9);
		return OperateResult.CreateSuccessResult(buffer);
	}

	/// <summary>
	/// 根据错误信息获取相关数据信息
	/// </summary>
	/// <param name="error">错误代号</param>
	/// <returns>实际的错误描述信息</returns>
	public static string GetErrorText(int error)
	{
		return error switch
		{
			18 => "Write of data to the program area", 
			32 => "Non-existing CMND code", 
			33 => "Input data is not in the order of data corresponding to CMND", 
			34 => "Operation only from the loader is effective. Operation from any other node is disabled", 
			36 => "A non-existing module has been specified", 
			50 => "An address out of the memory size has been specified", 
			_ => StringResources.Language.UnknownError, 
		};
	}

	/// <summary>
	/// 根据PLC返回的数据，解析出实际的数据内容
	/// </summary>
	/// <param name="send">发送给PLC的数据</param>
	/// <param name="response">PLC返回的数据</param>
	/// <returns>结果数据信息</returns>
	public static OperateResult<byte[]> UnpackResponseContentHelper(byte[] send, byte[] response)
	{
		try
		{
			if (response[1] != 0)
			{
				return new OperateResult<byte[]>(GetErrorText(response[1]));
			}
			if (response[0] == 1)
			{
				return OperateResult.CreateSuccessResult(new byte[0]);
			}
			if (response.Length < 10)
			{
				return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + "10, Source: " + response.ToHexString(' '));
			}
			return OperateResult.CreateSuccessResult(response.RemoveBegin(10));
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("UnpackResponseContentHelper failed: " + ex.Message + " Source: " + response.ToHexString(' '));
		}
	}
}

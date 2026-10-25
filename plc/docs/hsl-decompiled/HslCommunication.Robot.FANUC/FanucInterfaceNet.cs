// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;
using HslCommunication.Reflection;
using Newtonsoft.Json;

namespace HslCommunication.Robot.FANUC;

/// <summary>
/// Fanuc机器人的PC Interface实现，在R-30iB mate plus型号上测试通过，支持读写任意的数据，写入操作务必谨慎调用，写入数据不当造成生命财产损失，作者概不负责。读写任意的地址见api文档信息
/// The Fanuc robot's PC Interface implementation has been tested on R-30iB mate plus models. It supports reading and writing arbitrary data. The writing operation must be called carefully.
/// Improper writing of data will cause loss of life and property. The author is not responsible. Read and write arbitrary addresses see api documentation information
/// </summary>
/// <remarks>
/// 注意：如果再读取机器人的数据时，发生了GB2312编码获取的异常的时候(通常是基于.net core的项目会报错)，使用如下的方法进行解决
/// 1. 从nuget安装组件 System.Text.Encoding.CodePages
/// 2. 刚进入系统的时候，调用一行代码： System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
/// Note: If you read the data of the robot again, when an exception occurs in the GB2312 code acquisition (usually a project based on .net core will report an error), use the following method to solve it.
/// 1. Install the component System.Text.Encoding.CodePages from nuget
/// 2. When you first enter the system, call a line of code: System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
/// </remarks>
public class FanucInterfaceNet : DeviceTcpNet, IRobotNet, IReadWriteNet
{
	private FanucData fanucDataRetain = null;

	private DateTime fanucDataRefreshTime = DateTime.Now.AddSeconds(-10.0);

	private PropertyInfo[] fanucDataPropertyInfo = typeof(FanucData).GetProperties();

	private byte[] connect_req = new byte[56];

	private byte[] session_req = new byte[56]
	{
		8, 0, 1, 0, 0, 0, 0, 0, 0, 1,
		0, 0, 0, 0, 0, 0, 0, 1, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		1, 192, 0, 0, 0, 0, 16, 14, 0, 0,
		1, 1, 79, 1, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0
	};

	/// <summary>
	/// 获取或设置当前客户端的ID信息，默认为1024
	/// Gets or sets the ID information of the current client. The default is 1024.
	/// </summary>
	public int ClientId { get; private set; } = 1024;

	/// <summary>
	/// 获取或设置缓存的Fanuc数据的有效时间，对方法有效，默认为100，单位毫秒。也即是在100ms内频繁读取机器人的属性数据的时候，优先读取缓存值，提高读取效率。
	/// Gets or sets the valid time of the cached Fanuc data. It is valid for the  method. The default is 100, in milliseconds.
	/// That is, when the attribute data of the robot is frequently read within 100ms, the cache value is preferentially read to improve the reading efficiency.
	/// </summary>
	public int FanucDataRetainTime { get; set; } = 100;

	/// <summary>
	/// 获取或设置解析当前的字符串数据时使用的编码信息，默认使用 Encoding.Default 编码。
	/// Gets or sets the encoding information used when parsing the current string data, using Encoding.Default encoding by default.
	/// </summary>
	public Encoding StringEncoding { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public FanucInterfaceNet()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		StringEncoding = Encoding.Default;
	}

	/// <summary>
	/// 指定ip及端口来实例化一个默认的对象，端口默认60008
	/// Specify the IP and port to instantiate a default object, the port defaults to 60008
	/// </summary>
	/// <param name="ipAddress">ip地址</param>
	/// <param name="port">端口号</param>
	public FanucInterfaceNet(string ipAddress, int port = 60008)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new FanucRobotMessage();
	}

	private OperateResult ReadCommandFromRobot(CommunicationPipe pipe, string[] cmds)
	{
		for (int i = 0; i < cmds.Length; i++)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(cmds[i]);
			OperateResult<byte[]> operateResult = ReadFromCoreServer(pipe, FanucHelper.BuildWriteData(56, 1, bytes, bytes.Length), hasResponseData: true, usePackAndUnpack: true);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override OperateResult InitializationOnConnect()
	{
		BitConverter.GetBytes(ClientId).CopyTo(connect_req, 1);
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, connect_req, hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		operateResult = ReadFromCoreServer(CommunicationPipe, session_req, hasResponseData: true, usePackAndUnpack: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadCommandFromRobot(CommunicationPipe, FanucHelper.GetFanucCmds());
	}

	private async Task<OperateResult> ReadCommandFromRobotAsync(CommunicationPipe pipe, string[] cmds)
	{
		for (int i = 0; i < cmds.Length; i++)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(cmds[i]);
			OperateResult<byte[]> write = await ReadFromCoreServerAsync(pipe, FanucHelper.BuildWriteData(56, 1, buffer, buffer.Length), hasResponseData: true, usePackAndUnpack: true);
			if (!write.IsSuccess)
			{
				return write;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		BitConverter.GetBytes(ClientId).CopyTo(connect_req, 1);
		OperateResult<byte[]> receive = await ReadFromCoreServerAsync(CommunicationPipe, connect_req, hasResponseData: true, usePackAndUnpack: true).ConfigureAwait(continueOnCapturedContext: false);
		if (!receive.IsSuccess)
		{
			return receive;
		}
		receive = await ReadFromCoreServerAsync(CommunicationPipe, session_req, hasResponseData: true, usePackAndUnpack: true).ConfigureAwait(continueOnCapturedContext: false);
		if (!receive.IsSuccess)
		{
			return receive;
		}
		return await ReadCommandFromRobotAsync(CommunicationPipe, FanucHelper.GetFanucCmds()).ConfigureAwait(continueOnCapturedContext: false);
	}

	[HslMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the robot's original byte data information according to the address")]
	/// <summary>
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public OperateResult<byte[]> Read(string address)
	{
		return Read(8, 1, 6130);
	}

	[HslMqttApi(ApiTopic = "ReadRobotString", Description = "Read the string data information of the robot based on the address")]
	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public OperateResult<string> ReadString(string address)
	{
		if (string.IsNullOrEmpty(address))
		{
			OperateResult<FanucData> operateResult = ReadFanucData();
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<string>(operateResult);
			}
			fanucDataRetain = operateResult.Content;
			fanucDataRefreshTime = DateTime.Now;
			return OperateResult.CreateSuccessResult(JsonConvert.SerializeObject((object)operateResult.Content, (Formatting)1));
		}
		if ((DateTime.Now - fanucDataRefreshTime).TotalMilliseconds > (double)FanucDataRetainTime || fanucDataRetain == null)
		{
			OperateResult<FanucData> operateResult2 = ReadFanucData();
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<string>(operateResult2);
			}
			fanucDataRetain = operateResult2.Content;
			fanucDataRefreshTime = DateTime.Now;
		}
		PropertyInfo[] array = fanucDataPropertyInfo;
		foreach (PropertyInfo propertyInfo in array)
		{
			if (propertyInfo.Name == address)
			{
				return OperateResult.CreateSuccessResult(JsonConvert.SerializeObject(propertyInfo.GetValue(fanucDataRetain, null), (Formatting)1));
			}
		}
		return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
	}

	/// <summary>
	/// 根据地址读取机器人的原始的字节数据信息
	/// Read the robot's original byte data information according to the address
	/// </summary>
	/// <param name="address">指定的地址信息，对于某些机器人无效</param>
	/// <returns>带有成功标识的byte[]数组</returns>
	public async Task<OperateResult<byte[]>> ReadAsync(string address)
	{
		return await ReadAsync(8, 1, 6130);
	}

	/// <summary>
	/// 根据地址读取机器人的字符串的数据信息
	/// Read the string data information of the robot based on the address
	/// </summary>
	/// <param name="address">地址信息</param>
	/// <returns>带有成功标识的字符串数据</returns>
	public async Task<OperateResult<string>> ReadStringAsync(string address)
	{
		if (string.IsNullOrEmpty(address))
		{
			OperateResult<FanucData> read = await ReadFanucDataAsync();
			if (!read.IsSuccess)
			{
				return OperateResult.CreateFailedResult<string>(read);
			}
			fanucDataRetain = read.Content;
			fanucDataRefreshTime = DateTime.Now;
			return OperateResult.CreateSuccessResult(JsonConvert.SerializeObject((object)read.Content, (Formatting)1));
		}
		if ((DateTime.Now - fanucDataRefreshTime).TotalMilliseconds > (double)FanucDataRetainTime || fanucDataRetain == null)
		{
			OperateResult<FanucData> read2 = await ReadFanucDataAsync();
			if (!read2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<string>(read2);
			}
			fanucDataRetain = read2.Content;
			fanucDataRefreshTime = DateTime.Now;
		}
		PropertyInfo[] array = fanucDataPropertyInfo;
		foreach (PropertyInfo item in array)
		{
			if (item.Name == address)
			{
				return OperateResult.CreateSuccessResult(JsonConvert.SerializeObject(item.GetValue(fanucDataRetain, null), (Formatting)1));
			}
		}
		return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
	}

	[HslMqttApi("ReadByteArray", "")]
	/// <summary>
	/// 按照字为单位批量读取设备的原始数据，需要指定地址及长度，地址示例：D1，AI1，AQ1，共计3个区的数据，注意地址的起始为1
	/// Read the raw data of the device in batches in units of words. You need to specify the address and length. Example addresses: D1, AI1, AQ1, a total of 3 areas of data. Note that the start of the address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1</param>
	/// <param name="length">读取的长度，字为单位</param>
	/// <returns>返回的数据信息结果</returns>
	/// <remarks>
	/// 地址也支持直接使用 GO100, GI100
	/// </remarks>
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content1 == 8 || operateResult.Content1 == 10 || operateResult.Content1 == 12)
		{
			return Read(operateResult.Content1, operateResult.Content2, length);
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + ", Current address not support word read/write");
	}

	[HslMqttApi("WriteByteArray", "")]
	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1
	/// Write the original byte array data to the specified address, and return whether the write was successful. Example addresses: D1, AI1, AQ1, a total of 3 areas of data. Note that the start of the address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	/// <remarks>
	/// 地址也支持直接使用 GO100, GI100
	/// </remarks>
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (operateResult.Content1 == 8 || operateResult.Content1 == 10 || operateResult.Content1 == 12)
		{
			return Write(operateResult.Content1, operateResult.Content2, value);
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + ", Current address not support word read/write");
	}

	[HslMqttApi("ReadBoolArray", "")]
	/// <summary>
	/// 按照位为单位批量读取设备的原始数据，需要指定地址及长度，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1
	/// Read the raw data of the device in batches in units of boolean. You need to specify the address and length. Example addresses: M1，I1，Q1, a total of 3 areas of data. Note that the start of the address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1</param>
	/// <param name="length">读取的长度，位为单位</param>
	/// <returns>返回的数据信息结果</returns>
	/// <remarks>
	/// 地址也支持直接使用 SDO100, SDI100, RDI100, RDO100, UI100, UO100, SI100, SO100
	/// </remarks>
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (operateResult.Content1 == 70 || operateResult.Content1 == 72 || operateResult.Content1 == 76)
		{
			return ReadBool(operateResult.Content1, operateResult.Content2, length);
		}
		return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType + ", Current address not support bool read/write");
	}

	[HslMqttApi("WriteBoolArray", "")]
	/// <summary>
	/// 批量写入数组数据，返回是否写入成功，需要指定起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1
	/// Write  array data in batches. If the write success is returned, you need to specify the starting address. Example address: M1, I1, Q1, a total of 3 areas of data. Note that the starting address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1</param>
	/// <param name="value">等待写入的数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 地址也支持直接使用 SDO100, SDI100, RDI100, RDO100, UI100, UO100, SI100, SO100
	/// </remarks>
	public override OperateResult Write(string address, bool[] value)
	{
		OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content1 == 70 || operateResult.Content1 == 72 || operateResult.Content1 == 76)
		{
			return WriteBool(operateResult.Content1, operateResult.Content2, value);
		}
		return new OperateResult(StringResources.Language.NotSupportedDataType + ", Current address not support bool read/write");
	}

	/// <summary>
	/// 按照字为单位批量读取设备的原始数据，需要指定地址及长度，地址示例：D1，AI1，AQ1，共计3个区的数据，注意地址的起始为1
	/// Read the raw data of the device in batches in units of words. You need to specify the address and length. Example addresses: D1, AI1, AQ1, a total of 3 areas of data. Note that the start of the address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1</param>
	/// <param name="length">读取的长度，字为单位</param>
	/// <returns>返回的数据信息结果</returns>
	/// <remarks>
	/// 地址也支持直接使用 GO100, GI100
	/// </remarks>
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address, isBit: false);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(analysis);
		}
		if (analysis.Content1 == 8 || analysis.Content1 == 10 || analysis.Content1 == 12)
		{
			return await ReadAsync(analysis.Content1, analysis.Content2, length);
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + ", Current address not support word read/write");
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1
	/// Write the original byte array data to the specified address, and return whether the write was successful. Example addresses: D1, AI1, AQ1, a total of 3 areas of data. Note that the start of the address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1</param>
	/// <param name="value">写入值</param>
	/// <returns>带有成功标识的结果类对象</returns>
	/// <remarks>
	/// 地址也支持直接使用 GO100, GI100
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address, isBit: false);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(analysis);
		}
		if (analysis.Content1 == 8 || analysis.Content1 == 10 || analysis.Content1 == 12)
		{
			return await WriteAsync(analysis.Content1, analysis.Content2, value);
		}
		return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + ", Current address not support word read/write");
	}

	/// <summary>
	/// 按照位为单位批量读取设备的原始数据，需要指定地址及长度，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1
	/// Read the raw data of the device in batches in units of boolean. You need to specify the address and length. Example addresses: M1，I1，Q1, a total of 3 areas of data. Note that the start of the address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1</param>
	/// <param name="length">读取的长度，位为单位</param>
	/// <returns>返回的数据信息结果</returns>
	/// <remarks>
	/// 地址也支持直接使用 SDO100, SDI100, RDI100, RDO100, UI100, UO100, SI100, SO100
	/// </remarks>
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address, isBit: true);
		if (!analysis.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(analysis);
		}
		if (analysis.Content1 == 70 || analysis.Content1 == 72 || analysis.Content1 == 76)
		{
			return await ReadBoolAsync(analysis.Content1, analysis.Content2, length);
		}
		return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType + ", Current address not support bool read/write");
	}

	/// <summary>
	/// 批量写入数组数据，返回是否写入成功，需要指定起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1
	/// Write  array data in batches. If the write success is returned, you need to specify the starting address. Example address: M1, I1, Q1, a total of 3 areas of data. Note that the starting address is 1.
	/// </summary>
	/// <param name="address">起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1</param>
	/// <param name="value">等待写入的数据值</param>
	/// <returns>是否写入成功</returns>
	/// <remarks>
	/// 地址也支持直接使用 SDO100, SDI100, RDI100, RDO100, UI100, UO100, SI100, SO100
	/// </remarks>
	public override async Task<OperateResult> WriteAsync(string address, bool[] value)
	{
		OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address, isBit: true);
		if (!analysis.IsSuccess)
		{
			return analysis;
		}
		if (analysis.Content1 == 70 || analysis.Content1 == 72 || analysis.Content1 == 76)
		{
			return await WriteBoolAsync(analysis.Content1, analysis.Content2, value);
		}
		return new OperateResult(StringResources.Language.NotSupportedDataType + ", Current address not support bool read/write");
	}

	/// <summary>
	/// 按照字为单位批量读取设备的原始数据，需要指定数据块地址，偏移地址及长度，主要针对08, 10, 12的数据块，注意地址的起始为1
	/// Read the raw data of the device in batches in units of words. You need to specify the data block address, offset address, and length. It is mainly for data blocks of 08, 10, and 12. Note that the start of the address is 1.
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，字为单位</param>
	public OperateResult<byte[]> Read(byte select, ushort address, ushort length)
	{
		byte[] send = FanucHelper.BulidReadData(select, address, length);
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content[31] == 148)
		{
			return OperateResult.CreateSuccessResult(SoftBasic.ArrayRemoveBegin(operateResult.Content, 56));
		}
		if (operateResult.Content[31] == 212)
		{
			return OperateResult.CreateSuccessResult(SoftBasic.ArraySelectMiddle(operateResult.Content, 44, length * 2));
		}
		return new OperateResult<byte[]>(operateResult.Content[31], "Error");
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功，，需要指定数据块地址，偏移地址，主要针对08, 10, 12的数据块，注意起始的起始为1
	/// Write the original byte array data to the specified address, and return whether the writing is successful. You need to specify the data block address and offset address,
	/// which are mainly for the data blocks of 08, 10, and 12. Note that the start of the start is 1.
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">原始数据内容</param>
	public OperateResult Write(byte select, ushort address, byte[] value)
	{
		byte[] send = FanucHelper.BuildWriteData(select, address, value, value.Length / 2);
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (operateResult.Content[31] == 212)
		{
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult<byte[]>(operateResult.Content[31], "Error");
	}

	/// <summary>
	/// 按照位为单位批量读取设备的原始数据，需要指定数据块地址，偏移地址及长度，主要针对70, 72, 76的数据块，注意地址的起始为1
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，字为单位</param>
	public OperateResult<bool[]> ReadBool(byte select, ushort address, ushort length)
	{
		int num = address - 1 - (address - 1) % 8 + 1;
		int num2 = (((address + length - 1) % 8 == 0) ? (address + length - 1) : ((address + length - 1) / 8 * 8 + 8));
		int num3 = (num2 - num + 1) / 8;
		byte[] send = FanucHelper.BulidReadData(select, address, (ushort)(num3 * 8));
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		if (operateResult.Content[31] == 148)
		{
			bool[] sourceArray = SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(operateResult.Content, 56));
			bool[] array = new bool[length];
			Array.Copy(sourceArray, address - num, array, 0, length);
			return OperateResult.CreateSuccessResult(array);
		}
		if (operateResult.Content[31] == 212)
		{
			bool[] sourceArray2 = SoftBasic.ByteToBoolArray(SoftBasic.ArraySelectMiddle(operateResult.Content, 44, num3));
			bool[] array2 = new bool[length];
			Array.Copy(sourceArray2, address - num, array2, 0, length);
			return OperateResult.CreateSuccessResult(array2);
		}
		return new OperateResult<bool[]>(operateResult.Content[31], "Error");
	}

	/// <summary>
	/// 批量写入数组数据，返回是否写入成功，需要指定数据块地址，偏移地址，主要针对70, 72, 76的数据块，注意起始的起始为1
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">原始的数据内容</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteBool(byte select, ushort address, bool[] value)
	{
		int num = address - 1 - (address - 1) % 8 + 1;
		int num2 = (((address + value.Length - 1) % 8 == 0) ? (address + value.Length - 1) : ((address + value.Length - 1) / 8 * 8 + 8));
		int num3 = (num2 - num + 1) / 8;
		bool[] array = new bool[num3 * 8];
		Array.Copy(value, 0, array, address - num, value.Length);
		byte[] send = FanucHelper.BuildWriteData(select, address, base.ByteTransform.TransByte(array), value.Length);
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<short[]>(operateResult);
		}
		if (operateResult.Content[31] == 212)
		{
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult();
	}

	/// <summary>
	/// 按照字为单位批量读取设备的原始数据，需要指定数据块地址，偏移地址及长度，主要针对08, 10, 12的数据块，注意地址的起始为1
	/// Read the raw data of the device in batches in units of words. You need to specify the data block address, offset address, and length. It is mainly for data blocks of 08, 10, and 12. Note that the start of the address is 1.
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，字为单位</param>
	public async Task<OperateResult<byte[]>> ReadAsync(byte select, ushort address, ushort length)
	{
		byte[] send = FanucHelper.BulidReadData(select, address, length);
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(send);
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content[31] == 148)
		{
			return OperateResult.CreateSuccessResult(SoftBasic.ArrayRemoveBegin(read.Content, 56));
		}
		if (read.Content[31] == 212)
		{
			return OperateResult.CreateSuccessResult(SoftBasic.ArraySelectMiddle(read.Content, 44, length * 2));
		}
		return new OperateResult<byte[]>(read.Content[31], "Error");
	}

	/// <summary>
	/// 写入原始的byte数组数据到指定的地址，返回是否写入成功，，需要指定数据块地址，偏移地址，主要针对08, 10, 12的数据块，注意起始的起始为1
	/// Write the original byte array data to the specified address, and return whether the writing is successful. You need to specify the data block address and offset address,
	/// which are mainly for the data blocks of 08, 10, and 12. Note that the start of the start is 1.
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">原始数据内容</param>
	public async Task<OperateResult> WriteAsync(byte select, ushort address, byte[] value)
	{
		byte[] send = FanucHelper.BuildWriteData(select, address, value, value.Length / 2);
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(send);
		if (!read.IsSuccess)
		{
			return read;
		}
		if (read.Content[31] == 212)
		{
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult<byte[]>(read.Content[31], "Error");
	}

	/// <summary>
	/// 按照位为单位批量读取设备的原始数据，需要指定数据块地址，偏移地址及长度，主要针对70, 72, 76的数据块，注意地址的起始为1
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度，字为单位</param>
	public async Task<OperateResult<bool[]>> ReadBoolAsync(byte select, ushort address, ushort length)
	{
		int byteStartIndex = address - 1 - (address - 1) % 8 + 1;
		int byteEndIndex = (((address + length - 1) % 8 == 0) ? (address + length - 1) : ((address + length - 1) / 8 * 8 + 8));
		int byteLength = (byteEndIndex - byteStartIndex + 1) / 8;
		byte[] send = FanucHelper.BulidReadData(select, address, (ushort)(byteLength * 8));
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(send);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(read);
		}
		if (read.Content[31] == 148)
		{
			bool[] array = SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin(read.Content, 56));
			bool[] buffer = new bool[length];
			Array.Copy(array, address - byteStartIndex, buffer, 0, length);
			return OperateResult.CreateSuccessResult(buffer);
		}
		if (read.Content[31] == 212)
		{
			bool[] array2 = SoftBasic.ByteToBoolArray(SoftBasic.ArraySelectMiddle(read.Content, 44, byteLength));
			bool[] buffer2 = new bool[length];
			Array.Copy(array2, address - byteStartIndex, buffer2, 0, length);
			return OperateResult.CreateSuccessResult(buffer2);
		}
		return new OperateResult<bool[]>(read.Content[31], "Error");
	}

	/// <summary>
	/// 批量写入数组数据，返回是否写入成功，需要指定数据块地址，偏移地址，主要针对70, 72, 76的数据块，注意起始的起始为1
	/// </summary>
	/// <param name="select">数据块信息</param>
	/// <param name="address">偏移地址</param>
	/// <param name="value">原始的数据内容</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteBoolAsync(byte select, ushort address, bool[] value)
	{
		int byteStartIndex = address - 1 - (address - 1) % 8 + 1;
		int byteEndIndex = (((address + value.Length - 1) % 8 == 0) ? (address + value.Length - 1) : ((address + value.Length - 1) / 8 * 8 + 8));
		int byteLength = (byteEndIndex - byteStartIndex + 1) / 8;
		bool[] buffer = new bool[byteLength * 8];
		Array.Copy(value, 0, buffer, address - byteStartIndex, value.Length);
		byte[] send = FanucHelper.BuildWriteData(select, address, base.ByteTransform.TransByte(buffer), value.Length);
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(send);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<short[]>(read);
		}
		if (read.Content[31] == 212)
		{
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult();
	}

	[HslMqttApi(Description = "Read the details of the robot and return the resolved data type")]
	/// <summary>
	/// 读取机器人的详细信息，返回解析后的数据类型
	/// Read the details of the robot and return the resolved data type
	/// </summary>
	/// <returns>结果数据信息</returns>
	public OperateResult<FanucData> ReadFanucData()
	{
		OperateResult<byte[]> operateResult = Read("");
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FanucData>(operateResult);
		}
		return FanucData.PraseFrom(operateResult.Content, StringEncoding);
	}

	[HslMqttApi(Description = "Read the SDO information of the robot")]
	/// <summary>
	/// 读取机器人的SDO信息
	/// Read the SDO information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度</param>
	/// <returns>结果数据</returns>
	public OperateResult<bool[]> ReadSDO(ushort address, ushort length)
	{
		return ReadBool("SDO" + address, length);
	}

	[HslMqttApi(Description = "Write the SDO information of the robot")]
	/// <summary>
	/// 写入机器人的SDO信息
	/// Write the SDO information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteSDO(ushort address, bool[] value)
	{
		return Write("SDO" + address, value);
	}

	[HslMqttApi(Description = "Read the SDI information of the robot")]
	/// <summary>
	/// 读取机器人的SDI信息
	/// Read the SDI information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果内容</returns>
	public OperateResult<bool[]> ReadSDI(ushort address, ushort length)
	{
		return ReadBool("SDI" + address, length);
	}

	[HslMqttApi(Description = "Write the SDI information of the robot")]
	/// <summary>
	/// 写入机器人的SDI信息
	/// Write the SDI information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteSDI(ushort address, bool[] value)
	{
		return Write("SDI" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的RDI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadRDI(ushort address, ushort length)
	{
		return ReadBool("RDI" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的RDI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteRDI(ushort address, bool[] value)
	{
		return Write("RDI" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的UI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadUI(ushort address, ushort length)
	{
		return ReadBool("UI" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的UO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadUO(ushort address, ushort length)
	{
		return ReadBool("UO" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的UO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteUO(ushort address, bool[] value)
	{
		return Write("UO" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的SI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadSI(ushort address, ushort length)
	{
		return ReadBool("SI" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的SO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadSO(ushort address, ushort length)
	{
		return ReadBool("SO" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的SO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteSO(ushort address, bool[] value)
	{
		return Write("SO" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的GI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<ushort[]> ReadGI(ushort address, ushort length)
	{
		return ReadUInt16("GI" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的GI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteGI(ushort address, ushort[] value)
	{
		return Write("GI" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的GO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<ushort[]> ReadGO(ushort address, ushort length)
	{
		return ReadUInt16("GO" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的GO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>写入结果</returns>
	public OperateResult WriteGO(ushort address, ushort[] value)
	{
		return Write("GO" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的PMCR2信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadPMCR2(ushort address, ushort length)
	{
		return ReadBool(76, address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的PMCR2信息
	/// </summary>
	/// <param name="address">偏移信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WritePMCR2(ushort address, bool[] value)
	{
		return WriteBool(76, address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 读取机器人的RDO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public OperateResult<bool[]> ReadRDO(ushort address, ushort length)
	{
		return ReadBool("RDO" + address, length);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的RDO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteRDO(ushort address, bool[] value)
	{
		return Write("RDO" + address, value);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的Rxyzwpr信息，谨慎调用，
	/// </summary>
	/// <param name="Address">偏移地址</param>
	/// <param name="Xyzwpr">姿态信息</param>
	/// <param name="Config">设置信息</param>
	/// <param name="UserFrame">参考系</param>
	/// <param name="UserTool">工具</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteRXyzwpr(ushort Address, float[] Xyzwpr, short[] Config, short UserFrame, short UserTool)
	{
		int num = Xyzwpr.Length * 4 + Config.Length * 2 + 2;
		byte[] buffer = new byte[num];
		base.ByteTransform.TransByte(Xyzwpr).CopyTo(buffer, 0);
		base.ByteTransform.TransByte(Config).CopyTo(buffer, 36);
		OperateResult operateResult = Write(8, Address, buffer);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (0 <= UserFrame && UserFrame <= 15)
		{
			if (0 <= UserTool && UserTool <= 15)
			{
				operateResult = Write(8, (ushort)(Address + 45), base.ByteTransform.TransByte(new short[2] { UserFrame, UserTool }));
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
			else
			{
				operateResult = Write(8, (ushort)(Address + 45), base.ByteTransform.TransByte(new short[1] { UserFrame }));
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
		}
		else if (0 <= UserTool && UserTool <= 15)
		{
			operateResult = Write(8, (ushort)(Address + 46), base.ByteTransform.TransByte(new short[1] { UserTool }));
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的Rxyzwpr信息，谨慎调用，
	/// </summary>
	/// <param name="Address">偏移地址</param>
	/// <param name="Xyzwpr">姿态信息</param>
	/// <param name="Config">设置信息</param>
	/// <param name="UserFrame">参考系</param>
	/// <param name="UserTool">工具</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteRXyzwpr(ushort Address, float[] Xyzwpr, string[] Config, short UserFrame, short UserTool)
	{
		short[] array = new short[7];
		if (Config != null)
		{
			if (Config.Length != 0)
			{
				array[0] = (short)(Config[0].Equals("F", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
			}
			if (Config.Length > 1)
			{
				array[1] = (short)(Config[1].Equals("L", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
			}
			if (Config.Length > 2)
			{
				array[2] = (short)(Config[2].Equals("U", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
			}
			if (Config.Length > 3)
			{
				array[3] = (short)(Config[3].Equals("T", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
			}
			if (Config.Length > 4)
			{
				array[4] = short.Parse(Config[4]);
			}
			if (Config.Length > 5)
			{
				array[5] = short.Parse(Config[5]);
			}
			if (Config.Length > 6)
			{
				array[6] = short.Parse(Config[6]);
			}
		}
		return WriteRXyzwpr(Address, Xyzwpr, array, UserFrame, UserTool);
	}

	[HslMqttApi]
	/// <summary>
	/// 写入机器人的Joint信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="joint">关节坐标</param>
	/// <param name="UserFrame">参考系</param>
	/// <param name="UserTool">工具</param>
	/// <returns>是否写入成功</returns>
	public OperateResult WriteRJoint(ushort address, float[] joint, short UserFrame, short UserTool)
	{
		OperateResult operateResult = Write(8, (ushort)(address + 26), base.ByteTransform.TransByte(joint));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		if (0 <= UserFrame && UserFrame <= 15)
		{
			if (0 <= UserTool && UserTool <= 15)
			{
				operateResult = Write(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[3] { 0, UserFrame, UserTool }));
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
			else
			{
				operateResult = Write(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[2] { 0, UserFrame }));
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
		}
		else
		{
			operateResult = Write(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[1]));
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			if (0 <= UserTool && UserTool <= 15)
			{
				operateResult = Write(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[2] { 0, UserTool }));
				if (!operateResult.IsSuccess)
				{
					return operateResult;
				}
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 读取机器人的详细信息，返回解析后的数据类型
	/// Read the details of the robot and return the resolved data type
	/// </summary>
	/// <returns>结果数据信息</returns>
	public async Task<OperateResult<FanucData>> ReadFanucDataAsync()
	{
		OperateResult<byte[]> read = await ReadAsync("");
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FanucData>(read);
		}
		return FanucData.PraseFrom(read.Content, StringEncoding);
	}

	/// <summary>
	/// 读取机器人的SDO信息
	/// Read the SDO information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取的长度</param>
	/// <returns>结果数据</returns>
	public async Task<OperateResult<bool[]>> ReadSDOAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("SDO" + address, length);
	}

	/// <summary>
	/// 写入机器人的SDO信息
	/// Write the SDO information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteSDOAsync(ushort address, bool[] value)
	{
		return await WriteAsync("SDO" + address, value);
	}

	/// <summary>
	/// 读取机器人的SDI信息
	/// Read the SDI information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果内容</returns>
	public async Task<OperateResult<bool[]>> ReadSDIAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("SDI" + address, length);
	}

	/// <summary>
	/// 写入机器人的SDI信息
	/// Write the SDI information of the robot
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteSDIAsync(ushort address, bool[] value)
	{
		return await WriteAsync("SDI" + address, value);
	}

	/// <summary>
	/// 读取机器人的RDI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadRDIAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("RDI" + address, length);
	}

	/// <summary>
	/// 写入机器人的RDI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteRDIAsync(ushort address, bool[] value)
	{
		return await WriteAsync("RDI" + address, value);
	}

	/// <summary>
	/// 读取机器人的UI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadUIAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("UI" + address, length);
	}

	/// <summary>
	/// 读取机器人的UO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadUOAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("UO" + address, length);
	}

	/// <summary>
	/// 写入机器人的UO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteUOAsync(ushort address, bool[] value)
	{
		return await WriteAsync("UO" + address, value);
	}

	/// <summary>
	/// 读取机器人的SI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadSIAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("SI" + address, length);
	}

	/// <summary>
	/// 读取机器人的SO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadSOAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("SO" + address, length);
	}

	/// <summary>
	/// 写入机器人的SO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteSOAsync(ushort address, bool[] value)
	{
		return await WriteAsync("SO" + address, value);
	}

	/// <summary>
	/// 读取机器人的GI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">数据长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<ushort[]>> ReadGIAsync(ushort address, ushort length)
	{
		return await ReadUInt16Async("GI" + address, length);
	}

	/// <summary>
	/// 写入机器人的GI信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteGIAsync(ushort address, ushort[] value)
	{
		return await WriteAsync("GI" + address, value);
	}

	/// <summary>
	/// 读取机器人的GO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<ushort[]>> ReadGOAsync(ushort address, ushort length)
	{
		return await ReadUInt16Async("GO" + address, length);
	}

	/// <summary>
	/// 写入机器人的GO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>写入结果</returns>
	public async Task<OperateResult> WriteGOAsync(ushort address, ushort[] value)
	{
		return await WriteAsync("GO" + address, value);
	}

	/// <summary>
	/// 读取机器人的PMCR2信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadPMCR2Async(ushort address, ushort length)
	{
		return await ReadBoolAsync(76, address, length);
	}

	/// <summary>
	/// 写入机器人的PMCR2信息
	/// </summary>
	/// <param name="address">偏移信息</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WritePMCR2Async(ushort address, bool[] value)
	{
		return await WriteBoolAsync(76, address, value);
	}

	/// <summary>
	/// 读取机器人的RDO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="length">读取长度</param>
	/// <returns>结果信息</returns>
	public async Task<OperateResult<bool[]>> ReadRDOAsync(ushort address, ushort length)
	{
		return await ReadBoolAsync("RDO" + address, length);
	}

	/// <summary>
	/// 写入机器人的RDO信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="value">数据值</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteRDOAsync(ushort address, bool[] value)
	{
		return await WriteAsync("RDO" + address, value);
	}

	/// <summary>
	/// 写入机器人的Rxyzwpr信息，谨慎调用，
	/// </summary>
	/// <param name="Address">偏移地址</param>
	/// <param name="Xyzwpr">姿态信息</param>
	/// <param name="Config">设置信息</param>
	/// <param name="UserFrame">参考系</param>
	/// <param name="UserTool">工具</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteRXyzwprAsync(ushort Address, float[] Xyzwpr, short[] Config, short UserFrame, short UserTool)
	{
		int num = Xyzwpr.Length * 4 + Config.Length * 2 + 2;
		byte[] robotBuffer = new byte[num];
		base.ByteTransform.TransByte(Xyzwpr).CopyTo(robotBuffer, 0);
		base.ByteTransform.TransByte(Config).CopyTo(robotBuffer, 36);
		OperateResult write = await WriteAsync(8, Address, robotBuffer);
		if (!write.IsSuccess)
		{
			return write;
		}
		if (0 <= UserFrame && UserFrame <= 15)
		{
			if (0 <= UserTool && UserTool <= 15)
			{
				write = await WriteAsync(8, (ushort)(Address + 45), base.ByteTransform.TransByte(new short[2] { UserFrame, UserTool }));
				if (!write.IsSuccess)
				{
					return write;
				}
			}
			else
			{
				write = await WriteAsync(8, (ushort)(Address + 45), base.ByteTransform.TransByte(new short[1] { UserFrame }));
				if (!write.IsSuccess)
				{
					return write;
				}
			}
		}
		else if (0 <= UserTool && UserTool <= 15)
		{
			write = await WriteAsync(8, (ushort)(Address + 46), base.ByteTransform.TransByte(new short[1] { UserTool }));
			if (!write.IsSuccess)
			{
				return write;
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 写入机器人的Joint信息
	/// </summary>
	/// <param name="address">偏移地址</param>
	/// <param name="joint">关节坐标</param>
	/// <param name="UserFrame">参考系</param>
	/// <param name="UserTool">工具</param>
	/// <returns>是否写入成功</returns>
	public async Task<OperateResult> WriteRJointAsync(ushort address, float[] joint, short UserFrame, short UserTool)
	{
		OperateResult write = await WriteAsync(8, (ushort)(address + 26), base.ByteTransform.TransByte(joint));
		if (!write.IsSuccess)
		{
			return write;
		}
		if (0 <= UserFrame && UserFrame <= 15)
		{
			if (0 <= UserTool && UserTool <= 15)
			{
				write = await WriteAsync(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[3] { 0, UserFrame, UserTool }));
				if (!write.IsSuccess)
				{
					return write;
				}
			}
			else
			{
				write = await WriteAsync(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[2] { 0, UserFrame }));
				if (!write.IsSuccess)
				{
					return write;
				}
			}
		}
		else
		{
			write = await WriteAsync(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[1]));
			if (!write.IsSuccess)
			{
				return write;
			}
			if (0 <= UserTool && UserTool <= 15)
			{
				write = await WriteAsync(8, (ushort)(address + 44), base.ByteTransform.TransByte(new short[2] { 0, UserTool }));
				if (!write.IsSuccess)
				{
					return write;
				}
			}
		}
		return OperateResult.CreateSuccessResult();
	}

	public override string ToString()
	{
		return $"FanucInterfaceNet[{IpAddress}:{Port}]";
	}
}

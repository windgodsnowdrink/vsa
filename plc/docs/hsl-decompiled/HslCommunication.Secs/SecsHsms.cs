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
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Secs.Helper;
using HslCommunication.Secs.Message;
using HslCommunication.Secs.Types;

namespace HslCommunication.Secs;

/// <summary>
/// HSMS的协议实现，SECS基于TCP的版本，实现了secs1和secs2规约的封装，可以收发任意的功能码数据，业务逻辑部分需要二次开发
/// </summary>
public class SecsHsms : NetworkDoubleBase, ISecs
{
	public delegate void OnSecsMessageReceivedDelegate(object sender, SecsMessage secsMessage);

	private Encoding stringEncoding = Encoding.Default;

	private SoftIncrementCount incrementCount;

	private List<uint> identityQAs = new List<uint>();

	/// <summary>
	/// 获取或设置当前的DeivceID信息
	/// </summary>
	public ushort DeviceID { get; set; }

	/// <summary>
	/// 获取或设置当前的GEM信息，可以用来方便的调用一些常用的功能接口，或是自己实现自定义的接口方法
	/// </summary>
	public Gem Gem { get; set; }

	/// <summary>
	/// 是否使用S0F0来初始化当前的设备对象信息
	/// </summary>
	public bool InitializationS0F0 { get; set; } = false;

	/// <summary>
	/// 获取或设置用于字符串解析的编码信息
	/// </summary>
	public Encoding StringEncoding
	{
		get
		{
			return stringEncoding;
		}
		set
		{
			stringEncoding = value;
		}
	}

	/// <summary>
	/// 获取或设置是否自动处理S1F1功能码的返回，默认为True:自动处理，实际还包含 S1F13, S2F17 的自动处理，如果需要手动处理这些功能码，请设置为 False
	/// </summary>
	/// <remarks>
	/// 因为考虑代码的兼容性升级，所以遗留了本属性手动设置
	/// </remarks>
	public bool AutoBackS1F1 { get; set; } = true;

	/// <summary>
	/// 当接收到非应答消息的时候触发的事件
	/// </summary>
	public event OnSecsMessageReceivedDelegate OnSecsMessageReceived;

	/// <summary>
	/// 实例化一个默认的对象
	/// instantiate a default object
	/// </summary>
	public SecsHsms()
	{
		incrementCount = new SoftIncrementCount(4294967295L, 1L);
		base.ByteTransform = new ReverseBytesTransform();
		base.UseServerActivePush = true;
		Gem = new Gem(this);
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化一个默认的对象
	/// Specify the IP address and port number to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public SecsHsms(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SecsHsmsMessage();
	}

	protected override OperateResult InitializationOnConnect(Socket socket)
	{
		if (InitializationS0F0)
		{
			Send(socket, Secs1.BuildHSMSMessage(ushort.MaxValue, 0, 0, 1, (uint)incrementCount.GetCurrentValue(), null, wBit: false));
		}
		return base.InitializationOnConnect(socket);
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync(Socket socket)
	{
		if (InitializationS0F0)
		{
			await SendAsync(socket, Secs1.BuildHSMSMessage(ushort.MaxValue, 0, 0, 1, (uint)incrementCount.GetCurrentValue(), null, wBit: false));
		}
		return await base.InitializationOnConnectAsync(socket);
	}

	protected override bool DecideWhetherQAMessage(Socket socket, OperateResult<byte[]> receive)
	{
		if (!receive.IsSuccess)
		{
			return false;
		}
		byte[] content = receive.Content;
		SecsMessage secsMessage = null;
		try
		{
			secsMessage = new SecsMessage(content, 4);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), "DecideWhetherQAMessage.SecsMessage.cor", ex);
			return false;
		}
		secsMessage.StringEncoding = stringEncoding;
		if (secsMessage.StreamNo == 0 && secsMessage.FunctionNo == 0 && secsMessage.BlockNo % 2 == 1)
		{
			Send(socket, Secs1.BuildHSMSMessage(ushort.MaxValue, 0, 0, (ushort)(secsMessage.BlockNo + 1), secsMessage.MessageID, null, wBit: false));
			return false;
		}
		if (secsMessage.FunctionNo % 2 == 0 && secsMessage.FunctionNo != 0)
		{
			bool success = false;
			lock (identityQAs)
			{
				success = identityQAs.Remove(secsMessage.MessageID);
			}
			if (success)
			{
				return success;
			}
		}
		if (AutoBackS1F1)
		{
			if (secsMessage.StreamNo == 1 && secsMessage.FunctionNo == 13)
			{
				SendByCommand(secsMessage, new SecsValue(new object[2]
				{
					new byte[1],
					SecsValue.EmptyListValue()
				}), back: false);
				return false;
			}
			if (secsMessage.StreamNo == 2 && secsMessage.FunctionNo == 17)
			{
				SendByCommand(secsMessage, new SecsValue(DateTime.Now.ToString("yyyyMMddHHmmssff")), back: false);
				return false;
			}
			if (secsMessage.StreamNo == 1 && secsMessage.FunctionNo == 1)
			{
				SendByCommand(secsMessage, SecsValue.EmptyListValue(), back: false);
				return false;
			}
		}
		OnSecsMessageReceived?.Invoke(this, secsMessage);
		return false;
	}

	/// <summary>
	/// 将数据发送到设备方去，只是单纯的发送数据过去，并不等待设备的数据返回，返回是否发送成功，以及当前发送时候的 SystemBytes 的值。
	/// Sending data to the device side simply sends the data to the past, and does not wait for the data from the device to return, and returns whether the transmission is successful.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">原始的字节数据</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>是否发送成功的结果对象</returns>
	public OperateResult<uint> SendByCommand(byte stream, byte function, byte[] data, bool back)
	{
		uint num = (uint)incrementCount.GetCurrentValue();
		byte[] send = Secs1.BuildHSMSMessage(DeviceID, stream, function, 0, num, data, back);
		return ReadFromCoreServer(send, hasResponseData: false).Convert(num);
	}

	/// <summary>
	/// 将数据发送到设备方去，只是单纯的发送数据过去，并不等待设备的数据返回，返回是否发送成功，以及当前发送时候的 SystemBytes 的值。
	/// Sending data to the device side simply sends the data to the past, and does not wait for the data from the device to return, and returns whether the transmission is successful.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">Secs格式的对象信息</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>是否发送成功的结果对象</returns>
	public OperateResult<uint> SendByCommand(byte stream, byte function, SecsValue data, bool back)
	{
		return SendByCommand(stream, function, data.ToSourceBytes(stringEncoding), back);
	}

	/// <summary>
	/// 发送Secs数据到服务器上，根据收到的消息来自动确定功能码，消息号信息
	/// </summary>
	/// <param name="secsMessage">收到的Secs消息对象</param>
	/// <param name="data">Secs数据对象</param>
	/// <param name="back">是否返回</param>
	/// <returns>是否发送成功</returns>
	public OperateResult SendByCommand(SecsMessage secsMessage, SecsValue data, bool back)
	{
		byte[] send = Secs1.BuildHSMSMessage(DeviceID, secsMessage.StreamNo, (byte)(secsMessage.FunctionNo + 1), 0, secsMessage.MessageID, data.ToSourceBytes(stringEncoding), back);
		return ReadFromCoreServer(send, hasResponseData: false);
	}

	/// <summary>
	/// 根据指定的功能码将数据报文发送给设备，并且等待从SECS设备返回Secs消息，本访问机制是问答模式的。
	/// Send the data message to the device according to the specified function code, and wait for the Secs message to be returned from the SECS device. This access mechanism is in question-and-answer mode.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">原始的字节数据</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>返回SECS消息结果对象</returns>
	public OperateResult<SecsMessage> ReadSecsMessage(byte stream, byte function, byte[] data, bool back)
	{
		uint num = (uint)incrementCount.GetCurrentValue();
		lock (identityQAs)
		{
			identityQAs.Add(num);
		}
		OperateResult<byte[]> operateResult = ReadFromCoreServer(Secs1.BuildHSMSMessage(DeviceID, stream, function, 0, num, data, back));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<SecsMessage>(operateResult);
		}
		return OperateResult.CreateSuccessResult(new SecsMessage(operateResult.Content, 4)
		{
			StringEncoding = stringEncoding
		});
	}

	/// <summary>
	/// 根据指定的功能码将数据报文发送给设备，并且等待从SECS设备返回Secs消息，本访问机制是问答模式的。
	/// Send the data message to the device according to the specified function code, and wait for the Secs message to be returned from the SECS device. This access mechanism is in question-and-answer mode.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">Secs格式的对象信息</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>返回SECS消息结果对象</returns>
	public OperateResult<SecsMessage> ReadSecsMessage(byte stream, byte function, SecsValue data, bool back)
	{
		return ReadSecsMessage(stream, function, data.ToSourceBytes(stringEncoding), back);
	}

	/// <summary>
	/// 发送控制命令，需要指定  以及  参数信息，返回是否发送成功
	/// </summary>
	/// <param name="pType">控制码一</param>
	/// <param name="sType">控制码二</param>
	/// <returns>是否发送成功</returns>
	public OperateResult SendControlMessage(byte pType, byte sType)
	{
		byte[] send = Secs1.BuildHSMSMessage(ushort.MaxValue, 0, 0, (ushort)(pType * 256 + sType), (uint)incrementCount.GetCurrentValue(), null, wBit: true);
		return ReadFromCoreServer(send, hasResponseData: false);
	}

	/// <summary>
	/// 将数据发送到设备方去，只是单纯的发送数据过去，并不等待设备的数据返回，返回是否发送成功，以及当前发送时候的 SystemBytes 的值。
	/// Sending data to the device side simply sends the data to the past, and does not wait for the data from the device to return, and returns whether the transmission is successful.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">原始的字节数据</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>是否发送成功的结果对象</returns>
	public async Task<OperateResult> SendByCommandAsync(byte stream, byte function, byte[] data, bool back)
	{
		byte[] command = Secs1.BuildHSMSMessage(DeviceID, stream, function, 0, (uint)incrementCount.GetCurrentValue(), data, back);
		return await ReadFromCoreServerAsync(command, hasResponseData: false);
	}

	/// <summary>
	/// 将数据发送到设备方去，只是单纯的发送数据过去，并不等待设备的数据返回，返回是否发送成功，以及当前发送时候的 SystemBytes 的值。
	/// Sending data to the device side simply sends the data to the past, and does not wait for the data from the device to return, and returns whether the transmission is successful.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">Secs格式的对象信息</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>是否发送成功的结果对象</returns>
	public async Task<OperateResult> SendByCommandAsync(byte stream, byte function, SecsValue data, bool back)
	{
		return await SendByCommandAsync(stream, function, data.ToSourceBytes(stringEncoding), back);
	}

	/// <summary>
	/// 根据指定的功能码将数据报文发送给设备，并且等待从SECS设备返回Secs消息，本访问机制是问答模式的。
	/// Send the data message to the device according to the specified function code, and wait for the Secs message to be returned from the SECS device. This access mechanism is in question-and-answer mode.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">原始的字节数据</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>返回SECS消息结果对象</returns>
	public async Task<OperateResult<SecsMessage>> ReadSecsMessageAsync(byte stream, byte function, byte[] data, bool back)
	{
		uint identityQA = (uint)incrementCount.GetCurrentValue();
		lock (identityQAs)
		{
			identityQAs.Add(identityQA);
		}
		OperateResult<byte[]> read = await ReadFromCoreServerAsync(Secs1.BuildHSMSMessage(DeviceID, stream, function, 0, identityQA, data, back));
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<SecsMessage>(read);
		}
		return OperateResult.CreateSuccessResult(new SecsMessage(read.Content, 4)
		{
			StringEncoding = stringEncoding
		});
	}

	/// <summary>
	/// 根据指定的功能码将数据报文发送给设备，并且等待从SECS设备返回Secs消息，本访问机制是问答模式的。
	/// Send the data message to the device according to the specified function code, and wait for the Secs message to be returned from the SECS device. This access mechanism is in question-and-answer mode.
	/// </summary>
	/// <param name="stream">功能码1</param>
	/// <param name="function">功能码2</param>
	/// <param name="data">Secs格式的对象信息</param>
	/// <param name="back">是否必须返回，此标记仅仅是secs报文的是否返回标记，不表示问答模式</param>
	/// <returns>返回SECS消息结果对象</returns>
	public async Task<OperateResult<SecsMessage>> ReadSecsMessageAsync(byte stream, byte function, SecsValue data, bool back)
	{
		return await ReadSecsMessageAsync(stream, function, data.ToSourceBytes(stringEncoding), back);
	}

	public override string ToString()
	{
		return $"SecsHsms[{IpAddress}:{Port}]";
	}
}

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
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;
using HslCommunication.LogNet;
using HslCommunication.Reflection;

namespace HslCommunication.Core.Net;

/// <summary>
/// 二进制通信类，默认直接
/// </summary>
public class BinaryCommunication
{
	/// <summary>
	/// 设置日志记录报文是否二进制，如果为False，那就使用ASCII码
	/// Set whether the log message is binary, if it is False, then use ASCII code
	/// </summary>
	/// <remarks>
	/// 默认值为 true
	/// </remarks>
	protected bool LogMsgFormatBinary = true;

	private CommunicationPipe communicationPipe;

	private string connectionId;

	private byte[] sendbyteBefore = null;

	private string sendBefore = string.Empty;

	[HslMqttApi(HttpMethod = "GET", Description = "The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.")]
	/// <summary>
	/// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定
	/// The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.
	/// </summary>
	/// <remarks>
	/// 当前的唯一连接ID信息在DTU的模式下，将发挥关键作用，标记唯一的DTU ID
	/// </remarks>
	public string ConnectionId
	{
		get
		{
			return connectionId;
		}
		set
		{
			connectionId = value;
		}
	}

	/// <summary>
	/// 获取或设置在发送通信报文前追加发送的字节信息，HEX格式，通常用于lora组网时，需要携带 00 00 00 02 四个字节的站地址功能。
	/// Obtain or set the byte information sent before sending communication packets, HEX format, usually used for LORA networking, you need to carry 00 00 00 02 four-byte station address function.
	/// </summary>
	public string SendBeforeHex
	{
		get
		{
			return sendBefore;
		}
		set
		{
			sendBefore = value;
			if (string.IsNullOrEmpty(value))
			{
				sendbyteBefore = null;
			}
			else
			{
				sendbyteBefore = value.ToHexBytes();
			}
		}
	}

	/// <summary>
	/// 获取或设置当前的管道信息，管道类型为的继承类，内置了管道，管道，管道等
	/// Get or set the current pipeline information, the pipeline type is  inheritance class,  pipeline,  pipeline,  pipeline, etc
	/// </summary>
	public virtual CommunicationPipe CommunicationPipe
	{
		get
		{
			return communicationPipe;
		}
		set
		{
			communicationPipe = value;
		}
	}

	/// <summary>
	/// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以等级进行输出
	/// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at
	/// </summary>
	/// <remarks>
	/// 只要实例化即可以记录日志，实例化的对象需要实现接口  ，本组件提供了三个日志记录类，你可以实现基于   的对象。
	/// </remarks>
	public ILogNet LogNet { get; set; }

	[HslMqttApi(HttpMethod = "GET", Description = "Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback")]
	/// <summary>
	/// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈
	/// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
	/// </summary>
	/// <remarks>
	/// 超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。
	/// </remarks>
	public int ReceiveTimeOut
	{
		get
		{
			return communicationPipe.ReceiveTimeOut;
		}
		set
		{
			communicationPipe.ReceiveTimeOut = value;
		}
	}

	[HslMqttApi(HttpMethod = "GET", Description = "Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.")]
	/// <summary>
	/// 获取或设置在正式接收对方返回数据前的时候，需要休息的时间，当设置为0的时候，不需要休息。
	/// Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.
	/// </summary>
	public int SleepTime
	{
		get
		{
			return CommunicationPipe.SleepTime;
		}
		set
		{
			CommunicationPipe.SleepTime = value;
		}
	}

	/// <summary>
	/// 获取或设置当连接断开的时候，是否需要自动重新连接，默认是true
	/// Get or set whether to automatically reconnect when the connection is disconnected. The default is true.
	/// </summary>
	public bool AutoReConnect { get; set; } = true;

	/// <summary>
	/// 默认的无参构造函数
	/// Default no-parameter constructor
	/// </summary>
	public BinaryCommunication()
	{
		connectionId = SoftBasic.GetUniqueStringByGuidAndRandom();
	}

	/// <summary>
	/// 获取一个新的消息对象的方法，需要在继承类里面进行重写
	/// The method to get a new message object needs to be overridden in the inheritance class
	/// </summary>
	/// <returns>消息类对象</returns>
	protected virtual INetMessage GetNewNetMessage()
	{
		return null;
	}

	/// <summary>
	/// 决定当前的消息是否是用于问答机制返回的消息，默认直接返回 true, 实际的情况需要根据协议进行重写方法
	/// To determine whether the current message is the message returned by the question answering mechanism,
	/// the default is true. In actual cases, the rewriting method needs to be performed according to the protocol
	/// </summary>
	/// <param name="pipe">管道信息</param>
	/// <param name="receive">接收的数据信息</param>
	/// <returns>是否是问答的数据</returns>
	protected virtual bool DecideWhetherQAMessage(CommunicationPipe pipe, OperateResult<byte[]> receive)
	{
		return true;
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在创建连接之后，需要进行一些初始化的信号握手，才能最终建立网络通道。
	/// Whether to rewrite this method is based on the actual protocol. Some protocols require some initial signal handshake to establish a network channel after the connection is created.
	/// </summary>
	/// <returns>是否初始化成功，依据具体的协议进行重写</returns>
	protected virtual OperateResult InitializationOnConnect()
	{
		if (communicationPipe.UseServerActivePush)
		{
			communicationPipe.DecideWhetherQAMessageFunction = DecideWhetherQAMessage;
			communicationPipe.StartReceiveBackground(GetNewNetMessage());
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在断开连接之前，需要发送一些报文来关闭当前的网络通道
	/// Select whether to rewrite this method according to the actual protocol. Some protocols need to send some packets to close the current network channel before disconnecting.
	/// </summary>
	/// <returns>当断开连接时额外的操作结果</returns>
	protected virtual OperateResult ExtraOnDisconnect()
	{
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 和服务器交互完成的时候调用的方法，可以根据读写结果进行一些额外的操作，具体的操作需要根据实际的需求来重写实现
	/// The method called when the interaction with the server is completed can perform some additional operations based on the read and write results.
	/// The specific operations need to be rewritten according to actual needs.
	/// </summary>
	/// <param name="read">读取结果</param>
	protected virtual void ExtraAfterReadFromCoreServer(OperateResult read)
	{
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在创建连接之后，需要进行一些初始化的信号握手，才能最终建立网络通道。
	/// Whether to rewrite this method is based on the actual protocol. Some protocols require some initial signal handshake to establish a network channel after the connection is created.
	/// </summary>
	/// <returns>是否初始化成功，依据具体的协议进行重写</returns>
	protected virtual async Task<OperateResult> InitializationOnConnectAsync()
	{
		if (communicationPipe.UseServerActivePush)
		{
			communicationPipe.DecideWhetherQAMessageFunction = DecideWhetherQAMessage;
			communicationPipe.StartReceiveBackground(GetNewNetMessage());
		}
		return await Task.FromResult(OperateResult.CreateSuccessResult()).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 根据实际的协议选择是否重写本方法，有些协议在断开连接之前，需要发送一些报文来关闭当前的网络通道
	/// Select whether to rewrite this method according to the actual protocol. Some protocols need to send some packets to close the current network channel before disconnecting.
	/// </summary>
	/// <returns>当断开连接时额外的操作结果</returns>
	protected virtual async Task<OperateResult> ExtraOnDisconnectAsync()
	{
		return await Task.FromResult(OperateResult.CreateSuccessResult()).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 对当前的命令进行打包处理，通常是携带命令头内容，标记当前的命令的长度信息，需要进行重写，否则默认不打包
	/// The current command is packaged, usually carrying the content of the command header, marking the length of the current command,
	/// and it needs to be rewritten, otherwise it is not packaged by default
	/// </summary>
	/// <param name="command">发送的数据命令内容</param>
	/// <returns>打包之后的数据结果信息</returns>
	/// <remarks>
	/// 对发送的命令打包之后，直接发送给真实的对方设备了，例如在AB-PLC里面，就重写了打包方法，将当前的会话ID参数传递给PLC设备
	/// After packaging the sent command, it is directly sent to the real counterpart device. For example, in AB-PLC,
	/// the packaging method is rewritten and the current session ID parameter is passed to the PLC device.
	/// </remarks>
	public virtual byte[] PackCommandWithHeader(byte[] command)
	{
		return command;
	}

	/// <summary>
	/// 根据对方返回的报文命令，对命令进行基本的拆包，例如各种Modbus协议拆包为统一的核心报文，还支持对报文的验证
	/// According to the message command returned by the other party, the command is basically unpacked, for example,
	/// various Modbus protocols are unpacked into a unified core message, and the verification of the message is also supported
	/// </summary>
	/// <param name="send">发送的原始报文数据</param>
	/// <param name="response">设备方反馈的原始报文内容</param>
	/// <returns>返回拆包之后的报文信息，默认不进行任何的拆包操作</returns>
	/// <remarks>
	/// 在实际解包的操作过程中，通常对状态码，错误码等消息进行判断，如果校验不通过，将携带错误消息返回
	/// During the actual unpacking operation, the status code, error code and other messages are usually judged. If the verification fails, the error message will be returned.
	/// </remarks>
	public virtual OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return OperateResult.CreateSuccessResult(response);
	}

	/// <summary>
	/// 将二进制的数据发送到管道中去，然后从管道里接收二进制的数据回来，并返回是否成功的结果对象。
	/// Send binary data to the pipeline, and then receive binary data back from the pipeline, and return whether the success of the result object
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public virtual OperateResult<byte[]> ReadFromCoreServer(byte[] send)
	{
		return ReadFromCoreServer(send, hasResponseData: true, usePackAndUnpack: true);
	}

	/// <summary>
	/// 将多个数据报文按顺序发到设备，并从设备接收返回的数据内容，然后拼接成一个Byte[]信息，需要重写方法才能返回正确的结果。
	/// Send multiple data packets to the device in sequence, and receive the returned data content from the device, and then splicing them into a Byte[] message,
	/// you need to rewrite  method to return the correct result.
	/// </summary>
	/// <param name="send">发送的报文列表信息</param>
	/// <returns>是否接收成功</returns>
	public virtual OperateResult<byte[]> ReadFromCoreServer(IEnumerable<byte[]> send)
	{
		return NetSupport.ReadFromCoreServer(send, ReadFromCoreServer);
	}

	/// <summary>
	/// 将二进制的数据发送到管道中去，然后从管道里接收二进制的数据回来，并返回是否成功的结果对象。
	/// Send binary data to the pipeline, and then receive binary data back from the pipeline, and return whether the success of the result object
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public virtual OperateResult<byte[]> ReadFromCoreServer(byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
		OperateResult operateResult2 = communicationPipe.CommunicationLock.EnterLock(communicationPipe.ReceiveTimeOut);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>($"{communicationPipe}", operateResult2);
		}
		try
		{
			if (AutoReConnect)
			{
				OperateResult<bool> operateResult3 = communicationPipe.OpenCommunication();
				if (!operateResult3.IsSuccess)
				{
					communicationPipe.CommunicationLock.LeaveLock();
					return OperateResult.CreateFailedResult<byte[]>($"{communicationPipe}", operateResult3);
				}
				if (operateResult3.Content && communicationPipe.GetType() != typeof(PipeDebugRemote))
				{
					OperateResult operateResult4 = InitializationOnConnect();
					if (!operateResult4.IsSuccess)
					{
						communicationPipe.CommunicationLock.LeaveLock();
						return OperateResult.CreateFailedResult<byte[]>($"{communicationPipe}", operateResult4);
					}
				}
			}
			operateResult = ReadFromCoreServer(communicationPipe, send, hasResponseData, usePackAndUnpack);
			ExtraAfterReadFromCoreServer(operateResult);
			communicationPipe.CommunicationLock.LeaveLock();
		}
		catch
		{
			communicationPipe.CommunicationLock.LeaveLock();
			throw;
		}
		if (!communicationPipe.IsPersistentConnection)
		{
			ExtraOnDisconnect();
			communicationPipe.CloseCommunication();
		}
		return operateResult;
	}

	/// <summary>
	/// 使用指定的管道来进行数据通信，发送原始数据到管道，然后从管道接收相关的数据返回，本方法无锁
	/// </summary>
	/// <param name="pipe">管道信息</param>
	/// <param name="send">等待发送的数据</param>
	/// <param name="hasResponseData">是否需要返回的数据</param>
	/// <param name="usePackAndUnpack">是否进行封包，拆包操作</param>
	/// <returns>是否成功的结果对象</returns>
	public virtual OperateResult<byte[]> ReadFromCoreServer(CommunicationPipe pipe, byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		if (usePackAndUnpack)
		{
			send = PackCommandWithHeader(send);
		}
		if (sendbyteBefore != null)
		{
			OperateResult operateResult = pipe.Send(sendbyteBefore);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>($"{pipe}", operateResult);
			}
			LogSendMessage(sendbyteBefore);
		}
		LogSendMessage(send);
		OperateResult<byte[]> operateResult2 = pipe.ReadFromCoreServer(GetNewNetMessage(), send, hasResponseData, LogRevcMessage);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>($"{pipe}", operateResult2);
		}
		if (!usePackAndUnpack)
		{
			return operateResult2;
		}
		OperateResult<byte[]> operateResult3 = UnpackResponseContent(send, operateResult2.Content);
		if (!operateResult3.IsSuccess && operateResult3.ErrorCode == int.MinValue)
		{
			operateResult3.ErrorCode = 10000;
		}
		return operateResult3;
	}

	/// <summary>
	/// 将二进制的数据发送到管道中去，然后从管道里接收二进制的数据回来，并返回是否成功的结果对象。
	/// Send binary data to the pipeline, and then receive binary data back from the pipeline, and return whether the success of the result object
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send)
	{
		return await ReadFromCoreServerAsync(send, hasResponseData: true, usePackAndUnpack: true).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 将多个数据报文按顺序发到设备，并从设备接收返回的数据内容，然后拼接成一个Byte[]信息，需要重写方法才能返回正确的结果。
	/// Send multiple data packets to the device in sequence, and receive the returned data content from the device, and then splicing them into a Byte[] message,
	/// you need to rewrite  method to return the correct result.
	/// </summary>
	/// <param name="send">发送的报文列表信息</param>
	/// <returns>是否接收成功</returns>
	public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(IEnumerable<byte[]> send)
	{
		return await NetSupport.ReadFromCoreServerAsync(send, ReadFromCoreServerAsync).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// 将二进制的数据发送到管道中去，然后从管道里接收二进制的数据回来，并返回是否成功的结果对象。
	/// Send binary data to the pipeline, and then receive binary data back from the pipeline, and return whether the success of the result object
	/// </summary>
	/// <param name="send">发送的完整的报文信息</param>
	/// <param name="hasResponseData">是否有等待的数据返回</param>
	/// <param name="usePackAndUnpack">是否需要对命令重新打包，在重写方法后才会有影响</param>
	/// <returns>接收的完整的报文信息</returns>
	/// <remarks>
	/// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
	/// </remarks>
	public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		new OperateResult<byte[]>();
		OperateResult enter = ((!HslHelper.UseAsyncLock) ? communicationPipe.CommunicationLock.EnterLock(communicationPipe.ReceiveTimeOut) : (await Task.Run(() => communicationPipe.CommunicationLock.EnterLock(communicationPipe.ReceiveTimeOut)).ConfigureAwait(continueOnCapturedContext: false)));
		if (!enter.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>($"{communicationPipe}", enter);
		}
		OperateResult<byte[]> read;
		try
		{
			if (AutoReConnect)
			{
				OperateResult<bool> pipe = await communicationPipe.OpenCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (!pipe.IsSuccess)
				{
					communicationPipe.CommunicationLock.LeaveLock();
					return OperateResult.CreateFailedResult<byte[]>($"{communicationPipe}", pipe);
				}
				if (pipe.Content && communicationPipe.GetType() != typeof(PipeDebugRemote))
				{
					OperateResult ini = await InitializationOnConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
					if (!ini.IsSuccess)
					{
						communicationPipe.CommunicationLock.LeaveLock();
						return OperateResult.CreateFailedResult<byte[]>($"{communicationPipe}", ini);
					}
				}
			}
			read = await ReadFromCoreServerAsync(communicationPipe, send, hasResponseData, usePackAndUnpack).ConfigureAwait(continueOnCapturedContext: false);
			ExtraAfterReadFromCoreServer(read);
			communicationPipe.CommunicationLock.LeaveLock();
		}
		catch
		{
			communicationPipe.CommunicationLock.LeaveLock();
			throw;
		}
		if (!communicationPipe.IsPersistentConnection)
		{
			await ExtraOnDisconnectAsync().ConfigureAwait(continueOnCapturedContext: false);
			await communicationPipe.CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		return read;
	}

	/// <summary>
	/// 使用指定的管道来进行数据通信，发送原始数据到管道，然后从管道接收相关的数据返回，本方法无锁
	/// </summary>
	/// <param name="pipe">管道信息</param>
	/// <param name="send">等待发送的数据</param>
	/// <param name="hasResponseData">是否需要返回的数据</param>
	/// <param name="usePackAndUnpack">是否进行封包，拆包操作</param>
	/// <returns>是否成功的结果对象</returns>
	public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(CommunicationPipe pipe, byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		if (usePackAndUnpack)
		{
			send = PackCommandWithHeader(send);
		}
		if (sendbyteBefore != null)
		{
			OperateResult sendBefore = await communicationPipe.SendAsync(sendbyteBefore).ConfigureAwait(continueOnCapturedContext: false);
			if (!sendBefore.IsSuccess)
			{
				return OperateResult.CreateFailedResult<byte[]>($"{pipe}", sendBefore);
			}
			LogSendMessage(sendbyteBefore);
		}
		LogSendMessage(send);
		OperateResult<byte[]> read = await pipe.ReadFromCoreServerAsync(GetNewNetMessage(), send, hasResponseData, LogRevcMessage).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>($"{pipe}", read);
		}
		if (!usePackAndUnpack)
		{
			return read;
		}
		OperateResult<byte[]> unpack = UnpackResponseContent(send, read.Content);
		if (!unpack.IsSuccess && unpack.ErrorCode == int.MinValue)
		{
			unpack.ErrorCode = 10000;
		}
		return unpack;
	}

	/// <summary>
	/// 获取当前的报文进行日志记录的时候，是否使用二进制的格式记录，默认返回 ，重写可以根据对象分别返回不同记录模式
	/// Whether to log the current packet in binary format, the default return is . If you want to override it,
	/// different recording modes can be returned according to
	/// </summary>
	/// <param name="session">会话对象</param>
	/// <param name="content">等待记录的字节消息内容</param>
	/// <returns>是否二进制记录报文格式</returns>
	protected virtual string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		return LogMsgFormatBinary ? content.ToHexString(' ') : SoftBasic.GetAsciiStringRender(content);
	}

	/// <summary>
	/// 使用日志记录一个发送的报文信息
	/// Logs are used to record information about a send packet
	/// </summary>
	/// <param name="content">接收的报文信息</param>
	/// <param name="session">会话对象信息</param>
	protected void LogSendMessage(byte[] content)
	{
		LogSendMessage(content, null);
	}

	/// <summary>
	/// 使用日志记录一个发送的报文信息
	/// Logs are used to record information about a send packet
	/// </summary>
	/// <param name="content">接收的报文信息</param>
	/// <param name="session">会话对象信息</param>
	protected void LogSendMessage(byte[] content, PipeSession session)
	{
		if (content != null)
		{
			string text = ((session == null) ? string.Empty : $"<{session.Communication}> ");
			LogNet?.WriteDebug(ToString(), text + StringResources.Language.Send + " : " + GetLogTextFromBinary(session, content));
		}
	}

	/// <summary>
	/// 使用日志记录一个接收的报文信息
	/// Logs are used to record information about a received packet
	/// </summary>
	/// <param name="content">接收的报文信息</param>
	/// <param name="session">会话对象信息</param>
	protected void LogRevcMessage(byte[] content)
	{
		LogRevcMessage(content, null);
	}

	/// <summary>
	/// 使用日志记录一个接收的报文信息
	/// Logs are used to record information about a received packet
	/// </summary>
	/// <param name="content">接收的报文信息</param>
	/// <param name="session">会话对象信息</param>
	protected void LogRevcMessage(byte[] content, PipeSession session)
	{
		if (content != null)
		{
			string text = ((session == null) ? string.Empty : $"<{session.Communication}> ");
			LogNet?.WriteDebug(ToString(), text + StringResources.Language.Receive + " : " + GetLogTextFromBinary(session, content));
		}
	}

	/// <summary>
	/// 将当前的通信对象设置DTU模式，允许传入现成的管道，并返回初始化结果，如果该设备重写了握手报文，就是返回握手结果
	/// Set the current communication object to DTU mode, allow the existing pipe to be passed in, and return the initialization result,
	/// if the device rewrites the handshake packet, the handshake result is returned
	/// </summary>
	/// <param name="pipe">DTU的管道信息</param>
	/// <returns>是否设置管道并初始化成功</returns>
	public OperateResult SetDtuPipe(PipeDtuNet pipe)
	{
		if (pipe != null)
		{
			if (string.IsNullOrEmpty(ConnectionId))
			{
				ConnectionId = pipe.DTU;
			}
			CommunicationPipe = pipe;
			if (!pipe.IsConnectError())
			{
				return InitializationOnConnect();
			}
			return new OperateResult("Session dtu[" + pipe.DTU + "] net error");
		}
		return new OperateResult("pipe is NULL");
	}

	/// <summary>
	/// 将当前的通信对象设置DTU模式，允许传入现成的管道，并返回初始化结果，如果该设备重写了握手报文，就是返回握手结果
	/// Set the current communication object to DTU mode, allow the existing pipe to be passed in, and return the initialization result,
	/// if the device rewrites the handshake packet, the handshake result is returned
	/// </summary>
	/// <param name="pipe">DTU的管道信息</param>
	/// <returns>是否设置管道并初始化成功</returns>
	public async Task<OperateResult> SetDtuPipeAsync(PipeDtuNet pipe)
	{
		if (pipe != null)
		{
			if (string.IsNullOrEmpty(ConnectionId))
			{
				ConnectionId = pipe.DTU;
			}
			CommunicationPipe = pipe;
			if (!pipe.IsConnectError())
			{
				return await InitializationOnConnectAsync();
			}
			return new OperateResult("Session dtu[" + pipe.DTU + "] net error");
		}
		return new OperateResult("pipe is NULL");
	}
}

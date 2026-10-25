// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;
using HslCommunication.Instrument.IEC.Helper;

namespace HslCommunication.Instrument.IEC;

/// <summary>
/// IEC104规约实现的电力协议，支持总召唤，支持回调任意的突发上传事件
/// </summary>
/// <remarks>
/// 手动绑定事件 ，当收到设备的数据时就可以触发，如果需要总召唤，调用方法
/// </remarks>
public class IEC104 : DeviceTcpNet
{
	public class TypeID
	{
		public const byte M_SP_NA_1 = 1;

		public const byte M_DP_NA_1 = 3;

		public const byte M_ST_NA_1 = 5;

		public const byte M_BO_NA_1 = 7;

		public const byte M_ME_NA_1 = 9;

		public const byte M_ME_NB_1 = 11;

		public const byte M_ME_NC_1 = 13;

		public const byte M_IT_NA_1 = 15;

		public const byte M_PS_NA_1 = 20;

		public const byte M_ME_ND_1 = 21;

		public const byte M_SP_TB_1 = 30;

		public const byte M_DP_TB_1 = 31;

		public const byte M_ST_TB_1 = 32;

		public const byte M_BO_TB_1 = 33;

		public const byte M_ME_TD_1 = 34;

		public const byte M_ME_TE_1 = 35;

		public const byte M_ME_TF_1 = 36;

		public const byte M_IT_TB_1 = 37;

		public const byte C_SC_NA_1 = 45;

		public const byte C_DC_NA_1 = 46;

		public const byte C_RC_NA_1 = 47;

		public const byte C_SE_NA_1 = 48;

		public const byte C_SE_NB_1 = 49;

		public const byte C_SE_NC_1 = 50;

		public const byte C_SE_ND_1 = 51;

		public const byte C_SE_TA_1 = 58;

		public const byte C_SE_TB_1 = 59;

		public const byte C_SE_TC_1 = 60;

		public const byte C_SE_TD_1 = 61;

		public const byte C_SE_TE_1 = 62;

		public const byte C_SE_TF_1 = 63;

		public const byte C_SE_TG_1 = 64;

		public const byte C_SE_NE_1 = 136;

		public const byte M_EI_NA_1 = 70;

		public const byte C_IC_NA_1 = 100;

		public const byte C_CI_NA_1 = 101;

		public const byte C_RD_NA_1 = 102;

		public const byte C_CS_NA_1 = 103;

		public const byte C_RS_NA_1 = 105;

		public const byte C_TS_TA_1 = 107;
	}

	private int deviceSendLastID = 0;

	private object receiveLock = new object();

	private readonly SoftIncrementCount sendIncrementCount;

	private int receiveIncrementCount = 0;

	private int station = 1;

	/// <summary>
	/// 公共单元地址，在低版本远动程序中，站地址范围一般在1~254，255表示全局地址。新远动程序则可支持1~65534为站地址，而65535为全局地址。默认值为1
	/// </summary>
	public int Station
	{
		get
		{
			return station;
		}
		set
		{
			station = value;
		}
	}

	/// <summary>
	/// 当接收到了IEC104的消息触发的事件
	/// </summary>
	public event EventHandler<IEC104MessageEventArgs> OnIEC104MessageReceived;

	/// <summary>
	/// 实例化IEC104协议的通讯对象
	/// Instantiate the communication object of the IEC104 protocol
	/// </summary>
	public IEC104()
	{
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
		sendIncrementCount = new SoftIncrementCount(32767L, 0L);
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化一个默认的对象
	/// Specify the IP address and port number to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">IEC104的Ip地址</param>
	/// <param name="port">IEC104的端口, 默认是2404端口</param>
	public IEC104(string ipAddress, int port = 2404)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new IEC104Message();
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult operateResult = CommunicationPipe.Send(IECHelper.BuildFrameUMessage(7));
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		LogSendMessage(IECHelper.BuildFrameUMessage(7));
		OperateResult operateResult2 = CommunicationPipe.ReceiveMessage(GetNewNetMessage(), null, useActivePush: false, null, base.LogRevcMessage);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		CommunicationPipe.UseServerActivePush = true;
		sendIncrementCount.ResetCurrentValue(0L);
		receiveIncrementCount = 0;
		deviceSendLastID = 0;
		return base.InitializationOnConnect();
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult send1 = await CommunicationPipe.SendAsync(IECHelper.BuildFrameUMessage(7)).ConfigureAwait(continueOnCapturedContext: false);
		if (!send1.IsSuccess)
		{
			return send1;
		}
		LogSendMessage(IECHelper.BuildFrameUMessage(7));
		OperateResult recv1 = await CommunicationPipe.ReceiveMessageAsync(GetNewNetMessage(), null, useActivePush: false, null, base.LogRevcMessage).ConfigureAwait(continueOnCapturedContext: false);
		if (!recv1.IsSuccess)
		{
			return recv1;
		}
		CommunicationPipe.UseServerActivePush = true;
		sendIncrementCount.ResetCurrentValue(0L);
		receiveIncrementCount = 0;
		deviceSendLastID = 0;
		return await base.InitializationOnConnectAsync();
	}

	public override OperateResult Write(string address, bool value)
	{
		byte[] buffer = new byte[10]
		{
			45,
			1,
			3,
			0,
			BitConverter.GetBytes(station)[0],
			BitConverter.GetBytes(station)[1],
			0,
			0,
			0,
			0
		};
		BitConverter.GetBytes(ushort.Parse(address)).CopyTo(buffer, 6);
		if (value)
		{
			buffer[9] = 1;
		}
		return SendFrameIMessage(buffer);
	}

	/// <summary>
	/// 指定类型标识，传送原因，数据地址，数据值信息来写入到IEC的仪表中去
	/// </summary>
	/// <param name="type">类型标识</param>
	/// <param name="reason">传送原因</param>
	/// <param name="address">信息对象地址</param>
	/// <param name="value">值信息</param>
	/// <returns>是否发送到仪表成功</returns>
	public OperateResult WriteIec(byte type, ushort reason, ushort address, bool value)
	{
		return WriteIec(type, reason, address, (!value) ? new byte[1] : new byte[1] { 1 });
	}

	/// <summary>
	/// 指定类型标识，传送原因，数据地址，数据值信息来写入到IEC的仪表中去
	/// </summary>
	/// <param name="type">类型标识</param>
	/// <param name="reason">传送原因</param>
	/// <param name="address">信息对象地址</param>
	/// <param name="value">值信息</param>
	/// <returns>是否发送到仪表成功</returns>
	public OperateResult WriteIec(byte type, ushort reason, ushort address, short value)
	{
		return WriteIec(type, reason, address, BitConverter.GetBytes(value));
	}

	/// <summary>
	/// 指定类型标识，传送原因，数据地址，数据值信息来写入到IEC的仪表中去
	/// </summary>
	/// <param name="type">类型标识</param>
	/// <param name="reason">传送原因</param>
	/// <param name="address">信息对象地址</param>
	/// <param name="value">值信息</param>
	/// <returns>是否发送到仪表成功</returns>
	public OperateResult WriteIec(byte type, ushort reason, ushort address, float value)
	{
		return WriteIec(type, reason, address, BitConverter.GetBytes(value));
	}

	/// <summary>
	/// 指定类型标识，传送原因，数据地址，数据值信息来写入到IEC的仪表中去
	/// </summary>
	/// <param name="type">类型标识</param>
	/// <param name="reason">传送原因</param>
	/// <param name="address">信息对象地址</param>
	/// <param name="value">值信息</param>
	/// <returns>是否发送到仪表成功</returns>
	public OperateResult WriteIec(byte type, ushort reason, ushort address, byte[] value)
	{
		return SendFrameIMessage(IECHelper.BuildWriteIec(type, reason, (ushort)station, address, value));
	}

	private int GetReceiveIncrementCount(int deviceSendID)
	{
		lock (receiveLock)
		{
			if (deviceSendID != deviceSendLastID)
			{
				deviceSendLastID = deviceSendID;
				receiveIncrementCount++;
			}
			int result = receiveIncrementCount;
			if (receiveIncrementCount > 32767)
			{
				receiveIncrementCount = 0;
			}
			return result;
		}
	}

	protected override bool DecideWhetherQAMessage(CommunicationPipe pipe, OperateResult<byte[]> receive)
	{
		if (!receive.IsSuccess)
		{
			return false;
		}
		LogRevcMessage(receive.Content);
		byte[] content = receive.Content;
		if (content.Length < 6)
		{
			return false;
		}
		if (!content[2].GetBoolByIndex(0) && !content[4].GetBoolByIndex(0))
		{
			int num = BitConverter.ToUInt16(content, 2) / 2;
			int num2 = BitConverter.ToUInt16(content, 4) / 2;
			int num3 = GetReceiveIncrementCount(num);
			byte[] array = IECHelper.BuildFrameSMessage(num3);
			LogSendMessage(array);
			pipe.Send(array);
			if (num3 != num)
			{
				base.LogNet?.WriteError(ToString(), $"send[{num}] receive[{num3}] message id check failed: ");
			}
			IEC104MessageEventArgs e = null;
			try
			{
				e = new IEC104MessageEventArgs(content.RemoveBegin(6));
			}
			catch (Exception ex)
			{
				base.LogNet?.WriteException(ToString(), "IEC104MessageEventArg..ctor failed: " + content.ToHexString(' '), ex);
			}
			if (e != null)
			{
				OnIEC104MessageReceived?.Invoke(this, e);
			}
		}
		else if (((content[2] & 3) != 1 || content[4].GetBoolByIndex(0)) && (content[2] & 3) == 3 && !content[4].GetBoolByIndex(0) && content[2] == 67)
		{
			byte[] array2 = IECHelper.BuildFrameUMessage(131);
			LogSendMessage(array2);
			pipe.Send(array2);
		}
		return false;
	}

	/// <summary>
	/// 以I消息的格式发送传入的原始字节数据，传入的消息为asdu信息
	/// </summary>
	/// <param name="asdu">ASDU报文信息</param>
	/// <returns>是否发送成功</returns>
	public OperateResult SendFrameIMessage(byte[] asdu)
	{
		if (CommunicationPipe.IsConnectError())
		{
			OperateResult operateResult = ConnectServer();
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		int sendID = (int)sendIncrementCount.GetCurrentValue();
		int receiveID = receiveIncrementCount;
		byte[] array = IECHelper.BuildFrameIMessage(sendID, receiveID, asdu[0], asdu[1], base.ByteTransform.TransUInt16(asdu, 2), base.ByteTransform.TransUInt16(asdu, 4), asdu.RemoveBegin(6));
		LogSendMessage(array);
		return CommunicationPipe.Send(array);
	}

	/// <summary>
	/// 向设备发送U帧消息的报文信息，传入功能码，STARTDT: 0x07, STOPDT: 0x13; TESTFR: 0x43
	/// </summary>
	/// <param name="controlField">功能码，STARTDT: 0x07, STOPDT: 0x13; TESTFR: 0x43</param>
	/// <returns>是否发送成功</returns>
	public OperateResult SendFrameUMessage(byte controlField)
	{
		if (CommunicationPipe.IsConnectError())
		{
			OperateResult operateResult = ConnectServer();
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
		}
		byte[] array = IECHelper.BuildFrameUMessage(controlField);
		LogSendMessage(array);
		return CommunicationPipe.Send(array);
	}

	/// <summary>
	/// 总召唤，支持设置总召唤代码，0x01: 定时主动上送,  0x14: 相应总召唤上送,  0x03: 越限上送
	/// </summary>
	/// <param name="code">总召唤的代码，0x01: 定时主动上送,  0x14: 相应总召唤上送,  0x03: 越限上送</param>
	/// <returns>是否召唤成功</returns>
	public OperateResult TotalSubscriptions(byte code = 20)
	{
		return TotalSubscriptions(code, 6);
	}

	/// <summary>
	/// 总召唤，支持设置总召唤代码，0x01: 定时主动上送,  0x14: 相应总召唤上送,  0x03: 越限上送
	/// </summary>
	/// <param name="station">公共单元地址</param>
	/// <param name="code">总召唤的代码，0x01: 定时主动上送,  0x14: 相应总召唤上送,  0x03: 越限上送</param>
	/// <param name="reason">传送原因，通常可选 06:激活，07:激活确认</param>
	/// <returns>是否总召唤成功</returns>
	public OperateResult TotalSubscriptions(byte code, byte reason)
	{
		return TotalSubscriptions((ushort)Station, code, reason);
	}

	/// <summary>
	/// 总召唤，支持设置总召唤代码，0x01: 定时主动上送,  0x14: 相应总召唤上送,  0x03: 越限上送
	/// </summary>
	/// <param name="station">公共单元地址</param>
	/// <param name="code">总召唤的代码，0x01: 定时主动上送,  0x14: 相应总召唤上送,  0x03: 越限上送</param>
	/// <param name="reason">传送原因，通常可选 06:激活，07:激活确认</param>
	/// <returns>是否总召唤成功</returns>
	public OperateResult TotalSubscriptions(ushort station, byte code, byte reason)
	{
		byte[] buffer = new byte[10] { 100, 1, 0, 0, 1, 0, 0, 0, 0, 0 };
		buffer[2] = reason;
		buffer[9] = code;
		byte[] array = buffer;
		array[4] = BitConverter.GetBytes(station)[0];
		array[5] = BitConverter.GetBytes(station)[1];
		return SendFrameIMessage(array);
	}

	public override string ToString()
	{
		return $"IEC104[{IpAddress}:{Port}]";
	}
}

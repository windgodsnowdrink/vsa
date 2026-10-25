// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Device;

/// <summary>
/// 虚拟服务器（）的故障注入器，覆盖 TCP、UDP 与串口三种会话。开启之后，服务器在把响应发回客户端之前会按照设置
/// 注入延迟、丢包（不响应）、错帧（篡改数据，仅串口会话生效）、断链（仅网络会话生效），用来验证上位机的超时、重试、重连以及数据校验等异常处理逻辑。
/// 注入延迟、丢包（不响应）、错帧（篡改数据）、断链，用来验证上位机的超时、重试、重连以及数据校验等异常处理逻辑。
/// 默认是关闭状态，关闭时不会产生任何额外的开销，服务器原有的通信行为完全不受影响。
/// The fault injector of the virtual server (). When it is enabled, the server injects
/// delay, packet loss (no response), wrong frame (data tampering) and disconnection before sending the response
/// back to the client, which is used to verify the exception handling logic of the host computer, such as timeout,
/// retry, reconnection and data verification. It is disabled by default, and the original communication behavior
/// of the server is not affected at all when it is disabled.
/// </summary>
public sealed class ServerFaultInjection
{
	private int totalResponses;

	private int delayCount;

	private int dropCount;

	private int corruptCount;

	private int disconnectCount;

	private static readonly ServerFaultType[] AllFaultTypes = new ServerFaultType[4]
	{
		ServerFaultType.Delay,
		ServerFaultType.DropResponse,
		ServerFaultType.Corrupt,
		ServerFaultType.Disconnect
	};

	private readonly Random random = new Random();

	/// <summary>
	/// 是否开启故障注入，默认为 false。关闭时  立即返回，没有任何额外开销
	/// </summary>
	public bool Enabled { get; set; }

	/// <summary>
	/// 需要注入的故障类型，可以按位组合，默认为
	/// </summary>
	public ServerFaultType Types { get; set; }

	/// <summary>
	/// 响应延迟的毫秒数，用于 ，默认为 500
	/// </summary>
	public int DelayMs { get; set; }

	/// <summary>
	/// 每一次响应触发注入的概率，取值 1 ~ 100，100 表示每一次响应都注入，默认为 100。
	/// 小于等于 0 时相当于不注入，可以在注入过程中实时修改
	/// </summary>
	public int Probability { get; set; }

	/// <summary>
	/// 篡改的字节数量，用于 ，默认为 4
	/// </summary>
	public int CorruptBytes { get; set; }

	public int TotalResponses => totalResponses;

	public int DelayCount => delayCount;

	public int DropCount => dropCount;

	public int CorruptCount => corruptCount;

	public int DisconnectCount => disconnectCount;

	/// <summary>
	/// 发生一次故障注入的时候触发，可以用于界面记录注入日志
	/// </summary>
	public event EventHandler<ServerFaultEventArgs> OnFaultInjected;

	/// <summary>
	/// 实例化一个默认的故障注入器，默认是关闭状态
	/// </summary>
	public ServerFaultInjection()
	{
		Enabled = false;
		Types = ServerFaultType.None;
		DelayMs = 500;
		Probability = 100;
		CorruptBytes = 4;
	}

	/// <summary>
	/// 清零所有的注入统计信息
	/// </summary>
	public void ResetStatistics()
	{
		Interlocked.Exchange(ref totalResponses, 0);
		Interlocked.Exchange(ref delayCount, 0);
		Interlocked.Exchange(ref dropCount, 0);
		Interlocked.Exchange(ref corruptCount, 0);
		Interlocked.Exchange(ref disconnectCount, 0);
	}

	/// <summary>
	/// 在响应发送之前调用，决定本次响应需要执行的故障动作。延迟与数据篡改在本方法内部完成，
	/// 丢包与断链通过返回值告知调用方
	/// Called before the response is sent to determine the fault action for this response. The delay and the
	/// data tampering are completed inside this method, and the packet loss and disconnection are notified to
	/// the caller through the return value
	/// </summary>
	/// <param name="session">当前的会话信息</param>
	/// <param name="content">准备发送的响应内容，注入错帧时会被替换为篡改之后的新数组（原数组不会被修改）</param>
	/// <returns>调用方需要执行的动作</returns>
	public ServerFaultAction Process(PipeSession session, ref byte[] content)
	{
		if (!Enabled)
		{
			return ServerFaultAction.Send;
		}
		Interlocked.Increment(ref totalResponses);
		ServerFaultType serverFaultType = DecideFault();
		if (serverFaultType == ServerFaultType.None)
		{
			return ServerFaultAction.Send;
		}
		if ((serverFaultType & ServerFaultType.Delay) == ServerFaultType.Delay)
		{
			int delayMs = DelayMs;
			if (delayMs > 0)
			{
				Thread.Sleep(delayMs);
			}
			Interlocked.Increment(ref delayCount);
			RaiseFaultInjected(session, ServerFaultType.Delay, "delay response " + delayMs + " ms");
		}
		if ((serverFaultType & ServerFaultType.DropResponse) == ServerFaultType.DropResponse)
		{
			Interlocked.Increment(ref dropCount);
			RaiseFaultInjected(session, ServerFaultType.DropResponse, "drop response " + ((content != null) ? content.Length : 0) + " bytes");
			return ServerFaultAction.DropResponse;
		}
		if ((serverFaultType & ServerFaultType.Corrupt) == ServerFaultType.Corrupt && IsSerialSession(session))
		{
			content = Corrupt(content);
			Interlocked.Increment(ref corruptCount);
			RaiseFaultInjected(session, ServerFaultType.Corrupt, "corrupt " + CorruptBytes + " byte(s) of the response");
		}
		if ((serverFaultType & ServerFaultType.Disconnect) == ServerFaultType.Disconnect && !IsSerialSession(session))
		{
			Interlocked.Increment(ref disconnectCount);
			RaiseFaultInjected(session, ServerFaultType.Disconnect, "disconnect after the response");
			return ServerFaultAction.Disconnect;
		}
		return ServerFaultAction.Send;
	}

	/// <summary>
	/// 决定本次响应注入哪一种故障：先按照概率决定本次是否注入，再从已选择的类型里随机取一种。
	/// 每一次响应都是独立判定，因此概率 50 时不会出现严格交替的情况
	/// </summary>
	/// <returns>本次需要注入的故障类型</returns>
	private ServerFaultType DecideFault()
	{
		ServerFaultType types = Types;
		if (types == ServerFaultType.None)
		{
			return ServerFaultType.None;
		}
		int probability = Probability;
		if (probability <= 0)
		{
			return ServerFaultType.None;
		}
		if (probability < 100)
		{
			lock (random)
			{
				if (random.Next(1, 101) > probability)
				{
					return ServerFaultType.None;
				}
			}
		}
		int num = 0;
		for (int i = 0; i < AllFaultTypes.Length; i++)
		{
			if ((types & AllFaultTypes[i]) == AllFaultTypes[i])
			{
				num++;
			}
		}
		if (num == 0)
		{
			return ServerFaultType.None;
		}
		int num2 = 0;
		lock (random)
		{
			num2 = random.Next(num);
		}
		for (int j = 0; j < AllFaultTypes.Length; j++)
		{
			if ((types & AllFaultTypes[j]) == AllFaultTypes[j])
			{
				if (num2 == 0)
				{
					return AllFaultTypes[j];
				}
				num2--;
			}
		}
		return ServerFaultType.None;
	}

	/// <summary>
	/// 判断当前会话是否是串口会话，错帧注入只在串口上生效
	/// </summary>
	/// <param name="session">会话信息</param>
	/// <returns>是否为串口会话</returns>
	private static bool IsSerialSession(PipeSession session)
	{
		return session != null && session.Communication is PipeSerialPort;
	}

	/// <summary>
	/// 篡改响应尾部的若干字节（异或 0xFF），用于模拟现场干扰导致的报文损坏。
	/// 带校验的协议会报校验错误，不带校验的协议会把错误的数据交给上位机，两种都是真实的故障场景
	/// </summary>
	/// <param name="content">原始的响应内容</param>
	/// <returns>篡改之后的响应内容副本</returns>
	private byte[] Corrupt(byte[] content)
	{
		if (content == null || content.Length == 0)
		{
			return content;
		}
		byte[] array = (byte[])content.Clone();
		int num = CorruptBytes;
		if (num < 1)
		{
			num = 1;
		}
		if (num > array.Length)
		{
			num = array.Length;
		}
		for (int i = 0; i < num; i++)
		{
			array[array.Length - 1 - i] ^= byte.MaxValue;
		}
		return array;
	}

	private void RaiseFaultInjected(PipeSession session, ServerFaultType faultType, string message)
	{
		EventHandler<ServerFaultEventArgs> eventHandler = OnFaultInjected;
		if (eventHandler == null)
		{
			return;
		}
		try
		{
			eventHandler(this, new ServerFaultEventArgs
			{
				FaultType = faultType,
				Session = session,
				Message = message
			});
		}
		catch
		{
		}
	}
}

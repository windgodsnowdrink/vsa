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
using System.Runtime.InteropServices;
using System.Threading;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.LogNet;
using HslCommunication.Reflection;
using Newtonsoft.Json;

namespace HslCommunication;

/// <summary>
/// 超时操作的类
/// a class use to indicate the time-out of the connection
/// </summary>
/// <remarks>
/// 本类自动启动一个静态线程来处理
/// </remarks>
public class HslTimeOut
{
	private static long hslTimeoutId;

	private static List<HslTimeOut> WaitHandleTimeOut;

	private static object listLock;

	private static Thread threadCheckTimeOut;

	private static long threadUniqueId;

	private static DateTime threadActiveTime;

	private static int activeDisableCount;

	/// <summary>
	/// 当前的超时对象处理的总个数，如果需要重置该值，方便重新计数，请调用如下代码：long current = Interlocked.Exchange( ref HslCommunication.HslTimeOut.TimeoutDealCount, 0 );
	/// The total number of timeout objects processed at present, if you need to reset the value to facilitate recounting, call the following code: long current = Interlocked.Exchange( ref HslCommunication.HslTimeOut.TimeoutDealCount, 0 );
	/// </summary>
	public static long TimeoutDealCount;

	private static bool isLinuxPlatform;

	/// <summary>
	/// 当前超时对象的唯一ID信息，没实例化一个对象，id信息就会自增1
	/// The unique ID information of the current timeout object. If an object is not instantiated, the id information will increase by 1
	/// </summary>
	public long UniqueId { get; private set; }

	/// <summary>
	/// 操作的开始时间
	/// Start time of operation
	/// </summary>
	public DateTime StartTime { get; set; }

	/// <summary>
	/// 操作是否成功，当操作完成的时候，需要设置为True，超时检测自动结束。如果一直为False，超时检测到超时，设置为True
	/// Whether the operation is successful, when the operation is completed, it needs to be set to True,
	/// and the timeout detection will automatically end. If it is always False,
	/// the timeout is detected by the timeout, set  to True
	/// </summary>
	public bool IsSuccessful { get; set; }

	/// <summary>
	/// 延时的时间，单位毫秒
	/// Delay time, in milliseconds
	/// </summary>
	public int DelayTime { get; set; }

	[JsonIgnore]
	/// <summary>
	/// 连接超时用的Socket，本超时对象主要针对套接字的连接，接收数据的超时检测，也可以设置为空，用作其他用途的超时检测。
	/// Socket used for connection timeout. This timeout object is mainly for socket connection and timeout detection of received data.
	/// It can also be set to empty for other purposes.
	/// </summary>
	public Socket WorkSocket { get; set; }

	/// <summary>
	/// 是否发生了超时的操作，当调用方因为异常结束的时候，需要对进行判断，是否因为发送了超时导致的异常
	/// Whether a timeout operation has occurred, when the caller ends abnormally,
	/// it needs to judge , whether it is an exception caused by a timeout sent
	/// </summary>
	public bool IsTimeout { get; set; }

	/// <summary>
	/// 获取或设置当前的超时线程的状态日志信息，只要实例化，即可记录当前的超时日志运行信息
	/// Obtain or set the status log information of the current timeout thread, and record the current timeout log operation information as long as it is instantiated
	/// </summary>
	public static ILogNet TimeoutLogNet { get; set; }

	[HslMqttApi(Description = "Get the number of current check timeout objects", HttpMethod = "GET")]
	public static int TimeOutCheckCount => WaitHandleTimeOut.Count;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public HslTimeOut()
	{
		UniqueId = Interlocked.Increment(ref hslTimeoutId);
		StartTime = DateTime.Now;
		IsSuccessful = false;
		IsTimeout = false;
	}

	/// <summary>
	/// 获取到目前为止所花费的时间
	/// Get the time spent so far
	/// </summary>
	/// <returns>时间信息</returns>
	public TimeSpan GetConsumeTime()
	{
		return DateTime.Now - StartTime;
	}

	public override string ToString()
	{
		return $"HslTimeOut[{DelayTime}]";
	}

	/// <summary>
	/// 新增一个超时检测的对象，当操作完成的时候，需要自行标记对象的为True
	/// Add a new object for timeout detection. When the operation is completed,
	/// you need to mark the  of the  object as True
	/// </summary>
	/// <param name="timeOut">超时对象</param>
	public static void HandleTimeOutCheck(HslTimeOut timeOut)
	{
		lock (listLock)
		{
			if ((DateTime.Now - threadActiveTime).TotalSeconds > 60.0)
			{
				threadActiveTime = DateTime.Now;
				if (Interlocked.Increment(ref activeDisableCount) >= 2)
				{
					CreateTimeoutCheckThread();
				}
			}
			WaitHandleTimeOut.Add(timeOut);
		}
	}

	[HslMqttApi(Description = "Get the current list of all waiting timeout check objects, do not manually change the property value of the object", HttpMethod = "GET")]
	/// <summary>
	/// 获取当前的所有的等待超时检查对象列表，请勿手动更改对象的属性值
	/// Get the current list of all waiting timeout check objects, do not manually change the property value of the object
	/// </summary>
	/// <returns>HslTimeOut数组，请勿手动更改对象的属性值</returns>
	public static HslTimeOut[] GetHslTimeOutsSnapShoot()
	{
		lock (listLock)
		{
			return WaitHandleTimeOut.ToArray();
		}
	}

	/// <summary>
	/// 新增一个超时检测的对象，需要指定socket，超时时间，返回对象，用作标记完成信息
	/// Add a new object for timeout detection, you need to specify the socket, the timeout period,
	/// and return the  object for marking completion information
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="timeout">超时时间，单位为毫秒 Timeout period, in milliseconds</param>
	public static HslTimeOut HandleTimeOutCheck(Socket socket, int timeout)
	{
		HslTimeOut hslTimeOut = new HslTimeOut
		{
			DelayTime = timeout,
			IsSuccessful = false,
			StartTime = DateTime.Now,
			WorkSocket = socket
		};
		if (timeout > 0)
		{
			HandleTimeOutCheck(hslTimeOut);
		}
		return hslTimeOut;
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	static HslTimeOut()
	{
		hslTimeoutId = 0L;
		WaitHandleTimeOut = new List<HslTimeOut>(128);
		listLock = new object();
		threadUniqueId = 0L;
		activeDisableCount = 0;
		TimeoutDealCount = 0L;
		isLinuxPlatform = false;
		CreateTimeoutCheckThread();
	}

	private static void CreateTimeoutCheckThread()
	{
		threadActiveTime = DateTime.Now;
		try
		{
			threadCheckTimeOut?.Abort();
		}
		catch
		{
		}
		threadCheckTimeOut = new Thread(CheckTimeOut);
		threadCheckTimeOut.IsBackground = true;
		threadCheckTimeOut.Priority = ThreadPriority.AboveNormal;
		threadCheckTimeOut.Start(Interlocked.Increment(ref threadUniqueId));
	}

	/// <summary>
	/// 整个HslCommunication的检测超时的核心方法，由一个单独的线程运行，线程的优先级很高，当前其他所有的超时信息都可以放到这里处理
	/// The core method of detecting the timeout of th e entire HslCommunication is run by a separate thread.
	/// The priority of the thread is very high. All other timeout information can be processed here.
	/// </summary>
	/// <param name="obj">需要传入线程的id信息</param>
	private static void CheckTimeOut(object obj)
	{
		isLinuxPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		long num = (long)obj;
		TimeoutLogNet?.WriteInfo("HslCommunication.TimeoutThread", $"ID[{num}] CheckTimeOut start.");
		ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
		int num2 = 0;
		while (true)
		{
			try
			{
				manualResetEvent.WaitOne(100);
				num2 = 0;
			}
			catch (Exception ex)
			{
				num2++;
				TimeoutLogNet?.WriteWarn("HslCommunication.TimeoutThread", $"ID[{num}] Exception {SoftBasic.GetExceptionMessage(ex)}");
				HslHelper.ThreadSleep(100);
				if (num2 == 5 || num2 % 1000 == 0)
				{
					manualResetEvent.Close();
					manualResetEvent = new ManualResetEvent(initialState: false);
				}
			}
			if (num != threadUniqueId)
			{
				break;
			}
			threadActiveTime = DateTime.Now;
			activeDisableCount = 0;
			lock (listLock)
			{
				for (int num3 = WaitHandleTimeOut.Count - 1; num3 >= 0; num3--)
				{
					HslTimeOut hslTimeOut = WaitHandleTimeOut[num3];
					if (hslTimeOut.IsSuccessful)
					{
						WaitHandleTimeOut[num3].WorkSocket = null;
						WaitHandleTimeOut.RemoveAt(num3);
						Interlocked.Increment(ref TimeoutDealCount);
					}
					else
					{
						double num4 = (DateTime.Now - hslTimeOut.StartTime).TotalMilliseconds;
						if (num4 < 0.0)
						{
							hslTimeOut.StartTime = DateTime.Now;
							num4 = 0.0;
						}
						if (num4 > (double)hslTimeOut.DelayTime)
						{
							if (!hslTimeOut.IsSuccessful)
							{
								if (isLinuxPlatform)
								{
									try
									{
										hslTimeOut.WorkSocket?.Disconnect(reuseSocket: false);
									}
									catch
									{
									}
								}
								NetSupport.CloseSocket(hslTimeOut.WorkSocket);
								hslTimeOut.IsTimeout = true;
							}
							WaitHandleTimeOut[num3].WorkSocket = null;
							WaitHandleTimeOut.RemoveAt(num3);
							Interlocked.Increment(ref TimeoutDealCount);
						}
					}
				}
			}
		}
		TimeoutLogNet?.WriteWarn("HslCommunication.TimeoutThread", $"ID[{num}] break not same as {threadUniqueId}");
		manualResetEvent?.Close();
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 基于UDP/IT通信的管道信息
/// Pipeline information based on UDP/IT communication
/// </summary>
public class PipeUdpNet : PipeTcpNet
{
	/// <summary>
	/// 获取或设置一次接收时的数据长度，默认2KB数据长度，特殊情况的时候需要调整
	/// Gets or sets the length of data received at a time. The default length is 2KB
	/// </summary>
	public int ReceiveCacheLength { get; set; } = 2048;

	/// <summary>
	/// 实例化一个默认的对象
	/// Instantiate a default object
	/// </summary>
	public PipeUdpNet()
	{
	}

	/// <summary>
	/// 通过指定的IP地址和端口号来实例化一个对象
	/// Instantiate an object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号</param>
	public PipeUdpNet(string ipAddress, int port)
	{
		base.IpAddress = ipAddress;
		base.Port = port;
	}

	public override OperateResult<bool> OpenCommunication()
	{
		if (IsConnectError())
		{
			try
			{
				IPEndPoint connectIPEndPoint = GetConnectIPEndPoint();
				Socket socket = new Socket(connectIPEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				if (base.LocalBinding != null)
				{
					socket.Bind(base.LocalBinding);
				}
				base.Socket = socket;
				ResetConnectErrorCount();
				return OperateResult.CreateSuccessResult(value: true);
			}
			catch (Exception ex)
			{
				CloseCommunication();
				return new OperateResult<bool>(-IncrConnectErrorCount(), ex.Message);
			}
		}
		return OperateResult.CreateSuccessResult(value: false);
	}

	public override async Task<OperateResult<bool>> OpenCommunicationAsync()
	{
		return await Task.Run(() => OpenCommunication()).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override OperateResult Send(byte[] data, int offset, int size)
	{
		IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(base.IpAddress), base.Port);
		try
		{
			base.Socket.SendTo(data, offset, size, SocketFlags.None, remoteEP);
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			CloseCommunication();
			return new OperateResult<byte[]>(-IncrConnectErrorCount(), ex.Message);
		}
	}

	public override async Task<OperateResult> SendAsync(byte[] data, int offset, int size)
	{
		return await Task.Run(() => Send(data, offset, size)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override OperateResult<int> Receive(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		try
		{
			base.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeOut);
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(base.IpAddress), base.Port);
			IPEndPoint iPEndPoint2 = new IPEndPoint((iPEndPoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
			EndPoint remoteEP = iPEndPoint2;
			if (length > 0)
			{
				int value = base.Socket.ReceiveFrom(buffer, offset, length, SocketFlags.None, ref remoteEP);
				return OperateResult.CreateSuccessResult(value);
			}
			int value2 = base.Socket.ReceiveFrom(buffer, offset, buffer.Length - offset, SocketFlags.None, ref remoteEP);
			return OperateResult.CreateSuccessResult(value2);
		}
		catch (Exception ex)
		{
			CloseCommunication();
			return new OperateResult<int>(-IncrConnectErrorCount(), "Socket Exception -> " + ex.Message);
		}
	}

	public override async Task<OperateResult<int>> ReceiveAsync(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		return await Task.Run(() => Receive(buffer, offset, length, timeOut, reportProgress)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override OperateResult<byte[]> ReceiveMessage(INetMessage netMessage, byte[] sendValue, bool useActivePush = true, Action<long, long> reportProgress = null, Action<byte[]> logMessage = null)
	{
		if (base.UseServerActivePush)
		{
			return base.ReceiveMessage(netMessage, sendValue, useActivePush, reportProgress, logMessage);
		}
		return ReceiveMessage(netMessage, sendValue, null, logMessage);
	}

	/// <param name="alreadyReceive">已经接收的数据信息</param>
	public OperateResult<byte[]> ReceiveMessage(INetMessage netMessage, byte[] sendValue, byte[] alreadyReceive, Action<byte[]> logMessage = null, bool closeOnException = true)
	{
		try
		{
			MemoryStream ms = new MemoryStream();
			if (alreadyReceive != null && alreadyReceive.Length != 0)
			{
				ms.Write(alreadyReceive);
				if (CheckMessageComplete(netMessage, sendValue, ref ms))
				{
					return OperateResult.CreateSuccessResult(ms.ToArray());
				}
			}
			base.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, base.ReceiveTimeOut);
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(base.IpAddress), base.Port);
			IPEndPoint iPEndPoint2 = new IPEndPoint((iPEndPoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
			EndPoint remoteEP = iPEndPoint2;
			OperateResult<byte[]> operateResult = NetSupport.CreateReceiveBuffer(ReceiveCacheLength);
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			DateTime now = DateTime.Now;
			while (true)
			{
				int length = base.Socket.ReceiveFrom(operateResult.Content, ref remoteEP);
				byte[] array = operateResult.Content.SelectBegin(length);
				ms.Write(array);
				logMessage?.Invoke(array);
				if (netMessage == null || CheckMessageComplete(netMessage, sendValue, ref ms))
				{
					break;
				}
				if (base.ReceiveTimeOut >= 0 && (DateTime.Now - now).TotalMilliseconds > (double)base.ReceiveTimeOut)
				{
					return new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + base.ReceiveTimeOut + " Received: " + ms.ToArray().ToHexString(' '));
				}
			}
			return OperateResult.CreateSuccessResult(ms.ToArray());
		}
		catch (Exception ex)
		{
			if (closeOnException)
			{
				CloseCommunication();
			}
			return new OperateResult<byte[]>(-IncrConnectErrorCount(), "Socket Exception -> " + ex.Message);
		}
	}

	public override async Task<OperateResult<byte[]>> ReceiveMessageAsync(INetMessage netMessage, byte[] sendValue, bool useActivePush = true, Action<long, long> reportProgress = null, Action<byte[]> logMessage = null)
	{
		return await Task.Run(() => ReceiveMessage(netMessage, sendValue, useActivePush, reportProgress, logMessage)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override string ToString()
	{
		return $"PipeUdpNet[{base.IpAddress}:{base.Port}]";
	}
}

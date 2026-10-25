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
using System.Net;
using System.Text;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Core.Net;

/// <summary>
/// 异形客户端的基类，提供了基础的异形操作
/// The base class of the profiled client provides the basic profiled operation
/// </summary>
public class NetworkAlienClient : CommunicationTcpServer, IDisposable
{
	public delegate void OnClientConnectedDelegate(PipeDtuNet pipe);

	private byte[] password;

	private List<string> trustOnline;

	private SimpleHybirdLock trustLock;

	private bool isResponseAck = true;

	private bool isCheckPwd = true;

	private bool disposedValue;

	private bool useRegistrationPackage = true;

	/// <summary>
	/// 状态登录成功
	/// </summary>
	public const byte StatusOk = 0;

	/// <summary>
	/// 重复登录
	/// </summary>
	public const byte StatusLoginRepeat = 1;

	/// <summary>
	/// 禁止登录
	/// </summary>
	public const byte StatusLoginForbidden = 2;

	/// <summary>
	/// 密码错误
	/// </summary>
	public const byte StatusPasswodWrong = 3;

	/// <summary>
	/// 在DTU设备发送了注册报文的时候，指示是否返回响应报文，用来通知DTU设备是否登录成功，默认为 True
	/// When a DTU sends a registration packet, it indicates whether to return a response packet to notify the DTU whether the DTU is logged in, The default is True
	/// </summary>
	public bool IsResponseAck
	{
		get
		{
			return isResponseAck;
		}
		set
		{
			isResponseAck = value;
		}
	}

	/// <summary>
	/// 是否统一检查密码，如果每个会话需要自己检查密码，就需要设置为false
	/// Whether to check the password uniformly, if each session needs to check the password by itself, it needs to be set to false
	/// </summary>
	public bool IsCheckPwd
	{
		get
		{
			return isCheckPwd;
		}
		set
		{
			isCheckPwd = value;
		}
	}

	/// <summary>
	/// 是否启用注册包的功能，如果不启用，就不会进行注册包的验证，有连接上来就自动覆盖本地的会话信息，不是很安全
	/// Whether to enable the registration package function. If not enabled, the registration package verification will not be performed,
	/// and the local session information will be automatically overwritten when a connection is made, which is not very secure
	/// </summary>
	public bool UseRegistrationPackage
	{
		get
		{
			return useRegistrationPackage;
		}
		set
		{
			useRegistrationPackage = value;
		}
	}

	public event OnClientConnectedDelegate OnClientConnected = null;

	/// <summary>
	/// 默认的无参构造方法
	/// The default parameterless constructor
	/// </summary>
	public NetworkAlienClient()
	{
		password = new byte[6];
		trustOnline = new List<string>();
		trustLock = new SimpleHybirdLock();
	}

	/// <summary>
	/// 当接收到了新的请求的时候执行的操作
	/// An action performed when a new request is received
	/// </summary>
	/// <param name="pipeTcpNet">管道对象</param>
	/// <param name="endPoint">终结点</param>
	protected override async void ThreadPoolLogin(PipeTcpNet pipeTcpNet, IPEndPoint endPoint)
	{
		if (useRegistrationPackage)
		{
			OperateResult<byte[]> check = await pipeTcpNet.ReceiveMessageAsync(new AlienMessage(), null, useActivePush: false);
			if (!check.IsSuccess)
			{
				base.LogNet?.WriteDebug(ToString(), $"Initialize [{endPoint}] DTU failed: " + check.Message);
				return;
			}
			byte[] content = check.Content;
			if (content != null && content.Length < 16)
			{
				pipeTcpNet.CloseCommunication();
				return;
			}
			if (check.Content[0] != 72)
			{
				pipeTcpNet.CloseCommunication();
				return;
			}
			string dtu = Encoding.ASCII.GetString(check.Content, 5, 11).Trim('\0', ' ');
			bool needAckResult = true;
			if (check.Content.Length >= 29)
			{
				if (check.Content[28] == 1 || check.Content[28] == 49)
				{
					needAckResult = false;
				}
			}
			else if (check.Content.Length <= 16)
			{
				needAckResult = false;
			}
			bool isPasswrodRight = true;
			if (isCheckPwd)
			{
				for (int i = 0; i < password.Length; i++)
				{
					if (check.Content.Length > 16 + i)
					{
						if (check.Content[16 + i] != password[i])
						{
							isPasswrodRight = false;
							break;
						}
					}
					else if (password[i] != 0 && password[i] != 48)
					{
						isPasswrodRight = false;
						break;
					}
				}
			}
			if (!isPasswrodRight)
			{
				if (isResponseAck & needAckResult)
				{
					OperateResult send = pipeTcpNet.Send(GetResponse(3));
					if (send.IsSuccess)
					{
						pipeTcpNet.CloseCommunication();
					}
				}
				else
				{
					pipeTcpNet.CloseCommunication();
				}
				base.LogNet?.WriteWarn(ToString(), $"[{endPoint}] DTU:{dtu} Login Password Wrong, Id:" + dtu);
				return;
			}
			PipeDtuNet pipeDtuNet = new PipeDtuNet(pipeTcpNet)
			{
				DTU = dtu,
				DtuServer = this
			};
			int pwdLength = check.Content.Length - 16;
			if (pwdLength > 6)
			{
				pwdLength = 6;
			}
			pipeDtuNet.Pwd = ((pwdLength > 0) ? check.Content.SelectMiddle(16, pwdLength).ToHexString() : "");
			if (check.Content.Length >= 28)
			{
				pipeDtuNet.DTUIpAddress = BitConverter.ToInt32(check.Content, 22);
				pipeDtuNet.DTUPort = BitConverter.ToUInt16(check.Content, 26);
			}
			if (!IsClientPermission(pipeDtuNet))
			{
				if (isResponseAck & needAckResult)
				{
					OperateResult send2 = pipeTcpNet.Send(GetResponse(2));
					if (send2.IsSuccess)
					{
						pipeTcpNet.CloseCommunication();
					}
				}
				else
				{
					pipeTcpNet.CloseCommunication();
				}
				base.LogNet?.WriteWarn(ToString(), $"Initialize [{endPoint}] DTU:{dtu} Login Forbidden");
				return;
			}
			int status = IsClientOnline(pipeDtuNet);
			if (status != 0)
			{
				if (isResponseAck & needAckResult)
				{
					OperateResult send3 = pipeTcpNet.Send(GetResponse(1));
					if (send3.IsSuccess)
					{
						pipeTcpNet.CloseCommunication();
					}
				}
				else
				{
					pipeTcpNet.CloseCommunication();
				}
				base.LogNet?.WriteDebug(ToString(), GetMsgFromCode($"Initialize [{endPoint}] DTU:{dtu}", status, "  Ack :" + (isResponseAck & needAckResult)));
				return;
			}
			if (isResponseAck & needAckResult)
			{
				OperateResult send4 = pipeTcpNet.Send(GetResponse(0));
				if (!send4.IsSuccess)
				{
					return;
				}
			}
			base.LogNet?.WriteDebug(ToString(), GetMsgFromCode($"Initialize [{endPoint}] DTU:{dtu}", status, "  Ack :" + (isResponseAck & needAckResult)));
			RaiseClientConnected(pipeDtuNet);
		}
		else
		{
			RaiseClientConnected(new PipeDtuNet(pipeTcpNet)
			{
				DtuServer = this
			});
		}
	}

	private void RaiseClientConnected(PipeDtuNet pipeDtuNet)
	{
		base.LogNet?.WriteDebug(ToString(), $"Dtu Session<{pipeDtuNet}> Connected");
		OnClientConnected?.Invoke(pipeDtuNet);
	}

	/// <summary>
	/// 获取取返回的命令信息
	/// </summary>
	/// <param name="status">状态</param>
	/// <returns>回发的指令信息</returns>
	private byte[] GetResponse(byte status)
	{
		byte[] buffer = new byte[6] { 72, 115, 110, 0, 1, 0 };
		buffer[5] = status;
		return buffer;
	}

	/// <summary>
	/// 检测当前的DTU是否在线
	/// </summary>
	/// <param name="pipe">当前的会话信息</param>
	/// <returns>当前的会话是否在线</returns>
	public virtual int IsClientOnline(PipeDtuNet pipe)
	{
		return 0;
	}

	/// <summary>
	/// 检测当前的dtu是否允许登录
	/// </summary>
	/// <param name="session">当前的会话信息</param>
	/// <returns>当前的id是否可允许登录</returns>
	private bool IsClientPermission(PipeDtuNet session)
	{
		bool success = false;
		trustLock.Enter();
		if (trustOnline.Count == 0)
		{
			success = true;
		}
		else
		{
			for (int i = 0; i < trustOnline.Count; i++)
			{
				if (trustOnline[i] == session.DTU)
				{
					success = true;
					break;
				}
			}
		}
		trustLock.Leave();
		return success;
	}

	/// <summary>
	/// 设置密码，需要传入长度为6的字节数组
	/// To set the password, you need to pass in an array of bytes of length 6
	/// </summary>
	/// <param name="password">密码信息</param>
	public void SetPassword(byte[] password)
	{
		if (password != null && password.Length == 6)
		{
			password.CopyTo(this.password, 0);
		}
	}

	/// <summary>
	/// 设置可信任的客户端列表，传入一个DTU的列表信息
	/// Set up the list of trusted clients, passing in the list information for a DTU
	/// </summary>
	/// <param name="clients">客户端列表</param>
	public void SetTrustClients(string[] clients)
	{
		trustOnline = new List<string>(clients);
	}

	/// <summary>
	/// 释放当前的对象
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				trustLock?.Dispose();
				OnClientConnected = null;
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public override string ToString()
	{
		return "NetworkAlienBase";
	}

	/// <summary>
	/// 获取错误的描述信息
	/// </summary>
	/// <param name="head">dtu信息</param>
	/// <param name="code">错误码</param>
	/// <param name="info">其他消息</param>
	/// <returns>错误信息</returns>
	public static string GetMsgFromCode(string head, int code, string info)
	{
		return code switch
		{
			0 => head + "  Login Success" + info, 
			1 => head + "  Login Repeat" + info, 
			2 => head + "  Login Forbidden" + info, 
			3 => head + "  Login Passwod Wrong" + info, 
			_ => head + "  Login Unknow reason:" + info, 
		};
	}
}

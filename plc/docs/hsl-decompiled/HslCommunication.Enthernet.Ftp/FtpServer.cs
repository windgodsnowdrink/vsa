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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Enthernet.Ftp;

/// <summary>
/// 一个Ftp服务器
/// </summary>
public class FtpServer : CommunicationTcpServer
{
	private string currentDirectory = "";

	private AsyncCallback receiveAsyncCallback;

	private AsyncCallback receiveFileAsyncCallback;

	private List<FtpSession> ftpSessions = new List<FtpSession>();

	private object ftpSessionLock = new object();

	private bool allowAnonymous = true;

	private bool downloadOnly = false;

	private int portPasvMin = 50000;

	private int portPasvMax = 60000;

	/// <summary>
	/// 获取或设置 当前FTP服务器的根目录信息，如果目录不存在，则自动创建该目录。
	/// Get or set the root directory information of the current FTP server. If the directory does not exist, the directory is created automatically.
	/// </summary>
	public string CurrentDirectory
	{
		get
		{
			return currentDirectory;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				value = string.Empty;
			}
			if (value.EndsWith("\\"))
			{
				value = value.Substring(0, value.Length - 1);
			}
			if (!Directory.Exists(value))
			{
				Directory.CreateDirectory(value);
			}
			currentDirectory = value;
		}
	}

	/// <summary>
	/// 获取在线列表信息
	/// </summary>
	public FtpSession[] Sessions
	{
		get
		{
			lock (ftpSessionLock)
			{
				return ftpSessions.ToArray();
			}
		}
	}

	/// <summary>
	/// 允许匿名登录
	/// </summary>
	public bool AllowAnonymous
	{
		get
		{
			return allowAnonymous;
		}
		set
		{
			allowAnonymous = value;
		}
	}

	/// <summary>
	/// 仅允许下载，不允许上传，删除，重命名操作，对所有的账户都生效
	/// </summary>
	public bool DownloadOnly
	{
		get
		{
			return downloadOnly;
		}
		set
		{
			downloadOnly = value;
		}
	}

	/// <summary>
	/// 检查账户的委托，参数分别是用户名，密码，会话信息，可以详细控制每个账户的权限
	/// </summary>
	public Func<string, string, FtpSession, OperateResult> CheckAccountFunction { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FtpServer()
	{
		receiveAsyncCallback = ReceiveAsyncCallback;
		receiveFileAsyncCallback = ReceiveFileAsyncCallback;
	}

	/// <summary>
	/// 设置PASV功能下的随机端口范围，例如 10000-20000
	/// </summary>
	/// <param name="range">范围</param>
	public void SetPasvPortRange(string range)
	{
		MatchCollection matchCollection = Regex.Matches(range, "[0-9]+");
		if (matchCollection.Count == 1)
		{
			portPasvMin = Convert.ToInt32(matchCollection[0].Value);
			portPasvMax = portPasvMin + 1;
		}
		else if (matchCollection.Count == 2)
		{
			int val = Convert.ToInt32(matchCollection[0].Value);
			int val2 = Convert.ToInt32(matchCollection[1].Value);
			portPasvMin = Math.Min(val, val2);
			portPasvMax = Math.Max(val, val2);
			if (portPasvMin < 0)
			{
				portPasvMin = 0;
			}
			if (portPasvMax < 0 || portPasvMax > 65535)
			{
				portPasvMax = 65535;
			}
			if (portPasvMin == portPasvMax)
			{
				portPasvMax = portPasvMin + 1;
			}
		}
	}

	private void RemoveFtpSession(FtpSession session, string reason)
	{
		lock (ftpSessionLock)
		{
			if (ftpSessions.Remove(session))
			{
				session.Session.CloseCommunication();
				base.LogNet?.WriteDebug(ToString(), $"{session.Session} User:{session.UserName} logged out. Reason:{reason}");
			}
		}
	}

	private OperateResult SendString(PipeTcpNet pipe, int code, string msg)
	{
		string s = $"{code} {msg}\r\n";
		base.LogNet?.WriteDebug(ToString(), $"Send<{pipe}> {code} {msg}");
		return pipe.Send(Encoding.UTF8.GetBytes(s));
	}

	private OperateResult SendString(FtpSession session, int code, string msg)
	{
		string s = $"{code} {msg}\r\n";
		base.LogNet?.WriteDebug(ToString(), $"Send<{session.Session}> {code} {msg}");
		return session.Session.Send(session.Encoding.GetBytes(s));
	}

	private bool SendCommand(FtpSession session, int code, string msg, PasvServer pasv = null)
	{
		OperateResult operateResult = SendString(session, code, msg);
		if (!operateResult.IsSuccess)
		{
			pasv?.Close();
			RemoveFtpSession(session, operateResult.Message);
			return true;
		}
		return false;
	}

	private OperateResult<byte[]> ReceiveHelper(PipeTcpNet pipe)
	{
		OperateResult<byte[]> operateResult = pipe.ReceiveMessage(new SpecifiedCharacterMessage(13, 10), null);
		if (operateResult.IsSuccess)
		{
			base.LogNet?.WriteDebug(ToString(), $"Recv<{pipe}> {Encoding.UTF8.GetString(operateResult.Content).Trim()}");
		}
		return operateResult;
	}

	private bool ReceiveCommand(FtpSession session, out string command, PasvServer pasv = null)
	{
		command = null;
		OperateResult<byte[]> operateResult = session.Session.ReceiveMessage(new SpecifiedCharacterMessage(13, 10), null);
		if (!operateResult.IsSuccess)
		{
			pasv?.Close();
			RemoveFtpSession(session, operateResult.Message);
			return true;
		}
		base.LogNet?.WriteDebug(ToString(), $"Recv<{session.Session}> {session.Encoding.GetString(operateResult.Content).Trim()}");
		command = session.Encoding.GetString(operateResult.Content).Trim();
		return false;
	}

	private OperateResult ReceiveAndSend(PipeTcpNet pipe, int code, string msg)
	{
		OperateResult<byte[]> operateResult = ReceiveHelper(pipe);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return SendString(pipe, code, msg);
	}

	private OperateResult<FtpSession> CheckUser(PipeTcpNet pipe, string command)
	{
		OperateResult operateResult = null;
		if (command.StartsWith("USER ", StringComparison.OrdinalIgnoreCase))
		{
			operateResult = SendString(pipe, 331, "User name okay, need password.");
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<FtpSession>(operateResult);
			}
			OperateResult<byte[]> operateResult2 = ReceiveHelper(pipe);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<FtpSession>(operateResult2);
			}
			string message = Encoding.UTF8.GetString(operateResult2.Content).Trim();
			if (message.StartsWith("PASS ", StringComparison.OrdinalIgnoreCase))
			{
				string text2 = command.Substring(5);
				string arg = message.Substring(5);
				FtpSession ftpSession = new FtpSession
				{
					Session = pipe,
					UserName = text2,
					CurrentDirectory = "/",
					CanUpload = true
				};
				if (!allowAnonymous || !text2.Equals("anonymous"))
				{
					if (CheckAccountFunction == null)
					{
						OperateResult operateResult3 = SendString(pipe, 531, "User " + text2 + " not allow login.");
						if (!operateResult3.IsSuccess)
						{
							return OperateResult.CreateFailedResult<FtpSession>(operateResult3);
						}
						pipe?.CloseCommunication();
						return new OperateResult<FtpSession>();
					}
					OperateResult operateResult4 = CheckAccountFunction(text2, arg, ftpSession);
					if (!operateResult4.IsSuccess)
					{
						OperateResult operateResult5 = SendString(pipe, operateResult4.ErrorCode, "User " + text2 + " login failed. " + operateResult4.Message);
						if (!operateResult5.IsSuccess)
						{
							return OperateResult.CreateFailedResult<FtpSession>(operateResult5);
						}
						pipe?.CloseCommunication();
						return new OperateResult<FtpSession>();
					}
				}
				OperateResult operateResult6 = SendString(pipe, 230, "User logged in.");
				if (!operateResult6.IsSuccess)
				{
					return OperateResult.CreateFailedResult<FtpSession>(operateResult6);
				}
				return OperateResult.CreateSuccessResult(ftpSession);
			}
			OperateResult result = SendString(pipe, 500, "Syntax error, command unrecognized. please send PASS");
			pipe.CloseCommunication();
			return OperateResult.CreateFailedResult<FtpSession>(result);
		}
		operateResult = SendString(pipe, 500, "Syntax error, command unrecognized. please send USER");
		pipe.CloseCommunication();
		return new OperateResult<FtpSession>();
	}

	protected override void ThreadPoolLogin(PipeTcpNet pipe, IPEndPoint endPoint)
	{
		OperateResult operateResult = SendString(pipe, 220, "HslCommunication FTP Service");
		if (!operateResult.IsSuccess)
		{
			return;
		}
		Encoding encoding = Encoding.Default;
		string text;
		while (true)
		{
			OperateResult<byte[]> operateResult2 = ReceiveHelper(pipe);
			if (!operateResult2.IsSuccess)
			{
				return;
			}
			text = (text = Encoding.UTF8.GetString(operateResult2.Content).Trim());
			if (text.StartsWith("OPTS", StringComparison.OrdinalIgnoreCase))
			{
				if (text.IndexOf("UTF8 ON", StringComparison.OrdinalIgnoreCase) > 0)
				{
					encoding = Encoding.UTF8;
					if (!SendString(pipe, 200, "OPTS UTF8 command successful - UTF8 encoding now ON.").IsSuccess)
					{
						return;
					}
				}
				else if (!SendString(pipe, 200, text + " command successful").IsSuccess)
				{
					return;
				}
			}
			else if (text.Equals("SYST", StringComparison.OrdinalIgnoreCase))
			{
				if (!SendString(pipe, 215, Environment.OSVersion.VersionString).IsSuccess)
				{
					return;
				}
			}
			else if (text.StartsWithNoCase("AUTH TLS"))
			{
				if (!SendString(pipe, 534, "Local policy on server does not allow TLS secure connections.").IsSuccess)
				{
					return;
				}
			}
			else
			{
				if (!text.StartsWithNoCase("AUTH SSL"))
				{
					break;
				}
				if (!SendString(pipe, 534, "Local policy on server does not allow TLS secure connections.").IsSuccess)
				{
					return;
				}
			}
		}
		if (text.StartsWith("USER ", StringComparison.OrdinalIgnoreCase))
		{
			OperateResult<FtpSession> operateResult3 = CheckUser(pipe, text);
			if (!operateResult3.IsSuccess)
			{
				return;
			}
			try
			{
				operateResult3.Content.Encoding = encoding;
				operateResult3.Content.Session.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, receiveAsyncCallback, operateResult3.Content);
				lock (ftpSessionLock)
				{
					ftpSessions.Add(operateResult3.Content);
					return;
				}
			}
			catch (Exception ex)
			{
				pipe.CloseCommunication();
				base.LogNet?.WriteException("ThreadPoolLogin.BeginReceive", ex);
				return;
			}
		}
		SendString(pipe, 500, "Syntax error, command unrecognized. please send USER");
		pipe.CloseCommunication();
	}

	private void BeginReceiveAgain(FtpSession ftpSession)
	{
		try
		{
			ftpSession.Session.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, receiveAsyncCallback, ftpSession);
		}
		catch (Exception ex)
		{
			RemoveFtpSession(ftpSession, "BeginReceiveAgain Ex:" + ex.Message);
		}
	}

	private void ReceiveAsyncCallback(IAsyncResult iar)
	{
		if (!(iar.AsyncState is FtpSession ftpSession))
		{
			return;
		}
		try
		{
			int num = ftpSession.Session.Socket.EndReceive(iar);
		}
		catch (ObjectDisposedException)
		{
			RemoveFtpSession(ftpSession, "Server disposed");
			return;
		}
		catch (SocketException ex2)
		{
			RemoveFtpSession(ftpSession, $"Socket exception: {ex2.SocketErrorCode}");
			return;
		}
		catch (Exception ex3)
		{
			RemoveFtpSession(ftpSession, "Exception: " + ex3.Message);
			return;
		}
		PipeTcpNet session = ftpSession.Session;
		if (ReceiveCommand(ftpSession, out var command))
		{
			return;
		}
		if (command.Equals("QUIT", StringComparison.OrdinalIgnoreCase))
		{
			OperateResult operateResult = SendString(session, 221, "Goodbye.");
			session.CloseCommunication();
			RemoveFtpSession(ftpSession, "User quit");
			return;
		}
		PasvServer pasvServer;
		string command3;
		if (command.StartsWith("OPTS", StringComparison.OrdinalIgnoreCase))
		{
			if (command.IndexOf("UTF8 ON", StringComparison.OrdinalIgnoreCase) > 0)
			{
				ftpSession.Encoding = Encoding.UTF8;
				if (!SendString(session, 200, "OPTS UTF8 command successful - UTF8 encoding now ON.").IsSuccess)
				{
					return;
				}
			}
			else if (!SendString(session, 200, command + " command successful").IsSuccess)
			{
				return;
			}
		}
		else if (command.Equals("SYST", StringComparison.OrdinalIgnoreCase))
		{
			if (SendCommand(ftpSession, 215, Environment.OSVersion.VersionString))
			{
				return;
			}
		}
		else if (command.Equals("TYPE I", StringComparison.OrdinalIgnoreCase))
		{
			if (SendCommand(ftpSession, 200, "Type set to I."))
			{
				return;
			}
		}
		else if (command.Equals("TYPE A", StringComparison.OrdinalIgnoreCase))
		{
			if (SendCommand(ftpSession, 200, "Type set to A."))
			{
				return;
			}
		}
		else if (command.StartsWith("PWD", StringComparison.OrdinalIgnoreCase))
		{
			if (SendCommand(ftpSession, 257, "\"" + ftpSession.CurrentDirectory + "\" is current directory."))
			{
				return;
			}
		}
		else if (command.Equals("CWD ..", StringComparison.OrdinalIgnoreCase) || command.Equals("CDUP", StringComparison.OrdinalIgnoreCase))
		{
			if (ftpSession.CurrentDirectory == "/")
			{
				if (SendCommand(ftpSession, 550, "the system can not find the path specified."))
				{
					return;
				}
			}
			else
			{
				int num2 = ftpSession.CurrentDirectory.LastIndexOf("/");
				if (num2 <= 0)
				{
					ftpSession.CurrentDirectory = "/";
				}
				else
				{
					ftpSession.CurrentDirectory = ftpSession.CurrentDirectory.Substring(0, num2);
				}
				if (SendCommand(ftpSession, 250, (command.Equals("CDUP", StringComparison.OrdinalIgnoreCase) ? "CDUP" : "CWD") + " command successfully."))
				{
					return;
				}
			}
		}
		else if (command.StartsWith("CWD", StringComparison.OrdinalIgnoreCase))
		{
			ftpSession.CurrentDirectory = command.Substring((command.Length >= 4) ? 4 : 3).Trim();
			if (SendCommand(ftpSession, 250, "CWD command successfully."))
			{
				return;
			}
		}
		else if (command.StartsWith("SIZE ", StringComparison.OrdinalIgnoreCase))
		{
			string physicalFilePath = ftpSession.GetPhysicalFilePath(CurrentDirectory, command.Substring(4).Trim());
			if (!File.Exists(physicalFilePath))
			{
				if (SendCommand(ftpSession, 550, "the system can not find the file spcified."))
				{
					return;
				}
			}
			else
			{
				try
				{
					FileInfo fileInfo = new FileInfo(physicalFilePath);
					if (SendCommand(ftpSession, 213, fileInfo.Length.ToString()))
					{
						return;
					}
				}
				catch (Exception ex4)
				{
					RemoveFtpSession(ftpSession, ex4.Message);
					return;
				}
			}
		}
		else if (command.StartsWith("DELE ", StringComparison.OrdinalIgnoreCase))
		{
			if (downloadOnly)
			{
				if (SendCommand(ftpSession, 550, "the system can download file only."))
				{
					return;
				}
			}
			else if (!ftpSession.CanEdit)
			{
				if (SendCommand(ftpSession, 550, "the account[" + ftpSession.UserName + "] have no DELE permission."))
				{
					return;
				}
			}
			else
			{
				string physicalFilePath2 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command.Substring(4).Trim());
				if (!File.Exists(physicalFilePath2))
				{
					if (SendCommand(ftpSession, 550, "the system can not find the file spcified."))
					{
						return;
					}
				}
				else
				{
					try
					{
						File.Delete(physicalFilePath2);
						if (SendCommand(ftpSession, 250, "DELE command successful."))
						{
							return;
						}
					}
					catch (Exception ex5)
					{
						if (SendCommand(ftpSession, 550, "DELE command failed. " + ex5.Message))
						{
							return;
						}
					}
				}
			}
		}
		else if (command.StartsWith("MKD ", StringComparison.OrdinalIgnoreCase))
		{
			if (downloadOnly)
			{
				if (SendCommand(ftpSession, 550, "the system can download file only."))
				{
					return;
				}
			}
			else if (!ftpSession.CanEdit)
			{
				if (SendCommand(ftpSession, 550, "the account[" + ftpSession.UserName + "] have no MKD permission."))
				{
					return;
				}
			}
			else
			{
				string physicalFilePath3 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command.Substring(4).Trim());
				try
				{
					if (Directory.Exists(physicalFilePath3))
					{
						if (SendCommand(ftpSession, 550, "The system cannot find the path specified. "))
						{
							return;
						}
					}
					else
					{
						Directory.CreateDirectory(physicalFilePath3);
						if (SendCommand(ftpSession, 250, "MKD command successful."))
						{
							return;
						}
					}
				}
				catch (Exception ex6)
				{
					if (SendCommand(ftpSession, 550, "MKD command failed. " + ex6.Message))
					{
						return;
					}
				}
			}
		}
		else if (command.StartsWith("RNFR ", StringComparison.OrdinalIgnoreCase))
		{
			if (downloadOnly)
			{
				if (SendCommand(ftpSession, 550, "the system can download file only."))
				{
					return;
				}
			}
			else if (!ftpSession.CanEdit)
			{
				if (SendCommand(ftpSession, 550, "the account[" + ftpSession.UserName + "] have no RNFR permission."))
				{
					return;
				}
			}
			else
			{
				string physicalFilePath4 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command.Substring(5).Trim());
				if (SendCommand(ftpSession, 350, "Requested file action pending further information.") || ReceiveCommand(ftpSession, out var command2))
				{
					return;
				}
				if (!command2.StartsWith("RNTO ", StringComparison.OrdinalIgnoreCase))
				{
					if (SendCommand(ftpSession, 550, "the command should be RNTO."))
					{
						return;
					}
				}
				else
				{
					string physicalFilePath5 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command2.Substring(5).Trim());
					if (!File.Exists(physicalFilePath4) && Directory.Exists(physicalFilePath4))
					{
						if (Directory.Exists(physicalFilePath5) && SendCommand(ftpSession, 550, "Cannot create a directory when that dir already exists."))
						{
							return;
						}
						try
						{
							Directory.Move(physicalFilePath4, physicalFilePath5);
							if (SendCommand(ftpSession, 250, "RNTO command successful."))
							{
								return;
							}
						}
						catch (Exception ex7)
						{
							if (SendCommand(ftpSession, 550, "RNTO command faild." + ex7.Message))
							{
								return;
							}
						}
					}
					else
					{
						if (File.Exists(physicalFilePath5) && SendCommand(ftpSession, 550, "Cannot create a file when that file already exists."))
						{
							return;
						}
						try
						{
							File.Move(physicalFilePath4, physicalFilePath5);
							if (SendCommand(ftpSession, 250, "RNTO command successful."))
							{
								return;
							}
						}
						catch (Exception ex8)
						{
							if (SendCommand(ftpSession, 550, "RNTO command faild." + ex8.Message))
							{
								return;
							}
						}
					}
				}
			}
		}
		else if (command.StartsWith("RMD ", StringComparison.OrdinalIgnoreCase))
		{
			if (downloadOnly)
			{
				if (SendCommand(ftpSession, 550, "the system can download file only."))
				{
					return;
				}
			}
			else if (!ftpSession.CanEdit)
			{
				if (SendCommand(ftpSession, 550, "the account[" + ftpSession.UserName + "] have no RMD permission."))
				{
					return;
				}
			}
			else
			{
				string physicalFilePath6 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command.Substring(4).Trim());
				if (!Directory.Exists(physicalFilePath6))
				{
					if (SendCommand(ftpSession, 550, "Cannot delete a directory when not exists."))
					{
						return;
					}
				}
				else
				{
					try
					{
						if (Directory.GetFiles(physicalFilePath6).Length != 0)
						{
							if (SendCommand(ftpSession, 550, "Cannot delete a directory when have files."))
							{
								return;
							}
						}
						else
						{
							Directory.Delete(physicalFilePath6, recursive: true);
							if (SendCommand(ftpSession, 250, "RMD command successful."))
							{
								return;
							}
						}
					}
					catch (Exception ex9)
					{
						if (SendCommand(ftpSession, 550, "RMD command faild." + ex9.Message))
						{
							return;
						}
					}
				}
			}
		}
		else if (command.StartsWith("PASV", StringComparison.OrdinalIgnoreCase))
		{
			IPAddress address = (session.Socket.LocalEndPoint as IPEndPoint).Address;
			pasvServer = new PasvServer();
			int num3 = pasvServer.Start(portPasvMin, portPasvMax);
			if (num3 < 0)
			{
				pasvServer.Close();
				if (SendCommand(ftpSession, 502, "Failed to enter passive mode."))
				{
					return;
				}
			}
			else
			{
				string msg = string.Format("Entering Passive Mode ({0},{1},{2}).", address.ToString().Replace(".", ","), BitConverter.GetBytes(num3)[1], BitConverter.GetBytes(num3)[0]);
				if (SendCommand(ftpSession, 227, msg, pasvServer) || ReceiveCommand(ftpSession, out command3, pasvServer))
				{
					return;
				}
				if (!command3.StartsWith("STOR ", StringComparison.OrdinalIgnoreCase))
				{
					goto IL_0c9d;
				}
				if (downloadOnly)
				{
					if (SendCommand(ftpSession, 550, "the system can download file only."))
					{
						return;
					}
					pasvServer?.Close();
				}
				else
				{
					if (ftpSession.CanUpload)
					{
						string physicalFilePath7 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command3.Substring(5));
						try
						{
							if (File.Exists(physicalFilePath7))
							{
								using (new FileStream(physicalFilePath7, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
								{
									HslHelper.ThreadSleep(20);
								}
							}
						}
						catch
						{
							if (SendCommand(ftpSession, 550, "UnauthorizedAccess failed: could not open file for writing."))
							{
								return;
							}
							goto IL_11bb;
						}
						goto IL_0c9d;
					}
					if (SendCommand(ftpSession, 550, "the account[" + ftpSession.UserName + "] have no STOR permission."))
					{
						return;
					}
					pasvServer?.Close();
				}
			}
		}
		else if (SendCommand(ftpSession, 502, "Command not implemented."))
		{
			return;
		}
		goto IL_11bb;
		IL_11bb:
		try
		{
			ftpSession.Session.Socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, receiveAsyncCallback, ftpSession);
			return;
		}
		catch (Exception ex10)
		{
			RemoveFtpSession(ftpSession, "BeginReceiveAgain Ex:" + ex10.Message);
			return;
		}
		IL_0c9d:
		if (SendCommand(ftpSession, 150, "Opening BINARY mode data connection.", pasvServer))
		{
			return;
		}
		DateTime now = DateTime.Now;
		while (pasvServer.ClientSocket == null)
		{
			HslHelper.ThreadSleep(10);
			if ((DateTime.Now - now).TotalMilliseconds > 10000.0)
			{
				pasvServer.Close();
				SendString(session, 425, "Can't open data connection.");
				RemoveFtpSession(ftpSession, "Pasv wait client connect timeout: 10000");
				return;
			}
		}
		if (command3.StartsWith("LIST", StringComparison.OrdinalIgnoreCase) || command3.StartsWith("MLSD", StringComparison.OrdinalIgnoreCase))
		{
			string physicalPath = ftpSession.GetPhysicalPath(CurrentDirectory);
			try
			{
				string[] directories = Directory.GetDirectories(physicalPath);
				string[] array = directories;
				foreach (string fileName in array)
				{
					string s = GetFileString(fileName, isDir: true) + "\r\n";
					pasvServer.ClientSocket.Send(ftpSession.Encoding.GetBytes(s));
				}
			}
			catch (UnauthorizedAccessException)
			{
				pasvServer.Close();
				if (SendCommand(ftpSession, 530, "UnauthorizedAccess failed: " + CurrentDirectory))
				{
					return;
				}
				goto IL_11bb;
			}
			catch (Exception ex12)
			{
				pasvServer.Close();
				RemoveFtpSession(ftpSession, "Pasv List Directories failed: " + ex12.Message);
				return;
			}
			try
			{
				string[] files = Directory.GetFiles(physicalPath);
				string[] array2 = files;
				foreach (string fileName2 in array2)
				{
					string s2 = GetFileString(fileName2, isDir: false) + "\r\n";
					pasvServer.ClientSocket.Send(ftpSession.Encoding.GetBytes(s2));
				}
			}
			catch (UnauthorizedAccessException)
			{
				pasvServer.Close();
				if (SendCommand(ftpSession, 530, "UnauthorizedAccess failed: " + CurrentDirectory))
				{
					return;
				}
				goto IL_11bb;
			}
			catch (Exception ex14)
			{
				pasvServer.Close();
				RemoveFtpSession(ftpSession, "Pasv List GetFiles failed: " + ex14.Message);
				return;
			}
			pasvServer.Close();
			if (SendCommand(ftpSession, 226, "Transfer complete."))
			{
				return;
			}
		}
		else if (command3.StartsWith("RETR ", StringComparison.OrdinalIgnoreCase))
		{
			string physicalFilePath8 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command3.Substring(5));
			try
			{
				FileStream fileStream = new FileStream(physicalFilePath8, FileMode.Open, FileAccess.Read);
				pasvServer.buffer = new byte[4096];
				while (true)
				{
					int bytesRead = fileStream.Read(pasvServer.buffer, 0, pasvServer.buffer.Length);
					if (bytesRead <= 0)
					{
						break;
					}
					pasvServer.ClientSocket.Send(pasvServer.buffer, 0, bytesRead, SocketFlags.None);
				}
				fileStream.Dispose();
			}
			catch (Exception ex15)
			{
				pasvServer.Close();
				RemoveFtpSession(ftpSession, "Pasv download failed: " + ex15.Message);
				return;
			}
			pasvServer.Close();
			if (SendCommand(ftpSession, 226, "Transfer complete."))
			{
				return;
			}
		}
		else if (command3.StartsWith("STOR ", StringComparison.OrdinalIgnoreCase))
		{
			string physicalFilePath9 = ftpSession.GetPhysicalFilePath(CurrentDirectory, command3.Substring(5));
			try
			{
				FileInfo fileInfo2 = new FileInfo(physicalFilePath9);
				if (!fileInfo2.Directory.Exists)
				{
					fileInfo2.Directory.Create();
				}
				pasvServer.SaveFileStream = new FileStream(physicalFilePath9, FileMode.Create, FileAccess.Write);
			}
			catch (Exception ex16)
			{
				pasvServer.Close();
				RemoveFtpSession(ftpSession, "pasv upload failed: " + ex16.Message);
				return;
			}
			try
			{
				pasvServer.buffer = new byte[4096];
				pasvServer.ClientSocket.BeginReceive(pasvServer.buffer, 0, pasvServer.buffer.Length, SocketFlags.None, receiveFileAsyncCallback, pasvServer);
			}
			catch (Exception ex17)
			{
				pasvServer.Close();
				RemoveFtpSession(ftpSession, "pasv upload failed: " + ex17.Message);
				return;
			}
			do
			{
				HslHelper.ThreadSleep(20);
			}
			while (!pasvServer.IsCompleted);
			HslHelper.ThreadSleep(20);
			pasvServer.SaveFileStream.Flush();
			pasvServer.SaveFileStream.Dispose();
			pasvServer.Close();
			if (SendCommand(ftpSession, 226, "Transfer complete."))
			{
				return;
			}
		}
		else
		{
			pasvServer.Close();
			if (SendCommand(ftpSession, 502, "Command[" + command3 + "] not implemented for PASV."))
			{
				return;
			}
		}
		goto IL_11bb;
	}

	private void ReceiveFileAsyncCallback(IAsyncResult ar)
	{
		if (!(ar.AsyncState is PasvServer pasvServer))
		{
			return;
		}
		try
		{
			int num = pasvServer.ClientSocket.EndReceive(ar);
			if (num == 0)
			{
				pasvServer.IsCompleted = true;
				return;
			}
			pasvServer.SaveFileStream.Write(pasvServer.buffer, 0, num);
			pasvServer.ClientSocket.BeginReceive(pasvServer.buffer, 0, pasvServer.buffer.Length, SocketFlags.None, receiveFileAsyncCallback, pasvServer);
		}
		catch (Exception)
		{
			pasvServer.IsCompleted = true;
		}
	}

	private string GetFileString(string fileName, bool isDir)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (isDir)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(fileName);
			stringBuilder.Append(directoryInfo.LastWriteTime.ToString("MM-dd-yy  hh:mmtt"));
			stringBuilder.Append("       <DIR>          ");
			stringBuilder.Append(directoryInfo.Name);
		}
		else
		{
			FileInfo fileInfo = new FileInfo(fileName);
			stringBuilder.Append(fileInfo.LastWriteTime.ToString("MM-dd-yy  hh:mmtt"));
			stringBuilder.Append(fileInfo.Length.ToString().PadLeft(21, ' '));
			stringBuilder.Append(" ");
			stringBuilder.Append(fileInfo.Name);
		}
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		return $"FtpServer[{base.Port}]";
	}
}

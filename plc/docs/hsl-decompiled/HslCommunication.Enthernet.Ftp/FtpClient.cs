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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HslCommunication.Core;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Enthernet.Ftp;

/// <summary>
/// FTP协议的客户端对象，支持上传，下载，删除，浏览文件等功能
/// FTP protocol client object, supports uploading, downloading, deleting, browsing files and other functions.
/// </summary>
public class FtpClient : TcpNetCommunication
{
	private int serverInfo = 0;

	private bool pasvUseIp = false;

	/// <summary>
	/// 获取或设置登录的用户名信息，默认值为anonymous
	/// Get or set the username for login, default is anonymous.
	/// </summary>
	public string Username { get; set; } = "anonymous";

	/// <summary>
	/// 获取或设置登录的密码信息，默认值为 hslcommunication@example.com
	/// Get or set the password for login, default is hslcommunication@example.com.
	/// </summary>
	public string Password { get; set; }

	/// <summary>
	/// 系统的类型信息
	/// System type information.
	/// </summary>
	public string SystemType { get; private set; } = "";

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FtpClient()
	{
		LogMsgFormatBinary = false;
	}

	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<PipeTcpNet> operateResult = InitializeFtpClient(CommunicationPipe);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return CloseFtpClient(CommunicationPipe);
	}

	protected override OperateResult ExtraOnDisconnect()
	{
		return CloseFtpClient(CommunicationPipe);
	}

	protected override string GetLogTextFromBinary(PipeSession session, byte[] content)
	{
		return LogMsgFormatBinary ? content.ToHexString(' ') : Encoding.UTF8.GetString(content).Replace("\r\n", "\\r\\n");
	}

	private OperateResult SendHelper(CommunicationPipe pipe, string send)
	{
		if (!send.EndsWith("\r\n"))
		{
			send += "\r\n";
		}
		byte[] bytes = Encoding.UTF8.GetBytes(send);
		LogSendMessage(bytes);
		return pipe.Send(bytes);
	}

	private OperateResult<byte[]> ReceiveHelper(CommunicationPipe pipe)
	{
		OperateResult<byte[]> operateResult = pipe.ReceiveMessage(GetNewNetMessage(), null);
		if (operateResult.IsSuccess)
		{
			LogRevcMessage(operateResult.Content);
		}
		return operateResult;
	}

	private OperateResult<PipeTcpNet> InitializeFtpClient(CommunicationPipe pipe)
	{
		OperateResult<byte[]> operateResult = ReceiveHelper(pipe);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult);
		}
		string message = Encoding.UTF8.GetString(operateResult.Content);
		if (message.StartsWith("220-FileZilla Server", StringComparison.OrdinalIgnoreCase))
		{
			serverInfo = 1;
		}
		if (message.StartsWith("220 HslCommunication FTP", StringComparison.OrdinalIgnoreCase))
		{
			pasvUseIp = true;
		}
		OperateResult operateResult2 = SendHelper(pipe, "OPTS UTF8 ON");
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult2);
		}
		string text2;
		do
		{
			OperateResult<byte[]> operateResult3 = ReceiveHelper(pipe);
			if (!operateResult3.IsSuccess)
			{
				return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult3);
			}
			text2 = Encoding.UTF8.GetString(operateResult3.Content);
		}
		while (text2.StartsWith("220"));
		OperateResult operateResult4 = SendHelper(pipe, "USER " + (string.IsNullOrEmpty(Username) ? "anonymous" : Username));
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult4);
		}
		OperateResult<byte[]> operateResult5 = ReceiveHelper(pipe);
		if (!operateResult5.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult5);
		}
		OperateResult operateResult6 = SendHelper(pipe, "PASS " + (string.IsNullOrEmpty(Password) ? "hslcommunication@example.com" : Password));
		if (!operateResult6.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult6);
		}
		OperateResult<byte[]> operateResult7 = ReceiveHelper(pipe);
		if (!operateResult7.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult7);
		}
		string message2 = Encoding.UTF8.GetString(operateResult7.Content);
		if (!message2.StartsWith("230"))
		{
			pipe.CloseCommunication();
			return new OperateResult<PipeTcpNet>(int.Parse(message2.Substring(0, 3)), message2);
		}
		OperateResult operateResult8 = SendHelper(pipe, "SYST");
		if (!operateResult8.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult8);
		}
		OperateResult<byte[]> operateResult9 = ReceiveHelper(pipe);
		if (!operateResult9.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult9);
		}
		string message3 = Encoding.UTF8.GetString(operateResult9.Content).Trim();
		if (message3.StartsWith("215 "))
		{
			SystemType = message3.Substring(4);
		}
		if (Regex.IsMatch(message3, "FileZilla", RegexOptions.IgnoreCase))
		{
			serverInfo = 1;
		}
		OperateResult operateResult10 = SendHelper(pipe, "TYPE I");
		if (!operateResult10.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult10);
		}
		OperateResult<byte[]> operateResult11 = ReceiveHelper(pipe);
		if (!operateResult11.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult11);
		}
		return OperateResult.CreateSuccessResult(pipe as PipeTcpNet);
	}

	private OperateResult CloseFtpClient(CommunicationPipe pipe)
	{
		OperateResult operateResult = SendHelper(pipe, "QUIT");
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReceiveHelper(pipe);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	private OperateResult<PipeTcpNet> CreatePipe()
	{
		PipeTcpNet pipeTcpNet = new PipeTcpNet(IpAddress, Port);
		pipeTcpNet.ConnectTimeOut = ConnectTimeOut;
		pipeTcpNet.ReceiveTimeOut = base.ReceiveTimeOut;
		OperateResult operateResult = pipeTcpNet.OpenCommunication();
		if (!operateResult.IsSuccess)
		{
			pipeTcpNet.CloseCommunication();
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult);
		}
		OperateResult<PipeTcpNet> operateResult2 = InitializeFtpClient(pipeTcpNet);
		if (!operateResult2.IsSuccess)
		{
			pipeTcpNet.CloseCommunication();
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult2);
		}
		return operateResult2;
	}

	private OperateResult<PipeTcpNet> PasvCommand(CommunicationPipe pipe)
	{
		OperateResult operateResult = SendHelper(pipe, "PASV");
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReceiveHelper(pipe);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<PipeTcpNet>(operateResult2);
		}
		string message = Encoding.UTF8.GetString(operateResult2.Content);
		int num = message.IndexOf('(');
		int num2 = message.IndexOf(')');
		if (num < 0 || num2 < 0 || num2 <= num)
		{
			return new OperateResult<PipeTcpNet>("Pasv Format Error: " + message);
		}
		string[] array = message.Substring(num + 1, num2 - num - 1).Split(',');
		if (array.Length != 6)
		{
			return new OperateResult<PipeTcpNet>("Pasv Format Error: " + message);
		}
		string ipAddress = string.Join(".", array, 0, 4);
		int port = int.Parse(array[4]) * 256 + int.Parse(array[5]);
		return OperateResult.CreateSuccessResult(new PipeTcpNet(ipAddress, port)
		{
			ConnectTimeOut = 5000
		});
	}

	private OperateResult<string> CheckResultContent(string content, PipeTcpNet pipe)
	{
		if (content.StartsWith("5"))
		{
			pipe?.CloseCommunication();
			if (int.TryParse(content.Substring(0, 3), out var result))
			{
				return new OperateResult<string>(result, content);
			}
			return new OperateResult<string>(content);
		}
		return OperateResult.CreateSuccessResult(content);
	}

	private OperateResult<string> SendCommandAndCheckAuth(PipeTcpNet pipe, string command)
	{
		OperateResult operateResult = SendHelper(pipe, command);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult<byte[]> operateResult2 = ReceiveHelper(pipe);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		return CheckResultContent(Encoding.UTF8.GetString(operateResult2.Content), pipe);
	}

	/// <summary>
	/// 浏览指定目录下的文件及文件夹信息列表，默认根目录为 /
	/// Browse the list of file and folder information in the specified directory. The default root directory is /
	/// </summary>
	/// <param name="parent">指定的目录</param>
	/// <returns>文件列表信息</returns>
	public OperateResult<FtpFileItem[]> ListFiles(string parent = "/")
	{
		if (!parent.StartsWith("/"))
		{
			parent = "/" + parent;
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		OperateResult<FtpFileItem[]> operateResult2 = ListHelper(content, parent);
		if (operateResult2.IsSuccess)
		{
			CloseFtpClient(content);
		}
		return operateResult2;
	}

	private bool PipeConnectServer(FtpFileResult result)
	{
		PipeTcpNet pipeTcpNet = result.PipeTcpNet;
		if (pasvUseIp)
		{
			pipeTcpNet.IpAddress = IpAddress;
		}
		OperateResult operateResult = pipeTcpNet.OpenCommunication();
		if (!operateResult.IsSuccess)
		{
			pipeTcpNet.CloseCommunication();
			result.IsCompleted = true;
			result.ErrorMessage = $"{pipeTcpNet} connect failed: " + operateResult.Message;
			return false;
		}
		return true;
	}

	private void ThreadPool_ThreadWork(object obj)
	{
		if (!(obj is FtpFileResult { PipeTcpNet: var pipeTcpNet } ftpFileResult) || !PipeConnectServer(ftpFileResult))
		{
			return;
		}
		ftpFileResult.Files = new List<FtpFileItem>();
		while (true)
		{
			OperateResult<byte[]> operateResult = pipeTcpNet.ReceiveMessage(new SpecifiedCharacterMessage(13, 10), null);
			if (!operateResult.IsSuccess)
			{
				ftpFileResult.IsCompleted = true;
				pipeTcpNet.CloseCommunication();
				return;
			}
			string message = Encoding.UTF8.GetString(operateResult.Content).Trim();
			if (string.IsNullOrEmpty(message))
			{
				break;
			}
			try
			{
				FtpFileItem item = new FtpFileItem(message);
				ftpFileResult.Files.Add(item);
			}
			catch (Exception ex)
			{
				ftpFileResult.IsCompleted = true;
				pipeTcpNet.CloseCommunication();
				ftpFileResult.ErrorMessage = "Parse line failed: " + ex.Message + "\r\nSource Line: " + message;
				return;
			}
		}
		ftpFileResult.IsCompleted = true;
		pipeTcpNet.CloseCommunication();
	}

	/// <summary>
	/// 从FTP服务器下载文件到本地文件，支持进度汇报和取消操作
	/// Download files from an FTP server to local files, with support for progress reporting and cancellation operations.
	/// </summary>
	/// <param name="remoteFile">服务器的文件，例如 /A/123.txt</param>
	/// <param name="localFile">本地保存的文件名</param>
	/// <param name="processReport">进度报告</param>
	/// <param name="cancelToken">取消操作</param>
	/// <returns>是否下载成功</returns>
	public OperateResult DownloadFile(string remoteFile, string localFile, Action<long, long> processReport = null, HslCancelToken cancelToken = null)
	{
		if (!remoteFile.StartsWith("/"))
		{
			remoteFile = "/" + remoteFile;
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		OperateResult<string> operateResult2 = SendCommandAndCheckAuth(content, "SIZE " + remoteFile);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		long size = long.Parse(operateResult2.Content.Split(' ')[1].Trim());
		OperateResult<PipeTcpNet> operateResult3 = PasvCommand(content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult3);
		}
		PipeTcpNet content2 = operateResult3.Content;
		FtpFileResult ftpFileResult = new FtpFileResult
		{
			PipeTcpNet = content2,
			Files = new List<FtpFileItem>
			{
				new FtpFileItem
				{
					Name = localFile,
					Size = size
				}
			}
		};
		ftpFileResult.ProcessReport = processReport;
		ftpFileResult.CancelToken = cancelToken;
		if (serverInfo == 1)
		{
			ThreadPool.QueueUserWorkItem(ThreadPool_DownloadFile, ftpFileResult);
		}
		OperateResult<string> operateResult4 = SendCommandAndCheckAuth(content, "RETR " + remoteFile);
		if (!operateResult4.IsSuccess)
		{
			content2.CloseCommunication();
			return OperateResult.CreateFailedResult<string>(operateResult4);
		}
		if (serverInfo != 1)
		{
			ThreadPool.QueueUserWorkItem(ThreadPool_DownloadFile, ftpFileResult);
		}
		do
		{
			HslHelper.ThreadSleep(20);
		}
		while (!ftpFileResult.IsCompleted);
		HslHelper.ThreadSleep(20);
		if (!string.IsNullOrEmpty(ftpFileResult.ErrorMessage))
		{
			content2.CloseCommunication();
			CloseFtpClient(content);
			return new OperateResult<FtpFileItem[]>(ftpFileResult.ErrorMessage);
		}
		OperateResult<byte[]> operateResult5 = ReceiveHelper(content);
		if (!operateResult5.IsSuccess)
		{
			content2.CloseCommunication();
			return OperateResult.CreateFailedResult<string>(operateResult5);
		}
		CloseFtpClient(content);
		return CheckResultContent(Encoding.UTF8.GetString(operateResult5.Content), null);
	}

	private void ThreadPool_DownloadFile(object obj)
	{
		if (!(obj is FtpFileResult { PipeTcpNet: var pipeTcpNet } ftpFileResult) || !PipeConnectServer(ftpFileResult))
		{
			return;
		}
		long num = 0L;
		byte[] buffer = new byte[4096];
		try
		{
			using FileStream fileStream = new FileStream(ftpFileResult.Files[0].Name, FileMode.Create, FileAccess.Write);
			do
			{
				int length = Math.Min(buffer.Length, (int)(ftpFileResult.Files[0].Size - num));
				OperateResult<int> operateResult = pipeTcpNet.Receive(buffer, 0, length);
				if (!operateResult.IsSuccess)
				{
					ftpFileResult.IsCompleted = true;
					pipeTcpNet.CloseCommunication();
					ftpFileResult.ErrorMessage = $"Receive file[{ftpFileResult.Files[0].Size}] failed: " + operateResult.Message;
					return;
				}
				if (operateResult.Content == 0)
				{
					fileStream.Flush();
					ftpFileResult.IsCompleted = true;
					pipeTcpNet.CloseCommunication();
					return;
				}
				fileStream.Write(buffer, 0, operateResult.Content);
				num += operateResult.Content;
				if (ftpFileResult.CancelToken != null && ftpFileResult.CancelToken.IsCancelled)
				{
					ftpFileResult.IsCompleted = true;
					pipeTcpNet.CloseCommunication();
					ftpFileResult.ErrorMessage = "User Cancelled";
					return;
				}
				ftpFileResult.ProcessReport?.Invoke(num, ftpFileResult.Files[0].Size);
			}
			while (num < ftpFileResult.Files[0].Size);
			fileStream.Flush();
			ftpFileResult.IsCompleted = true;
			pipeTcpNet.CloseCommunication();
		}
		catch (Exception ex)
		{
			ftpFileResult.IsCompleted = true;
			pipeTcpNet.CloseCommunication();
			ftpFileResult.ErrorMessage = ex.Message;
		}
	}

	/// <summary>
	/// 上传本地文件到FTP服务器，支持进度汇报和取消操作
	/// Upload local files to an FTP server, with support for progress reporting and cancellation operations.
	/// </summary>
	/// <param name="localFile">本地的文件</param>
	/// <param name="remoteFile">服务器保存的路径，例如 /A/123.txt</param>
	/// <param name="processReport">上传的进度</param>
	/// <param name="cancelToken">取消操作对象</param>
	/// <returns>是否上传成功</returns>
	public OperateResult UploadFile(string localFile, string remoteFile, Action<long, long> processReport = null, HslCancelToken cancelToken = null)
	{
		if (!remoteFile.StartsWith("/"))
		{
			remoteFile = "/" + remoteFile;
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		string text = remoteFile.Substring(0, remoteFile.LastIndexOf('/'));
		remoteFile = remoteFile.Substring(remoteFile.LastIndexOf('/') + 1);
		OperateResult<string> operateResult2 = SendCommandAndCheckAuth(content, "CWD " + text);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		OperateResult<PipeTcpNet> operateResult3 = PasvCommand(content);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult3);
		}
		PipeTcpNet content2 = operateResult3.Content;
		FtpFileResult ftpFileResult = new FtpFileResult
		{
			PipeTcpNet = content2,
			Files = new List<FtpFileItem>
			{
				new FtpFileItem
				{
					Name = localFile
				}
			}
		};
		if (serverInfo == 1)
		{
			ThreadPool.QueueUserWorkItem(ThreadPool_UploadFile, ftpFileResult);
		}
		OperateResult<string> operateResult4 = SendCommandAndCheckAuth(content, "STOR " + remoteFile);
		if (!operateResult4.IsSuccess)
		{
			content2.CloseCommunication();
			return OperateResult.CreateFailedResult<string>(operateResult4);
		}
		if (serverInfo != 1)
		{
			ThreadPool.QueueUserWorkItem(ThreadPool_UploadFile, ftpFileResult);
		}
		do
		{
			HslHelper.ThreadSleep(20);
		}
		while (!ftpFileResult.IsCompleted);
		HslHelper.ThreadSleep(20);
		OperateResult<byte[]> operateResult5 = ReceiveHelper(content);
		if (!operateResult5.IsSuccess)
		{
			content2.CloseCommunication();
			return OperateResult.CreateFailedResult<string>(operateResult5);
		}
		CloseFtpClient(content);
		return CheckResultContent(Encoding.UTF8.GetString(operateResult5.Content), null);
	}

	private void ThreadPool_UploadFile(object obj)
	{
		if (!(obj is FtpFileResult { PipeTcpNet: var pipeTcpNet } ftpFileResult) || !PipeConnectServer(ftpFileResult))
		{
			return;
		}
		long num = 0L;
		byte[] buffer = new byte[4096];
		try
		{
			using FileStream fileStream = new FileStream(ftpFileResult.Files[0].Name, FileMode.Open, FileAccess.Read);
			while (true)
			{
				int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
				if (bytesRead == 0)
				{
					ftpFileResult.IsCompleted = true;
					pipeTcpNet.CloseCommunication();
					return;
				}
				OperateResult operateResult = pipeTcpNet.Send(buffer, 0, bytesRead);
				if (!operateResult.IsSuccess)
				{
					ftpFileResult.IsCompleted = true;
					pipeTcpNet.CloseCommunication();
					ftpFileResult.ErrorMessage = $"Upload file[{ftpFileResult.Files[0].Size}] failed: " + operateResult.Message;
					return;
				}
				num += bytesRead;
				if (ftpFileResult.CancelToken != null && ftpFileResult.CancelToken.IsCancelled)
				{
					break;
				}
				ftpFileResult.ProcessReport?.Invoke(num, ftpFileResult.Files[0].Size);
			}
			ftpFileResult.IsCompleted = true;
			pipeTcpNet.CloseCommunication();
			ftpFileResult.ErrorMessage = "User Cancelled";
		}
		catch (Exception ex)
		{
			ftpFileResult.IsCompleted = true;
			pipeTcpNet.CloseCommunication();
			ftpFileResult.ErrorMessage = ex.Message;
		}
	}

	/// <summary>
	/// 删除FTP服务器上的指定文件
	/// Delete the specified file(s) on the FTP server.
	/// </summary>
	/// <param name="remoteFile">服务器上的文件，例如 /A/123.txt</param>
	/// <returns>是否删除成功</returns>
	public OperateResult DeleteFile(string remoteFile)
	{
		if (!remoteFile.StartsWith("/"))
		{
			remoteFile = "/" + remoteFile;
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		OperateResult<string> operateResult2 = SendCommandAndCheckAuth(content, "DELE " + remoteFile);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		CloseFtpClient(content);
		return OperateResult.CreateSuccessResult();
	}

	private OperateResult<FtpFileItem[]> ListHelper(PipeTcpNet pipeTcpNet, string path)
	{
		OperateResult operateResult = SendCommandAndCheckAuth(pipeTcpNet, "CWD " + path);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult);
		}
		if (serverInfo == 1)
		{
			OperateResult operateResult2 = SendCommandAndCheckAuth(pipeTcpNet, "PWD");
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult2);
			}
		}
		OperateResult<PipeTcpNet> operateResult3 = PasvCommand(pipeTcpNet);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult3);
		}
		PipeTcpNet content = operateResult3.Content;
		FtpFileResult ftpFileResult = new FtpFileResult
		{
			PipeTcpNet = content
		};
		if (serverInfo == 1)
		{
			ThreadPool.QueueUserWorkItem(ThreadPool_ThreadWork, ftpFileResult);
		}
		OperateResult<string> operateResult4 = SendCommandAndCheckAuth(pipeTcpNet, (serverInfo == 0) ? "LIST -l" : "MLSD");
		if (!operateResult4.IsSuccess)
		{
			content.CloseCommunication();
			return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult4);
		}
		if (operateResult4.Content.StartsWith("425"))
		{
			return new OperateResult<FtpFileItem[]>(425, operateResult4.Content);
		}
		if (serverInfo == 0)
		{
			ThreadPool.QueueUserWorkItem(ThreadPool_ThreadWork, ftpFileResult);
		}
		OperateResult<byte[]> operateResult5 = ReceiveHelper(pipeTcpNet);
		if (!operateResult5.IsSuccess)
		{
			content.CloseCommunication();
			return OperateResult.CreateFailedResult<FtpFileItem[]>(operateResult5);
		}
		do
		{
			HslHelper.ThreadSleep(20);
		}
		while (!ftpFileResult.IsCompleted);
		if (!string.IsNullOrEmpty(ftpFileResult.ErrorMessage))
		{
			return new OperateResult<FtpFileItem[]>(ftpFileResult.ErrorMessage);
		}
		return OperateResult.CreateSuccessResult(ftpFileResult.Files.ToArray());
	}

	private OperateResult DeleteDirectoryHelper(PipeTcpNet pipe, string path, bool clearFiles)
	{
		if (clearFiles)
		{
			OperateResult<FtpFileItem[]> operateResult = ListHelper(pipe, path);
			if (!operateResult.IsSuccess)
			{
				return OperateResult.CreateFailedResult<string>(operateResult);
			}
			FtpFileItem[] content = operateResult.Content;
			foreach (FtpFileItem ftpFileItem in content)
			{
				if (ftpFileItem.IsDirectory)
				{
					OperateResult operateResult2 = DeleteDirectoryHelper(pipe, path + "/" + ftpFileItem.Name, clearFiles: true);
					if (!operateResult2.IsSuccess)
					{
						return OperateResult.CreateFailedResult<string>(operateResult2);
					}
				}
				else
				{
					OperateResult<string> operateResult3 = SendCommandAndCheckAuth(pipe, "DELE " + path + "/" + ftpFileItem.Name);
					if (!operateResult3.IsSuccess)
					{
						return OperateResult.CreateFailedResult<string>(operateResult3);
					}
				}
			}
		}
		OperateResult<string> operateResult4 = SendCommandAndCheckAuth(pipe, "RMD " + path);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult4);
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 删除FTP服务器上的指定目录，并且可以指定是否删除所有子文件
	/// Delete the specified directory on the FTP server, with the option to specify whether to delete all subfiles.
	/// </summary>
	/// <param name="path">Ftp服务器上的目录</param>
	/// <param name="clearFiles">是否删除所有的文件</param>
	/// <returns>是否删除成功</returns>
	public OperateResult DeleteDirectory(string path, bool clearFiles = false)
	{
		if (!path.StartsWith("/"))
		{
			path = "/" + path;
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		OperateResult operateResult2 = DeleteDirectoryHelper(content, path, clearFiles);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		CloseFtpClient(content);
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 在FTP服务器上创建指定的目录
	/// Create the specified directory on the FTP server.
	/// </summary>
	/// <param name="path">指定的目录</param>
	/// <returns>是否创建目录成功</returns>
	public OperateResult CreateDirectory(string path)
	{
		if (!path.StartsWith("/"))
		{
			path = "/" + path;
		}
		if (path.Length > 2 && path.EndsWith("/"))
		{
			path = path.Substring(0, path.Length - 1);
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		string value = "/";
		int num = path.LastIndexOf(value);
		if (num > 0)
		{
			value = path.Substring(0, num);
			path = path.Substring(num + 1);
			OperateResult<string> operateResult2 = SendCommandAndCheckAuth(content, "CWD " + value);
			if (!operateResult2.IsSuccess)
			{
				return OperateResult.CreateFailedResult<string>(operateResult2);
			}
		}
		OperateResult<string> operateResult3 = SendCommandAndCheckAuth(content, "MKD " + path);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult3);
		}
		CloseFtpClient(content);
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 重命名目录，不支持跨目录重命名
	/// Rename a directory; cross-directory renaming is not supported.
	/// </summary>
	/// <param name="oldPath">例如  /A/B</param>
	/// <param name="newPath">例如 C</param>
	/// <returns>是否重命名成功</returns>
	public OperateResult RenameFolder(string oldPath, string newPath)
	{
		if (string.IsNullOrEmpty(oldPath) || oldPath == "/")
		{
			return new OperateResult("Old path invalid. can not be empty or '/'");
		}
		if (string.IsNullOrEmpty(newPath) || newPath == "/")
		{
			return new OperateResult("New path invalid. can not be empty or '/'");
		}
		if (oldPath.EndsWith("/"))
		{
			oldPath = oldPath.Substring(0, oldPath.Length - 1);
		}
		if (newPath.EndsWith("/"))
		{
			newPath = newPath.Substring(0, newPath.Length - 1);
		}
		if (oldPath.StartsWith("/"))
		{
			oldPath = oldPath.Substring(1);
		}
		if (newPath.StartsWith("/"))
		{
			newPath = newPath.Substring(1);
		}
		string text = "/";
		if (oldPath.Contains("/"))
		{
			text = oldPath.Substring(0, oldPath.LastIndexOf('/'));
			oldPath = oldPath.Substring(oldPath.LastIndexOf('/') + 1);
		}
		if (newPath.Contains("/"))
		{
			string text2 = newPath.Substring(0, newPath.LastIndexOf('/'));
			if (text2 != text)
			{
				return new OperateResult("Can't rename to different folder.");
			}
			newPath = newPath.Substring(newPath.LastIndexOf('/') + 1);
		}
		OperateResult<PipeTcpNet> operateResult = CreatePipe();
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		PipeTcpNet content = operateResult.Content;
		OperateResult<string> operateResult2 = SendCommandAndCheckAuth(content, "CWD " + text);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult2);
		}
		OperateResult<string> operateResult3 = SendCommandAndCheckAuth(content, "RNFR " + oldPath);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult3);
		}
		OperateResult<string> operateResult4 = SendCommandAndCheckAuth(content, "RNTO " + newPath);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult4);
		}
		CloseFtpClient(content);
		return OperateResult.CreateSuccessResult();
	}

	/// <summary>
	/// 重命名文件的名称
	/// Rename the name of the file.
	/// </summary>
	/// <param name="oldName">旧的文件名，例如 /A/123.txt</param>
	/// <param name="newName">新文件名，例如 124.txt</param>
	/// <returns>是否重命名成功</returns>
	public OperateResult RenameFile(string oldName, string newName)
	{
		return RenameFolder(oldName, newName);
	}

	public override string ToString()
	{
		return $"FtpClient[{IpAddress}:{Port}]";
	}
}

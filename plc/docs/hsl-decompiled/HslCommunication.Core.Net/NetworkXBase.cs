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
using System.Net.Sockets;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using Newtonsoft.Json.Linq;

namespace HslCommunication.Core.Net;

/// <summary>
/// 包含了主动异步接收的方法实现和文件类异步读写的实现
/// Contains the implementation of the active asynchronous receiving method and the implementation of asynchronous reading and writing of the file class
/// </summary>
public class NetworkXBase : NetworkBase
{
	/// <summary>
	/// 对客户端而言是的通讯用的套接字，对服务器来说是用于侦听的套接字
	/// A communication socket for the client, or a listening socket for the server
	/// </summary>
	protected Socket CoreSocket = null;

	/// <summary>
	/// [自校验] 将文件数据发送至套接字，如果结果异常，则结束通讯
	/// [Self-check] Send the file data to the socket. If the result is abnormal, the communication is ended.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="filename">完整的文件路径</param>
	/// <param name="filelength">文件的长度</param>
	/// <param name="report">进度报告器</param>
	/// <returns>是否发送成功</returns>
	protected OperateResult SendFileStreamToSocket(Socket socket, string filename, long filelength, Action<long, long> report = null)
	{
		try
		{
			OperateResult result = new OperateResult();
			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				result = SendStreamToSocket(socket, stream, filelength, report, reportByPercent: true);
			}
			return result;
		}
		catch (Exception ex)
		{
			socket?.Close();
			base.LogNet?.WriteException(ToString(), ex);
			return new OperateResult(ex.Message);
		}
	}

	/// <summary>
	/// [自校验] 将文件数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
	/// [Self-checking] Send the file data to the socket. The specific sending details will be implemented in the inherited class. If the result is abnormal, the communication will end
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <param name="filename">文件名称，文件必须存在</param>
	/// <param name="servername">远程端的文件名称</param>
	/// <param name="filetag">文件的额外标签</param>
	/// <param name="fileupload">文件的上传人</param>
	/// <param name="sendReport">发送进度报告</param>
	/// <returns>是否发送成功</returns>
	protected OperateResult SendFileAndCheckReceive(Socket socket, string filename, string servername, string filetag, string fileupload, Action<long, long> sendReport = null)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b5: Expected O, but got Unknown
		FileInfo fileInfo = new FileInfo(filename);
		if (!File.Exists(filename))
		{
			OperateResult operateResult = SendStringAndCheckReceive(socket, 0, "");
			if (!operateResult.IsSuccess)
			{
				return operateResult;
			}
			socket?.Close();
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		JObject val = new JObject();
		val.Add("FileName", (JToken)new JValue(servername));
		val.Add("FileSize", (JToken)new JValue(fileInfo.Length));
		val.Add("FileTag", (JToken)new JValue(filetag));
		val.Add("FileUpload", (JToken)new JValue(fileupload));
		JObject val2 = val;
		OperateResult operateResult2 = SendStringAndCheckReceive(socket, 1, ((object)val2).ToString());
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return SendFileStreamToSocket(socket, filename, fileInfo.Length, sendReport);
	}

	/// <summary>
	/// [自校验] 将流数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
	/// [Self-checking] Send stream data to the socket. The specific sending details will be implemented in the inherited class.
	/// If the result is abnormal, the communication will be terminated
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <param name="stream">文件名称，文件必须存在</param>
	/// <param name="servername">远程端的文件名称</param>
	/// <param name="filetag">文件的额外标签</param>
	/// <param name="fileupload">文件的上传人</param>
	/// <param name="sendReport">发送进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	protected OperateResult SendFileAndCheckReceive(Socket socket, Stream stream, string servername, string filetag, string fileupload, Action<long, long> sendReport = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0056: Expected O, but got Unknown
		JObject val = new JObject();
		val.Add("FileName", (JToken)new JValue(servername));
		val.Add("FileSize", (JToken)new JValue(stream.Length));
		val.Add("FileTag", (JToken)new JValue(filetag));
		val.Add("FileUpload", (JToken)new JValue(fileupload));
		JObject val2 = val;
		OperateResult operateResult = SendStringAndCheckReceive(socket, 1, ((object)val2).ToString());
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return SendStreamToSocket(socket, stream, stream.Length, sendReport, reportByPercent: true);
	}

	/// <summary>
	/// [自校验] 从套接字中接收文件头信息
	/// [Self-checking] Receive file header information from socket
	/// </summary>
	/// <param name="socket">套接字的网络</param>
	/// <returns>包含文件信息的结果对象</returns>
	protected OperateResult<FileBaseInfo> ReceiveFileHeadFromSocket(Socket socket)
	{
		OperateResult<int, string> operateResult = ReceiveStringContentFromSocket(socket);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileBaseInfo>(operateResult);
		}
		if (operateResult.Content1 == 0)
		{
			socket?.Close();
			base.LogNet?.WriteWarn(ToString(), StringResources.Language.FileRemoteNotExist);
			return new OperateResult<FileBaseInfo>(StringResources.Language.FileNotExist);
		}
		OperateResult<FileBaseInfo> operateResult2 = new OperateResult<FileBaseInfo>
		{
			Content = new FileBaseInfo()
		};
		try
		{
			JObject json = JObject.Parse(operateResult.Content2);
			operateResult2.Content.Name = SoftBasic.GetValueFromJsonObject(json, "FileName", "");
			operateResult2.Content.Size = SoftBasic.GetValueFromJsonObject(json, "FileSize", 0L);
			operateResult2.Content.Tag = SoftBasic.GetValueFromJsonObject(json, "FileTag", "");
			operateResult2.Content.Upload = SoftBasic.GetValueFromJsonObject(json, "FileUpload", "");
			operateResult2.IsSuccess = true;
		}
		catch (Exception ex)
		{
			socket?.Close();
			operateResult2.Message = "Extra File Head Wrong:" + ex.Message;
		}
		return operateResult2;
	}

	/// <summary>
	/// [自校验] 从网络中接收一个文件，如果结果异常，则结束通讯
	/// [Self-checking] Receive a file from the network. If the result is abnormal, the communication ends.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="savename">接收文件后保存的文件名</param>
	/// <param name="receiveReport">接收进度报告</param>
	/// <returns>包含文件信息的结果对象</returns>
	protected OperateResult<FileBaseInfo> ReceiveFileFromSocket(Socket socket, string savename, Action<long, long> receiveReport)
	{
		OperateResult<FileBaseInfo> operateResult = ReceiveFileHeadFromSocket(socket);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		try
		{
			OperateResult operateResult2 = null;
			using (FileStream stream = new FileStream(savename, FileMode.Create, FileAccess.Write))
			{
				operateResult2 = WriteStreamFromSocket(socket, stream, operateResult.Content.Size, receiveReport, reportByPercent: true);
			}
			if (!operateResult2.IsSuccess)
			{
				if (File.Exists(savename))
				{
					File.Delete(savename);
				}
				return OperateResult.CreateFailedResult<FileBaseInfo>(operateResult2);
			}
			return operateResult;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), ex);
			socket?.Close();
			return new OperateResult<FileBaseInfo>
			{
				Message = ex.Message
			};
		}
	}

	/// <summary>
	/// [自校验] 从网络中接收一个文件，写入数据流，如果结果异常，则结束通讯，参数顺序文件名，文件大小，文件标识，上传人
	/// [Self-checking] Receive a file from the network. If the result is abnormal, the communication ends.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="stream">等待写入的数据流</param>
	/// <param name="receiveReport">接收进度报告</param>
	/// <returns>文件头结果</returns>
	protected OperateResult<FileBaseInfo> ReceiveFileFromSocket(Socket socket, Stream stream, Action<long, long> receiveReport)
	{
		OperateResult<FileBaseInfo> operateResult = ReceiveFileHeadFromSocket(socket);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		try
		{
			WriteStreamFromSocket(socket, stream, operateResult.Content.Size, receiveReport, reportByPercent: true);
			return operateResult;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), ex);
			socket?.Close();
			return new OperateResult<FileBaseInfo>
			{
				Message = ex.Message
			};
		}
	}

	/// <summary>
	/// [自校验] 将文件数据发送至套接字，如果结果异常，则结束通讯
	/// [Self-check] Send the file data to the socket. If the result is abnormal, the communication is ended.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="filename">完整的文件路径</param>
	/// <param name="filelength">文件的长度</param>
	/// <param name="report">进度报告器</param>
	/// <returns>是否发送成功</returns>
	protected async Task<OperateResult> SendFileStreamToSocketAsync(Socket socket, string filename, long filelength, Action<long, long> report = null)
	{
		try
		{
			OperateResult result = new OperateResult();
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				result = await SendStreamToSocketAsync(socket, fs, filelength, report, reportByPercent: true);
			}
			return result;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			socket?.Close();
			base.LogNet?.WriteException(ToString(), ex2);
			return new OperateResult(ex2.Message);
		}
	}

	/// <summary>
	/// [自校验] 将文件数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
	/// [Self-checking] Send the file data to the socket. The specific sending details will be implemented in the inherited class. If the result is abnormal, the communication will end
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <param name="filename">文件名称，文件必须存在</param>
	/// <param name="servername">远程端的文件名称</param>
	/// <param name="filetag">文件的额外标签</param>
	/// <param name="fileupload">文件的上传人</param>
	/// <param name="sendReport">发送进度报告</param>
	/// <returns>是否发送成功</returns>
	protected async Task<OperateResult> SendFileAndCheckReceiveAsync(Socket socket, string filename, string servername, string filetag, string fileupload, Action<long, long> sendReport = null)
	{
		FileInfo info = new FileInfo(filename);
		if (!File.Exists(filename))
		{
			OperateResult stringResult = await SendStringAndCheckReceiveAsync(socket, 0, "");
			if (!stringResult.IsSuccess)
			{
				return stringResult;
			}
			socket?.Close();
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		JObject val = new JObject();
		val.Add("FileName", (JToken)new JValue(servername));
		val.Add("FileSize", (JToken)new JValue(info.Length));
		val.Add("FileTag", (JToken)new JValue(filetag));
		val.Add("FileUpload", (JToken)new JValue(fileupload));
		JObject json = val;
		OperateResult sendResult = await SendStringAndCheckReceiveAsync(socket, 1, ((object)json).ToString());
		if (!sendResult.IsSuccess)
		{
			return sendResult;
		}
		return await SendFileStreamToSocketAsync(socket, filename, info.Length, sendReport);
	}

	/// <summary>
	/// [自校验] 将流数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯
	/// [Self-checking] Send stream data to the socket. The specific sending details will be implemented in the inherited class.
	/// If the result is abnormal, the communication will be terminated
	/// </summary>
	/// <param name="socket">套接字</param>
	/// <param name="stream">文件名称，文件必须存在</param>
	/// <param name="servername">远程端的文件名称</param>
	/// <param name="filetag">文件的额外标签</param>
	/// <param name="fileupload">文件的上传人</param>
	/// <param name="sendReport">发送进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	protected async Task<OperateResult> SendFileAndCheckReceiveAsync(Socket socket, Stream stream, string servername, string filetag, string fileupload, Action<long, long> sendReport = null)
	{
		JObject val = new JObject();
		val.Add("FileName", (JToken)new JValue(servername));
		val.Add("FileSize", (JToken)new JValue(stream.Length));
		val.Add("FileTag", (JToken)new JValue(filetag));
		val.Add("FileUpload", (JToken)new JValue(fileupload));
		JObject json = val;
		OperateResult fileResult = await SendStringAndCheckReceiveAsync(socket, 1, ((object)json).ToString());
		if (!fileResult.IsSuccess)
		{
			return fileResult;
		}
		return await SendStreamToSocketAsync(socket, stream, stream.Length, sendReport, reportByPercent: true);
	}

	/// <summary>
	/// [自校验] 从套接字中接收文件头信息
	/// [Self-checking] Receive file header information from socket
	/// </summary>
	/// <param name="socket">套接字的网络</param>
	/// <returns>包含文件信息的结果对象</returns>
	protected async Task<OperateResult<FileBaseInfo>> ReceiveFileHeadFromSocketAsync(Socket socket)
	{
		OperateResult<int, string> receiveString = await ReceiveStringContentFromSocketAsync(socket);
		if (!receiveString.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileBaseInfo>(receiveString);
		}
		if (receiveString.Content1 == 0)
		{
			socket?.Close();
			base.LogNet?.WriteWarn(ToString(), StringResources.Language.FileRemoteNotExist);
			return new OperateResult<FileBaseInfo>(StringResources.Language.FileNotExist);
		}
		OperateResult<FileBaseInfo> result = new OperateResult<FileBaseInfo>
		{
			Content = new FileBaseInfo()
		};
		try
		{
			JObject json = JObject.Parse(receiveString.Content2);
			result.Content.Name = SoftBasic.GetValueFromJsonObject(json, "FileName", "");
			result.Content.Size = SoftBasic.GetValueFromJsonObject(json, "FileSize", 0L);
			result.Content.Tag = SoftBasic.GetValueFromJsonObject(json, "FileTag", "");
			result.Content.Upload = SoftBasic.GetValueFromJsonObject(json, "FileUpload", "");
			result.IsSuccess = true;
		}
		catch (Exception ex)
		{
			socket?.Close();
			result.Message = "Extra File Head Wrong:" + ex.Message;
		}
		return result;
	}

	/// <summary>
	/// [自校验] 从网络中接收一个文件，如果结果异常，则结束通讯
	/// [Self-checking] Receive a file from the network. If the result is abnormal, the communication ends.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="savename">接收文件后保存的文件名</param>
	/// <param name="receiveReport">接收进度报告</param>
	/// <returns>包含文件信息的结果对象</returns>
	protected async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAsync(Socket socket, string savename, Action<long, long> receiveReport)
	{
		OperateResult<FileBaseInfo> fileResult = await ReceiveFileHeadFromSocketAsync(socket);
		if (!fileResult.IsSuccess)
		{
			return fileResult;
		}
		try
		{
			OperateResult write = null;
			using (FileStream fs = new FileStream(savename, FileMode.Create, FileAccess.Write))
			{
				write = await WriteStreamFromSocketAsync(socket, fs, fileResult.Content.Size, receiveReport, reportByPercent: true);
			}
			if (!write.IsSuccess)
			{
				if (File.Exists(savename))
				{
					File.Delete(savename);
				}
				return OperateResult.CreateFailedResult<FileBaseInfo>(write);
			}
			return fileResult;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), ex);
			socket?.Close();
			return new OperateResult<FileBaseInfo>(ex.Message);
		}
	}

	/// <summary>
	/// [自校验] 从网络中接收一个文件，写入数据流，如果结果异常，则结束通讯，参数顺序文件名，文件大小，文件标识，上传人
	/// [Self-checking] Receive a file from the network. If the result is abnormal, the communication ends.
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="stream">等待写入的数据流</param>
	/// <param name="receiveReport">接收进度报告</param>
	/// <returns>文件头结果</returns>
	protected async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAsync(Socket socket, Stream stream, Action<long, long> receiveReport)
	{
		OperateResult<FileBaseInfo> fileResult = await ReceiveFileHeadFromSocketAsync(socket);
		if (!fileResult.IsSuccess)
		{
			return fileResult;
		}
		try
		{
			await WriteStreamFromSocketAsync(socket, stream, fileResult.Content.Size, receiveReport, reportByPercent: true);
			return fileResult;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), ex);
			socket?.Close();
			return new OperateResult<FileBaseInfo>(ex.Message);
		}
	}

	public override string ToString()
	{
		return "NetworkXBase";
	}
}

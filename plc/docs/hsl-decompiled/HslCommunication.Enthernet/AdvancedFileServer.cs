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
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using Newtonsoft.Json.Linq;

namespace HslCommunication.Enthernet;

/// <summary>
/// 文件管理类服务器，负责服务器所有分类文件的管理，特点是不支持文件附加数据，但是支持直接访问文件名
/// </summary>
/// <remarks>
/// 本文件的服务器不支持存储文件携带的额外信息，是直接将文件存放在服务器指定目录下的，文件名不更改，特点是服务器查看方便。
/// </remarks>
public class AdvancedFileServer : NetworkFileServerBase
{
	private string m_FilesDirectoryPathTemp = null;

	/// <summary>
	/// 用于接收上传文件时的临时文件夹，临时文件使用结束后会被删除
	/// Used to receive the temporary folder when uploading files. The temporary files will be deleted after use
	/// </summary>
	public string FilesDirectoryPathTemp
	{
		get
		{
			return m_FilesDirectoryPathTemp;
		}
		set
		{
			m_FilesDirectoryPathTemp = value;
		}
	}

	protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
	{
		new OperateResult();
		string IpAddress = endPoint.Address.ToString();
		OperateResult<FileGroupInfo> infoResult = await ReceiveInformationHeadAsync(socket);
		if (!infoResult.IsSuccess)
		{
			return;
		}
		int customer = infoResult.Content.Command;
		string Factory = infoResult.Content.Factory;
		string Group = infoResult.Content.Group;
		string Identify = infoResult.Content.Identify;
		string fileName = infoResult.Content.FileName;
		string relativeName = GetRelativeFileName(Factory, Group, Identify, fileName);
		switch (customer)
		{
		case 2001:
		{
			string fullFileName3 = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
			OperateResult sendFile = await SendFileAndCheckReceiveAsync(socket, fullFileName3, fileName, "", "");
			if (!sendFile.IsSuccess)
			{
				base.LogNet?.WriteError(ToString(), StringResources.Language.FileDownloadFailed + ":" + relativeName + " ip:" + IpAddress + " reason：" + sendFile.Message);
			}
			else
			{
				socket?.Close();
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDownloadSuccess + ":" + relativeName);
			}
			break;
		}
		case 2002:
		{
			string tempFileName = Path.Combine(FilesDirectoryPathTemp, CreateRandomFileName());
			string fullFileName5 = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
			CheckFolderAndCreate();
			try
			{
				FileInfo info2 = new FileInfo(fullFileName5);
				if (!Directory.Exists(info2.DirectoryName))
				{
					Directory.CreateDirectory(info2.DirectoryName);
				}
			}
			catch (Exception ex)
			{
				Exception ex3 = ex;
				base.LogNet?.WriteException(ToString(), StringResources.Language.FilePathCreateFailed + fullFileName5, ex3);
				socket?.Close();
				break;
			}
			OperateResult<FileBaseInfo> receiveFile = await ReceiveFileFromSocketAndMoveFileAsync(socket, tempFileName, fullFileName5);
			if (receiveFile.IsSuccess)
			{
				socket?.Close();
				OnFileUpload(new FileServerInfo
				{
					ActualFileFullName = fullFileName5,
					Name = receiveFile.Content.Name,
					Size = receiveFile.Content.Size,
					Tag = receiveFile.Content.Tag,
					Upload = receiveFile.Content.Upload
				});
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileUploadSuccess + ":" + relativeName);
			}
			else
			{
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileUploadFailed + ":" + relativeName + " " + StringResources.Language.TextDescription + receiveFile.Message);
			}
			break;
		}
		case 2003:
		{
			string fullFileName4 = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
			bool deleteResult3 = DeleteFileByName(fullFileName4);
			if ((await SendStringAndCheckReceiveAsync(socket, deleteResult3 ? 1 : 0, deleteResult3 ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed)).IsSuccess)
			{
				socket?.Close();
			}
			if (deleteResult3)
			{
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteSuccess + ":" + relativeName);
			}
			break;
		}
		case 2011:
		{
			bool deleteResult = true;
			string[] fileNames = infoResult.Content.FileNames;
			foreach (string item in fileNames)
			{
				string fullFileName = ReturnAbsoluteFileName(Factory, Group, Identify, item);
				deleteResult = DeleteFileByName(fullFileName);
				if (deleteResult)
				{
					base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteSuccess + ":" + relativeName);
					continue;
				}
				deleteResult = false;
				break;
			}
			if ((await SendStringAndCheckReceiveAsync(socket, deleteResult ? 1 : 0, deleteResult ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed)).IsSuccess)
			{
				socket?.Close();
			}
			break;
		}
		case 2012:
		{
			string fullPath2 = ReturnAbsoluteFileName(Factory, Group, Identify, string.Empty);
			DirectoryInfo info3 = new DirectoryInfo(fullPath2);
			bool deleteResult4 = false;
			try
			{
				if (info3.Exists)
				{
					info3.Delete(recursive: true);
				}
				deleteResult4 = true;
			}
			catch (Exception ex)
			{
				Exception ex4 = ex;
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteFailed + " [" + fullPath2 + "] " + ex4.Message);
			}
			if ((await SendStringAndCheckReceiveAsync(socket, deleteResult4 ? 1 : 0, deleteResult4 ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed)).IsSuccess)
			{
				socket?.Close();
			}
			if (deleteResult4)
			{
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteSuccess + ":" + fullPath2);
			}
			break;
		}
		case 2014:
		{
			string fullPath = ReturnAbsoluteFileName(Factory, Group, Identify, string.Empty);
			DirectoryInfo info = new DirectoryInfo(fullPath);
			bool deleteResult2 = false;
			try
			{
				DirectoryInfo[] directories = info.GetDirectories();
				foreach (DirectoryInfo path in directories)
				{
					FileInfo[] files = path.GetFiles();
					if (files != null && files.Length == 0 && path.Exists)
					{
						path.Delete(recursive: true);
					}
				}
				deleteResult2 = true;
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteFailed + " [" + fullPath + "] " + ex2.Message);
			}
			if ((await SendStringAndCheckReceiveAsync(socket, deleteResult2 ? 1 : 0, deleteResult2 ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed)).IsSuccess)
			{
				socket?.Close();
			}
			if (deleteResult2)
			{
				base.LogNet?.WriteInfo(ToString(), StringResources.Language.FileDeleteSuccess + ":" + fullPath);
			}
			break;
		}
		case 2007:
		{
			List<GroupFileItem> fileNames2 = new List<GroupFileItem>();
			string[] directoryFiles = GetDirectoryFiles(Factory, Group, Identify);
			foreach (string m in directoryFiles)
			{
				FileInfo fileInfo = new FileInfo(m);
				fileNames2.Add(new GroupFileItem
				{
					FileName = fileInfo.Name,
					FileSize = fileInfo.Length
				});
			}
			JArray jArray = JArray.FromObject((object)fileNames2.ToArray());
			if ((await SendStringAndCheckReceiveAsync(socket, 2007, ((object)jArray).ToString())).IsSuccess)
			{
				socket?.Close();
			}
			break;
		}
		case 2008:
		{
			List<string> folders = new List<string>();
			string[] directories2 = GetDirectories(Factory, Group, Identify);
			foreach (string m2 in directories2)
			{
				DirectoryInfo directory = new DirectoryInfo(m2);
				folders.Add(directory.Name);
			}
			JArray jArray2 = JArray.FromObject((object)folders.ToArray());
			if ((await SendStringAndCheckReceiveAsync(socket, 2007, ((object)jArray2).ToString())).IsSuccess)
			{
				socket?.Close();
			}
			break;
		}
		case 2013:
		{
			string fullFileName2 = ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
			bool isExists = File.Exists(fullFileName2);
			if ((await SendStringAndCheckReceiveAsync(socket, isExists ? 1 : 0, StringResources.Language.FileNotExist)).IsSuccess)
			{
				socket?.Close();
			}
			break;
		}
		default:
			socket?.Close();
			break;
		}
	}

	protected override void StartInitialization()
	{
		if (string.IsNullOrEmpty(FilesDirectoryPathTemp))
		{
			throw new ArgumentNullException("FilesDirectoryPathTemp", "No saved path is specified");
		}
		base.StartInitialization();
	}

	protected override void CheckFolderAndCreate()
	{
		if (!Directory.Exists(FilesDirectoryPathTemp))
		{
			Directory.CreateDirectory(FilesDirectoryPathTemp);
		}
		base.CheckFolderAndCreate();
	}

	/// <summary>
	/// 从网络套接字接收文件并移动到目标的文件夹中，如果结果异常，则结束通讯
	/// </summary>
	private OperateResult<FileBaseInfo> ReceiveFileFromSocketAndMoveFile(Socket socket, string savename, string fileNameNew)
	{
		OperateResult<FileBaseInfo> operateResult = ReceiveFileFromSocket(socket, savename, null);
		if (!operateResult.IsSuccess)
		{
			DeleteFileByName(savename);
			return OperateResult.CreateFailedResult<FileBaseInfo>(operateResult);
		}
		int num = 0;
		int num2 = 0;
		while (num2 < 3)
		{
			num2++;
			if (MoveFileToNewFile(savename, fileNameNew))
			{
				num = 1;
				break;
			}
			HslHelper.ThreadSleep(500);
		}
		if (num == 0)
		{
			DeleteFileByName(savename);
		}
		OperateResult operateResult2 = SendStringAndCheckReceive(socket, num, "success");
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileBaseInfo>(operateResult2);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content);
	}

	/// <summary>
	/// 从网络套接字接收文件并移动到目标的文件夹中，如果结果异常，则结束通讯
	/// </summary>
	private async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAndMoveFileAsync(Socket socket, string savename, string fileNameNew)
	{
		OperateResult<FileBaseInfo> fileInfo = await ReceiveFileFromSocketAsync(socket, savename, null);
		if (!fileInfo.IsSuccess)
		{
			DeleteFileByName(savename);
			return OperateResult.CreateFailedResult<FileBaseInfo>(fileInfo);
		}
		int customer = 0;
		int times = 0;
		while (times < 3)
		{
			times++;
			if (MoveFileToNewFile(savename, fileNameNew))
			{
				customer = 1;
				break;
			}
			HslHelper.ThreadSleep(500);
		}
		if (customer == 0)
		{
			DeleteFileByName(savename);
		}
		OperateResult sendString = await SendStringAndCheckReceiveAsync(socket, customer, "success");
		if (!sendString.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileBaseInfo>(sendString);
		}
		return OperateResult.CreateSuccessResult(fileInfo.Content);
	}

	public override string ToString()
	{
		return $"AdvancedFileServer[{base.Port}]";
	}
}

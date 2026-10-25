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
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;

namespace HslCommunication.Core.Net;

/// <summary>
/// 文件服务器类的基类，为直接映射文件模式和间接映射文件模式提供基础的方法支持，主要包含了对文件的一些操作的功能
/// The base class of the file server class, which provides basic method support for the direct mapping file mode and the indirect mapping file mode, and mainly includes the functions of some operations on files
/// </summary>
public class NetworkFileServerBase : NetworkServerBase
{
	public delegate void FileUploadDelegate(FileServerInfo fileInfo);

	private readonly Dictionary<string, FileMarkId> dictionaryFilesMarks;

	private readonly object dictHybirdLock;

	private string m_FilesDirectoryPath = null;

	/// <summary>
	/// 文件所存储的路径
	/// </summary>
	public string FilesDirectoryPath
	{
		get
		{
			return m_FilesDirectoryPath;
		}
		set
		{
			m_FilesDirectoryPath = value;
		}
	}

	public int FileMarkIdCount => dictionaryFilesMarks.Count;

	/// <summary>
	/// 文件传输的时候的缓存大小，直接影响传输的速度，值越大，传输速度越快，越占内存，默认为100K大小
	/// The size of the cache during file transfer directly affects the speed of the transfer. The larger the value, the faster the transfer speed and the more memory it takes. The default size is 100K.
	/// </summary>
	public int FileCacheSize
	{
		get
		{
			return fileCacheSize;
		}
		set
		{
			fileCacheSize = value;
		}
	}

	/// <summary>
	/// 文件上传的事件，当文件上传的时候触发。
	/// </summary>
	public event FileUploadDelegate OnFileUploadEvent;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public NetworkFileServerBase()
	{
		dictionaryFilesMarks = new Dictionary<string, FileMarkId>(100);
		dictHybirdLock = new object();
	}

	/// <summary>
	/// 获取当前文件的读写锁，如果没有会自动创建，文件名应该是guid文件名，例如 b35a11ec533147ca80c7f7d1713f015b7909
	/// Acquire the read-write lock of the current file. If not, it will be created automatically.
	/// The file name should be the guid file name, for example, b35a11ec533147ca80c7f7d1713f015b7909
	/// </summary>
	/// <param name="fileName">完整的文件路径</param>
	/// <returns>返回携带文件信息的读写锁</returns>
	protected FileMarkId GetFileMarksFromDictionaryWithFileName(string fileName)
	{
		FileMarkId fileMarkId;
		lock (dictHybirdLock)
		{
			if (dictionaryFilesMarks.ContainsKey(fileName))
			{
				fileMarkId = dictionaryFilesMarks[fileName];
			}
			else
			{
				fileMarkId = new FileMarkId(base.LogNet, fileName);
				dictionaryFilesMarks.Add(fileName, fileMarkId);
			}
		}
		return fileMarkId;
	}

	/// <summary>
	/// 接收本次操作的信息头数据
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <returns>是否成功的结果对象</returns>
	protected OperateResult<FileGroupInfo> ReceiveInformationHead(Socket socket)
	{
		FileGroupInfo fileGroupInfo = new FileGroupInfo();
		OperateResult<byte[], byte[]> operateResult = ReceiveAndCheckBytes(socket, 30000);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(operateResult);
		}
		fileGroupInfo.Command = BitConverter.ToInt32(operateResult.Content1, 4);
		switch (BitConverter.ToInt32(operateResult.Content1, 0))
		{
		case 1001:
			fileGroupInfo.FileName = Encoding.Unicode.GetString(operateResult.Content2);
			break;
		case 1005:
			fileGroupInfo.FileNames = HslProtocol.UnPackStringArrayFromByte(operateResult.Content2);
			break;
		}
		OperateResult<int, string> operateResult2 = ReceiveStringContentFromSocket(socket);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(operateResult2);
		}
		fileGroupInfo.Factory = operateResult2.Content2;
		OperateResult<int, string> operateResult3 = ReceiveStringContentFromSocket(socket);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(operateResult3);
		}
		fileGroupInfo.Group = operateResult3.Content2;
		OperateResult<int, string> operateResult4 = ReceiveStringContentFromSocket(socket);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(operateResult4);
		}
		fileGroupInfo.Identify = operateResult4.Content2;
		return OperateResult.CreateSuccessResult(fileGroupInfo);
	}

	/// <summary>
	/// 接收本次操作的信息头数据
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <returns>是否成功的结果对象</returns>
	protected async Task<OperateResult<FileGroupInfo>> ReceiveInformationHeadAsync(Socket socket)
	{
		FileGroupInfo ret = new FileGroupInfo();
		OperateResult<byte[], byte[]> receive = await ReceiveAndCheckBytesAsync(socket, 30000);
		if (!receive.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(receive);
		}
		ret.Command = BitConverter.ToInt32(receive.Content1, 4);
		switch (BitConverter.ToInt32(receive.Content1, 0))
		{
		case 1001:
			ret.FileName = Encoding.Unicode.GetString(receive.Content2);
			break;
		case 1005:
			ret.FileNames = HslProtocol.UnPackStringArrayFromByte(receive.Content2);
			break;
		}
		OperateResult<int, string> factoryResult = await ReceiveStringContentFromSocketAsync(socket);
		if (!factoryResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(factoryResult);
		}
		ret.Factory = factoryResult.Content2;
		OperateResult<int, string> groupResult = await ReceiveStringContentFromSocketAsync(socket);
		if (!groupResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(groupResult);
		}
		ret.Group = groupResult.Content2;
		OperateResult<int, string> idResult = await ReceiveStringContentFromSocketAsync(socket);
		if (!idResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<FileGroupInfo>(idResult);
		}
		ret.Identify = idResult.Content2;
		return OperateResult.CreateSuccessResult(ret);
	}

	/// <summary>
	/// 获取一个随机的文件名，由GUID码和随机数字组成
	/// </summary>
	/// <returns>文件名</returns>
	protected string CreateRandomFileName()
	{
		return SoftBasic.GetUniqueStringByGuidAndRandom();
	}

	/// <summary>
	/// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][Factory][Group][Id] 信息
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	protected string ReturnAbsoluteFilePath(string factory, string group, string id)
	{
		string text = m_FilesDirectoryPath;
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			if (!string.IsNullOrEmpty(factory))
			{
				text = text + "/" + factory;
			}
			if (!string.IsNullOrEmpty(group))
			{
				text = text + "/" + group;
			}
			if (!string.IsNullOrEmpty(id))
			{
				text = text + "/" + id;
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(factory))
			{
				text = text + "\\" + factory;
			}
			if (!string.IsNullOrEmpty(group))
			{
				text = text + "\\" + group;
			}
			if (!string.IsNullOrEmpty(id))
			{
				text = text + "\\" + id;
			}
		}
		return text;
	}

	/// <summary>
	/// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][Factory][Group][Id][FileName] 信息
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileName">文件名</param>
	/// <returns>是否成功的结果对象</returns>
	protected string ReturnAbsoluteFileName(string factory, string group, string id, string fileName)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return ReturnAbsoluteFilePath(factory, group, id) + "/" + fileName;
		}
		return ReturnAbsoluteFilePath(factory, group, id) + "\\" + fileName;
	}

	/// <summary>
	/// 返回相对路径的名称
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileName">文件名</param>
	/// <returns>是否成功的结果对象</returns>
	protected string GetRelativeFileName(string factory, string group, string id, string fileName)
	{
		string text = "";
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			if (!string.IsNullOrEmpty(factory))
			{
				text = text + factory + "/";
			}
			if (!string.IsNullOrEmpty(group))
			{
				text = text + group + "/";
			}
			if (!string.IsNullOrEmpty(id))
			{
				text = text + id + "/";
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(factory))
			{
				text = text + factory + "\\";
			}
			if (!string.IsNullOrEmpty(group))
			{
				text = text + group + "\\";
			}
			if (!string.IsNullOrEmpty(id))
			{
				text = text + id + "\\";
			}
		}
		return text + fileName;
	}

	/// <summary>
	/// 移动一个文件到新的文件去
	/// </summary>
	/// <param name="fileNameOld">旧的文件名称</param>
	/// <param name="fileNameNew">新的文件名称</param>
	/// <returns>是否成功</returns>
	protected bool MoveFileToNewFile(string fileNameOld, string fileNameNew)
	{
		try
		{
			FileInfo fileInfo = new FileInfo(fileNameNew);
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
			if (File.Exists(fileNameNew))
			{
				File.Delete(fileNameNew);
			}
			File.Move(fileNameOld, fileNameNew);
			return true;
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException(ToString(), "Move a file to new file failed: ", ex);
			return false;
		}
	}

	/// <summary>
	/// 删除文件并回发确认信息，如果结果异常，则结束通讯
	/// </summary>
	/// <param name="socket">网络套接字</param>
	/// <param name="fullname">完整路径的文件名称</param>
	/// <returns>是否成功的结果对象</returns>
	protected OperateResult DeleteFileAndCheck(Socket socket, string fullname)
	{
		int customer = 0;
		int num = 0;
		while (num < 3)
		{
			num++;
			if (DeleteFileByName(fullname))
			{
				customer = 1;
				break;
			}
			HslHelper.ThreadSleep(500);
		}
		return SendStringAndCheckReceive(socket, customer, StringResources.Language.SuccessText);
	}

	/// <summary>
	/// 触发一个文件上传的事件。
	/// </summary>
	/// <param name="fileInfo">文件的基本信息</param>
	protected void OnFileUpload(FileServerInfo fileInfo)
	{
		OnFileUploadEvent?.Invoke(fileInfo);
	}

	/// <summary>
	/// 服务器启动时的操作
	/// </summary>
	protected override void StartInitialization()
	{
		if (string.IsNullOrEmpty(FilesDirectoryPath))
		{
			throw new ArgumentNullException("FilesDirectoryPath", "No saved path is specified");
		}
		CheckFolderAndCreate();
		base.StartInitialization();
	}

	/// <summary>
	/// 检查文件夹是否存在，不存在就创建
	/// </summary>
	protected virtual void CheckFolderAndCreate()
	{
		if (!Directory.Exists(FilesDirectoryPath))
		{
			Directory.CreateDirectory(FilesDirectoryPath);
		}
	}

	/// <summary>
	/// 获取文件夹的所有文件列表
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>文件列表</returns>
	public virtual string[] GetDirectoryFiles(string factory, string group, string id)
	{
		if (string.IsNullOrEmpty(FilesDirectoryPath))
		{
			return new string[0];
		}
		string path = ReturnAbsoluteFilePath(factory, group, id);
		if (!Directory.Exists(path))
		{
			return new string[0];
		}
		return Directory.GetFiles(path);
	}

	/// <summary>
	/// 获取文件夹的所有文件夹列表
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>文件夹列表</returns>
	public string[] GetDirectories(string factory, string group, string id)
	{
		if (string.IsNullOrEmpty(FilesDirectoryPath))
		{
			return new string[0];
		}
		string path = ReturnAbsoluteFilePath(factory, group, id);
		if (!Directory.Exists(path))
		{
			return new string[0];
		}
		return Directory.GetDirectories(path);
	}

	public override string ToString()
	{
		return "NetworkFileServerBase";
	}
}

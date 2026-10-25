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
using HslCommunication.Core;
using HslCommunication.Reflection;
using Newtonsoft.Json.Linq;

namespace HslCommunication.Enthernet;

/// <summary>
/// 与服务器文件引擎交互的客户端类，支持操作Advanced引擎和Ultimate引擎，用来上传，下载，删除服务器中的文件操作。
/// The client class that interacts with the server file engine, supports the operation of the Advanced engine and the Ultimate engine,
/// and is used to upload, download, and delete file operations on the server.
/// </summary>
/// <remarks>
/// 这里需要需要的是，本客户端支持Advanced引擎和Ultimate引擎文件服务器，服务的类型需要您根据自己的需求来选择。
/// 需要注意的是，三个分类信息，factory, group, id 的字符串是不区分大小写的。
/// </remarks>
public class IntegrationFileClient : FileClientBase
{
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
	/// 实例化一个默认的对象，需要提前指定服务器的远程地址
	/// Instantiate a default object, you need to specify the remote address of the server in advance
	/// </summary>
	public IntegrationFileClient()
	{
	}

	/// <summary>
	/// 通过指定的Ip地址及端口号实例化一个对象
	/// Instantiate an object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">服务器的ip地址</param>
	/// <param name="port">端口号信息</param>
	public IntegrationFileClient(string ipAddress, int port)
	{
		base.ServerIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
	}

	[HslMqttApi(ApiTopic = "DeleteFileFactoryGroupId", Description = "Delete the file operation of the server, you need to specify the file name and the three-level classification information of the file")]
	/// <summary>
	/// 删除服务器的文件操作，需要指定文件名称，文件的三级分类信息
	/// Delete the file operation of the server, you need to specify the file name and the three-level classification information of the file
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult DeleteFile(string fileName, string factory, string group, string id)
	{
		return DeleteFileBase(fileName, factory, group, id);
	}

	[HslMqttApi(Description = "Delete the file operation of the server, the classification of the file is empty here")]
	/// <summary>
	/// 删除服务器的文件操作，此处文件的分类为空
	/// Delete the file operation of the server, the classification of the file is empty here
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult DeleteFile(string fileName)
	{
		return DeleteFileBase(fileName, "", "", "");
	}

	[HslMqttApi(ApiTopic = "DeleteFilesFactoryGroupId", Description = "Delete the file operation of the server, you need to specify the file names and the three-level classification information of the file")]
	/// <summary>
	/// 删除服务器的文件数组操作，需要指定文件名称，文件的三级分类信息
	/// Delete the file operation of the server, you need to specify the file names and the three-level classification information of the file
	/// </summary>
	/// <param name="fileNames">文件名称数组，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult DeleteFile(string[] fileNames, string factory, string group, string id)
	{
		return DeleteFileBase(fileNames, factory, group, id);
	}

	[HslMqttApi(Description = "Delete all file operations of the server folder, the three-level classification information of the file")]
	/// <summary>
	/// 删除服务器的文件夹的所有文件操作，文件的三级分类信息
	/// Delete all file operations of the server folder, the three-level classification information of the file
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult DeleteFolderFiles(string factory, string group, string id)
	{
		return DeleteFolderBase(factory, group, id);
	}

	[HslMqttApi(Description = "To delete all empty sub-file directories of the server's folder, you need to pass in the three-level classification information of the file")]
	/// <summary>
	/// 删除服务器的文件夹的所有空的子文件目录操作，需要传入文件的三级分类信息
	/// To delete all empty sub-file directories of the server's folder, you need to pass in the three-level classification information of the file
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult DeleteEmptyFolders(string factory, string group, string id)
	{
		return DeleteEmptyFoldersBase(factory, group, id);
	}

	[HslMqttApi(Description = "Get the file statistics of the specified directory of the server folder, including the number of files, the total size, and the last update time")]
	/// <summary>
	/// 获取服务器文件夹的指定目录的文件统计信息，包括文件数量，总大小，最后更新时间
	/// Get the file statistics of the specified directory of the server folder, including the number of files, the total size, and the last update time
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>返回路径的信息，包含文件大小，数量，最后更新时间</returns>
	public OperateResult<GroupFileInfo> GetGroupFileInfo(string factory, string group, string id)
	{
		OperateResult<Socket> operateResult = CreateSocketAndConnect(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(operateResult);
		}
		OperateResult operateResult2 = SendStringAndCheckReceive(operateResult.Content, 2016, "");
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(operateResult2);
		}
		OperateResult operateResult3 = SendFactoryGroupId(operateResult.Content, factory, group, id);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(operateResult3);
		}
		OperateResult<int, string> operateResult4 = ReceiveStringContentFromSocket(operateResult.Content);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(operateResult4);
		}
		return OperateResult.CreateSuccessResult(((JToken)JObject.Parse(operateResult4.Content2)).ToObject<GroupFileInfo>());
	}

	[HslMqttApi(Description = "Get the file information of all subdirectories of the specified directory of the server folder, including the number of files in each subdirectory, the total size, and the last update time")]
	/// <summary>
	/// 获取服务器文件夹的指定目录的所有子目录的文件信息，包括每个子目录的文件数量，总大小，最后更新时间
	/// Get the file information of all subdirectories of the specified directory of the server folder, including the number of files in each subdirectory, the total size, and the last update time
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>返回路径的信息，包含文件大小，数量，最后更新时间</returns>
	public OperateResult<GroupFileInfo[]> GetSubGroupFileInfos(string factory, string group, string id)
	{
		OperateResult<Socket> operateResult = CreateSocketAndConnect(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(operateResult);
		}
		OperateResult operateResult2 = SendStringAndCheckReceive(operateResult.Content, 2016, "");
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(operateResult2);
		}
		OperateResult operateResult3 = SendFactoryGroupId(operateResult.Content, factory, group, id);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(operateResult3);
		}
		OperateResult<int, string> operateResult4 = ReceiveStringContentFromSocket(operateResult.Content);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(operateResult4);
		}
		return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(operateResult4.Content2)).ToObject<GroupFileInfo[]>());
	}

	/// <summary>
	/// 删除服务器的文件操作，需要指定文件名称，文件的三级分类信息
	/// Delete the file operation of the server, you need to specify the file name and the three-level classification information of the file
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> DeleteFileAsync(string fileName, string factory, string group, string id)
	{
		return await DeleteFileBaseAsync(fileName, factory, group, id);
	}

	/// <summary>
	/// 删除服务器的文件操作，此处文件的分类为空
	/// Delete the file operation of the server, the classification of the file is empty here
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> DeleteFileAsync(string fileName)
	{
		return await DeleteFileBaseAsync(fileName, "", "", "");
	}

	/// <summary>
	/// 删除服务器的文件数组操作，需要指定文件名称，文件的三级分类信息
	/// Delete the file operation of the server, you need to specify the file names and the three-level classification information of the file
	/// </summary>
	/// <param name="fileNames">文件名称数组，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> DeleteFileAsync(string[] fileNames, string factory, string group, string id)
	{
		return await DeleteFileBaseAsync(fileNames, factory, group, id);
	}

	/// <summary>
	/// 删除服务器的文件夹的所有文件操作，文件的三级分类信息
	/// Delete all file operations of the server folder, the three-level classification information of the file
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> DeleteFolderFilesAsync(string factory, string group, string id)
	{
		return await DeleteFolderBaseAsync(factory, group, id);
	}

	/// <summary>
	/// 删除服务器的文件夹的所有空的子文件目录操作，需要传入文件的三级分类信息
	/// To delete all empty sub-file directories of the server's folder, you need to pass in the three-level classification information of the file
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> DeleteEmptyFoldersAsync(string factory, string group, string id)
	{
		return await DeleteEmptyFoldersBaseAsync(factory, group, id);
	}

	/// <summary>
	/// 获取服务器文件夹的指定目录的文件统计信息，包括文件数量，总大小，最后更新时间
	/// Get the file statistics of the specified directory of the server folder, including the number of files, the total size, and the last update time
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>返回路径的信息，包含文件大小，数量，最后更新时间</returns>
	public async Task<OperateResult<GroupFileInfo>> GetGroupFileInfoAsync(string factory, string group, string id)
	{
		OperateResult<Socket> socketResult = await CreateSocketAndConnectAsync(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!socketResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(socketResult);
		}
		OperateResult sendString = await SendStringAndCheckReceiveAsync(socketResult.Content, 2015, "");
		if (!sendString.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(sendString);
		}
		OperateResult sendFileInfo = await SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
		if (!sendFileInfo.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(sendFileInfo);
		}
		OperateResult<int, string> receiveBack = await ReceiveStringContentFromSocketAsync(socketResult.Content);
		if (!receiveBack.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo>(receiveBack);
		}
		return OperateResult.CreateSuccessResult(((JToken)JObject.Parse(receiveBack.Content2)).ToObject<GroupFileInfo>());
	}

	/// <summary>
	/// 获取服务器文件夹的指定目录的文件统计信息，包括文件数量，总大小，最后更新时间
	/// Get the file statistics of the specified directory of the server folder, including the number of files, the total size, and the last update time
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>返回路径的信息，包含文件大小，数量，最后更新时间</returns>
	public async Task<OperateResult<GroupFileInfo[]>> GetSubGroupFileInfosAsync(string factory, string group, string id)
	{
		OperateResult<Socket> socketResult = await CreateSocketAndConnectAsync(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!socketResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(socketResult);
		}
		OperateResult sendString = await SendStringAndCheckReceiveAsync(socketResult.Content, 2016, "");
		if (!sendString.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(sendString);
		}
		OperateResult sendFileInfo = await SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
		if (!sendFileInfo.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(sendFileInfo);
		}
		OperateResult<int, string> receiveBack = await ReceiveStringContentFromSocketAsync(socketResult.Content);
		if (!receiveBack.IsSuccess)
		{
			return OperateResult.CreateFailedResult<GroupFileInfo[]>(receiveBack);
		}
		return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(receiveBack.Content2)).ToObject<GroupFileInfo[]>());
	}

	[HslMqttApi(Description = "To download a file from the server to a local file, you need to specify the name of the downloaded file and three-level classification information")]
	/// <summary>
	/// 下载服务器的文件到本地的文件操作，需要指定下载的文件的名字，三级分类信息，本次保存的文件名，支持进度报告。
	/// To download a file from the server to a local file, you need to specify the name of the downloaded file,
	/// the three-level classification information, the name of the file saved this time, and support for progress reports.
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="processReport">下载的进度报告，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
	/// <param name="fileSaveName">准备本地保存的名称</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是服务器不存在文件。
	/// </remarks>
	public OperateResult DownloadFile(string fileName, string factory, string group, string id, Action<long, long> processReport, string fileSaveName)
	{
		return DownloadFileBase(factory, group, id, fileName, processReport, fileSaveName);
	}

	/// <summary>
	/// 下载服务器的文件到本地的文件操作，需要指定下载的文件的名字，三级分类信息，本次保存的文件名，支持进度报告。
	/// To download a file from the server to a local file, you need to specify the name of the downloaded file,
	/// the three-level classification information, the name of the file saved this time, and support for progress reports.
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="processReport">下载的进度报告，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
	/// <param name="fileSaveName">准备本地保存的名称</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是服务器不存在文件。
	/// </remarks>
	public OperateResult DownloadFile(string fileName, string factory, string group, string id, Action<long, long> processReport, Stream stream)
	{
		return DownloadFileBase(factory, group, id, fileName, processReport, stream);
	}

	/// <summary>
	/// 下载服务器的文件到本地的文件操作，需要指定下载的文件的名字，三级分类信息，本次保存的文件名，支持进度报告。
	/// To download a file from the server to a local file, you need to specify the name of the downloaded file,
	/// the three-level classification information, the name of the file saved this time, and support for progress reports.
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="processReport">下载的进度报告，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
	/// <param name="fileSaveName">准备本地保存的名称</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是服务器不存在文件。
	/// </remarks>
	public async Task<OperateResult> DownloadFileAsync(string fileName, string factory, string group, string id, Action<long, long> processReport, string fileSaveName)
	{
		return await DownloadFileBaseAsync(factory, group, id, fileName, processReport, fileSaveName);
	}

	/// <summary>
	/// 下载服务器的文件到本地的文件操作，需要指定下载的文件的名字，三级分类信息，本次保存的文件名，支持进度报告。
	/// To download a file from the server to a local file, you need to specify the name of the downloaded file,
	/// the three-level classification information, the name of the file saved this time, and support for progress reports.
	/// </summary>
	/// <param name="fileName">文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="processReport">下载的进度报告，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
	/// <param name="fileSaveName">准备本地保存的名称</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是服务器不存在文件。
	/// </remarks>
	public async Task<OperateResult> DownloadFileAsync(string fileName, string factory, string group, string id, Action<long, long> processReport, Stream stream)
	{
		return await DownloadFileBaseAsync(factory, group, id, fileName, processReport, stream);
	}

	[HslMqttApi(Description = "Upload a local file to the server. If the file already exists, update the file.")]
	/// <summary>
	/// 上传本地的文件到服务器操作，如果该文件已经存在，那么就更新这个文件。
	/// Upload a local file to the server. If the file already exists, update the file.
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="serverName">服务器存储的文件名称，带后缀，例如123.txt</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileTag">文件的额外描述</param>
	/// <param name="fileUpload">文件的上传人</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
	/// </remarks>
	public OperateResult UploadFile(string fileName, string serverName, string factory, string group, string id, string fileTag, string fileUpload, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		return UploadFileBase(fileName, serverName, factory, group, id, fileTag, fileUpload, processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileTag">文件的额外描述</param>
	/// <param name="fileUpload">文件的上传人</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult UploadFile(string fileName, string factory, string group, string id, string fileTag, string fileUpload, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return UploadFileBase(fileName, fileInfo.Name, factory, group, id, fileTag, fileUpload, processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult UploadFile(string fileName, string factory, string group, string id, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return UploadFileBase(fileName, fileInfo.Name, factory, group, id, "", "", processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称，其余参数默认为空
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult UploadFile(string fileName, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return UploadFileBase(fileName, fileInfo.Name, "", "", "", "", "", processReport);
	}

	/// <summary>
	/// 上传数据流到服务器操作
	/// </summary>
	/// <param name="stream">数据流内容</param>
	/// <param name="serverName">服务器存储的文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileTag">文件的额外描述</param>
	/// <param name="fileUpload">文件的上传人</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
	/// </remarks>
	public OperateResult UploadFile(Stream stream, string serverName, string factory, string group, string id, string fileTag, string fileUpload, Action<long, long> processReport)
	{
		return UploadFileBase(stream, serverName, factory, group, id, fileTag, fileUpload, processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，如果该文件已经存在，那么就更新这个文件。
	/// Upload a local file to the server. If the file already exists, update the file.
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="serverName">服务器存储的文件名称，带后缀，例如123.txt</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileTag">文件的额外描述</param>
	/// <param name="fileUpload">文件的上传人</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
	/// </remarks>
	public async Task<OperateResult> UploadFileAsync(string fileName, string serverName, string factory, string group, string id, string fileTag, string fileUpload, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		return await UploadFileBaseAsync(fileName, serverName, factory, group, id, fileTag, fileUpload, processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileTag">文件的额外描述</param>
	/// <param name="fileUpload">文件的上传人</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> UploadFileAsync(string fileName, string factory, string group, string id, string fileTag, string fileUpload, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return await UploadFileBaseAsync(fileName, fileInfo.Name, factory, group, id, fileTag, fileUpload, processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> UploadFileAsync(string fileName, string factory, string group, string id, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return await UploadFileBaseAsync(fileName, fileInfo.Name, factory, group, id, "", "", processReport);
	}

	/// <summary>
	/// 上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称，其余参数默认为空
	/// </summary>
	/// <param name="fileName">本地的完整路径的文件名称</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> UploadFileAsync(string fileName, Action<long, long> processReport)
	{
		if (!File.Exists(fileName))
		{
			return new OperateResult(StringResources.Language.FileNotExist);
		}
		FileInfo fileInfo = new FileInfo(fileName);
		return await UploadFileBaseAsync(fileName, fileInfo.Name, "", "", "", "", "", processReport);
	}

	/// <summary>
	/// 上传数据流到服务器操作
	/// </summary>
	/// <param name="stream">数据流内容</param>
	/// <param name="serverName">服务器存储的文件名称，带后缀</param>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <param name="fileTag">文件的额外描述</param>
	/// <param name="fileUpload">文件的上传人</param>
	/// <param name="processReport">上传的进度报告</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
	/// </remarks>
	public async Task<OperateResult> UploadFileAsync(Stream stream, string serverName, string factory, string group, string id, string fileTag, string fileUpload, Action<long, long> processReport)
	{
		return await UploadFileBaseAsync(stream, serverName, factory, group, id, fileTag, fileUpload, processReport);
	}

	[HslMqttApi(Description = "Get all documents in the specified path")]
	/// <summary>
	/// 获取指定路径下的所有的文档
	/// Get all documents in the specified path
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常。
	/// </remarks>
	public OperateResult<GroupFileItem[]> DownloadPathFileNames(string factory, string group, string id)
	{
		return DownloadStringArrays<GroupFileItem>(2007, factory, group, id);
	}

	/// <summary>
	/// 获取指定路径下的所有的文档
	/// Get all documents in the specified path
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常。
	/// </remarks>
	public async Task<OperateResult<GroupFileItem[]>> DownloadPathFileNamesAsync(string factory, string group, string id)
	{
		return await DownloadStringArraysAsync<GroupFileItem>(2007, factory, group, id);
	}

	[HslMqttApi(Description = "Get all directories under the specified path")]
	/// <summary>
	/// 获取指定路径下的所有的目录
	/// Get all directories under the specified path
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常。
	/// </remarks>
	public OperateResult<string[]> DownloadPathFolders(string factory, string group, string id)
	{
		return DownloadStringArrays<string>(2008, factory, group, id);
	}

	/// <summary>
	/// 获取指定路径下的所有的目录
	/// Get all directories under the specified path
	/// </summary>
	/// <param name="factory">第一大类</param>
	/// <param name="group">第二大类</param>
	/// <param name="id">第三大类</param>
	/// <returns>是否成功的结果对象</returns>
	/// <remarks>
	/// 用于分类的参数，，中间不需要的可以为空，对应的是服务器上的路径系统。
	/// 失败的原因大多数来自于网络的接收异常。
	/// </remarks>
	public async Task<OperateResult<string[]>> DownloadPathFoldersAsync(string factory, string group, string id)
	{
		return await DownloadStringArraysAsync<string>(2008, factory, group, id);
	}

	/// <summary>
	/// 检查当前的文件是否在服务器端存在，列表中需要存在文件的名称，映射的文件也需要存在。
	/// Check whether the current file exists on the server side, the name of the file must exist in the list, and the mapped file must also exist.
	/// </summary>
	/// <param name="fileName">当前的文件名称，举例123.txt</param>
	/// <param name="factory">第一级分类信息</param>
	/// <param name="group">第二级分类信息</param>
	/// <param name="id">第三级分类信息</param>
	/// <returns>是否存在，存在返回true, 否则，返回false</returns>
	public OperateResult<bool> IsFileExists(string fileName, string factory, string group, string id)
	{
		OperateResult<Socket> operateResult = CreateSocketAndConnect(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult);
		}
		OperateResult operateResult2 = SendStringAndCheckReceive(operateResult.Content, 2013, fileName);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult2);
		}
		OperateResult operateResult3 = SendFactoryGroupId(operateResult.Content, factory, group, id);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult3);
		}
		OperateResult<int, string> operateResult4 = ReceiveStringContentFromSocket(operateResult.Content);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(operateResult4);
		}
		OperateResult<bool> result = OperateResult.CreateSuccessResult(operateResult4.Content1 == 1);
		operateResult.Content?.Close();
		return result;
	}

	/// <summary>
	/// 检查当前的文件是否在服务器端存在，列表中需要存在文件的名称，映射的文件也需要存在。
	/// Check whether the current file exists on the server side, the name of the file must exist in the list, and the mapped file must also exist.
	/// </summary>
	/// <param name="fileName">当前的文件名称，举例123.txt</param>
	/// <param name="factory">第一级分类信息</param>
	/// <param name="group">第二级分类信息</param>
	/// <param name="id">第三级分类信息</param>
	/// <returns>是否存在，存在返回true, 否则，返回false</returns>
	public async Task<OperateResult<bool>> IsFileExistsAsync(string fileName, string factory, string group, string id)
	{
		OperateResult<Socket> socketResult = await CreateSocketAndConnectAsync(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!socketResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(socketResult);
		}
		OperateResult sendString = await SendStringAndCheckReceiveAsync(socketResult.Content, 2013, fileName);
		if (!sendString.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(sendString);
		}
		OperateResult sendFileInfo = await SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
		if (!sendFileInfo.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(sendFileInfo);
		}
		OperateResult<int, string> receiveBack = await ReceiveStringContentFromSocketAsync(socketResult.Content);
		if (!receiveBack.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool>(receiveBack);
		}
		OperateResult<bool> result = OperateResult.CreateSuccessResult(receiveBack.Content1 == 1);
		socketResult.Content?.Close();
		return result;
	}

	private OperateResult<T[]> DownloadStringArrays<T>(int protocol, string factory, string group, string id)
	{
		OperateResult<Socket> operateResult = CreateSocketAndConnect(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(operateResult);
		}
		OperateResult operateResult2 = SendStringAndCheckReceive(operateResult.Content, protocol, "nosense");
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(operateResult2);
		}
		OperateResult operateResult3 = SendFactoryGroupId(operateResult.Content, factory, group, id);
		if (!operateResult3.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(operateResult3);
		}
		OperateResult<int, string> operateResult4 = ReceiveStringContentFromSocket(operateResult.Content);
		if (!operateResult4.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(operateResult4);
		}
		operateResult.Content?.Close();
		try
		{
			return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(operateResult4.Content2)).ToObject<T[]>());
		}
		catch (Exception ex)
		{
			return new OperateResult<T[]>(ex.Message);
		}
	}

	private async Task<OperateResult<T[]>> DownloadStringArraysAsync<T>(int protocol, string factory, string group, string id)
	{
		OperateResult<Socket> socketResult = await CreateSocketAndConnectAsync(base.ServerIpEndPoint, base.ConnectTimeOut);
		if (!socketResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(socketResult);
		}
		OperateResult send = await SendStringAndCheckReceiveAsync(socketResult.Content, protocol, "nosense");
		if (!send.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(send);
		}
		OperateResult sendClass = await SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
		if (!sendClass.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(sendClass);
		}
		OperateResult<int, string> receive = await ReceiveStringContentFromSocketAsync(socketResult.Content);
		if (!receive.IsSuccess)
		{
			return OperateResult.CreateFailedResult<T[]>(receive);
		}
		socketResult.Content?.Close();
		try
		{
			return OperateResult.CreateSuccessResult(((JToken)JArray.Parse(receive.Content2)).ToObject<T[]>());
		}
		catch (Exception ex)
		{
			return new OperateResult<T[]>(ex.Message);
		}
	}

	public override string ToString()
	{
		return $"IntegrationFileClient[{base.ServerIpEndPoint}]";
	}
}

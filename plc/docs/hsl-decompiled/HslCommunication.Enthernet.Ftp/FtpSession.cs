// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Enthernet.Ftp;

/// <summary>
/// Ftp的会话信息对象
/// Ftp session information object
/// </summary>
public class FtpSession : ISessionContext
{
	/// <summary>
	/// 当前会话的编码信息
	/// Current session encoding information
	/// </summary>
	internal Encoding Encoding { get; set; } = Encoding.Default;

	/// <summary>
	/// 当前的目录信息
	/// Current directory information
	/// </summary>
	public string CurrentDirectory { get; set; } = "/";

	/// <summary>
	/// 获取或设置当前的会话对象
	/// Get or set the current session object
	/// </summary>
	public PipeTcpNet Session { get; set; }

	/// <summary>
	/// 当前的用户名
	/// Current username
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// 是否允许上传文件操作
	/// can upload file operation
	/// </summary>
	public bool CanUpload { get; set; } = true;

	/// <summary>
	/// 是否允许删除文件，删除文件夹，创建文件夹，重命名文件，重命名文件夹操作
	/// can delete file operation
	/// </summary>
	public bool CanEdit { get; set; } = true;

	/// <summary>
	/// 当前的会话信息关联的自定义信息
	/// Custom information associated with the current session information
	/// </summary>
	public object Tag { get; set; }

	/// <summary>
	/// 当前接收的客户端ID信息
	/// Client ID information currently received
	/// </summary>
	public string ClientId { get; set; }

	/// <summary>
	/// 示例化一个Ftp的会话对象
	/// Instantiate an Ftp session object
	/// </summary>
	public FtpSession()
	{
		UserName = "anonymous";
		ClientId = string.Empty;
		Tag = null;
	}

	/// <summary>
	/// 获取实际的物理路径
	/// </summary>
	/// <param name="basePath">基础路径</param>
	/// <returns>实际的路径</returns>
	public string GetPhysicalPath(string basePath)
	{
		string text = CurrentDirectory;
		if (string.IsNullOrEmpty(text))
		{
			text = "/";
		}
		if (!text.StartsWith("/"))
		{
			text = "/" + text;
		}
		text = text.Replace("/", "\\");
		return basePath + text;
	}

	/// <summary>
	/// 获取文件路径信息
	/// </summary>
	public string GetPhysicalFilePath(string basePath, string fileName)
	{
		string text = GetPhysicalPath(basePath);
		if (!text.EndsWith("\\"))
		{
			text += "\\";
		}
		if (fileName.StartsWith("/"))
		{
			fileName = fileName.Substring(1);
		}
		fileName = fileName.Replace("/", "\\");
		return text + fileName;
	}

	public override string ToString()
	{
		return $"Ftp Session[{UserName}@{Session}]";
	}
}

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
using System.Text;
using HslCommunication.Core;
using HslCommunication.LogNet;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 文件存储功能的基类，包含了文件存储路径，存储方法等
/// </summary>
/// <remarks>
/// 需要继承才能实现你想存储的数据，比较经典的例子就是存储你的应用程序的配置信息，通常的格式就是xml文件或是json文件。具体请看例子：
/// </remarks>
public class SoftFileSaveBase : ISoftFileSaveBase
{
	private SimpleHybirdLock HybirdLock;

	/// <summary>
	/// 在日志保存时的标记当前调用类的信息
	/// </summary>
	protected string LogHeaderText { get; set; }

	/// <summary>
	/// 文件存储的路径
	/// </summary>
	public string FileSavePath { get; set; }

	/// <summary>
	/// 日志记录类
	/// </summary>
	public ILogNet ILogNet { get; set; }

	/// <summary>
	/// 实例化一个文件存储的基类
	/// </summary>
	public SoftFileSaveBase()
	{
		HybirdLock = new SimpleHybirdLock();
	}

	/// <summary>
	/// 获取需要保存的数据，需要重写实现
	/// </summary>
	/// <returns>需要存储的信息</returns>
	public virtual string ToSaveString()
	{
		return string.Empty;
	}

	/// <summary>
	/// 从字符串加载数据，需要重写实现
	/// </summary>
	/// <param name="content">字符串数据</param>
	public virtual void LoadByString(string content)
	{
	}

	/// <summary>
	/// 不使用解密方法从文件读取数据
	/// </summary>
	public virtual void LoadByFile()
	{
		LoadByFile((string m) => m);
	}

	/// <summary>
	/// 使用用户自定义的解密方法从文件读取数据
	/// </summary>
	/// <param name="decrypt">用户自定义的解密方法</param>
	public void LoadByFile(Converter<string, string> decrypt)
	{
		if (!(FileSavePath != "") || !File.Exists(FileSavePath))
		{
			return;
		}
		HybirdLock.Enter();
		try
		{
			using StreamReader streamReader = new StreamReader(FileSavePath, Encoding.Default);
			LoadByString(decrypt(streamReader.ReadToEnd()));
		}
		catch (Exception ex)
		{
			ILogNet?.WriteException(LogHeaderText, StringResources.Language.FileLoadFailed, ex);
		}
		finally
		{
			HybirdLock.Leave();
		}
	}

	/// <summary>
	/// 不使用加密方法保存数据到文件
	/// </summary>
	public virtual void SaveToFile()
	{
		SaveToFile((string m) => m);
	}

	/// <summary>
	/// 使用用户自定义的加密方法保存数据到文件
	/// </summary>
	/// <param name="encrypt">用户自定义的加密方法</param>
	public void SaveToFile(Converter<string, string> encrypt)
	{
		if (!(FileSavePath != ""))
		{
			return;
		}
		HybirdLock.Enter();
		try
		{
			using StreamWriter streamWriter = new StreamWriter(FileSavePath, append: false, Encoding.Default);
			streamWriter.Write(encrypt(ToSaveString()));
			streamWriter.Flush();
		}
		catch (Exception ex)
		{
			ILogNet?.WriteException(LogHeaderText, StringResources.Language.FileSaveFailed, ex);
		}
		finally
		{
			HybirdLock.Leave();
		}
	}
}

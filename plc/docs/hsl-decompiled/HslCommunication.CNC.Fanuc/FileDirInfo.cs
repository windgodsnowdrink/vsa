// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core;

namespace HslCommunication.CNC.Fanuc;

/// <summary>
/// 文件或是文件夹的信息
/// </summary>
public class FileDirInfo
{
	/// <summary>
	/// 是否为文件夹，True就是文件夹，False就是文件
	/// </summary>
	public bool IsDirectory { get; set; }

	/// <summary>
	/// 文件或是文件夹的名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 最后一次更新时间，当为文件的时候有效
	/// </summary>
	public DateTime LastModified { get; set; }

	/// <summary>
	/// 文件的大小，当为文件的时候有效
	/// </summary>
	public int Size { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public FileDirInfo()
	{
	}

	/// <summary>
	/// 使用原始字节来实例化对象
	/// </summary>
	/// <param name="byteTransform">字节变换对象</param>
	/// <param name="buffer">原始的字节信息</param>
	/// <param name="index">起始的索引信息</param>
	public FileDirInfo(IByteTransform byteTransform, byte[] buffer, int index)
	{
		IsDirectory = byteTransform.TransInt16(buffer, index) == 0;
		Name = buffer.GetStringOrEndChar(index + 28, 36, Encoding.ASCII);
		if (!IsDirectory)
		{
			try
			{
				LastModified = new DateTime(byteTransform.TransInt16(buffer, index + 2), byteTransform.TransInt16(buffer, index + 4), byteTransform.TransInt16(buffer, index + 6), byteTransform.TransInt16(buffer, index + 8), byteTransform.TransInt16(buffer, index + 10), byteTransform.TransInt16(buffer, index + 12));
			}
			catch
			{
				LastModified = new DateTime(2000, 1, 1);
			}
			Size = byteTransform.TransInt32(buffer, index + 20);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(IsDirectory ? "[PATH]   " : "[FILE]   ");
		stringBuilder.Append(Name.PadRight(40));
		if (!IsDirectory)
		{
			stringBuilder.Append("     ");
			stringBuilder.Append(LastModified.ToString("yyyy-MM-dd HH:mm:ss"));
			stringBuilder.Append("         ");
			stringBuilder.Append(SoftBasic.GetSizeDescription(Size));
		}
		return stringBuilder.ToString();
	}
}

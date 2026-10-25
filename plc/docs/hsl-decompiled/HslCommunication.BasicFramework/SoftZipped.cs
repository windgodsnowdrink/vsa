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
using System.IO.Compression;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 一个负责压缩解压数据字节的类
/// </summary>
public class SoftZipped
{
	/// <summary>
	/// 压缩字节数据
	/// </summary>
	/// <param name="bytes">等待被压缩的数据</param>
	/// <returns>压缩之后的字节数据</returns>
	public static byte[] CompressBytes(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		using MemoryStream memoryStream = new MemoryStream();
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
		{
			gZipStream.Write(bytes, 0, bytes.Length);
		}
		return memoryStream.ToArray();
	}

	/// <summary>
	/// 解压压缩后的数据
	/// </summary>
	/// <param name="bytes">压缩后的数据</param>
	/// <returns>压缩前的原始字节数据</returns>
	public static byte[] Decompress(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		using MemoryStream stream = new MemoryStream(bytes);
		using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
		using MemoryStream memoryStream = new MemoryStream();
		int num = 1024;
		byte[] buffer = new byte[num];
		int num2 = 0;
		while ((num2 = gZipStream.Read(buffer, 0, num)) > 0)
		{
			memoryStream.Write(buffer, 0, num2);
		}
		return memoryStream.ToArray();
	}
}

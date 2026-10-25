// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// 结构体的句柄信息
/// </summary>
public class AbStructHandle
{
	/// <summary>
	/// 返回项数
	/// </summary>
	/// <remarks>
	/// Count of Items returned
	/// </remarks>
	public ushort ReturnCount { get; set; }

	/// <summary>
	/// 结构体定义大小
	/// </summary>
	/// <remarks>
	/// This is the number of structure members
	/// </remarks>
	public uint TemplateObjectDefinitionSize { get; set; }

	/// <summary>
	/// 使用读取标记服务读取结构时在线路上传输的字节数
	/// </summary>
	/// <remarks>
	/// This is the number of bytes of the structure data
	/// </remarks>
	public uint TemplateStructureSize { get; set; }

	/// <summary>
	/// 成员数量
	/// </summary>
	/// <remarks>
	/// This is the number of structure members
	/// </remarks>
	public ushort MemberCount { get; set; }

	/// <summary>
	/// 结构体的handle
	/// </summary>
	/// <remarks>
	/// This is the Tag Type Parameter used in Read/Write Tag service
	/// </remarks>
	public ushort StructureHandle { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// instantiate a default object
	/// </summary>
	public AbStructHandle()
	{
	}

	/// <summary>
	/// 使用原始字节的数据，索引信息来实例化一个对象
	/// Instantiate an object with raw bytes of data, index information
	/// </summary>
	/// <param name="source">原始字节数据</param>
	/// <param name="index">起始的偏移索引</param>
	public AbStructHandle(byte[] source, int index)
	{
		ReturnCount = BitConverter.ToUInt16(source, index);
		TemplateObjectDefinitionSize = BitConverter.ToUInt32(source, index + 6);
		TemplateStructureSize = BitConverter.ToUInt32(source, index + 14);
		MemberCount = BitConverter.ToUInt16(source, index + 22);
		StructureHandle = BitConverter.ToUInt16(source, index + 28);
	}
}

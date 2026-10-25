// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Xml.Linq;

namespace HslCommunication.Profinet.AllenBradley;

/// <summary>
/// AB PLC的标签节点数据信息
/// </summary>
public class AllenBradleyItemValue
{
	/// <summary>
	/// 当前标签的名称信息
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 真实的数组缓存
	/// </summary>
	public byte[] Buffer { get; set; }

	/// <summary>
	/// 是否是数组的数据
	/// </summary>
	public bool IsArray { get; set; }

	/// <summary>
	/// 单个单位的数据长度信息
	/// </summary>
	public int TypeLength { get; set; } = 1;

	/// <summary>
	/// 数据类型信息
	/// </summary>
	public ushort TypeCode { get; set; } = 193;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public AllenBradleyItemValue()
	{
	}

	/// <summary>
	/// 指定XML元素的资源来实例化对象信息
	/// </summary>
	/// <param name="element">XML元素信息</param>
	public AllenBradleyItemValue(XElement element)
	{
		LoadByXml(element);
	}

	private string getTypeHelper(string type)
	{
		if (IsArray)
		{
			if (TypeLength == 0)
			{
				return type;
			}
			return $"{type}[{Buffer.Length / TypeLength}]";
		}
		return type ?? "";
	}

	/// <summary>
	/// 获取当前的数据类型名称描述
	/// </summary>
	/// <returns>类型信息，如果不支持解析，则返回类型代号</returns>
	public string GetTypeName()
	{
		if (TypeCode == 194)
		{
			return getTypeHelper("sbyte");
		}
		if (TypeCode == 195)
		{
			return getTypeHelper("short");
		}
		if (TypeCode == 196)
		{
			return getTypeHelper("int");
		}
		if (TypeCode == 197)
		{
			return getTypeHelper("long");
		}
		if (TypeCode == 198)
		{
			return getTypeHelper("byte");
		}
		if (TypeCode == 199)
		{
			return getTypeHelper("ushort");
		}
		if (TypeCode == 200)
		{
			return getTypeHelper("uint");
		}
		if (TypeCode == 201)
		{
			return getTypeHelper("ulong");
		}
		if (TypeCode == 202)
		{
			return getTypeHelper("float");
		}
		if (TypeCode == 203)
		{
			return getTypeHelper("double");
		}
		if (TypeCode == 208)
		{
			return getTypeHelper("string");
		}
		if (TypeCode == 209)
		{
			return getTypeHelper("string");
		}
		if (TypeCode == 193)
		{
			return getTypeHelper("bool");
		}
		return $"0x{TypeCode:X}";
	}

	/// <summary>
	/// 将值转换为同等描述的序列化字符串信息
	/// </summary>
	/// <returns>Xml元素</returns>
	public XElement ToXml()
	{
		XElement xElement = new XElement("AllenBradleyItemValue");
		xElement.SetAttributeValue("Name", Name);
		xElement.SetAttributeValue("TypeCode", TypeCode);
		xElement.SetAttributeValue("IsArray", IsArray);
		xElement.SetAttributeValue("TypeLength", TypeLength);
		if (Buffer != null)
		{
			xElement.SetAttributeValue("Buffer", Buffer.ToHexString());
		}
		return xElement;
	}

	private T GetXmlValue<T>(XElement element, string name, T defaultValue, Func<string, T> trans)
	{
		XAttribute xAttribute = element.Attribute(name);
		if (xAttribute == null)
		{
			return defaultValue;
		}
		try
		{
			return trans(xAttribute.Value);
		}
		catch
		{
			return defaultValue;
		}
	}

	/// <summary>
	/// 从xml元素加载当前的节点数据信息
	/// </summary>
	/// <param name="element">元素信息</param>
	public void LoadByXml(XElement element)
	{
		if (element.Name == "AllenBradleyItemValue")
		{
			Name = GetXmlValue(element, "Name", Name, (string m) => m);
			TypeCode = GetXmlValue(element, "TypeCode", TypeCode, ushort.Parse);
			IsArray = GetXmlValue(element, "IsArray", IsArray, bool.Parse);
			TypeLength = GetXmlValue(element, "TypeLength", TypeLength, int.Parse);
			Buffer = GetXmlValue(element, "Buffer", "", (string m) => m).ToHexBytes();
		}
	}
}

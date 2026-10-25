// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace HslCommunication;

/// <summary>
/// 用于网络传递的信息头，使用上等同于int
/// </summary>
/// <remarks>
/// 通常用于和之间的通信，以及和通讯
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
public struct NetHandle
{
	[FieldOffset(0)]
	/// <summary>
	/// 完整的暗号值
	/// </summary>
	private int m_CodeValue;

	[FieldOffset(3)]
	/// <summary>
	/// 主暗号分类0-255
	/// </summary>
	private byte m_CodeMajor;

	[FieldOffset(2)]
	/// <summary>
	/// 次要的暗号分类0-255
	/// </summary>
	private byte m_CodeMinor;

	[FieldOffset(0)]
	/// <summary>
	/// 暗号的编号分类0-65535
	/// </summary>
	private ushort m_CodeIdentifier;

	/// <summary>
	/// 完整的暗号值
	/// </summary>
	public int CodeValue
	{
		get
		{
			return m_CodeValue;
		}
		set
		{
			m_CodeValue = value;
		}
	}

	/// <summary>
	/// 主暗号分类0-255
	/// </summary>
	public byte CodeMajor
	{
		get
		{
			return m_CodeMajor;
		}
		private set
		{
			m_CodeMajor = value;
		}
	}

	/// <summary>
	/// 次要的暗号分类0-255
	/// </summary>
	public byte CodeMinor
	{
		get
		{
			return m_CodeMinor;
		}
		private set
		{
			m_CodeMinor = value;
		}
	}

	/// <summary>
	/// 暗号的编号分类0-65535
	/// </summary>
	public ushort CodeIdentifier
	{
		get
		{
			return m_CodeIdentifier;
		}
		private set
		{
			m_CodeIdentifier = value;
		}
	}

	/// <summary>
	/// 初始化一个暗号对象
	/// </summary>
	/// <param name="value">使用一个默认的数值进行初始化</param>
	public static implicit operator NetHandle(int value)
	{
		return new NetHandle(value);
	}

	public static implicit operator int(NetHandle netHandle)
	{
		return netHandle.m_CodeValue;
	}

	public static bool operator ==(NetHandle netHandle1, NetHandle netHandle2)
	{
		return netHandle1.CodeValue == netHandle2.CodeValue;
	}

	public static bool operator !=(NetHandle netHandle1, NetHandle netHandle2)
	{
		return netHandle1.CodeValue != netHandle2.CodeValue;
	}

	public static NetHandle operator +(NetHandle netHandle1, NetHandle netHandle2)
	{
		return new NetHandle(netHandle1.CodeValue + netHandle2.CodeValue);
	}

	public static NetHandle operator -(NetHandle netHandle1, NetHandle netHandle2)
	{
		return new NetHandle(netHandle1.CodeValue - netHandle2.CodeValue);
	}

	public static bool operator <(NetHandle netHandle1, NetHandle netHandle2)
	{
		return netHandle1.CodeValue < netHandle2.CodeValue;
	}

	public static bool operator >(NetHandle netHandle1, NetHandle netHandle2)
	{
		return netHandle1.CodeValue > netHandle2.CodeValue;
	}

	/// <summary>
	/// 初始化一个暗号对象
	/// </summary>
	/// <param name="value">使用一个默认的数值进行初始化</param>
	public NetHandle(int value)
	{
		m_CodeMajor = 0;
		m_CodeMinor = 0;
		m_CodeIdentifier = 0;
		m_CodeValue = value;
	}

	/// <summary>
	/// 根据三个值来初始化暗号对象
	/// </summary>
	/// <param name="major">主暗号</param>
	/// <param name="minor">次暗号</param>
	/// <param name="identifier">暗号编号</param>
	public NetHandle(byte major, byte minor, ushort identifier)
	{
		m_CodeValue = 0;
		m_CodeMajor = major;
		m_CodeMinor = minor;
		m_CodeIdentifier = identifier;
	}

	/// <summary>
	/// 获取完整的暗号数据
	/// </summary>
	/// <returns>返回暗号的字符串表示形式</returns>
	public override string ToString()
	{
		return m_CodeValue.ToString();
	}

	/// <summary>
	/// 判断两个实例是否相同
	/// </summary>
	/// <param name="obj">对比的对象</param>
	/// <returns>相同返回True，否则返回False</returns>
	public override bool Equals(object obj)
	{
		if (obj is NetHandle netHandle)
		{
			return CodeValue.Equals(netHandle.CodeValue);
		}
		return false;
	}

	/// <summary>
	/// 获取哈希值
	/// </summary>
	/// <returns>返回当前对象的哈希值</returns>
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

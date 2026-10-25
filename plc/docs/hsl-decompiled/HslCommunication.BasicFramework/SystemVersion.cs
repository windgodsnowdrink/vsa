// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.BasicFramework;

/// <summary>
/// 系统版本类，由三部分组成，包含了一个大版本，小版本，修订版，还有一个开发者维护的内部版
/// System version class, consisting of three parts, including a major version, minor version, revised version, and an internal version maintained by the developer
/// </summary>
[Serializable]
public sealed class SystemVersion
{
	private int m_MainVersion = 2;

	private int m_SecondaryVersion = 0;

	private int m_EditVersion = 0;

	private int m_InnerVersion = 0;

	public int MainVersion => m_MainVersion;

	public int SecondaryVersion => m_SecondaryVersion;

	public int EditVersion => m_EditVersion;

	public int InnerVersion => m_InnerVersion;

	/// <summary>
	/// 根据格式化字符串的版本号初始化，例如：1.0或1.0.0或1.0.0.0503
	/// Initialize according to the version number of the formatted string, for example: 1.0 or 1.0.0 or 1.0.0.0503
	/// </summary>
	/// <param name="VersionString">格式化的字符串，例如：1.0或1.0.0或1.0.0.0503</param>
	public SystemVersion(string VersionString)
	{
		string[] array = VersionString.SplitDot();
		if (array.Length >= 1)
		{
			m_MainVersion = Convert.ToInt32(array[0]);
		}
		if (array.Length >= 2)
		{
			m_SecondaryVersion = Convert.ToInt32(array[1]);
		}
		if (array.Length >= 3)
		{
			m_EditVersion = Convert.ToInt32(array[2]);
		}
		if (array.Length >= 4)
		{
			m_InnerVersion = Convert.ToInt32(array[3]);
		}
	}

	/// <summary>
	/// 根据指定的主版本，次版本，修订版来实例化一个对象
	/// Instantiate an object based on the specified major, minor, and revision
	/// </summary>
	/// <param name="main">主版本</param>
	/// <param name="sec">次版本</param>
	/// <param name="edit">修订版</param>
	public SystemVersion(int main, int sec, int edit)
	{
		m_MainVersion = main;
		m_SecondaryVersion = sec;
		m_EditVersion = edit;
	}

	/// <summary>
	/// 根据指定的主版本，次版本，修订版，内部版本来实例化一个对象
	/// Instantiate an object based on the specified major, minor, revision, and build
	/// </summary>
	/// <param name="main">主版本</param>
	/// <param name="sec">次版本</param>
	/// <param name="edit">修订版</param>
	/// <param name="inner">内部版本号</param>
	public SystemVersion(int main, int sec, int edit, int inner)
	{
		m_MainVersion = main;
		m_SecondaryVersion = sec;
		m_EditVersion = edit;
		m_InnerVersion = inner;
	}

	/// <summary>
	/// 根据格式化为支持返回的不同信息的版本号
	/// C返回1.0.0.0
	/// N返回1.0.0
	/// S返回1.0
	/// </summary>
	/// <param name="format">格式化信息</param>
	/// <returns>版本号信息</returns>
	public string ToString(string format)
	{
		return format switch
		{
			"C" => $"{MainVersion}.{SecondaryVersion}.{EditVersion}.{InnerVersion}", 
			"N" => $"{MainVersion}.{SecondaryVersion}.{EditVersion}", 
			"S" => $"{MainVersion}.{SecondaryVersion}", 
			_ => ToString(), 
		};
	}

	/// <summary>
	/// 获取版本号的字符串形式，如果内部版本号为0，则显示时不携带
	/// </summary>
	/// <returns>版本号信息</returns>
	public override string ToString()
	{
		if (InnerVersion == 0)
		{
			return ToString("N");
		}
		return ToString("C");
	}

	/// <summary>
	/// 判断两个实例是否相等
	/// </summary>
	/// <param name="obj">版本号</param>
	/// <returns>是否一致</returns>
	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	/// <summary>
	/// 获取哈希值
	/// </summary>
	/// <returns>哈希值</returns>
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(SystemVersion SV1, SystemVersion SV2)
	{
		if (SV1.MainVersion != SV2.MainVersion)
		{
			return false;
		}
		if (SV1.SecondaryVersion != SV2.SecondaryVersion)
		{
			return false;
		}
		if (SV1.m_EditVersion != SV2.m_EditVersion)
		{
			return false;
		}
		if (SV1.InnerVersion != SV2.InnerVersion)
		{
			return false;
		}
		return true;
	}

	public static bool operator !=(SystemVersion SV1, SystemVersion SV2)
	{
		if (SV1.MainVersion != SV2.MainVersion)
		{
			return true;
		}
		if (SV1.SecondaryVersion != SV2.SecondaryVersion)
		{
			return true;
		}
		if (SV1.m_EditVersion != SV2.m_EditVersion)
		{
			return true;
		}
		if (SV1.InnerVersion != SV2.InnerVersion)
		{
			return true;
		}
		return false;
	}

	public static bool operator >(SystemVersion SV1, SystemVersion SV2)
	{
		if (SV1.MainVersion > SV2.MainVersion)
		{
			return true;
		}
		if (SV1.MainVersion < SV2.MainVersion)
		{
			return false;
		}
		if (SV1.SecondaryVersion > SV2.SecondaryVersion)
		{
			return true;
		}
		if (SV1.SecondaryVersion < SV2.SecondaryVersion)
		{
			return false;
		}
		if (SV1.EditVersion > SV2.EditVersion)
		{
			return true;
		}
		if (SV1.EditVersion < SV2.EditVersion)
		{
			return false;
		}
		if (SV1.InnerVersion > SV2.InnerVersion)
		{
			return true;
		}
		if (SV1.InnerVersion < SV2.InnerVersion)
		{
			return false;
		}
		return false;
	}

	public static bool operator <(SystemVersion SV1, SystemVersion SV2)
	{
		if (SV1.MainVersion < SV2.MainVersion)
		{
			return true;
		}
		if (SV1.MainVersion > SV2.MainVersion)
		{
			return false;
		}
		if (SV1.SecondaryVersion < SV2.SecondaryVersion)
		{
			return true;
		}
		if (SV1.SecondaryVersion > SV2.SecondaryVersion)
		{
			return false;
		}
		if (SV1.EditVersion < SV2.EditVersion)
		{
			return true;
		}
		if (SV1.EditVersion > SV2.EditVersion)
		{
			return false;
		}
		if (SV1.InnerVersion < SV2.InnerVersion)
		{
			return true;
		}
		if (SV1.InnerVersion > SV2.InnerVersion)
		{
			return false;
		}
		return false;
	}
}

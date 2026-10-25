// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;

namespace HslCommunication.Reflection;

/// <summary>
/// 可以指定方法的权限内容，可以限定MQTT会话的ClientID信息或是UserName内容
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HslMqttPermissionAttribute : Attribute
{
	/// <summary>
	/// ClientId的限定内容
	/// </summary>
	public string ClientID { get; set; }

	/// <summary>
	/// UserName的限定内容
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// 检查当前的客户端ID是否通过
	/// </summary>
	/// <param name="clientID">ID信息</param>
	/// <returns>是否检测成功</returns>
	public virtual bool CheckClientID(string clientID)
	{
		if (string.IsNullOrEmpty(ClientID))
		{
			return true;
		}
		return ClientID == clientID;
	}

	/// <summary>
	/// 检查当前的用户名是否通过
	/// </summary>
	/// <param name="name">用户名</param>
	/// <returns>是否检测成功</returns>
	public virtual bool CheckUserName(string name)
	{
		if (string.IsNullOrEmpty(UserName))
		{
			return true;
		}
		return UserName == name;
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HslCommunication.BasicFramework;

[Serializable]
public sealed class Exception<TExceptionArgs> : Exception, ISerializable where TExceptionArgs : ExceptionArgs
{
	private const string c_args = "Args";

	private readonly TExceptionArgs m_args;

	public TExceptionArgs Args => m_args;

	public override string Message
	{
		get
		{
			string message = base.Message;
			return (m_args == null) ? message : (message + " (" + m_args.Message + ")");
		}
	}

	public Exception(string message = null, Exception innerException = null)
		: this((TExceptionArgs)null, message, innerException)
	{
	}

	public Exception(TExceptionArgs args, string message = null, Exception innerException = null)
		: base(message, innerException)
	{
		m_args = args;
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	private Exception(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		m_args = (TExceptionArgs)info.GetValue("Args", typeof(TExceptionArgs));
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Args", m_args);
		base.GetObjectData(info, context);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Exception<TExceptionArgs> ex))
		{
			return false;
		}
		return object.Equals(m_args, ex.m_args) && base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

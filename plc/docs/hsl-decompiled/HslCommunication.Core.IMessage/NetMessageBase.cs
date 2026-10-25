// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.IO;

namespace HslCommunication.Core.IMessage;

/// <summary>
/// 消息类的基类
/// </summary>
public class NetMessageBase
{
	/// <summary>
	/// 消息头字节
	/// Message header byte
	/// </summary>
	public byte[] HeadBytes { get; set; }

	/// <summary>
	/// 消息内容字节
	/// Message content byte
	/// </summary>
	public byte[] ContentBytes { get; set; }

	/// <summary>
	/// 发送的字节信息
	/// Byte information sent
	/// </summary>
	public byte[] SendBytes { get; set; }

	/// <summary>
	/// 在接收头报文的时候，返回前置无效的报文头字节长度，默认为0，不处理
	/// When receiving a header message, return the header byte length of the invalid header, the default is 0, and no processing is performed.
	/// </summary>
	/// <param name="headByte">接收到的头子节信息</param>
	/// <returns>头子节中无效的字节长度信息</returns>
	public virtual int PependedUselesByteLength(byte[] headByte)
	{
		return 0;
	}

	/// <summary>
	/// 获取头子节里的消息标识
	/// Get the message ID in the header subsection
	/// </summary>
	/// <returns>消息标识</returns>
	public virtual int GetHeadBytesIdentity()
	{
		return 0;
	}

	/// <summary>
	/// 检查头子节的合法性
	/// Check the legitimacy of the head subsection
	/// </summary>
	/// <param name="token">特殊的令牌，有些特殊消息的验证</param>
	/// <returns>是否成功的结果</returns>
	public virtual bool CheckHeadBytesLegal(byte[] token)
	{
		return true;
	}

	/// <summary>
	/// 当消息头报文的长度定义为-1的时候，则使用动态的长度信息，可以使用本方法来判断一个消息是否处于完整的状态。
	/// If the length of the message header is defined as -1, this method can be used to determine whether a message is in the complete state by using dynamic length information.
	/// </summary>
	/// <param name="send">发送的消息</param>
	/// <param name="ms">接收到的数据内容</param>
	/// <returns>是否是完整的消息</returns>
	public virtual bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		return true;
	}

	/// <summary>
	/// 检查发送的接收的报文是否是匹配的，如果匹配，则返回 1,  如果不匹配且直接返回错误，则返回 0，如果不匹配继续接收，直到匹配或是超时，则返回 -1
	/// If the packet is matched, 1 is returned. If the packet is not matched and an error is returned, 0 is returned. If the packet is not matched, -1 is returned until the packet is matched or times out
	/// </summary>
	/// <param name="send">当前发送的报文</param>
	/// <param name="receive">当前接收的报文信息</param>
	/// <returns>如果匹配，则返回 1,  如果不匹配且直接返回错误，则返回 0，如果不匹配继续接收，直到匹配或是超时，则返回 -1</returns>
	/// <remarks>
	/// 在某些协议里，存在一个消息id，发送方的消息id和返回的消息id是必须一致的。
	/// </remarks>
	public virtual int CheckMessageMatch(byte[] send, byte[] receive)
	{
		return 1;
	}

	public override string ToString()
	{
		return GetType().Name ?? "";
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using HslCommunication.Core.Net;

namespace HslCommunication.Enthernet;

/// <summary>
/// UDP客户端的类，负责发送数据到服务器，然后从服务器接收对应的数据信息，该数据经过HSL封装
/// UDP client class, responsible for sending data to the server, and then receiving the corresponding data information from the server, the data is encapsulated by HSL
/// </summary>
public class NetUdpClient : NetworkUdpBase
{
	/// <summary>
	/// 实例化对象，指定发送的服务器地址和端口号
	/// Instantiated object, specifying the server address and port number to send
	/// </summary>
	/// <param name="ipAddress">服务器的Ip地址</param>
	/// <param name="port">端口号</param>
	public NetUdpClient(string ipAddress, int port)
	{
		IpAddress = ipAddress;
		Port = port;
	}

	/// <summary>
	/// 客户端向服务器进行请求，请求字符串数据，忽略了自定义消息反馈
	/// The client makes a request to the server, requesting string data, and ignoring custom message feedback
	/// </summary>
	/// <param name="customer">用户的指令头</param>
	/// <param name="send">发送数据</param>
	/// <returns>带返回消息的结果对象</returns>
	public OperateResult<string> ReadFromServer(NetHandle customer, string send = null)
	{
		OperateResult<byte[]> operateResult = ReadFromServerBase(HslProtocol.CommandBytes(customer, base.Token, send));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		return OperateResult.CreateSuccessResult(Encoding.Unicode.GetString(operateResult.Content));
	}

	/// <summary>
	/// 客户端向服务器进行请求，请求字节数据
	/// The client makes a request to the server, requesting byte data
	/// </summary>
	/// <param name="customer">用户的指令头</param>
	/// <param name="send">发送的字节内容</param>
	/// <returns>带返回消息的结果对象</returns>
	public OperateResult<byte[]> ReadFromServer(NetHandle customer, byte[] send)
	{
		return ReadFromServerBase(HslProtocol.CommandBytes(customer, base.Token, send));
	}

	/// <summary>
	/// 客户端向服务器进行请求，请求字符串数据，并返回状态信息
	/// The client makes a request to the server, requests string data, and returns status information
	/// </summary>
	/// <param name="customer">用户的指令头</param>
	/// <param name="send">发送数据</param>
	/// <returns>带返回消息的结果对象</returns>
	public OperateResult<NetHandle, string> ReadCustomerFromServer(NetHandle customer, string send = null)
	{
		OperateResult<NetHandle, byte[]> operateResult = ReadCustomerFromServerBase(HslProtocol.CommandBytes(customer, base.Token, send));
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<NetHandle, string>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content1, Encoding.Unicode.GetString(operateResult.Content2));
	}

	/// <summary>
	/// 客户端向服务器进行请求，请求字节数据，并返回状态信息
	/// The client makes a request to the server, requests byte data, and returns status information
	/// </summary>
	/// <param name="customer">用户的指令头</param>
	/// <param name="send">发送数据</param>
	/// <returns>带返回消息的结果对象</returns>
	public OperateResult<NetHandle, byte[]> ReadCustomerFromServer(NetHandle customer, byte[] send)
	{
		return ReadCustomerFromServerBase(HslProtocol.CommandBytes(customer, base.Token, send));
	}

	/// <summary>
	/// 发送的底层数据，然后返回结果数据
	/// Send the underlying data and then return the result data
	/// </summary>
	/// <param name="send">需要发送的底层数据</param>
	/// <returns>带返回消息的结果对象</returns>
	private OperateResult<byte[]> ReadFromServerBase(byte[] send)
	{
		OperateResult<NetHandle, byte[]> operateResult = ReadCustomerFromServerBase(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content2);
	}

	/// <summary>
	/// 发送的底层数据，然后返回结果数据，该结果是带Handle信息的。
	/// Send the underlying data, and then return the result data, the result is with Handle information.
	/// </summary>
	/// <param name="send">需要发送的底层数据</param>
	/// <returns>带返回消息的结果对象</returns>
	private OperateResult<NetHandle, byte[]> ReadCustomerFromServerBase(byte[] send)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<NetHandle, byte[]>(operateResult);
		}
		return HslProtocol.ExtractHslData(operateResult.Content);
	}

	public override string ToString()
	{
		return $"NetUdpClient[{IpAddress}:{Port}]";
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System.Text;
using System.Threading.Tasks;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Core.Pipe;

namespace HslCommunication.WebSocket;

/// <summary>
/// WebSocket的问答机制的客户端，本客户端将会在请求头上追加 RequestAndAnswer: true，本客户端将会请求服务器的信息，然后等待服务器的返回
/// Client of WebSocket Q & A mechanism, this client will append RequestAndAnswer: true to the request header, this client will request the server information, and then wait for the server to return
/// </summary>
public class WebSocketQANet : TcpNetCommunication
{
	/// <summary>
	/// 根据指定的ip地址及端口号，实例化一个默认的对象
	/// Instantiates a default object based on the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">远程服务器的ip地址</param>
	/// <param name="port">端口号信息</param>
	public WebSocketQANet(string ipAddress, int port)
	{
		IpAddress = HslHelper.GetIpAddressFromInput(ipAddress);
		Port = port;
	}

	protected override OperateResult InitializationOnConnect()
	{
		byte[] data = WebSocketHelper.BuildWsQARequest(IpAddress, Port);
		OperateResult operateResult = CommunicationPipe.Send(data);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = CommunicationPipe.Receive(-1, 10000);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		return OperateResult.CreateSuccessResult();
	}

	public override OperateResult<byte[]> ReadFromCoreServer(CommunicationPipe pipe, byte[] send, bool hasResponseData = true, bool usePackHeader = true)
	{
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send, ' '));
		OperateResult operateResult = pipe.Send(send);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		if (base.ReceiveTimeOut < 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		OperateResult<WebSocketMessage> operateResult2 = WebSocketHelper.ReceiveWebSocketPayload(pipe);
		if (!operateResult2.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult2);
		}
		base.LogNet?.WriteDebug(ToString(), $"{StringResources.Language.Receive} : OpCode[{operateResult2.Content.OpCode}] Mask[{operateResult2.Content.HasMask}] {SoftBasic.ByteToHexString(operateResult2.Content.Payload, ' ')}");
		return OperateResult.CreateSuccessResult(operateResult2.Content.Payload);
	}

	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		byte[] command = WebSocketHelper.BuildWsQARequest(IpAddress, Port);
		OperateResult send = await CommunicationPipe.SendAsync(command);
		if (!send.IsSuccess)
		{
			return send;
		}
		OperateResult<byte[]> rece = await CommunicationPipe.ReceiveAsync(-1, 10000);
		if (!rece.IsSuccess)
		{
			return rece;
		}
		return OperateResult.CreateSuccessResult();
	}

	public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(CommunicationPipe pipe, byte[] send, bool hasResponseData = true, bool usePackHeader = true)
	{
		base.LogNet?.WriteDebug(ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send, ' '));
		OperateResult sendResult = await pipe.SendAsync(send);
		if (!sendResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(sendResult);
		}
		if (base.ReceiveTimeOut < 0)
		{
			return OperateResult.CreateSuccessResult(new byte[0]);
		}
		OperateResult<WebSocketMessage> read = await WebSocketHelper.ReceiveWebSocketPayloadAsync(pipe).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(read);
		}
		base.LogNet?.WriteDebug(ToString(), $"{StringResources.Language.Receive} : OpCode[{read.Content.OpCode}] Mask[{read.Content.HasMask}] {SoftBasic.ByteToHexString(read.Content.Payload, ' ')}");
		return OperateResult.CreateSuccessResult(read.Content.Payload);
	}

	/// <summary>
	/// 和websocket的服务器交互，将负载数据发送到服务器端，然后等待接收服务器的数据
	/// Interact with the websocket server, send the load data to the server, and then wait to receive data from the server
	/// </summary>
	/// <param name="payload">数据负载</param>
	/// <returns>返回的结果数据</returns>
	public OperateResult<string> ReadFromServer(string payload)
	{
		return ByteTransformHelper.GetSuccessResultFromOther(ReadFromCoreServer(WebSocketHelper.WebScoketPackData(1, isMask: true, payload)), Encoding.UTF8.GetString);
	}

	/// <summary>
	/// 和websocket的服务器交互，将负载数据发送到服务器端，然后等待接收服务器的数据
	/// Interact with the websocket server, send the load data to the server, and then wait to receive data from the server
	/// </summary>
	/// <param name="payload">数据负载</param>
	/// <returns>返回的结果数据</returns>
	public async Task<OperateResult<string>> ReadFromServerAsync(string payload)
	{
		return ByteTransformHelper.GetSuccessResultFromOther(await ReadFromCoreServerAsync(WebSocketHelper.WebScoketPackData(1, isMask: true, payload)), Encoding.UTF8.GetString);
	}

	public override string ToString()
	{
		return $"WebSocketQANet[{IpAddress}:{Port}]";
	}
}

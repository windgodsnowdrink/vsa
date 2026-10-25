// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace HslCommunication.Core.Pipe;

/// <summary>
/// 基于SSL/TLS加密的管道信息，内部基于 TCP/IP 通信实现
/// Pipe information based on SSL/TLS encryption, and internal TCP/IP communication
/// </summary>
public class PipeSslNet : PipeTcpNet
{
	private bool isServerMode = true;

	private NetworkStream networkStream;

	private SslStream sslStream = null;

	private X509Certificate certificate = null;

	/// <summary>
	/// 获取或设置当前的证书内容，
	/// </summary>
	public X509Certificate Certificate
	{
		get
		{
			return certificate;
		}
		set
		{
			certificate = value;
		}
	}

	/// <summary>
	/// 获取或设置是否检查远程的证书，默认为不检查
	/// </summary>
	public bool RemoteCertificateCheck { get; set; } = false;

	/// <summary>
	/// 获取或设置当前的 SSL/TLS 协议版本，默认为 Tls | Tls11 | Tls12
	/// </summary>
	public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls;

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeSslNet(bool serverMode)
	{
		isServerMode = serverMode;
		sslInit();
	}

	/// <summary>
	/// 通过指定的IP地址和端口号来实例化一个对象
	/// Instantiate an object with the specified IP address and port number
	/// </summary>
	/// <param name="ipAddress">IP地址信息</param>
	/// <param name="port">端口号</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeSslNet(string ipAddress, int port, bool serverMode)
		: base(ipAddress, port)
	{
		isServerMode = serverMode;
		sslInit();
	}

	/// <summary>
	/// 通过指定的套接字及连接的终结点来实例化一个对象
	/// Instantiate an object by specifying the socket and the endpoint of the connection
	/// </summary>
	/// <param name="socket">连接的套接字对象</param>
	/// <param name="iPEndPoint">连接的远程地址信息</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeSslNet(Socket socket, IPEndPoint iPEndPoint, bool serverMode)
		: base(socket, iPEndPoint)
	{
		isServerMode = serverMode;
		sslInit();
	}

	/// <summary>
	/// 通过指定的套接字及连接的终结点来实例化一个对象
	/// Instantiate an object by specifying the socket and the endpoint of the connection
	/// </summary>
	/// <param name="socket">连接的套接字对象</param>
	/// <param name="iPEndPoint">连接的远程地址信息</param>
	/// <param name="serverMode">是否为服务器模式</param>
	public PipeSslNet(Socket socket, bool serverMode)
	{
		isServerMode = serverMode;
		base.Socket = socket;
		sslInit();
		CreateSslStream(socket, createNew: true);
	}

	private void sslInit()
	{
		SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
	}

	/// <summary>
	/// 使用一个证书路径来初始化 SSL/TLS 通信
	/// Use a certificate path to initialize SSL/TLS communication
	/// </summary>
	/// <param name="certificateFile">证书路径</param>
	public void SetCertficate(string certificateFile)
	{
		if (!string.IsNullOrEmpty(certificateFile))
		{
			certificate = X509Certificate.CreateFromCertFile(certificateFile);
		}
	}

	protected override OperateResult OnCommunicationOpen(Socket socket)
	{
		OperateResult<SslStream> operateResult = CreateSslStream(socket, createNew: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		return base.OnCommunicationOpen(socket);
	}

	/// <summary>
	/// 当前的管道创建一个 SSL/TLS 加密的流
	/// The current pipeline creates an SSL/TLS encrypted stream
	/// </summary>
	/// <param name="socket">套接字对象</param>
	/// <param name="createNew">是否创建一个新的流</param>
	/// <returns>如果创建成功，返回流对象</returns>
	public OperateResult<SslStream> CreateSslStream(Socket socket, bool createNew = false)
	{
		if (createNew)
		{
			networkStream?.Close();
			sslStream?.Close();
			networkStream = new NetworkStream(socket, ownsSocket: false);
			sslStream = new SslStream(networkStream, leaveInnerStreamOpen: false, ValidateCertificate, null);
			try
			{
				if (isServerMode)
				{
					sslStream.AuthenticateAsServer(certificate, clientCertificateRequired: false, SslProtocols, checkCertificateRevocation: true);
					return OperateResult.CreateSuccessResult(sslStream);
				}
				if (certificate == null)
				{
					sslStream.AuthenticateAsClient(host);
				}
				else
				{
					X509CertificateCollection clientCertificates = new X509CertificateCollection(new X509Certificate[1] { certificate });
					sslStream.AuthenticateAsClient(host, clientCertificates, SslProtocols, checkCertificateRevocation: false);
				}
				return OperateResult.CreateSuccessResult(sslStream);
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					return new OperateResult<SslStream>(ex.InnerException.Message);
				}
				return new OperateResult<SslStream>(ex.Message);
			}
		}
		return OperateResult.CreateSuccessResult(sslStream);
	}

	private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		if (sslPolicyErrors == SslPolicyErrors.None)
		{
			return true;
		}
		return !RemoteCertificateCheck;
	}

	public override OperateResult Send(byte[] data, int offset, int size)
	{
		OperateResult<SslStream> operateResult = CreateSslStream(base.Socket);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(operateResult);
		}
		OperateResult operateResult2 = NetSupport.SocketSend(operateResult.Content, data, offset, size);
		if (!operateResult2.IsSuccess && operateResult2.ErrorCode == NetSupport.SocketErrorCode)
		{
			CloseCommunication();
			return new OperateResult<byte[]>(-IncrConnectErrorCount(), operateResult2.Message);
		}
		return operateResult2;
	}

	public override OperateResult<int> Receive(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		OperateResult<SslStream> operateResult = CreateSslStream(base.Socket);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<int>(operateResult);
		}
		OperateResult<int> operateResult2 = NetSupport.SocketReceive(operateResult.Content, buffer, offset, length, timeOut, reportProgress);
		if (!operateResult2.IsSuccess && operateResult2.ErrorCode == NetSupport.SocketErrorCode)
		{
			CloseCommunication();
			return new OperateResult<int>(-IncrConnectErrorCount(), "Socket Exception -> " + operateResult2.Message);
		}
		return operateResult2;
	}

	public override async Task<OperateResult> SendAsync(byte[] data, int offset, int size)
	{
		OperateResult<SslStream> ssl = CreateSslStream(base.Socket);
		if (!ssl.IsSuccess)
		{
			return OperateResult.CreateFailedResult<string>(ssl);
		}
		OperateResult send = await NetSupport.SocketSendAsync(ssl.Content, data, offset, size).ConfigureAwait(continueOnCapturedContext: false);
		if (!send.IsSuccess && send.ErrorCode == NetSupport.SocketErrorCode)
		{
			await CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
			return new OperateResult<byte[]>(-IncrConnectErrorCount(), send.Message);
		}
		return send;
	}

	public override async Task<OperateResult<int>> ReceiveAsync(byte[] buffer, int offset, int length, int timeOut = 60000, Action<long, long> reportProgress = null)
	{
		OperateResult<SslStream> ssl = CreateSslStream(base.Socket);
		if (!ssl.IsSuccess)
		{
			return OperateResult.CreateFailedResult<int>(ssl);
		}
		OperateResult<int> receive = await NetSupport.SocketReceiveAsync(ssl.Content, buffer, offset, length, timeOut, reportProgress).ConfigureAwait(continueOnCapturedContext: false);
		if (!receive.IsSuccess && receive.ErrorCode == NetSupport.SocketErrorCode)
		{
			await CloseCommunicationAsync().ConfigureAwait(continueOnCapturedContext: false);
			return new OperateResult<int>(-IncrConnectErrorCount(), "Socket Exception -> " + receive.Message);
		}
		return receive;
	}

	public override string ToString()
	{
		return $"PipeSslNet[{host}:{base.Port}]";
	}
}

// -----------------------------------------------------------------------------
// 反编译产物 | 来源程序集: HslCommunication.dll (v13.0.2.0)
// SHA-256: F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30
// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)
// 生成日期  : 2026-10-18
// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）
// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。
// -----------------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using HslCommunication.Algorithms.ConnectPool;

namespace HslCommunication.MQTT;

/// <summary>
/// [商业授权] MqttSyncClient客户端的连接池类对象，用于共享当前的连接池，合理的动态调整连接对象，然后进行高效通信的操作，默认连接数无限大。
/// [Authorization] The connection pool class object of the MqttSyncClient is used to share the current connection pool,
/// reasonably dynamically adjust the connection object, and then perform efficient communication operations,
/// The default number of connections is unlimited
/// </summary>
/// <remarks>
/// 本连接池用于提供高并发的读写性能，仅对商业授权用户开放。使用起来和一致，但是更加的高性能，在密集型数据交互时，优势尤为明显。
/// </remarks>
public class MqttSyncClientPool
{
	private MqttConnectionOptions connectionOptions;

	private ConnectPool<IMqttSyncConnector> mqttConnectPool;

	public ConnectPool<IMqttSyncConnector> GetMqttSyncConnectPool => mqttConnectPool;

	/// <summary>
	/// 获取或设置最大的连接数，当实际的连接数超过最大的连接数的时候，就会进行阻塞，直到有新的连接对象为止。
	/// Get or set the maximum number of connections. When the actual number of connections exceeds the maximum number of connections,
	/// it will block until there is a new connection object.
	/// </summary>
	public int MaxConnector
	{
		get
		{
			return mqttConnectPool.MaxConnector;
		}
		set
		{
			mqttConnectPool.MaxConnector = value;
		}
	}

	/// <summary>
	/// 通过MQTT连接参数实例化一个对象
	/// Instantiate an object through MQTT connection parameters
	/// </summary>
	/// <param name="options">MQTT的连接参数信息</param>
	public MqttSyncClientPool(MqttConnectionOptions options)
	{
		connectionOptions = options;
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			mqttConnectPool = new ConnectPool<IMqttSyncConnector>(() => new IMqttSyncConnector(options));
			mqttConnectPool.MaxConnector = int.MaxValue;
			return;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	/// <summary>
	/// 通过MQTT连接参数以及自定义的初始化方法来实例化一个对象
	/// Instantiate an object through MQTT connection parameters and custom initialization methods
	/// </summary>
	/// <param name="options">MQTT的连接参数信息</param>
	/// <param name="initialize">自定义的初始化方法</param>
	public MqttSyncClientPool(MqttConnectionOptions options, Action<MqttSyncClient> initialize)
	{
		connectionOptions = options;
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			mqttConnectPool = new ConnectPool<IMqttSyncConnector>(delegate
			{
				MqttSyncClient mqttSyncClient = new MqttSyncClient(options);
				initialize(mqttSyncClient);
				return new IMqttSyncConnector
				{
					SyncClient = mqttSyncClient
				};
			});
			mqttConnectPool.MaxConnector = int.MaxValue;
			return;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private OperateResult<T> ConnectPoolExecute<T>(Func<MqttSyncClient, OperateResult<T>> exec)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IMqttSyncConnector availableConnector = mqttConnectPool.GetAvailableConnector();
			OperateResult<T> result = exec(availableConnector.SyncClient);
			mqttConnectPool.ReturnConnector(availableConnector);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private OperateResult<T1, T2> ConnectPoolExecute<T1, T2>(Func<MqttSyncClient, OperateResult<T1, T2>> exec)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IMqttSyncConnector availableConnector = mqttConnectPool.GetAvailableConnector();
			OperateResult<T1, T2> result = exec(availableConnector.SyncClient);
			mqttConnectPool.ReturnConnector(availableConnector);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private async Task<OperateResult<T>> ConnectPoolExecuteAsync<T>(Func<MqttSyncClient, Task<OperateResult<T>>> exec)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IMqttSyncConnector client = mqttConnectPool.GetAvailableConnector();
			OperateResult<T> result = await exec(client.SyncClient);
			mqttConnectPool.ReturnConnector(client);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private async Task<OperateResult<T1, T2>> ConnectPoolExecuteAsync<T1, T2>(Func<MqttSyncClient, Task<OperateResult<T1, T2>>> execAsync)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IMqttSyncConnector client = mqttConnectPool.GetAvailableConnector();
			OperateResult<T1, T2> result = await execAsync(client.SyncClient);
			mqttConnectPool.ReturnConnector(client);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将payload发送到服务器，然后从服务器返回相关的数据，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the payload to the server, and then return relevant data from the server,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	/// <remarks>
	/// 进度报告可以实现一个比较有意思的功能，可以用来数据的上传和下载，提供一个友好的进度条，因为网络的好坏通常是不确定的。
	/// </remarks>
	public OperateResult<string, byte[]> Read(string topic, byte[] payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.Read(topic, payload, sendProgress, handleProgress, receiveProgress));
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将指定编码的字符串payload发送到服务器，然后从服务器返回相关的数据，并转换为指定编码的字符串，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the specified encoded string payload to the server,
	/// and then return the data from the server, and convert it to the specified encoded string,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	public OperateResult<string, string> ReadString(string topic, string payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadString(topic, payload, sendProgress, handleProgress, receiveProgress));
	}

	public OperateResult<T> ReadRpc<T>(string topic, string payload)
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadRpc<T>(topic, payload));
	}

	public OperateResult<T> ReadRpc<T>(string topic, object payload)
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadRpc<T>(topic, payload));
	}

	/// <summary>
	/// 读取服务器的已经注册的API信息列表，将返回API的主题路径，注释信息，示例的传入的数据信息。
	/// Read the registered API information list of the server, and return the API subject path, annotation information, and sample incoming data information.
	/// </summary>
	/// <returns>包含是否成功的api信息的列表</returns>
	public OperateResult<MqttRpcApiInfo[]> ReadRpcApis()
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadRpcApis());
	}

	/// <summary>
	/// 读取服务器的指定的API接口的每天的调用次数，如果API接口不存在，或是还没有调用数据，则返回失败。
	/// Read the number of calls per day of the designated API interface of the server.
	/// If the API interface does not exist or the data has not been called yet, it returns a failure.
	/// </summary>
	/// <param name="api">等待请求的API的接口信息，如果为空，就是请求所有的接口的调用的统计信息。</param>
	/// <returns>最近几日的连续的调用情况，例如[1,2,3]，表示前提调用1次，昨天调用2次，今天3次</returns>
	/// <remarks>
	/// 如果api的参数为空字符串，就是请求所有的接口的调用的统计信息。
	/// </remarks>
	public OperateResult<long[]> ReadRpcApiLog(string api)
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadRpcApiLog(api));
	}

	/// <summary>
	/// 读取服务器的已经驻留的所有消息的主题列表
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <returns>消息列表对象</returns>
	public OperateResult<string[]> ReadRetainTopics()
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadRetainTopics());
	}

	/// <summary>
	/// 读取服务器的已经驻留的指定主题的消息内容
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <param name="topic">指定的主题消息</param>
	/// <param name="receiveProgress">结果进度报告</param>
	/// <returns>消息列表对象</returns>
	public OperateResult<MqttClientApplicationMessage> ReadTopicPayload(string topic, Action<long, long> receiveProgress = null)
	{
		return ConnectPoolExecute((MqttSyncClient m) => m.ReadTopicPayload(topic, receiveProgress));
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将payload发送到服务器，然后从服务器返回相关的数据，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the payload to the server, and then return relevant data from the server,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	/// <remarks>
	/// 进度报告可以实现一个比较有意思的功能，可以用来数据的上传和下载，提供一个友好的进度条，因为网络的好坏通常是不确定的。
	/// </remarks>
	public async Task<OperateResult<string, byte[]>> ReadAsync(string topic, byte[] payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadAsync(topic, payload, sendProgress, handleProgress, receiveProgress));
	}

	/// <summary>
	/// 从MQTT服务器同步读取数据，将指定编码的字符串payload发送到服务器，然后从服务器返回相关的数据，并转换为指定编码的字符串，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作
	/// Synchronously read data from the MQTT server, send the specified encoded string payload to the server,
	/// and then return the data from the server, and convert it to the specified encoded string,
	/// support data transmission progress report, the server executes the progress report, and receives the data progress report
	/// </summary>
	/// <param name="topic">主题信息</param>
	/// <param name="payload">负载数据</param>
	/// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
	/// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
	/// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
	/// <returns>服务器返回的数据信息</returns>
	public async Task<OperateResult<string, string>> ReadStringAsync(string topic, string payload, Action<long, long> sendProgress = null, Action<string, string> handleProgress = null, Action<long, long> receiveProgress = null)
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadStringAsync(topic, payload, sendProgress, handleProgress, receiveProgress));
	}

	public async Task<OperateResult<T>> ReadRpcAsync<T>(string topic, string payload)
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadRpcAsync<T>(topic, payload));
	}

	public async Task<OperateResult<T>> ReadRpcAsync<T>(string topic, object payload)
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadRpcAsync<T>(topic, payload));
	}

	/// <summary>
	/// 读取服务器的已经注册的API信息列表，将返回API的主题路径，注释信息，示例的传入的数据信息。
	/// Read the registered API information list of the server, and return the API subject path, annotation information, and sample incoming data information.
	/// </summary>
	/// <returns>包含是否成功的api信息的列表</returns>
	public async Task<OperateResult<MqttRpcApiInfo[]>> ReadRpcApisAsync()
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadRpcApisAsync());
	}

	/// <summary>
	/// 读取服务器的指定的API接口的每天的调用次数，如果API接口不存在，或是还没有调用数据，则返回失败。
	/// Read the number of calls per day of the designated API interface of the server.
	/// If the API interface does not exist or the data has not been called yet, it returns a failure.
	/// </summary>
	/// <param name="api">等待请求的API的接口信息，如果为空，就是请求所有的接口的调用的统计信息。</param>
	/// <returns>最近几日的连续的调用情况，例如[1,2,3]，表示前提调用1次，昨天调用2次，今天3次</returns>
	/// <remarks>
	/// 如果api的参数为空字符串，就是请求所有的接口的调用的统计信息。
	/// </remarks>
	public async Task<OperateResult<long[]>> ReadRpcApiLogAsync(string api)
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadRpcApiLogAsync(api));
	}

	/// <summary>
	/// 读取服务器的已经驻留的所有消息的主题列表
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <returns>消息列表对象</returns>
	public async Task<OperateResult<string[]>> ReadRetainTopicsAsync()
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadRetainTopicsAsync());
	}

	/// <summary>
	/// 读取服务器的已经驻留的指定主题的消息内容
	/// Read the topic list of all messages that have resided on the server
	/// </summary>
	/// <param name="topic">指定的主题消息</param>
	/// <param name="receiveProgress">结果进度报告</param>
	/// <returns>消息列表对象</returns>
	public async Task<OperateResult<MqttClientApplicationMessage>> ReadTopicPayloadAsync(string topic, Action<long, long> receiveProgress = null)
	{
		return await ConnectPoolExecuteAsync((MqttSyncClient m) => m.ReadTopicPayloadAsync(topic, receiveProgress));
	}

	public override string ToString()
	{
		return $"MqttSyncClientPool[{mqttConnectPool.MaxConnector}]";
	}
}

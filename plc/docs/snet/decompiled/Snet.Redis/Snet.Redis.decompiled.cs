using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Snet.Core.extend;
using Snet.Model.data;
using Snet.Model.@interface;
using Snet.Utility;
using StackExchange.Redis;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETCoreApp,Version=v8.0", FrameworkDisplayName = ".NET 8.0")]
[assembly: AssemblyCompany("Shun")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("扩展工具：Redis（非关系型内存数据库，用于数据快速出站入站存储）")]
[assembly: AssemblyFileVersion("26.235.2.0")]
[assembly: AssemblyInformationalVersion("26.235.2+9f2214f5720028cb59246a846a7581b9982a1a3b")]
[assembly: AssemblyProduct("Extend")]
[assembly: AssemblyTitle("Snet.Redis")]
[assembly: AssemblyVersion("26.235.2.0")]
[module: RefSafetyRules(11)]
namespace Snet.Redis;

/// <summary>
/// Redis 数据配置类，封装 Redis 连接参数（连接字符串、数据库 ID、Key 前缀、过期时间等）。
/// </summary>
public class RedisData
{
	/// <summary>
	/// 基础数据
	/// </summary>
	public class Basics
	{
		/// <summary>
		/// 标识 前缀
		/// </summary>
		[Category("基础数据")]
		[Description("标识，前缀")]
		public string TAG { get; set; } = "S:";

		/// <summary>
		/// 连接字符串
		/// </summary>
		[Description("连接字符串")]
		public string ConnectStr { get; set; }

		/// <summary>
		/// 数据库ID
		/// </summary>
		[Description("数据库ID")]
		public int DataBaseID { get; set; }

		/// <summary>
		/// 有效时间 ms
		/// </summary>
		[Description("有效时间(ms)")]
		public int Expiry { get; set; } = 86400000;
	}
}
/// <summary>
/// Redis 操作类<br />
/// <br />
/// 功能：<br />
/// - 基于 StackExchange.Redis 实现 Redis 数据库操作<br />
/// - 支持 String、List、Hash、SortedSet、Key 五大数据类型<br />
/// - 支持发布/订阅消息模式<br />
/// - 支持同步/异步双模式操作<br />
/// - 支持 TAG 前缀自动拼接、模糊查询批量删除
/// </summary>
public class RedisOperate : CoreUnify<RedisOperate, RedisData.Basics>, IOn, IOff, IStatus, IObject, IDisposable, IAsyncDisposable
{
	/// <summary>
	/// Redis 排序类型
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum OrderType
	{
		Asc,
		Desc
	}

	/// <summary>
	/// 连接的Redis对象
	/// </summary>
	private IConnectionMultiplexer? conn;

	/// <summary>
	/// 数据库
	/// </summary>
	private IDatabase? db;

	/// <summary>
	/// 有参构造函数
	/// </summary>
	/// <param name="basics">基础数据</param>
	public RedisOperate(RedisData.Basics basics)
		: base(basics)
	{
	}

	/// <inheritdoc />
	public override void Dispose()
	{
		Off(hardClose: true);
		base.Dispose();
	}

	/// <inheritdoc />
	public override async ValueTask DisposeAsync()
	{
		await OffAsync(hardClose: true);
		await base.DisposeAsync();
	}

	/// <inheritdoc />
	public OperateResult On()
	{
		return OnAsync().GetAwaiter().GetResult();
	}

	/// <inheritdoc />
	public OperateResult Off(bool hardClose = false)
	{
		return OffAsync(hardClose).GetAwaiter().GetResult();
	}

	/// <inheritdoc />
	public OperateResult GetStatus()
	{
		return GetStatusAsync().GetAwaiter().GetResult();
	}

	/// <inheritdoc />
	public OperateResult GetBaseObject()
	{
		return GetBaseObjectAsync().GetAwaiter().GetResult();
	}

	/// <inheritdoc />
	public async Task<OperateResult> OnAsync(CancellationToken token = default(CancellationToken))
	{
		await base.BegOperateAsync(token, "OnAsync");
		try
		{
			string text = default(string);
			if (((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
			{
				return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "OnAsync", 69);
			}
			conn = (IConnectionMultiplexer?)(object)(await ConnectionMultiplexer.ConnectAsync(base.basics.ConnectStr, (TextWriter)null));
			db = conn.GetDatabase(base.basics.DataBaseID, (object)null);
			AddRegisterEvent();
			return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "OnAsync", 77);
		}
		catch (Exception ex)
		{
			await OffAsync(hardClose: true, token);
			return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "OnAsync", 82);
		}
	}

	/// <inheritdoc />
	public async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default(CancellationToken))
	{
		await base.BegOperateAsync(token, "OffAsync");
		try
		{
			string text = default(string);
			if (!hardClose && !((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
			{
				return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "OffAsync", 95);
			}
			if (conn != null)
			{
				await conn.CloseAsync(true);
				await ((IAsyncDisposable)conn).DisposeAsync();
			}
			conn = null;
			db = null;
			return await base.EndOperateAsync(true, (string)null, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "OffAsync", 105);
		}
		catch (Exception ex)
		{
			return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "OffAsync", 109);
		}
	}

	/// <inheritdoc />
	public async Task<OperateResult> GetStatusAsync(CancellationToken token = default(CancellationToken))
	{
		await base.BegOperateAsync(token, "GetStatusAsync");
		try
		{
			if (conn == null || !conn.IsConnected)
			{
				return await base.EndOperateAsync(false, "未连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "GetStatusAsync", 120);
			}
			return await base.EndOperateAsync(true, "已连接", (object)null, (Exception)null, false, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "GetStatusAsync", 122);
		}
		catch (Exception ex)
		{
			return await base.EndOperateAsync(false, ex.Message, (object)null, ex, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "GetStatusAsync", 126);
		}
	}

	/// <inheritdoc />
	public async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default(CancellationToken))
	{
		await base.BegOperateAsync(token, "GetBaseObjectAsync");
		string text = default(string);
		if (!((ResultModel)(await GetStatusAsync(token))).GetDetails(ref text))
		{
			return await base.EndOperateAsync(false, text, (object)null, (Exception)null, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "GetBaseObjectAsync", 135);
		}
		return await base.EndOperateAsync(true, (string)null, (object)conn, (Exception)null, true, true, token, "F:\\Snet\\Redis\\Snet.Redis\\RedisOperate.cs", "GetBaseObjectAsync", 137);
	}

	/// <summary>
	/// 添加注册事件
	/// </summary>
	private void AddRegisterEvent()
	{
		if (((BaseModel)GetStatus()).Status)
		{
			conn.ConnectionRestored += Conn_ConnectionRestored;
			conn.ConnectionFailed += Conn_ConnectionFailed;
			conn.ErrorMessage += Conn_ErrorMessage;
			conn.ConfigurationChanged += Conn_ConfigurationChanged;
			conn.HashSlotMoved += Conn_HashSlotMoved;
			conn.InternalError += Conn_InternalError;
			conn.ConfigurationChangedBroadcast += Conn_ConfigurationChangedBroadcast;
		}
	}

	/// <summary>
	/// 建立物理连接时
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_ConnectionRestored(object? sender, ConnectionFailedEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, $"{"Conn_ConnectionRestored"}: {e.Exception}"));
	}

	/// <summary>
	/// 重新配置广播时（通常意味着主从同步更改）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_ConfigurationChangedBroadcast(object? sender, EndPointEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, $"{"Conn_ConfigurationChangedBroadcast"}: {e.EndPoint}"));
	}

	/// <summary>
	/// 发生内部错误时（主要用于调试）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_InternalError(object? sender, InternalErrorEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, $"{"Conn_InternalError"}: {e.Exception}"));
	}

	/// <summary>
	/// 更改集群时
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_HashSlotMoved(object? sender, HashSlotMovedEventArgs e)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, $"{"Conn_HashSlotMoved"}: {"OldEndPoint"}-{e.OldEndPoint} To {"NewEndPoint"}-{e.NewEndPoint}"));
	}

	/// <summary>
	/// 配置更改时
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_ConfigurationChanged(object? sender, EndPointEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, $"{"Conn_ConfigurationChanged"}: {e.EndPoint}"));
	}

	/// <summary>
	/// 发生错误时
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_ErrorMessage(object? sender, RedisErrorEventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, "Conn_ErrorMessage: " + e.Message));
	}

	/// <summary>
	/// 物理连接失败时
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Conn_ConnectionFailed(object? sender, ConnectionFailedEventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		base.OnInfoEventHandler(sender, new EventInfoResult(true, $"{"Conn_ConnectionFailed"}: {e.Exception}"));
	}

	/// <summary>
	/// 保存单个键值对字符串（如果 key 已存在，则覆盖值）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true表示成功）</returns>
	public bool StringSet(string key, string value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.StringSet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), Expiration.op_Implicit(new TimeSpan(0, 0, 0, 0, base.basics.Expiry)), default(ValueCondition), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 获取键对应值字符串
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（true表示成功）</returns>
	public string? StringGet(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(db.StringGet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 保存多个多个键值对字符串
	/// </summary>
	/// <param name="keyValuePairs">键值对容器</param>
	/// <returns>返回结果（true表示成功）</returns>
	public bool StringSet(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.StringSet(keyValuePairs.Select<KeyValuePair<string, string>, KeyValuePair<RedisKey, RedisValue>>(delegate(KeyValuePair<string, string> x)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				return new KeyValuePair<RedisKey, RedisValue>(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(x.Key) : x.Key), RedisValue.op_Implicit(x.Value));
			}).ToArray(), (When)0, default(Expiration), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 存储一个对象（该对象会被序列化保存）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值（会被序列化）</param>
	/// <returns>返回结果（true表示成功）</returns>
	public bool StringSet<T>(string key, T value)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref value;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_0068;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_0068;
		}
		return false;
		IL_0068:
		return obj.StringSet(val, RedisValue.op_Implicit((string)obj2), Expiration.op_Implicit(new TimeSpan(0, 0, 0, 0, base.basics.Expiry)), default(ValueCondition), (CommandFlags)0);
	}

	/// <summary>
	/// 获取一个对象（会进行反序列化）
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（true表示成功）</returns>
	public T? StringGet<T>(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)db.StringGet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0)/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 保存一个键值对字符串（异步方式）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true表示成功）</returns>
	public async Task<bool> StringSetAsync(string key, string value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).StringSetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), Expiration.op_Implicit(new TimeSpan(0, 0, 0, 0, base.basics.Expiry)), default(ValueCondition), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 获取单个值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="expiry">时间间隔</param>
	/// <returns>返回结果（true表示成功）</returns>
	public async Task<string?> StringGetAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(await ((IDatabaseAsync)db).StringGetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 保存一组字符串值
	/// </summary>
	/// <param name="keyValuePairs">键值对容器</param>
	/// <returns>返回结果（true表示成功）</returns>
	public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).StringSetAsync(keyValuePairs.Select<KeyValuePair<string, string>, KeyValuePair<RedisKey, RedisValue>>(delegate(KeyValuePair<string, string> x)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				return new KeyValuePair<RedisKey, RedisValue>(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(x.Key) : x.Key), RedisValue.op_Implicit(x.Value));
			}).ToArray(), (When)0, default(Expiration), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 存储一个对象（该对象会被序列化保存）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true表示成功）</returns>
	public async Task<bool> StringSetAsync<T>(string key, T value)
	{
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref value;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_0106;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_0106;
		}
		return false;
		IL_0106:
		return await ((IDatabaseAsync)obj).StringSetAsync(val, RedisValue.op_Implicit((string)obj2), Expiration.op_Implicit(new TimeSpan(0, 0, 0, 0, base.basics.Expiry)), default(ValueCondition), (CommandFlags)0);
	}

	/// <summary>
	/// 获取一个对象（会进行反序列化）
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（true表示成功）</returns>
	public async Task<T?> StringGetAsync<T>(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)(await ((IDatabaseAsync)db).StringGetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0))/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 移除并返回存储在该键列表的第一个元素
	/// </summary>
	/// <param name="key">键</param>
	/// <returns></returns>
	public string? ListLeftPop(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(db.ListLeftPop(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 移除并返回存储在该键列表的最后一个元素
	/// </summary>
	/// <param name="key">键</param>
	/// <returns></returns>
	public string? ListRightPop(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(db.ListRightPop(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 移除列表指定键上与该值相同的元素
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long ListRemove(string key, string value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.ListRemove(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), 0L, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 在列表尾部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long ListRightPush(string key, string value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.ListRightPush(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), (When)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 在列表头部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long ListLeftPush(string key, string value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.ListLeftPush(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), (When)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 返回列表上该键的长度，如果不存在，返回 0
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long ListLength(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.ListLength(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 返回在该列表上键所对应的元素
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="start">起始点</param>
	/// <param name="stop">停止点</param>
	/// <returns></returns>
	public IEnumerable<string?> ListRange(string key, long start = 0L, long stop = -1L)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)db.ListRange(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), start, stop, (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 移除并返回存储在该键列表的第一个元素对象
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回反序列化后对象</returns>
	public T? ListLeftPop<T>(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)db.ListLeftPop(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0)/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 移除并返回存储在该键列表的最后一个元素对象
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回反序列化后的对象</returns>
	public T? ListRightPop<T>(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)db.ListRightPop(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0)/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 在列表尾部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long ListRightPush<T>(string key, T value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref value;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_0065;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_0065;
		}
		return -1L;
		IL_0065:
		return obj.ListRightPush(val, RedisValue.op_Implicit((string)obj2), (When)0, (CommandFlags)0);
	}

	/// <summary>
	/// 在列表头部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long ListLeftPush<T>(string key, T value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref value;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_0065;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_0065;
		}
		return -1L;
		IL_0065:
		return obj.ListLeftPush(val, RedisValue.op_Implicit((string)obj2), (When)0, (CommandFlags)0);
	}

	/// <summary>
	/// 移除并返回存储在该键列表的第一个元素
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public async Task<string?> ListLeftPopAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(await ((IDatabaseAsync)db).ListLeftPopAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 移除并返回存储在该键列表的最后一个元素
	/// </summary>
	/// <param name="key">键</param>
	/// <returns></returns>
	public async Task<string?> ListRightPopAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(await ((IDatabaseAsync)db).ListRightPopAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 移除列表指定键上与该值相同的元素
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> ListRemoveAsync(string key, string value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).ListRemoveAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), 0L, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 在列表尾部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> ListRightPushAsync(string key, string value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).ListRightPushAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), (When)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 在列表头部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> ListLeftPushAsync(string key, string value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).ListLeftPushAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(value), (When)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 返回列表上该键的长度，如果不存在，返回 0
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> ListLengthAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).ListLengthAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 返回在该列表上键所对应的元素（异步方式）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="start">起始索引</param>
	/// <param name="stop">结束索引（-1 表示最后一个）</param>
	/// <returns>元素集合</returns>
	public unsafe async Task<IEnumerable<string?>?> ListRangeAsync(string key, long start = 0L, long stop = -1L)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return (await ((IDatabaseAsync)db).ListRangeAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), start, stop, (CommandFlags)0)).Select((RedisValue x) => ((object)(*(RedisValue*)(&x))/*cast due to constrained. prefix*/).ToString());
		}
		return null;
	}

	/// <summary>
	/// 移除并返回存储在该键列表的第一个元素对象
	/// </summary>
	/// <param name="key"></param>
	/// <returns>返回反序列化后的对象</returns>
	public async Task<T?> ListLeftPopAsync<T>(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)(await ((IDatabaseAsync)db).ListLeftPopAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0))/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 移除并返回存储在该键列表的最后一个元素对象
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回反序列化后的对象</returns>
	public async Task<T?> ListRightPopAsync<T>(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)(await ((IDatabaseAsync)db).ListRightPopAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0))/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 在列表尾部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> ListRightPushAsync<T>(string key, T value)
	{
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref value;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_00e4;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_00e4;
		}
		return -1L;
		IL_00e4:
		return await ((IDatabaseAsync)obj).ListRightPushAsync(val, RedisValue.op_Implicit((string)obj2), (When)0, (CommandFlags)0);
	}

	/// <summary>
	/// 在列表头部插入值。如果键不存在，先创建再插入值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> ListLeftPushAsync<T>(string key, T value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).ListLeftPushAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(ExtendMethod.ToJson<T>(value, false)), (When)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 判断该字段是否存在hash中
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool HashExists(string key, string hashField)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.HashExists(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 从 hash 中移除指定字段
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool HashDelete(string key, string hashField)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.HashDelete(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 从 hash 中移除指定字段
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashFields">字段</param>
	/// <returns>返回结果（-1：表示失败）</returns>
	public long HashDelete(string key, IEnumerable<string> hashFields)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.HashDelete(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), hashFields.Select(delegate(string x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return RedisValue.op_Implicit(x);
			}).ToArray(), (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 在 hash 设定值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool HashSet(string key, string hashField, string value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.HashSet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), RedisValue.op_Implicit(value), (When)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 在 hash 中设定值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashFields">字段容器</param>
	public void HashSet(string key, IEnumerable<KeyValuePair<string, string>> hashFields)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			db.HashSet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), hashFields.Select<KeyValuePair<string, string>, HashEntry>(delegate(KeyValuePair<string, string> x)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				return new HashEntry(RedisValue.op_Implicit(x.Key), RedisValue.op_Implicit(x.Value));
			}).ToArray(), (CommandFlags)0);
		}
	}

	/// <summary>
	/// 在 hash 中获取值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <returns>返回结果</returns>
	public string? HashGet(string key, string hashField)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(db.HashGet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 在hash获取值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashFields">字段</param>
	/// <returns>返回结果</returns>
	public IEnumerable<string?> HashGet(string key, IEnumerable<string> hashFields)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)db.HashGet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), hashFields.Select(delegate(string x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return RedisValue.op_Implicit(x);
			}).ToArray(), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 从 hash 返回所有的字段值
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果</returns>
	public IEnumerable<string?> HashKeys(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)db.HashKeys(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 根据键获取hash中的所有值
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回hash结果</returns>
	public IEnumerable<string?> HashValues(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)db.HashValues(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 在 hash 设定值（序列化）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true:表示成功）</returns>
	public bool HashSet<T>(string key, string hashField, T value)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		IDatabase? obj;
		RedisKey val;
		RedisValue val2;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			val2 = RedisValue.op_Implicit(hashField);
			ref T reference = ref value;
			T val3 = default(T);
			if (val3 == null)
			{
				val3 = reference;
				reference = ref val3;
				if (val3 == null)
				{
					obj2 = null;
					goto IL_006b;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_006b;
		}
		return false;
		IL_006b:
		return obj.HashSet(val, val2, RedisValue.op_Implicit((string)obj2), (When)0, (CommandFlags)0);
	}

	/// <summary>
	/// 在 hash 中获取值（反序列化）
	/// </summary>
	/// <param name="key"></param>
	/// <param name="hashField"></param>
	/// <returns></returns>
	public T? HashGet<T>(string key, string hashField)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)db.HashGet(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0)/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// 判断该字段是否存在hash中（异步方式）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <returns>返回结果（true：表示存在）</returns>
	public async Task<bool> HashExistsAsync(string key, string hashField)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).HashExistsAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 从hash中移除指定字段
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public async Task<bool> HashDeleteAsync(string key, string hashField)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).HashDeleteAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 从hash中移除指定字段
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashFields">字段</param>
	/// <returns>返回删除结果（-1 表示失败）</returns>
	public async Task<long> HashDeleteAsync(string key, IEnumerable<string> hashFields)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			IEnumerable<RedisValue> source = hashFields.Select(delegate(string x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return RedisValue.op_Implicit(x);
			});
			return await ((IDatabaseAsync)db).HashDeleteAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), source.ToArray(), (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 在 hash 设定值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true:表示成功）</returns>
	public async Task<bool> HashSetAsync(string key, string hashField, string value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).HashSetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), RedisValue.op_Implicit(value), (When)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 在 hash 中设定值
	/// </summary>
	/// <param name="key"></param>
	/// <param name="hashFields"></param>
	public async Task HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> hashFields)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			await ((IDatabaseAsync)db).HashSetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), hashFields.Select<KeyValuePair<string, string>, HashEntry>(delegate(KeyValuePair<string, string> x)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				return new HashEntry(RedisValue.op_Implicit(x.Key), RedisValue.op_Implicit(x.Value));
			}).ToArray(), (CommandFlags)0);
		}
	}

	/// <summary>
	/// 在 hash 中获取值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <returns>返回结果</returns>
	public async Task<string?> HashGetAsync(string key, string hashField)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return RedisValue.op_Implicit(await ((IDatabaseAsync)db).HashGetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0));
		}
		return null;
	}

	/// <summary>
	/// 在 hash 中获取值
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashFields">字段</param>
	/// <param name="value">值</param>
	/// <returns>返回结果</returns>
	public async Task<IEnumerable<string?>?> HashGetAsync(string key, IEnumerable<string> hashFields, string value)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)(await ((IDatabaseAsync)db).HashGetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), hashFields.Select(delegate(string x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return RedisValue.op_Implicit(x);
			}).ToArray(), (CommandFlags)0)));
		}
		return null;
	}

	/// <summary>
	/// 从 hash 返回所有的字段值
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回对应的hash字段值</returns>
	public async Task<IEnumerable<string?>?> HashKeysAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)(await ((IDatabaseAsync)db).HashKeysAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0)));
		}
		return null;
	}

	/// <summary>
	/// 返回 hash 中的所有值
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回hash中的所有值</returns>
	public async Task<IEnumerable<string?>?> HashValuesAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)(await ((IDatabaseAsync)db).HashValuesAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0)));
		}
		return null;
	}

	/// <summary>
	/// 在 hash 设定值（序列化）
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="hashField">字段</param>
	/// <param name="value">值</param>
	/// <returns>返回结果（true:表示成功）</returns>
	public async Task<bool> HashSetAsync<T>(string key, string hashField, T value)
	{
		IDatabase? obj;
		RedisKey val;
		RedisValue val2;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			val2 = RedisValue.op_Implicit(hashField);
			ref T reference = ref value;
			T val3 = default(T);
			if (val3 == null)
			{
				val3 = reference;
				reference = ref val3;
				if (val3 == null)
				{
					obj2 = null;
					goto IL_00ef;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_00ef;
		}
		return false;
		IL_00ef:
		return await ((IDatabaseAsync)obj).HashSetAsync(val, val2, RedisValue.op_Implicit((string)obj2), (When)0, (CommandFlags)0);
	}

	/// <summary>
	/// 在 hash 中获取值（反序列化）
	/// </summary>
	/// <param name="key"></param>
	/// <param name="hashField"></param>
	/// <returns></returns>
	public async Task<T?> HashGetAsync<T>(string key, string hashField)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return JsonSerializer.Deserialize<T>(((object)(await ((IDatabaseAsync)db).HashGetAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(hashField), (CommandFlags)0))/*cast due to constrained. prefix*/).ToString());
		}
		return default(T);
	}

	/// <summary>
	/// SortedSet 新增
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="member"></param>
	/// <param name="score"></param>
	/// <returns></returns>
	public bool SortedSetAdd(string key, string member, double score)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.SortedSetAdd(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(member), score, (SortedSetWhen)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 在有序集合中返回指定范围的元素，默认情况下从低到高
	/// </summary>
	/// <param name="key"></param>
	/// <param name="start"></param>
	/// <param name="stop"></param>
	/// <param name="order"></param>
	/// <returns></returns>
	public unsafe IEnumerable<string> SortedSetRangeByRank(string key, long start = 0L, long stop = -1L, OrderType order = OrderType.Asc)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return from x in db.SortedSetRangeByRank(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), start, stop, (Order)order, (CommandFlags)0)
				select ((object)(*(RedisValue*)(&x))/*cast due to constrained. prefix*/).ToString();
		}
		return null;
	}

	/// <summary>
	/// 返回有序集合的元素个数
	/// </summary>
	/// <param name="key"></param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public long SortedSetLength(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.SortedSetLength(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), double.NegativeInfinity, double.PositiveInfinity, (Exclude)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 从存储在键处的已排序集合中删除指定的成员。不存在的成员将被忽略
	/// </summary>
	/// <param name="key"></param>
	/// <param name="memebr"></param>
	/// <returns>返回结果（true:表示成功）</returns>
	public bool SortedSetRemove(string key, string memebr)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.SortedSetRemove(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(memebr), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// SortedSet 新增
	/// </summary>
	/// <param name="key"></param>
	/// <param name="member"></param>
	/// <param name="score"></param>
	/// <returns>返回结果（true:表示成功）</returns>
	public bool SortedSetAdd<T>(string key, T member, double score)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref member;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_0065;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_0065;
		}
		return false;
		IL_0065:
		return obj.SortedSetAdd(val, RedisValue.op_Implicit((string)obj2), score, (SortedSetWhen)0, (CommandFlags)0);
	}

	/// <summary>
	/// 增量的得分排序的集合中的成员存储键值键按增量
	/// </summary>
	/// <param name="key"></param>
	/// <param name="member"></param>
	/// <param name="value"></param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public double SortedSetIncrement(string key, string member, double value = 1.0)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.SortedSetIncrement(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(member), value, (CommandFlags)0);
		}
		return -1.0;
	}

	/// <summary>
	/// SortedSet 新增
	/// </summary>
	/// <param name="key"></param>
	/// <param name="member"></param>
	/// <param name="score"></param>
	/// <returns>返回结果（true:表示成功）</returns>
	public async Task<bool> SortedSetAddAsync(string key, string member, double score)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).SortedSetAddAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(member), score, (SortedSetWhen)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 在有序集合中返回指定范围的元素，默认情况下从低到高。
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public async Task<IEnumerable<string?>?> SortedSetRangeByRankAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return ConvertStrings<RedisValue>((IEnumerable<RedisValue>)(await ((IDatabaseAsync)db).SortedSetRangeByRankAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), 0L, -1L, (Order)0, (CommandFlags)0)));
		}
		return null;
	}

	/// <summary>
	/// 返回有序集合的元素个数
	/// </summary>
	/// <param name="key"></param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> SortedSetLengthAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).SortedSetLengthAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), double.NegativeInfinity, double.PositiveInfinity, (Exclude)0, (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 返回有序集合的元素个数
	/// </summary>
	/// <param name="key"></param>
	/// <param name="memebr"></param>
	/// <returns>返回结果（true:表示成功）</returns>
	public async Task<bool> SortedSetRemoveAsync(string key, string memebr)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).SortedSetRemoveAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(memebr), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// SortedSet 新增
	/// </summary>
	/// <param name="key"></param>
	/// <param name="member"></param>
	/// <param name="score"></param>
	/// <returns></returns>
	public async Task<bool> SortedSetAddAsync<T>(string key, T member, double score)
	{
		IDatabase? obj;
		RedisKey val;
		object obj2;
		if (((BaseModel)GetStatus()).Status)
		{
			obj = db;
			val = RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
			ref T reference = ref member;
			T val2 = default(T);
			if (val2 == null)
			{
				val2 = reference;
				reference = ref val2;
				if (val2 == null)
				{
					obj2 = null;
					goto IL_00ea;
				}
			}
			obj2 = ExtendMethod.ToJson<T>(reference, false);
			goto IL_00ea;
		}
		return false;
		IL_00ea:
		return await ((IDatabaseAsync)obj).SortedSetAddAsync(val, RedisValue.op_Implicit((string)obj2), score, (SortedSetWhen)0, (CommandFlags)0);
	}

	/// <summary>
	/// 增量的得分排序的集合中的成员存储键值键按增量
	/// </summary>
	/// <param name="key"></param>
	/// <param name="member"></param>
	/// <param name="value"></param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<double> SortedSetIncrementAsync(string key, string member, double value = 1.0)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).SortedSetIncrementAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisValue.op_Implicit(member), value, (CommandFlags)0);
		}
		return -1.0;
	}

	/// <summary>
	/// 删除指定Key
	/// </summary>
	/// <param name="key">键(带*号支持模糊查询批量删除)</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool KeyDelete(string key)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		key = ((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key);
		if (((BaseModel)GetStatus()).Status)
		{
			if (key.Contains("*"))
			{
				RedisResult result = FuzzyQuery(key).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter().GetResult();
				if (!result.IsNull)
				{
					return !KeyDelete((string[])result).Equals(-1L);
				}
				return false;
			}
			return db.KeyDelete(RedisKey.op_Implicit(key), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 删除指定Key列表
	/// </summary>
	/// <param name="keys">键容器</param>
	/// <returns>移除指定键的数量(-1:表示失败)</returns>
	public long KeyDelete(IEnumerable<string> keys)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return db.KeyDelete(keys.Select(delegate(string x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return RedisKey.op_Implicit(x);
			}).ToArray(), (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 检查Key是否存在
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool KeyExists(string key)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.KeyExists(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 重命名Key
	/// </summary>
	/// <param name="key">原来的键</param>
	/// <param name="newKey">新的键名</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool KeyRename(string key, string newKey)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.KeyRename(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisKey.op_Implicit(newKey), (When)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 设置 Key 的时间
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="expiry">时间间隔</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public bool KeyExpire(string key, TimeSpan? expiry)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseModel)GetStatus()).Status)
		{
			return db.KeyExpire(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), expiry, (ExpireWhen)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 移除指定 Key
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public async Task<bool> KeyDeleteAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).KeyDeleteAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 删除指定 Key 列表
	/// </summary>
	/// <param name="keys">键</param>
	/// <returns>返回结果（-1 表示失败）</returns>
	public async Task<long> KeyDeleteAsync(IEnumerable<string> keys)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).KeyDeleteAsync(keys.Select(delegate(string x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return RedisKey.op_Implicit(x);
			}).ToArray(), (CommandFlags)0);
		}
		return -1L;
	}

	/// <summary>
	/// 检查 Key 是否存在
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public async Task<bool> KeyExistsAsync(string key)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).KeyExistsAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 重命名 Key
	/// </summary>
	/// <param name="key">旧的键名称</param>
	/// <param name="newKey">新的键名称</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public async Task<bool> KeyRenameAsync(string key, string newKey)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).KeyRenameAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), RedisKey.op_Implicit(newKey), (When)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 设置 Key 的时间
	/// </summary>
	/// <param name="key">键</param>
	/// <param name="expiry">间隔时间</param>
	/// <returns>返回结果（true：表示成功）</returns>
	public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry)
	{
		if (((BaseModel)GetStatus()).Status)
		{
			return await ((IDatabaseAsync)db).KeyExpireAsync(RedisKey.op_Implicit((!string.IsNullOrWhiteSpace(base.basics.TAG)) ? GTAG(key) : key), expiry, (ExpireWhen)0, (CommandFlags)0);
		}
		return false;
	}

	/// <summary>
	/// 订阅
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="handle"></param>
	public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		conn.GetSubscriber((object)null).Subscribe(channel, handle, (CommandFlags)0);
	}

	/// <summary>
	/// 发布
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public long Publish(RedisChannel channel, RedisValue message)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return conn.GetSubscriber((object)null).Publish(channel, message, (CommandFlags)0);
	}

	/// <summary>
	/// 发布（使用序列化）
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public long Publish<T>(RedisChannel channel, T message)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ISubscriber subscriber = conn.GetSubscriber((object)null);
		ref T reference = ref message;
		T val = default(T);
		object obj;
		if (val == null)
		{
			val = reference;
			reference = ref val;
			if (val == null)
			{
				obj = null;
				goto IL_003e;
			}
		}
		obj = ExtendMethod.ToJson<T>(reference, false);
		goto IL_003e;
		IL_003e:
		return subscriber.Publish(channel, RedisValue.op_Implicit((string)obj), (CommandFlags)0);
	}

	/// <summary>
	/// 订阅
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="handle"></param>
	public async Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handle)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		await conn.GetSubscriber((object)null).SubscribeAsync(channel, handle, (CommandFlags)0);
	}

	/// <summary>
	/// 发布
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public async Task<long> PublishAsync(RedisChannel channel, RedisValue message)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return await conn.GetSubscriber((object)null).PublishAsync(channel, message, (CommandFlags)0);
	}

	/// <summary>
	/// 发布（使用序列化）
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public async Task<long> PublishAsync<T>(RedisChannel channel, T message)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ISubscriber subscriber = conn.GetSubscriber((object)null);
		ref T reference = ref message;
		T val = default(T);
		object obj;
		if (val == null)
		{
			val = reference;
			reference = ref val;
			if (val == null)
			{
				obj = null;
				goto IL_00b4;
			}
		}
		obj = ExtendMethod.ToJson<T>(reference, false);
		goto IL_00b4;
		IL_00b4:
		return await subscriber.PublishAsync(channel, RedisValue.op_Implicit((string)obj), (CommandFlags)0);
	}

	/// <summary>
	/// 获取TAG 前缀
	/// </summary>
	/// <param name="key">键</param>
	/// <returns>返回前缀 + 键</returns>
	private string GTAG(string key)
	{
		return base.basics.TAG + key;
	}

	/// <summary>
	/// 转换为字符串
	/// </summary>
	/// <typeparam name="T">数据类型</typeparam>
	/// <param name="list">数据列表</param>
	/// <returns>返回转为字符串后的数据列表</returns>
	private static IEnumerable<string?> ConvertStrings<T>(IEnumerable<T> list) where T : struct
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		return list.Select((T x) => x.ToString());
	}

	/// <summary>
	/// 模糊查询
	/// </summary>
	/// <param name="key">键</param>
	private async Task<RedisResult> FuzzyQuery(string key)
	{
		return await ((IDatabaseAsync)db).ScriptEvaluateAsync(LuaScript.Prepare(" local res = redis.call('KEYS', @keypattern)  return res "), (object)new
		{
			keypattern = key
		}, (CommandFlags)0);
	}
}

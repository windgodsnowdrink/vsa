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

namespace HslCommunication.Enthernet.Redis;

public class RedisClientPool
{
	private ConnectPool<IRedisConnector> redisConnectPool;

	public ConnectPool<IRedisConnector> GetRedisConnectPool => redisConnectPool;

	/// <summary>
	/// 获取或设置最大的连接数，当实际的连接数超过最大的连接数的时候，就会进行阻塞，直到有新的连接对象为止。
	/// Get or set the maximum number of connections. When the actual number of connections exceeds the maximum number of connections,
	/// it will block until there is a new connection object.
	/// </summary>
	public int MaxConnector
	{
		get
		{
			return redisConnectPool.MaxConnector;
		}
		set
		{
			redisConnectPool.MaxConnector = value;
		}
	}

	public RedisClientPool(string ipAddress, int port, string password)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			redisConnectPool = new ConnectPool<IRedisConnector>(() => new IRedisConnector
			{
				Redis = new RedisClient(ipAddress, port, password)
			});
			redisConnectPool.MaxConnector = int.MaxValue;
			return;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	public RedisClientPool(string ipAddress, int port, string password, Action<RedisClient> initialize)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			redisConnectPool = new ConnectPool<IRedisConnector>(delegate
			{
				RedisClient redisClient = new RedisClient(ipAddress, port, password);
				initialize(redisClient);
				return new IRedisConnector
				{
					Redis = redisClient
				};
			});
			redisConnectPool.MaxConnector = int.MaxValue;
			return;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private OperateResult<T> ConnectPoolExecute<T>(Func<RedisClient, OperateResult<T>> exec)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IRedisConnector availableConnector = redisConnectPool.GetAvailableConnector();
			OperateResult<T> result = exec(availableConnector.Redis);
			redisConnectPool.ReturnConnector(availableConnector);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private OperateResult ConnectPoolExecute(Func<RedisClient, OperateResult> exec)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IRedisConnector availableConnector = redisConnectPool.GetAvailableConnector();
			OperateResult result = exec(availableConnector.Redis);
			redisConnectPool.ReturnConnector(availableConnector);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private async Task<OperateResult<T>> ConnectPoolExecuteAsync<T>(Func<RedisClient, Task<OperateResult<T>>> execAsync)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IRedisConnector client = redisConnectPool.GetAvailableConnector();
			OperateResult<T> result = await execAsync(client.Redis);
			redisConnectPool.ReturnConnector(client);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	private async Task<OperateResult> ConnectPoolExecuteAsync(Func<RedisClient, Task<OperateResult>> execAsync)
	{
		if (Authorization.asdniasnfaksndiqwhawfskhfaiw())
		{
			IRedisConnector client = redisConnectPool.GetAvailableConnector();
			OperateResult result = await execAsync(client.Redis);
			redisConnectPool.ReturnConnector(client);
			return result;
		}
		throw new Exception(StringResources.Language.InsufficientPrivileges);
	}

	/// <summary>
	/// 删除给定的一个或多个 key 。不存在的 key 会被忽略。
	/// </summary>
	/// <param name="keys">关键字</param>
	/// <returns>被删除 key 的数量。</returns>
	public OperateResult<int> DeleteKey(string[] keys)
	{
		return ConnectPoolExecute((RedisClient m) => m.DeleteKey(keys));
	}

	/// <summary>
	/// 删除给定的一个或多个 key 。不存在的 key 会被忽略。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>被删除 key 的数量。</returns>
	public OperateResult<int> DeleteKey(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.DeleteKey(key));
	}

	/// <summary>
	/// 检查给定 key 是否存在。若 key 存在，返回 1 ，否则返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>若 key 存在，返回 1 ，否则返回 0 。</returns>
	public OperateResult<int> ExistsKey(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ExistsKey(key));
	}

	/// <summary>
	/// 为给定 key 设置生存时间，当 key 过期时(生存时间为 0 )，它会被自动删除。设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="seconds">当前key的生存时间，单位为秒</param>
	/// <returns>设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。</returns>
	/// <remarks>
	/// 在 Redis 中，带有生存时间的 key 被称为『易失的』(volatile)。
	/// 生存时间可以通过使用 DEL 命令来删除整个 key 来移除，或者被 SET 和 GETSET 命令覆写( overwrite)，这意味着，如果一个命令只是修改( alter)一个带生存时间的 key 的值而不是用一个新的 key 值来代替( replace)它的话，那么生存时间不会被改变。
	/// 比如说，对一个 key 执行 INCR 命令，对一个列表进行 LPUSH 命令，或者对一个哈希表执行 HSET 命令，这类操作都不会修改 key 本身的生存时间。
	/// 另一方面，如果使用 RENAME 对一个 key 进行改名，那么改名后的 key 的生存时间和改名前一样。
	/// RENAME 命令的另一种可能是，尝试将一个带生存时间的 key 改名成另一个带生存时间的 another_key ，这时旧的 another_key( 以及它的生存时间)会被删除，然后旧的 key 会改名为 another_key ，因此，新的 another_key 的生存时间也和原本的 key 一样。
	/// 使用 PERSIST 命令可以在不删除 key 的情况下，移除 key 的生存时间，让 key 重新成为一个『持久的』(persistent) key 。
	/// 更新生存时间
	/// 可以对一个已经带有生存时间的 key 执行 EXPIRE 命令，新指定的生存时间会取代旧的生存时间。
	/// 过期时间的精确度
	/// 在 Redis 2.4 版本中，过期时间的延迟在 1 秒钟之内 —— 也即是，就算 key 已经过期，但它还是可能在过期之后一秒钟之内被访问到，而在新的 Redis 2.6 版本中，延迟被降低到 1 毫秒之内。
	/// Redis 2.1.3 之前的不同之处
	/// 在 Redis 2.1.3 之前的版本中，修改一个带有生存时间的 key 会导致整个 key 被删除，这一行为是受当时复制( replication)层的限制而作出的，现在这一限制已经被修复。
	/// </remarks>
	public OperateResult<int> ExpireKey(string key, int seconds)
	{
		return ConnectPoolExecute((RedisClient m) => m.ExpireKey(key, seconds));
	}

	/// <summary>
	/// 查找所有符合给定模式 pattern 的 key 。
	/// * 匹配数据库中所有 key。
	/// h?llo 匹配 hello ， hallo 和 hxllo 等。
	/// h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo 。
	/// </summary>
	/// <param name="pattern">给定模式</param>
	/// <returns>符合给定模式的 key 列表。</returns>
	public OperateResult<string[]> ReadAllKeys(string pattern)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadAllKeys(pattern));
	}

	/// <summary>
	/// 将当前数据库的 key 移动到给定的数据库 db 当中。
	/// 如果当前数据库(源数据库)和给定数据库(目标数据库)有相同名字的给定 key ，或者 key 不存在于当前数据库，那么 MOVE 没有任何效果。
	/// 因此，也可以利用这一特性，将 MOVE 当作锁(locking)原语(primitive)。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="db">数据块</param>
	/// <returns>是否移动成功</returns>
	public OperateResult MoveKey(string key, int db)
	{
		return ConnectPoolExecute((RedisClient m) => m.MoveKey(key, db));
	}

	/// <summary>
	/// 移除给定 key 的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
	/// 当生存时间移除成功时，返回 1 .
	/// 如果 key 不存在或 key 没有设置生存时间，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>当生存时间移除成功时，返回 1 . 如果 key 不存在或 key 没有设置生存时间，返回 0 。</returns>
	public OperateResult<int> PersistKey(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.PersistKey(key));
	}

	/// <summary>
	/// 从当前数据库中随机返回(不删除)一个 key 。
	/// 当数据库不为空时，返回一个 key 。
	/// 当数据库为空时，返回 nil 。
	/// </summary>
	/// <returns>当数据库不为空时，返回一个 key 。 当数据库为空时，返回 nil 。</returns>
	public OperateResult<string> ReadRandomKey()
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadRandomKey());
	}

	/// <summary>
	/// 将 key 改名为 newkey 。
	/// 当 key 和 newkey 相同，或者 key 不存在时，返回一个错误。
	/// 当 newkey 已经存在时， RENAME 命令将覆盖旧值。
	/// </summary>
	/// <param name="key1">旧的key</param>
	/// <param name="key2">新的key</param>
	/// <returns>改名成功时提示 OK ，失败时候返回一个错误。</returns>
	public OperateResult RenameKey(string key1, string key2)
	{
		return ConnectPoolExecute((RedisClient m) => m.RenameKey(key1, key2));
	}

	/// <summary>
	/// 返回 key 所储存的值的类型。none (key不存在)，string (字符串)，list (列表)，set (集合)，zset (有序集)，hash (哈希表)
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>类型</returns>
	public OperateResult<string> ReadKeyType(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadKeyType(key));
	}

	/// <summary>
	/// 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>当 key 不存在时，返回 -2 。当 key 存在但没有设置剩余生存时间时，返回 -1 。否则，以秒为单位，返回 key 的剩余生存时间。</returns>
	public OperateResult<int> ReadKeyTTL(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadKeyTTL(key));
	}

	/// <summary>
	/// 删除给定的一个或多个 key 。不存在的 key 会被忽略。
	/// </summary>
	/// <param name="keys">关键字</param>
	/// <returns>被删除 key 的数量。</returns>
	public async Task<OperateResult<int>> DeleteKeyAsync(string[] keys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.DeleteKeyAsync(keys));
	}

	/// <summary>
	/// 删除给定的一个或多个 key 。不存在的 key 会被忽略。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>被删除 key 的数量。</returns>
	public async Task<OperateResult<int>> DeleteKeyAsync(string key)
	{
		return await DeleteKeyAsync(new string[1] { key });
	}

	/// <summary>
	/// 检查给定 key 是否存在。若 key 存在，返回 1 ，否则返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>若 key 存在，返回 1 ，否则返回 0 。</returns>
	public async Task<OperateResult<int>> ExistsKeyAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ExistsKeyAsync(key));
	}

	/// <summary>
	/// 为给定 key 设置生存时间，当 key 过期时(生存时间为 0 )，它会被自动删除。设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="seconds">当前key的生存时间，单位为秒</param>
	/// <returns>设置成功返回 1 。当 key 不存在或者不能为 key 设置生存时间时，返回 0 。</returns>
	/// <remarks>
	/// 在 Redis 中，带有生存时间的 key 被称为『易失的』(volatile)。
	/// 生存时间可以通过使用 DEL 命令来删除整个 key 来移除，或者被 SET 和 GETSET 命令覆写( overwrite)，这意味着，如果一个命令只是修改( alter)一个带生存时间的 key 的值而不是用一个新的 key 值来代替( replace)它的话，那么生存时间不会被改变。
	/// 比如说，对一个 key 执行 INCR 命令，对一个列表进行 LPUSH 命令，或者对一个哈希表执行 HSET 命令，这类操作都不会修改 key 本身的生存时间。
	/// 另一方面，如果使用 RENAME 对一个 key 进行改名，那么改名后的 key 的生存时间和改名前一样。
	/// RENAME 命令的另一种可能是，尝试将一个带生存时间的 key 改名成另一个带生存时间的 another_key ，这时旧的 another_key( 以及它的生存时间)会被删除，然后旧的 key 会改名为 another_key ，因此，新的 another_key 的生存时间也和原本的 key 一样。
	/// 使用 PERSIST 命令可以在不删除 key 的情况下，移除 key 的生存时间，让 key 重新成为一个『持久的』(persistent) key 。
	/// 更新生存时间
	/// 可以对一个已经带有生存时间的 key 执行 EXPIRE 命令，新指定的生存时间会取代旧的生存时间。
	/// 过期时间的精确度
	/// 在 Redis 2.4 版本中，过期时间的延迟在 1 秒钟之内 —— 也即是，就算 key 已经过期，但它还是可能在过期之后一秒钟之内被访问到，而在新的 Redis 2.6 版本中，延迟被降低到 1 毫秒之内。
	/// Redis 2.1.3 之前的不同之处
	/// 在 Redis 2.1.3 之前的版本中，修改一个带有生存时间的 key 会导致整个 key 被删除，这一行为是受当时复制( replication)层的限制而作出的，现在这一限制已经被修复。
	/// </remarks>
	public async Task<OperateResult<int>> ExpireKeyAsync(string key, int seconds)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ExpireKeyAsync(key, seconds));
	}

	/// <summary>
	/// 查找所有符合给定模式 pattern 的 key 。
	/// * 匹配数据库中所有 key。
	/// h?llo 匹配 hello ， hallo 和 hxllo 等。
	/// h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo 。
	/// </summary>
	/// <param name="pattern">给定模式</param>
	/// <returns>符合给定模式的 key 列表。</returns>
	public async Task<OperateResult<string[]>> ReadAllKeysAsync(string pattern)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadAllKeysAsync(pattern));
	}

	/// <summary>
	/// 将当前数据库的 key 移动到给定的数据库 db 当中。
	/// 如果当前数据库(源数据库)和给定数据库(目标数据库)有相同名字的给定 key ，或者 key 不存在于当前数据库，那么 MOVE 没有任何效果。
	/// 因此，也可以利用这一特性，将 MOVE 当作锁(locking)原语(primitive)。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="db">数据块</param>
	/// <returns>是否移动成功</returns>
	public async Task<OperateResult> MoveKeyAsync(string key, int db)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.MoveKeyAsync(key, db));
	}

	/// <summary>
	/// 移除给定 key 的生存时间，将这个 key 从『易失的』(带生存时间 key )转换成『持久的』(一个不带生存时间、永不过期的 key )。
	/// 当生存时间移除成功时，返回 1 .
	/// 如果 key 不存在或 key 没有设置生存时间，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>当生存时间移除成功时，返回 1 . 如果 key 不存在或 key 没有设置生存时间，返回 0 。</returns>
	public async Task<OperateResult<int>> PersistKeyAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.PersistKeyAsync(key));
	}

	/// <summary>
	/// 从当前数据库中随机返回(不删除)一个 key 。
	/// 当数据库不为空时，返回一个 key 。
	/// 当数据库为空时，返回 nil 。
	/// </summary>
	/// <returns>当数据库不为空时，返回一个 key 。 当数据库为空时，返回 nil 。</returns>
	public async Task<OperateResult<string>> ReadRandomKeyAsync()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadRandomKeyAsync());
	}

	/// <summary>
	/// 将 key 改名为 newkey 。
	/// 当 key 和 newkey 相同，或者 key 不存在时，返回一个错误。
	/// 当 newkey 已经存在时， RENAME 命令将覆盖旧值。
	/// </summary>
	/// <param name="key1">旧的key</param>
	/// <param name="key2">新的key</param>
	/// <returns>改名成功时提示 OK ，失败时候返回一个错误。</returns>
	public async Task<OperateResult> RenameKeyAsync(string key1, string key2)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.RenameKeyAsync(key1, key2));
	}

	/// <summary>
	/// 返回 key 所储存的值的类型。none (key不存在)，string (字符串)，list (列表)，set (集合)，zset (有序集)，hash (哈希表)
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>类型</returns>
	public async Task<OperateResult<string>> ReadKeyTypeAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadKeyTypeAsync(key));
	}

	/// <summary>
	/// 以秒为单位，返回给定 key 的剩余生存时间(TTL, time to live)。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>当 key 不存在时，返回 -2 。当 key 存在但没有设置剩余生存时间时，返回 -1 。否则，以秒为单位，返回 key 的剩余生存时间。</returns>
	public async Task<OperateResult<int>> ReadKeyTTLAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadKeyTTLAsync(key));
	}

	/// <summary>
	/// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
	/// 如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样。
	/// 返回追加 value 之后， key 中字符串的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <returns>追加 value 之后， key 中字符串的长度。</returns>
	public OperateResult<int> AppendKey(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.AppendKey(key, value));
	}

	/// <summary>
	/// 将 key 中储存的数字值减一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// 本操作的值限制在 64 位(bit)有符号数字表示之内。
	/// 返回执行 DECR 命令之后 key 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>执行 DECR 命令之后 key 的值。</returns>
	public OperateResult<long> DecrementKey(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.DecrementKey(key));
	}

	/// <summary>
	/// 将 key 所储存的值减去减量 decrement 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// 本操作的值限制在 64 位(bit)有符号数字表示之内。
	/// 返回减去 decrement 之后， key 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">操作的值</param>
	/// <returns>返回减去 decrement 之后， key 的值。</returns>
	public OperateResult<long> DecrementKey(string key, long value)
	{
		return ConnectPoolExecute((RedisClient m) => m.DecrementKey(key, value));
	}

	/// <summary>
	/// 返回 key 所关联的字符串值。如果 key 不存在那么返回特殊值 nil 。
	/// 假如 key 储存的值不是字符串类型，返回一个错误，因为 GET 只能用于处理字符串值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>当 key 不存在时，返回 nil ，否则，返回 key 的值。</returns>
	public OperateResult<string> ReadKey(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadKey(key));
	}

	/// <summary>
	/// 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
	/// 负数偏移量表示从字符串最后开始计数， -1 表示最后一个字符， -2 表示倒数第二个，以此类推。
	/// 返回截取得出的子字符串。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="start">截取开始的位置</param>
	/// <param name="end">截取结束的位置</param>
	/// <returns>返回截取得出的子字符串。</returns>
	public OperateResult<string> ReadKeyRange(string key, int start, int end)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadKeyRange(key, start, end));
	}

	/// <summary>
	/// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)。当 key 存在但不是字符串类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">新的值</param>
	/// <returns>返回给定 key 的旧值。当 key 没有旧值时，也即是， key 不存在时，返回 nil 。</returns>
	public OperateResult<string> ReadAndWriteKey(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadAndWriteKey(key, value));
	}

	/// <summary>
	/// 将 key 中储存的数字值增一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// 返回执行 INCR 命令之后 key 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>返回执行 INCR 命令之后 key 的值。</returns>
	public OperateResult<long> IncrementKey(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.IncrementKey(key));
	}

	/// <summary>
	/// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">增量数据</param>
	/// <returns>加上 increment 之后， key 的值。</returns>
	public OperateResult<long> IncrementKey(string key, long value)
	{
		return ConnectPoolExecute((RedisClient m) => m.IncrementKey(key, value));
	}

	/// <summary>
	/// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCRBYFLOAT 操作。
	/// 如果命令执行成功，那么 key 的值会被更新为（执行加法之后的）新值，并且新值会以字符串的形式返回给调用者
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">增量数据</param>
	/// <returns>执行命令之后 key 的值。</returns>
	public OperateResult<string> IncrementKey(string key, float value)
	{
		return ConnectPoolExecute((RedisClient m) => m.IncrementKey(key, value));
	}

	/// <summary>
	/// 返回所有(一个或多个)给定 key 的值。
	/// 如果给定的 key 里面，有某个 key 不存在，那么这个 key 返回特殊值 null 。因此，该命令永不失败。
	/// </summary>
	/// <param name="keys">关键字数组</param>
	/// <returns>一个包含所有给定 key 的值的列表。</returns>
	public OperateResult<string[]> ReadKey(string[] keys)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadKey(keys));
	}

	/// <summary>
	/// 同时设置一个或多个 key-value 对。
	/// 如果某个给定 key 已经存在，那么 MSET 会用新值覆盖原来的旧值，如果这不是你所希望的效果，请考虑使用 MSETNX 命令：它只会在所有给定 key 都不存在的情况下进行设置操作。
	/// </summary>
	/// <param name="keys">关键字数组</param>
	/// <param name="values">值数组</param>
	/// <returns>总是返回 OK (因为 MSET 不可能失败)</returns>
	public OperateResult WriteKey(string[] keys, string[] values)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteKey(keys, values));
	}

	/// <summary>
	/// 将字符串值 value 关联到 key 。
	/// 如果 key 已经持有其他值， SET 就覆写旧值，无视类型。
	/// 对于某个原本带有生存时间（TTL）的键来说， 当 SET 命令成功在这个键上执行时，这个键原有的 TTL 将被清除。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数据值</param>
	/// <returns>SET 在设置操作成功完成时，才返回 OK 。</returns>
	public OperateResult WriteKey(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteKey(key, value));
	}

	/// <summary>
	/// 将字符串值 value 关联到 key 。并发布一个订阅的频道数据，都成功时，才返回成功
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数据值</param>
	/// <returns>是否成功的结果对象</returns>
	public OperateResult WriteAndPublishKey(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteAndPublishKey(key, value));
	}

	/// <summary>
	/// 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)。如果 key 已经存在， SETEX 命令将覆写旧值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="seconds">生存时间，单位秒</param>
	/// <returns>设置成功时返回 OK 。当 seconds 参数不合法时，返回一个错误。</returns>
	public OperateResult WriteExpireKey(string key, string value, long seconds)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteExpireKey(key, value, seconds));
	}

	/// <summary>
	/// 将 key 的值设为 value ，当且仅当 key 不存在。若给定的 key 已经存在，则 SETNX 不做任何动作。设置成功，返回 1 。设置失败，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数据值</param>
	/// <returns>设置成功，返回 1 。设置失败，返回 0 。</returns>
	public OperateResult<int> WriteKeyIfNotExists(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteKeyIfNotExists(key, value));
	}

	/// <summary>
	/// 用 value 参数覆写(overwrite)给定 key 所储存的字符串值，从偏移量 offset 开始。不存在的 key 当作空白字符串处理。返回被 SETRANGE 修改之后，字符串的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="offset">起始的偏移量</param>
	/// <returns>被 SETRANGE 修改之后，字符串的长度。</returns>
	public OperateResult<int> WriteKeyRange(string key, string value, int offset)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteKeyRange(key, value, offset));
	}

	/// <summary>
	/// 返回 key 所储存的字符串值的长度。当 key 储存的不是字符串值时，返回一个错误。返回符串值的长度。当 key 不存在时，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>字符串值的长度。当 key 不存在时，返回 0 。</returns>
	public OperateResult<int> ReadKeyLength(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadKeyLength(key));
	}

	/// <summary>
	/// 如果 key 已经存在并且是一个字符串， APPEND 命令将 value 追加到 key 原来的值的末尾。
	/// 如果 key 不存在， APPEND 就简单地将给定 key 设为 value ，就像执行 SET key value 一样。
	/// 返回追加 value 之后， key 中字符串的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <returns>追加 value 之后， key 中字符串的长度。</returns>
	public async Task<OperateResult<int>> AppendKeyAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.AppendKeyAsync(key, value));
	}

	/// <summary>
	/// 将 key 中储存的数字值减一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// 本操作的值限制在 64 位(bit)有符号数字表示之内。
	/// 返回执行 DECR 命令之后 key 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>执行 DECR 命令之后 key 的值。</returns>
	public async Task<OperateResult<long>> DecrementKeyAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.DecrementKeyAsync(key));
	}

	/// <summary>
	/// 将 key 所储存的值减去减量 decrement 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 DECR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// 本操作的值限制在 64 位(bit)有符号数字表示之内。
	/// 返回减去 decrement 之后， key 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">操作的值</param>
	/// <returns>返回减去 decrement 之后， key 的值。</returns>
	public async Task<OperateResult<long>> DecrementKeyAsync(string key, long value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.DecrementKeyAsync(key, value));
	}

	/// <summary>
	/// 返回 key 所关联的字符串值。如果 key 不存在那么返回特殊值 nil 。
	/// 假如 key 储存的值不是字符串类型，返回一个错误，因为 GET 只能用于处理字符串值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>当 key 不存在时，返回 nil ，否则，返回 key 的值。</returns>
	public async Task<OperateResult<string>> ReadKeyAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadKeyAsync(key));
	}

	/// <summary>
	/// 返回 key 中字符串值的子字符串，字符串的截取范围由 start 和 end 两个偏移量决定(包括 start 和 end 在内)。
	/// 负数偏移量表示从字符串最后开始计数， -1 表示最后一个字符， -2 表示倒数第二个，以此类推。
	/// 返回截取得出的子字符串。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="start">截取开始的位置</param>
	/// <param name="end">截取结束的位置</param>
	/// <returns>返回截取得出的子字符串。</returns>
	public async Task<OperateResult<string>> ReadKeyRangeAsync(string key, int start, int end)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadKeyRangeAsync(key, start, end));
	}

	/// <summary>
	/// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)。当 key 存在但不是字符串类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">新的值</param>
	/// <returns>返回给定 key 的旧值。当 key 没有旧值时，也即是， key 不存在时，返回 nil 。</returns>
	public async Task<OperateResult<string>> ReadAndWriteKeyAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadAndWriteKeyAsync(key, value));
	}

	/// <summary>
	/// 将 key 中储存的数字值增一。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// 返回执行 INCR 命令之后 key 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>返回执行 INCR 命令之后 key 的值。</returns>
	public async Task<OperateResult<long>> IncrementKeyAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.IncrementKeyAsync(key));
	}

	/// <summary>
	/// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCR 操作。
	/// 如果值包含错误的类型，或字符串类型的值不能表示为数字，那么返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">增量数据</param>
	/// <returns>加上 increment 之后， key 的值。</returns>
	public async Task<OperateResult<long>> IncrementKeyAsync(string key, long value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.IncrementKeyAsync(key, value));
	}

	/// <summary>
	/// 将 key 所储存的值加上增量 increment 。如果 key 不存在，那么 key 的值会先被初始化为 0 ，然后再执行 INCRBYFLOAT 操作。
	/// 如果命令执行成功，那么 key 的值会被更新为（执行加法之后的）新值，并且新值会以字符串的形式返回给调用者
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">增量数据</param>
	/// <returns>执行命令之后 key 的值。</returns>
	public async Task<OperateResult<string>> IncrementKeyAsync(string key, float value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.IncrementKeyAsync(key, value));
	}

	/// <summary>
	/// 返回所有(一个或多个)给定 key 的值。
	/// 如果给定的 key 里面，有某个 key 不存在，那么这个 key 返回特殊值 null 。因此，该命令永不失败。
	/// </summary>
	/// <param name="keys">关键字数组</param>
	/// <returns>一个包含所有给定 key 的值的列表。</returns>
	public async Task<OperateResult<string[]>> ReadKeyAsync(string[] keys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadKeyAsync(keys));
	}

	/// <summary>
	/// 同时设置一个或多个 key-value 对。
	/// 如果某个给定 key 已经存在，那么 MSET 会用新值覆盖原来的旧值，如果这不是你所希望的效果，请考虑使用 MSETNX 命令：它只会在所有给定 key 都不存在的情况下进行设置操作。
	/// </summary>
	/// <param name="keys">关键字数组</param>
	/// <param name="values">值数组</param>
	/// <returns>总是返回 OK (因为 MSET 不可能失败)</returns>
	public async Task<OperateResult> WriteKeyAsync(string[] keys, string[] values)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteKeyAsync(keys, values));
	}

	/// <summary>
	/// 将字符串值 value 关联到 key 。
	/// 如果 key 已经持有其他值， SET 就覆写旧值，无视类型。
	/// 对于某个原本带有生存时间（TTL）的键来说， 当 SET 命令成功在这个键上执行时，这个键原有的 TTL 将被清除。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数据值</param>
	/// <returns>SET 在设置操作成功完成时，才返回 OK 。</returns>
	public async Task<OperateResult> WriteKeyAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteKeyAsync(key, value));
	}

	/// <summary>
	/// 将字符串值 value 关联到 key 。并发布一个订阅的频道数据，都成功时，才返回成功
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数据值</param>
	/// <returns>是否成功的结果对象</returns>
	public async Task<OperateResult> WriteAndPublishKeyAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteAndPublishKeyAsync(key, value));
	}

	/// <summary>
	/// 将值 value 关联到 key ，并将 key 的生存时间设为 seconds (以秒为单位)。如果 key 已经存在， SETEX 命令将覆写旧值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="seconds">生存时间，单位秒</param>
	/// <returns>设置成功时返回 OK 。当 seconds 参数不合法时，返回一个错误。</returns>
	public async Task<OperateResult> WriteExpireKeyAsync(string key, string value, long seconds)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteExpireKeyAsync(key, value, seconds));
	}

	/// <summary>
	/// 将 key 的值设为 value ，当且仅当 key 不存在。若给定的 key 已经存在，则 SETNX 不做任何动作。设置成功，返回 1 。设置失败，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数据值</param>
	/// <returns>设置成功，返回 1 。设置失败，返回 0 。</returns>
	public async Task<OperateResult<int>> WriteKeyIfNotExistsAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteKeyIfNotExistsAsync(key, value));
	}

	/// <summary>
	/// 用 value 参数覆写(overwrite)给定 key 所储存的字符串值，从偏移量 offset 开始。不存在的 key 当作空白字符串处理。返回被 SETRANGE 修改之后，字符串的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="offset">起始的偏移量</param>
	/// <returns>被 SETRANGE 修改之后，字符串的长度。</returns>
	public async Task<OperateResult<int>> WriteKeyRangeAsync(string key, string value, int offset)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteKeyRangeAsync(key, value, offset));
	}

	/// <summary>
	/// 返回 key 所储存的字符串值的长度。当 key 储存的不是字符串值时，返回一个错误。返回符串值的长度。当 key 不存在时，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>字符串值的长度。当 key 不存在时，返回 0 。</returns>
	public async Task<OperateResult<int>> ReadKeyLengthAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadKeyLengthAsync(key));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 当中，位于值 pivot 之前。
	/// 当 pivot 不存在于列表 key 时，不执行任何操作。
	/// 当 key 不存在时， key 被视为空列表，不执行任何操作。
	/// 如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="pivot">原先的值</param>
	/// <returns>如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到 pivot ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。</returns>
	public OperateResult<int> ListInsertBefore(string key, string value, string pivot)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListInsertBefore(key, value, pivot));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 当中，位于值 pivot 之后。
	/// 当 pivot 不存在于列表 key 时，不执行任何操作。
	/// 当 key 不存在时， key 被视为空列表，不执行任何操作。
	/// 如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="pivot">原先的值</param>
	/// <returns>如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到 pivot ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。</returns>
	public OperateResult<int> ListInsertAfter(string key, string value, string pivot)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListInsertAfter(key, value, pivot));
	}

	/// <summary>
	/// 返回列表 key 的长度。如果 key 不存在，则 key 被解释为一个空列表，返回 0 .如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>列表 key 的长度。</returns>
	public OperateResult<int> GetListLength(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.GetListLength(key));
	}

	/// <summary>
	/// 返回列表 key 中，下标为 index 的元素。下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
	/// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="index">索引位置</param>
	/// <returns>列表中下标为 index 的元素。如果 index 参数的值不在列表的区间范围内(out of range)，返回 nil 。</returns>
	public OperateResult<string> ReadListByIndex(string key, long index)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadListByIndex(key, index));
	}

	/// <summary>
	/// 移除并返回列表 key 的头元素。列表的头元素。当 key 不存在时，返回 nil 。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <returns>列表的头元素。</returns>
	public OperateResult<string> ListLeftPop(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListLeftPop(key));
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>执行 LPUSH 命令后，列表的长度。</returns>
	public OperateResult<int> ListLeftPush(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListLeftPush(key, value));
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="values">值</param>
	/// <returns>执行 LPUSH 命令后，列表的长度。</returns>
	public OperateResult<int> ListLeftPush(string key, string[] values)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListLeftPush(key, values));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 的表头，当且仅当 key 存在并且是一个列表。和 LPUSH 命令相反，当 key 不存在时， LPUSHX 命令什么也不做。
	/// 返回LPUSHX 命令执行之后，表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>是否插入数据成功</returns>
	public OperateResult<int> ListLeftPushX(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListLeftPushX(key, value));
	}

	/// <summary>
	/// 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 stop 指定。
	/// 下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
	/// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
	/// 返回一个列表，包含指定区间内的元素。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="start">开始的索引</param>
	/// <param name="stop">结束的索引</param>
	/// <returns>返回一个列表，包含指定区间内的元素。</returns>
	public OperateResult<string[]> ListRange(string key, long start, long stop)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRange(key, start, stop));
	}

	/// <summary>
	/// 根据参数 count 的值，移除列表中与参数 value 相等的元素。count 的值可以是以下几种：
	/// count > 0 : 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count 。
	/// count < 0 : 从表尾开始向表头搜索，移除与 value 相等的元素，数量为 count 的绝对值。
	/// count = 0 : 移除表中所有与 value 相等的值。
	/// 返回被移除的数量。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="count">移除参数</param>
	/// <param name="value">匹配的值</param>
	/// <returns>被移除元素的数量。因为不存在的 key 被视作空表(empty list)，所以当 key 不存在时， LREM 命令总是返回 0 。</returns>
	public OperateResult<int> ListRemoveElementMatch(string key, long count, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRemoveElementMatch(key, count, value));
	}

	/// <summary>
	/// 设置数组的某一个索引的数据信息，当 index 参数超出范围，或对一个空列表( key 不存在)进行 LSET 时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="index">索引位置</param>
	/// <param name="value">值</param>
	/// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
	public OperateResult ListSet(string key, long index, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListSet(key, index, value));
	}

	/// <summary>
	/// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
	/// 举个例子，执行命令 LTRIM list 0 2 ，表示只保留列表 list 的前三个元素，其余元素全部删除。
	/// 下标( index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
	/// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
	/// 当 key 不是列表类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="start">起始的索引信息</param>
	/// <param name="end">结束的索引信息</param>
	/// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
	public OperateResult ListTrim(string key, long start, long end)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListTrim(key, start, end));
	}

	/// <summary>
	/// 移除并返回列表 key 的尾元素。当 key 不存在时，返回 nil 。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <returns>列表的尾元素。</returns>
	public OperateResult<string> ListRightPop(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRightPop(key));
	}

	/// <summary>
	/// 命令 RPOPLPUSH 在一个原子时间内，执行以下两个动作：
	/// 1. 将列表 source 中的最后一个元素( 尾元素)弹出，并返回给客户端。
	/// 2. 将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。
	/// 举个例子，你有两个列表 source 和 destination ， source 列表有元素 a, b, c ， destination 列表有元素 x, y, z ，执行 RPOPLPUSH source destination 之后， source 列表包含元素 a, b ， destination 列表包含元素 c, x, y, z ，并且元素 c 会被返回给客户端。
	/// 如果 source 不存在，值 nil 被返回，并且不执行其他动作。
	/// 如果 source 和 destination 相同，则列表中的表尾元素被移动到表头，并返回该元素，可以把这种特殊情况视作列表的旋转( rotation)操作。
	/// </summary>
	/// <param name="key1">第一个关键字</param>
	/// <param name="key2">第二个关键字</param>
	/// <returns>返回的移除的对象</returns>
	public OperateResult<string> ListRightPopLeftPush(string key1, string key2)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRightPopLeftPush(key1, key2));
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
	/// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
	public OperateResult<int> ListRightPush(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRightPush(key, value));
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
	/// 如果有多个 value 值，那么各个 value 值按从左到右的顺序依次插入到表尾：比如对一个空列表 mylist 执行 RPUSH mylist a b c ，得出的结果列表为 a b c ，
	/// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
	/// 返回执行 RPUSH 操作后，表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="values">值</param>
	/// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
	public OperateResult<int> ListRightPush(string key, string[] values)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRightPush(key, values));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 的表尾，当且仅当 key 存在并且是一个列表。
	/// 和 RPUSH 命令相反，当 key 不存在时， RPUSHX 命令什么也不做。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>RPUSHX 命令执行之后，表的长度。</returns>
	public OperateResult<int> ListRightPushX(string key, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.ListRightPushX(key, value));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 当中，位于值 pivot 之前。
	/// 当 pivot 不存在于列表 key 时，不执行任何操作。
	/// 当 key 不存在时， key 被视为空列表，不执行任何操作。
	/// 如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="pivot">原先的值</param>
	/// <returns>如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到 pivot ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。</returns>
	public async Task<OperateResult<int>> ListInsertBeforeAsync(string key, string value, string pivot)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListInsertBeforeAsync(key, value, pivot));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 当中，位于值 pivot 之后。
	/// 当 pivot 不存在于列表 key 时，不执行任何操作。
	/// 当 key 不存在时， key 被视为空列表，不执行任何操作。
	/// 如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">数值</param>
	/// <param name="pivot">原先的值</param>
	/// <returns>如果命令执行成功，返回插入操作完成之后，列表的长度。 如果没有找到 pivot ，返回 -1 。 如果 key 不存在或为空列表，返回 0 。</returns>
	public async Task<OperateResult<int>> ListInsertAfterAsync(string key, string value, string pivot)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListInsertAfterAsync(key, value, pivot));
	}

	/// <summary>
	/// 返回列表 key 的长度。如果 key 不存在，则 key 被解释为一个空列表，返回 0 .如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>列表 key 的长度。</returns>
	public async Task<OperateResult<int>> GetListLengthAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.GetListLengthAsync(key));
	}

	/// <summary>
	/// 返回列表 key 中，下标为 index 的元素。下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
	/// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。如果 key 不是列表类型，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="index">索引位置</param>
	/// <returns>列表中下标为 index 的元素。如果 index 参数的值不在列表的区间范围内(out of range)，返回 nil 。</returns>
	public async Task<OperateResult<string>> ReadListByIndexAsync(string key, long index)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadListByIndexAsync(key, index));
	}

	/// <summary>
	/// 移除并返回列表 key 的头元素。列表的头元素。当 key 不存在时，返回 nil 。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <returns>列表的头元素。</returns>
	public async Task<OperateResult<string>> ListLeftPopAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListLeftPopAsync(key));
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>执行 LPUSH 命令后，列表的长度。</returns>
	public async Task<OperateResult<int>> ListLeftPushAsync(string key, string value)
	{
		return await ListLeftPushAsync(key, new string[1] { value });
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表头，如果 key 不存在，一个空列表会被创建并执行 LPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。返回执行 LPUSH 命令后，列表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="values">值</param>
	/// <returns>执行 LPUSH 命令后，列表的长度。</returns>
	public async Task<OperateResult<int>> ListLeftPushAsync(string key, string[] values)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListLeftPushAsync(key, values));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 的表头，当且仅当 key 存在并且是一个列表。和 LPUSH 命令相反，当 key 不存在时， LPUSHX 命令什么也不做。
	/// 返回LPUSHX 命令执行之后，表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>是否插入数据成功</returns>
	public async Task<OperateResult<int>> ListLeftPushXAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListLeftPushXAsync(key, value));
	}

	/// <summary>
	/// 返回列表 key 中指定区间内的元素，区间以偏移量 start 和 stop 指定。
	/// 下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
	/// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
	/// 返回一个列表，包含指定区间内的元素。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="start">开始的索引</param>
	/// <param name="stop">结束的索引</param>
	/// <returns>返回一个列表，包含指定区间内的元素。</returns>
	public async Task<OperateResult<string[]>> ListRangeAsync(string key, long start, long stop)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListRangeAsync(key, start, stop));
	}

	/// <summary>
	/// 根据参数 count 的值，移除列表中与参数 value 相等的元素。count 的值可以是以下几种：
	/// count > 0 : 从表头开始向表尾搜索，移除与 value 相等的元素，数量为 count 。
	/// count < 0 : 从表尾开始向表头搜索，移除与 value 相等的元素，数量为 count 的绝对值。
	/// count = 0 : 移除表中所有与 value 相等的值。
	/// 返回被移除的数量。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="count">移除参数</param>
	/// <param name="value">匹配的值</param>
	/// <returns>被移除元素的数量。因为不存在的 key 被视作空表(empty list)，所以当 key 不存在时， LREM 命令总是返回 0 。</returns>
	public async Task<OperateResult<int>> ListRemoveElementMatchAsync(string key, long count, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListRemoveElementMatchAsync(key, count, value));
	}

	/// <summary>
	/// 设置数组的某一个索引的数据信息，当 index 参数超出范围，或对一个空列表( key 不存在)进行 LSET 时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="index">索引位置</param>
	/// <param name="value">值</param>
	/// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
	public async Task<OperateResult> ListSetAsync(string key, long index, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListSetAsync(key, index, value));
	}

	/// <summary>
	/// 对一个列表进行修剪(trim)，就是说，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
	/// 举个例子，执行命令 LTRIM list 0 2 ，表示只保留列表 list 的前三个元素，其余元素全部删除。
	/// 下标( index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
	/// 你也可以使用负数下标，以 -1 表示列表的最后一个元素， -2 表示列表的倒数第二个元素，以此类推。
	/// 当 key 不是列表类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="start">起始的索引信息</param>
	/// <param name="end">结束的索引信息</param>
	/// <returns>操作成功返回 ok ，否则返回错误信息。</returns>
	public async Task<OperateResult> ListTrimAsync(string key, long start, long end)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListTrimAsync(key, start, end));
	}

	/// <summary>
	/// 移除并返回列表 key 的尾元素。当 key 不存在时，返回 nil 。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <returns>列表的尾元素。</returns>
	public async Task<OperateResult<string>> ListRightPopAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListRightPopAsync(key));
	}

	/// <summary>
	/// 命令 RPOPLPUSH 在一个原子时间内，执行以下两个动作：
	/// 1. 将列表 source 中的最后一个元素( 尾元素)弹出，并返回给客户端。
	/// 2. 将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。
	/// 举个例子，你有两个列表 source 和 destination ， source 列表有元素 a, b, c ， destination 列表有元素 x, y, z ，执行 RPOPLPUSH source destination 之后， source 列表包含元素 a, b ， destination 列表包含元素 c, x, y, z ，并且元素 c 会被返回给客户端。
	/// 如果 source 不存在，值 nil 被返回，并且不执行其他动作。
	/// 如果 source 和 destination 相同，则列表中的表尾元素被移动到表头，并返回该元素，可以把这种特殊情况视作列表的旋转( rotation)操作。
	/// </summary>
	/// <param name="key1">第一个关键字</param>
	/// <param name="key2">第二个关键字</param>
	/// <returns>返回的移除的对象</returns>
	public async Task<OperateResult<string>> ListRightPopLeftPushAsync(string key1, string key2)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListRightPopLeftPushAsync(key1, key2));
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
	/// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
	public async Task<OperateResult<int>> ListRightPushAsync(string key, string value)
	{
		return await ListRightPushAsync(key, new string[1] { value });
	}

	/// <summary>
	/// 将一个或多个值 value 插入到列表 key 的表尾(最右边)。
	/// 如果有多个 value 值，那么各个 value 值按从左到右的顺序依次插入到表尾：比如对一个空列表 mylist 执行 RPUSH mylist a b c ，得出的结果列表为 a b c ，
	/// 如果 key 不存在，一个空列表会被创建并执行 RPUSH 操作。当 key 存在但不是列表类型时，返回一个错误。
	/// 返回执行 RPUSH 操作后，表的长度。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="values">值</param>
	/// <returns>返回执行 RPUSH 操作后，表的长度。</returns>
	public async Task<OperateResult<int>> ListRightPushAsync(string key, string[] values)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListRightPushAsync(key, values));
	}

	/// <summary>
	/// 将值 value 插入到列表 key 的表尾，当且仅当 key 存在并且是一个列表。
	/// 和 RPUSH 命令相反，当 key 不存在时， RPUSHX 命令什么也不做。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="value">值</param>
	/// <returns>RPUSHX 命令执行之后，表的长度。</returns>
	public async Task<OperateResult<int>> ListRightPushXAsync(string key, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ListRightPushXAsync(key, value));
	}

	/// <summary>
	/// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <returns>被成功移除的域的数量，不包括被忽略的域。</returns>
	public OperateResult<int> DeleteHashKey(string key, string field)
	{
		return ConnectPoolExecute((RedisClient m) => m.DeleteHashKey(key, field));
	}

	/// <summary>
	/// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。返回被成功移除的域的数量，不包括被忽略的域。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="fields">所有的域</param>
	/// <returns>返回被成功移除的域的数量，不包括被忽略的域。</returns>
	public OperateResult<int> DeleteHashKey(string key, string[] fields)
	{
		return ConnectPoolExecute((RedisClient m) => m.DeleteHashKey(key, fields));
	}

	/// <summary>
	/// 查看哈希表 key 中，给定域 field 是否存在。如果哈希表含有给定域，返回 1 。
	/// 如果哈希表不含有给定域，或 key 不存在，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <returns>如果哈希表含有给定域，返回 1 。如果哈希表不含有给定域，或 key 不存在，返回 0 。</returns>
	public OperateResult<int> ExistsHashKey(string key, string field)
	{
		return ConnectPoolExecute((RedisClient m) => m.ExistsHashKey(key, field));
	}

	/// <summary>
	/// 返回哈希表 key 中给定域 field 的值。当给定域不存在或是给定 key 不存在时，返回 nil
	/// </summary>
	/// <param name="key">关键值</param>
	/// <param name="field">域</param>
	/// <returns>给定域的值。 当给定域不存在或是给定 key 不存在时，返回 nil 。</returns>
	public OperateResult<string> ReadHashKey(string key, string field)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadHashKey(key, field));
	}

	/// <summary>
	/// 返回哈希表 key 中，所有的域和值。在返回值里，紧跟每个域名(field name)之后是域的值(value)，所以返回值的长度是哈希表大小的两倍。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <returns>以列表形式返回哈希表的域和域的值。 若 key 不存在，返回空列表。</returns>
	public OperateResult<string[]> ReadHashKeyAll(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadHashKeyAll(key));
	}

	/// <summary>
	/// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
	/// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">增量值</param>
	/// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
	public OperateResult<long> IncrementHashKey(string key, string field, long value)
	{
		return ConnectPoolExecute((RedisClient m) => m.IncrementHashKey(key, field, value));
	}

	/// <summary>
	/// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
	/// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">增量值</param>
	/// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
	public OperateResult<string> IncrementHashKey(string key, string field, float value)
	{
		return ConnectPoolExecute((RedisClient m) => m.IncrementHashKey(key, field, value));
	}

	/// <summary>
	/// 返回哈希表 key 中的所有域。当 key 不存在时，返回一个空表。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <returns>一个包含哈希表中所有域的表。 当 key 不存在时，返回一个空表。</returns>
	public OperateResult<string[]> ReadHashKeys(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadHashKeys(key));
	}

	/// <summary>
	/// 返回哈希表 key 中域的数量。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>哈希表中域的数量。当 key 不存在时，返回 0 。</returns>
	public OperateResult<int> ReadHashKeyLength(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadHashKeyLength(key));
	}

	/// <summary>
	/// 返回哈希表 key 中，一个或多个给定域的值。如果给定的域不存在于哈希表，那么返回一个 nil 值。
	/// 因为不存在的 key 被当作一个空哈希表来处理，所以对一个不存在的 key 进行 HMGET 操作将返回一个只带有 nil 值的表。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <param name="fields">指定的域</param>
	/// <returns>一个包含多个给定域的关联值的表，表值的排列顺序和给定域参数的请求顺序一样。</returns>
	public OperateResult<string[]> ReadHashKey(string key, string[] fields)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadHashKey(key, fields));
	}

	/// <summary>
	/// 将哈希表 key 中的域 field 的值设为 value 。
	/// 如果 key 不存在，一个新的哈希表被创建并进行 HSET 操作。
	/// 如果域 field 已经存在于哈希表中，旧值将被覆盖。
	/// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
	/// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">数据值</param>
	/// <returns>如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。</returns>
	public OperateResult<int> WriteHashKey(string key, string field, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteHashKey(key, field, value));
	}

	/// <summary>
	/// 同时将多个 field-value (域-值)对设置到哈希表 key 中。
	/// 此命令会覆盖哈希表中已存在的域。
	/// 如果 key 不存在，一个空哈希表被创建并执行 HMSET 操作。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="fields">域</param>
	/// <param name="values">数据值</param>
	/// <returns>如果命令执行成功，返回 OK 。 当 key 不是哈希表(hash)类型时，返回一个错误</returns>
	public OperateResult WriteHashKey(string key, string[] fields, string[] values)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteHashKey(key, fields, values));
	}

	/// <summary>
	/// 将哈希表 key 中的域 field 的值设置为 value ，当且仅当域 field 不存在。若域 field 已经存在，该操作无效。
	/// 设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">数据值</param>
	/// <returns>设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。</returns>
	public OperateResult<int> WriteHashKeyNx(string key, string field, string value)
	{
		return ConnectPoolExecute((RedisClient m) => m.WriteHashKeyNx(key, field, value));
	}

	/// <summary>
	/// 返回哈希表 key 中所有域的值。当 key 不存在时，返回一个空表。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <returns>返回哈希表 key 中所有域的值。 当 key 不存在时，返回一个空表。</returns>
	public OperateResult<string[]> ReadHashValues(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadHashValues(key));
	}

	/// <summary>
	/// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <returns>被成功移除的域的数量，不包括被忽略的域。</returns>
	public async Task<OperateResult<int>> DeleteHashKeyAsync(string key, string field)
	{
		return await DeleteHashKeyAsync(key, new string[1] { field });
	}

	/// <summary>
	/// 删除哈希表 key 中的一个或多个指定域，不存在的域将被忽略。返回被成功移除的域的数量，不包括被忽略的域。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="fields">所有的域</param>
	/// <returns>返回被成功移除的域的数量，不包括被忽略的域。</returns>
	public async Task<OperateResult<int>> DeleteHashKeyAsync(string key, string[] fields)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.DeleteHashKeyAsync(key, fields));
	}

	/// <summary>
	/// 查看哈希表 key 中，给定域 field 是否存在。如果哈希表含有给定域，返回 1 。
	/// 如果哈希表不含有给定域，或 key 不存在，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <returns>如果哈希表含有给定域，返回 1 。如果哈希表不含有给定域，或 key 不存在，返回 0 。</returns>
	public async Task<OperateResult<int>> ExistsHashKeyAsync(string key, string field)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ExistsHashKeyAsync(key, field));
	}

	/// <summary>
	/// 返回哈希表 key 中给定域 field 的值。当给定域不存在或是给定 key 不存在时，返回 nil
	/// </summary>
	/// <param name="key">关键值</param>
	/// <param name="field">域</param>
	/// <returns>给定域的值。 当给定域不存在或是给定 key 不存在时，返回 nil 。</returns>
	public async Task<OperateResult<string>> ReadHashKeyAsync(string key, string field)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadHashKeyAsync(key, field));
	}

	/// <summary>
	/// 返回哈希表 key 中，所有的域和值。在返回值里，紧跟每个域名(field name)之后是域的值(value)，所以返回值的长度是哈希表大小的两倍。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <returns>以列表形式返回哈希表的域和域的值。 若 key 不存在，返回空列表。</returns>
	public async Task<OperateResult<string[]>> ReadHashKeyAllAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadHashKeyAllAsync(key));
	}

	/// <summary>
	/// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
	/// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">增量值</param>
	/// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
	public async Task<OperateResult<long>> IncrementHashKeyAsync(string key, string field, long value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.IncrementHashKeyAsync(key, field, value));
	}

	/// <summary>
	/// 为哈希表 key 中的域 field 的值加上增量 increment 。增量也可以为负数，相当于对给定域进行减法操作。
	/// 如果 key 不存在，一个新的哈希表被创建并执行 HINCRBY 命令。返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">增量值</param>
	/// <returns>返回执行 HINCRBY 命令之后，哈希表 key 中域 field 的值。</returns>
	public async Task<OperateResult<string>> IncrementHashKeyAsync(string key, string field, float value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.IncrementHashKeyAsync(key, field, value));
	}

	/// <summary>
	/// 返回哈希表 key 中的所有域。当 key 不存在时，返回一个空表。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <returns>一个包含哈希表中所有域的表。 当 key 不存在时，返回一个空表。</returns>
	public async Task<OperateResult<string[]>> ReadHashKeysAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadHashKeysAsync(key));
	}

	/// <summary>
	/// 返回哈希表 key 中域的数量。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <returns>哈希表中域的数量。当 key 不存在时，返回 0 。</returns>
	public async Task<OperateResult<int>> ReadHashKeyLengthAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadHashKeyLengthAsync(key));
	}

	/// <summary>
	/// 返回哈希表 key 中，一个或多个给定域的值。如果给定的域不存在于哈希表，那么返回一个 nil 值。
	/// 因为不存在的 key 被当作一个空哈希表来处理，所以对一个不存在的 key 进行 HMGET 操作将返回一个只带有 nil 值的表。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <param name="fields">指定的域</param>
	/// <returns>一个包含多个给定域的关联值的表，表值的排列顺序和给定域参数的请求顺序一样。</returns>
	public async Task<OperateResult<string[]>> ReadHashKeyAsync(string key, string[] fields)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadHashKeyAsync(key, fields));
	}

	/// <summary>
	/// 将哈希表 key 中的域 field 的值设为 value 。
	/// 如果 key 不存在，一个新的哈希表被创建并进行 HSET 操作。
	/// 如果域 field 已经存在于哈希表中，旧值将被覆盖。
	/// 如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。
	/// 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">数据值</param>
	/// <returns>如果 field 是哈希表中的一个新建域，并且值设置成功，返回 1 。 如果哈希表中域 field 已经存在且旧值已被新值覆盖，返回 0 。</returns>
	public async Task<OperateResult<int>> WriteHashKeyAsync(string key, string field, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteHashKeyAsync(key, field, value));
	}

	/// <summary>
	/// 同时将多个 field-value (域-值)对设置到哈希表 key 中。
	/// 此命令会覆盖哈希表中已存在的域。
	/// 如果 key 不存在，一个空哈希表被创建并执行 HMSET 操作。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="fields">域</param>
	/// <param name="values">数据值</param>
	/// <returns>如果命令执行成功，返回 OK 。 当 key 不是哈希表(hash)类型时，返回一个错误</returns>
	public async Task<OperateResult> WriteHashKeyAsync(string key, string[] fields, string[] values)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteHashKeyAsync(key, fields, values));
	}

	/// <summary>
	/// 将哈希表 key 中的域 field 的值设置为 value ，当且仅当域 field 不存在。若域 field 已经存在，该操作无效。
	/// 设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。
	/// </summary>
	/// <param name="key">关键字</param>
	/// <param name="field">域</param>
	/// <param name="value">数据值</param>
	/// <returns>设置成功，返回 1 。如果给定域已经存在且没有操作被执行，返回 0 。</returns>
	public async Task<OperateResult<int>> WriteHashKeyNxAsync(string key, string field, string value)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteHashKeyNxAsync(key, field, value));
	}

	/// <summary>
	/// 返回哈希表 key 中所有域的值。当 key 不存在时，返回一个空表。
	/// </summary>
	/// <param name="key">关键值</param>
	/// <returns>返回哈希表 key 中所有域的值。 当 key 不存在时，返回一个空表。</returns>
	public async Task<OperateResult<string[]>> ReadHashValuesAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadHashValuesAsync(key));
	}

	/// <summary>
	/// 将一个member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略。假如 key 不存在，则创建一个只包含 member 元素作成员的集合。当 key 不是集合类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="member">等待添加的元素</param>
	/// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
	public OperateResult<int> SetAdd(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetAdd(key, member));
	}

	/// <summary>
	/// 将一个或多个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略。假如 key 不存在，则创建一个只包含 member 元素作成员的集合。当 key 不是集合类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="members">等待添加的元素</param>
	/// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
	public OperateResult<int> SetAdd(string key, string[] members)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetAdd(key, members));
	}

	/// <summary>
	/// 返回集合 key 的基数(集合中元素的数量)。当 key 不存在时，返回 0 。
	/// </summary>
	/// <param name="key">集合 key 的名称</param>
	/// <returns>集合的基数。</returns>
	public OperateResult<int> SetCard(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetCard(key));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合之间的差集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="diffKey">集合关键字</param>
	/// <returns>交集成员的列表。</returns>
	public OperateResult<string[]> SetDiff(string key, string diffKey)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetDiff(key, diffKey));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合之间的差集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="diffKeys">集合关键字</param>
	/// <returns>交集成员的列表。</returns>
	public OperateResult<string[]> SetDiff(string key, string[] diffKeys)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetDiff(key, diffKeys));
	}

	/// <summary>
	/// 这个命令的作用和 SDIFF 类似，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="diffKey">运算的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public OperateResult<int> SetDiffStore(string destination, string key, string diffKey)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetDiffStore(destination, key, diffKey));
	}

	/// <summary>
	/// 这个命令的作用和 SDIFF 类似，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="diffKeys">运算的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public OperateResult<int> SetDiffStore(string destination, string key, string[] diffKeys)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetDiffStore(destination, key, diffKeys));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合的交集。不存在的 key 被视为空集。当给定集合当中有一个空集时，结果也为空集(根据集合运算定律)。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="interKey">运算的集合</param>
	/// <returns>交集成员的列表。</returns>
	public OperateResult<string[]> SetInter(string key, string interKey)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetInter(key, interKey));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合的交集。不存在的 key 被视为空集。当给定集合当中有一个空集时，结果也为空集(根据集合运算定律)。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="interKeys">运算的集合</param>
	/// <returns>交集成员的列表。</returns>
	public OperateResult<string[]> SetInter(string key, string[] interKeys)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetInter(key, interKeys));
	}

	/// <summary>
	/// 这个命令类似于 SINTER 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="interKey">运算的集合</param>
	/// <returns>结果集中的成员数量。</returns>
	public OperateResult<int> SetInterStore(string destination, string key, string interKey)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetInterStore(destination, key, interKey));
	}

	/// <summary>
	/// 这个命令类似于 SINTER 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="interKeys">运算的集合</param>
	/// <returns>结果集中的成员数量。</returns>
	public OperateResult<int> SetInterStore(string destination, string key, string[] interKeys)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetInterStore(destination, key, interKeys));
	}

	/// <summary>
	/// 判断 member 元素是否集合 key 的成员。如果 member 元素是集合的成员，返回 1 。如果 member 元素不是集合的成员，或 key 不存在，返回 0 。
	/// </summary>
	/// <param name="key">集合key</param>
	/// <param name="member">元素</param>
	/// <returns>如果 member 元素是集合的成员，返回 1 。如果 member 元素不是集合的成员，或 key 不存在，返回 0 。</returns>
	public OperateResult<int> SetIsMember(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetIsMember(key, member));
	}

	/// <summary>
	/// 返回集合 key 中的所有成员。不存在的 key 被视为空集合。
	/// </summary>
	/// <param name="key">集合key</param>
	/// <returns>集合中的所有成员。</returns>
	public OperateResult<string[]> SetMembers(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetMembers(key));
	}

	/// <summary>
	/// 将 member 元素从 source 集合移动到 destination 集合。如果 source 集合不存在或不包含指定的 member 元素，则 SMOVE 命令不执行任何操作，仅返回 0 。
	/// 否则， member 元素从 source 集合中被移除，并添加到 destination 集合中去。当 destination 集合已经包含 member 元素时， SMOVE 命令只是简单地将 source 集合中的 member 元素删除。
	/// 当 source 或 destination 不是集合类型时，返回一个错误。
	/// </summary>
	/// <param name="source">原集合</param>
	/// <param name="destination">目标集合</param>
	/// <param name="member">元素</param>
	/// <returns>如果 member 元素被成功移除，返回 1 。如果 member 元素不是 source 集合的成员，并且没有任何操作对 destination 集合执行，那么返回 0 。</returns>
	public OperateResult<int> SetMove(string source, string destination, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetMove(source, destination, member));
	}

	/// <summary>
	/// 移除并返回集合中的一个随机元素。如果只想获取一个随机元素，但不想该元素从集合中被移除的话，可以使用 SRANDMEMBER 命令。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <returns>被移除的随机元素。当 key 不存在或 key 是空集时，返回 nil 。</returns>
	public OperateResult<string> SetPop(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetPop(key));
	}

	/// <summary>
	/// 那么返回集合中的一个随机元素。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <returns>返回一个元素；如果集合为空，返回 nil 。</returns>
	public OperateResult<string> SetRandomMember(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetRandomMember(key));
	}

	/// <summary>
	/// 返回集合中的多个随机元素。
	/// 如果 count 为正数，且小于集合基数，那么命令返回一个包含 count 个元素的数组，数组中的元素各不相同。如果 count 大于等于集合基数，那么返回整个集合。
	/// 如果 count 为负数，那么命令返回一个数组，数组中的元素可能会重复出现多次，而数组的长度为 count 的绝对值。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="count">元素个数</param>
	/// <returns>返回一个数组；如果集合为空，返回空数组。</returns>
	public OperateResult<string[]> SetRandomMember(string key, int count)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetRandomMember(key, count));
	}

	/// <summary>
	/// 移除集合 key 中的一个元素，不存在的 member 元素会被忽略。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="member">等待移除的元素</param>
	/// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
	public OperateResult<int> SetRemove(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetRemove(key, member));
	}

	/// <summary>
	/// 移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="members">等待移除的元素</param>
	/// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
	public OperateResult<int> SetRemove(string key, string[] members)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetRemove(key, members));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合的并集。不存在的 key 被视为空集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKey">并集的集合</param>
	/// <returns>并集成员的列表。</returns>
	public OperateResult<string[]> SetUnion(string key, string unionKey)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetUnion(key, unionKey));
	}

	/// <summary>
	/// 返回一个或多个集合的全部成员，该集合是所有给定集合的并集。不存在的 key 被视为空集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKeys">并集的集合</param>
	/// <returns>并集成员的列表。</returns>
	public OperateResult<string[]> SetUnion(string key, string[] unionKeys)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetUnion(key, unionKeys));
	}

	/// <summary>
	/// 这个命令类似于 SUNION 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">存储的目标集合</param>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKey">并集的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public OperateResult<int> SetUnionStore(string destination, string key, string unionKey)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetUnionStore(destination, key, unionKey));
	}

	/// <summary>
	/// 这个命令类似于 SUNION 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">存储的目标集合</param>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKeys">并集的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public OperateResult<int> SetUnionStore(string destination, string key, string[] unionKeys)
	{
		return ConnectPoolExecute((RedisClient m) => m.SetUnionStore(destination, key, unionKeys));
	}

	/// <summary>
	/// 将一个member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略。假如 key 不存在，则创建一个只包含 member 元素作成员的集合。当 key 不是集合类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="member">等待添加的元素</param>
	/// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
	public async Task<OperateResult<int>> SetAddAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetAddAsync(key, member));
	}

	/// <summary>
	/// 将一个或多个 member 元素加入到集合 key 当中，已经存在于集合的 member 元素将被忽略。假如 key 不存在，则创建一个只包含 member 元素作成员的集合。当 key 不是集合类型时，返回一个错误。
	/// </summary>
	/// <param name="key">关键字信息</param>
	/// <param name="members">等待添加的元素</param>
	/// <returns>被添加到集合中的新元素的数量，不包括被忽略的元素。</returns>
	public async Task<OperateResult<int>> SetAddAsync(string key, string[] members)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetAddAsync(key, members));
	}

	/// <summary>
	/// 返回集合 key 的基数(集合中元素的数量)。当 key 不存在时，返回 0 。
	/// </summary>
	/// <param name="key">集合 key 的名称</param>
	/// <returns>集合的基数。</returns>
	public async Task<OperateResult<int>> SetCardAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetCardAsync(key));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合之间的差集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="diffKey">集合关键字</param>
	/// <returns>交集成员的列表。</returns>
	public async Task<OperateResult<string[]>> SetDiffAsync(string key, string diffKey)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetDiffAsync(key, diffKey));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合之间的差集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="diffKeys">集合关键字</param>
	/// <returns>交集成员的列表。</returns>
	public async Task<OperateResult<string[]>> SetDiffAsync(string key, string[] diffKeys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetDiffAsync(key, diffKeys));
	}

	/// <summary>
	/// 这个命令的作用和 SDIFF 类似，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="diffKey">运算的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public async Task<OperateResult<int>> SetDiffStoreAsync(string destination, string key, string diffKey)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetDiffStoreAsync(destination, key, diffKey));
	}

	/// <summary>
	/// 这个命令的作用和 SDIFF 类似，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="diffKeys">运算的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public async Task<OperateResult<int>> SetDiffStoreAsync(string destination, string key, string[] diffKeys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetDiffStoreAsync(destination, key, diffKeys));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合的交集。不存在的 key 被视为空集。当给定集合当中有一个空集时，结果也为空集(根据集合运算定律)。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="interKey">运算的集合</param>
	/// <returns>交集成员的列表。</returns>
	public async Task<OperateResult<string[]>> SetInterAsync(string key, string interKey)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetInterAsync(key, interKey));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合的交集。不存在的 key 被视为空集。当给定集合当中有一个空集时，结果也为空集(根据集合运算定律)。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="interKeys">运算的集合</param>
	/// <returns>交集成员的列表。</returns>
	public async Task<OperateResult<string[]>> SetInterAsync(string key, string[] interKeys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetInterAsync(key, interKeys));
	}

	/// <summary>
	/// 这个命令类似于 SINTER 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="interKey">运算的集合</param>
	/// <returns>结果集中的成员数量。</returns>
	public async Task<OperateResult<int>> SetInterStoreAsync(string destination, string key, string interKey)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetInterStoreAsync(destination, key, interKey));
	}

	/// <summary>
	/// 这个命令类似于 SINTER 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 集合已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">目标集合</param>
	/// <param name="key">等待操作的集合</param>
	/// <param name="interKeys">运算的集合</param>
	/// <returns>结果集中的成员数量。</returns>
	public async Task<OperateResult<int>> SetInterStoreAsync(string destination, string key, string[] interKeys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetInterStoreAsync(destination, key, interKeys));
	}

	/// <summary>
	/// 判断 member 元素是否集合 key 的成员。如果 member 元素是集合的成员，返回 1 。如果 member 元素不是集合的成员，或 key 不存在，返回 0 。
	/// </summary>
	/// <param name="key">集合key</param>
	/// <param name="member">元素</param>
	/// <returns>如果 member 元素是集合的成员，返回 1 。如果 member 元素不是集合的成员，或 key 不存在，返回 0 。</returns>
	public async Task<OperateResult<int>> SetIsMemberAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetIsMemberAsync(key, member));
	}

	/// <summary>
	/// 返回集合 key 中的所有成员。不存在的 key 被视为空集合。
	/// </summary>
	/// <param name="key">集合key</param>
	/// <returns>集合中的所有成员。</returns>
	public async Task<OperateResult<string[]>> SetMembersAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetMembersAsync(key));
	}

	/// <summary>
	/// 将 member 元素从 source 集合移动到 destination 集合。如果 source 集合不存在或不包含指定的 member 元素，则 SMOVE 命令不执行任何操作，仅返回 0 。
	/// 否则， member 元素从 source 集合中被移除，并添加到 destination 集合中去。当 destination 集合已经包含 member 元素时， SMOVE 命令只是简单地将 source 集合中的 member 元素删除。
	/// 当 source 或 destination 不是集合类型时，返回一个错误。
	/// </summary>
	/// <param name="source">原集合</param>
	/// <param name="destination">目标集合</param>
	/// <param name="member">元素</param>
	/// <returns>如果 member 元素被成功移除，返回 1 。如果 member 元素不是 source 集合的成员，并且没有任何操作对 destination 集合执行，那么返回 0 。</returns>
	public async Task<OperateResult<int>> SetMoveAsync(string source, string destination, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetMoveAsync(source, destination, member));
	}

	/// <summary>
	/// 移除并返回集合中的一个随机元素。如果只想获取一个随机元素，但不想该元素从集合中被移除的话，可以使用 SRANDMEMBER 命令。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <returns>被移除的随机元素。当 key 不存在或 key 是空集时，返回 nil 。</returns>
	public async Task<OperateResult<string>> SetPopAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetPopAsync(key));
	}

	/// <summary>
	/// 那么返回集合中的一个随机元素。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <returns>返回一个元素；如果集合为空，返回 nil 。</returns>
	public async Task<OperateResult<string>> SetRandomMemberAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetRandomMemberAsync(key));
	}

	/// <summary>
	/// 返回集合中的多个随机元素。
	/// 如果 count 为正数，且小于集合基数，那么命令返回一个包含 count 个元素的数组，数组中的元素各不相同。如果 count 大于等于集合基数，那么返回整个集合。
	/// 如果 count 为负数，那么命令返回一个数组，数组中的元素可能会重复出现多次，而数组的长度为 count 的绝对值。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="count">元素个数</param>
	/// <returns>返回一个数组；如果集合为空，返回空数组。</returns>
	public async Task<OperateResult<string[]>> SetRandomMemberAsync(string key, int count)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetRandomMemberAsync(key, count));
	}

	/// <summary>
	/// 移除集合 key 中的一个元素，不存在的 member 元素会被忽略。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="member">等待移除的元素</param>
	/// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
	public async Task<OperateResult<int>> SetRemoveAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetRemoveAsync(key, member));
	}

	/// <summary>
	/// 移除集合 key 中的一个或多个 member 元素，不存在的 member 元素会被忽略。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="members">等待移除的元素</param>
	/// <returns>被成功移除的元素的数量，不包括被忽略的元素。</returns>
	public async Task<OperateResult<int>> SetRemoveAsync(string key, string[] members)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetRemoveAsync(key, members));
	}

	/// <summary>
	/// 返回一个集合的全部成员，该集合是所有给定集合的并集。不存在的 key 被视为空集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKey">并集的集合</param>
	/// <returns>并集成员的列表。</returns>
	public async Task<OperateResult<string[]>> SetUnionAsync(string key, string unionKey)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetUnionAsync(key, unionKey));
	}

	/// <summary>
	/// 返回一个或多个集合的全部成员，该集合是所有给定集合的并集。不存在的 key 被视为空集。
	/// </summary>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKeys">并集的集合</param>
	/// <returns>并集成员的列表。</returns>
	public async Task<OperateResult<string[]>> SetUnionAsync(string key, string[] unionKeys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetUnionAsync(key, unionKeys));
	}

	/// <summary>
	/// 这个命令类似于 SUNION 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">存储的目标集合</param>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKey">并集的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public async Task<OperateResult<int>> SetUnionStoreAsync(string destination, string key, string unionKey)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetUnionStoreAsync(destination, key, unionKey));
	}

	/// <summary>
	/// 这个命令类似于 SUNION 命令，但它将结果保存到 destination 集合，而不是简单地返回结果集。如果 destination 已经存在，则将其覆盖。destination 可以是 key 本身。
	/// </summary>
	/// <param name="destination">存储的目标集合</param>
	/// <param name="key">集合关键字</param>
	/// <param name="unionKeys">并集的集合</param>
	/// <returns>结果集中的元素数量。</returns>
	public async Task<OperateResult<int>> SetUnionStoreAsync(string destination, string key, string[] unionKeys)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SetUnionStoreAsync(destination, key, unionKeys));
	}

	/// <summary>
	/// 将一个 member 元素及其 score 值加入到有序集 key 当中。如果某个 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上。
	/// score 值可以是整数值或双精度浮点数。
	/// 如果 key 不存在，则创建一个空的有序集并执行 ZADD 操作。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">有序集合的元素</param>
	/// <param name="score">每个元素的得分</param>
	/// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
	public OperateResult<int> ZSetAdd(string key, string member, double score)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetAdd(key, member, score));
	}

	/// <summary>
	/// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中。如果某个 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上。
	/// score 值可以是整数值或双精度浮点数。
	/// 如果 key 不存在，则创建一个空的有序集并执行 ZADD 操作。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="members">有序集合的元素</param>
	/// <param name="scores">每个元素的得分</param>
	/// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
	public OperateResult<int> ZSetAdd(string key, string[] members, double[] scores)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetAdd(key, members, scores));
	}

	/// <summary>
	/// 返回有序集 key 的基数。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <returns>当 key 存在且是有序集类型时，返回有序集的基数。当 key 不存在时，返回 0 。</returns>
	public OperateResult<int> ZSetCard(string key)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetCard(key));
	}

	/// <summary>
	/// 返回有序集 key 中， score 值在 min 和 max 之间(默认包括 score 值等于 min 或 max )的成员的数量。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="min">最小值，包含</param>
	/// <param name="max">最大值，包含</param>
	/// <returns>score 值在 min 和 max 之间的成员的数量。</returns>
	public OperateResult<int> ZSetCount(string key, double min, double max)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetCount(key, min, max));
	}

	/// <summary>
	/// 为有序集 key 的成员 member 的 score 值加上增量 increment 。可以通过传递一个负数值 increment ，让 score 减去相应的值，比如 ZINCRBY key -5 member ，就是让 member 的 score 值减去 5 。
	/// 当 key 不存在，或 member 不是 key 的成员时， ZINCRBY key increment member 等同于 ZADD key increment member 。当 key 不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员名称</param>
	/// <param name="increment">增量数据，可以为负数</param>
	/// <returns>member 成员的新 score 值，以字符串形式表示。</returns>
	public OperateResult<string> ZSetIncreaseBy(string key, string member, double increment)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetIncreaseBy(key, member, increment));
	}

	/// <summary>
	/// 返回有序集 key 中，指定区间内的成员。其中成员的位置按 score 值递增(从小到大)来排序。具有相同 score 值的成员按字典序来排列。
	/// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="start">起始的下标</param>
	/// <param name="stop">结束的下标</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，根据参数 withScore 来决定是否带 score 值的有序集成员的列表。</returns>
	/// <remarks>
	/// 超出范围的下标并不会引起错误。比如说，当 start 的值比有序集的最大下标还要大，或是 start > stop 时， ZRANGE 命令只是简单地返回一个空列表。另一方面，假如 stop 参数的值比有序集的最大下标还要大，那么 Redis 将 stop 当作最大下标来处理。
	/// 可以通过使用 WITHSCORES 选项，来让成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示。客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
	/// </remarks>
	public OperateResult<string[]> ZSetRange(string key, int start, int stop, bool withScore = false)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRange(key, start, stop, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排列。
	/// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
	/// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 "(" 符号来使用可选的开区间 (小于或大于)。"(5"代表不包含5
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="min">最小值，可以为-inf，代表最高，如果为5，代表大于等于5，如果是(5，代表大于5</param>
	/// <param name="max">最大值，可以为+inf，代表最低，如果为10，代表小于等于5，如果是(10，代表小于10</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，带有 score 值(根据参数 withScore 来决定)的有序集成员的列表。</returns>
	public OperateResult<string[]> ZSetRangeByScore(string key, string min, string max, bool withScore = false)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRangeByScore(key, min, max, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递增(从小到大)顺序排列。排名以 0 为底，也就是说， score 值最小的成员排名为 0 。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员 member 的名称</param>
	/// <returns>如果 member 是有序集 key 的成员，返回 member 的排名。如果 member 不是有序集 key 的成员，返回 nil 。</returns>
	public OperateResult<int> ZSetRank(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRank(key, member));
	}

	/// <summary>
	/// 移除有序集 key 中的指定成员，不存在的成员将被忽略。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">等待被移除的成员</param>
	/// <returns>被成功移除的成员的数量，不包括被忽略的成员。</returns>
	public OperateResult<int> ZSetRemove(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRemove(key, member));
	}

	/// <summary>
	/// 移除有序集 key 中的一个或多个成员，不存在的成员将被忽略。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="members">等待被移除的成员</param>
	/// <returns>被成功移除的成员的数量，不包括被忽略的成员。</returns>
	public OperateResult<int> ZSetRemove(string key, string[] members)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRemove(key, members));
	}

	/// <summary>
	/// 移除有序集 key 中，指定排名(rank)区间内的所有成员。区间分别以下标参数 start 和 stop 指出，包含 start 和 stop 在内。
	/// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
	/// </summary>
	/// <param name="key">有序集合的关键</param>
	/// <param name="start">开始的下标</param>
	/// <param name="stop">结束的下标</param>
	/// <returns>被移除成员的数量。</returns>
	public OperateResult<int> ZSetRemoveRangeByRank(string key, int start, int stop)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRemoveRangeByRank(key, start, stop));
	}

	/// <summary>
	/// 移除有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。
	/// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
	/// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 "(" 符号来使用可选的开区间 (小于或大于)。例如"(5"代表不包括5
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="min">最小值，可以为-inf，代表最低，如果为5，代表大于等于5，如果是(5，代表大于5</param>
	/// <param name="max">最大值，可以为+inf，代表最低，如果为10，代表小于等于5，如果是(10，代表小于10</param>
	/// <returns>被移除成员的数量。</returns>
	public OperateResult<int> ZSetRemoveRangeByScore(string key, string min, string max)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetRemoveRangeByScore(key, min, max));
	}

	/// <summary>
	/// 返回有序集 key 中，指定区间内的成员。其中成员的位置按 score 值递减(从大到小)来排列。具有相同 score 值的成员按字典序来排列。
	/// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="start">起始的下标</param>
	/// <param name="stop">结束的下标</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，根据参数 withScore 来决定是否带 score 值的有序集成员的列表。</returns>
	/// <remarks>
	/// 超出范围的下标并不会引起错误。比如说，当 start 的值比有序集的最大下标还要大，或是 start > stop 时， ZRANGE 命令只是简单地返回一个空列表。另一方面，假如 stop 参数的值比有序集的最大下标还要大，那么 Redis 将 stop 当作最大下标来处理。
	/// 可以通过使用 WITHSCORES 选项，来让成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示。客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
	/// </remarks>
	public OperateResult<string[]> ZSetReverseRange(string key, int start, int stop, bool withScore = false)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetReverseRange(key, start, stop, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。序集成员按 score 值递减(从大到小)的次序排列。
	/// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
	/// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 ( 符号来使用可选的开区间 (小于或大于)。(5代表不包含5
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="max">最大值，可以为+inf，代表最高，如果为10，代表小于等于5，如果是(10，代表小于10</param>
	/// <param name="min">最小值，可以为-inf，代表最低，如果为5，代表大于等于5，如果是(5，代表大于5</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，带有 score 值(根据参数 withScore 来决定)的有序集成员的列表。</returns>
	public OperateResult<string[]> ZSetReverseRangeByScore(string key, string max, string min, bool withScore = false)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetReverseRangeByScore(key, max, min, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递减(从大到小)排序。排名以 0 为底，也就是说，score 值最大的成员排名为 0 。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员 member 的名称</param>
	/// <returns>如果 member 是有序集 key 的成员，返回 member 的排名。如果 member 不是有序集 key 的成员，返回 nil 。</returns>
	public OperateResult<int> ZSetReverseRank(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetReverseRank(key, member));
	}

	/// <summary>
	/// 返回有序集 key 中，成员 member 的 score 值。如果 member 元素不是有序集 key 的成员，或 key 不存在，返回 nil 。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员的名称</param>
	/// <returns>member 成员的 score 值，以字符串形式表示。</returns>
	public OperateResult<string> ZSetScore(string key, string member)
	{
		return ConnectPoolExecute((RedisClient m) => m.ZSetScore(key, member));
	}

	/// <summary>
	/// 将一个 member 元素及其 score 值加入到有序集 key 当中。如果某个 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上。
	/// score 值可以是整数值或双精度浮点数。
	/// 如果 key 不存在，则创建一个空的有序集并执行 ZADD 操作。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">有序集合的元素</param>
	/// <param name="score">每个元素的得分</param>
	/// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
	public async Task<OperateResult<int>> ZSetAddAsync(string key, string member, double score)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetAddAsync(key, member, score));
	}

	/// <summary>
	/// 将一个或多个 member 元素及其 score 值加入到有序集 key 当中。如果某个 member 已经是有序集的成员，那么更新这个 member 的 score 值，并通过重新插入这个 member 元素，来保证该 member 在正确的位置上。
	/// score 值可以是整数值或双精度浮点数。
	/// 如果 key 不存在，则创建一个空的有序集并执行 ZADD 操作。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="members">有序集合的元素</param>
	/// <param name="scores">每个元素的得分</param>
	/// <returns>被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
	public async Task<OperateResult<int>> ZSetAddAsync(string key, string[] members, double[] scores)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetAddAsync(key, members, scores));
	}

	/// <summary>
	/// 返回有序集 key 的基数。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <returns>当 key 存在且是有序集类型时，返回有序集的基数。当 key 不存在时，返回 0 。</returns>
	public async Task<OperateResult<int>> ZSetCardAsync(string key)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetCardAsync(key));
	}

	/// <summary>
	/// 返回有序集 key 中， score 值在 min 和 max 之间(默认包括 score 值等于 min 或 max )的成员的数量。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="min">最小值，包含</param>
	/// <param name="max">最大值，包含</param>
	/// <returns>score 值在 min 和 max 之间的成员的数量。</returns>
	public async Task<OperateResult<int>> ZSetCountAsync(string key, double min, double max)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetCountAsync(key, min, max));
	}

	/// <summary>
	/// 为有序集 key 的成员 member 的 score 值加上增量 increment 。可以通过传递一个负数值 increment ，让 score 减去相应的值，比如 ZINCRBY key -5 member ，就是让 member 的 score 值减去 5 。
	/// 当 key 不存在，或 member 不是 key 的成员时， ZINCRBY key increment member 等同于 ZADD key increment member 。当 key 不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员名称</param>
	/// <param name="increment">增量数据，可以为负数</param>
	/// <returns>member 成员的新 score 值，以字符串形式表示。</returns>
	public async Task<OperateResult<string>> ZSetIncreaseByAsync(string key, string member, double increment)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetIncreaseByAsync(key, member, increment));
	}

	/// <summary>
	/// 返回有序集 key 中，指定区间内的成员。其中成员的位置按 score 值递增(从小到大)来排序。具有相同 score 值的成员按字典序来排列。
	/// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="start">起始的下标</param>
	/// <param name="stop">结束的下标</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，根据参数 withScore 来决定是否带 score 值的有序集成员的列表。</returns>
	/// <remarks>
	/// 超出范围的下标并不会引起错误。比如说，当 start 的值比有序集的最大下标还要大，或是 start > stop 时， ZRANGE 命令只是简单地返回一个空列表。另一方面，假如 stop 参数的值比有序集的最大下标还要大，那么 Redis 将 stop 当作最大下标来处理。
	/// 可以通过使用 WITHSCORES 选项，来让成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示。客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
	/// </remarks>
	public async Task<OperateResult<string[]>> ZSetRangeAsync(string key, int start, int stop, bool withScore = false)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRangeAsync(key, start, stop, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。有序集成员按 score 值递增(从小到大)次序排列。
	/// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
	/// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 "(" 符号来使用可选的开区间 (小于或大于)。"(5"代表不包含5
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="min">最小值，可以为-inf，代表最高，如果为5，代表大于等于5，如果是(5，代表大于5</param>
	/// <param name="max">最大值，可以为+inf，代表最低，如果为10，代表小于等于5，如果是(10，代表小于10</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，带有 score 值(根据参数 withScore 来决定)的有序集成员的列表。</returns>
	public async Task<OperateResult<string[]>> ZSetRangeByScoreAsync(string key, string min, string max, bool withScore = false)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRangeByScoreAsync(key, min, max, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递增(从小到大)顺序排列。排名以 0 为底，也就是说， score 值最小的成员排名为 0 。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员 member 的名称</param>
	/// <returns>如果 member 是有序集 key 的成员，返回 member 的排名。如果 member 不是有序集 key 的成员，返回 nil 。</returns>
	public async Task<OperateResult<int>> ZSetRankAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRankAsync(key, member));
	}

	/// <summary>
	/// 移除有序集 key 中的指定成员，不存在的成员将被忽略。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">等待被移除的成员</param>
	/// <returns>被成功移除的成员的数量，不包括被忽略的成员。</returns>
	public async Task<OperateResult<int>> ZSetRemoveAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRemoveAsync(key, member));
	}

	/// <summary>
	/// 移除有序集 key 中的一个或多个成员，不存在的成员将被忽略。当 key 存在但不是有序集类型时，返回一个错误。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="members">等待被移除的成员</param>
	/// <returns>被成功移除的成员的数量，不包括被忽略的成员。</returns>
	public async Task<OperateResult<int>> ZSetRemoveAsync(string key, string[] members)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRemoveAsync(key, members));
	}

	/// <summary>
	/// 移除有序集 key 中，指定排名(rank)区间内的所有成员。区间分别以下标参数 start 和 stop 指出，包含 start 和 stop 在内。
	/// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
	/// </summary>
	/// <param name="key">有序集合的关键</param>
	/// <param name="start">开始的下标</param>
	/// <param name="stop">结束的下标</param>
	/// <returns>被移除成员的数量。</returns>
	public async Task<OperateResult<int>> ZSetRemoveRangeByRankAsync(string key, int start, int stop)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRemoveRangeByRankAsync(key, start, stop));
	}

	/// <summary>
	/// 移除有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。
	/// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
	/// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 "(" 符号来使用可选的开区间 (小于或大于)。例如"(5"代表不包括5
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="min">最小值，可以为-inf，代表最低，如果为5，代表大于等于5，如果是(5，代表大于5</param>
	/// <param name="max">最大值，可以为+inf，代表最低，如果为10，代表小于等于5，如果是(10，代表小于10</param>
	/// <returns>被移除成员的数量。</returns>
	public async Task<OperateResult<int>> ZSetRemoveRangeByScoreAsync(string key, string min, string max)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetRemoveRangeByScoreAsync(key, min, max));
	}

	/// <summary>
	/// 返回有序集 key 中，指定区间内的成员。其中成员的位置按 score 值递减(从大到小)来排列。具有相同 score 值的成员按字典序来排列。
	/// 下标参数 start 和 stop 都以 0 为底，也就是说，以 0 表示有序集第一个成员，以 1 表示有序集第二个成员，以此类推。你也可以使用负数下标，以 -1 表示最后一个成员， -2 表示倒数第二个成员，以此类推。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="start">起始的下标</param>
	/// <param name="stop">结束的下标</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，根据参数 withScore 来决定是否带 score 值的有序集成员的列表。</returns>
	/// <remarks>
	/// 超出范围的下标并不会引起错误。比如说，当 start 的值比有序集的最大下标还要大，或是 start > stop 时， ZRANGE 命令只是简单地返回一个空列表。另一方面，假如 stop 参数的值比有序集的最大下标还要大，那么 Redis 将 stop 当作最大下标来处理。
	/// 可以通过使用 WITHSCORES 选项，来让成员和它的 score 值一并返回，返回列表以 value1,score1, ..., valueN,scoreN 的格式表示。客户端库可能会返回一些更复杂的数据类型，比如数组、元组等。
	/// </remarks>
	public async Task<OperateResult<string[]>> ZSetReverseRangeAsync(string key, int start, int stop, bool withScore = false)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetReverseRangeAsync(key, start, stop, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中，所有 score 值介于 min 和 max 之间(包括等于 min 或 max )的成员。序集成员按 score 值递减(从大到小)的次序排列。
	/// min 和 max 可以是 -inf 和 +inf ，这样一来，你就可以在不知道有序集的最低和最高 score 值的情况下，使用 ZRANGEBYSCORE 这类命令。
	/// 默认情况下，区间的取值使用闭区间 (小于等于或大于等于)，你也可以通过给参数前增加 ( 符号来使用可选的开区间 (小于或大于)。(5代表不包含5
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="max">最大值，可以为+inf，代表最高，如果为10，代表小于等于5，如果是(10，代表小于10</param>
	/// <param name="min">最小值，可以为-inf，代表最低，如果为5，代表大于等于5，如果是(5，代表大于5</param>
	/// <param name="withScore">是否带有 score 返回</param>
	/// <returns>指定区间内，带有 score 值(根据参数 withScore 来决定)的有序集成员的列表。</returns>
	public async Task<OperateResult<string[]>> ZSetReverseRangeByScoreAsync(string key, string max, string min, bool withScore = false)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetReverseRangeByScoreAsync(key, max, min, withScore));
	}

	/// <summary>
	/// 返回有序集 key 中成员 member 的排名。其中有序集成员按 score 值递减(从大到小)排序。排名以 0 为底，也就是说，score 值最大的成员排名为 0 。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员 member 的名称</param>
	/// <returns>如果 member 是有序集 key 的成员，返回 member 的排名。如果 member 不是有序集 key 的成员，返回 nil 。</returns>
	public async Task<OperateResult<int>> ZSetReverseRankAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetReverseRankAsync(key, member));
	}

	/// <summary>
	/// 返回有序集 key 中，成员 member 的 score 值。如果 member 元素不是有序集 key 的成员，或 key 不存在，返回 nil 。
	/// </summary>
	/// <param name="key">有序集合的关键字</param>
	/// <param name="member">成员的名称</param>
	/// <returns>member 成员的 score 值，以字符串形式表示。</returns>
	public async Task<OperateResult<string>> ZSetScoreAsync(string key, string member)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ZSetScoreAsync(key, member));
	}

	public OperateResult<T> Read<T>() where T : class, new()
	{
		return ConnectPoolExecute((RedisClient m) => m.Read<T>());
	}

	public OperateResult Write<T>(T data) where T : class, new()
	{
		return ConnectPoolExecute((RedisClient m) => m.Write(data));
	}

	public async Task<OperateResult<T>> ReadAsync<T>() where T : class, new()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadAsync<T>());
	}

	public async Task<OperateResult> WriteAsync<T>(T data) where T : class, new()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.WriteAsync(data));
	}

	public OperateResult Save()
	{
		return ConnectPoolExecute((RedisClient m) => m.Save());
	}

	public OperateResult SaveAsync()
	{
		return ConnectPoolExecute((RedisClient m) => m.SaveAsync());
	}

	public OperateResult<DateTime> ReadServerTime()
	{
		return ConnectPoolExecute((RedisClient m) => m.ReadServerTime());
	}

	public OperateResult Ping()
	{
		return ConnectPoolExecute((RedisClient m) => m.Ping());
	}

	public OperateResult<long> DBSize()
	{
		return ConnectPoolExecute((RedisClient m) => m.DBSize());
	}

	public OperateResult FlushDB()
	{
		return ConnectPoolExecute((RedisClient m) => m.FlushDB());
	}

	public OperateResult ChangePassword(string password)
	{
		return ConnectPoolExecute((RedisClient m) => m.ChangePassword(password));
	}

	public async Task<OperateResult<DateTime>> ReadServerTimeAsync()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ReadServerTimeAsync());
	}

	public async Task<OperateResult> PingAsync()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.PingAsync());
	}

	public async Task<OperateResult<long>> DBSizeAsync()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.DBSizeAsync());
	}

	public async Task<OperateResult> FlushDBAsync()
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.FlushDBAsync());
	}

	public async Task<OperateResult> ChangePasswordAsync(string password)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.ChangePasswordAsync(password));
	}

	public OperateResult<int> Publish(string channel, string message)
	{
		return ConnectPoolExecute((RedisClient m) => m.Publish(channel, message));
	}

	public async Task<OperateResult<int>> PublishAsync(string channel, string message)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.PublishAsync(channel, message));
	}

	public OperateResult SelectDB(int db)
	{
		return ConnectPoolExecute((RedisClient m) => m.SelectDB(db));
	}

	public async Task<OperateResult> SelectDBAsync(int db)
	{
		return await ConnectPoolExecuteAsync((RedisClient m) => m.SelectDBAsync(db));
	}

	public override string ToString()
	{
		return $"RedisConnectPool[{redisConnectPool.MaxConnector}]";
	}
}

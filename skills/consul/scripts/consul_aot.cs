#:sdk Microsoft.NET.Sdk.Web
#:package Consul@1.7.10.1
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Consul.AOT
{
    /// <summary>
    /// Consul客户端配置选项
    /// </summary>
    public class ConsulClientOptions
    {
        /// <summary>
        /// Consul服务器地址
        /// </summary>
        public string Address { get; set; } = "http://localhost:8500";

        /// <summary>
        /// Datacenter名称
        /// </summary>
        public string Datacenter { get; set; } = string.Empty;

        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 用户名（用于HTTP认证）
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码（用于HTTP认证）
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Consul服务接口
    /// 定义了Consul客户端的核心功能
    /// </summary>
    public interface IConsulService
    {
        /// <summary>
        /// 获取Consul客户端实例
        /// </summary>
        /// <returns>Consul客户端实例</returns>
        IConsulClient GetClient();

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serviceId">服务ID</param>
        /// <param name="address">服务地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="tags">服务标签</param>
        /// <param name="healthCheck">健康检查配置</param>
        /// <returns>注册结果</returns>
        Task<bool> RegisterServiceAsync(string serviceName, string serviceId, string address, int port, string[] tags = null, AgentServiceCheck healthCheck = null);

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="serviceId">服务ID</param>
        /// <returns>注销结果</returns>
        Task<bool> DeregisterServiceAsync(string serviceId);

        /// <summary>
        /// 获取服务列表
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务列表</returns>
        Task<ServiceEntry[]> GetServicesAsync(string serviceName);

        /// <summary>
        /// 设置键值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>设置结果</returns>
        Task<bool> PutKeyValueAsync(string key, string value);

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>键值</returns>
        Task<string> GetKeyValueAsync(string key);

        /// <summary>
        /// 删除键值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>删除结果</returns>
        Task<bool> DeleteKeyValueAsync(string key);
    }

    /// <summary>
    /// Consul服务实现
    /// 基于.NET 10 AOT架构，提供高性能Consul客户端功能
    /// </summary>
    public class ConsulService : IConsulService
    {
        private readonly ILogger<ConsulService> _logger;
        private readonly ConsulClientOptions _options;
        private readonly Lazy<IConsulClient> _client;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public ConsulService(ILogger<ConsulService> logger, IOptions<ConsulClientOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _client = new Lazy<IConsulClient>(CreateConsulClient);
        }

        /// <summary>
        /// 创建Consul客户端
        /// </summary>
        /// <returns>Consul客户端实例</returns>
        private IConsulClient CreateConsulClient()
        {
            try
            {
                var configuration = new ConsulClientConfiguration
                {
                    Address = new Uri(_options.Address),
                    Datacenter = _options.Datacenter,
                    Token = _options.Token
                };

                // 添加HTTP认证（如果配置了）
                if (!string.IsNullOrEmpty(_options.Username) && !string.IsNullOrEmpty(_options.Password))
                {
                    configuration.HttpAuth = new System.Net.NetworkCredential(_options.Username, _options.Password);
                }

                _logger.LogInformation($"正在连接到Consul服务器: {_options.Address}");
                var client = new ConsulClient(configuration);
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建Consul客户端失败");
                throw;
            }
        }

        /// <summary>
        /// 获取Consul客户端实例
        /// </summary>
        public IConsulClient GetClient()
        {
            return _client.Value;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public async Task<bool> RegisterServiceAsync(string serviceName, string serviceId, string address, int port, string[] tags = null, AgentServiceCheck healthCheck = null)
        {
            try
            {
                var registration = new AgentServiceRegistration
                {
                    ID = serviceId,
                    Name = serviceName,
                    Address = address,
                    Port = port,
                    Tags = tags ?? Array.Empty<string>(),
                    Check = healthCheck
                };

                await _client.Value.Agent.ServiceRegister(registration);
                _logger.LogInformation($"服务注册成功: {serviceName} ({serviceId})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"服务注册失败: {serviceName} ({serviceId})");
                return false;
            }
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            try
            {
                await _client.Value.Agent.ServiceDeregister(serviceId);
                _logger.LogInformation($"服务注销成功: {serviceId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"服务注销失败: {serviceId}");
                return false;
            }
        }

        /// <summary>
        /// 获取服务列表
        /// </summary>
        public async Task<ServiceEntry[]> GetServicesAsync(string serviceName)
        {
            try
            {
                var queryResult = await _client.Value.Health.Service(serviceName, string.Empty, true);
                _logger.LogInformation($"获取服务列表成功: {serviceName}, 共找到 {queryResult.Response.Length} 个服务实例");
                return queryResult.Response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取服务列表失败: {serviceName}");
                return Array.Empty<ServiceEntry>();
            }
        }

        /// <summary>
        /// 设置键值
        /// </summary>
        public async Task<bool> PutKeyValueAsync(string key, string value)
        {
            try
            {
                var result = await _client.Value.KV.Put(new KVPair(key) { Value = System.Text.Encoding.UTF8.GetBytes(value) });
                if (result.Response)
                {
                    _logger.LogInformation($"键值设置成功: {key}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"键值设置失败: {key}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"键值设置失败: {key}");
                return false;
            }
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        public async Task<string> GetKeyValueAsync(string key)
        {
            try
            {
                var result = await _client.Value.KV.Get(key);
                if (result.Response != null)
                {
                    var value = System.Text.Encoding.UTF8.GetString(result.Response.Value);
                    _logger.LogInformation($"键值获取成功: {key}");
                    return value;
                }
                else
                {
                    _logger.LogWarning($"键值不存在: {key}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"键值获取失败: {key}");
                return null;
            }
        }

        /// <summary>
        /// 删除键值
        /// </summary>
        public async Task<bool> DeleteKeyValueAsync(string key)
        {
            try
            {
                var result = await _client.Value.KV.Delete(key);
                if (result.Response)
                {
                    _logger.LogInformation($"键值删除成功: {key}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"键值删除失败: {key}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"键值删除失败: {key}");
                return false;
            }
        }
    }

    /// <summary>
    /// Consul AOT执行引擎
    /// 管理Consul操作的执行
    /// </summary>
    public class ConsulAotEngine
    {
        private readonly ILogger<ConsulAotEngine> _logger;
        private readonly IConsulService _consulService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="consulService">Consul服务</param>
        public ConsulAotEngine(ILogger<ConsulAotEngine> logger, IConsulService consulService)
        {
            _logger = logger;
            _consulService = consulService;
        }

        /// <summary>
        /// 执行服务注册
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serviceId">服务ID</param>
        /// <param name="address">服务地址</param>
        /// <param name="port">服务端口</param>
        /// <returns>执行结果</returns>
        public async Task<bool> ExecuteRegisterServiceAsync(string serviceName, string serviceId, string address, int port)
        {
            return await _consulService.RegisterServiceAsync(serviceName, serviceId, address, port);
        }

        /// <summary>
        /// 执行服务发现
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务列表</returns>
        public async Task<ServiceEntry[]> ExecuteDiscoverServiceAsync(string serviceName)
        {
            return await _consulService.GetServicesAsync(serviceName);
        }

        /// <summary>
        /// 执行键值操作
        /// </summary>
        /// <param name="operation">操作类型：put, get, delete</param>
        /// <param name="key">键</param>
        /// <param name="value">值（仅用于put操作）</param>
        /// <returns>操作结果</returns>
        public async Task<object> ExecuteKeyValueOperationAsync(string operation, string key, string value = null)
        {
            switch (operation.ToLower())
            {
                case "put":
                    return await _consulService.PutKeyValueAsync(key, value);
                case "get":
                    return await _consulService.GetKeyValueAsync(key);
                case "delete":
                    return await _consulService.DeleteKeyValueAsync(key);
                default:
                    throw new NotSupportedException($"不支持的键值操作: {operation}");
            }
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 构建主机
            var builder = Host.CreateApplicationBuilder(args);

            // 配置Consul选项
            builder.Configuration.AddJsonFile("consul_aot.setting.json", optional: true);
            builder.Services.Configure<ConsulClientOptions>(builder.Configuration.GetSection("Consul"));

            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // 注册服务
            builder.Services.AddSingleton<IConsulService, ConsulService>();
            builder.Services.AddSingleton<ConsulAotEngine>();

            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;

            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<ConsulAotEngine>();

            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  服务注册: consul_aot.exe register <servicename> <serviceid> <address> <port>");
                Console.WriteLine("  服务发现: consul_aot.exe discover <servicename>");
                Console.WriteLine("  键值设置: consul_aot.exe kv put <key> <value>");
                Console.WriteLine("  键值获取: consul_aot.exe kv get <key>");
                Console.WriteLine("  键值删除: consul_aot.exe kv delete <key>");
                return 1;
            }

            try
            {
                bool result;
                string command = args[0].ToLower();

                switch (command)
                {
                    case "register":
                        if (args.Length < 5)
                        {
                            Console.WriteLine("注册服务需要4个参数: <servicename> <serviceid> <address> <port>");
                            return 1;
                        }
                        string serviceName = args[1];
                        string serviceId = args[2];
                        string address = args[3];
                        int port = int.Parse(args[4]);
                        result = await engine.ExecuteRegisterServiceAsync(serviceName, serviceId, address, port);
                        Console.WriteLine($"服务注册结果: {(result ? "成功" : "失败")}");
                        break;

                    case "discover":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("服务发现需要1个参数: <servicename>");
                            return 1;
                        }
                        string discoverServiceName = args[1];
                        var services = await engine.ExecuteDiscoverServiceAsync(discoverServiceName);
                        Console.WriteLine($"找到 {services.Length} 个服务实例:");
                        foreach (var service in services)
                        {
                            Console.WriteLine($"  - {service.Service.ID}: {service.Service.Address}:{service.Service.Port}");
                        }
                        result = services.Length > 0;
                        break;

                    case "kv":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("键值操作需要2个参数: <operation> <key> [value]");
                            return 1;
                        }
                        string kvOperation = args[1];
                        string key = args[2];
                        string kvValue = args.Length > 3 ? args[3] : null;
                        var kvResult = await engine.ExecuteKeyValueOperationAsync(kvOperation, key, kvValue);
                        if (kvOperation == "get")
                        {
                            Console.WriteLine($"键 '{key}' 的值: {kvResult}");
                        }
                        else
                        {
                            result = (bool)kvResult;
                            Console.WriteLine($"键值 {kvOperation} 结果: {(result ? "成功" : "失败")}");
                        }
                        break;

                    default:
                        Console.WriteLine($"未知命令: {command}");
                        return 1;
                }

                return result ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行错误: {ex.Message}");
                return 1;
            }
        }
    }
}
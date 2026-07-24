#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FreeIM.AOT
{
    /// <summary>
    /// FreeIM 命令类型枚举
    /// </summary>
    public enum FreeIMCommandType { SendMessage, ReceiveMessage, Connect, Disconnect, ListUsers, VersionInfo, Help }

    /// <summary>
    /// FreeIM 选项配置
    /// </summary>
    public class FreeIMOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
        
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 1000;
        
        /// <summary>
        /// IM 服务器地址
        /// </summary>
        public string ServerAddress { get; set; } = "localhost";
        
        /// <summary>
        /// IM 服务器端口
        /// </summary>
        public int ServerPort { get; set; } = 8080;
        
        /// <summary>
        /// 是否启用 SSL
        /// </summary>
        public bool EnableSsl { get; set; } = false;
    }

    /// <summary>
    /// 消息对象
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息 ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender { get; set; } = string.Empty;
        
        /// <summary>
        /// 接收者
        /// </summary>
        public string Receiver { get; set; } = string.Empty;
        
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type { get; set; } = "text";
        
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; } = false;
    }

    /// <summary>
    /// 用户对象
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = "offline";
        
        /// <summary>
        /// 最后在线时间
        /// </summary>
        public DateTime LastOnline { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// FreeIM 命令结果
    /// </summary>
    public class FreeIMCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 消息列表
        /// </summary>
        public List<Message>? Messages { get; set; }
        
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<User>? Users { get; set; }
    }

    /// <summary>
    /// FreeIM 服务接口
    /// </summary>
    public interface IFreeIMService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<FreeIMCommandResult> ExecuteCommandAsync(FreeIMCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="receiver">接收者</param>
        /// <param name="content">消息内容</param>
        /// <param name="type">消息类型</param>
        /// <returns>发送结果</returns>
        Task<FreeIMCommandResult> SendMessageAsync(string sender, string receiver, string content, string type = "text");
        
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="receiver">接收者</param>
        /// <returns>接收结果</returns>
        Task<FreeIMCommandResult> ReceiveMessageAsync(string receiver);
        
        /// <summary>
        /// 连接到 IM 服务器
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>连接结果</returns>
        Task<FreeIMCommandResult> ConnectAsync(string userId);
        
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>断开结果</returns>
        Task<FreeIMCommandResult> DisconnectAsync(string userId);
        
        /// <summary>
        /// 列出在线用户
        /// </summary>
        /// <returns>用户列表</returns>
        Task<FreeIMCommandResult> ListUsersAsync();
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<FreeIMCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// FreeIM 服务实现
    /// </summary>
    public class FreeIMService : IFreeIMService
    {
        private readonly FreeIMOptions _options;
        private readonly ILogger<FreeIMService> _logger;
        private readonly List<Message> _messages = new List<Message>();
        private readonly List<User> _users = new List<User>();
        private readonly HashSet<string> _connectedUsers = new HashSet<string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">FreeIM 选项</param>
        /// <param name="logger">日志记录器</param>
        public FreeIMService(IOptions<FreeIMOptions> options, ILogger<FreeIMService> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            // 初始化示例用户
            InitializeSampleData();
        }

        /// <summary>
        /// 初始化示例数据
        /// </summary>
        private void InitializeSampleData()
        {
            // 添加示例用户
            _users.Add(new User { Id = "1", Username = "user1", DisplayName = "用户1", Status = "offline" });
            _users.Add(new User { Id = "2", Username = "user2", DisplayName = "用户2", Status = "offline" });
            _users.Add(new User { Id = "3", Username = "user3", DisplayName = "用户3", Status = "offline" });
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> ExecuteCommandAsync(FreeIMCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                switch (commandType)
                {
                    case FreeIMCommandType.SendMessage:
                        if (parameters?.ContainsKey("sender") == true && parameters?.ContainsKey("receiver") == true && parameters?.ContainsKey("content") == true)
                        {
                            string sender = parameters["sender"];
                            string receiver = parameters["receiver"];
                            string content = parameters["content"];
                            string type = parameters?.ContainsKey("type") == true ? parameters["type"] : "text";
                            result = await SendMessageAsync(sender, receiver, content, type);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: sender, receiver 和 content";
                        }
                        break;
                    
                    case FreeIMCommandType.ReceiveMessage:
                        if (parameters?.ContainsKey("receiver") == true)
                        {
                            string receiver = parameters["receiver"];
                            result = await ReceiveMessageAsync(receiver);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: receiver";
                        }
                        break;
                    
                    case FreeIMCommandType.Connect:
                        if (parameters?.ContainsKey("userId") == true)
                        {
                            string userId = parameters["userId"];
                            result = await ConnectAsync(userId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: userId";
                        }
                        break;
                    
                    case FreeIMCommandType.Disconnect:
                        if (parameters?.ContainsKey("userId") == true)
                        {
                            string userId = parameters["userId"];
                            result = await DisconnectAsync(userId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "缺少必要的参数: userId";
                        }
                        break;
                    
                    case FreeIMCommandType.ListUsers:
                        result = await ListUsersAsync();
                        break;
                    
                    case FreeIMCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = "未知命令类型";
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> SendMessageAsync(string sender, string receiver, string content, string type = "text")
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                _logger.LogInformation("发送消息: 从 {Sender} 到 {Receiver}", sender, receiver);
                
                // 检查用户是否存在
                var senderUser = _users.FirstOrDefault(u => u.Id == sender || u.Username == sender);
                var receiverUser = _users.FirstOrDefault(u => u.Id == receiver || u.Username == receiver);
                
                if (senderUser == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "发送者不存在";
                    return result;
                }
                
                if (receiverUser == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "接收者不存在";
                    return result;
                }
                
                // 检查发送者是否在线
                if (!_connectedUsers.Contains(senderUser.Id))
                {
                    result.Success = false;
                    result.ErrorMessage = "发送者未连接";
                    return result;
                }
                
                // 创建消息
                var message = new Message
                {
                    Sender = senderUser.Id,
                    Receiver = receiverUser.Id,
                    Content = content,
                    Type = type,
                    SendTime = DateTime.Now
                };
                
                // 添加到消息列表
                _messages.Add(message);
                
                // 模拟发送延迟
                await Task.Delay(100);
                
                result.Success = true;
                result.Results.Add($"成功发送消息: {message.Id}");
                result.Results.Add($"发送者: {senderUser.DisplayName}");
                result.Results.Add($"接收者: {receiverUser.DisplayName}");
                result.Results.Add($"消息类型: {type}");
                result.Results.Add($"发送时间: {message.SendTime}");
                result.Messages = new List<Message> { message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送消息时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> ReceiveMessageAsync(string receiver)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                _logger.LogInformation("接收消息: 接收者 {Receiver}", receiver);
                
                // 检查用户是否存在
                var receiverUser = _users.FirstOrDefault(u => u.Id == receiver || u.Username == receiver);
                
                if (receiverUser == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "接收者不存在";
                    return result;
                }
                
                // 检查接收者是否在线
                if (!_connectedUsers.Contains(receiverUser.Id))
                {
                    result.Success = false;
                    result.ErrorMessage = "接收者未连接";
                    return result;
                }
                
                // 获取未读消息
                var unreadMessages = _messages.Where(m => m.Receiver == receiverUser.Id && !m.IsRead).ToList();
                
                // 模拟接收延迟
                await Task.Delay(50);
                
                // 标记消息为已读
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                }
                
                result.Success = true;
                result.Results.Add($"成功接收消息: {unreadMessages.Count} 条");
                result.Results.Add($"接收者: {receiverUser.DisplayName}");
                result.Messages = unreadMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "接收消息时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> ConnectAsync(string userId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                _logger.LogInformation("连接到 IM 服务器: {UserId}", userId);
                
                // 检查用户是否存在
                var user = _users.FirstOrDefault(u => u.Id == userId || u.Username == userId);
                
                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "用户不存在";
                    return result;
                }
                
                // 检查用户是否已连接
                if (_connectedUsers.Contains(user.Id))
                {
                    result.Success = false;
                    result.ErrorMessage = "用户已连接";
                    return result;
                }
                
                // 模拟连接过程
                await Task.Delay(200);
                
                // 添加到已连接用户
                _connectedUsers.Add(user.Id);
                
                // 更新用户状态
                user.Status = "online";
                user.LastOnline = DateTime.Now;
                
                result.Success = true;
                result.Results.Add($"成功连接到 IM 服务器");
                result.Results.Add($"用户: {user.DisplayName}");
                result.Results.Add($"服务器地址: {_options.ServerAddress}:{_options.ServerPort}");
                result.Results.Add($"连接时间: {DateTime.Now}");
                result.Users = new List<User> { user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "连接到 IM 服务器时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> DisconnectAsync(string userId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                _logger.LogInformation("断开连接: {UserId}", userId);
                
                // 检查用户是否存在
                var user = _users.FirstOrDefault(u => u.Id == userId || u.Username == userId);
                
                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "用户不存在";
                    return result;
                }
                
                // 检查用户是否已连接
                if (!_connectedUsers.Contains(user.Id))
                {
                    result.Success = false;
                    result.ErrorMessage = "用户未连接";
                    return result;
                }
                
                // 模拟断开过程
                await Task.Delay(100);
                
                // 从已连接用户中移除
                _connectedUsers.Remove(user.Id);
                
                // 更新用户状态
                user.Status = "offline";
                user.LastOnline = DateTime.Now;
                
                result.Success = true;
                result.Results.Add($"成功断开连接");
                result.Results.Add($"用户: {user.DisplayName}");
                result.Results.Add($"断开时间: {DateTime.Now}");
                result.Users = new List<User> { user };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "断开连接时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> ListUsersAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                _logger.LogInformation("列出在线用户");
                
                // 模拟获取用户列表
                await Task.Delay(50);
                
                result.Success = true;
                result.Results.Add($"成功获取用户列表");
                result.Results.Add($"总用户数: {_users.Count}");
                result.Results.Add($"在线用户数: {_connectedUsers.Count}");
                result.Users = _users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出用户时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<FreeIMCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FreeIMCommandResult();

            try
            {
                _logger.LogInformation("获取 FreeIM 版本信息");
                
                // 模拟获取版本信息
                await Task.Delay(50);
                
                result.Success = true;
                result.Results.Add("FreeIM AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"服务器地址: {_options.ServerAddress}:{_options.ServerPort}");
                result.Results.Add($"启用 SSL: {_options.EnableSsl}");
                result.Results.Add($"工作目录: {_options.WorkingDirectory}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取版本信息时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }
    }

    /// <summary>
    /// FreeIM AOT 引擎
    /// </summary>
    public class FreeIMAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FreeIMAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public FreeIMAotEngine(IServiceProvider serviceProvider, ILogger<FreeIMAotEngine> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 执行命令行操作
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出码</returns>
        public async Task<int> ExecuteCommandLineAsync(string[] args)
        {
            _logger.LogInformation("FreeIM AOT Engine 启动，参数: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var freeIMService = _serviceProvider.GetRequiredService<IFreeIMService>();
            FreeIMCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "send":
                    case "sendmessage":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("错误: 需要提供发送者、接收者和消息内容");
                            return 1;
                        }
                        string sender = args[1];
                        string receiver = args[2];
                        string content = string.Join(" ", args.Skip(3));
                        string type = "text";
                        if (args.Length > 4 && args[4].StartsWith("type:"))
                        {
                            type = args[4].Substring(5);
                        }
                        result = await freeIMService.SendMessageAsync(sender, receiver, content, type);
                        break;
                    
                    case "receive":
                    case "receivemessage":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供接收者");
                            return 1;
                        }
                        string receiveReceiver = args[1];
                        result = await freeIMService.ReceiveMessageAsync(receiveReceiver);
                        break;
                    
                    case "connect":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供用户 ID");
                            return 1;
                        }
                        string connectUserId = args[1];
                        result = await freeIMService.ConnectAsync(connectUserId);
                        break;
                    
                    case "disconnect":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供用户 ID");
                            return 1;
                        }
                        string disconnectUserId = args[1];
                        result = await freeIMService.DisconnectAsync(disconnectUserId);
                        break;
                    
                    case "list":
                    case "listusers":
                        result = await freeIMService.ListUsersAsync();
                        break;
                    
                    case "version":
                    case "info":
                        result = await freeIMService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"错误: 未知命令 '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                return 1;
            }

            // 显示结果
            if (result != null)
            {
                Console.WriteLine($"\n命令执行结果: {(result.Success ? "成功" : "失败")}");
                Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
                
                if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"错误信息: {result.ErrorMessage}");
                }
                
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"- {item}");
                }
                
                if (result.Messages != null && result.Messages.Any())
                {
                    Console.WriteLine($"\n消息列表:");
                    foreach (var message in result.Messages)
                    {
                        Console.WriteLine($"  [{message.SendTime}] {message.Sender} -> {message.Receiver}: {message.Content}");
                    }
                }
                
                if (result.Users != null && result.Users.Any())
                {
                    Console.WriteLine($"\n用户列表:");
                    foreach (var user in result.Users)
                    {
                        Console.WriteLine($"  {user.DisplayName} ({user.Username}) - {user.Status}");
                    }
                }
            }
            
            return result?.Success == true ? 0 : 1;
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        private void ShowHelp()
        {
            Console.WriteLine("FreeIM AOT Engine - .NET 10 AOT 编译的即时通讯引擎");
            Console.WriteLine();
            Console.WriteLine("用法: freeim_aot <命令> [参数]");
            Console.WriteLine();
            Console.WriteLine("命令:");
            Console.WriteLine("  send|sendmessage <sender> <receiver> <content> [type:<type>]    发送消息");
            Console.WriteLine("  receive|receivemessage <receiver>                    接收消息");
            Console.WriteLine("  connect <userId>                                   连接到 IM 服务器");
            Console.WriteLine("  disconnect <userId>                                断开连接");
            Console.WriteLine("  list|listusers                                     列出用户");
            Console.WriteLine("  version|info                                       显示版本信息");
            Console.WriteLine("  help|--help|-h                                     显示帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  freeim_aot connect user1");
            Console.WriteLine("  freeim_aot send user1 user2 'Hello World'");
            Console.WriteLine("  freeim_aot receive user2");
            Console.WriteLine("  freeim_aot list");
            Console.WriteLine("  freeim_aot disconnect user1");
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
        /// <returns>退出码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 创建主机
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            // 配置服务
            builder.Services.Configure<FreeIMOptions>(builder.Configuration.GetSection("FreeIM"));
            builder.Services.AddSingleton<IFreeIMService, FreeIMService>();
            builder.Services.AddSingleton<FreeIMAotEngine>();
            
            // 构建主机
            using var host = builder.Build();
            
            // 获取引擎实例
            var engine = host.Services.GetRequiredService<FreeIMAotEngine>();
            
            // 执行命令
            return await engine.ExecuteCommandLineAsync(args);
        }
    }
}
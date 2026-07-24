#:sdk Microsoft.NET.Sdk
#:package Twilio@7.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Scrutor@4.0.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Logging.Console@10.0.0
#:package Newtonsoft.Json@13.0.3
#:property LangVersion=latest
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SmsSkill
{
    // 数据模型
    public class SmsResult
    {
        public bool Success { get; set; }
        public string MessageSid { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SmsTemplate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SmsScheduleResult
    {
        public bool Success { get; set; }
        public string ScheduleId { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public DateTime SendTime { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SmsAnalytics
    {
        public int TotalSms { get; set; }
        public int SuccessfulSms { get; set; }
        public double SuccessRate { get; set; }
        public double AverageMessageLength { get; set; }
        public Dictionary<string, int> StatusDistribution { get; set; }
        public Dictionary<DateTime, int> DailyCounts { get; set; }
    }

    public class SmsEvent
    {
        public string Id { get; set; }
        public string MessageSid { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime EventTime { get; set; }
        public string EventType { get; set; }
    }

    public class SmsMessage
    {
        public string MessageSid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public DateTime ReceivedAt { get; set; }
        public Dictionary<string, string> MediaUrls { get; set; }
    }

    // 配置选项
    public class TwilioOptions
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromNumber { get; set; }
    }

    public class SmsOptions
    {
        public int BatchSize { get; set; } = 100;
        public int RateLimitPerSecond { get; set; } = 10;
        public string TemplateDirectory { get; set; } = "./templates";
        public string AnalyticsStoragePath { get; set; } = "./analytics";
    }

    // 核心接口
    public interface ISmsProvider
    {
        Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default);
        Task<SmsResult> SendWithMediaAsync(string to, string message, IEnumerable<Uri> mediaUrls, CancellationToken cancellationToken = default);
        Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default);
    }

    public interface ITemplateService
    {
        Task<string> CreateTemplateAsync(string name, string content, CancellationToken cancellationToken = default);
        Task<IEnumerable<SmsTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
        Task<SmsTemplate> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default);
        Task UpdateTemplateAsync(string templateId, string name, string content, CancellationToken cancellationToken = default);
        Task DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default);
        Task<string> RenderTemplateAsync(string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    }

    public interface IAnalyticsService
    {
        Task RecordSmsEventAsync(SmsEvent @event, CancellationToken cancellationToken = default);
        Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<SmsEvent>> GetEventsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }

    public interface ISmsService
    {
        Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default);
        Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
        Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default);
        Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default);
        Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default);
    }

    // 实现类
    public class TwilioSmsProvider : ISmsProvider
    {
        private readonly TwilioOptions _options;
        private readonly ILogger<TwilioSmsProvider> _logger;

        public TwilioSmsProvider(IOptions<TwilioOptions> options, ILogger<TwilioSmsProvider> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            // 初始化 Twilio 客户端
            TwilioClient.Init(_options.AccountSid, _options.AuthToken);
        }

        public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("发送SMS到 {To}", to);
                
                var messageOptions = new CreateMessageOptions(new PhoneNumber(to))
                {
                    From = new PhoneNumber(_options.FromNumber),
                    Body = message
                };

                var twilioMessage = await MessageResource.CreateAsync(messageOptions, cancellationToken: cancellationToken);
                
                return new SmsResult
                {
                    Success = true,
                    MessageSid = twilioMessage.Sid,
                    To = to,
                    From = _options.FromNumber,
                    SentAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送SMS失败到 {To}", to);
                return new SmsResult
                {
                    Success = false,
                    To = to,
                    From = _options.FromNumber,
                    ErrorMessage = ex.Message,
                    SentAt = DateTime.Now
                };
            }
        }

        public async Task<SmsResult> SendWithMediaAsync(string to, string message, IEnumerable<Uri> mediaUrls, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("发送带媒体的SMS到 {To}", to);
                
                var messageOptions = new CreateMessageOptions(new PhoneNumber(to))
                {
                    From = new PhoneNumber(_options.FromNumber),
                    Body = message
                };

                foreach (var mediaUrl in mediaUrls)
                {
                    messageOptions.MediaUrl.Add(mediaUrl);
                }

                var twilioMessage = await MessageResource.CreateAsync(messageOptions, cancellationToken: cancellationToken);
                
                return new SmsResult
                {
                    Success = true,
                    MessageSid = twilioMessage.Sid,
                    To = to,
                    From = _options.FromNumber,
                    SentAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送带媒体的SMS失败到 {To}", to);
                return new SmsResult
                {
                    Success = false,
                    To = to,
                    From = _options.FromNumber,
                    ErrorMessage = ex.Message,
                    SentAt = DateTime.Now
                };
            }
        }

        public Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            // Twilio 接收 SMS 通过 webhook 实现，这里返回空集合
            // 实际实现需要配置 webhook 端点并处理回调
            return Task.FromResult<IEnumerable<SmsMessage>>(new List<SmsMessage>());
        }
    }

    public class TemplateService : ITemplateService
    {
        private readonly SmsOptions _options;
        private readonly ILogger<TemplateService> _logger;
        private readonly Dictionary<string, SmsTemplate> _templates;

        public TemplateService(IOptions<SmsOptions> options, ILogger<TemplateService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _templates = new Dictionary<string, SmsTemplate>();
            
            // 初始化模板存储目录
            Directory.CreateDirectory(_options.TemplateDirectory);
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            try
            {
                var templateFiles = Directory.GetFiles(_options.TemplateDirectory, "*.json");
                foreach (var file in templateFiles)
                {
                    var content = File.ReadAllText(file);
                    var template = JsonConvert.DeserializeObject<SmsTemplate>(content);
                    if (template != null)
                    {
                        _templates[template.Id] = template;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载模板失败");
            }
        }

        private void SaveTemplate(SmsTemplate template)
        {
            try
            {
                var filePath = Path.Combine(_options.TemplateDirectory, $"{template.Id}.json");
                var content = JsonConvert.SerializeObject(template, Formatting.Indented);
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存模板失败: {TemplateId}", template.Id);
            }
        }

        public Task<string> CreateTemplateAsync(string name, string content, CancellationToken cancellationToken = default)
        {
            var templateId = Guid.NewGuid().ToString();
            var template = new SmsTemplate
            {
                Id = templateId,
                Name = name,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _templates[templateId] = template;
            SaveTemplate(template);
            
            _logger.LogInformation("创建模板: {TemplateName} (ID: {TemplateId})", name, templateId);
            return Task.FromResult(templateId);
        }

        public Task<IEnumerable<SmsTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<SmsTemplate>>(_templates.Values);
        }

        public Task<SmsTemplate> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default)
        {
            if (_templates.TryGetValue(templateId, out var template))
            {
                return Task.FromResult(template);
            }
            throw new KeyNotFoundException($"模板不存在: {templateId}");
        }

        public Task UpdateTemplateAsync(string templateId, string name, string content, CancellationToken cancellationToken = default)
        {
            if (_templates.TryGetValue(templateId, out var template))
            {
                template.Name = name;
                template.Content = content;
                template.UpdatedAt = DateTime.Now;
                SaveTemplate(template);
                
                _logger.LogInformation("更新模板: {TemplateId}", templateId);
            }
            else
            {
                throw new KeyNotFoundException($"模板不存在: {templateId}");
            }
            return Task.CompletedTask;
        }

        public Task DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default)
        {
            if (_templates.Remove(templateId))
            {
                var filePath = Path.Combine(_options.TemplateDirectory, $"{templateId}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _logger.LogInformation("删除模板: {TemplateId}", templateId);
            }
            else
            {
                throw new KeyNotFoundException($"模板不存在: {templateId}");
            }
            return Task.CompletedTask;
        }

        public async Task<string> RenderTemplateAsync(string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            var template = await GetTemplateAsync(templateId, cancellationToken);
            var content = template.Content;
            
            foreach (var param in parameters)
            {
                content = content.Replace($"{{{{{param.Key}}}}}", param.Value);
            }
            
            return content;
        }
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly SmsOptions _options;
        private readonly ILogger<AnalyticsService> _logger;
        private readonly List<SmsEvent> _events;

        public AnalyticsService(IOptions<SmsOptions> options, ILogger<AnalyticsService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _events = new List<SmsEvent>();
            
            // 初始化分析存储目录
            Directory.CreateDirectory(_options.AnalyticsStoragePath);
            LoadEvents();
        }

        private void LoadEvents()
        {
            try
            {
                var eventFiles = Directory.GetFiles(_options.AnalyticsStoragePath, "*.json");
                foreach (var file in eventFiles)
                {
                    var content = File.ReadAllText(file);
                    var events = JsonConvert.DeserializeObject<List<SmsEvent>>(content);
                    if (events != null)
                    {
                        _events.AddRange(events);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载事件数据失败");
            }
        }

        private void SaveEvents()
        {
            try
            {
                var fileName = $"events_{DateTime.Now:yyyyMMdd}.json";
                var filePath = Path.Combine(_options.AnalyticsStoragePath, fileName);
                var content = JsonConvert.SerializeObject(_events, Formatting.Indented);
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存事件数据失败");
            }
        }

        public Task RecordSmsEventAsync(SmsEvent @event, CancellationToken cancellationToken = default)
        {
            @event.Id = Guid.NewGuid().ToString();
            @event.EventTime = DateTime.Now;
            _events.Add(@event);
            
            // 每 100 个事件保存一次
            if (_events.Count % 100 == 0)
            {
                SaveEvents();
            }
            
            return Task.CompletedTask;
        }

        public Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var filteredEvents = _events.Where(e => e.EventTime >= startDate && e.EventTime <= endDate).ToList();
            
            var analytics = new SmsAnalytics
            {
                TotalSms = filteredEvents.Count,
                SuccessfulSms = filteredEvents.Count(e => e.Success),
                StatusDistribution = filteredEvents.GroupBy(e => e.Success ? "成功" : "失败").ToDictionary(g => g.Key, g => g.Count()),
                DailyCounts = filteredEvents.GroupBy(e => e.EventTime.Date).ToDictionary(g => g.Key, g => g.Count())
            };

            if (analytics.TotalSms > 0)
            {
                analytics.SuccessRate = (double)analytics.SuccessfulSms / analytics.TotalSms;
                analytics.AverageMessageLength = filteredEvents.Average(e => e.Message?.Length ?? 0);
            }
            
            return Task.FromResult(analytics);
        }

        public Task<IEnumerable<SmsEvent>> GetEventsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var filteredEvents = _events.Where(e => e.EventTime >= startDate && e.EventTime <= endDate);
            return Task.FromResult<IEnumerable<SmsEvent>>(filteredEvents);
        }
    }

    public class SmsService : ISmsService
    {
        private readonly ISmsProvider _smsProvider;
        private readonly ITemplateService _templateService;
        private readonly IAnalyticsService _analyticsService;
        private readonly SmsOptions _options;
        private readonly ILogger<SmsService> _logger;

        public SmsService(
            ISmsProvider smsProvider,
            ITemplateService templateService,
            IAnalyticsService analyticsService,
            IOptions<SmsOptions> options,
            ILogger<SmsService> logger)
        {
            _smsProvider = smsProvider;
            _templateService = templateService;
            _analyticsService = analyticsService;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
        {
            var result = await _smsProvider.SendAsync(to, message, cancellationToken);
            
            // 记录事件
            var @event = new SmsEvent
            {
                MessageSid = result.MessageSid,
                To = to,
                From = result.From,
                Message = message,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage,
                EventType = "send"
            };
            await _analyticsService.RecordSmsEventAsync(@event, cancellationToken);
            
            return result;
        }

        public async Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            var renderedMessage = await _templateService.RenderTemplateAsync(templateId, parameters, cancellationToken);
            return await SendAsync(to, renderedMessage, cancellationToken);
        }

        public async Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default)
        {
            var results = new List<SmsResult>();
            var recipientList = recipients.ToList();
            
            _logger.LogInformation("开始批量发送SMS，共 {Count} 条", recipientList.Count);
            
            // 分批处理
            for (int i = 0; i < recipientList.Count; i += _options.BatchSize)
            {
                var batch = recipientList.Skip(i).Take(_options.BatchSize).ToList();
                
                foreach (var recipient in batch)
                {
                    // 速率限制
                    await Task.Delay(1000 / _options.RateLimitPerSecond, cancellationToken);
                    
                    var result = await SendAsync(recipient, message, cancellationToken);
                    results.Add(result);
                }
            }
            
            _logger.LogInformation("批量发送完成，成功 {SuccessCount}/{TotalCount}", results.Count(r => r.Success), results.Count);
            return results;
        }

        public Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default)
        {
            // 这里实现简单的内存定时任务
            // 实际生产环境中应使用持久化的定时任务系统
            var scheduleId = Guid.NewGuid().ToString();
            
            _logger.LogInformation("调度SMS发送到 {To}，时间: {SendTime}", to, sendTime);
            
            // 启动后台任务
            _ = Task.Run(async () =>
            {
                var delay = sendTime - DateTime.Now;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
                
                await SendAsync(to, message, cancellationToken);
            }, cancellationToken);
            
            return Task.FromResult(new SmsScheduleResult
            {
                Success = true,
                ScheduleId = scheduleId,
                To = to,
                Message = message,
                SendTime = sendTime
            });
        }

        public Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return _analyticsService.GetAnalyticsAsync(startDate, endDate, cancellationToken);
        }

        public Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            return _smsProvider.ReceiveAsync(cancellationToken);
        }
    }

    // 依赖注入扩展
    public static class SmsServiceCollectionExtensions
    {
        public static IServiceCollection AddSmsServices(this IServiceCollection services)
        {
            // 配置选项
            services.Configure<TwilioOptions>(options =>
            {
                // 这里设置默认值，实际使用时应从配置文件或环境变量读取
                options.AccountSid = "your-twilio-account-sid";
                options.AuthToken = "your-twilio-auth-token";
                options.FromNumber = "your-twilio-phone-number";
            });
            
            services.Configure<SmsOptions>(options =>
            {
                options.BatchSize = 100;
                options.RateLimitPerSecond = 10;
                options.TemplateDirectory = "./templates";
                options.AnalyticsStoragePath = "./analytics";
            });
            
            // 注册服务
            services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
            services.AddSingleton<ITemplateService, TemplateService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ISmsService, SmsService>();
            
            // 注册日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            return services;
        }
    }

    // 命令行接口
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // 创建根命令
            var rootCommand = new RootCommand("SMS 技能命令行工具");
            
            // 发送命令
            var sendCommand = new Command("send", "发送SMS消息");
            var toOption = new Option<string>("--to", "目标手机号码") { IsRequired = true };
            var messageOption = new Option<string>("--message", "消息内容") { IsRequired = true };
            sendCommand.AddOption(toOption);
            sendCommand.AddOption(messageOption);
            sendCommand.SetHandler(async (context) =>
            {
                var to = context.ParseResult.GetValueForOption(toOption);
                var message = context.ParseResult.GetValueForOption(messageOption);
                await HandleSendCommand(to, message);
            });
            
            // 模板命令
            var templateCommand = new Command("template", "管理SMS模板");
            
            // 创建模板
            var createTemplateCommand = new Command("create", "创建SMS模板");
            var nameOption = new Option<string>("--name", "模板名称") { IsRequired = true };
            var contentOption = new Option<string>("--content", "模板内容") { IsRequired = true };
            createTemplateCommand.AddOption(nameOption);
            createTemplateCommand.AddOption(contentOption);
            createTemplateCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(nameOption);
                var content = context.ParseResult.GetValueForOption(contentOption);
                await HandleCreateTemplateCommand(name, content);
            });
            
            // 列出模板
            var listTemplateCommand = new Command("list", "列出所有SMS模板");
            listTemplateCommand.SetHandler(async () =>
            {
                await HandleListTemplatesCommand();
            });
            
            templateCommand.AddCommand(createTemplateCommand);
            templateCommand.AddCommand(listTemplateCommand);
            
            // 批量发送命令
            var batchCommand = new Command("batch", "批量发送SMS消息");
            var fileOption = new Option<string>("--file", "包含手机号码列表的文件路径") { IsRequired = true };
            var batchMessageOption = new Option<string>("--message", "消息内容") { IsRequired = true };
            batchCommand.AddOption(fileOption);
            batchCommand.AddOption(batchMessageOption);
            batchCommand.SetHandler(async (context) =>
            {
                var file = context.ParseResult.GetValueForOption(fileOption);
                var message = context.ParseResult.GetValueForOption(batchMessageOption);
                await HandleBatchCommand(file, message);
            });
            
            // 定时发送命令
            var scheduleCommand = new Command("schedule", "定时发送SMS消息");
            var scheduleToOption = new Option<string>("--to", "目标手机号码") { IsRequired = true };
            var scheduleMessageOption = new Option<string>("--message", "消息内容") { IsRequired = true };
            var timeOption = new Option<string>("--time", "发送时间 (yyyy-MM-dd HH:mm:ss)") { IsRequired = true };
            scheduleCommand.AddOption(scheduleToOption);
            scheduleCommand.AddOption(scheduleMessageOption);
            scheduleCommand.AddOption(timeOption);
            scheduleCommand.SetHandler(async (context) =>
            {
                var to = context.ParseResult.GetValueForOption(scheduleToOption);
                var message = context.ParseResult.GetValueForOption(scheduleMessageOption);
                var timeStr = context.ParseResult.GetValueForOption(timeOption);
                if (DateTime.TryParse(timeStr, out var time))
                {
                    await HandleScheduleCommand(to, message, time);
                }
                else
                {
                    Console.WriteLine("无效的时间格式，请使用 yyyy-MM-dd HH:mm:ss 格式");
                }
            });
            
            // 分析命令
            var analyticsCommand = new Command("analytics", "查看SMS发送分析");
            var startOption = new Option<string>("--start", "开始日期 (yyyy-MM-dd)") { IsRequired = true };
            var endOption = new Option<string>("--end", "结束日期 (yyyy-MM-dd)") { IsRequired = true };
            analyticsCommand.AddOption(startOption);
            analyticsCommand.AddOption(endOption);
            analyticsCommand.SetHandler(async (context) =>
            {
                var startStr = context.ParseResult.GetValueForOption(startOption);
                var endStr = context.ParseResult.GetValueForOption(endOption);
                if (DateTime.TryParse(startStr, out var start) && DateTime.TryParse(endStr, out var end))
                {
                    await HandleAnalyticsCommand(start, end.AddDays(1).AddSeconds(-1));
                }
                else
                {
                    Console.WriteLine("无效的日期格式，请使用 yyyy-MM-dd 格式");
                }
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(sendCommand);
            rootCommand.AddCommand(templateCommand);
            rootCommand.AddCommand(batchCommand);
            rootCommand.AddCommand(scheduleCommand);
            rootCommand.AddCommand(analyticsCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static async Task HandleSendCommand(string to, string message)
        {
            var services = new ServiceCollection();
            services.AddSmsServices();
            
            using var serviceProvider = services.BuildServiceProvider();
            var smsService = serviceProvider.GetRequiredService<ISmsService>();
            
            var result = await smsService.SendAsync(to, message);
            Console.WriteLine($"发送结果: {(result.Success ? "成功" : "失败")}");
            if (result.Success)
            {
                Console.WriteLine($"消息ID: {result.MessageSid}");
                Console.WriteLine($"发送时间: {result.SentAt}");
            }
            else
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }
        }
        
        private static async Task HandleCreateTemplateCommand(string name, string content)
        {
            var services = new ServiceCollection();
            services.AddSmsServices();
            
            using var serviceProvider = services.BuildServiceProvider();
            var templateService = serviceProvider.GetRequiredService<ITemplateService>();
            
            var templateId = await templateService.CreateTemplateAsync(name, content);
            Console.WriteLine($"模板创建成功，ID: {templateId}");
        }
        
        private static async Task HandleListTemplatesCommand()
        {
            var services = new ServiceCollection();
            services.AddSmsServices();
            
            using var serviceProvider = services.BuildServiceProvider();
            var templateService = serviceProvider.GetRequiredService<ITemplateService>();
            
            var templates = await templateService.GetTemplatesAsync();
            Console.WriteLine("模板列表:");
            foreach (var template in templates)
            {
                Console.WriteLine($"ID: {template.Id}");
                Console.WriteLine($"名称: {template.Name}");
                Console.WriteLine($"内容: {template.Content}");
                Console.WriteLine($"创建时间: {template.CreatedAt}");
                if (template.UpdatedAt.HasValue)
                {
                    Console.WriteLine($"更新时间: {template.UpdatedAt.Value}");
                }
                Console.WriteLine();
            }
        }
        
        private static async Task HandleBatchCommand(string file, string message)
        {
            try
            {
                var recipients = File.ReadAllLines(file).Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
                Console.WriteLine($"读取到 {recipients.Count} 个手机号码");
                
                var services = new ServiceCollection();
                services.AddSmsServices();
                
                using var serviceProvider = services.BuildServiceProvider();
                var smsService = serviceProvider.GetRequiredService<ISmsService>();
                
                var results = await smsService.BatchSendAsync(recipients, message);
                
                var successCount = results.Count(r => r.Success);
                var totalCount = results.Count();
                Console.WriteLine($"批量发送完成: 成功 {successCount}/{totalCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"批量发送失败: {ex.Message}");
            }
        }
        
        private static async Task HandleScheduleCommand(string to, string message, DateTime sendTime)
        {
            var services = new ServiceCollection();
            services.AddSmsServices();
            
            using var serviceProvider = services.BuildServiceProvider();
            var smsService = serviceProvider.GetRequiredService<ISmsService>();
            
            var result = await smsService.ScheduleSendAsync(to, message, sendTime);
            Console.WriteLine($"定时发送设置: {(result.Success ? "成功" : "失败")}");
            if (result.Success)
            {
                Console.WriteLine($"调度ID: {result.ScheduleId}");
                Console.WriteLine($"发送时间: {result.SendTime}");
                Console.WriteLine($"目标号码: {result.To}");
            }
            else
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }
        }
        
        private static async Task HandleAnalyticsCommand(DateTime startDate, DateTime endDate)
        {
            var services = new ServiceCollection();
            services.AddSmsServices();
            
            using var serviceProvider = services.BuildServiceProvider();
            var analyticsService = serviceProvider.GetRequiredService<IAnalyticsService>();
            
            var analytics = await analyticsService.GetAnalyticsAsync(startDate, endDate);
            Console.WriteLine("SMS发送分析:");
            Console.WriteLine($"总发送量: {analytics.TotalSms}");
            Console.WriteLine($"成功量: {analytics.SuccessfulSms}");
            Console.WriteLine($"成功率: {analytics.SuccessRate:P2}");
            Console.WriteLine($"平均消息长度: {analytics.AverageMessageLength:F2}");
            
            Console.WriteLine("\n状态分布:");
            foreach (var (status, count) in analytics.StatusDistribution)
            {
                Console.WriteLine($"{status}: {count}");
            }
            
            Console.WriteLine("\n每日发送量:");
            foreach (var (date, count) in analytics.DailyCounts.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"{date:yyyy-MM-dd}: {count}");
            }
        }
    }
}

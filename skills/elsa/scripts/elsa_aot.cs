#:sdk Microsoft.NET.Sdk
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elsa.AOT
{
    /// <summary>
    /// Elsa 命令类型枚举
    /// </summary>
    public enum ElsaCommandType
    {
        /// <summary>
        /// 启动工作流
        /// </summary>
        StartWorkflow,
        /// <summary>
        /// 暂停工作流
        /// </summary>
        SuspendWorkflow,
        /// <summary>
        /// 恢复工作流
        /// </summary>
        ResumeWorkflow,
        /// <summary>
        /// 终止工作流
        /// </summary>
        TerminateWorkflow,
        /// <summary>
        /// 查询工作流状态
        /// </summary>
        QueryWorkflowStatus,
        /// <summary>
        /// 列出工作流实例
        /// </summary>
        ListWorkflowInstances,
        /// <summary>
        /// 显示版本信息
        /// </summary>
        VersionInfo
    }

    /// <summary>
    /// Elsa 选项配置
    /// </summary>
    public class ElsaOptions
    {
        /// <summary>
        /// 工作流默认超时时间（毫秒）
        /// </summary>
        public int DefaultTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用工作流缓存
        /// </summary>
        public bool EnableWorkflowCache { get; set; } = true;
        
        /// <summary>
        /// 最大缓存工作流实例数量
        /// </summary>
        public int MaxCacheSize { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 工作流存储类型
        /// </summary>
        public string StorageType { get; set; } = "Memory";
        
        /// <summary>
        /// 工作流存储连接字符串
        /// </summary>
        public string StorageConnectionString { get; set; } = "MemoryStore";        
    }

    /// <summary>
    /// 工作流实例
    /// </summary>
    public class WorkflowInstance
    {
        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// 工作流定义ID
        /// </summary>
        public string WorkflowDefinitionId { get; set; } = "default-definition";
        
        /// <summary>
        /// 工作流名称
        /// </summary>
        public string WorkflowName { get; set; } = "Default Workflow";
        
        /// <summary>
        /// 工作流状态
        /// </summary>
        public string Status { get; set; } = "Running";
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Elsa 命令结果
    /// </summary>
    public class ElsaCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public ElsaCommandType CommandType { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 工作流实例ID
        /// </summary>
        public string? WorkflowInstanceId { get; set; }
        
        /// <summary>
        /// 工作流实例列表
        /// </summary>
        public List<WorkflowInstance>? WorkflowInstances { get; set; }
    }

    /// <summary>
    /// Elsa 服务接口
    /// </summary>
    public interface IElsaService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<ElsaCommandResult> ExecuteCommandAsync(ElsaCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 启动工作流
        /// </summary>
        /// <param name="workflowDefinitionId">工作流定义ID</param>
        /// <param name="inputData">输入数据</param>
        /// <param name="timeoutMs">超时时间（毫秒）</param>
        /// <returns>工作流执行结果</returns>
        Task<ElsaCommandResult> StartWorkflowAsync(string? workflowDefinitionId = null, Dictionary<string, object>? inputData = null, int? timeoutMs = null);
        
        /// <summary>
        /// 暂停工作流
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <returns>暂停结果</returns>
        Task<ElsaCommandResult> SuspendWorkflowAsync(string workflowInstanceId);
        
        /// <summary>
        /// 恢复工作流
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <returns>恢复结果</returns>
        Task<ElsaCommandResult> ResumeWorkflowAsync(string workflowInstanceId);
        
        /// <summary>
        /// 终止工作流
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <returns>终止结果</returns>
        Task<ElsaCommandResult> TerminateWorkflowAsync(string workflowInstanceId);
        
        /// <summary>
        /// 查询工作流状态
        /// </summary>
        /// <param name="workflowInstanceId">工作流实例ID</param>
        /// <returns>工作流状态</returns>
        Task<ElsaCommandResult> QueryWorkflowStatusAsync(string workflowInstanceId);
        
        /// <summary>
        /// 列出工作流实例
        /// </summary>
        /// <param name="statusFilter">状态过滤器</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>工作流实例列表</returns>
        Task<ElsaCommandResult> ListWorkflowInstancesAsync(string? statusFilter = null, int pageSize = 10, int pageIndex = 0);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<ElsaCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// Elsa 服务实现
    /// </summary>
    public class ElsaService : IElsaService
    {
        private readonly ElsaOptions _options;
        private readonly ILogger<ElsaService> _logger;
        private readonly Dictionary<string, WorkflowInstance> _workflowInstances = new Dictionary<string, WorkflowInstance>();
        private readonly object _instancesLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Elsa 选项</param>
        /// <param name="logger">日志记录器</param>
        public ElsaService(IOptions<ElsaOptions> options, ILogger<ElsaService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ElsaCommandResult> ExecuteCommandAsync(ElsaCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case ElsaCommandType.StartWorkflow:
                        string? definitionId = parameters?.ContainsKey("definitionId") == true ? parameters["definitionId"] : null;
                        result = await StartWorkflowAsync(definitionId);
                        break;
                    
                    case ElsaCommandType.SuspendWorkflow:
                        if (parameters?.ContainsKey("instanceId") == true)
                        {
                            string instanceId = parameters["instanceId"];
                            result = await SuspendWorkflowAsync(instanceId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InstanceId parameter is required";
                        }
                        break;
                    
                    case ElsaCommandType.ResumeWorkflow:
                        if (parameters?.ContainsKey("instanceId") == true)
                        {
                            string instanceId = parameters["instanceId"];
                            result = await ResumeWorkflowAsync(instanceId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InstanceId parameter is required";
                        }
                        break;
                    
                    case ElsaCommandType.TerminateWorkflow:
                        if (parameters?.ContainsKey("instanceId") == true)
                        {
                            string instanceId = parameters["instanceId"];
                            result = await TerminateWorkflowAsync(instanceId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InstanceId parameter is required";
                        }
                        break;
                    
                    case ElsaCommandType.QueryWorkflowStatus:
                        if (parameters?.ContainsKey("instanceId") == true)
                        {
                            string instanceId = parameters["instanceId"];
                            result = await QueryWorkflowStatusAsync(instanceId);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InstanceId parameter is required";
                        }
                        break;
                    
                    case ElsaCommandType.ListWorkflowInstances:
                        string? statusFilter = parameters?.ContainsKey("status") == true ? parameters["status"] : null;
                        int pageSize = parameters?.ContainsKey("pageSize") == true ? int.Parse(parameters["pageSize"]) : 10;
                        int pageIndex = parameters?.ContainsKey("pageIndex") == true ? int.Parse(parameters["pageIndex"]) : 0;
                        result = await ListWorkflowInstancesAsync(statusFilter, pageSize, pageIndex);
                        break;
                    
                    case ElsaCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing command: {CommandType}", commandType);
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
        public async Task<ElsaCommandResult> StartWorkflowAsync(string? workflowDefinitionId, Dictionary<string, object>? inputData = null, int? timeoutMs = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.StartWorkflow
            };

            try
            {
                _logger.LogInformation("Starting workflow...");
                
                // 创建工作流实例
                var instance = new WorkflowInstance
                {
                    WorkflowDefinitionId = workflowDefinitionId ?? "default-definition",
                    WorkflowName = workflowDefinitionId ?? "Default Workflow",
                    Status = "Running"
                };
                
                // 添加到实例字典
                lock (_instancesLock)
                {
                    _workflowInstances[instance.Id] = instance;
                }
                
                result.Success = true;
                result.WorkflowInstanceId = instance.Id;
                result.Results.Add($"Workflow {instance.Id} started successfully");
                result.Results.Add($"Definition: {instance.WorkflowDefinitionId}");
                result.Results.Add($"Name: {instance.WorkflowName}");
                result.Results.Add($"Status: {instance.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting workflow");
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
        public async Task<ElsaCommandResult> SuspendWorkflowAsync(string workflowInstanceId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.SuspendWorkflow,
                WorkflowInstanceId = workflowInstanceId
            };

            try
            {
                _logger.LogInformation("Suspending workflow: {InstanceId}", workflowInstanceId);
                
                lock (_instancesLock)
                {
                    if (_workflowInstances.TryGetValue(workflowInstanceId, out var instance))
                    {
                        instance.Status = "Suspended";
                        instance.UpdatedAt = DateTime.UtcNow;
                        
                        result.Success = true;
                        result.Results.Add($"Workflow {workflowInstanceId} suspended successfully");
                        result.Results.Add($"New Status: {instance.Status}");
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Workflow instance not found: {workflowInstanceId}";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending workflow: {InstanceId}", workflowInstanceId);
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
        public async Task<ElsaCommandResult> ResumeWorkflowAsync(string workflowInstanceId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.ResumeWorkflow,
                WorkflowInstanceId = workflowInstanceId
            };

            try
            {
                _logger.LogInformation("Resuming workflow: {InstanceId}", workflowInstanceId);
                
                lock (_instancesLock)
                {
                    if (_workflowInstances.TryGetValue(workflowInstanceId, out var instance))
                    {
                        instance.Status = "Running";
                        instance.UpdatedAt = DateTime.UtcNow;
                        
                        result.Success = true;
                        result.Results.Add($"Workflow {workflowInstanceId} resumed successfully");
                        result.Results.Add($"New Status: {instance.Status}");
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Workflow instance not found: {workflowInstanceId}";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming workflow: {InstanceId}", workflowInstanceId);
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
        public async Task<ElsaCommandResult> TerminateWorkflowAsync(string workflowInstanceId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.TerminateWorkflow,
                WorkflowInstanceId = workflowInstanceId
            };

            try
            {
                _logger.LogInformation("Terminating workflow: {InstanceId}", workflowInstanceId);
                
                lock (_instancesLock)
                {
                    if (_workflowInstances.TryGetValue(workflowInstanceId, out var instance))
                    {
                        instance.Status = "Terminated";
                        instance.UpdatedAt = DateTime.UtcNow;
                        instance.CompletedAt = DateTime.UtcNow;
                        
                        result.Success = true;
                        result.Results.Add($"Workflow {workflowInstanceId} terminated successfully");
                        result.Results.Add($"New Status: {instance.Status}");
                        result.Results.Add($"Completed At: {instance.CompletedAt}");
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Workflow instance not found: {workflowInstanceId}";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error terminating workflow: {InstanceId}", workflowInstanceId);
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
        public async Task<ElsaCommandResult> QueryWorkflowStatusAsync(string workflowInstanceId)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.QueryWorkflowStatus,
                WorkflowInstanceId = workflowInstanceId
            };

            try
            {
                _logger.LogInformation("Querying workflow status: {InstanceId}", workflowInstanceId);
                
                lock (_instancesLock)
                {
                    if (_workflowInstances.TryGetValue(workflowInstanceId, out var instance))
                    {
                        result.Success = true;
                        result.Results.Add($"Workflow {workflowInstanceId} status:");
                        result.Results.Add($"  ID: {instance.Id}");
                        result.Results.Add($"  Definition: {instance.WorkflowDefinitionId}");
                        result.Results.Add($"  Name: {instance.WorkflowName}");
                        result.Results.Add($"  Status: {instance.Status}");
                        result.Results.Add($"  Created: {instance.CreatedAt}");
                        result.Results.Add($"  Updated: {instance.UpdatedAt}");
                        if (instance.CompletedAt.HasValue)
                        {
                            result.Results.Add($"  Completed: {instance.CompletedAt}");
                        }
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Workflow instance not found: {workflowInstanceId}";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying workflow status: {InstanceId}", workflowInstanceId);
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
        public async Task<ElsaCommandResult> ListWorkflowInstancesAsync(string? statusFilter = null, int pageSize = 10, int pageIndex = 0)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.ListWorkflowInstances
            };

            try
            {
                _logger.LogInformation("Listing workflow instances...");
                
                List<WorkflowInstance> instances;
                lock (_instancesLock)
                {
                    instances = _workflowInstances.Values.ToList();
                }
                
                // 应用状态过滤器
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    instances = instances.Where(i => i.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                // 分页
                instances = instances.OrderByDescending(i => i.CreatedAt)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToList();
                
                result.Success = true;
                result.Results.Add($"Found {instances.Count} workflow instances:");
                result.WorkflowInstances = instances;
                
                // 添加实例信息到结果
                foreach (var instance in instances)
                {
                    result.Results.Add($"  {instance.Id}: {instance.WorkflowName} ({instance.Status})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing workflow instances");
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
        public async Task<ElsaCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ElsaCommandResult
            {
                CommandType = ElsaCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("Getting version info");
                
                result.Success = true;
                result.Results.Add("Elsa AOT Engine");
                result.Results.Add($"Version: 1.0.0");
                result.Results.Add($".NET Version: {Environment.Version}");
                result.Results.Add($"OS: {Environment.OSVersion}");
                result.Results.Add($"Architecture: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT Compiled: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version info");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return await Task.FromResult(result);
        }
    }

    /// <summary>
    /// Elsa AOT 引擎
    /// </summary>
    public class ElsaAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ElsaAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public ElsaAotEngine(IServiceProvider serviceProvider, ILogger<ElsaAotEngine> logger)
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
            _logger.LogInformation("Elsa AOT Engine starting with args: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var elsaService = _serviceProvider.GetRequiredService<IElsaService>();
            ElsaCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "start":
                        string? definitionId = args.Length > 1 ? args[1] : null;
                        result = await elsaService.StartWorkflowAsync(definitionId);
                        break;
                    
                    case "suspend":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Workflow instance ID is required");
                            return 1;
                        }
                        result = await elsaService.SuspendWorkflowAsync(args[1]);
                        break;
                    
                    case "resume":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Workflow instance ID is required");
                            return 1;
                        }
                        result = await elsaService.ResumeWorkflowAsync(args[1]);
                        break;
                    
                    case "terminate":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Workflow instance ID is required");
                            return 1;
                        }
                        result = await elsaService.TerminateWorkflowAsync(args[1]);
                        break;
                    
                    case "status":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Workflow instance ID is required");
                            return 1;
                        }
                        result = await elsaService.QueryWorkflowStatusAsync(args[1]);
                        break;
                    
                    case "list":
                        string? status = args.Length > 1 ? args[1] : null;
                        result = await elsaService.ListWorkflowInstancesAsync(status);
                        break;
                    
                    case "version":
                        result = await elsaService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"Error: Unknown command '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}
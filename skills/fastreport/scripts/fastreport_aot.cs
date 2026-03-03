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
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastReport.AOT
{
    /// <summary>
    /// FastReport 命令类型枚举
    /// </summary>
    public enum FastReportCommandType { CreateReport, LoadReport, RunReport, ExportReport, PreviewReport, VersionInfo }

    /// <summary>
    /// FastReport 选项配置
    /// </summary>
    public class FastReportOptions
    {
        /// <summary>
        /// 报表模板目录
        /// </summary>
        public string TemplateDirectory { get; set; } = "templates";
        
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDirectory { get; set; } = "output";
        
        /// <summary>
        /// 默认导出格式
        /// </summary>
        public string DefaultExportFormat { get; set; } = "PDF";
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
        
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 100;
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
    }

    /// <summary>
    /// 报表参数
    /// </summary>
    public class ReportParameter
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; } = null!;
        
        /// <summary>
        /// 参数类型
        /// </summary>
        public string Type { get; set; } = "String";
    }

    /// <summary>
    /// 报表数据
    /// </summary>
    public class ReportData
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 数据行
        /// </summary>
        public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();
        
        /// <summary>
        /// 列定义
        /// </summary>
        public Dictionary<string, string> Columns { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// FastReport 命令结果
    /// </summary>
    public class FastReportCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public FastReportCommandType CommandType { get; set; }
        
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
        /// 报表文件路径
        /// </summary>
        public string? ReportPath { get; set; }
        
        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string? OutputPath { get; set; }
        
        /// <summary>
        /// 报表名称
        /// </summary>
        public string? ReportName { get; set; }
    }

    /// <summary>
    /// FastReport 服务接口
    /// </summary>
    public interface IFastReportService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<FastReportCommandResult> ExecuteCommandAsync(FastReportCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 创建报表
        /// </summary>
        /// <param name="reportName">报表名称</param>
        /// <param name="parameters">报表参数</param>
        /// <param name="data">报表数据</param>
        /// <returns>操作结果</returns>
        Task<FastReportCommandResult> CreateReportAsync(string reportName, List<ReportParameter>? parameters = null, List<ReportData>? data = null);
        
        /// <summary>
        /// 加载报表
        /// </summary>
        /// <param name="reportPath">报表路径</param>
        /// <returns>操作结果</returns>
        Task<FastReportCommandResult> LoadReportAsync(string reportPath);
        
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <param name="reportPath">报表路径</param>
        /// <param name="parameters">报表参数</param>
        /// <returns>操作结果</returns>
        Task<FastReportCommandResult> RunReportAsync(string reportPath, List<ReportParameter>? parameters = null);
        
        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="reportPath">报表路径</param>
        /// <param name="outputPath">输出路径</param>
        /// <param name="format">导出格式</param>
        /// <param name="parameters">报表参数</param>
        /// <returns>操作结果</returns>
        Task<FastReportCommandResult> ExportReportAsync(string reportPath, string outputPath, string format = "PDF", List<ReportParameter>? parameters = null);
        
        /// <summary>
        /// 预览报表
        /// </summary>
        /// <param name="reportPath">报表路径</param>
        /// <param name="parameters">报表参数</param>
        /// <returns>操作结果</returns>
        Task<FastReportCommandResult> PreviewReportAsync(string reportPath, List<ReportParameter>? parameters = null);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<FastReportCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// FastReport 服务实现
    /// </summary>
    public class FastReportService : IFastReportService
    {
        private readonly FastReportOptions _options;
        private readonly ILogger<FastReportService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">FastReport 选项</param>
        /// <param name="logger">日志记录器</param>
        public FastReportService(IOptions<FastReportOptions> options, ILogger<FastReportService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FastReportCommandResult> ExecuteCommandAsync(FastReportCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FastReportCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case FastReportCommandType.CreateReport:
                        if (parameters?.ContainsKey("reportName") == true)
                        {
                            string reportName = parameters["reportName"];
                            // 模拟参数和数据，实际实现中应解析 parameters
                            var parametersList = new List<ReportParameter>();
                            var dataList = new List<ReportData>();
                            result = await CreateReportAsync(reportName, parametersList, dataList);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "ReportName parameter is required";
                        }
                        break;
                    
                    case FastReportCommandType.LoadReport:
                        if (parameters?.ContainsKey("reportPath") == true)
                        {
                            string reportPath = parameters["reportPath"];
                            result = await LoadReportAsync(reportPath);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "ReportPath parameter is required";
                        }
                        break;
                    
                    case FastReportCommandType.RunReport:
                        if (parameters?.ContainsKey("reportPath") == true)
                        {
                            string reportPath = parameters["reportPath"];
                            // 模拟参数，实际实现中应解析 parameters
                            var parametersList = new List<ReportParameter>();
                            result = await RunReportAsync(reportPath, parametersList);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "ReportPath parameter is required";
                        }
                        break;
                    
                    case FastReportCommandType.ExportReport:
                        if (parameters?.ContainsKey("reportPath") == true && parameters?.ContainsKey("outputPath") == true)
                        {
                            string reportPath = parameters["reportPath"];
                            string outputPath = parameters["outputPath"];
                            string format = parameters?.ContainsKey("format") == true ? parameters["format"] : "PDF";
                            // 模拟参数，实际实现中应解析 parameters
                            var parametersList = new List<ReportParameter>();
                            result = await ExportReportAsync(reportPath, outputPath, format, parametersList);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "ReportPath and OutputPath parameters are required";
                        }
                        break;
                    
                    case FastReportCommandType.PreviewReport:
                        if (parameters?.ContainsKey("reportPath") == true)
                        {
                            string reportPath = parameters["reportPath"];
                            // 模拟参数，实际实现中应解析 parameters
                            var parametersList = new List<ReportParameter>();
                            result = await PreviewReportAsync(reportPath, parametersList);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "ReportPath parameter is required";
                        }
                        break;
                    
                    case FastReportCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"未知命令类型: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行命令时出错: {CommandType}", commandType);
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
        public async Task<FastReportCommandResult> CreateReportAsync(string reportName, List<ReportParameter>? parameters = null, List<ReportData>? data = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FastReportCommandResult
            {
                CommandType = FastReportCommandType.CreateReport,
                ReportName = reportName
            };

            try
            {
                _logger.LogInformation("创建报表: {ReportName}", reportName);
                
                // 确保模板目录存在
                if (!Directory.Exists(_options.TemplateDirectory))
                {
                    Directory.CreateDirectory(_options.TemplateDirectory);
                }
                
                // 生成报表文件路径
                var reportPath = Path.Combine(_options.TemplateDirectory, $"{reportName}.frx");
                
                // 模拟创建报表
                // 实际实现中应使用 FastReport 库创建报表
                await Task.Delay(100); // 模拟操作
                
                // 创建空的报表文件
                File.WriteAllText(reportPath, $"<?xml version=\"1.0\" encoding=\"utf-8\"?><Report Name=\"{reportName}\
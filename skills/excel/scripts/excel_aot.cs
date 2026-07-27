#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.IO.Pipelines@8.0.0
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
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Excel.AOT
{
    /// <summary>
    /// Excel 命令类型枚举
    /// </summary>
    public enum ExcelCommandType { ReadExcel, WriteExcel, ConvertExcel, MergeExcel, SplitExcel, VersionInfo }

    /// <summary>
    /// Excel 选项配置
    /// </summary>
    public class ExcelOptions
    {
        /// <summary>
        /// Excel 读取模式
        /// </summary>
        public string ReadMode { get; set; } = "Auto";
        
        /// <summary>
        /// Excel 写入模式
        /// </summary>
        public string WriteMode { get; set; } = "Xlsx";
        
        /// <summary>
        /// 最大行数
        /// </summary>
        public int MaxRows { get; set; } = 10000;
        
        /// <summary>
        /// 最大列数
        /// </summary>
        public int MaxColumns { get; set; } = 100;
        
        /// <summary>
        /// 是否启用数据验证
        /// </summary>
        public bool EnableDataValidation { get; set; } = false;
        
        /// <summary>
        /// 是否启用格式化
        /// </summary>
        public bool EnableFormatting { get; set; } = true;
        
        /// <summary>
        /// 是否启用压缩
        /// </summary>
        public bool EnableCompression { get; set; } = true;
        
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
    /// Excel 单元格数据
    /// </summary>
    public class ExcelCell
    {
        /// <summary>
        /// 行索引
        /// </summary>
        public int RowIndex { get; set; }
        
        /// <summary>
        /// 列索引
        /// </summary>
        public int ColumnIndex { get; set; }
        
        /// <summary>
        /// 单元格值
        /// </summary>
        public string Value { get; set; } = string.Empty;
        
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; } = "String";
        
        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; set; } = string.Empty;
    }

    /// <summary>
    /// Excel 工作表数据
    /// </summary>
    public class ExcelWorksheet
    {
        /// <summary>
        /// 工作表名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 行数据
        /// </summary>
        public List<List<ExcelCell>> Rows { get; set; } = new List<List<ExcelCell>>();
        
        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount => Rows.Count;
        
        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount => Rows.Count > 0 ? Rows[0].Count : 0;
    }

    /// <summary>
    /// Excel 文档数据
    /// </summary>
    public class ExcelDocument
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
        
        /// <summary>
        /// 工作表列表
        /// </summary>
        public List<ExcelWorksheet> Worksheets { get; set; } = new List<ExcelWorksheet>();
        
        /// <summary>
        /// 工作表数量
        /// </summary>
        public int WorksheetCount => Worksheets.Count;
    }

    /// <summary>
    /// Excel 命令结果
    /// </summary>
    public class ExcelCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public ExcelCommandType CommandType { get; set; }
        
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
        /// Excel 文档数据
        /// </summary>
        public ExcelDocument? ExcelDocument { get; set; }
        
        /// <summary>
        /// 处理的文件路径
        /// </summary>
        public string? FilePath { get; set; }
        
        /// <summary>
        /// 生成的文件路径
        /// </summary>
        public string? OutputFilePath { get; set; }
    }

    /// <summary>
    /// Excel 服务接口
    /// </summary>
    public interface IExcelService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<ExcelCommandResult> ExecuteCommandAsync(ExcelCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 读取 Excel 文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>操作结果</returns>
        Task<ExcelCommandResult> ReadExcelAsync(string filePath, string? sheetName = null);
        
        /// <summary>
        /// 写入 Excel 文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="data">数据</param>
        /// <returns>操作结果</returns>
        Task<ExcelCommandResult> WriteExcelAsync(string filePath, List<Dictionary<string, object>> data);
        
        /// <summary>
        /// 转换 Excel 文件格式
        /// </summary>
        /// <param name="inputFilePath">输入文件路径</param>
        /// <param name="outputFilePath">输出文件路径</param>
        /// <returns>操作结果</returns>
        Task<ExcelCommandResult> ConvertExcelAsync(string inputFilePath, string outputFilePath);
        
        /// <summary>
        /// 合并 Excel 文件
        /// </summary>
        /// <param name="inputFilePaths">输入文件路径列表</param>
        /// <param name="outputFilePath">输出文件路径</param>
        /// <returns>操作结果</returns>
        Task<ExcelCommandResult> MergeExcelAsync(List<string> inputFilePaths, string outputFilePath);
        
        /// <summary>
        /// 拆分 Excel 文件
        /// </summary>
        /// <param name="inputFilePath">输入文件路径</param>
        /// <param name="outputDirectory">输出目录</param>
        /// <returns>操作结果</returns>
        Task<ExcelCommandResult> SplitExcelAsync(string inputFilePath, string outputDirectory);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<ExcelCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// Excel 服务实现
    /// </summary>
    public class ExcelService : IExcelService
    {
        private readonly ExcelOptions _options;
        private readonly ILogger<ExcelService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Excel 选项</param>
        /// <param name="logger">日志记录器</param>
        public ExcelService(IOptions<ExcelOptions> options, ILogger<ExcelService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ExcelCommandResult> ExecuteCommandAsync(ExcelCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ExcelCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case ExcelCommandType.ReadExcel:
                        if (parameters?.ContainsKey("filePath") == true)
                        {
                            string filePath = parameters["filePath"];
                            string? sheetName = parameters?.ContainsKey("sheetName") == true ? parameters["sheetName"] : null;
                            result = await ReadExcelAsync(filePath, sheetName);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "FilePath parameter is required";
                        }
                        break;
                    
                    case ExcelCommandType.WriteExcel:
                        if (parameters?.ContainsKey("filePath") == true && parameters?.ContainsKey("data") == true)
                        {
                            string filePath = parameters["filePath"];
                            // 模拟数据，实际实现中应解析 parameters["data"]
                            var data = new List<Dictionary<string, object>>();
                            result = await WriteExcelAsync(filePath, data);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "FilePath and Data parameters are required";
                        }
                        break;
                    
                    case ExcelCommandType.ConvertExcel:
                        if (parameters?.ContainsKey("inputFilePath") == true && parameters?.ContainsKey("outputFilePath") == true)
                        {
                            string inputFilePath = parameters["inputFilePath"];
                            string outputFilePath = parameters["outputFilePath"];
                            result = await ConvertExcelAsync(inputFilePath, outputFilePath);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InputFilePath and OutputFilePath parameters are required";
                        }
                        break;
                    
                    case ExcelCommandType.MergeExcel:
                        if (parameters?.ContainsKey("inputFilePaths") == true && parameters?.ContainsKey("outputFilePath") == true)
                        {
                            List<string> inputFilePaths = parameters["inputFilePaths"].Split(';').ToList();
                            string outputFilePath = parameters["outputFilePath"];
                            result = await MergeExcelAsync(inputFilePaths, outputFilePath);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InputFilePaths and OutputFilePath parameters are required";
                        }
                        break;
                    
                    case ExcelCommandType.SplitExcel:
                        if (parameters?.ContainsKey("inputFilePath") == true && parameters?.ContainsKey("outputDirectory") == true)
                        {
                            string inputFilePath = parameters["inputFilePath"];
                            string outputDirectory = parameters["outputDirectory"];
                            result = await SplitExcelAsync(inputFilePath, outputDirectory);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "InputFilePath and OutputDirectory parameters are required";
                        }
                        break;
                    
                    case ExcelCommandType.VersionInfo:
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
        public async Task<ExcelCommandResult> ReadExcelAsync(string filePath, string? sheetName = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ExcelCommandResult
            {
                CommandType = ExcelCommandType.ReadExcel,
                FilePath = filePath
            };

            try
            {
                _logger.LogInformation("读取 Excel 文件: {FilePath}, 工作表: {SheetName}", filePath, sheetName ?? "所有");
                
                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"文件不存在: {filePath}
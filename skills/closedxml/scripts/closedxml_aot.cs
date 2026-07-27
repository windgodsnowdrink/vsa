#:sdk Microsoft.NET.Sdk.Web
#:package ClosedXML@0.104.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClosedXML.AOT
{
    /// <summary>
    /// ClosedXML AOT执行引擎
    /// 基于.NET 10 AOT架构，提供高性能Excel处理能力
    /// </summary>
    public class ClosedXmlAotEngine
    {
        private readonly ILogger<ClosedXmlAotEngine> _logger;
        private readonly ExcelService _excelService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="excelService">Excel服务</param>
        public ClosedXmlAotEngine(ILogger<ClosedXmlAotEngine> logger, ExcelService excelService)
        {
            _logger = logger;
            _excelService = excelService;
        }

        /// <summary>
        /// 执行Excel处理任务
        /// </summary>
        /// <param name="inputFile">输入文件路径</param>
        /// <param name="outputFile">输出文件路径</param>
        /// <returns>任务结果</returns>
        public async Task<bool> ExecuteAsync(string inputFile, string outputFile)
        {
            try
            {
                _logger.LogInformation("开始执行ClosedXML AOT处理任务");
                _logger.LogInformation($"输入文件: {inputFile}");
                _logger.LogInformation($"输出文件: {outputFile}");

                // 执行Excel处理
                var result = await _excelService.ProcessExcelAsync(inputFile, outputFile);

                _logger.LogInformation("ClosedXML AOT处理任务执行完成");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClosedXML AOT处理任务执行失败");
                return false;
            }
        }
    }

    /// <summary>
    /// Excel服务接口
    /// </summary>
    public interface IExcelService
    {
        /// <summary>
        /// 处理Excel文件
        /// </summary>
        /// <param name="inputFile">输入文件路径</param>
        /// <param name="outputFile">输出文件路径</param>
        /// <returns>处理结果</returns>
        Task<bool> ProcessExcelAsync(string inputFile, string outputFile);
    }

    /// <summary>
    /// Excel服务实现
    /// </summary>
    public class ExcelService : IExcelService
    {
        private readonly ILogger<ExcelService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public ExcelService(ILogger<ExcelService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 处理Excel文件
        /// </summary>
        /// <param name="inputFile">输入文件路径</param>
        /// <param name="outputFile">输出文件路径</param>
        /// <returns>处理结果</returns>
        public async Task<bool> ProcessExcelAsync(string inputFile, string outputFile)
        {
            // 验证文件存在
            if (!File.Exists(inputFile))
            {
                _logger.LogError($"输入文件不存在: {inputFile}");
                return false;
            }

            try
            {
                // 使用ClosedXML处理Excel
                using var workbook = new XLWorkbook(inputFile);
                var worksheet = workbook.Worksheet(1);

                // 示例：读取数据并进行处理
                var rowCount = worksheet.RowsUsed().Count();
                var colCount = worksheet.ColumnsUsed().Count();

                _logger.LogInformation($"Excel文件包含 {rowCount} 行，{colCount} 列");

                // 示例：在第一行前插入标题行
                worksheet.Row(1).InsertRowsAbove(1);
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

                // 为每列设置标题
                for (int i = 1; i <= colCount; i++)
                {
                    headerRow.Cell(i).Value = $"列 {i}";
                }

                // 保存处理后的文件
                workbook.SaveAs(outputFile);
                _logger.LogInformation($"处理后的文件已保存到: {outputFile}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理Excel文件时发生错误");
                return false;
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

            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // 注册服务
            builder.Services.AddSingleton<IExcelService, ExcelService>();
            builder.Services.AddSingleton<ClosedXmlAotEngine>();

            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;

            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<ClosedXmlAotEngine>();

            // 解析命令行参数
            if (args.Length < 2)
            {
                Console.WriteLine("用法: closedxml_aot.exe <输入文件路径> <输出文件路径>");
                return 1;
            }

            var inputFile = args[0];
            var outputFile = args[1];

            // 执行处理
            var result = await engine.ExecuteAsync(inputFile, outputFile);

            return result ? 0 : 1;
        }
    }
}
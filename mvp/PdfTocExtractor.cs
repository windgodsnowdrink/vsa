#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@9.0.9
#:package Microsoft.Extensions.ObjectPool@9.0.9
#:package Scalar.AspNetCore@2.8.0
#:package PdfTocExtractor@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scalar.AspNetCore;
using PdfTocExtractor;
using PdfTocExtractor.Exporters;
using PdfTocExtractor.Models;

var builder = WebApplication.CreateBuilder(args);

// 创建提取器实例
using var extractor = new PdfTocExtractor();

// 🌟 智能提取（推荐）- 自动选择最佳方法
var tocItems = await extractor.ExtractTocSmartAsync("document.pdf");

// 📖 传统方法：提取PDF书签
var bookmarkItems = await extractor.ExtractTocAsync("document.pdf");

// 🧠 结构分析：适用于无书签的PDF
var structureItems = await extractor.AnalyzeStructureAsync("document.pdf");

// 🧠 结构分析：使用自定义配置
var analysisOptions = new StructureAnalysisOptions
{
    MinFontSizeForHeading = 14f,
    UseBoldAsIndicator = true,
    MaxHeadingLevels = 4,
    RequireStandaloneHeadings = true,
    DebugMode = false
};

var customStructureItems = await extractor.AnalyzeStructureAsync("document.pdf", analysisOptions);

// 🧠 使用预设配置
var strictItems = await extractor.AnalyzeStructureAsync("document.pdf", StructureAnalysisOptions.Strict);
var relaxedItems = await extractor.AnalyzeStructureAsync("document.pdf", StructureAnalysisOptions.Relaxed);

// 导出为Markdown
await extractor.ExportToFileAsync(tocItems, "output.md", "markdown");

// 导出为JSON（带自定义选项）
var exportOptions = new ExportOptions
{
    MaxDepth = 3,
    IncludePageNumbers = true,
    CustomTitle = "文档目录"
};

await extractor.ExportToFileAsync(tocItems, "output.json", "json", exportOptions);

// 智能提取并直接导出
await extractor.ExtractSmartAndExportAsync("document.pdf", "output.xml",
    exportOptions: exportOptions,
    structureOptions: StructureAnalysisOptions.Default);

var app = builder.Build();

app.Run();

//命令行#
//安装 CLI 工具

//dotnet tool install --global PdfTocExtractor.Cli
//命令行使用

//# 🌟 智能提取（推荐）- 自动选择最佳方法
//pdftoc smart document.pdf -o output.md

//# 📖 提取PDF书签（传统方法）
//pdftoc extract document.pdf -o output.md

//# 🧠 语义分析（v2.0新功能 - 适用于无书签的PDF）
//pdftoc semantic document.pdf -o output.md

//# 指定输出格式
//pdftoc smart document.pdf -o output.json -f json

//# 设置最大层级深度
//pdftoc smart document.pdf -o output.xml --max-depth 3

//# 自定义标题和页码格式
//pdftoc smart document.pdf -o output.txt --title "我的文档目录" --page-format "第 {0} 页"

//# 语义分析 - 严格模式（更精确的标题识别）
//pdftoc semantic document.pdf -o output.md --mode strict --confidence 0.7

//# 语义分析 - 调试模式（查看分析过程）
//pdftoc semantic document.pdf -o output.md --debug --verbose

//# 结构分析 - 宽松模式（识别更多潜在标题）
//pdftoc analyze document.pdf -o output.md --relaxed

//# 结构分析 - 自定义参数
//pdftoc analyze document.pdf -o output.md --min-font-size 14 --use-bold --debug

//# 智能提取 - 带结构分析配置
//pdftoc smart document.pdf -o output.md --analysis-preset strict --debug-analysis

//# 显示详细输出
//pdftoc smart document.pdf -o output.md --verbose

//# 诊断PDF文件问题
//pdftoc diagnose document.pdf
//作为库使用#
//安装核心库

//dotnet add package PdfTocExtractor

// 可以通过实现 IExporter 接口来创建自定义导出器
public class CustomExporter : IExporter
{
    public string FormatName => "Custom";
    public string FileExtension => "custom";

    public string Export(IEnumerable<TocItem> tocItems, ExportOptions? options = null)
    {
        // 实现自定义导出逻辑
        return "custom format content";
    }

    public async Task ExportToFileAsync(IEnumerable<TocItem> tocItems, string filePath, ExportOptions? options = null)
    {
        var content = Export(tocItems, options);
        await File.WriteAllTextAsync(filePath, content);
    }
}
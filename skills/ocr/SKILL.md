# OCR Agent Skill - OCR 技能

## 技能概述

OCR 技能是一个基于 .NET 10 的光学字符识别解决方案，提供了完整的图像文字识别功能。该技能采用 AOT（Ahead-of-Time）编译架构，确保高性能的文字识别和低内存占用。

### 主要功能

- **光学字符识别**：从图像中准确识别文字
- **多语言支持**：支持多种语言的文字识别
- **AOT 编译支持**：优化运行时性能和内存使用
- **依赖注入**：无缝集成到 .NET 依赖注入系统
- **配置管理**：通过 Options 模式管理 OCR 配置
- **日志集成**：与 Microsoft.Extensions.Logging 集成
- **高性能设计**：支持批处理、并行处理和内存优化

### 应用场景

- **文档数字化**：将纸质文档转换为可编辑的数字文档
- **证件识别**：识别身份证、护照等证件信息
- **发票识别**：自动识别发票内容并提取关键信息
- **车牌识别**：识别车辆牌照信息
- **票据识别**：识别各类票据的文字信息
- **图像搜索**：基于图像中的文字进行搜索
- **辅助阅读**：帮助视障人士阅读图像中的文字

## 快速入门

### 安装依赖

```bash
# 添加 NuGet 包
dotnet add package System.Drawing.Common
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Options
```

### 配置服务

```csharp
// 注册 OCR 服务
builder.Services.AddOCRServices();

// 配置 OCR 选项
builder.Services.Configure<OCROptions>(options => {
    options.Enabled = true;
    options.MaxImageSize = 4096;
    options.Timeout = TimeSpan.FromSeconds(30);
    options.Languages = new List<string> { "zh-CN", "en-US" };
});
```

### 使用 OCR 服务

```csharp
// 获取 OCR 服务
var ocrService = serviceProvider.GetRequiredService<IOCRService>();

// 识别图像中的文字
var imagePath = "path/to/image.jpg";
var result = await ocrService.RecognizeTextAsync(imagePath);

// 处理识别结果
foreach (var text in result.TextLines)
{
    Console.WriteLine($"识别到文字: {text}");
}
```

## 核心组件

### OCR 服务

- **IOCRService**：OCR 服务的主接口，提供文字识别功能
- **IImageProcessor**：图像处理器，用于图像预处理
- **ITextRecognizer**：文本识别器，用于识别图像中的文字
- **IOCRResult**：OCR 结果接口，包含识别到的文字信息

### 配置选项

- **OCROptions**：OCR 配置选项，包括启用状态、最大图像大小、超时设置等

### 数据模型

- **OCRResult**：OCR 识别结果，包含识别到的文字行、置信度等信息
- **OCRTextLine**：OCR 文本行，包含识别到的文字和位置信息

## 性能特性

- **AOT 编译**：减少启动时间和内存占用
- **并行处理**：支持多线程并行处理多个图像
- **批处理**：支持批量图像识别，提高处理效率
- **内存优化**：使用 Span、Memory 等技术减少内存分配
- **图像预处理**：支持图像缩放、灰度转换等预处理操作，提高识别准确率

## 部署指南

### AOT 编译

```bash
# 发布为 AOT 编译的单文件应用
dotnet publish --configuration Release --output ./publish --runtime win-x64 --self-contained true -p:PublishAot=true
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64

## 参考文档

- [详细文档](reference/README.md)
- [使用示例](reference/examples.md)

## 版本信息

- **技能版本**：1.0.0
- **.NET 版本**：net10.0

## 许可证

MIT License
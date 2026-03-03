# OCR 技能参考文档

## 概述

OCR 技能是一个基于 .NET 10 的光学字符识别解决方案，提供了完整的图像文字识别功能。本文档详细介绍了 OCR 技能的核心组件、使用方法、配置选项和部署指南。

## 核心组件

### OCR 服务

#### IOCRService

OCR 服务的主接口，提供文字识别功能。

```csharp
public interface IOCRService
{
    Task<IOCRResult> RecognizeTextAsync(string imagePath, CancellationToken cancellationToken = default);
    Task<IOCRResult> RecognizeTextAsync(Stream imageStream, CancellationToken cancellationToken = default);
    Task<IOCRResult> RecognizeTextAsync(Bitmap image, CancellationToken cancellationToken = default);
    Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths, CancellationToken cancellationToken = default);
}
```

#### IImageProcessor

图像处理器接口，用于图像预处理。

```csharp
public interface IImageProcessor
{
    Task<Bitmap> PreprocessAsync(Bitmap image, CancellationToken cancellationToken = default);
    Task<Bitmap> ResizeAsync(Bitmap image, int maxSize, CancellationToken cancellationToken = default);
    Task<Bitmap> ConvertToGrayscaleAsync(Bitmap image, CancellationToken cancellationToken = default);
    Task<Bitmap> EnhanceContrastAsync(Bitmap image, CancellationToken cancellationToken = default);
}
```

#### ITextRecognizer

文本识别器接口，用于识别图像中的文字。

```csharp
public interface ITextRecognizer
{
    Task<IOCRResult> RecognizeAsync(Bitmap image, IEnumerable<string> languages, CancellationToken cancellationToken = default);
}
```

#### IOCRResult

OCR 结果接口，包含识别到的文字信息。

```csharp
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    IEnumerable<string> TextLines { get; }
    IEnumerable<OCRTextLine> TextLineDetails { get; }
    double Confidence { get; }
    TimeSpan ProcessingTime { get; }
}
```

### 数据模型

#### OCRTextLine

OCR 文本行，包含识别到的文字和位置信息。

```csharp
public class OCRTextLine
{
    public string Text { get; set; }
    public double Confidence { get; set; }
    public Rectangle BoundingBox { get; set; }
    public int LineNumber { get; set; }
}
```

### 配置选项

#### OCROptions

OCR 配置选项，包括启用状态、最大图像大小、超时设置等。

| 属性 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| Enabled | bool | true | 是否启用 OCR 服务 |
| MaxImageSize | int | 4096 | 最大图像大小（像素） |
| Timeout | TimeSpan | 30秒 | OCR 处理超时时间 |
| Languages | List<string> | ["zh-CN", "en-US"] | 支持的语言列表 |
| EnableImagePreprocessing | bool | true | 是否启用图像预处理 |
| EnableParallelProcessing | bool | true | 是否启用并行处理 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |
| BatchSize | int | 10 | 批处理大小 |

## 使用示例

### 基本使用

```csharp
// 注册服务
builder.Services.AddOCRServices();

// 配置选项
builder.Services.Configure<OCROptions>(options => {
    options.Enabled = true;
    options.MaxImageSize = 4096;
    options.Languages = new List<string> { "zh-CN", "en-US" };
});

// 获取服务
var ocrService = serviceProvider.GetRequiredService<IOCRService>();

// 识别图像中的文字
var imagePath = "path/to/image.jpg";
var result = await ocrService.RecognizeTextAsync(imagePath);

// 处理识别结果
if (result.Success)
{
    Console.WriteLine("识别成功！");
    foreach (var text in result.TextLines)
    {
        Console.WriteLine($"识别到文字: {text}");
    }
    Console.WriteLine($"置信度: {result.Confidence:P}");
    Console.WriteLine($"处理时间: {result.ProcessingTime.TotalMilliseconds:F2}ms");
}
else
{
    Console.WriteLine($"识别失败: {result.ErrorMessage}");
}
```

### 批量处理

```csharp
// 批量识别多个图像
var imagePaths = new List<string>
{
    "image1.jpg",
    "image2.jpg",
    "image3.jpg"
};

var results = await ocrService.RecognizeTextBatchAsync(imagePaths);

foreach (var (result, index) in results.Select((r, i) => (r, i)))
{
    Console.WriteLine($"图像 {index + 1}: {(result.Success ? "成功" : "失败")}");
    if (result.Success)
    {
        Console.WriteLine($"识别到 {result.TextLines.Count()} 行文字");
    }
    else
    {
        Console.WriteLine($"错误: {result.ErrorMessage}");
    }
}
```

### 高级配置

```csharp
// 高级配置
builder.Services.AddOCRServices(options => {
    options.Enabled = true;
    options.MaxImageSize = 8192;
    options.Timeout = TimeSpan.FromMinutes(1);
    options.Languages = new List<string> { "zh-CN", "en-US", "ja-JP" };
    options.EnableImagePreprocessing = true;
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = 8;
    options.BatchSize = 20;
});
```

### 流式处理

```csharp
// 从流中识别文字
using var fileStream = File.OpenRead("image.jpg");
var result = await ocrService.RecognizeTextAsync(fileStream);
```

### 内存图像处理

```csharp
// 从内存图像中识别文字
using var bitmap = new Bitmap(800, 600);
// 绘制图像内容...
var result = await ocrService.RecognizeTextAsync(bitmap);
```

## 性能优化

### 图像预处理

启用图像预处理可以提高识别准确率和速度：

```csharp
options.EnableImagePreprocessing = true;
```

预处理步骤包括：
- 图像大小调整
- 灰度转换
- 对比度增强

### 并行处理

启用并行处理可以提高批量处理的速度：

```csharp
options.EnableParallelProcessing = true;
options.MaxDegreeOfParallelism = Environment.ProcessorCount;
```

### 批处理

使用批量处理方法可以减少网络开销和提高处理效率：

```csharp
var results = await ocrService.RecognizeTextBatchAsync(imagePaths);
```

### 内存优化

- 使用 `using` 语句确保及时释放图像资源
- 对于大图像，考虑先调整大小再处理
- 使用 `CancellationToken` 控制处理超时

## 错误处理

### 超时处理

```csharp
try
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var result = await ocrService.RecognizeTextAsync(imagePath, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("OCR 处理超时");
}
catch (Exception ex)
{
    Console.WriteLine($"OCR 处理失败: {ex.Message}");
}
```

### 错误处理

```csharp
var result = await ocrService.RecognizeTextAsync(imagePath);
if (!result.Success)
{
    Console.WriteLine($"识别失败: {result.ErrorMessage}");
    // 处理错误情况
}
```

## 部署指南

### AOT 编译

OCR 技能支持 AOT 编译，可以显著提高运行时性能和减少内存使用。

```bash
# 发布为 AOT 编译的单文件应用
dotnet publish --configuration Release --output ./publish --runtime win-x64 --self-contained true -p:PublishAot=true
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64

### 容器化部署

#### Dockerfile 示例

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish -r linux-x64 --self-contained true -p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./ocr_app"]
```

### 环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| DOTNET_ENVIRONMENT | 运行环境 | Development |
| DOTNET_PUBLISH_AOT | 是否启用 AOT | 1 |
| DOTNET_TieredCompilation | 是否启用分层编译 | 1 |
| DOTNET_ReadyToRun | 是否启用 ReadyToRun | 1 |
| LOG_LEVEL | 日志级别 | Information |

## 最佳实践

### 图像准备

1. **图像质量**：使用高分辨率、清晰的图像
2. **光照条件**：确保图像光照均匀，避免阴影和反光
3. **文字方向**：确保文字水平排列，避免倾斜
4. **背景干净**：使用干净的背景，避免干扰
5. **图像大小**：控制图像大小在合理范围内（建议不超过 4096x4096）

### 性能优化

1. **启用预处理**：对于低质量图像，启用图像预处理
2. **批量处理**：对于多个图像，使用批量处理方法
3. **并行处理**：启用并行处理以提高速度
4. **内存管理**：及时释放图像资源，避免内存泄漏
5. **超时控制**：设置合理的超时时间，避免长时间阻塞

### 错误处理

1. **超时处理**：使用 `CancellationToken` 控制处理超时
2. **错误检查**：检查 `result.Success` 并处理错误情况
3. **重试机制**：对于网络 OCR 服务，实现重试机制
4. **日志记录**：记录 OCR 处理的详细日志，便于排查问题

### 部署建议

1. **AOT 编译**：生产环境使用 AOT 编译以提高性能
2. **容器化**：使用 Docker 容器化部署，便于管理和扩展
3. **资源限制**：根据实际需求设置合理的 CPU 和内存限制
4. **监控**：实现 OCR 服务的监控，包括处理时间、成功率等指标
5. **扩展**：对于高并发场景，考虑使用负载均衡和水平扩展

## 故障排除

### 常见问题

1. **识别率低**
   - 检查图像质量
   - 启用图像预处理
   - 调整图像大小
   - 确保文字清晰可见

2. **处理速度慢**
   - 启用并行处理
   - 使用批量处理
   - 调整最大并行度
   - 减少图像大小

3. **内存使用高**
   - 及时释放图像资源
   - 减少批量大小
   - 调整最大图像大小
   - 使用 AOT 编译

4. **处理超时**
   - 增加超时设置
   - 减少图像大小
   - 优化图像预处理
   - 检查系统资源

5. **服务启动失败**
   - 检查依赖项是否安装
   - 检查配置文件是否正确
   - 检查系统权限
   - 查看日志了解详细错误

## 参考资源

- [System.Drawing.Common 文档](https://docs.microsoft.com/en-us/dotnet/api/system.drawing)
- [Microsoft.Extensions.DependencyInjection 文档](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Microsoft.Extensions.Options 文档](https://docs.microsoft.com/en-us/dotnet/core/extensions/options)
- [.NET AOT 文档](https://docs.microsoft.com/en-us/dotnet/core/deploying/native-aot)
- [Task Parallel Library 文档](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-parallel-library-tpl)

## 版本历史

| 版本 | 日期 | 变更说明 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本，支持 AOT 编译和完整的 OCR 功能 |

# qrcode Agent Skill - qrcode 技能

## 技能概述

基于.NET 10的高性能QR码和条码生成与识别技能，为.NET开发者提供强大的条码处理功能。该技能支持QR码和多种类型条码的生成与识别，具有高性能、跨平台、AOT编译支持等特性。

## 快速开始指南

### 安装依赖

在主应用程序的runfile中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.0
#:package ZXing.Net@0.16.12
#:package SkiaSharp@2.88.6
```

### 注册服务

在主应用程序中注册qrcode服务：

```csharp
// 注册qrcode服务
builder.Services.AddQrCodeServices(options => {
    options.EnableCaching = true;
    options.CacheSize = 100;
    options.DefaultErrorCorrectionLevel = ErrorCorrectionLevel.Medium;
    options.DefaultQRCodeSize = 256;
    options.EnableAsyncProcessing = true;
});
```

### 使用示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using QrCodeIntegration;

// 获取QR码生成服务
var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();

// 生成QR码
var qrCodeOptions = new QrCodeGenerationOptions
{
    Content = "https://www.example.com",
    Size = 256,
    ForegroundColor = Color.Black,
    BackgroundColor = Color.White,
    ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
};

var qrCodeImage = await qrCodeGenerator.GenerateQrCodeAsync(qrCodeOptions);

// 保存QR码到文件
await File.WriteAllBytesAsync("qrcode.png", qrCodeImage);
Console.WriteLine("QR码生成成功并保存到qrcode.png");

// 获取条码生成服务
var barcodeGenerator = serviceProvider.GetRequiredService<IBarcodeGenerator>();

// 生成条码
var barcodeOptions = new BarcodeGenerationOptions
{
    Content = "123456789012",
    BarcodeFormat = BarcodeFormat.EAN_13,
    Width = 300,
    Height = 100
};

var barcodeImage = await barcodeGenerator.GenerateBarcodeAsync(barcodeOptions);

// 保存条码到文件
await File.WriteAllBytesAsync("barcode.png", barcodeImage);
Console.WriteLine("条码生成成功并保存到barcode.png");
```

## 导航地图

```
qrcode/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── qrcode_barcode_integration.cs     # QR码和条码核心实现
    ├── qrcode_barcode_integration.run.json  # 运行配置
    └── qrcode_barcode_integration.setting.json  # 设置文件
```

## 主要功能

1. **QR码生成**：支持自定义大小、颜色、容错级别，生成高质量QR码
2. **QR码识别**：支持从图片、流中识别QR码，返回解码内容
3. **条码生成**：支持多种条码类型，包括Code128、EAN13、UPC-A等
4. **条码识别**：支持从图片、流中识别条码，返回解码内容
5. **批量处理**：支持批量生成和识别，提高处理效率
6. **高性能设计**：优化的编码/解码算法，减少CPU和内存使用
7. **跨平台支持**：使用SkiaSharp实现跨平台图形处理，支持Windows、Linux、macOS
8. **AOT编译支持**：避免反射和动态代码，支持AOT编译，提高性能和减小部署大小
9. **缓存机制**：缓存常用配置和结果，提高重复操作的性能
10. **依赖注入集成**：无缝集成Microsoft.Extensions.DependencyInjection

## AOT 编译配置

### 项目文件配置

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
  <ReadyToRun>true</ReadyToRun>
  <TieredCompilation>true</TieredCompilation>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  <Deterministic>true</Deterministic>
  <Strict>true</Strict>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
  <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
  <PackageReference Include="ZXing.Net" Version="0.16.12" />
  <PackageReference Include="SkiaSharp" Version="2.88.6" />
</ItemGroup>
```

### 发布命令

```bash
# 发布 Windows x64 版本
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true

# 发布 Linux x64 版本
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishAot=true

# 发布 macOS x64 版本
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishAot=true
```

## 故障排除

### 常见问题

1. **QR码生成失败**
   - 检查内容是否过长：QR码有内容长度限制
   - 检查尺寸设置：尺寸过小可能无法容纳所有内容
   - 检查依赖项：确保所有依赖项已正确安装

2. **条码识别失败**
   - 检查图片质量：确保图片清晰，条码完整
   - 检查条码类型：确保使用正确的条码类型进行识别
   - 检查光线条件：避免过度曝光或光线不足的图片

3. **跨平台兼容性问题**
   - 确保使用SkiaSharp而非System.Drawing.Common进行跨平台图形处理
   - 避免使用平台特定的API
   - 测试在目标平台上的运行情况

4. **AOT编译错误**
   - 确保代码中没有使用反射或动态代码
   - 确保所有依赖项都支持AOT编译
   - 检查TrimMode设置，必要时调整为partial

5. **性能问题**
   - 启用缓存：设置EnableCaching为true
   - 使用批量处理：对于多个条码，使用批量API
   - 调整缓存大小：根据实际需求调整CacheSize

### 错误处理

```csharp
try
{
    var qrCodeImage = await qrCodeGenerator.GenerateQrCodeAsync(qrCodeOptions);
    // 处理成功
} catch (QrCodeGenerationException ex)
{
    Console.WriteLine($"QR码生成错误: {ex.Message}");
} catch (Exception ex)
{
    Console.WriteLine($"未知错误: {ex.Message}");
}

try
{
    var decodedContent = await qrCodeReader.ReadQrCodeAsync(qrCodeImage);
    // 处理成功
} catch (QrCodeReadException ex)
{
    Console.WriteLine($"QR码读取错误: {ex.Message}");
} catch (Exception ex)
{
    Console.WriteLine($"未知错误: {ex.Message}");
}
```

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，避免直接实例化
2. **异步编程**：优先使用异步API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，提供友好的错误信息
4. **缓存使用**：对于重复生成相同内容的条码，启用缓存提高性能
5. **批量处理**：对于多个条码的生成或识别，使用批量API提高效率
6. **资源管理**：正确释放不再使用的资源，避免内存泄漏
7. **配置管理**：使用Options模式管理配置，便于统一配置和修改
8. **日志记录**：添加适当的日志记录，便于排查问题
9. **性能监控**：监控关键性能指标，如生成/识别时间、内存使用等
10. **AOT优化**：利用AOT编译提高性能和减小部署大小

## 扩展说明

该技能提供了完整的QR码和条码解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现IQrCodeGenerator、IBarcodeGenerator等接口
2. **扩展功能**：添加新的条码类型或QR码功能
3. **集成其他系统**：与其他系统集成，如库存管理、支付系统等
4. **性能优化**：针对特定场景优化性能
5. **自定义渲染**：实现自定义的条码渲染逻辑

## 版本历史

### v1.0.0
- 初始版本
- 支持QR码生成和识别
- 支持多种条码类型的生成和识别
- 支持跨平台
- 支持AOT编译

### v1.1.0
- 增加批量处理功能
- 优化性能和内存使用
- 增加更多条码类型支持
- 改进错误处理

### v1.2.0
- 增加缓存机制
- 优化AOT编译支持
- 增加更多自定义选项
- 改进跨平台支持

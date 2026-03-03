# captcha Agent Skill - 验证码技能

## 技能概述

基于 .NET 10 的高性能验证码技能，为 .NET 开发者提供强大的验证码生成、验证和识别功能，支持 AOT（提前编译）编译，适用于构建安全、高效的验证码系统。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:package LazyCaptcha@2.0.0
#:package CaptchaSharp@1.0.0
```

### 注册服务

在您的主应用程序中注册验证码服务：

```csharp
// 配置验证码服务
builder.Services.AddCaptcha(options => {
    // 图片验证码配置
    options.ImageCaptcha = new ImageCaptchaOptions {
        Width = 120,
        Height = 40,
        Length = 4,
        FontSize = 20,
        BackgroundColor = Color.White,
        NoiseLevel = NoiseLevel.Medium,
        DistortionLevel = DistortionLevel.High
    };
    
    // 滑块验证码配置
    options.SlideCaptcha = new SlideCaptchaOptions {
        Width = 300,
        Height = 150,
        SliderWidth = 40,
        SliderHeight = 40,
        Tolerance = 5,
        BackgroundType = BackgroundType.Gradient
    };
    
    // AOT 优化配置
    options.EnableAotOptimization = true;
    options.UseMemoryCache = true;
    options.CacheSize = 1000;
});

// 注册验证码生成器
builder.Services.AddSingleton<IImageCaptchaGenerator, DefaultImageCaptchaGenerator>();
builder.Services.AddSingleton<ISlideCaptchaGenerator, DefaultSlideCaptchaGenerator>();
builder.Services.AddSingleton<ICaptchaValidator, DefaultCaptchaValidator>();
builder.Services.AddSingleton<ICaptchaRecognizer, DefaultCaptchaRecognizer>();
```

### 使用示例

```csharp
// 获取验证码服务
var captchaService = serviceProvider.GetRequiredService<ICaptchaService>();

// 生成图片验证码
var imageCaptcha = await captchaService.GenerateImageCaptchaAsync();
Console.WriteLine($"验证码文本: {imageCaptcha.Text}");
Console.WriteLine($"验证码ID: {imageCaptcha.Id}");
// 验证码图片数据在 imageCaptcha.ImageData 中

// 验证图片验证码
var validateResult = await captchaService.ValidateImageCaptchaAsync(imageCaptcha.Id, "1234");
Console.WriteLine($"验证结果: {validateResult}");

// 生成滑块验证码
var slideCaptcha = await captchaService.GenerateSlideCaptchaAsync();
Console.WriteLine($"滑块验证码ID: {slideCaptcha.Id}");
// 滑块验证码背景图片: slideCaptcha.BackgroundImage
// 滑块图片: slideCaptcha.SliderImage
// 正确位置: slideCaptcha.CorrectPosition

// 验证滑块验证码
var slideResult = await captchaService.ValidateSlideCaptchaAsync(slideCaptcha.Id, new Point(150, 50));
Console.WriteLine($"滑块验证结果: {slideResult}");
```

## 导航地图

```
captcha/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── captcha_recognizer_integration.cs       # 验证码识别集成示例
    ├── captcha_recognizer_integration.run.json  # 运行配置
    ├── captcha_recognizer_integration.setting.json  # 设置文件
    ├── captchagen_integration.cs                # 验证码生成集成示例
    ├── captchagen_integration.run.json           # 运行配置
    ├── captchagen_integration.setting.json       # 设置文件
    ├── captchasharp_integration.cs              # CaptchaSharp 集成示例
    ├── captchasharp_integration.run.json         # 运行配置
    ├── captchasharp_integration.setting.json     # 设置文件
    ├── hei_captcha_integration.cs               # HeiCaptcha 集成示例
    ├── hei_captcha_integration.run.json          # 运行配置
    ├── hei_captcha_integration.setting.json      # 设置文件
    ├── lazycaptcha_integration.cs               # LazyCaptcha 集成示例
    ├── lazycaptcha_integration.run.json          # 运行配置
    ├── lazycaptcha_integration.setting.json      # 设置文件
    ├── lazyslidecaptcha_integration.cs          # LazySlideCaptcha 集成示例
    ├── lazyslidecaptcha_integration.run.json     # 运行配置
    └── lazyslidecaptcha_integration.setting.json # 设置文件
```

## 主要功能

1. **多种验证码类型支持**: 图片验证码、滑块验证码、点选验证码、短信验证码等
2. **验证码生成**: 支持自定义样式、尺寸、字体、颜色、噪声等参数
3. **验证码验证**: 支持多种验证方式，包括精确匹配、模糊匹配、容错验证等
4. **验证码识别**: 支持自动识别多种类型的验证码
5. **AOT 编译优化**: 支持将应用编译为本机代码，提高运行时性能
6. **高性能设计**: 优化的内存使用和并发处理，支持高并发场景
7. **可定制性**: 支持自定义验证码生成器、验证器和识别器
8. **缓存支持**: 内置缓存机制，提高性能和减少资源消耗
9. **异步编程模型**: 基于异步编程模型，避免阻塞主线程
10. **易于集成**: 与 .NET 生态系统无缝集成，支持依赖注入

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
  <PackageReference Include="SixLabors.ImageSharp" Version="3.1.0" />
  <PackageReference Include="LazyCaptcha" Version="2.0.0" />
</ItemGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained
```

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的验证码库版本支持 AOT 编译
2. **避免反射**: 避免在验证码生成和验证中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用值类型**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Program.cs 中配置
var builder = WebApplication.CreateBuilder(args);

// 配置验证码服务
builder.Services.AddCaptcha(options => {
    options.ImageCaptcha.Width = 150;
    options.ImageCaptcha.Height = 50;
    options.ImageCaptcha.Length = 5;
    options.EnableAotOptimization = true;
});

var app = builder.Build();

// 图片验证码端点
app.MapGet("/captcha/image", async (ICaptchaService captchaService) => {
    var captcha = await captchaService.GenerateImageCaptchaAsync();
    return Results.Ok(new {
        id = captcha.Id,
        image = Convert.ToBase64String(captcha.ImageData),
        contentType = "image/png"
    });
});

// 滑块验证码端点
app.MapGet("/captcha/slide", async (ICaptchaService captchaService) => {
    var captcha = await captchaService.GenerateSlideCaptchaAsync();
    return Results.Ok(new {
        id = captcha.Id,
        background = Convert.ToBase64String(captcha.BackgroundImage),
        slider = Convert.ToBase64String(captcha.SliderImage),
        contentType = "image/png"
    });
});

// 验证码验证端点
app.MapPost("/captcha/validate", async (ICaptchaService captchaService, CaptchaValidationRequest request) => {
    bool result;
    if (request.Type == CaptchaType.Image) {
        result = await captchaService.ValidateImageCaptchaAsync(request.Id, request.Value);
    } else {
        result = await captchaService.ValidateSlideCaptchaAsync(request.Id, request.Position);
    }
    return Results.Ok(new { success = result });
});

await app.RunAsync();
```

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用缓存**: 启用内置缓存，减少重复生成相同验证码的开销
3. **优化图片尺寸**: 根据实际需求调整验证码图片尺寸，避免过大的图片
4. **减少噪声和扭曲**: 适当调整噪声和扭曲级别，平衡安全性和性能
5. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
6. **配置合理的缓存大小**: 根据预期并发量调整缓存大小
7. **使用内存池**: 对于图片数据，使用内存池减少 GC 压力
8. **优化字体加载**: 预加载常用字体，避免运行时字体加载开销

## 故障排除

### 常见问题

1. **验证码生成缓慢**
   - 调整验证码尺寸和复杂度
   - 启用缓存
   - 优化字体和资源加载
   - 考虑使用 AOT 编译

2. **验证码验证失败**
   - 检查验证码 ID 是否有效
   - 验证验证码文本是否正确
   - 检查验证码是否已过期
   - 查看详细日志

3. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性

4. **内存占用过高**
   - 调整缓存大小
   - 优化图片尺寸
   - 使用内存池
   - 调整验证码生成频率

## 扩展开发

### 添加自定义验证码生成器

```csharp
// 自定义图片验证码生成器
public class CustomImageCaptchaGenerator : IImageCaptchaGenerator
{
    private readonly ImageCaptchaOptions _options;
    
    public CustomImageCaptchaGenerator(IOptions<ImageCaptchaOptions> options) {
        _options = options.Value;
    }
    
    public ImageCaptcha Generate() {
        // 自定义验证码生成逻辑
        var captcha = new ImageCaptcha {
            Id = Guid.NewGuid().ToString(),
            Text = GenerateRandomText(_options.Length),
            // 实现自定义图片生成
            ImageData = GenerateCustomImage(_options, GenerateRandomText(_options.Length))
        };
        
        return captcha;
    }
    
    // 其他方法实现...
}

// 注册自定义生成器
builder.Services.AddSingleton<IImageCaptchaGenerator, CustomImageCaptchaGenerator>();
```

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理验证码服务
2. **采用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **实现幂等验证**: 确保验证码验证逻辑是幂等的
4. **设置合理的过期时间**: 根据业务需求设置合理的验证码过期时间
5. **启用详细日志**: 在开发和测试阶段启用详细日志，便于调试
6. **定期轮换验证码**: 定期更新验证码样式和参数，提高安全性
7. **结合多种验证码类型**: 根据业务场景结合使用多种验证码类型
8. **考虑安全性和可用性平衡**: 平衡验证码的安全性和用户体验
9. **进行性能测试**: 在生产环境部署前进行充分的性能测试
10. **监控验证码使用情况**: 监控验证码生成和验证的成功率、延迟等指标

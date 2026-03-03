# captcha - 参考文档

## 概述

captcha是一个基于.NET 10的高性能验证码系统，专为.NET开发者设计，支持多种验证码类型的生成、验证和识别，具备AOT（提前编译）编译支持。

## 核心组件

### 1. 验证码服务
- **位置**: scripts/captcha_recognizer_integration.cs
- **功能**: 验证码核心业务逻辑处理
- **特性**: 
  - 多种验证码类型支持
  - 高性能验证码生成算法
  - 可靠的验证码验证机制
  - 智能验证码识别
  - 详细的错误处理
  - 完整的日志记录
  - AOT编译优化

### 2. 图片验证码生成器
- **位置**: scripts/lazycaptcha_integration.cs
- **功能**: 生成各种样式的图片验证码
- **特性**: 
  - 自定义尺寸和字体
  - 支持多种噪声和扭曲效果
  - 可配置的背景和颜色
  - 高性能图片生成
  - AOT兼容设计

### 3. 滑块验证码生成器
- **位置**: scripts/lazyslidecaptcha_integration.cs
- **功能**: 生成滑块验证码
- **特性**: 
  - 随机背景图片
  - 精确的滑块定位
  - 支持多种滑块形状
  - 容错验证机制

### 4. 验证码验证器
- **功能**: 验证用户提交的验证码
- **特性**: 
  - 支持多种验证方式
  - 容错验证选项
  - 验证码过期处理
  - 防止暴力破解

### 5. 验证码识别器
- **功能**: 自动识别验证码
- **特性**: 
  - 支持多种验证码类型
  - 高精度识别算法
  - 异步识别支持
  - 可扩展的识别引擎

## 使用示例

### 基本使用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:package LazyCaptcha@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;

// 配置服务
var builder = new ServiceCollection();
builder.AddLogging(logging => {
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// 配置验证码服务
builder.AddCaptcha(options => {
    options.ImageCaptcha = new ImageCaptchaOptions {
        Width = 120,
        Height = 40,
        Length = 4,
        FontSize = 20,
        NoiseLevel = NoiseLevel.Medium,
        DistortionLevel = DistortionLevel.High
    };
    options.EnableAotOptimization = true;
    options.UseMemoryCache = true;
});

// 注册服务
builder.AddSingleton<IImageCaptchaGenerator, DefaultImageCaptchaGenerator>();
builder.AddSingleton<ICaptchaValidator, DefaultCaptchaValidator>();

var serviceProvider = builder.BuildServiceProvider();

// 获取验证码服务
var captchaGenerator = serviceProvider.GetRequiredService<IImageCaptchaGenerator>();
var captchaValidator = serviceProvider.GetRequiredService<ICaptchaValidator>();

// 生成验证码
var captcha = captchaGenerator.Generate();
Console.WriteLine($"验证码文本: {captcha.Text}");
Console.WriteLine($"验证码ID: {captcha.Id}");

// 验证验证码
var isValid = captchaValidator.Validate(captcha.Id, captcha.Text);
Console.WriteLine($"验证结果: {isValid}");
```

### 高级配置

```csharp
// 高级验证码配置
var captchaOptions = new CaptchaOptions {
    // 图片验证码配置
    ImageCaptcha = new ImageCaptchaOptions {
        Width = 150,
        Height = 50,
        Length = 5,
        FontSize = 24,
        BackgroundColor = Color.LightGray,
        FontFamily = "Arial, Microsoft YaHei",
        NoiseLevel = NoiseLevel.High,
        DistortionLevel = DistortionLevel.Medium,
        CharacterSet = CharacterSet.Alphanumeric,
        UseLowercase = true,
        UseUppercase = true,
        UseNumbers = true,
        ExcludeSimilarCharacters = true
    },
    
    // 滑块验证码配置
    SlideCaptcha = new SlideCaptchaOptions {
        Width = 300,
        Height = 150,
        SliderWidth = 40,
        SliderHeight = 40,
        Tolerance = 3,
        BackgroundType = BackgroundType.Image,
        SliderType = SliderType.Circle,
        MaskType = MaskType.Round
    },
    
    // 全局配置
    EnableAotOptimization = true,
    UseMemoryCache = true,
    CacheSize = 2000,
    CacheExpiration = TimeSpan.FromMinutes(5),
    EnableDetailedLogging = false,
    MaxAttempts = 5,
    LockoutDuration = TimeSpan.FromMinutes(1)
};

// 注册配置
builder.Services.Configure<CaptchaOptions>(options => {
    options.ImageCaptcha = captchaOptions.ImageCaptcha;
    options.SlideCaptcha = captchaOptions.SlideCaptcha;
    options.EnableAotOptimization = captchaOptions.EnableAotOptimization;
    options.UseMemoryCache = captchaOptions.UseMemoryCache;
    options.CacheSize = captchaOptions.CacheSize;
    options.CacheExpiration = captchaOptions.CacheExpiration;
});
```

## 配置选项

### 验证码配置

```json
{
  "Captcha": {
    "EnableAotOptimization": true,          // 启用AOT优化
    "UseMemoryCache": true,                 // 使用内存缓存
    "CacheSize": 1000,                      // 缓存大小
    "CacheExpiration": "00:05:00",         // 缓存过期时间
    "EnableDetailedLogging": false,         // 启用详细日志
    "MaxAttempts": 5,                       // 最大尝试次数
    "LockoutDuration": "00:01:00",         // 锁定时长
    "ImageCaptcha": {
      "Width": 120,                         // 图片宽度
      "Height": 40,                        // 图片高度
      "Length": 4,                         // 验证码长度
      "FontSize": 20,                      // 字体大小
      "BackgroundColor": "#FFFFFF",       // 背景颜色
      "FontFamily": "Arial, Microsoft YaHei", // 字体
      "NoiseLevel": "Medium",             // 噪声级别
      "DistortionLevel": "High",          // 扭曲级别
      "CharacterSet": "Alphanumeric",     // 字符集
      "UseLowercase": true,                // 使用小写字母
      "UseUppercase": true,                // 使用大写字母
      "UseNumbers": true,                  // 使用数字
      "ExcludeSimilarCharacters": true     // 排除相似字符
    },
    "SlideCaptcha": {
      "Width": 300,                        // 背景宽度
      "Height": 150,                       // 背景高度
      "SliderWidth": 40,                   // 滑块宽度
      "SliderHeight": 40,                  // 滑块高度
      "Tolerance": 5,                      // 容差值
      "BackgroundType": "Gradient",       // 背景类型
      "SliderType": "Rectangle",          // 滑块类型
      "MaskType": "Round"                 // 遮罩类型
    }
  }
}
```

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
  <PackageReference Include="SlideCaptcha" Version="1.5.0" />
</ItemGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的验证码库版本支持 AOT 编译
2. **避免反射**: 避免在验证码生成和验证中使用反射
3. **资源处理**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **值类型优先**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用缓存**: 启用内置缓存，减少重复生成相同验证码的开销
3. **优化图片尺寸**: 根据实际需求调整验证码图片尺寸，避免过大的图片
4. **减少噪声和扭曲**: 适当调整噪声和扭曲级别，平衡安全性和性能
5. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
6. **配置合理的缓存大小**: 根据预期并发量调整缓存大小
7. **使用内存池**: 对于图片数据，使用内存池减少 GC 压力
8. **优化字体加载**: 预加载常用字体，避免运行时字体加载开销
9. **减少验证码长度**: 在保证安全性的前提下，减少验证码长度
10. **使用高效的图片格式**: 考虑使用 WebP 等高效图片格式

## 故障排除

### 常见问题

1. **验证码生成缓慢**
   - 调整验证码尺寸和复杂度
   - 启用缓存
   - 优化字体和资源加载
   - 考虑使用 AOT 编译
   - 检查系统资源使用情况

2. **验证码验证失败**
   - 检查验证码 ID 是否有效
   - 验证验证码文本是否正确
   - 检查验证码是否已过期
   - 查看详细日志
   - 检查缓存配置

3. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode

4. **内存占用过高**
   - 调整缓存大小
   - 优化图片尺寸
   - 使用内存池
   - 调整验证码生成频率
   - 考虑使用分布式缓存

5. **验证码识别率低**
   - 调整验证码复杂度
   - 改进识别算法
   - 考虑使用第三方识别服务
   - 增加训练数据

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
        // 生成随机验证码文本
        var text = GenerateRandomText(_options.Length);
        
        // 创建验证码图片
        using var image = new Image<Rgba32>(_options.Width, _options.Height);
        
        // 自定义绘制逻辑
        DrawBackground(image);
        DrawText(image, text);
        AddNoise(image);
        AddDistortion(image);
        
        // 转换为字节数组
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        
        return new ImageCaptcha {
            Id = Guid.NewGuid().ToString(),
            Text = text,
            ImageData = stream.ToArray(),
            CreatedAt = DateTime.UtcNow
        };
    }
    
    private string GenerateRandomText(int length) {
        // 自定义随机文本生成逻辑
    }
    
    private void DrawBackground(Image<Rgba32> image) {
        // 自定义背景绘制逻辑
    }
    
    private void DrawText(Image<Rgba32> image, string text) {
        // 自定义文本绘制逻辑
    }
    
    private void AddNoise(Image<Rgba32> image) {
        // 自定义噪声添加逻辑
    }
    
    private void AddDistortion(Image<Rgba32> image) {
        // 自定义扭曲添加逻辑
    }
}

// 注册自定义生成器
builder.Services.AddSingleton<IImageCaptchaGenerator, CustomImageCaptchaGenerator>();
```

### 添加自定义验证码验证器

```csharp
// 自定义验证码验证器
public class CustomCaptchaValidator : ICaptchaValidator
{
    private readonly ICaptchaCache _cache;
    private readonly CaptchaOptions _options;
    
    public CustomCaptchaValidator(ICaptchaCache cache, IOptions<CaptchaOptions> options) {
        _cache = cache;
        _options = options.Value;
    }
    
    public async Task<bool> ValidateImageCaptchaAsync(string captchaId, string inputText) {
        // 从缓存获取验证码
        var captcha = await _cache.GetImageCaptchaAsync(captchaId);
        if (captcha == null) {
            return false;
        }
        
        // 验证验证码是否过期
        if (DateTime.UtcNow - captcha.CreatedAt > _options.CacheExpiration) {
            await _cache.RemoveImageCaptchaAsync(captchaId);
            return false;
        }
        
        // 自定义验证逻辑
        bool isValid = CompareCaptchaText(captcha.Text, inputText, _options.IgnoreCase);
        
        // 如果验证失败，增加尝试次数
        if (!isValid) {
            await _cache.IncrementAttemptsAsync(captchaId);
            
            // 检查是否达到最大尝试次数
            var attempts = await _cache.GetAttemptsAsync(captchaId);
            if (attempts >= _options.MaxAttempts) {
                await _cache.LockCaptchaAsync(captchaId, _options.LockoutDuration);
            }
        } else {
            // 验证成功，移除验证码
            await _cache.RemoveImageCaptchaAsync(captchaId);
        }
        
        return isValid;
    }
    
    private bool CompareCaptchaText(string expected, string actual, bool ignoreCase) {
        // 自定义文本比较逻辑
    }
    
    // 实现其他接口方法...
}

// 注册自定义验证器
builder.Services.AddSingleton<ICaptchaValidator, CustomCaptchaValidator>();
```

## 安全最佳实践

1. **定期更新验证码样式**: 定期更新验证码样式和参数，提高安全性
2. **使用合适的验证码类型**: 根据业务场景选择合适的验证码类型
3. **设置合理的过期时间**: 验证码应在短时间内过期
4. **限制尝试次数**: 限制同一IP或用户的尝试次数
5. **实现锁定机制**: 对多次失败的用户进行临时锁定
6. **结合其他安全措施**: 结合IP限制、设备指纹等其他安全措施
7. **避免可预测的验证码**: 确保验证码生成算法是随机的
8. **保护验证码ID**: 确保验证码ID难以猜测
9. **使用HTTPS**: 始终使用HTTPS传输验证码
10. **定期审计**: 定期审计验证码使用情况，发现异常行为

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
    options.UseMemoryCache = true;
    options.CacheSize = 1000;
});

var app = builder.Build();

// 图片验证码端点
app.MapGet("/api/captcha/image", async (ICaptchaService captchaService) => {
    var captcha = await captchaService.GenerateImageCaptchaAsync();
    return Results.Ok(new {
        id = captcha.Id,
        image = Convert.ToBase64String(captcha.ImageData),
        contentType = "image/png"
    });
});

// 滑块验证码端点
app.MapGet("/api/captcha/slide", async (ICaptchaService captchaService) => {
    var captcha = await captchaService.GenerateSlideCaptchaAsync();
    return Results.Ok(new {
        id = captcha.Id,
        background = Convert.ToBase64String(captcha.BackgroundImage),
        slider = Convert.ToBase64String(captcha.SliderImage),
        contentType = "image/png"
    });
});

// 验证码验证端点
app.MapPost("/api/captcha/validate", async (ICaptchaService captchaService, CaptchaValidationRequest request) => {
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

### 与 React 前端集成

```javascript
// React 组件示例
function ImageCaptcha() {
    const [captcha, setCaptcha] = useState(null);
    const [input, setInput] = useState('');
    const [isValid, setIsValid] = useState(false);
    
    // 加载验证码
    const loadCaptcha = async () => {
        const response = await fetch('/api/captcha/image');
        const data = await response.json();
        setCaptcha(data);
    };
    
    // 验证验证码
    const validateCaptcha = async () => {
        const response = await fetch('/api/captcha/validate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                id: captcha.id,
                value: input,
                type: 'Image'
            })
        });
        const data = await response.json();
        setIsValid(data.success);
    };
    
    useEffect(() => {
        loadCaptcha();
    }, []);
    
    return (
        <div className="captcha-container">
            {captcha && (
                <>
                    <img 
                        src={`data:${captcha.contentType};base64,${captcha.image}`} 
                        alt="验证码" 
                        onClick={loadCaptcha} 
                        style={{ cursor: 'pointer' }}
                    />
                    <input
                        type="text"
                        value={input}
                        onChange={(e) => setInput(e.target.value)}
                        placeholder="请输入验证码"
                    />
                    <button onClick={validateCaptcha}>验证</button>
                    {isValid && <span>验证成功！</span>}
                </>
            )}
        </div>
    );
}
```
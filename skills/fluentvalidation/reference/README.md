# FluentValidation - 参考文档

## 概述

FluentValidation 是基于 .NET 10 AOT 编译的高性能验证系统，专为 .NET 开发者设计。

## 核心组件

### 1. IFluentValidationService（验证服务接口）
- **位置**: scripts/fluentvalidation_aot.cs
- **功能**: 定义验证服务的核心业务逻辑接口
- **特性**: 
  - 异步方法支持
  - 完整的验证功能
  - 统一的命令执行接口
  - 详细的结果返回

### 2. FluentValidationService（验证服务实现）
- **位置**: scripts/fluentvalidation_aot.cs
- **功能**: 实现验证服务的核心业务逻辑
- **特性**: 
  - 完整的对象验证实现
  - 测试模型验证支持
  - 详细的错误处理
  - 性能监控

### 3. FluentValidationAotEngine（命令行引擎）
- **位置**: scripts/fluentvalidation_aot.cs
- **功能**: 处理命令行参数和执行验证操作
- **特性**: 
  - 支持多种命令别名
  - 详细的帮助信息
  - 命令行参数解析
  - 结果格式化输出

### 4. TestModelValidator（测试模型验证器）
- **位置**: scripts/fluentvalidation_aot.cs
- **功能**: 验证测试模型的各个字段
- **特性**: 
  - 完整的字段验证规则
  - 详细的错误消息
  - 支持复杂验证规则，如密码确认

## 使用示例

### 基本用法

`csharp
// 获取 FluentValidation 服务
var validationService = serviceProvider.GetRequiredService<IFluentValidationService>();

// 创建测试模型
var testModel = new TestModel
{
    Name = "张三",
    Age = 25,
    Email = "zhangsan@example.com",
    Phone = "13800138000",
    Password = "password123",
    ConfirmPassword = "password123"
};

// 验证测试模型
var validateResult = await validationService.ValidateTestModelAsync(testModel);
Console.WriteLine($"验证结果: {(validateResult.Success ? "成功" : "失败")}");
Console.WriteLine($"执行时间: {validateResult.ExecutionTimeMs} ms");

if (validateResult.ValidationResult != null)
{
    Console.WriteLine($"验证状态: {(validateResult.ValidationResult.IsValid ? "通过" : "失败")}");
    if (!validateResult.ValidationResult.IsValid)
    {
        Console.WriteLine("验证错误:");
        foreach (var error in validateResult.ValidationResult.Errors)
        {
            Console.WriteLine($"  {error.PropertyName}: {error.ErrorMessage}");
        }
    }
}

// 获取验证器信息
var validatorsResult = await validationService.GetValidatorsAsync();
Console.WriteLine("\n验证器信息:");
foreach (var item in validatorsResult.Results)
{
    Console.WriteLine($"- {item}");
}

// 获取版本信息
var versionResult = await validationService.GetVersionInfoAsync();
Console.WriteLine("\n版本信息:");
foreach (var item in versionResult.Results)
{
    Console.WriteLine($"- {item}");
}
`

### 高级配置

`csharp
// 配置 FluentValidation 选项
var fluentValidationOptions = new FluentValidationOptions
{
    WorkingDirectory = Environment.CurrentDirectory,
    EnableDetailedLogging = true,
    EnablePerformanceMonitoring = true,
    RequestTimeoutMs = 60000,
    EnableCache = true,
    CacheSize = 2000
};

builder.Services.Configure<FluentValidationOptions>(options => {
    options.WorkingDirectory = fluentValidationOptions.WorkingDirectory;
    options.EnableDetailedLogging = fluentValidationOptions.EnableDetailedLogging;
    options.EnablePerformanceMonitoring = fluentValidationOptions.EnablePerformanceMonitoring;
    options.RequestTimeoutMs = fluentValidationOptions.RequestTimeoutMs;
    options.EnableCache = fluentValidationOptions.EnableCache;
    options.CacheSize = fluentValidationOptions.CacheSize;
});
`

## 配置选项

### FluentValidation 配置

`json
{
  "FluentValidation": {
    "WorkingDirectory": "",          // 工作目录
    "EnableDetailedLogging": false,    // 是否启用详细日志
    "EnablePerformanceMonitoring": true, // 是否启用性能监控
    "RequestTimeoutMs": 30000,        // 请求超时时间（毫秒）
    "EnableCache": true,              // 是否启用缓存
    "CacheSize": 1000                // 缓存大小
  }
}
`

## 性能优化

1. **AOT 编译**: 提前编译为本地代码，减少启动时间和内存占用
2. **异步编程**: 使用异步 API 避免阻塞
3. **缓存使用**: 启用缓存以提高性能，特别是对于重复验证的对象
4. **批量处理**: 对于多个对象的验证，考虑批处理以提高效率
5. **验证器复用**: 复用验证器实例，避免重复创建

## 故障排除

### 常见问题

1. **验证失败**
   - 检查验证规则是否正确
   - 验证输入数据是否符合规则
   - 查看详细的验证错误信息

2. **性能问题**
   - 启用 AOT 编译
   - 启用缓存
   - 优化验证规则，避免复杂的验证逻辑
   - 考虑使用批处理验证

3. **配置问题**
   - 检查配置文件是否正确
   - 验证依赖项是否正确安装
   - 查看日志信息以获取更多详情

## 扩展开发

### 添加自定义验证器

`csharp
// 自定义模型
public class CustomModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

// 自定义验证器
public class CustomModelValidator : AbstractValidator<CustomModel>
{
    public CustomModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("名称不能为空")
            .Length(2, 100).WithMessage("名称长度必须在2-100个字符之间");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("手机号不能为空")
            .Length(11).WithMessage("手机号必须为11位");
    }
}

// 注册自定义验证器
builder.Services.AddSingleton<IValidator<CustomModel>, CustomModelValidator>();

// 使用自定义验证器
public async Task<ValidationCommandResult> ValidateCustomModelAsync(CustomModel model)
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var result = new ValidationCommandResult
    {
        CommandType = ValidationCommandType.Validate
    };

    try
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<CustomModel>>();
        var validationResult = await validator.ValidateAsync(model);
        result.Success = validationResult.IsValid;
        result.ValidationResult = MapValidationResult(validationResult);
        result.ValidationResult.ValidationTimeMs = stopwatch.ElapsedMilliseconds;
        result.ValidationResult.ObjectType = typeof(CustomModel).Name;
        
        if (validationResult.IsValid)
        {
            result.Results.Add("自定义模型验证成功");
        }
        else
        {
            result.Results.Add("自定义模型验证失败");
            result.Results.Add($"错误数量: {validationResult.Errors.Count}");
            foreach (var error in validationResult.Errors)
            {
                result.Results.Add($"- {error.PropertyName}: {error.ErrorMessage}");
            }
        }
    }
    catch (Exception ex)
    {
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
`

### 扩展验证服务

`csharp
public class ExtendedFluentValidationService : FluentValidationService
{
    public ExtendedFluentValidationService(IOptions<FluentValidationOptions> options, ILogger<FluentValidationService> logger, IValidator<TestModel> testModelValidator)
        : base(options, logger, testModelValidator)
    {}

    public async Task<ValidationCommandResult> ValidateCustomModelAsync(CustomModel model)
    {
        // 实现自定义模型验证逻辑
        // ...
    }
}

// 注册扩展服务
builder.Services.AddSingleton<IFluentValidationService, ExtendedFluentValidationService>();
`

# FluentValidation Agent Skill - FluentValidation 技能

## 技能概述

基于 .NET 10 AOT 编译的高性能验证技能，为 .NET 开发者提供强大的 FluentValidation 功能。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package FluentValidation@11.10.0
`

### 注册服务

在主应用程序中注册 FluentValidation 服务：

`csharp
// 注册 FluentValidation 服务
builder.Services.Configure<FluentValidationOptions>(builder.Configuration.GetSection("FluentValidation"));
builder.Services.AddSingleton<IValidator<TestModel>, TestModelValidator>();
builder.Services.AddSingleton<IFluentValidationService, FluentValidationService>();
builder.Services.AddSingleton<FluentValidationAotEngine>();
`

### 使用示例

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

// 验证模型
var result = await validationService.ValidateTestModelAsync(testModel);
Console.WriteLine($"验证结果: {(result.Success ? "成功" : "失败")}");
Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
`

## 导航地图

`
fluentvalidation/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? fluentvalidation_aot.cs     # FluentValidation AOT 核心实现
    ????? fluentvalidation_aot.run.json  # 运行配置
    ????? fluentvalidation_aot.setting.json  # 设置文件
    ????? fluentvalidation_integration.cs  # FluentValidation 集成
    ????? fv_fluent_validation.cs  # FluentValidation 实现
`

## 主要功能

1. **对象验证**：支持复杂对象的验证，包括必填字段、长度限制、格式验证等
2. **测试模型验证**：提供完整的测试模型验证，包括姓名、年龄、邮箱、手机号、密码等字段
3. **详细的验证结果**：返回详细的验证错误信息，包括错误字段、错误消息等
4. **高性能设计**：AOT 编译优化，减少启动时间和内存占用
5. **易用的命令行接口**：支持多种命令别名，方便使用
6. **可配置的选项**：通过 Options 模式支持灵活配置
7. **依赖注入**：基于 Microsoft.Extensions.DependencyInjection 的服务管理
8. **详细的错误处理**：完善的错误捕获和日志记录

## 扩展说明

此技能提供了完整的验证解决方案，您可以根据需要进行扩展：

1. **自定义验证器**：实现 AbstractValidator<T> 接口，创建自定义验证器
2. **扩展验证规则**：添加新的验证规则和自定义验证逻辑
3. **与其他系统集成**：与其他验证系统或框架集成
4. **性能优化**：针对特定场景优化验证性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理验证器和服务
2. **异步编程**：优先使用异步验证方法，避免阻塞
3. **错误处理**：正确处理验证错误和异常情况
4. **日志记录**：添加适当的日志记录，便于调试和监控
5. **性能监控**：监控验证性能，优化验证逻辑
6. **AOT 优化**：利用 AOT 编译提高性能
7. **模块化设计**：将验证逻辑模块化，便于维护和扩展

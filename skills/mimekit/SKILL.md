# MimeKit 智能体技能 - 邮件处理技能

## 技能概述

基于 .NET 10 的高性能 MimeKit 邮件处理技能实现，为 .NET 开发者提供强大的邮件处理功能。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package MimeKit@4.0.0
#:package MailKit@4.0.0
```

### 注册服务

在主应用程序中注册邮件处理服务：

```csharp
// 注册邮件处理服务
builder.Services.AddEmailProcessingServices();
```

### 使用示例

```csharp
// 获取邮件服务
var emailService = serviceProvider.GetRequiredService<IEmailService>();

// 构建邮件
var message = new MimeMessage();
message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
message.To.Add(new MailboxAddress("收件人", "recipient@example.com"));
message.Subject = "测试邮件";
message.Body = new TextPart(TextFormat.Plain) {
    Text = "这是一封测试邮件"
};

// 发送邮件
await emailService.SendEmailAsync(message);
Console.WriteLine("邮件发送成功");
```

## AOT 架构执行

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### 执行流程

1. **编译阶段**：使用 .NET 10 的 AOT 编译功能将代码编译为本地机器码
2. **打包阶段**：将编译后的代码打包为单文件可执行文件
3. **部署阶段**：将打包后的可执行文件部署到目标环境
4. **运行阶段**：执行单文件可执行文件，处理邮件相关任务

## 导航地图

```
mimekit/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? *.cs                    # 邮件处理核心实现
    ????? *.run.json              # 运行配置
    ????? *.setting.json          # 设置文件
```

## 主要功能

1. **邮件解析**：解析各种格式的邮件
2. **邮件构建**：构建各种类型的邮件
3. **邮件发送**：发送邮件到各种邮件服务器
4. **邮件附件处理**：处理邮件附件
5. **邮件加密/解密**：支持邮件加密和解密
6. **邮件签名/验证**：支持邮件签名和验证
7. **高性能设计**：优化的性能实现
8. **易于使用的 API**：简单直观的 API 设计
9. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的邮件处理解决方案，您可以根据需要进行扩展：

1. **自定义邮件服务**：实现 IEmailService 接口
2. **扩展功能**：添加新的邮件处理功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **邮件格式**：使用标准的邮件格式
7. **附件处理**：合理处理邮件附件
8. **安全性**：确保邮件处理的安全性

## 邮件处理技巧

1. **邮件解析**：使用 MimeKit 解析各种格式的邮件
2. **邮件构建**：使用 MimeKit 构建结构化的邮件
3. **邮件发送**：使用 MailKit 发送邮件
4. **附件处理**：使用 MimeKit 处理各种类型的附件
5. **邮件加密**：使用 S/MIME 或 PGP 加密邮件
6. **邮件签名**：使用 S/MIME 或 PGP 签名邮件
7. **批量处理**：批量处理邮件提高效率
8. **异步处理**：使用异步 API 处理邮件

## 性能优化建议

1. **内存分配优化**：减少不必要的内存分配
2. **GC 压力优化**：减少 GC 触发次数
3. **并发优化**：使用线程安全的邮件处理方案
4. **批处理优化**：批量处理邮件提高效率
5. **缓存使用**：合理使用缓存提高性能
6. **网络传输优化**：优化网络传输中的邮件处理
7. **附件处理优化**：优化大附件的处理
8. **邮件解析优化**：优化邮件解析性能

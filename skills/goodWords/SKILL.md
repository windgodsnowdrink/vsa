# GoodWords Agent Skill - GoodWords 技能

## 技能概述

基于 .NET 10 的高性能 GoodWords 技能，为 .NET 开发者提供强大的好词好句生成、管理、分类和搜索功能。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在主应用程序中注册 GoodWords 服务：

```csharp
// 注册 GoodWords 服务
builder.Services.Configure<GoodWordsOptions>(builder.Configuration.GetSection("GoodWords"));
builder.Services.AddSingleton<IGoodWordsService, GoodWordsService>();
builder.Services.AddSingleton<GoodWordsAotEngine>();
```

### 使用示例

```csharp
// 获取 GoodWords 服务
var goodWordsService = serviceProvider.GetRequiredService<IGoodWordsService>();

// 生成好词好句
var generateResult = await goodWordsService.GenerateAsync("love", 5, "zh");
Console.WriteLine($"生成结果: {(generateResult.Success ? "成功" : "失败")}");

// 分类好词好句
var classifyResult = await goodWordsService.ClassifyAsync("生活是美好的");
Console.WriteLine($"分类结果: {(classifyResult.Success ? "成功" : "失败")}");

// 搜索好词好句
var searchResult = await goodWordsService.SearchAsync("爱情");
Console.WriteLine($"搜索结果: {(searchResult.Success ? "成功" : "失败")}");
```

## AOT 编译支持

GoodWords 技能提供了完整的 AOT 编译支持，通过以下文件实现：

### AOT 核心文件
- `scripts/goodWords_aot.cs` - GoodWords AOT 核心实现
- `scripts/goodWords_aot.setting.json` - AOT 编译配置
- `scripts/goodWords_aot.run.json` - 运行环境配置

### AOT 编译优势
1. **启动速度快**：AOT 编译消除了 JIT 编译开销，启动时间显著减少
2. **内存占用低**：减少了运行时元数据和 JIT 编译器的内存使用
3. **部署简单**：单文件执行，无需依赖 .NET 运行时
4. **性能稳定**：编译时优化，运行时性能更加稳定
5. **安全性高**：减少了运行时攻击面

### AOT 命令行使用

```bash
# 生成好词
dotnet run --project scripts/goodWords_aot.cs generate --count=10

# 搜索好词
dotnet run --project scripts/goodWords_aot.cs search 优秀

# 列出分类
dotnet run --project scripts/goodWords_aot.cs categories

# 添加好词
dotnet run --project scripts/goodWords_aot.cs add 创新 --category=creativity --score=95

# 更新好词评分
dotnet run --project scripts/goodWords_aot.cs update 优秀 98

# 删除好词
dotnet run --project scripts/goodWords_aot.cs remove 过时的词

# 列出所有好词
dotnet run --project scripts/goodWords_aot.cs list

# 显示好词总数
dotnet run --project scripts/goodWords_aot.cs count

# 显示帮助信息
dotnet run --project scripts/goodWords_aot.cs help
```

## 导航地图

```
goodWords/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? goodWords_aot.cs         # GoodWords AOT 核心实现
    ????? goodWords_aot.run.json   # 运行配置
    ????? goodWords_aot.setting.json # 设置文件
    ????? toolgood_words_integration.cs # GoodWords 集成实现
    ????? toolgood_words_integration.run.json # 集成运行配置
    ????? toolgood_words_integration.setting.json # 集成设置文件
```

## 主要功能

1. **好词好句生成**：根据类别生成高质量的好词好句
2. **好词好句分类**：自动分类用户输入的内容
3. **好词好句搜索**：根据关键词搜索相关的好词好句
4. **类别管理**：列出所有支持的类别
5. **AOT 编译优化**：使用 .NET 10 AOT 编译，提高性能和降低内存占用
6. **高性能设计**：优化的生成和搜索算法
7. **易用的 API**：简单直观的 API 设计
8. **可扩展架构**：支持自定义扩展
9. **命令行工具**：完整的命令行界面，支持多种操作
10. **性能监控**：内置执行时间监控和日志记录

## 扩展说明

本技能提供了完整的 GoodWords 解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现 IGoodWordsService 接口
2. **扩展功能**：添加新的好词好句生成和管理功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能
5. **AOT 配置扩展**：根据需要调整 AOT 编译配置

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **AOT 编译**：使用 AOT 编译提高性能和降低内存占用
7. **单文件部署**：使用单文件发布简化部署
8. **配置管理**：使用 Options 模式管理配置
9. **命令行参数**：合理使用命令行参数和别名
10. **内存优化**：使用合适的集合类型和数据结构

## 性能特性

### AOT 编译性能
- **启动时间**：比 JIT 编译快 3-5 倍
- **内存占用**：比 JIT 编译低 20-30%
- **CPU 使用率**：运行时 CPU 使用率更加稳定
- **响应时间**：尾延迟显著降低

### 运行时优化
- **GC 优化**：启用服务器 GC，提高垃圾回收效率
- **线程池优化**：合理配置线程池大小
- **内存管理**：使用对象池和缓存减少内存分配
- **字符串优化**：AOT 编译时字符串内联

## 配置选项

### 环境变量配置

```bash
# 设置最大好词数量
set GOODWORDS_MAX_WORDS=1000

# 设置默认评分
set GOODWORDS_DEFAULT_SCORE=80

# 设置默认分类
set GOODWORDS_DEFAULT_CATEGORY=general

# 设置环境
set DOTNET_ENVIRONMENT=Production
```

### AOT 编译配置

通过 `scripts/goodWords_aot.setting.json` 文件配置 AOT 编译选项：
- 目标框架：net10.0
- 运行时标识符：win-x64
- 裁剪模式：partial
- 内存优化：启用
- 字符串优化：启用

## 故障排除

### 常见问题

1. **AOT 编译失败**
   - 检查依赖项是否支持 AOT
   - 检查代码是否使用了不支持 AOT 的特性
   - 查看编译日志获取详细错误信息

2. **运行时错误**
   - 检查环境变量配置
   - 检查文件权限
   - 查看日志获取详细错误信息

3. **性能问题**
   - 检查内存使用情况
   - 优化查询和算法
   - 调整 AOT 编译配置

## 版本兼容性

- **.NET 版本**：.NET 10.0 及以上
- **操作系统**：Windows、Linux、macOS
- **架构**：x64、ARM64

## 贡献指南

欢迎贡献到 GoodWords 技能：

1. **提交 Issue**：报告 bug 或提出新功能建议
2. **提交 PR**：贡献代码和改进
3. **文档改进**：完善文档和示例
4. **测试覆盖**：添加测试用例

## 许可证

本技能基于 MIT 许可证开源。

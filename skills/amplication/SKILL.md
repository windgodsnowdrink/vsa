# Amplication Agent Skill - 代码生成工具集成

## 技能概览

Amplication是一个强大的低代码开发平台，用于自动化生成高质量的API、数据库和前端代码。本技能实现了与Amplication平台的深度集成，提供了自动化代码生成、数据库设计、前端代码生成等企业级功能，专为快速API开发、微服务架构和低代码平台场景打造。

## 快速入门指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package System.Net.Http.Json@8.0.0
#:package Microsoft.Extensions.Http@8.0.0
#:package System.Text.Json@8.0.0
```

### 注册服务

在主应用程序中注册Amplication服务：

```csharp
// 注册Amplication服务
builder.Services.AddHttpClient<AmplicationService>();
builder.Services.AddSingleton<AmplicationConfig>(new AmplicationConfig
{
    ApiKey = "your-amplication-api-key",
    BaseUrl = "https://api.amplication.com",
    ProjectId = "your-project-id"
});
builder.Services.AddSingleton<AmplicationIntegration>();
```

### 使用示例

#### 初始化Amplication集成

```csharp
// 获取Amplication集成服务
var amplicationIntegration = app.Services.GetRequiredService<AmplicationIntegration>();

// 初始化集成
await amplicationIntegration.InitializeAsync();
```

#### 生成API代码

```csharp
// 创建API生成请求
var apiRequest = new ApiGenerationRequest
{
    ProjectId = "your-project-id",
    EntityName = "Product",
    Properties = new List<EntityProperty>
    {
        new EntityProperty { Name = "Name", Type = "string", Required = true },
        new EntityProperty { Name = "Description", Type = "string" },
        new EntityProperty { Name = "Price", Type = "decimal", Required = true },
        new EntityProperty { Name = "CreatedAt", Type = "datetime" }
    },
    ApiType = ApiType.Rest,
    DatabaseType = DatabaseType.MongoDb
};

// 生成API代码
var result = await amplicationIntegration.GenerateApiAsync(apiRequest);

// 处理生成结果
if (result.Success)
{
    Console.WriteLine("API代码生成成功！");
    Console.WriteLine($"生成的API端点: {result.ApiEndpoints.Count} 个");
    Console.WriteLine($"生成的数据库模型: {result.DatabaseModels.Count} 个");
}
else
{
    Console.WriteLine($"API代码生成失败: {result.ErrorMessage}");
}
```

#### 生成前端代码

```csharp
// 创建前端代码生成请求
var frontendRequest = new FrontendGenerationRequest
{
    ProjectId = "your-project-id",
    Framework = FrontendFramework.React,
    Components = new List<FrontendComponent>
    {
        FrontendComponent.ListView,
        FrontendComponent.Form,
        FrontendComponent.DetailView
    },
    ApiEndpoints = result.ApiEndpoints
};

// 生成前端代码
var frontendResult = await amplicationIntegration.GenerateFrontendAsync(frontendRequest);

if (frontendResult.Success)
{
    Console.WriteLine("前端代码生成成功！");
    Console.WriteLine($"生成的组件: {frontendResult.Components.Count} 个");
    Console.WriteLine($"生成的页面: {frontendResult.Pages.Count} 个");
}
```

## 导航地图

```
amplication/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点 (当前文件)
├── reference/                  # 引用文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── amplication_integration.cs # Amplication集成脚本
    └── ...                    # 其他相关脚本
```

## 主要功能

1. **Amplication项目集成**：
   - 连接到Amplication平台
   - 管理Amplication项目
   - 集成Amplication工作流
   - 支持多个Amplication项目

2. **API自动生成**：
   - REST API生成
   - GraphQL API生成
   - API端点自动测试
   - API文档生成
   - 支持多种认证方式

3. **数据库设计与迁移**：
   - 支持多种数据库（MongoDB、PostgreSQL、MySQL等）
   - 自动生成数据库模型
   - 数据库迁移脚本生成
   - 数据库索引优化
   - 数据种子生成

4. **前端代码生成**：
   - 支持多种前端框架（React、Angular、Vue等）
   - 自动生成CRUD组件
   - 表单生成
   - 列表视图生成
   - 详情视图生成
   - 状态管理集成

5. **微服务架构支持**：
   - 微服务自动生成
   - API网关生成
   - 服务发现集成
   - 分布式跟踪支持
   - 负载均衡配置

6. **插件系统集成**：
   - 支持Amplication插件
   - 自定义代码生成模板
   - 插件开发框架
   - 插件市场集成

7. **CI/CD流水线生成**：
   - GitHub Actions流水线生成
   - GitLab CI流水线生成
   - Jenkins流水线生成
   - Docker配置生成
   - Kubernetes配置生成

8. **代码质量检查**：
   - 自动代码格式化
   - 代码质量分析
   - 安全漏洞扫描
   - 性能优化建议
   - 最佳实践检查

## 扩展说明

本技能提供了基础的Amplication集成实现，您可以根据需要扩展：

1. **添加更多数据库支持**：扩展支持其他数据库类型
2. **添加更多前端框架支持**：扩展支持其他前端框架
3. **实现自定义代码生成模板**：创建符合自己团队规范的代码模板
4. **添加更多API类型支持**：扩展支持其他API类型
5. **实现更复杂的CI/CD流水线**：创建更复杂的CI/CD配置
6. **添加监控和告警**：实现Amplication集成的监控和告警功能

## 最佳实践

1. **项目设计原则**：
   - 保持实体设计简洁明了
   - 合理设计关系和索引
   - 考虑性能和扩展性
   - 遵循领域驱动设计原则

2. **API设计原则**：
   - 遵循RESTful设计原则
   - 合理使用HTTP方法
   - 提供良好的API文档
   - 实现适当的认证和授权
   - 考虑API版本管理

3. **前端代码生成**：
   - 选择适合项目的前端框架
   - 合理设计组件结构
   - 考虑状态管理方案
   - 实现响应式设计
   - 考虑性能优化

4. **微服务架构**：
   - 合理划分微服务边界
   - 实现适当的服务间通信
   - 考虑分布式事务处理
   - 实现服务发现和负载均衡
   - 考虑监控和告警

5. **CI/CD流水线**：
   - 实现自动化测试
   - 实现自动化部署
   - 考虑回滚策略
   - 实现适当的监控和告警
   - 考虑安全性

## 技术特性

- **高性能设计**：
  - 异步API调用
  - 批量代码生成
  - 内存优化设计
  - 并行处理支持

- **可靠性设计**：
  - 完整的错误处理机制
  - 详细的日志记录
  - 提供诊断指标
  - 支持监控和告警
  - 实现优雅降级

- **易用性设计**：
  - 提供简洁的API
  - 详细的文档和示例
  - 支持多种配置方式
  - 模块化设计，易于扩展
  - 与.NET生态系统良好集成

- **安全性设计**：
  - 实现安全的API认证
  - 支持加密通信
  - 提供安全的配置选项
  - 实现输入验证和输出编码

## 监控和诊断

Amplication集成提供了丰富的监控指标和诊断信息：

- **API调用指标**：
  - API调用次数和成功率
  - API响应时间分布
  - API错误率和错误类型
  - API调用频率

- **代码生成指标**：
  - 代码生成次数和成功率
  - 生成的代码行数和文件数
  - 代码生成时间分布
  - 代码生成错误率

- **项目管理指标**：
  - 项目数量和活跃度
  - 实体和属性数量
  - API端点数量
  - 数据库模型数量

- **诊断信息**：
  - 详细的错误日志
  - API请求和响应日志
  - 代码生成过程日志
  - 配置验证结果

## 部署建议

1. **开发环境**：
   - 本地安装Amplication CLI
   - 使用Amplication云服务
   - 配置开发数据库
   - 设置开发API密钥

2. **测试环境**：
   - 使用Amplication测试环境
   - 配置测试数据库
   - 设置测试API密钥
   - 实现自动化测试

3. **生产环境**：
   - 使用Amplication生产环境
   - 配置生产数据库
   - 设置生产API密钥
   - 实现高可用性配置
   - 实现监控和告警

4. **CI/CD集成**：
   - 集成到现有的CI/CD流水线
   - 实现自动化代码生成
   - 实现自动化测试
   - 实现自动化部署

## 版本管理

- **1.0.0**：初始版本，包含基础Amplication集成
- **1.1.0**：添加API自动生成功能
- **1.2.0**：添加数据库设计与迁移功能
- **1.3.0**：添加前端代码生成功能
- **1.4.0**：添加微服务架构支持
- **1.5.0**：添加CI/CD流水线生成功能
- **2.0.0**：支持.NET 10和AOT编译

## 贡献指南

欢迎提交Issue和Pull Request来改进Amplication集成：

1. Fork项目仓库
2. 创建功能分支
3. 实现新功能或修复Bug
4. 编写测试用例
5. 提交Pull Request
6. 代码审查和合并

## 许可证

本项目采用MIT许可证，详见LICENSE文件。

## 联系方式

- 项目地址：[GitHub Repository]
- 文档地址：[Documentation]
- 问题反馈：[Issues]
- 贡献指南：[Contributing Guide]
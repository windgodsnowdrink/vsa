# Tye参考文档

## 1. 概述

本文档提供了Tye服务管理工具的详细参考信息，包括API接口、配置选项、命令行参数和使用示例。Tye是一个.NET服务管理工具，专为微服务和分布式应用开发设计，提供了本地开发、Kubernetes部署和服务监控的完整解决方案。

## 2. 核心概念

### 2.1 服务管理

Tye的核心概念是服务管理，它允许您：

- 定义和管理多个服务
- 处理服务间的依赖关系
- 自动配置服务发现
- 监控服务健康状态
- 管理服务生命周期

### 2.2 配置模型

Tye使用`tye.yaml`文件来定义服务配置，包括：

- 服务名称和类型
- 项目路径或容器镜像
- 绑定端口和协议
- 环境变量
- 依赖关系
- 资源限制

### 2.3 部署模型

Tye支持两种主要的部署模型：

- **本地开发**：在本地机器上运行和管理服务
- **Kubernetes**：将服务部署到Kubernetes集群

## 3. API参考

### 3.1 命令行接口

#### 3.1.1 基本命令

| 命令 | 描述 | 参数 |
|------|------|------|
| `tye run` | 启动本地开发环境 | `--services`：指定服务<br>`--dashboard`：启用仪表板<br>`--port`：仪表板端口<br>`--verbosity`：日志级别 |
| `tye deploy` | 部署到Kubernetes | `--namespace`：命名空间<br>`--context`：Kubernetes上下文<br>`--interactive`：交互式模式 |
| `tye undeploy` | 从Kubernetes删除部署 | `--namespace`：命名空间<br>`--context`：Kubernetes上下文 |
| `tye build` | 构建服务 | `--push`：推送镜像<br>`--registry`：镜像仓库<br>`--tag`：镜像标签 |
| `tye logs` | 查看服务日志 | `--service`：服务名称<br>`--follow`：持续跟踪<br>`--tail`：显示最后N行 |
| `tye init` | 初始化Tye配置 | `--force`：覆盖现有配置 |
| `tye ps` | 查看运行中的服务 | - |
| `tye stop` | 停止运行中的服务 | - |
| `tye help` | 显示帮助信息 | `[command]`：命令名称 |
| `tye version` | 显示版本信息 | - |

#### 3.1.2 全局选项

| 选项 | 描述 | 默认值 |
|------|------|--------|
| `--help` | 显示帮助信息 | - |
| `--version` | 显示版本信息 | - |
| `--verbosity` | 设置日志级别 | `info` |
| `--non-interactive` | 非交互式模式 | `false` |

### 3.2 配置文件格式

#### 3.2.1 基本结构

```yaml
name: <应用名称>
services:
  - name: <服务名称>
    project: <项目路径> | image: <容器镜像>
    bindings:
      - port: <端口>
        protocol: <协议>
        connectionString: <连接字符串>
    env:
      - name: <环境变量名称>
        value: <环境变量值>
    replicas: <副本数量>
    resources:
      cpu: <CPU限制>
      memory: <内存限制>
    labels:
      <标签键>: <标签值>
    annotations:
      <注解键>: <注解值>
```

#### 3.2.2 服务类型

| 类型 | 描述 | 配置方式 |
|------|------|----------|
| **ASP.NET Core** | Web应用服务 | 使用`project`指定.csproj文件 |
| **Worker** | 后台工作服务 | 使用`project`指定.csproj文件 |
| **gRPC** | gRPC服务 | 使用`project`指定.csproj文件 |
| **External** | 外部服务（如数据库） | 使用`image`指定容器镜像 |

### 3.3 环境变量

#### 3.3.1 内置环境变量

| 变量名 | 描述 | 示例值 |
|--------|------|--------|
| `TYE_DASHBOARD_PORT` | 仪表板端口 | `8000` |
| `TYE_KUBE_CONFIG` | Kubernetes配置文件路径 | `~/.kube/config` |
| `TYE_NAMESPACE` | 默认Kubernetes命名空间 | `default` |
| `TYE_REGISTRY` | 容器镜像仓库 | `myregistry.azurecr.io` |
| `TYE_PROJECT_ROOT` | 项目根目录 | `.` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core环境 | `Development` |
| `DOTNET_ENVIRONMENT` | .NET环境 | `Development` |

#### 3.3.2 服务特定环境变量

Tye会为每个服务自动设置以下环境变量：

| 变量名 | 描述 | 示例值 |
|--------|------|--------|
| `<SERVICE_NAME>_HOST` | 服务主机名 | `localhost` |
| `<SERVICE_NAME>_PORT` | 服务端口 | `5000` |
| `<SERVICE_NAME>_PROTOCOL` | 服务协议 | `http` |
| `<SERVICE_NAME>_URI` | 服务完整URI | `http://localhost:5000` |

## 4. 服务发现

### 4.1 本地开发环境

在本地开发环境中，Tye使用以下机制进行服务发现：

1. **环境变量**：为每个服务设置环境变量
2. **配置注入**：将服务配置注入到应用配置中
3. **DNS解析**：在本地网络中解析服务名称

### 4.2 Kubernetes环境

在Kubernetes环境中，Tye利用Kubernetes内置的服务发现机制：

1. **服务DNS**：使用Kubernetes服务DNS名称
2. **环境变量**：利用Kubernetes自动注入的环境变量
3. **配置映射**：使用Kubernetes配置映射管理配置

## 5. 仪表板功能

### 5.1 访问方式

启动Tye后，可以通过以下地址访问仪表板：

```
http://localhost:8000
```

### 5.2 功能模块

| 模块 | 描述 | 功能 |
|------|------|------|
| **服务列表** | 显示所有服务状态 | 查看服务状态、地址、端口 |
| **服务详情** | 显示单个服务详情 | 查看服务配置、日志、健康状态 |
| **拓扑图** | 可视化服务依赖关系 | 查看服务间调用关系 |
| **日志聚合** | 聚合所有服务日志 | 查看、过滤、搜索日志 |
| **健康检查** | 显示服务健康状态 | 查看服务健康检查结果 |
| **资源使用** | 显示资源使用情况 | 查看CPU和内存使用 |

## 6. 配置管理

### 6.1 配置源

Tye支持以下配置源：

1. **tye.yaml**：主要配置文件
2. **环境变量**：系统环境变量
3. **命令行参数**：运行时命令行参数
4. **应用配置文件**：appsettings.json等

### 6.2 配置优先级

配置优先级从高到低：

1. 命令行参数
2. 环境变量
3. tye.yaml文件
4. 应用配置文件
5. 默认配置

### 6.3 密钥管理

对于敏感配置，推荐使用以下方法：

1. **环境变量**：在部署环境中设置环境变量
2. **Kubernetes密钥**：在Kubernetes中使用密钥
3. **Azure Key Vault**：使用Azure Key Vault存储密钥

## 7. 部署管理

### 7.1 本地开发部署

#### 7.1.1 基本流程

1. **初始化配置**：运行`tye init`生成配置文件
2. **修改配置**：编辑`tye.yaml`文件，添加服务和配置
3. **启动服务**：运行`tye run`启动所有服务
4. **监控服务**：访问仪表板监控服务状态
5. **停止服务**：按Ctrl+C停止所有服务

#### 7.1.2 高级选项

- **指定服务**：`tye run --services service1,service2`
- **禁用仪表板**：`tye run --dashboard false`
- **自定义端口**：`tye run --port 9000`
- **详细日志**：`tye run --verbosity debug`

### 7.2 Kubernetes部署

#### 7.2.1 基本流程

1. **构建镜像**：运行`tye build --push`构建并推送镜像
2. **部署应用**：运行`tye deploy`部署到Kubernetes
3. **验证部署**：使用`kubectl get pods`验证部署状态
4. **访问服务**：使用Kubernetes服务地址访问应用
5. **删除部署**：运行`tye undeploy`删除部署

#### 7.2.2 高级选项

- **指定命名空间**：`tye deploy --namespace myapp`
- **指定上下文**：`tye deploy --context mycluster`
- **交互式部署**：`tye deploy --interactive`
- **自定义镜像仓库**：`tye build --registry myregistry.azurecr.io`

## 8. 日志管理

### 8.1 查看日志

#### 8.1.1 基本用法

```bash
# 查看所有服务日志
tye logs

# 查看特定服务日志
tye logs --service web

# 持续跟踪日志
tye logs --follow

# 查看最后100行日志
tye logs --tail 100
```

#### 8.1.2 日志级别

Tye支持以下日志级别：

| 级别 | 描述 |
|------|------|
| `debug` | 详细调试信息 |
| `info` | 一般信息 |
| `warn` | 警告信息 |
| `error` | 错误信息 |
| `critical` | 严重错误信息 |

### 8.2 日志格式

Tye的日志格式如下：

```
[2024-12-24 10:30:45] [web] [Information] Application started. Press Ctrl+C to shut down.
```

## 9. 健康检查

### 9.1 内置健康检查

Tye会自动检测以下类型的健康检查：

1. **ASP.NET Core健康检查**：使用`Microsoft.Extensions.Diagnostics.HealthChecks`
2. **gRPC健康检查**：使用gRPC健康检查协议
3. **进程健康**：检查服务进程是否运行

### 9.2 自定义健康检查

对于ASP.NET Core应用，可以添加自定义健康检查：

```csharp
// 在Startup.cs中
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddCheck("database", new SqlConnectionHealthCheck(connectionString))
        .AddCheck("redis", new RedisHealthCheck(redisConnectionString));
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHealthChecks("/health");
}
```

## 10. 资源管理

### 10.1 本地开发环境

在本地开发环境中，Tye会：

- 自动分配端口，避免端口冲突
- 监控服务资源使用情况
- 提供资源使用统计信息

### 10.2 Kubernetes环境

在Kubernetes环境中，Tye支持以下资源配置：

| 配置项 | 描述 | 示例值 |
|--------|------|--------|
| `cpu_request` | 请求的CPU数量 | `100m` |
| `cpu_limit` | CPU限制 | `500m` |
| `memory_request` | 请求的内存数量 | `256Mi` |
| `memory_limit` | 内存限制 | `512Mi` |

## 11. 常见问题

### 11.1 配置问题

**Q: 如何配置服务的环境变量？**
A: 在tye.yaml文件中，为服务添加env部分：

```yaml
services:
  - name: web
    project: Web/Web.csproj
    env:
      - name: ASPNETCORE_ENVIRONMENT
        value: Development
      - name: ConnectionStrings__DefaultConnection
        value: Server=localhost;Database=myapp;Integrated Security=True;
```

**Q: 如何配置服务的端口？**
A: 在tye.yaml文件中，为服务添加bindings部分：

```yaml
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
      - port: 8081
        protocol: https
```

### 11.2 部署问题

**Q: 如何部署到特定的Kubernetes命名空间？**
A: 使用--namespace参数：

```bash
tye deploy --namespace myapp
```

**Q: 如何推送镜像到私有仓库？**
A: 使用--push和--registry参数：

```bash
tye build --push --registry myregistry.azurecr.io
```

### 11.3 服务发现问题

**Q: 服务如何发现其他服务？**
A: Tye会为每个服务设置环境变量，例如：

```csharp
var apiUrl = Environment.GetEnvironmentVariable("API_SERVICE_URI") ?? "http://localhost:5001";
```

**Q: 如何在配置文件中引用其他服务？**
A: 使用服务名称作为占位符：

```yaml
services:
  - name: web
    project: Web/Web.csproj
    env:
      - name: ApiUrl
        value: ${API_SERVICE_URI}
```

### 11.4 性能问题

**Q: 如何优化Tye的性能？**
A: 可以采取以下措施：

1. 减少服务数量，合并相关服务
2. 优化服务启动时间
3. 减少日志输出量
4. 使用合适的资源限制
5. 避免不必要的健康检查

## 12. 最佳实践

### 12.1 配置管理

- **使用环境变量**：对于敏感配置，使用环境变量而不是硬编码
- **分层配置**：为不同环境使用不同的配置文件
- **配置验证**：在应用启动时验证配置有效性
- **配置监控**：监控配置变更和使用情况

### 12.2 服务设计

- **单一职责**：每个服务只负责一个功能领域
- **健康检查**：为每个服务添加健康检查端点
- **错误处理**：实现统一的错误处理机制
- **日志记录**：使用结构化日志，便于分析和监控
- **重试机制**：实现服务间调用的重试和超时机制

### 12.3 部署策略

- **蓝绿部署**：使用蓝绿部署减少 downtime
- **金丝雀发布**：逐步将流量转移到新版本
- **滚动更新**：使用Kubernetes滚动更新机制
- **回滚策略**：准备回滚方案，以便在出现问题时快速回滚
- **监控告警**：设置部署监控和告警机制

### 12.4 监控和日志

- **集中日志**：使用ELK或类似系统集中管理日志
- **分布式追踪**：实现分布式追踪，便于排查问题
- **指标监控**：监控服务性能和资源使用情况
- **告警机制**：设置合理的告警阈值和规则
- **可视化**：使用Grafana等工具可视化监控数据

## 13. 命令行示例

### 13.1 基本操作

```bash
# 初始化配置
tye init

# 启动服务
tye run

# 查看运行状态
tye ps

# 查看日志
tye logs

# 停止服务
tye stop
```

### 13.2 部署操作

```bash
# 构建并推送镜像
tye build --push --registry myregistry.azurecr.io

# 部署到Kubernetes
tye deploy --namespace myapp

# 查看Kubernetes部署状态
kubectl get pods -n myapp

# 从Kubernetes删除部署
tye undeploy --namespace myapp
```

### 13.3 高级操作

```bash
# 启动特定服务
tye run --services web,api

# 禁用仪表板启动
tye run --dashboard false

# 详细日志输出
tye run --verbosity debug

# 持续跟踪特定服务日志
tye logs --service web --follow

# 构建特定版本的镜像
tye build --push --registry myregistry.azurecr.io --tag v1.0.0
```

## 14. 配置文件示例

### 14.1 基本配置

```yaml
name: myapp
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
  - name: api
    project: Api/Api.csproj
    bindings:
      - port: 8081
        protocol: http
  - name: db
    image: mcr.microsoft.com/mssql/server:2022-latest
    bindings:
      - port: 1433
        connectionString: Server=${host},${port};Database=myapp;User Id=sa;Password=YourStrong!Passw0rd;
    env:
      - name: SA_PASSWORD
        value: YourStrong!Passw0rd
      - name: ACCEPT_EULA
        value: Y
```

### 14.2 高级配置

```yaml
name: myapp
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
      - port: 8081
        protocol: https
    env:
      - name: ASPNETCORE_ENVIRONMENT
        value: Development
      - name: ApiUrl
        value: ${API_SERVICE_URI}
    replicas: 2
    resources:
      cpu: 500m
      memory: 512Mi
  - name: api
    project: Api/Api.csproj
    bindings:
      - port: 8082
        protocol: http
    env:
      - name: DatabaseConnectionString
        value: ${DB_SERVICE_CONNECTIONSTRING}
    replicas: 3
    resources:
      cpu: 300m
      memory: 256Mi
  - name: db
    image: postgres:14-alpine
    bindings:
      - port: 5432
        connectionString: Host=${host};Port=${port};Database=myapp;Username=postgres;Password=postgres;
    env:
      - name: POSTGRES_USER
        value: postgres
      - name: POSTGRES_PASSWORD
        value: postgres
      - name: POSTGRES_DB
        value: myapp
    resources:
      cpu: 200m
      memory: 128Mi
```

## 15. 故障排除

### 15.1 常见错误

| 错误消息 | 可能原因 | 解决方案 |
|---------|---------|--------|
| `Port already in use` | 端口被占用 | 修改服务绑定端口 |
| `Service not found` | 服务未启动或名称错误 | 检查服务名称和状态 |
| `Connection refused` | 服务未就绪或网络问题 | 检查服务健康状态和网络连接 |
| `Image pull failed` | 镜像仓库认证失败或镜像不存在 | 检查镜像仓库认证和镜像名称 |
| `Deployment failed` | Kubernetes配置错误或资源不足 | 检查Kubernetes配置和集群资源 |
| `Health check failed` | 服务健康检查失败 | 检查服务日志和健康检查实现 |

### 15.2 诊断工具

| 工具 | 描述 | 用法 |
|------|------|------|
| `tye logs` | 查看服务日志 | `tye logs --service <service-name>` |
| `kubectl get` | 查看Kubernetes资源 | `kubectl get pods -n <namespace>` |
| `kubectl describe` | 查看Kubernetes资源详情 | `kubectl describe pod <pod-name> -n <namespace>` |
| `kubectl logs` | 查看Kubernetes容器日志 | `kubectl logs <pod-name> -n <namespace>` |
| `netstat` | 查看网络连接 | `netstat -ano | findstr <port>` |
| `docker ps` | 查看Docker容器 | `docker ps` |
| `docker logs` | 查看Docker容器日志 | `docker logs <container-id>` |

### 15.3 故障排除流程

1. **检查服务状态**：运行`tye ps`查看服务状态
2. **查看服务日志**：运行`tye logs --service <service-name>`查看日志
3. **检查网络连接**：验证服务间网络连接是否正常
4. **验证配置**：检查tye.yaml配置文件是否正确
5. **检查依赖**：验证服务依赖是否满足
6. **检查资源**：验证系统资源是否充足
7. **重启服务**：尝试重启有问题的服务
8. **重置环境**：如果问题持续，尝试重置开发环境

## 16. 总结

Tye是一个强大的.NET服务管理工具，为微服务和分布式应用开发提供了完整的解决方案。通过本文档，您应该了解了Tye的核心概念、API接口、配置选项和使用方法。

Tye的主要优势包括：

- **简化开发**：自动处理服务发现、网络配置和依赖管理
- **一致体验**：在本地开发和Kubernetes部署之间提供一致的体验
- **强大工具**：提供命令行工具和Web仪表板，便于管理和监控服务
- **灵活配置**：支持多种配置方式，适应不同的开发和部署场景
- **集成生态**：与.NET生态系统紧密集成，支持主流的.NET技术栈

通过使用Tye，您可以更专注于业务逻辑的实现，而不是环境配置和部署管理的复杂性，从而提高开发效率和部署可靠性。
# Tye使用示例

## 1. 基本使用示例

### 1.1 初始化项目

**场景**：开始一个新的Tye项目

**操作步骤**：

1. 创建一个新的目录并进入
2. 初始化Tye配置
3. 查看生成的配置文件

**示例代码**：

```bash
# 创建项目目录
mkdir my-tye-project
cd my-tye-project

# 初始化Tye配置
tye init

# 查看生成的配置文件
cat tye.yaml
```

**生成的配置文件**：

```yaml
name: my-tye-project
services:
```

### 1.2 添加服务

**场景**：向Tye项目添加ASP.NET Core Web服务和Worker服务

**操作步骤**：

1. 创建ASP.NET Core Web应用
2. 创建Worker服务
3. 更新tye.yaml配置文件

**示例代码**：

```bash
# 创建ASP.NET Core Web应用
dotnet new web -n WebApp

# 创建Worker服务
dotnet new worker -n WorkerService

# 更新tye.yaml配置
cat > tye.yaml << 'EOF'
name: my-tye-project
services:
  - name: webapp
    project: WebApp/WebApp.csproj
    bindings:
      - port: 8080
        protocol: http
  - name: worker
    project: WorkerService/WorkerService.csproj
EOF
```

### 1.3 启动服务

**场景**：启动所有服务并查看状态

**操作步骤**：

1. 启动Tye
2. 访问仪表板
3. 查看服务状态

**示例代码**：

```bash
# 启动Tye
tye run

# 查看服务状态（在另一个终端）
tye ps
```

**预期输出**：

```
查看 http://localhost:8000 了解详细信息
[webapp] 正在运行，地址: http://localhost:8080
[worker] 正在运行
```

## 2. 高级配置示例

### 2.1 多服务依赖

**场景**：配置Web服务依赖于API服务，API服务依赖于数据库服务

**配置文件**：

```yaml
name: microservices-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: ApiBaseUrl
        value: ${API_SERVICE_URI}
  - name: api
    project: Api/Api.csproj
    bindings:
      - port: 8081
        protocol: http
    env:
      - name: DatabaseConnectionString
        value: ${DB_SERVICE_CONNECTIONSTRING}
  - name: db
    image: mcr.microsoft.com/mssql/server:2022-latest
    bindings:
      - port: 1433
        connectionString: Server=${host},${port};Database=DemoDb;User Id=sa;Password=YourStrong!Passw0rd;
    env:
      - name: SA_PASSWORD
        value: YourStrong!Passw0rd
      - name: ACCEPT_EULA
        value: Y
```

### 2.2 资源限制配置

**场景**：为服务配置CPU和内存限制

**配置文件**：

```yaml
name: resource-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    replicas: 2
    resources:
      cpu: 500m
      memory: 512Mi
  - name: api
    project: Api/Api.csproj
    bindings:
      - port: 8081
        protocol: http
    replicas: 3
    resources:
      cpu: 300m
      memory: 256Mi
```

### 2.3 HTTPS配置

**场景**：为服务配置HTTPS

**配置文件**：

```yaml
name: https-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
      - port: 8081
        protocol: https
    env:
      - name: ASPNETCORE_URLS
        value: http://*:8080;https://*:8081
      - name: ASPNETCORE_Kestrel__Certificates__Default__Password
        value: password
      - name: ASPNETCORE_Kestrel__Certificates__Default__Path
        value: ./certificate.pfx
```

## 3. 服务发现示例

### 3.1 环境变量使用

**场景**：在代码中使用环境变量进行服务发现

**示例代码**：

```csharp
// 在Web服务中调用API服务
public class WeatherForecastController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public WeatherForecastController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiBaseUrl = Environment.GetEnvironmentVariable("API_SERVICE_URI") ?? "http://localhost:8081";
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await _httpClient.GetAsync($"{_apiBaseUrl}/weatherforecast");
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            return Ok(data);
        }
        return StatusCode((int)response.StatusCode);
    }
}
```

### 3.2 配置文件使用

**场景**：在配置文件中使用服务发现占位符

**appsettings.json**：

```json
{
  "ApiSettings": {
    "BaseUrl": "${API_SERVICE_URI}",
    "Timeout": 30
  },
  "Database": {
    "ConnectionString": "${DB_SERVICE_CONNECTIONSTRING}"
  }
}
```

**使用代码**：

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.Configure<ApiSettings>(Configuration.GetSection("ApiSettings"));
    }
}

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _settings;

    public ApiService(HttpClient httpClient, IOptions<ApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<string> GetDataAsync()
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Timeout);
        var response = await _httpClient.GetAsync($"{_settings.BaseUrl}/data");
        return await response.Content.ReadAsStringAsync();
    }
}
```

## 4. 数据库集成示例

### 4.1 SQL Server

**场景**：配置SQL Server数据库服务

**配置文件**：

```yaml
name: sql-server-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: ConnectionStrings__DefaultConnection
        value: ${DB_SERVICE_CONNECTIONSTRING}
  - name: db
    image: mcr.microsoft.com/mssql/server:2022-latest
    bindings:
      - port: 1433
        connectionString: Server=${host},${port};Database=MyDb;User Id=sa;Password=YourStrong!Passw0rd;
    env:
      - name: SA_PASSWORD
        value: YourStrong!Passw0rd
      - name: ACCEPT_EULA
        value: Y
```

### 4.2 PostgreSQL

**场景**：配置PostgreSQL数据库服务

**配置文件**：

```yaml
name: postgres-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: ConnectionStrings__DefaultConnection
        value: ${DB_SERVICE_CONNECTIONSTRING}
  - name: db
    image: postgres:14-alpine
    bindings:
      - port: 5432
        connectionString: Host=${host};Port=${port};Database=MyDb;Username=postgres;Password=postgres;
    env:
      - name: POSTGRES_USER
        value: postgres
      - name: POSTGRES_PASSWORD
        value: postgres
      - name: POSTGRES_DB
        value: MyDb
```

### 4.3 Redis

**场景**：配置Redis缓存服务

**配置文件**：

```yaml
name: redis-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: RedisConnectionString
        value: ${REDIS_SERVICE_HOST}:${REDIS_SERVICE_PORT}
  - name: redis
    image: redis:7-alpine
    bindings:
      - port: 6379
        protocol: redis
```

## 5. Kubernetes部署示例

### 5.1 基本部署

**场景**：构建并部署应用到Kubernetes

**操作步骤**：

1. 构建镜像
2. 部署到Kubernetes
3. 验证部署

**示例代码**：

```bash
# 构建镜像并推送
tye build --push --registry myregistry.azurecr.io

# 部署到Kubernetes
tye deploy --namespace myapp

# 验证部署
kubectl get pods -n myapp
kubectl get services -n myapp
```

### 5.2 高级Kubernetes配置

**场景**：配置Kubernetes特定的资源和标签

**配置文件**：

```yaml
name: k8s-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    replicas: 3
    resources:
      cpu: 500m
      memory: 512Mi
    labels:
      app: web
      environment: production
      tier: frontend
    annotations:
      description: "Web前端服务"
      version: "1.0.0"
  - name: api
    project: Api/Api.csproj
    bindings:
      - port: 8081
        protocol: http
    replicas: 2
    resources:
      cpu: 300m
      memory: 256Mi
    labels:
      app: api
      environment: production
      tier: backend
```

### 5.3  ingress配置

**场景**：配置Kubernetes Ingress以暴露服务

**tye.yaml**：

```yaml
name: ingress-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    kubernetes:
      ingress:
        - name: web-ingress
          host: web.example.com
          paths:
            - path: /
              pathType: Prefix
              backend:
                service:
                  name: web
                  port:
                    number: 8080
```

## 6. 日志管理示例

### 6.1 查看服务日志

**场景**：查看特定服务的日志

**示例代码**：

```bash
# 查看所有服务日志
tye logs

# 查看特定服务日志
tye logs --service web

# 持续跟踪日志
tye logs --follow

# 跟踪特定服务日志
tye logs --service web --follow

# 查看最后100行日志
tye logs --tail 100

# 查看特定服务的最后50行日志
tye logs --service api --tail 50
```

### 6.2 结构化日志

**场景**：在服务中使用结构化日志

**示例代码**：

```csharp
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("获取天气预报数据");
        
        try
        {
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();
            
            _logger.LogInformation("成功获取天气预报数据，共 {Count} 条", forecasts.Length);
            return forecasts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取天气预报数据时出错");
            throw;
        }
    }
}
```

## 7. 健康检查示例

### 7.1 基本健康检查

**场景**：为ASP.NET Core应用添加健康检查

**代码**：

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHealthChecks("/health");
    }
}
```

### 7.2 高级健康检查

**场景**：添加数据库和Redis健康检查

**代码**：

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        var redisConnection = Configuration.GetConnectionString("RedisConnection");

        services.AddHealthChecks()
            .AddSqlServer(connectionString, name: "database")
            .AddRedis(redisConnection, name: "redis")
            .AddCheck<CustomHealthCheck>("custom");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var result = JsonSerializer.Serialize(
                    new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration.ToString()
                        }),
                        totalDuration = report.TotalDuration.ToString()
                    });
                
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
        });
    }
}

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // 自定义健康检查逻辑
        var isHealthy = true; // 实际检查逻辑
        
        if (isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("自定义检查通过"));
        }
        
        return Task.FromResult(HealthCheckResult.Unhealthy("自定义检查失败"));
    }
}
```

## 8. 持续集成/持续部署示例

### 8.1 GitHub Actions

**场景**：使用GitHub Actions自动化构建和部署

**.github/workflows/ci-cd.yml**：

```yaml
name: CI/CD

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    
    - name: Install dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-restore --verbosity normal

  deploy:
    needs: build
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    
    - name: Install Tye
      run: dotnet tool install -g Microsoft.Tye
    
    - name: Build and Deploy
      run: |
        tye build --push --registry ${{ secrets.DOCKER_REGISTRY }}
        tye deploy --namespace myapp
      env:
        DOCKER_USERNAME: ${{ secrets.DOCKER_USERNAME }}
        DOCKER_PASSWORD: ${{ secrets.DOCKER_PASSWORD }}
        KUBE_CONFIG: ${{ secrets.KUBE_CONFIG }}
```

### 8.2 Azure DevOps Pipeline

**场景**：使用Azure DevOps Pipeline自动化构建和部署

**azure-pipelines.yml**：

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dockerRegistryServiceConnection: 'docker-registry-connection'
  imageRepository: 'myapp'
  containerRegistry: 'myregistry.azurecr.io'
  kubernetesNamespace: 'myapp'

steps:
- task: UseDotNet@2
  inputs:
    version: '10.0.x'
    includePreviewVersions: true

- script: dotnet restore
  displayName: 'Restore dependencies'

- script: dotnet build --configuration $(buildConfiguration)
  displayName: 'Build application'

- script: dotnet test --configuration $(buildConfiguration)
  displayName: 'Run tests'

- task: DockerInstaller@0
  displayName: 'Install Docker CLI'

- task: DockerLogin@1
  displayName: 'Login to Docker Registry'
  inputs:
    containerRegistryType: 'Container Registry'
    dockerRegistryEndpoint: '$(dockerRegistryServiceConnection)'

- script: |
    dotnet tool install -g Microsoft.Tye
    tye build --push --registry $(containerRegistry)
  displayName: 'Build and push images'

- task: KubernetesManifest@0
  displayName: 'Deploy to Kubernetes'
  inputs:
    action: 'deploy'
    kubernetesServiceConnection: 'kubernetes-connection'
    namespace: '$(kubernetesNamespace)'
    manifests: |
      kubernetes/deployment.yaml
      kubernetes/service.yaml
      kubernetes/ingress.yaml
```

## 9. 性能优化示例

### 9.1 资源限制

**场景**：配置服务的资源限制以优化性能

**配置文件**：

```yaml
name: performance-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    replicas: 2
    resources:
      cpu: 1000m
      memory: 1Gi
  - name: api
    project: Api/Api.csproj
    bindings:
      - port: 8081
        protocol: http
    replicas: 4
    resources:
      cpu: 500m
      memory: 512Mi
  - name: worker
    project: Worker/Worker.csproj
    replicas: 3
    resources:
      cpu: 200m
      memory: 256Mi
```

### 9.2 连接池配置

**场景**：配置数据库连接池以优化性能

**代码**：

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                sqlOptions.MinBatchSize(1);
                sqlOptions.MaxBatchSize(1000);
                sqlOptions.CommandTimeout(30);
            }));
        
        // 配置HttpClient连接池
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(Configuration["ApiSettings:BaseUrl"]);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = 100
        });
    }
}
```

## 10. 安全性示例

### 10.1 环境变量管理

**场景**：使用环境变量管理敏感配置

**配置文件**：

```yaml
name: security-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: ASPNETCORE_ENVIRONMENT
        value: Production
      - name: JWT_SECRET
        value: ${JWT_SECRET}
      - name: DB_PASSWORD
        value: ${DB_PASSWORD}
      - name: API_KEY
        value: ${API_KEY}
```

**启动命令**：

```bash
# 设置环境变量并启动
export JWT_SECRET=your-secret-key
export DB_PASSWORD=your-db-password
export API_KEY=your-api-key
tye run
```

### 10.2 Kubernetes密钥

**场景**：使用Kubernetes密钥管理敏感配置

**操作步骤**：

1. 创建Kubernetes密钥
2. 在tye.yaml中引用密钥
3. 部署应用

**示例代码**：

```bash
# 创建Kubernetes密钥
kubectl create secret generic app-secrets \
  --from-literal=jwt-secret=your-secret-key \
  --from-literal=db-password=your-db-password \
  --from-literal=api-key=your-api-key \
  --namespace myapp
```

**tye.yaml**：

```yaml
name: k8s-secrets-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    kubernetes:
      env:
        - name: JWT_SECRET
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: jwt-secret
        - name: DB_PASSWORD
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: db-password
        - name: API_KEY
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: api-key
```

## 11. 故障排除示例

### 11.1 服务启动失败

**场景**：排查服务启动失败的原因

**操作步骤**：

1. 查看详细日志
2. 检查端口占用
3. 验证依赖服务

**示例代码**：

```bash
# 查看详细日志
tye run --verbosity debug

# 检查端口占用
netstat -ano | findstr :8080

# 检查依赖服务状态
tye ps

# 查看服务日志
tye logs --service web
```

### 11.2 服务间通信失败

**场景**：排查服务间通信失败的原因

**操作步骤**：

1. 检查服务状态
2. 验证服务发现配置
3. 测试网络连接

**示例代码**：

```bash
# 检查服务状态
tye ps

# 查看环境变量
docker exec <container-id> env | grep SERVICE

# 测试网络连接
docker exec <container-id> curl http://api:8081/health

# 查看服务日志
tye logs --service web
tye logs --service api
```

### 11.3 Kubernetes部署失败

**场景**：排查Kubernetes部署失败的原因

**操作步骤**：

1. 查看Pod状态
2. 检查Pod日志
3. 验证资源配置

**示例代码**：

```bash
# 查看Pod状态
kubectl get pods -n myapp

# 查看Pod详情
kubectl describe pod <pod-name> -n myapp

# 查看Pod日志
kubectl logs <pod-name> -n myapp

# 检查资源使用情况
kubectl top pods -n myapp
kubectl describe nodes
```

## 12. 最佳实践示例

### 12.1 多环境配置

**场景**：为不同环境配置不同的设置

**配置文件结构**：

```
config/
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Staging.json
└── appsettings.Production.json
```

**tye.yaml**：

```yaml
name: multi-environment-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: ASPNETCORE_ENVIRONMENT
        value: ${ASPNETCORE_ENVIRONMENT:-Development}
      - name: ConnectionStrings__DefaultConnection
        value: ${DB_CONNECTION_STRING}
```

**启动命令**：

```bash
# 开发环境
export ASPNETCORE_ENVIRONMENT=Development
export DB_CONNECTION_STRING="Server=localhost;Database=DevDb;Integrated Security=True;"
tye run

# 生产环境
export ASPNETCORE_ENVIRONMENT=Production
export DB_CONNECTION_STRING="Server=prod-db;Database=ProdDb;User Id=app;Password=secure-password;"
tye run
```

### 12.2 模块化架构

**场景**：使用模块化架构组织大型应用

**项目结构**：

```
my-tye-app/
├── tye.yaml
├── src/
│   ├── Modules/
│   │   ├── Catalog/
│   │   │   ├── Catalog.Api/
│   │   │   ├── Catalog.Core/
│   │   │   └── Catalog.Infrastructure/
│   │   ├── Order/
│   │   │   ├── Order.Api/
│   │   │   ├── Order.Core/
│   │   │   └── Order.Infrastructure/
│   │   └── Payment/
│   │       ├── Payment.Api/
│   │       ├── Payment.Core/
│   │       └── Payment.Infrastructure/
│   └── Shared/
│       ├── Shared.Core/
│       └── Shared.Infrastructure/
└── tests/
    ├── Catalog.Tests/
    ├── Order.Tests/
    └── Payment.Tests/
```

**tye.yaml**：

```yaml
name: modular-architecture-demo
services:
  - name: catalog-api
    project: src/Modules/Catalog/Catalog.Api/Catalog.Api.csproj
    bindings:
      - port: 8081
        protocol: http
  - name: order-api
    project: src/Modules/Order/Order.Api/Order.Api.csproj
    bindings:
      - port: 8082
        protocol: http
  - name: payment-api
    project: src/Modules/Payment/Payment.Api/Payment.Api.csproj
    bindings:
      - port: 8083
        protocol: http
  - name: web-gateway
    project: src/WebGateway/WebGateway.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: CatalogApiUrl
        value: ${CATALOG-API_SERVICE_URI}
      - name: OrderApiUrl
        value: ${ORDER-API_SERVICE_URI}
      - name: PaymentApiUrl
        value: ${PAYMENT-API_SERVICE_URI}
```

### 12.3 监控和可观测性

**场景**：配置应用的监控和可观测性

**代码**：

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 添加健康检查
        services.AddHealthChecks()
            .AddSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            .AddRedis(Configuration.GetConnectionString("RedisConnection"))
            .AddCheck<CustomHealthCheck>("custom");

        // 添加OpenTelemetry
        services.AddOpenTelemetryTracing(builder =>
        {
            builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddJaegerExporter(options =>
                {
                    options.AgentHost = Configuration["Jaeger:Host"];
                    options.AgentPort = int.Parse(Configuration["Jaeger:Port"]);
                });
        });

        // 添加Prometheus指标
        services.AddMetrics()
            .AddPrometheusExporter()
            .AddHealthChecksMetrics();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHealthChecks("/health");
        app.UseMetricsEndpoint("/metrics");
        app.UseMetricsTextEndpoint("/metrics-text");
        app.UsePrometheusExporter();
    }
}
```

**tye.yaml**：

```yaml
name: observability-demo
services:
  - name: web
    project: Web/Web.csproj
    bindings:
      - port: 8080
        protocol: http
    env:
      - name: Jaeger__Host
        value: ${JAEGER_SERVICE_HOST:-localhost}
      - name: Jaeger__Port
        value: ${JAEGER_SERVICE_PORT:-6831}
  - name: jaeger
    image: jaegertracing/all-in-one:1.35
    bindings:
      - port: 6831
        protocol: udp
      - port: 16686
        protocol: http
  - name: prometheus
    image: prom/prometheus:v2.37.0
    bindings:
      - port: 9090
        protocol: http
    volumes:
      - name: prometheus-config
        hostPath: ./config/prometheus.yml
        mountPath: /etc/prometheus/prometheus.yml
  - name: grafana
    image: grafana/grafana:8.5.0
    bindings:
      - port: 3000
        protocol: http
```

## 13. 总结

本文档提供了Tye服务管理工具的各种使用示例，涵盖了从基本使用到高级配置的各个方面。通过这些示例，您可以了解如何：

1. **基本使用**：初始化项目、添加服务、启动服务
2. **高级配置**：多服务依赖、资源限制、HTTPS配置
3. **服务发现**：环境变量使用、配置文件使用
4. **数据库集成**：SQL Server、PostgreSQL、Redis
5. **Kubernetes部署**：基本部署、高级配置、Ingress配置
6. **日志管理**：查看服务日志、结构化日志
7. **健康检查**：基本健康检查、高级健康检查
8. **持续集成/持续部署**：GitHub Actions、Azure DevOps Pipeline
9. **性能优化**：资源限制、连接池配置
10. **安全性**：环境变量管理、Kubernetes密钥
11. **故障排除**：服务启动失败、服务间通信失败、Kubernetes部署失败
12. **最佳实践**：多环境配置、模块化架构、监控和可观测性

这些示例可以帮助您快速上手Tye，并在实际项目中应用最佳实践，提高开发效率和部署可靠性。
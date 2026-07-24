#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:package Prometheus.Client.AspNetCore@5.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property DockerDefaultTargetOS Linux

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus.Client;

namespace DockerComposeAdvancedIntegration
{
    public record DockerComposeEnvironment(string Name, string ConnectionString, int TimeoutSeconds);

    public class DockerComposeOptions
    {
        public DockerComposeEnvironment[] Environments { get; set; } = Array.Empty<DockerComposeEnvironment>();
        public bool EnableDetailedHealthChecks { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDockerComposeAdvancedIntegration(
            this IServiceCollection services, 
            Action<DockerComposeOptions> configure)
        {
            services.Configure(configure);
            
            var options = new DockerComposeOptions();
            configure(options);

            // 多环境健康检查
            foreach (var env in options.Environments)
            {
                services.AddHealthChecks()
                    .AddSqlServer(
                        name: $"sqlserver-{env.Name}",
                        connectionString: env.ConnectionString,
                        timeout: TimeSpan.FromSeconds(env.TimeoutSeconds),
                        tags: new[] { "db", "ready" });
            }

            // 指标监控
            if (options.EnableMetrics)
            {
                services.AddSingleton<IMetricFactory>(MetricFactory.Default);
                services.AddHostedService<DockerComposeMetricsService>();
            }
                
            return services;
        }
    }

    public class DockerComposeMetricsService : IHostedService
    {
        private readonly IMetricFactory _metricFactory;
        private readonly IHealthCheckService _healthCheck;

        public DockerComposeMetricsService(IMetricFactory metricFactory, IHealthCheckService healthCheck)
        {
            _metricFactory = metricFactory;
            _healthCheck = healthCheck;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 创建并注册Prometheus指标
            var healthGauge = _metricFactory.CreateGauge("docker_compose_health", "Service health status");
            
            // 定期更新健康状态指标
            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var report = await _healthCheck.CheckHealthAsync(cancellationToken);
                    healthGauge.Set(report.Status == HealthStatus.Healthy ? 1 : 0);
                    await Task.Delay(5000, cancellationToken);
                }
            }, cancellationToken);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDockerComposeAdvancedIntegration(options =>
            {
                options.Environments = new[]
                {
                    new DockerComposeEnvironment("Production", "Server=db-prod;Database=master;User=sa;Password=Your_password123;", 30),
                    new DockerComposeEnvironment("Staging", "Server=db-stage;Database=master;User=sa;Password=Your_password123;", 30)
                };
                options.EnableDetailedHealthChecks = true;
                options.EnableMetrics = true;
            });
        }
    }
}

/* docker-compose.multi-environment.yml 配置示例
version: '3.8'

services:
  webapp:
    image: ${DOCKER_REGISTRY-}webapp
    build:
      context: .
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db-prod;Database=master;User=sa;Password=Your_password123;
    ports:
      - "8080:80"
    depends_on:
      db-prod:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/healthz"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s

  db-prod:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-U", "sa", "-P", "Your_password123", "-Q", "SELECT 1"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s

  db-stage:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
      - MSSQL_PID=Developer
    ports:
      - "1434:1433"
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-U", "sa", "-P", "Your_password123", "-Q", "SELECT 1"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s
*/
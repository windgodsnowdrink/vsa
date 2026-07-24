#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property DockerDefaultTargetOS Linux

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DockerComposeIntegration
{
    public class DockerComposeOptions
    {
        public string ConnectionString { get; set; } = "Server=db;Database=master;User=sa;Password=Your_password123;";
        public int HealthCheckTimeoutSeconds { get; set; } = 30;
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDockerComposeIntegration(this IServiceCollection services, Action<DockerComposeOptions> configure)
        {
            services.Configure(configure);
            
            // 添加容器健康检查
            services.AddHealthChecks()
                .AddSqlServer(
                    name: "sqlserver",
                    connectionString: services.BuildServiceProvider()
                        .GetRequiredService<IOptions<DockerComposeOptions>>().Value.ConnectionString,
                    timeout: TimeSpan.FromSeconds(10),
                    tags: new[] { "db", "ready" });
                
            return services;
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDockerComposeIntegration(options =>
            {
                options.ConnectionString = "Server=db;Database=master;User=sa;Password=Your_password123;";
                options.HealthCheckTimeoutSeconds = 30;
            });
        }
    }
}

// docker-compose.yml 配置示例
/*
version: '3.8'

services:
  webapp:
    image: ${DOCKER_REGISTRY-}webapp
    build:
      context: .
      dockerfile: Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=master;User=sa;Password=Your_password123;
    ports:
      - "8080:80"
    depends_on:
      db:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost/healthz"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 60s

  db:
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
*/
# Consul AOT Agent Skill - Consul AOT高性能Consul客户端

## 技能概述

基于.NET 10 AOT架构的高性能Consul客户端，提供高效、可靠的服务注册与发现、键值存储等功能。通过AOT编译技术，实现了启动速度快、内存占用低、部署简单的特性，适合在各种环境下运行。

## 快速入门指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

`yaml
#:package Consul@1.7.10.1
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
`

### 配置AOT编译

在项目文件中添加以下属性：

`yaml
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true
`

### 注册服务

在主应用程序中注册Consul服务：

`csharp
// 配置Consul选项
builder.Configuration.AddJsonFile("consul_aot.setting.json");
builder.Services.Configure<Consul.AOT.ConsulClientOptions>(builder.Configuration.GetSection("Consul"));

// 注册Consul服务
builder.Services.AddSingleton<Consul.AOT.IConsulService, Consul.AOT.ConsulService>();
builder.Services.AddSingleton<Consul.AOT.ConsulAotEngine>();
`

### 使用示例

`csharp
// 获取Consul AOT引擎
var engine = serviceProvider.GetRequiredService<Consul.AOT.ConsulAotEngine>();

// 执行服务注册
var registerResult = await engine.ExecuteRegisterServiceAsync("web-service", "web-1", "localhost", 8080);
Console.WriteLine($"服务注册结果: {(registerResult ? "成功" : "失败")}

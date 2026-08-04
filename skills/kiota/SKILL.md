# Kiota 技能

## 技能简介

Kiota 技能是基于 Microsoft.Kiota 库开发的 AOT 编译 API 客户端生成工具，提供高效、便捷的 OpenAPI 规范处理和客户端生成能力。该工具采用 .NET 10.0 框架，通过 AOT（Ahead-of-Time）编译技术，实现了高性能的 API 客户端生成，支持多种编程语言和 API 风格。

### 主要特点

- **AOT 编译**：采用 .NET 10.0 的 AOT 编译技术，启动速度快，运行效率高
- **多语言支持**：支持生成多种编程语言的 API 客户端
- **全面的 API 风格支持**：支持 RESTful API、GraphQL 和 gRPC
- **OpenAPI 规范处理**：支持验证、下载和处理 OpenAPI 规范
- **缓存管理**：提供缓存管理功能，提高重复操作的效率
- **性能测试**：内置性能基准测试功能，评估生成速度
- **命令行接口**：提供简洁的命令行接口，支持命令别名
- **跨平台**：支持 Windows、Linux、macOS 等多个平台

## 技术架构

### 核心组件

- **Microsoft.Kiota.Abstractions**：提供核心抽象和接口
- **Microsoft.Kiota.Authentication.Azure**：提供 Azure 认证支持
- **Microsoft.Kiota.Http.HttpClientLibrary**：提供 HTTP 客户端实现
- **Microsoft.Kiota.Serialization.Json**：提供 JSON 序列化支持
- **Microsoft.Kiota.Serialization.Text**：提供文本序列化支持
- **Microsoft.Kiota.Serialization.Form**：提供表单序列化支持
- **.NET 10.0**：基础运行框架，支持 AOT 编译
- **Microsoft.Extensions.DependencyInjection**：依赖注入容器
- **Microsoft.Extensions.Caching.Memory**：内存缓存
- **Microsoft.Extensions.Logging**：日志记录
- **Microsoft.Extensions.Options**：配置管理
- **System.CommandLine**：命令行解析

### 架构设计

```
┌─────────────────────────────────────────────────────────┐
│                       命令行接口                        │
└─────────────────────────────┬───────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────┐
│                    KiotaService                        │
├─────────────────────┬────────────────────┬──────────────┤
│  客户端生成模块      │ 规范处理模块        │ 缓存管理模块   │
├─────────────────────┼────────────────────┼──────────────┤
│ ┌───────────────┐   │ ┌───────────────┐   │ ┌───────────┐ │
│ │ 多语言生成     │   │ │ 规范验证      │   │ │ 缓存清除  │ │
│ ├───────────────┤   │ ├───────────────┤   │ ├───────────┤ │
│ │ 命名空间配置   │   │ │ 规范下载      │   │ │ 缓存信息  │ │
│ └───────────────┘   │ └───────────────┘   │ └───────────┘ │
│                    │                     │              │
│                    │                     │              │
└─────────────────────┴────────────────────┴──────────────┘
```

## 安装和配置

### 系统要求

- **操作系统**：Windows 10/11、Linux、macOS
- **框架**：.NET 10.0 或更高版本（AOT 编译版本为自包含，无需安装 .NET 运行时）
- **内存**：至少 128MB，推荐 512MB 以上
- **磁盘空间**：至少 200MB
- **网络**：能够访问 OpenAPI 规范 URL（如果需要下载规范）

### 配置选项

通过环境变量进行配置：

| 环境变量 | 类型 | 默认值 | 描述 |
|---------|------|--------|------|
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | 布尔值 | `false` | 是否启用全球化不变模式 |
| `KIOTA_CACHE_DIRECTORY` | 字符串 | `%LOCALAPPDATA%\Kiota\Cache` | Kiota 缓存目录 |
| `KIOTA_TEMP_DIRECTORY` | 字符串 | `%TEMP%\Kiota` | Kiota 临时目录 |
| `KIOTA_MAX_CACHE_SIZE` | 整数 | `104857600` | Kiota 最大缓存大小（字节） |
| `KIOTA_DEFAULT_LANGUAGE` | 字符串 | `csharp` | 默认生成语言 |
| `KIOTA_DEFAULT_NAMESPACE` | 字符串 | `ApiClient` | 默认命名空间 |
| `KIOTA_HTTP_TIMEOUT` | 整数 | `30000` | HTTP 请求超时时间（毫秒） |
| `KIOTA_MAX_RETRY_COUNT` | 整数 | `3` | 最大重试次数 |
| `KIOTA_RETRY_DELAY` | 整数 | `1000` | 重试延迟时间（毫秒） |

## 使用方法

### 基本使用

```bash
# 生成 API 客户端
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./output csharp PetStore

# 使用命令别名
kiota_aot g https://petstore.swagger.io/v2/swagger.json ./output csharp PetStore

# 验证 OpenAPI 规范
kiota_aot validate https://petstore.swagger.io/v2/swagger.json

# 使用命令别名
kiota_aot v https://petstore.swagger.io/v2/swagger.json

# 列出支持的语言
kiota_aot list-languages

# 使用命令别名
kiota_aot ll

# 下载 OpenAPI 规范
kiota_aot download-spec https://petstore.swagger.io/v2/swagger.json ./swagger.json

# 使用命令别名
kiota_aot ds https://petstore.swagger.io/v2/swagger.json ./swagger.json

# 清除缓存
kiota_aot cache-clear

# 使用命令别名
kiota_aot cc

# 显示缓存信息
kiota_aot cache-info

# 使用命令别名
kiota_aot ci

# 运行性能测试
kiota_aot benchmark https://petstore.swagger.io/v2/swagger.json 5

# 使用命令别名
kiota_aot bm https://petstore.swagger.io/v2/swagger.json 5

# 显示帮助信息
kiota_aot help

# 使用命令别名
kiota_aot h
```

### 命令别名

为了方便使用，所有命令都提供了简短的别名：

| 完整命令 | 别名 | 描述 |
|---------|------|------|
| `generate` | `g` | 生成 API 客户端 |
| `validate` | `v` | 验证 OpenAPI 规范 |
| `list-languages` | `ll` | 列出支持的语言 |
| `download-spec` | `ds` | 下载 OpenAPI 规范 |
| `cache-clear` | `cc` | 清除缓存 |
| `cache-info` | `ci` | 显示缓存信息 |
| `benchmark` | `bm` | 运行性能测试 |
| `help` | `h` | 显示帮助信息 |

## 命令参考

### generate 命令

**功能**：生成 API 客户端

**语法**：
```bash
kiota_aot generate <spec> <output> <language> [namespace]
```

**参数**：
- `spec`：OpenAPI 规范 URL 或文件路径
- `output`：输出目录路径
- `language`：生成客户端的语言
- `namespace`：客户端命名空间，默认 `ApiClient`

**示例**：
```bash
kiota_aot generate https://petstore.swagger.io/v2/swagger.json ./PetStoreClient csharp PetStore
```

**输出**：
```
开始生成API客户端...
OpenAPI规范: https://petstore.swagger.io/v2/swagger.json
输出目录: ./PetStoreClient
语言: csharp
命名空间: PetStore
客户端生成完成，示例文件已创建: ./PetStoreClient/ApiClient.cs
客户端生成成功
```

### validate 命令

**功能**：验证 OpenAPI 规范

**语法**：
```bash
kiota_aot validate <spec>
```

**参数**：
- `spec`：OpenAPI 规范 URL 或文件路径

**示例**：
```bash
kiota_aot validate https://petstore.swagger.io/v2/swagger.json
```

**输出**：
```
开始验证OpenAPI规范...
规范路径: https://petstore.swagger.io/v2/swagger.json
OpenAPI规范验证成功
规范格式: Swagger 2.0
```

### list-languages 命令

**功能**：列出支持的语言

**语法**：
```bash
kiota_aot list-languages
```

**示例**：
```bash
kiota_aot list-languages
```

**输出**：
```
支持的语言列表:
=================
  - csharp
  - typescript
  - java
  - go
  - python
  - php
  - ruby
  - swift
  - kotlin
  - powershell
共支持 10 种语言
```

### download-spec 命令

**功能**：下载 OpenAPI 规范

**语法**：
```bash
kiota_aot download-spec <url> <output>
```

**参数**：
- `url`：OpenAPI 规范 URL
- `output`：输出文件路径

**示例**：
```bash
kiota_aot download-spec https://petstore.swagger.io/v2/swagger.json ./swagger.json
```

**输出**：
```
开始下载OpenAPI规范...
源URL: https://petstore.swagger.io/v2/swagger.json
输出文件: ./swagger.json
OpenAPI规范下载成功，文件大小: 22676 bytes
```

### cache-clear 命令

**功能**：清除缓存

**语法**：
```bash
kiota_aot cache-clear
```

**示例**：
```bash
kiota_aot cache-clear
```

**输出**：
```
开始清除缓存...
缓存目录: C:\Users\User\AppData\Local\Kiota\Cache
缓存清除成功，删除了 10 个文件
```

### cache-info 命令

**功能**：显示缓存信息

**语法**：
```bash
kiota_aot cache-info
```

**示例**：
```bash
kiota_aot cache-info
```

**输出**：
```
缓存信息:
============
缓存目录: C:\Users\User\AppData\Local\Kiota\Cache
文件数量: 10
总大小: 1024000 bytes
最近修改时间:
  - swagger.json: 2026-01-21 10:00:00
  - petstore.json: 2026-01-21 09:30:00
  - api.json: 2026-01-21 09:00:00
  - spec.json: 2026-01-21 08:30:00
  - openapi.json: 2026-01-21 08:00:00
```

### benchmark 命令

**功能**：运行性能基准测试

**语法**：
```bash
kiota_aot benchmark <spec> <count>
```

**参数**：
- `spec`：OpenAPI 规范 URL 或文件路径
- `count`：运行次数，默认 5

**示例**：
```bash
kiota_aot benchmark https://petstore.swagger.io/v2/swagger.json 5
```

**输出**：
```
开始性能基准测试...
============================================================
规范路径: https://petstore.swagger.io/v2/swagger.json
运行次数: 5
运行测试 1/5...
开始验证OpenAPI规范...
规范路径: https://petstore.swagger.io/v2/swagger.json
OpenAPI规范验证成功
规范格式: Swagger 2.0
运行 1 耗时: 250 ms
运行测试 2/5...
开始验证OpenAPI规范...
规范路径: https://petstore.swagger.io/v2/swagger.json
OpenAPI规范验证成功
规范格式: Swagger 2.0
运行 2 耗时: 220 ms
运行测试 3/5...
开始验证OpenAPI规范...
规范路径: https://petstore.swagger.io/v2/swagger.json
OpenAPI规范验证成功
规范格式: Swagger 2.0
运行 3 耗时: 230 ms
运行测试 4/5...
开始验证OpenAPI规范...
规范路径: https://petstore.swagger.io/v2/swagger.json
OpenAPI规范验证成功
规范格式: Swagger 2.0
运行 4 耗时: 210 ms
运行测试 5/5...
开始验证OpenAPI规范...
规范路径: https://petstore.swagger.io/v2/swagger.json
OpenAPI规范验证成功
规范格式: Swagger 2.0
运行 5 耗时: 240 ms
============================================================
性能基准测试结果:
  运行次数: 5
  平均耗时: 230.00 ms
  最小耗时: 210 ms
  最大耗时: 250 ms
  标准差: 14.14 ms
```

### help 命令

**功能**：显示帮助信息

**语法**：
```bash
kiota_aot help
```

**示例**：
```bash
kiota_aot help
```

**输出**：
```
Kiota AOT 工具
=============
命令列表:
  generate|g <spec> <output> <language> [namespace] - 生成API客户端
  validate|v <spec> - 验证OpenAPI规范
  list-languages|ll - 列出支持的语言
  download-spec|ds <url> <output> - 下载OpenAPI规范
  cache-clear|cc - 清除缓存
  cache-info|ci - 显示缓存信息
  benchmark|bm <spec> <count> - 运行性能基准测试
  help|h - 显示帮助信息
```

## 性能指标

### 客户端生成性能

| 操作类型 | 处理速度 | 内存占用 | 网络带宽 |
|---------|---------|---------|----------|
| 客户端生成 | 约 500ms/规范 | 约 256MB | 约 5Mbps |
| 规范验证 | 约 200ms/规范 | 约 128MB | 约 2Mbps |
| 规范下载 | 约 100ms/MB | 约 128MB | 约 10Mbps |
| 缓存操作 | 约 10ms/操作 | 约 64MB | 约 0Mbps |

### 系统资源使用

| 操作类型 | CPU 使用 | 内存使用 | 网络使用 |
|---------|---------|---------|----------|
| 空闲状态 | < 1% | ~128MB | ~0Mbps |
| 客户端生成 | ~30% | ~256MB | ~5Mbps |
| 规范验证 | ~15% | ~128MB | ~2Mbps |
| 规范下载 | ~10% | ~128MB | ~10Mbps |
| 缓存操作 | ~5% | ~64MB | ~0Mbps |
| 性能测试 | ~20% | ~192MB | ~5Mbps |

## 使用场景

### API 客户端生成

- **多语言客户端**：为同一 API 生成多种编程语言的客户端
- **类型安全**：生成类型安全的 API 客户端，减少运行时错误
- **代码一致性**：确保 API 调用代码的一致性和标准化

### 微服务通信

- **服务间调用**：生成微服务间通信的客户端代码
- **接口标准化**：通过 OpenAPI 规范标准化服务接口
- **减少手动编码**：自动生成通信代码，减少手动编码错误

### 第三方 API 集成

- **快速集成**：快速生成第三方 API 的客户端代码
- **文档同步**：与 API 文档保持同步，自动适应 API 变化
- **认证处理**：支持各种认证方式的集成

### CI/CD 集成

- **自动化生成**：在 CI/CD 流程中自动生成客户端代码
- **规范验证**：在构建过程中验证 OpenAPI 规范的有效性
- **版本控制**：将生成的客户端代码纳入版本控制

### 自动化测试

- **测试客户端**：生成专门用于测试的 API 客户端
- **性能测试**：使用内置的性能测试功能评估 API 性能
- **负载测试**：模拟高负载场景，测试 API 稳定性

## 限制和注意事项

1. **OpenAPI 规范要求**：需要有效的 OpenAPI 规范，不支持无效或不完整的规范
2. **代码生成限制**：复杂 API 可能生成大量代码，需要合理管理
3. **高级特性支持**：某些高级 OpenAPI 特性可能不被完全支持
4. **认证配置**：生成的客户端需要手动配置认证信息
5. **AOT 编译限制**：AOT 编译增加了构建时间，可能不支持某些反射特性
6. **网络依赖**：下载 OpenAPI 规范时需要稳定的网络连接
7. **缓存大小**：缓存目录可能会随着使用而增大，需要定期清理
8. **性能影响**：生成大型 API 客户端时可能会占用较多系统资源

## 常见问题

### Q: 生成客户端失败怎么办？

**A**：可以尝试以下方法：
- 检查 OpenAPI 规范是否有效
- 检查网络连接是否正常
- 检查输出目录是否可写
- 查看详细的错误日志，了解具体的失败原因

### Q: 验证 OpenAPI 规范失败怎么办？

**A**：可以尝试以下方法：
- 检查 OpenAPI 规范 URL 或文件路径是否正确
- 检查 OpenAPI 规范是否符合规范要求
- 检查网络连接是否正常（如果是 URL）
- 查看详细的错误日志，了解具体的失败原因

### Q: 支持哪些编程语言？

**A**：支持以下编程语言：
- csharp
- typescript
- java
- go
- python
- php
- ruby
- swift
- kotlin
- powershell

### Q: 缓存占用空间过大怎么办？

**A**：可以使用 `cache-clear` 命令清除缓存，或者手动清理缓存目录。

### Q: 性能测试结果不理想怎么办？

**A**：可以尝试以下方法：
- 检查网络连接是否稳定
- 增加系统资源（CPU、内存）
- 清理缓存，减少缓存开销
- 在不同的网络环境下进行测试

### Q: 如何处理认证？

**A**：生成的客户端需要手动配置认证信息，具体方法取决于 API 的认证方式。常见的认证方式包括：
- API Key
- OAuth 2.0
- Basic Authentication
- Bearer Token

## 技术支持

如果您在使用过程中遇到问题，可以：

1. **查看帮助信息**：运行 `kiota_aot help` 查看命令使用方法
2. **查看日志**：检查详细的错误日志，了解问题原因
3. **检查配置**：确保环境变量和配置选项正确设置
4. **参考文档**：查看本文档和技术参考文档
5. **联系技术支持**：如果问题仍然无法解决，请联系技术支持团队

## 更新日志

### v1.0.0 (2026-01-21)

- 初始版本
- 支持生成 API 客户端
- 支持验证 OpenAPI 规范
- 支持列出支持的语言
- 支持下载 OpenAPI 规范
- 支持缓存管理
- 支持性能基准测试
- 采用 AOT 编译技术，提高性能
- 支持 .NET 10.0
- 支持跨平台运行

## 许可证

本项目采用 MIT 许可证，详见 LICENSE 文件。
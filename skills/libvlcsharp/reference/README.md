# LibVLCSharp 技术参考文档

## 概述

LibVLCSharp 是基于 .NET 10 的高性能媒体处理系统，专为 .NET 开发者设计，采用 AOT（Ahead-of-Time）编译架构，提供强大的媒体处理功能。

## 核心组件

### 1. LibVLCSharpService
- **位置**: scripts/libvlcsharp_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 媒体播放、录制、流媒体、格式转换等核心功能实现
  - 性能优化（异步编程、缓存机制）
  - 错误处理和异常捕获
  - 详细的日志记录
  - 支持多种媒体格式和编解码器

### 2. 依赖注入容器
- **位置**: scripts/libvlcsharp_aot.cs
- **功能**: 管理服务生命周期和依赖关系
- **特性**: 
  - 服务注册和解析
  - 单例模式管理核心服务
  - 配置管理

## 技术架构

### AOT 编译架构

- **编译模式**: Ahead-of-Time (AOT) 编译
- **框架**: .NET 10.0
- **部署模式**: 自包含部署
- **运行时标识符**: win-x64 (支持其他平台)
- **优化级别**: Release

### 核心技术栈

- **C#**: 主要开发语言
- **.NET 10.0**: 运行时框架
- **LibVLCSharp**: 核心媒体处理库
- **Microsoft.Extensions.DependencyInjection**: 依赖注入
- **Microsoft.Extensions.Caching.Memory**: 内存缓存
- **Microsoft.Extensions.Logging**: 日志记录
- **Microsoft.Extensions.Options**: 配置管理
- **System.CommandLine**: 命令行接口

## API 参考

### LibVLCSharpService 方法

| 方法名 | 描述 | 参数 | 返回值 |
|-------|------|------|--------|
| PlayMediaAsync | 播放媒体文件 | mediaPath: string (媒体路径)<br>duration: int (播放时长，秒) | Task |
| RecordMediaAsync | 录制媒体文件 | mediaPath: string (媒体路径)<br>outputPath: string (输出路径)<br>duration: int (录制时长，秒) | Task |
| StreamMediaAsync | 流媒体播放 | mediaPath: string (媒体路径)<br>streamUrl: string (流媒体URL)<br>duration: int (流媒体时长，秒) | Task |
| ConvertMediaAsync | 转换媒体格式 | inputPath: string (输入路径)<br>outputPath: string (输出路径)<br>format: string (输出格式) | Task |
| GetMediaInfoAsync | 获取媒体信息 | mediaPath: string (媒体路径) | Task |
| ListCodecsAsync | 列出支持的编解码器 | 无 | Task |
| ListFiltersAsync | 列出支持的过滤器 | 无 | Task |
| RunBenchmarkAsync | 运行性能基准测试 | mediaPath: string (媒体路径)<br>iterations: int (运行次数) | Task |

## 配置选项

### 环境变量配置

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| VLC_PLUGIN_PATH | VLC 插件路径 | %ProgramFiles%\VideoLAN\VLC\plugins |
| VLC_LIB_PATH | VLC 库路径 | %ProgramFiles%\VideoLAN\VLC |
| LIBVLCSharp_LOG_LEVEL | LibVLCSharp 日志级别 | INFO |
| LIBVLCSharp_CACHE_DIR | LibVLCSharp 缓存目录 | %LOCALAPPDATA%\LibVLCSharp\Cache |
| LIBVLCSharp_VIDEO_CACHE_SIZE | 视频缓存大小 | 3000 |
| LIBVLCSharp_AUDIO_CACHE_SIZE | 音频缓存大小 | 2000 |
| LIBVLCSharp_NETWORK_CACHE_SIZE | 网络缓存大小 | 1000 |

### 运行配置 (libvlcsharp_aot.run.json)

```json
{
  "runtime": {
    "framework": "net11.0",
    "aot": true,
    "selfContained": true,
    "runtimeIdentifier": "win-x64",
    "optimizationLevel": "Release"
  },
  "environmentVariables": {
    "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT": "false",
    "VLC_PLUGIN_PATH": "%ProgramFiles%\\VideoLAN\\VLC\\plugins"
  },
  "memory": {
    "initial": 256,
    "maximum": 1024
  },
  "timeouts": {
    "command": 300000,
    "play": 300000,
    "record": 600000,
    "stream": 600000,
    "convert": 600000,
    "info": 30000,
    "listOperation": 10000,
    "benchmark": 300000
  }
}
```

### 构建配置 (libvlcsharp_aot.setting.json)

```json
{
  "name": "libvlcsharp_aot",
  "description": "基于LibVLCSharp的AOT编译媒体处理工具",
  "version": "1.0.0",
  "build": {
    "targetFramework": "net11.0",
    "publishAot": true,
    "selfContained": true,
    "runtimeIdentifier": "win-x64",
    "optimizationLevel": "Release"
  },
  "execution": {
    "timeout": 300000,
    "memoryLimit": 1024
  }
}
```

## 性能优化

1. **缓存使用**: 启用内存缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批量处理**: 对于多个媒体文件，使用批量处理提高效率
4. **连接池**: 管理媒体资源连接，避免频繁创建和销毁
5. **硬件加速**: 启用硬件加速以提高视频处理性能
6. **缓存大小调优**: 根据实际情况调整视频、音频和网络缓存大小

## 错误处理

### 常见错误和解决方案

1. **VLC 插件未找到**
   - 检查 VLC_PLUGIN_PATH 环境变量设置
   - 确保已正确安装 VLC 播放器
   - 验证插件目录是否存在

2. **媒体文件无法播放**
   - 检查媒体文件路径是否正确
   - 验证媒体文件格式是否被 VLC 支持
   - 检查文件权限

3. **网络流媒体连接失败**
   - 检查网络连接是否稳定
   - 验证流媒体 URL 是否正确
   - 增加网络缓存大小

4. **内存不足错误**
   - 减少同时处理的媒体文件数量
   - 增加最大内存限制
   - 优化媒体文件大小

5. **性能下降**
   - 关闭其他占用系统资源的应用程序
   - 调整缓存大小
   - 启用硬件加速

## 部署指南

### Windows 部署

1. **安装依赖**
   - 安装 .NET 10.0 运行时
   - 安装 VLC 播放器 (3.0 或更高版本)

2. **配置环境变量**
   - 设置 VLC_PLUGIN_PATH 指向 VLC 插件目录
   - 设置 VLC_LIB_PATH 指向 VLC 安装目录

3. **运行应用**
   ```bash
   libvlcsharp_aot.exe play video.mp4
   ```

### Linux 部署

1. **安装依赖**
   ```bash
   sudo apt-get install vlc
   sudo apt-get install libvlc-dev
   ```

2. **配置环境变量**
   ```bash
   export VLC_PLUGIN_PATH=/usr/lib/x86_64-linux-gnu/vlc/plugins
   export VLC_LIB_PATH=/usr/lib/x86_64-linux-gnu
   ```

3. **运行应用**
   ```bash
   ./libvlcsharp_aot play video.mp4
   ```

### macOS 部署

1. **安装依赖**
   - 通过 Homebrew 安装 VLC: `brew install --cask vlc`

2. **配置环境变量**
   ```bash
   export VLC_PLUGIN_PATH=/Applications/VLC.app/Contents/MacOS/plugins
   export VLC_LIB_PATH=/Applications/VLC.app/Contents/MacOS
   ```

3. **运行应用**
   ```bash
   ./libvlcsharp_aot play video.mp4
   ```

## 监控与日志

### 日志配置

- **日志级别**: 可通过 LIBVLCSharp_LOG_LEVEL 环境变量设置 (DEBUG, INFO, WARN, ERROR)
- **日志格式**: 支持控制台输出和文件输出
- **详细日志**: 启用详细日志以获取更详细的调试信息

### 监控指标

- **媒体播放时间**: 监控媒体文件播放的启动时间和总时长
- **内存使用**: 监控应用程序内存使用情况
- **CPU 使用率**: 监控媒体处理过程中的 CPU 使用率
- **网络带宽**: 监控网络流媒体的带宽使用

## 扩展性

### 自定义功能扩展

```csharp
public class CustomLibVLCSharpService : LibVLCSharpService
{
    public async Task<CustomResult> CustomMediaProcessingAsync(string mediaPath)
    {
        // 实现自定义媒体处理逻辑
        // 可以调用基类方法或添加新功能
        return new CustomResult();
    }
}
```

### 集成其他系统

```csharp
// 与 ASP.NET Core 集成
builder.Services.AddSingleton<LibVLCSharpService>();

// 与 Blazor 集成
builder.Services.AddScoped<LibVLCSharpService>();

// 与 MAUI 集成
services.AddSingleton<LibVLCSharpService>();
```

## 最佳实践

1. **依赖注入**: 使用依赖注入管理服务生命周期
2. **异步编程**: 优先使用异步 API 避免阻塞
3. **错误处理**: 正确处理异常情况
4. **日志记录**: 添加适当的日志记录
5. **性能监控**: 监控关键性能指标
6. **配置管理**: 使用配置文件和环境变量管理配置
7. **资源管理**: 正确释放媒体资源，避免内存泄漏
8. **异常处理**: 捕获和处理特定异常，提供有意义的错误信息

## 常见问题

### Q: 为什么需要安装 VLC 播放器？
**A:** LibVLCSharp 是 VLC 媒体播放器的 .NET 绑定，需要 VLC 提供底层的编解码器和插件支持。

### Q: 如何提高媒体播放性能？
**A:** 可以通过以下方式提高性能：
- 启用硬件加速
- 调整缓存大小
- 使用 AOT 编译的版本
- 关闭其他占用系统资源的应用程序

### Q: 支持哪些媒体格式？
**A:** LibVLCSharp 支持 VLC 播放器支持的所有媒体格式，包括但不限于：
- 视频：MP4, AVI, MKV, WMV, FLV, MOV 等
- 音频：MP3, AAC, WAV, FLAC, OGG 等
- 流媒体：HTTP, RTSP, RTMP, HLS 等

### Q: 如何处理网络流媒体的缓冲问题？
**A:** 可以通过增加 LIBVLCSharp_NETWORK_CACHE_SIZE 环境变量的值来解决缓冲问题，建议根据网络状况调整。

### Q: 如何在没有图形界面的服务器上使用？
**A:** 可以使用 VLC 的无界面模式，通过命令行参数或配置文件设置 `--no-xlib` 或 `--no-video` 选项。

## 版本历史

### 1.0.0 (2026-01-22)
- 初始版本
- 实现了媒体播放、录制、流媒体、格式转换等核心功能
- 采用 AOT 编译架构，提高性能
- 支持跨平台运行
- 提供详细的命令行接口

## 参考资料

- [LibVLCSharp 官方文档](https://code.videolan.org/videolan/LibVLCSharp)
- [VLC 官方文档](https://www.videolan.org/doc/)
- [.NET 10.0 文档](https://learn.microsoft.com/zh-cn/dotnet/)
- [Microsoft.Extensions.DependencyInjection 文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/dependency-injection)
- [System.CommandLine 文档](https://learn.microsoft.com/zh-cn/dotnet/standard/commandline/)

## 附录

### 环境变量列表

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| DOTNET_SYSTEM_GLOBALIZATION_INVARIANT | 是否启用不变全球化模式 | false |
| VLC_PLUGIN_PATH | VLC 插件路径 | %ProgramFiles%\VideoLAN\VLC\plugins |
| VLC_LIB_PATH | VLC 库路径 | %ProgramFiles%\VideoLAN\VLC |
| LIBVLCSharp_LOG_LEVEL | LibVLCSharp 日志级别 | INFO |
| LIBVLCSharp_CACHE_DIR | LibVLCSharp 缓存目录 | %LOCALAPPDATA%\LibVLCSharp\Cache |
| LIBVLCSharp_VIDEO_CACHE_SIZE | 视频缓存大小 | 3000 |
| LIBVLCSharp_AUDIO_CACHE_SIZE | 音频缓存大小 | 2000 |
| LIBVLCSharp_NETWORK_CACHE_SIZE | 网络缓存大小 | 1000 |

### 命令行参数列表

| 命令 | 别名 | 描述 | 参数 |
|------|------|------|------|
| play | p | 播放媒体文件 | <media> [duration] |
| record | r | 录制媒体文件 | <media> <output> [duration] |
| stream | s | 流媒体播放 | <media> <stream-url> [duration] |
| convert | c | 转换媒体格式 | <input> <output> [format] |
| info | i | 获取媒体信息 | <media> |
| list-codecs | lc | 列出支持的编解码器 | 无 |
| list-filters | lf | 列出支持的过滤器 | 无 |
| benchmark | bm | 运行性能基准测试 | <media> [iterations] |
| help | h | 显示帮助信息 | 无 |

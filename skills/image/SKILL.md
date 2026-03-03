# Image 技能文档

## 概述

Image 技能是一个基于 .NET 10 的高性能图像处理引擎，支持 AOT（Ahead-of-Time）编译，提供了丰富的图像处理功能。该技能采用单文件执行模式，具有优异的性能和可靠性。

## 核心功能

### 基本图像处理
- **图像大小调整**：支持按指定尺寸调整图像大小
- **图像格式转换**：支持在多种图像格式之间转换
- **图像裁剪**：支持按指定区域裁剪图像
- **图像旋转**：支持按指定角度旋转图像
- **图像翻转**：支持水平或垂直翻转图像
- **添加水印**：支持在图像上添加文本水印
- **应用滤镜**：支持应用多种图像滤镜效果
- **获取图像元数据**：支持获取图像的详细信息
- **图像优化**：支持优化图像质量和大小

### 高级功能
- **批量处理**：支持对多个图像进行批量操作
- **基准测试**：支持运行性能基准测试
- **配置管理**：支持查看当前配置信息

## 技术特性

### 性能优化
- **AOT 编译**：使用 Ahead-of-Time 编译提高性能
- **单文件发布**：生成单个可执行文件，便于部署和使用
- **内存缓存**：使用内存缓存提高重复操作的性能
- **并行处理**：支持并行处理批量操作，提高处理速度
- **异步编程**：使用异步编程模型提高并发性能

### 技术架构
- **模块化设计**：采用模块化架构，便于维护和扩展
- **依赖注入**：使用依赖注入模式，提高代码可测试性
- **配置选项**：使用强类型配置选项，便于管理配置
- **错误处理**：完善的错误处理和日志记录
- **命令行界面**：友好的命令行界面，支持命令别名

### 支持的图像格式
- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)
- WebP (.webp)
- BMP (.bmp)

## 安装与配置

### 系统要求
- .NET 10 运行时
- Windows x64 操作系统

### 安装方法
1. 确保已安装 .NET 10 运行时
2. 下载发布的单文件可执行程序
3. 直接运行即可使用

### 配置选项
配置文件位于 `image_aot.setting.json`，支持以下配置：

| 配置项 | 描述 | 默认值 |
|--------|------|--------|
| DefaultFormat | 默认图像格式 | png |
| DefaultQuality | 默认图像质量 | 85 |
| EnableCache | 是否启用缓存 | true |
| CacheSize | 缓存大小 | 100 |
| CacheExpiry | 缓存过期时间 | 30分钟 |
| EnableParallelProcessing | 是否启用并行处理 | true |
| MaxDegreeOfParallelism | 最大并行度 | CPU核心数 |
| TempDirectory | 临时目录 | 系统临时目录 |
| SupportedFormats | 支持的图像格式 | jpeg, jpg, png, gif, webp, bmp |

## 使用方法

### 命令格式
```bash
image_aot <command> [arguments]
```

### 命令列表

| 命令 | 别名 | 描述 | 参数 |
|------|------|------|------|
| resize | r | 调整图像大小 | inputFile outputFile size |
| convert | c | 转换图像格式 | inputFile outputFile format |
| crop | cr | 裁剪图像 | inputFile outputFile x y width height |
| rotate | ro | 旋转图像 | inputFile outputFile angle |
| flip | f | 翻转图像 | inputFile outputFile direction |
| watermark | w | 添加水印 | inputFile outputFile watermark position |
| filter | fi | 应用滤镜 | inputFile outputFile filter |
| metadata | m | 获取图像元数据 | inputFile |
| optimize | o | 优化图像 | inputFile outputFile [quality] |
| batch | b | 批量处理图像 | inputDir outputDir operation [parameters] |
| benchmark | bm | 运行基准测试 | [operation] [iterations] |
| config | co | 显示配置信息 | |
| help | h, ? | 显示帮助信息 | |

### 示例

#### 1. 调整图像大小
```bash
image_aot resize input.jpg output.jpg 800x600
```

#### 2. 转换图像格式
```bash
image_aot convert input.jpg output.png png
```

#### 3. 裁剪图像
```bash
image_aot crop input.jpg output.jpg 100 100 400 300
```

#### 4. 旋转图像
```bash
image_aot rotate input.jpg output.jpg 90
```

#### 5. 翻转图像
```bash
image_aot flip input.jpg output.jpg horizontal
```

#### 6. 添加水印
```bash
image_aot watermark input.jpg output.jpg "版权所有" bottom-right
```

#### 7. 应用滤镜
```bash
image_aot filter input.jpg output.jpg grayscale
```

#### 8. 获取图像元数据
```bash
image_aot metadata input.jpg
```

#### 9. 优化图像
```bash
image_aot optimize input.jpg output.jpg 80
```

#### 10. 批量处理图像
```bash
image_aot batch input_folder output_folder resize 800x600
```

#### 11. 运行基准测试
```bash
image_aot benchmark resize 10
```

#### 12. 显示配置信息
```bash
image_aot config
```

#### 13. 显示帮助信息
```bash
image_aot help
```

## 性能特性

### AOT 编译优势
- **启动速度快**：AOT 编译消除了 JIT 编译的开销
- **内存占用低**：减少了运行时的内存使用
- **部署简单**：单文件发布，无需额外依赖
- **安全性高**：减少了运行时攻击面

### 性能优化策略
1. **缓存机制**：对重复使用的图像进行缓存
2. **并行处理**：利用多核 CPU 并行处理批量操作
3. **异步 I/O**：使用异步 I/O 操作减少阻塞
4. **内存管理**：优化内存分配和释放
5. **算法优化**：使用高效的图像处理算法

## 故障排除

### 常见问题

#### 1. 无法运行程序
- 检查是否已安装 .NET 10 运行时
- 检查操作系统是否为 Windows x64

#### 2. 图像处理失败
- 检查输入文件是否存在
- 检查输入文件是否为支持的图像格式
- 检查输出目录是否可写

#### 3. 性能问题
- 对于大量图像处理，建议使用批量处理命令
- 对于重复处理相同图像，确保启用了缓存

### 日志记录
程序会在控制台输出日志信息，包括：
- 操作开始和完成的时间
- 处理状态和结果
- 错误信息和异常详情

## 扩展与定制

### 扩展功能
1. **添加新滤镜**：在 `ApplyFilterAsync` 方法中添加新的滤镜处理逻辑
2. **支持新格式**：在 `SaveImageAsync` 方法中添加新的图像格式支持
3. **添加新命令**：在 `Main` 方法中添加新的命令处理逻辑

### 定制配置
1. 修改 `image_aot.setting.json` 文件调整配置
2. 通过环境变量覆盖默认配置

## 版本历史

### v1.0.0
- 初始版本
- 支持基本图像处理功能
- 支持 AOT 编译和单文件发布
- 支持批量处理和基准测试

## 许可证

本项目采用 MIT 许可证，详见 LICENSE 文件。

## 联系方式

如有问题或建议，请联系：
- 作者：NET 专家
- 邮箱：contact@example.com

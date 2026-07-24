# TorchSharp 技能参考文档

## 技能概述

TorchSharp 技能是一个基于 .NET 10 和 TorchSharp 库的深度学习解决方案，专为高性能推理和模型训练而设计。该技能支持 AOT（Ahead-of-Time）编译，提供卓越的启动速度和内存效率，同时集成了 Scrutor 高级依赖注入功能，支持复杂的服务注册和装饰模式。

### 主要特性

- **高性能张量操作**：基于 PyTorch 的张量操作，支持 CPU 和 GPU 加速
- **模型训练与推理**：完整的模型训练和推理流程支持
- **图像处理**：集成 OpenCVSharp4 提供强大的图像处理能力
- **机器学习集成**：与 Microsoft.ML 无缝集成，支持传统机器学习任务
- **AOT 编译优化**：支持 .NET 10 AOT 编译，减少启动时间和内存占用
- **Scrutor 高级依赖注入**：支持高级服务注册和装饰模式
- **Docker 部署**：提供 Docker 容器化部署方案
- **云服务集成**：支持 Azure 和 AWS 云部署

## 目录结构

TorchSharp 技能采用模块化目录结构，清晰分离核心功能、配置文件和参考文档：

```
torchsharp/
├── index.yaml           # 技能元数据和配置
├── SKILL.md             # 技能详细文档
├── scripts/             # .NET 10 单文件执行脚本
│   ├── torchsharp_core.cs                # TorchSharp 核心实现
│   ├── torchsharp_core.setting.json      # TorchSharp 核心编译配置
│   ├── torchsharp_core.run.json          # TorchSharp 核心运行配置
│   ├── torchsharp_generator.cs           # Scrutor 用法示例
│   ├── torchsharp_generator.setting.json # Scrutor 演示编译配置
│   └── torchsharp_generator.run.json     # Scrutor 演示运行配置
├── reference/           # 参考文档
│   ├── README.md        # 本参考文档
│   └── examples.md      # 使用示例文档
├── models/              # 模型文件（默认目录）
├── images/              # 图像文件（默认目录）
├── logs/                # 日志文件（默认目录）
├── bin/                 # 编译输出目录
└── obj/                 # 中间文件目录
```

## 使用指南

### 环境要求

- .NET 10 SDK 或更高版本
- Windows 10/11（64 位）
- CUDA 11.8+（可选，用于 GPU 加速）
- Docker（可选，用于容器化部署）

### 安装与编译

1. **克隆技能仓库**

   ```bash
   git clone https://github.com/your-repo/torchsharp-skill.git
   cd torchsharp-skill
   ```

2. **安装依赖**

   ```bash
   dotnet restore
   ```

3. **编译技能**

   ```bash
   dotnet build -c Release
   ```

4. **执行 AOT 编译**（可选）

   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
   ```

### 基本使用

#### 张量操作

```bash
# 创建一个 2x3 的随机张量
dotnet run -- tensor create 2,3

# 执行张量加法
dotnet run -- tensor add

# 执行张量乘法
dotnet run -- tensor multiply

# 变换张量形状
dotnet run -- tensor reshape --shape 3,2

# 移动张量到 GPU（如果可用）
dotnet run -- tensor device --device cuda
```

#### 模型操作

```bash
# 加载模型

   ```bash

dotnet run -- model load --model_path models/resnet18.pt

# 保存模型

   ```bash

dotnet run -- model save --model_path models/my_model.pt

# 执行模型推理

   ```bash

dotnet run -- model infer --model_path models/resnet18.pt

# 获取模型信息

   ```bash

dotnet run -- model info --model_path models/resnet18.pt
```

#### 图像处理

```bash
# 读取图像

   ```bash

dotnet run -- image read --image_path images/cat.jpg

# 调整图像大小

   ```bash

dotnet run -- image resize --image_path images/cat.jpg --width 224 --height 224

# 图像转张量

   ```bash

dotnet run -- image tensor --image_path images/cat.jpg
```

#### Scrutor 演示

```bash
# 基本服务注册演示

   ```bash

dotnet run -- scrutor basic

# 装饰器模式演示

   ```bash

dotnet run -- scrutor decorator

# 服务筛选演示

   ```bash

dotnet run -- scrutor filter

# 生命周期管理演示

   ```bash

dotnet run -- scrutor lifetime
```

## 命令参考

### tensor 命令

用于执行张量相关操作。

**语法**：
```bash
dotnet run -- tensor <operation> [options]
```

**操作**：
- `create`：创建张量
- `add`：执行张量加法
- `multiply`：执行张量乘法
- `reshape`：变换张量形状
- `device`：移动张量到指定设备

**选项**：
- `--shape`：张量形状（例如：2,3,4）
- `--device`：目标设备（cpu 或 cuda）

### model 命令

用于执行模型相关操作。

**语法**：
```bash
dotnet run -- model <operation> [options]
```

**操作**：
- `load`：加载模型
- `save`：保存模型
- `infer`：执行模型推理
- `info`：获取模型信息

**选项**：
- `--model_path`：模型路径
- `--input_path`：输入数据路径
- `--output_path`：输出结果路径

### image 命令

用于执行图像处理操作。

**语法**：
```bash
dotnet run -- image <operation> [options]
```

**操作**：
- `read`：读取图像
- `save`：保存图像
- `resize`：调整图像大小
- `tensor`：图像转张量

**选项**：
- `--image_path`：图像路径
- `--output_path`：输出路径
- `--width`：目标宽度
- `--height`：目标高度

### scrutor 命令

用于执行 Scrutor 演示。

**语法**：
```bash
dotnet run -- scrutor <demo>
```

**演示**：
- `basic`：基本服务注册演示
- `decorator`：装饰器模式演示
- `filter`：服务筛选演示
- `lifetime`：生命周期管理演示

## 配置选项

### 编译配置

编译配置文件 `*.setting.json` 包含以下主要选项：

- **target_framework**：目标 .NET 框架版本（默认为 net11.0）
- **publish_aot**：是否启用 AOT 编译（默认为 true）
- **trim_mode**：裁剪模式（默认为 partial）
- **self_contained**：是否自包含发布（默认为 true）
- **publish_single_file**：是否发布为单文件（默认为 true）
- **runtime_identifier**：运行时标识符（默认为 win-x64）
- **dependencies**：项目依赖项列表

### 运行配置

运行配置文件 `*.run.json` 包含以下主要选项：

- **runtime**：运行时配置（命令、参数、工作目录等）
- **environment_variables**：环境变量配置
- **memory**：内存使用配置
- **cpu**：CPU 使用配置
- **logging**：日志配置
- **profiles**：预定义运行配置文件

## 环境变量

| 环境变量 | 描述 | 默认值 |
|----------|------|--------|
| `DOTNET_ENVIRONMENT` | .NET 环境 | Production |
| `TORCHSHARP_USE_CUDA` | 是否使用 CUDA | true |
| `TORCHSHARP_CUDA_VERSION` | CUDA 版本 | 11.8 |
| `CUDA_PATH` | CUDA 安装路径 | C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8 |
| `SCRUTOR_DEMO` | 是否启用 Scrutor 演示 | true |
| `IS_PREMIUM` | 是否为高级模式 | false |
| `GENERATOR_OUTPUT_PATH` | 生成器输出路径 | ../generated |
| `LOG_LEVEL` | 日志级别 | Information |

## 故障排除

### 常见问题

1. **CUDA 相关错误**

   **症状**：`CUDA error: no kernel image is available for execution on the device`
   **原因**：CUDA 版本不兼容
   **解决方案**：安装与 TorchSharp 兼容的 CUDA 版本（推荐 CUDA 11.8）

2. **内存不足错误**

   **症状**：`CUDA out of memory`
   **原因**：GPU 内存不足
   **解决方案**：减小批量大小或使用 CPU 模式

3. **模型加载错误**

   **症状**：`RuntimeError: Error loading model`
   **原因**：模型文件损坏或格式不兼容
   **解决方案**：确保模型文件正确且与 TorchSharp 版本兼容

4. **AOT 编译错误**

   **症状**：`Trim analysis error`
   **原因**：反射或动态代码使用导致的裁剪错误
   **解决方案**：在项目文件中添加裁剪排除规则

5. **Docker 构建错误**

   **症状**：`failed to solve: process "..." did not complete successfully`
   **原因**：Dockerfile 配置错误或依赖缺失
   **解决方案**：检查 Dockerfile 配置并确保所有依赖都正确安装

### 调试技巧

1. **启用详细日志**

   ```bash
   dotnet run --verbosity detailed
   ```

2. **检查 CUDA 状态**

   ```bash
   nvidia-smi
   ```

3. **使用 CPU 模式进行调试**

   ```bash
   dotnet run -- --device cpu
   ```

4. **检查 .NET 版本**

   ```bash
   dotnet --version
   ```

## 性能优化

### 内存优化

- **使用 using 语句**：确保及时释放张量和模型资源
- **内存池**：使用对象池减少内存分配
- **批量处理**：合理设置批量大小，平衡内存使用和计算效率
- **数据类型**：根据需要选择合适的数据类型（如 float16 用于 GPU 推理）

### 计算优化

- **GPU 加速**：优先使用 GPU 进行计算密集型操作
- **并行处理**：使用多线程处理数据加载和预处理
- **缓存**：缓存频繁使用的中间结果
- **算子融合**：利用 TorchSharp 的算子融合功能减少内存访问

### 推理优化

- **模型量化**：使用 INT8 量化减少模型大小和推理时间
- **模型剪枝**：移除不重要的神经元，减少模型复杂度
- **AOT 编译**：使用 AOT 编译减少启动时间
- **批处理**：使用批量推理提高吞吐量

## 部署指南

### 本地部署

**步骤**：
1. 编译技能：`dotnet build -c Release`
2. 运行技能：`dotnet run -- <command> [options]`

### AOT 编译部署

**步骤**：
1. 执行 AOT 编译：
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
   ```
2. 分发生成的单文件可执行文件

### Docker 部署

**步骤**：
1. 创建 Dockerfile：
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
   WORKDIR /app
   
   # 复制项目文件
   COPY *.csproj .
   RUN dotnet restore
   
   # 复制源代码
   COPY . .
   
   # 构建项目
   RUN dotnet build -c Release
   
   # 发布项目
   RUN dotnet publish -c Release -o out
   
   # 运行时镜像
   FROM mcr.microsoft.com/dotnet/runtime:10.0-windowsservercore-ltsc2022
   WORKDIR /app
   COPY --from=build /app/out .
   
   # 设置入口点
   ENTRYPOINT ["torchsharp_core.exe"]
   ```

2. 构建 Docker 镜像：
   ```bash
   docker build -t torchsharp-skill .
   ```

3. 运行 Docker 容器：
   ```bash
   docker run --rm torchsharp-skill tensor create 2,3
   ```

### 云部署

#### Azure 部署

**步骤**：
1. 创建 Azure 容器实例：
   ```bash
   az container create --name torchsharp-skill --image torchsharp-skill --resource-group myResourceGroup --command-line "tensor create 2,3"
   ```

2. 查看容器日志：
   ```bash
   az container logs --name torchsharp-skill --resource-group myResourceGroup
   ```

#### AWS 部署

**步骤**：
1. 创建 ECR 仓库：
   ```bash
   aws ecr create-repository --repository-name torchsharp-skill
   ```

2. 推送镜像到 ECR：
   ```bash
   docker tag torchsharp-skill:latest <aws-account-id>.dkr.ecr.<region>.amazonaws.com/torchsharp-skill:latest
   docker push <aws-account-id>.dkr.ecr.<region>.amazonaws.com/torchsharp-skill:latest
   ```

3. 运行 ECS 任务：
   ```bash
   aws ecs run-task --cluster myCluster --task-definition torchsharp-skill-task
   ```

## 参考资料

### 官方文档

- [TorchSharp 官方文档](https://github.com/dotnet/TorchSharp)
- [.NET 10 官方文档](https://learn.microsoft.com/zh-cn/dotnet/)
- [Scrutor 官方文档](https://github.com/khellang/Scrutor)
- [System.CommandLine 官方文档](https://learn.microsoft.com/zh-cn/dotnet/standard/commandline/)
- [Microsoft.Extensions.DependencyInjection 官方文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/dependency-injection)
- [OpenCVSharp4 官方文档](https://github.com/shimat/opencvsharp)
- [Microsoft.ML 官方文档](https://learn.microsoft.com/zh-cn/dotnet/machine-learning/)

### 教程和示例

- [TorchSharp 入门教程](https://github.com/dotnet/TorchSharp/tree/main/examples)
- [Scrutor 高级用法](https://andrewlock.net/using-scrutor-to-automatically-register-services-with-the-asp-net-core-di-container/)
- [.NET AOT 编译指南](https://learn.microsoft.com/zh-cn/dotnet/core/deploying/native-aot/)
- [深度学习基础教程](https://pytorch.org/tutorials/beginner/basics/intro.html)
- [图像处理入门](https://opencv.org/opencv-free-course/)

### 工具和库

- [PyTorch](https://pytorch.org/)：TorchSharp 的底层库
- [CUDA Toolkit](https://developer.nvidia.com/cuda-toolkit)：GPU 加速工具包
- [Docker](https://www.docker.com/)：容器化平台
- [Visual Studio 2022](https://visualstudio.microsoft.com/zh-cn/)：.NET 开发 IDE
- [Visual Studio Code](https://code.visualstudio.com/)：轻量级代码编辑器

## 联系我们

- **作者**：NET 专家
- **邮箱**：contact@net-expert.com
- **GitHub**：https://github.com/net-expert
- **网站**：https://net-expert.com

---

**© 2026 NET 专家. 保留所有权利.**

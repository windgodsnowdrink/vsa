# TorchSharp 深度学习技能

## 技能概述

TorchSharp 深度学习技能是一个基于 TorchSharp 库的高性能深度学习解决方案，专为 .NET 10 平台优化，支持 AOT（Ahead-of-Time）编译，提供卓越的推理性能和内存效率。

### 主要特性

- **高性能张量操作**：基于 PyTorch 的张量操作，支持 CPU 和 GPU 加速
- **模型训练与推理**：完整的模型训练和推理流程支持
- **图像处理**：集成 OpenCVSharp4 提供强大的图像处理能力
- **机器学习集成**：与 Microsoft.ML 无缝集成，支持传统机器学习任务
- **AOT 编译优化**：支持 .NET 10 AOT 编译，减少启动时间和内存占用
- **Scrutor 高级依赖注入**：支持高级服务注册和装饰模式
- **Docker 部署**：提供 Docker 容器化部署方案
- **云服务集成**：支持 Azure 和 AWS 云部署

### 技术栈

- **核心框架**：.NET 10、TorchSharp
- **依赖注入**：Microsoft.Extensions.DependencyInjection、Scrutor
- **命令行**：System.CommandLine
- **日志系统**：Microsoft.Extensions.Logging
- **缓存**：Microsoft.Extensions.Caching.Memory
- **文件系统**：System.IO.Abstractions
- **JSON 处理**：Newtonsoft.Json
- **机器学习**：Microsoft.ML
- **图像处理**：OpenCVSharp4

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- Windows 10/11（64 位）
- CUDA 11.8+（可选，用于 GPU 加速）
- Docker（可选，用于容器化部署）

### 安装

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

### 基本使用

#### 张量操作

```bash
# 创建一个 2x3 的随机张量
dotnet run -- tensor create 2,3

# 执行张量加法
dotnet run -- tensor add
```

#### 模型推理

```bash
# 加载模型并执行推理
dotnet run -- model infer --model_path models/resnet18.pt
```

#### 图像处理

```bash
# 调整图像大小
dotnet run -- image resize --image_path images/cat.jpg
```

#### Scrutor 演示

```bash
# 执行 Scrutor 装饰器演示
dotnet run -- scrutor decorator
```

## 核心功能

### 1. 张量操作

- **张量创建**：支持从数组、随机值、常量值创建张量
- **张量运算**：支持加减乘除、矩阵乘法、激活函数等
- **张量转换**：支持张量类型转换、形状变换、设备移动
- **自动微分**：支持自动计算梯度，用于模型训练

### 2. 模型训练

- **模型定义**：支持自定义模型架构
- **损失函数**：内置多种损失函数
- **优化器**：支持 SGD、Adam、RMSprop 等优化器
- **训练循环**：提供完整的训练循环实现
- **模型保存**：支持保存训练好的模型

### 3. 模型推理

- **模型加载**：支持加载 PyTorch 和 TorchSharp 模型
- **推理执行**：高性能模型推理
- **批处理**：支持批量推理
- **结果处理**：提供推理结果的后处理功能

### 4. 图像处理

- **图像读取**：支持多种图像格式
- **图像变换**：支持 resize、crop、flip 等变换
- **图像增强**：支持亮度、对比度、饱和度调整
- **特征提取**：支持从图像中提取特征

### 5. Scrutor 集成

- **服务注册**：支持基于约定的服务注册
- **装饰器模式**：支持服务装饰，实现横切关注点
- **服务筛选**：支持基于条件的服务注册
- **生命周期管理**：支持多种服务生命周期

## API 参考

### ITorchService

```csharp
public interface ITorchService
{
    // 创建随机张量
    Tensor CreateRandomTensor(int[] shape);
    
    // 执行张量加法
    Tensor AddTensors(Tensor a, Tensor b);
    
    // 执行张量乘法
    Tensor MultiplyTensors(Tensor a, Tensor b);
    
    // 移动张量到指定设备
    Tensor MoveToDevice(Tensor tensor, Device device);
}
```

### ITensorService

```csharp
public interface ITensorService
{
    // 从数组创建张量
    Tensor FromArray<T>(T[] data, int[] shape) where T : unmanaged;
    
    // 张量转数组
    T[] ToArray<T>(Tensor tensor) where T : unmanaged;
    
    // 张量形状变换
    Tensor Reshape(Tensor tensor, int[] shape);
    
    // 张量类型转换
    Tensor ToType(Tensor tensor, ScalarType type);
}
```

### IModelService

```csharp
public interface IModelService
{
    // 加载模型
    Module LoadModel(string modelPath);
    
    // 保存模型
    void SaveModel(Module model, string modelPath);
    
    // 执行模型推理
    Tensor Infer(Module model, Tensor input);
    
    // 获取模型信息
    ModelInfo GetModelInfo(Module model);
}
```

### IImageProcessingService

```csharp
public interface IImageProcessingService
{
    // 读取图像
    Mat ReadImage(string imagePath);
    
    // 保存图像
    void SaveImage(Mat image, string imagePath);
    
    // 调整图像大小
    Mat Resize(Mat image, Size size);
    
    // 图像转张量
    Tensor ToTensor(Mat image);
    
    // 张量转图像
    Mat ToImage(Tensor tensor);
}
```

### IScrutorDemoService

```csharp
public interface IScrutorDemoService
{
    // 执行基本服务注册演示
    void BasicRegistrationDemo();
    
    // 执行装饰器模式演示
    void DecoratorPatternDemo();
    
    // 执行服务筛选演示
    void ServiceFilteringDemo();
    
    // 执行生命周期管理演示
    void LifetimeManagementDemo();
}
```

## 命令行接口

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
- `convert`：转换图像格式
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
dotnet run -- scrutor <demo> [options]
```

**演示**：
- `basic`：基本服务注册演示
- `decorator`：装饰器模式演示
- `filter`：服务筛选演示
- `lifetime`：生命周期管理演示

## AOT 编译

### 配置

AOT 编译配置已在 `index.yaml` 文件中设置：

```yaml
compilation:
  target_framework: net10.0
  publish_aot: true
  trim_mode: partial
  self_contained: true
  publish_single_file: true
  runtime_identifier: win-x64
  additional_options: -p:UseAppHost=true
```

### 执行 AOT 编译

```bash
# 发布为 AOT 编译的单文件可执行文件
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
```

### AOT 编译优势

- **更快的启动时间**：减少 JIT 编译开销
- **更小的内存占用**：移除未使用的代码
- **更好的安全性**：减少可攻击面
- **无需运行时依赖**：单文件可执行，易于分发

## 示例

### 1. 基本张量操作

```csharp
using TorchSharp;
using TorchSharp.Modules;

// 创建两个随机张量
var tensor1 = torch.randn(new[] { 2, 3 });
var tensor2 = torch.randn(new[] { 2, 3 });

// 执行加法
var result = tensor1 + tensor2;

// 打印结果
Console.WriteLine("Tensor 1:");
Console.WriteLine(tensor1);
Console.WriteLine("\nTensor 2:");
Console.WriteLine(tensor2);
Console.WriteLine("\nResult:");
Console.WriteLine(result);

// 释放资源
tensor1.Dispose();
tensor2.Dispose();
result.Dispose();
```

### 2. 模型推理

```csharp
using TorchSharp;
using TorchSharp.Modules;

// 加载模型
var model = torch.jit.load("models/resnet18.pt");
model.eval();

// 创建输入张量 (batch_size=1, channels=3, height=224, width=224)
var input = torch.randn(new[] { 1, 3, 224, 224 });

// 执行推理
using var output = model.forward(input);

// 处理结果
var probabilities = torch.softmax(output, dim: 1);
var predictedClass = torch.argmax(probabilities, dim: 1).item<int>();

Console.WriteLine($"Predicted class: {predictedClass}");

// 释放资源
input.Dispose();
model.Dispose();
```

### 3. 图像处理

```csharp
using OpenCVSharp;
using TorchSharp;

// 读取图像
using var image = Cv2.ImRead("images/cat.jpg");

// 调整大小
using var resized = new Mat();
Cv2.Resize(image, resized, new Size(224, 224));

// 转换为张量
using var tensor = ImageToTensor(resized);

// 执行预处理（归一化）
using var normalized = (tensor / 255.0f - new[] { 0.485f, 0.456f, 0.406f }) / new[] { 0.229f, 0.224f, 0.225f };

// 添加批次维度
using var batched = normalized.unsqueeze(0);

Console.WriteLine($"Image shape: ({image.Width}, {image.Height})");
Console.WriteLine($"Tensor shape: {batched.shape}");

// 辅助函数：图像转张量
Tensor ImageToTensor(Mat image)
{
    // 转换为 RGB 格式
    using var rgb = new Mat();
    Cv2.CvtColor(image, rgb, ColorConversionCodes.BGR2RGB);
    
    // 创建张量
    var tensor = torch.tensor(rgb.Data, dtype: torch.float32);
    
    // 调整维度顺序 (H, W, C) -> (C, H, W)
    return tensor.permute(new[] { 2, 0, 1 });
}
```

### 4. Scrutor 装饰器模式

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 定义服务接口
public interface ICalculator
{
    int Add(int a, int b);
}

// 实现服务
public class Calculator : ICalculator
{
    public int Add(int a, int b)
    {
        Console.WriteLine($"Adding {a} + {b}");
        return a + b;
    }
}

// 定义装饰器
public class LoggingCalculator : ICalculator
{
    private readonly ICalculator _calculator;
    
    public LoggingCalculator(ICalculator calculator)
    {
        _calculator = calculator;
    }
    
    public int Add(int a, int b)
    {
        Console.WriteLine("Logging before calculation");
        var result = _calculator.Add(a, b);
        Console.WriteLine($"Logging after calculation: {result}");
        return result;
    }
}

// 注册服务和装饰器
var services = new ServiceCollection();
services.AddSingleton<ICalculator, Calculator>();
services.Decorate<ICalculator, LoggingCalculator>();

// 构建服务提供者
var serviceProvider = services.BuildServiceProvider();

// 解析服务
var calculator = serviceProvider.GetRequiredService<ICalculator>();

// 使用服务
var result = calculator.Add(5, 3);
Console.WriteLine($"Final result: {result}");
```

### 5. 完整的模型训练示例

```csharp
using TorchSharp;
using TorchSharp.Modules;
using Microsoft.Extensions.Logging;

// 定义简单的线性模型
public class LinearModel : nn.Module
{
    private readonly nn.Linear _linear;
    
    public LinearModel(int inputSize, int outputSize)
        : base(nameof(LinearModel))
    {
        _linear = nn.Linear(inputSize, outputSize);
        RegisterComponents();
    }
    
    public override Tensor forward(Tensor input)
    {
        return _linear.forward(input);
    }
}

// 创建模型、损失函数和优化器
var model = new LinearModel(10, 2);
var criterion = nn.CrossEntropyLoss();
var optimizer = torch.optim.Adam(model.parameters(), lr: 0.001f);

// 创建示例数据
var inputs = torch.randn(new[] { 64, 10 });  // 64 个样本，每个样本 10 个特征
var targets = torch.randint(0, 2, new[] { 64 }, dtype: torch.int64);  // 64 个标签，0 或 1

// 训练循环
for (int epoch = 0; epoch < 10; epoch++)
{
    // 前向传播
    var outputs = model.forward(inputs);
    var loss = criterion.forward(outputs, targets);
    
    // 反向传播和优化
    optimizer.zero_grad();
    loss.backward();
    optimizer.step();
    
    // 打印损失
    Console.WriteLine($"Epoch {epoch+1}, Loss: {loss.item<float>()}");
}

// 保存模型
model.save("models/linear_model.pt");
Console.WriteLine("Model saved successfully!");

// 释放资源
inputs.Dispose();
targets.Dispose();
model.Dispose();
criterion.Dispose();
optimizer.Dispose();
```

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

### 1. 内存优化

- **使用 using 语句**：确保及时释放张量和模型资源
- **内存池**：使用对象池减少内存分配
- **批量处理**：合理设置批量大小，平衡内存使用和计算效率
- **数据类型**：根据需要选择合适的数据类型（如 float16 用于 GPU 推理）

### 2. 计算优化

- **GPU 加速**：优先使用 GPU 进行计算密集型操作
- **并行处理**：使用多线程处理数据加载和预处理
- **缓存**：缓存频繁使用的中间结果
- **算子融合**：利用 TorchSharp 的算子融合功能减少内存访问

### 3. 推理优化

- **模型量化**：使用 INT8 量化减少模型大小和推理时间
- **模型剪枝**：移除不重要的神经元，减少模型复杂度
- **AOT 编译**：使用 AOT 编译减少启动时间
- **批处理**：使用批量推理提高吞吐量

### 4. 最佳实践

- **预热**：在实际推理前执行几次预热推理
- **内存管理**：定期清理不再使用的张量
- **设备选择**：根据操作类型选择合适的设备（CPU 或 GPU）
- **监控**：使用性能监控工具跟踪内存使用和计算时间

## 扩展和定制

### 1. 添加自定义模型

1. **创建模型类**

   ```csharp
   public class CustomModel : nn.Module
   {
       // 定义模型组件
       private readonly nn.Conv2d _conv1;
       private readonly nn.Linear _fc;
       
       public CustomModel() : base(nameof(CustomModel))
       {
           _conv1 = nn.Conv2d(3, 16, kernelSize: 3, stride: 1, padding: 1);
           _fc = nn.Linear(16 * 32 * 32, 10);
           RegisterComponents();
       }
       
       public override Tensor forward(Tensor input)
       {
           var x = torch.relu(_conv1.forward(input));
           x = x.flatten(1);
           return _fc.forward(x);
       }
   }
   ```

2. **注册模型到服务容器**

   ```csharp
services.AddSingleton<CustomModel>();
   ```

### 2. 自定义命令

1. **添加命令处理程序**

   ```csharp
   public class CustomCommand : Command
   {
       public CustomCommand() : base("custom", "执行自定义操作")
       {
           // 配置命令选项
           AddOption(new Option<string>("--param", "自定义参数"));
           
           // 设置处理程序
           SetHandler((param) => {
               Console.WriteLine($"执行自定义操作，参数：{param}");
           }, new Option<string>("--param"));
       }
   }
   ```

2. **注册命令**

   ```csharp
   var rootCommand = new RootCommand("TorchSharp 技能");
   rootCommand.AddCommand(new CustomCommand());
   ```

### 3. 集成第三方库

1. **安装 NuGet 包**

   ```bash
   dotnet add package <PackageName>
   ```

2. **更新 index.yaml**

   ```yaml
dependencies:
  - name: <PackageName>
    version: <Version>
   ```

3. **使用第三方库**

   ```csharp
   // 示例：使用 ML.NET 进行数据预处理
   using Microsoft.ML;
   using Microsoft.ML.Data;
   
   var mlContext = new MLContext();
   // 执行 ML.NET 操作
   ```

## 部署指南

### 1. 本地部署

**步骤**：
1. 编译技能：`dotnet build -c Release`
2. 运行技能：`dotnet run -- <command> [options]`

### 2. AOT 编译部署

**步骤**：
1. 执行 AOT 编译：
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
   ```
2. 分发生成的单文件可执行文件

### 3. Docker 部署

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

### 4. 云部署

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

## 贡献指南

### 开发流程

1. **Fork 仓库**
2. **创建特性分支**：`git checkout -b feature/your-feature`
3. **提交更改**：`git commit -m "Add your feature"`
4. **推送分支**：`git push origin feature/your-feature`
5. **创建 Pull Request**

### 代码规范

- 遵循 .NET 编码规范
- 使用 C# 10 语法特性
- 提供详细的代码注释
- 编写单元测试
- 确保代码通过 CI/CD 流程

### 测试

```bash
# 运行单元测试
dotnet test

# 运行集成测试
dotnet test --filter Category=Integration
```

### 文档

- 更新 SKILL.md 文档
- 为新功能添加示例
- 更新 API 参考
- 提供详细的使用说明

## 许可证

本技能采用 MIT 许可证。详见 [LICENSE](LICENSE) 文件。

## 联系方式

- **作者**：NET 专家
- **邮箱**：contact@net-expert.com
- **GitHub**：https://github.com/net-expert
- **网站**：https://net-expert.com

---

**© 2026 NET 专家. 保留所有权利.**

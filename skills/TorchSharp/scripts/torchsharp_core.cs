#:sdk Microsoft.NET.Sdk
#:package TorchSharp@0.100.0-preview.1
#:package Scrutor@4.0.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package System.IO.Abstractions@19.2.0
#:package Newtonsoft.Json@13.0.3
#:package Microsoft.ML@3.0.0
#:package OpenCVSharp4@4.9.0
#:package OpenCVSharp4.runtime.win@4.9.0
#:property LangVersion=10.0
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenCVSharp;
using TorchSharp;
using TorchSharp.Modules;

namespace TorchSharp.Skill
{
    // 服务接口定义
    public interface ITorchService
    {
        Tensor CreateRandomTensor(int[] shape);
        Tensor AddTensors(Tensor a, Tensor b);
        Tensor MultiplyTensors(Tensor a, Tensor b);
        Tensor MoveToDevice(Tensor tensor, Device device);
    }

    public interface ITensorService
    {
        Tensor FromArray<T>(T[] data, int[] shape) where T : unmanaged;
        T[] ToArray<T>(Tensor tensor) where T : unmanaged;
        Tensor Reshape(Tensor tensor, int[] shape);
        Tensor ToType(Tensor tensor, ScalarType type);
    }

    public interface IModelService
    {
        Module LoadModel(string modelPath);
        void SaveModel(Module model, string modelPath);
        Tensor Infer(Module model, Tensor input);
        ModelInfo GetModelInfo(Module model);
    }

    public interface IImageProcessingService
    {
        Mat ReadImage(string imagePath);
        void SaveImage(Mat image, string imagePath);
        Mat Resize(Mat image, Size size);
        Tensor ToTensor(Mat image);
        Mat ToImage(Tensor tensor);
    }

    public interface IScrutorDemoService
    {
        void BasicRegistrationDemo();
        void DecoratorPatternDemo();
        void ServiceFilteringDemo();
        void LifetimeManagementDemo();
    }

    // 模型信息类
    public class ModelInfo
    {
        public string Name { get; set; }
        public int InputSize { get; set; }
        public int OutputSize { get; set; }
        public int ParameterCount { get; set; }
    }

    // 服务实现
    public class TorchService : ITorchService
    {
        public Tensor CreateRandomTensor(int[] shape)
        {
            return torch.randn(shape);
        }

        public Tensor AddTensors(Tensor a, Tensor b)
        {
            return a + b;
        }

        public Tensor MultiplyTensors(Tensor a, Tensor b)
        {
            return a * b;
        }

        public Tensor MoveToDevice(Tensor tensor, Device device)
        {
            return tensor.to(device);
        }
    }

    public class TensorService : ITensorService
    {
        public Tensor FromArray<T>(T[] data, int[] shape) where T : unmanaged
        {
            return torch.tensor(data, shape);
        }

        public T[] ToArray<T>(Tensor tensor) where T : unmanaged
        {
            return tensor.to_array<T>();
        }

        public Tensor Reshape(Tensor tensor, int[] shape)
        {
            return tensor.reshape(shape);
        }

        public Tensor ToType(Tensor tensor, ScalarType type)
        {
            return tensor.to_type(type);
        }
    }

    public class ModelService : IModelService
    {
        public Module LoadModel(string modelPath)
        {
            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException($"Model file not found: {modelPath}");
            }
            return torch.jit.load(modelPath);
        }

        public void SaveModel(Module model, string modelPath)
        {
            var directory = Path.GetDirectoryName(modelPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            model.save(modelPath);
        }

        public Tensor Infer(Module model, Tensor input)
        {
            model.eval();
            using var noGrad = torch.no_grad();
            return model.forward(input);
        }

        public ModelInfo GetModelInfo(Module model)
        {
            // 简单实现，实际项目中可能需要更详细的信息
            return new ModelInfo
            {
                Name = model.GetType().Name,
                InputSize = 0,
                OutputSize = 0,
                ParameterCount = model.parameters().Count()
            };
        }
    }

    public class ImageProcessingService : IImageProcessingService
    {
        public Mat ReadImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image file not found: {imagePath}");
            }
            return Cv2.ImRead(imagePath);
        }

        public void SaveImage(Mat image, string imagePath)
        {
            var directory = Path.GetDirectoryName(imagePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            Cv2.ImWrite(imagePath, image);
        }

        public Mat Resize(Mat image, Size size)
        {
            var resized = new Mat();
            Cv2.Resize(image, resized, size);
            return resized;
        }

        public Tensor ToTensor(Mat image)
        {
            using var rgb = new Mat();
            Cv2.CvtColor(image, rgb, ColorConversionCodes.BGR2RGB);
            var tensor = torch.tensor(rgb.Data, dtype: torch.float32);
            return tensor.permute(new[] { 2, 0, 1 }) / 255.0f;
        }

        public Mat ToImage(Tensor tensor)
        {
            // 假设输入张量形状为 (C, H, W) 且值在 [0, 1] 范围内
            using var clamped = torch.clamp(tensor * 255.0f, 0, 255);
            using var permuted = clamped.permute(new[] { 1, 2, 0 });
            using var numpy = permuted.to(torch.uint8);
            var data = numpy.to_array<byte>();
            var mat = new Mat(tensor.shape[1], tensor.shape[2], MatType.CV_8UC3, data);
            using var bgr = new Mat();
            Cv2.CvtColor(mat, bgr, ColorConversionCodes.RGB2BGR);
            return bgr;
        }
    }

    // Scrutor 演示服务
    public class ScrutorDemoService : IScrutorDemoService
    {
        private readonly IServiceProvider _serviceProvider;

        public ScrutorDemoService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void BasicRegistrationDemo()
        {
            Console.WriteLine("=== Scrutor 基本服务注册演示 ===");
            
            var services = new ServiceCollection();
            
            // 使用 Scrutor 基于约定注册服务
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
            
            var provider = services.BuildServiceProvider();
            
            // 验证服务注册
            var torchService = provider.GetService<ITorchService>();
            Console.WriteLine($"ITorchService 注册成功: {torchService != null}");
            
            var tensorService = provider.GetService<ITensorService>();
            Console.WriteLine($"ITensorService 注册成功: {tensorService != null}");
            
            Console.WriteLine("基本服务注册演示完成！");
        }

        public void DecoratorPatternDemo()
        {
            Console.WriteLine("=== Scrutor 装饰器模式演示 ===");
            
            // 定义服务接口和实现
            interface ICalculator { int Add(int a, int b); }
            class Calculator : ICalculator { public int Add(int a, int b) { Console.WriteLine($"Calculator.Add({a}, {b})"); return a + b; } }
            class LoggingCalculator : ICalculator 
            {
                private readonly ICalculator _calculator;
                public LoggingCalculator(ICalculator calculator) { _calculator = calculator; }
                public int Add(int a, int b) 
                {
                    Console.WriteLine($"Logging before: {a} + {b}");
                    var result = _calculator.Add(a, b);
                    Console.WriteLine($"Logging after: {result}");
                    return result;
                }
            }
            
            var services = new ServiceCollection();
            services.AddSingleton<ICalculator, Calculator>();
            services.Decorate<ICalculator, LoggingCalculator>();
            
            var provider = services.BuildServiceProvider();
            var calculator = provider.GetRequiredService<ICalculator>();
            
            var result = calculator.Add(5, 3);
            Console.WriteLine($"最终结果: {result}");
            
            Console.WriteLine("装饰器模式演示完成！");
        }

        public void ServiceFilteringDemo()
        {
            Console.WriteLine("=== Scrutor 服务筛选演示 ===");
            
            var services = new ServiceCollection();
            
            // 使用 Scrutor 筛选特定服务
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.Where(type => 
                    type.Name.EndsWith("Service") && 
                    type.Name != "ScrutorDemoService"))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            var provider = services.BuildServiceProvider();
            
            // 验证服务注册
            var serviceTypes = new[] { typeof(ITorchService), typeof(ITensorService), typeof(IModelService), typeof(IImageProcessingService) };
            foreach (var serviceType in serviceTypes)
            {
                var service = provider.GetService(serviceType);
                Console.WriteLine($"{serviceType.Name} 注册成功: {service != null}");
            }
            
            Console.WriteLine("服务筛选演示完成！");
        }

        public void LifetimeManagementDemo()
        {
            Console.WriteLine("=== Scrutor 生命周期管理演示 ===");
            
            var services = new ServiceCollection();
            
            // 使用 Scrutor 为不同服务设置不同生命周期
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.Where(type => type.Name == "TorchService"))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.Where(type => type.Name == "TensorService"))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.Where(type => type.Name == "ModelService"))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            var provider = services.BuildServiceProvider();
            
            // 验证服务生命周期
            using (var scope = provider.CreateScope())
            {
                var torchService1 = provider.GetService<ITorchService>();
                var torchService2 = provider.GetService<ITorchService>();
                Console.WriteLine($"ITorchService (Singleton) 相同实例: {ReferenceEquals(torchService1, torchService2)}");
                
                var tensorService1 = scope.ServiceProvider.GetService<ITensorService>();
                var tensorService2 = scope.ServiceProvider.GetService<ITensorService>();
                Console.WriteLine($"ITensorService (Scoped) 相同实例: {ReferenceEquals(tensorService1, tensorService2)}");
                
                var modelService1 = scope.ServiceProvider.GetService<IModelService>();
                var modelService2 = scope.ServiceProvider.GetService<IModelService>();
                Console.WriteLine($"IModelService (Transient) 相同实例: {ReferenceEquals(modelService1, modelService2)}");
            }
            
            Console.WriteLine("生命周期管理演示完成！");
        }
    }

    // 主程序类
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        public static async Task<int> Main(string[] args)
        {
            // 初始化服务容器
            InitializeServices();

            // 构建命令行解析器
            var rootCommand = BuildCommandLine();
            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

            // 执行命令
            return await parser.InvokeAsync(args);
        }

        private static void InitializeServices()
        {
            var services = new ServiceCollection();

            // 添加日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 添加核心服务
            services.AddSingleton<ITorchService, TorchService>();
            services.AddSingleton<ITensorService, TensorService>();
            services.AddSingleton<IModelService, ModelService>();
            services.AddSingleton<IImageProcessingService, ImageProcessingService>();
            services.AddSingleton<IScrutorDemoService, ScrutorDemoService>();

            // 构建服务提供者
            _serviceProvider = services.BuildServiceProvider();
        }

        private static RootCommand BuildCommandLine()
        {
            var rootCommand = new RootCommand("TorchSharp 深度学习技能");

            // Tensor 命令
            var tensorCommand = new Command("tensor", "执行张量操作");
            var tensorOperationOption = new Option<string>("<operation>", "张量操作类型");
            var tensorShapeOption = new Option<string>("--shape", "张量形状");
            var tensorDeviceOption = new Option<string>("--device", () => "cpu", "目标设备");

            tensorCommand.AddOption(tensorOperationOption);
            tensorCommand.AddOption(tensorShapeOption);
            tensorCommand.AddOption(tensorDeviceOption);

            tensorCommand.SetHandler(async (operation, shape, device) =>
            {
                await HandleTensorCommand(operation, shape, device);
            }, tensorOperationOption, tensorShapeOption, tensorDeviceOption);

            // Model 命令
            var modelCommand = new Command("model", "执行模型操作");
            var modelOperationOption = new Option<string>("<operation>", "模型操作类型");
            var modelPathOption = new Option<string>("--model_path", "模型路径");
            var inputPathOption = new Option<string>("--input_path", "输入数据路径");
            var outputPathOption = new Option<string>("--output_path", "输出结果路径");

            modelCommand.AddOption(modelOperationOption);
            modelCommand.AddOption(modelPathOption);
            modelCommand.AddOption(inputPathOption);
            modelCommand.AddOption(outputPathOption);

            modelCommand.SetHandler(async (operation, modelPath, inputPath, outputPath) =>
            {
                await HandleModelCommand(operation, modelPath, inputPath, outputPath);
            }, modelOperationOption, modelPathOption, inputPathOption, outputPathOption);

            // Image 命令
            var imageCommand = new Command("image", "执行图像处理");
            var imageOperationOption = new Option<string>("<operation>", "图像处理操作类型");
            var imagePathOption = new Option<string>("--image_path", "图像路径");
            var outputPathImageOption = new Option<string>("--output_path", "输出路径");
            var widthOption = new Option<int>("--width", "目标宽度");
            var heightOption = new Option<int>("--height", "目标高度");

            imageCommand.AddOption(imageOperationOption);
            imageCommand.AddOption(imagePathOption);
            imageCommand.AddOption(outputPathImageOption);
            imageCommand.AddOption(widthOption);
            imageCommand.AddOption(heightOption);

            imageCommand.SetHandler(async (operation, imagePath, outputPath, width, height) =>
            {
                await HandleImageCommand(operation, imagePath, outputPath, width, height);
            }, imageOperationOption, imagePathOption, outputPathImageOption, widthOption, heightOption);

            // Scrutor 命令
            var scrutorCommand = new Command("scrutor", "执行 Scrutor 演示");
            var scrutorDemoOption = new Option<string>("<demo>", "Scrutor 演示类型");

            scrutorCommand.AddOption(scrutorDemoOption);

            scrutorCommand.SetHandler(async (demo) =>
            {
                await HandleScrutorCommand(demo);
            }, scrutorDemoOption);

            // 添加子命令到根命令
            rootCommand.AddCommand(tensorCommand);
            rootCommand.AddCommand(modelCommand);
            rootCommand.AddCommand(imageCommand);
            rootCommand.AddCommand(scrutorCommand);

            return rootCommand;
        }

        private static async Task HandleTensorCommand(string operation, string shape, string device)
        {
            var torchService = _serviceProvider.GetRequiredService<ITorchService>();
            var tensorService = _serviceProvider.GetRequiredService<ITensorService>();

            switch (operation.ToLower())
            {
                case "create":
                    {
                        int[] tensorShape = string.IsNullOrEmpty(shape) ? new[] { 2, 3 } : shape.Split(',').Select(int.Parse).ToArray();
                        using var tensor = torchService.CreateRandomTensor(tensorShape);
                        Console.WriteLine($"创建的张量形状: {string.Join(", ", tensor.shape)}");
                        Console.WriteLine("张量值:");
                        Console.WriteLine(tensor);
                        break;
                    }
                case "add":
                    {
                        using var tensor1 = torchService.CreateRandomTensor(new[] { 2, 3 });
                        using var tensor2 = torchService.CreateRandomTensor(new[] { 2, 3 });
                        using var result = torchService.AddTensors(tensor1, tensor2);
                        Console.WriteLine("Tensor 1:");
                        Console.WriteLine(tensor1);
                        Console.WriteLine("\nTensor 2:");
                        Console.WriteLine(tensor2);
                        Console.WriteLine("\nResult:");
                        Console.WriteLine(result);
                        break;
                    }
                case "multiply":
                    {
                        using var tensor1 = torchService.CreateRandomTensor(new[] { 2, 3 });
                        using var tensor2 = torchService.CreateRandomTensor(new[] { 2, 3 });
                        using var result = torchService.MultiplyTensors(tensor1, tensor2);
                        Console.WriteLine("Tensor 1:");
                        Console.WriteLine(tensor1);
                        Console.WriteLine("\nTensor 2:");
                        Console.WriteLine(tensor2);
                        Console.WriteLine("\nResult:");
                        Console.WriteLine(result);
                        break;
                    }
                case "reshape":
                    {
                        int[] tensorShape = string.IsNullOrEmpty(shape) ? new[] { 3, 2 } : shape.Split(',').Select(int.Parse).ToArray();
                        using var tensor = torchService.CreateRandomTensor(new[] { 2, 3 });
                        using var reshaped = tensorService.Reshape(tensor, tensorShape);
                        Console.WriteLine("原始张量形状: {0}", string.Join(", ", tensor.shape));
                        Console.WriteLine("原始张量:");
                        Console.WriteLine(tensor);
                        Console.WriteLine("\n重塑后形状: {0}", string.Join(", ", reshaped.shape));
                        Console.WriteLine("重塑后张量:");
                        Console.WriteLine(reshaped);
                        break;
                    }
                case "device":
                    {
                        using var tensor = torchService.CreateRandomTensor(new[] { 2, 3 });
                        var targetDevice = device.ToLower() == "cuda" && torch.cuda.is_available() ? torch.CUDA : torch.CPU;
                        using var movedTensor = torchService.MoveToDevice(tensor, targetDevice);
                        Console.WriteLine($"张量从 {tensor.device} 移动到 {movedTensor.device}");
                        Console.WriteLine("移动后张量:");
                        Console.WriteLine(movedTensor);
                        break;
                    }
                default:
                    Console.WriteLine($"未知的张量操作: {operation}");
                    break;
            }

            await Task.CompletedTask;
        }

        private static async Task HandleModelCommand(string operation, string modelPath, string inputPath, string outputPath)
        {
            var modelService = _serviceProvider.GetRequiredService<IModelService>();
            var torchService = _serviceProvider.GetRequiredService<ITorchService>();

            switch (operation.ToLower())
            {
                case "load":
                    {
                        if (string.IsNullOrEmpty(modelPath))
                        {
                            Console.WriteLine("请提供模型路径");
                            return;
                        }
                        
                        try
                        {
                            using var model = modelService.LoadModel(modelPath);
                            var modelInfo = modelService.GetModelInfo(model);
                            Console.WriteLine("模型加载成功！");
                            Console.WriteLine($"模型名称: {modelInfo.Name}");
                            Console.WriteLine($"参数数量: {modelInfo.ParameterCount}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"模型加载失败: {ex.Message}");
                        }
                        break;
                    }
                case "save":
                    {
                        if (string.IsNullOrEmpty(modelPath))
                        {
                            Console.WriteLine("请提供模型保存路径");
                            return;
                        }
                        
                        // 创建一个简单的模型用于演示
                        using var model = new nn.Linear(10, 2);
                        modelService.SaveModel(model, modelPath);
                        Console.WriteLine($"模型保存成功: {modelPath}");
                        break;
                    }
                case "infer":
                    {
                        if (string.IsNullOrEmpty(modelPath))
                        {
                            Console.WriteLine("请提供模型路径");
                            return;
                        }
                        
                        try
                        {
                            using var model = modelService.LoadModel(modelPath);
                            
                            // 创建示例输入
                            using var input = torchService.CreateRandomTensor(new[] { 1, 10 });
                            using var output = modelService.Infer(model, input);
                            
                            Console.WriteLine("模型推理成功！");
                            Console.WriteLine("输入:");
                            Console.WriteLine(input);
                            Console.WriteLine("\n输出:");
                            Console.WriteLine(output);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"模型推理失败: {ex.Message}");
                        }
                        break;
                    }
                case "info":
                    {
                        if (string.IsNullOrEmpty(modelPath))
                        {
                            Console.WriteLine("请提供模型路径");
                            return;
                        }
                        
                        try
                        {
                            using var model = modelService.LoadModel(modelPath);
                            var modelInfo = modelService.GetModelInfo(model);
                            Console.WriteLine("模型信息:");
                            Console.WriteLine($"名称: {modelInfo.Name}");
                            Console.WriteLine($"输入大小: {modelInfo.InputSize}");
                            Console.WriteLine($"输出大小: {modelInfo.OutputSize}");
                            Console.WriteLine($"参数数量: {modelInfo.ParameterCount}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"获取模型信息失败: {ex.Message}");
                        }
                        break;
                    }
                default:
                    Console.WriteLine($"未知的模型操作: {operation}");
                    break;
            }

            await Task.CompletedTask;
        }

        private static async Task HandleImageCommand(string operation, string imagePath, string outputPath, int width, int height)
        {
            var imageService = _serviceProvider.GetRequiredService<IImageProcessingService>();

            switch (operation.ToLower())
            {
                case "read":
                    {
                        if (string.IsNullOrEmpty(imagePath))
                        {
                            Console.WriteLine("请提供图像路径");
                            return;
                        }
                        
                        try
                        {
                            using var image = imageService.ReadImage(imagePath);
                            Console.WriteLine("图像读取成功！");
                            Console.WriteLine($"图像大小: {image.Width}x{image.Height}");
                            Console.WriteLine($"图像通道: {image.Channels()}");
                            Console.WriteLine($"图像类型: {image.Type()}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"图像读取失败: {ex.Message}");
                        }
                        break;
                    }
                case "resize":
                    {
                        if (string.IsNullOrEmpty(imagePath))
                        {
                            Console.WriteLine("请提供图像路径");
                            return;
                        }
                        
                        if (string.IsNullOrEmpty(outputPath))
                        {
                            outputPath = Path.Combine(Path.GetDirectoryName(imagePath) ?? ".", $"resized_{Path.GetFileName(imagePath)}");
                        }
                        
                        try
                        {
                            using var image = imageService.ReadImage(imagePath);
                            var targetSize = new Size(width > 0 ? width : 224, height > 0 ? height : 224);
                            using var resized = imageService.Resize(image, targetSize);
                            imageService.SaveImage(resized, outputPath);
                            Console.WriteLine($"图像调整大小成功！");
                            Console.WriteLine($"原始大小: {image.Width}x{image.Height}");
                            Console.WriteLine($"调整后大小: {resized.Width}x{resized.Height}");
                            Console.WriteLine($"保存路径: {outputPath}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"图像调整大小失败: {ex.Message}");
                        }
                        break;
                    }
                case "tensor":
                    {
                        if (string.IsNullOrEmpty(imagePath))
                        {
                            Console.WriteLine("请提供图像路径");
                            return;
                        }
                        
                        try
                        {
                            using var image = imageService.ReadImage(imagePath);
                            using var tensor = imageService.ToTensor(image);
                            Console.WriteLine("图像转张量成功！");
                            Console.WriteLine($"图像大小: {image.Width}x{image.Height}");
                            Console.WriteLine($"张量形状: {string.Join(", ", tensor.shape)}");
                            Console.WriteLine($"张量类型: {tensor.dtype}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"图像转张量失败: {ex.Message}");
                        }
                        break;
                    }
                default:
                    Console.WriteLine($"未知的图像处理操作: {operation}");
                    break;
            }

            await Task.CompletedTask;
        }

        private static async Task HandleScrutorCommand(string demo)
        {
            var scrutorService = _serviceProvider.GetRequiredService<IScrutorDemoService>();

            switch (demo.ToLower())
            {
                case "basic":
                    scrutorService.BasicRegistrationDemo();
                    break;
                case "decorator":
                    scrutorService.DecoratorPatternDemo();
                    break;
                case "filter":
                    scrutorService.ServiceFilteringDemo();
                    break;
                case "lifetime":
                    scrutorService.LifetimeManagementDemo();
                    break;
                default:
                    Console.WriteLine($"未知的 Scrutor 演示: {demo}");
                    break;
            }

            await Task.CompletedTask;
        }
    }
}

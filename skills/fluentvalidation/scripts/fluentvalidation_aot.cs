#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package FluentValidation@11.10.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentValidation.AOT
{
    /// <summary>
    /// FluentValidation 命令类型枚举
    /// </summary>
    public enum ValidationCommandType { Validate, ValidateModel, GetValidators, VersionInfo }

    /// <summary>
    /// FluentValidation 选项配置
    /// </summary>
    public class FluentValidationOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
        
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 1000;
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidationResultDto
    {
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// 验证错误信息
        /// </summary>
        public List<ValidationErrorDto> Errors { get; set; } = new List<ValidationErrorDto>();
        
        /// <summary>
        /// 验证时间（毫秒）
        /// </summary>
        public long ValidationTimeMs { get; set; }
        
        /// <summary>
        /// 验证对象类型
        /// </summary>
        public string? ObjectType { get; set; }
    }

    /// <summary>
    /// 验证错误信息
    /// </summary>
    public class ValidationErrorDto
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;
        
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// 错误代码
        /// </summary>
        public string? ErrorCode { get; set; }
        
        /// <summary>
        /// 尝试的值
        /// </summary>
        public object? AttemptedValue { get; set; }
    }

    /// <summary>
    /// 验证命令结果
    /// </summary>
    public class ValidationCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public ValidationCommandType CommandType { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 验证结果
        /// </summary>
        public ValidationResultDto? ValidationResult { get; set; }
    }

    /// <summary>
    /// 测试模型
    /// </summary>
    public class TestModel
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 测试模型验证器
    /// </summary>
    public class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("姓名不能为空")
                .Length(2, 50).WithMessage("姓名长度必须在2-50个字符之间");

            RuleFor(x => x.Age)
                .NotNull().WithMessage("年龄不能为空")
                .GreaterThan(0).WithMessage("年龄必须大于0")
                .LessThanOrEqualTo(150).WithMessage("年龄不能超过150");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("邮箱不能为空")
                .EmailAddress().WithMessage("邮箱格式不正确");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("手机号不能为空")
                .Length(11).WithMessage("手机号必须为11位");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("密码不能为空")
                .Length(6, 20).WithMessage("密码长度必须在6-20个字符之间");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("确认密码不能为空")
                .Equal(x => x.Password).WithMessage("两次输入的密码不一致");
        }
    }

    /// <summary>
    /// FluentValidation 服务接口
    /// </summary>
    public interface IFluentValidationService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<ValidationCommandResult> ExecuteCommandAsync(ValidationCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 验证对象
        /// </summary>
        /// <param name="obj">要验证的对象</param>
        /// <returns>验证结果</returns>
        Task<ValidationCommandResult> ValidateAsync(object obj);
        
        /// <summary>
        /// 验证测试模型
        /// </summary>
        /// <param name="model">测试模型</param>
        /// <returns>验证结果</returns>
        Task<ValidationCommandResult> ValidateTestModelAsync(TestModel model);
        
        /// <summary>
        /// 获取验证器信息
        /// </summary>
        /// <returns>验证器信息</returns>
        Task<ValidationCommandResult> GetValidatorsAsync();
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<ValidationCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// FluentValidation 服务实现
    /// </summary>
    public class FluentValidationService : IFluentValidationService
    {
        private readonly FluentValidationOptions _options;
        private readonly ILogger<FluentValidationService> _logger;
        private readonly IValidator<TestModel> _testModelValidator;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">FluentValidation 选项</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="testModelValidator">测试模型验证器</param>
        public FluentValidationService(IOptions<FluentValidationOptions> options, ILogger<FluentValidationService> logger, IValidator<TestModel> testModelValidator)
        {
            _options = options.Value;
            _logger = logger;
            _testModelValidator = testModelValidator;
        }

        /// <inheritdoc/>
        public async Task<ValidationCommandResult> ExecuteCommandAsync(ValidationCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ValidationCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case ValidationCommandType.Validate:
                        if (parameters?.ContainsKey("model") == true)
                        {
                            string modelJson = parameters["model"];
                            // 简化处理，这里只验证测试模型
                            var testModel = ParseTestModelFromJson(modelJson);
                            result = await ValidateTestModelAsync(testModel);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Model parameter is required";
                        }
                        break;
                    
                    case ValidationCommandType.ValidateModel:
                        if (parameters?.ContainsKey("name") == true && parameters?.ContainsKey("age") == true && parameters?.ContainsKey("email") == true)
                        {
                            var testModel = new TestModel
                            {
                                Name = parameters["name"],
                                Age = int.TryParse(parameters["age"], out int age) ? age : null,
                                Email = parameters["email"],
                                Phone = parameters?.ContainsKey("phone") == true ? parameters["phone"] : null,
                                Password = parameters?.ContainsKey("password") == true ? parameters["password"] : null,
                                ConfirmPassword = parameters?.ContainsKey("confirmPassword") == true ? parameters["confirmPassword"] : null
                            };
                            result = await ValidateTestModelAsync(testModel);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Name, Age and Email parameters are required";
                        }
                        break;
                    
                    case ValidationCommandType.GetValidators:
                        result = await GetValidatorsAsync();
                        break;
                    
                    case ValidationCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"未知命令类型: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行命令时出错: {CommandType}", commandType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ValidationCommandResult> ValidateAsync(object obj)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ValidationCommandResult
            {
                CommandType = ValidationCommandType.Validate
            };

            try
            {
                _logger.LogInformation("验证对象: {ObjectType}", obj.GetType().Name);
                
                // 根据对象类型选择验证器
                if (obj is TestModel testModel)
                {
                    var validationResult = await _testModelValidator.ValidateAsync(testModel);
                    result.Success = validationResult.IsValid;
                    result.ValidationResult = MapValidationResult(validationResult);
                    result.ValidationResult.ValidationTimeMs = stopwatch.ElapsedMilliseconds;
                    result.ValidationResult.ObjectType = obj.GetType().Name;
                    
                    if (validationResult.IsValid)
                    {
                        result.Results.Add("验证成功");
                        result.Results.Add($"验证对象类型: {obj.GetType().Name}");
                    }
                    else
                    {
                        result.Results.Add("验证失败");
                        result.Results.Add($"验证对象类型: {obj.GetType().Name}");
                        result.Results.Add($"错误数量: {validationResult.Errors.Count}");
                    }
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"不支持的对象类型: {obj.GetType().Name}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证对象时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ValidationCommandResult> ValidateTestModelAsync(TestModel model)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ValidationCommandResult
            {
                CommandType = ValidationCommandType.ValidateModel
            };

            try
            {
                _logger.LogInformation("验证测试模型");
                
                var validationResult = await _testModelValidator.ValidateAsync(model);
                result.Success = validationResult.IsValid;
                result.ValidationResult = MapValidationResult(validationResult);
                result.ValidationResult.ValidationTimeMs = stopwatch.ElapsedMilliseconds;
                result.ValidationResult.ObjectType = typeof(TestModel).Name;
                
                if (validationResult.IsValid)
                {
                    result.Results.Add("测试模型验证成功");
                    result.Results.Add($"姓名: {model.Name}");
                    result.Results.Add($"年龄: {model.Age}");
                    result.Results.Add($"邮箱: {model.Email}");
                }
                else
                {
                    result.Results.Add("测试模型验证失败");
                    result.Results.Add($"错误数量: {validationResult.Errors.Count}");
                    foreach (var error in validationResult.Errors)
                    {
                        result.Results.Add($"- {error.PropertyName}: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证测试模型时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ValidationCommandResult> GetValidatorsAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ValidationCommandResult
            {
                CommandType = ValidationCommandType.GetValidators
            };

            try
            {
                _logger.LogInformation("获取验证器信息");
                
                result.Success = true;
                result.Results.Add("FluentValidation 验证器列表:");
                result.Results.Add($"- TestModelValidator: 测试模型验证器");
                result.Results.Add($"  - 验证规则: 姓名、年龄、邮箱、手机号、密码、确认密码");
                result.Results.Add($"- 缓存启用: {_options.EnableCache}");
                result.Results.Add($"- 缓存大小: {_options.CacheSize}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取验证器信息时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<ValidationCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new ValidationCommandResult
            {
                CommandType = ValidationCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("获取 FluentValidation 版本信息");
                
                await Task.Delay(50); // 模拟操作
                
                result.Success = true;
                result.Results.Add("FluentValidation AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"FluentValidation 版本: 11.10.0");
                result.Results.Add($"工作目录: {_options.WorkingDirectory}");
                result.Results.Add($"启用缓存: {_options.EnableCache}");
                result.Results.Add($"缓存大小: {_options.CacheSize}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取版本信息时出错");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <summary>
        /// 从 JSON 解析测试模型
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>测试模型</returns>
        private TestModel ParseTestModelFromJson(string json)
        {
            // 简化处理，实际项目中应使用 JSON 序列化库
            var testModel = new TestModel();
            
            // 简单解析 JSON 字符串
            if (json.Contains("name"))
            {
                int nameStart = json.IndexOf("name") + 6;
                int nameEnd = json.IndexOf(",", nameStart);
                if (nameEnd == -1) nameEnd = json.IndexOf("}", nameStart);
                testModel.Name = json.Substring(nameStart, nameEnd - nameStart).Trim('"');
            }
            
            if (json.Contains("age"))
            {
                int ageStart = json.IndexOf("age") + 5;
                int ageEnd = json.IndexOf(",", ageStart);
                if (ageEnd == -1) ageEnd = json.IndexOf("}", ageStart);
                string ageStr = json.Substring(ageStart, ageEnd - ageStart);
                testModel.Age = int.TryParse(ageStr, out int age) ? age : null;
            }
            
            if (json.Contains("email"))
            {
                int emailStart = json.IndexOf("email") + 7;
                int emailEnd = json.IndexOf(",", emailStart);
                if (emailEnd == -1) emailEnd = json.IndexOf("}", emailStart);
                testModel.Email = json.Substring(emailStart, emailEnd - emailStart).Trim('"');
            }
            
            return testModel;
        }

        /// <summary>
        /// 映射验证结果
        /// </summary>
        /// <param name="validationResult">FluentValidation 验证结果</param>
        /// <returns>验证结果 DTO</returns>
        private ValidationResultDto MapValidationResult(ValidationResult validationResult)
        {
            var result = new ValidationResultDto
            {
                IsValid = validationResult.IsValid
            };
            
            foreach (var error in validationResult.Errors)
            {
                result.Errors.Add(new ValidationErrorDto
                {
                    PropertyName = error.PropertyName,
                    ErrorMessage = error.ErrorMessage,
                    ErrorCode = error.ErrorCode,
                    AttemptedValue = error.AttemptedValue
                });
            }
            
            return result;
        }
    }

    /// <summary>
    /// FluentValidation AOT 引擎
    /// </summary>
    public class FluentValidationAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FluentValidationAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public FluentValidationAotEngine(IServiceProvider serviceProvider, ILogger<FluentValidationAotEngine> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 执行命令行操作
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出码</returns>
        public async Task<int> ExecuteCommandLineAsync(string[] args)
        {
            _logger.LogInformation("FluentValidation AOT Engine 启动，参数: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var validationService = _serviceProvider.GetRequiredService<IFluentValidationService>();
            ValidationCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "validate":
                    case "validateobject":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供模型参数");
                            return 1;
                        }
                        string modelJson = string.Join(" ", args.Skip(1));
                        var parameters = new Dictionary<string, string> { { "model", modelJson } };
                        result = await validationService.ExecuteCommandAsync(ValidationCommandType.Validate, parameters);
                        break;
                    
                    case "validatemodel":
                        if (args.Length < 4)
                        {
                            Console.WriteLine("错误: 需要提供姓名、年龄和邮箱参数");
                            return 1;
                        }
                        var modelParameters = new Dictionary<string, string>
                        {
                            { "name", args[1] },
                            { "age", args[2] },
                            { "email", args[3] }
                        };
                        if (args.Length > 4) modelParameters["phone"] = args[4];
                        if (args.Length > 5) modelParameters["password"] = args[5];
                        if (args.Length > 6) modelParameters["confirmPassword"] = args[6];
                        result = await validationService.ExecuteCommandAsync(ValidationCommandType.ValidateModel, modelParameters);
                        break;
                    
                    case "validators":
                    case "getvalidators":
                        result = await validationService.GetValidatorsAsync();
                        break;
                    
                    case "version":
                    case "info":
                        result = await validationService.GetVersionInfoAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"错误: 未知命令 '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                return 1;
            }

            // 显示结果
            if (result != null)
            {
                Console.WriteLine($"\n命令执行结果: {(result.Success ? "成功" : "失败")}");
                Console.WriteLine($"执行时间: {result.ExecutionTimeMs} ms");
                
                if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"错误信息: {result.ErrorMessage}");
                }
                
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"- {item}");
                }
                
                if (result.ValidationResult != null)
                {
                    Console.WriteLine($"\n验证结果: {(result.ValidationResult.IsValid ? "通过" : "失败")}");
                    Console.WriteLine($"验证时间: {result.ValidationResult.ValidationTimeMs} ms");
                    if (!result.ValidationResult.IsValid && result.ValidationResult.Errors.Any())
                    {
                        Console.WriteLine($"\n验证错误:");
                        foreach (var error in result.ValidationResult.Errors)
                        {
                            Console.WriteLine($"  {error.PropertyName}: {error.ErrorMessage}
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Encrypt.AOT
{
    /// <summary>
    /// 加密命令类型枚举
    /// </summary>
    public enum EncryptCommandType { Encrypt, Decrypt, GenerateKey, GenerateHash, ValidateHash, VersionInfo }

    /// <summary>
    /// 加密选项配置
    /// </summary>
    public class EncryptOptions
    {
        /// <summary>
        /// 默认加密算法
        /// </summary>
        public string DefaultAlgorithm { get; set; } = "AES-GCM";
        
        /// <summary>
        /// AES密钥大小（位）
        /// </summary>
        public int AesKeySize { get; set; } = 256;
        
        /// <summary>
        /// RSA密钥大小（位）
        /// </summary>
        public int RsaKeySize { get; set; } = 4096;
        
        /// <summary>
        /// 默认哈希算法
        /// </summary>
        public string DefaultHashAlgorithm { get; set; } = "SHA256";
        
        /// <summary>
        /// 是否启用密钥缓存
        /// </summary>
        public bool EnableKeyCache { get; set; } = true;
        
        /// <summary>
        /// 密钥缓存大小
        /// </summary>
        public int KeyCacheSize { get; set; } = 1000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
    }

    /// <summary>
    /// 加密命令结果
    /// </summary>
    public class EncryptCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public EncryptCommandType CommandType { get; set; }
        
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
        /// 加密后的数据
        /// </summary>
        public string? EncryptedData { get; set; }
        
        /// <summary>
        /// 解密后的数据
        /// </summary>
        public string? DecryptedData { get; set; }
        
        /// <summary>
        /// 生成的密钥
        /// </summary>
        public string? GeneratedKey { get; set; }
        
        /// <summary>
        /// 生成的哈希值
        /// </summary>
        public string? GeneratedHash { get; set; }
        
        /// <summary>
        /// 哈希验证结果
        /// </summary>
        public bool? HashValid { get; set; }
    }

    /// <summary>
    /// 加密服务接口
    /// </summary>
    public interface IEncryptService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<EncryptCommandResult> ExecuteCommandAsync(EncryptCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <param name="key">密钥（可选）</param>
        /// <param name="algorithm">算法（可选）</param>
        /// <returns>加密结果</returns>
        Task<EncryptCommandResult> EncryptAsync(string data, string? key = null, string? algorithm = null);
        
        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="encryptedData">加密的数据</param>
        /// <param name="key">密钥</param>
        /// <param name="algorithm">算法（可选）</param>
        /// <returns>解密结果</returns>
        Task<EncryptCommandResult> DecryptAsync(string encryptedData, string key, string? algorithm = null);
        
        /// <summary>
        /// 生成密钥
        /// </summary>
        /// <param name="algorithm">算法（可选）</param>
        /// <param name="keySize">密钥大小（可选）</param>
        /// <returns>密钥生成结果</returns>
        Task<EncryptCommandResult> GenerateKeyAsync(string? algorithm = null, int? keySize = null);
        
        /// <summary>
        /// 生成哈希值
        /// </summary>
        /// <param name="data">要哈希的数据</param>
        /// <param name="algorithm">算法（可选）</param>
        /// <returns>哈希生成结果</returns>
        Task<EncryptCommandResult> GenerateHashAsync(string data, string? algorithm = null);
        
        /// <summary>
        /// 验证哈希值
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="hash">要验证的哈希值</param>
        /// <param name="algorithm">算法（可选）</param>
        /// <returns>哈希验证结果</returns>
        Task<EncryptCommandResult> ValidateHashAsync(string data, string hash, string? algorithm = null);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<EncryptCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// 加密服务实现
    /// </summary>
    public class EncryptService : IEncryptService
    {
        private readonly EncryptOptions _options;
        private readonly ILogger<EncryptService> _logger;
        private readonly Dictionary<string, string> _keyCache = new Dictionary<string, string>();
        private readonly object _cacheLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">加密选项</param>
        /// <param name="logger">日志记录器</param>
        public EncryptService(IOptions<EncryptOptions> options, ILogger<EncryptService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<EncryptCommandResult> ExecuteCommandAsync(EncryptCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case EncryptCommandType.Encrypt:
                        if (parameters?.ContainsKey("data") == true)
                        {
                            string data = parameters["data"];
                            string? key = parameters?.ContainsKey("key") == true ? parameters["key"] : null;
                            string? algorithm = parameters?.ContainsKey("algorithm") == true ? parameters["algorithm"] : null;
                            result = await EncryptAsync(data, key, algorithm);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Data parameter is required";
                        }
                        break;
                    
                    case EncryptCommandType.Decrypt:
                        if (parameters?.ContainsKey("encryptedData") == true && parameters?.ContainsKey("key") == true)
                        {
                            string encryptedData = parameters["encryptedData"];
                            string key = parameters["key"];
                            string? algorithm = parameters?.ContainsKey("algorithm") == true ? parameters["algorithm"] : null;
                            result = await DecryptAsync(encryptedData, key, algorithm);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "EncryptedData and Key parameters are required";
                        }
                        break;
                    
                    case EncryptCommandType.GenerateKey:
                        string? genAlgorithm = parameters?.ContainsKey("algorithm") == true ? parameters["algorithm"] : null;
                        int? keySize = parameters?.ContainsKey("keySize") == true ? int.Parse(parameters["keySize"]) : null;
                        result = await GenerateKeyAsync(genAlgorithm, keySize);
                        break;
                    
                    case EncryptCommandType.GenerateHash:
                        if (parameters?.ContainsKey("data") == true)
                        {
                            string data = parameters["data"];
                            string? hashAlgorithm = parameters?.ContainsKey("algorithm") == true ? parameters["algorithm"] : null;
                            result = await GenerateHashAsync(data, hashAlgorithm);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Data parameter is required";
                        }
                        break;
                    
                    case EncryptCommandType.ValidateHash:
                        if (parameters?.ContainsKey("data") == true && parameters?.ContainsKey("hash") == true)
                        {
                            string data = parameters["data"];
                            string hash = parameters["hash"];
                            string? hashAlgorithm = parameters?.ContainsKey("algorithm") == true ? parameters["algorithm"] : null;
                            result = await ValidateHashAsync(data, hash, hashAlgorithm);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "Data and Hash parameters are required";
                        }
                        break;
                    
                    case EncryptCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing command: {CommandType}", commandType);
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
        public async Task<EncryptCommandResult> EncryptAsync(string data, string? key = null, string? algorithm = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = EncryptCommandType.Encrypt
            };

            try
            {
                algorithm ??= _options.DefaultAlgorithm;
                _logger.LogInformation("Encrypting data using {Algorithm} algorithm", algorithm);
                
                byte[] encryptedBytes;
                string base64Key;
                
                switch (algorithm.ToUpper())
                {
                    case "AES-GCM":
                        using (var aes = AesGcm.Create())
                        {
                            byte[] keyBytes;
                            if (string.IsNullOrEmpty(key))
                            {
                                keyBytes = new byte[aes.KeySize / 8];
                                RandomNumberGenerator.Fill(keyBytes);
                                base64Key = Convert.ToBase64String(keyBytes);
                            }
                            else
                            {
                                keyBytes = Convert.FromBase64String(key);
                                base64Key = key;
                            }
                            
                            var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
                            RandomNumberGenerator.Fill(nonce);
                            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
                            var plaintextBytes = Encoding.UTF8.GetBytes(data);
                            encryptedBytes = new byte[plaintextBytes.Length];
                            
                            aes.Encrypt(nonce, plaintextBytes, encryptedBytes, tag, keyBytes);
                            
                            // 组合加密结果：nonce + tag + encryptedData
                            var combined = new byte[nonce.Length + tag.Length + encryptedBytes.Length];
                            Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
                            Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
                            Buffer.BlockCopy(encryptedBytes, 0, combined, nonce.Length + tag.Length, encryptedBytes.Length);
                            
                            result.EncryptedData = Convert.ToBase64String(combined);
                            result.GeneratedKey = base64Key;
                            result.Results.Add($"数据已使用 {algorithm} 算法加密");
                            result.Results.Add($"加密后数据: {result.EncryptedData}");
                            if (string.IsNullOrEmpty(key))
                            {
                                result.Results.Add($"生成的密钥: {base64Key}");
                            }
                        }
                        break;
                    
                    default:
                        throw new NotSupportedException($"Unsupported encryption algorithm: {algorithm}");
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting data");
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
        public async Task<EncryptCommandResult> DecryptAsync(string encryptedData, string key, string? algorithm = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = EncryptCommandType.Decrypt
            };

            try
            {
                algorithm ??= _options.DefaultAlgorithm;
                _logger.LogInformation("Decrypting data using {Algorithm} algorithm", algorithm);
                
                var combined = Convert.FromBase64String(encryptedData);
                var keyBytes = Convert.FromBase64String(key);
                string decryptedText;
                
                switch (algorithm.ToUpper())
                {
                    case "AES-GCM":
                        using (var aes = AesGcm.Create())
                        {
                            var nonceLength = AesGcm.NonceByteSizes.MaxSize;
                            var tagLength = AesGcm.TagByteSizes.MaxSize;
                            
                            var nonce = new byte[nonceLength];
                            var tag = new byte[tagLength];
                            var encryptedBytes = new byte[combined.Length - nonceLength - tagLength];
                            
                            Buffer.BlockCopy(combined, 0, nonce, 0, nonceLength);
                            Buffer.BlockCopy(combined, nonceLength, tag, 0, tagLength);
                            Buffer.BlockCopy(combined, nonceLength + tagLength, encryptedBytes, 0, encryptedBytes.Length);
                            
                            var plaintextBytes = new byte[encryptedBytes.Length];
                            aes.Decrypt(nonce, encryptedBytes, tag, plaintextBytes, keyBytes);
                            decryptedText = Encoding.UTF8.GetString(plaintextBytes);
                        }
                        break;
                    
                    default:
                        throw new NotSupportedException($"Unsupported encryption algorithm: {algorithm}");
                }
                
                result.DecryptedData = decryptedText;
                result.Results.Add($"数据已使用 {algorithm} 算法解密");
                result.Results.Add($"解密后数据: {decryptedText}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrypting data");
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
        public async Task<EncryptCommandResult> GenerateKeyAsync(string? algorithm = null, int? keySize = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = EncryptCommandType.GenerateKey
            };

            try
            {
                algorithm ??= _options.DefaultAlgorithm;
                _logger.LogInformation("Generating key for {Algorithm} algorithm", algorithm);
                
                byte[] keyBytes;
                
                switch (algorithm.ToUpper())
                {
                    case "AES-GCM":
                        int aesKeySize = keySize ?? _options.AesKeySize;
                        keyBytes = new byte[aesKeySize / 8];
                        RandomNumberGenerator.Fill(keyBytes);
                        break;
                    
                    case "RSA":
                        int rsaKeySize = keySize ?? _options.RsaKeySize;
                        using (var rsa = RSA.Create(rsaKeySize))
                        {
                            var privateKey = rsa.ExportPkcs8PrivateKey();
                            keyBytes = privateKey;
                        }
                        break;
                    
                    default:
                        throw new NotSupportedException($"Unsupported algorithm for key generation: {algorithm}");
                }
                
                var base64Key = Convert.ToBase64String(keyBytes);
                result.GeneratedKey = base64Key;
                result.Results.Add($"已生成 {algorithm} 算法密钥");
                result.Results.Add($"密钥大小: {keySize ?? (algorithm.ToUpper() == "RSA" ? _options.RsaKeySize : _options.AesKeySize)} 位");
                result.Results.Add($"密钥: {base64Key}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating key");
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
        public async Task<EncryptCommandResult> GenerateHashAsync(string data, string? algorithm = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = EncryptCommandType.GenerateHash
            };

            try
            {
                algorithm ??= _options.DefaultHashAlgorithm;
                _logger.LogInformation("Generating hash using {Algorithm} algorithm", algorithm);
                
                byte[] hashBytes;
                
                using (var hashAlgorithm = HashAlgorithmName.FromOid(algorithm))
                {
                    using (var hasher = IncrementalHash.CreateHash(hashAlgorithm))
                    {
                        var dataBytes = Encoding.UTF8.GetBytes(data);
                        hasher.AppendData(dataBytes);
                        hashBytes = hasher.GetCurrentHash();
                    }
                }
                
                var base64Hash = Convert.ToBase64String(hashBytes);
                result.GeneratedHash = base64Hash;
                result.Results.Add($"已使用 {algorithm} 算法生成哈希值");
                result.Results.Add($"原始数据: {data}");
                result.Results.Add($"哈希值: {base64Hash}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating hash");
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
        public async Task<EncryptCommandResult> ValidateHashAsync(string data, string hash, string? algorithm = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = EncryptCommandType.ValidateHash
            };

            try
            {
                algorithm ??= _options.DefaultHashAlgorithm;
                _logger.LogInformation("Validating hash using {Algorithm} algorithm", algorithm);
                
                // 生成数据的哈希值
                var generateResult = await GenerateHashAsync(data, algorithm);
                bool isValid = generateResult.GeneratedHash == hash;
                
                result.HashValid = isValid;
                result.Results.Add($"使用 {algorithm} 算法验证哈希值");
                result.Results.Add($"原始数据: {data}");
                result.Results.Add($"提供的哈希值: {hash}");
                result.Results.Add($"计算的哈希值: {generateResult.GeneratedHash}");
                result.Results.Add($"验证结果: {(isValid ? "有效" : "无效")}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating hash");
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
        public async Task<EncryptCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EncryptCommandResult
            {
                CommandType = EncryptCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("Getting version info");
                
                result.Success = true;
                result.Results.Add("Encrypt AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"默认加密算法: {_options.DefaultAlgorithm}");
                result.Results.Add($"默认哈希算法: {_options.DefaultHashAlgorithm}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting version info");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return await Task.FromResult(result);
        }
    }

    /// <summary>
    /// Encrypt AOT 引擎
    /// </summary>
    public class EncryptAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EncryptAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public EncryptAotEngine(IServiceProvider serviceProvider, ILogger<EncryptAotEngine> logger)
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
            _logger.LogInformation("Encrypt AOT Engine starting with args: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var encryptService = _serviceProvider.GetRequiredService<IEncryptService>();
            EncryptCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "encrypt":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供要加密的数据");
                            return 1;
                        }
                        string data = args[1];
                        string? key = args.Length > 2 ? args[2] : null;
                        string? algorithm = args.Length > 3 ? args[3] : null;
                        result = await encryptService.EncryptAsync(data, key, algorithm);
                        break;
                    
                    case "decrypt":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供加密的数据和密钥");
                            return 1;
                        }
                        string encryptedData = args[1];
                        string decryptKey = args[2];
                        string? decryptAlgorithm = args.Length > 3 ? args[3] : null;
                        result = await encryptService.DecryptAsync(encryptedData, decryptKey, decryptAlgorithm);
                        break;
                    
                    case "generatekey":
                    case "genkey":
                        string? genAlgorithm = args.Length > 1 ? args[1] : null;
                        int? keySize = args.Length > 2 ? int.Parse(args[2]) : null;
                        result = await encryptService.GenerateKeyAsync(genAlgorithm, keySize);
                        break;
                    
                    case "generatehash":
                    case "genhash":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供要哈希的数据");
                            return 1;
                        }
                        string hashData = args[1];
                        string? hashAlgorithm = args.Length > 2 ? args[2] : null;
                        result = await encryptService.GenerateHashAsync(hashData, hashAlgorithm);
                        break;
                    
                    case "validatehash":
                    case "valhash":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供原始数据和哈希值");
                            return 1;
                        }
                        string valData = args[1];
                        string valHash = args[2];
                        string? valAlgorithm = args.Length > 3 ? args[3] : null;
                        result = await encryptService.ValidateHashAsync(valData, valHash, valAlgorithm);
                        break;
                    
                    case "version":
                        result = await encryptService.GetVersionInfoAsync();
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
                else
                {
                    foreach (var res in result.Results)
                    {
                        Console.WriteLine($"- {res}");
                    }
                }
            }

            return result?.Success == true ? 0 : 1;
        }

        private void ShowHelp()
        {
            Console.WriteLine("Encrypt AOT Engine - 基于 .NET 10 AOT 的加密工具");
            Console.WriteLine("=");
            Console.WriteLine();
            Console.WriteLine("可用命令:");
            Console.WriteLine();
            Console.WriteLine("  encrypt <data> [key] [algorithm]    - 加密数据");
            Console.WriteLine("  decrypt <encryptedData> <key> [algorithm]  - 解密数据");
            Console.WriteLine("  generatekey|genkey [algorithm] [keySize]  - 生成密钥");
            Console.WriteLine("  generatehash|genhash <data> [algorithm]  - 生成哈希值");
            Console.WriteLine("  validatehash|valhash <data> <hash> [algorithm]  - 验证哈希值");
            Console.WriteLine("  version                      - 显示版本信息");
            Console.WriteLine("  help                         - 显示此帮助信息");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  encrypt "Hello World"          - 使用默认算法加密数据");
            Console.WriteLine("  decrypt <encryptedData> <key>  - 解密数据");
            Console.WriteLine("  genkey AES-GCM 256            - 生成 256 位 AES-GCM 密钥");
            Console.WriteLine("  genhash "Hello World" SHA512    - 生成 SHA512 哈希值");
        }
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class EncryptAotExtensions
    {
        /// <summary>
        /// 添加 Encrypt AOT 服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddEncryptAot(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddOptions<EncryptOptions>();
            services.AddSingleton<IEncryptService, EncryptService>();
            services.AddSingleton<EncryptAotEngine>();
            return services;
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 创建服务集合
            var services = new ServiceCollection();
            
            // 配置服务
            services.AddEncryptAot();
            
            // 构建服务提供器
            var serviceProvider = services.BuildServiceProvider();
            
            // 获取引擎并执行命令
            var engine = serviceProvider.GetRequiredService<EncryptAotEngine>();
            return await engine.ExecuteCommandLineAsync(args);
        }
    }
}
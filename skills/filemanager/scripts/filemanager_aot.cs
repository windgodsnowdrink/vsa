#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileManager.AOT
{
    /// <summary>
    /// FileManager 命令类型枚举
    /// </summary>
    public enum FileManagerCommandType { CreateFile, ReadFile, WriteFile, DeleteFile, ListFiles, CopyFile, MoveFile, VersionInfo }

    /// <summary>
    /// FileManager 选项配置
    /// </summary>
    public class FileManagerOptions
    {
        /// <summary>
        /// 工作目录
        /// </summary>
        public string WorkingDirectory { get; set; } = Environment.CurrentDirectory;
        
        /// <summary>
        /// 是否启用文件监控
        /// </summary>
        public bool EnableFileMonitoring { get; set; } = false;
        
        /// <summary>
        /// 是否启用文件版本控制
        /// </summary>
        public bool EnableVersioning { get; set; } = false;
        
        /// <summary>
        /// 版本控制目录
        /// </summary>
        public string VersioningDirectory { get; set; } = ".versions";
        
        /// <summary>
        /// 最大版本数量
        /// </summary>
        public int MaxVersions { get; set; } = 10;
        
        /// <summary>
        /// 是否启用事务
        /// </summary>
        public bool EnableTransactions { get; set; } = false;
        
        /// <summary>
        /// 请求超时时间（毫秒）
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 30000;
        
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
    /// 文件信息
    /// </summary>
    public class FileInfoDto
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastWriteTime { get; set; }
        
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }
        
        /// <summary>
        /// 文件属性
        /// </summary>
        public string Attributes { get; set; } = string.Empty;
    }

    /// <summary>
    /// FileManager 命令结果
    /// </summary>
    public class FileManagerCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public FileManagerCommandType CommandType { get; set; }
        
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
        /// 文件信息列表
        /// </summary>
        public List<FileInfoDto>? Files { get; set; }
        
        /// <summary>
        /// 源文件路径
        /// </summary>
        public string? SourcePath { get; set; }
        
        /// <summary>
        /// 目标文件路径
        /// </summary>
        public string? TargetPath { get; set; }
    }

    /// <summary>
    /// FileManager 服务接口
    /// </summary>
    public interface IFileManagerService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<FileManagerCommandResult> ExecuteCommandAsync(FileManagerCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> CreateFileAsync(string filePath, string content);
        
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> ReadFileAsync(string filePath);
        
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">文件内容</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> WriteFileAsync(string filePath, string content);
        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="recursive">是否递归删除</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> DeleteFileAsync(string filePath, bool recursive = false);
        
        /// <summary>
        /// 列出文件
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="pattern">文件模式</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> ListFilesAsync(string directoryPath, string pattern = "*");
        
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> CopyFileAsync(string sourcePath, string targetPath, bool overwrite = false);
        
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="overwrite">是否覆盖</param>
        /// <returns>操作结果</returns>
        Task<FileManagerCommandResult> MoveFileAsync(string sourcePath, string targetPath, bool overwrite = false);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<FileManagerCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// FileManager 服务实现
    /// </summary>
    public class FileManagerService : IFileManagerService
    {
        private readonly FileManagerOptions _options;
        private readonly ILogger<FileManagerService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">FileManager 选项</param>
        /// <param name="logger">日志记录器</param>
        public FileManagerService(IOptions<FileManagerOptions> options, ILogger<FileManagerService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<FileManagerCommandResult> ExecuteCommandAsync(FileManagerCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case FileManagerCommandType.CreateFile:
                        if (parameters?.ContainsKey("filePath") == true && parameters?.ContainsKey("content") == true)
                        {
                            string filePath = parameters["filePath"];
                            string content = parameters["content"];
                            result = await CreateFileAsync(filePath, content);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "FilePath and Content parameters are required";
                        }
                        break;
                    
                    case FileManagerCommandType.ReadFile:
                        if (parameters?.ContainsKey("filePath") == true)
                        {
                            string filePath = parameters["filePath"];
                            result = await ReadFileAsync(filePath);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "FilePath parameter is required";
                        }
                        break;
                    
                    case FileManagerCommandType.WriteFile:
                        if (parameters?.ContainsKey("filePath") == true && parameters?.ContainsKey("content") == true)
                        {
                            string filePath = parameters["filePath"];
                            string content = parameters["content"];
                            result = await WriteFileAsync(filePath, content);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "FilePath and Content parameters are required";
                        }
                        break;
                    
                    case FileManagerCommandType.DeleteFile:
                        if (parameters?.ContainsKey("filePath") == true)
                        {
                            string filePath = parameters["filePath"];
                            bool recursive = parameters?.ContainsKey("recursive") == true ? bool.Parse(parameters["recursive"]) : false;
                            result = await DeleteFileAsync(filePath, recursive);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "FilePath parameter is required";
                        }
                        break;
                    
                    case FileManagerCommandType.ListFiles:
                        if (parameters?.ContainsKey("directoryPath") == true)
                        {
                            string directoryPath = parameters["directoryPath"];
                            string pattern = parameters?.ContainsKey("pattern") == true ? parameters["pattern"] : "*";
                            result = await ListFilesAsync(directoryPath, pattern);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "DirectoryPath parameter is required";
                        }
                        break;
                    
                    case FileManagerCommandType.CopyFile:
                        if (parameters?.ContainsKey("sourcePath") == true && parameters?.ContainsKey("targetPath") == true)
                        {
                            string sourcePath = parameters["sourcePath"];
                            string targetPath = parameters["targetPath"];
                            bool overwrite = parameters?.ContainsKey("overwrite") == true ? bool.Parse(parameters["overwrite"]) : false;
                            result = await CopyFileAsync(sourcePath, targetPath, overwrite);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "SourcePath and TargetPath parameters are required";
                        }
                        break;
                    
                    case FileManagerCommandType.MoveFile:
                        if (parameters?.ContainsKey("sourcePath") == true && parameters?.ContainsKey("targetPath") == true)
                        {
                            string sourcePath = parameters["sourcePath"];
                            string targetPath = parameters["targetPath"];
                            bool overwrite = parameters?.ContainsKey("overwrite") == true ? bool.Parse(parameters["overwrite"]) : false;
                            result = await MoveFileAsync(sourcePath, targetPath, overwrite);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "SourcePath and TargetPath parameters are required";
                        }
                        break;
                    
                    case FileManagerCommandType.VersionInfo:
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
        public async Task<FileManagerCommandResult> CreateFileAsync(string filePath, string content)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.CreateFile,
                SourcePath = filePath
            };

            try
            {
                _logger.LogInformation("创建文件: {FilePath}", filePath);
                
                // 确保目录存在
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // 创建文件
                File.WriteAllText(filePath, content);
                
                // 模拟版本控制
                if (_options.EnableVersioning)
                {
                    await CreateVersionAsync(filePath);
                }
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功创建文件: {filePath}");
                result.Results.Add($"文件大小: {new FileInfo(filePath).Length} 字节");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建文件时出错: {FilePath}", filePath);
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
        public async Task<FileManagerCommandResult> ReadFileAsync(string filePath)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.ReadFile,
                SourcePath = filePath
            };

            try
            {
                _logger.LogInformation("读取文件: {FilePath}", filePath);
                
                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"文件不存在: {filePath}";
                    return result;
                }
                
                // 读取文件内容
                var content = File.ReadAllText(filePath);
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功读取文件: {filePath}");
                result.Results.Add($"文件大小: {new FileInfo(filePath).Length} 字节");
                result.Results.Add($"文件内容: {content.Substring(0, Math.Min(100, content.Length))}{(content.Length > 100 ? "..." : "")}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取文件时出错: {FilePath}", filePath);
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
        public async Task<FileManagerCommandResult> WriteFileAsync(string filePath, string content)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.WriteFile,
                SourcePath = filePath
            };

            try
            {
                _logger.LogInformation("写入文件: {FilePath}", filePath);
                
                // 确保目录存在
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // 模拟版本控制
                if (_options.EnableVersioning && File.Exists(filePath))
                {
                    await CreateVersionAsync(filePath);
                }
                
                // 写入文件
                File.WriteAllText(filePath, content);
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功写入文件: {filePath}");
                result.Results.Add($"文件大小: {new FileInfo(filePath).Length} 字节");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "写入文件时出错: {FilePath}", filePath);
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
        public async Task<FileManagerCommandResult> DeleteFileAsync(string filePath, bool recursive = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.DeleteFile,
                SourcePath = filePath
            };

            try
            {
                _logger.LogInformation("删除文件: {FilePath}, 递归: {Recursive}", filePath, recursive);
                
                // 检查文件是否存在
                if (!File.Exists(filePath) && !Directory.Exists(filePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"文件或目录不存在: {filePath}";
                    return result;
                }
                
                // 删除文件或目录
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                else if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, recursive);
                }
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功删除: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除文件时出错: {FilePath}", filePath);
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
        public async Task<FileManagerCommandResult> ListFilesAsync(string directoryPath, string pattern = "*")
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.ListFiles,
                SourcePath = directoryPath
            };

            try
            {
                _logger.LogInformation("列出文件: {DirectoryPath}, 模式: {Pattern}", directoryPath, pattern);
                
                // 检查目录是否存在
                if (!Directory.Exists(directoryPath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"目录不存在: {directoryPath}";
                    return result;
                }
                
                // 列出文件
                var files = new List<FileInfoDto>();
                
                // 获取文件
                var fileInfos = new DirectoryInfo(directoryPath).GetFiles(pattern);
                foreach (var fileInfo in fileInfos)
                {
                    files.Add(new FileInfoDto
                    {
                        Path = fileInfo.FullName,
                        Name = fileInfo.Name,
                        Size = fileInfo.Length,
                        CreationTime = fileInfo.CreationTime,
                        LastWriteTime = fileInfo.LastWriteTime,
                        IsDirectory = false,
                        Attributes = fileInfo.Attributes.ToString()
                    });
                }
                
                // 获取目录
                var directoryInfos = new DirectoryInfo(directoryPath).GetDirectories();
                foreach (var dirInfo in directoryInfos)
                {
                    files.Add(new FileInfoDto
                    {
                        Path = dirInfo.FullName,
                        Name = dirInfo.Name,
                        Size = 0,
                        CreationTime = dirInfo.CreationTime,
                        LastWriteTime = dirInfo.LastWriteTime,
                        IsDirectory = true,
                        Attributes = dirInfo.Attributes.ToString()
                    });
                }
                
                // 模拟成功结果
                result.Success = true;
                result.Files = files;
                result.Results.Add($"成功列出目录: {directoryPath}");
                result.Results.Add($"文件数量: {fileInfos.Length}");
                result.Results.Add($"目录数量: {directoryInfos.Length}");
                result.Results.Add($"总数量: {files.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出文件时出错: {DirectoryPath}", directoryPath);
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
        public async Task<FileManagerCommandResult> CopyFileAsync(string sourcePath, string targetPath, bool overwrite = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.CopyFile,
                SourcePath = sourcePath,
                TargetPath = targetPath
            };

            try
            {
                _logger.LogInformation("复制文件: {SourcePath} -> {TargetPath}, 覆盖: {Overwrite}", sourcePath, targetPath, overwrite);
                
                // 检查源文件是否存在
                if (!File.Exists(sourcePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"源文件不存在: {sourcePath}";
                    return result;
                }
                
                // 确保目标目录存在
                var targetDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                
                // 检查目标文件是否存在
                if (File.Exists(targetPath) && !overwrite)
                {
                    result.Success = false;
                    result.ErrorMessage = $"目标文件已存在: {targetPath}";
                    return result;
                }
                
                // 复制文件
                File.Copy(sourcePath, targetPath, overwrite);
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功复制文件: {sourcePath} -> {targetPath}");
                result.Results.Add($"文件大小: {new FileInfo(targetPath).Length} 字节");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "复制文件时出错: {SourcePath} -> {TargetPath}", sourcePath, targetPath);
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
        public async Task<FileManagerCommandResult> MoveFileAsync(string sourcePath, string targetPath, bool overwrite = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.MoveFile,
                SourcePath = sourcePath,
                TargetPath = targetPath
            };

            try
            {
                _logger.LogInformation("移动文件: {SourcePath} -> {TargetPath}, 覆盖: {Overwrite}", sourcePath, targetPath, overwrite);
                
                // 检查源文件是否存在
                if (!File.Exists(sourcePath))
                {
                    result.Success = false;
                    result.ErrorMessage = $"源文件不存在: {sourcePath}";
                    return result;
                }
                
                // 确保目标目录存在
                var targetDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                
                // 检查目标文件是否存在
                if (File.Exists(targetPath) && !overwrite)
                {
                    result.Success = false;
                    result.ErrorMessage = $"目标文件已存在: {targetPath}";
                    return result;
                }
                
                // 移动文件
                File.Move(sourcePath, targetPath, overwrite);
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add($"成功移动文件: {sourcePath} -> {targetPath}");
                result.Results.Add($"文件大小: {new FileInfo(targetPath).Length} 字节");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移动文件时出错: {SourcePath} -> {TargetPath}", sourcePath, targetPath);
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
        public async Task<FileManagerCommandResult> GetVersionInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new FileManagerCommandResult
            {
                CommandType = FileManagerCommandType.VersionInfo
            };

            try
            {
                _logger.LogInformation("获取 FileManager 版本信息");
                
                // 模拟获取版本信息
                await Task.Delay(50); // 模拟操作
                
                // 模拟成功结果
                result.Success = true;
                result.Results.Add("FileManager AOT Engine");
                result.Results.Add($"版本: 1.0.0");
                result.Results.Add($".NET 版本: {Environment.Version}");
                result.Results.Add($"操作系统: {Environment.OSVersion}");
                result.Results.Add($"架构: {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                result.Results.Add($"AOT 编译: {AppContext.TryGetSwitch("PublishAot", out bool isAot) && isAot}");
                result.Results.Add($"工作目录: {_options.WorkingDirectory}");
                result.Results.Add($"启用版本控制: {_options.EnableVersioning}");
                result.Results.Add($"启用事务: {_options.EnableTransactions}");
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

            return await Task.FromResult(result);
        }

        /// <summary>
        /// 创建版本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>任务</returns>
        private async Task CreateVersionAsync(string filePath)
        {
            // 确保版本目录存在
            var versionDir = Path.Combine(_options.VersioningDirectory, Path.GetDirectoryName(filePath) ?? string.Empty);
            if (!Directory.Exists(versionDir))
            {
                Directory.CreateDirectory(versionDir);
            }
            
            // 创建版本文件
            var versionFileName = $"{Path.GetFileName(filePath)}.{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}";
            var versionPath = Path.Combine(versionDir, versionFileName);
            
            // 复制文件到版本目录
            File.Copy(filePath, versionPath, true);
            
            // 清理旧版本
            await CleanupOldVersionsAsync(versionDir, Path.GetFileName(filePath));
        }

        /// <summary>
        /// 清理旧版本
        /// </summary>
        /// <param name="versionDir">版本目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>任务</returns>
        private async Task CleanupOldVersionsAsync(string versionDir, string fileName)
        {
            // 获取版本文件
            var versionFiles = Directory.GetFiles(versionDir, $"{fileName}.*")
                .OrderByDescending(f => f)
                .ToList();
            
            // 删除超出最大版本数量的文件
            for (int i = _options.MaxVersions; i < versionFiles.Count; i++)
            {
                try
                {
                    File.Delete(versionFiles[i]);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "清理旧版本时出错: {FilePath}", versionFiles[i]);
                }
            }
        }
    }

    /// <summary>
    /// FileManager AOT 引擎
    /// </summary>
    public class FileManagerAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileManagerAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public FileManagerAotEngine(IServiceProvider serviceProvider, ILogger<FileManagerAotEngine> logger)
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
            _logger.LogInformation("FileManager AOT Engine 启动，参数: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var fileManagerService = _serviceProvider.GetRequiredService<IFileManagerService>();
            FileManagerCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "create":
                    case "createfile":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供文件路径和内容");
                            return 1;
                        }
                        string createPath = args[1];
                        string content = string.Join(" ", args.Skip(2));
                        result = await fileManagerService.CreateFileAsync(createPath, content);
                        break;
                    
                    case "read":
                    case "readfile":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供文件路径");
                            return 1;
                        }
                        string readPath = args[1];
                        result = await fileManagerService.ReadFileAsync(readPath);
                        break;
                    
                    case "write":
                    case "writefile":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供文件路径和内容");
                            return 1;
                        }
                        string writePath = args[1];
                        string writeContent = string.Join(" ", args.Skip(2));
                        result = await fileManagerService.WriteFileAsync(writePath, writeContent);
                        break;
                    
                    case "delete":
                    case "deletefile":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供文件路径");
                            return 1;
                        }
                        string deletePath = args[1];
                        bool recursive = args.Length > 2 && args[2].Equals("--recursive", StringComparison.OrdinalIgnoreCase);
                        result = await fileManagerService.DeleteFileAsync(deletePath, recursive);
                        break;
                    
                    case "list":
                    case "listfiles":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("错误: 需要提供目录路径");
                            return 1;
                        }
                        string listPath = args[1];
                        string pattern = args.Length > 2 ? args[2] : "*";
                        result = await fileManagerService.ListFilesAsync(listPath, pattern);
                        break;
                    
                    case "copy":
                    case "copyfile":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供源文件路径和目标文件路径");
                            return 1;
                        }
                        string sourcePath = args[1];
                        string targetPath = args[2];
                        bool overwrite = args.Length > 3 && args[3].Equals("--overwrite", StringComparison.OrdinalIgnoreCase);
                        result = await fileManagerService.CopyFileAsync(sourcePath, targetPath, overwrite);
                        break;
                    
                    case "move":
                    case "movefile":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("错误: 需要提供源文件路径和目标文件路径");
                            return 1;
                        }
                        string moveSource = args[1];
                        string moveTarget = args[2];
                        bool moveOverwrite = args.Length > 3 && args[3].Equals("--overwrite", StringComparison.OrdinalIgnoreCase);
                        result = await fileManagerService.MoveFileAsync(moveSource, moveTarget, moveOverwrite);
                        break;
                    
                    case "version":
                        result = await fileManagerService.GetVersionInfoAsync();
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
                
                if (result.Files != null && result.Files.Any())
                {
                    Console.WriteLine($"\n文件列表:");
                    foreach (var file in result.Files)
                    {
                        Console.WriteLine($"  {file.Name} {(file.IsDirectory ? "[目录]" : $"[{file.Size} 字节]")}");
                    }
                }
                
                if (!string.IsNullOrEmpty(result.SourcePath))
                {
                    Console.WriteLine($"源路径: {result.SourcePath}");
                }
                
                if (!string.IsNullOrEmpty(result.TargetPath))
                {
                    Console.WriteLine($"目标路径: {result.TargetPath}");
                }
            }
            
            return result?.Success == true ? 0 : 1;
        }
        
        /// <summary>
        /// 显示帮助信息
        /// </summary>
        private void ShowHelp()
        {
            Console.WriteLine("FileManager AOT Engine - .NET 10 AOT 编译的文件管理引擎");
            Console.WriteLine();
            Console.WriteLine("用法: filemanager_aot <命令> [参数]");
            Console.WriteLine();
            Console.WriteLine("命令:");
            Console.WriteLine("  createfile <filePath> <content>       创建文件");
            Console.WriteLine("  readfile <filePath>                   读取文件");
            Console.WriteLine("  writefile <filePath> <content>        写入文件");
            Console.WriteLine("  deletefile <filePath> [--recursive]    删除文件");
            Console.WriteLine("  listfiles <directoryPath> [pattern]   列出文件");
            Console.WriteLine("  copyfile <sourcePath> <targetPath> [--overwrite]  复制文件");
            Console.WriteLine("  movefile <sourcePath> <targetPath> [--overwrite]  移动文件");
            Console.WriteLine("  version                               显示版本信息");
            Console.WriteLine("  help                                  显示帮助信息");
            Console.WriteLine();
            Console.WriteLine("别名:");
            Console.WriteLine("  createfile -> create");
            Console.WriteLine("  readfile -> read");
            Console.WriteLine("  writefile -> write");
            Console.WriteLine("  deletefile -> delete");
            Console.WriteLine("  listfiles -> list");
            Console.WriteLine("  copyfile -> copy");
            Console.WriteLine("  movefile -> move");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine("  filemanager_aot create test.txt 'Hello World'");
            Console.WriteLine("  filemanager_aot read test.txt");
            Console.WriteLine("  filemanager_aot write test.txt 'Updated content'");
            Console.WriteLine("  filemanager_aot delete test.txt");
            Console.WriteLine("  filemanager_aot list .");
            Console.WriteLine("  filemanager_aot copy source.txt target.txt");
            Console.WriteLine("  filemanager_aot move source.txt target.txt");
            Console.WriteLine();
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
            // 创建主机构建器
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            // 配置选项
            builder.Services.Configure<FileManagerOptions>(builder.Configuration.GetSection("FileManager"));
            
            // 注册服务
            builder.Services.AddSingleton<IFileManagerService, FileManagerService>();
            builder.Services.AddSingleton<FileManagerAotEngine>();
            
            // 构建主机
            var host = builder.Build();
            
            // 获取引擎实例
            var engine = host.Services.GetRequiredService<FileManagerAotEngine>();
            
            // 执行命令行操作
            return await engine.ExecuteCommandLineAsync(args);
        }
    }
}

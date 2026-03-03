#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Docker.DotNet@3.125.15
#:package Docker.DotNet.BasicAuth@3.125.15
#:package Docker.DotNet.X509@3.125.15
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
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Docker.AOT
{
    /// <summary>
    /// Docker记录类型枚举
    /// </summary>
    public enum DockerCommandType
    {
        /// <summary>
        /// 容器列表
        /// </summary>
        ContainerList,
        /// <summary>
        /// 镜像列表
        /// </summary>
        ImageList,
        /// <summary>
        /// 运行容器
        /// </summary>
        ContainerRun,
        /// <summary>
        /// 停止容器
        /// </summary>
        ContainerStop,
        /// <summary>
        /// 删除容器
        /// </summary>
        ContainerRemove,
        /// <summary>
        /// 拉取镜像
        /// </summary>
        ImagePull,
        /// <summary>
        /// 删除镜像
        /// </summary>
        ImageRemove,
        /// <summary>
        /// Docker信息
        /// </summary>
        DockerInfo,
        /// <summary>
        /// Docker版本
        /// </summary>
        DockerVersion
    }

    /// <summary>
    /// Docker选项配置
    /// </summary>
    public class DockerOptions
    {
        /// <summary>
        /// Docker API 端点
        /// </summary>
        public string DockerEndpoint { get; set; } = "npipe://./pipe/docker_engine"; // Windows 默认
        
        /// <summary>
        /// 是否使用TLS
        /// </summary>
        public bool UseTls { get; set; } = false;
        
        /// <summary>
        /// TLS证书路径
        /// </summary>
        public string? TlsCertPath { get; set; }
        
        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        public int TimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 默认镜像
        /// </summary>
        public string DefaultImage { get; set; } = "nginx:latest";
        
        /// <summary>
        /// 默认容器名称前缀
        /// </summary>
        public string ContainerNamePrefix { get; set; } = "vsa-";
    }

    /// <summary>
    /// Docker查询结果
    /// </summary>
    public class DockerCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public DockerCommandType CommandType { get; set; }
        
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
        /// Docker API端点
        /// </summary>
        public string? DockerEndpoint { get; set; }
    }

    /// <summary>
    /// Docker服务接口
    /// </summary>
    public interface IDockerService
    {
        /// <summary>
        /// 执行Docker命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<DockerCommandResult> ExecuteCommandAsync(DockerCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 获取容器列表
        /// </summary>
        /// <param name="showAll">是否显示所有容器</param>
        /// <returns>容器列表</returns>
        Task<DockerCommandResult> GetContainersAsync(bool showAll = false);
        
        /// <summary>
        /// 获取镜像列表
        /// </summary>
        /// <param name="showAll">是否显示所有镜像</param>
        /// <returns>镜像列表</returns>
        Task<DockerCommandResult> GetImagesAsync(bool showAll = false);
        
        /// <summary>
        /// 运行容器
        /// </summary>
        /// <param name="image">镜像名称</param>
        /// <param name="containerName">容器名称</param>
        /// <param name="ports">端口映射</param>
        /// <returns>运行结果</returns>
        Task<DockerCommandResult> RunContainerAsync(string image, string? containerName = null, Dictionary<string, string>? ports = null);
        
        /// <summary>
        /// 停止容器
        /// </summary>
        /// <param name="containerIdOrName">容器ID或名称</param>
        /// <returns>停止结果</returns>
        Task<DockerCommandResult> StopContainerAsync(string containerIdOrName);
        
        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerIdOrName">容器ID或名称</param>
        /// <param name="force">是否强制删除</param>
        /// <returns>删除结果</returns>
        Task<DockerCommandResult> RemoveContainerAsync(string containerIdOrName, bool force = false);
        
        /// <summary>
        /// 拉取镜像
        /// </summary>
        /// <param name="image">镜像名称</param>
        /// <returns>拉取结果</returns>
        Task<DockerCommandResult> PullImageAsync(string image);
        
        /// <summary>
        /// 删除镜像
        /// </summary>
        /// <param name="imageIdOrName">镜像ID或名称</param>
        /// <param name="force">是否强制删除</param>
        /// <returns>删除结果</returns>
        Task<DockerCommandResult> RemoveImageAsync(string imageIdOrName, bool force = false);
        
        /// <summary>
        /// 获取Docker信息
        /// </summary>
        /// <returns>Docker信息</returns>
        Task<DockerCommandResult> GetDockerInfoAsync();
        
        /// <summary>
        /// 获取Docker版本
        /// </summary>
        /// <returns>Docker版本</returns>
        Task<DockerCommandResult> GetDockerVersionAsync();
    }

    /// <summary>
    /// Docker服务实现
    /// </summary>
    public class DockerService : IDockerService
    {
        private readonly DockerOptions _options;
        private readonly ILogger<DockerService> _logger;
        private IDockerClient? _dockerClient;
        private readonly object _clientLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Docker选项</param>
        /// <param name="logger">日志记录器</param>
        public DockerService(IOptions<DockerOptions> options, ILogger<DockerService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        private IDockerClient DockerClient
        {
            get
            {
                if (_dockerClient == null)
                {
                    lock (_clientLock)
                    {
                        if (_dockerClient == null)
                        {
                            _dockerClient = CreateDockerClient();
                        }
                    }
                }
                return _dockerClient;
            }
        }

        private IDockerClient CreateDockerClient()
        {
            _logger.LogInformation("Creating Docker client for endpoint: {Endpoint}", _options.DockerEndpoint);
            
            DockerClientConfiguration config;
            
            if (_options.UseTls && !string.IsNullOrEmpty(_options.TlsCertPath))
            {
                // TLS配置
                config = new DockerClientConfiguration(new Uri(_options.DockerEndpoint), new DockerCertificateCredentials(_options.TlsCertPath));
            }
            else
            {
                // 无TLS配置
                config = new DockerClientConfiguration(new Uri(_options.DockerEndpoint));
            }
            
            return config.CreateClient();
        }

        /// <inheritdoc/>
        public async Task<DockerCommandResult> ExecuteCommandAsync(DockerCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = commandType,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                switch (commandType)
                {
                    case DockerCommandType.ContainerList:
                        bool showAll = parameters?.ContainsKey("showAll") == true && bool.Parse(parameters["showAll"]);
                        result = await GetContainersAsync(showAll);
                        break;
                    
                    case DockerCommandType.ImageList:
                        bool showAllImages = parameters?.ContainsKey("showAll") == true && bool.Parse(parameters["showAll"]);
                        result = await GetImagesAsync(showAllImages);
                        break;
                    
                    case DockerCommandType.ContainerRun:
                        string image = parameters?.ContainsKey("image") == true ? parameters["image"] : _options.DefaultImage;
                        string? containerName = parameters?.ContainsKey("containerName") == true ? parameters["containerName"] : null;
                        Dictionary<string, string>? ports = null;
                        if (parameters?.ContainsKey("ports") == true)
                        {
                            ports = new Dictionary<string, string>();
                            foreach (var portMapping in parameters["ports"].Split(';'))
                            {
                                var parts = portMapping.Split(':');
                                if (parts.Length == 2)
                                {
                                    ports[parts[0]] = parts[1];
                                }
                            }
                        }
                        result = await RunContainerAsync(image, containerName, ports);
                        break;
                    
                    case DockerCommandType.ContainerStop:
                        if (parameters?.ContainsKey("containerIdOrName") == true)
                        {
                            result = await StopContainerAsync(parameters["containerIdOrName"]);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "containerIdOrName parameter is required";
                        }
                        break;
                    
                    case DockerCommandType.ContainerRemove:
                        if (parameters?.ContainsKey("containerIdOrName") == true)
                        {
                            bool force = parameters?.ContainsKey("force") == true && bool.Parse(parameters["force"]);
                            result = await RemoveContainerAsync(parameters["containerIdOrName"], force);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "containerIdOrName parameter is required";
                        }
                        break;
                    
                    case DockerCommandType.ImagePull:
                        if (parameters?.ContainsKey("image") == true)
                        {
                            result = await PullImageAsync(parameters["image"]);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "image parameter is required";
                        }
                        break;
                    
                    case DockerCommandType.ImageRemove:
                        if (parameters?.ContainsKey("imageIdOrName") == true)
                        {
                            bool force = parameters?.ContainsKey("force") == true && bool.Parse(parameters["force"]);
                            result = await RemoveImageAsync(parameters["imageIdOrName"], force);
                        }
                        else
                        {
                            result.Success = false;
                            result.ErrorMessage = "imageIdOrName parameter is required";
                        }
                        break;
                    
                    case DockerCommandType.DockerInfo:
                        result = await GetDockerInfoAsync();
                        break;
                    
                    case DockerCommandType.DockerVersion:
                        result = await GetDockerVersionAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing Docker command: {CommandType}", commandType);
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
        public async Task<DockerCommandResult> GetContainersAsync(bool showAll = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ContainerList,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Getting containers, showAll: {ShowAll}", showAll);
                
                var containers = await DockerClient.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = showAll
                });
                
                foreach (var container in containers)
                {
                    result.Results.Add($"{container.ID.Substring(0, 12)} {container.Names.FirstOrDefault()?.TrimStart('/') ?? "-"} {container.Image} {container.State} {container.Status}");
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting containers");
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
        public async Task<DockerCommandResult> GetImagesAsync(bool showAll = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ImageList,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Getting images, showAll: {ShowAll}", showAll);
                
                var images = await DockerClient.Images.ListImagesAsync(new ImagesListParameters
                {
                    All = showAll
                });
                
                foreach (var image in images)
                {
                    string tags = image.RepoTags != null && image.RepoTags.Any() ? string.Join(", ", image.RepoTags) : "<none>:<none>";
                    result.Results.Add($"{image.ID.Substring(0, 12)} {tags} {image.Size} bytes");
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images");
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
        public async Task<DockerCommandResult> RunContainerAsync(string image, string? containerName = null, Dictionary<string, string>? ports = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ContainerRun,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Running container for image: {Image}, name: {Name}", image, containerName);
                
                var createParams = new CreateContainerParameters
                {
                    Image = image,
                    Name = containerName ?? $"{_options.ContainerNamePrefix}{Guid.NewGuid().ToString().Substring(0, 8)}",
                    ExposedPorts = new Dictionary<string, EmptyStruct>(),
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>()
                    }
                };
                
                // 处理端口映射
                if (ports != null && ports.Any())
                {
                    foreach (var portMapping in ports)
                    {
                        // 内部端口:外部端口
                        var internalPort = portMapping.Key;
                        var externalPort = portMapping.Value;
                        
                        createParams.ExposedPorts.Add(internalPort, default);
                        createParams.HostConfig.PortBindings.Add(internalPort, new List<PortBinding>
                        {
                            new PortBinding { HostPort = externalPort }
                        });
                    }
                }
                
                // 创建容器
                var createResponse = await DockerClient.Containers.CreateContainerAsync(createParams);
                
                // 启动容器
                await DockerClient.Containers.StartContainerAsync(createResponse.ID, new ContainerStartParameters());
                
                result.Results.Add($"Container created and started: {createResponse.ID.Substring(0, 12)} {createParams.Name}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running container for image: {Image}", image);
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
        public async Task<DockerCommandResult> StopContainerAsync(string containerIdOrName)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ContainerStop,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Stopping container: {ContainerIdOrName}", containerIdOrName);
                
                await DockerClient.Containers.StopContainerAsync(containerIdOrName, new ContainerStopParameters());
                
                result.Results.Add($"Container stopped: {containerIdOrName}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping container: {ContainerIdOrName}", containerIdOrName);
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
        public async Task<DockerCommandResult> RemoveContainerAsync(string containerIdOrName, bool force = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ContainerRemove,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Removing container: {ContainerIdOrName}, force: {Force}", containerIdOrName, force);
                
                await DockerClient.Containers.RemoveContainerAsync(containerIdOrName, new ContainerRemoveParameters
                {
                    Force = force
                });
                
                result.Results.Add($"Container removed: {containerIdOrName}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing container: {ContainerIdOrName}", containerIdOrName);
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
        public async Task<DockerCommandResult> PullImageAsync(string image)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ImagePull,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Pulling image: {Image}", image);
                
                // 处理镜像名称，提取仓库和标签
                string repo, tag;
                if (image.Contains(":"))
                {
                    var parts = image.Split(':');
                    repo = parts[0];
                    tag = parts[1];
                }
                else
                {
                    repo = image;
                    tag = "latest";
                }
                
                // 拉取镜像
                await DockerClient.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = repo,
                        Tag = tag
                    },
                    null,
                    new Progress<JSONMessage>(msg =>
                    {
                        if (!string.IsNullOrEmpty(msg.Status))
                        {
                            _logger.LogDebug("Image pull status: {Status}", msg.Status);
                        }
                    }));
                
                result.Results.Add($"Image pulled successfully: {image}");
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pulling image: {Image}", image);
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
        public async Task<DockerCommandResult> RemoveImageAsync(string imageIdOrName, bool force = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.ImageRemove,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Removing image: {ImageIdOrName}, force: {Force}", imageIdOrName, force);
                
                var removeResult = await DockerClient.Images.RemoveImageAsync(imageIdOrName, new ImagesRemoveParameters
                {
                    Force = force
                });
                
                foreach (var msg in removeResult)
                {
                    if (!string.IsNullOrEmpty(msg.Deleted))
                    {
                        result.Results.Add($"Image deleted: {msg.Deleted}");
                    }
                    if (!string.IsNullOrEmpty(msg.Untagged))
                    {
                        result.Results.Add($"Image untagged: {msg.Untagged}");
                    }
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing image: {ImageIdOrName}", imageIdOrName);
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
        public async Task<DockerCommandResult> GetDockerInfoAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.DockerInfo,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Getting Docker info");
                
                var info = await DockerClient.System.GetSystemInfoAsync();
                
                result.Results.Add($"ServerVersion: {info.ServerVersion}");
                result.Results.Add($"Containers: {info.Containers} (Running: {info.ContainersRunning}, Paused: {info.ContainersPaused}, Stopped: {info.ContainersStopped})");
                result.Results.Add($"Images: {info.Images}");
                result.Results.Add($"OperatingSystem: {info.OperatingSystem}");
                result.Results.Add($"Architecture: {info.Architecture}");
                result.Results.Add($"CPUs: {info.NCPU}");
                result.Results.Add($"Total Memory: {info.MemTotal} bytes");
                result.Results.Add($"Docker Root Dir: {info.DockerRootDir}");
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Docker info");
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
        public async Task<DockerCommandResult> GetDockerVersionAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new DockerCommandResult
            {
                CommandType = DockerCommandType.DockerVersion,
                DockerEndpoint = _options.DockerEndpoint
            };

            try
            {
                _logger.LogInformation("Getting Docker version");
                
                var version = await DockerClient.System.GetVersionAsync();
                
                result.Results.Add($"Version: {version.Version}");
                result.Results.Add($"ApiVersion: {version.ApiVersion}");
                result.Results.Add($"MinAPIVersion: {version.MinAPIVersion}");
                result.Results.Add($"GitCommit: {version.GitCommit}");
                result.Results.Add($"GoVersion: {version.GoVersion}");
                result.Results.Add($"Os: {version.Os}");
                result.Results.Add($"Arch: {version.Arch}");
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Docker version");
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
    }

    /// <summary>
    /// Docker AOT 引擎
    /// </summary>
    public class DockerAotEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DockerAotEngine> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="logger">日志记录器</param>
        public DockerAotEngine(IServiceProvider serviceProvider, ILogger<DockerAotEngine> logger)
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
            _logger.LogInformation("Docker AOT Engine starting with args: {Args}", string.Join(" ", args));
            
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var dnsService = _serviceProvider.GetRequiredService<IDockerService>();
            DockerCommandResult? result = null;

            try
            {
                switch (command)
                {
                    case "ps":
                    case "container":
                    case "containers":
                        bool showAll = args.Contains("-a") || args.Contains("--all");
                        result = await dnsService.GetContainersAsync(showAll);
                        break;
                    
                    case "images":
                    case "image":
                        bool showAllImages = args.Contains("-a") || args.Contains("--all");
                        result = await dnsService.GetImagesAsync(showAllImages);
                        break;
                    
                    case "run":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Image name is required");
                            return 1;
                        }
                        
                        string image = args[1];
                        string? containerName = null;
                        Dictionary<string, string>? ports = null;
                        
                        // 解析容器名称和端口映射
                        for (int i = 2; i < args.Length; i++)
                        {
                            if (args[i].StartsWith("--name"))
                            {
                                if (i + 1 < args.Length)
                                {
                                    containerName = args[i + 1];
                                    i++;
                                }
                            }
                            else if (args[i].StartsWith("-p") || args[i].StartsWith("--publish"))
                            {
                                if (i + 1 < args.Length)
                                {
                                    if (ports == null)
                                    {
                                        ports = new Dictionary<string, string>();
                                    }
                                    var portMapping = args[i + 1];
                                    var parts = portMapping.Split(':');
                                    if (parts.Length == 2)
                                    {
                                        ports[parts[1]] = parts[0]; // 内部端口:外部端口
                                    }
                                    i++;
                                }
                            }
                        }
                        
                        result = await dnsService.RunContainerAsync(image, containerName, ports);
                        break;
                    
                    case "stop":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Container ID or name is required");
                            return 1;
                        }
                        result = await dnsService.StopContainerAsync(args[1]);
                        break;
                    
                    case "rm":
                    case "remove":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Container ID or name is required");
                            return 1;
                        }
                        bool force = args.Contains("-f") || args.Contains("--force");
                        result = await dnsService.RemoveContainerAsync(args[1], force);
                        break;
                    
                    case "pull":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Image name is required");
                            return 1;
                        }
                        result = await dnsService.PullImageAsync(args[1]);
                        break;
                    
                    case "rmi":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("Error: Image ID or name is required");
                            return 1;
                        }
                        bool forceImage = args.Contains("-f") || args.Contains("--force");
                        result = await dnsService.RemoveImageAsync(args[1], forceImage);
                        break;
                    
                    case "info":
                        result = await dnsService.GetDockerInfoAsync();
                        break;
                    
                    case "version":
                        result = await dnsService.GetDockerVersionAsync();
                        break;
                    
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return 0;
                    
                    default:
                        Console.WriteLine($"Error: Unknown command '{command}'");
                        ShowHelp();
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Error executing command: {Command}", command);
                return 1;
            }

            if (result != null)
            {
                DisplayResult(result);
                return result.Success ? 0 : 1;
            }

            return 0;
        }

        private void ShowHelp()
        {
            Console.WriteLine("Docker AOT Command Line Tool");
            Console.WriteLine("=============================");
            Console.WriteLine("Usage: docker_aot <command> [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  ps, container, containers  List containers");
            Console.WriteLine("  images, image              List images");
            Console.WriteLine("  run <image> [options]      Run a container");
            Console.WriteLine("  stop <container>           Stop a container");
            Console.WriteLine("  rm, remove <container>     Remove a container");
            Console.WriteLine("  pull <image>               Pull an image");
            Console.WriteLine("  rmi <image>                Remove an image");
            Console.WriteLine("  info                       Display system-wide information");
            Console.WriteLine("  version                    Show the Docker version information");
            Console.WriteLine("  help, --help, -h           Show this help message");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -a, --all                  Show all containers/images");
            Console.WriteLine("  -f, --force                Force removal");
            Console.WriteLine("  --name <name>              Assign a name to the container");
            Console.WriteLine("  -p, --publish <port>       Publish a container's port(s) to the host");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  docker_aot ps -a           List all containers");
            Console.WriteLine("  docker_aot run nginx       Run a new nginx container");
            Console.WriteLine("  docker_aot run -p 8080:80 --name mynginx nginx");
            Console.WriteLine("  docker_aot stop mynginx    Stop container named mynginx");
            Console.WriteLine("  docker_aot rm mynginx      Remove container named mynginx");
            Console.WriteLine("  docker_aot pull ubuntu     Pull the latest ubuntu image");
            Console.WriteLine("  docker_aot images          List all images");
        }

        private void DisplayResult(DockerCommandResult result)
        {
            Console.WriteLine($"Command: {result.CommandType}");
            Console.WriteLine($"Endpoint: {result.DockerEndpoint}");
            Console.WriteLine($"Status: {(result.Success ? "Success" : "Failed")}");
            Console.WriteLine($"Time: {result.ExecutionTimeMs} ms");
            
            if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                Console.WriteLine($"Error: {result.ErrorMessage}");
            }
            else if (result.Results.Any())
            {
                Console.WriteLine("Results:");
                foreach (var item in result.Results)
                {
                    Console.WriteLine($"  {item}");
                }
            }
            else
            {
                Console.WriteLine("No results returned");
            }
        }
    }
}

// 扩展方法
public static class DockerServiceExtensions
{
    /// <summary>
    /// 添加Docker服务到依赖注入容器
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddDocker(this IServiceCollection services)
    {
        services.AddSingleton<IDockerService, Docker.AOT.DockerService>();
        services.AddSingleton<Docker.AOT.DockerAotEngine>();
        return services;
    }
}

// 主程序
class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("docker_aot.setting.json", optional: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<Docker.AOT.DockerOptions>(context.Configuration.GetSection("Docker"));
                services.AddDocker();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        var engine = host.Services.GetRequiredService<Docker.AOT.DockerAotEngine>();
        var exitCode = await engine.ExecuteCommandLineAsync(args);
        Environment.Exit(exitCode);
    }
}
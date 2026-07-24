#:sdk Microsoft.NET.Sdk
#:package SharpPcap@6.3.0
#:package PacketDotNet@1.4.7
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Text.Json@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace SharpPcapSkill
{
    // 网络接口信息
    public class NetworkInterfaceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }

    // 数据包信息
    public class PacketInfo
    {
        public DateTime Timestamp { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public int Length { get; set; }
        public string Protocol { get; set; }
        public string Info { get; set; }
    }

    // 数据包分析结果
    public class PacketAnalysisResult
    {
        public int PacketCount { get; set; }
        public List<PacketInfo> Packets { get; set; } = new List<PacketInfo>();
        public Dictionary<string, int> ProtocolDistribution { get; set; } = new Dictionary<string, int>();
    }

    // 网络统计信息
    public class NetworkStats
    {
        public long BytesReceived { get; set; }
        public long BytesSent { get; set; }
        public int PacketsReceived { get; set; }
        public int PacketsSent { get; set; }
        public int ErrorsReceived { get; set; }
        public int ErrorsSent { get; set; }
    }

    // 网络数据包捕获服务接口
    public interface IPacketCaptureService
    {
        Task<IEnumerable<NetworkInterfaceInfo>> GetNetworkInterfacesAsync();
        Task StartCaptureAsync(string interfaceName, string filter, string outputPath, int durationSeconds);
        Task StopCaptureAsync();
    }

    // 网络数据包分析服务接口
    public interface IPacketAnalyzerService
    {
        Task<PacketAnalysisResult> AnalyzeFileAsync(string filePath, string protocol);
        Task<PacketAnalysisResult> AnalyzePacketAsync(byte[] packetData);
    }

    // 网络数据包过滤服务接口
    public interface IPacketFilterService
    {
        Task FilterFileAsync(string inputPath, string filterExpression, string outputPath);
        Task<byte[]> FilterPacketAsync(byte[] packetData, string filterExpression);
    }

    // 网络统计信息服务接口
    public interface INetworkStatsService
    {
        Task<NetworkStats> GetStatsAsync(string interfaceName);
        Task StartStatsMonitoringAsync(string interfaceName, int intervalSeconds, Action<NetworkStats> callback);
        Task StopStatsMonitoringAsync();
    }

    // 网络数据包捕获服务实现
    public class PacketCaptureService : IPacketCaptureService
    {
        private readonly ILogger<PacketCaptureService> _logger;
        private ICaptureDevice _captureDevice;
        private bool _isCapturing;

        public PacketCaptureService(ILogger<PacketCaptureService> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<NetworkInterfaceInfo>> GetNetworkInterfacesAsync()
        {
            var devices = CaptureDeviceList.Instance;
            var interfaceInfos = new List<NetworkInterfaceInfo>();

            foreach (var device in devices)
            {
                var info = new NetworkInterfaceInfo
                {
                    Name = device.Name,
                    Description = device.Description
                };

                // 尝试获取 MAC 地址和 IP 地址
                if (device is LibPcapLiveDevice liveDevice)
                {
                    info.MacAddress = liveDevice.MacAddress?.ToString() ?? "N/A";
                    info.IpAddress = liveDevice.Addresses?.FirstOrDefault(a => a.Addr.type == Sockaddr.AddressTypes.AF_INET)?.Addr.ToString() ?? "N/A";
                }

                interfaceInfos.Add(info);
            }

            return Task.FromResult<IEnumerable<NetworkInterfaceInfo>>(interfaceInfos);
        }

        public async Task StartCaptureAsync(string interfaceName, string filter, string outputPath, int durationSeconds)
        {
            try
            {
                // 获取指定的网络接口
                var device = CaptureDeviceList.Instance.FirstOrDefault(d => d.Name == interfaceName);
                if (device == null)
                {
                    throw new ArgumentException($"找不到名称为 {interfaceName} 的网络接口");
                }

                _captureDevice = device;
                
                // 打开设备
                _captureDevice.Open();
                
                // 设置过滤器
                if (!string.IsNullOrEmpty(filter))
                {
                    _captureDevice.Filter = filter;
                }

                // 创建输出文件
                using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                using var pcapWriter = new SharpPcap.PcapWriter(fileStream);

                // 开始捕获
                _isCapturing = true;
                _logger.LogInformation($"开始在接口 {interfaceName} 上捕获数据包，过滤器: {filter}");

                // 注册数据包接收事件
                _captureDevice.OnPacketArrival += (sender, e) =>
                {
                    if (_isCapturing)
                    {
                        pcapWriter.Write(e.Packet);
                    }
                };

                // 开始捕获
                _captureDevice.StartCapture();

                // 等待指定的时间
                await Task.Delay(durationSeconds * 1000);

                // 停止捕获
                await StopCaptureAsync();
                
                _logger.LogInformation($"捕获完成，数据已保存到 {outputPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "捕获数据包时发生错误");
                throw;
            }
        }

        public Task StopCaptureAsync()
        {
            _isCapturing = false;
            
            if (_captureDevice != null)
            {
                _captureDevice.StopCapture();
                _captureDevice.Close();
                _captureDevice = null;
            }

            return Task.CompletedTask;
        }
    }

    // 网络数据包分析服务实现
    public class PacketAnalyzerService : IPacketAnalyzerService
    {
        private readonly ILogger<PacketAnalyzerService> _logger;

        public PacketAnalyzerService(ILogger<PacketAnalyzerService> logger)
        {
            _logger = logger;
        }

        public async Task<PacketAnalysisResult> AnalyzeFileAsync(string filePath, string protocol)
        {
            try
            {
                var result = new PacketAnalysisResult();
                
                // 打开捕获文件
                using var device = new CaptureFileReaderDevice(filePath);
                device.Open();

                // 分析数据包
                device.OnPacketArrival += (sender, e) =>
                {
                    var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                    var packetInfo = AnalyzePacket(packet);
                    
                    if (string.IsNullOrEmpty(protocol) || packetInfo.Protocol.Equals(protocol, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Packets.Add(packetInfo);
                        result.PacketCount++;

                        // 更新协议分布
                        if (!result.ProtocolDistribution.ContainsKey(packetInfo.Protocol))
                        {
                            result.ProtocolDistribution[packetInfo.Protocol] = 0;
                        }
                        result.ProtocolDistribution[packetInfo.Protocol]++;
                    }
                };

                // 开始分析
                device.Capture();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "分析数据包文件时发生错误");
                throw;
            }
        }

        public Task<PacketAnalysisResult> AnalyzePacketAsync(byte[] packetData)
        {
            try
            {
                var result = new PacketAnalysisResult();
                var packet = Packet.ParsePacket(LinkLayers.Ethernet, packetData);
                var packetInfo = AnalyzePacket(packet);
                
                result.Packets.Add(packetInfo);
                result.PacketCount = 1;
                result.ProtocolDistribution[packetInfo.Protocol] = 1;

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "分析单个数据包时发生错误");
                throw;
            }
        }

        private PacketInfo AnalyzePacket(Packet packet)
        {
            var info = new PacketInfo
            {
                Timestamp = DateTime.Now
            };

            // 分析以太网帧
            if (packet is EthernetPacket ethernetPacket)
            {
                info.Source = ethernetPacket.SourceHardwareAddress.ToString();
                info.Destination = ethernetPacket.DestinationHardwareAddress.ToString();
                info.Length = packet.TotalPacketLength;

                // 分析网络层协议
                var ipPacket = packet.Extract<IpPacket>();
                if (ipPacket != null)
                {
                    info.Source = ipPacket.SourceAddress.ToString();
                    info.Destination = ipPacket.DestinationAddress.ToString();
                    info.Protocol = ipPacket.Protocol.ToString();

                    // 分析传输层协议
                    var tcpPacket = packet.Extract<TcpPacket>();
                    if (tcpPacket != null)
                    {
                        info.Protocol = "TCP";
                        info.Info = $"{tcpPacket.SourcePort} -> {tcpPacket.DestinationPort}";
                    }

                    var udpPacket = packet.Extract<UdpPacket>();
                    if (udpPacket != null)
                    {
                        info.Protocol = "UDP";
                        info.Info = $"{udpPacket.SourcePort} -> {udpPacket.DestinationPort}";
                    }

                    var icmpPacket = packet.Extract<IcmpV4Packet>();
                    if (icmpPacket != null)
                    {
                        info.Protocol = "ICMP";
                        info.Info = icmpPacket.TypeCode.ToString();
                    }
                }
                else
                {
                    info.Protocol = "Ethernet";
                }
            }

            return info;
        }
    }

    // 网络数据包过滤服务实现
    public class PacketFilterService : IPacketFilterService
    {
        private readonly ILogger<PacketFilterService> _logger;

        public PacketFilterService(ILogger<PacketFilterService> logger)
        {
            _logger = logger;
        }

        public async Task FilterFileAsync(string inputPath, string filterExpression, string outputPath)
        {
            try
            {
                // 打开输入文件
                using var inputDevice = new CaptureFileReaderDevice(inputPath);
                inputDevice.Open();

                // 创建输出文件
                using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                using var pcapWriter = new SharpPcap.PcapWriter(outputStream);

                // 应用过滤器
                inputDevice.OnPacketArrival += (sender, e) =>
                {
                    // 这里简化处理，实际应用中可能需要更复杂的过滤逻辑
                    var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                    if (MatchesFilter(packet, filterExpression))
                    {
                        pcapWriter.Write(e.Packet);
                    }
                };

                // 开始过滤
                inputDevice.Capture();

                _logger.LogInformation($"过滤完成，结果已保存到 {outputPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "过滤数据包文件时发生错误");
                throw;
            }
        }

        public Task<byte[]> FilterPacketAsync(byte[] packetData, string filterExpression)
        {
            try
            {
                var packet = Packet.ParsePacket(LinkLayers.Ethernet, packetData);
                if (MatchesFilter(packet, filterExpression))
                {
                    return Task.FromResult(packetData);
                }
                return Task.FromResult(Array.Empty<byte>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "过滤单个数据包时发生错误");
                throw;
            }
        }

        private bool MatchesFilter(Packet packet, string filterExpression)
        {
            // 简化的过滤器匹配逻辑
            // 实际应用中可能需要使用更复杂的 BPF 过滤器实现
            if (string.IsNullOrEmpty(filterExpression))
            {
                return true;
            }

            // 检查协议类型
            if (filterExpression.Contains("tcp", StringComparison.OrdinalIgnoreCase) && packet.Extract<TcpPacket>() != null)
            {
                return true;
            }

            if (filterExpression.Contains("udp", StringComparison.OrdinalIgnoreCase) && packet.Extract<UdpPacket>() != null)
            {
                return true;
            }

            if (filterExpression.Contains("icmp", StringComparison.OrdinalIgnoreCase) && packet.Extract<IcmpV4Packet>() != null)
            {
                return true;
            }

            return false;
        }
    }

    // 网络统计信息服务实现
    public class NetworkStatsService : INetworkStatsService
    {
        private readonly ILogger<NetworkStatsService> _logger;
        private bool _isMonitoring;
        private CancellationTokenSource _cts;

        public NetworkStatsService(ILogger<NetworkStatsService> logger)
        {
            _logger = logger;
        }

        public Task<NetworkStats> GetStatsAsync(string interfaceName)
        {
            try
            {
                // 获取指定的网络接口
                var device = CaptureDeviceList.Instance.FirstOrDefault(d => d.Name == interfaceName);
                if (device == null)
                {
                    throw new ArgumentException($"找不到名称为 {interfaceName} 的网络接口");
                }

                // 这里简化处理，实际应用中可能需要使用系统 API 获取更详细的统计信息
                var stats = new NetworkStats
                {
                    BytesReceived = 0,
                    BytesSent = 0,
                    PacketsReceived = 0,
                    PacketsSent = 0,
                    ErrorsReceived = 0,
                    ErrorsSent = 0
                };

                return Task.FromResult(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取网络统计信息时发生错误");
                throw;
            }
        }

        public async Task StartStatsMonitoringAsync(string interfaceName, int intervalSeconds, Action<NetworkStats> callback)
        {
            _isMonitoring = true;
            _cts = new CancellationTokenSource();

            try
            {
                while (_isMonitoring && !_cts.Token.IsCancellationRequested)
                {
                    var stats = await GetStatsAsync(interfaceName);
                    callback(stats);
                    await Task.Delay(intervalSeconds * 1000, _cts.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // 正常取消，忽略异常
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "监控网络统计信息时发生错误");
            }
        }

        public Task StopStatsMonitoringAsync()
        {
            _isMonitoring = false;
            _cts?.Cancel();
            _cts?.Dispose();
            return Task.CompletedTask;
        }
    }

    // 命令行工具
    public class SharpPcapCli
    {
        private readonly IPacketCaptureService _captureService;
        private readonly IPacketAnalyzerService _analyzerService;
        private readonly IPacketFilterService _filterService;
        private readonly INetworkStatsService _statsService;
        private readonly ILogger<SharpPcapCli> _logger;

        public SharpPcapCli(
            IPacketCaptureService captureService,
            IPacketAnalyzerService analyzerService,
            IPacketFilterService filterService,
            INetworkStatsService statsService,
            ILogger<SharpPcapCli> logger)
        {
            _captureService = captureService;
            _analyzerService = analyzerService;
            _filterService = filterService;
            _statsService = statsService;
            _logger = logger;
        }

        public async Task<int> RunAsync(string[] args)
        {
            // 创建根命令
            var rootCommand = new RootCommand("SharpPcap 命令行工具");

            // 捕获命令
            var captureCommand = new Command("capture", "网络数据包捕获");
            var interfaceOption = new Option<string>("--interface", "网络接口名称");
            var filterOption = new Option<string>("--filter", "数据包过滤器表达式");
            var outputOption = new Option<string>("--output", "输出文件路径");
            var durationOption = new Option<int>("--duration", "捕获持续时间（秒）");

            captureCommand.AddOption(interfaceOption);
            captureCommand.AddOption(filterOption);
            captureCommand.AddOption(outputOption);
            captureCommand.AddOption(durationOption);

            captureCommand.Handler = CommandHandler.Create<string, string, string, int>(async (interfaceName, filter, output, duration) =>
            {
                await _captureService.StartCaptureAsync(interfaceName, filter, output, duration);
            });

            // 分析命令
            var analyzeCommand = new Command("analyze", "网络数据包分析");
            var inputOption = new Option<string>("--input", "输入文件路径");
            var protocolOption = new Option<string>("--protocol", "协议类型");

            analyzeCommand.AddOption(inputOption);
            analyzeCommand.AddOption(protocolOption);

            analyzeCommand.Handler = CommandHandler.Create<string, string>(async (input, protocol) =>
            {
                var result = await _analyzerService.AnalyzeFileAsync(input, protocol);
                Console.WriteLine($"分析结果: 共 {result.PacketCount} 个数据包");
                Console.WriteLine("协议分布:");
                foreach (var kvp in result.ProtocolDistribution)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
                Console.WriteLine("前 5 个数据包:");
                foreach (var packet in result.Packets.Take(5))
                {
                    Console.WriteLine($"  时间: {packet.Timestamp}, 源: {packet.Source}, 目标: {packet.Destination}, 协议: {packet.Protocol}, 信息: {packet.Info}");
                }
            });

            // 过滤命令
            var filterCommand = new Command("filter", "网络数据包过滤");
            var filterInputOption = new Option<string>("--input", "输入文件路径");
            var expressionOption = new Option<string>("--expression", "过滤表达式");
            var filterOutputOption = new Option<string>("--output", "输出文件路径");

            filterCommand.AddOption(filterInputOption);
            filterCommand.AddOption(expressionOption);
            filterCommand.AddOption(filterOutputOption);

            filterCommand.Handler = CommandHandler.Create<string, string, string>(async (input, expression, output) =>
            {
                await _filterService.FilterFileAsync(input, expression, output);
                Console.WriteLine($"过滤完成，结果已保存到 {output}");
            });

            // 统计命令
            var statsCommand = new Command("stats", "网络统计信息");
            var statsInterfaceOption = new Option<string>("--interface", "网络接口名称");
            var intervalOption = new Option<int>("--interval", "统计间隔（秒）");

            statsCommand.AddOption(statsInterfaceOption);
            statsCommand.AddOption(intervalOption);

            statsCommand.Handler = CommandHandler.Create<string, int>(async (interfaceName, interval) =>
            {
                Console.WriteLine($"开始监控接口 {interfaceName} 的统计信息，间隔 {interval} 秒...");
                Console.WriteLine("按 Ctrl+C 停止监控");

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                var monitoringTask = _statsService.StartStatsMonitoringAsync(interfaceName, interval, stats =>
                {
                    Console.WriteLine($"{DateTime.Now}: 接收: {stats.BytesReceived} 字节, 发送: {stats.BytesSent} 字节, 接收包数: {stats.PacketsReceived}, 发送包数: {stats.PacketsSent}");
                });

                try
                {
                    await Task.Delay(Timeout.Infinite, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // 正常取消
                }
                finally
                {
                    await _statsService.StopStatsMonitoringAsync();
                    Console.WriteLine("监控停止");
                }
            });

            // 接口命令
            var interfacesCommand = new Command("interfaces", "列出所有网络接口");
            interfacesCommand.Handler = CommandHandler.Create(async () =>
            {
                var interfaces = await _captureService.GetNetworkInterfacesAsync();
                Console.WriteLine("可用的网络接口:");
                foreach (var iface in interfaces)
                {
                    Console.WriteLine($"  名称: {iface.Name}");
                    Console.WriteLine($"  描述: {iface.Description}");
                    Console.WriteLine($"  MAC 地址: {iface.MacAddress}");
                    Console.WriteLine($"  IP 地址: {iface.IpAddress}");
                    Console.WriteLine();
                }
            });

            // 添加命令到根命令
            rootCommand.AddCommand(captureCommand);
            rootCommand.AddCommand(analyzeCommand);
            rootCommand.AddCommand(filterCommand);
            rootCommand.AddCommand(statsCommand);
            rootCommand.AddCommand(interfacesCommand);

            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
    }

    // 主程序
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // 创建服务容器
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .AddSingleton<IPacketCaptureService, PacketCaptureService>()
                .AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>()
                .AddSingleton<IPacketFilterService, PacketFilterService>()
                .AddSingleton<INetworkStatsService, NetworkStatsService>()
                .AddSingleton<SharpPcapCli>()
                .BuildServiceProvider();

            // 获取命令行工具
            var cli = serviceProvider.GetRequiredService<SharpPcapCli>();

            // 运行命令
            return await cli.RunAsync(args);
        }
    }
}

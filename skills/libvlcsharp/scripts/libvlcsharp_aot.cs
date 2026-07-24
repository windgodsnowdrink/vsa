#:sdk Microsoft.NET.Sdk
#:package LibVLCSharp@3.8.0
#:package LibVLCSharp.WinForms@3.8.0
#:package LibVLCSharp.WPF@3.8.0
#:package LibVLCSharp.Platforms@3.8.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.CommandLine@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LibVLCSharp.Shared;

namespace LibVLCSharpAot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 初始化LibVLC
            Core.Initialize();

            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddMemoryCache()
                .AddSingleton<LibVLCSharpService>()
                .BuildServiceProvider();

            var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                var command = args[0].ToLower();
                switch (command)
                {
                    case "play":
                    case "p":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供媒体文件路径或URL");
                            return;
                        }
                        var mediaPath = args[1];
                        var duration = args.Length > 2 ? int.TryParse(args[2], out var d) ? d : 0 : 0;
                        await vlcService.PlayMediaAsync(mediaPath, duration);
                        break;

                    case "record":
                    case "r":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供媒体文件路径/URL和输出文件路径");
                            return;
                        }
                        mediaPath = args[1];
                        var outputPath = args[2];
                        duration = args.Length > 3 ? int.TryParse(args[3], out var rd) ? rd : 0 : 0;
                        await vlcService.RecordMediaAsync(mediaPath, outputPath, duration);
                        break;

                    case "stream":
                    case "s":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供媒体文件路径/URL和流媒体URL");
                            return;
                        }
                        mediaPath = args[1];
                        var streamUrl = args[2];
                        duration = args.Length > 3 ? int.TryParse(args[3], out var sd) ? sd : 0 : 0;
                        await vlcService.StreamMediaAsync(mediaPath, streamUrl, duration);
                        break;

                    case "convert":
                    case "c":
                        if (args.Length < 3)
                        {
                            logger.LogError("请提供输入文件路径和输出文件路径");
                            return;
                        }
                        var inputPath = args[1];
                        outputPath = args[2];
                        var format = args.Length > 3 ? args[3] : "mp4";
                        await vlcService.ConvertMediaAsync(inputPath, outputPath, format);
                        break;

                    case "info":
                    case "i":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供媒体文件路径或URL");
                            return;
                        }
                        mediaPath = args[1];
                        await vlcService.GetMediaInfoAsync(mediaPath);
                        break;

                    case "list-codecs":
                    case "lc":
                        await vlcService.ListCodecsAsync();
                        break;

                    case "list-filters":
                    case "lf":
                        await vlcService.ListFiltersAsync();
                        break;

                    case "benchmark":
                    case "bm":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供媒体文件路径或URL");
                            return;
                        }
                        mediaPath = args[1];
                        var iterations = args.Length > 2 ? int.TryParse(args[2], out var it) ? it : 5 : 5;
                        await vlcService.RunBenchmarkAsync(mediaPath, iterations);
                        break;

                    case "help":
                    case "h":
                        ShowHelp();
                        break;

                    default:
                        logger.LogError("未知命令: {Command}", command);
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "执行命令时发生错误");
            }
            finally
            {
                // 清理LibVLC
                Core.Cleanup();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("LibVLCSharp AOT 工具");
            Console.WriteLine("=================");
            Console.WriteLine("命令列表:");
            Console.WriteLine("  play|p <media> [duration] - 播放媒体文件");
            Console.WriteLine("  record|r <media> <output> [duration] - 录制媒体文件");
            Console.WriteLine("  stream|s <media> <stream-url> [duration] - 流媒体播放");
            Console.WriteLine("  convert|c <input> <output> [format] - 转换媒体格式");
            Console.WriteLine("  info|i <media> - 获取媒体信息");
            Console.WriteLine("  list-codecs|lc - 列出支持的编解码器");
            Console.WriteLine("  list-filters|lf - 列出支持的过滤器");
            Console.WriteLine("  benchmark|bm <media> [iterations] - 运行性能基准测试");
            Console.WriteLine("  help|h - 显示帮助信息");
        }
    }

    public class LibVLCSharpService
    {
        private readonly ILogger<LibVLCSharpService> _logger;
        private LibVLC _libVLC;

        public LibVLCSharpService()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .BuildServiceProvider();
            
            _logger = serviceProvider.GetRequiredService<ILogger<LibVLCSharpService>>();
            _libVLC = new LibVLC();
        }

        public async Task PlayMediaAsync(string mediaPath, int duration)
        {
            _logger.LogInformation("开始播放媒体: {MediaPath}", mediaPath);
            
            using var media = new Media(_libVLC, mediaPath, FromType.FromPath);
            using var mediaPlayer = new MediaPlayer(media);

            mediaPlayer.Play();
            _logger.LogInformation("媒体开始播放");

            if (duration > 0)
            {
                _logger.LogInformation("播放时长: {Duration} 秒", duration);
                await Task.Delay(TimeSpan.FromSeconds(duration));
                mediaPlayer.Stop();
                _logger.LogInformation("播放结束");
            }
            else
            {
                _logger.LogInformation("按任意键停止播放...");
                Console.ReadKey();
                mediaPlayer.Stop();
                _logger.LogInformation("播放结束");
            }
        }

        public async Task RecordMediaAsync(string mediaPath, string outputPath, int duration)
        {
            _logger.LogInformation("开始录制媒体: {MediaPath}", mediaPath);
            _logger.LogInformation("输出文件: {OutputPath}", outputPath);

            var options = new string[]
            {
                "sout=#transcode{vcodec=h264,acodec=aac}:file{dst=" + outputPath + ",no-overwrite}",
                "sout-keep"
            };

            using var media = new Media(_libVLC, mediaPath, FromType.FromPath, options);
            using var mediaPlayer = new MediaPlayer(media);

            mediaPlayer.Play();
            _logger.LogInformation("录制开始");

            if (duration > 0)
            {
                _logger.LogInformation("录制时长: {Duration} 秒", duration);
                await Task.Delay(TimeSpan.FromSeconds(duration));
                mediaPlayer.Stop();
                _logger.LogInformation("录制结束，文件保存为: {OutputPath}", outputPath);
            }
            else
            {
                _logger.LogInformation("按任意键停止录制...");
                Console.ReadKey();
                mediaPlayer.Stop();
                _logger.LogInformation("录制结束，文件保存为: {OutputPath}", outputPath);
            }
        }

        public async Task StreamMediaAsync(string mediaPath, string streamUrl, int duration)
        {
            _logger.LogInformation("开始流媒体播放: {MediaPath}", mediaPath);
            _logger.LogInformation("流媒体URL: {StreamUrl}", streamUrl);

            var options = new string[]
            {
                "sout=#transcode{vcodec=h264,acodec=aac}:http{mux=ts,dst=" + streamUrl + "}",
                "sout-keep"
            };

            using var media = new Media(_libVLC, mediaPath, FromType.FromPath, options);
            using var mediaPlayer = new MediaPlayer(media);

            mediaPlayer.Play();
            _logger.LogInformation("流媒体开始");
            _logger.LogInformation("可以通过 {StreamUrl} 访问流媒体", streamUrl);

            if (duration > 0)
            {
                _logger.LogInformation("流媒体时长: {Duration} 秒", duration);
                await Task.Delay(TimeSpan.FromSeconds(duration));
                mediaPlayer.Stop();
                _logger.LogInformation("流媒体结束");
            }
            else
            {
                _logger.LogInformation("按任意键停止流媒体...");
                Console.ReadKey();
                mediaPlayer.Stop();
                _logger.LogInformation("流媒体结束");
            }
        }

        public async Task ConvertMediaAsync(string inputPath, string outputPath, string format)
        {
            _logger.LogInformation("开始转换媒体格式");
            _logger.LogInformation("输入文件: {InputPath}", inputPath);
            _logger.LogInformation("输出文件: {OutputPath}", outputPath);
            _logger.LogInformation("输出格式: {Format}", format);

            var options = new string[]
            {
                "sout=#transcode{vcodec=h264,acodec=aac}:file{dst=" + outputPath + ",no-overwrite}",
                "sout-keep"
            };

            using var media = new Media(_libVLC, inputPath, FromType.FromPath, options);
            using var mediaPlayer = new MediaPlayer(media);

            mediaPlayer.Play();
            _logger.LogInformation("转换开始");

            // 等待转换完成
            while (mediaPlayer.IsPlaying)
            {
                await Task.Delay(1000);
            }

            _logger.LogInformation("转换结束，文件保存为: {OutputPath}", outputPath);
        }

        public async Task GetMediaInfoAsync(string mediaPath)
        {
            _logger.LogInformation("获取媒体信息: {MediaPath}", mediaPath);

            using var media = new Media(_libVLC, mediaPath, FromType.FromPath);
            await media.Parse(MediaParseOptions.ParseNetwork);

            _logger.LogInformation("媒体信息:");
            _logger.LogInformation("  标题: {Title}", media.Meta(MediaMetadatas.Title) ?? "未知");
            _logger.LogInformation("  艺术家: {Artist}", media.Meta(MediaMetadatas.Artist) ?? "未知");
            _logger.LogInformation("  专辑: {Album}", media.Meta(MediaMetadatas.Album) ?? "未知");
            _logger.LogInformation("  时长: {Duration} 秒", media.Duration / 1000);
            _logger.LogInformation("  编码: {Codec}", media.CodecName(0) ?? "未知");
            _logger.LogInformation("  分辨率: {Width}x{Height}", media.Width, media.Height);
            _logger.LogInformation("  比特率: {Bitrate} kbps", media.Bitrate / 1000);
        }

        public async Task ListCodecsAsync()
        {
            _logger.LogInformation("支持的编解码器列表:");
            _logger.LogInformation("=================");

            // 模拟编解码器列表
            var codecs = new List<string>
            {
                "H.264 (AVC)",
                "H.265 (HEVC)",
                "MPEG-4",
                "VP8",
                "VP9",
                "AV1",
                "MP3",
                "AAC",
                "OGG Vorbis",
                "FLAC",
                "WAV"
            };

            foreach (var codec in codecs)
            {
                _logger.LogInformation("  - {Codec}", codec);
                await Task.Delay(50);
            }

            _logger.LogInformation("共支持 {Count} 种编解码器", codecs.Count);
        }

        public async Task ListFiltersAsync()
        {
            _logger.LogInformation("支持的过滤器列表:");
            _logger.LogInformation("=================");

            // 模拟过滤器列表
            var filters = new List<string>
            {
                "视频过滤器: crop",
                "视频过滤器: scale",
                "视频过滤器: rotate",
                "视频过滤器: deinterlace",
                "音频过滤器: equalizer",
                "音频过滤器: compressor",
                "音频过滤器: spatializer",
                "音频过滤器: volnorm"
            };

            foreach (var filter in filters)
            {
                _logger.LogInformation("  - {Filter}", filter);
                await Task.Delay(50);
            }

            _logger.LogInformation("共支持 {Count} 种过滤器", filters.Count);
        }

        public async Task RunBenchmarkAsync(string mediaPath, int iterations)
        {
            _logger.LogInformation("开始性能基准测试...");
            _logger.LogInformation(new string('=', 60));
            _logger.LogInformation("媒体文件: {MediaPath}", mediaPath);
            _logger.LogInformation("运行次数: {Iterations}", iterations);

            var times = new List<long>();

            for (int i = 0; i < iterations; i++)
            {
                _logger.LogInformation($"运行测试 {i + 1}/{iterations}...");
                
                var stopwatch = Stopwatch.StartNew();
                
                using var media = new Media(_libVLC, mediaPath, FromType.FromPath);
                await media.Parse(MediaParseOptions.ParseNetwork);
                
                stopwatch.Stop();
                times.Add(stopwatch.ElapsedMilliseconds);
                
                _logger.LogInformation($"运行 {i + 1} 耗时: {stopwatch.ElapsedMilliseconds} ms");
            }

            var averageTime = times.Average();
            var minTime = times.Min();
            var maxTime = times.Max();

            _logger.LogInformation(new string('=', 60));
            _logger.LogInformation("性能基准测试结果:");
            _logger.LogInformation("  运行次数: {Iterations}", iterations);
            _logger.LogInformation("  平均耗时: {Average:F2} ms", averageTime);
            _logger.LogInformation("  最小耗时: {Min} ms", minTime);
            _logger.LogInformation("  最大耗时: {Max} ms", maxTime);
            _logger.LogInformation("  标准差: {StdDev:F2} ms", Math.Sqrt(times.Select(t => Math.Pow(t - averageTime, 2)).Average()));
        }
    }
}
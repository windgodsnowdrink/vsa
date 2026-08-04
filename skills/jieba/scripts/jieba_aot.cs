#:sdk Microsoft.NET.Sdk
#:package jieba.NET@0.37.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
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
using System.Threading.Tasks;
using JiebaNet.Segmenter;
using JiebaNet.Segmenter.PosTagging;
using JiebaNet.Segmenter.Tokenizers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JiebaAot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .AddMemoryCache()
                .AddSingleton<JiebaService>()
                .BuildServiceProvider();

            var jiebaService = serviceProvider.GetRequiredService<JiebaService>();
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
                    case "cut":
                    case "c":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供要分词的文本");
                            return;
                        }
                        var text = string.Join(" ", args.Skip(1));
                        var result = await jiebaService.CutAsync(text);
                        logger.LogInformation("分词结果: {Result}", string.Join(" ", result));
                        break;

                    case "cutall":
                    case "ca":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供要分词的文本");
                            return;
                        }
                        text = string.Join(" ", args.Skip(1));
                        result = await jiebaService.CutAllAsync(text);
                        logger.LogInformation("全模式分词结果: {Result}", string.Join(" ", result));
                        break;

                    case "cutforsearch":
                    case "cfs":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供要分词的文本");
                            return;
                        }
                        text = string.Join(" ", args.Skip(1));
                        result = await jiebaService.CutForSearchAsync(text);
                        logger.LogInformation("搜索引擎模式分词结果: {Result}", string.Join(" ", result));
                        break;

                    case "extract":
                    case "e":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供要提取关键词的文本");
                            return;
                        }
                        text = string.Join(" ", args.Skip(1));
                        var topN = 5;
                        var keywords = await jiebaService.ExtractTagsAsync(text, topN);
                        logger.LogInformation("关键词提取结果: {Result}", string.Join(" ", keywords));
                        break;

                    case "pos":
                    case "p":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供要词性标注的文本");
                            return;
                        }
                        text = string.Join(" ", args.Skip(1));
                        var posResult = await jiebaService.PosSegmentAsync(text);
                        logger.LogInformation("词性标注结果:");
                        foreach (var (word, pos) in posResult)
                        {
                            logger.LogInformation("{Word}: {Pos}", word, pos);
                        }
                        break;

                    case "batch":
                    case "b":
                        if (args.Length < 2)
                        {
                            logger.LogError("请提供要批量处理的文件路径");
                            return;
                        }
                        var inputPath = args[1];
                        var outputPath = args.Length > 2 ? args[2] : Path.Combine(Path.GetDirectoryName(inputPath)!, $"{Path.GetFileNameWithoutExtension(inputPath)}_segmented.txt");
                        await jiebaService.BatchProcessAsync(inputPath, outputPath);
                        logger.LogInformation("批量处理完成，结果保存到: {OutputPath}", outputPath);
                        break;

                    case "benchmark":
                    case "bm":
                        await jiebaService.RunBenchmarkAsync();
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
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Jieba.NET AOT 分词工具");
            Console.WriteLine("=====================");
            Console.WriteLine("命令列表:");
            Console.WriteLine("  cut|c <text>        - 精确模式分词");
            Console.WriteLine("  cutall|ca <text>    - 全模式分词");
            Console.WriteLine("  cutforsearch|cfs <text> - 搜索引擎模式分词");
            Console.WriteLine("  extract|e <text>    - 提取关键词");
            Console.WriteLine("  pos|p <text>        - 词性标注");
            Console.WriteLine("  batch|b <input> [output] - 批量处理文本文件");
            Console.WriteLine("  benchmark|bm        - 运行性能基准测试");
            Console.WriteLine("  help|h              - 显示帮助信息");
        }
    }

    public class JiebaService
    {
        private readonly JiebaSegmenter _segmenter;
        private readonly PosSegmenter _posSegmenter;
        private readonly ILogger<JiebaService> _logger;

        public JiebaService()
        {
            _segmenter = new JiebaSegmenter();
            _posSegmenter = new PosSegmenter();
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole())
                .BuildServiceProvider();
            _logger = serviceProvider.GetRequiredService<ILogger<JiebaService>>();
        }

        public async Task<List<string>> CutAsync(string text)
        {
            return await Task.Run(() => _segmenter.Cut(text).ToList());
        }

        public async Task<List<string>> CutAllAsync(string text)
        {
            return await Task.Run(() => _segmenter.Cut(text, true).ToList());
        }

        public async Task<List<string>> CutForSearchAsync(string text)
        {
            return await Task.Run(() => _segmenter.CutForSearch(text).ToList());
        }

        public async Task<List<string>> ExtractTagsAsync(string text, int topN = 5)
        {
            return await Task.Run(() => _segmenter.ExtractTags(text, topN).ToList());
        }

        public async Task<List<(string, string)>> PosSegmentAsync(string text)
        {
            return await Task.Run(() => _posSegmenter.Cut(text).Select(t => (t.Word, t.Flag)).ToList());
        }

        public async Task BatchProcessAsync(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("输入文件不存在", inputPath);
            }

            var lines = await File.ReadAllLinesAsync(inputPath);
            var results = new List<string>();

            await Task.WhenAll(lines.Select(async (line, index) =>
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var segmented = await CutAsync(line);
                    var resultLine = $"{index + 1}: {string.Join(" ", segmented)}";
                    lock (results)
                    {
                        results.Add(resultLine);
                    }
                }
            }));

            await File.WriteAllLinesAsync(outputPath, results);
        }

        public async Task RunBenchmarkAsync()
        {
            var testText = "这是一个测试文本，用于测试分词性能.这是一个较长的测试文本，包含多个句子和词汇.";
            var iterations = 10000;

            _logger.LogInformation("开始性能基准测试...");
            _logger.LogInformation(new string('=', 60));

            // 测试精确模式
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                await CutAsync(testText);
            }
            stopwatch.Stop();
            _logger.LogInformation("精确模式: {Iterations} 次迭代，耗时 {ElapsedMilliseconds} ms", iterations, stopwatch.ElapsedMilliseconds);

            // 测试全模式
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await CutAllAsync(testText);
            }
            stopwatch.Stop();
            _logger.LogInformation("全模式: {Iterations} 次迭代，耗时 {ElapsedMilliseconds} ms", iterations, stopwatch.ElapsedMilliseconds);

            // 测试搜索引擎模式
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await CutForSearchAsync(testText);
            }
            stopwatch.Stop();
            _logger.LogInformation("搜索引擎模式: {Iterations} 次迭代，耗时 {ElapsedMilliseconds} ms", iterations, stopwatch.ElapsedMilliseconds);

            // 测试关键词提取
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await ExtractTagsAsync(testText);
            }
            stopwatch.Stop();
            _logger.LogInformation("关键词提取: {Iterations} 次迭代，耗时 {ElapsedMilliseconds} ms", iterations, stopwatch.ElapsedMilliseconds);

            // 测试词性标注
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await PosSegmentAsync(testText);
            }
            stopwatch.Stop();
            _logger.LogInformation("词性标注: {Iterations} 次迭代，耗时 {ElapsedMilliseconds} ms", iterations, stopwatch.ElapsedMilliseconds);

            _logger.LogInformation(new string('=', 60));
            _logger.LogInformation("性能基准测试完成");
        }
    }
}
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Collections.Immutable@8.0.0
#:package System.Linq.Async@6.0.1
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property RuntimeIdentifier=win-x64
#:property SelfContained=true
#:property PublishTrimmed=true
#:property TrimMode=partial
#:property EnableCompilation=false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GoodWordsAOT
{
    public class GoodWordsEngine
    {
        private readonly GoodWordsOptions _options;
        private readonly ILogger<GoodWordsEngine> _logger;
        private readonly List<GoodWord> _words;
        private readonly Dictionary<string, List<GoodWord>> _wordsByCategory;
        private readonly HashSet<string> _wordSet;

        public GoodWordsEngine(IOptions<GoodWordsOptions> options, ILogger<GoodWordsEngine> logger)
        {
            _options = options.Value;
            _logger = logger;
            _words = new List<GoodWord>();
            _wordsByCategory = new Dictionary<string, List<GoodWord>>();
            _wordSet = new HashSet<string>();
            InitializeDefaultWords();
        }

        private void InitializeDefaultWords()
        {
            var defaultWords = new List<GoodWord>
            {
                new GoodWord { Text = "优秀", Category = "praise", Score = 95 },
                new GoodWord { Text = "杰出", Category = "praise", Score = 90 },
                new GoodWord { Text = "卓越", Category = "praise", Score = 98 },
                new GoodWord { Text = "完美", Category = "praise", Score = 100 },
                new GoodWord { Text = "出色", Category = "praise", Score = 85 },
                new GoodWord { Text = "良好", Category = "praise", Score = 80 },
                new GoodWord { Text = "优秀团队", Category = "team", Score = 92 },
                new GoodWord { Text = "协作精神", Category = "team", Score = 88 },
                new GoodWord { Text = "创新思维", Category = "creativity", Score = 94 },
                new GoodWord { Text = "精益求精", Category = "quality", Score = 91 },
                new GoodWord { Text = "客户至上", Category = "service", Score = 89 },
                new GoodWord { Text = "专业素养", Category = "professional", Score = 93 }
            };

            foreach (var word in defaultWords)
            {
                AddWord(word);
            }

            _logger.LogInformation("GoodWords引擎初始化完成，加载了{Count}个默认好词", defaultWords.Count);
        }

        public void AddWord(GoodWord word)
        {
            if (_wordSet.Contains(word.Text))
            {
                _logger.LogWarning("好词 '{Text}' 已存在", word.Text);
                return;
            }

            _words.Add(word);
            _wordSet.Add(word.Text);

            if (!_wordsByCategory.ContainsKey(word.Category))
            {
                _wordsByCategory[word.Category] = new List<GoodWord>();
            }
            _wordsByCategory[word.Category].Add(word);

            _logger.LogInformation("添加好词: {Text} (分类: {Category}, 评分: {Score})", word.Text, word.Category, word.Score);
        }

        public IEnumerable<GoodWord> GenerateWords(int count, string category = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new List<GoodWord>();

            try
            {
                IEnumerable<GoodWord> sourceWords = _words;
                if (!string.IsNullOrEmpty(category) && _wordsByCategory.ContainsKey(category))
                {
                    sourceWords = _wordsByCategory[category];
                }

                var random = new Random();
                var availableWords = sourceWords.ToList();

                for (int i = 0; i < count && availableWords.Count > 0; i++)
                {
                    int index = random.Next(availableWords.Count);
                    var selectedWord = availableWords[index];
                    result.Add(selectedWord);
                    availableWords.RemoveAt(index);
                }

                stopwatch.Stop();
                _logger.LogInformation("生成{Count}个好词耗时: {Elapsed}ms", result.Count, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "生成好词时发生错误");
            }

            return result;
        }

        public IEnumerable<GoodWord> SearchWords(string keyword, int maxResults = 10)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new List<GoodWord>();

            try
            {
                result = _words.Where(w => w.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(w => w.Score)
                    .Take(maxResults)
                    .ToList();

                stopwatch.Stop();
                _logger.LogInformation("搜索关键词 '{Keyword}' 找到{Count}个结果，耗时: {Elapsed}ms", keyword, result.Count, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "搜索好词时发生错误");
            }

            return result;
        }

        public IEnumerable<GoodWord> GetWordsByCategory(string category)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new List<GoodWord>();

            try
            {
                if (_wordsByCategory.ContainsKey(category))
                {
                    result = _wordsByCategory[category].OrderByDescending(w => w.Score).ToList();
                }

                stopwatch.Stop();
                _logger.LogInformation("获取分类 '{Category}' 的好词，共{Count}个，耗时: {Elapsed}ms", category, result.Count, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "获取分类好词时发生错误");
            }

            return result;
        }

        public IEnumerable<string> GetCategories()
        {
            return _wordsByCategory.Keys;
        }

        public int GetWordCount()
        {
            return _words.Count;
        }

        public void UpdateWordScore(string text, int score)
        {
            var word = _words.FirstOrDefault(w => w.Text == text);
            if (word != null)
            {
                word.Score = score;
                _logger.LogInformation("更新好词评分: {Text} -> {Score}", text, score);
            }
            else
            {
                _logger.LogWarning("好词 '{Text}' 不存在，无法更新评分", text);
            }
        }

        public void RemoveWord(string text)
        {
            var word = _words.FirstOrDefault(w => w.Text == text);
            if (word != null)
            {
                _words.Remove(word);
                _wordSet.Remove(text);
                if (_wordsByCategory.ContainsKey(word.Category))
                {
                    _wordsByCategory[word.Category].Remove(word);
                }
                _logger.LogInformation("删除好词: {Text}", text);
            }
            else
            {
                _logger.LogWarning("好词 '{Text}' 不存在，无法删除", text);
            }
        }
    }

    public class GoodWord
    {
        public string Text { get; set; }
        public string Category { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            return $"{Text} (分类: {Category}, 评分: {Score})";
        }
    }

    public class GoodWordsOptions
    {
        public int MaxWords { get; set; } = 1000;
        public int DefaultScore { get; set; } = 80;
        public string DefaultCategory { get; set; } = "general";
    }

    public class GoodWordsCommandHandler
    {
        private readonly GoodWordsEngine _engine;
        private readonly ILogger<GoodWordsCommandHandler> _logger;

        public GoodWordsCommandHandler(GoodWordsEngine engine, ILogger<GoodWordsCommandHandler> logger)
        {
            _engine = engine;
            _logger = logger;
        }

        public async Task HandleCommandAsync(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var command = args[0].ToLower();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                switch (command)
                {
                    case "generate":
                        await HandleGenerateCommand(args);
                        break;
                    case "search":
                        await HandleSearchCommand(args);
                        break;
                    case "add":
                        await HandleAddCommand(args);
                        break;
                    case "remove":
                        await HandleRemoveCommand(args);
                        break;
                    case "update":
                        await HandleUpdateCommand(args);
                        break;
                    case "list":
                        await HandleListCommand(args);
                        break;
                    case "categories":
                        await HandleCategoriesCommand(args);
                        break;
                    case "count":
                        await HandleCountCommand(args);
                        break;
                    case "help":
                    case "--help":
                    case "-h":
                        ShowHelp();
                        break;
                    default:
                        _logger.LogError("未知命令: {Command}", command);
                        ShowHelp();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行命令时发生错误");
                Console.WriteLine($"错误: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("命令执行完成，耗时: {Elapsed}ms", stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task HandleGenerateCommand(string[] args)
        {
            int count = 5;
            string category = null;

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("--count=") || args[i].StartsWith("-c="))
                {
                    int.TryParse(args[i].Split('=')[1], out count);
                }
                else if (args[i].StartsWith("--category=") || args[i].StartsWith("-cat="))
                {
                    category = args[i].Split('=')[1];
                }
            }

            var words = _engine.GenerateWords(count, category);
            Console.WriteLine($"\n生成的好词 ({count}个):");
            Console.WriteLine("=================================");
            foreach (var word in words)
            {
                Console.WriteLine($"  - {word}");
            }
            Console.WriteLine("=================================");
        }

        private async Task HandleSearchCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("请提供搜索关键词");
                return;
            }

            string keyword = args[1];
            int maxResults = 10;

            for (int i = 2; i < args.Length; i++)
            {
                if (args[i].StartsWith("--max=") || args[i].StartsWith("-m="))
                {
                    int.TryParse(args[i].Split('=')[1], out maxResults);
                }
            }

            var results = _engine.SearchWords(keyword, maxResults);
            Console.WriteLine($"\n搜索结果 (关键词: '{keyword}'):");
            Console.WriteLine("=================================");
            foreach (var word in results)
            {
                Console.WriteLine($"  - {word}");
            }
            Console.WriteLine("=================================");
        }

        private async Task HandleAddCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("请提供要添加的好词");
                return;
            }

            string text = args[1];
            string category = "general";
            int score = 80;

            for (int i = 2; i < args.Length; i++)
            {
                if (args[i].StartsWith("--category=") || args[i].StartsWith("-cat="))
                {
                    category = args[i].Split('=')[1];
                }
                else if (args[i].StartsWith("--score=") || args[i].StartsWith("-s="))
                {
                    int.TryParse(args[i].Split('=')[1], out score);
                }
            }

            var word = new GoodWord { Text = text, Category = category, Score = score };
            _engine.AddWord(word);
            Console.WriteLine($"\n好词添加成功: {word}");
        }

        private async Task HandleRemoveCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("请提供要删除的好词");
                return;
            }

            string text = args[1];
            _engine.RemoveWord(text);
            Console.WriteLine($"\n好词删除完成: '{text}'");
        }

        private async Task HandleUpdateCommand(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("请提供要更新的好词和新评分");
                return;
            }

            string text = args[1];
            int score = 0;
            int.TryParse(args[2], out score);

            _engine.UpdateWordScore(text, score);
            Console.WriteLine($"\n好词评分更新完成: '{text}' -> {score}");
        }

        private async Task HandleListCommand(string[] args)
        {
            string category = null;

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("--category=") || args[i].StartsWith("-cat="))
                {
                    category = args[i].Split('=')[1];
                }
            }

            var words = category != null ? _engine.GetWordsByCategory(category) : _engine.GenerateWords(_engine.GetWordCount());
            Console.WriteLine($"\n好词列表 ({words.Count()}个):");
            Console.WriteLine("=================================");
            foreach (var word in words)
            {
                Console.WriteLine($"  - {word}");
            }
            Console.WriteLine("=================================");
        }

        private async Task HandleCategoriesCommand(string[] args)
        {
            var categories = _engine.GetCategories();
            Console.WriteLine($"\n可用分类 ({categories.Count()}个):");
            Console.WriteLine("=================================");
            foreach (var category in categories)
            {
                var count = _engine.GetWordsByCategory(category).Count();
                Console.WriteLine($"  - {category} ({count}个好词)");
            }
            Console.WriteLine("=================================");
        }

        private async Task HandleCountCommand(string[] args)
        {
            int count = _engine.GetWordCount();
            Console.WriteLine($"\n好词总数: {count}");
        }

        private void ShowHelp()
        {
            Console.WriteLine("\nGoodWords AOT 命令行工具");
            Console.WriteLine("=================================");
            Console.WriteLine("命令列表:");
            Console.WriteLine("  generate      - 生成好词");
            Console.WriteLine("    选项: --count=<数量> -c=<数量>, --category=<分类> -cat=<分类>");
            Console.WriteLine("  search        - 搜索好词");
            Console.WriteLine("    参数: <关键词> [--max=<最大结果数> -m=<最大结果数>]");
            Console.WriteLine("  add           - 添加好词");
            Console.WriteLine("    参数: <好词> [--category=<分类> -cat=<分类>] [--score=<评分> -s=<评分>]");
            Console.WriteLine("  remove        - 删除好词");
            Console.WriteLine("    参数: <好词>");
            Console.WriteLine("  update        - 更新好词评分");
            Console.WriteLine("    参数: <好词> <新评分>");
            Console.WriteLine("  list          - 列出所有好词");
            Console.WriteLine("    选项: --category=<分类> -cat=<分类>");
            Console.WriteLine("  categories    - 列出所有分类");
            Console.WriteLine("  count         - 显示好词总数");
            Console.WriteLine("  help          - 显示帮助信息");
            Console.WriteLine("=================================");
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.SetMinimumLevel(LogLevel.Information);
                })
                .Configure<GoodWordsOptions>(options =>
                {
                    options.MaxWords = 1000;
                    options.DefaultScore = 80;
                    options.DefaultCategory = "general";
                })
                .AddSingleton<GoodWordsEngine>()
                .AddSingleton<GoodWordsCommandHandler>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var commandHandler = serviceProvider.GetRequiredService<GoodWordsCommandHandler>();

            try
            {
                logger.LogInformation("GoodWords AOT 引擎启动");
                await commandHandler.HandleCommandAsync(args);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "应用程序启动失败");
                Console.WriteLine($"启动错误: {ex.Message}");
            }
            finally
            {
                if (serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}

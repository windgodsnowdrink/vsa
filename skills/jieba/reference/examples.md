# Jieba 技能使用示例

## 1. 命令行使用示例

### 1.1 基本分词示例

#### 1.1.1 精确模式分词

**功能**：精确模式分词，试图将句子最精确地切开，适合文本分析。

**示例**：
```bash
# 使用完整命令
jieba_aot cut 这是一个测试文本，用于演示精确模式分词

# 使用命令别名
jieba_aot c 这是一个测试文本，用于演示精确模式分词
```

**输出**：
```
分词结果: 这 是 一个 测试 文本 ， 用于 演示 精确 模式 分词
```

#### 1.1.2 全模式分词

**功能**：全模式分词，把句子中所有的可以成词的词语都扫描出来，速度非常快，但是不能解决歧义。

**示例**：
```bash
# 使用完整命令
jieba_aot cutall 这是一个测试文本

# 使用命令别名
jieba_aot ca 这是一个测试文本
```

**输出**：
```
全模式分词结果: 这 是 一个 测试 文本
```

#### 1.1.3 搜索引擎模式分词

**功能**：搜索引擎模式分词，在精确模式的基础上，对长词再次切分，提高召回率，适合用于搜索引擎分词。

**示例**：
```bash
# 使用完整命令
jieba_aot cutforsearch 这是一个测试文本

# 使用命令别名
jieba_aot cfs 这是一个测试文本
```

**输出**：
```
搜索引擎模式分词结果: 这 是 一个 测试 文本
```

### 1.2 关键词提取示例

**功能**：提取文本中的关键词，基于 TF-IDF 算法。

**示例**：
```bash
# 使用完整命令
jieba_aot extract 这是一个测试文本，用于测试关键词提取功能。关键词提取是文本分析的重要步骤，能够帮助我们快速了解文本的主要内容。

# 使用命令别名
jieba_aot e 这是一个测试文本，用于测试关键词提取功能。关键词提取是文本分析的重要步骤，能够帮助我们快速了解文本的主要内容。
```

**输出**：
```
关键词提取结果: 测试 文本 关键词 提取 功能 分析 重要 步骤 快速 了解
```

### 1.3 词性标注示例

**功能**：对分词结果进行词性标注，标注每个词的词性。

**示例**：
```bash
# 使用完整命令
jieba_aot pos 这是一个测试文本，用于演示词性标注功能

# 使用命令别名
jieba_aot p 这是一个测试文本，用于演示词性标注功能
```

**输出**：
```
词性标注结果:
这: r
是: v
一个: m
测试: v
文本: n
，: x
用于: p
演示: v
词性: n
标注: v
功能: n
```

### 1.4 批量处理示例

**功能**：批量处理文本文件，对文件中的每一行进行分词处理。

**准备输入文件**：创建 `input.txt` 文件，内容如下：
```
这是第一行测试文本
这是第二行测试文本，包含更多内容
这是第三行测试文本，用于演示批量处理功能
```

**示例**：
```bash
# 使用完整命令，指定输出文件
jieba_aot batch input.txt output.txt

# 使用命令别名，使用默认输出文件
jieba_aot b input.txt
```

**输出文件内容** (`output.txt`)：
```
1: 这 是 第一 行 测试 文本
2: 这 是 第二 行 测试 文本 ， 包含 更多 内容
3: 这 是 第三 行 测试 文本 ， 用于 演示 批量 处理 功能
```

### 1.5 性能测试示例

**功能**：运行性能基准测试，测试各种分词模式的性能。

**示例**：
```bash
# 使用完整命令
jieba_aot benchmark

# 使用命令别名
jieba_aot bm
```

**输出**：
```
开始性能基准测试...
============================================================
精确模式: 10000 次迭代，耗时 1234 ms
全模式: 10000 次迭代，耗时 876 ms
搜索引擎模式: 10000 次迭代，耗时 1543 ms
关键词提取: 10000 次迭代，耗时 2345 ms
词性标注: 10000 次迭代，耗时 3456 ms
============================================================
性能基准测试完成
```

## 2. API 集成示例

### 2.1 基本集成示例

**功能**：在 C# 应用程序中集成 JiebaService，使用其 API 进行分词操作。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        // 初始化依赖注入容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        // 获取 JiebaService 实例
        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 测试文本
        var testText = "这是一个测试文本，用于演示 API 集成示例";

        // 1. 精确模式分词
        Console.WriteLine("\n1. 精确模式分词:");
        var cutResult = await jiebaService.CutAsync(testText);
        Console.WriteLine(string.Join(" ", cutResult));

        // 2. 全模式分词
        Console.WriteLine("\n2. 全模式分词:");
        var cutAllResult = await jiebaService.CutAllAsync(testText);
        Console.WriteLine(string.Join(" ", cutAllResult));

        // 3. 搜索引擎模式分词
        Console.WriteLine("\n3. 搜索引擎模式分词:");
        var cutForSearchResult = await jiebaService.CutForSearchAsync(testText);
        Console.WriteLine(string.Join(" ", cutForSearchResult));

        // 4. 关键词提取
        Console.WriteLine("\n4. 关键词提取:");
        var extractResult = await jiebaService.ExtractTagsAsync(testText, 5);
        Console.WriteLine(string.Join(" ", extractResult));

        // 5. 词性标注
        Console.WriteLine("\n5. 词性标注:");
        var posResult = await jiebaService.PosSegmentAsync(testText);
        foreach (var (word, pos) in posResult)
        {
            Console.WriteLine($"{word}: {pos}");
        }
    }
}
```

### 2.2 ASP.NET Core 集成示例

**功能**：在 ASP.NET Core 应用程序中集成 JiebaService，提供分词 API。

**示例代码**：
```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using JiebaAot;

// Startup.cs 中注册服务
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<JiebaService>();
    services.AddControllers();
}

// 控制器
[ApiController]
[Route("api/[controller]")]
public class SegmentController : ControllerBase
{
    private readonly JiebaService _jiebaService;

    public SegmentController(JiebaService jiebaService)
    {
        _jiebaService = jiebaService;
    }

    [HttpPost("cut")]
    public async Task<ActionResult<List<string>>> Cut([FromBody] string text)
    {
        var result = await _jiebaService.CutAsync(text);
        return Ok(result);
    }

    [HttpPost("cutall")]
    public async Task<ActionResult<List<string>>> CutAll([FromBody] string text)
    {
        var result = await _jiebaService.CutAllAsync(text);
        return Ok(result);
    }

    [HttpPost("cutforsearch")]
    public async Task<ActionResult<List<string>>> CutForSearch([FromBody] string text)
    {
        var result = await _jiebaService.CutForSearchAsync(text);
        return Ok(result);
    }

    [HttpPost("extract")]
    public async Task<ActionResult<List<string>>> Extract([FromBody] ExtractRequest request)
    {
        var result = await _jiebaService.ExtractTagsAsync(request.Text, request.TopN);
        return Ok(result);
    }

    [HttpPost("pos")]
    public async Task<ActionResult<List<(string, string)>>> Pos([FromBody] string text)
    {
        var result = await _jiebaService.PosSegmentAsync(text);
        return Ok(result);
    }
}

public class ExtractRequest
{
    public string Text { get; set; }
    public int TopN { get; set; } = 5;
}
```

## 3. 应用场景示例

### 3.1 文本分析场景

#### 3.1.1 情感分析预处理

**功能**：对文本进行分词和关键词提取，作为情感分析的预处理步骤。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class SentimentAnalysisExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 测试文本
        var texts = new List<string>
        {
            "这部电影非常好看，演员表演出色，剧情紧凑",
            "这个产品质量很差，服务态度也不好，非常失望",
            "今天天气很好，心情不错，打算出去走走"
        };

        foreach (var text in texts)
        {
            Console.WriteLine($"\n原始文本: {text}");

            // 1. 分词
            var words = await jiebaService.CutAsync(text);
            Console.WriteLine($"分词结果: {string.Join(" ", words)}");

            // 2. 提取关键词
            var keywords = await jiebaService.ExtractTagsAsync(text, 5);
            Console.WriteLine($"关键词: {string.Join(" ", keywords)}");

            // 3. 词性标注
            var posTags = await jiebaService.PosSegmentAsync(text);
            Console.WriteLine("词性标注:");
            foreach (var (word, pos) in posTags)
            {
                Console.WriteLine($"  {word}: {pos}");
            }

            // 4. 情感分析预处理 (示例)
            var positiveWords = new List<string> { "好", "好看", "出色", "不错" };
            var negativeWords = new List<string> { "差", "不好", "失望" };

            int positiveCount = words.Intersect(positiveWords).Count();
            int negativeCount = words.Intersect(negativeWords).Count();

            string sentiment = "中性";
            if (positiveCount > negativeCount)
                sentiment = "正面";
            else if (negativeCount > positiveCount)
                sentiment = "负面";

            Console.WriteLine($"情感倾向: {sentiment}");
        }
    }
}
```

#### 3.1.2 主题识别

**功能**：通过分词和关键词提取，识别文本的主要主题。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class TopicIdentificationExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 测试文本
        var texts = new List<string>
        {
            "人工智能技术近年来发展迅速，特别是深度学习和大语言模型",
            "足球比赛非常精彩，球员们表现出色，观众热情高涨",
            "这道菜味道不错，食材新鲜，烹饪手法独特"
        };

        foreach (var text in texts)
        {
            Console.WriteLine($"\n原始文本: {text}");

            // 提取关键词
            var keywords = await jiebaService.ExtractTagsAsync(text, 5);
            Console.WriteLine($"关键词: {string.Join(" ", keywords)}");

            // 主题识别 (示例)
            var topics = new Dictionary<string, List<string>>
            {
                { "科技", new List<string> { "人工智能", "技术", "深度学习", "模型" } },
                { "体育", new List<string> { "足球", "比赛", "球员", "观众" } },
                { "美食", new List<string> { "菜", "味道", "食材", "烹饪" } }
            };

            string identifiedTopic = "未知";
            int maxMatch = 0;

            foreach (var (topic, topicWords) in topics)
            {
                int matchCount = keywords.Intersect(topicWords).Count();
                if (matchCount > maxMatch)
                {
                    maxMatch = matchCount;
                    identifiedTopic = topic;
                }
            }

            Console.WriteLine($"识别主题: {identifiedTopic}");
        }
    }
}
```

### 3.2 搜索引擎场景

#### 3.2.1 索引构建

**功能**：对网页内容进行分词，构建搜索索引。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class SearchIndexExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 模拟网页内容
        var documents = new Dictionary<int, string>
        {
            { 1, "人工智能技术介绍：人工智能是指让计算机模拟人类智能的技术" },
            { 2, "深度学习入门：深度学习是人工智能的一个重要分支" },
            { 3, "足球比赛新闻：昨晚的足球比赛非常精彩" }
        };

        // 构建索引
        var index = new Dictionary<string, List<int>>();

        foreach (var (docId, content) in documents)
        {
            Console.WriteLine($"\n处理文档 {docId}: {content}");

            // 使用搜索引擎模式分词
            var words = await jiebaService.CutForSearchAsync(content);
            Console.WriteLine($"分词结果: {string.Join(" ", words)}");

            // 添加到索引
            foreach (var word in words)
            {
                if (!index.ContainsKey(word))
                {
                    index[word] = new List<int>();
                }
                if (!index[word].Contains(docId))
                {
                    index[word].Add(docId);
                }
            }
        }

        // 显示索引
        Console.WriteLine("\n构建的搜索索引:");
        foreach (var (word, docIds) in index)
        {
            Console.WriteLine($"{word}: {string.Join(", ", docIds)}");
        }

        // 模拟搜索
        var query = "人工智能";
        Console.WriteLine($"\n搜索查询: {query}");

        var queryWords = await jiebaService.CutForSearchAsync(query);
        Console.WriteLine($"查询分词: {string.Join(" ", queryWords)}");

        var resultDocs = new HashSet<int>();
        foreach (var word in queryWords)
        {
            if (index.ContainsKey(word))
            {
                foreach (var docId in index[word])
                {
                    resultDocs.Add(docId);
                }
            }
        }

        Console.WriteLine($"搜索结果: {string.Join(", ", resultDocs)}");
        foreach (var docId in resultDocs)
        {
            Console.WriteLine($"  文档 {docId}: {documents[docId]}");
        }
    }
}
```

### 3.3 自然语言处理场景

#### 3.3.1 命名实体识别（简单示例）

**功能**：基于分词和词性标注，识别文本中的命名实体。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class NamedEntityRecognitionExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 测试文本
        var texts = new List<string>
        {
            "张三在北京大学学习计算机科学",
            "李四于2023年加入了微软公司",
            "王五出生于北京，现在居住在上海"
        };

        foreach (var text in texts)
        {
            Console.WriteLine($"\n原始文本: {text}");

            // 词性标注
            var posTags = await jiebaService.PosSegmentAsync(text);
            Console.WriteLine("词性标注:");
            foreach (var (word, pos) in posTags)
            {
                Console.WriteLine($"  {word}: {pos}");
            }

            // 简单的命名实体识别
            var personNames = new List<string>();
            var locations = new List<string>();
            var organizations = new List<string>();
            var times = new List<string>();

            foreach (var (word, pos) in posTags)
            {
                // 基于词性标注的简单规则
                if (pos == "nr") // 人名
                    personNames.Add(word);
                else if (pos == "ns") // 地名
                    locations.Add(word);
                else if (pos == "nt") // 机构名
                    organizations.Add(word);
                else if (pos == "t") // 时间
                    times.Add(word);
            }

            Console.WriteLine($"人名: {string.Join(", ", personNames)}");
            Console.WriteLine($"地名: {string.Join(", ", locations)}");
            Console.WriteLine($"机构名: {string.Join(", ", organizations)}");
            Console.WriteLine($"时间: {string.Join(", ", times)}");
        }
    }
}
```

### 3.4 信息检索场景

#### 3.4.1 文档检索

**功能**：基于分词结果检索相关文档。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class DocumentRetrievalExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 模拟文档集合
        var documents = new List<Document>
        {
            new Document { Id = 1, Title = "人工智能简介", Content = "人工智能是研究、开发用于模拟、延伸和扩展人的智能的理论、方法、技术及应用系统的一门新的技术科学" },
            new Document { Id = 2, Title = "深度学习基础", Content = "深度学习是机器学习的一个分支，它通过模拟人脑的神经网络结构来进行学习和预测" },
            new Document { Id = 3, Title = "计算机科学概论", Content = "计算机科学是研究计算机及其周围各种现象和规律的科学，包括计算机硬件、软件、网络等方面" },
            new Document { Id = 4, Title = "足球比赛规则", Content = "足球比赛是一项团队运动，双方各11名球员，通过将球踢入对方球门得分" },
            new Document { Id = 5, Title = "美食制作技巧", Content = "烹饪是一门艺术，掌握好食材的选择和烹饪时间是制作美食的关键" }
        };

        // 构建文档向量
        var documentVectors = new Dictionary<int, HashSet<string>>();
        foreach (var doc in documents)
        {
            var content = $"{doc.Title} {doc.Content}";
            var words = await jiebaService.CutAsync(content);
            documentVectors[doc.Id] = new HashSet<string>(words);
        }

        // 检索函数
        async Task<List<Document>> RetrieveDocuments(string query, int topK = 3)
        {
            var queryWords = await jiebaService.CutAsync(query);
            var querySet = new HashSet<string>(queryWords);

            // 计算相似度（简单的余弦相似度）
            var scores = new List<(int DocId, double Score)>();
            foreach (var (docId, docWords) in documentVectors)
            {
                var intersection = querySet.Intersect(docWords).Count();
                var union = querySet.Union(docWords).Count();
                var score = union > 0 ? (double)intersection / union : 0;
                scores.Add((docId, score));
            }

            // 排序并返回结果
            var topDocs = scores
                .Where(s => s.Score > 0)
                .OrderByDescending(s => s.Score)
                .Take(topK)
                .Select(s => documents.First(d => d.Id == s.DocId))
                .ToList();

            return topDocs;
        }

        // 测试检索
        var queries = new List<string>
        {
            "人工智能",
            "深度学习",
            "计算机科学",
            "足球",
            "美食"
        };

        foreach (var query in queries)
        {
            Console.WriteLine($"\n检索查询: {query}");
            var results = await RetrieveDocuments(query);

            if (results.Count > 0)
            {
                Console.WriteLine("检索结果:");
                foreach (var doc in results)
                {
                    Console.WriteLine($"  文档 {doc.Id}: {doc.Title}");
                }
            }
            else
            {
                Console.WriteLine("无匹配文档");
            }
        }
    }
}

class Document
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
```

## 4. 高级使用示例

### 4.1 自定义词典示例

**功能**：使用自定义词典，提高特定领域的分词准确性。

**准备自定义词典**：创建 `user_dict.txt` 文件，内容如下：
```
云计算 5 n
人工智能 3 n
深度学习 4 n
自然语言处理 5 n
```

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class CustomDictionaryExample
{
    static async Task Main(string[] args)
    {
        // 设置环境变量，指定用户词典路径
        Environment.SetEnvironmentVariable("JIEBA_USER_DICT_PATH", "user_dict.txt");

        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 测试文本
        var testText = "云计算和人工智能是当前热门的技术领域，深度学习和自然语言处理是其重要分支";

        Console.WriteLine($"原始文本: {testText}");

        // 分词
        var words = await jiebaService.CutAsync(testText);
        Console.WriteLine($"分词结果: {string.Join(" ", words)}");

        // 提取关键词
        var keywords = await jiebaService.ExtractTagsAsync(testText, 10);
        Console.WriteLine($"关键词: {string.Join(" ", keywords)}");

        // 词性标注
        var posTags = await jiebaService.PosSegmentAsync(testText);
        Console.WriteLine("词性标注:");
        foreach (var (word, pos) in posTags)
        {
            Console.WriteLine($"  {word}: {pos}");
        }
    }
}
```

### 4.2 批量处理大文件示例

**功能**：处理大文本文件，使用批量处理模式提高效率。

**示例代码**：
```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class LargeFileProcessingExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 生成测试大文件
        var inputFile = "large_input.txt";
        var outputFile = "large_output.txt";

        // 生成测试数据
        Console.WriteLine("生成测试大文件...");
        using (var writer = new StreamWriter(inputFile))
        {
            for (int i = 0; i < 10000; i++)
            {
                writer.WriteLine($"这是第{i + 1}行测试文本，包含一些关键词如人工智能、云计算、深度学习等");
            }
        }
        Console.WriteLine($"测试大文件生成完成，共 10000 行");

        // 批量处理
        Console.WriteLine("\n开始批量处理...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await jiebaService.BatchProcessAsync(inputFile, outputFile);

        stopwatch.Stop();
        Console.WriteLine($"批量处理完成，耗时 {stopwatch.ElapsedMilliseconds} ms");

        // 验证输出
        var outputLines = File.ReadAllLines(outputFile);
        Console.WriteLine($"输出文件共 {outputLines.Length} 行");

        // 显示前几行结果
        Console.WriteLine("\n前 5 行处理结果:");
        for (int i = 0; i < Math.Min(5, outputLines.Length); i++)
        {
            Console.WriteLine(outputLines[i]);
        }

        // 清理临时文件
        File.Delete(inputFile);
        File.Delete(outputFile);
    }
}
```

## 5. 性能优化示例

### 5.1 内存缓存示例

**功能**：使用内存缓存提高重复文本的处理速度。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

class MemoryCacheExample
{
    private static MemoryCache _cache;

    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();
        _cache = serviceProvider.GetRequiredService<IMemoryCache>() as MemoryCache;

        // 测试文本
        var testTexts = new List<string>
        {
            "这是一个测试文本",
            "这是另一个测试文本",
            "这是一个测试文本", // 重复
            "这是第三个测试文本",
            "这是另一个测试文本" // 重复
        };

        // 不使用缓存
        Console.WriteLine("不使用缓存:");
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            foreach (var text in testTexts)
            {
                await jiebaService.CutAsync(text);
            }
        }
        stopwatch.Stop();
        Console.WriteLine($"耗时: {stopwatch.ElapsedMilliseconds} ms");

        // 使用缓存
        Console.WriteLine("\n使用缓存:");
        stopwatch.Restart();
        for (int i = 0; i < 1000; i++)
        {
            foreach (var text in testTexts)
            {
                var result = await CutWithCacheAsync(jiebaService, text);
            }
        }
        stopwatch.Stop();
        Console.WriteLine($"耗时: {stopwatch.ElapsedMilliseconds} ms");
    }

    static async Task<List<string>> CutWithCacheAsync(JiebaService jiebaService, string text)
    {
        var cacheKey = $"cut:{text}";
        if (_cache.TryGetValue(cacheKey, out List<string> cachedResult))
        {
            return cachedResult;
        }

        var result = await jiebaService.CutAsync(text);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        return result;
    }
}
```

### 5.2 并行处理示例

**功能**：使用并行处理提高批量文本的处理速度。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class ParallelProcessingExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 生成测试文本
        var testTexts = new List<string>();
        for (int i = 0; i < 1000; i++)
        {
            testTexts.Add($"这是第{i + 1}行测试文本，用于演示并行处理性能");
        }

        // 串行处理
        Console.WriteLine("串行处理:");
        var stopwatch = Stopwatch.StartNew();
        var serialResults = new List<List<string>>();
        foreach (var text in testTexts)
        {
            var result = await jiebaService.CutAsync(text);
            serialResults.Add(result);
        }
        stopwatch.Stop();
        Console.WriteLine($"耗时: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"处理文本数: {serialResults.Count}");

        // 并行处理
        Console.WriteLine("\n并行处理:");
        stopwatch.Restart();
        var parallelResults = await Task.WhenAll(
            testTexts.Select(async text => await jiebaService.CutAsync(text))
        );
        stopwatch.Stop();
        Console.WriteLine($"耗时: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"处理文本数: {parallelResults.Length}");

        // 验证结果
        Console.WriteLine($"\n结果验证: {(serialResults.Count == parallelResults.Length ? "成功" : "失败"}");
    }
}
```

## 6. 实际应用案例

### 6.1 聊天机器人预处理

**功能**：对用户输入进行分词和关键词提取，作为聊天机器人的预处理步骤。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class ChatbotPreprocessingExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 模拟用户输入
        var userInputs = new List<string>
        {
            "你好，请问今天天气怎么样？",
            "我想了解一下人工智能的发展趋势",
            "推荐一些好看的电影",
            "如何提高英语口语水平？"
        };

        foreach (var input in userInputs)
        {
            Console.WriteLine($"\n用户输入: {input}");

            // 1. 分词
            var words = await jiebaService.CutAsync(input);
            Console.WriteLine($"分词结果: {string.Join(" ", words)}");

            // 2. 提取关键词
            var keywords = await jiebaService.ExtractTagsAsync(input, 5);
            Console.WriteLine($"关键词: {string.Join(" ", keywords)}");

            // 3. 意图识别（简单示例）
            var intentKeywords = new Dictionary<string, List<string>>
            {
                { "天气查询", new List<string> { "天气", "怎么样" } },
                { "技术咨询", new List<string> { "人工智能", "发展趋势" } },
                { "娱乐推荐", new List<string> { "电影", "推荐" } },
                { "学习咨询", new List<string> { "如何", "提高", "英语" } }
            };

            string intent = "未知意图";
            int maxMatch = 0;

            foreach (var (intentName, intentWords) in intentKeywords)
            {
                int matchCount = keywords.Intersect(intentWords).Count();
                if (matchCount > maxMatch)
                {
                    maxMatch = matchCount;
                    intent = intentName;
                }
            }

            Console.WriteLine($"识别意图: {intent}");

            // 4. 聊天机器人响应（示例）
            var responses = new Dictionary<string, string>
            {
                { "天气查询", "抱歉，我无法获取实时天气信息，请查看天气应用" },
                { "技术咨询", "人工智能正在快速发展，主要趋势包括深度学习、自然语言处理和计算机视觉等领域" },
                { "娱乐推荐", "最近有很多好看的电影，如《复仇者联盟》系列、《流浪地球》等" },
                { "学习咨询", "提高英语口语的方法包括多听多说、参加语言交换、观看英语电影等" },
                { "未知意图", "抱歉，我不太理解您的意思，请换一种方式表达" }
            };

            Console.WriteLine($"机器人响应: {responses.GetValueOrDefault(intent, "抱歉，我不太理解您的意思")}");
        }
    }
}
```

### 6.2 文本分类

**功能**：基于分词结果进行文本分类。

**示例代码**：
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JiebaAot;
using Microsoft.Extensions.DependencyInjection;

class TextClassificationExample
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<JiebaService>()
            .BuildServiceProvider();

        var jiebaService = serviceProvider.GetRequiredService<JiebaService>();

        // 训练数据
        var trainingData = new List<(string Text, string Category)>
        {
            ("人工智能是未来的发展方向", "科技"),
            ("深度学习在图像识别中应用广泛", "科技"),
            ("计算机编程需要逻辑思维能力", "科技"),
            ("足球比赛非常精彩", "体育"),
            ("篮球运动员需要良好的身体素质", "体育"),
            ("奥运会是全球最大的体育盛会", "体育"),
            ("这道菜味道不错", "美食"),
            ("烹饪技巧很重要", "美食"),
            ("食材的新鲜程度直接影响菜品质量", "美食")
        };

        // 构建分类器（简单的词袋模型）
        var categoryWords = new Dictionary<string, HashSet<string>>();
        var allWords = new HashSet<string>();

        foreach (var (text, category) in trainingData)
        {
            var words = await jiebaService.CutAsync(text);
            if (!categoryWords.ContainsKey(category))
            {
                categoryWords[category] = new HashSet<string>();
            }
            foreach (var word in words)
            {
                categoryWords[category].Add(word);
                allWords.Add(word);
            }
        }

        // 分类函数
        async Task<string> ClassifyText(string text)
        {
            var words = await jiebaService.CutAsync(text);
            var wordSet = new HashSet<string>(words);

            var scores = new Dictionary<string, int>();
            foreach (var (category, categoryWordSet) in categoryWords)
            {
                int score = wordSet.Intersect(categoryWordSet).Count();
                scores[category] = score;
            }

            if (scores.Count == 0)
                return "未知类别";

            return scores.OrderByDescending(s => s.Value).First().Key;
        }

        // 测试分类
        var testTexts = new List<string>
        {
            "机器学习是人工智能的重要分支",
            "乒乓球是中国的国球",
            "厨师需要掌握多种烹饪方法",
            "网络安全是当前面临的重要挑战"
        };

        foreach (var text in testTexts)
        {
            var category = await ClassifyText(text);
            Console.WriteLine($"文本: {text}");
            Console.WriteLine($"分类结果: {category}");
            Console.WriteLine();
        }
    }
}
```

## 7. 常见问题解决方案

### 7.1 分词结果不准确

**问题**：分词结果与预期不符。

**解决方案**：

**示例**：使用搜索引擎模式分词，提高召回率
```bash
jieba_aot cutforsearch 中华人民共和国成立于1949年
```

**输出**：
```
搜索引擎模式分词结果: 中华 华人 人民 共和 共和国 成立 于 1949 年
```

**示例**：添加自定义词典
```bash
# 创建自定义词典文件 user_dict.txt
# 内容：中华人民共和国 10 n

# 设置环境变量
set JIEBA_USER_DICT_PATH=user_dict.txt

# 运行分词
jieba_aot cut 中华人民共和国成立于1949年
```

**输出**：
```
分词结果: 中华人民共和国 成立 于 1949 年
```

### 7.2 处理速度慢

**问题**：处理大文本时速度慢。

**解决方案**：

**示例**：使用全模式分词，提高处理速度
```bash
jieba_aot cutall "这是一个很长的测试文本，包含很多内容，用于测试分词速度。这是一个很长的测试文本，包含很多内容，用于测试分词速度。"
```

**示例**：使用批量处理模式
```bash
# 将大文本保存到文件
# 然后使用批量处理
jieba_aot batch large_text.txt output.txt
```

### 7.3 内存占用高

**问题**：处理大文件时内存占用高。

**解决方案**：

**示例**：调整批量处理大小
```bash
# 设置环境变量
set JIEBA_BATCH_SIZE=50

# 运行批量处理
jieba_aot batch large_text.txt output.txt
```

**示例**：分割大文件
```bash
# 将大文件分割成小文件
# 然后分别处理
jieba_aot batch part1.txt output1.txt
jieba_aot batch part2.txt output2.txt
# 合并结果
copy output1.txt+output2.txt output.txt
```

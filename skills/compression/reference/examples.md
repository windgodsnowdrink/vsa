# Compression AOT 使用示例

## 1. 基本压缩示例

### 1.1 使用Gzip压缩文件

```csharp
// 创建主机
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
builder.Services.AddSingleton<Compression.AOT.CompressionAotEngine>();

var host = builder.Build();
var engine = host.Services.GetRequiredService<Compression.AOT.CompressionAotEngine>();

// 使用Gzip压缩文件
var result = await engine.ExecuteCompressAsync("input.txt", "output.gz");
Console.WriteLine($"压缩结果: {(result ? "成功" : "失败")}");
```

### 1.2 使用Brotli压缩文件

```csharp
// 使用Brotli压缩算法
var result = await engine.ExecuteCompressAsync(
    "input.txt", 
    "output.br", 
    Compression.AOT.CompressionAlgorithm.Brotli
);
```

## 2. 不同压缩级别示例

### 2.1 最快压缩

```csharp
// 使用最快压缩级别
var result = await engine.ExecuteCompressAsync(
    "input.txt", 
    "output.gz", 
    CompressionLevel: System.IO.Compression.CompressionLevel.Fastest
);
```

### 2.2 最优压缩

```csharp
// 使用最优压缩级别
var result = await engine.ExecuteCompressAsync(
    "input.txt", 
    "output.gz", 
    CompressionLevel: System.IO.Compression.CompressionLevel.Optimal
);
```

### 2.3 不压缩

```csharp
// 使用不压缩级别
var result = await engine.ExecuteCompressAsync(
    "input.txt", 
    "output.gz", 
    CompressionLevel: System.IO.Compression.CompressionLevel.NoCompression
);
```

## 3. 解压缩示例

### 3.1 解压缩Gzip文件

```csharp
// 解压缩Gzip文件
var result = await engine.ExecuteDecompressAsync("input.gz", "output.txt");
```

### 3.2 解压缩Brotli文件

```csharp
// 解压缩Brotli文件
var result = await engine.ExecuteDecompressAsync(
    "input.br", 
    "output.txt", 
    Compression.AOT.CompressionAlgorithm.Brotli
);
```

## 4. 命令行使用示例

### 4.1 基本压缩命令

```bash
# 基本压缩
compression_aot.exe compress input.txt output.gz

# 使用指定算法
compression_aot.exe compress input.txt output.br brotli

# 使用指定算法和压缩级别
compression_aot.exe compress input.txt output.gz gzip fastest
```

### 4.2 解压缩命令

```bash
# 基本解压缩
compression_aot.exe decompress input.gz output.txt

# 指定解压缩算法
compression_aot.exe decompress input.br output.txt brotli
```

### 4.3 查看帮助

```bash
# 查看命令行帮助
compression_aot.exe --help
```

## 5. 高级使用示例

### 5.1 直接使用压缩服务

```csharp
// 直接使用压缩服务
var compressionService = serviceProvider.GetRequiredService<Compression.AOT.ICompressionService>();

// 压缩数据流
using var inputStream = File.OpenRead("input.txt");
using var outputStream = File.Create("output.gz");
var result = await compressionService.CompressStreamAsync(
    inputStream, 
    outputStream, 
    Compression.AOT.CompressionAlgorithm.Gzip
);
```

### 5.2 解压缩数据流

```csharp
// 解压缩数据流
using var inputStream = File.OpenRead("input.gz");
using var outputStream = File.Create("output.txt");
var result = await compressionService.DecompressStreamAsync(
    inputStream, 
    outputStream, 
    Compression.AOT.CompressionAlgorithm.Gzip
);
```

## 6. 批量处理示例

### 6.1 批量压缩文件

```csharp
// 批量压缩多个文件
public async Task BatchCompressFiles(string[] inputFiles, string outputDirectory, Compression.AOT.CompressionAlgorithm algorithm = Compression.AOT.CompressionAlgorithm.Gzip)
{
    var builder = Host.CreateApplicationBuilder();
    builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
    builder.Services.AddSingleton<Compression.AOT.CompressionAotEngine>();
    
    var host = builder.Build();
    var engine = host.Services.GetRequiredService<Compression.AOT.CompressionAotEngine>();
    
    // 确保输出目录存在
    Directory.CreateDirectory(outputDirectory);
    
    // 并行处理多个文件
    var tasks = inputFiles.Select(async inputFile =>
    {
        var fileName = Path.GetFileName(inputFile);
        var outputFile = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}.gz");
        
        var result = await engine.ExecuteCompressAsync(inputFile, outputFile, algorithm);
        Console.WriteLine($"压缩 {inputFile} -> {outputFile}: {(result ? "成功" : "失败")}");
        return result;
    });
    
    // 等待所有任务完成
    var results = await Task.WhenAll(tasks);
    var successCount = results.Count(r => r);
    Console.WriteLine($"批量压缩完成: {successCount}/{inputFiles.Length} 个文件成功");
}
```

### 6.2 批量解压缩文件

```csharp
// 批量解压缩文件
public async Task BatchDecompressFiles(string[] inputFiles, string outputDirectory)
{
    var builder = Host.CreateApplicationBuilder();
    builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
    builder.Services.AddSingleton<Compression.AOT.CompressionAotEngine>();
    
    var host = builder.Build();
    var engine = host.Services.GetRequiredService<Compression.AOT.CompressionAotEngine>();
    
    Directory.CreateDirectory(outputDirectory);
    
    var tasks = inputFiles.Select(async inputFile =>
    {
        var fileName = Path.GetFileName(inputFile);
        var outputFile = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(fileName));
        
        // 根据文件扩展名自动选择算法
        var algorithm = GetAlgorithmFromExtension(fileName);
        var result = await engine.ExecuteDecompressAsync(inputFile, outputFile, algorithm);
        
        Console.WriteLine($"解压缩 {inputFile} -> {outputFile}: {(result ? "成功" : "失败")}");
        return result;
    });
    
    var results = await Task.WhenAll(tasks);
    var successCount = results.Count(r => r);
    Console.WriteLine($"批量解压缩完成: {successCount}/{inputFiles.Length} 个文件成功");
}

// 根据文件扩展名获取压缩算法
private Compression.AOT.CompressionAlgorithm GetAlgorithmFromExtension(string fileName)
{
    var extension = Path.GetExtension(fileName).ToLower();
    return extension switch
    {
        ".gz" => Compression.AOT.CompressionAlgorithm.Gzip,
        ".br" => Compression.AOT.CompressionAlgorithm.Brotli,
        ".zip" => Compression.AOT.CompressionAlgorithm.Zip,
        _ => Compression.AOT.CompressionAlgorithm.Deflate
    };
}
```

## 7. 与ASP.NET Core集成示例

### 7.1 文件压缩API

```csharp
// ASP.NET Core控制器
[ApiController]
[Route("api/[controller]")]
public class CompressionController : ControllerBase
{
    private readonly Compression.AOT.ICompressionService _compressionService;
    
    public CompressionController(Compression.AOT.ICompressionService compressionService)
    {
        _compressionService = compressionService;
    }
    
    [HttpPost("compress")]
    public async Task<IActionResult> CompressFile([FromForm] IFormFile file, [FromQuery] string algorithm = "gzip")
    {
        try
        {
            // 解析算法
            var compressionAlgorithm = algorithm.ToLower() switch
            {
                "gzip" => Compression.AOT.CompressionAlgorithm.Gzip,
                "deflate" => Compression.AOT.CompressionAlgorithm.Deflate,
                "brotli" => Compression.AOT.CompressionAlgorithm.Brotli,
                "zip" => Compression.AOT.CompressionAlgorithm.Zip,
                _ => throw new ArgumentException("不支持的压缩算法")
            };
            
            // 创建临时文件
            var tempInput = Path.GetTempFileName();
            var tempOutput = Path.GetTempFileName() + $".{algorithm}";
            
            // 保存上传的文件
            using (var stream = new FileStream(tempInput, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // 压缩文件
            var result = await _compressionService.CompressFileAsync(tempInput, tempOutput, compressionAlgorithm);
            
            if (result)
            {
                // 返回压缩后的文件
                var fileBytes = await System.IO.File.ReadAllBytesAsync(tempOutput);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName) + $".{algorithm}";
                return File(fileBytes, "application/octet-stream", fileName);
            }
            else
            {
                return BadRequest("压缩失败");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("decompress")]
    public async Task<IActionResult> DecompressFile([FromForm] IFormFile file, [FromQuery] string algorithm = "gzip")
    {
        // 类似的解压缩实现...
    }
}

// 注册服务
builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
```

## 8. 与Worker Service集成示例

### 8.1 自动压缩服务

```csharp
// Worker Service
public class AutoCompressionWorker : BackgroundService
{
    private readonly ILogger<AutoCompressionWorker> _logger;
    private readonly Compression.AOT.ICompressionService _compressionService;
    private readonly string _watchDirectory = "./watch";
    private readonly string _outputDirectory = "./compressed";
    
    public AutoCompressionWorker(ILogger<AutoCompressionWorker> logger, Compression.AOT.ICompressionService compressionService)
    {
        _logger = logger;
        _compressionService = compressionService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 确保目录存在
        Directory.CreateDirectory(_watchDirectory);
        Directory.CreateDirectory(_outputDirectory);
        
        // 创建文件系统监听器
        using var watcher = new FileSystemWatcher(_watchDirectory);
        watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        watcher.Filter = "*.*";
        watcher.EnableRaisingEvents = true;
        
        var fileCreated = new SemaphoreSlim(0);
        string? newFile = null;
        
        // 监听文件创建事件
        watcher.Created += (sender, e) =>
        {
            newFile = e.FullPath;
            fileCreated.Release();
        };
        
        _logger.LogInformation("自动压缩服务已启动，监听目录: {WatchDirectory}", _watchDirectory);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await fileCreated.WaitAsync(stoppingToken);
            
            if (newFile != null)
            {
                var fileName = Path.GetFileName(newFile);
                var outputFile = Path.Combine(_outputDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}.gz");
                
                _logger.LogInformation("检测到新文件: {FileName}，开始压缩...", fileName);
                
                // 等待文件完全写入
                await Task.Delay(1000, stoppingToken);
                
                // 执行压缩
                var result = await _compressionService.CompressFileAsync(newFile, outputFile);
                
                if (result)
                {
                    _logger.LogInformation("文件压缩成功: {FileName} -> {OutputFile}", fileName, outputFile);
                }
                else
                {
                    _logger.LogError("文件压缩失败: {FileName}", fileName);
                }
                
                newFile = null;
            }
        }
    }
}

// 注册服务
builder.Services.AddHostedService<AutoCompressionWorker>();
builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
```

## 9. 常见场景示例

### 9.1 日志文件压缩

```csharp
// 压缩日志文件
public async Task CompressLogFiles(string logDirectory, int daysToKeep = 7)
{
    var builder = Host.CreateApplicationBuilder();
    builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
    
    var host = builder.Build();
    var compressionService = host.Services.GetRequiredService<Compression.AOT.ICompressionService>();
    
    // 获取指定天数前的日志文件
    var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
    var logFiles = Directory.GetFiles(logDirectory, "*.log")
        .Where(f => new FileInfo(f).CreationTime < cutoffDate)
        .ToList();
    
    _logger.LogInformation("找到 {Count} 个需要压缩的日志文件", logFiles.Count);
    
    foreach (var logFile in logFiles)
    {
        var compressedFile = logFile + ".gz";
        var result = await compressionService.CompressFileAsync(logFile, compressedFile);
        
        if (result)
        {
            _logger.LogInformation("压缩日志文件成功: {LogFile} -> {CompressedFile}", logFile, compressedFile);
            // 压缩成功后删除原文件
            System.IO.File.Delete(logFile);
        }
        else
        {
            _logger.LogError("压缩日志文件失败: {LogFile}", logFile);
        }
    }
}
```

### 9.2 数据备份压缩

```csharp
// 备份数据并压缩
public async Task BackupAndCompressData(string dataDirectory, string backupDirectory)
{
    var builder = Host.CreateApplicationBuilder();
    builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
    
    var host = builder.Build();
    var compressionService = host.Services.GetRequiredService<Compression.AOT.ICompressionService>();
    
    // 创建备份目录
    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    var backupPath = Path.Combine(backupDirectory, $"backup_{timestamp}");
    Directory.CreateDirectory(backupPath);
    
    // 复制数据文件到备份目录
    var dataFiles = Directory.GetFiles(dataDirectory);
    foreach (var dataFile in dataFiles)
    {
        var fileName = Path.GetFileName(dataFile);
        var destFile = Path.Combine(backupPath, fileName);
        System.IO.File.Copy(dataFile, destFile);
    }
    
    // 创建临时zip文件
    var tempZip = Path.GetTempFileName() + ".zip";
    var finalZip = Path.Combine(backupDirectory, $"backup_{timestamp}.zip");
    
    // 压缩备份目录
    using var zipArchive = ZipFile.Open(tempZip, ZipArchiveMode.Create);
    foreach (var file in Directory.GetFiles(backupPath))
    {
        var fileName = Path.GetFileName(file);
        zipArchive.CreateEntryFromFile(file, fileName, CompressionLevel.Optimal);
    }
    
    // 关闭zipArchive
    zipArchive.Dispose();
    
    // 移动到最终位置
    System.IO.File.Move(tempZip, finalZip);
    
    // 删除临时备份目录
    Directory.Delete(backupPath, true);
    
    _logger.LogInformation("数据备份并压缩成功: {FinalZip}", finalZip);
}
```

## 10. 性能测试示例

### 10.1 不同算法性能比较

```csharp
// 测试不同压缩算法的性能
public async Task TestCompressionAlgorithms(string testFile)
{
    var builder = Host.CreateApplicationBuilder();
    builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
    
    var host = builder.Build();
    var compressionService = host.Services.GetRequiredService<Compression.AOT.ICompressionService>();
    
    var algorithms = new[]
    {
        Compression.AOT.CompressionAlgorithm.Gzip,
        Compression.AOT.CompressionAlgorithm.Deflate,
        Compression.AOT.CompressionAlgorithm.Brotli,
        Compression.AOT.CompressionAlgorithm.Zip
    };
    
    var fileInfo = new FileInfo(testFile);
    Console.WriteLine($"测试文件: {testFile}, 大小: {fileInfo.Length:N0} 字节");
    Console.WriteLine("\n算法性能测试结果:");
    Console.WriteLine("-
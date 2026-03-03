#:sdk Microsoft.NET.Sdk.Web
#:package CsvHelper@33.1.0
#:package Pomelo.EntityFrameworkCore.MySql@9.0.0
#:package Microsoft.EntityFrameworkCore@9.0.0
#:package Microsoft.EntityFrameworkCore.Design@9.0.0
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=false
#:property PublishReadyToRun=false
#:property PublishSingleFile=false

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

        // 注册服务
        var connectionString = "Server=172.100.61.40;CharSet=utf8;Port=3306;Database=robot_device_whdev;Uid=root;Pwd=Admin123;Allow User Variables=True;TreatTinyAsBoolean=false;SSL Mode=None;pooling=true;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;";
        
        // 注册DbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            
        // 注册DatabaseService
        builder.Services.AddScoped<DatabaseService>();

        var app = builder.Build();
        
        // 初始化数据库
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // 先删除现有数据库（测试环境下使用，生产环境不要这样做）
            Console.WriteLine("正在删除现有数据库...");
            dbContext.Database.EnsureDeleted();
            
            // 重新创建数据库和表结构
            Console.WriteLine("正在创建新数据库...");
            var created = dbContext.Database.EnsureCreated();
            Console.WriteLine(created ? "数据库已创建" : "数据库已存在");
            
            // 输出表信息
            var tableInfo = dbContext.Model.FindEntityType(typeof(HotQuestion));
            if (tableInfo != null)
            {
                Console.WriteLine($"表名: {tableInfo.GetTableName()}");
                foreach (var property in tableInfo.GetProperties())
                {
                    Console.WriteLine($"列名: {property.GetColumnName()}, 类型: {property.ClrType.Name}");
                }
            }
        }

        // 主页路由 - 返回HTML表单
        app.MapGet("/", async (HttpContext context) =>
        {
            var html = """
            <!DOCTYPE html>
            <html lang='zh-CN'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>CSV文件上传</title>
                <script src='https://unpkg.com/htmx.org@1.9.11'></script>
                <style>
                    body { font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; }
                    h1 { color: #333; }
                    form { margin: 20px 0; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
                    input[type='file'] { margin: 10px 0; }
                    button { background-color: #4CAF50; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; }
                    button:hover { background-color: #45a049; }
                    #progress { display: none; margin: 20px 0; padding: 15px; background-color: #f2f2f2; border-radius: 5px; }
                    #result { margin: 20px 0; padding: 15px; border-radius: 5px; }
                    .success { background-color: #d4edda; border: 1px solid #c3e6cb; color: #155724; }
                    .error { background-color: #f8d7da; border: 1px solid #f5c6cb; color: #721c24; }
                </style>
            </head>
            <body>
                <h1>热点问题CSV文件上传</h1>
                <form hx-post='/upload' hx-target='#result' hx-swap='innerHTML' enctype='multipart/form-data'>
                    <div>
                        <label for='file'>选择CSV文件:</label>
                        <input type='file' id='file' name='file' accept='.csv' required>
                    </div>
                    <button type='submit'>上传并处理</button>
                </form>
                
                <div id='progress'>
                    <h3>处理进度</h3>
                    <div>正在处理...</div>
                </div>
                
                <div id='result'></div>
                
                <script>
                    // 显示进度
                    document.querySelector('form').addEventListener('htmx:configRequest', function() {
                        document.getElementById('progress').style.display = 'block';
                    });
                    
                    // 隐藏进度
                    document.querySelector('form').addEventListener('htmx:afterRequest', function() {
                        document.getElementById('progress').style.display = 'none';
                    });
                </script>
            </body>
            </html>
            """;
                
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(html);
        });

        // 上传处理路由 - 禁用防伪检查
        app.MapPost("/upload", async (IFormFile file, DatabaseService dbService) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.Content("<div class='error'>未选择文件</div>", "text/html");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                return Results.Content("<div class='error'>仅支持CSV文件</div>", "text/html");
            }

            try
            {
                // 读取CSV文件
                var questions = new List<HotQuestion>();
                
                // 尝试多种编码读取文件，解决中文乱码问题
                StreamReader reader = null;
                
                // 先尝试UTF-8
                try
                {
                    reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.UTF8);
                    // 读取并测试是否有乱码
                    var content = reader.ReadToEnd();
                    if (content.Contains('�'))
                    {
                        reader.Dispose();
                        reader = null;
                    }else{
                        // UTF-8编码有效，重置流位置
                        file.OpenReadStream().Seek(0, SeekOrigin.Begin);
                        reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.UTF8);
                    }
                }
                catch { }
                
                // 如果UTF-8失败，尝试GB2312
                if (reader == null)
                {
                    try
                    {
                        reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.GetEncoding("GB2312"));
                    }
                    catch { }
                }
                
                // 如果GB2312也失败，尝试GBK
                if (reader == null)
                {
                    try
                    {
                        reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.GetEncoding("GBK"));
                    }
                    catch { }
                }
                
                // 如果所有编码都失败，使用默认编码
                if (reader == null)
                {
                    reader = new StreamReader(file.OpenReadStream());
                }
                using var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null // 忽略缺失的字段
                });
                
                // 检查CSV文件的列头
                await csv.ReadAsync();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;
                
                if (headers.Length == 1 && headers[0].Trim().ToLower() == "question")
                {
                    // 处理只有一个question列的CSV文件
                    while (await csv.ReadAsync())
                    {
                        var questionText = csv.GetField<string>(0);
                        if (!string.IsNullOrWhiteSpace(questionText))
                        {
                            questions.Add(new HotQuestion
                            {
                                QuestionId = Guid.NewGuid().ToString(),
                                Question = questionText
                            });
                        }
                    }
                }
                else
                {
                    // 处理完整格式的CSV文件
                    await csv.ReadAsync(); // 重置到第一条数据行
                    csv.ReadHeader(); // 重新读取表头
                    await foreach (var record in csv.GetRecordsAsync<HotQuestion>())
                    {
                        if (record.QuestionId == null) record.QuestionId = Guid.NewGuid().ToString();
                        questions.Add(record);
                    }
                }

                // 插入数据库
                var insertedCount = await dbService.InsertHotQuestionsAsync(questions);

                return Results.Content($"<div class='success'>成功导入 {insertedCount} 条记录</div>", "text/html");
            }
            catch (Exception ex)
            {
                // 显示详细错误信息，包括内部异常
                var errorMessage = $"处理失败: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n内部异常: {ex.InnerException.Message}";
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage += $"\n内部内部异常: {ex.InnerException.InnerException.Message}";
                    }
                }
                return Results.Content($"<div class='error'>{errorMessage}</div>", "text/html");
            }
        }).DisableAntiforgery();

await app.RunAsync();


// 数据模型
public class HotQuestion
{
    [Key]
    [MaxLength(36)]
    public string QuestionId { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(1000)]
    public string Question { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Category { get; set; } = "默认分类";
    
    [DefaultValue(0)]
    public int AnswerCount { get; set; } = 0;
    
    [DefaultValue(0)]
    public int ViewCount { get; set; } = 0;
    
    [DefaultValue(0)]
    public int LikeCount { get; set; } = 0;
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// 数据库上下文
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HotQuestion> HotQuestions { get; set; }
}

// 数据库服务
public class DatabaseService
{
    private readonly AppDbContext _dbContext;

    public DatabaseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> InsertHotQuestionsAsync(List<HotQuestion> questions)
    {
        await _dbContext.HotQuestions.AddRangeAsync(questions);
        return await _dbContext.SaveChangesAsync();
    }
}

#:sdk Microsoft.NET.Sdk.Web
#:package CsvHelper@33.1.0
#:package MySql.Data@8.3.0
#:property TargetFramework=net11.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// 从配置文件读取数据库连接字符串 // builder.Configuration.GetConnectionString("Default")
var connectionString = "Server=172.100.61.40;CharSet=utf8;Port=3306;Database=robot_device_whdev;Uid=root;Pwd=Admin123;Allow User Variables=True;TreatTinyAsBoolean=false;SSL Mode=None;pooling=true;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;";

// 注册服务
builder.Services.AddSingleton(new DatabaseService(connectionString));

var app = builder.Build();

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
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%); min-height: 100vh; padding: 20px; display: flex; justify-content: center; align-items: center; }
                    .container { background: white; border-radius: 20px; box-shadow: 0 12px 40px rgba(0, 0, 0, 0.12); padding: 30px; width: 100%; max-width: 500px; border: 1px solid rgba(25, 118, 210, 0.1); }
                    h1 { color: #1976d2; font-size: 24px; margin-bottom: 25px; text-align: center; font-weight: 600; display: flex; align-items: center; justify-content: center; gap: 10px; }
                    h1::before { content: "📊"; font-size: 28px; }
                    form { background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%); padding: 25px; border-radius: 15px; margin-bottom: 25px; border: 1px solid rgba(25, 118, 210, 0.2); }
                    .form-group { margin-bottom: 20px; }
                    label { display: block; color: #1976d2; font-weight: 600; margin-bottom: 8px; font-size: 14px; }
                    input[type='file'] { width: 100%; padding: 12px; border: 2px solid #bbdefb; border-radius: 10px; background: white; cursor: pointer; transition: all 0.3s ease; font-family: inherit; }
                    input[type='file']:hover { border-color: #1976d2; box-shadow: 0 2px 12px rgba(25, 118, 210, 0.25); }
                    input[type='file']:focus { outline: none; border-color: #1976d2; box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1); }
                    button { background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%); color: white; padding: 14px 25px; border: none; border-radius: 12px; cursor: pointer; font-size: 16px; font-weight: 600; width: 100%; transition: all 0.3s ease; box-shadow: 0 6px 20px rgba(25, 118, 210, 0.35); position: relative; overflow: hidden; }
                    button::before { content: ''; position: absolute; top: 0; left: -100%; width: 100%; height: 100%; background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent); transition: left 0.5s; }
                    button:hover::before { left: 100%; }
                    button:hover { background: linear-gradient(135deg, #1565c0 0%, #0d47a1 100%); transform: translateY(-2px); box-shadow: 0 8px 25px rgba(25, 118, 210, 0.45); }
                    button:active { transform: translateY(0); box-shadow: 0 4px 15px rgba(25, 118, 210, 0.3); }
                    #progress { display: none; background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%); padding: 25px; border-radius: 15px; margin-bottom: 20px; text-align: center; border: 1px solid rgba(25, 118, 210, 0.2); animation: pulse 2s infinite; }
                    @keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(25, 118, 210, 0.1); } 70% { box-shadow: 0 0 0 10px rgba(25, 118, 210, 0); } 100% { box-shadow: 0 0 0 0 rgba(25, 118, 210, 0); } }
                    #progress h3 { color: #1976d2; margin-bottom: 10px; font-size: 18px; display: flex; align-items: center; justify-content: center; gap: 8px; }
                    #progress h3::before { content: "🔄"; animation: spin 1s linear infinite; }
                    @keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }
                    #progress div { color: #1565c0; font-size: 16px; }
                    #result { padding: 20px; border-radius: 15px; text-align: center; font-size: 16px; font-weight: 600; border-width: 2px; border-style: solid; }
                    .success { background: linear-gradient(135deg, #e8f5e8 0%, #c8e6c9 100%); color: #2e7d32; border-color: #a5d6a7; box-shadow: 0 4px 15px rgba(46, 125, 50, 0.15); }
                    .error { background: linear-gradient(135deg, #ffebee 0%, #ffcdd2 100%); color: #c62828; border-color: #ef9a9a; box-shadow: 0 4px 15px rgba(198, 40, 40, 0.15); }
                    @media (max-width: 480px) { .container { padding: 20px; border-radius: 15px; } h1 { font-size: 20px; } form { padding: 20px; } button { padding: 12px 20px; font-size: 15px; } }
                </style>
            </head>
            <body>
                <div class='container'>
                <h1>热点问题CSV文件上传</h1>
                <form hx-post='/upload' hx-target='#result' hx-swap='innerHTML' enctype='multipart/form-data'>
                    <div class='form-group'>
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
                </div>
                
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
            // 添加空值检查
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
                    }
                    else
                    {
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
                    MissingFieldFound = null, // 忽略缺失的字段
                    BadDataFound = null, // 忽略格式错误的数据
                    TrimOptions = TrimOptions.Trim // 去除字段值前后的空格
                });

                // 检查CSV文件的列头
                await csv.ReadAsync();
                csv.ReadHeader();
                var headers = csv.HeaderRecord;
                var headerList = headers.Select(h => h.Trim().ToLower()).ToList();

                if (headerList.Count == 1 && headerList[0] == "question")
                {
                    // 处理只有一个question列的CSV文件
                    while (await csv.ReadAsync())
                    {
                        var questionText = csv.GetField<string>(0);
                        if (!string.IsNullOrWhiteSpace(questionText))
                        {
                            questions.Add(new HotQuestion
                            {
                                Question = questionText
                            });
                        }
                    }
                }
                else if (headerList.Contains("序号") && headerList.Contains("类别") && headerList.Contains("人员类别") && headerList.Contains("热点问题") && headerList.Contains("内容"))
                {
                    // 处理新格式的CSV文件（序号,类别,人员类别,热点问题,内容）

                    Console.WriteLine("检测到新格式的CSV文件，开始导入...");
                    int rowCount = 0;
                    while (await csv.ReadAsync())
                    {
                        rowCount++;
                        try
                        {
                            // 获取序号并转换为Guid
                            var serialNumber = csv.GetField<string>("序号");
                            Guid qaId;
                            if (Guid.TryParse(serialNumber, out qaId))
                            {
                                // 如果序号已经是Guid格式，直接使用
                            }
                            else
                            {
                                // 否则基于序号生成Guid
                                qaId = Guid.NewGuid();
                            }

                            var category = csv.GetField<string>("类别") ?? string.Empty;
                            var personType = csv.GetField<string>("人员类别") ?? string.Empty;
                            var hotQuestion = csv.GetField<string>("热点问题") ?? string.Empty;
                            var content = csv.GetField<string>("内容") ?? string.Empty;

                            if (!string.IsNullOrWhiteSpace(hotQuestion))
                            {
                                var question = new HotQuestion
                                {
                                    QAId = qaId,
                                    Question = hotQuestion,
                                    Answer = content,
                                    BusType = category,
                                    Mode = personType,
                                    // 将类别和人员类别作为关键词
                                    KeyWords = new List<string> { category, personType }.Where(k => !string.IsNullOrWhiteSpace(k)).ToList()
                                };

                                questions.Add(question);
                                Console.WriteLine($"成功读取第 {rowCount} 行数据: {hotQuestion}");
                            }
                            else
                            {
                                Console.WriteLine($"第 {rowCount} 行数据的热点问题为空，跳过");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"读取第 {rowCount} 行数据时发生错误: {ex.Message}");
                        }
                    }
                    Console.WriteLine($"CSV文件读取完成，共读取 {questions.Count} 条有效数据");
                }
                else
                {
                    // 处理旧格式的完整CSV文件
                    while (await csv.ReadAsync())
                    {
                        var question = new HotQuestion
                        {
                            Question = csv.GetField<string>("Question")
                        };

                        // 尝试获取其他可选字段
                        if (headerList.Contains("answer"))
                        {
                            question.Answer = csv.GetField<string>("Answer") ?? string.Empty;
                        }

                        if (headerList.Contains("keywords"))
                        {
                            var keywordsStr = csv.GetField<string>("KeyWords");
                            if (!string.IsNullOrEmpty(keywordsStr))
                            {
                                question.KeyWords = keywordsStr.Split(',').Select(k => k.Trim()).ToList();
                            }
                        }

                        if (headerList.Contains("bustype"))
                        {
                            question.BusType = csv.GetField<string>("BusType") ?? "common";
                        }

                        if (headerList.Contains("mode"))
                        {
                            question.Mode = csv.GetField<string>("Mode");
                        }

                        if (headerList.Contains("language"))
                        {
                            question.Language = csv.GetField<string>("Language") ?? "zh";
                        }

                        if (headerList.Contains("robot"))
                        {
                            question.Robot = csv.GetField<string>("Robot");
                        }

                        if (headerList.Contains("description"))
                        {
                            question.Description = csv.GetField<string>("Description");
                        }

                        if (headerList.Contains("promptcrispeid"))
                        {
                            var promptCrispeIdStr = csv.GetField<string>("PromptCrispeId");
                            if (!string.IsNullOrEmpty(promptCrispeIdStr) && Guid.TryParse(promptCrispeIdStr, out var promptCrispeId))
                            {
                                question.PromptCrispeId = promptCrispeId;
                            }
                        }

                        // 新增字段读取
                        if (headerList.Contains("extraproperties"))
                        {
                            question.ExtraProperties = csv.GetField<string>("ExtraProperties");
                        }

                        if (headerList.Contains("concurrencystamp"))
                        {
                            question.ConcurrencyStamp = csv.GetField<string>("ConcurrencyStamp");
                        }

                        if (headerList.Contains("isdeleted"))
                        {
                            var isDeletedStr = csv.GetField<string>("IsDeleted");
                            if (bool.TryParse(isDeletedStr, out var isDeleted))
                            {
                                question.IsDeleted = isDeleted;
                            }
                        }

                        if (headerList.Contains("creatorby"))
                        {
                            question.CreatorBy = csv.GetField<string>("CreatorBy");
                        }

                        if (headerList.Contains("creatorname"))
                        {
                            question.CreatorName = csv.GetField<string>("CreatorName");
                        }

                        if (headerList.Contains("deletedby"))
                        {
                            question.DeletedBy = csv.GetField<string>("DeletedBy");
                        }

                        if (headerList.Contains("deletedname"))
                        {
                            question.DeletedName = csv.GetField<string>("DeletedName");
                        }

                        if (headerList.Contains("deletedon"))
                        {
                            var deletedOnStr = csv.GetField<string>("DeletedOn");
                            if (DateTime.TryParse(deletedOnStr, out var deletedOn))
                            {
                                question.DeletedOn = deletedOn;
                            }
                        }

                        if (headerList.Contains("lastmodifiedby"))
                        {
                            question.LastModifiedBy = csv.GetField<string>("LastModifiedBy");
                        }

                        if (headerList.Contains("lastmodifiedname"))
                        {
                            question.LastModifiedName = csv.GetField<string>("LastModifiedName");
                        }

                        if (headerList.Contains("lastmodifiedon"))
                        {
                            var lastModifiedOnStr = csv.GetField<string>("LastModifiedOn");
                            if (DateTime.TryParse(lastModifiedOnStr, out var lastModifiedOn))
                            {
                                question.LastModifiedOn = lastModifiedOn;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(question.Question))
                        {
                            questions.Add(question);
                        }
                    }
                }

                // 插入数据库
                var insertedCount = await dbService.InsertHotQuestionsAsync(questions);

                return Results.Content($"<div class='success'>成功导入 {insertedCount} 条记录</div>", "text/html");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // 处理MySQL特定错误
                string errorMessage;
                if (ex.Message.Contains("Unknown database"))
                {
                    errorMessage = "MySQL数据库不存在，请检查数据库名称是否正确";
                    throw;
                }
                else if (ex.Message.Contains("caching_sha2_password"))
                {
                    errorMessage = "MySQL身份验证失败，可能需要修改身份验证方法";
                    throw;
                }
                else if (ex.Message.Contains("Access denied"))
                {
                    errorMessage = "MySQL用户名或密码错误";
                    throw;
                }
                else if (ex.Message.Contains("Connection refused"))
                {
                    errorMessage = "无法连接到MySQL服务器，请检查服务器地址和端口";
                    throw;
                }
                else
                {
                    errorMessage = $"MySQL操作失败: {ex.Message}";
                    throw;
                }
                return Results.Content($"<div class='error'>{errorMessage}</div>", "text/html");
            }
            catch (Exception ex)
            {
                return Results.Content($"<div class='error'>处理失败: {ex.Message}</div>", "text/html");
            }
        }).DisableAntiforgery();

await app.RunAsync();

// 数据模型
public class HotQuestion
{
    public Guid QAId { get; set; } = Guid.NewGuid();
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public List<string> KeyWords { get; set; } = [];
    public string BusType { get; set; } = "common";
    public string? Mode { get; set; }
    public string Language { get; set; } = "zh";
    public string? Robot { get; set; }
    public string? Description { get; set; }
    public Guid? PromptCrispeId { get; set; }
    public DateTime? RecordTime { get; set; } = DateTime.Now;
    public string? ExtraProperties { get; set; }
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public DateTime? RowVersion { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public string? CreatorBy { get; set; }
    public string? CreatorName { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.Now;
    public string? DeletedBy { get; set; }
    public string? DeletedName { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public string? LastModifiedName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

// 数据库服务
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> InsertHotQuestionsAsync(List<HotQuestion> questions)
    {
        // 参数验证
        if (questions == null || questions.Count == 0)
            return 0;

        try
        {
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                    INSERT INTO robot_t_qa 
                    (qaid, question, answer, keywords, bustype, mode, lang, robot, description, promptcrispeid, recordtime, 
                     extraproperties, concurrencystamp, rowversion, isdeleted, creatorby, creatorname, createdon,
                     deletedby, deletedname, deletedon, lastmodifiedby, lastmodifiedname, lastmodifiedon)
                    VALUES 
                    (@QAId, @Question, @Answer, @KeyWords, @BusType, @Mode, @Lang, @Robot, @Description, @PromptCrispeId, NOW(), 
                     @ExtraProperties, @ConcurrencyStamp, NOW(), @IsDeleted, @CreatorBy, @CreatorName, NOW(),
                     @DeletedBy, @DeletedName, @DeletedOn, @LastModifiedBy, @LastModifiedName, @LastModifiedOn)
                ";

            int insertedCount = 0;

            // 使用批量插入提高性能
            using (var transaction = await connection.BeginTransactionAsync())
            {
                using var command = new MySql.Data.MySqlClient.MySqlCommand(query, connection, transaction);
                // 添加参数但不赋值
                var qaIdParam = command.Parameters.Add("@QAId", MySql.Data.MySqlClient.MySqlDbType.VarChar, 36);
                var questionParam = command.Parameters.Add("@Question", MySql.Data.MySqlClient.MySqlDbType.VarChar, 256);
                var answerParam = command.Parameters.Add("@Answer", MySql.Data.MySqlClient.MySqlDbType.LongText);
                var keywordsParam = command.Parameters.Add("@KeyWords", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var busTypeParam = command.Parameters.Add("@BusType", MySql.Data.MySqlClient.MySqlDbType.VarChar, 32);
                var modeParam = command.Parameters.Add("@Mode", MySql.Data.MySqlClient.MySqlDbType.VarChar, 16);
                var langParam = command.Parameters.Add("@Lang", MySql.Data.MySqlClient.MySqlDbType.VarChar, 16);
                var robotParam = command.Parameters.Add("@Robot", MySql.Data.MySqlClient.MySqlDbType.VarChar, 32);
                var descriptionParam = command.Parameters.Add("@Description", MySql.Data.MySqlClient.MySqlDbType.VarChar, 256);
                var promptCrispeIdParam = command.Parameters.Add("@PromptCrispeId", MySql.Data.MySqlClient.MySqlDbType.VarChar, 36);
                var extraPropertiesParam = command.Parameters.Add("@ExtraProperties", MySql.Data.MySqlClient.MySqlDbType.LongText);
                var concurrencyStampParam = command.Parameters.Add("@ConcurrencyStamp", MySql.Data.MySqlClient.MySqlDbType.VarChar, 40);
                var isDeletedParam = command.Parameters.Add("@IsDeleted", MySql.Data.MySqlClient.MySqlDbType.Bit);
                var creatorByParam = command.Parameters.Add("@CreatorBy", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var creatorNameParam = command.Parameters.Add("@CreatorName", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var deletedByParam = command.Parameters.Add("@DeletedBy", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var deletedNameParam = command.Parameters.Add("@DeletedName", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var deletedOnParam = command.Parameters.Add("@DeletedOn", MySql.Data.MySqlClient.MySqlDbType.DateTime);
                var lastModifiedByParam = command.Parameters.Add("@LastModifiedBy", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var lastModifiedNameParam = command.Parameters.Add("@LastModifiedName", MySql.Data.MySqlClient.MySqlDbType.VarChar, 64);
                var lastModifiedOnParam = command.Parameters.Add("@LastModifiedOn", MySql.Data.MySqlClient.MySqlDbType.DateTime);

                // 批量执行
                foreach (var question in questions)
                {
                    if (string.IsNullOrWhiteSpace(question?.Question))
                        continue;

                    qaIdParam.Value = question.QAId.ToString();
                    questionParam.Value = question.Question;
                    answerParam.Value = string.IsNullOrEmpty(question.Answer) ? DBNull.Value : (object)question.Answer;
                    keywordsParam.Value = string.Join(",", question.KeyWords);
                    busTypeParam.Value = question.BusType;
                    modeParam.Value = string.IsNullOrEmpty(question.Mode) ? DBNull.Value : (object)question.Mode;
                    langParam.Value = question.Language;
                    robotParam.Value = string.IsNullOrEmpty(question.Robot) ? DBNull.Value : (object)question.Robot;
                    descriptionParam.Value = string.IsNullOrEmpty(question.Description) ? DBNull.Value : (object)question.Description;
                    promptCrispeIdParam.Value = question.PromptCrispeId.HasValue ? (object)question.PromptCrispeId.Value.ToString() : DBNull.Value;
                    extraPropertiesParam.Value = string.IsNullOrEmpty(question.ExtraProperties) ? DBNull.Value : (object)question.ExtraProperties;
                    concurrencyStampParam.Value = string.IsNullOrEmpty(question.ConcurrencyStamp) ? DBNull.Value : (object)question.ConcurrencyStamp;
                    isDeletedParam.Value = question.IsDeleted;
                    creatorByParam.Value = string.IsNullOrEmpty(question.CreatorBy) ? DBNull.Value : (object)question.CreatorBy;
                    creatorNameParam.Value = string.IsNullOrEmpty(question.CreatorName) ? DBNull.Value : (object)question.CreatorName;
                    deletedByParam.Value = string.IsNullOrEmpty(question.DeletedBy) ? DBNull.Value : (object)question.DeletedBy;
                    deletedNameParam.Value = string.IsNullOrEmpty(question.DeletedName) ? DBNull.Value : (object)question.DeletedName;
                    deletedOnParam.Value = question.DeletedOn.HasValue ? (object)question.DeletedOn.Value : DBNull.Value;
                    lastModifiedByParam.Value = string.IsNullOrEmpty(question.LastModifiedBy) ? DBNull.Value : (object)question.LastModifiedBy;
                    lastModifiedNameParam.Value = string.IsNullOrEmpty(question.LastModifiedName) ? DBNull.Value : (object)question.LastModifiedName;
                    lastModifiedOnParam.Value = question.LastModifiedOn.HasValue ? (object)question.LastModifiedOn.Value : DBNull.Value;

                    insertedCount += await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                Console.WriteLine($"数据库事务提交成功，共插入 {insertedCount} 条数据");
            }

            return insertedCount;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"数据库插入失败: {ex.Message}");
            throw;
        }
    }
}
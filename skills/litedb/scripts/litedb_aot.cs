#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.CommandLine@2.0.0
#:package LiteDB@5.0.16
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property Optimize=true
#:property PublishTrimmed=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiteDBAot
{
    /// <summary>
    /// LiteDB 服务接口
    /// </summary>
    public interface ILiteDBService
    {
        /// <summary>
        /// 获取数据库实例
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>数据库实例</returns>
        ILiteDatabase GetDatabase(string databasePath);

        /// <summary>
        /// 创建集合
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <returns>是否成功</returns>
        Task<bool> CreateCollectionAsync(string databasePath, string collectionName);

        /// <summary>
        /// 插入文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="document">文档内容</param>
        /// <returns>插入结果</returns>
        Task<string> InsertDocumentAsync(string databasePath, string collectionName, string document);

        /// <summary>
        /// 查询文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        /// <returns>查询结果</returns>
        Task<string> QueryDocumentsAsync(string databasePath, string collectionName, string query = null);

        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="id">文档ID</param>
        /// <param name="document">文档内容</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateDocumentAsync(string databasePath, string collectionName, string id, string document);

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="id">文档ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteDocumentAsync(string databasePath, string collectionName, string id);

        /// <summary>
        /// 删除集合
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <returns>是否成功</returns>
        Task<bool> DropCollectionAsync(string databasePath, string collectionName);

        /// <summary>
        /// 列出所有集合
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>集合列表</returns>
        Task<string> ListCollectionsAsync(string databasePath);

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="backupPath">备份路径</param>
        /// <returns>是否成功</returns>
        Task<bool> BackupDatabaseAsync(string databasePath, string backupPath);

        /// <summary>
        /// 压缩数据库
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>是否成功</returns>
        Task<bool> CompactDatabaseAsync(string databasePath);

        /// <summary>
        /// 获取数据库信息
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>数据库信息</returns>
        Task<string> GetDatabaseInfoAsync(string databasePath);
    }

    /// <summary>
    /// LiteDB 服务实现
    /// </summary>
    public class LiteDBService : ILiteDBService
    {
        private readonly ILogger<LiteDBService> _logger;
        private readonly IMemoryCache _cache;
        private readonly Dictionary<string, ILiteDatabase> _databaseInstances = new();
        private readonly object _lock = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="cache">内存缓存</param>
        public LiteDBService(ILogger<LiteDBService> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// 获取数据库实例
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>数据库实例</returns>
        public ILiteDatabase GetDatabase(string databasePath)
        {
            lock (_lock)
            {
                if (_databaseInstances.TryGetValue(databasePath, out var database))
                {
                    return database;
                }

                try
                {
                    // 确保目录存在
                    var directory = Path.GetDirectoryName(databasePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // 创建数据库实例
                    var connectionString = new ConnectionString(databasePath)
                    {
                        Mode = FileMode.Exclusive,
                        Journal = true,
                        CacheSize = 1024,
                        Timeout = TimeSpan.FromMinutes(1)
                    };

                    database = new LiteDatabase(connectionString);
                    _databaseInstances[databasePath] = database;

                    _logger.LogInformation($"数据库已打开: {databasePath}");
                    return database;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"打开数据库失败: {databasePath}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 创建集合
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <returns>是否成功</returns>
        public async Task<bool> CreateCollectionAsync(string databasePath, string collectionName)
        {
            try
            {
                var database = GetDatabase(databasePath);
                database.GetCollection(collectionName);
                _logger.LogInformation($"集合已创建: {collectionName} 在数据库 {databasePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建集合失败: {collectionName} 在数据库 {databasePath}");
                return false;
            }
        }

        /// <summary>
        /// 插入文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="document">文档内容</param>
        /// <returns>插入结果</returns>
        public async Task<string> InsertDocumentAsync(string databasePath, string collectionName, string document)
        {
            try
            {
                var database = GetDatabase(databasePath);
                var collection = database.GetCollection(collectionName);
                var bsonDocument = BsonDocument.Parse(document);
                var id = collection.Insert(bsonDocument);
                _logger.LogInformation($"文档已插入到集合 {collectionName}，ID: {id}");
                return id.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"插入文档失败到集合 {collectionName}");
                throw;
            }
        }

        /// <summary>
        /// 查询文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        /// <returns>查询结果</returns>
        public async Task<string> QueryDocumentsAsync(string databasePath, string collectionName, string query = null)
        {
            try
            {
                var database = GetDatabase(databasePath);
                var collection = database.GetCollection(collectionName);
                IEnumerable<BsonDocument> results;

                if (string.IsNullOrEmpty(query))
                {
                    results = collection.FindAll();
                }
                else
                {
                    var bsonQuery = BsonExpression.Create(query);
                    results = collection.Find(bsonQuery);
                }

                var documents = new List<object>();
                foreach (var doc in results)
                {
                    documents.Add(doc.AsDocument);
                }

                var jsonResult = JsonSerializer.Serialize(documents, new JsonSerializerOptions { WriteIndented = true });
                return jsonResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"查询文档失败从集合 {collectionName}");
                throw;
            }
        }

        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="id">文档ID</param>
        /// <param name="document">文档内容</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDocumentAsync(string databasePath, string collectionName, string id, string document)
        {
            try
            {
                var database = GetDatabase(databasePath);
                var collection = database.GetCollection(collectionName);
                var bsonDocument = BsonDocument.Parse(document);
                bsonDocument["_id"] = BsonValue.Parse(id);
                var result = collection.Update(bsonDocument);
                _logger.LogInformation($"文档已更新在集合 {collectionName}，ID: {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新文档失败在集合 {collectionName}，ID: {id}");
                return false;
            }
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <param name="id">文档ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDocumentAsync(string databasePath, string collectionName, string id)
        {
            try
            {
                var database = GetDatabase(databasePath);
                var collection = database.GetCollection(collectionName);
                var bsonId = BsonValue.Parse(id);
                var result = collection.Delete(bsonId);
                _logger.LogInformation($"文档已删除从集合 {collectionName}，ID: {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除文档失败从集合 {collectionName}，ID: {id}");
                return false;
            }
        }

        /// <summary>
        /// 删除集合
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="collectionName">集合名称</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DropCollectionAsync(string databasePath, string collectionName)
        {
            try
            {
                var database = GetDatabase(databasePath);
                database.DropCollection(collectionName);
                _logger.LogInformation($"集合已删除: {collectionName} 在数据库 {databasePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除集合失败: {collectionName} 在数据库 {databasePath}");
                return false;
            }
        }

        /// <summary>
        /// 列出所有集合
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>集合列表</returns>
        public async Task<string> ListCollectionsAsync(string databasePath)
        {
            try
            {
                var database = GetDatabase(databasePath);
                var collections = database.GetCollectionNames();
                var jsonResult = JsonSerializer.Serialize(collections, new JsonSerializerOptions { WriteIndented = true });
                return jsonResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"列出集合失败在数据库 {databasePath}");
                throw;
            }
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <param name="backupPath">备份路径</param>
        /// <returns>是否成功</returns>
        public async Task<bool> BackupDatabaseAsync(string databasePath, string backupPath)
        {
            try
            {
                // 确保备份目录存在
                var backupDirectory = Path.GetDirectoryName(backupPath);
                if (!string.IsNullOrEmpty(backupDirectory) && !Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                using (var sourceDb = new LiteDatabase(databasePath))
                {
                    sourceDb.Backup(backupPath);
                }

                _logger.LogInformation($"数据库已备份: {databasePath} 到 {backupPath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"备份数据库失败: {databasePath} 到 {backupPath}");
                return false;
            }
        }

        /// <summary>
        /// 压缩数据库
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>是否成功</returns>
        public async Task<bool> CompactDatabaseAsync(string databasePath)
        {
            try
            {
                using (var db = new LiteDatabase(databasePath))
                {
                    db.Shrink();
                }

                _logger.LogInformation($"数据库已压缩: {databasePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"压缩数据库失败: {databasePath}");
                return false;
            }
        }

        /// <summary>
        /// 获取数据库信息
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        /// <returns>数据库信息</returns>
        public async Task<string> GetDatabaseInfoAsync(string databasePath)
        {
            try
            {
                var database = GetDatabase(databasePath);
                var collectionNames = database.GetCollectionNames();
                var fileInfo = new FileInfo(databasePath);

                var info = new
                {
                    DatabasePath = databasePath,
                    FileSize = fileInfo.Length,
                    CollectionCount = collectionNames.Count,
                    Collections = collectionNames
                };

                var jsonResult = JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
                return jsonResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取数据库信息失败: {databasePath}");
                throw;
            }
        }
    }

    /// <summary>
    /// 程序主类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 设置依赖注入
            var serviceProvider = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConsole()
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Information))
                .AddMemoryCache()
                .AddSingleton<ILiteDBService, LiteDBService>()
                .BuildServiceProvider();

            // 创建命令行根命令
            var rootCommand = new RootCommand("LiteDB AOT 工具 - 用于 LiteDB 数据库操作");

            // 创建子命令
            var createCollectionCommand = new Command("create-collection", "创建集合")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--collection", "集合名称") { IsRequired = true }
            };

            var insertDocumentCommand = new Command("insert", "插入文档")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--collection", "集合名称") { IsRequired = true },
                new Option<string>("--document", "文档内容") { IsRequired = true }
            };

            var queryDocumentsCommand = new Command("query", "查询文档")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--collection", "集合名称") { IsRequired = true },
                new Option<string>("--query", "查询条件") { IsRequired = false }
            };

            var updateDocumentCommand = new Command("update", "更新文档")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--collection", "集合名称") { IsRequired = true },
                new Option<string>("--id", "文档ID") { IsRequired = true },
                new Option<string>("--document", "文档内容") { IsRequired = true }
            };

            var deleteDocumentCommand = new Command("delete", "删除文档")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--collection", "集合名称") { IsRequired = true },
                new Option<string>("--id", "文档ID") { IsRequired = true }
            };

            var dropCollectionCommand = new Command("drop-collection", "删除集合")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--collection", "集合名称") { IsRequired = true }
            };

            var listCollectionsCommand = new Command("list-collections", "列出所有集合")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true }
            };

            var backupCommand = new Command("backup", "备份数据库")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true },
                new Option<string>("--backup", "备份路径") { IsRequired = true }
            };

            var compactCommand = new Command("compact", "压缩数据库")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true }
            };

            var infoCommand = new Command("info", "获取数据库信息")
            {
                new Option<string>("--database", "数据库路径") { IsRequired = true }
            };

            // 设置命令处理器
            createCollectionCommand.Handler = CommandHandler.Create<string, string>(async (database, collection) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.CreateCollectionAsync(database, collection);
                Console.WriteLine(result ? "集合创建成功" : "集合创建失败");
            });

            insertDocumentCommand.Handler = CommandHandler.Create<string, string, string>(async (database, collection, document) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var id = await service.InsertDocumentAsync(database, collection, document);
                Console.WriteLine($"文档插入成功，ID: {id}");
            });

            queryDocumentsCommand.Handler = CommandHandler.Create<string, string, string>(async (database, collection, query) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.QueryDocumentsAsync(database, collection, query);
                Console.WriteLine(result);
            });

            updateDocumentCommand.Handler = CommandHandler.Create<string, string, string, string>(async (database, collection, id, document) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.UpdateDocumentAsync(database, collection, id, document);
                Console.WriteLine(result ? "文档更新成功" : "文档更新失败");
            });

            deleteDocumentCommand.Handler = CommandHandler.Create<string, string, string>(async (database, collection, id) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.DeleteDocumentAsync(database, collection, id);
                Console.WriteLine(result ? "文档删除成功" : "文档删除失败");
            });

            dropCollectionCommand.Handler = CommandHandler.Create<string, string>(async (database, collection) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.DropCollectionAsync(database, collection);
                Console.WriteLine(result ? "集合删除成功" : "集合删除失败");
            });

            listCollectionsCommand.Handler = CommandHandler.Create<string>(async (database) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.ListCollectionsAsync(database);
                Console.WriteLine(result);
            });

            backupCommand.Handler = CommandHandler.Create<string, string>(async (database, backup) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.BackupDatabaseAsync(database, backup);
                Console.WriteLine(result ? "数据库备份成功" : "数据库备份失败");
            });

            compactCommand.Handler = CommandHandler.Create<string>(async (database) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.CompactDatabaseAsync(database);
                Console.WriteLine(result ? "数据库压缩成功" : "数据库压缩失败");
            });

            infoCommand.Handler = CommandHandler.Create<string>(async (database) =>
            {
                var service = serviceProvider.GetRequiredService<ILiteDBService>();
                var result = await service.GetDatabaseInfoAsync(database);
                Console.WriteLine(result);
            });

            // 添加子命令到根命令
            rootCommand.AddCommand(createCollectionCommand);
            rootCommand.AddCommand(insertDocumentCommand);
            rootCommand.AddCommand(queryDocumentsCommand);
            rootCommand.AddCommand(updateDocumentCommand);
            rootCommand.AddCommand(deleteDocumentCommand);
            rootCommand.AddCommand(dropCollectionCommand);
            rootCommand.AddCommand(listCollectionsCommand);
            rootCommand.AddCommand(backupCommand);
            rootCommand.AddCommand(compactCommand);
            rootCommand.AddCommand(infoCommand);

            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
    }
}
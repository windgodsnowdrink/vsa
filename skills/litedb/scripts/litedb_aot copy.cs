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

                    _logger.LogInformation($"数据库已打开: {databasePath}
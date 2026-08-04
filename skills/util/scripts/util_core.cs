#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Scrutor@4.2.2
#:package System.Text.Json@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ===============================
// Util 核心服务
// ===============================

namespace Util.Services
{
    public interface IUtilService
    {
        IFileUtil FileUtil { get; }
        ICryptoUtil CryptoUtil { get; }
        ISerializationUtil SerializationUtil { get; }
        IConfigUtil ConfigUtil { get; }
        IReflectionUtil ReflectionUtil { get; }
        ITimeUtil TimeUtil { get; }
        INetworkUtil NetworkUtil { get; }
    }

    public class DefaultUtilService : IUtilService
    {
        public IFileUtil FileUtil { get; }
        public ICryptoUtil CryptoUtil { get; }
        public ISerializationUtil SerializationUtil { get; }
        public IConfigUtil ConfigUtil { get; }
        public IReflectionUtil ReflectionUtil { get; }
        public ITimeUtil TimeUtil { get; }
        public INetworkUtil NetworkUtil { get; }

        public DefaultUtilService(
            IFileUtil fileUtil,
            ICryptoUtil cryptoUtil,
            ISerializationUtil serializationUtil,
            IConfigUtil configUtil,
            IReflectionUtil reflectionUtil,
            ITimeUtil timeUtil,
            INetworkUtil networkUtil)
        {
            FileUtil = fileUtil;
            CryptoUtil = cryptoUtil;
            SerializationUtil = serializationUtil;
            ConfigUtil = configUtil;
            ReflectionUtil = reflectionUtil;
            TimeUtil = timeUtil;
            NetworkUtil = networkUtil;
        }
    }

    // ===============================
    // 文件工具
    // ===============================

    public interface IFileUtil
    {
        Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);
        Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
        Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
        Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);
        Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
        Task DeleteAsync(string path, CancellationToken cancellationToken = default);
        Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
        Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default);
        Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default);
        Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
        Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default);
        Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default);
    }

    public class DefaultFileUtil : IFileUtil
    {
        private readonly ILogger<DefaultFileUtil> _logger;
        private readonly FileUtilSettings _settings;

        public DefaultFileUtil(ILogger<DefaultFileUtil> logger, IOptions<FileUtilSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Checking if file exists: {Path}", path);
            return Task.FromResult(File.Exists(path));
        }

        public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Reading all text from file: {Path}", path);
            return await File.ReadAllTextAsync(path, cancellationToken);
        }

        public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Writing all text to file: {Path}", path);
            
            // 检查文件大小
            if (content.Length > _settings.MaxFileSize)
            {
                throw new IOException($"File size exceeds maximum limit of {_settings.MaxFileSize} bytes");
            }
            
            await File.WriteAllTextAsync(path, content, cancellationToken);
        }

        public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Reading all bytes from file: {Path}", path);
            return await File.ReadAllBytesAsync(path, cancellationToken);
        }

        public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Writing all bytes to file: {Path}", path);
            
            // 检查文件大小
            if (bytes.Length > _settings.MaxFileSize)
            {
                throw new IOException($"File size exceeds maximum limit of {_settings.MaxFileSize} bytes");
            }
            
            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Deleting file: {Path}", path);
            File.Delete(path);
            return Task.CompletedTask;
        }

        public Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Copying file from {Source} to {Destination}", sourcePath, destinationPath);
            File.Copy(sourcePath, destinationPath, true);
            return Task.CompletedTask;
        }

        public Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Moving file from {Source} to {Destination}", sourcePath, destinationPath);
            
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            File.Move(sourcePath, destinationPath);
            return Task.CompletedTask;
        }

        public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Checking if directory exists: {Path}", path);
            return Task.FromResult(Directory.Exists(path));
        }

        public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Creating directory: {Path}", path);
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Deleting directory: {Path}", path);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<string>> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Enumerating files in directory: {Path}", path);
            var files = Directory.EnumerateFiles(path);
            return Task.FromResult(files);
        }

        public Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Getting file info for: {Path}", path);
            var info = new FileInfo(path);
            return Task.FromResult(info);
        }
    }

    // ===============================
    // 加密工具
    // ===============================

    public interface ICryptoUtil
    {
        Task<string> EncryptAsync(string plainText, string key = null, CancellationToken cancellationToken = default);
        Task<string> DecryptAsync(string encryptedText, string key = null, CancellationToken cancellationToken = default);
        Task<string> ComputeHashAsync(string input, string algorithm = "SHA256", CancellationToken cancellationToken = default);
        Task<bool> VerifyHashAsync(string input, string hash, string algorithm = "SHA256", CancellationToken cancellationToken = default);
        Task<string> ComputePasswordHashAsync(string password, CancellationToken cancellationToken = default);
        Task<bool> VerifyPasswordHashAsync(string password, string hash, CancellationToken cancellationToken = default);
        Task<string> GenerateRandomStringAsync(int length, CancellationToken cancellationToken = default);
        Task<int> GenerateRandomNumberAsync(int min, int max, CancellationToken cancellationToken = default);
    }

    public class DefaultCryptoUtil : ICryptoUtil
    {
        private readonly ILogger<DefaultCryptoUtil> _logger;
        private readonly CryptoUtilSettings _settings;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public DefaultCryptoUtil(ILogger<DefaultCryptoUtil> logger, IOptions<CryptoUtilSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
            
            // 初始化默认密钥
            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes("UtilDefaultEncryptionKey1234567890"));
            _iv = new byte[16]; // 简化处理，实际应用中应该使用随机IV
        }

        public async Task<string> EncryptAsync(string plainText, string key = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Encrypting text with length: {Length}", plainText.Length);
            
            var keyBytes = string.IsNullOrEmpty(key) ? _key : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);
            
            await swEncrypt.WriteAsync(plainText);
            await swEncrypt.FlushAsync();
            csEncrypt.FlushFinalBlock();
            
            var encrypted = msEncrypt.ToArray();
            var result = Convert.ToBase64String(encrypted);
            _logger.LogDebug("Encryption completed, result length: {Length}", result.Length);
            return result;
        }

        public async Task<string> DecryptAsync(string encryptedText, string key = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Decrypting text with length: {Length}", encryptedText.Length);
            
            var keyBytes = string.IsNullOrEmpty(key) ? _key : SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
            var cipherText = Convert.FromBase64String(encryptedText);
            
            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            
            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipherText);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            var result = await srDecrypt.ReadToEndAsync();
            _logger.LogDebug("Decryption completed, result length: {Length}", result.Length);
            return result;
        }

        public Task<string> ComputeHashAsync(string input, string algorithm = "SHA256", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Computing hash for input with length: {Length} using algorithm: {Algorithm}", input.Length, algorithm);
            
            HashAlgorithm hashAlgorithm;
            switch (algorithm.ToUpper())
            {
                case "MD5":
                    hashAlgorithm = MD5.Create();
                    break;
                case "SHA1":
                    hashAlgorithm = SHA1.Create();
                    break;
                case "SHA256":
                default:
                    hashAlgorithm = SHA256.Create();
                    break;
            }
            
            using (hashAlgorithm)
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = hashAlgorithm.ComputeHash(inputBytes);
                var result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                _logger.LogDebug("Hash computation completed");
                return Task.FromResult(result);
            }
        }

        public Task<bool> VerifyHashAsync(string input, string hash, string algorithm = "SHA256", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Verifying hash for input with length: {Length}", input.Length);
            return ComputeHashAsync(input, algorithm, cancellationToken)
                .ContinueWith(t => t.Result.Equals(hash, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        public Task<string> ComputePasswordHashAsync(string password, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Computing password hash");
            
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            
            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            
            var result = Convert.ToBase64String(hashBytes);
            _logger.LogDebug("Password hash computation completed");
            return Task.FromResult(result);
        }

        public Task<bool> VerifyPasswordHashAsync(string password, string hash, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Verifying password hash");
            
            var hashBytes = Convert.FromBase64String(hash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(32);
            
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != computedHash[i])
                {
                    _logger.LogDebug("Password hash verification failed");
                    return Task.FromResult(false);
                }
            }
            
            _logger.LogDebug("Password hash verification succeeded");
            return Task.FromResult(true);
        }

        public Task<string> GenerateRandomStringAsync(int length, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Generating random string with length: {Length}", length);
            
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringBuilder = new StringBuilder(length);
            var random = new Random();
            
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            
            var result = stringBuilder.ToString();
            _logger.LogDebug("Random string generation completed");
            return Task.FromResult(result);
        }

        public Task<int> GenerateRandomNumberAsync(int min, int max, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Generating random number between {Min} and {Max}", min, max);
            var random = new Random();
            var result = random.Next(min, max + 1);
            _logger.LogDebug("Random number generation completed: {Result}", result);
            return Task.FromResult(result);
        }
    }

    // ===============================
    // 序列化工具
    // ===============================

    public interface ISerializationUtil
    {
        Task<string> SerializeToJsonAsync(object value, CancellationToken cancellationToken = default);
        Task<T> DeserializeFromJsonAsync<T>(string json, CancellationToken cancellationToken = default);
        Task<string> SerializeToXmlAsync(object value, CancellationToken cancellationToken = default);
        Task<T> DeserializeFromXmlAsync<T>(string xml, CancellationToken cancellationToken = default);
        Task<byte[]> SerializeToBinaryAsync(object value, CancellationToken cancellationToken = default);
        Task<T> DeserializeFromBinaryAsync<T>(byte[] bytes, CancellationToken cancellationToken = default);
    }

    public class DefaultSerializationUtil : ISerializationUtil
    {
        private readonly ILogger<DefaultSerializationUtil> _logger;
        private readonly System.Text.Json.JsonSerializerOptions _jsonOptions;

        public DefaultSerializationUtil(ILogger<DefaultSerializationUtil> logger)
        {
            _logger = logger;
            _jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                IgnoreNullValues = true
            };
        }

        public Task<string> SerializeToJsonAsync(object value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Serializing object to JSON");
            var json = System.Text.Json.JsonSerializer.Serialize(value, _jsonOptions);
            _logger.LogDebug("JSON serialization completed, result length: {Length}", json.Length);
            return Task.FromResult(json);
        }

        public Task<T> DeserializeFromJsonAsync<T>(string json, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Deserializing JSON to object of type: {Type}", typeof(T).Name);
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(json, _jsonOptions);
            _logger.LogDebug("JSON deserialization completed");
            return Task.FromResult(result);
        }

        public Task<string> SerializeToXmlAsync(object value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Serializing object to XML");
            
            using var stringWriter = new StringWriter();
            var serializer = new System.Xml.Serialization.XmlSerializer(value.GetType());
            serializer.Serialize(stringWriter, value);
            var xml = stringWriter.ToString();
            
            _logger.LogDebug("XML serialization completed, result length: {Length}", xml.Length);
            return Task.FromResult(xml);
        }

        public Task<T> DeserializeFromXmlAsync<T>(string xml, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Deserializing XML to object of type: {Type}", typeof(T).Name);
            
            using var stringReader = new StringReader(xml);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var result = (T)serializer.Deserialize(stringReader);
            
            _logger.LogDebug("XML deserialization completed");
            return Task.FromResult(result);
        }

        public Task<byte[]> SerializeToBinaryAsync(object value, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Serializing object to binary");
            
            using var memoryStream = new MemoryStream();
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(memoryStream, value);
            var bytes = memoryStream.ToArray();
            
            _logger.LogDebug("Binary serialization completed, result length: {Length}", bytes.Length);
            return Task.FromResult(bytes);
        }

        public Task<T> DeserializeFromBinaryAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Deserializing binary to object of type: {Type}", typeof(T).Name);
            
            using var memoryStream = new MemoryStream(bytes);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var result = (T)formatter.Deserialize(memoryStream);
            
            _logger.LogDebug("Binary deserialization completed");
            return Task.FromResult(result);
        }
    }

    // ===============================
    // 配置工具
    // ===============================

    public interface IConfigUtil
    {
        T GetSection<T>(string sectionName) where T : class, new();
        string GetValue(string key, string defaultValue = null);
        int GetValue(string key, int defaultValue);
        bool GetValue(string key, bool defaultValue);
        double GetValue(string key, double defaultValue);
        T GetValue<T>(string key, T defaultValue);
        void Reload();
    }

    public class DefaultConfigUtil : IConfigUtil
    {
        private readonly ILogger<DefaultConfigUtil> _logger;
        private readonly IConfigurationRoot _configuration;

        public DefaultConfigUtil(ILogger<DefaultConfigUtil> logger, IConfigurationRoot configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public T GetSection<T>(string sectionName) where T : class, new()
        {
            _logger.LogDebug("Getting configuration section: {SectionName}", sectionName);
            var section = new T();
            _configuration.GetSection(sectionName).Bind(section);
            return section;
        }

        public string GetValue(string key, string defaultValue = null)
        {
            _logger.LogDebug("Getting string value for key: {Key}", key);
            return _configuration.GetValue<string>(key, defaultValue);
        }

        public int GetValue(string key, int defaultValue)
        {
            _logger.LogDebug("Getting int value for key: {Key}", key);
            return _configuration.GetValue<int>(key, defaultValue);
        }

        public bool GetValue(string key, bool defaultValue)
        {
            _logger.LogDebug("Getting bool value for key: {Key}", key);
            return _configuration.GetValue<bool>(key, defaultValue);
        }

        public double GetValue(string key, double defaultValue)
        {
            _logger.LogDebug("Getting double value for key: {Key}", key);
            return _configuration.GetValue<double>(key, defaultValue);
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            _logger.LogDebug("Getting value of type {Type} for key: {Key}", typeof(T).Name, key);
            return _configuration.GetValue<T>(key, defaultValue);
        }

        public void Reload()
        {
            _logger.LogDebug("Reloading configuration");
            _configuration.Reload();
        }
    }

    // ===============================
    // 反射工具
    // ===============================

    public interface IReflectionUtil
    {
        Type GetType(string typeName);
        object CreateInstance(Type type, params object[] args);
        object CreateInstance(string typeName, params object[] args);
        MethodInfo GetMethod(Type type, string methodName, params Type[] parameterTypes);
        MethodInfo GetMethod(string typeName, string methodName, params Type[] parameterTypes);
        object InvokeMethod(object instance, string methodName, params object[] args);
        object InvokeMethod(Type type, string methodName, object instance, params object[] args);
        PropertyInfo GetProperty(Type type, string propertyName);
        PropertyInfo GetProperty(string typeName, string propertyName);
        object GetPropertyValue(object instance, string propertyName);
        void SetPropertyValue(object instance, string propertyName, object value);
        FieldInfo GetField(Type type, string fieldName);
        FieldInfo GetField(string typeName, string fieldName);
        object GetFieldValue(object instance, string fieldName);
        void SetFieldValue(object instance, string fieldName, object value);
        IEnumerable<MethodInfo> GetMethods(Type type);
        IEnumerable<PropertyInfo> GetProperties(Type type);
        IEnumerable<FieldInfo> GetFields(Type type);
        IEnumerable<Attribute> GetAttributes(Type type);
        bool HasAttribute(Type type, Type attributeType);
        T GetAttribute<T>(Type type) where T : Attribute;
    }

    public class DefaultReflectionUtil : IReflectionUtil
    {
        private readonly ILogger<DefaultReflectionUtil> _logger;

        public DefaultReflectionUtil(ILogger<DefaultReflectionUtil> logger)
        {
            _logger = logger;
        }

        public Type GetType(string typeName)
        {
            _logger.LogDebug("Getting type by name: {TypeName}", typeName);
            return Type.GetType(typeName, true);
        }

        public object CreateInstance(Type type, params object[] args)
        {
            _logger.LogDebug("Creating instance of type: {TypeName}", type.Name);
            return Activator.CreateInstance(type, args);
        }

        public object CreateInstance(string typeName, params object[] args)
        {
            _logger.LogDebug("Creating instance of type by name: {TypeName}", typeName);
            var type = GetType(typeName);
            return CreateInstance(type, args);
        }

        public MethodInfo GetMethod(Type type, string methodName, params Type[] parameterTypes)
        {
            _logger.LogDebug("Getting method {MethodName} from type: {TypeName}", methodName, type.Name);
            return type.GetMethod(methodName, parameterTypes);
        }

        public MethodInfo GetMethod(string typeName, string methodName, params Type[] parameterTypes)
        {
            _logger.LogDebug("Getting method {MethodName} from type by name: {TypeName}", methodName, typeName);
            var type = GetType(typeName);
            return GetMethod(type, methodName, parameterTypes);
        }

        public object InvokeMethod(object instance, string methodName, params object[] args)
        {
            _logger.LogDebug("Invoking method {MethodName} on instance of type: {TypeName}", methodName, instance.GetType().Name);
            var method = instance.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new MissingMethodException(instance.GetType().Name, methodName);
            }
            return method.Invoke(instance, args);
        }

        public object InvokeMethod(Type type, string methodName, object instance, params object[] args)
        {
            _logger.LogDebug("Invoking method {MethodName} on type: {TypeName}", methodName, type.Name);
            var method = GetMethod(type, methodName);
            if (method == null)
            {
                throw new MissingMethodException(type.Name, methodName);
            }
            return method.Invoke(instance, args);
        }

        public PropertyInfo GetProperty(Type type, string propertyName)
        {
            _logger.LogDebug("Getting property {PropertyName} from type: {TypeName}", propertyName, type.Name);
            return type.GetProperty(propertyName);
        }

        public PropertyInfo GetProperty(string typeName, string propertyName)
        {
            _logger.LogDebug("Getting property {PropertyName} from type by name: {TypeName}", propertyName, typeName);
            var type = GetType(typeName);
            return GetProperty(type, propertyName);
        }

        public object GetPropertyValue(object instance, string propertyName)
        {
            _logger.LogDebug("Getting property value {PropertyName} from instance of type: {TypeName}", propertyName, instance.GetType().Name);
            var property = instance.GetType().GetProperty(propertyName);
            if (property == null)
            {
                throw new MissingMemberException(instance.GetType().Name, propertyName);
            }
            return property.GetValue(instance);
        }

        public void SetPropertyValue(object instance, string propertyName, object value)
        {
            _logger.LogDebug("Setting property value {PropertyName} on instance of type: {TypeName}", propertyName, instance.GetType().Name);
            var property = instance.GetType().GetProperty(propertyName);
            if (property == null)
            {
                throw new MissingMemberException(instance.GetType().Name, propertyName);
            }
            property.SetValue(instance, value);
        }

        public FieldInfo GetField(Type type, string fieldName)
        {
            _logger.LogDebug("Getting field {FieldName} from type: {TypeName}", fieldName, type.Name);
            return type.GetField(fieldName);
        }

        public FieldInfo GetField(string typeName, string fieldName)
        {
            _logger.LogDebug("Getting field {FieldName} from type by name: {TypeName}", fieldName, typeName);
            var type = GetType(typeName);
            return GetField(type, fieldName);
        }

        public object GetFieldValue(object instance, string fieldName)
        {
            _logger.LogDebug("Getting field value {FieldName} from instance of type: {TypeName}", fieldName, instance.GetType().Name);
            var field = instance.GetType().GetField(fieldName);
            if (field == null)
            {
                throw new MissingMemberException(instance.GetType().Name, fieldName);
            }
            return field.GetValue(instance);
        }

        public void SetFieldValue(object instance, string fieldName, object value)
        {
            _logger.LogDebug("Setting field value {FieldName} on instance of type: {TypeName}", fieldName, instance.GetType().Name);
            var field = instance.GetType().GetField(fieldName);
            if (field == null)
            {
                throw new MissingMemberException(instance.GetType().Name, fieldName);
            }
            field.SetValue(instance, value);
        }

        public IEnumerable<MethodInfo> GetMethods(Type type)
        {
            _logger.LogDebug("Getting all methods from type: {TypeName}", type.Name);
            return type.GetMethods();
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            _logger.LogDebug("Getting all properties from type: {TypeName}", type.Name);
            return type.GetProperties();
        }

        public IEnumerable<FieldInfo> GetFields(Type type)
        {
            _logger.LogDebug("Getting all fields from type: {TypeName}", type.Name);
            return type.GetFields();
        }

        public IEnumerable<Attribute> GetAttributes(Type type)
        {
            _logger.LogDebug("Getting all attributes from type: {TypeName}", type.Name);
            return type.GetCustomAttributes();
        }

        public bool HasAttribute(Type type, Type attributeType)
        {
            _logger.LogDebug("Checking if type {TypeName} has attribute {AttributeType}", type.Name, attributeType.Name);
            return type.GetCustomAttributes(attributeType, true).Length > 0;
        }

        public T GetAttribute<T>(Type type) where T : Attribute
        {
            _logger.LogDebug("Getting attribute {AttributeType} from type: {TypeName}", typeof(T).Name, type.Name);
            return (T)type.GetCustomAttribute(typeof(T), true);
        }
    }

    // ===============================
    // 时间工具
    // ===============================

    public interface ITimeUtil
    {
        string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss");
        string FormatDate(DateTime dateTime, string format = "yyyy-MM-dd");
        string FormatTime(DateTime dateTime, string format = "HH:mm:ss");
        DateTime ParseDateTime(string dateTimeString, string format = "yyyy-MM-dd HH:mm:ss");
        DateTime ParseDate(string dateString, string format = "yyyy-MM-dd");
        DateTime ParseTime(string timeString, string format = "HH:mm:ss");
        long ToTimestamp(DateTime dateTime);
        long ToUnixTimestamp(DateTime dateTime);
        DateTime FromTimestamp(long timestamp);
        DateTime FromUnixTimestamp(long timestamp);
        TimeSpan GetTimeSpan(DateTime startDateTime, DateTime endDateTime);
        DateTime AddDays(DateTime dateTime, double days);
        DateTime AddHours(DateTime dateTime, double hours);
        DateTime AddMinutes(DateTime dateTime, double minutes);
        DateTime AddSeconds(DateTime dateTime, double seconds);
        DateTime AddMilliseconds(DateTime dateTime, double milliseconds);
        DateTime ConvertTimeZone(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone);
        DateTime ConvertToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone);
        DateTime ConvertFromUtc(DateTime utcDateTime, TimeZoneInfo destinationTimeZone);
    }

    public class DefaultTimeUtil : ITimeUtil
    {
        private readonly ILogger<DefaultTimeUtil> _logger;

        public DefaultTimeUtil(ILogger<DefaultTimeUtil> logger)
        {
            _logger = logger;
        }

        public string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            _logger.LogDebug("Formatting DateTime with format: {Format}", format);
            return dateTime.ToString(format);
        }

        public string FormatDate(DateTime dateTime, string format = "yyyy-MM-dd")
        {
            _logger.LogDebug("Formatting Date with format: {Format}", format);
            return dateTime.ToString(format);
        }

        public string FormatTime(DateTime dateTime, string format = "HH:mm:ss")
        {
            _logger.LogDebug("Formatting Time with format: {Format}", format);
            return dateTime.ToString(format);
        }

        public DateTime ParseDateTime(string dateTimeString, string format = "yyyy-MM-dd HH:mm:ss")
        {
            _logger.LogDebug("Parsing DateTime string with format: {Format}", format);
            return DateTime.ParseExact(dateTimeString, format, null);
        }

        public DateTime ParseDate(string dateString, string format = "yyyy-MM-dd")
        {
            _logger.LogDebug("Parsing Date string with format: {Format}", format);
            return DateTime.ParseExact(dateString, format, null);
        }

        public DateTime ParseTime(string timeString, string format = "HH:mm:ss")
        {
            _logger.LogDebug("Parsing Time string with format: {Format}", format);
            return DateTime.ParseExact(timeString, format, null);
        }

        public long ToTimestamp(DateTime dateTime)
        {
            _logger.LogDebug("Converting DateTime to timestamp");
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public long ToUnixTimestamp(DateTime dateTime)
        {
            _logger.LogDebug("Converting DateTime to Unix timestamp");
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public DateTime FromTimestamp(long timestamp)
        {
            _logger.LogDebug("Converting timestamp to DateTime");
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
        }

        public DateTime FromUnixTimestamp(long timestamp)
        {
            _logger.LogDebug("Converting Unix timestamp to DateTime");
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
        }

        public TimeSpan GetTimeSpan(DateTime startDateTime, DateTime endDateTime)
        {
            _logger.LogDebug("Calculating time span between two DateTime values");
            return endDateTime - startDateTime;
        }

        public DateTime AddDays(DateTime dateTime, double days)
        {
            _logger.LogDebug("Adding {Days} days to DateTime", days);
            return dateTime.AddDays(days);
        }

        public DateTime AddHours(DateTime dateTime, double hours)
        {
            _logger.LogDebug("Adding {Hours} hours to DateTime", hours);
            return dateTime.AddHours(hours);
        }

        public DateTime AddMinutes(DateTime dateTime, double minutes)
        {
            _logger.LogDebug("Adding {Minutes} minutes to DateTime", minutes);
            return dateTime.AddMinutes(minutes);
        }

        public DateTime AddSeconds(DateTime dateTime, double seconds)
        {
            _logger.LogDebug("Adding {Seconds} seconds to DateTime", seconds);
            return dateTime.AddSeconds(seconds);
        }

        public DateTime AddMilliseconds(DateTime dateTime, double milliseconds)
        {
            _logger.LogDebug("Adding {Milliseconds} milliseconds to DateTime", milliseconds);
            return dateTime.AddMilliseconds(milliseconds);
        }

        public DateTime ConvertTimeZone(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            _logger.LogDebug("Converting DateTime from {SourceTimeZone} to {DestinationTimeZone}", 
                sourceTimeZone.Id, destinationTimeZone.Id);
            return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone);
        }

        public DateTime ConvertToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone)
        {
            _logger.LogDebug("Converting DateTime to UTC from {SourceTimeZone}", sourceTimeZone.Id);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
        }

        public DateTime ConvertFromUtc(DateTime utcDateTime, TimeZoneInfo destinationTimeZone)
        {
            _logger.LogDebug("Converting DateTime from UTC to {DestinationTimeZone}", destinationTimeZone.Id);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, destinationTimeZone);
        }
    }

    // ===============================
    // 网络工具
    // ===============================

    public interface INetworkUtil
    {
        Task<string> HttpGetAsync(string url, CancellationToken cancellationToken = default);
        Task<string> HttpPostAsync(string url, string content, string contentType = "application/json", CancellationToken cancellationToken = default);
        Task<string> HttpPutAsync(string url, string content, string contentType = "application/json", CancellationToken cancellationToken = default);
        Task<string> HttpDeleteAsync(string url, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadFileAsync(string url, CancellationToken cancellationToken = default);
        Task UploadFileAsync(string url, string filePath, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);
        bool IsValidUrl(string url);
        bool IsValidEmail(string email);
        bool IsValidIpAddress(string ipAddress);
        string GetHostName();
        string GetIpAddress();
        bool PingHost(string hostNameOrAddress);
    }

    public class DefaultNetworkUtil : INetworkUtil
    {
        private readonly ILogger<DefaultNetworkUtil> _logger;
        private readonly HttpClient _httpClient;

        public DefaultNetworkUtil(ILogger<DefaultNetworkUtil> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<string> HttpGetAsync(string url, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Sending HTTP GET request to: {Url}", url);
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("HTTP GET request completed, response length: {Length}", content.Length);
            return content;
        }

        public async Task<string> HttpPostAsync(string url, string content, string contentType = "application/json", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Sending HTTP POST request to: {Url}", url);
            var httpContent = new StringContent(content, Encoding.UTF8, contentType);
            var response = await _httpClient.PostAsync(url, httpContent, cancellationToken);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("HTTP POST request completed, response length: {Length}", responseContent.Length);
            return responseContent;
        }

        public async Task<string> HttpPutAsync(string url, string content, string contentType = "application/json", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Sending HTTP PUT request to: {Url}", url);
            var httpContent = new StringContent(content, Encoding.UTF8, contentType);
            var response = await _httpClient.PutAsync(url, httpContent, cancellationToken);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("HTTP PUT request completed, response length: {Length}", responseContent.Length);
            return responseContent;
        }

        public async Task<string> HttpDeleteAsync(string url, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Sending HTTP DELETE request to: {Url}", url);
            var response = await _httpClient.DeleteAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("HTTP DELETE request completed");
            return content;
        }

        public async Task<byte[]> DownloadFileAsync(string url, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Downloading file from: {Url}", url);
            var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken);
            _logger.LogDebug("File download completed, size: {Size} bytes", bytes.Length);
            return bytes;
        }

        public async Task UploadFileAsync(string url, string filePath, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogDebug("Uploading file: {FilePath} to: {Url}", filePath, url);
            
            using var fileStream = File.OpenRead(filePath);
            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogDebug("File upload completed");
        }

        public bool IsValidUrl(string url)
        {
            _logger.LogDebug("Validating URL: {Url}", url);
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public bool IsValidEmail(string email)
        {
            _logger.LogDebug("Validating email: {Email}", email);
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidIpAddress(string ipAddress)
        {
            _logger.LogDebug("Validating IP address: {IpAddress}", ipAddress);
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }

        public string GetHostName()
        {
            _logger.LogDebug("Getting host name");
            return System.Net.Dns.GetHostName();
        }

        public string GetIpAddress()
        {
            _logger.LogDebug("Getting IP address");
            var hostName = System.Net.Dns.GetHostName();
            var ipAddresses = System.Net.Dns.GetHostAddresses(hostName);
            foreach (var ip in ipAddresses)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        public bool PingHost(string hostNameOrAddress)
        {
            _logger.LogDebug("Pinging host: {Host}", hostNameOrAddress);
            using var ping = new System.Net.NetworkInformation.Ping();
            try
            {
                var reply = ping.Send(hostNameOrAddress, 1000);
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }

    // ===============================
    // 设置类
    // ===============================

    public class UtilSettings
    {
        public bool EnableCache { get; set; } = true;
        public int CacheSize { get; set; } = 1000;
        public int DefaultTimeout { get; set; } = 30;
        public bool EnableDetailedLogging { get; set; } = false;
        public int MaxRetryAttempts { get; set; } = 3;
        public bool EnableCompression { get; set; } = true;
    }

    public class FileUtilSettings
    {
        public int BufferSize { get; set; } = 8192;
        public long MaxFileSize { get; set; } = 104857600; // 100MB
        public bool EnableFileWatch { get; set; } = false;
        public int WatchInterval { get; set; } = 5000;
    }

    public class CryptoUtilSettings
    {
        public int AesKeySize { get; set; } = 256;
        public int RsaKeySize { get; set; } = 2048;
        public string HashAlgorithm { get; set; } = "SHA256";
        public string KeyStoragePath { get; set; } = "keys/";
        public bool EnableKeyRotation { get; set; } = false;
    }

    public class SerializationUtilSettings
    {
        public bool IndentJson { get; set; } = true;
        public bool IgnoreNullValues { get; set; } = true;
        public string PropertyNamingPolicy { get; set; } = "CamelCase";
    }

    // ===============================
    // 服务扩展
    // ===============================

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUtilServices(this IServiceCollection services)
        {
            // 注册设置
            services.Configure<UtilSettings>(options => { });
            services.Configure<FileUtilSettings>(options => { });
            services.Configure<CryptoUtilSettings>(options => { });
            services.Configure<SerializationUtilSettings>(options => { });

            // 注册服务
            services.AddSingleton<IFileUtil, DefaultFileUtil>();
            services.AddSingleton<ICryptoUtil, DefaultCryptoUtil>();
            services.AddSingleton<ISerializationUtil, DefaultSerializationUtil>();
            services.AddSingleton<IConfigUtil, DefaultConfigUtil>();
            services.AddSingleton<IReflectionUtil, DefaultReflectionUtil>();
            services.AddSingleton<ITimeUtil, DefaultTimeUtil>();
            services.AddSingleton<INetworkUtil, DefaultNetworkUtil>();
            services.AddSingleton<IUtilService, DefaultUtilService>();

            // 注册配置
            services.AddSingleton<IConfigurationRoot>(provider =>
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                    .AddEnvironmentVariables();
                return builder.Build();
            });

            return services;
        }

        public static IServiceCollection AddUtilServicesWithScrutor(this IServiceCollection services)
        {
            // 注册设置
            services.Configure<UtilSettings>(options => { });
            services.Configure<FileUtilSettings>(options => { });
            services.Configure<CryptoUtilSettings>(options => { });
            services.Configure<SerializationUtilSettings>(options => { });

            // 使用Scrutor自动注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<IUtilService>()
                .AddClasses(classes => classes.AssignableTo<IUtilService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<IFileUtil>()
                .AddClasses(classes => classes.AssignableTo<IFileUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<ICryptoUtil>()
                .AddClasses(classes => classes.AssignableTo<ICryptoUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<ISerializationUtil>()
                .AddClasses(classes => classes.AssignableTo<ISerializationUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<IConfigUtil>()
                .AddClasses(classes => classes.AssignableTo<IConfigUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<IReflectionUtil>()
                .AddClasses(classes => classes.AssignableTo<IReflectionUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<ITimeUtil>()
                .AddClasses(classes => classes.AssignableTo<ITimeUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<INetworkUtil>()
                .AddClasses(classes => classes.AssignableTo<INetworkUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            // 注册配置
            services.AddSingleton<IConfigurationRoot>(provider =>
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                    .AddEnvironmentVariables();
                return builder.Build();
            });

            return services;
        }
    }

    // ===============================
    // 装饰器
    // ===============================

    namespace Decorators
    {
        public class CachedFileUtil : IFileUtil
        {
            private readonly IFileUtil _inner;
            private readonly Dictionary<string, string> _textCache;
            private readonly Dictionary<string, byte[]> _byteCache;
            private readonly object _cacheLock = new object();

            public CachedFileUtil(IFileUtil inner)
            {
                _inner = inner;
                _textCache = new Dictionary<string, string>();
                _byteCache = new Dictionary<string, byte[]>();
            }

            public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
            {
                return _inner.ExistsAsync(path, cancellationToken);
            }

            public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
            {
                // 检查缓存
                if (_textCache.TryGetValue(path, out var cachedContent))
                {
                    Console.WriteLine($"[缓存命中] 读取文件: {path}");
                    return cachedContent;
                }

                // 从文件读取
                Console.WriteLine($"[从文件读取] 读取文件: {path}");
                var content = await _inner.ReadAllTextAsync(path, cancellationToken);

                // 更新缓存
                lock (_cacheLock)
                {
                    _textCache[path] = content;
                }

                return content;
            }

            public async Task WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
            {
                // 写入文件
                await _inner.WriteAllTextAsync(path, content, cancellationToken);

                // 更新缓存
                lock (_cacheLock)
                {
                    _textCache[path] = content;
                }
            }

            public async Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
            {
                // 检查缓存
                if (_byteCache.TryGetValue(path, out var cachedBytes))
                {
                    Console.WriteLine($"[缓存命中] 读取文件字节: {path}");
                    return cachedBytes;
                }

                // 从文件读取
                Console.WriteLine($"[从文件读取] 读取文件字节: {path}");
                var bytes = await _inner.ReadAllBytesAsync(path, cancellationToken);

                // 更新缓存
                lock (_cacheLock)
                {
                    _byteCache[path] = bytes;
                }

                return bytes;
            }

            public async Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
            {
                // 写入文件
                await _inner.WriteAllBytesAsync(path, bytes, cancellationToken);

                // 更新缓存
                lock (_cacheLock)
                {
                    _byteCache[path] = bytes;
                }
            }

            public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
            {
                // 删除文件
                await _inner.DeleteAsync(path, cancellationToken);

                // 从缓存中移除
                lock (_cacheLock)
                {
                    _textCache.Remove(path);
                    _byteCache.Remove(path);
                }
            }

            public async Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
            {
                // 复制文件
                await _inner.CopyAsync(sourcePath, destinationPath, cancellationToken);

                // 更新缓存
                if (_textCache.TryGetValue(sourcePath, out var textContent))
                {
                    lock (_cacheLock)
                    {
                        _textCache[destinationPath] = textContent;
                    }
                }
                else if (_byteCache.TryGetValue(sourcePath, out var byteContent))
                {
                    lock (_cacheLock)
                    {
                        _byteCache[destinationPath] = byteContent;
                    }
                }
            }

            public async Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken = default)
            {
                // 移动文件
                await _inner.MoveAsync(sourcePath, destinationPath, cancellationToken);

                // 更新缓存
                lock (_cacheLock)
                {
                    if (_textCache.TryGetValue(sourcePath, out var textContent))
                    {
                        _textCache[destinationPath] = textContent;
                        _textCache.Remove(sourcePath);
                    }
                    if (_byteCache.TryGetValue(sourcePath, out var byteContent))
                    {
                        _byteCache[destinationPath] = byteContent;
                        _byteCache.Remove(sourcePath);
                    }
                }
            }

            public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
            {
                return _inner.DirectoryExistsAsync(path, cancellationToken);
            }

            public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
            {
                return _inner.CreateDirectoryAsync(path, cancellationToken);
            }

            public async Task DeleteDirectoryAsync(string path, CancellationToken cancellationToken = default)
            {
                // 获取目录中的文件
                var files = await _inner.EnumerateFilesAsync(path, cancellationToken);

                // 删除目录
                await _inner.DeleteDirectoryAsync(path, cancellationToken);

                // 从缓存中移除相关文件
                lock (_cacheLock)
                {
                    foreach (var file in files)
                    {
                        _textCache.Remove(file);
                        _byteCache.Remove(file);
                    }
                }
            }

            public Task<IEnumerable<string>> EnumerateFilesAsync(string path, CancellationToken cancellationToken = default)
            {
                return _inner.EnumerateFilesAsync(path, cancellationToken);
            }

            public Task<FileInfo> GetFileInfoAsync(string path, CancellationToken cancellationToken = default)
            {
                return _inner.GetFileInfoAsync(path, cancellationToken);
            }
        }

        public class LoggingCryptoUtil : ICryptoUtil
        {
            private readonly ICryptoUtil _inner;

            public LoggingCryptoUtil(ICryptoUtil inner)
            {
                _inner = inner;
            }

            public async Task<string> EncryptAsync(string plainText, string key = null, CancellationToken cancellationToken = default)
            {
                Console.WriteLine($"[加密操作] 加密文本长度: {plainText.Length}");
                var result = await _inner.EncryptAsync(plainText, key, cancellationToken);
                Console.WriteLine($"[加密操作] 加密结果长度: {result.Length}");
                return result;
            }

            public async Task<string> DecryptAsync(string encryptedText, string key = null, CancellationToken cancellationToken = default)
            {
                Console.WriteLine($"[解密操作] 解密文本长度: {encryptedText.Length}");
                var result = await _inner.DecryptAsync(encryptedText, key, cancellationToken);
                Console.WriteLine($"[解密操作] 解密结果长度: {result.Length}");
                return result;
            }

            public async Task<string> ComputeHashAsync(string input, string algorithm = "SHA256", CancellationToken cancellationToken = default)
            {
                Console.WriteLine($"[哈希操作] 计算哈希，输入长度: {input.Length}, 算法: {algorithm}");
                var result = await _inner.ComputeHashAsync(input, algorithm, cancellationToken);
                Console.WriteLine($"[哈希操作] 哈希计算完成");
                return result;
            }

            public async Task<bool> VerifyHashAsync(string input, string hash, string algorithm = "SHA256", CancellationToken cancellationToken = default)
            {
                Console.WriteLine($"[哈希操作] 验证哈希，输入长度: {input.Length}");
                var result = await _inner.VerifyHashAsync(input, hash, algorithm, cancellationToken);
                Console.WriteLine($"[哈希操作] 哈希验证结果: {result}");
                return result;
            }

            public async Task<string> ComputePasswordHashAsync(string password, CancellationToken cancellationToken = default)
            {
                Console.WriteLine("[密码操作] 计算密码哈希");
                var result = await _inner.ComputePasswordHashAsync(password, cancellationToken);
                Console.WriteLine("[密码操作] 密码哈希计算完成");
                return result;
            }

            public async Task<bool> VerifyPasswordHashAsync(string password, string hash, CancellationToken cancellationToken = default)
            {
                Console.WriteLine("[密码操作] 验证密码哈希");
                var result = await _inner.VerifyPasswordHashAsync(password, hash, cancellationToken);
                Console.WriteLine($"[密码操作] 密码哈希验证结果: {result}");
                return result;
            }

            public async Task<string> GenerateRandomStringAsync(int length, CancellationToken cancellationToken = default)
            {
                Console.WriteLine($"[随机操作] 生成随机字符串，长度: {length}");
                var result = await _inner.GenerateRandomStringAsync(length, cancellationToken);
                Console.WriteLine("[随机操作] 随机字符串生成完成");
                return result;
            }

            public async Task<int> GenerateRandomNumberAsync(int min, int max, CancellationToken cancellationToken = default)
            {
                Console.WriteLine($"[随机操作] 生成随机数字，范围: {min}-{max}");
                var result = await _inner.GenerateRandomNumberAsync(min, max, cancellationToken);
                Console.WriteLine($"[随机操作] 随机数字生成完成: {result}");
                return result;
            }
        }

        public class CompressedSerializationUtil : ISerializationUtil
        {
            private readonly ISerializationUtil _inner;

            public CompressedSerializationUtil(ISerializationUtil inner)
            {
                _inner = inner;
            }

            public Task<string> SerializeToJsonAsync(object value, CancellationToken cancellationToken = default)
            {
                return _inner.SerializeToJsonAsync(value, cancellationToken);
            }

            public Task<T> DeserializeFromJsonAsync<T>(string json, CancellationToken cancellationToken = default)
            {
                return _inner.DeserializeFromJsonAsync<T>(json, cancellationToken);
            }

            public Task<string> SerializeToXmlAsync(object value, CancellationToken cancellationToken = default)
            {
                return _inner.SerializeToXmlAsync(value, cancellationToken);
            }

            public Task<T> DeserializeFromXmlAsync<T>(string xml, CancellationToken cancellationToken = default)
            {
                return _inner.DeserializeFromXmlAsync<T>(xml, cancellationToken);
            }

            public async Task<byte[]> SerializeToBinaryAsync(object value, CancellationToken cancellationToken = default)
            {
                var bytes = await _inner.SerializeToBinaryAsync(value, cancellationToken);
                // 这里可以添加压缩逻辑
                return bytes;
            }

            public async Task<T> DeserializeFromBinaryAsync<T>(byte[] bytes, CancellationToken cancellationToken = default)
            {
                // 这里可以添加解压缩逻辑
                return await _inner.DeserializeFromBinaryAsync<T>(bytes, cancellationToken);
            }
        }
    }

    // ===============================
    // 主程序
    // ===============================

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Util Core 示例程序");
            Console.WriteLine("=" * 50);

            // 初始化服务容器
            var serviceProvider = BuildServiceProvider();

            // 测试基本功能
            await TestBasicFunctionality(serviceProvider);

            // 测试Scrutor集成
            await TestScrutorIntegration();

            Console.WriteLine("\nUtil Core 示例程序完成！");
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var builder = new ServiceCollection();
            builder.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });
            builder.AddUtilServices();
            return builder.BuildServiceProvider();
        }

        private static async Task TestBasicFunctionality(ServiceProvider serviceProvider)
        {
            Console.WriteLine("\n1. 测试基本功能:");

            // 获取Util服务
            var utilService = serviceProvider.GetRequiredService<IUtilService>();

            // 测试文件工具
            Console.WriteLine("\n1.1 测试文件工具:");
            var testFile = "test.txt";
            await utilService.FileUtil.WriteAllTextAsync(testFile, "Hello, Util!");
            var content = await utilService.FileUtil.ReadAllTextAsync(testFile);
            Console.WriteLine($"文件内容: {content}");
            var exists = await utilService.FileUtil.ExistsAsync(testFile);
            Console.WriteLine($"文件是否存在: {exists}");

            // 测试加密工具
            Console.WriteLine("\n1.2 测试加密工具:");
            var originalText = "敏感信息";
            var encrypted = await utilService.CryptoUtil.EncryptAsync(originalText);
            var decrypted = await utilService.CryptoUtil.DecryptAsync(encrypted);
            Console.WriteLine($"原始文本: {originalText}");
            Console.WriteLine($"加密结果: {encrypted}");
            Console.WriteLine($"解密结果: {decrypted}");
            Console.WriteLine($"加密/解密成功: {originalText == decrypted}");

            // 测试序列化工具
            Console.WriteLine("\n1.3 测试序列化工具:");
            var user = new User { Id = 1, Name = "张三", Email = "zhangsan@example.com" };
            var json = await utilService.SerializationUtil.SerializeToJsonAsync(user);
            var deserializedUser = await utilService.SerializationUtil.DeserializeFromJsonAsync<User>(json);
            Console.WriteLine($"序列化结果: {json}");
            Console.WriteLine($"反序列化结果: Id={deserializedUser.Id}, Name={deserializedUser.Name}, Email={deserializedUser.Email}");

            // 测试时间工具
            Console.WriteLine("\n1.4 测试时间工具:");
            var now = DateTime.Now;
            var formattedDateTime = utilService.TimeUtil.FormatDateTime(now);
            var timestamp = utilService.TimeUtil.ToTimestamp(now);
            var dateFromTimestamp = utilService.TimeUtil.FromTimestamp(timestamp);
            Console.WriteLine($"当前时间: {formattedDateTime}");
            Console.WriteLine($"时间戳: {timestamp}");
            Console.WriteLine($"从时间戳转换: {utilService.TimeUtil.FormatDateTime(dateFromTimestamp)}");

            // 测试网络工具
            Console.WriteLine("\n1.5 测试网络工具:");
            var hostName = utilService.NetworkUtil.GetHostName();
            var ipAddress = utilService.NetworkUtil.GetIpAddress();
            Console.WriteLine($"主机名: {hostName}");
            Console.WriteLine($"IP地址: {ipAddress}");

            // 清理测试文件
            if (await utilService.FileUtil.ExistsAsync("test.txt"))
            {
                await utilService.FileUtil.DeleteAsync("test.txt");
            }
        }

        private static async Task TestScrutorIntegration()
        {
            Console.WriteLine("\n2. 测试Scrutor集成:");

            // 初始化服务容器（使用Scrutor）
            var builder = new ServiceCollection();
            builder.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });
            
            // 使用Scrutor自动注册服务
            builder.Scan(scan => scan
                .FromAssemblyOf<IUtilService>()
                .AddClasses(classes => classes.AssignableTo<IUtilService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<IFileUtil>()
                .AddClasses(classes => classes.AssignableTo<IFileUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<ICryptoUtil>()
                .AddClasses(classes => classes.AssignableTo<ICryptoUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<ISerializationUtil>()
                .AddClasses(classes => classes.AssignableTo<ISerializationUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<IConfigUtil>()
                .AddClasses(classes => classes.AssignableTo<IConfigUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<IReflectionUtil>()
                .AddClasses(classes => classes.AssignableTo<IReflectionUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<ITimeUtil>()
                .AddClasses(classes => classes.AssignableTo<ITimeUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                .FromAssemblyOf<INetworkUtil>()
                .AddClasses(classes => classes.AssignableTo<INetworkUtil>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            // 注册装饰器
            builder.Decorate<IFileUtil, Decorators.CachedFileUtil>();
            builder.Decorate<ICryptoUtil, Decorators.LoggingCryptoUtil>();

            // 注册配置
            builder.AddSingleton<IConfigurationRoot>(provider =>
            {
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables();
                return configBuilder.Build();
            });

            var serviceProvider = builder.BuildServiceProvider();

            // 测试Scrutor注册的服务
            var utilService = serviceProvider.GetRequiredService<IUtilService>();
            Console.WriteLine($"\nScrutor注册测试:");
            Console.WriteLine($"Util服务注册成功: {utilService != null}");
            Console.WriteLine($"文件工具注册成功: {utilService.FileUtil != null}");
            Console.WriteLine($"加密工具注册成功: {utilService.CryptoUtil != null}");

            // 测试装饰器
            Console.WriteLine("\n装饰器测试:");

            // 测试文件工具装饰器
            var testFile = "scrutor_test.txt";
            await utilService.FileUtil.WriteAllTextAsync(testFile, "测试文件内容");
            
            // 第一次读取（应该从文件读取）
            var content1 = await utilService.FileUtil.ReadAllTextAsync(testFile);
            Console.WriteLine($"第一次读取内容: {content1}");
            
            // 第二次读取（应该从缓存读取）
            var content2 = await utilService.FileUtil.ReadAllTextAsync(testFile);
            Console.WriteLine($"第二次读取内容: {content2}");

            // 测试加密工具装饰器
            var testText = "测试装饰器加密";
            var encrypted = await utilService.CryptoUtil.EncryptAsync(testText);
            var decrypted = await utilService.CryptoUtil.DecryptAsync(encrypted);
            Console.WriteLine($"加密测试结果: {testText == decrypted}");

            // 清理测试文件
            if (await utilService.FileUtil.ExistsAsync(testFile))
            {
                await utilService.FileUtil.DeleteAsync(testFile);
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
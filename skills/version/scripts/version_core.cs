#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Scrutor@4.2.2
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VersionSkill
{
    /// <summary>
    /// 版本格式异常类
    /// </summary>
    public class VersionFormatException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="VersionFormatException"/> 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        public VersionFormatException(string message) : base(message)
        {}

        /// <summary>
        /// 初始化 <see cref="VersionFormatException"/> 类的新实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="innerException">内部异常</param>
        public VersionFormatException(string message, Exception innerException) : base(message, innerException)
        {}
    }

    /// <summary>
    /// 版本选项类
    /// </summary>
    public class VersionOptions
    {
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool CacheEnabled { get; set; } = true;

        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 1000;

        /// <summary>
        /// 是否使用严格模式解析版本号
        /// </summary>
        public bool StrictMode { get; set; } = false;
    }

    /// <summary>
    /// 语义化版本类
    /// </summary>
    public class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        /// <summary>
        /// 版本号正则表达式
        /// </summary>
        private static readonly Regex VersionRegex = new Regex(
            @"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<preRelease>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?(?:\+(?<buildMetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
            RegexOptions.Compiled);

        /// <summary>
        /// 主版本号
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// 次版本号
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// 补丁版本号
        /// </summary>
        public int Patch { get; }

        /// <summary>
        /// 预发布版本号
        /// </summary>
        public string PreRelease { get; }

        /// <summary>
        /// 构建元数据
        /// </summary>
        public string BuildMetadata { get; }

        /// <summary>
        /// 初始化 <see cref="SemanticVersion"/> 类的新实例
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="patch">补丁版本号</param>
        /// <param name="preRelease">预发布版本号</param>
        /// <param name="buildMetadata">构建元数据</param>
        public SemanticVersion(int major, int minor, int patch, string preRelease = null, string buildMetadata = null)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major), "主版本号不能为负数");
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor), "次版本号不能为负数");
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch), "补丁版本号不能为负数");

            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease;
            BuildMetadata = buildMetadata;
        }

        /// <summary>
        /// 解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>语义化版本对象</returns>
        /// <exception cref="VersionFormatException">版本格式无效时抛出</exception>
        public static SemanticVersion Parse(string versionString)
        {
            if (string.IsNullOrWhiteSpace(versionString))
            {
                throw new VersionFormatException("版本字符串不能为空");
            }

            var match = VersionRegex.Match(versionString);
            if (!match.Success)
            {
                throw new VersionFormatException($"版本字符串 '{versionString}' 格式无效");
            }

            int major = int.Parse(match.Groups["major"].Value);
            int minor = int.Parse(match.Groups["minor"].Value);
            int patch = int.Parse(match.Groups["patch"].Value);
            string preRelease = match.Groups["preRelease"].Success ? match.Groups["preRelease"].Value : null;
            string buildMetadata = match.Groups["buildMetadata"].Success ? match.Groups["buildMetadata"].Value : null;

            return new SemanticVersion(major, minor, patch, preRelease, buildMetadata);
        }

        /// <summary>
        /// 尝试解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">语义化版本对象</param>
        /// <returns>解析是否成功</returns>
        public static bool TryParse(string versionString, out SemanticVersion version)
        {
            try
            {
                version = Parse(versionString);
                return true;
            }
            catch
            {
                version = null;
                return false;
            }
        }

        /// <summary>
        /// 检查版本字符串是否有效
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>版本字符串是否有效</returns>
        public static bool IsValid(string versionString)
        {
            return TryParse(versionString, out _);
        }

        /// <summary>
        /// 递增主版本号
        /// </summary>
        /// <returns>新的语义化版本对象</returns>
        public SemanticVersion IncrementMajor()
        {
            return new SemanticVersion(Major + 1, 0, 0, null, null);
        }

        /// <summary>
        /// 递增次版本号
        /// </summary>
        /// <returns>新的语义化版本对象</returns>
        public SemanticVersion IncrementMinor()
        {
            return new SemanticVersion(Major, Minor + 1, 0, null, null);
        }

        /// <summary>
        /// 递增补丁版本号
        /// </summary>
        /// <returns>新的语义化版本对象</returns>
        public SemanticVersion IncrementPatch()
        {
            return new SemanticVersion(Major, Minor, Patch + 1, null, null);
        }

        /// <summary>
        /// 转换为核心版本字符串（主版本.次版本.补丁版本）
        /// </summary>
        /// <returns>核心版本字符串</returns>
        public string ToCoreString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        /// <summary>
        /// 转换为版本字符串（包含预发布版本）
        /// </summary>
        /// <returns>版本字符串</returns>
        public string ToVersionString()
        {
            if (!string.IsNullOrEmpty(PreRelease))
            {
                return $"{ToCoreString()}-{PreRelease}";
            }
            return ToCoreString();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>版本字符串</returns>
        public override string ToString()
        {
            var versionStr = ToVersionString();
            if (!string.IsNullOrEmpty(BuildMetadata))
            {
                return $"{versionStr}+{BuildMetadata}";
            }
            return versionStr;
        }

        /// <summary>
        /// 比较两个版本对象
        /// </summary>
        /// <param name="other">另一个版本对象</param>
        /// <returns>比较结果</returns>
        public int CompareTo(SemanticVersion other)
        {
            if (other == null)
            {
                return 1;
            }

            // 比较主版本号
            if (Major != other.Major)
            {
                return Major.CompareTo(other.Major);
            }

            // 比较次版本号
            if (Minor != other.Minor)
            {
                return Minor.CompareTo(other.Minor);
            }

            // 比较补丁版本号
            if (Patch != other.Patch)
            {
                return Patch.CompareTo(other.Patch);
            }

            // 比较预发布版本号
            if (string.IsNullOrEmpty(PreRelease) && string.IsNullOrEmpty(other.PreRelease))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(PreRelease))
            {
                return 1; // 正式版本大于预发布版本
            }
            if (string.IsNullOrEmpty(other.PreRelease))
            {
                return -1; // 预发布版本小于正式版本
            }

            // 比较预发布版本号的各个部分
            var thisParts = PreRelease.Split('.');
            var otherParts = other.PreRelease.Split('.');
            var minLength = Math.Min(thisParts.Length, otherParts.Length);

            for (int i = 0; i < minLength; i++)
            {
                var thisPart = thisParts[i];
                var otherPart = otherParts[i];

                int thisInt, otherInt;
                bool thisIsInt = int.TryParse(thisPart, out thisInt);
                bool otherIsInt = int.TryParse(otherPart, out otherInt);

                if (thisIsInt && otherIsInt)
                {
                    if (thisInt != otherInt)
                    {
                        return thisInt.CompareTo(otherInt);
                    }
                }
                else
                {
                    var result = thisPart.CompareTo(otherPart);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            // 比较预发布版本号的长度
            return thisParts.Length.CompareTo(otherParts.Length);
        }

        /// <summary>
        /// 检查两个版本对象是否相等
        /// </summary>
        /// <param name="other">另一个版本对象</param>
        /// <returns>是否相等</returns>
        public bool Equals(SemanticVersion other)
        {
            if (other == null)
            {
                return false;
            }

            return Major == other.Major &&
                   Minor == other.Minor &&
                   Patch == other.Patch &&
                   PreRelease == other.PreRelease;
            // 构建元数据不参与相等性比较
        }

        /// <summary>
        /// 检查对象是否相等
        /// </summary>
        /// <param name="obj">另一个对象</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SemanticVersion);
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns>哈希码</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Major.GetHashCode();
                hash = hash * 23 + Minor.GetHashCode();
                hash = hash * 23 + Patch.GetHashCode();
                hash = hash * 23 + (PreRelease?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// 相等运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            return left.Equals(right);
        }

        /// <summary>
        /// 不等运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否不等</returns>
        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 小于运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否小于</returns>
        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// 小于等于运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否小于等于</returns>
        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// 大于运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否大于</returns>
        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// 大于等于运算符
        /// </summary>
        /// <param name="left">左操作数</param>
        /// <param name="right">右操作数</param>
        /// <returns>是否大于等于</returns>
        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return left.CompareTo(right) >= 0;
        }
    }

    /// <summary>
    /// 版本范围类
    /// </summary>
    public class VersionRange
    {
        /// <summary>
        /// 版本范围类型
        /// </summary>
        private enum RangeType
        {
            Exact,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            Range
        }

        /// <summary>
        /// 版本范围类型
        /// </summary>
        private RangeType Type { get; }

        /// <summary>
        /// 最小值版本
        /// </summary>
        private SemanticVersion MinVersion { get; }

        /// <summary>
        /// 最大值版本
        /// </summary>
        private SemanticVersion MaxVersion { get; }

        /// <summary>
        /// 是否包含最小值
        /// </summary>
        private bool IncludeMin { get; }

        /// <summary>
        /// 是否包含最大值
        /// </summary>
        private bool IncludeMax { get; }

        /// <summary>
        /// 初始化 <see cref="VersionRange"/> 类的新实例
        /// </summary>
        /// <param name="type">版本范围类型</param>
        /// <param name="minVersion">最小值版本</param>
        /// <param name="maxVersion">最大值版本</param>
        /// <param name="includeMin">是否包含最小值</param>
        /// <param name="includeMax">是否包含最大值</param>
        private VersionRange(RangeType type, SemanticVersion minVersion, SemanticVersion maxVersion = null, bool includeMin = false, bool includeMax = false)
        {
            Type = type;
            MinVersion = minVersion;
            MaxVersion = maxVersion;
            IncludeMin = includeMin;
            IncludeMax = includeMax;
        }

        /// <summary>
        /// 解析版本范围字符串
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <returns>版本范围对象</returns>
        /// <exception cref="VersionFormatException">版本范围格式无效时抛出</exception>
        public static VersionRange Parse(string rangeString)
        {
            if (string.IsNullOrWhiteSpace(rangeString))
            {
                throw new VersionFormatException("版本范围字符串不能为空");
            }

            // 移除空格
            rangeString = rangeString.Trim();

            // 精确版本匹配
            if (!rangeString.Contains(">") && !rangeString.Contains("<") && !rangeString.Contains("~") && !rangeString.Contains("^"))
            {
                var version = SemanticVersion.Parse(rangeString);
                return new VersionRange(RangeType.Exact, version);
            }

            // 波浪号范围 (~1.0.0 相当于 >=1.0.0 <1.1.0)
            if (rangeString.StartsWith("~"))
            {
                var versionString = rangeString.Substring(1);
                var version = SemanticVersion.Parse(versionString);
                var maxVersion = new SemanticVersion(version.Major, version.Minor + 1, 0);
                return new VersionRange(RangeType.Range, version, maxVersion, true, false);
            }

            // 插入符号范围 (^1.0.0 相当于 >=1.0.0 <2.0.0)
            if (rangeString.StartsWith("^"))
            {
                var versionString = rangeString.Substring(1);
                var version = SemanticVersion.Parse(versionString);
                var maxVersion = new SemanticVersion(version.Major + 1, 0, 0);
                return new VersionRange(RangeType.Range, version, maxVersion, true, false);
            }

            // 区间范围 (>=1.0.0 <2.0.0)
            if (rangeString.Contains(" "))
            {
                var parts = rangeString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    throw new VersionFormatException($"版本范围字符串 '{rangeString}' 格式无效");
                }

                var minPart = parts[0];
                var maxPart = parts[1];

                SemanticVersion minVersion = null;
                SemanticVersion maxVersion = null;
                bool includeMin = false;
                bool includeMax = false;

                // 解析最小值部分
                if (minPart.StartsWith(">="))
                {
                    includeMin = true;
                    minVersion = SemanticVersion.Parse(minPart.Substring(2));
                }
                else if (minPart.StartsWith(">"))
                {
                    includeMin = false;
                    minVersion = SemanticVersion.Parse(minPart.Substring(1));
                }
                else
                {
                    throw new VersionFormatException($"版本范围字符串 '{rangeString}' 格式无效");
                }

                // 解析最大值部分
                if (maxPart.StartsWith("<="))
                {
                    includeMax = true;
                    maxVersion = SemanticVersion.Parse(maxPart.Substring(2));
                }
                else if (maxPart.StartsWith("<"))
                {
                    includeMax = false;
                    maxVersion = SemanticVersion.Parse(maxPart.Substring(1));
                }
                else
                {
                    throw new VersionFormatException($"版本范围字符串 '{rangeString}' 格式无效");
                }

                return new VersionRange(RangeType.Range, minVersion, maxVersion, includeMin, includeMax);
            }

            // 单个比较符
            if (rangeString.StartsWith(">="))
            {
                var version = SemanticVersion.Parse(rangeString.Substring(2));
                return new VersionRange(RangeType.GreaterThanOrEqual, version, includeMin: true);
            }
            else if (rangeString.StartsWith(">"))
            {
                var version = SemanticVersion.Parse(rangeString.Substring(1));
                return new VersionRange(RangeType.GreaterThan, version);
            }
            else if (rangeString.StartsWith("<="))
            {
                var version = SemanticVersion.Parse(rangeString.Substring(2));
                return new VersionRange(RangeType.LessThanOrEqual, null, version, includeMax: true);
            }
            else if (rangeString.StartsWith("<"))
            {
                var version = SemanticVersion.Parse(rangeString.Substring(1));
                return new VersionRange(RangeType.LessThan, null, version);
            }

            throw new VersionFormatException($"版本范围字符串 '{rangeString}' 格式无效");
        }

        /// <summary>
        /// 检查版本是否在范围内
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>版本是否在范围内</returns>
        public bool IsInRange(SemanticVersion version)
        {
            if (version == null)
            {
                return false;
            }

            switch (Type)
            {
                case RangeType.Exact:
                    return version == MinVersion;

                case RangeType.GreaterThan:
                    return version > MinVersion;

                case RangeType.GreaterThanOrEqual:
                    return version >= MinVersion;

                case RangeType.LessThan:
                    return version < MaxVersion;

                case RangeType.LessThanOrEqual:
                    return version <= MaxVersion;

                case RangeType.Range:
                    bool minCheck = IncludeMin ? version >= MinVersion : version > MinVersion;
                    bool maxCheck = IncludeMax ? version <= MaxVersion : version < MaxVersion;
                    return minCheck && maxCheck;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>版本范围字符串</returns>
        public override string ToString()
        {
            switch (Type)
            {
                case RangeType.Exact:
                    return MinVersion.ToString();

                case RangeType.GreaterThan:
                    return $">{MinVersion}";

                case RangeType.GreaterThanOrEqual:
                    return $">={MinVersion}";

                case RangeType.LessThan:
                    return $"<{MaxVersion}";

                case RangeType.LessThanOrEqual:
                    return $"<={MaxVersion}";

                case RangeType.Range:
                    string minStr = IncludeMin ? $">={MinVersion}" : $">{MinVersion}";
                    string maxStr = IncludeMax ? $"<={MaxVersion}" : $"<{MaxVersion}";
                    return $"{minStr} {maxStr}";

                default:
                    return string.Empty;
            }
        }
    }

    /// <summary>
    /// 版本缓存类
    /// </summary>
    internal class VersionCache
    {
        /// <summary>
        /// 版本解析缓存
        /// </summary>
        private readonly Dictionary<string, SemanticVersion> _versionCache;

        /// <summary>
        /// 版本范围解析缓存
        /// </summary>
        private readonly Dictionary<string, VersionRange> _rangeCache;

        /// <summary>
        /// 缓存大小
        /// </summary>
        private readonly int _cacheSize;

        /// <summary>
        /// 初始化 <see cref="VersionCache"/> 类的新实例
        /// </summary>
        /// <param name="cacheSize">缓存大小</param>
        public VersionCache(int cacheSize = 1000)
        {
            _cacheSize = Math.Max(1, cacheSize);
            _versionCache = new Dictionary<string, SemanticVersion>(_cacheSize);
            _rangeCache = new Dictionary<string, VersionRange>(_cacheSize);
        }

        /// <summary>
        /// 获取缓存的版本对象
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">版本对象</param>
        /// <returns>是否命中缓存</returns>
        public bool TryGetVersion(string versionString, out SemanticVersion version)
        {
            return _versionCache.TryGetValue(versionString, out version);
        }

        /// <summary>
        /// 设置缓存的版本对象
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">版本对象</param>
        public void SetVersion(string versionString, SemanticVersion version)
        {
            if (_versionCache.Count >= _cacheSize)
            {
                // 移除第一个元素
                var firstKey = _versionCache.Keys.First();
                _versionCache.Remove(firstKey);
            }
            _versionCache[versionString] = version;
        }

        /// <summary>
        /// 获取缓存的版本范围对象
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <param name="range">版本范围对象</param>
        /// <returns>是否命中缓存</returns>
        public bool TryGetRange(string rangeString, out VersionRange range)
        {
            return _rangeCache.TryGetValue(rangeString, out range);
        }

        /// <summary>
        /// 设置缓存的版本范围对象
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <param name="range">版本范围对象</param>
        public void SetRange(string rangeString, VersionRange range)
        {
            if (_rangeCache.Count >= _cacheSize)
            {
                // 移除第一个元素
                var firstKey = _rangeCache.Keys.First();
                _rangeCache.Remove(firstKey);
            }
            _rangeCache[rangeString] = range;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            _versionCache.Clear();
            _rangeCache.Clear();
        }
    }

    /// <summary>
    /// 版本服务接口
    /// </summary>
    public interface IVersionService
    {
        /// <summary>
        /// 解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>语义化版本对象</returns>
        SemanticVersion Parse(string versionString);

        /// <summary>
        /// 尝试解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">语义化版本对象</param>
        /// <returns>解析是否成功</returns>
        bool TryParse(string versionString, out SemanticVersion version);

        /// <summary>
        /// 检查版本字符串是否有效
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>版本字符串是否有效</returns>
        bool IsValidVersion(string versionString);

        /// <summary>
        /// 递增主版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        SemanticVersion IncrementMajor(SemanticVersion version);

        /// <summary>
        /// 递增次版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        SemanticVersion IncrementMinor(SemanticVersion version);

        /// <summary>
        /// 递增补丁版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        SemanticVersion IncrementPatch(SemanticVersion version);

        /// <summary>
        /// 解析版本范围字符串
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <returns>版本范围对象</returns>
        VersionRange ParseRange(string rangeString);

        /// <summary>
        /// 检查版本是否在范围内
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <param name="range">版本范围对象</param>
        /// <returns>版本是否在范围内</returns>
        bool IsInRange(SemanticVersion version, VersionRange range);
    }

    /// <summary>
    /// 版本服务实现
    /// </summary>
    internal class VersionService : IVersionService
    {
        /// <summary>
        /// 版本缓存
        /// </summary>
        private readonly VersionCache _cache;

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        private readonly bool _cacheEnabled;

        /// <summary>
        /// 初始化 <see cref="VersionService"/> 类的新实例
        /// </summary>
        /// <param name="options">版本选项</param>
        public VersionService(IOptions<VersionOptions> options)
        {
            var versionOptions = options?.Value ?? new VersionOptions();
            _cacheEnabled = versionOptions.CacheEnabled;
            _cache = new VersionCache(versionOptions.CacheSize);
        }

        /// <summary>
        /// 解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>语义化版本对象</returns>
        public SemanticVersion Parse(string versionString)
        {
            if (_cacheEnabled && _cache.TryGetVersion(versionString, out var version))
            {
                return version;
            }

            version = SemanticVersion.Parse(versionString);

            if (_cacheEnabled)
            {
                _cache.SetVersion(versionString, version);
            }

            return version;
        }

        /// <summary>
        /// 尝试解析版本字符串
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <param name="version">语义化版本对象</param>
        /// <returns>解析是否成功</returns>
        public bool TryParse(string versionString, out SemanticVersion version)
        {
            if (_cacheEnabled && _cache.TryGetVersion(versionString, out version))
            {
                return true;
            }

            bool success = SemanticVersion.TryParse(versionString, out version);

            if (success && _cacheEnabled)
            {
                _cache.SetVersion(versionString, version);
            }

            return success;
        }

        /// <summary>
        /// 检查版本字符串是否有效
        /// </summary>
        /// <param name="versionString">版本字符串</param>
        /// <returns>版本字符串是否有效</returns>
        public bool IsValidVersion(string versionString)
        {
            return TryParse(versionString, out _);
        }

        /// <summary>
        /// 递增主版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementMajor(SemanticVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return version.IncrementMajor();
        }

        /// <summary>
        /// 递增次版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementMinor(SemanticVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return version.IncrementMinor();
        }

        /// <summary>
        /// 递增补丁版本号
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <returns>新的版本对象</returns>
        public SemanticVersion IncrementPatch(SemanticVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return version.IncrementPatch();
        }

        /// <summary>
        /// 解析版本范围字符串
        /// </summary>
        /// <param name="rangeString">版本范围字符串</param>
        /// <returns>版本范围对象</returns>
        public VersionRange ParseRange(string rangeString)
        {
            if (_cacheEnabled && _cache.TryGetRange(rangeString, out var range))
            {
                return range;
            }

            range = VersionRange.Parse(rangeString);

            if (_cacheEnabled)
            {
                _cache.SetRange(rangeString, range);
            }

            return range;
        }

        /// <summary>
        /// 检查版本是否在范围内
        /// </summary>
        /// <param name="version">版本对象</param>
        /// <param name="range">版本范围对象</param>
        /// <returns>版本是否在范围内</returns>
        public bool IsInRange(SemanticVersion version, VersionRange range)
        {
            if (version == null || range == null)
            {
                return false;
            }

            return range.IsInRange(version);
        }
    }

    /// <summary>
    /// 版本技能扩展方法
    /// </summary>
    public static class VersionSkillExtensions
    {
        /// <summary>
        /// 添加版本技能服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddVersionSkill(this IServiceCollection services, Action<VersionOptions> configureOptions = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // 配置选项
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<VersionOptions>(options => { });
            }

            // 注册服务
            services.AddSingleton<IVersionService, VersionService>();

            return services;
        }
    }

    /// <summary>
    /// 主程序类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args">命令行参数</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Version 技能演示");
            Console.WriteLine("==================");

            // 基本使用示例
            Console.WriteLine("\n1. 基本使用示例:");
            
            // 创建版本对象
            var version1 = new SemanticVersion(1, 2, 3);
            Console.WriteLine($"创建版本对象: {version1}");

            // 解析版本字符串
            var version2 = SemanticVersion.Parse("2.0.0-alpha.1");
            Console.WriteLine($"解析版本字符串: {version2}");

            // 比较版本
            var version3 = new SemanticVersion(1, 0, 0);
            var version4 = new SemanticVersion(1, 1, 0);
            Console.WriteLine($"比较版本 {version3} < {version4}: {version3 < version4}");

            // 递增版本
            var version5 = version3.IncrementPatch();
            Console.WriteLine($"递增补丁版本: {version3} -> {version5}");

            // 版本范围
            var range = VersionRange.Parse(">=1.0.0 <2.0.0");
            Console.WriteLine($"版本范围 {range}");
            Console.WriteLine($"版本 {version1} 在范围内: {range.IsInRange(version1)}");
            Console.WriteLine($"版本 {version2} 在范围内: {range.IsInRange(version2)}");

            // 依赖注入示例
            Console.WriteLine("\n2. 依赖注入示例:");
            
            var services = new ServiceCollection();
            services.AddVersionSkill(options =>
            {
                options.CacheEnabled = true;
                options.CacheSize = 1000;
            });
            
            var serviceProvider = services.BuildServiceProvider();
            var versionService = serviceProvider.GetRequiredService<IVersionService>();

            // 使用服务
            var version6 = versionService.Parse("1.0.0");
            Console.WriteLine($"使用服务解析版本: {version6}");

            var version7 = versionService.IncrementMinor(version6);
            Console.WriteLine($"使用服务递增次版本: {version6} -> {version7}");

            var range2 = versionService.ParseRange("^1.0.0");
            Console.WriteLine($"使用服务解析版本范围: {range2}");
            Console.WriteLine($"版本 {version7} 在范围内: {versionService.IsInRange(version7, range2)}");

            Console.WriteLine("\n演示完成！");
        }
    }
}

//#!/usr/bin/env dotnet
//#:sdk Microsoft.NET.Sdk.Web
//#:package Scrutor@7.0.0
//#:package Carter@10.0.0
//#:package Microsoft.EntityFrameworkCore.InMemory@10.0.3
//#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.3
//#:package Microsoft.Extensions.Caching.Memory@10.0.3
//#:property TargetFramework=net11.0
//#:property RollForward=Major
//#:property Nullable=enable
//#:property ImplicitUsings=enable
//#:property PublishAot=false
//#:property PublishReadyToRun=false
//#:property PublishSingleFile=false

using System;
using System.Threading;
using System.Security.Cryptography;

// MT19937随机数生成器类
// - 使用System.Random和System.Security.Cryptography.RandomNumberGenerator实现随机数生成器
// - MathNet.Numerics已提供MT19937随机数生成器类

namespace System.Threading.Tasks.Cs;

/// <summary>
/// MT19937随机数生成器类，用于生成伪随机数
/// </summary>
/// <remarks>
/// 该类基于MT19937算法，用于生成伪随机数。
/// 该类是Random类的子类，提供了与Random类相同的方法，同时增加了一些额外的方法。
/// MT19937算法是一种高质量的伪随机数生成算法，具有周期长、分布均匀等特点。
/// </remarks>
/// <seealso cref="Random"/>
public class mt19937 : Random
{
    /// <summary>
    /// 53位全1值
    /// </summary>
    private const double FiftyThreeBitsOf1s = 9007199254740991.0;
    /// <summary>
    /// 53位全1值的倒数
    /// </summary>
    private const double Inverse53BitsOf1s = 1.0 / FiftyThreeBitsOf1s;
    /// <summary>
    /// 53位全1值加1
    /// </summary>
    private const double OnePlus53BitsOf1s = FiftyThreeBitsOf1s + 1;
    /// <summary>
    /// 53位全1值加1的倒数
    /// </summary>
    private const double InverseOnePlus53BitsOf1s = 1.0 / OnePlus53BitsOf1s;

    /// <summary>
    /// 线程本地的全局随机数生成器
    /// </summary>
    private static readonly ThreadLocal<mt19937> _globalRandGen = new ThreadLocal<mt19937>();
    /// <summary>
    /// 加密安全的随机数生成器
    /// </summary>
    private readonly RandomNumberGenerator _rng;
    /// <summary>
    /// 基于时间的随机数生成器
    /// </summary>
    private readonly Random _random;

    /// <summary>
    /// 使用指定种子初始化随机数生成器
    /// </summary>
    /// <param name="seed">种子值</param>
    public mt19937(int seed)
    {
        _rng = RandomNumberGenerator.Create();
        _random = new Random(seed);
    }

    /// <summary>
    /// 使用当前系统时间初始化随机数生成器
    /// </summary>
    public mt19937()
    {
        _rng = RandomNumberGenerator.Create();
        _random = new Random();
    }

    /// <summary>
    /// 使用指定的密钥数组初始化随机数生成器
    /// </summary>
    /// <param name="initKey">密钥数组</param>
    public mt19937(uint[] initKey)
    {
        _rng = RandomNumberGenerator.Create();
        // 使用密钥数组计算种子值
        int seed = 0;
        foreach (uint key in initKey)
        {
            seed = seed * 31 + (int)key;
        }
        _random = new Random(seed);
    }

    /// <summary>
    /// 获取线程本地的全局随机数生成器
    /// </summary>
    public static mt19937 global
    {
        get
        {
            mt19937 randGen = _globalRandGen.Value;
            if (null == randGen)
            {
                randGen = new mt19937((int)DateTime.Now.Ticks * Thread.CurrentThread.ManagedThreadId);
                _globalRandGen.Value = randGen;
            }
            return randGen;
        }
    }

    /// <summary>
    /// 生成一个32位无符号随机整数
    /// </summary>
    /// <returns>32位无符号随机整数</returns>
    public virtual uint NextUInt32()
    {
        byte[] buffer = new byte[4];
        _rng.GetBytes(buffer);
        return BitConverter.ToUInt32(buffer, 0);
    }

    /// <summary>
    /// 生成一个小于指定最大值的32位无符号随机整数
    /// </summary>
    /// <param name="maxValue">最大值（不包含）</param>
    /// <returns>小于指定最大值的32位无符号随机整数</returns>
    public virtual uint NextUInt32(uint maxValue)
    {
        if (maxValue == 0)
        {
            return 0;
        }
        byte[] buffer = new byte[4];
        uint value;
        do
        {
            _rng.GetBytes(buffer);
            value = BitConverter.ToUInt32(buffer, 0);
        } while (value >= uint.MaxValue - (uint.MaxValue % maxValue));
        return value % maxValue;
    }

    /// <summary>
    /// 生成一个指定范围内的32位无符号随机整数
    /// </summary>
    /// <param name="minValue">最小值（包含）</param>
    /// <param name="maxValue">最大值（不包含）</param>
    /// <returns>指定范围内的32位无符号随机整数</returns>
    /// <exception cref="ArgumentOutOfRangeException">当最小值大于等于最大值时抛出</exception>
    public virtual uint NextUInt32(uint minValue, uint maxValue)
    {
        if (minValue >= maxValue)
        {
            throw new ArgumentOutOfRangeException();
        }
        uint range = maxValue - minValue;
        return NextUInt32(range) + minValue;
    }

    /// <summary>
    /// 生成一个非负随机整数
    /// </summary>
    /// <returns>非负随机整数</returns>
    public override int Next()
    {
        return Next(int.MaxValue);
    }

    /// <summary>
    /// 生成一个小于指定最大值的非负随机整数
    /// </summary>
    /// <param name="maxValue">最大值（不包含）</param>
    /// <returns>小于指定最大值的非负随机整数</returns>
    /// <exception cref="ArgumentOutOfRangeException">当最大值小于0时抛出</exception>
    public override int Next(int maxValue)
    {
        if (maxValue <= 1)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return 0;
        }
        return _random.Next(maxValue);
    }

    /// <summary>
    /// 生成一个指定范围内的随机整数
    /// </summary>
    /// <param name="minValue">最小值（包含）</param>
    /// <param name="maxValue">最大值（不包含）</param>
    /// <returns>指定范围内的随机整数</returns>
    /// <exception cref="ArgumentOutOfRangeException">当最大值小于等于最小值时抛出</exception>
    public override int Next(int minValue, int maxValue)
    {
        if (maxValue <= minValue)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (maxValue == minValue)
        {
            return minValue;
        }
        return _random.Next(minValue, maxValue);
    }

    /// <summary>
    /// 用随机数填充指定的字节数组
    /// </summary>
    /// <param name="buffer">要填充的字节数组</param>
    /// <exception cref="ArgumentNullException">当buffer为null时抛出</exception>
    public override void NextBytes(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException();
        }
        _rng.GetBytes(buffer);
    }

    /// <summary>
    /// 生成一个0.0到1.0之间的随机浮点数
    /// </summary>
    /// <returns>0.0到1.0之间的随机浮点数</returns>
    public override double NextDouble()
    {
        return _random.NextDouble();
    }

    /// <summary>
    /// 生成一个0.0到1.0之间的随机浮点数
    /// </summary>
    /// <param name="includeOne">是否包含1.0</param>
    /// <returns>0.0到1.0之间的随机浮点数</returns>
    public double NextDouble(bool includeOne)
    {
        if (includeOne)
        {
            byte[] buffer = new byte[8];
            _rng.GetBytes(buffer);
            ulong value = BitConverter.ToUInt64(buffer, 0);
            return value / (double)ulong.MaxValue;
        }
        return NextDouble();
    }

    /// <summary>
    /// 生成一个0.5到1.0之间的随机浮点数
    /// </summary>
    /// <returns>0.5到1.0之间的随机浮点数</returns>
    public double NextDoublePositive()
    {
        return 0.5 + (NextDouble() * 0.5);
    }

    /// <summary>
    /// 生成一个0.0到1.0之间的单精度随机浮点数
    /// </summary>
    /// <returns>0.0到1.0之间的单精度随机浮点数</returns>
    public float NextSingle()
    {
        return (float)NextDouble();
    }

    /// <summary>
    /// 生成一个0.0到1.0之间的单精度随机浮点数
    /// </summary>
    /// <param name="includeOne">是否包含1.0</param>
    /// <returns>0.0到1.0之间的单精度随机浮点数</returns>
    public float NextSingle(bool includeOne)
    {
        return (float)NextDouble(includeOne);
    }

    /// <summary>
    /// 生成一个0.5到1.0之间的单精度随机浮点数
    /// </summary>
    /// <returns>0.5到1.0之间的单精度随机浮点数</returns>
    public float NextSinglePositive()
    {
        return (float)NextDoublePositive();
    }
}

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

namespace System.Threading.Tasks.Go;

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
    /// 状态向量长度
    /// </summary>
    private const int N = 624;
    /// <summary>
    /// 移位参数
    /// </summary>
    private const int M = 397;
    /// <summary>
    /// 矩阵参数
    /// </summary>
    private const uint MatrixA = 0x9908b0df;
    /// <summary>
    /// 高位掩码
    /// </summary>
    private const uint UpperMask = 0x80000000;
    /// <summary>
    /// 低位掩码
    /// </summary>
    private const uint LowerMask = 0x7fffffff;
    /// <summary>
    ///  tempering掩码B
    /// </summary>
    private const uint TemperingMaskB = 0x9d2c5680;
    /// <summary>
    /// tempering掩码C
    /// </summary>
    private const uint TemperingMaskC = 0xefc60000;
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
    /// 用于生成随机数的数组
    /// </summary>
    private static readonly uint[] _mag01 = { 0x0, MatrixA };
    /// <summary>
    /// 线程本地的全局随机数生成器
    /// </summary>
    private static readonly ThreadLocal<mt19937> _globalRandGen = new ThreadLocal<mt19937>();
    /// <summary>
    /// 状态向量
    /// </summary>
    private readonly uint[] _mt = new uint[N];
    /// <summary>
    /// 状态向量索引
    /// </summary>
    private short _mti;

    /// <summary>
    /// 使用指定种子初始化随机数生成器
    /// </summary>
    /// <param name="seed">种子值</param>
    public mt19937(int seed)
    {
        init((uint)seed);
    }

    /// <summary>
    /// 使用当前系统时间初始化随机数生成器
    /// </summary>
    public mt19937()
    {
        init((uint)system_tick.get_tick());
    }

    /// <summary>
    /// 使用指定的密钥数组初始化随机数生成器
    /// </summary>
    /// <param name="initKey">密钥数组</param>
    public mt19937(uint[] initKey)
    {
        init(initKey);
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
                randGen = new mt19937((int)system_tick.get_tick() * Thread.CurrentThread.ManagedThreadId);
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
        return GenerateUInt32();
    }

    /// <summary>
    /// 生成一个小于指定最大值的32位无符号随机整数
    /// </summary>
    /// <param name="maxValue">最大值（不包含）</param>
    /// <returns>小于指定最大值的32位无符号随机整数</returns>
    public virtual uint NextUInt32(uint maxValue)
    {
        return (uint)(GenerateUInt32() / ((double)uint.MaxValue / maxValue));
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
        return (uint)(GenerateUInt32() / ((double)uint.MaxValue / (maxValue - minValue)) + minValue);
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
        return (int)(NextDouble() * maxValue);
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
        return Next(maxValue - minValue) + minValue;
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
        int bufLen = buffer.Length;
        for (int idx = 0; idx < bufLen; ++idx)
        {
            buffer[idx] = (byte)Next(256);
        }
    }

    /// <summary>
    /// 生成一个0.0到1.0之间的随机浮点数
    /// </summary>
    /// <returns>0.0到1.0之间的随机浮点数</returns>
    public override double NextDouble()
    {
        return compute53BitRandom(0, InverseOnePlus53BitsOf1s);
    }

    /// <summary>
    /// 生成一个0.0到1.0之间的随机浮点数
    /// </summary>
    /// <param name="includeOne">是否包含1.0</param>
    /// <returns>0.0到1.0之间的随机浮点数</returns>
    public double NextDouble(bool includeOne)
    {
        return includeOne ? compute53BitRandom(0, Inverse53BitsOf1s) : NextDouble();
    }

    /// <summary>
    /// 生成一个0.5到1.0之间的随机浮点数
    /// </summary>
    /// <returns>0.5到1.0之间的随机浮点数</returns>
    public double NextDoublePositive()
    {
        return compute53BitRandom(0.5, Inverse53BitsOf1s);
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

    /// <summary>
    /// 生成32位无符号随机整数的核心方法
    /// </summary>
    /// <returns>32位无符号随机整数</returns>
    private uint GenerateUInt32()
    {
        uint y;
        if (_mti >= N)
        {
            short kk = 0;
            for (; kk < N - M; ++kk)
            {
                y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                _mt[kk] = _mt[kk + M] ^ (y >> 1) ^ _mag01[y & 0x1];
            }
            for (; kk < N - 1; ++kk)
            {
                y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                _mt[kk] = _mt[kk + (M - N)] ^ (y >> 1) ^ _mag01[y & 0x1];
            }
            y = (_mt[N - 1] & UpperMask) | (_mt[0] & LowerMask);
            _mt[N - 1] = _mt[M - 1] ^ (y >> 1) ^ _mag01[y & 0x1];
            _mti = 0;
        }
        y = _mt[_mti++];
        y ^= temperingShiftU(y);
        y ^= temperingShiftS(y) & TemperingMaskB;
        y ^= temperingShiftT(y) & TemperingMaskC;
        y ^= temperingShiftL(y);
        return y;
    }

    /// <summary>
    ///  tempering移位U
    /// </summary>
    /// <param name="y">输入值</param>
    /// <returns>移位后的值</returns>
    private static uint temperingShiftU(uint y)
    {
        return (y >> 11);
    }

    /// <summary>
    /// tempering移位S
    /// </summary>
    /// <param name="y">输入值</param>
    /// <returns>移位后的值</returns>
    private static uint temperingShiftS(uint y)
    {
        return (y << 7);
    }

    /// <summary>
    /// tempering移位T
    /// </summary>
    /// <param name="y">输入值</param>
    /// <returns>移位后的值</returns>
    private static uint temperingShiftT(uint y)
    {
        return (y << 15);
    }

    /// <summary>
    /// tempering移位L
    /// </summary>
    /// <param name="y">输入值</param>
    /// <returns>移位后的值</returns>
    private static uint temperingShiftL(uint y)
    {
        return (y >> 18);
    }

    /// <summary>
    /// 使用种子初始化状态向量
    /// </summary>
    /// <param name="seed">种子值</param>
    private void init(uint seed)
    {
        _mt[0] = seed & 0xffffffffU;
        for (_mti = 1; _mti < N; _mti++)
        {
            _mt[_mti] = (uint)(1812433253U * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 30)) + _mti);
            _mt[_mti] &= 0xffffffffU;
        }
    }

    /// <summary>
    /// 使用密钥数组初始化状态向量
    /// </summary>
    /// <param name="key">密钥数组</param>
    private void init(uint[] key)
    {
        int i, j, k;
        init(19650218U);
        int keyLength = key.Length;
        i = 1; j = 0;
        k = (N > keyLength ? N : keyLength);
        for (; k > 0; k--)
        {
            _mt[i] = (uint)((_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 30)) * 1664525U)) + key[j] + j);
            _mt[i] &= 0xffffffffU;
            i++; j++;
            if (i >= N) { _mt[0] = _mt[N - 1]; i = 1; }
            if (j >= keyLength) j = 0;
        }
        for (k = N - 1; k > 0; k--)
        {
            _mt[i] = (uint)((_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 30)) * 1566083941U)) - i);
            _mt[i] &= 0xffffffffU;
            i++;
            if (i < N)
            {
                continue;
            }
            _mt[0] = _mt[N - 1]; i = 1;
        }
        _mt[0] = 0x80000000U;
    }

    /// <summary>
    /// 计算53位随机数
    /// </summary>
    /// <param name="translate">平移量</param>
    /// <param name="scale">缩放因子</param>
    /// <returns>53位随机数</returns>
    private double compute53BitRandom(double translate, double scale)
    {
        ulong a = (ulong)GenerateUInt32() >> 5;
        ulong b = (ulong)GenerateUInt32() >> 6;
        return ((a * 67108864.0 + b) + translate) * scale;
    }
}

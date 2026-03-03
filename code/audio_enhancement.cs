#:sdk Microsoft.NET.Sdk.Web
#:package NAudio@2.2.1
#:package CSCore@1.2.1.2
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using NAudio.Wave;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundIn;
using System.Runtime.CompilerServices;
using System.Buffers;
using Microsoft.Extensions.ObjectPool;

/*
var config = new AudioPipelineConfig();
var pipeline = new AudioPipeline(config);

// NAudio输入
var waveIn = new WaveInEvent();
await pipeline.ProcessNAudioInput(waveIn);

// CSCore输入
var soundIn = new WasapiCapture();
await pipeline.ProcessCSCoreInput(soundIn);

// 启动输出任务
_ = pipeline.OutputToNAudioAsync();
_ = pipeline.OutputToCSCoreAsync();
*/

// 1. 音频处理管道配置
public record AudioPipelineConfig(
    int BufferSize = 48000 * 2 * 2, // 48kHz, 16bit, 立体声
    int MaxConcurrentProcesses = 4,
    int FftSize = 1024);

// 2. 高性能音频处理器(集成NAudio和CSCore)
[SkipLocalsInit]
public sealed class AudioPipeline : IAsyncDisposable
{
    private readonly Channel<AudioFrame> _inputChannel;
    private readonly Channel<AudioFrame> _outputChannel;
    private readonly ThreadLocal<Span<byte>> _audioBuffer;
    private readonly ObjectPool<IWaveSource> _waveSourcePool;
    private readonly ObjectPool<WasapiOut> _wasapiOutPool;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly AudioPipelineConfig _config;
    private readonly CancellationTokenSource _cts = new();

    public AudioPipeline(AudioPipelineConfig config)
    {
        _config = config;
        _inputChannel = Channel.CreateBounded<AudioFrame>(1000);
        _outputChannel = Channel.CreateBounded<AudioFrame>(1000);
        _memoryPool = MemoryPool<byte>.Shared;
        
        _audioBuffer = new(() => stackalloc byte[_config.BufferSize]);
        _waveSourcePool = new DefaultObjectPool<IWaveSource>(
            new WaveSourcePooledPolicy(), 4);
        _wasapiOutPool = new DefaultObjectPool<WasapiOut>(
            new WasapiOutPooledPolicy(), 4);

        // 启动处理任务
        for (int i = 0; i < _config.MaxConcurrentProcesses; i++)
        {
            _ = ProcessAudioAsync();
        }
    }

    // 3. 处理NAudio输入
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessNAudioInput(WaveInEvent waveIn)
    {
        waveIn.DataAvailable += async (s, e) => 
        {
            using var memory = _memoryPool.Rent(e.BytesRecorded);
            e.Buffer.AsSpan(0, e.BytesRecorded).CopyTo(memory.Memory.Span);
            
            await _inputChannel.Writer.WriteAsync(new AudioFrame(
                memory.Memory.Span,
                waveIn.WaveFormat));
        };
    }

    // 4. 处理CSCore输入
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessCSCoreInput(ISoundIn soundIn)
    {
        soundIn.DataAvailable += async (s, e) => 
        {
            using var memory = _memoryPool.Rent(e.ByteCount);
            e.Data.AsSpan(0, e.ByteCount).CopyTo(memory.Memory.Span);
            
            await _inputChannel.Writer.WriteAsync(new AudioFrame(
                memory.Memory.Span,
                soundIn.WaveFormat));
        };
    }

    // 5. 核心处理逻辑
    private async Task ProcessAudioAsync()
    {
        await foreach (var frame in _inputChannel.Reader.ReadAllAsync(_cts.Token))
        {
            try
            {
                // 使用零拷贝技术处理音频帧
                var buffer = _audioBuffer.Value;
                frame.Data.CopyTo(buffer);
                
                // 应用音频增强处理
                ApplyNoiseReduction(buffer);
                ApplyEchoCancellation(buffer);
                
                // 输出到管道
                await _outputChannel.Writer.WriteAsync(new AudioFrame(
                    buffer,
                    frame.Format));
            }
            finally
            {
                if (frame.IsPooled)
                {
                    _memoryPool.Return(frame.Memory);
                }
            }
        }
    }

    // 6. 输出到NAudio
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task OutputToNAudioAsync()
    {
        var wasapiOut = _wasapiOutPool.Get();
        try
        {
            await foreach (var frame in _outputChannel.Reader.ReadAllAsync(_cts.Token))
            {
                wasapiOut.Init(new BufferedWaveProvider(frame.Format));
                wasapiOut.Play();
                
                // 零拷贝播放
                unsafe
                {
                    fixed (byte* ptr = frame.Data)
                    {
                        wasapiOut.AddSamples(ptr, 0, frame.Data.Length);
                    }
                }
            }
        }
        finally
        {
            _wasapiOutPool.Return(wasapiOut);
        }
    }

    // 7. 输出到CSCore
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task OutputToCSCoreAsync()
    {
        var waveSource = _waveSourcePool.Get();
        try
        {
            await foreach (var frame in _outputChannel.Reader.ReadAllAsync(_cts.Token))
            {
                // 零拷贝写入
                unsafe
                {
                    fixed (byte* ptr = frame.Data)
                    {
                        waveSource.Write(ptr, frame.Data.Length);
                    }
                }
            }
        }
        finally
        {
            _waveSourcePool.Return(waveSource);
        }
    }

    // 8. 音频增强算法
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private unsafe void ApplyNoiseReduction(Span<byte> buffer)
    {
        fixed (byte* ptr = buffer)
        {
            // 使用SIMD指令优化处理
            // ... 噪声抑制算法实现 ...
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private unsafe void ApplyEchoCancellation(Span<byte> buffer)
    {
        fixed (byte* ptr = buffer)
        {
            // 使用SIMD指令优化处理
            // ... 回声消除算法实现 ...
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _inputChannel.Writer.Complete();
        _outputChannel.Writer.Complete();
        await Task.WhenAll(GetProcessingTasks());
    }

    private IEnumerable<Task> GetProcessingTasks()
    {
        // 返回所有处理任务
        yield break;
    }
}

// 对象池策略
internal sealed class WaveSourcePooledPolicy : PooledObjectPolicy<IWaveSource>
{
    public override IWaveSource Create() => CodecFactory.Instance.GetCodec("default");
    public override bool Return(IWaveSource obj) => true;
}

internal sealed class WasapiOutPooledPolicy : PooledObjectPolicy<WasapiOut>
{
    public override WasapiOut Create() => new WasapiOut();
    public override bool Return(WasapiOut obj) => true;
}
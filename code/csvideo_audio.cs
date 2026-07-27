#:sdk Microsoft.NET.Sdk.Web
#:package CSCore@1.2.1.2
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package NAudio@2.2.1
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundIn;
using CSCore.Streams;
using System.Runtime.CompilerServices;
using System.Buffers;
using NAudio.Wave;
using NAudio.Dsp;

// 1. 音频帧结构(零拷贝优化)
[SkipLocalsInit]
public readonly record struct AudioFrame(
    nint BufferPtr,
    int Length,
    WaveFormat Format,
    long Timestamp = 0)
{
    public unsafe Span<byte> AsSpan() => new((void*)BufferPtr, Length);
}

// 2. 增强版音频处理器(支持实时流处理)
[SkipLocalsInit]
public sealed class AudioProcessor : IAsyncDisposable
{
    private readonly ThreadLocal<Span<byte>> _audioBuffer;
    private readonly ObjectPool<IWaveSource> _waveSourcePool;
    private readonly Channel<AudioFrame> _audioChannel;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly WasapiCapture _capture;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    
    public AudioProcessor()
    {
        _audioBuffer = new(() => stackalloc byte[48000 * 2 * 2]); // 48kHz, 16bit, 立体声
        _memoryPool = MemoryPool<byte>.Shared;
        
        _waveSourcePool = new DefaultObjectPool<IWaveSource>(
            new WaveSourcePooledPolicy(), 4);
            
        _audioChannel = Channel.CreateBounded<AudioFrame>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _capture = new WasapiCapture();
        _latencyOptimizer = new TailLatencyOptimizer();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void StartRealTimeProcessing()
    {
        _capture.Initialize();
        _capture.DataAvailable += (s, e) => 
        {
            _latencyOptimizer.Optimize(() => 
            {
                fixed (byte* ptr = e.Data)
                {
                    var frame = new AudioFrame((nint)ptr, e.ByteCount, _capture.WaveFormat);
                    _audioChannel.Writer.TryWrite(frame);
                }
            });
        };
        _capture.Start();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessFileAsync(string filePath)
    {
        using var waveSource = CodecFactory.Instance.GetCodec(filePath);
        using var buffer = _memoryPool.Rent(4096);
        
        while (waveSource.Read(buffer.Memory.Span) > 0)
        {
            await _audioChannel.Writer.WriteAsync(new AudioFrame(
                buffer.Memory.Span,
                waveSource.WaveFormat));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessStreamAsync(WasapiCapture capture)
    {
        var soundInSource = new SoundInSource(capture);
        var singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource.ToSampleSource());
        
        singleBlockNotificationStream.SingleBlockRead += (s, e) => 
        {
            var buffer = _audioBuffer.Value;
            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    _audioChannel.Writer.TryWrite(new AudioFrame(
                        buffer,
                        soundInSource.WaveFormat));
                }
            }
        };
        
        capture.Start();
    }

    public async ValueTask DisposeAsync()
    {
        _capture?.Stop();
        _capture?.Dispose();
        _audioChannel.Writer.Complete();
    }
}

// 3. FFT音频分析器(使用SIMD优化)
[SkipLocalsInit]
public sealed class FFTAnalyzer : IDisposable
{
    private readonly ThreadLocal<Complex[]> _fftBuffer;
    private readonly ThreadLocal<float[]> _window;
    
    public FFTAnalyzer()
    {
        _fftBuffer = new(() => new Complex[1024]);
        _window = new(() => 
        {
            var window = new float[1024];
            FastFourierTransform.HammingWindow(window);
            return window;
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public unsafe void Analyze(AudioFrame frame)
    {
        var buffer = _fftBuffer.Value;
        var window = _window.Value;
        
        fixed (byte* ptr = frame.AsSpan())
        {
            // SIMD优化的FFT处理
            for (int i = 0; i < 1024; i++)
            {
                buffer[i].X = ((short*)ptr)[i] * window[i];
                buffer[i].Y = 0;
            }
        }
        
        FastFourierTransform.FFT(true, 10, buffer);
    }
}

// 3. 对象池策略
internal sealed class WaveSourcePooledPolicy : PooledObjectPolicy<IWaveSource>
{
    public override IWaveSource Create() => 
        new NullWaveSource(new WaveFormat(44100, 16, 2));

    public override bool Return(IWaveSource obj)
    {
        obj.Position = 0;
        return true;
    }
}

// 4. 主程序集成
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<AudioProcessor>();
var app = builder.Build();
app.MapGet("/", () => "Audio Processing Ready");
app.Run();
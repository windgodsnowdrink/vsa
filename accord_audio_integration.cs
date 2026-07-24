#:sdk Microsoft.NET.Sdk.Web
#:package Accord.Audio@3.8.0
#:package NAudio@2.2.1
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Buffers;
using System.Threading.Channels;
using Accord.Audio;
using Accord.Audio.Filters;
using Accord.Audio.Windows;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Runtime.CompilerServices;

// 1. 音频处理管道接口
public interface IAudioProcessingPipeline
{
    ValueTask ProcessAudioAsync(byte[] audioData, WaveFormat format);
    IAsyncEnumerable<FFTResult> GetFFTResultsAsync(CancellationToken ct = default);
    IAsyncEnumerable<Voiceprint> GetVoiceprintsAsync(CancellationToken ct = default);
}

// 2. 高性能音频处理器实现
[SkipLocalsInit]
public sealed class AudioProcessor : IAudioProcessingPipeline, IDisposable
{
    private readonly ThreadLocal<Span<byte>> _audioBuffer;
    private readonly ObjectPool<Signal> _signalPool;
    private readonly Channel<FFTResult> _fftChannel;
    private readonly Channel<Voiceprint> _voiceprintChannel;
    private readonly List<IAudioAnalyzer> _analyzers;
    private readonly CancellationTokenSource _cts = new();

    public AudioProcessor(IEnumerable<IAudioAnalyzer> analyzers)
    {
        _analyzers = analyzers.ToList();
        _audioBuffer = new(() => stackalloc byte[48000 * 2 * 2]); // 48kHz, 16bit, 立体声
        _signalPool = new DefaultObjectPool<Signal>(new SignalPooledPolicy(), 4);
        _fftChannel = Channel.CreateBounded<FFTResult>(1000);
        _voiceprintChannel = Channel.CreateBounded<Voiceprint>(1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe ValueTask ProcessAudioAsync(byte[] audioData, WaveFormat format)
    {
        var signal = _signalPool.Get();
        try
        {
            // 零拷贝处理
            fixed (byte* ptr = audioData)
            {
                signal.FromArray((float*)ptr, audioData.Length / sizeof(float));
            }
            
            // 并行分析
            var tasks = _analyzers
                .Where(x => x.SupportsFormat(format))
                .Select(x => x.AnalyzeAsync(signal, format));
            
            return new ValueTask(Task.WhenAll(tasks));
        }
        finally
        {
            _signalPool.Return(signal);
        }
    }

    public async IAsyncEnumerable<FFTResult> GetFFTResultsAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var result in _fftChannel.Reader.ReadAllAsync(ct))
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<Voiceprint> GetVoiceprintsAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var voiceprint in _voiceprintChannel.Reader.ReadAllAsync(ct))
        {
            yield return voiceprint;
        }
    }

    public void Dispose() => _cts.Cancel();
}

// 3. FFT分析器实现
[SkipLocalsInit]
public sealed class FFTAnalyzer : IAudioAnalyzer
{
    private readonly ChannelWriter<FFTResult> _writer;
    private readonly ThreadLocal<Span<byte>> _fftBuffer;

    public FFTAnalyzer(Channel<FFTResult> channel)
    {
        _writer = channel.Writer;
        _fftBuffer = new(() => stackalloc byte[4096]);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AnalyzeAsync(Signal signal, WaveFormat format)
    {
        // 应用汉宁窗
        var window = new HanningWindow(signal.Length);
        window.ApplyInPlace(signal);
        
        // 执行FFT
        var fft = new FourierTransform();
        var spectrum = fft.Forward(signal);
        
        // 计算幅度谱
        var magnitudes = new float[spectrum.Length];
        for (int i = 0; i < spectrum.Length; i++)
        {
            magnitudes[i] = (float)spectrum[i].Magnitude;
        }
        
        // 发送FFT结果
        await _writer.WriteAsync(new FFTResult(
            magnitudes,
            DateTime.UtcNow,
            format.SampleRate));
    }
}

// 4. 声纹特征提取器
[SkipLocalsInit]
public sealed class VoiceprintExtractor : IAudioAnalyzer
{
    private readonly ChannelWriter<Voiceprint> _writer;
    private readonly ThreadLocal<Span<byte>> _voiceprintBuffer;

    public VoiceprintExtractor(Channel<Voiceprint> channel)
    {
        _writer = channel.Writer;
        _voiceprintBuffer = new(() => stackalloc byte[2048]);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AnalyzeAsync(Signal signal, WaveFormat format)
    {
        // 提取MFCC特征
        var mfcc = new MelFrequencyCepstrumCoefficient();
        var coefficients = mfcc.Transform(signal);
        
        // 发送声纹特征
        await _writer.WriteAsync(new Voiceprint(
            coefficients.Select(x => (float)x).ToArray(),
            DateTime.UtcNow));
    }
}

// 5. 主程序集成
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<IAudioProcessingPipeline, AudioProcessor>();
var app = builder.Build();
app.MapGet("/", () => "Audio Processing Ready");
app.Run();

// 数据结构
public record FFTResult(
    float[] Magnitudes,
    DateTime Timestamp,
    int SampleRate);

public record Voiceprint(
    float[] Coefficients,
    DateTime Timestamp);
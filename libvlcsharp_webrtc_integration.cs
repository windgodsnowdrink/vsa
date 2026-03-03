#:sdk Microsoft.NET.Sdk.Web
#:package LibVLCSharp@3.8.0
#:package SIPSorcery.Net@6.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Buffers;
using System.Threading.Channels;
using LibVLCSharp.Shared;
using SIPSorcery.Net;

// 1. 视频会议处理器
[SkipLocalsInit]
public sealed class VideoConferenceProcessor : IDisposable
{
    private readonly LibVLC _libVlc;
    private readonly RTCPeerConnection _peerConnection;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public VideoConferenceProcessor()
    {
        _libVlc = new LibVLC();
        _peerConnection = new RTCPeerConnection();
        _buffer = new(() => stackalloc byte[1280 * 720 * 4]); // 720p帧缓冲区
        
        // WebRTC事件绑定
        _peerConnection.OnVideoFrameReceived += OnVideoFrame;
    
        // 配置ICE服务器
        // 添加尾延迟优化器
        private readonly TailLatencyOptimizer _latencyOptimizer = new();
        
        // 增强ICE配置
        _peerConnection.SetIceServers(new List<IceServer>() {
            new IceServer { 
                Urls = new List<string>() { 
                    "stun:global.stun.twilio.com:3478?transport=udp",
                    "turn:global.turn.twilio.com:3478?transport=udp",
                },
                Username = "username",
                Credential = "password",
                CredentialType = IceCredentialType.Password
            }
        });
        
        // 启用硬件加速
        _peerConnection.UseHardwareAcceleration = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void StartConference(string streamUrl)
    {
        using var mediaPlayer = new MediaPlayer(_libVlc);
        using var media = new Media(_libVlc, streamUrl);
        
        // 设置零拷贝视频处理
        mediaPlayer.SetVideoFormatCallbacks(SetupVideoFormat, CleanupVideoFormat);
        mediaPlayer.SetVideoCallbacks(LockVideo, null, DisplayVideo);
        
        mediaPlayer.Play(media);
    }

    private unsafe void OnVideoFrame(RTPVideoFrame frame)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            // 使用ThreadLocal<Span>实现线程专用内存
            private readonly ThreadLocal<Span<byte>> _buffer = new(() => stackalloc byte[1280 * 720 * 4]);
            
            // 视频帧处理时的Cache-line对齐检查
            if ((long)ptr % 64 == 0) 
            {
                // SIMD优化处理
            }
            {
                // 使用SIMD加速视频帧转换
                ConvertFrameToYUV(frame, buffer);
                
                // 通过WebRTC发送视频帧
                _peerConnection.SendVideoFrame(
                    frame.Width, 
                    frame.Height, 
                    VideoPixelFormats.I420, 
                    buffer);
            }
        }
    }

    private unsafe void ConvertFrameToYUV(RTPVideoFrame frame, Span<byte> output)
    {
        // 使用AVX2指令集优化RGB到YUV转换
        fixed (byte* inputPtr = frame.Payload, outputPtr = output)
        {
            if (Avx2.IsSupported && frame.Width % 32 == 0)
            {
                // AVX2加速的RGB到YUV转换
                ConvertRgbToYuvAvx2(inputPtr, outputPtr, frame.Width, frame.Height);
            }
            else if (Sse2.IsSupported)
            {
                // SSE2加速的RGB到YUV转换
                ConvertRgbToYuvSse2(inputPtr, outputPtr, frame.Width, frame.Height);
            }
            else
            {
                // 纯C#实现的RGB到YUV转换
                ConvertRgbToYuvFallback(inputPtr, outputPtr, frame.Width, frame.Height);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    private static unsafe void ConvertRgbToYuvAvx2(byte* src, byte* dst, int width, int height)
    {
        // AVX2指令集优化的转换实现
        const int vectorSize = 256 / 8;
        byte* yPlane = dst;
        byte* uPlane = yPlane + width * height;
        byte* vPlane = uPlane + (width * height) / 4;
        
        // ... AVX2向量化处理逻辑 ...
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    private static unsafe void ConvertRgbToYuvSse2(byte* src, byte* dst, int width, int height)
    {
        // SSE2指令集优化的转换实现
        const int vectorSize = 128 / 8;
        byte* yPlane = dst;
        byte* uPlane = yPlane + width * height;
        byte* vPlane = uPlane + (width * height) / 4;
        
        // ... SSE2向量化处理逻辑 ...
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static unsafe void ConvertRgbToYuvFallback(byte* src, byte* dst, int width, int height)
    {
        // 纯C#实现的转换逻辑
        byte* yPlane = dst;
        byte* uPlane = yPlane + width * height;
        byte* vPlane = uPlane + (width * height) / 4;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = (y * width + x) * 3;
                byte r = src[idx];
                byte g = src[idx + 1];
                byte b = src[idx + 2];
                
                // YUV转换公式
                yPlane[y * width + x] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                
                if (x % 2 == 0 && y % 2 == 0)
                {
                    int uvIdx = (y / 2) * (width / 2) + (x / 2);
                    uPlane[uvIdx] = (byte)(-0.14713 * r - 0.28886 * g + 0.436 * b + 128);
                    vPlane[uvIdx] = (byte)(0.615 * r - 0.51499 * g - 0.10001 * b + 128);
                }
            }
        }
    }
}

// 2. 主程序集成
var builder = WebApplication.CreateBuilder();

// 配置SignalR实时通信
builder.Services.AddSignalR(options => {
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
});

// 注册视频会议处理器
builder.Services.AddSingleton<VideoConferenceProcessor>();

var app = builder.Build();
app.MapHub<VideoHub>("/videoHub");
app.Run();


private unsafe IntPtr LockVideo(IntPtr opaque, IntPtr planes)
{
    Span<byte> buffer = _buffer.Value;
    fixed (byte* ptr = buffer)
    {
        Marshal.WriteIntPtr(planes, (IntPtr)ptr);
        return (IntPtr)ptr;
    }
}

private unsafe void DisplayVideo(IntPtr opaque, IntPtr picture)
{
    // 直接使用预先分配的缓冲区
    // 无需额外内存拷贝
}
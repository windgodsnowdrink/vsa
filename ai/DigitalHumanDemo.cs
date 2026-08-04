#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.3
#:package System.Threading.Channels@10.0.3
#:package Microsoft.Extensions.ObjectPool@10.0.3
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:package Ardalis.ListStartupServices@1.1.4
#:package Serilog@4.3.0
#:package Serilog.AspNetCore@9.0.0
#:package Serilog.Enrichers.Environment@3.0.1
#:package Serilog.Enrichers.Process@3.0.0
#:package Serilog.Enrichers.Span@3.1.0
#:package Serilog.Enrichers.Thread@4.0.0
#:package Serilog.Exceptions@8.4.0
#:package Serilog.Extensions.Hosting@9.0.0
#:package Serilog.Extensions.Logging@9.0.2
#:package Serilog.Formatting.Compact@3.0.0
#:package Serilog.Settings.Configuration@9.0.0
#:package Serilog.Sinks.Async@2.1.0
#:package Serilog.Sinks.Console@6.0.0
#:package Serilog.Sinks.EventLog@4.0.0
#:package Serilog.Sinks.Http@9.2.0
#:package Serilog.Sinks.RollingFileAlternate@2.0.9
#:package Serilog.Sinks.Seq@9.0.0
#:package Serilog.Sinks.SpectreConsole@0.3.3
#:package Serilog.Sinks.Trace@4.0.0
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:package NAudio.Core@2.3.0
#:package Whisper.net@1.9.1-preview1
#:package Whisper.net.Runtime@1.9.1-preview1
#:package OllamaSharp@5.4.23
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.

/*
 * 安装 .NET 10 SDK（预览版即可）
 * https://github.com/dotnet/installer
 * 安装 Linux 依赖（Ubuntu/Debian 举例） sudo apt update sudo apt install ffmpeg libportaudio2 alsa-utils
 * 安装 Ollama（本地 LLM）
 * curl -fsSL https://ollama.com/install.sh | sh
 * ollama pull llama3.2:3b # 3B 模型足够
 * 新建文件夹 DigitalHumanDemo 并进入 mkdir DigitalHumanDemo && cd DigitalHumanDemo
 */
// dotnet 10 单文件 + VSA 数字人 Demo
// 编译：dotnet publish -c Release -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -r linux-x64 --self-contained
// 权限:  chmod +x DigitalHumanDemo ./DigitalHumanDemo
// 运行：./DigitalHumanDemo

using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Whisper.net;
using Whisper.net.Ggml;
using OllamaSharp;
using OllamaSharp.Models;

// 用 Redis 或 Postgres 做对话上下文存储
// 用 YARP 做网关，支持多数字人并发
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");
var app = builder.Build();

// 1. 初始化 Whisper（ASR）
var whisperModel = "ggml-base.bin";
if (!File.Exists(whisperModel))
{
    Console.WriteLine("下载 Whisper 模型...");
    var httpClient = new HttpClient();
    Whisper.net.Ggml.WhisperGgmlDownloader downloader = new Whisper.net.Ggml.WhisperGgmlDownloader(httpClient);
    using var modelStream = await downloader.GetGgmlModelAsync(GgmlType.Base);
    using var fileWriter = File.OpenWrite(whisperModel);
    await modelStream.CopyToAsync(fileWriter);
}
var whisperFactory = WhisperFactory.FromPath(whisperModel);

// 2. 初始化 Ollama（LLM）
var ollama = new OllamaApiClient(new Uri("http://localhost:11434"));
ollama.SelectedModel = "llama3.2:3b";
var chat = new Chat(ollama);

// 3. 初始化 TTS（System.Speech 仅 Windows，这里用 espeak-ng 命令行）
async Task<byte[]> SynthesizeAsync(string text, CancellationToken ct = default)
{
    var tmp = Path.GetTempFileName() + ".wav";
    // 用 SileroVAD 做端点检测，实现“打断”
    //  StyleTTS2 或 Coqui XTTS 替换 espeak-ng，提升音质
    var psi = new System.Diagnostics.ProcessStartInfo
    {
        FileName = "espeak-ng",
        Arguments = $"-w {tmp} -p 60 -s 170 \"{text}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    using var proc = System.Diagnostics.Process.Start(psi);
    await proc.WaitForExitAsync(ct);
    var bytes = await File.ReadAllBytesAsync(tmp, ct);
    File.Delete(tmp);
    return bytes;
}

// 4. WebSocket 处理
app.UseWebSockets();
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path == "/ws" && ctx.WebSockets.IsWebSocketRequest)
    {
        using var ws = await ctx.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[1024 * 32];
        await foreach (var seg in ReceiveAudioAsync(ws, buffer))
        {
            // ASR
            var text = await TranscribeAsync(seg);
            if (string.IsNullOrWhiteSpace(text)) continue;

            // LLM
            await foreach (var reply in chat.SendAsync(text))
            {
                // Console.Write(reply);
                // TTS
                var wav = await SynthesizeAsync(reply);

                // 回传
                await ws.SendAsync(new ArraySegment<byte>(wav), WebSocketMessageType.Binary, true, CancellationToken.None);

            }
        }
    }
    else await next();
});

// 前端页面（直接内嵌）,前端改用 React + WebRTC，支持流式 ASR/TTS
const string Html = """
<!doctype html>
<html lang="zh-CN">
<head>
<meta charset="utf-8"/>
<title>数字人 Demo</title>
<style>
body{margin:0;background:#111;color:#eee;font-family:Arial,Helvetica,sans-serif;display:flex;justify-content:center;align-items:center;height:100vh;flex-direction:column}
#btn{width:90px;height:90px;border-radius:50%;border:3px solid #0cf;background:#222;color:#0cf;font-size:40px;cursor:pointer}
#btn.recording{background:#f33;color:#fff}
#status{margin-top:20px}
</style>
</head>
<body>
<button id="btn">🎤</button>
<div id="status">点击麦克风开始说话</div>
<script>
const btn = document.getElementById('btn');
const status = document.getElementById('status');
let recorder, ws, chunks = [];
btn.onclick = async () => {
  if (!recorder) {
    const stream = await navigator.mediaDevices.getUserMedia({audio:true});
    recorder = new MediaRecorder(stream, {mimeType:'audio/webm'});
    recorder.ondataavailable = e => chunks.push(e.data);
    recorder.onstop = async () => {
      const blob = new Blob(chunks, {type:'audio/webm'});
      ws.send(blob);
      chunks = [];
    };
  }
  if (btn.classList.toggle('recording')) {
    ws = new WebSocket(`ws://${location.host}/ws`);
    ws.binaryType = 'arraybuffer';
    ws.onmessage = async e => {
      const blob = new Blob([e.data], {type:'audio/wav'});
      const url = URL.createObjectURL(blob);
      new Audio(url).play();
      status.textContent = '播放中...';
    };
    recorder.start();
    status.textContent = '正在录音...';
  } else {
    recorder.stop();
    status.textContent = '识别中...';
  }
};
</script>
</body>
</html>
""";

// 5. 静态页面
app.MapGet("/", () => Results.Content(Html, "text/html"));

app.Run();

// ------------------ 下面是 VSA 的“切片”实现 ------------------
// 后端拆分 Vertical Slice：ASR Slice / LLM Slice / TTS Slice
async IAsyncEnumerable<byte[]> ReceiveAudioAsync(WebSocket ws, byte[] buffer)
{
    using var ms = new MemoryStream();
    WebSocketReceiveResult result;
    do
    {
        result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        ms.Write(buffer, 0, result.Count);
    } while (!result.CloseStatus.HasValue && result.MessageType != WebSocketMessageType.Close);
    yield return ms.ToArray();
}

async Task<string> TranscribeAsync(byte[] pcm)
{
    using var processor = whisperFactory.CreateBuilder()
                                        .WithLanguage("auto")
                                        .Build();
    using var ms = new MemoryStream(pcm);
    var text = new StringBuilder();
    await foreach (var seg in processor.ProcessAsync(ms))
        text.Append(seg.Text);
    return text.ToString().Trim();
}

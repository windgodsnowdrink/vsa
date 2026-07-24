#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@6.1.0
#:package Minio@7.1.0
#:package System.IO.Pipelines@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using FastEndpoints;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

//app.UseFastEndpoints();

public class MultipartUploadEndpoint : Endpoint<FileUploadRequest, FileUploadResponse>
{
    private readonly AdvancedMinioService _minioService;
    private readonly ObjectPool<Memory<byte>> _bufferPool;

    public MultipartUploadEndpoint(
        AdvancedMinioService minioService,
        ObjectPool<Memory<byte>> bufferPool)
    {
        _minioService = minioService;
        _bufferPool = bufferPool;
    }

    public override void Configure()
    {
        Post("/api/minio/multipart");
        AllowFileUploads();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task HandleAsync(FileUploadRequest req, CancellationToken ct)
    {
        using var stream = req.File.OpenReadStream();
        await _minioService.MultiThreadUploadAsync(req.Bucket, req.File.FileName, stream);
        await SendAsync(new(req.File.FileName, req.File.Length));
    }
}

public class StreamUploadEndpoint : Endpoint<FileUploadRequest, FileUploadResponse>
{
    private readonly AdvancedMinioService _minioService;
    private readonly ObjectPool<Memory<byte>> _bufferPool;

    public StreamUploadEndpoint(
        AdvancedMinioService minioService,
        ObjectPool<Memory<byte>> bufferPool)
    {
        _minioService = minioService;
        _bufferPool = bufferPool;
    }

    public override void Configure()
    {
        Post("/api/minio/stream");
        AllowFileUploads();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task HandleAsync(FileUploadRequest req, CancellationToken ct)
    {
        var pipe = new Pipe();
        var writingTask = WriteToPipeAsync(req.File, pipe.Writer);
        var uploadingTask = _minioService.StreamUploadAsync(req.Bucket, req.File.FileName, pipe.Reader);

        await Task.WhenAll(writingTask, uploadingTask);
        await SendAsync(new(req.File.FileName, req.File.Length));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task WriteToPipeAsync(IFormFile file, PipeWriter writer)
    {
        var buffer = _bufferPool.Get();
        try
        {
            using var stream = file.OpenReadStream();
            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer);
                if (bytesRead == 0) break;

                var memory = writer.GetMemory(buffer.Length);
                buffer.CopyTo(memory.Span);
                writer.Advance(bytesRead);

                var result = await writer.FlushAsync();
                if (result.IsCompleted) break;
            }
        }
        finally
        {
            _bufferPool.Return(buffer);
            await writer.CompleteAsync();
        }
    }
}

public class BatchUploadEndpoint : Endpoint<BatchFileUploadRequest, BatchUploadResponse>
{
    private readonly AdvancedMinioService _minioService;

    public BatchUploadEndpoint(AdvancedMinioService minioService)
    {
        _minioService = minioService;
    }

    public override void Configure()
    {
        Post("/api/minio/batch");
        AllowFileUploads();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task HandleAsync(BatchFileUploadRequest req, CancellationToken ct)
    {
        var tasks = req.Files.Select(file => 
        {
            using var stream = file.OpenReadStream();
            return _minioService.MultiThreadUploadAsync(req.Bucket, file.FileName, stream);
        });

        await Task.WhenAll(tasks);
        await SendAsync(new(req.Files.Count()));
    }
}

public class MultipartWithProgressEndpoint : Endpoint<FileUploadRequest, UploadProgressResponse>
{
    private readonly AdvancedMinioService _minioService;
    private readonly IHubContext<UploadProgressHub> _hubContext;

    public MultipartWithProgressEndpoint(
        AdvancedMinioService minioService,
        IHubContext<UploadProgressHub> hubContext)
    {
        _minioService = minioService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("/api/minio/multipart-with-progress");
        AllowFileUploads();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task HandleAsync(FileUploadRequest req, CancellationToken ct)
    {
        var uploadId = Guid.NewGuid().ToString();
        var progressChannel = Channel.CreateBounded<UploadProgress>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _ = Task.Run(async () => 
        {
            await foreach (var progress in progressChannel.Reader.ReadAllAsync(ct))
            {
                await _hubContext.Clients.Group(uploadId)
                    .SendAsync("ProgressUpdate", new 
                    {
                        UploadId = uploadId,
                        progress.BytesTransferred,
                        progress.Percentage
                    }, ct);
            }
        });

        using var stream = req.File.OpenReadStream();
        await _minioService.MultiThreadUploadWithProgressAsync(
            req.Bucket, 
            req.File.FileName, 
            stream,
            progressChannel.Writer);

        await SendAsync(new(uploadId, req.File.FileName));
    }
}

public class DownloadEndpoint : Endpoint<DownloadRequest>
{
    private readonly AdvancedMinioService _minioService;

    public DownloadEndpoint(AdvancedMinioService minioService)
    {
        _minioService = minioService;
    }

    public override void Configure()
    {
        Get("/api/minio/download");
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task HandleAsync(DownloadRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _minioService.DownloadFileAsync(req.Bucket, req.ObjectName, req.UseCache);
            await SendStreamAsync(result, req.ObjectName, "application/octet-stream");
        }
        catch (Exception ex)
        {
            AddError(ex.Message);
            await SendErrorsAsync();
        }
    }
}

// DTOs
public record FileUploadRequest([FromForm] IFormFile File, [FromQuery] string Bucket = "default");
public record FileUploadResponse(string FileName, long Length);
public record BatchFileUploadRequest([FromForm] IEnumerable<IFormFile> Files, [FromQuery] string Bucket = "default");
public record BatchUploadResponse(int Count);
public record UploadProgressResponse(string UploadId, string FileName);
public record DownloadRequest([FromQuery] string ObjectName, [FromQuery] string Bucket = "default", [FromQuery] bool UseCache = true);
public record UploadProgress(long BytesTransferred, double Percentage);
#:sdk Microsoft.NET.Sdk.Web
#:package Minio@7.1.0
#:package System.IO.Pipelines@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System.IO.Pipelines;
using Microsoft.AspNetCore.Http.HttpResults;
using Vertical.Slice.Template.Shared.Core;

namespace Vertical.Slice.Template.Features.Minio.Upload;

/*
app.MapPost("/api/minio/multipart", MinioEndpoints.MultipartUploadAsync);
app.MapPost("/api/minio/stream", MinioEndpoints.StreamUploadAsync);
app.MapPost("/api/minio/batch", MinioEndpoints.BatchUploadAsync);
app.MapPost("/api/minio/multipart-with-progress", MinioEndpoints.MultipartUploadWithProgressAsync);
*/

public static class MinioEndpoints
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static async Task<IResult> MultipartUploadAsync(
        [FromForm] IFormFile file,
        [FromQuery] string bucket,
        AdvancedMinioService minioService,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        using var stream = file.OpenReadStream();
        await minioService.MultiThreadUploadAsync(bucket, file.FileName, stream);
        
        return Results.Ok(new { file.FileName, file.Length });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static async Task<IResult> StreamUploadAsync(
        [FromForm] IFormFile file,
        [FromQuery] string bucket,
        AdvancedMinioService minioService,
        ObjectPool<Memory<byte>> bufferPool,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        var pipe = new Pipe();
        var writingTask = WriteToPipeAsync(file, pipe.Writer, bufferPool);
        var uploadingTask = minioService.StreamUploadAsync(bucket, file.FileName, pipe.Reader);

        await Task.WhenAll(writingTask, uploadingTask);
        return Results.Ok(new { file.FileName, file.Length });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static async Task<IResult> BatchUploadAsync(
        [FromForm] IEnumerable<IFormFile> files,
        [FromQuery] string bucket,
        AdvancedMinioService minioService,
        CancellationToken cancellationToken)
    {
        if (files == null || !files.Any())
            return Results.BadRequest("No files uploaded");

        var tasks = files.Select(file => 
        {
            using var stream = file.OpenReadStream();
            return minioService.MultiThreadUploadAsync(bucket, file.FileName, stream);
        });

        await Task.WhenAll(tasks);
        return Results.Ok(new { Count = files.Count() });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static async Task<IResult> MultipartUploadWithProgressAsync(
        [FromForm] IFormFile file,
        [FromQuery] string bucket,
        AdvancedMinioService minioService,
        IHubContext<UploadProgressHub> hubContext,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file uploaded");

        var uploadId = Guid.NewGuid().ToString();
        using var stream = file.OpenReadStream();
        
        var progressChannel = Channel.CreateBounded<UploadProgress>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _ = Task.Run(async () => 
        {
            await foreach (var progress in progressChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await hubContext.Clients.Group(uploadId)
                    .SendAsync("ProgressUpdate", new 
                    {
                        UploadId = uploadId,
                        progress.BytesTransferred,
                        progress.Percentage
                    }, cancellationToken);
            }
        });

        await minioService.MultiThreadUploadWithProgressAsync(
            bucket, 
            file.FileName, 
            stream,
            progressChannel.Writer);

        return Results.Ok(new { UploadId = uploadId, file.FileName });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static async Task WriteToPipeAsync(
        IFormFile file, 
        PipeWriter writer,
        ObjectPool<Memory<byte>> bufferPool)
    {
        var buffer = bufferPool.Get();
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
            bufferPool.Return(buffer);
            await writer.CompleteAsync();
        }
    }
}

public record UploadProgress(long BytesTransferred, double Percentage);
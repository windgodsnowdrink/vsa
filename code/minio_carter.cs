#:sdk Microsoft.NET.Sdk.Web
#:package Carter@7.0.0
#:package Minio@7.1.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable

using Carter;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Http;

public class MinioModule : CarterModule
{
    private readonly AdvancedMinioService _minioService;
    private readonly ObjectPool<Memory<byte>> _bufferPool;

    public MinioModule(AdvancedMinioService minioService, ObjectPool<Memory<byte>> bufferPool)
    {
        _minioService = minioService;
        _bufferPool = bufferPool;

        // 多线程分块上传
        Post("/minio/multipart", async (req, res) => 
        {
            var file = req.Form.Files.GetFile("file");
            var bucket = req.Query["bucket"].ToString() ?? "default";
            
            if (file == null || file.Length == 0)
            {
                await res.AsJson(new { Error = "No file uploaded" }, statusCode: 400);
                return;
            }

            using var stream = file.OpenReadStream();
            await _minioService.MultiThreadUploadAsync(bucket, file.FileName, stream);
            
            await res.AsJson(new { file.FileName, file.Length });
        });

        // 流式上传
        Post("/minio/stream", async (req, res) => 
        {
            var file = req.Form.Files.GetFile("file");
            var bucket = req.Query["bucket"].ToString() ?? "default";
            
            if (file == null || file.Length == 0)
            {
                await res.AsJson(new { Error = "No file uploaded" }, statusCode: 400);
                return;
            }

            var pipe = new Pipe();
            var writingTask = WriteToPipeAsync(file, pipe.Writer);
            var uploadingTask = _minioService.StreamUploadAsync(bucket, file.FileName, pipe.Reader);

            await Task.WhenAll(writingTask, uploadingTask);
            await res.AsJson(new { file.FileName, file.Length });
        });

        // 批量上传
        Post("/minio/batch", async (req, res) => 
        {
            var files = req.Form.Files;
            var bucket = req.Query["bucket"].ToString() ?? "default";
            
            if (files == null || !files.Any())
            {
                await res.AsJson(new { Error = "No files uploaded" }, statusCode: 400);
                return;
            }

            var tasks = files.Select(file => 
            {
                using var stream = file.OpenReadStream();
                return _minioService.MultiThreadUploadAsync(bucket, file.FileName, stream);
            });

            await Task.WhenAll(tasks);
            await res.AsJson(new { Count = files.Count });
        });

        // 带进度回调的分块上传
        Post("/minio/multipart-with-progress", async (req, res) => 
        {
            var file = req.Form.Files.GetFile("file");
            var bucket = req.Query["bucket"].ToString() ?? "default";
            var hubContext = req.HttpContext.RequestServices.GetService<IHubContext<UploadProgressHub>>();
            
            if (file == null || file.Length == 0)
            {
                await res.AsJson(new { Error = "No file uploaded" }, statusCode: 400);
                return;
            }

            var uploadId = Guid.NewGuid().ToString();
            using var stream = file.OpenReadStream();
            
            var progressChannel = Channel.CreateBounded<UploadProgress>(new BoundedChannelOptions(1000)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });

            // 启动后台进度推送任务
            _ = Task.Run(async () => 
            {
                await foreach (var progress in progressChannel.Reader.ReadAllAsync())
                {
                    await hubContext.Clients.Group(uploadId)
                        .SendAsync("ProgressUpdate", new 
                        {
                            UploadId = uploadId,
                            progress.BytesTransferred,
                            progress.Percentage
                        });
                }
            });

            await _minioService.MultiThreadUploadWithProgressAsync(
                bucket, 
                file.FileName, 
                stream,
                progressChannel.Writer);

            await res.AsJson(new { UploadId = uploadId, file.FileName });
        });

        // 下载接口
        Get("/minio/download", async (req, res) => 
        {
            var objectName = req.Query["objectName"].ToString();
            var bucket = req.Query["bucket"].ToString() ?? "default";
            var useCache = bool.TryParse(req.Query["useCache"], out var uc) && uc;

            try
            {
                var result = await _minioService.DownloadFileAsync(bucket, objectName, useCache);
                await res.FromStream(result, "application/octet-stream", objectName);
            }
            catch (Exception ex)
            {
                await res.AsJson(new { Error = ex.Message }, statusCode: 400);
            }
        });

        // 批量删除接口
        Delete("/minio/batch", async (req, res) => 
        {
            var request = await req.BindJson<BatchDeleteRequest>();
            var bucket = req.Query["bucket"].ToString() ?? "default";

            var tasks = request.ObjectKeys.Select(key => 
                _minioService.DeleteObjectAsync(bucket, key));

            await Task.WhenAll(tasks);
            await res.AsJson(new { DeletedCount = request.ObjectKeys.Count() });
        });
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

// DTOs
public record UploadProgress(long BytesTransferred, double Percentage);
public record BatchDeleteRequest(IEnumerable<string> ObjectKeys);
#:sdk Microsoft.NET.Sdk
#:package Microsoft.ML.OnnxRuntime@1.15.1
#:package Microsoft.ML.OnnxRuntime.Gpu@1.15.1
#:package SixLabors.ImageSharp@2.1.3
#:property LangVersion preview
#:property TargetFramework net10.0

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

public class DetectionResult
{
    public string Label { get; set; }
    public float Confidence { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

public interface IObjectDetectionService
{
    Task<IReadOnlyList<DetectionResult>> DetectAsync(byte[] image);
    Task<IReadOnlyList<DetectionResult>> DetectAsync(string imagePath);
}

public class YoloV7DetectionService : IObjectDetectionService
{
    private readonly InferenceSession _session;
    private readonly ObjectPool<Image<Rgb24>> _imagePool;
    private readonly string[] _labels;
    
    public YoloV7DetectionService(
        InferenceSession session, 
        ObjectPool<Image<Rgb24>> imagePool,
        string[] labels)
    {
        _session = session;
        _imagePool = imagePool;
        _labels = labels;
    }

    public async Task<IReadOnlyList<DetectionResult>> DetectAsync(byte[] image)
    {
        var img = _imagePool.Get();
        try
        {
            using (var stream = new MemoryStream(image))
            {
                img = await Image.LoadAsync<Rgb24>(stream);
                return await ProcessImage(img);
            }
        }
        finally
        {
            _imagePool.Return(img);
        }
    }

    public async Task<IReadOnlyList<DetectionResult>> DetectAsync(string imagePath)
    {
        var img = _imagePool.Get();
        try
        {
            img = await Image.LoadAsync<Rgb24>(imagePath);
            return await ProcessImage(img);
        }
        finally
        {
            _imagePool.Return(img);
        }
    }

    private async Task<IReadOnlyList<DetectionResult>> ProcessImage(Image<Rgb24> image)
    {
        // Preprocess
        var resized = image.Clone(x => x.Resize(new Size(640, 640)));
        
        // Create tensor
        var input = new DenseTensor<float>(new[] { 1, 3, 640, 640 });
        resized.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                for (int x = 0; x < accessor.Width; x++)
                {
                    input[0, 0, y, x] = pixelSpan[x].R / 255.0f;
                    input[0, 1, y, x] = pixelSpan[x].G / 255.0f;
                    input[0, 2, y, x] = pixelSpan[x].B / 255.0f;
                }
            }
        });

        // Run inference
        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("images", input)
        };
        
        using (var results = _session.Run(inputs))
        {
            var output = results.First().AsTensor<float>();
            return ParseOutput(output, image.Width, image.Height);
        }
    }

    private IReadOnlyList<DetectionResult> ParseOutput(Tensor<float> output, int originalWidth, int originalHeight)
    {
        var results = new List<DetectionResult>();
        
        for (int i = 0; i < output.Dimensions[1]; i++)
        {
            if (output[0, i, 4] < 0.5f) continue;
            
            var x = output[0, i, 0];
            var y = output[0, i, 1];
            var w = output[0, i, 2];
            var h = output[0, i, 3];
            
            // Scale to original image size
            x *= originalWidth;
            y *= originalHeight;
            w *= originalWidth;
            h *= originalHeight;
            
            // Find class with max confidence
            int classId = 0;
            float maxConf = 0;
            for (int j = 5; j < output.Dimensions[2]; j++)
            {
                if (output[0, i, j] > maxConf)
                {
                    maxConf = output[0, i, j];
                    classId = j - 5;
                }
            }
            
            results.Add(new DetectionResult
            {
                Label = _labels[classId],
                Confidence = maxConf,
                X = x,
                Y = y,
                Width = w,
                Height = h
            });
        }
        
        return results;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddObjectDetectionServices(
        this IServiceCollection services,
        string modelPath,
        string[] labels)
    {
        // Configure session options for optimal performance
        var sessionOptions = new SessionOptions
        {
            EnableCpuMemArena = true,
            EnableMemoryPattern = true,
            ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
        };
        
        // Use CUDA if available
        try
        {
            sessionOptions.AppendExecutionProvider_CUDA();
        }
        catch { }
        
        services.AddSingleton<InferenceSession>(sp => new InferenceSession(modelPath, sessionOptions));
        
        services.AddSingleton<ObjectPool<Image<Rgb24>>>(sp => 
        {
            var policy = new DefaultPooledObjectPolicy<Image<Rgb24>>();
            return new DefaultObjectPool<Image<Rgb24>>(policy, Environment.ProcessorCount * 2);
        });
        
        services.AddSingleton<string[]>(labels);
        services.AddSingleton<IObjectDetectionService, YoloV7DetectionService>();
        
        return services;
    }
}
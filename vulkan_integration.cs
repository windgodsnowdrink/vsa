#:sdk Microsoft.NET.Sdk.Web
#:package Vulkan.Core@1.3.275
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using System.Threading.Tasks;
using Vulkan;
using Vulkan.Khronos;
using Microsoft.Extensions.ObjectPool;

public class VulkanContext : IDisposable
{
    private Instance _instance;
    private Device _device;
    private PhysicalDevice _physicalDevice;
    private CommandPool _commandPool;
    private Channel<Action<Vk>> _commandChannel;
    private ObjectPool<CommandBuffer> _commandBufferPool;
    private ConcurrentDictionary<int, Fence> _fenceCache;
    private Stopwatch _frameTimer;
    private long _lastFrameTime;
    private BufferMemoryPool _memoryPool;
    
    public VulkanContext()
    {
        _commandChannel = Channel.CreateUnbounded<Action<Vk>>();
        _fenceCache = new ConcurrentDictionary<int, Fence>();
        _frameTimer = Stopwatch.StartNew();
        
        var poolPolicy = new DefaultPooledObjectPolicy<CommandBuffer>(() => 
            _device.AllocateCommandBuffers(new CommandBufferAllocateInfo
            {
                CommandPool = _commandPool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1
            })[0]);
            
        _commandBufferPool = new DefaultObjectPool<CommandBuffer>(poolPolicy);
        _memoryPool = new BufferMemoryPool(_device, _physicalDevice);
        
        InitializeVulkan();
        StartCommandProcessor();
        StartPerformanceMonitor();
    }
    
    private void InitializeVulkan()
    {
        var appInfo = new ApplicationInfo
        {
            ApplicationName = "VulkanIntegration",
            ApplicationVersion = new Version(1, 0, 0),
            EngineName = "NoEngine",
            EngineVersion = new Version(1, 0, 0),
            ApiVersion = Vk.Version_1_0
        };
        
        _instance = Vk.CreateInstance(new InstanceCreateInfo
        {
            ApplicationInfo = appInfo
        });
        
        _physicalDevice = _instance.EnumeratePhysicalDevices()[0];
        
        _device = _physicalDevice.CreateDevice(new DeviceCreateInfo
        {
            QueueCreateInfos = new[]
            {
                new DeviceQueueCreateInfo
                {
                    QueueFamilyIndex = 0,
                    QueuePriorities = new[] { 1.0f }
                }
            }
        });
        
        _commandPool = _device.CreateCommandPool(new CommandPoolCreateInfo
        {
            QueueFamilyIndex = 0
        });
    }
    
    private void StartCommandProcessor()
    {
        Task.Run(async () =>
        {
            await foreach (var command in _commandChannel.Reader.ReadAllAsync())
            {
                command.Invoke(Vk.GetApi());
            }
        });
    }
    
    public void SubmitCommand(Action<Vk> command)
    {
        _commandChannel.Writer.TryWrite(command);
    }
    
    public void Dispose()
    {
        _commandChannel.Writer.Complete();
        foreach (var fence in _fenceCache.Values)
            fence.Dispose();
        _memoryPool.Dispose();
        _device?.Dispose();
        _instance?.Dispose();
    }
    
    private void StartPerformanceMonitor()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(1000);
                var currentTime = _frameTimer.ElapsedMilliseconds;
                var frameTime = currentTime - _lastFrameTime;
                _lastFrameTime = currentTime;
                Console.WriteLine($"Frame time: {frameTime}ms");
            }
        });
    }
    
    public CommandBuffer RentCommandBuffer() => _commandBufferPool.Get();
    
    public void ReturnCommandBuffer(CommandBuffer buffer) => _commandBufferPool.Return(buffer);
    
    public Fence GetOrCreateFence(int id)
    {
        return _fenceCache.GetOrAdd(id, _ => _device.CreateFence(new FenceCreateInfo()));
    }
    
    public BufferMemory AllocateBufferMemory(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties)
    {
        return _memoryPool.Allocate(size, usage, properties);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVulkanIntegration(this IServiceCollection services)
    {
        return services.AddSingleton<VulkanContext>();
    }
}
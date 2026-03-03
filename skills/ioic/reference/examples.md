# ioic - Usage Examples

## Quick Start

### 1. Basic Usage Example

`csharp
using System;
using ioic;

public class Program
{
    public static async Task Main()
    {
        // Initialize services
        var serviceProvider = BuildServiceProvider();
        var  = serviceProvider.GetRequiredService<>();
        
        Console.WriteLine("ioic Basic Usage Example");
        Console.WriteLine("=" * 50);
        
        // Use ioic functionality
        var result = await .DoSomethingAsync();
        Console.WriteLine($"Result: {result}");
        
        // Other operations...
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<>();
        builder.AddSingleton<I, >();
        return builder.BuildServiceProvider();
    }
}
`

### 2. Advanced Configuration Example

`csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ioic Advanced Configuration Example");
        Console.WriteLine("=" * 50);
        
        // Build service container
        var builder = new ServiceCollection();
        
        // Configure ioic settings
        builder.Configure<>(options => {
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
        });
        
        // Register services
        builder.AddSingleton<I, >();
        builder.AddSingleton<>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // Get configuration
        var settings = serviceProvider.GetRequiredService<IOptions<>>().Value;
        Console.WriteLine($"Configuration: Cache={settings.EnableCache}, Size={settings.CacheSize}");
        
        // Use service
        var  = serviceProvider.GetRequiredService<>();
        var result = await .DoSomethingAsync();
        Console.WriteLine($"Result: {result}");
    }
}
`

### 3. Performance Optimization Example

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ioic Performance Optimization Example");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var  = serviceProvider.GetRequiredService<>();
        
        // Performance test
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            await .DoSomethingAsync();
        }
        
        stopwatch.Stop();
        Console.WriteLine($"Execution time for {iterations} iterations: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"Average per iteration: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
}
`

### 4. Error Handling Example

`csharp
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ioic Error Handling Example");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var  = serviceProvider.GetRequiredService<>();
        
        try
        {
            var result = await .DoSomethingAsync();
            Console.WriteLine($"Success: {result}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Timeout Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
        }
    }
}
`

## Summary

The above examples demonstrate the main functions and usage methods of the ioic skill. Through these examples, you can:

1. Get started with basic operations quickly
2. Configure advanced options
3. Optimize performance
4. Handle error situations

The system design follows .NET 10 best practices, with good scalability and maintainability, suitable for projects of various sizes and complexity.

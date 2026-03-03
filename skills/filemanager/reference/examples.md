# FileManager - 使用示例

## 快速开始

### 1. 基本使用示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FileManager.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var host = CreateHostBuilder().Build();
        var fileManagerService = host.Services.GetRequiredService<IFileManagerService>();
        
        Console.WriteLine("FileManager 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 FileManager 功能
        Console.WriteLine("1. 创建文件");
        var createResult = await fileManagerService.CreateFileAsync("test.txt", "Hello World from FileManager");
        Console.WriteLine($"创建文件结果: {(createResult.Success ? "成功" : "失败")}");
        if (createResult.Success)
        {
            Console.WriteLine($"执行时间: {createResult.ExecutionTimeMs} ms");
            foreach (var item in createResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
        
        Console.WriteLine("\n2. 读取文件");
        var readResult = await fileManagerService.ReadFileAsync("test.txt");
        Console.WriteLine($"读取文件结果: {(readResult.Success ? "成功" : "失败")}");
        if (readResult.Success)
        {
            Console.WriteLine($"执行时间: {readResult.ExecutionTimeMs} ms");
            foreach (var item in readResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
        
        Console.WriteLine("\n3. 写入文件");
        var writeResult = await fileManagerService.WriteFileAsync("test.txt", "Updated content from FileManager");
        Console.WriteLine($"写入文件结果: {(writeResult.Success ? "成功" : "失败")}");
        if (writeResult.Success)
        {
            Console.WriteLine($"执行时间: {writeResult.ExecutionTimeMs} ms");
            foreach (var item in writeResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
        
        Console.WriteLine("\n4. 列出文件");
        var listResult = await fileManagerService.ListFilesAsync(".");
        Console.WriteLine($"列出文件结果: {(listResult.Success ? "成功" : "失败")}");
        if (listResult.Success)
        {
            Console.WriteLine($"执行时间: {listResult.ExecutionTimeMs} ms");
            foreach (var item in listResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
            if (listResult.Files != null && listResult.Files.Any())
            {
                Console.WriteLine("\n文件列表:");
                foreach (var file in listResult.Files)
                {
                    Console.WriteLine($"  {file.Name} {(file.IsDirectory ? "[目录]" : $"[{file.Size} 字节]

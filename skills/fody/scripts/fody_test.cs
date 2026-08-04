// 获取 Fody 服务
var fodyService = serviceProvider.GetRequiredService<IFodyService>();

// 列出可用的织入器
var weaversResult = await fodyService.ListWeaversAsync();
Console.WriteLine($"获取织入器结果: {(weaversResult.Success ? "成功" : "失败")}");
foreach (var item in weaversResult.Results)
{
    Console.WriteLine($"- {item}");
}

// 织入程序集
var weaveResult = await fodyService.WeaveAssemblyAsync("MyAssembly.dll", "Output.dll");
Console.WriteLine($"织入结果: {(weaveResult.Success ? "成功" : "失败")}");
Console.WriteLine($"执行时间: {weaveResult.ExecutionTimeMs} ms");
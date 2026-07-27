#load "vulkan_integration.cs"

Console.WriteLine("=== vulkan_integration.cs Test ===");

try
{
    // 验证 class: VulkanContext
    var type_VulkanContext = Type.GetType("VulkanContext");
    if (type_VulkanContext != null)
    {
        Console.WriteLine("[PASS] 类型 VulkanContext (class) 存在");
        var ctors_VulkanContext = type_VulkanContext.GetConstructors();
        Console.WriteLine($"[PASS] VulkanContext 构造函数数量: {ctors_VulkanContext.Length}");
        var methods_VulkanContext = type_VulkanContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VulkanContext 公开方法数量: {methods_VulkanContext.Length}");
        foreach (var m in methods_VulkanContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VulkanContext 未找到，尝试无命名空间...");
        type_VulkanContext = Type.GetType("VulkanContext");
        if (type_VulkanContext != null)
            Console.WriteLine("[PASS] 类型 VulkanContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VulkanContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

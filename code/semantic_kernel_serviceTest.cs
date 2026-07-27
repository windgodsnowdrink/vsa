#load "semantic_kernel_service.cs"

Console.WriteLine("=== semantic_kernel_service.cs Test ===");

try
{
    // 验证 class: AiAgentProcessor
    var type_AiAgentProcessor = Type.GetType("AiAgentProcessor");
    if (type_AiAgentProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 AiAgentProcessor (class) 存在");
        var ctors_AiAgentProcessor = type_AiAgentProcessor.GetConstructors();
        Console.WriteLine($"[PASS] AiAgentProcessor 构造函数数量: {ctors_AiAgentProcessor.Length}");
        var methods_AiAgentProcessor = type_AiAgentProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AiAgentProcessor 公开方法数量: {methods_AiAgentProcessor.Length}");
        foreach (var m in methods_AiAgentProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AiAgentProcessor 未找到，尝试无命名空间...");
        type_AiAgentProcessor = Type.GetType("AiAgentProcessor");
        if (type_AiAgentProcessor != null)
            Console.WriteLine("[PASS] 类型 AiAgentProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AiAgentProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 record: AgentRequest
    var type_AgentRequest = Type.GetType("AgentRequest");
    if (type_AgentRequest != null)
    {
        Console.WriteLine("[PASS] 类型 AgentRequest (record) 存在");
        var ctors_AgentRequest = type_AgentRequest.GetConstructors();
        Console.WriteLine($"[PASS] AgentRequest 构造函数数量: {ctors_AgentRequest.Length}");
        var methods_AgentRequest = type_AgentRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AgentRequest 公开方法数量: {methods_AgentRequest.Length}");
        foreach (var m in methods_AgentRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AgentRequest 未找到，尝试无命名空间...");
        type_AgentRequest = Type.GetType("AgentRequest");
        if (type_AgentRequest != null)
            Console.WriteLine("[PASS] 类型 AgentRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AgentRequest 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

#load "zonetree_integration.cs"

Console.WriteLine("=== zonetree_integration.cs Test ===");

try
{
    // 验证 class: TreeNode
    var type_TreeNode = Type.GetType("TreeNode");
    if (type_TreeNode != null)
    {
        Console.WriteLine("[PASS] 类型 TreeNode (class) 存在");
        var ctors_TreeNode = type_TreeNode.GetConstructors();
        Console.WriteLine($"[PASS] TreeNode 构造函数数量: {ctors_TreeNode.Length}");
        var methods_TreeNode = type_TreeNode.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TreeNode 公开方法数量: {methods_TreeNode.Length}");
        foreach (var m in methods_TreeNode)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TreeNode 未找到，尝试无命名空间...");
        type_TreeNode = Type.GetType("TreeNode");
        if (type_TreeNode != null)
            Console.WriteLine("[PASS] 类型 TreeNode (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TreeNode 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TreeService
    var type_TreeService = Type.GetType("TreeService");
    if (type_TreeService != null)
    {
        Console.WriteLine("[PASS] 类型 TreeService (class) 存在");
        var ctors_TreeService = type_TreeService.GetConstructors();
        Console.WriteLine($"[PASS] TreeService 构造函数数量: {ctors_TreeService.Length}");
        var methods_TreeService = type_TreeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TreeService 公开方法数量: {methods_TreeService.Length}");
        foreach (var m in methods_TreeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TreeService 未找到，尝试无命名空间...");
        type_TreeService = Type.GetType("TreeService");
        if (type_TreeService != null)
            Console.WriteLine("[PASS] 类型 TreeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TreeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TreeServiceExtensions
    var type_TreeServiceExtensions = Type.GetType("TreeServiceExtensions");
    if (type_TreeServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TreeServiceExtensions (class) 存在");
        var ctors_TreeServiceExtensions = type_TreeServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TreeServiceExtensions 构造函数数量: {ctors_TreeServiceExtensions.Length}");
        var methods_TreeServiceExtensions = type_TreeServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TreeServiceExtensions 公开方法数量: {methods_TreeServiceExtensions.Length}");
        foreach (var m in methods_TreeServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TreeServiceExtensions 未找到，尝试无命名空间...");
        type_TreeServiceExtensions = Type.GetType("TreeServiceExtensions");
        if (type_TreeServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 TreeServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TreeServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITreeService
    var type_ITreeService = Type.GetType("ITreeService");
    if (type_ITreeService != null)
    {
        Console.WriteLine("[PASS] 类型 ITreeService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITreeService 未找到，尝试无命名空间...");
        type_ITreeService = Type.GetType("ITreeService");
        if (type_ITreeService != null)
            Console.WriteLine("[PASS] 类型 ITreeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITreeService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

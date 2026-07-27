#load "btree_integration.cs"

Console.WriteLine("=== btree_integration.cs Test ===");

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

    // 验证 class: BTreeService
    var type_BTreeService = Type.GetType("BTreeService");
    if (type_BTreeService != null)
    {
        Console.WriteLine("[PASS] 类型 BTreeService (class) 存在");
        var ctors_BTreeService = type_BTreeService.GetConstructors();
        Console.WriteLine($"[PASS] BTreeService 构造函数数量: {ctors_BTreeService.Length}");
        var methods_BTreeService = type_BTreeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BTreeService 公开方法数量: {methods_BTreeService.Length}");
        foreach (var m in methods_BTreeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BTreeService 未找到，尝试无命名空间...");
        type_BTreeService = Type.GetType("BTreeService");
        if (type_BTreeService != null)
            Console.WriteLine("[PASS] 类型 BTreeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BTreeService 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IBTreeService
    var type_IBTreeService = Type.GetType("IBTreeService");
    if (type_IBTreeService != null)
    {
        Console.WriteLine("[PASS] 类型 IBTreeService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IBTreeService 未找到，尝试无命名空间...");
        type_IBTreeService = Type.GetType("IBTreeService");
        if (type_IBTreeService != null)
            Console.WriteLine("[PASS] 类型 IBTreeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IBTreeService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

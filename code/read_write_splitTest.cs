#load "read_write_split.cs"

Console.WriteLine("=== read_write_split.cs Test ===");

try
{
    // 验证 class: ReadOnly
    var type_ReadOnly = Type.GetType("ReadOnly");
    if (type_ReadOnly != null)
    {
        Console.WriteLine("[PASS] 类型 ReadOnly (class) 存在");
        var ctors_ReadOnly = type_ReadOnly.GetConstructors();
        Console.WriteLine($"[PASS] ReadOnly 构造函数数量: {ctors_ReadOnly.Length}");
        var methods_ReadOnly = type_ReadOnly.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReadOnly 公开方法数量: {methods_ReadOnly.Length}");
        foreach (var m in methods_ReadOnly)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReadOnly 未找到，尝试无命名空间...");
        type_ReadOnly = Type.GetType("ReadOnly");
        if (type_ReadOnly != null)
            Console.WriteLine("[PASS] 类型 ReadOnly (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReadOnly 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ReadWriteDbContext
    var type_ReadWriteDbContext = Type.GetType("ReadWriteDbContext");
    if (type_ReadWriteDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 ReadWriteDbContext (class) 存在");
        var ctors_ReadWriteDbContext = type_ReadWriteDbContext.GetConstructors();
        Console.WriteLine($"[PASS] ReadWriteDbContext 构造函数数量: {ctors_ReadWriteDbContext.Length}");
        var methods_ReadWriteDbContext = type_ReadWriteDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReadWriteDbContext 公开方法数量: {methods_ReadWriteDbContext.Length}");
        foreach (var m in methods_ReadWriteDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReadWriteDbContext 未找到，尝试无命名空间...");
        type_ReadWriteDbContext = Type.GetType("ReadWriteDbContext");
        if (type_ReadWriteDbContext != null)
            Console.WriteLine("[PASS] 类型 ReadWriteDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReadWriteDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ReadOnlyDbContext
    var type_ReadOnlyDbContext = Type.GetType("ReadOnlyDbContext");
    if (type_ReadOnlyDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 ReadOnlyDbContext (class) 存在");
        var ctors_ReadOnlyDbContext = type_ReadOnlyDbContext.GetConstructors();
        Console.WriteLine($"[PASS] ReadOnlyDbContext 构造函数数量: {ctors_ReadOnlyDbContext.Length}");
        var methods_ReadOnlyDbContext = type_ReadOnlyDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReadOnlyDbContext 公开方法数量: {methods_ReadOnlyDbContext.Length}");
        foreach (var m in methods_ReadOnlyDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReadOnlyDbContext 未找到，尝试无命名空间...");
        type_ReadOnlyDbContext = Type.GetType("ReadOnlyDbContext");
        if (type_ReadOnlyDbContext != null)
            Console.WriteLine("[PASS] 类型 ReadOnlyDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReadOnlyDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DapperReadOnlyRepository
    var type_DapperReadOnlyRepository = Type.GetType("DapperReadOnlyRepository");
    if (type_DapperReadOnlyRepository != null)
    {
        Console.WriteLine("[PASS] 类型 DapperReadOnlyRepository (class) 存在");
        var ctors_DapperReadOnlyRepository = type_DapperReadOnlyRepository.GetConstructors();
        Console.WriteLine($"[PASS] DapperReadOnlyRepository 构造函数数量: {ctors_DapperReadOnlyRepository.Length}");
        var methods_DapperReadOnlyRepository = type_DapperReadOnlyRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DapperReadOnlyRepository 公开方法数量: {methods_DapperReadOnlyRepository.Length}");
        foreach (var m in methods_DapperReadOnlyRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DapperReadOnlyRepository 未找到，尝试无命名空间...");
        type_DapperReadOnlyRepository = Type.GetType("DapperReadOnlyRepository");
        if (type_DapperReadOnlyRepository != null)
            Console.WriteLine("[PASS] 类型 DapperReadOnlyRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DapperReadOnlyRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IReadOnly
    var type_IReadOnly = Type.GetType("IReadOnly");
    if (type_IReadOnly != null)
    {
        Console.WriteLine("[PASS] 类型 IReadOnly (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IReadOnly 未找到，尝试无命名空间...");
        type_IReadOnly = Type.GetType("IReadOnly");
        if (type_IReadOnly != null)
            Console.WriteLine("[PASS] 类型 IReadOnly (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IReadOnly 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

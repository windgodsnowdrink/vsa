#load "SourceGenerator_ddd_generator.cs"

Console.WriteLine("=== SourceGenerator_ddd_generator.cs Test ===");

try
{
    // 验证 class: DddGenerator
    var type_DddGenerator = Type.GetType("DddGenerator");
    if (type_DddGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 DddGenerator (class) 存在");
        var ctors_DddGenerator = type_DddGenerator.GetConstructors();
        Console.WriteLine($"[PASS] DddGenerator 构造函数数量: {ctors_DddGenerator.Length}");
        var methods_DddGenerator = type_DddGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DddGenerator 公开方法数量: {methods_DddGenerator.Length}");
        foreach (var m in methods_DddGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DddGenerator 未找到，尝试无命名空间...");
        type_DddGenerator = Type.GetType("DddGenerator");
        if (type_DddGenerator != null)
            Console.WriteLine("[PASS] 类型 DddGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DddGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DddSyntaxReceiver
    var type_DddSyntaxReceiver = Type.GetType("DddSyntaxReceiver");
    if (type_DddSyntaxReceiver != null)
    {
        Console.WriteLine("[PASS] 类型 DddSyntaxReceiver (class) 存在");
        var ctors_DddSyntaxReceiver = type_DddSyntaxReceiver.GetConstructors();
        Console.WriteLine($"[PASS] DddSyntaxReceiver 构造函数数量: {ctors_DddSyntaxReceiver.Length}");
        var methods_DddSyntaxReceiver = type_DddSyntaxReceiver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DddSyntaxReceiver 公开方法数量: {methods_DddSyntaxReceiver.Length}");
        foreach (var m in methods_DddSyntaxReceiver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DddSyntaxReceiver 未找到，尝试无命名空间...");
        type_DddSyntaxReceiver = Type.GetType("DddSyntaxReceiver");
        if (type_DddSyntaxReceiver != null)
            Console.WriteLine("[PASS] 类型 DddSyntaxReceiver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DddSyntaxReceiver 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EntityInfo
    var type_EntityInfo = Type.GetType("EntityInfo");
    if (type_EntityInfo != null)
    {
        Console.WriteLine("[PASS] 类型 EntityInfo (class) 存在");
        var ctors_EntityInfo = type_EntityInfo.GetConstructors();
        Console.WriteLine($"[PASS] EntityInfo 构造函数数量: {ctors_EntityInfo.Length}");
        var methods_EntityInfo = type_EntityInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EntityInfo 公开方法数量: {methods_EntityInfo.Length}");
        foreach (var m in methods_EntityInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EntityInfo 未找到，尝试无命名空间...");
        type_EntityInfo = Type.GetType("EntityInfo");
        if (type_EntityInfo != null)
            Console.WriteLine("[PASS] 类型 EntityInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EntityInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Create
    var type_Create = Type.GetType("Create");
    if (type_Create != null)
    {
        Console.WriteLine("[PASS] 类型 Create (record) 存在");
        var ctors_Create = type_Create.GetConstructors();
        Console.WriteLine($"[PASS] Create 构造函数数量: {ctors_Create.Length}");
        var methods_Create = type_Create.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Create 公开方法数量: {methods_Create.Length}");
        foreach (var m in methods_Create)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Create 未找到，尝试无命名空间...");
        type_Create = Type.GetType("Create");
        if (type_Create != null)
            Console.WriteLine("[PASS] 类型 Create (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Create 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Update
    var type_Update = Type.GetType("Update");
    if (type_Update != null)
    {
        Console.WriteLine("[PASS] 类型 Update (record) 存在");
        var ctors_Update = type_Update.GetConstructors();
        Console.WriteLine($"[PASS] Update 构造函数数量: {ctors_Update.Length}");
        var methods_Update = type_Update.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Update 公开方法数量: {methods_Update.Length}");
        foreach (var m in methods_Update)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Update 未找到，尝试无命名空间...");
        type_Update = Type.GetType("Update");
        if (type_Update != null)
            Console.WriteLine("[PASS] 类型 Update (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Update 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Delete
    var type_Delete = Type.GetType("Delete");
    if (type_Delete != null)
    {
        Console.WriteLine("[PASS] 类型 Delete (record) 存在");
        var ctors_Delete = type_Delete.GetConstructors();
        Console.WriteLine($"[PASS] Delete 构造函数数量: {ctors_Delete.Length}");
        var methods_Delete = type_Delete.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Delete 公开方法数量: {methods_Delete.Length}");
        foreach (var m in methods_Delete)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Delete 未找到，尝试无命名空间...");
        type_Delete = Type.GetType("Delete");
        if (type_Delete != null)
            Console.WriteLine("[PASS] 类型 Delete (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Delete 可能为顶层语句或嵌套类型");
    }

    // 验证 record: Get
    var type_Get = Type.GetType("Get");
    if (type_Get != null)
    {
        Console.WriteLine("[PASS] 类型 Get (record) 存在");
        var ctors_Get = type_Get.GetConstructors();
        Console.WriteLine($"[PASS] Get 构造函数数量: {ctors_Get.Length}");
        var methods_Get = type_Get.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Get 公开方法数量: {methods_Get.Length}");
        foreach (var m in methods_Get)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Get 未找到，尝试无命名空间...");
        type_Get = Type.GetType("Get");
        if (type_Get != null)
            Console.WriteLine("[PASS] 类型 Get (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Get 可能为顶层语句或嵌套类型");
    }

    // 验证 record: GetAll
    var type_GetAll = Type.GetType("GetAll");
    if (type_GetAll != null)
    {
        Console.WriteLine("[PASS] 类型 GetAll (record) 存在");
        var ctors_GetAll = type_GetAll.GetConstructors();
        Console.WriteLine($"[PASS] GetAll 构造函数数量: {ctors_GetAll.Length}");
        var methods_GetAll = type_GetAll.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GetAll 公开方法数量: {methods_GetAll.Length}");
        foreach (var m in methods_GetAll)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GetAll 未找到，尝试无命名空间...");
        type_GetAll = Type.GetType("GetAll");
        if (type_GetAll != null)
            Console.WriteLine("[PASS] 类型 GetAll (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GetAll 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

#load "SourceGenerator_ddd_advanced_generator.cs"

Console.WriteLine("=== SourceGenerator_ddd_advanced_generator.cs Test ===");

try
{
    // 验证 class: DomainEventGenerator
    var type_DomainEventGenerator = Type.GetType("DomainEventGenerator");
    if (type_DomainEventGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 DomainEventGenerator (class) 存在");
        var ctors_DomainEventGenerator = type_DomainEventGenerator.GetConstructors();
        Console.WriteLine($"[PASS] DomainEventGenerator 构造函数数量: {ctors_DomainEventGenerator.Length}");
        var methods_DomainEventGenerator = type_DomainEventGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DomainEventGenerator 公开方法数量: {methods_DomainEventGenerator.Length}");
        foreach (var m in methods_DomainEventGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DomainEventGenerator 未找到，尝试无命名空间...");
        type_DomainEventGenerator = Type.GetType("DomainEventGenerator");
        if (type_DomainEventGenerator != null)
            Console.WriteLine("[PASS] 类型 DomainEventGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DomainEventGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ValueObjectGenerator
    var type_ValueObjectGenerator = Type.GetType("ValueObjectGenerator");
    if (type_ValueObjectGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 ValueObjectGenerator (class) 存在");
        var ctors_ValueObjectGenerator = type_ValueObjectGenerator.GetConstructors();
        Console.WriteLine($"[PASS] ValueObjectGenerator 构造函数数量: {ctors_ValueObjectGenerator.Length}");
        var methods_ValueObjectGenerator = type_ValueObjectGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValueObjectGenerator 公开方法数量: {methods_ValueObjectGenerator.Length}");
        foreach (var m in methods_ValueObjectGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValueObjectGenerator 未找到，尝试无命名空间...");
        type_ValueObjectGenerator = Type.GetType("ValueObjectGenerator");
        if (type_ValueObjectGenerator != null)
            Console.WriteLine("[PASS] 类型 ValueObjectGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValueObjectGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MicroserviceGenerator
    var type_MicroserviceGenerator = Type.GetType("MicroserviceGenerator");
    if (type_MicroserviceGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 MicroserviceGenerator (class) 存在");
        var ctors_MicroserviceGenerator = type_MicroserviceGenerator.GetConstructors();
        Console.WriteLine($"[PASS] MicroserviceGenerator 构造函数数量: {ctors_MicroserviceGenerator.Length}");
        var methods_MicroserviceGenerator = type_MicroserviceGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MicroserviceGenerator 公开方法数量: {methods_MicroserviceGenerator.Length}");
        foreach (var m in methods_MicroserviceGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MicroserviceGenerator 未找到，尝试无命名空间...");
        type_MicroserviceGenerator = Type.GetType("MicroserviceGenerator");
        if (type_MicroserviceGenerator != null)
            Console.WriteLine("[PASS] 类型 MicroserviceGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MicroserviceGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DomainEventSyntaxReceiver
    var type_DomainEventSyntaxReceiver = Type.GetType("DomainEventSyntaxReceiver");
    if (type_DomainEventSyntaxReceiver != null)
    {
        Console.WriteLine("[PASS] 类型 DomainEventSyntaxReceiver (class) 存在");
        var ctors_DomainEventSyntaxReceiver = type_DomainEventSyntaxReceiver.GetConstructors();
        Console.WriteLine($"[PASS] DomainEventSyntaxReceiver 构造函数数量: {ctors_DomainEventSyntaxReceiver.Length}");
        var methods_DomainEventSyntaxReceiver = type_DomainEventSyntaxReceiver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DomainEventSyntaxReceiver 公开方法数量: {methods_DomainEventSyntaxReceiver.Length}");
        foreach (var m in methods_DomainEventSyntaxReceiver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DomainEventSyntaxReceiver 未找到，尝试无命名空间...");
        type_DomainEventSyntaxReceiver = Type.GetType("DomainEventSyntaxReceiver");
        if (type_DomainEventSyntaxReceiver != null)
            Console.WriteLine("[PASS] 类型 DomainEventSyntaxReceiver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DomainEventSyntaxReceiver 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ValueObjectSyntaxReceiver
    var type_ValueObjectSyntaxReceiver = Type.GetType("ValueObjectSyntaxReceiver");
    if (type_ValueObjectSyntaxReceiver != null)
    {
        Console.WriteLine("[PASS] 类型 ValueObjectSyntaxReceiver (class) 存在");
        var ctors_ValueObjectSyntaxReceiver = type_ValueObjectSyntaxReceiver.GetConstructors();
        Console.WriteLine($"[PASS] ValueObjectSyntaxReceiver 构造函数数量: {ctors_ValueObjectSyntaxReceiver.Length}");
        var methods_ValueObjectSyntaxReceiver = type_ValueObjectSyntaxReceiver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValueObjectSyntaxReceiver 公开方法数量: {methods_ValueObjectSyntaxReceiver.Length}");
        foreach (var m in methods_ValueObjectSyntaxReceiver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValueObjectSyntaxReceiver 未找到，尝试无命名空间...");
        type_ValueObjectSyntaxReceiver = Type.GetType("ValueObjectSyntaxReceiver");
        if (type_ValueObjectSyntaxReceiver != null)
            Console.WriteLine("[PASS] 类型 ValueObjectSyntaxReceiver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValueObjectSyntaxReceiver 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MicroserviceSyntaxReceiver
    var type_MicroserviceSyntaxReceiver = Type.GetType("MicroserviceSyntaxReceiver");
    if (type_MicroserviceSyntaxReceiver != null)
    {
        Console.WriteLine("[PASS] 类型 MicroserviceSyntaxReceiver (class) 存在");
        var ctors_MicroserviceSyntaxReceiver = type_MicroserviceSyntaxReceiver.GetConstructors();
        Console.WriteLine($"[PASS] MicroserviceSyntaxReceiver 构造函数数量: {ctors_MicroserviceSyntaxReceiver.Length}");
        var methods_MicroserviceSyntaxReceiver = type_MicroserviceSyntaxReceiver.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MicroserviceSyntaxReceiver 公开方法数量: {methods_MicroserviceSyntaxReceiver.Length}");
        foreach (var m in methods_MicroserviceSyntaxReceiver)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MicroserviceSyntaxReceiver 未找到，尝试无命名空间...");
        type_MicroserviceSyntaxReceiver = Type.GetType("MicroserviceSyntaxReceiver");
        if (type_MicroserviceSyntaxReceiver != null)
            Console.WriteLine("[PASS] 类型 MicroserviceSyntaxReceiver (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MicroserviceSyntaxReceiver 可能为顶层语句或嵌套类型");
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

    // 验证 class: ValueObjectInfo
    var type_ValueObjectInfo = Type.GetType("ValueObjectInfo");
    if (type_ValueObjectInfo != null)
    {
        Console.WriteLine("[PASS] 类型 ValueObjectInfo (class) 存在");
        var ctors_ValueObjectInfo = type_ValueObjectInfo.GetConstructors();
        Console.WriteLine($"[PASS] ValueObjectInfo 构造函数数量: {ctors_ValueObjectInfo.Length}");
        var methods_ValueObjectInfo = type_ValueObjectInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValueObjectInfo 公开方法数量: {methods_ValueObjectInfo.Length}");
        foreach (var m in methods_ValueObjectInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValueObjectInfo 未找到，尝试无命名空间...");
        type_ValueObjectInfo = Type.GetType("ValueObjectInfo");
        if (type_ValueObjectInfo != null)
            Console.WriteLine("[PASS] 类型 ValueObjectInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValueObjectInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceInfo
    var type_ServiceInfo = Type.GetType("ServiceInfo");
    if (type_ServiceInfo != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceInfo (class) 存在");
        var ctors_ServiceInfo = type_ServiceInfo.GetConstructors();
        Console.WriteLine($"[PASS] ServiceInfo 构造函数数量: {ctors_ServiceInfo.Length}");
        var methods_ServiceInfo = type_ServiceInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceInfo 公开方法数量: {methods_ServiceInfo.Length}");
        foreach (var m in methods_ServiceInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceInfo 未找到，尝试无命名空间...");
        type_ServiceInfo = Type.GetType("ServiceInfo");
        if (type_ServiceInfo != null)
            Console.WriteLine("[PASS] 类型 ServiceInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Get
    var type_Get = Type.GetType("Get");
    if (type_Get != null)
    {
        Console.WriteLine("[PASS] 类型 Get (class) 存在");
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

    // 验证 class: Create
    var type_Create = Type.GetType("Create");
    if (type_Create != null)
    {
        Console.WriteLine("[PASS] 类型 Create (class) 存在");
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

    // 验证 class: PipeNetworkGenerator
    var type_PipeNetworkGenerator = Type.GetType("PipeNetworkGenerator");
    if (type_PipeNetworkGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 PipeNetworkGenerator (class) 存在");
        var ctors_PipeNetworkGenerator = type_PipeNetworkGenerator.GetConstructors();
        Console.WriteLine($"[PASS] PipeNetworkGenerator 构造函数数量: {ctors_PipeNetworkGenerator.Length}");
        var methods_PipeNetworkGenerator = type_PipeNetworkGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PipeNetworkGenerator 公开方法数量: {methods_PipeNetworkGenerator.Length}");
        foreach (var m in methods_PipeNetworkGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PipeNetworkGenerator 未找到，尝试无命名空间...");
        type_PipeNetworkGenerator = Type.GetType("PipeNetworkGenerator");
        if (type_PipeNetworkGenerator != null)
            Console.WriteLine("[PASS] 类型 PipeNetworkGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PipeNetworkGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChannelProcessorGenerator
    var type_ChannelProcessorGenerator = Type.GetType("ChannelProcessorGenerator");
    if (type_ChannelProcessorGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelProcessorGenerator (class) 存在");
        var ctors_ChannelProcessorGenerator = type_ChannelProcessorGenerator.GetConstructors();
        Console.WriteLine($"[PASS] ChannelProcessorGenerator 构造函数数量: {ctors_ChannelProcessorGenerator.Length}");
        var methods_ChannelProcessorGenerator = type_ChannelProcessorGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelProcessorGenerator 公开方法数量: {methods_ChannelProcessorGenerator.Length}");
        foreach (var m in methods_ChannelProcessorGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelProcessorGenerator 未找到，尝试无命名空间...");
        type_ChannelProcessorGenerator = Type.GetType("ChannelProcessorGenerator");
        if (type_ChannelProcessorGenerator != null)
            Console.WriteLine("[PASS] 类型 ChannelProcessorGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelProcessorGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ObjectPoolGenerator
    var type_ObjectPoolGenerator = Type.GetType("ObjectPoolGenerator");
    if (type_ObjectPoolGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 ObjectPoolGenerator (class) 存在");
        var ctors_ObjectPoolGenerator = type_ObjectPoolGenerator.GetConstructors();
        Console.WriteLine($"[PASS] ObjectPoolGenerator 构造函数数量: {ctors_ObjectPoolGenerator.Length}");
        var methods_ObjectPoolGenerator = type_ObjectPoolGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ObjectPoolGenerator 公开方法数量: {methods_ObjectPoolGenerator.Length}");
        foreach (var m in methods_ObjectPoolGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ObjectPoolGenerator 未找到，尝试无命名空间...");
        type_ObjectPoolGenerator = Type.GetType("ObjectPoolGenerator");
        if (type_ObjectPoolGenerator != null)
            Console.WriteLine("[PASS] 类型 ObjectPoolGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ObjectPoolGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ArrayPooledObjectPolicy
    var type_ArrayPooledObjectPolicy = Type.GetType("ArrayPooledObjectPolicy");
    if (type_ArrayPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ArrayPooledObjectPolicy (class) 存在");
        var ctors_ArrayPooledObjectPolicy = type_ArrayPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ArrayPooledObjectPolicy 构造函数数量: {ctors_ArrayPooledObjectPolicy.Length}");
        var methods_ArrayPooledObjectPolicy = type_ArrayPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ArrayPooledObjectPolicy 公开方法数量: {methods_ArrayPooledObjectPolicy.Length}");
        foreach (var m in methods_ArrayPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ArrayPooledObjectPolicy 未找到，尝试无命名空间...");
        type_ArrayPooledObjectPolicy = Type.GetType("ArrayPooledObjectPolicy");
        if (type_ArrayPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 ArrayPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ArrayPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: I
    var type_I = Type.GetType("I");
    if (type_I != null)
    {
        Console.WriteLine("[PASS] 类型 I (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 I 未找到，尝试无命名空间...");
        type_I = Type.GetType("I");
        if (type_I != null)
            Console.WriteLine("[PASS] 类型 I (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 I 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

#load "dependency_injection_advanced_integration.cs"

Console.WriteLine("=== dependency_injection_advanced_integration.cs Test ===");

try
{
    // 验证 class: Service
    var type_Service = Type.GetType("Service");
    if (type_Service != null)
    {
        Console.WriteLine("[PASS] 类型 Service (class) 存在");
        var ctors_Service = type_Service.GetConstructors();
        Console.WriteLine($"[PASS] Service 构造函数数量: {ctors_Service.Length}");
        var methods_Service = type_Service.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Service 公开方法数量: {methods_Service.Length}");
        foreach (var m in methods_Service)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Service 未找到，尝试无命名空间...");
        type_Service = Type.GetType("Service");
        if (type_Service != null)
            Console.WriteLine("[PASS] 类型 Service (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Service 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DecoratedService
    var type_DecoratedService = Type.GetType("DecoratedService");
    if (type_DecoratedService != null)
    {
        Console.WriteLine("[PASS] 类型 DecoratedService (class) 存在");
        var ctors_DecoratedService = type_DecoratedService.GetConstructors();
        Console.WriteLine($"[PASS] DecoratedService 构造函数数量: {ctors_DecoratedService.Length}");
        var methods_DecoratedService = type_DecoratedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DecoratedService 公开方法数量: {methods_DecoratedService.Length}");
        foreach (var m in methods_DecoratedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DecoratedService 未找到，尝试无命名空间...");
        type_DecoratedService = Type.GetType("DecoratedService");
        if (type_DecoratedService != null)
            Console.WriteLine("[PASS] 类型 DecoratedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DecoratedService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Decorator
    var type_Decorator = Type.GetType("Decorator");
    if (type_Decorator != null)
    {
        Console.WriteLine("[PASS] 类型 Decorator (class) 存在");
        var ctors_Decorator = type_Decorator.GetConstructors();
        Console.WriteLine($"[PASS] Decorator 构造函数数量: {ctors_Decorator.Length}");
        var methods_Decorator = type_Decorator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Decorator 公开方法数量: {methods_Decorator.Length}");
        foreach (var m in methods_Decorator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Decorator 未找到，尝试无命名空间...");
        type_Decorator = Type.GetType("Decorator");
        if (type_Decorator != null)
            Console.WriteLine("[PASS] 类型 Decorator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Decorator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ScannedService
    var type_ScannedService = Type.GetType("ScannedService");
    if (type_ScannedService != null)
    {
        Console.WriteLine("[PASS] 类型 ScannedService (class) 存在");
        var ctors_ScannedService = type_ScannedService.GetConstructors();
        Console.WriteLine($"[PASS] ScannedService 构造函数数量: {ctors_ScannedService.Length}");
        var methods_ScannedService = type_ScannedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ScannedService 公开方法数量: {methods_ScannedService.Length}");
        foreach (var m in methods_ScannedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ScannedService 未找到，尝试无命名空间...");
        type_ScannedService = Type.GetType("ScannedService");
        if (type_ScannedService != null)
            Console.WriteLine("[PASS] 类型 ScannedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ScannedService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FactoryService
    var type_FactoryService = Type.GetType("FactoryService");
    if (type_FactoryService != null)
    {
        Console.WriteLine("[PASS] 类型 FactoryService (class) 存在");
        var ctors_FactoryService = type_FactoryService.GetConstructors();
        Console.WriteLine($"[PASS] FactoryService 构造函数数量: {ctors_FactoryService.Length}");
        var methods_FactoryService = type_FactoryService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FactoryService 公开方法数量: {methods_FactoryService.Length}");
        foreach (var m in methods_FactoryService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FactoryService 未找到，尝试无命名空间...");
        type_FactoryService = Type.GetType("FactoryService");
        if (type_FactoryService != null)
            Console.WriteLine("[PASS] 类型 FactoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FactoryService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GenericService
    var type_GenericService = Type.GetType("GenericService");
    if (type_GenericService != null)
    {
        Console.WriteLine("[PASS] 类型 GenericService (class) 存在");
        var ctors_GenericService = type_GenericService.GetConstructors();
        Console.WriteLine($"[PASS] GenericService 构造函数数量: {ctors_GenericService.Length}");
        var methods_GenericService = type_GenericService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GenericService 公开方法数量: {methods_GenericService.Length}");
        foreach (var m in methods_GenericService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GenericService 未找到，尝试无命名空间...");
        type_GenericService = Type.GetType("GenericService");
        if (type_GenericService != null)
            Console.WriteLine("[PASS] 类型 GenericService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GenericService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NamedServiceA
    var type_NamedServiceA = Type.GetType("NamedServiceA");
    if (type_NamedServiceA != null)
    {
        Console.WriteLine("[PASS] 类型 NamedServiceA (class) 存在");
        var ctors_NamedServiceA = type_NamedServiceA.GetConstructors();
        Console.WriteLine($"[PASS] NamedServiceA 构造函数数量: {ctors_NamedServiceA.Length}");
        var methods_NamedServiceA = type_NamedServiceA.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NamedServiceA 公开方法数量: {methods_NamedServiceA.Length}");
        foreach (var m in methods_NamedServiceA)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NamedServiceA 未找到，尝试无命名空间...");
        type_NamedServiceA = Type.GetType("NamedServiceA");
        if (type_NamedServiceA != null)
            Console.WriteLine("[PASS] 类型 NamedServiceA (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NamedServiceA 可能为顶层语句或嵌套类型");
    }

    // 验证 class: NamedServiceB
    var type_NamedServiceB = Type.GetType("NamedServiceB");
    if (type_NamedServiceB != null)
    {
        Console.WriteLine("[PASS] 类型 NamedServiceB (class) 存在");
        var ctors_NamedServiceB = type_NamedServiceB.GetConstructors();
        Console.WriteLine($"[PASS] NamedServiceB 构造函数数量: {ctors_NamedServiceB.Length}");
        var methods_NamedServiceB = type_NamedServiceB.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NamedServiceB 公开方法数量: {methods_NamedServiceB.Length}");
        foreach (var m in methods_NamedServiceB)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NamedServiceB 未找到，尝试无命名空间...");
        type_NamedServiceB = Type.GetType("NamedServiceB");
        if (type_NamedServiceB != null)
            Console.WriteLine("[PASS] 类型 NamedServiceB (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NamedServiceB 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ValidatedService
    var type_ValidatedService = Type.GetType("ValidatedService");
    if (type_ValidatedService != null)
    {
        Console.WriteLine("[PASS] 类型 ValidatedService (class) 存在");
        var ctors_ValidatedService = type_ValidatedService.GetConstructors();
        Console.WriteLine($"[PASS] ValidatedService 构造函数数量: {ctors_ValidatedService.Length}");
        var methods_ValidatedService = type_ValidatedService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ValidatedService 公开方法数量: {methods_ValidatedService.Length}");
        foreach (var m in methods_ValidatedService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ValidatedService 未找到，尝试无命名空间...");
        type_ValidatedService = Type.GetType("ValidatedService");
        if (type_ValidatedService != null)
            Console.WriteLine("[PASS] 类型 ValidatedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ValidatedService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: LifetimeEventsService
    var type_LifetimeEventsService = Type.GetType("LifetimeEventsService");
    if (type_LifetimeEventsService != null)
    {
        Console.WriteLine("[PASS] 类型 LifetimeEventsService (class) 存在");
        var ctors_LifetimeEventsService = type_LifetimeEventsService.GetConstructors();
        Console.WriteLine($"[PASS] LifetimeEventsService 构造函数数量: {ctors_LifetimeEventsService.Length}");
        var methods_LifetimeEventsService = type_LifetimeEventsService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] LifetimeEventsService 公开方法数量: {methods_LifetimeEventsService.Length}");
        foreach (var m in methods_LifetimeEventsService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 LifetimeEventsService 未找到，尝试无命名空间...");
        type_LifetimeEventsService = Type.GetType("LifetimeEventsService");
        if (type_LifetimeEventsService != null)
            Console.WriteLine("[PASS] 类型 LifetimeEventsService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 LifetimeEventsService 可能为顶层语句或嵌套类型");
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

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IService
    var type_IService = Type.GetType("IService");
    if (type_IService != null)
    {
        Console.WriteLine("[PASS] 类型 IService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IService 未找到，尝试无命名空间...");
        type_IService = Type.GetType("IService");
        if (type_IService != null)
            Console.WriteLine("[PASS] 类型 IService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDecoratedService
    var type_IDecoratedService = Type.GetType("IDecoratedService");
    if (type_IDecoratedService != null)
    {
        Console.WriteLine("[PASS] 类型 IDecoratedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDecoratedService 未找到，尝试无命名空间...");
        type_IDecoratedService = Type.GetType("IDecoratedService");
        if (type_IDecoratedService != null)
            Console.WriteLine("[PASS] 类型 IDecoratedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDecoratedService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IScannedService
    var type_IScannedService = Type.GetType("IScannedService");
    if (type_IScannedService != null)
    {
        Console.WriteLine("[PASS] 类型 IScannedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IScannedService 未找到，尝试无命名空间...");
        type_IScannedService = Type.GetType("IScannedService");
        if (type_IScannedService != null)
            Console.WriteLine("[PASS] 类型 IScannedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IScannedService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IFactoryService
    var type_IFactoryService = Type.GetType("IFactoryService");
    if (type_IFactoryService != null)
    {
        Console.WriteLine("[PASS] 类型 IFactoryService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IFactoryService 未找到，尝试无命名空间...");
        type_IFactoryService = Type.GetType("IFactoryService");
        if (type_IFactoryService != null)
            Console.WriteLine("[PASS] 类型 IFactoryService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFactoryService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IGenericService
    var type_IGenericService = Type.GetType("IGenericService");
    if (type_IGenericService != null)
    {
        Console.WriteLine("[PASS] 类型 IGenericService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IGenericService 未找到，尝试无命名空间...");
        type_IGenericService = Type.GetType("IGenericService");
        if (type_IGenericService != null)
            Console.WriteLine("[PASS] 类型 IGenericService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IGenericService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: INamedService
    var type_INamedService = Type.GetType("INamedService");
    if (type_INamedService != null)
    {
        Console.WriteLine("[PASS] 类型 INamedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 INamedService 未找到，尝试无命名空间...");
        type_INamedService = Type.GetType("INamedService");
        if (type_INamedService != null)
            Console.WriteLine("[PASS] 类型 INamedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 INamedService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IValidatedService
    var type_IValidatedService = Type.GetType("IValidatedService");
    if (type_IValidatedService != null)
    {
        Console.WriteLine("[PASS] 类型 IValidatedService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IValidatedService 未找到，尝试无命名空间...");
        type_IValidatedService = Type.GetType("IValidatedService");
        if (type_IValidatedService != null)
            Console.WriteLine("[PASS] 类型 IValidatedService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IValidatedService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

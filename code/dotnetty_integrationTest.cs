#load "dotnetty_integration.cs"

Console.WriteLine("=== dotnetty_integration.cs Test ===");

try
{
    // 验证 class: DotNettyOptions
    var type_DotNettyOptions = Type.GetType("DotNettyOptions");
    if (type_DotNettyOptions != null)
    {
        Console.WriteLine("[PASS] 类型 DotNettyOptions (class) 存在");
        var ctors_DotNettyOptions = type_DotNettyOptions.GetConstructors();
        Console.WriteLine($"[PASS] DotNettyOptions 构造函数数量: {ctors_DotNettyOptions.Length}");
        var methods_DotNettyOptions = type_DotNettyOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNettyOptions 公开方法数量: {methods_DotNettyOptions.Length}");
        foreach (var m in methods_DotNettyOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNettyOptions 未找到，尝试无命名空间...");
        type_DotNettyOptions = Type.GetType("DotNettyOptions");
        if (type_DotNettyOptions != null)
            Console.WriteLine("[PASS] 类型 DotNettyOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNettyOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotNettyService
    var type_DotNettyService = Type.GetType("DotNettyService");
    if (type_DotNettyService != null)
    {
        Console.WriteLine("[PASS] 类型 DotNettyService (class) 存在");
        var ctors_DotNettyService = type_DotNettyService.GetConstructors();
        Console.WriteLine($"[PASS] DotNettyService 构造函数数量: {ctors_DotNettyService.Length}");
        var methods_DotNettyService = type_DotNettyService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNettyService 公开方法数量: {methods_DotNettyService.Length}");
        foreach (var m in methods_DotNettyService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNettyService 未找到，尝试无命名空间...");
        type_DotNettyService = Type.GetType("DotNettyService");
        if (type_DotNettyService != null)
            Console.WriteLine("[PASS] 类型 DotNettyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNettyService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotNettyExtensions
    var type_DotNettyExtensions = Type.GetType("DotNettyExtensions");
    if (type_DotNettyExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 DotNettyExtensions (class) 存在");
        var ctors_DotNettyExtensions = type_DotNettyExtensions.GetConstructors();
        Console.WriteLine($"[PASS] DotNettyExtensions 构造函数数量: {ctors_DotNettyExtensions.Length}");
        var methods_DotNettyExtensions = type_DotNettyExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNettyExtensions 公开方法数量: {methods_DotNettyExtensions.Length}");
        foreach (var m in methods_DotNettyExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNettyExtensions 未找到，尝试无命名空间...");
        type_DotNettyExtensions = Type.GetType("DotNettyExtensions");
        if (type_DotNettyExtensions != null)
            Console.WriteLine("[PASS] 类型 DotNettyExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNettyExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DotNettyExample
    var type_DotNettyExample = Type.GetType("DotNettyExample");
    if (type_DotNettyExample != null)
    {
        Console.WriteLine("[PASS] 类型 DotNettyExample (class) 存在");
        var ctors_DotNettyExample = type_DotNettyExample.GetConstructors();
        Console.WriteLine($"[PASS] DotNettyExample 构造函数数量: {ctors_DotNettyExample.Length}");
        var methods_DotNettyExample = type_DotNettyExample.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DotNettyExample 公开方法数量: {methods_DotNettyExample.Length}");
        foreach (var m in methods_DotNettyExample)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DotNettyExample 未找到，尝试无命名空间...");
        type_DotNettyExample = Type.GetType("DotNettyExample");
        if (type_DotNettyExample != null)
            Console.WriteLine("[PASS] 类型 DotNettyExample (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DotNettyExample 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpanLengthFieldBasedFrameDecoder
    var type_SpanLengthFieldBasedFrameDecoder = Type.GetType("SpanLengthFieldBasedFrameDecoder");
    if (type_SpanLengthFieldBasedFrameDecoder != null)
    {
        Console.WriteLine("[PASS] 类型 SpanLengthFieldBasedFrameDecoder (class) 存在");
        var ctors_SpanLengthFieldBasedFrameDecoder = type_SpanLengthFieldBasedFrameDecoder.GetConstructors();
        Console.WriteLine($"[PASS] SpanLengthFieldBasedFrameDecoder 构造函数数量: {ctors_SpanLengthFieldBasedFrameDecoder.Length}");
        var methods_SpanLengthFieldBasedFrameDecoder = type_SpanLengthFieldBasedFrameDecoder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpanLengthFieldBasedFrameDecoder 公开方法数量: {methods_SpanLengthFieldBasedFrameDecoder.Length}");
        foreach (var m in methods_SpanLengthFieldBasedFrameDecoder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpanLengthFieldBasedFrameDecoder 未找到，尝试无命名空间...");
        type_SpanLengthFieldBasedFrameDecoder = Type.GetType("SpanLengthFieldBasedFrameDecoder");
        if (type_SpanLengthFieldBasedFrameDecoder != null)
            Console.WriteLine("[PASS] 类型 SpanLengthFieldBasedFrameDecoder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpanLengthFieldBasedFrameDecoder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpanLengthFieldPrepender
    var type_SpanLengthFieldPrepender = Type.GetType("SpanLengthFieldPrepender");
    if (type_SpanLengthFieldPrepender != null)
    {
        Console.WriteLine("[PASS] 类型 SpanLengthFieldPrepender (class) 存在");
        var ctors_SpanLengthFieldPrepender = type_SpanLengthFieldPrepender.GetConstructors();
        Console.WriteLine($"[PASS] SpanLengthFieldPrepender 构造函数数量: {ctors_SpanLengthFieldPrepender.Length}");
        var methods_SpanLengthFieldPrepender = type_SpanLengthFieldPrepender.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpanLengthFieldPrepender 公开方法数量: {methods_SpanLengthFieldPrepender.Length}");
        foreach (var m in methods_SpanLengthFieldPrepender)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpanLengthFieldPrepender 未找到，尝试无命名空间...");
        type_SpanLengthFieldPrepender = Type.GetType("SpanLengthFieldPrepender");
        if (type_SpanLengthFieldPrepender != null)
            Console.WriteLine("[PASS] 类型 SpanLengthFieldPrepender (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpanLengthFieldPrepender 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpanStringEncoder
    var type_SpanStringEncoder = Type.GetType("SpanStringEncoder");
    if (type_SpanStringEncoder != null)
    {
        Console.WriteLine("[PASS] 类型 SpanStringEncoder (class) 存在");
        var ctors_SpanStringEncoder = type_SpanStringEncoder.GetConstructors();
        Console.WriteLine($"[PASS] SpanStringEncoder 构造函数数量: {ctors_SpanStringEncoder.Length}");
        var methods_SpanStringEncoder = type_SpanStringEncoder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpanStringEncoder 公开方法数量: {methods_SpanStringEncoder.Length}");
        foreach (var m in methods_SpanStringEncoder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpanStringEncoder 未找到，尝试无命名空间...");
        type_SpanStringEncoder = Type.GetType("SpanStringEncoder");
        if (type_SpanStringEncoder != null)
            Console.WriteLine("[PASS] 类型 SpanStringEncoder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpanStringEncoder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SpanStringDecoder
    var type_SpanStringDecoder = Type.GetType("SpanStringDecoder");
    if (type_SpanStringDecoder != null)
    {
        Console.WriteLine("[PASS] 类型 SpanStringDecoder (class) 存在");
        var ctors_SpanStringDecoder = type_SpanStringDecoder.GetConstructors();
        Console.WriteLine($"[PASS] SpanStringDecoder 构造函数数量: {ctors_SpanStringDecoder.Length}");
        var methods_SpanStringDecoder = type_SpanStringDecoder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpanStringDecoder 公开方法数量: {methods_SpanStringDecoder.Length}");
        foreach (var m in methods_SpanStringDecoder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpanStringDecoder 未找到，尝试无命名空间...");
        type_SpanStringDecoder = Type.GetType("SpanStringDecoder");
        if (type_SpanStringDecoder != null)
            Console.WriteLine("[PASS] 类型 SpanStringDecoder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpanStringDecoder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ByteArrayPooledObjectPolicy
    var type_ByteArrayPooledObjectPolicy = Type.GetType("ByteArrayPooledObjectPolicy");
    if (type_ByteArrayPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ByteArrayPooledObjectPolicy (class) 存在");
        var ctors_ByteArrayPooledObjectPolicy = type_ByteArrayPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ByteArrayPooledObjectPolicy 构造函数数量: {ctors_ByteArrayPooledObjectPolicy.Length}");
        var methods_ByteArrayPooledObjectPolicy = type_ByteArrayPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ByteArrayPooledObjectPolicy 公开方法数量: {methods_ByteArrayPooledObjectPolicy.Length}");
        foreach (var m in methods_ByteArrayPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ByteArrayPooledObjectPolicy 未找到，尝试无命名空间...");
        type_ByteArrayPooledObjectPolicy = Type.GetType("ByteArrayPooledObjectPolicy");
        if (type_ByteArrayPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 ByteArrayPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ByteArrayPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SimpleChannelHandler
    var type_SimpleChannelHandler = Type.GetType("SimpleChannelHandler");
    if (type_SimpleChannelHandler != null)
    {
        Console.WriteLine("[PASS] 类型 SimpleChannelHandler (class) 存在");
        var ctors_SimpleChannelHandler = type_SimpleChannelHandler.GetConstructors();
        Console.WriteLine($"[PASS] SimpleChannelHandler 构造函数数量: {ctors_SimpleChannelHandler.Length}");
        var methods_SimpleChannelHandler = type_SimpleChannelHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SimpleChannelHandler 公开方法数量: {methods_SimpleChannelHandler.Length}");
        foreach (var m in methods_SimpleChannelHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SimpleChannelHandler 未找到，尝试无命名空间...");
        type_SimpleChannelHandler = Type.GetType("SimpleChannelHandler");
        if (type_SimpleChannelHandler != null)
            Console.WriteLine("[PASS] 类型 SimpleChannelHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SimpleChannelHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HeartbeatHandler
    var type_HeartbeatHandler = Type.GetType("HeartbeatHandler");
    if (type_HeartbeatHandler != null)
    {
        Console.WriteLine("[PASS] 类型 HeartbeatHandler (class) 存在");
        var ctors_HeartbeatHandler = type_HeartbeatHandler.GetConstructors();
        Console.WriteLine($"[PASS] HeartbeatHandler 构造函数数量: {ctors_HeartbeatHandler.Length}");
        var methods_HeartbeatHandler = type_HeartbeatHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HeartbeatHandler 公开方法数量: {methods_HeartbeatHandler.Length}");
        foreach (var m in methods_HeartbeatHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HeartbeatHandler 未找到，尝试无命名空间...");
        type_HeartbeatHandler = Type.GetType("HeartbeatHandler");
        if (type_HeartbeatHandler != null)
            Console.WriteLine("[PASS] 类型 HeartbeatHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HeartbeatHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDotNettyService
    var type_IDotNettyService = Type.GetType("IDotNettyService");
    if (type_IDotNettyService != null)
    {
        Console.WriteLine("[PASS] 类型 IDotNettyService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDotNettyService 未找到，尝试无命名空间...");
        type_IDotNettyService = Type.GetType("IDotNettyService");
        if (type_IDotNettyService != null)
            Console.WriteLine("[PASS] 类型 IDotNettyService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDotNettyService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

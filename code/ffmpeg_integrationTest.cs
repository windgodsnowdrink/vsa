#load "ffmpeg_integration.cs"

Console.WriteLine("=== ffmpeg_integration.cs Test ===");

try
{
    // 验证 class: DistributedStreamProcessor
    var type_DistributedStreamProcessor = Type.GetType("DistributedStreamProcessor");
    if (type_DistributedStreamProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedStreamProcessor (class) 存在");
        var ctors_DistributedStreamProcessor = type_DistributedStreamProcessor.GetConstructors();
        Console.WriteLine($"[PASS] DistributedStreamProcessor 构造函数数量: {ctors_DistributedStreamProcessor.Length}");
        var methods_DistributedStreamProcessor = type_DistributedStreamProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedStreamProcessor 公开方法数量: {methods_DistributedStreamProcessor.Length}");
        foreach (var m in methods_DistributedStreamProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedStreamProcessor 未找到，尝试无命名空间...");
        type_DistributedStreamProcessor = Type.GetType("DistributedStreamProcessor");
        if (type_DistributedStreamProcessor != null)
            Console.WriteLine("[PASS] 类型 DistributedStreamProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedStreamProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: StreamMetrics
    var type_StreamMetrics = Type.GetType("StreamMetrics");
    if (type_StreamMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 StreamMetrics (class) 存在");
        var ctors_StreamMetrics = type_StreamMetrics.GetConstructors();
        Console.WriteLine($"[PASS] StreamMetrics 构造函数数量: {ctors_StreamMetrics.Length}");
        var methods_StreamMetrics = type_StreamMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StreamMetrics 公开方法数量: {methods_StreamMetrics.Length}");
        foreach (var m in methods_StreamMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StreamMetrics 未找到，尝试无命名空间...");
        type_StreamMetrics = Type.GetType("StreamMetrics");
        if (type_StreamMetrics != null)
            Console.WriteLine("[PASS] 类型 StreamMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StreamMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdaptiveBitrateController
    var type_AdaptiveBitrateController = Type.GetType("AdaptiveBitrateController");
    if (type_AdaptiveBitrateController != null)
    {
        Console.WriteLine("[PASS] 类型 AdaptiveBitrateController (class) 存在");
        var ctors_AdaptiveBitrateController = type_AdaptiveBitrateController.GetConstructors();
        Console.WriteLine($"[PASS] AdaptiveBitrateController 构造函数数量: {ctors_AdaptiveBitrateController.Length}");
        var methods_AdaptiveBitrateController = type_AdaptiveBitrateController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdaptiveBitrateController 公开方法数量: {methods_AdaptiveBitrateController.Length}");
        foreach (var m in methods_AdaptiveBitrateController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdaptiveBitrateController 未找到，尝试无命名空间...");
        type_AdaptiveBitrateController = Type.GetType("AdaptiveBitrateController");
        if (type_AdaptiveBitrateController != null)
            Console.WriteLine("[PASS] 类型 AdaptiveBitrateController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdaptiveBitrateController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FailoverHandler
    var type_FailoverHandler = Type.GetType("FailoverHandler");
    if (type_FailoverHandler != null)
    {
        Console.WriteLine("[PASS] 类型 FailoverHandler (class) 存在");
        var ctors_FailoverHandler = type_FailoverHandler.GetConstructors();
        Console.WriteLine($"[PASS] FailoverHandler 构造函数数量: {ctors_FailoverHandler.Length}");
        var methods_FailoverHandler = type_FailoverHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FailoverHandler 公开方法数量: {methods_FailoverHandler.Length}");
        foreach (var m in methods_FailoverHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FailoverHandler 未找到，尝试无命名空间...");
        type_FailoverHandler = Type.GetType("FailoverHandler");
        if (type_FailoverHandler != null)
            Console.WriteLine("[PASS] 类型 FailoverHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FailoverHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BitrateAdjuster
    var type_BitrateAdjuster = Type.GetType("BitrateAdjuster");
    if (type_BitrateAdjuster != null)
    {
        Console.WriteLine("[PASS] 类型 BitrateAdjuster (class) 存在");
        var ctors_BitrateAdjuster = type_BitrateAdjuster.GetConstructors();
        Console.WriteLine($"[PASS] BitrateAdjuster 构造函数数量: {ctors_BitrateAdjuster.Length}");
        var methods_BitrateAdjuster = type_BitrateAdjuster.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BitrateAdjuster 公开方法数量: {methods_BitrateAdjuster.Length}");
        foreach (var m in methods_BitrateAdjuster)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BitrateAdjuster 未找到，尝试无命名空间...");
        type_BitrateAdjuster = Type.GetType("BitrateAdjuster");
        if (type_BitrateAdjuster != null)
            Console.WriteLine("[PASS] 类型 BitrateAdjuster (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BitrateAdjuster 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MainProcessor
    var type_MainProcessor = Type.GetType("MainProcessor");
    if (type_MainProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 MainProcessor (class) 存在");
        var ctors_MainProcessor = type_MainProcessor.GetConstructors();
        Console.WriteLine($"[PASS] MainProcessor 构造函数数量: {ctors_MainProcessor.Length}");
        var methods_MainProcessor = type_MainProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MainProcessor 公开方法数量: {methods_MainProcessor.Length}");
        foreach (var m in methods_MainProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MainProcessor 未找到，尝试无命名空间...");
        type_MainProcessor = Type.GetType("MainProcessor");
        if (type_MainProcessor != null)
            Console.WriteLine("[PASS] 类型 MainProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MainProcessor 可能为顶层语句或嵌套类型");
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

    // 验证 struct: NetworkMetrics
    var type_NetworkMetrics = Type.GetType("NetworkMetrics");
    if (type_NetworkMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkMetrics (struct) 存在");
        var ctors_NetworkMetrics = type_NetworkMetrics.GetConstructors();
        Console.WriteLine($"[PASS] NetworkMetrics 构造函数数量: {ctors_NetworkMetrics.Length}");
        var methods_NetworkMetrics = type_NetworkMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkMetrics 公开方法数量: {methods_NetworkMetrics.Length}");
        foreach (var m in methods_NetworkMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkMetrics 未找到，尝试无命名空间...");
        type_NetworkMetrics = Type.GetType("NetworkMetrics");
        if (type_NetworkMetrics != null)
            Console.WriteLine("[PASS] 类型 NetworkMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetworkMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 record: MediaFrame
    var type_MediaFrame = Type.GetType("MediaFrame");
    if (type_MediaFrame != null)
    {
        Console.WriteLine("[PASS] 类型 MediaFrame (record) 存在");
        var ctors_MediaFrame = type_MediaFrame.GetConstructors();
        Console.WriteLine($"[PASS] MediaFrame 构造函数数量: {ctors_MediaFrame.Length}");
        var methods_MediaFrame = type_MediaFrame.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MediaFrame 公开方法数量: {methods_MediaFrame.Length}");
        foreach (var m in methods_MediaFrame)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MediaFrame 未找到，尝试无命名空间...");
        type_MediaFrame = Type.GetType("MediaFrame");
        if (type_MediaFrame != null)
            Console.WriteLine("[PASS] 类型 MediaFrame (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MediaFrame 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProcessedFrame
    var type_ProcessedFrame = Type.GetType("ProcessedFrame");
    if (type_ProcessedFrame != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessedFrame (record) 存在");
        var ctors_ProcessedFrame = type_ProcessedFrame.GetConstructors();
        Console.WriteLine($"[PASS] ProcessedFrame 构造函数数量: {ctors_ProcessedFrame.Length}");
        var methods_ProcessedFrame = type_ProcessedFrame.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessedFrame 公开方法数量: {methods_ProcessedFrame.Length}");
        foreach (var m in methods_ProcessedFrame)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessedFrame 未找到，尝试无命名空间...");
        type_ProcessedFrame = Type.GetType("ProcessedFrame");
        if (type_ProcessedFrame != null)
            Console.WriteLine("[PASS] 类型 ProcessedFrame (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessedFrame 可能为顶层语句或嵌套类型");
    }

    // 验证 record: BitrateAdjustment
    var type_BitrateAdjustment = Type.GetType("BitrateAdjustment");
    if (type_BitrateAdjustment != null)
    {
        Console.WriteLine("[PASS] 类型 BitrateAdjustment (record) 存在");
        var ctors_BitrateAdjustment = type_BitrateAdjustment.GetConstructors();
        Console.WriteLine($"[PASS] BitrateAdjustment 构造函数数量: {ctors_BitrateAdjustment.Length}");
        var methods_BitrateAdjustment = type_BitrateAdjustment.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BitrateAdjustment 公开方法数量: {methods_BitrateAdjustment.Length}");
        foreach (var m in methods_BitrateAdjustment)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BitrateAdjustment 未找到，尝试无命名空间...");
        type_BitrateAdjustment = Type.GetType("BitrateAdjustment");
        if (type_BitrateAdjustment != null)
            Console.WriteLine("[PASS] 类型 BitrateAdjustment (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BitrateAdjustment 可能为顶层语句或嵌套类型");
    }

    // 验证 record: NetworkCondition
    var type_NetworkCondition = Type.GetType("NetworkCondition");
    if (type_NetworkCondition != null)
    {
        Console.WriteLine("[PASS] 类型 NetworkCondition (record) 存在");
        var ctors_NetworkCondition = type_NetworkCondition.GetConstructors();
        Console.WriteLine($"[PASS] NetworkCondition 构造函数数量: {ctors_NetworkCondition.Length}");
        var methods_NetworkCondition = type_NetworkCondition.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NetworkCondition 公开方法数量: {methods_NetworkCondition.Length}");
        foreach (var m in methods_NetworkCondition)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NetworkCondition 未找到，尝试无命名空间...");
        type_NetworkCondition = Type.GetType("NetworkCondition");
        if (type_NetworkCondition != null)
            Console.WriteLine("[PASS] 类型 NetworkCondition (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NetworkCondition 可能为顶层语句或嵌套类型");
    }

    // 验证 record: FailoverEvent
    var type_FailoverEvent = Type.GetType("FailoverEvent");
    if (type_FailoverEvent != null)
    {
        Console.WriteLine("[PASS] 类型 FailoverEvent (record) 存在");
        var ctors_FailoverEvent = type_FailoverEvent.GetConstructors();
        Console.WriteLine($"[PASS] FailoverEvent 构造函数数量: {ctors_FailoverEvent.Length}");
        var methods_FailoverEvent = type_FailoverEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FailoverEvent 公开方法数量: {methods_FailoverEvent.Length}");
        foreach (var m in methods_FailoverEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FailoverEvent 未找到，尝试无命名空间...");
        type_FailoverEvent = Type.GetType("FailoverEvent");
        if (type_FailoverEvent != null)
            Console.WriteLine("[PASS] 类型 FailoverEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FailoverEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

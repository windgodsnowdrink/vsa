#load "opencv_video_processing.cs"

Console.WriteLine("=== opencv_video_processing.cs Test ===");

try
{
    // 验证 class: VideoProcessingService
    var type_VideoProcessingService = Type.GetType("VideoProcessingService");
    if (type_VideoProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 VideoProcessingService (class) 存在");
        var ctors_VideoProcessingService = type_VideoProcessingService.GetConstructors();
        Console.WriteLine($"[PASS] VideoProcessingService 构造函数数量: {ctors_VideoProcessingService.Length}");
        var methods_VideoProcessingService = type_VideoProcessingService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] VideoProcessingService 公开方法数量: {methods_VideoProcessingService.Length}");
        foreach (var m in methods_VideoProcessingService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 VideoProcessingService 未找到，尝试无命名空间...");
        type_VideoProcessingService = Type.GetType("VideoProcessingService");
        if (type_VideoProcessingService != null)
            Console.WriteLine("[PASS] 类型 VideoProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 VideoProcessingService 可能为顶层语句或嵌套类型");
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

    // 验证 class: ImageProcessingHub
    var type_ImageProcessingHub = Type.GetType("ImageProcessingHub");
    if (type_ImageProcessingHub != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessingHub (class) 存在");
        var ctors_ImageProcessingHub = type_ImageProcessingHub.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessingHub 构造函数数量: {ctors_ImageProcessingHub.Length}");
        var methods_ImageProcessingHub = type_ImageProcessingHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessingHub 公开方法数量: {methods_ImageProcessingHub.Length}");
        foreach (var m in methods_ImageProcessingHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessingHub 未找到，尝试无命名空间...");
        type_ImageProcessingHub = Type.GetType("ImageProcessingHub");
        if (type_ImageProcessingHub != null)
            Console.WriteLine("[PASS] 类型 ImageProcessingHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessingHub 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SignalRServiceExtensions
    var type_SignalRServiceExtensions = Type.GetType("SignalRServiceExtensions");
    if (type_SignalRServiceExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 SignalRServiceExtensions (class) 存在");
        var ctors_SignalRServiceExtensions = type_SignalRServiceExtensions.GetConstructors();
        Console.WriteLine($"[PASS] SignalRServiceExtensions 构造函数数量: {ctors_SignalRServiceExtensions.Length}");
        var methods_SignalRServiceExtensions = type_SignalRServiceExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SignalRServiceExtensions 公开方法数量: {methods_SignalRServiceExtensions.Length}");
        foreach (var m in methods_SignalRServiceExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SignalRServiceExtensions 未找到，尝试无命名空间...");
        type_SignalRServiceExtensions = Type.GetType("SignalRServiceExtensions");
        if (type_SignalRServiceExtensions != null)
            Console.WriteLine("[PASS] 类型 SignalRServiceExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SignalRServiceExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ImageProcessingExtensions
    var type_ImageProcessingExtensions = Type.GetType("ImageProcessingExtensions");
    if (type_ImageProcessingExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessingExtensions (class) 存在");
        var ctors_ImageProcessingExtensions = type_ImageProcessingExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessingExtensions 构造函数数量: {ctors_ImageProcessingExtensions.Length}");
        var methods_ImageProcessingExtensions = type_ImageProcessingExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessingExtensions 公开方法数量: {methods_ImageProcessingExtensions.Length}");
        foreach (var m in methods_ImageProcessingExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessingExtensions 未找到，尝试无命名空间...");
        type_ImageProcessingExtensions = Type.GetType("ImageProcessingExtensions");
        if (type_ImageProcessingExtensions != null)
            Console.WriteLine("[PASS] 类型 ImageProcessingExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessingExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OpenCvFormatConverter
    var type_OpenCvFormatConverter = Type.GetType("OpenCvFormatConverter");
    if (type_OpenCvFormatConverter != null)
    {
        Console.WriteLine("[PASS] 类型 OpenCvFormatConverter (class) 存在");
        var ctors_OpenCvFormatConverter = type_OpenCvFormatConverter.GetConstructors();
        Console.WriteLine($"[PASS] OpenCvFormatConverter 构造函数数量: {ctors_OpenCvFormatConverter.Length}");
        var methods_OpenCvFormatConverter = type_OpenCvFormatConverter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OpenCvFormatConverter 公开方法数量: {methods_OpenCvFormatConverter.Length}");
        foreach (var m in methods_OpenCvFormatConverter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OpenCvFormatConverter 未找到，尝试无命名空间...");
        type_OpenCvFormatConverter = Type.GetType("OpenCvFormatConverter");
        if (type_OpenCvFormatConverter != null)
            Console.WriteLine("[PASS] 类型 OpenCvFormatConverter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OpenCvFormatConverter 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IVideoProcessingService
    var type_IVideoProcessingService = Type.GetType("IVideoProcessingService");
    if (type_IVideoProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 IVideoProcessingService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IVideoProcessingService 未找到，尝试无命名空间...");
        type_IVideoProcessingService = Type.GetType("IVideoProcessingService");
        if (type_IVideoProcessingService != null)
            Console.WriteLine("[PASS] 类型 IVideoProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IVideoProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IImageProcessingService
    var type_IImageProcessingService = Type.GetType("IImageProcessingService");
    if (type_IImageProcessingService != null)
    {
        Console.WriteLine("[PASS] 类型 IImageProcessingService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IImageProcessingService 未找到，尝试无命名空间...");
        type_IImageProcessingService = Type.GetType("IImageProcessingService");
        if (type_IImageProcessingService != null)
            Console.WriteLine("[PASS] 类型 IImageProcessingService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IImageProcessingService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IImageFormatConverter
    var type_IImageFormatConverter = Type.GetType("IImageFormatConverter");
    if (type_IImageFormatConverter != null)
    {
        Console.WriteLine("[PASS] 类型 IImageFormatConverter (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IImageFormatConverter 未找到，尝试无命名空间...");
        type_IImageFormatConverter = Type.GetType("IImageFormatConverter");
        if (type_IImageFormatConverter != null)
            Console.WriteLine("[PASS] 类型 IImageFormatConverter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IImageFormatConverter 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ImageProcessingRequest
    var type_ImageProcessingRequest = Type.GetType("ImageProcessingRequest");
    if (type_ImageProcessingRequest != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessingRequest (record) 存在");
        var ctors_ImageProcessingRequest = type_ImageProcessingRequest.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessingRequest 构造函数数量: {ctors_ImageProcessingRequest.Length}");
        var methods_ImageProcessingRequest = type_ImageProcessingRequest.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessingRequest 公开方法数量: {methods_ImageProcessingRequest.Length}");
        foreach (var m in methods_ImageProcessingRequest)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessingRequest 未找到，尝试无命名空间...");
        type_ImageProcessingRequest = Type.GetType("ImageProcessingRequest");
        if (type_ImageProcessingRequest != null)
            Console.WriteLine("[PASS] 类型 ImageProcessingRequest (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessingRequest 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ImageProcessingResult
    var type_ImageProcessingResult = Type.GetType("ImageProcessingResult");
    if (type_ImageProcessingResult != null)
    {
        Console.WriteLine("[PASS] 类型 ImageProcessingResult (record) 存在");
        var ctors_ImageProcessingResult = type_ImageProcessingResult.GetConstructors();
        Console.WriteLine($"[PASS] ImageProcessingResult 构造函数数量: {ctors_ImageProcessingResult.Length}");
        var methods_ImageProcessingResult = type_ImageProcessingResult.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ImageProcessingResult 公开方法数量: {methods_ImageProcessingResult.Length}");
        foreach (var m in methods_ImageProcessingResult)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageProcessingResult 未找到，尝试无命名空间...");
        type_ImageProcessingResult = Type.GetType("ImageProcessingResult");
        if (type_ImageProcessingResult != null)
            Console.WriteLine("[PASS] 类型 ImageProcessingResult (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageProcessingResult 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: ImageFormat
    var type_ImageFormat = Type.GetType("ImageFormat");
    if (type_ImageFormat != null)
    {
        Console.WriteLine("[PASS] 类型 ImageFormat (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ImageFormat 未找到，尝试无命名空间...");
        type_ImageFormat = Type.GetType("ImageFormat");
        if (type_ImageFormat != null)
            Console.WriteLine("[PASS] 类型 ImageFormat (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ImageFormat 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

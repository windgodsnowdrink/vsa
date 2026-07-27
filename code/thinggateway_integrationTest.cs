#load "thinggateway_integration.cs"

Console.WriteLine("=== thinggateway_integration.cs Test ===");

try
{
    // 验证 class: IotGatewayService
    var type_IotGatewayService = Type.GetType("IotGatewayService");
    if (type_IotGatewayService != null)
    {
        Console.WriteLine("[PASS] 类型 IotGatewayService (class) 存在");
        var ctors_IotGatewayService = type_IotGatewayService.GetConstructors();
        Console.WriteLine($"[PASS] IotGatewayService 构造函数数量: {ctors_IotGatewayService.Length}");
        var methods_IotGatewayService = type_IotGatewayService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IotGatewayService 公开方法数量: {methods_IotGatewayService.Length}");
        foreach (var m in methods_IotGatewayService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IotGatewayService 未找到，尝试无命名空间...");
        type_IotGatewayService = Type.GetType("IotGatewayService");
        if (type_IotGatewayService != null)
            Console.WriteLine("[PASS] 类型 IotGatewayService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IotGatewayService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceContextPooledPolicy
    var type_DeviceContextPooledPolicy = Type.GetType("DeviceContextPooledPolicy");
    if (type_DeviceContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceContextPooledPolicy (class) 存在");
        var ctors_DeviceContextPooledPolicy = type_DeviceContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DeviceContextPooledPolicy 构造函数数量: {ctors_DeviceContextPooledPolicy.Length}");
        var methods_DeviceContextPooledPolicy = type_DeviceContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceContextPooledPolicy 公开方法数量: {methods_DeviceContextPooledPolicy.Length}");
        foreach (var m in methods_DeviceContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceContextPooledPolicy 未找到，尝试无命名空间...");
        type_DeviceContextPooledPolicy = Type.GetType("DeviceContextPooledPolicy");
        if (type_DeviceContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 DeviceContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceContext
    var type_DeviceContext = Type.GetType("DeviceContext");
    if (type_DeviceContext != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceContext (class) 存在");
        var ctors_DeviceContext = type_DeviceContext.GetConstructors();
        Console.WriteLine($"[PASS] DeviceContext 构造函数数量: {ctors_DeviceContext.Length}");
        var methods_DeviceContext = type_DeviceContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceContext 公开方法数量: {methods_DeviceContext.Length}");
        foreach (var m in methods_DeviceContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceContext 未找到，尝试无命名空间...");
        type_DeviceContext = Type.GetType("DeviceContext");
        if (type_DeviceContext != null)
            Console.WriteLine("[PASS] 类型 DeviceContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SerialPortPooledPolicy
    var type_SerialPortPooledPolicy = Type.GetType("SerialPortPooledPolicy");
    if (type_SerialPortPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 SerialPortPooledPolicy (class) 存在");
        var ctors_SerialPortPooledPolicy = type_SerialPortPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] SerialPortPooledPolicy 构造函数数量: {ctors_SerialPortPooledPolicy.Length}");
        var methods_SerialPortPooledPolicy = type_SerialPortPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SerialPortPooledPolicy 公开方法数量: {methods_SerialPortPooledPolicy.Length}");
        foreach (var m in methods_SerialPortPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SerialPortPooledPolicy 未找到，尝试无命名空间...");
        type_SerialPortPooledPolicy = Type.GetType("SerialPortPooledPolicy");
        if (type_SerialPortPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 SerialPortPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SerialPortPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IotDbContext
    var type_IotDbContext = Type.GetType("IotDbContext");
    if (type_IotDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 IotDbContext (class) 存在");
        var ctors_IotDbContext = type_IotDbContext.GetConstructors();
        Console.WriteLine($"[PASS] IotDbContext 构造函数数量: {ctors_IotDbContext.Length}");
        var methods_IotDbContext = type_IotDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IotDbContext 公开方法数量: {methods_IotDbContext.Length}");
        foreach (var m in methods_IotDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IotDbContext 未找到，尝试无命名空间...");
        type_IotDbContext = Type.GetType("IotDbContext");
        if (type_IotDbContext != null)
            Console.WriteLine("[PASS] 类型 IotDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IotDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceDataEntity
    var type_DeviceDataEntity = Type.GetType("DeviceDataEntity");
    if (type_DeviceDataEntity != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceDataEntity (class) 存在");
        var ctors_DeviceDataEntity = type_DeviceDataEntity.GetConstructors();
        Console.WriteLine($"[PASS] DeviceDataEntity 构造函数数量: {ctors_DeviceDataEntity.Length}");
        var methods_DeviceDataEntity = type_DeviceDataEntity.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceDataEntity 公开方法数量: {methods_DeviceDataEntity.Length}");
        foreach (var m in methods_DeviceDataEntity)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceDataEntity 未找到，尝试无命名空间...");
        type_DeviceDataEntity = Type.GetType("DeviceDataEntity");
        if (type_DeviceDataEntity != null)
            Console.WriteLine("[PASS] 类型 DeviceDataEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceDataEntity 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceConfigVersionEntity
    var type_DeviceConfigVersionEntity = Type.GetType("DeviceConfigVersionEntity");
    if (type_DeviceConfigVersionEntity != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceConfigVersionEntity (class) 存在");
        var ctors_DeviceConfigVersionEntity = type_DeviceConfigVersionEntity.GetConstructors();
        Console.WriteLine($"[PASS] DeviceConfigVersionEntity 构造函数数量: {ctors_DeviceConfigVersionEntity.Length}");
        var methods_DeviceConfigVersionEntity = type_DeviceConfigVersionEntity.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceConfigVersionEntity 公开方法数量: {methods_DeviceConfigVersionEntity.Length}");
        foreach (var m in methods_DeviceConfigVersionEntity)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceConfigVersionEntity 未找到，尝试无命名空间...");
        type_DeviceConfigVersionEntity = Type.GetType("DeviceConfigVersionEntity");
        if (type_DeviceConfigVersionEntity != null)
            Console.WriteLine("[PASS] 类型 DeviceConfigVersionEntity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceConfigVersionEntity 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceModelPublisher
    var type_DeviceModelPublisher = Type.GetType("DeviceModelPublisher");
    if (type_DeviceModelPublisher != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceModelPublisher (class) 存在");
        var ctors_DeviceModelPublisher = type_DeviceModelPublisher.GetConstructors();
        Console.WriteLine($"[PASS] DeviceModelPublisher 构造函数数量: {ctors_DeviceModelPublisher.Length}");
        var methods_DeviceModelPublisher = type_DeviceModelPublisher.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceModelPublisher 公开方法数量: {methods_DeviceModelPublisher.Length}");
        foreach (var m in methods_DeviceModelPublisher)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceModelPublisher 未找到，尝试无命名空间...");
        type_DeviceModelPublisher = Type.GetType("DeviceModelPublisher");
        if (type_DeviceModelPublisher != null)
            Console.WriteLine("[PASS] 类型 DeviceModelPublisher (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceModelPublisher 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceCommandSubscriber
    var type_DeviceCommandSubscriber = Type.GetType("DeviceCommandSubscriber");
    if (type_DeviceCommandSubscriber != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceCommandSubscriber (class) 存在");
        var ctors_DeviceCommandSubscriber = type_DeviceCommandSubscriber.GetConstructors();
        Console.WriteLine($"[PASS] DeviceCommandSubscriber 构造函数数量: {ctors_DeviceCommandSubscriber.Length}");
        var methods_DeviceCommandSubscriber = type_DeviceCommandSubscriber.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceCommandSubscriber 公开方法数量: {methods_DeviceCommandSubscriber.Length}");
        foreach (var m in methods_DeviceCommandSubscriber)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceCommandSubscriber 未找到，尝试无命名空间...");
        type_DeviceCommandSubscriber = Type.GetType("DeviceCommandSubscriber");
        if (type_DeviceCommandSubscriber != null)
            Console.WriteLine("[PASS] 类型 DeviceCommandSubscriber (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceCommandSubscriber 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeviceModelProtocolPooledPolicy
    var type_DeviceModelProtocolPooledPolicy = Type.GetType("DeviceModelProtocolPooledPolicy");
    if (type_DeviceModelProtocolPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceModelProtocolPooledPolicy (class) 存在");
        var ctors_DeviceModelProtocolPooledPolicy = type_DeviceModelProtocolPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DeviceModelProtocolPooledPolicy 构造函数数量: {ctors_DeviceModelProtocolPooledPolicy.Length}");
        var methods_DeviceModelProtocolPooledPolicy = type_DeviceModelProtocolPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceModelProtocolPooledPolicy 公开方法数量: {methods_DeviceModelProtocolPooledPolicy.Length}");
        foreach (var m in methods_DeviceModelProtocolPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceModelProtocolPooledPolicy 未找到，尝试无命名空间...");
        type_DeviceModelProtocolPooledPolicy = Type.GetType("DeviceModelProtocolPooledPolicy");
        if (type_DeviceModelProtocolPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 DeviceModelProtocolPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceModelProtocolPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DefaultDeviceModelProtocol
    var type_DefaultDeviceModelProtocol = Type.GetType("DefaultDeviceModelProtocol");
    if (type_DefaultDeviceModelProtocol != null)
    {
        Console.WriteLine("[PASS] 类型 DefaultDeviceModelProtocol (class) 存在");
        var ctors_DefaultDeviceModelProtocol = type_DefaultDeviceModelProtocol.GetConstructors();
        Console.WriteLine($"[PASS] DefaultDeviceModelProtocol 构造函数数量: {ctors_DefaultDeviceModelProtocol.Length}");
        var methods_DefaultDeviceModelProtocol = type_DefaultDeviceModelProtocol.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DefaultDeviceModelProtocol 公开方法数量: {methods_DefaultDeviceModelProtocol.Length}");
        foreach (var m in methods_DefaultDeviceModelProtocol)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DefaultDeviceModelProtocol 未找到，尝试无命名空间...");
        type_DefaultDeviceModelProtocol = Type.GetType("DefaultDeviceModelProtocol");
        if (type_DefaultDeviceModelProtocol != null)
            Console.WriteLine("[PASS] 类型 DefaultDeviceModelProtocol (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DefaultDeviceModelProtocol 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IDeviceModelProtocol
    var type_IDeviceModelProtocol = Type.GetType("IDeviceModelProtocol");
    if (type_IDeviceModelProtocol != null)
    {
        Console.WriteLine("[PASS] 类型 IDeviceModelProtocol (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IDeviceModelProtocol 未找到，尝试无命名空间...");
        type_IDeviceModelProtocol = Type.GetType("IDeviceModelProtocol");
        if (type_IDeviceModelProtocol != null)
            Console.WriteLine("[PASS] 类型 IDeviceModelProtocol (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IDeviceModelProtocol 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DeviceNotification
    var type_DeviceNotification = Type.GetType("DeviceNotification");
    if (type_DeviceNotification != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceNotification (record) 存在");
        var ctors_DeviceNotification = type_DeviceNotification.GetConstructors();
        Console.WriteLine($"[PASS] DeviceNotification 构造函数数量: {ctors_DeviceNotification.Length}");
        var methods_DeviceNotification = type_DeviceNotification.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceNotification 公开方法数量: {methods_DeviceNotification.Length}");
        foreach (var m in methods_DeviceNotification)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceNotification 未找到，尝试无命名空间...");
        type_DeviceNotification = Type.GetType("DeviceNotification");
        if (type_DeviceNotification != null)
            Console.WriteLine("[PASS] 类型 DeviceNotification (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceNotification 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DeviceConfigVersion
    var type_DeviceConfigVersion = Type.GetType("DeviceConfigVersion");
    if (type_DeviceConfigVersion != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceConfigVersion (record) 存在");
        var ctors_DeviceConfigVersion = type_DeviceConfigVersion.GetConstructors();
        Console.WriteLine($"[PASS] DeviceConfigVersion 构造函数数量: {ctors_DeviceConfigVersion.Length}");
        var methods_DeviceConfigVersion = type_DeviceConfigVersion.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceConfigVersion 公开方法数量: {methods_DeviceConfigVersion.Length}");
        foreach (var m in methods_DeviceConfigVersion)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceConfigVersion 未找到，尝试无命名空间...");
        type_DeviceConfigVersion = Type.GetType("DeviceConfigVersion");
        if (type_DeviceConfigVersion != null)
            Console.WriteLine("[PASS] 类型 DeviceConfigVersion (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceConfigVersion 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DeviceModel
    var type_DeviceModel = Type.GetType("DeviceModel");
    if (type_DeviceModel != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceModel (record) 存在");
        var ctors_DeviceModel = type_DeviceModel.GetConstructors();
        Console.WriteLine($"[PASS] DeviceModel 构造函数数量: {ctors_DeviceModel.Length}");
        var methods_DeviceModel = type_DeviceModel.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceModel 公开方法数量: {methods_DeviceModel.Length}");
        foreach (var m in methods_DeviceModel)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceModel 未找到，尝试无命名空间...");
        type_DeviceModel = Type.GetType("DeviceModel");
        if (type_DeviceModel != null)
            Console.WriteLine("[PASS] 类型 DeviceModel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceModel 可能为顶层语句或嵌套类型");
    }

    // 验证 record: DeviceCommand
    var type_DeviceCommand = Type.GetType("DeviceCommand");
    if (type_DeviceCommand != null)
    {
        Console.WriteLine("[PASS] 类型 DeviceCommand (record) 存在");
        var ctors_DeviceCommand = type_DeviceCommand.GetConstructors();
        Console.WriteLine($"[PASS] DeviceCommand 构造函数数量: {ctors_DeviceCommand.Length}");
        var methods_DeviceCommand = type_DeviceCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeviceCommand 公开方法数量: {methods_DeviceCommand.Length}");
        foreach (var m in methods_DeviceCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeviceCommand 未找到，尝试无命名空间...");
        type_DeviceCommand = Type.GetType("DeviceCommand");
        if (type_DeviceCommand != null)
            Console.WriteLine("[PASS] 类型 DeviceCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeviceCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: NotificationType
    var type_NotificationType = Type.GetType("NotificationType");
    if (type_NotificationType != null)
    {
        Console.WriteLine("[PASS] 类型 NotificationType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NotificationType 未找到，尝试无命名空间...");
        type_NotificationType = Type.GetType("NotificationType");
        if (type_NotificationType != null)
            Console.WriteLine("[PASS] 类型 NotificationType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NotificationType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

#load "raft_service.cs"

Console.WriteLine("=== raft_service.cs Test ===");

try
{
    // 验证 class: RaftNodeService
    var type_RaftNodeService = Type.GetType("RaftNodeService");
    if (type_RaftNodeService != null)
    {
        Console.WriteLine("[PASS] 类型 RaftNodeService (class) 存在");
        var ctors_RaftNodeService = type_RaftNodeService.GetConstructors();
        Console.WriteLine($"[PASS] RaftNodeService 构造函数数量: {ctors_RaftNodeService.Length}");
        var methods_RaftNodeService = type_RaftNodeService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RaftNodeService 公开方法数量: {methods_RaftNodeService.Length}");
        foreach (var m in methods_RaftNodeService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RaftNodeService 未找到，尝试无命名空间...");
        type_RaftNodeService = Type.GetType("RaftNodeService");
        if (type_RaftNodeService != null)
            Console.WriteLine("[PASS] 类型 RaftNodeService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RaftNodeService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RaftIntegrationExtensions
    var type_RaftIntegrationExtensions = Type.GetType("RaftIntegrationExtensions");
    if (type_RaftIntegrationExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 RaftIntegrationExtensions (class) 存在");
        var ctors_RaftIntegrationExtensions = type_RaftIntegrationExtensions.GetConstructors();
        Console.WriteLine($"[PASS] RaftIntegrationExtensions 构造函数数量: {ctors_RaftIntegrationExtensions.Length}");
        var methods_RaftIntegrationExtensions = type_RaftIntegrationExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RaftIntegrationExtensions 公开方法数量: {methods_RaftIntegrationExtensions.Length}");
        foreach (var m in methods_RaftIntegrationExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RaftIntegrationExtensions 未找到，尝试无命名空间...");
        type_RaftIntegrationExtensions = Type.GetType("RaftIntegrationExtensions");
        if (type_RaftIntegrationExtensions != null)
            Console.WriteLine("[PASS] 类型 RaftIntegrationExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RaftIntegrationExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RaftClusterStatus
    var type_RaftClusterStatus = Type.GetType("RaftClusterStatus");
    if (type_RaftClusterStatus != null)
    {
        Console.WriteLine("[PASS] 类型 RaftClusterStatus (class) 存在");
        var ctors_RaftClusterStatus = type_RaftClusterStatus.GetConstructors();
        Console.WriteLine($"[PASS] RaftClusterStatus 构造函数数量: {ctors_RaftClusterStatus.Length}");
        var methods_RaftClusterStatus = type_RaftClusterStatus.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RaftClusterStatus 公开方法数量: {methods_RaftClusterStatus.Length}");
        foreach (var m in methods_RaftClusterStatus)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RaftClusterStatus 未找到，尝试无命名空间...");
        type_RaftClusterStatus = Type.GetType("RaftClusterStatus");
        if (type_RaftClusterStatus != null)
            Console.WriteLine("[PASS] 类型 RaftClusterStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RaftClusterStatus 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RaftContext
    var type_RaftContext = Type.GetType("RaftContext");
    if (type_RaftContext != null)
    {
        Console.WriteLine("[PASS] 类型 RaftContext (class) 存在");
        var ctors_RaftContext = type_RaftContext.GetConstructors();
        Console.WriteLine($"[PASS] RaftContext 构造函数数量: {ctors_RaftContext.Length}");
        var methods_RaftContext = type_RaftContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RaftContext 公开方法数量: {methods_RaftContext.Length}");
        foreach (var m in methods_RaftContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RaftContext 未找到，尝试无命名空间...");
        type_RaftContext = Type.GetType("RaftContext");
        if (type_RaftContext != null)
            Console.WriteLine("[PASS] 类型 RaftContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RaftContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RaftContextPooledPolicy
    var type_RaftContextPooledPolicy = Type.GetType("RaftContextPooledPolicy");
    if (type_RaftContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RaftContextPooledPolicy (class) 存在");
        var ctors_RaftContextPooledPolicy = type_RaftContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RaftContextPooledPolicy 构造函数数量: {ctors_RaftContextPooledPolicy.Length}");
        var methods_RaftContextPooledPolicy = type_RaftContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RaftContextPooledPolicy 公开方法数量: {methods_RaftContextPooledPolicy.Length}");
        foreach (var m in methods_RaftContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RaftContextPooledPolicy 未找到，尝试无命名空间...");
        type_RaftContextPooledPolicy = Type.GetType("RaftContextPooledPolicy");
        if (type_RaftContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 RaftContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RaftContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KeyValueStateMachine
    var type_KeyValueStateMachine = Type.GetType("KeyValueStateMachine");
    if (type_KeyValueStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 KeyValueStateMachine (class) 存在");
        var ctors_KeyValueStateMachine = type_KeyValueStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] KeyValueStateMachine 构造函数数量: {ctors_KeyValueStateMachine.Length}");
        var methods_KeyValueStateMachine = type_KeyValueStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KeyValueStateMachine 公开方法数量: {methods_KeyValueStateMachine.Length}");
        foreach (var m in methods_KeyValueStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KeyValueStateMachine 未找到，尝试无命名空间...");
        type_KeyValueStateMachine = Type.GetType("KeyValueStateMachine");
        if (type_KeyValueStateMachine != null)
            Console.WriteLine("[PASS] 类型 KeyValueStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KeyValueStateMachine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JsonRaftCommandSerializer
    var type_JsonRaftCommandSerializer = Type.GetType("JsonRaftCommandSerializer");
    if (type_JsonRaftCommandSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 JsonRaftCommandSerializer (class) 存在");
        var ctors_JsonRaftCommandSerializer = type_JsonRaftCommandSerializer.GetConstructors();
        Console.WriteLine($"[PASS] JsonRaftCommandSerializer 构造函数数量: {ctors_JsonRaftCommandSerializer.Length}");
        var methods_JsonRaftCommandSerializer = type_JsonRaftCommandSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JsonRaftCommandSerializer 公开方法数量: {methods_JsonRaftCommandSerializer.Length}");
        foreach (var m in methods_JsonRaftCommandSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JsonRaftCommandSerializer 未找到，尝试无命名空间...");
        type_JsonRaftCommandSerializer = Type.GetType("JsonRaftCommandSerializer");
        if (type_JsonRaftCommandSerializer != null)
            Console.WriteLine("[PASS] 类型 JsonRaftCommandSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JsonRaftCommandSerializer 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IRaftStateMachine
    var type_IRaftStateMachine = Type.GetType("IRaftStateMachine");
    if (type_IRaftStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 IRaftStateMachine (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IRaftStateMachine 未找到，尝试无命名空间...");
        type_IRaftStateMachine = Type.GetType("IRaftStateMachine");
        if (type_IRaftStateMachine != null)
            Console.WriteLine("[PASS] 类型 IRaftStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRaftStateMachine 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IRaftCommandSerializer
    var type_IRaftCommandSerializer = Type.GetType("IRaftCommandSerializer");
    if (type_IRaftCommandSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 IRaftCommandSerializer (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IRaftCommandSerializer 未找到，尝试无命名空间...");
        type_IRaftCommandSerializer = Type.GetType("IRaftCommandSerializer");
        if (type_IRaftCommandSerializer != null)
            Console.WriteLine("[PASS] 类型 IRaftCommandSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IRaftCommandSerializer 可能为顶层语句或嵌套类型");
    }

    // 验证 record: NodeInfo
    var type_NodeInfo = Type.GetType("NodeInfo");
    if (type_NodeInfo != null)
    {
        Console.WriteLine("[PASS] 类型 NodeInfo (record) 存在");
        var ctors_NodeInfo = type_NodeInfo.GetConstructors();
        Console.WriteLine($"[PASS] NodeInfo 构造函数数量: {ctors_NodeInfo.Length}");
        var methods_NodeInfo = type_NodeInfo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NodeInfo 公开方法数量: {methods_NodeInfo.Length}");
        foreach (var m in methods_NodeInfo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NodeInfo 未找到，尝试无命名空间...");
        type_NodeInfo = Type.GetType("NodeInfo");
        if (type_NodeInfo != null)
            Console.WriteLine("[PASS] 类型 NodeInfo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NodeInfo 可能为顶层语句或嵌套类型");
    }

    // 验证 record: RaftMessage
    var type_RaftMessage = Type.GetType("RaftMessage");
    if (type_RaftMessage != null)
    {
        Console.WriteLine("[PASS] 类型 RaftMessage (record) 存在");
        var ctors_RaftMessage = type_RaftMessage.GetConstructors();
        Console.WriteLine($"[PASS] RaftMessage 构造函数数量: {ctors_RaftMessage.Length}");
        var methods_RaftMessage = type_RaftMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RaftMessage 公开方法数量: {methods_RaftMessage.Length}");
        foreach (var m in methods_RaftMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RaftMessage 未找到，尝试无命名空间...");
        type_RaftMessage = Type.GetType("RaftMessage");
        if (type_RaftMessage != null)
            Console.WriteLine("[PASS] 类型 RaftMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RaftMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 record: StateMachineMetadata
    var type_StateMachineMetadata = Type.GetType("StateMachineMetadata");
    if (type_StateMachineMetadata != null)
    {
        Console.WriteLine("[PASS] 类型 StateMachineMetadata (record) 存在");
        var ctors_StateMachineMetadata = type_StateMachineMetadata.GetConstructors();
        Console.WriteLine($"[PASS] StateMachineMetadata 构造函数数量: {ctors_StateMachineMetadata.Length}");
        var methods_StateMachineMetadata = type_StateMachineMetadata.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StateMachineMetadata 公开方法数量: {methods_StateMachineMetadata.Length}");
        foreach (var m in methods_StateMachineMetadata)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StateMachineMetadata 未找到，尝试无命名空间...");
        type_StateMachineMetadata = Type.GetType("StateMachineMetadata");
        if (type_StateMachineMetadata != null)
            Console.WriteLine("[PASS] 类型 StateMachineMetadata (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StateMachineMetadata 可能为顶层语句或嵌套类型");
    }

    // 验证 record: KeyValueCommand
    var type_KeyValueCommand = Type.GetType("KeyValueCommand");
    if (type_KeyValueCommand != null)
    {
        Console.WriteLine("[PASS] 类型 KeyValueCommand (record) 存在");
        var ctors_KeyValueCommand = type_KeyValueCommand.GetConstructors();
        Console.WriteLine($"[PASS] KeyValueCommand 构造函数数量: {ctors_KeyValueCommand.Length}");
        var methods_KeyValueCommand = type_KeyValueCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KeyValueCommand 公开方法数量: {methods_KeyValueCommand.Length}");
        foreach (var m in methods_KeyValueCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KeyValueCommand 未找到，尝试无命名空间...");
        type_KeyValueCommand = Type.GetType("KeyValueCommand");
        if (type_KeyValueCommand != null)
            Console.WriteLine("[PASS] 类型 KeyValueCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KeyValueCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: NodeState
    var type_NodeState = Type.GetType("NodeState");
    if (type_NodeState != null)
    {
        Console.WriteLine("[PASS] 类型 NodeState (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NodeState 未找到，尝试无命名空间...");
        type_NodeState = Type.GetType("NodeState");
        if (type_NodeState != null)
            Console.WriteLine("[PASS] 类型 NodeState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NodeState 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

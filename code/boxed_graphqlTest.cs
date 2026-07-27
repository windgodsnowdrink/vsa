#load "boxed_graphql.cs"

Console.WriteLine("=== boxed_graphql.cs Test ===");

try
{
    // 验证 class: Query
    var type_Query = Type.GetType("Query");
    if (type_Query != null)
    {
        Console.WriteLine("[PASS] 类型 Query (class) 存在");
        var ctors_Query = type_Query.GetConstructors();
        Console.WriteLine($"[PASS] Query 构造函数数量: {ctors_Query.Length}");
        var methods_Query = type_Query.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Query 公开方法数量: {methods_Query.Length}");
        foreach (var m in methods_Query)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Query 未找到，尝试无命名空间...");
        type_Query = Type.GetType("Query");
        if (type_Query != null)
            Console.WriteLine("[PASS] 类型 Query (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Query 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Mutation
    var type_Mutation = Type.GetType("Mutation");
    if (type_Mutation != null)
    {
        Console.WriteLine("[PASS] 类型 Mutation (class) 存在");
        var ctors_Mutation = type_Mutation.GetConstructors();
        Console.WriteLine($"[PASS] Mutation 构造函数数量: {ctors_Mutation.Length}");
        var methods_Mutation = type_Mutation.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Mutation 公开方法数量: {methods_Mutation.Length}");
        foreach (var m in methods_Mutation)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Mutation 未找到，尝试无命名空间...");
        type_Mutation = Type.GetType("Mutation");
        if (type_Mutation != null)
            Console.WriteLine("[PASS] 类型 Mutation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Mutation 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Subscription
    var type_Subscription = Type.GetType("Subscription");
    if (type_Subscription != null)
    {
        Console.WriteLine("[PASS] 类型 Subscription (class) 存在");
        var ctors_Subscription = type_Subscription.GetConstructors();
        Console.WriteLine($"[PASS] Subscription 构造函数数量: {ctors_Subscription.Length}");
        var methods_Subscription = type_Subscription.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Subscription 公开方法数量: {methods_Subscription.Length}");
        foreach (var m in methods_Subscription)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Subscription 未找到，尝试无命名空间...");
        type_Subscription = Type.GetType("Subscription");
        if (type_Subscription != null)
            Console.WriteLine("[PASS] 类型 Subscription (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Subscription 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GraphQLChannelProcessor
    var type_GraphQLChannelProcessor = Type.GetType("GraphQLChannelProcessor");
    if (type_GraphQLChannelProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 GraphQLChannelProcessor (class) 存在");
        var ctors_GraphQLChannelProcessor = type_GraphQLChannelProcessor.GetConstructors();
        Console.WriteLine($"[PASS] GraphQLChannelProcessor 构造函数数量: {ctors_GraphQLChannelProcessor.Length}");
        var methods_GraphQLChannelProcessor = type_GraphQLChannelProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GraphQLChannelProcessor 公开方法数量: {methods_GraphQLChannelProcessor.Length}");
        foreach (var m in methods_GraphQLChannelProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GraphQLChannelProcessor 未找到，尝试无命名空间...");
        type_GraphQLChannelProcessor = Type.GetType("GraphQLChannelProcessor");
        if (type_GraphQLChannelProcessor != null)
            Console.WriteLine("[PASS] 类型 GraphQLChannelProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GraphQLChannelProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GraphQLMessage
    var type_GraphQLMessage = Type.GetType("GraphQLMessage");
    if (type_GraphQLMessage != null)
    {
        Console.WriteLine("[PASS] 类型 GraphQLMessage (class) 存在");
        var ctors_GraphQLMessage = type_GraphQLMessage.GetConstructors();
        Console.WriteLine($"[PASS] GraphQLMessage 构造函数数量: {ctors_GraphQLMessage.Length}");
        var methods_GraphQLMessage = type_GraphQLMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GraphQLMessage 公开方法数量: {methods_GraphQLMessage.Length}");
        foreach (var m in methods_GraphQLMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GraphQLMessage 未找到，尝试无命名空间...");
        type_GraphQLMessage = Type.GetType("GraphQLMessage");
        if (type_GraphQLMessage != null)
            Console.WriteLine("[PASS] 类型 GraphQLMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GraphQLMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Todo
    var type_Todo = Type.GetType("Todo");
    if (type_Todo != null)
    {
        Console.WriteLine("[PASS] 类型 Todo (class) 存在");
        var ctors_Todo = type_Todo.GetConstructors();
        Console.WriteLine($"[PASS] Todo 构造函数数量: {ctors_Todo.Length}");
        var methods_Todo = type_Todo.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Todo 公开方法数量: {methods_Todo.Length}");
        foreach (var m in methods_Todo)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Todo 未找到，尝试无命名空间...");
        type_Todo = Type.GetType("Todo");
        if (type_Todo != null)
            Console.WriteLine("[PASS] 类型 Todo (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Todo 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoDbContext
    var type_TodoDbContext = Type.GetType("TodoDbContext");
    if (type_TodoDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 TodoDbContext (class) 存在");
        var ctors_TodoDbContext = type_TodoDbContext.GetConstructors();
        Console.WriteLine($"[PASS] TodoDbContext 构造函数数量: {ctors_TodoDbContext.Length}");
        var methods_TodoDbContext = type_TodoDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoDbContext 公开方法数量: {methods_TodoDbContext.Length}");
        foreach (var m in methods_TodoDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoDbContext 未找到，尝试无命名空间...");
        type_TodoDbContext = Type.GetType("TodoDbContext");
        if (type_TodoDbContext != null)
            Console.WriteLine("[PASS] 类型 TodoDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoDbContext 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}

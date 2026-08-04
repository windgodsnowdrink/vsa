// 自定义加载上下文
public class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}

// 增强的插件加载器
public class IsolatedPluginLoader : IPluginLoader
{
    private readonly Dictionary<string, (PluginLoadContext, Assembly)> _loadedAssemblies = new();

    public IEnumerable<ICarterModule> LoadModulesFromDll(string dllPath)
    {
        var context = new PluginLoadContext(dllPath);
        var assembly = context.LoadFromAssemblyPath(dllPath);
        _loadedAssemblies[dllPath] = (context, assembly);

        return assembly.GetTypes()
            .Where(t => typeof(ICarterModule).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => (ICarterModule)Activator.CreateInstance(t));
    }

    public void UnloadDll(string dllPath)
    {
        if (_loadedAssemblies.TryGetValue(dllPath, out var entry))
        {
            entry.Item1.Unload();
            _loadedAssemblies.Remove(dllPath);
        }
    }
}

跨应用域通信 ：
- 使用 AssemblyLoadContext 创建隔离的加载上下文
- 通过 MarshalByRefObject 或gRPC实现跨域通信

// 创建隔离的加载上下文
var alc = new AssemblyLoadContext("PluginContext", true);
// 加载DLL
var assembly = alc.LoadFromAssemblyPath(dllPath);

依赖项隔离 ：
- 实现自定义 PluginLoadContext 继承 AssemblyLoadContext
- 重写 Load 方法控制依赖解析
- 使用 DependencyContext 管理依赖树

protected override Assembly Load(AssemblyName assemblyName)
{
    // 自定义依赖解析逻辑
    if(IsSharedDependency(assemblyName)) 
        return Default.LoadFromAssemblyName(assemblyName);
    return base.Load(assemblyName);
}

DLL签名验证 ：
- 使用 AuthenticodeSignatureVerifier 验证强名称签名
- 实现证书链验证和吊销列表检查

var cert = X509Certificate.CreateFromSignedFile(dllPath);
var chain = new X509Chain();
chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
if(!chain.Build(cert))
    throw new SecurityException("Invalid DLL signature");
    
// 加载DLL
var assembly = alc.LoadFromAssemblyPath(dllPath);

// 验证签名
var verifier = new AuthenticodeSignatureVerifier();
var result = verifier.VerifySignature(assembly);
if(result != SignatureVerificationResult.Valid)
{
    throw new InvalidOperationException("DLL 签名验证失败");
}
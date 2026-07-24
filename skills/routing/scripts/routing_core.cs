#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
#:package System.Text.Json@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Routing.Core
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Routing 核心工具");
            
            // 创建添加路由命令
            var addCommand = new Command("add", "添加新路由");
            var pathOption = new Option<string>("--path", "路由路径");
            var methodOption = new Option<string>("--method", () => "GET", "HTTP 方法");
            var handlerOption = new Option<string>("--handler", "路由处理程序");
            addCommand.AddOption(pathOption);
            addCommand.AddOption(methodOption);
            addCommand.AddOption(handlerOption);
            addCommand.SetHandler(async (context) =>
            {
                var path = context.ParseResult.GetValueForOption(pathOption);
                var method = context.ParseResult.GetValueForOption(methodOption);
                var handler = context.ParseResult.GetValueForOption(handlerOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleAddCommand(serviceProvider, path, method, handler, cancellationToken);
            });
            
            // 创建列出路由命令
            var listCommand = new Command("list", "列出所有路由");
            var formatOption = new Option<string>("--format", () => "json", "输出格式");
            listCommand.AddOption(formatOption);
            listCommand.SetHandler(async (context) =>
            {
                var format = context.ParseResult.GetValueForOption(formatOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleListCommand(serviceProvider, format, cancellationToken);
            });
            
            // 创建删除路由命令
            var removeCommand = new Command("remove", "删除路由");
            var removePathOption = new Option<string>("--path", "路由路径");
            var removeMethodOption = new Option<string>("--method", () => "GET", "HTTP 方法");
            removeCommand.AddOption(removePathOption);
            removeCommand.AddOption(removeMethodOption);
            removeCommand.SetHandler(async (context) =>
            {
                var path = context.ParseResult.GetValueForOption(removePathOption);
                var method = context.ParseResult.GetValueForOption(removeMethodOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleRemoveCommand(serviceProvider, path, method, cancellationToken);
            });
            
            // 创建添加中间件命令
            var addMiddlewareCommand = new Command("add-middleware", "添加全局中间件");
            var middlewareNameOption = new Option<string>("--name", "中间件名称");
            var middlewareHandlerOption = new Option<string>("--handler", "中间件处理程序");
            addMiddlewareCommand.AddOption(middlewareNameOption);
            addMiddlewareCommand.AddOption(middlewareHandlerOption);
            addMiddlewareCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(middlewareNameOption);
                var handler = context.ParseResult.GetValueForOption(middlewareHandlerOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleAddMiddlewareCommand(serviceProvider, name, handler, cancellationToken);
            });
            
            // 创建添加路由中间件命令
            var addRouteMiddlewareCommand = new Command("add-route-middleware", "添加路由级中间件");
            var routeMiddlewarePathOption = new Option<string>("--path", "路由路径");
            var routeMiddlewareMethodOption = new Option<string>("--method", () => "GET", "HTTP 方法");
            var routeMiddlewareNameOption = new Option<string>("--name", "中间件名称");
            var routeMiddlewareHandlerOption = new Option<string>("--handler", "中间件处理程序");
            addRouteMiddlewareCommand.AddOption(routeMiddlewarePathOption);
            addRouteMiddlewareCommand.AddOption(routeMiddlewareMethodOption);
            addRouteMiddlewareCommand.AddOption(routeMiddlewareNameOption);
            addRouteMiddlewareCommand.AddOption(routeMiddlewareHandlerOption);
            addRouteMiddlewareCommand.SetHandler(async (context) =>
            {
                var path = context.ParseResult.GetValueForOption(routeMiddlewarePathOption);
                var method = context.ParseResult.GetValueForOption(routeMiddlewareMethodOption);
                var name = context.ParseResult.GetValueForOption(routeMiddlewareNameOption);
                var handler = context.ParseResult.GetValueForOption(routeMiddlewareHandlerOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleAddRouteMiddlewareCommand(serviceProvider, path, method, name, handler, cancellationToken);
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(removeCommand);
            rootCommand.AddCommand(addMiddlewareCommand);
            rootCommand.AddCommand(addRouteMiddlewareCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 配置配置管理
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
            
            // 注册服务
            services.AddSingleton<IRouteManager, RouteManager>();
            services.AddSingleton<IRouteParser, RouteParser>();
            services.AddSingleton<IMiddlewareManager, MiddlewareManager>();
            services.AddSingleton<IRouteStore, FileRouteStore>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleAddCommand(IServiceProvider serviceProvider, string path, string method, string handler, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("错误: 必须指定路由路径");
                return;
            }
            
            if (string.IsNullOrEmpty(handler))
            {
                Console.WriteLine("错误: 必须指定路由处理程序");
                return;
            }
            
            try
            {
                var routeManager = serviceProvider.GetRequiredService<IRouteManager>();
                var route = await routeManager.AddRouteAsync(path, method, handler, cancellationToken);
                
                Console.WriteLine($"成功添加路由: {route.Method} {route.Path} -> {route.Handler}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加路由时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleListCommand(IServiceProvider serviceProvider, string format, CancellationToken cancellationToken)
        {
            try
            {
                var routeManager = serviceProvider.GetRequiredService<IRouteManager>();
                var routes = await routeManager.GetRoutesAsync(cancellationToken);
                
                switch (format.ToLower())
                {
                    case "json":
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        var jsonContent = JsonSerializer.Serialize(routes, jsonOptions);
                        Console.WriteLine(jsonContent);
                        break;
                    case "yaml":
                        // 简单的 YAML 格式输出
                        foreach (var route in routes)
                        {
                            Console.WriteLine($"- path: {route.Path}");
                            Console.WriteLine($"  method: {route.Method}");
                            Console.WriteLine($"  handler: {route.Handler}");
                            if (route.Parameters.Any())
                            {
                                Console.WriteLine("  parameters:");
                                foreach (var param in route.Parameters)
                                {
                                    Console.WriteLine($"    - name: {param.Name}");
                                    Console.WriteLine($"      type: {param.Type}");
                                    Console.WriteLine($"      source: {param.Source}");
                                }
                            }
                            Console.WriteLine();
                        }
                        break;
                    case "text":
                    default:
                        foreach (var route in routes)
                        {
                            Console.WriteLine($"{route.Method.PadRight(8)} {route.Path} -> {route.Handler}");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"列出路由时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleRemoveCommand(IServiceProvider serviceProvider, string path, string method, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("错误: 必须指定路由路径");
                return;
            }
            
            try
            {
                var routeManager = serviceProvider.GetRequiredService<IRouteManager>();
                var success = await routeManager.RemoveRouteAsync(path, method, cancellationToken);
                
                if (success)
                {
                    Console.WriteLine($"成功删除路由: {method} {path}");
                }
                else
                {
                    Console.WriteLine($"未找到路由: {method} {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除路由时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleAddMiddlewareCommand(IServiceProvider serviceProvider, string name, string handler, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("错误: 必须指定中间件名称");
                return;
            }
            
            if (string.IsNullOrEmpty(handler))
            {
                Console.WriteLine("错误: 必须指定中间件处理程序");
                return;
            }
            
            try
            {
                var middlewareManager = serviceProvider.GetRequiredService<IMiddlewareManager>();
                var middleware = await middlewareManager.AddGlobalMiddlewareAsync(name, handler, cancellationToken);
                
                Console.WriteLine($"成功添加全局中间件: {middleware.Name} -> {middleware.Handler}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加中间件时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleAddRouteMiddlewareCommand(IServiceProvider serviceProvider, string path, string method, string name, string handler, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("错误: 必须指定路由路径");
                return;
            }
            
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("错误: 必须指定中间件名称");
                return;
            }
            
            if (string.IsNullOrEmpty(handler))
            {
                Console.WriteLine("错误: 必须指定中间件处理程序");
                return;
            }
            
            try
            {
                var routeManager = serviceProvider.GetRequiredService<IRouteManager>();
                var success = await routeManager.AddRouteMiddlewareAsync(path, method, name, handler, cancellationToken);
                
                if (success)
                {
                    Console.WriteLine($"成功添加路由中间件: {name} -> {handler} 到 {method} {path}");
                }
                else
                {
                    Console.WriteLine($"未找到路由: {method} {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加路由中间件时发生错误: {ex.Message}");
            }
        }
    }
    
    // 服务接口
    public interface IRouteManager
    {
        Task<Route> AddRouteAsync(string path, string method, string handler, CancellationToken cancellationToken = default);
        Task<IEnumerable<Route>> GetRoutesAsync(CancellationToken cancellationToken = default);
        Task<bool> RemoveRouteAsync(string path, string method, CancellationToken cancellationToken = default);
        Task<bool> AddRouteMiddlewareAsync(string path, string method, string middlewareName, string middlewareHandler, CancellationToken cancellationToken = default);
    }
    
    public interface IRouteParser
    {
        Route Parse(string path, string method, string handler);
        Dictionary<string, string> ParseParameters(string routePath, string requestPath);
    }
    
    public interface IMiddlewareManager
    {
        Task<MiddlewareInfo> AddGlobalMiddlewareAsync(string name, string handler, CancellationToken cancellationToken = default);
        Task<IEnumerable<MiddlewareInfo>> GetGlobalMiddlewaresAsync(CancellationToken cancellationToken = default);
        Task<bool> RemoveGlobalMiddlewareAsync(string name, CancellationToken cancellationToken = default);
    }
    
    public interface IRouteStore
    {
        Task<IEnumerable<Route>> LoadRoutesAsync(CancellationToken cancellationToken = default);
        Task SaveRoutesAsync(IEnumerable<Route> routes, CancellationToken cancellationToken = default);
    }
    
    // 数据结构
    public class Route
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string Handler { get; set; }
        public List<RouteParameter> Parameters { get; set; } = new List<RouteParameter>();
        public List<MiddlewareInfo> Middlewares { get; set; } = new List<MiddlewareInfo>();
    }
    
    public class RouteParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Source { get; set; } // path, query, form
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
    }
    
    public class MiddlewareInfo
    {
        public string Name { get; set; }
        public string Handler { get; set; }
        public int Order { get; set; }
    }
    
    // 实现类
    public class RouteManager : IRouteManager
    {
        private readonly IRouteParser _routeParser;
        private readonly IRouteStore _routeStore;
        
        public RouteManager(IRouteParser routeParser, IRouteStore routeStore)
        {
            _routeParser = routeParser;
            _routeStore = routeStore;
        }
        
        public async Task<Route> AddRouteAsync(string path, string method, string handler, CancellationToken cancellationToken = default)
        {
            // 解析路由
            var route = _routeParser.Parse(path, method, handler);
            
            // 加载现有路由
            var routes = await _routeStore.LoadRoutesAsync(cancellationToken);
            
            // 检查路由冲突
            var existingRoute = routes.FirstOrDefault(r => r.Path == route.Path && r.Method.Equals(method, StringComparison.OrdinalIgnoreCase));
            if (existingRoute != null)
            {
                throw new InvalidOperationException($"路由冲突: {method} {path} 已存在");
            }
            
            // 添加新路由
            var updatedRoutes = routes.Append(route).ToList();
            await _routeStore.SaveRoutesAsync(updatedRoutes, cancellationToken);
            
            return route;
        }
        
        public async Task<IEnumerable<Route>> GetRoutesAsync(CancellationToken cancellationToken = default)
        {
            return await _routeStore.LoadRoutesAsync(cancellationToken);
        }
        
        public async Task<bool> RemoveRouteAsync(string path, string method, CancellationToken cancellationToken = default)
        {
            // 加载现有路由
            var routes = await _routeStore.LoadRoutesAsync(cancellationToken);
            
            // 查找路由
            var routeToRemove = routes.FirstOrDefault(r => r.Path == path && r.Method.Equals(method, StringComparison.OrdinalIgnoreCase));
            if (routeToRemove == null)
            {
                return false;
            }
            
            // 删除路由
            var updatedRoutes = routes.Where(r => !(r.Path == path && r.Method.Equals(method, StringComparison.OrdinalIgnoreCase))).ToList();
            await _routeStore.SaveRoutesAsync(updatedRoutes, cancellationToken);
            
            return true;
        }
        
        public async Task<bool> AddRouteMiddlewareAsync(string path, string method, string middlewareName, string middlewareHandler, CancellationToken cancellationToken = default)
        {
            // 加载现有路由
            var routes = await _routeStore.LoadRoutesAsync(cancellationToken);
            
            // 查找路由
            var route = routes.FirstOrDefault(r => r.Path == path && r.Method.Equals(method, StringComparison.OrdinalIgnoreCase));
            if (route == null)
            {
                return false;
            }
            
            // 添加中间件
            var middleware = new MiddlewareInfo
            {
                Name = middlewareName,
                Handler = middlewareHandler,
                Order = route.Middlewares.Count
            };
            
            route.Middlewares.Add(middleware);
            
            // 保存更新后的路由
            await _routeStore.SaveRoutesAsync(routes, cancellationToken);
            
            return true;
        }
    }
    
    public class RouteParser : IRouteParser
    {
        public Route Parse(string path, string method, string handler)
        {
            var route = new Route
            {
                Path = path,
                Method = method.ToUpper(),
                Handler = handler
            };
            
            // 解析路径参数
            var pathParameterRegex = new Regex(@"{([^}]+)}");
            var matches = pathParameterRegex.Matches(path);
            
            foreach (Match match in matches)
            {
                var parameterName = match.Groups[1].Value;
                var parameter = new RouteParameter
                {
                    Name = parameterName,
                    Type = "string",
                    Source = "path",
                    Required = true
                };
                
                route.Parameters.Add(parameter);
            }
            
            return route;
        }
        
        public Dictionary<string, string> ParseParameters(string routePath, string requestPath)
        {
            var parameters = new Dictionary<string, string>();
            
            // 替换路径参数为正则表达式
            var regexPattern = Regex.Replace(routePath, @"{([^}]+)}", @"(?<$1>[^/]+)");
            regexPattern = "^" + regexPattern + "$";
            
            var regex = new Regex(regexPattern);
            var match = regex.Match(requestPath);
            
            if (match.Success)
            {
                foreach (var groupName in regex.GetGroupNames())
                {
                    if (groupName != "0") // 跳过整个匹配
                    {
                        parameters[groupName] = match.Groups[groupName].Value;
                    }
                }
            }
            
            return parameters;
        }
    }
    
    public class MiddlewareManager : IMiddlewareManager
    {
        private const string MiddlewareStorePath = "middlewares.json";
        
        public async Task<MiddlewareInfo> AddGlobalMiddlewareAsync(string name, string handler, CancellationToken cancellationToken = default)
        {
            // 加载现有中间件
            var middlewares = await LoadMiddlewaresAsync(cancellationToken);
            
            // 检查中间件是否存在
            var existingMiddleware = middlewares.FirstOrDefault(m => m.Name == name);
            if (existingMiddleware != null)
            {
                throw new InvalidOperationException($"中间件 {name} 已存在");
            }
            
            // 创建新中间件
            var middleware = new MiddlewareInfo
            {
                Name = name,
                Handler = handler,
                Order = middlewares.Count
            };
            
            // 添加中间件
            var updatedMiddlewares = middlewares.Append(middleware).ToList();
            await SaveMiddlewaresAsync(updatedMiddlewares, cancellationToken);
            
            return middleware;
        }
        
        public async Task<IEnumerable<MiddlewareInfo>> GetGlobalMiddlewaresAsync(CancellationToken cancellationToken = default)
        {
            return await LoadMiddlewaresAsync(cancellationToken);
        }
        
        public async Task<bool> RemoveGlobalMiddlewareAsync(string name, CancellationToken cancellationToken = default)
        {
            // 加载现有中间件
            var middlewares = await LoadMiddlewaresAsync(cancellationToken);
            
            // 查找中间件
            var middlewareToRemove = middlewares.FirstOrDefault(m => m.Name == name);
            if (middlewareToRemove == null)
            {
                return false;
            }
            
            // 删除中间件
            var updatedMiddlewares = middlewares.Where(m => m.Name != name).ToList();
            
            // 更新顺序
            for (int i = 0; i < updatedMiddlewares.Count; i++)
            {
                updatedMiddlewares[i].Order = i;
            }
            
            await SaveMiddlewaresAsync(updatedMiddlewares, cancellationToken);
            
            return true;
        }
        
        private async Task<List<MiddlewareInfo>> LoadMiddlewaresAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(MiddlewareStorePath))
            {
                return new List<MiddlewareInfo>();
            }
            
            var jsonContent = await File.ReadAllTextAsync(MiddlewareStorePath, cancellationToken);
            return JsonSerializer.Deserialize<List<MiddlewareInfo>>(jsonContent) ?? new List<MiddlewareInfo>();
        }
        
        private async Task SaveMiddlewaresAsync(List<MiddlewareInfo> middlewares, CancellationToken cancellationToken = default)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(middlewares, jsonOptions);
            await File.WriteAllTextAsync(MiddlewareStorePath, jsonContent, cancellationToken);
        }
    }
    
    public class FileRouteStore : IRouteStore
    {
        private const string RouteStorePath = "routes.json";
        
        public async Task<IEnumerable<Route>> LoadRoutesAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(RouteStorePath))
            {
                return new List<Route>();
            }
            
            var jsonContent = await File.ReadAllTextAsync(RouteStorePath, cancellationToken);
            return JsonSerializer.Deserialize<List<Route>>(jsonContent) ?? new List<Route>();
        }
        
        public async Task SaveRoutesAsync(IEnumerable<Route> routes, CancellationToken cancellationToken = default)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(routes, jsonOptions);
            await File.WriteAllTextAsync(RouteStorePath, jsonContent, cancellationToken);
        }
    }
}

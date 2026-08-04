# Source Generator 技能使用示例

## 1. Scrutor 用法示例

### 1.1 按约定注册服务

```csharp
// 示例1：按类名后缀注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Generator")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// 示例2：按接口注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Implement<ICodeGeneratorService>())
    .As<ICodeGeneratorService>()
    .WithTransientLifetime()
);

// 示例3：按命名空间注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.InNamespaces("SourceGenerator.Generator.Generators"))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);
```

### 1.2 按属性注册服务

```csharp
// 定义生成器属性
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GeneratorAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public GeneratorAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

// 应用属性到生成器类
[GeneratorAttribute("ClassGenerator", "Generates C# class files")]
public class ClassGenerator
{
    public string GenerateClass(string className, string @namespace, IEnumerable<string> properties)
    {
        // 实现...
    }
}

// 按属性注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.WithAttribute<GeneratorAttribute>())
    .AsSelfWithInterfaces()
    .WithSingletonLifetime()
);
```

### 1.3 注册装饰器

```csharp
// 定义装饰器
public class CodeGeneratorLoggingDecorator : ICodeGeneratorService
{
    private readonly ICodeGeneratorService _inner;
    private readonly ILoggerService _logger;

    public CodeGeneratorLoggingDecorator(ICodeGeneratorService inner, ILoggerService logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters)
    {
        _logger.LogInformation("Generating code for template: {TemplateName}", templateName);
        try
        {
            var result = await _inner.GenerateCodeAsync(templateName, parameters);
            _logger.LogInformation("Code generated successfully for template: {TemplateName}", templateName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error generating code for template {TemplateName}: {Error}", templateName, ex.Message);
            throw;
        }
    }

    // 实现其他方法...
}

// 注册装饰器
services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();
services.Decorate<ICodeGeneratorService, CodeGeneratorValidationDecorator>();

// 带条件的装饰器
services.Decorate<ITemplateService>((inner, provider) =>
    new TemplateServiceCachingDecorator(inner, provider.GetRequiredService<ICacheService>())
);
```

### 1.4 注册开放泛型

```csharp
// 定义泛型接口和实现
public interface IRepository<T>
{
    Task<T> GetAsync(int id);
    Task SaveAsync(T entity);
}

public class Repository<T> : IRepository<T>
{
    public Task<T> GetAsync(int id)
    {
        // 实现...
        return Task.FromResult(default(T)!);
    }

    public Task SaveAsync(T entity)
    {
        // 实现...
        return Task.CompletedTask;
    }
}

// 注册开放泛型
services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

// 使用泛型服务
public class UserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _userRepository.GetAsync(id);
    }
}
```

### 1.5 高级注册模式

```csharp
// 示例1：组合注册
services.Scan(scan => scan
    .FromAssembliesOf(typeof(Program), typeof(AnotherAssemblyType))
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

// 示例2：条件注册
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(c => 
        c.Name.EndsWith("Generator") && 
        c.GetCustomAttributes<GeneratorAttribute>().Any()
    ))
    .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name))
    .WithTransientLifetime()
);

// 示例3：自定义生命周期
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Factory")))
    .AsImplementedInterfaces()
    .WithLifetime(() => ServiceLifetime.Singleton)
);
```

## 2. 代码生成示例

### 2.1 生成类文件

```csharp
// 命令行示例
sourcegenerator_generator.exe generate --template ClassGenerator --output ./Generated/MyClass.cs --parameters namespace=MyProject className=MyClass properties=Id:int,Name:string,Description:string

// 生成的代码示例
namespace MyProject
{
    public class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
```

### 2.2 生成接口文件

```csharp
// 命令行示例
sourcegenerator_generator.exe generate --template InterfaceGenerator --output ./Generated/IMyService.cs --parameters namespace=MyProject interfaceName=IMyService methods=Task<MyClass> GetByIdAsync(int id),Task SaveAsync(MyClass entity),Task DeleteAsync(int id)

// 生成的代码示例
namespace MyProject
{
    public interface IMyService
    {
        Task<MyClass> GetByIdAsync(int id);
        Task SaveAsync(MyClass entity);
        Task DeleteAsync(int id);
    }
}
```

### 2.3 生成服务文件

```csharp
// 命令行示例
sourcegenerator_generator.exe generate --template ServiceGenerator --output ./Generated/MyService.cs --parameters namespace=MyProject serviceName=MyService interfaceName=IMyService methods=Task<MyClass> GetByIdAsync(int id),Task SaveAsync(MyClass entity),Task DeleteAsync(int id)

// 生成的代码示例
namespace MyProject
{
    public class MyService : IMyService
    {
        public Task<MyClass> GetByIdAsync(int id)
        {
            // Implementation goes here
        }

        public Task SaveAsync(MyClass entity)
        {
            // Implementation goes here
        }

        public Task DeleteAsync(int id)
        {
            // Implementation goes here
        }
    }
}
```

### 2.4 生成仓库文件

```csharp
// 命令行示例
sourcegenerator_generator.exe generate --template ClassGenerator --output ./Generated/MyRepository.cs --parameters namespace=MyProject.Data className=MyRepository properties=DbContext:DbContext,Logger:ILogger<MyRepository> methods=Task<MyClass> GetByIdAsync(int id),Task SaveAsync(MyClass entity),Task DeleteAsync(int id)

// 生成的代码示例
namespace MyProject.Data
{
    public class MyRepository
    {
        public DbContext DbContext { get; set; }
        public ILogger<MyRepository> Logger { get; set; }

        public Task<MyClass> GetByIdAsync(int id)
        {
            // Implementation goes here
        }

        public Task SaveAsync(MyClass entity)
        {
            // Implementation goes here
        }

        public Task DeleteAsync(int id)
        {
            // Implementation goes here
        }
    }
}
```

### 2.5 生成控制器文件

```csharp
// 命令行示例
sourcegenerator_generator.exe generate --template ClassGenerator --output ./Generated/MyController.cs --parameters namespace=MyProject.Controllers className=MyController properties=MyService:IMyService methods=Task<IActionResult> Get(int id),Task<IActionResult> Post([FromBody]MyClass entity),Task<IActionResult> Delete(int id)

// 生成的代码示例
namespace MyProject.Controllers
{
    public class MyController
    {
        public IMyService MyService { get; set; }

        public Task<IActionResult> Get(int id)
        {
            // Implementation goes here
        }

        public Task<IActionResult> Post([FromBody]MyClass entity)
        {
            // Implementation goes here
        }

        public Task<IActionResult> Delete(int id)
        {
            // Implementation goes here
        }
    }
}
```

## 3. 模板使用示例

### 3.1 创建和使用模板

#### 创建模板文件 (`ClassTemplate.tmpl`)

```
namespace {{Namespace}}
{
    {{#if GeneratePartial}}
    public partial class {{ClassName}}
    {{else}}
    public class {{ClassName}}
    {{/if}}
    {
        {{#each Properties}}
        public {{Type}} {{Name}} { get; set; }
        {{/each}}

        {{#if GenerateConstructor}}
        public {{ClassName}}({{#each Properties}}{{Type}} {{Name}}{{#if @last}}, {{/if}}{{/each}})
        {
            {{#each Properties}}
            this.{{Name}} = {{Name}};
            {{/each}}
        }
        {{/if}}
    }
}
```

#### 使用模板

```csharp
// 代码中使用模板
var templateService = serviceProvider.GetRequiredService<ITemplateService>();

var parameters = new Dictionary<string, string>
{
    { "Namespace", "MyProject.Models" },
    { "ClassName", "Product" },
    { "GeneratePartial", "true" },
    { "GenerateConstructor", "true" },
    { "Properties", "Id:int,Name:string,Price:decimal,Description:string" }
};

var templateContent = await templateService.GetTemplateAsync("ClassTemplate");
var generatedCode = templateService.ProcessTemplate(templateContent!, parameters);

await File.WriteAllTextAsync("./Generated/Product.cs", generatedCode, Encoding.UTF8);

// 或使用 CodeGeneratorService
var generatorService = serviceProvider.GetRequiredService<ICodeGeneratorService>();
await generatorService.GenerateFileAsync("ClassTemplate", "./Generated/Product.cs", parameters);
```

### 3.2 高级模板使用

#### 循环和条件

```
{{#each Items}}
{{#if IsActive}}
public {{Type}} {{Name}} { get; set; }
{{else}}
// {{Name}} is inactive
{{/if}}
{{/each}}
```

#### 嵌套模板

```
// 主模板
namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        {{> properties}}
        {{> methods}}
    }
}

// 子模板：properties
{{#each Properties}}
public {{Type}} {{Name}} { get; set; }
{{/each}}

// 子模板：methods
{{#each Methods}}
public {{ReturnType}} {{Name}}({{#each Parameters}}{{Type}} {{Name}}{{#if @last}}, {{/if}}{{/each}})
{
    {{Body}}
}
{{/each}}
```

#### 模板继承

```
// 基础模板
namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        {{#block properties}}
        // Default properties
        {{/block}}

        {{#block methods}}
        // Default methods
        {{/block}}
    }
}

// 派生模板
{{#extends "base"}}
{{#block properties}}
public int Id { get; set; }
public string Name { get; set; }
{{/block}}

{{#block methods}}
public void Print()
{
    Console.WriteLine($"Id: {Id}, Name: {Name}");
}
{{/block}}
{{/extends}}
```

## 4. 与其他框架集成示例

### 4.1 与 ASP.NET Core 集成

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 注册 Source Generator 服务
builder.Services.AddScoped<ISourceGeneratorService, SourceGeneratorService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();

// 使用 Scrutor 注册生成器
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Generator")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

// 注册装饰器
builder.Services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();

var app = builder.Build();

// 代码生成端点
app.MapPost("/api/generate", async (GenerateCodeRequest request, ISourceGeneratorService generatorService) =>
{
    await generatorService.GenerateCodeAsync(request.ProjectPath, request.OutputPath);
    return Results.Ok(new { message = "Code generated successfully" });
});

app.Run();

// 生成代码请求模型
public class GenerateCodeRequest
{
    public string ProjectPath { get; set; }
    public string OutputPath { get; set; }
}
```

### 4.2 与控制台应用集成

```csharp
// Program.cs
class Program
{
    static async Task Main(string[] args)
    {
        // 配置依赖注入
        var services = new ServiceCollection();
        services.AddScoped<ISourceGeneratorService, SourceGeneratorService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
        services.AddScoped<ILoggerService, ConsoleLoggerService>();

        // 使用 Scrutor 注册生成器
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Generator")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // 注册装饰器
        services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();
        services.Decorate<ICodeGeneratorService, CodeGeneratorValidationDecorator>();

        var serviceProvider = services.BuildServiceProvider();

        // 使用代码生成服务
        var generatorService = serviceProvider.GetRequiredService<ICodeGeneratorService>();

        var parameters = new Dictionary<string, string>
        {
            { "Namespace", "MyProject" },
            { "ClassName", "Customer" },
            { "Properties", "Id:int,Name:string,Email:string,Phone:string" }
        };

        await generatorService.GenerateFileAsync("ClassGenerator", "./Generated/Customer.cs", parameters);

        Console.WriteLine("Code generated successfully!");
    }
}
```

### 4.3 与 Blazor 集成

```csharp
// GenerateCode.razor
@page "/generate"
@inject ICodeGeneratorService CodeGeneratorService
@inject ITemplateService TemplateService

<h3>Code Generator</h3>

<div class="form-group">
    <label for="template">Template:</label>
    <select id="template" @bind="SelectedTemplate">
        @foreach (var template in Templates)
        {
            <option value="@template">@template</option>
        }
    </select>
</div>

<div class="form-group">
    <label for="output">Output Path:</label>
    <input type="text" id="output" @bind="OutputPath" class="form-control" />
</div>

<div class="form-group">
    <label for="namespace">Namespace:</label>
    <input type="text" id="namespace" @bind="Parameters["Namespace"]" class="form-control" />
</div>

<div class="form-group">
    <label for="className">Class Name:</label>
    <input type="text" id="className" @bind="Parameters["ClassName"]" class="form-control" />
</div>

<div class="form-group">
    <label for="properties">Properties (Id:int,Name:string):</label>
    <input type="text" id="properties" @bind="Parameters["Properties"]" class="form-control" />
</div>

<button class="btn btn-primary" @onclick="GenerateCode">Generate</button>

<p>@StatusMessage</p>

@code {
    private string SelectedTemplate { get; set; } = "ClassGenerator";
    private string OutputPath { get; set; } = "./Generated/Class.cs";
    private Dictionary<string, string> Parameters { get; set; } = new()
    {
        { "Namespace", "MyProject" },
        { "ClassName", "MyClass" },
        { "Properties", "Id:int,Name:string" }
    };
    private List<string> Templates { get; set; } = new();
    private string StatusMessage { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        Templates = (await TemplateService.GetTemplateNamesAsync()).ToList();
    }

    private async Task GenerateCode()
    {
        try
        {
            await CodeGeneratorService.GenerateFileAsync(SelectedTemplate, OutputPath, Parameters);
            StatusMessage = "Code generated successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}
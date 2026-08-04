#:sdk Microsoft.NET.Sdk.Web
#:package WatchDog.NET@3.0.0
#:package Microsoft.SemanticKernel@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.SemanticKernel;

public class SemanticTracker
{
    private readonly IKernel _kernel;
    
    public SemanticTracker(IKernel kernel)
    {
        _kernel = kernel;
    }

    public async Task AnalyzeLogsAsync(string logs)
    {
        var function = _kernel.CreateFunctionFromPrompt(logs);
        await _kernel.InvokeAsync(function);
    }
}

var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<SemanticTracker>();
builder.Services.AddWatchDogServices();
var app = builder.Build();
app.UseWatchDog();
app.Run();
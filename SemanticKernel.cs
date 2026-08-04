using Microsoft.SemanticKernel;

// Create kernel with a custom http address
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(
    modelId: "phi4",
    endpoint: new Uri("http://localhost:11434"),
    apiKey: "apikey");
var kernel = builder.Build();

var prompt = "Write a joke about net10.";
var response = await kernel.InvokePromptAsync(prompt);
Console.WriteLine(response.GetValue<string>());

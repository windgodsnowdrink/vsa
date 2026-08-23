using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PlcAiot.Shared.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddPlcAiotShared();

await builder.Build().RunAsync();

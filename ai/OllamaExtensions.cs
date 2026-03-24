#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Hosting@10.0.5
#:package Microsoft.Extensions.DependencyInjection@10.0.5
#:package Microsoft.Extensions.Logging@10.0.5
#:package Microsoft.Extensions.Configuration@10.0.5
#:package Microsoft.Extensions.Primitives@10.0.5
#:package Microsoft.Extensions.Options@10.0.5
#:package Microsoft.Extensions.Caching.Memory@10.0.5
#:package Microsoft.Extensions.Http@10.0.5
#:package Microsoft.Extensions.Http.Polly@10.0.5
#:package Microsoft.Extensions.ObjectPool@10.0.5
#:package Microsoft.Extensions.Diagnostics@10.0.5
#:package Microsoft.Extensions.WebEncoders@10.0.5
#:package Microsoft.Extensions.AI@10.4.1
#:package OllamaSharp@5.4.24
#:package System.Text.Json@10.0.5
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=false

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OllamaSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Diagnostics;
using Microsoft.Extensions.WebEncoders;
using Microsoft.Extensions.AI;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddChatClient(
    new OllamaApiClient(
        new Uri("http://localhost:11434"),
        "llama3.2-vision:latest"));

var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();

// var message = new ChatMessage(
//     ChatRole.User, "What's in this image?");

var systemMessage = new ChatMessage(ChatRole.System,
    @"""
    You are a receipt parsing assistant. Extract all line items from the receipt image.
    For each line item, extract the name, quantity, unit price, and total price.
    Quantity can be a decimal number (e.g. weight in kg like 0.550 or 1.105).
    Extract the subtotal which is the final total amount shown on the receipt.
    IMPORTANT: Read every digit exactly as printed on the receipt.
    Pay very close attention to each decimal digit - do NOT round or approximate.
    For example, if the receipt shows 1.105, report exactly 1.105, not 1.1 or 1.2.
    Verify that quantity * unitPrice = totalPrice for each line item.
    Don't invent items that aren't on the receipt.

    DECIMAL FORMAT: Receipts may use different number formats depending on locale.
    - Some use period as decimal separator: 7,499.00
    - Some use comma as decimal separator: 7.499,00
    First, detect which format the receipt uses by examining the numbers on it.
    Then, always output numbers in the JSON using a period as the decimal separator.
    For example: 7499.00, not 7.499,00 or 7,499.00.
    """);

var message = new ChatMessage(ChatRole.User,
    """
    Extract all line items from this receipt.
    Respond in JSON format with this structure:
    {
        "items": [
            {
                "group": ".NET",
                "name": "buybackoff",
                "Default data JIT": 1.791,
                "Default data AOT": 1.765,
                "10K stations JIT": 3.213,
                "10K stations JIT": 3.176,
                "Language": "C#",
                "Runtime": ".NET 8",
            }
        ]
    }
    """);

message.Contents.Add(
    new DataContent(
        File.ReadAllBytes("receipts/1brc.png"),
        "image/png"));

// var response = await chatClient.GetResponseAsync([message]);

// Console.WriteLine(response.Text);

var response = await chatClient.GetResponseAsync<Receipt>(
    [message],
    new ChatOptions { Temperature = 0 });

if (response.Result is { } receipt)
{
    Console.WriteLine(
        $"\nExtracted {receipt.Items.Count} line items:");

    foreach (var item in receipt.Items)
    {
        Console.WriteLine(
            $"  {item.Group} - " +
            $"  {item.Name} - " +
            $"Default Data: {item.DefaultJIT} - {item.DefaultAOT}" +
            $"10K Stations: {item.Stations10KJIT} - {item.Stations10KAOT}" + 
            $"  {item.Language} - " +
            $"  {item.Runtime} - ");
    }
}

public class Receipt
{
    public List<LineItem> Items { get; set; } = [];
}

public class LineItem
{
    public string Group { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal DefaultJIT { get; set; }
    public decimal DefaultAOT { get; set; }
    public decimal Stations10KJIT { get; set; }
    public decimal Stations10KAOT { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Runtime { get; set; } = string.Empty;
}
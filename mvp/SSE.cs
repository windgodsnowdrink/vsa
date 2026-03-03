#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@9.0.9
#:package Microsoft.Extensions.ObjectPool@9.0.9
#:package Scalar.AspNetCore@2.8.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scalar.AspNetCore;

// Content-Type:text/event-stream
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(); // /scalar

builder.Services.AddSingleton<StockService>();
// ���� CORS ����
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("*")
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowFrontend");
    app.MapScalarApiReference();
}

app.MapGet("/stocks", (StockService stockService, CancellationToken ct) =>
{
    return TypedResults.ServerSentEvents(
       stockService.GenerateStockPrices(ct),
       eventType: "stockUpdate"
    );
});

// �ں�ˣ����ǿ��Լ�� HttpRequest.Headers["Last-Event-ID"] ��ȷ��������ָ����������������طŴ�������Ŀ���¼�����¼�
app.MapGet("/stocks2", (
    StockService stockService,
    HttpRequest httpRequest,
    CancellationToken ct) =>
{
    // 1. ��ȡ Last-Event-ID (�����)
    var lastEventId = httpRequest.Headers.TryGetValue("Last-Event-ID", outvar id)
       ? id.ToString()
       : null;

    // 2. ��ѡ���¼�����ָ��߼�
    if (!string.IsNullOrEmpty(lastEventId))
    {
        app.Logger.LogInformation("Reconnected, client last saw ID {LastId}", lastEventId);
    }

    // 3. ʹ�� lastEventId �� retry ��ʽ���� SSE
    var stream = stockService.GenerateStockPricesSince(lastEventId, ct)
       .Select(evt =>
       {
           var sseItem = new SseItem<StockPriceEvent>(evt, "stockUpdate")
           {
               EventId = evt.Id
           };

           return sseItem;
       });

    return TypedResults.ServerSentEvents(
       stream,
       eventType: "stockUpdate",
    );
});

app.Run();

public record StockPriceEvent(string Id, string Symbol, decimal Price, DateTime Timestamp);

// SSE �Զ������������ӶϿ�ʱ����������Զ������������ӣ�������ʹ�� Last-Event-ID ��ͷ�ӶϿ��ĵط��ָ�
public class StockService
{
    public async IAsyncEnumerable<StockPriceEvent> GenerateStockPrices([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var symbols = new[] { "MSFT", "AAPL", "GOOG", "AMZN" };

        while (!cancellationToken.IsCancellationRequested)
        {
            // ���ѡ��һ�����źͼ۸�
            var symbol = symbols[Random.Shared.Next(symbols.Length)];
            var price = Math.Round((decimal)(100 + Random.Shared.NextDouble() * 50), 2);

            var id = DateTime.UtcNow.ToString("o");

            yield return new StockPriceEvent(id, symbol, price, DateTime.UtcNow);

            // ������һ������ǰ�ȴ� 2 ��
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}

//@ServerSentEvents_HostAddress = http://localhost:5000

//### Test SSE stream from .NET 10 Minimal API
//GET {{ServerSentEvents_HostAddress}}/ stocks
//Accept: text /event-stream
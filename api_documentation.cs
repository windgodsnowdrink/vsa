#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@9.0.0-preview.4.24267.6
#:package Swashbuckle.AspNetCore@6.5.0
#:package Scalar.AspNetCore@1.0.0
#:package IGeekFan.AspNetCore.RapiDoc@0.0.8
#:package IGeekFan.AspNetCore.Knife4jUI@0.0.16
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using IGeekFan.AspNetCore.RapiDoc;
using IGeekFan.AspNetCore.Knife4jUI;

var builder = WebApplication.CreateBuilder(args);

// 添加OpenAPI/Swagger服务
builder.Services.AddEndpointsApiExplorer();

// 配置Swagger生成器
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new { Title = "API文档", Version = "v1" });
    // c.EnableAnnotations();

    // 启用XML注释
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// 开发环境下启用文档
if (app.Environment.IsDevelopment())
{
    // 使用Scalar UI
    app.UseScalar(config =>
    {
        config.SpecUrl = "/swagger/v1/swagger.json";
        config.HideModelsSection = false;
    });

    // 使用RapiDoc UI
    app.UseRapiDocUI(c =>
    {
        c.RoutePrefix = "rapidoc";
        c.SpecUrl = "/swagger/v1/swagger.json";
        c.HeadContent = "<title>RapiDoc</title>";
    });

    // 使用Knife4j UI
    app.UseKnife4UI(c =>
    {
        c.RoutePrefix = "knife4j";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API文档 V1");
    });

    // 默认Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API文档 V1"));
}

app.MapGet("/", () => "API文档中心");
app.Run();

return services;
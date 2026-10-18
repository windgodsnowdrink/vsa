// Program.cs — PLC·AIOT 多租户 SaaS（File-based App 入口，.NET 11 preview.6 / C# 14）
// 运行： dotnet run Program.cs   （等待 "Now listening on http://localhost:5000"）
// 所有共享设施经 #include "infra.cs"；各垂直切片自包含端点 + CQRS Handler + DTO。
//#nullable enable
//#include "GlobalUsings.cs"
//#include "infra.cs"
//#include "slices/tenant.register.cs"
//#include "slices/tenant.activate.cs"
//#include "slices/identity.login.cs"
//#include "slices/identity.members.cs"
//#include "slices/identity.mqtt.cs"
//#include "slices/billing.usage.cs"
//#include "slices/billing.plan.cs"
//#include "slices/billing.quota.cs"
//#include "slices/devices.list.cs"
//#include "slices/faults.ack.cs"
//#include "slices/faults.stream.cs"
//#include "slices/ops.tenants.cs"
//#include "slices/ops.pricing.cs"
//#include "slices/ops.roles.cs"
//#include "slices/ops.health.cs"
//#include "slices/metering.agent.cs"
//#include "slices/ingest.bridge.cs"
//#include "slices/ingest.http.cs"
//#include "slices/ai.assistant.cs"
//#include "seed.cs"

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSaasDb(builder.Configuration);
builder.Services.AddSaasAuth(builder.Configuration);
builder.Services.AddSaasMediator();
builder.Services.AddSaasSignalR();
builder.Services.AddSaasRateLimiter();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSaasIngest(builder.Configuration);
builder.Services.AddSaasAi(builder.Configuration);

builder.Services.AddCors(o => o.AddPolicy("saas-dev", p =>
    p.WithOrigins("http://localhost:5173", "https://localhost:5173")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddHostedService<MeteringAgent>();

var app = builder.Build();

app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles();   // 托管 wwwroot 下的 SaaS 控制台 SPA（Phase 3）
app.UseCors("saas-dev");
app.UseAuthentication();
app.UseTenantContext();
app.UseAuthorization();
app.UseRateLimiter();

var api = app.MapGroup("/api/v1");
TenantRegisterApi.Map(api);
TenantActivateApi.Map(api);
IdentityLoginApi.Map(api);
IdentityMembersApi.Map(api);
IdentityMqttApi.Map(api);
BillingUsageApi.Map(api);
BillingPlanApi.Map(api);
BillingQuotaApi.Map(api);
DevicesApi.Map(api);
FaultsAckApi.Map(api);
OpsTenantsApi.Map(api);
OpsPricingApi.Map(api);
OpsRolesApi.Map(api);
OpsHealthApi.Map(api);
IngestApi.Map(api);
AiAssistantApi.Map(api);

app.MapHub<FaultsHub>("/hubs/faults");

// MCP Server（ADR-112，外部 AI Agent 安全问诊）；仅 Mcp:Enabled 时暴露 /mcp（Streamable HTTP + SSE）
if (builder.Configuration.GetValue("Mcp:Enabled", false))
    app.MapMcp("/mcp"); // Hub 鍚? faults锛涘崗鍟? /hubs/faults/negotiate

// SPA 回退：未命中的非 API/Hub 路由（含 hash 深链刷新）一律返回 index.html（须置于 API/Hub 映射之后）
app.MapFallbackToFile("index.html");

// 鍚姩锛氬箓绛夊缓琛? + RLS 鑷姩搴旂敤锛堝厛浜庣瀛愶紝纭繚琛ㄤ笌琛岀骇瀹夊叏灏辩华锛?
using (var scope = app.Services.CreateScope())
{
    await SaasSchemaBootstrap.BootstrapAsync(scope.ServiceProvider);
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();

// 渚夸簬闆嗘垚娴嬭瘯鍙栧湴鍧€
public partial class Program { }

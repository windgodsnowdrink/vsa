// Program.cs — PLC·AIOT 多租户 SaaS（File-based App 入口，.NET 11 preview.6 / C# 14）
// 运行： dotnet run Program.cs   （等待 "Now listening on http://localhost:5000"）
// 所有共享设施经 #include "infra.cs"；各垂直切片自包含端点 + CQRS Handler + DTO。
#nullable enable
#include "GlobalUsings.cs"
#include "infra.cs"
#include "slices/tenant.register.cs"
#include "slices/tenant.activate.cs"
#include "slices/identity.login.cs"
#include "slices/identity.members.cs"
#include "slices/identity.mqtt.cs"
#include "slices/billing.usage.cs"
#include "slices/billing.plan.cs"
#include "slices/billing.quota.cs"
#include "slices/devices.list.cs"
#include "slices/faults.ack.cs"
#include "slices/faults.stream.cs"
#include "slices/ops.tenants.cs"
#include "slices/ops.pricing.cs"
#include "slices/ops.roles.cs"
#include "slices/ops.health.cs"
#include "slices/metering.agent.cs"
#include "seed.cs"

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSaasDb(builder.Configuration);
builder.Services.AddSaasAuth(builder.Configuration);
builder.Services.AddSaasMediator();
builder.Services.AddSaasSignalR();
builder.Services.AddSaasRateLimiter();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(o => o.AddPolicy("saas-dev", p =>
    p.WithOrigins("http://localhost:5173", "https://localhost:5173")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddHostedService<MeteringAgent>();

var app = builder.Build();

app.UseRouting();
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

app.MapHub<FaultsHub>("/hubs/faults"); // Hub 名 faults；协商 /hubs/faults/negotiate

// 启动：幂等种子（套餐/配额策略/平台超管）
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();

// 便于集成测试取地址
public partial class Program { }

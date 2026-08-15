// seed.cs — 幂等种子（套餐档 + 配额策略 + 平台超管 tid=root）
// 由 Program.cs #include 引入。仅含配额差异化数据，无任何货币字段（ADR-108）。
#if !SEED_CS
#define SEED_CS

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<BaseDbContext>();
        var users = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = sp.GetRequiredService<RoleManager<ApplicationRole>>();
        var cfg = sp.GetRequiredService<IConfiguration>();

        await db.Database.EnsureCreatedAsync(); // 开发期建表；生产用迁移（见报告）
        await SaasRls.ApplyAsync(sp, db);        // 可选 RLS（Saas:ApplyRlsOnStartup）

        if (!await db.Plans.IgnoreQueryFilters().AnyAsync())
        {
            var plans = new[]
            {
                new Plan { Name = SaasPlans.Free, DeviceQuota = 5, TelemetryQuota = 100_000 },
                new Plan { Name = SaasPlans.Pro, DeviceQuota = 50, TelemetryQuota = 1_000_000 },
                new Plan { Name = SaasPlans.Business, DeviceQuota = 200, TelemetryQuota = 5_000_000 },
                new Plan { Name = SaasPlans.Enterprise, DeviceQuota = 1000, TelemetryQuota = 50_000_000 },
            };
            db.Plans.AddRange(plans);
            await db.SaveChangesAsync();

            foreach (var plan in plans)
            {
                db.QuotaPolicies.AddRange(
                    new QuotaPolicy { PlanId = plan.Id, Metric = QuotaMetric.device, ThresholdPct = 90, Action = QuotaAction.Warn },
                    new QuotaPolicy { PlanId = plan.Id, Metric = QuotaMetric.telemetry, ThresholdPct = 90, Action = QuotaAction.Warn });
            }
            await db.SaveChangesAsync();
        }

        // 平台超管（tid=root，TenantId=Guid.Empty）
        var rootEmail = cfg["Root:Email"] ?? "root@plc-aiot.example";
        var rootPwd = cfg["Root:Password"] ?? "Root@Dev-ChangeMe-2026";
        var rootUser = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.NormalizedEmail == rootEmail.ToUpper());
        if (rootUser is null)
        {
            var rootRole = await roles.FindByNameAsync(SaasRoles.SuperAdmin);
            if (rootRole is null)
            {
                rootRole = new ApplicationRole(SaasRoles.SuperAdmin) { TenantId = Guid.Empty };
                await roles.CreateAsync(rootRole);
            }
            rootUser = new ApplicationUser
            {
                UserName = rootEmail,
                Email = rootEmail,
                NormalizedEmail = rootEmail.ToUpper(),
                TenantId = Guid.Empty,
                Name = "平台超管",
            };
            await users.CreateAsync(rootUser, rootPwd);
            await users.AddToRoleAsync(rootUser, SaasRoles.SuperAdmin);
        }
    }
}
#endif

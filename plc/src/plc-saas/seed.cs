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

        // schema 建表 + RLS 策略已由 Program.cs 中 SaasSchemaBootstrap.BootstrapAsync 幂等应用（先于本 Seed）。

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
        // 用户裁定（2026-08-18）：默认租户/密码 root/admin（多租户 SaaS 重要等级降低，先用最简凭据）
        var rootEmail = cfg["Root:Email"] ?? "root";
        var rootPwd = cfg["Root:Password"] ?? "admin";
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
                // 必须显式赋值：AddToRoleAsync → UpdateUserAsync → ValidateUserAsync 会读取 SecurityStamp，
                // 为空则抛 "User security stamp cannot be null."（默认 CreateAsync(user, password)
                // 在密码校验失败时会提前返回，此处不会走到赋 stamp 那一步）。
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            // 密码哈希直接写入并调用无密码重载 CreateAsync(user)：绕开 Identity 默认密码策略
            // （要求数字/大写/小写/非字母数字），使既定降级凭据 root/admin 可用。
            // 注意：这里只放宽这一个平台超管种子账号，不动全站租户用户的密码强度要求。
            rootUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(rootUser, rootPwd);
            var create = await users.CreateAsync(rootUser);
            if (!create.Succeeded)
            {
                var errs = string.Join("; ", create.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"平台超管种子账号创建失败（{rootEmail}）：{errs}");
            }
            await users.AddToRoleAsync(rootUser, SaasRoles.SuperAdmin);
        }
    }
}
#endif

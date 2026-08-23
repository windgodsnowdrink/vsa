-- sql/rls.sql — PostgreSQL 行级安全兜底（ADR-103 ③.4 / SaaS-DB-Schema §3）
-- 纵深防御：即便代码误用 IgnoreQueryFilters() 或走原始 SQL，RLS 仍强制租户隔离。
-- 连接拦截器（TenantConnectionInterceptor）每连接执行 SET app.tenant_id = '<tid>'。
-- 运行时机：EnsureCreated / 迁移建表后执行一次（app.tenant_id 为自定义 GUC，app.* 前缀无需预注册）。

-- 1) 启用 RLS + 策略（USING 过滤读/删；WITH CHECK 强制写同租户）
--    仅对 ITenantEntity 表启用；目录表（tenants/plans/quota_policies）与平台级 audit_logs 不启用。

DO $$
DECLARE t text;
BEGIN
  FOREACH t IN ARRAY ARRAY[
    'asp_net_users','asp_net_roles','asp_net_user_roles',
    'devices','fault_events','subscriptions','usage_meters','outbox_messages','refresh_tokens'
  ]
  LOOP
    EXECUTE format('ALTER TABLE %I ENABLE ROW LEVEL SECURITY;', t);
    EXECUTE format(
      'DROP POLICY IF EXISTS tenant_isolation ON %1$I; ' ||
      'CREATE POLICY tenant_isolation ON %1$I ' ||
      'USING ("TenantId" = current_setting(''app.tenant_id'')::uuid) ' ||
      'WITH CHECK ("TenantId" = current_setting(''app.tenant_id'')::uuid);', t);
  END LOOP;
END $$;

-- 2) 角色划分（部署约定）
--    租户作用域连接：普通角色，每连接由拦截器 SET app.tenant_id = '<tid>' → RLS 强制隔离。
--    平台超管 / 后台计量作业：具 BYPASSRLS 权限的角色连接 → RLS 对其透明跳过（见 SaaS-DB-Schema §3）。
--    以下为示例，实际凭据由部署配置注入连接字符串：
--    CREATE ROLE saas_tenant NOINHERIT;                 -- 租户作用域
--    CREATE ROLE saas_bypass BYPASSRLS NOINHERIT;       -- root / 计量 Agent
--    GRANT USAGE ON SCHEMA public TO saas_tenant, saas_bypass;
--    GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO saas_tenant, saas_bypass;

-- 3) 验证（手动）
--    SET app.tenant_id = '11111111-1111-1111-1111-111111111111';
--    SELECT count(*) FROM devices;   -- 仅返回该租户行（被 RLS 过滤）
--    RESET app.tenant_id;

-- sql/rls.sql — SQL Server 行级安全兜底（ADR-103 ③.4 / SaaS-DB-Schema §3）
-- 纵深防御：即便代码误用 IgnoreQueryFilters() 或走原始 SQL，RLS 仍强制租户隔离。
-- 连接拦截器（TenantConnectionInterceptor）每连接执行
--   EXEC sp_set_session_context @key = N'TenantId', @value = '<tid>';
-- 运行时机：EnsureCreated 建表后执行一次（幂等：CREATE OR ALTER FUNCTION + DROP/CREATE SECURITY POLICY）。
-- 批次分隔：用 GO（SaasSchemaBootstrap 按 GO 拆分逐批执行；SqlCommand 不支持 GO，
--   CREATE FUNCTION / CREATE SECURITY POLICY 必须各自成批）。
-- 依赖：SQL Server 2016+（RLS + SESSION_CONTEXT）。Windows 账户登录需具 CREATE SECURITY POLICY 权限；
--   权限不足时 bootstrap 记录告警不阻断启动，由具权限角色补执行。

-- 1) 谓词函数（inline TVF）：行 TenantId 与 SESSION_CONTEXT(N'TenantId') 一致则返回 1 行
--    CREATE OR ALTER 幂等，可每启动重跑。
CREATE OR ALTER FUNCTION dbo.fn_tenant_predicate(@TenantId uniqueidentifier)
RETURNS TABLE
AS
RETURN
    SELECT 1 AS fn_result
    WHERE @TenantId = CONVERT(uniqueidentifier, SESSION_CONTEXT(N'TenantId'));
GO

-- 2) 安全策略：对 ITenantEntity 表启用 FILTER（读/删过滤）+ CHECK（写强制同租户）
--    目录表（tenants/plans/quota_policies）与平台级 audit_logs 不启用。
--    DROP IF EXISTS 幂等后重建，可每启动重跑。
DROP SECURITY POLICY IF EXISTS dbo.tenant_isolation;
GO

CREATE SECURITY POLICY dbo.tenant_isolation
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.asp_net_users,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.asp_net_users,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.asp_net_roles,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.asp_net_roles,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.asp_net_user_roles,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.asp_net_user_roles,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.devices,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.devices,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.fault_events,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.fault_events,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.subscriptions,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.subscriptions,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.usage_meters,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.usage_meters,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.outbox_messages,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.outbox_messages,
    ADD FILTER PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.refresh_tokens,
    ADD CHECK  PREDICATE dbo.fn_tenant_predicate(TenantId) ON dbo.refresh_tokens
WITH (STATE = ON);
GO

-- 3) 角色划分（部署约定）
--    租户作用域连接：Windows 账户登录，每连接由拦截器 sp_set_session_context → RLS 强制隔离。
--    平台超管 / 后台计量作业：可在 SESSION_CONTEXT(N'TenantId')=00000000-0000-0000-0000-000000000000
--      （Guid.Empty）下读写 root 行；如需完全跳过 RLS，由 db_owner 或 CONTROL SERVER 主体操作。
--    （Windows 账户登录下无 SQL LOGIN 创建脚本；凭据由部署侧 AD/本机账户注入连接串。）

-- 4) 验证（手动，SSMS / sqlcmd）
--    EXEC sp_set_session_context @key = N'TenantId', @value = '11111111-1111-1111-1111-111111111111';
--    SELECT count(*) FROM dbo.devices;   -- 仅返回该租户行（被 RLS FILTER 过滤）
--    -- 重置：连接释放后 sp_reset_connection 自动清空 SESSION_CONTEXT

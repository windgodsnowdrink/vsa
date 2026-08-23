-- 在 PostgreSQL 初始化阶段创建 SaaS 应用专用角色。
-- appsettings.json 连接串使用 plcsaas / plcsaas；PG 超级用户为 postgresql / postgresql（compose 设定）。
-- 设为 SUPERUSER 仅为本地开发便利：EnsureCreated + 种子阶段免去 RLS 摩擦；
-- 生产应改为受限角色（依赖 TenantConnectionInterceptor 每连接 SET app.tenant_id + RLS 兜底）。

DO $$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'plcsaas') THEN
    CREATE ROLE plcsaas LOGIN PASSWORD 'plcsaas' SUPERUSER;
  END IF;
END
$$;

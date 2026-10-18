/*
 * app.js — PLC·AIOT 多租户 SaaS 控制台（前端 SPA，Phase 3）
 * 设计约束（SaaS-Spec §8 / ADR-106）：单品牌科技天蓝、暗色优先、玻璃态、Lucide 图标、禁 emoji、禁紫粉渐变。
 * 计费红线（ADR-108）：仅计量不收费，前端绝不展示价格 / 货币 / 金额字段。
 * 统一响应信封：{ code, message, data }，code===0 为成功。
 */
'use strict';

const API = '/api/v1';
const TOKEN_KEY = 'plcsaas.token';
const SLUG_KEY = 'plcsaas.slug';

const state = {
  token: null,          // { accessToken, refreshToken, expiresIn, tenantId }
  tenantId: null,
  tenantSlug: null,
  isSuperAdmin: false,
};

class ApiError extends Error {
  constructor(message, code, status) {
    super(message);
    this.code = code;
    this.status = status;
  }
}

/* ---------------- 工具 ---------------- */
function esc(s) {
  if (s === null || s === undefined) return '';
  return String(s).replace(/[&<>"']/g, c => ({
    '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;',
  }[c]));
}
function icon(name, size) {
  const s = size || 'md';
  return `<svg class="icon ${s}" viewBox="0 0 24 24" aria-hidden="true"><use href="/css/icons.svg#i-${name}"></use></svg>`;
}
function fmtDate(d) {
  if (!d) return '—';
  try { return new Date(d).toLocaleString('zh-CN', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' }); }
  catch { return esc(d); }
}
function num(n) { return (n === null || n === undefined) ? '—' : Number(n).toLocaleString('zh-CN'); }
function pct(part, total) {
  if (!total) return 0;
  return Math.max(0, Math.min(100, Math.round((part / total) * 100)));
}
function meterClass(p) { return p >= 95 ? 'bad' : p >= 80 ? 'warn' : 'ok'; }

function saveToken() {
  try { localStorage.setItem(TOKEN_KEY, JSON.stringify(state.token)); } catch {}
  try { if (state.tenantSlug) localStorage.setItem(SLUG_KEY, state.tenantSlug); } catch {}
}
function loadToken() {
  try {
    const t = localStorage.getItem(TOKEN_KEY);
    if (t) { state.token = JSON.parse(t); state.tenantId = state.token.tenantId; state.tenantSlug = localStorage.getItem(SLUG_KEY); }
  } catch {}
}
function logout() {
  state.token = null; state.tenantId = null; state.tenantSlug = null; state.isSuperAdmin = false;
  try { localStorage.removeItem(TOKEN_KEY); localStorage.removeItem(SLUG_KEY); } catch {}
  location.hash = '#/login';
  render();
}

function toast(msg, kind) {
  const host = document.getElementById('toasts');
  if (!host) return;
  const div = document.createElement('div');
  div.className = 'toast ' + (kind || '');
  div.textContent = msg;
  host.appendChild(div);
  setTimeout(() => { div.style.opacity = '0'; setTimeout(() => div.remove(), 200); }, 3600);
}

/* ---------------- API 客户端 ---------------- */
async function apiFetch(path, { method = 'GET', body, auth = true } = {}) {
  const headers = { 'Content-Type': 'application/json' };
  if (auth && state.token) headers['Authorization'] = 'Bearer ' + state.token.accessToken;
  const doFetch = () => fetch(API + path, {
    method, headers, body: body ? JSON.stringify(body) : undefined,
  });
  let res = await doFetch();
  if (res.status === 401 && auth && state.token && state.token.refreshToken) {
    const ok = await tryRefresh();
    if (ok) { headers['Authorization'] = 'Bearer ' + state.token.accessToken; res = await doFetch(); }
  }
  let json;
  try { json = await res.json(); } catch { json = { code: res.status, message: res.statusText, data: null }; }
  if (res.status === 401) { logout(); throw new ApiError((json && json.message) || '未认证或会话已过期', json && json.code, 401); }
  if (!res.ok || (json && json.code !== 0 && json.code !== undefined)) {
    throw new ApiError((json && json.message) || ('HTTP ' + res.status), json && json.code, res.status);
  }
  return json.data;
}
async function tryRefresh() {
  try {
    const r = await fetch(API + '/auth/refresh', {
      method: 'POST', headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: state.token.refreshToken }),
    });
    if (!r.ok) return false;
    const j = await r.json();
    if (!j || j.code !== 0 || !j.data) return false;
    state.token.accessToken = j.data.accessToken;
    state.token.expiresIn = j.data.expiresIn;
    if (j.data.refreshToken) state.token.refreshToken = j.data.refreshToken;
    saveToken();
    return true;
  } catch { return false; }
}

async function probeSuperAdmin() {
  try { await apiFetch('/ops/tenants'); state.isSuperAdmin = true; }
  catch { state.isSuperAdmin = false; }
}

/* ---------------- 路由 ---------------- */
const NAV = [
  {
    group: '账户', items: [
      { route: '#/account/usage', icon: 'gauge', label: '用量概览' },
      { route: '#/account/plan', icon: 'layers', label: '套餐与订阅' },
      { route: '#/account/members', icon: 'users', label: '成员管理' },
      { route: '#/account/billing', icon: 'receipt', label: '账单与发票' },
    ],
  },
  {
    group: '运营后台', super: true, items: [
      { route: '#/ops/tenants', icon: 'building', label: '组织管理' },
      { route: '#/ops/pricing', icon: 'tags', label: '计费策略' },
      { route: '#/ops/roles', icon: 'key-round', label: '全局角色' },
      { route: '#/ops/audit', icon: 'scroll-text', label: '审计日志' },
      { route: '#/ops/health', icon: 'activity', label: '平台健康' },
    ],
  },
];

function currentRoute() {
  const h = location.hash || '#/';
  return h;
}

function render() {
  const route = currentRoute();
  if (!state.token) {
    if (route === '#/register') return renderAuth('register');
    return renderAuth('login');
  }
  if (route === '#/' || route === '') { location.hash = '#/account/usage'; return; }
  renderShell(route);
}

/* ---------------- 鉴权视图（登录 / 开通） ---------------- */
function renderAuth(tab) {
  const app = document.getElementById('app');
  app.innerHTML = `
  <div class="auth-wrap">
    <div class="card auth-card">
      <div class="brand" style="padding:0 0 16px">
        <div class="mark">P</div>
        <div><div class="name">PLC·AIOT 控制台</div><div class="sub">多租户 SaaS · 内网免费</div></div>
      </div>
      <div class="auth-tabs">
        <div class="seg" style="width:100%">
          <button data-tab="login" class="${tab === 'login' ? 'active' : ''}">登录</button>
          <button data-tab="register" class="${tab === 'register' ? 'active' : ''}">开通租户</button>
        </div>
      </div>
      <div id="auth-body"></div>
    </div>
  </div>`;
  app.querySelectorAll('.auth-tabs .seg button').forEach(b => b.onclick = () => {
    location.hash = b.dataset.tab === 'register' ? '#/register' : '#/login';
    renderAuth(b.dataset.tab);
  });
  if (tab === 'register') renderRegisterForm();
  else renderLoginForm();
}

function renderLoginForm() {
  document.getElementById('auth-body').innerHTML = `
    <form id="f-login" autocomplete="on">
      <div class="field"><label>租户标识（slug）</label><input class="input" name="tenant" placeholder="如 test-mfg" required></div>
      <div class="field"><label>账号邮箱</label><input class="input" type="email" name="email" placeholder="admin@test.com" required></div>
      <div class="field"><label>密码</label><input class="input" type="password" name="password" required></div>
      <button class="btn btn-primary" type="submit" style="width:100%">登录 ${icon('log-out', 'sm')}</button>
    </form>`;
  document.getElementById('f-login').onsubmit = async (e) => {
    e.preventDefault();
    const f = e.target;
    try {
      await login(f.tenant.value.trim(), f.email.value.trim(), f.password.value);
      toast('登录成功', 'ok');
      location.hash = '#/account/usage';
      render();
    } catch (err) { toast(err.message || '登录失败', 'bad'); }
  };
}

function renderRegisterForm() {
  document.getElementById('auth-body').innerHTML = `
    <form id="f-reg" autocomplete="on">
      <div class="field"><label>企业名称</label><input class="input" name="companyName" placeholder="测试制造企业" required></div>
      <div class="field"><label>租户标识 slug（小写字母/数字/连字符）</label><input class="input" name="slug" placeholder="test-mfg" pattern="[a-z0-9][a-z0-9-]{2,30}" required></div>
      <div class="field"><label>管理员邮箱</label><input class="input" type="email" name="adminEmail" placeholder="admin@test.com" required></div>
      <div class="field"><label>管理员密码（≥8 位）</label><input class="input" type="password" name="adminPassword" minlength="8" required></div>
      <button class="btn btn-primary" type="submit" style="width:100%">创建租户 ${icon('rocket', 'sm')}</button>
      <p class="scope-note">开通即在 5 分钟内创建租户、预置租户 ID 与默认管理员账号（AC-01）。</p>
    </form>`;
  document.getElementById('f-reg').onsubmit = async (e) => {
    e.preventDefault();
    const f = e.target;
    const btn = f.querySelector('button');
    btn.disabled = true;
    try {
      const data = await register(f.companyName.value.trim(), f.slug.value.trim(), f.adminEmail.value.trim(), f.adminPassword.value);
      toast('租户已创建：' + (data.slug || f.slug.value), 'ok');
      renderAuth('login');
      const lf = document.getElementById('f-login');
      if (lf) { lf.tenant.value = f.slug.value.trim(); lf.email.value = f.adminEmail.value.trim(); }
    } catch (err) { toast(err.message || '开通失败', 'bad'); btn.disabled = false; }
  };
}

async function login(tenant, email, password) {
  const data = await apiFetch('/auth/login', { method: 'POST', auth: false, body: { tenant, email, password } });
  state.token = { accessToken: data.accessToken, refreshToken: data.refreshToken, expiresIn: data.expiresIn, tenantId: data.tenantId };
  state.tenantId = data.tenantId;
  state.tenantSlug = tenant;
  saveToken();
  await probeSuperAdmin();
  return data;
}
async function register(companyName, slug, adminEmail, adminPassword) {
  return apiFetch('/tenants', { method: 'POST', auth: false, body: { companyName, slug, adminEmail, adminPassword } });
}

/* ---------------- 已登录外壳 ---------------- */
function renderShell(route) {
  const app = document.getElementById('app');
  const navHtml = NAV.filter(g => !g.super || state.isSuperAdmin).map(g => `
    <div class="nav-group">
      <div class="label">${icon('user', 'sm')} ${esc(g.group)}</div>
      ${g.items.map(it => `<a class="nav-link ${route.startsWith(it.route.split('/').slice(0, 3).join('/')) ? 'active' : ''}" href="${it.route}">${icon(it.icon, 'sm')} ${esc(it.label)}</a>`).join('')}
    </div>`).join('');

  app.innerHTML = `
  <div class="shell">
    <aside class="sidebar">
      <div class="brand"><div class="mark">P</div><div><div class="name">PLC·AIOT</div><div class="sub">多租户 SaaS 控制台</div></div></div>
      ${navHtml}
      <div class="foot"><a class="nav-link" href="#/login" onclick="event.preventDefault();logout()">${icon('log-out', 'sm')} 退出登录</a></div>
    </aside>
    <div>
      <header class="topbar">
        <div class="scope"><span class="dot"></span><span class="who">${esc(state.tenantSlug || '租户')}</span><span class="slug">@${esc(state.tenantSlug || '—')}</span></div>
        <div class="scope-bar">数据范围：当前租户（TenantId 中间件强制隔离，AC-02）</div>
        <div class="spacer"></div>
        <div class="user-chip">${state.isSuperAdmin ? '<span class="badge info">平台超管</span>' : '<span class="badge muted">租户用户</span>'}</div>
      </header>
      <main class="main" id="main"></main>
    </div>
  </div>
  <div class="drawer-wrap" id="drawer"><div class="drawer" id="drawer-body"></div></div>`;

  document.getElementById('drawer').onclick = (e) => { if (e.target.id === 'drawer') closeDrawer(); };

  dispatch(route);
}

function dispatch(route) {
  const main = document.getElementById('main');
  if (route.startsWith('#/account/usage')) return pageUsage(main);
  if (route.startsWith('#/account/plan')) return pagePlan(main);
  if (route.startsWith('#/account/members')) return pageMembers(main);
  if (route.startsWith('#/account/billing')) return pageBilling(main);
  if (route.startsWith('#/ops/tenants')) return pageOpsTenants(main);
  if (route.startsWith('#/ops/pricing')) return pageOpsPricing(main);
  if (route.startsWith('#/ops/roles')) return pageOpsRoles(main);
  if (route.startsWith('#/ops/audit')) return pageOpsAudit(main);
  if (route.startsWith('#/ops/health')) return pageOpsHealth(main);
  main.innerHTML = `<div class="empty">${icon('box', 'lg')}<p>页面不存在</p></div>`;
}

function closeDrawer() { const d = document.getElementById('drawer'); if (d) d.classList.remove('open'); }
function openDrawer(html) { const d = document.getElementById('drawer'); if (d) { document.getElementById('drawer-body').innerHTML = html; d.classList.add('open'); } }

/* ---------------- 账户 · 用量概览 ---------------- */
async function pageUsage(main) {
  main.innerHTML = `<div class="page-title">用量概览</div><div class="page-desc">本租户设备数、遥测用量与配额剩余（仅计量，不收费 · ADR-108）。</div><div class="empty">${icon('gauge', 'lg')}<p>加载中…</p></div>`;
  let usage, quota;
  try { usage = await apiFetch('/billing/usage'); } catch (e) { main.innerHTML = errCard(e); return; }
  try { quota = await apiFetch('/billing/quota'); } catch (e) { quota = null; }

  const plan = usage.plan || {};
  const devUsed = usage.deviceCount || 0, devQuota = plan.deviceQuota || 0;
  const telUsed = usage.telemetryPoints || 0, telQuota = plan.telemetryQuota || 0;
  const devPct = pct(devUsed, devQuota), telPct = pct(telUsed, telQuota);

  main.innerHTML = `
    <div class="page-title">用量概览</div>
    <div class="page-desc">计费周期 <span class="mono">${esc(usage.period || '—')}</span> · 当前套餐 <span class="badge info">${esc(plan.name || '—')}</span>（仅计量，不收费）</div>
    <div class="grid kpis">
      <div class="card kpi"><div class="label">${icon('box', 'sm')} 设备数</div><div class="value">${num(devUsed)}</div><div class="hint">配额 ${num(devQuota)} · 剩余 ${num((usage.quotaRemaining && usage.quotaRemaining.devices) ?? (devQuota - devUsed))}</div>
        ${meter(devUsed, devQuota, '设备配额', '设备配额使用率')}</div>
      <div class="card kpi"><div class="label">${icon('activity', 'sm')} 遥测点数</div><div class="value">${num(telUsed)}</div><div class="hint">配额 ${num(telQuota)} · 剩余 ${num((usage.quotaRemaining && usage.quotaRemaining.telemetry) ?? (telQuota - telUsed))}</div>
        ${meter(telUsed, telQuota, '遥测配额', '遥测配额使用率')}</div>
      <div class="card kpi"><div class="label">${icon('layers', 'sm')} 当前套餐档</div><div class="value" style="font-size:20px">${esc(plan.name || '—')}</div><div class="hint">切换套餐见「套餐与订阅」</div></div>
    </div>
    ${quotaPoliciesHtml(quota)}`;
}

function meter(used, total, label, ariaLabel) {
  const p = pct(used, total); const cls = meterClass(p);
  return `<div class="meter">
    <div class="head"><span>${esc(label)}</span><span>${p}% · ${num(used)}/${num(total)}</span></div>
    <div class="track"><div class="fill ${cls}" role="progressbar" aria-valuenow="${used}" aria-valuemin="0" aria-valuemax="${total}" aria-label="${esc(ariaLabel)}" style="width:${p}%"></div></div>
  </div>`;
}
function quotaPoliciesHtml(quota) {
  if (!quota || !quota.policies || !quota.policies.length) return '';
  const rows = quota.policies.map(p => `<tr><td>${esc(p.metric)}</td><td>${p.thresholdPct}%</td><td><span class="badge ${p.action === 'Throttle' ? 'bad' : 'warn'}">${esc(p.action)}</span></td></tr>`).join('');
  return `<div class="section-title">配额超限策略（G8）</div>
    <div class="card"><table class="tbl"><thead><tr><th>指标</th><th>阈值</th><th>动作</th></tr></thead><tbody>${rows}</tbody></table></div>`;
}

/* ---------------- 账户 · 套餐与订阅 ---------------- */
async function pagePlan(main) {
  main.innerHTML = `<div class="page-title">套餐与订阅</div><div class="page-desc">当前套餐档与配额（免费档间切换由平台运营在「运营后台·计费策略」管理）。</div><div class="empty">加载中…</div>`;
  let plan;
  try { plan = await apiFetch('/billing/plan'); } catch (e) { main.innerHTML = errCard(e); return; }
  main.innerHTML = `
    <div class="page-title">套餐与订阅</div>
    <div class="page-desc">当前套餐档与配额（免费档间切换由平台运营在「运营后台·计费策略」管理）。</div>
    <div class="grid cols-2">
      <div class="card">
        <div class="label muted" style="font-size:var(--text-caption)">当前套餐档</div>
        <div style="margin-top:8px"><span class="badge info">${esc(plan.name || '—')}</span></div>
        <div class="grid kpis" style="margin-top:18px">
          <div class="kpi"><div class="label">${icon('box', 'sm')} 设备配额</div><div class="value">${num(plan.deviceQuota)}</div></div>
          <div class="kpi"><div class="label">${icon('activity', 'sm')} 遥测配额</div><div class="value">${num(plan.telemetryQuota)}</div></div>
        </div>
        <p class="scope-note">订阅生命周期：Active（运行态）。平台可在「运营后台」暂停 / 恢复租户，数据保留（AC-07）。</p>
      </div>
      <div class="card">
        <div class="label muted" style="font-size:var(--text-caption)">计费裁定（ADR-108）</div>
        <p style="margin-top:10px;color:var(--text-2);line-height:1.6">本平台为内网免费使用：套餐档仅作<strong>配额差异化</strong>，不产生任何费用、账单或支付流程。响应中不含任何价格 / 货币 / 金额字段（AC-10）。</p>
      </div>
    </div>`;
}

/* ---------------- 账户 · 成员管理 ---------------- */
async function pageMembers(main) {
  main.innerHTML = `<div class="page-title">成员管理</div><div class="page-desc">租户内成员与角色（四角色：Admin / Analyst / Operator / DeviceEng）。</div><div class="empty">加载中…</div>`;
  let members;
  try { members = await apiFetch('/tenant/members'); } catch (e) { main.innerHTML = errCard(e); return; }
  const rows = (members || []).map(m => `
    <tr>
      <td><div style="font-weight:600">${esc(m.name)}</div><div class="mono">${esc(m.email)}</div></td>
      <td><span class="badge ${roleBadge(m.role)}">${esc(m.role)}</span></td>
      <td class="mono">${esc(m.id)}</td>
      <td><select class="select btn-sm" data-role="${esc(m.id)}" style="width:auto">
        ${['Admin', 'Analyst', 'Operator', 'DeviceEng'].map(r => `<option value="${r}" ${r === m.role ? 'selected' : ''}>${r}</option>`).join('')}
      </select></td>
    </tr>`).join('');
  main.innerHTML = `
    <div class="page-title">成员管理</div>
    <div class="page-desc">租户内成员与角色（四角色：Admin / Analyst / Operator / DeviceEng）。</div>
    <div class="btn-row" style="margin-bottom:14px"><button class="btn btn-primary btn-sm" id="add-member">${icon('plus', 'sm')} 添加成员</button></div>
    <div class="card"><table class="tbl"><thead><tr><th>成员</th><th>角色</th><th>用户 ID</th><th>改角色</th></tr></thead><tbody>${rows || `<tr><td colspan="4" class="muted">暂无成员</td></tr>`}</tbody></table></div>`;
  main.querySelectorAll('select[data-role]').forEach(sel => sel.onchange = async () => {
    const id = sel.dataset.role, role = sel.value;
    try { await apiFetch(`/tenant/members/${id}/role`, { method: 'POST', body: { role } }); toast('角色已更新为 ' + role, 'ok'); }
    catch (e) { toast(e.message || '更新失败（需管理员权限）', 'bad'); }
  });
  document.getElementById('add-member').onclick = () => openDrawer(`
    <h3>${icon('users', 'sm')} 添加成员</h3>
    <form id="f-add">
      <div class="field"><label>姓名</label><input class="input" name="name" required></div>
      <div class="field"><label>邮箱</label><input class="input" type="email" name="email" required></div>
      <div class="field"><label>角色</label><select class="select" name="role">
        <option>Admin</option><option>Analyst</option><option>Operator</option><option>DeviceEng</option></select></div>
      <div class="btn-row"><button class="btn btn-primary" type="submit">添加</button><button class="btn btn-ghost" type="button" onclick="closeDrawerViaBtn()">取消</button></div>
    </form>`);
  const fa = document.getElementById('f-add');
  if (fa) fa.onsubmit = async (e) => {
    e.preventDefault(); const f = e.target;
    try { await apiFetch('/tenant/members', { method: 'POST', body: { name: f.name.value.trim(), email: f.email.value.trim(), role: f.role.value } }); toast('成员已添加', 'ok'); closeDrawer(); pageMembers(main); }
    catch (err) { toast(err.message || '添加失败（需管理员权限）', 'bad'); }
  };
}
function roleBadge(r) { return r === 'Admin' ? 'info' : r === 'Analyst' ? 'muted' : 'ok'; }
function closeDrawerViaBtn() { closeDrawer(); }

/* ---------------- 账户 · 账单与发票（只读占位） ---------------- */
function pageBilling(main) {
  main.innerHTML = `
    <div class="page-title">账单与发票</div>
    <div class="page-desc">本期为内网免费使用，不产生费用、账单或支付流程（ADR-108）。</div>
    <div class="card">
      <div class="empty" style="padding:36px">
        ${icon('receipt', 'lg')}
        <p style="margin-top:10px">当前没有任何账单或发票。</p>
        <p class="muted">计费上下文仅做套餐配额与用量计量；本平台不引入货币字段、不接支付、不出账（Out-of-Scope，详见 SaaS-Spec §3）。</p>
      </div>
    </div>`;
}

/* ---------------- 运营 · 组织管理 ---------------- */
async function pageOpsTenants(main) {
  main.innerHTML = `<div class="page-title">组织管理</div><div class="page-desc">全部租户及其运营健康（超管）。</div><div class="empty">加载中…</div>`;
  let list;
  try { list = await apiFetch('/ops/tenants'); } catch (e) { main.innerHTML = errCard(e); return; }
  const rows = (list || []).map(t => `
    <tr>
      <td><div style="font-weight:600">${esc(t.name)}</div><div class="mono">${esc(t.slug)}</div></td>
      <td><span class="badge ${t.status === 'Active' ? 'ok' : 'muted'}">${esc(t.status)}</span></td>
      <td>${esc(t.planName || '—')}</td>
      <td>${num(t.deviceCount)}</td>
      <td>${num(t.openFaults)}</td>
      <td class="mono">${fmtDate(t.createdAt)}</td>
      <td>
        ${t.status === 'Active'
          ? `<button class="btn btn-sm btn-danger" data-suspend="${esc(t.id)}">${icon('pause', 'sm')} 暂停</button>`
          : `<button class="btn btn-sm" data-resume="${esc(t.id)}">${icon('play', 'sm')} 恢复</button>`}
      </td>
    </tr>`).join('');
  main.innerHTML = `
    <div class="page-title">组织管理</div>
    <div class="page-desc">全部租户及其运营健康（超管）。暂停后读写被拒、数据保留（AC-07）。</div>
    <div class="card"><table class="tbl"><thead><tr><th>租户</th><th>状态</th><th>套餐</th><th>设备</th><th>未决告警</th><th>创建于</th><th>运营</th></tr></thead><tbody>${rows || `<tr><td colspan="7" class="muted">暂无租户</td></tr>`}</tbody></table></div>`;
  main.querySelectorAll('button[data-suspend]').forEach(b => b.onclick = () => actTenant(b.dataset.suspend, 'suspend', main));
  main.querySelectorAll('button[data-resume]').forEach(b => b.onclick = () => actTenant(b.dataset.resume, 'resume', main));
}
async function actTenant(id, action, main) {
  try { await apiFetch(`/ops/tenants/${id}/${action}`, { method: 'POST' }); toast((action === 'suspend' ? '已暂停' : '已恢复') + '租户', 'ok'); pageOpsTenants(main); }
  catch (e) { toast(e.message || '操作失败', 'bad'); }
}

/* ---------------- 运营 · 计费策略 ---------------- */
async function pageOpsPricing(main) {
  main.innerHTML = `<div class="page-title">计费策略</div><div class="page-desc">套餐档定义与配额策略（无价格字段，ADR-108）。</div><div class="empty">加载中…</div>`;
  let data;
  try { data = await apiFetch('/ops/pricing'); } catch (e) { main.innerHTML = errCard(e); return; }
  const plans = (data.plans || []).map(p => `<tr><td><span class="badge info">${esc(p.name)}</span></td><td>${num(p.deviceQuota)}</td><td>${num(p.telemetryQuota)}</td><td class="mono">${esc(p.id)}</td></tr>`).join('');
  const pols = (data.quotaPolicies || []).map(p => `<tr><td>${esc(p.metric)}</td><td>${p.thresholdPct}%</td><td><span class="badge ${p.action === 'Throttle' ? 'bad' : 'warn'}">${esc(p.action)}</span></td></tr>`).join('');
  main.innerHTML = `
    <div class="page-title">计费策略</div>
    <div class="page-desc">套餐档定义与配额策略（无价格字段，ADR-108）。</div>
    <div class="grid cols-2">
      <div class="card"><div class="section-title" style="margin-top:0">套餐档</div><table class="tbl"><thead><tr><th>档位</th><th>设备配额</th><th>遥测配额</th><th>ID</th></tr></thead><tbody>${plans}</tbody></table></div>
      <div class="card"><div class="section-title" style="margin-top:0">配额策略</div><table class="tbl"><thead><tr><th>指标</th><th>阈值</th><th>动作</th></tr></thead><tbody>${pols}</tbody></table></div>
    </div>`;
}

/* ---------------- 运营 · 全局角色 ---------------- */
async function pageOpsRoles(main) {
  main.innerHTML = `<div class="page-title">全局角色</div><div class="page-desc">四角色权限矩阵（超管）。</div><div class="empty">加载中…</div>`;
  let data;
  try { data = await apiFetch('/ops/roles'); } catch (e) { main.innerHTML = errCard(e); return; }
  const rows = (data.roles || []).map(r => `<tr><td><span class="badge ${roleBadge(r.role)}">${esc(r.role)}</span></td><td style="color:var(--text-2)">${(r.permissions || []).map(esc).join('、') || '—'}</td></tr>`).join('');
  main.innerHTML = `
    <div class="page-title">全局角色</div>
    <div class="page-desc">四角色权限矩阵（超管）。分析师仅只读（AC-04）。</div>
    <div class="card"><table class="tbl"><thead><tr><th>角色</th><th>权限</th></tr></thead><tbody>${rows}</tbody></table></div>`;
}

/* ---------------- 运营 · 审计日志（Backlog） ---------------- */
function pageOpsAudit(main) {
  main.innerHTML = `
    <div class="page-title">审计日志</div>
    <div class="page-desc">平台与租户审计（Backlog，Phase 4 前补齐）。</div>
    <div class="card"><div class="empty" style="padding:36px">${icon('scroll-text', 'lg')}<p>审计日志暂未启用。</p><p class="muted">AuditLogs 表已建（TenantId 可空=平台），范围控制见 SaaS-Spec §3 Out-of-Scope。</p></div></div>`;
}

/* ---------------- 运营 · 平台健康 ---------------- */
async function pageOpsHealth(main) {
  main.innerHTML = `<div class="page-title">平台健康</div><div class="page-desc">平台 SLO 与各组件健康（超管）。</div><div class="empty">加载中…</div>`;
  let data;
  try { data = await apiFetch('/ops/health'); } catch (e) { main.innerHTML = errCard(e); return; }
  const comps = (data.components || []).map(c => `<tr><td>${esc(c.name)}</td><td><span class="badge ${c.status === 'Healthy' ? 'ok' : c.status === 'Degraded' ? 'warn' : 'bad'}">${esc(c.status)}</span></td></tr>`).join('');
  main.innerHTML = `
    <div class="page-title">平台健康</div>
    <div class="page-desc">平台 SLO 与各组件健康（超管）。</div>
    <div class="grid kpis">
      <div class="card kpi"><div class="label">${icon('activity', 'sm')} 总体状态</div><div class="value" style="font-size:20px"><span class="badge ${data.status === 'Healthy' ? 'ok' : data.status === 'Degraded' ? 'warn' : 'bad'}">${esc(data.status || '—')}</span></div></div>
      <div class="card kpi"><div class="label">${icon('gauge', 'sm')} 可用性 SLA</div><div class="value">${data.sla && data.sla.uptimePct != null ? data.sla.uptimePct + '%' : '—'}</div></div>
    </div>
    <div class="section-title">组件</div>
    <div class="card"><table class="tbl"><thead><tr><th>组件</th><th>状态</th></tr></thead><tbody>${comps || `<tr><td colspan="2" class="muted">无组件数据</td></tr>`}</tbody></table></div>`;
}

function errCard(e) {
  return `<div class="card"><div class="empty">${icon('triangle-alert', 'lg')}<p style="color:var(--bad)">${esc(e && e.message || '请求失败')}</p></div></div>`;
}

/* ---------------- 启动 ---------------- */
window.closeDrawerViaBtn = closeDrawerViaBtn;
loadToken();
if (state.token) probeSuperAdmin().finally(render);
else render();
window.addEventListener('hashchange', render);

/* =========================================================
   PLC·AIOT — 前端逻辑 v3（SaaS 多租户增量）
   体验架构：分组导航 + hash 深链 + 跨页钻取（ctx）
   HTMX 负责页面片段切换，Axios 负责拉取 data/data.json（单一数据源）。

   数据层：loadData() 是唯一数据入口。后端就绪后切到 /api/... 仅需改此处。
   图标：全站 Lucide 雪碧图（<svg class="ico"><use href="#i-xxx"/></svg>），禁 emoji（ADR-106）。
   ========================================================= */
(function () {
  "use strict";

  const STATUS_COLOR = { "在线": "var(--ok)", "预警": "var(--warn)", "告警": "var(--bad)", "离线": "var(--text-3)" };

  /* 信息架构：按「用户任务心智」分五组；SaaS 管理面独立分组（账户 / 运营后台），不混入既有三组。
     图标字段统一为 Lucide symbol 名（参照 DESIGN §10.1 映射表），渲染时用 iconSvg() 生成 <use>。 */
  const GROUPS = [
    { title: "运营监控", items: [
      { id: "dashboard", icon: "layout-dashboard", label: "驾驶舱" },
      { id: "devices",   icon: "cpu", label: "设备" },
      { id: "alarms",    icon: "bell-ring", label: "告警", badge: "12" },
      { id: "realtime",  icon: "activity", label: "实时看板" },
      { id: "map",       icon: "map", label: "地图" }
    ]},
    { title: "分析洞察", items: [
      { id: "statistics", icon: "bar-chart-3", label: "统计" },
      { id: "social",     icon: "megaphone", label: "社媒矩阵" },
      { id: "notion",     icon: "kanban", label: "团队协作" }
    ]},
    { title: "系统治理", items: [
      { id: "system", icon: "sliders-horizontal", label: "系统" },
      { id: "admin",  icon: "boxes", label: "后端管理" }
    ]},
    { title: "账户", items: [
      { id: "account/usage",   icon: "gauge",        label: "用量概览" },
      { id: "account/plan",    icon: "credit-card",  label: "套餐与订阅" },
      { id: "account/members", icon: "users",        label: "成员管理" },
      { id: "account/billing", icon: "receipt",      label: "账单与发票" }
    ]},
    { title: "运营后台", items: [
      { id: "ops/tenants", icon: "building-2",   label: "组织管理" },
      { id: "ops/pricing", icon: "credit-card",  label: "计费策略" },
      { id: "ops/roles",   icon: "shield-check", label: "全局角色" },
      { id: "ops/audit",   icon: "scroll-text",  label: "审计日志" },
      { id: "ops/health",  icon: "activity",     label: "平台健康" }
    ]}
  ];
  const NAV = GROUPS.flatMap(g => g.items);

  let DATA = null;
  let currentPage = "dashboard";
  let searchQ = "";
  let timers = [];
  const ctx = { deviceId: null, alarmFilter: null }; // 跨页上下文

  /* 租户上下文（SaaS 切换器）：当前数据范围。切换后回首页重渲。 */
  let tenantContext = { id: "t-foxconn", name: "—" };
  let confirmCallback = null; // 通用确认弹窗回调

  const $  = (s, r) => (r || document).querySelector(s);
  const $$ = (s, r) => Array.from((r || document).querySelectorAll(s));
  const el = (tag, cls, html) => { const e = document.createElement(tag); if (cls) e.className = cls; if (html != null) e.innerHTML = html; return e; };
  const iconSvg = (name, cls) => `<svg class="${cls || 'ico'}" aria-hidden="true"><use href="#i-${name}"></use></svg>`;
  const fmtNum = (n) => (n == null ? "0" : Number(n).toLocaleString("en-US"));
  function clearTimers() { timers.forEach(t => clearInterval(t)); timers = []; }
  function isSaas(route) { return route.indexOf("account/") === 0 || route.indexOf("ops/") === 0; }
  function currentTenant() {
    const s = DATA && DATA.saas; if (!s) return null;
    return s.tenants.find(t => t.id === tenantContext.id) || s.tenants[0] || null;
  }
  function viewerRole() { return (DATA && DATA.saas && DATA.saas.viewerRole) || "管理员"; }

  /* ---------- 数据加载（唯一数据源；后端就绪改此处为 /api/...） ---------- */
  async function loadData() {
    const res = await axios.get("data/data.json", { responseType: "json" });
    DATA = res.data;
    return DATA;
  }

  /* 配额 meter（AC-11：progressbar + 文本值；阈值色用既有语义色） */
  function meterBar(value, max, label, unit) {
    const pct = max > 0 ? Math.min(100, Math.round(value / max * 100)) : 0;
    const cls = pct >= 95 ? "is-bad" : pct >= 80 ? "is-warn" : "";
    const valStr = unit ? `${fmtNum(value)} ${unit}` : fmtNum(value);
    const maxStr = unit ? `${fmtNum(max)} ${unit}` : fmtNum(max);
    return `<div class="meter" role="progressbar" aria-valuenow="${value}" aria-valuemax="${max}" aria-valuetext="${label} ${valStr} / ${maxStr} · ${pct}%">` +
      `<i class="${cls}" style="width:${pct}%"></i></div>` +
      `<div class="meter-cap">${valStr} / ${maxStr} · ${pct}%</div>`;
  }

  /* =========================================================
     SVG 图表工具（响应式，离线可用）
     ========================================================= */
  function lineChart(series, labels) {
    const W = 600, H = 220, pad = 26;
    const all = series.flatMap(s => s.data);
    const max = Math.max.apply(null, all) * 1.15 || 1;
    const x = i => pad + i * (W - 2 * pad) / (labels.length - 1);
    const y = v => H - pad - (v / max) * (H - 2 * pad);
    let grid = "";
    for (let g = 0; g <= 4; g++) { const gy = pad + g * (H - 2 * pad) / 4; grid += `<line x1="${pad}" y1="${gy}" x2="${W - pad}" y2="${gy}" stroke="currentColor" stroke-opacity=".07"/>`; }
    let defs = "", areas = "", lines = "";
    series.forEach((s, si) => {
      const pts = s.data.map((v, i) => `${x(i)},${y(v)}`).join(" ");
      if (si === 0) {
        defs = `<linearGradient id="lg${si}" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="${s.color}" stop-opacity=".35"/><stop offset="100%" stop-color="${s.color}" stop-opacity="0"/></linearGradient>`;
        const ap = `${x(0)},${H - pad} ${pts} ${x(s.data.length - 1)},${H - pad}`;
        areas = `<polygon points="${ap}" fill="url(#lg${si})"/>`;
      }
      lines += `<polyline points="${pts}" fill="none" stroke="${s.color}" stroke-width="2.5" stroke-linejoin="round" stroke-linecap="round"/>`;
      s.data.forEach((v, i) => { lines += `<circle cx="${x(i)}" cy="${y(v)}" r="2.6" fill="${s.color}"/>`; });
    });
    return `<svg viewBox="0 0 ${W} ${H}" preserveAspectRatio="none" style="color:var(--text-3)"><defs>${defs}</defs>${grid}${areas}${lines}</svg>`;
  }

  function barChart(items, color) {
    const W = 600, H = 160, pad = 20;
    const max = Math.max.apply(null, items.map(i => i.value)) * 1.2 || 1;
    const bw = (W - 2 * pad) / items.length * 0.58;
    const gap = (W - 2 * pad) / items.length;
    let rects = "", labs = "";
    items.forEach((it, i) => {
      const h = (it.value / max) * (H - 2 * pad);
      const x = pad + i * gap + (gap - bw) / 2, y = H - pad - h;
      rects += `<rect x="${x}" y="${y}" width="${bw}" height="${h}" rx="5" fill="${color || "#60a5fa"}"><title>${it.name}: ${it.value}</title></rect>`;
      labs += `<text x="${x + bw / 2}" y="${H - 4}" text-anchor="middle" font-size="11" fill="var(--text-3)">${it.name}</text>`;
    });
    return `<svg viewBox="0 0 ${W} ${H}" preserveAspectRatio="none">${rects}${labs}</svg>`;
  }

  function donut(items) {
    const R = 50, C = 2 * Math.PI * R;
    const total = items.reduce((a, b) => a + b.value, 0) || 1;
    let off = 0, circles = "";
    items.forEach(it => {
      const len = (it.value / total) * C;
      circles += `<circle cx="60" cy="60" r="${R}" fill="none" stroke="${it.color}" stroke-width="14" stroke-dasharray="${len} ${C - len}" stroke-dashoffset="${-off}" transform="rotate(-90 60 60)"/>`;
      off += len;
    });
    return `<svg viewBox="0 0 120 120" style="width:170px;height:170px">${circles}<text x="60" y="58" text-anchor="middle" font-size="20" font-weight="800" fill="var(--text)">${total}</text><text x="60" y="74" text-anchor="middle" font-size="10" fill="var(--text-3)">设备</text></svg>`;
  }

  /* 社媒组合图（折线=曝光，柱=互动，hover 洞察） */
  function socialChart(exp, eng) {
    const W = 600, H = 240, pad = 24, max = Math.max.apply(null, exp) * 1.1 || 1;
    const x = i => pad + i * (W - 2 * pad) / (exp.length - 1);
    const y = v => H - pad - (v / max) * (H - 2 * pad);
    let grid = "";
    for (let g = 0; g <= 4; g++) { const gy = pad + g * (H - 2 * pad) / 4; grid += `<line x1="${pad}" y1="${gy}" x2="${W - pad}" y2="${gy}" stroke="currentColor" stroke-opacity=".07"/>`; }
    const pts = exp.map((v, i) => `${x(i)},${y(v)}`).join(" ");
    const area = `<polygon points="${x(0)},${H - pad} ${pts} ${x(exp.length - 1)},${H - pad}" fill="url(#slg)"/>`;
    let line = `<polyline points="${pts}" fill="none" stroke="#3b82f6" stroke-width="2.5"/>`;
    let bars = "", dots = "";
    const bw = 18;
    exp.forEach((v, i) => {
      const bh = (v / max) * (H - 2 * pad);
      bars += `<rect class="sdot" data-i="${i}" x="${x(i) - bw / 2}" y="${H - pad - bh}" width="${bw}" height="${bh}" rx="4" fill="#93c5fd" opacity=".85"/>`;
      line += `<circle class="sdot" data-i="${i}" cx="${x(i)}" cy="${y(v)}" r="3.5" fill="#3b82f6"/>`;
    });
    return `<svg viewBox="0 0 ${W} ${H}" style="color:var(--text-3)"><defs><linearGradient id="slg" x1="0" y1="0" x2="0" y2="1"><stop offset="0%" stop-color="#3b82f6" stop-opacity=".3"/><stop offset="100%" stop-color="#3b82f6" stop-opacity="0"/></linearGradient></defs>${grid}${area}${line}${bars}${dots}</svg>`;
  }

  /* =========================================================
     跨页钻取核心：全局打开设备抽屉（修复 B3/B4/B5/B6/B7）
     ========================================================= */
  function openDevice(id) {
    const d = DATA && DATA.devices.find(x => x.id === id);
    if (d) openDrawer(d);
    return !!d;
  }

  /* =========================================================
     各页渲染
     ========================================================= */
  const renderers = {
    dashboard(data, root) {
      $("#kpiGrid", root).innerHTML = data.kpis.map(k => `
        <div class="card kpi ${k.accent}">
          <div class="top"><span class="label">${k.label}</span><span class="delta ${k.trend}">${k.trend === "up" ? "↑" : "↓"} ${k.delta}</span></div>
          <div class="value">${k.value}<span class="u">${k.unit}</span></div>
          <div class="bar"><i style="width:${k.pct}%"></i></div>
        </div>`).join("");
      $("#throughputChart", root).innerHTML = lineChart(data.throughput.series, data.throughput.labels);
      $("#throughputLegend", root).innerHTML = data.throughput.series.map(s => `<span><i style="background:${s.color}"></i>${s.name}</span>`).join("");
      $("#aggregateChart", root).innerHTML = barChart(data.aggregate, "#60a5fa");
      $("#statusList", root).innerHTML = data.statusDist.map(s => `
        <div class="stat-row"><span><span style="color:${s.color}">●</span> ${s.label}</span><b>${s.value}</b></div>`).join("");
      const active = data.alarms.filter(a => a.st !== "已解决").length;
      const cta = $("#alertCta", root);
      if (cta) {
        cta.innerHTML = `${iconSvg("alert-triangle", "ico")}<div><b>${active} 条未决告警</b><div style="font-size:12px;color:var(--text-3)">点击进入告警页并预置「活跃」筛选</div></div><span class="go">去处理 →</span>`;
        cta.onclick = () => { ctx.alarmFilter = "active"; navigate("alarms"); };
        cta.onkeydown = e => { if (e.key === "Enter" || e.key === " ") { e.preventDefault(); cta.onclick(); } };
      }
      const stream = $("#stream", root);
      const devs = data.devices;
      function push() {
        const a = data.activity[i++ % data.activity.length];
        const dev = devs[i % devs.length];
        const li = el("li");
        li.style.cursor = "pointer";
        li.title = "查看 " + dev.name;
        li.innerHTML = `${iconSvg(a.icon, "ico ico-sm tone-" + (a.tone || "info"))}<div style="min-width:0"><div style="overflow:hidden;text-overflow:ellipsis;white-space:nowrap">${a.text}</div><code>${a.topic}</code></div><span class="t">${new Date().toLocaleTimeString()}</span>`;
        li.onclick = () => openDevice(dev.id);
        stream.prepend(li);
        if (stream.children.length > 12) stream.lastChild.remove();
      }
      let i = 0;
      for (let k = 0; k < 6; k++) push();
      timers.push(setInterval(push, 2500));
      timers.push(setInterval(() => { const l = $("#lastSync", root); if (l) l.textContent = new Date().toLocaleTimeString(); }, 1500));
    },

    devices(data, root) {
      const grid = $("#devGrid", root);
      function render() {
        const f = $("#devFilter", root).value;
        const q = searchQ;
        const list = data.devices.filter(d => (!f || d.status === f) && (!q || (d.id + " " + d.name + " " + d.model).toLowerCase().includes(q)));
        grid.innerHTML = "";
        list.forEach(d => {
          const c = el("div", "card hover dev-card");
          c.innerHTML = `
            <div class="row"><span style="font-weight:600">${d.name}</span><span class="badge ${d.status === "在线" ? "ok" : d.status === "预警" ? "warn" : d.status === "告警" ? "bad" : "muted"}">${d.status}</span></div>
            <div class="meta">${d.id} · ${d.model} · ${d.proto}</div>
            <div class="foot"><span>心跳 ${d.last}</span><span class="go">详情 →</span></div>`;
          c.onclick = () => openDrawer(d);
          grid.appendChild(c);
        });
        const cnt = $("#devCount", root);
        if (cnt) cnt.textContent = `${list.length} 台设备` + (q ? `（匹配「${q}」）` : "");
      }
      $("#devFilter", root).onchange = render;
      render();
    },

    alarms(data, root) {
      const body = $("#almBody", root), stats = $("#almStats", root);
      const filter = ctx.alarmFilter;
      function render() {
        const f = filter || "all";
        const active = data.alarms.filter(a => a.st !== "已解决").length;
        const ack = data.alarms.filter(a => a.st === "已确认").length;
        const resolved = data.alarms.filter(a => a.st === "已解决").length;
        stats.innerHTML = [
          ["活跃", active, "bad"], ["已确认", ack, ""], ["已解决", resolved, "ok"], ["MTTR", "8.4m", "ok"]
        ].map(([l, v, c]) => `<div class="card kpi ${c}"><div class="label">${l}</div><div class="value">${v}</div></div>`).join("");
        body.innerHTML = "";
        data.alarms.filter(a => f === "all" || (f === "active" && a.st !== "已解决") || (f === "resolved" && a.st === "已解决"))
          .forEach(a => {
            const lc = a.lv === "严重" ? "bad" : a.lv === "警告" ? "warn" : "info";
            const tr = el("tr");
            tr.innerHTML = `<td style="color:var(--text-3)">${a.t}</td><td>${a.dev}</td>
              <td><span class="badge ${lc}">${a.lv}</span></td><td>${a.msg}</td><td>${a.st}</td>
              <td style="text-align:right;white-space:nowrap">
                <span class="link" data-act="view">查看</span>
                ${a.st !== "已解决"
                  ? `<span class="link" data-act="ack" style="margin-left:10px">确认</span><span class="link bad" data-act="resolve" style="margin-left:10px">解决</span>`
                  : `<span class="link bad" data-act="close" style="margin-left:10px">关闭</span>`}
              </td>`;
            tr.querySelector('[data-act="view"]').onclick = () => openDevice(a.dev);
            const ackB = tr.querySelector('[data-act="ack"]'); if (ackB) ackB.onclick = () => { a.st = "已确认"; render(); updateAlarmBadge(); };
            const resB = tr.querySelector('[data-act="resolve"]'); if (resB) resB.onclick = () => { a.st = "已解决"; render(); updateAlarmBadge(); };
            const clB = tr.querySelector('[data-act="close"]'); if (clB) clB.onclick = () => render();
            body.appendChild(tr);
          });
      }
      $$("#almFilter button", root).forEach(b => {
        if ((filter || "all") === b.dataset.s) b.classList.add("on"); else b.classList.remove("on");
        b.onclick = () => { ctx.alarmFilter = b.dataset.s === "all" ? null : b.dataset.s; $$("#almFilter button", root).forEach(x => x.classList.remove("on")); b.classList.add("on"); render(); };
      });
      render();
    },

    statistics(data, root) {
      $("#statKpis", root).innerHTML = [
        ["日均采集点", data.stats.dailyAvg, "M", "brand"],
        ["OEE", data.oee.value, "%", "ok"],
        ["协议种类", data.stats.protocols, "", "brand"],
        ["园区数", data.stats.parks, "", "brand"]
      ].map(([l, v, u, c]) => `<div class="card kpi ${c}"><div class="label">${l}</div><div class="value">${v}<span class="u">${u}</span></div></div>`).join("");
      $("#donutChart", root).innerHTML = donut(data.statusDist);
      $("#donutLegend", root).innerHTML = data.statusDist.map(s => `<span><i style="background:${s.color}"></i>${s.label} ${s.value}</span>`).join("");
      $("#trendLine", root).innerHTML = lineChart([{ name: "采集点/s", color: "#3b82f6", data: [820, 1180, 940, 1320, 1260, 1020, 980] }], ["一", "二", "三", "四", "五", "六", "日"]);
      $("#alarmBar", root).innerHTML = barChart(data.alarmTrend, "#f59e0b");
      $("#oeeNote", root).textContent = data.oee.sub;
    },

    realtime(data, root) {
      const modules = data.plcModules;
      const W = 760, H = 300, startX = 40, gap = (W - 80) / modules.length;
      let mods = "", legend = "";
      modules.forEach((m, i) => {
        const x = startX + i * gap, bw = 68, bh = 150, y = 70;
        mods += `<g class="plc-mod" data-key="${m.key}" style="cursor:pointer">
          <rect x="${x}" y="${y}" width="${bw}" height="${bh}" rx="8" fill="${m.color}" fill-opacity=".85" stroke="rgba(255,255,255,.25)"/>
          <circle cx="${x + bw / 2}" cy="${y + 18}" r="5" fill="#22c55e"><animate attributeName="opacity" values="1;.4;1" dur="1.6s" repeatCount="indefinite"/></circle>
          <text x="${x + bw / 2}" y="${y + 52}" text-anchor="middle" font-size="14" font-weight="700" fill="#fff">${m.key}</text>
          <text x="${x + bw / 2}" y="${y + 72}" text-anchor="middle" font-size="9" fill="rgba(255,255,255,.85)">${m.name.slice(0, 12)}</text>
        </g>`;
        legend += `<span><i style="background:${m.color}"></i>${m.name}</span>`;
      });
      $("#plcRack", root).innerHTML = `<svg viewBox="0 0 ${W} ${H}" class="chart" id="plcSvg">
        <rect x="20" y="${H - 60}" width="${W - 40}" height="10" rx="4" fill="#1e293b" opacity=".5"/>
        ${mods}
        <text x="20" y="40" font-size="12" fill="var(--text-3)">PLC 机架 · 程序化结构透视（悬停模块查看遥测）</text></svg>`;
      $("#plcLegend", root).innerHTML = legend;
      const svg = $("#plcSvg", root);
      const info = $("#plcInfo", root);
      function show(m) {
        info.innerHTML = `<div style="font-weight:700;color:var(--brand-500)">${m.name}</div><div style="color:var(--text-3)" id="plcMetric">${m.metric}</div><div style="font-size:12px;color:var(--text-3);margin-top:6px">资产管线：Blender MCP → .glb → GLTFLoader（此处为程序化机架回退）</div>`;
        tip.textContent = `${m.name} · ${m.metric}`;
      }
      svg.addEventListener("mousemove", e => {
        const g = e.target.closest("g.plc-mod");
        if (g) { const m = modules.find(x => x.key === g.dataset.key); show(m); moveTip(e); }
        else { tip.style.display = "none"; }
      });
      svg.addEventListener("mouseleave", () => { tip.style.display = "none"; });
      timers.push(setInterval(() => {
        if (currentPage !== "realtime") return;
        info.innerHTML = modules.map(m => `<div class="stat-row"><span>${m.name}</span><b style="color:var(--ok)">${m.metric}</b></div>`).join("");
      }, 2000));
      info.innerHTML = modules.map(m => `<div class="stat-row"><span>${m.name}</span><b style="color:var(--ok)">${m.metric}</b></div>`).join("");
    },

    map(data, root) {
      const W = 760, H = 380;
      let grid = "";
      for (let i = 1; i < 12; i++) { const x = i * W / 12; grid += `<line x1="${x}" y1="0" x2="${x}" y2="${H}" stroke="var(--brand-500)" stroke-opacity=".06"/>`; }
      for (let j = 1; j < 7; j++) { const y = j * H / 7; grid += `<line x1="0" y1="${y}" x2="${W}" y2="${y}" stroke="var(--brand-500)" stroke-opacity=".06"/>`; }
      const proj = (lat, lon) => [(lon + 180) / 360 * W, (90 - lat) / 180 * H];
      const hub = proj(34.75, 113.62);
      let arcs = "", pins = "", list = "";
      data.devices.forEach(d => {
        const [px, py] = proj(d.lat, d.lon); const col = STATUS_COLOR[d.status];
        const mx = (px + hub[0]) / 2, my = (py + hub[1]) / 2 - 60;
        arcs += `<path d="M${px},${py} Q${mx},${my} ${hub[0]},${hub[1]}" fill="none" stroke="#60a5fa" stroke-opacity=".28" stroke-width="1"/>`;
        const active = ctx.deviceId === d.id;
        pins += `<g class="map-pin" data-id="${d.id}" style="cursor:pointer"><circle cx="${px}" cy="${py}" r="${active ? 9 : 6}" fill="${col}"/><circle cx="${px}" cy="${py}" r="6" fill="none" stroke="${col}" stroke-opacity=".5"><animate attributeName="r" values="6;13;6" dur="2s" repeatCount="indefinite"/><animate attributeName="stroke-opacity" values=".6;0;.6" dur="2s" repeatCount="indefinite"/></circle>${active ? `<circle cx="${px}" cy="${py}" r="13" fill="none" stroke="#fff" stroke-width="2"/>` : ""}</g>`;
        list += `<div class="stat-row" data-dev="${d.id}" style="cursor:pointer"><span>${d.name}</span><b style="color:${col}">${d.status}</b></div>`;
      });
      $("#mapSvg", root).innerHTML = `<svg viewBox="0 0 ${W} ${H}" class="chart" id="worldSvg" style="background:radial-gradient(circle at 50% 40%, rgba(59,130,246,.10), transparent 70%)">
        ${grid}${arcs}${pins}<circle cx="${hub[0]}" cy="${hub[1]}" r="7" fill="#fff" stroke="var(--brand-500)" stroke-width="3"/>
        <text x="10" y="24" font-size="12" fill="var(--text-3)">全球设备分布 · 中心枢纽 郑州</text></svg>`;
      $("#mapList", root).innerHTML = list;
      const svg = $("#worldSvg", root);
      const info = $("#mapInfo", root);
      svg.addEventListener("mousemove", e => {
        const g = e.target.closest("g.map-pin");
        if (g) { const d = data.devices.find(x => x.id === g.dataset.id); info.innerHTML = `<div style="font-weight:700;color:var(--brand-500)">${d.name}</div><div style="color:var(--text-3)">${d.id} · ${d.model}</div><div style="color:var(--text-3)">状态 ${d.status} · 心跳 ${d.last}</div>`; tip.textContent = `${d.name} · ${d.status}`; moveTip(e); }
        else { tip.style.display = "none"; }
      });
      svg.addEventListener("mouseleave", () => { tip.style.display = "none"; });
      svg.addEventListener("click", e => { const g = e.target.closest("g.map-pin"); if (g) openDevice(g.dataset.id); });
      $$("#mapList [data-dev]", root).forEach(r => r.onclick = () => openDevice(r.dataset.dev));
    },

    system(data, root) {
      $("#svcGrid", root).innerHTML = data.services.map(v => {
        const sc = v.s === "在线" ? "#10b981" : v.s === "同步中" ? "#3b82f6" : "#f59e0b";
        return `<div class="card"><div class="row"><span style="font-weight:600">${v.n}</span><span class="dot" style="width:8px;height:8px;border-radius:50%;background:${sc};display:inline-block;animation:pulse 1.8s infinite"></span></div><div style="font-size:12px;color:var(--text-3);margin-top:4px">${v.d}</div><div style="font-size:12px;margin-top:8px;color:${sc}">${v.s}</div></div>`;
      }).join("");
      $("#resGrid", root).innerHTML = data.resources.map(r =>
        `<div><div class="row"><span>${r.k}</span><b>${r.v}</b></div><div class="meter"><i style="width:${r.p}%"></i></div></div>`).join("");
    },

    admin(data, root) {
      const panel = $("#adminPanel", root);
      function render(t) {
        const rows = data.admin[t];
        let html = '<table class="tbl"><tbody>';
        rows.forEach(r => {
          if (t === "plugin") html += `<tr><td style="font-weight:600">${r[0]}</td><td style="color:var(--text-3)">${r[1]}</td><td><span class="badge ${r[2] === "已启用" ? "ok" : "muted"}">${r[2]}</span></td><td style="text-align:right"><span class="link">${r[2] === "已启用" ? "停用" : "启用"}</span></td></tr>`;
          else if (t === "user") html += `<tr><td style="font-weight:600">${r[0]}</td><td style="color:var(--text-3)">${r[1]}</td><td><span class="badge info">${r[2]}</span></td></tr>`;
          else if (t === "topic") html += `<tr><td><code>${r[0]}</code></td><td><span class="badge warn">${r[1]}</span></td></tr>`;
          else html += `<tr><td style="font-weight:600">${r[0]}</td><td><span class="badge ${r[1].indexOf("连接") >= 0 ? "ok" : "info"}">${r[1]}</span></td></tr>`;
        });
        html += "</tbody></table>";
        panel.innerHTML = html;
      }
      $$("#adminTabs button", root).forEach(b => b.onclick = () => {
        $$("#adminTabs button", root).forEach(x => x.classList.remove("on")); b.classList.add("on"); render(b.dataset.t);
      });
      render("plugin");
    },

    social(data, root) {
      const s = data.social;
      $("#socialKpis", root).innerHTML = s.kpis.map(k =>
        `<div class="card kpi ${k.trend}"><div class="label">${k.label}</div><div class="value">${k.value}</div><div class="delta ${k.trend}">${k.trend === "up" ? "↑" : "↓"} ${k.delta}</div></div>`).join("");
      const chart = $("#socialChart", root), label = $("#rangeLabel", root);
      function draw(range) {
        const d = s.ranges[range];
        label.textContent = d.label;
        chart.innerHTML = socialChart(d.exp, d.eng);
        $$("#socialChart .sdot", root).forEach(dot => dot.addEventListener("mousemove", e => {
          const i = +dot.dataset.i;
          tip.style.display = "block"; tip.style.left = e.clientX + "px"; tip.style.top = e.clientY + "px";
          tip.innerHTML = `第 ${i + 1} 期<br>曝光 ${d.exp[i]}M · 互动 ${d.eng[i]}K`;
        }));
        chart.onmouseleave = () => { tip.style.display = "none"; };
      }
      draw("7");
      $("#socialTable", root).innerHTML = s.platforms.map(p =>
        `<tr><td style="font-weight:600">${p.n}</td><td>${p.exp}</td><td>${p.eng}</td><td>${p.fan}</td><td>${p.rate}</td></tr>`).join("");
      $$("#socialRanges button", root).forEach(b => b.onclick = () => {
        $$("#socialRanges button", root).forEach(x => x.classList.remove("on")); b.classList.add("on"); draw(b.dataset.r);
      });
    },

    notion(data, root) {
      const n = data.notion;
      $("#notionKpis", root).innerHTML = n.kpis.map(k => {
        const up = k.d >= 0;
        return `<div class="card kpi"><div class="label">${k.t}</div><div class="value">${k.v}</div><div class="delta ${up ? "up" : "down"}">${up ? "↑" : "↓"} ${Math.abs(k.d)}%</div></div>`;
      }).join("");
      $("#notionTrend", root).innerHTML = lineChart([{ name: "趋势", color: "#3b82f6", data: n.trend }], ["一", "二", "三", "四", "五", "六", "日"]);
      const sync = $("#notionSync", root); if (sync) sync.textContent = "同步：" + new Date().toLocaleTimeString();
      const stream = $("#notionStream", root);
      function push() {
        const li = el("div", "stream-row");
        li.innerHTML = `<span class="dot" style="background:#10b981;width:7px;height:7px;border-radius:50%;flex:0 0 auto"></span><span>${n.acts[Math.floor(Math.random() * n.acts.length)]}</span><span style="margin-left:auto;color:var(--text-3);font-size:12px">${new Date().toLocaleTimeString()}</span>`;
        stream.prepend(li); if (stream.children.length > 7) stream.lastChild.remove();
      }
      for (let k = 0; k < 4; k++) push();
      timers.push(setInterval(push, 3000));
      $("#notionTasks", root).innerHTML = n.tasks.map(t => {
        const tc = t.st === "进行中" ? "info" : t.st === "已完成" ? "ok" : t.st === "阻塞" ? "bad" : "muted";
        return `<tr><td style="font-weight:600">${t.name}</td><td style="color:var(--text-3)">${t.owner}</td><td><span class="badge ${tc}">${t.st}</span></td><td><button class="link" data-dev="${t.dev}">${t.dev}</button></td><td>${t.due}</td></tr>`;
      }).join("");
      $$("#notionTasks [data-dev]", root).forEach(b => b.onclick = () => openDevice(b.dataset.dev));
      $("#notionConn", root).innerHTML = n.connectors.map(c => `<span class="chip" onclick="this.classList.toggle('on')">${c}</span>`).join("");
    },

    /* ===================== SaaS · 账户 ===================== */

    "account/usage"(data, root) {
      const t = currentTenant(); if (!t) return;
      const u = t.usage;
      const sb = $("#sbTenant", root); if (sb) sb.textContent = t.name;
      const periodEl = $("#usagePeriod", root); if (periodEl) periodEl.textContent = u.period;
      const items = [
        { label: "设备数", value: u.deviceCount, max: u.deviceQuota, unit: "" },
        { label: "席位", value: u.seats, max: u.seatQuota, unit: "" },
        { label: "API 调用", value: u.apiCalls, max: u.apiQuota, unit: "" },
        { label: "存储", value: u.storageGB, max: u.storageQuota, unit: "GB" }
      ];
      $("#usageKpis", root).innerHTML = items.map(it => `
        <div class="card kpi">
          <div class="top"><span class="label">${it.label}</span></div>
          <div class="value">${it.unit ? fmtNum(it.value) : fmtNum(it.value)}<span class="u">${it.unit ? " " + it.unit : ""}</span></div>
          ${meterBar(it.value, it.max, it.label, it.unit)}
        </div>`).join("");
      $("#usageChart", root).innerHTML = lineChart([{ name: "日均遥测点（百万）", color: "#3b82f6", data: u.trend }], ["一", "二", "三", "四", "五", "六", "日"]);
      const empty = $("#usageEmpty", root);
      if (u.deviceCount === 0) {
        empty.hidden = false;
        empty.innerHTML = `<div class="card empty-state">
          ${iconSvg("cpu", "ico ico-lg")}
          <div><b>本组织暂无设备</b><div class="sub">注册首台设备后即可查看用量</div></div>
          <button class="btn btn-primary" id="regDev">注册首台设备</button></div>`;
        const b = $("#regDev", root); if (b) b.onclick = () => navigate("devices");
      } else { empty.hidden = true; }
      $$("#usageRange button", root).forEach(b => b.onclick = () => {
        $$("#usageRange button", root).forEach(x => x.classList.remove("on")); b.classList.add("on");
      });
    },

    "account/plan"(data, root) {
      const t = currentTenant(); if (!t) return;
      const sb = $("#sbTenant", root); if (sb) sb.textContent = t.name;
      const plans = data.saas.plans;
      const p = plans.find(x => x.name === t.plan) || plans[1];
      $("#planCurrent", root).innerHTML = `
        <div class="row" style="align-items:center;justify-content:space-between">
          <div><span class="badge tier">${t.plan}</span><div style="font-size:12px;color:var(--text-3);margin-top:8px">当前套餐档 · 内网免费使用</div></div>
          <div style="text-align:right"><div class="kpi-value-sm">${fmtNum(p.deviceQuota)}</div><div class="sub">设备配额上限</div></div>
        </div>
        <div class="plan-meters">${meterBar(t.usage.deviceCount, p.deviceQuota, "设备数", "")}${meterBar(t.usage.telemetryPoints, p.telemetryQuota, "遥测点", "")}</div>
        <button class="btn btn-primary" id="planCta" ${viewerRole() !== "管理员" ? 'aria-disabled="true" disabled' : ""}>
          ${iconSvg("arrow-up-right", "ico ico-sm")} ${nextPlanName(plans, t.plan) ? "升级到" + nextPlanName(plans, t.plan) + "以提升设备上限" : "已为最高档"}
        </button>`;
      const cta = $("#planCta", root);
      if (cta && !cta.disabled) cta.onclick = () => {
        const np = nextPlanName(plans, t.plan);
        if (!np) return;
        openConfirm({
          title: "切换套餐档",
          desc: "套餐档仅决定配额上限，内网免费使用，不产生费用。确定切换？",
          target: `<span class="ts-dlg-row">${t.plan} → <b>${np}</b></span>`,
          onConfirm: () => { t.plan = np; showToast(root, "已切换至" + np); renderers["account/plan"](data, root); buildTenantSwitcher(); }
        });
      };
      $("#planGrid", root).innerHTML = plans.map(pl => {
        const cur = pl.name === t.plan;
        const canUp = !cur && viewerRole() === "管理员" && nextPlanName(plans, t.plan) === pl.name;
        return `<div class="card hover plan-card ${cur ? "is-current" : ""}">
          <div class="row" style="align-items:center;justify-content:space-between">
            <span class="badge tier">${pl.name}</span>${cur ? '<span class="badge info">当前方案</span>' : ""}
          </div>
          <ul class="feat">
            ${pl.features.map(f => `<li>${iconSvg(pl.lock && !cur ? "lock" : "check", "ico ico-sm")}<span>${f}</span></li>`).join("")}
          </ul>
          ${canUp ? `<button class="btn btn-ghost plan-pick" data-plan="${pl.name}">选择</button>` : (cur ? "" : `<button class="btn btn-ghost plan-pick" data-plan="${pl.name}" ${viewerRole() !== "管理员" ? "disabled aria-disabled=true" : ""}>选择</button>`)}
        </div>`;
      }).join("");
      $$("#planGrid .plan-pick", root).forEach(b => b.onclick = () => {
        if (b.disabled) return;
        const np = b.dataset.plan;
        openConfirm({
          title: "切换套餐档",
          desc: "套餐档仅决定配额上限，内网免费使用，不产生费用。确定切换？",
          target: `<span class="ts-dlg-row">${t.plan} → <b>${np}</b></span>`,
          onConfirm: () => { t.plan = np; showToast(root, "已切换至" + np); renderers["account/plan"](data, root); buildTenantSwitcher(); }
        });
      });
    },

    "account/members"(data, root) {
      const t = currentTenant(); if (!t) return;
      const sb = $("#sbTenant", root); if (sb) sb.textContent = t.name;
      const isAdmin = viewerRole() === "管理员";
      const inviteBtn = $("#inviteBtn", root);
      if (inviteBtn) {
        if (!isAdmin) { inviteBtn.style.display = "none"; }
        else inviteBtn.onclick = () => openInvite();
      }
      const perms = $("#memberPerm", root);
      if (perms) perms.hidden = isAdmin;
      const rows = $("#memberRows", root);
      if (!t.members.length) {
        rows.innerHTML = "";
        const empty = $("#memberEmpty", root);
        empty.hidden = false;
        empty.innerHTML = `<div class="card empty-state">${iconSvg("users", "ico ico-lg")}<div><b>本组织暂无其他成员</b><div class="sub">${isAdmin ? "邀请同事协作" : "邀请需由组织管理员操作"}</div></div></div>`;
      } else {
        rows.innerHTML = t.members.map(m => `
          <tr>
            <td><div style="font-weight:600">${m.name}</div><div style="font-size:12px;color:var(--text-3)">${m.email}</div></td>
            <td><span class="badge info">${m.role}</span></td>
            <td><span class="badge ok">${m.status}</span></td>
            <td style="font-family:var(--mono);font-size:12px;color:var(--text-2)">${m.last}</td>
            <td style="text-align:right;white-space:nowrap">
              ${isAdmin ? `<span class="link" data-role="${m.id}">改角色</span> <span class="link bad" data-rm="${m.id}" style="margin-left:10px">移除</span>` : '<span style="color:var(--text-3)">—</span>'}
            </td>
          </tr>`).join("");
        $$("[data-role]", rows).forEach(b => b.onclick = () => changeRole(t, b.dataset.role, root));
        $$("[data-rm]", rows).forEach(b => b.onclick = () => removeMember(t, b.dataset.rm, root));
      }
      // 邀请抽屉事件
      const invClose = $("#invClose", root), invMask = $("#inviteMask", root), invSubmit = $("#invSubmit", root);
      if (invClose) invClose.onclick = closeInvite;
      if (invMask) invMask.onclick = closeInvite;
      if (invSubmit) invSubmit.onclick = () => submitInvite(t, root);
    },

    "account/billing"(data, root) {
      const t = currentTenant(); if (!t) return;
      const sb = $("#sbTenant", root); if (sb) sb.textContent = t.name;
      const u = t.usage;
      $("#billingReview", root).innerHTML = `
        <div class="kpi-grid kpi-grid-2">
          <div class="card kpi"><div class="label">设备数</div><div class="value">${fmtNum(u.deviceCount)}</div></div>
          <div class="card kpi"><div class="label">遥测点数</div><div class="value">${fmtNum(u.telemetryPoints)}</div></div>
          <div class="card kpi"><div class="label">配额剩余（设备）</div><div class="value">${fmtNum(u.deviceQuota - u.deviceCount)}</div></div>
          <div class="card kpi"><div class="label">当期</div><div class="value" style="font-size:20px">${u.period}</div></div>
        </div>`;
      const body = $("#invoiceBody", root);
      if (body) body.innerHTML = `<tr><td colspan="4" style="text-align:center;color:var(--text-3);padding:28px">当前账期暂无账单</td></tr>`;
      root.setAttribute("aria-disabled", "true");
    },

    /* ===================== SaaS · 运营后台 ===================== */

    "ops/tenants"(data, root) {
      const s = data.saas;
      const body = $("#tenantRows", root);
      if (!s.tenants.length) {
        body.innerHTML = `<tr><td colspan="7" style="text-align:center;color:var(--text-3);padding:28px">尚无租户，点击开通组织创建首个租户</td></tr>`;
        return;
      }
      body.innerHTML = s.tenants.map(t => {
        const stCls = t.status === "active" ? "ok" : t.status === "suspended" ? "muted" : "warn";
        const stText = t.status === "active" ? "活跃" : t.status === "suspended" ? "暂停" : "试用";
        return `<tr>
          <td><div style="display:flex;align-items:center;gap:8px"><svg class="ico ico-sm" style="color:var(--tenant-dot)"><use href="#i-building-2"/></svg><b>${t.name}</b></div><div style="font-size:12px;color:var(--text-3)">${t.slug}</div></td>
          <td><span class="badge tier">${t.plan}</span></td>
          <td>${meterBar(t.usage.deviceCount, t.usage.deviceQuota, "设备数", "").replace('class="meter"', 'class="meter meter-sm"')}</td>
          <td><span class="badge ${stCls}">${stText}</span></td>
          <td style="font-family:var(--mono);font-size:12px;color:var(--text-2)">${t.renew}</td>
          <td style="text-align:right;white-space:nowrap">
            <button class="btn btn-ghost btn-sm" data-edit="${t.id}">${iconSvg("pencil", "ico ico-sm")}</button>
            ${t.status === "suspended"
              ? `<button class="btn btn-ghost btn-sm" data-resume="${t.id}" style="margin-left:6px">${iconSvg("circle-check", "ico ico-sm")} 恢复</button>`
              : `<button class="btn btn-ghost btn-sm" data-suspend="${t.id}" style="margin-left:6px;color:var(--bad)">${iconSvg("ban", "ico ico-sm")} 暂停</button>`}
          </td>
        </tr>`;
      }).join("");
      $$("[data-suspend]", body).forEach(b => b.onclick = () => {
        const t = s.tenants.find(x => x.id === b.dataset.suspend);
        openConfirm({
          title: "暂停组织",
          desc: "确认暂停该组织？暂停后其用户将无法登录，数据保留。",
          target: `<span class="ts-dlg-row">${iconSvg("building-2", "ico ico-sm")} ${t.name}</span>`,
          onConfirm: () => { t.status = "suspended"; renderers["ops/tenants"](data, root); }
        });
      });
      $$("[data-resume]", body).forEach(b => b.onclick = () => {
        const t = s.tenants.find(x => x.id === b.dataset.resume);
        openConfirm({
          title: "恢复组织",
          desc: "确认恢复该组织访问？",
          target: `<span class="ts-dlg-row">${iconSvg("building-2", "ico ico-sm")} ${t.name}</span>`,
          onConfirm: () => { t.status = "active"; renderers["ops/tenants"](data, root); }
        });
      });
      $$("[data-edit]", body).forEach(b => b.onclick = () => {
        const t = s.tenants.find(x => x.id === b.dataset.edit);
        showToast(root, "编辑「" + t.name + "」开通信息（MVP 只读）");
      });
    },

    "ops/pricing"(data, root) {
      const s = data.saas;
      const body = $("#pricingBody", root);
      body.innerHTML = s.plans.map(pl => `
        <tr>
          <td><span class="badge tier">${pl.name}</span></td>
          <td style="font-family:var(--mono)">${fmtNum(pl.deviceQuota)}</td>
          <td style="font-family:var(--mono)">${fmtNum(pl.telemetryQuota)}</td>
          <td>${pl.features.map(f => `<span class="feat-inline">${iconSvg("check", "ico ico-sm")}${f}</span>`).join("")}</td>
        </tr>`).join("");
      $("#quotaPolicy", root).innerHTML = s.pricing.quotaPolicies.map(p => `
        <div class="card"><div class="row" style="justify-content:space-between"><b>${p.label}</b><span class="badge ${p.action === "Throttle" ? "bad" : "warn"}">${p.thresholdPct}% · ${p.action === "Throttle" ? "限流" : "提醒"}</span></div></div>`).join("");
    },

    "ops/roles"(data, root) {
      const s = data.saas;
      const roles = ["管理员", "分析师", "操作员", "设备工程师"];
      const head = `<tr><th scope="col">资源 / 动作</th>${roles.map(r => `<th scope="col">${r}</th>`).join("")}</tr>`;
      const body = s.roles.matrix.map(row => {
        return `<tr><td style="font-weight:600">${row.action}</td>${roles.map(r => {
          const allowed = row.权限.indexOf(r) >= 0;
          return `<td style="text-align:center">${allowed
            ? `<svg class="ico ico-sm" style="color:var(--ok)" aria-label="允许"><use href="#i-check"/></svg>`
            : `<svg class="ico ico-sm" style="color:var(--text-3)" aria-label="不允许"><use href="#i-minus"/></svg>`}</td>`;
        }).join("")}</tr>`;
      }).join("");
      $("#roleMatrix", root).innerHTML = `<thead>${head}</thead><tbody>${body}</tbody>`;
    },

    "ops/audit"(data, root) {
      const at = $("#auditTenant", root);
      if (at) at.textContent = (currentTenant() && currentTenant().name) || "全局";
      const body = $("#auditBody", root);
      if (body) body.innerHTML = `<tr><td colspan="5" style="text-align:center;color:var(--text-3);padding:28px">审计日志将于平台正式运营前启用，届时记录跨租户操作便于追溯</td></tr>`;
    },

    "ops/health"(data, root) {
      const h = data.saas.health;
      const degraded = h.components.filter(c => c.status !== "ok").length;
      const sumEl = $("#healthSummary", root);
      if (sumEl) {
        if (degraded === 0) { sumEl.className = "scope-bar ok-bar"; sumEl.innerHTML = `${iconSvg("circle-check", "ico ico-sm")}<span>全部服务正常 · 可用性 ${h.uptimePct}%</span>`; }
        else { sumEl.className = "scope-bar warn-bar"; sumEl.innerHTML = `${iconSvg("alert-triangle", "ico ico-sm")}<span>${degraded} 项服务降级，已触发告警</span>`; }
      }
      $("#healthGrid", root).innerHTML = h.components.map(c => {
        const cls = c.status === "ok" ? "ok" : c.status === "warn" ? "is-warn" : "is-bad";
        const label = c.status === "ok" ? "正常" : c.status === "warn" ? "降级" : "故障";
        const dotCls = c.status === "ok" ? "dot-ok" : c.status === "warn" ? "dot-warn" : "dot-bad";
        return `<div class="card ${cls}">
          <div class="row" style="align-items:center;justify-content:space-between"><b>${c.name}</b><span class="${dotCls}"></span></div>
          <div class="kpi-grid kpi-grid-2" style="margin-top:10px">
            <div><div class="sub">p95 延迟</div><div class="value" style="font-size:20px">${c.p95}<span class="u">ms</span></div></div>
            <div><div class="sub">错误率</div><div class="value" style="font-size:20px">${c.err}<span class="u">%</span></div></div>
          </div>
          <div class="badge ${c.status === "ok" ? "ok" : c.status === "warn" ? "warn" : "bad"}" style="margin-top:8px">${label}</div>
        </div>`;
      }).join("");
    }
  };

  function nextPlanName(plans, current) {
    const i = plans.findIndex(p => p.name === current);
    return (i >= 0 && i < plans.length - 1) ? plans[i + 1].name : null;
  }
  function showToast(root, msg) {
    const t = $("#planToast", root) || $("#opsToast", root);
    if (!t) return;
    t.textContent = msg; t.hidden = false;
    t.classList.add("show");
    setTimeout(() => { t.classList.remove("show"); setTimeout(() => t.hidden = true, 250); }, 2200);
  }
  function changeRole(tenant, memberId, root) {
    const m = tenant.members.find(x => x.id === memberId);
    if (!m) return;
    const roles = ["管理员", "分析师", "操作员", "设备工程师"];
    const pick = prompt("修改「" + m.name + "」角色为：\n" + roles.join(" / "));
    if (pick && roles.indexOf(pick) >= 0) { m.role = pick; renderers["account/members"](DATA, root); }
  }
  function removeMember(tenant, memberId, root) {
    const m = tenant.members.find(x => x.id === memberId);
    if (!m) return;
    openConfirm({
      title: "移除成员",
      desc: "确认将「" + m.name + "」移出本组织？其访问权限将立即撤销。",
      target: `<span class="ts-dlg-row">${iconSvg("users", "ico ico-sm")} ${m.name}</span>`,
      onConfirm: () => { tenant.members = tenant.members.filter(x => x.id !== memberId); renderers["account/members"](DATA, root); }
    });
  }
  function openInvite() {
    const d = $("#inviteDrawer"), m = $("#inviteMask");
    if (!d) return;
    d.classList.add("open"); m.classList.add("open");
    const email = $("#invEmail"); if (email) { email.value = ""; setTimeout(() => email.focus(), 50); }
    trapFocus(d);
  }
  function closeInvite() {
    const d = $("#inviteDrawer"), m = $("#inviteMask");
    if (!d) return;
    d.classList.remove("open"); m.classList.remove("open");
  }
  function submitInvite(tenant, root) {
    const email = $("#invEmail"), name = $("#invName"), role = $("#invRole");
    if (!email || !email.value) return;
    tenant.members.push({
      id: "m" + Date.now(),
      name: (name && name.value) || email.value.split("@")[0],
      email: email.value,
      role: (role && role.value) || "分析师",
      status: "活跃",
      last: "刚刚"
    });
    closeInvite();
    renderers["account/members"](DATA, root);
  }

  /* ---------- 设备详情抽屉（全局） ---------- */
  const DW_TABS = ["信息", "心跳", "操作", "日志", "指令", "计划", "任务", "地图"];
  function openDrawer(d) {
    $("#dwName").textContent = d.name;
    $("#dwId").textContent = `${d.id} · ${d.model} · ${d.proto}`;
    const tabs = $("#dwTabs"); tabs.innerHTML = "";
    DW_TABS.forEach((t, i) => {
      const b = el("button", i === 0 ? "on" : "", t);
      b.onclick = () => { $$("#dwTabs button").forEach(x => x.classList.remove("on")); b.classList.add("on"); renderDwTab(d, t); };
      tabs.appendChild(b);
    });
    renderDwTab(d, "信息");
    $("#drawerMask").classList.add("open");
    $("#drawer").classList.add("open");
    $("#dwClose").focus();
  }
  function closeDrawer() { $("#drawerMask").classList.remove("open"); $("#drawer").classList.remove("open"); }
  function renderDwTab(d, t) {
    const body = $("#dwBody");
    const row = (k, v) => `<div class="stat-row"><span style="color:var(--text-3)">${k}</span><span>${v}</span></div>`;
    if (t === "信息") body.innerHTML = row("型号", d.model) + row("协议", d.proto) + row("状态", d.status) + row("位置", `${d.lat}, ${d.lon}`);
    else if (t === "心跳") body.innerHTML = row("最后心跳", d.last) + row("间隔", "5s") + row("延迟", "18ms") + row("探针", "HealthCheck OK");
    else if (t === "操作") body.innerHTML = `<div style="display:flex;gap:8px;flex-wrap:wrap"><button class="btn btn-primary">复位</button><button class="btn btn-ghost">启动</button><button class="btn btn-ghost">停止</button></div><div style="font-size:12px;color:var(--text-3);margin-top:8px">经 CQRS 写命令 → IProtocolAdapter.WriteAsync</div>`;
    else if (t === "日志") body.innerHTML = ["12:01 采集正常", "11:58 通信恢复", "11:40 温度 71℃ 预警"].map(l => `<div style="padding:8px;border-radius:10px;background:var(--brand-50)">${l}</div>`).join("");
    else if (t === "指令") body.innerHTML = `<div style="display:flex;gap:8px"><input placeholder="地址 如 40001" class="select" style="flex:1"><input placeholder="值" class="select" style="width:80px"><button class="btn btn-primary">下发</button></div><div style="font-size:12px;color:var(--text-3);margin-top:8px">经 Channel → MQTT → 设备</div>`;
    else if (t === "计划") body.innerHTML = ["每日 02:00 批量轮询", "每周日 固件巡检"].map(l => `<div class="pill">${l}</div>`).join("");
    else if (t === "任务") body.innerHTML = ["校正任务 #T204 进行中", "备份任务 #T198 完成"].map(l => `<div class="pill">${l}</div>`).join("");
    else body.innerHTML = `<div style="color:var(--text-3)">经纬度 ${d.lat}, ${d.lon}</div><button class="btn btn-primary" id="dwMapBtn" style="margin-top:10px">在地图中查看 →</button><div style="font-size:12px;margin-top:8px">完整地理分布见「地图」页。</div>`;
    const mb = $("#dwMapBtn"); if (mb) mb.onclick = () => { closeDrawer(); navigate("map", d.id); };
  }

  /* ---------- 悬停提示 ---------- */
  const tip = $("#tip");
  function moveTip(e) { tip.style.display = "block"; tip.style.left = e.clientX + "px"; tip.style.top = e.clientY + "px"; }

  /* ---------- 租户切换器 + 通用确认弹窗 ---------- */
  function openTsMenu() {
    const menu = $("#tsMenu"), btn = $("#tsTrigger");
    menu.hidden = false; menu.classList.add("open");
    btn.setAttribute("aria-expanded", "true");
    const inp = $("#tsSearch"); if (inp) setTimeout(() => inp.focus(), 30);
  }
  function closeTsMenu() {
    const menu = $("#tsMenu"), btn = $("#tsTrigger");
    menu.hidden = true; menu.classList.remove("open");
    btn.setAttribute("aria-expanded", "false");
  }
  function buildTenantSwitcher() {
    const list = $("#tsList");
    if (!list || !DATA || !DATA.saas) return;
    function render(filter) {
      const q = (filter || "").toLowerCase();
      const ts = DATA.saas.tenants.filter(t => !q || t.name.toLowerCase().includes(q) || t.slug.includes(q));
      list.innerHTML = ts.map(t => `
        <button class="ts-item ${t.id === tenantContext.id ? "sel" : ""}" role="option" aria-selected="${t.id === tenantContext.id}" data-id="${t.id}">
          ${iconSvg("building-2", "ico ico-sm")}
          <span class="ts-item-name">${t.name}</span>
          <span class="badge tier">${t.plan}</span>
          ${t.id === tenantContext.id ? iconSvg("check", "ico ico-sm ts-check") : ""}
        </button>`).join("");
      $("#tsEmpty").hidden = ts.length > 0;
      $$(".ts-item", list).forEach(b => b.onclick = () => {
        const id = b.dataset.id;
        const t = DATA.saas.tenants.find(x => x.id === id);
        if (id === tenantContext.id) { closeTsMenu(); return; }
        openConfirm({
          title: "切换组织",
          desc: "切换组织后将返回首页并刷新该组织数据，确定继续？",
          target: `<span class="ts-dlg-row">${iconSvg("building-2", "ico ico-sm")} ${t.name} · <span class="badge tier">${t.plan}</span></span>`,
          onConfirm: () => applyTenant(id)
        });
      });
    }
    render("");
    const search = $("#tsSearch"); if (search) search.oninput = e => render(e.target.value);
  }
  function applyTenant(id) {
    const t = DATA.saas.tenants.find(x => x.id === id); if (!t) return;
    tenantContext = { id: t.id, name: t.name };
    const nameEl = $("#tsName"); if (nameEl) nameEl.textContent = t.name;
    closeTsMenu();
    navigate("dashboard"); // 回首页并刷新该组织数据
  }
  function openConfirm(opts) {
    $("#tsDlgTitle").textContent = opts.title || "确认";
    $("#tsDlgDesc").textContent = opts.desc || "";
    $("#tsDlgTarget").innerHTML = opts.target || "";
    confirmCallback = opts.onConfirm || null;
    $("#tsMask").classList.add("open");
    $("#tsDialog").hidden = false;
    $("#tsDialog").classList.add("open");
    const cancel = $("#tsDlgCancel"); if (cancel) cancel.focus();
    trapFocus($("#tsDialog"));
  }
  function closeConfirm() {
    $("#tsMask").classList.remove("open");
    $("#tsDialog").hidden = true;
    $("#tsDialog").classList.remove("open");
    confirmCallback = null;
  }
  function trapFocus(container) {
    container.addEventListener("keydown", function onTab(e) {
      if (e.key !== "Tab") return;
      const f = $$('a[href],button:not([disabled]),input:not([disabled]),select,textarea,[tabindex]:not([tabindex="-1"])', container).filter(el => el.offsetParent !== null);
      if (!f.length) return;
      const first = f[0], last = f[f.length - 1];
      if (e.shiftKey && document.activeElement === first) { e.preventDefault(); last.focus(); }
      else if (!e.shiftKey && document.activeElement === last) { e.preventDefault(); first.focus(); }
    });
  }

  /* ---------- 导航 / 深链 / 上下文（修复 B1/B8） ---------- */
  function parseHash() {
    const h = location.hash.replace(/^#/, "");
    if (!h) return { route: "dashboard", deviceId: null };
    const parts = h.split("/");
    if (parts[0] === "account" || parts[0] === "ops") {
      return { route: parts.slice(0, 2).join("/"), deviceId: null };
    }
    const page = parts[0] || "dashboard";
    const deviceId = parts[1] ? decodeURIComponent(parts[1]) : null;
    return { route: page, deviceId };
  }
  function navigate(route, deviceId) {
    let file;
    if (isSaas(route)) {
      const [grp, sub] = route.split("/");
      file = `pages/${grp}/${sub}.html`;
    } else {
      const base = route.split("/")[0];
      if (!NAV.find(n => n.id === base)) route = "dashboard";
      file = `pages/${base}.html`;
    }
    const box = $("#searchResults"); if (box) box.classList.remove("show");
    ctx.deviceId = deviceId || null;
    const h = deviceId ? `#${route}/${encodeURIComponent(deviceId)}` : `#${route}`;
    if (location.hash !== h) history.replaceState(null, "", h);
    htmx.ajax("GET", file, { target: "#view", swap: "innerHTML" });
  }
  window.addEventListener("hashchange", () => {
    const { route, deviceId } = parseHash();
    if (route !== currentPage || (deviceId && deviceId !== ctx.deviceId) || (!deviceId && ctx.deviceId))
      navigate(route, deviceId);
  });

  function setActiveNav(route) {
    $$("#sideNav a, #topNav a").forEach(a => {
      const on = a.dataset.page === route;
      a.classList.toggle("active", on);
      if (on) a.setAttribute("aria-current", "page");
      else a.removeAttribute("aria-current");
    });
    updateAlarmBadge();
  }
  function updateAlarmBadge() {
    if (!DATA) return;
    const cnt = DATA.alarms.filter(a => a.st !== "已解决").length;
    $$('[data-badge="alarms"]').forEach(b => { b.textContent = cnt; b.style.display = cnt ? "" : "none"; });
  }
  function buildNav() {
    const sideNav = $("#sideNav"), topNav = $("#topNav");
    sideNav.innerHTML = ""; topNav.innerHTML = "";
    GROUPS.forEach(g => {
      sideNav.appendChild(el("div", "nav-group-label", g.title));
      g.items.forEach(n => {
        const a = el("a", "", `${iconSvg(n.icon)}<span>${n.label}</span>` + (n.badge ? `<span class="badge-count" data-badge="alarms">${n.badge}</span>` : ""));
        a.href = `#${n.id}`; a.dataset.page = n.id;
        a.addEventListener("click", e => { e.preventDefault(); closeSidebar(); navigate(n.id); });
        sideNav.appendChild(a);
        const b = a.cloneNode(true); b.className = "pill"; topNav.appendChild(b);
      });
    });
  }

  /* ---------- 搜索结果下拉（修复 B9，防跳 + 空态） ---------- */
  function initSearch() {
    const input = $("#globalSearch"), box = $("#searchResults");
    function renderResults() {
      const q = searchQ;
      if (!q) { box.innerHTML = ""; box.classList.remove("show"); return; }
      const hits = (DATA ? DATA.devices : []).filter(d => (d.id + " " + d.name + " " + d.model).toLowerCase().includes(q)).slice(0, 6);
      if (!hits.length) { box.innerHTML = `<div class="sr-empty">未找到匹配「${q}」的设备</div>`; box.classList.add("show"); return; }
      box.innerHTML = hits.map(d => `<div class="sr-item" data-dev="${d.id}"><span><b>${d.name}</b><br><small style="color:var(--text-3)">${d.id} · ${d.model}</small></span><span class="badge ${d.status === "在线" ? "ok" : d.status === "预警" ? "warn" : d.status === "告警" ? "bad" : "muted"}">${d.status}</span></div>`).join("")
        + `<div class="sr-foot" data-all="1">在设备中查看全部结果 →</div>`;
      box.classList.add("show");
      $$(".sr-item", box).forEach(it => it.onclick = () => { openDevice(it.dataset.dev); box.classList.remove("show"); input.blur(); });
      const foot = $(".sr-foot", box); if (foot) foot.onclick = () => { box.classList.remove("show"); navigate("devices"); };
    }
    input.addEventListener("input", e => { searchQ = e.target.value.trim().toLowerCase(); renderResults(); });
    input.addEventListener("focus", renderResults);
    document.addEventListener("click", e => { if (!e.target.closest(".search")) box.classList.remove("show"); });
  }

  /* ---------- 外壳初始化 ---------- */
  function initShell() {
    buildNav();
    const saved = localStorage.getItem("theme");
    if (saved === "light") document.documentElement.classList.remove("dark");
    $("#themeBtn").onclick = () => {
      const dark = document.documentElement.classList.toggle("dark");
      localStorage.setItem("theme", dark ? "dark" : "light");
    };
    $("#bellBtn").onclick = () => $("#notif").classList.toggle("show");
    $("#notifClose").onclick = () => $("#notif").classList.remove("show");
    $$("#notif .notif-item").forEach(li => li.onclick = () => {
      const dev = li.dataset.dev;
      $("#notif").classList.remove("show");
      if (dev && openDevice(dev)) return;
      ctx.alarmFilter = "active"; navigate("alarms");
    });
    $("#dwClose").onclick = closeDrawer;
    $("#drawerMask").onclick = closeDrawer;
    $("#hamburger").onclick = () => { $("#sidebar").classList.add("open"); $("#sideOverlay").classList.add("show"); };
    $("#sideOverlay").onclick = closeSidebar;
    // 租户切换器
    const tsTrigger = $("#tsTrigger"); if (tsTrigger) tsTrigger.onclick = (e) => { e.stopPropagation(); const menu = $("#tsMenu"); if (menu.hidden) openTsMenu(); else closeTsMenu(); };
    document.addEventListener("click", e => { const menu = $("#tsMenu"); if (menu && !menu.hidden && !e.target.closest(".tenant-switcher")) closeTsMenu(); });
    // 通用确认弹窗
    const cConfirm = $("#tsDlgConfirm"); if (cConfirm) cConfirm.onclick = () => { const cb = confirmCallback; closeConfirm(); if (cb) cb(); };
    const cCancel = $("#tsDlgCancel"); if (cCancel) cCancel.onclick = closeConfirm;
    const cClose = $("#tsDlgClose"); if (cClose) cClose.onclick = closeConfirm;
    const cMask = $("#tsMask"); if (cMask) cMask.onclick = closeConfirm;
    initSearch();
    document.addEventListener("keydown", e => {
      if (e.key !== "Escape") return;
      if ($("#tsDialog") && !$("#tsDialog").hidden) { closeConfirm(); return; }
      if ($("#inviteDrawer") && $("#inviteDrawer").classList.contains("open")) { closeInvite(); return; }
      if ($("#drawer").classList.contains("open")) { closeDrawer(); return; }
      else if ($("#notif").classList.contains("show")) $("#notif").classList.remove("show");
      else closeSidebar();
    });
  }
  function closeSidebar() { $("#sidebar").classList.remove("open"); $("#sideOverlay").classList.remove("show"); }

  /* ---------- 片段切换后渲染 ---------- */
  function onSwap(viewEl) {
    const root = viewEl.querySelector("[data-page]");
    if (!root) return;
    const page = root.dataset.page;
    currentPage = page;
    setActiveNav(page);
    clearTimers();
    root.classList.add("view-anim");
    $$(".seg", root).forEach(seg => seg.addEventListener("click", e => {
      if (e.target.tagName === "BUTTON") { $$("button", seg).forEach(b => b.classList.remove("on")); e.target.classList.add("on"); }
    }));
    if (renderers[page]) renderers[page](DATA, root);
    if (page === "devices" && ctx.deviceId) {
      const d = DATA.devices.find(x => x.id === ctx.deviceId);
      if (d) { searchQ = d.id.toLowerCase(); const si = $("#globalSearch"); if (si) si.value = d.id; renderers.devices(DATA, root); openDevice(d.id); }
    }
    if (page === "map" && ctx.deviceId) {
      openDevice(ctx.deviceId);
    }
  }

  /* ---------- 启动 ---------- */
  async function boot() {
    initShell();
    document.body.addEventListener("htmx:afterSwap", e => {
      const t = (e.detail && e.detail.target) || e.target;
      if (t && t.id === "view") onSwap(t);
    });
    try {
      await loadData();
    } catch (err) {
      console.error("数据加载失败", err);
    }
    if (DATA && DATA.saas) {
      tenantContext = {
        id: DATA.saas.currentTenantId,
        name: (DATA.saas.tenants.find(t => t.id === DATA.saas.currentTenantId) || {}).name || "—"
      };
      const nameEl = $("#tsName"); if (nameEl) nameEl.textContent = tenantContext.name;
      buildTenantSwitcher();
    }
    const { route, deviceId } = parseHash();
    navigate(route, deviceId);
  }

  if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", boot);
  else boot();
})();

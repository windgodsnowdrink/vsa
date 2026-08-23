// GlobalUsings.cs — File-based App 共享全局 using
// 与 Microsoft.NET.Sdk.Web 隐式 using 对齐，并补齐 SaaS 依赖命名空间。
// 这样各垂直切片（全局命名空间）与 entities.cs（PlcAiot.Saas.Domain）无需逐文件重复 using。
// 由 Program.cs 经 #include 引入；verify 工程直接编译（随顶部 *.cs 一并剥离后编译）。
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Net;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using Mediator;
global using PlcAiot.Saas.Domain;

// Mediator 源生成器配置：handler 生命周期 = Scoped（与 BaseDbContext / ICurrentTenant 对齐）。
// 必须用 assembly attribute，因为源生成器在编译期解析配置；lambda 形式在某些工具链下不被识别。
// 若 handler 仍以 Singleton 注册会触发 captive dependency 校验失败：
//   "Cannot consume scoped service 'BaseDbContext' from singleton 'XxxHandler'"
// 改了此 attribute 后必须清 obj/bin 重新构建，源生成器才会重新生成 AddMediator 注册代码。
[assembly: MediatorOptions(ServiceLifetime = ServiceLifetime.Scoped)]

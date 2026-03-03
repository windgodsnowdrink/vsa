# Routing 技能使用示例

## 1. 路由管理示例

### 1.1 添加路由

**功能说明**：添加新的路由配置

**命令示例**：

```bash
# 添加 GET 路由
routing_core.exe add --path /api/users --method GET --handler UsersController.GetUsers

# 添加 POST 路由
routing_core.exe add --path /api/users --method POST --handler UsersController.CreateUser

# 添加带参数的路由
routing_core.exe add --path /api/users/{id} --method GET --handler UsersController.GetUserById

# 添加 PUT 路由
routing_core.exe add --path /api/users/{id} --method PUT --handler UsersController.UpdateUser

# 添加 DELETE 路由
routing_core.exe add --path /api/users/{id} --method DELETE --handler UsersController.DeleteUser
```

**预期输出**：

```
成功添加路由: GET /api/users -> UsersController.GetUsers
成功添加路由: POST /api/users -> UsersController.CreateUser
成功添加路由: GET /api/users/{id} -> UsersController.GetUserById
成功添加路由: PUT /api/users/{id} -> UsersController.UpdateUser
成功添加路由: DELETE /api/users/{id} -> UsersController.DeleteUser
```

### 1.2 列出路由

**功能说明**：列出所有已配置的路由

**命令示例**：

```bash
# 以 JSON 格式列出路由
routing_core.exe list --format json

# 以文本格式列出路由
routing_core.exe list --format text

# 以 YAML 格式列出路由
routing_core.exe list --format yaml
```

**预期输出**（JSON 格式）：

```json
[
  {
    "Path": "/api/users",
    "Method": "GET",
    "Handler": "UsersController.GetUsers",
    "Parameters": [],
    "Middlewares": []
  },
  {
    "Path": "/api/users",
    "Method": "POST",
    "Handler": "UsersController.CreateUser",
    "Parameters": [],
    "Middlewares": []
  },
  {
    "Path": "/api/users/{id}",
    "Method": "GET",
    "Handler": "UsersController.GetUserById",
    "Parameters": [
      {
        "Name": "id",
        "Type": "string",
        "Source": "path",
        "Required": true,
        "DefaultValue": null
      }
    ],
    "Middlewares": []
  }
]
```

**预期输出**（文本格式）：

```
GET     /api/users -> UsersController.GetUsers
POST    /api/users -> UsersController.CreateUser
GET     /api/users/{id} -> UsersController.GetUserById
PUT     /api/users/{id} -> UsersController.UpdateUser
DELETE  /api/users/{id} -> UsersController.DeleteUser
```

### 1.3 删除路由

**功能说明**：删除指定的路由

**命令示例**：

```bash
# 删除 GET 路由
routing_core.exe remove --path /api/users --method GET

# 删除带参数的路由
routing_core.exe remove --path /api/users/{id} --method DELETE
```

**预期输出**：

```
成功删除路由: GET /api/users
成功删除路由: DELETE /api/users/{id}
```

## 2. 中间件管理示例

### 2.1 添加全局中间件

**功能说明**：添加全局中间件，应用于所有路由

**命令示例**：

```bash
# 添加认证中间件
routing_core.exe add-middleware --name AuthenticationMiddleware --handler AuthenticationMiddleware.Invoke

# 添加日志中间件
routing_core.exe add-middleware --name LoggingMiddleware --handler LoggingMiddleware.Invoke

# 添加异常处理中间件
routing_core.exe add-middleware --name ExceptionMiddleware --handler ExceptionMiddleware.Invoke
```

**预期输出**：

```
成功添加全局中间件: AuthenticationMiddleware -> AuthenticationMiddleware.Invoke
成功添加全局中间件: LoggingMiddleware -> LoggingMiddleware.Invoke
成功添加全局中间件: ExceptionMiddleware -> ExceptionMiddleware.Invoke
```

### 2.2 添加路由级中间件

**功能说明**：为指定路由添加中间件

**命令示例**：

```bash
# 为用户路由添加授权中间件
routing_core.exe add-route-middleware --path /api/users --method GET --name AuthorizationMiddleware --handler AuthorizationMiddleware.Invoke

# 为用户详情路由添加验证中间件
routing_core.exe add-route-middleware --path /api/users/{id} --method GET --name ValidationMiddleware --handler ValidationMiddleware.Invoke
```

**预期输出**：

```
成功添加路由中间件: AuthorizationMiddleware -> AuthorizationMiddleware.Invoke 到 GET /api/users
成功添加路由中间件: ValidationMiddleware -> ValidationMiddleware.Invoke 到 GET /api/users/{id}
```

## 3. 路由代码生成示例

### 3.1 生成 ASP.NET Core 路由代码

**功能说明**：生成 ASP.NET Core 风格的路由代码

**配置文件示例**（routes.json）：

```json
{
  "Routes": [
    {
      "Path": "/api/users",
      "Method": "GET",
      "Handler": "UsersController.GetUsers",
      "Parameters": [],
      "Middlewares": []
    },
    {
      "Path": "/api/users/{id}",
      "Method": "GET",
      "Handler": "UsersController.GetUserById",
      "Parameters": [
        {
          "Name": "id",
          "Type": "int",
          "Source": "path",
          "Required": true
        }
      ],
      "Middlewares": []
    },
    {
      "Path": "/api/users",
      "Method": "POST",
      "Handler": "UsersController.CreateUser",
      "Parameters": [
        {
          "Name": "user",
          "Type": "UserDto",
          "Source": "body",
          "Required": true
        }
      ],
      "Middlewares": []
    }
  ],
  "Middlewares": [
    {
      "Name": "AuthenticationMiddleware",
      "Handler": "AuthenticationMiddleware",
      "Order": 0
    },
    {
      "Name": "LoggingMiddleware",
      "Handler": "LoggingMiddleware",
      "Order": 1
    }
  ]
}
```

**命令示例**：

```bash
routing_generator.exe generate --framework aspnetcore --input routes.json --output AspNetCoreRoutes.cs --format csharp
```

**生成的代码示例**：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Routing.Generated
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            // 实现路由处理逻辑
            // 处理程序: UsersController.GetUsers
            return Ok();
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            // 实现路由处理逻辑
            // 处理程序: UsersController.GetUserById
            return Ok();
        }

        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            // 实现路由处理逻辑
            // 处理程序: UsersController.CreateUser
            return Ok();
        }
    }
}
```

### 3.2 生成 FastEndpoints 路由代码

**功能说明**：生成 FastEndpoints 风格的路由代码

**命令示例**：

```bash
routing_generator.exe generate --framework fastendpoints --input routes.json --output FastEndpointsRoutes.cs --format csharp
```

**生成的代码示例**：

```csharp
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Routing.Generated
{
    public class GetUsersRequest
    {
    }

    public class GetUsersEndpoint : Endpoint<GetUsersRequest, IResult>
    {
        public override void Configure()
        {
            Verbs(new[] { "GET" });
            Routes("/api/users");
        }

        public override async Task HandleAsync(GetUsersRequest req, CancellationToken ct)
        {
            // 实现路由处理逻辑
            // 处理程序: UsersController.GetUsers
            await SendAsync(Results.Ok());
        }
    }

    public class GetUserByIdRequest
    {
    }

    public class GetUserByIdEndpoint : Endpoint<GetUserByIdRequest, IResult>
    {
        public override void Configure()
        {
            Verbs(new[] { "GET" });
            Routes("/api/users/{id}");
        }

        public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
        {
            // 实现路由处理逻辑
            // 处理程序: UsersController.GetUserById
            await SendAsync(Results.Ok());
        }
    }

    public class CreateUserRequest
    {
        public UserDto user { get; set; }
    }

    public class CreateUserEndpoint : Endpoint<CreateUserRequest, IResult>
    {
        public override void Configure()
        {
            Verbs(new[] { "POST" });
            Routes("/api/users");
        }

        public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
        {
            // 实现路由处理逻辑
            // 处理程序: UsersController.CreateUser
            await SendAsync(Results.Ok());
        }
    }
}
```

### 3.3 生成 Minimal API 路由代码

**功能说明**：生成 Minimal API 风格的路由代码

**命令示例**：

```bash
routing_generator.exe generate --framework minimalapi --input routes.json --output MinimalApiRoutes.cs --format csharp
```

**生成的代码示例**：

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Routing.Generated
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapGeneratedEndpoints(this IEndpointRouteBuilder app)
        {
            // 注册全局中间件: AuthenticationMiddleware
            app.UseMiddleware<AuthenticationMiddleware>();

            // 注册全局中间件: LoggingMiddleware
            app.UseMiddleware<LoggingMiddleware>();

            // GET /api/users
            app.MapGet("/api/users", async (HttpContext context) =>
            {
                // 实现路由处理逻辑
                // 处理程序: UsersController.GetUsers
                return Results.Ok();
            });

            // GET /api/users/{id}
            app.MapGet("/api/users/{id}", async (HttpContext context, int id) =>
            {
                // 实现路由处理逻辑
                // 处理程序: UsersController.GetUserById
                return Results.Ok();
            });

            // POST /api/users
            app.MapPost("/api/users", async (HttpContext context, UserDto user) =>
            {
                // 实现路由处理逻辑
                // 处理程序: UsersController.CreateUser
                return Results.Ok();
            });

            return app;
        }
    }
}
```

## 4. 配置验证示例

### 4.1 验证路由配置

**功能说明**：验证路由配置文件的正确性

**命令示例**：

```bash
routing_generator.exe validate --input routes.json
```

**预期输出**（配置正确）：

```
配置验证成功!
找到 5 个路由
找到 3 个中间件
```

**预期输出**（配置错误）：

```
配置验证失败:
- 路由缺少路径
- 路由缺少方法: /api/users
- 路由缺少处理程序: POST /api/users
```

## 5. 模板管理示例

### 5.1 列出可用模板

**功能说明**：列出所有可用的代码模板

**命令示例**：

```bash
routing_generator.exe template list
```

**预期输出**：

```
可用的代码模板:
--------------------------------------------------------------------------------
模板: aspnetcore-controller
描述: ASP.NET Core Controller 模板
框架: aspnetcore
格式: csharp
--------------------------------------------------------------------------------
模板: fastendpoints-endpoint
描述: FastEndpoints Endpoint 模板
框架: fastendpoints
格式: csharp
--------------------------------------------------------------------------------
模板: minimalapi-endpoint
描述: Minimal API 端点模板
框架: minimalapi
格式: csharp
--------------------------------------------------------------------------------
```

## 6. 完整应用示例

### 6.1 创建一个完整的 API 路由配置

**步骤 1**：创建路由配置文件

```json
{
  "Routes": [
    {
      "Path": "/api/products",
      "Method": "GET",
      "Handler": "ProductsController.GetProducts",
      "Parameters": [],
      "Middlewares": []
    },
    {
      "Path": "/api/products/{id}",
      "Method": "GET",
      "Handler": "ProductsController.GetProductById",
      "Parameters": [
        {
          "Name": "id",
          "Type": "int",
          "Source": "path",
          "Required": true
        }
      ],
      "Middlewares": []
    },
    {
      "Path": "/api/products",
      "Method": "POST",
      "Handler": "ProductsController.CreateProduct",
      "Parameters": [
        {
          "Name": "product",
          "Type": "ProductDto",
          "Source": "body",
          "Required": true
        }
      ],
      "Middlewares": [
        {
          "Name": "ValidationMiddleware",
          "Handler": "ValidationMiddleware.Invoke",
          "Order": 0
        }
      ]
    },
    {
      "Path": "/api/products/{id}",
      "Method": "PUT",
      "Handler": "ProductsController.UpdateProduct",
      "Parameters": [
        {
          "Name": "id",
          "Type": "int",
          "Source": "path",
          "Required": true
        },
        {
          "Name": "product",
          "Type": "ProductDto",
          "Source": "body",
          "Required": true
        }
      ],
      "Middlewares": [
        {
          "Name": "ValidationMiddleware",
          "Handler": "ValidationMiddleware.Invoke",
          "Order": 0
        }
      ]
    },
    {
      "Path": "/api/products/{id}",
      "Method": "DELETE",
      "Handler": "ProductsController.DeleteProduct",
      "Parameters": [
        {
          "Name": "id",
          "Type": "int",
          "Source": "path",
          "Required": true
        }
      ],
      "Middlewares": []
    }
  ],
  "Middlewares": [
    {
      "Name": "AuthenticationMiddleware",
      "Handler": "AuthenticationMiddleware.Invoke",
      "Order": 0
    },
    {
      "Name": "LoggingMiddleware",
      "Handler": "LoggingMiddleware.Invoke",
      "Order": 1
    },
    {
      "Name": "ExceptionMiddleware",
      "Handler": "ExceptionMiddleware.Invoke",
      "Order": 2
    }
  ]
}
```

**步骤 2**：验证配置文件

```bash
routing_generator.exe validate --input products-routes.json
```

**步骤 3**：生成 ASP.NET Core 代码

```bash
routing_generator.exe generate --framework aspnetcore --input products-routes.json --output ProductsController.cs --format csharp
```

**步骤 4**：生成 FastEndpoints 代码

```bash
routing_generator.exe generate --framework fastendpoints --input products-routes.json --output ProductsEndpoints.cs --format csharp
```

**步骤 5**：生成 Minimal API 代码

```bash
routing_generator.exe generate --framework minimalapi --input products-routes.json --output ProductsMinimalApi.cs --format csharp
```

### 6.2 集成到 ASP.NET Core 应用

**步骤 1**：创建 ASP.NET Core 项目

```bash
dotnet new webapi -n ProductApi
cd ProductApi
```

**步骤 2**：添加生成的控制器文件

将生成的 `ProductsController.cs` 文件复制到 `Controllers` 目录。

**步骤 3**：修改 Program.cs 文件

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 使用控制器
app.MapControllers();

app.Run();
```

**步骤 4**：运行应用

```bash
dotnet run
```

**步骤 5**：测试 API

打开浏览器访问 `https://localhost:5001/swagger`，查看生成的 API 文档并测试各个端点。

### 6.3 集成到 Minimal API 应用

**步骤 1**：创建 Minimal API 项目

```bash
dotnet new web -n ProductMinimalApi
cd ProductMinimalApi
```

**步骤 2**：添加生成的端点扩展文件

将生成的 `ProductsMinimalApi.cs` 文件复制到项目根目录。

**步骤 3**：修改 Program.cs 文件

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Routing.Generated;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 映射生成的端点
app.MapGeneratedEndpoints();

app.Run();
```

**步骤 4**：运行应用

```bash
dotnet run
```

**步骤 5**：测试 API

打开浏览器访问 `https://localhost:5001/swagger`，查看生成的 API 文档并测试各个端点。

## 7. 高级使用示例

### 7.1 版本化 API 路由

**功能说明**：创建版本化的 API 路由

**命令示例**：

```bash
# 添加 v1 版本路由
routing_core.exe add --path /api/v1/users --method GET --handler v1.UsersController.GetUsers

# 添加 v2 版本路由
routing_core.exe add --path /api/v2/users --method GET --handler v2.UsersController.GetUsers

# 添加 v2 版本的新端点
routing_core.exe add --path /api/v2/users/me --method GET --handler v2.UsersController.GetCurrentUser
```

**生成的配置示例**：

```json
[
  {
    "Path": "/api/v1/users",
    "Method": "GET",
    "Handler": "v1.UsersController.GetUsers",
    "Parameters": [],
    "Middlewares": []
  },
  {
    "Path": "/api/v2/users",
    "Method": "GET",
    "Handler": "v2.UsersController.GetUsers",
    "Parameters": [],
    "Middlewares": []
  },
  {
    "Path": "/api/v2/users/me",
    "Method": "GET",
    "Handler": "v2.UsersController.GetCurrentUser",
    "Parameters": [],
    "Middlewares": []
  }
]
```

### 7.2 复杂路由参数

**功能说明**：创建带复杂参数的路由

**命令示例**：

```bash
# 添加带多个参数的路由
routing_core.exe add --path /api/products/{category}/{id} --method GET --handler ProductsController.GetProductByCategoryAndId

# 添加带查询参数的路由
routing_core.exe add --path /api/products/search --method GET --handler ProductsController.SearchProducts

# 添加带矩阵参数的路由
routing_core.exe add --path /api/products/filter --method GET --handler ProductsController.FilterProducts
```

**生成的配置示例**：

```json
[
  {
    "Path": "/api/products/{category}/{id}",
    "Method": "GET",
    "Handler": "ProductsController.GetProductByCategoryAndId",
    "Parameters": [
      {
        "Name": "category",
        "Type": "string",
        "Source": "path",
        "Required": true
      },
      {
        "Name": "id",
        "Type": "int",
        "Source": "path",
        "Required": true
      }
    ],
    "Middlewares": []
  },
  {
    "Path": "/api/products/search",
    "Method": "GET",
    "Handler": "ProductsController.SearchProducts",
    "Parameters": [],
    "Middlewares": []
  },
  {
    "Path": "/api/products/filter",
    "Method": "GET",
    "Handler": "ProductsController.FilterProducts",
    "Parameters": [],
    "Middlewares": []
  }
]
```

### 7.3 自定义中间件链

**功能说明**：为不同路由创建不同的中间件链

**命令示例**：

```bash
# 添加公共路由（无需认证）
routing_core.exe add --path /api/public --method GET --handler PublicController.GetPublicData

# 添加需要认证的路由
routing_core.exe add --path /api/protected --method GET --handler ProtectedController.GetProtectedData
routing_core.exe add-route-middleware --path /api/protected --method GET --name AuthenticationMiddleware --handler AuthenticationMiddleware.Invoke

# 添加需要认证和授权的路由
routing_core.exe add --path /api/admin --method GET --handler AdminController.GetAdminData
routing_core.exe add-route-middleware --path /api/admin --method GET --name AuthenticationMiddleware --handler AuthenticationMiddleware.Invoke
routing_core.exe add-route-middleware --path /api/admin --method GET --name AuthorizationMiddleware --handler AuthorizationMiddleware.Invoke
```

**生成的配置示例**：

```json
[
  {
    "Path": "/api/public",
    "Method": "GET",
    "Handler": "PublicController.GetPublicData",
    "Parameters": [],
    "Middlewares": []
  },
  {
    "Path": "/api/protected",
    "Method": "GET",
    "Handler": "ProtectedController.GetProtectedData",
    "Parameters": [],
    "Middlewares": [
      {
        "Name": "AuthenticationMiddleware",
        "Handler": "AuthenticationMiddleware.Invoke",
        "Order": 0
      }
    ]
  },
  {
    "Path": "/api/admin",
    "Method": "GET",
    "Handler": "AdminController.GetAdminData",
    "Parameters": [],
    "Middlewares": [
      {
        "Name": "AuthenticationMiddleware",
        "Handler": "AuthenticationMiddleware.Invoke",
        "Order": 0
      },
      {
        "Name": "AuthorizationMiddleware",
        "Handler": "AuthorizationMiddleware.Invoke",
        "Order": 1
      }
    ]
  }
]
```

## 8. 总结

本示例文档提供了 Routing 技能的完整使用示例，包括：

- **路由管理**：添加、列出、删除路由
- **中间件管理**：添加全局中间件和路由级中间件
- **代码生成**：为 ASP.NET Core、FastEndpoints 和 Minimal API 生成代码
- **配置验证**：验证路由配置文件的正确性
- **模板管理**：列出可用的代码模板
- **完整应用集成**：将生成的代码集成到实际应用中
- **高级使用场景**：版本化 API、复杂路由参数、自定义中间件链

通过这些示例，开发者可以快速掌握 Routing 技能的使用方法，为实际项目中的路由管理和代码生成提供有力支持。
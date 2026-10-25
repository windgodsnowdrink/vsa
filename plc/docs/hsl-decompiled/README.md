# HslCommunication 13.0.2.0 反编译源码（附官方文档注释）

本目录是对 `plc/src/plc-gateway/lib/HslCommunication.dll` 的反编译产物，**不是原始源码**，
仅用于阅读、排障、协议核对与依赖行为确认。

## 来源与工具

| 项目 | 值 |
|---|---|
| 程序集 | `HslCommunication.dll` |
| 版本 | 13.0.2.0 |
| SHA-256 | `F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30` |
| 反编译器 | ilspycmd 11.0.0.9375（ICSharpCode.Decompiler） |
| 目标框架（推导） | netstandard2.1 |
| 注释来源 | 官方 XML 文档 `HslCommunication.xml` |
| 生成日期 | 2026-10-18 |
| 仓库版本 | vsa **dev** 分支 @ `d0d7c65` |

## 目录结构

```
hsl-decompiled/
├── HslCommunication.csproj          # ilspycmd 生成的工程（引用未配置，不可直接构建）
├── HslCommunication.docs.xml        # 修复后的官方 XML 文档副本（见「已知缺陷」）
├── README.md
└── <命名空间>/<类型>.cs              # 每个类型一个文件，目录名即完整命名空间
```

658 个 C# 文件，96 个命名空间（Siemens / Omron / Melsec / Modbus / MQTT / Redis / Robot / Secs …）。

## 注释约定

每个文件包含：

1. **文件头**：来源程序集、版本、SHA-256、工具、日期、免责说明；
2. **类型级** `/// <summary>` / `<remarks>`：取自官方文档 `T:` 条目；
3. **成员级** `/// <summary>` / `<param>` / `<returns>` / `<remarks>`：取自 `M:` / `P:` / `F:` / `E:` 条目。

注释措辞**原样保留**（官方文档为中英双语，故常见中英对照），只做机械搬运，未改写。

| 指标 | 数量 |
|---|---|
| 类型已注释 | 625（未注释 32，多为 internal / 编译器生成类型） |
| 成员已注释 | 7643 |
| 含注释文件 | 638 / 658 |
| `///` 总行数 | 约 4.9 万 |

公开 API 抽样覆盖率：`ModbusTcpNet` 95%、`SiemensS7Net` 89%、`MqttClient` 83%。
匹配遵循「宁缺勿错」：同名同参数个数的重载若无法用参数类型唯一区分就跳过，不贴错文档。

## 变量重命名

反编译产物没有 PDB，方法内局部变量名全部丢失（ILSpy 只能给出 `num` / `num2` / `text` / `array`）。
`scripts/tools/LocalRenamer`（Roslyn 语法树，非正则）只对**局部变量声明**做语义化重命名：

| 原形态 | 判定依据 | 新名 |
|---|---|---|
| `byte[] array = new byte[...]` | 数组分配 | `buffer`（冲突时 `buffer2`…） |
| `byte[] array = X.GetBytes(...)` | 编码转换 | `bytes` |
| `int num = sock.Receive(...)` / `.Read(...)` | 读到的字节数 | `bytesRead` |
| `int num = X.Length` / `X.Count` | 长度 / 计数 | `count` |
| `for (int num = 0; ...)` | 循环计数器 | `i` / `j` / `k` |
| `bool flag = false` | 成功标志 | `success` |
| `string text = X.GetString(...)` | 二进制解码 | `message` |

- 只改局部变量：**参数名、字段名、成员名一律不动**（来自元数据，本就是作者原名）。
- 同一方法内自动避让重名（`buffer` → `buffer2`），并校验无同块重复声明。
- 结果：**569 个变量重命名，4349 处标识符改动**。
  其余 `num` / `array` 等无法从上下文唯一确定语义（既可能是偏移量也可能是总量），**保留原样**，不做猜测。

示例（`HslCommunication.Core.Net.NetSupport`）：

```csharp
// 重命名前
int num2 = socket.Receive(buffer, num + offset, size, SocketFlags.None);
num += num2;
if (num2 == 0) { throw new RemoteCloseException(); }

// 重命名后
int bytesRead = socket.Receive(buffer, num + offset, size, SocketFlags.None);
num += bytesRead;
if (bytesRead == 0) { throw new RemoteCloseException(); }
```

## 质量校验

- **语法**：全部文件用 Roslyn 重新解析，**0 个语法错误文件、0 处同块重复声明**
  （基线：未做任何改动的原始反编译产物同样 0 错误）。
- **编译器生成类型**：`<>c__DisplayClass`、`<>c`、`b__N` 实际出现次数均为 **0** ——
  ILSpy 已把闭包还原成 lambda / 局部函数，无需额外处理。
- 校验命令：

```powershell
.\scripts\tools\LocalRenamer\bin\Release\net11.0\LocalRenamer.exe .\hsl-decompiled --check
```

## 已知缺陷（源文件本身）

官方 `HslCommunication.xml` 有多处打包损坏，严格 XML 解析器无法读取：

- 第 15538 行：`</me])"/>`（应为 `</member>`）；
- 第 24116 行：属性语法错误（`'client' is an unexpected token`）。

因此注释注入脚本改用宽松正则提取，并把修复后的副本存为 `HslCommunication.docs.xml`。
**原始 XML 未被修改。**

## 构建

```powershell
cd .\hsl-decompiled
dotnet build          # 0 error / 29 warning（均为 SYSLIB 过时 API 与 CA 命名规则）
```

产物：`bin/Debug/net11.0/HslCommunication.dll`（约 4.85 MB）。

### 为通过构建所做的修正

| 问题 | 表现 | 处理 |
|---|---|---|
| TFM 写错 | `<TargetFramework>net11</TargetFramework>` 被解析成 **.NET Framework 1.1**，报 `MSB3644` | 改为 `net11.0`（`net11` 是旧式 TFM 写法，等同 v1.1） |
| 裸引用无路径 | `<Reference Include="Newtonsoft.Json" />` 解析不到程序集 | 改为 `PackageReference`：`Newtonsoft.Json 13.0.3`、`System.IO.Ports 10.0.0` |
| `base._002Ector();` | ILSpy 无法表达基类构造调用时输出的转义标识符（4 处） | 删除该行——C# 会隐式调用基类无参构造，语义等价 |
| `ref` 局部变量逃逸 | `ConnectPool.GetAvailableConnector` 中 `reference = ref val2` 触发 `CS8374`（C# 11+ ref 安全规则），降 LangVersion 无效 | 按语义等价改写为 `val.LastUseTime = DateTime.Now;` |
| 运算符显式调用 | `JToken.op_Implicit(item.Value)` 触发 `CS0571` | 改为显式转换 `(JToken)item.Value` |
| `Extensions` 二义性 | `Newtonsoft.Json.Linq.Extensions` 与 `System.Xml.Linq.Extensions` 冲突（`CS0104`） | 全限定 `Newtonsoft.Json.Linq.Extensions.Value<T>(...)` |

文档注释类警告（`CS1570/1572/1573/1587`）与过时 API（`SYSLIB*`）、命名规则（`CA1707` 等）
属于「机械注入 + 反编译」的固有产物，已在 `NoWarn` 中收敛，避免淹没真实错误。

## 重要限制

- **可构建但不可用于生产**：工程已修到 `dotnet build` 零错误（见下节），产物仅用于验证
  反编译代码的类型完整性；不要拿它替代官方 NuGet 包。
- **反编译 ≠ 原始源码**：局部变量名、编译器生成的成员布局与作者原始工程必然存在差异；
  变量重命名是按规则推断的语义名，不等同于作者命名。
- **无运行时验证**：本目录仅为静态产物，未执行、未做行为对照。
- **版权**：HslCommunication 为商业组件，版权归原作者所有；仅供内部技术参考，不得再分发。

## 重新生成

```powershell
# 1) 反编译
ilspycmd -p -o .\hsl-decompiled ..\..\src\plc-gateway\lib\HslCommunication.dll

# 2) 回填官方文档注释
pwsh -File .\scripts\inject-xml-doc-comments.ps1 `
  -SourceDir .\hsl-decompiled `
  -XmlPath ..\..\src\plc-gateway\lib\HslCommunication.xml `
  -AssemblyVersion "13.0.2.0" `
  -AssemblyHash "F5B02381F1C74B0578BDD5E883C3C0CB548B223015CDD81EE3D7922A26FA9F30"

# 3) 局部变量重命名（先干跑预览，再 --apply）
dotnet build .\scripts\tools\LocalRenamer -c Release
.\scripts\tools\LocalRenamer\bin\Release\net11.0\LocalRenamer.exe .\hsl-decompiled
.\scripts\tools\LocalRenamer\bin\Release\net11.0\LocalRenamer.exe .\hsl-decompiled --apply
.\scripts\tools\LocalRenamer\bin\Release\net11.0\LocalRenamer.exe .\hsl-decompiled --check
```
## 运行方式

两种等价实现（对 658 个文件做 SHA-256 比对，输出完全一致）：

```powershell
# A. 单文件 —— .NET 11 file-based app，首次运行需联网还原 Roslyn 包（走代理）
dotnet run .\scripts\tools\LocalRenamer.cs -- .\hsl-decompiled --check
dotnet run .\scripts\tools\LocalRenamer.cs -- .\hsl-decompiled            # 干跑预览
dotnet run .\scripts\tools\LocalRenamer.cs -- .\hsl-decompiled --apply

# B. 常规工程 —— 离线可构建，Roslyn 直接取 SDK 自带程序集
dotnet build .\scripts\tools\LocalRenamer -c Release
.\scripts\tools\LocalRenamer\bin\Release\net11.0\LocalRenamer.exe .\hsl-decompiled --check
```

单文件版依赖 `#:package Microsoft.CodeAnalysis.CSharp@4.14.0`，首次运行会触发 NuGet 还原；
工程版通过 `$(RoslynTargetsPath)` 引用 SDK 内置 Roslyn，无需联网。

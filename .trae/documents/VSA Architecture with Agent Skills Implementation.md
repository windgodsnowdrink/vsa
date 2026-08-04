## VSA架构与Agent Skills实现计划 (修正版)

### 1. Agent Skills目录结构设计

```
vsa/
├── api.cs                    # 主应用程序入口 (net10 runfile)
├── api.settings.json         # 应用程序配置
├── api.Development.json      # 开发环境配置
├── scripts/                  # 脚本文件夹
├── README.md                 # 项目文档
├── Carter/                   # Agent Skill - Carter API模块
│   ├── index.yaml           # 元数据层 (YAML frontmatter, 总是加载)
│   ├── SKILL.md             # 技能入口点 (<=200行, 技能激活时加载)
│   ├── UserModule.cs         # 引用文件 - 用户API实现
│   └── UserModule_test.cs    # 引用文件 - 单元测试
└── Scrutor/                  # Agent Skill - Scrutor服务装饰
    ├── index.yaml           # 元数据层 (YAML frontmatter, 总是加载)
    ├── SKILL.md             # 技能入口点 (<=200行, 技能激活时加载)
    ├── ServiceDecoration.cs  # 引用文件 - 服务装饰实现
    └── ServiceDecoration_test.cs # 引用文件 - 单元测试
```

### 2. 实现步骤

#### 步骤1: 创建主应用程序
- 实现 `api.cs` 作为主入口 (net10 runfile)
- 配置 net10 runfile 格式
- 集成 Carter 和 Scrutor 框架
- 实现 Startup 类用于服务配置

#### 步骤2: 实现 Carter Agent Skill
1. **元数据层**: 创建 `Carter/index.yaml` 包含技能元数据
2. **技能入口点**: 创建 `Carter/SKILL.md` (<=200行)，包含：
   - 技能概览
   - 快速入门指南
   - 导航地图
3. **引用文件**: 实现 `Carter/UserModule.cs` 包含用户CRUD API端点
4. **测试文件**: 实现 `Carter/UserModule_test.cs` 包含单元测试

#### 步骤3: 实现 Scrutor Agent Skill
1. **元数据层**: 创建 `Scrutor/index.yaml` 包含技能元数据
2. **技能入口点**: 创建 `Scrutor/SKILL.md` (<=200行)，包含：
   - 技能概览
   - 快速入门指南
   - 导航地图
3. **引用文件**: 实现 `Scrutor/ServiceDecoration.cs` 包含服务装饰器模式
4. **测试文件**: 实现 `Scrutor/ServiceDecoration_test.cs` 包含单元测试

#### 步骤4: 添加技能激活机制
- 实现技能按需加载机制
- 添加code-review技能用于测试
- 添加sequential-thinking技能用于调试
- 添加devops技能用于部署
- 添加ui-styling和web-frameworks技能用于UI构建

### 3. 技术实现要点

- **三层架构**: 严格遵循元数据层、技能入口点、引用文件的三层结构
- **模块化设计**: 每个功能作为独立Agent Skill
- **依赖注入**: 使用Scrutor自动注册和装饰服务
- **API路由**: 使用Carter实现模块化API
- **测试驱动**: 每个技能包含对应的单元测试
- **配置管理**: 支持多环境配置文件
- **文档完善**: 每个技能包含详细SKILL.md和README

### 4. 代码规范

- 遵循SOLID原则
- 使用async/await模式
- 添加详细中文注释
- 遵循项目现有代码风格
- 使用xUnit + Moq进行测试
- 每个引用文件控制在200-300行
- SKILL.md控制在200行以内

### 5. 验证计划

- 先实现主应用程序和一个技能
- 运行测试确保功能正常
- 然后实现第二个技能
- 再次运行测试
- 确保所有组件协同工作

### 6. 技能激活说明

- **编写测试**: 激活 code-review 技能
- **调试生产问题**: 激活 sequential-thinking 技能
- **部署基础设施**: 激活 devops 技能
- **构建UI**: 激活 ui-styling 和 web-frameworks 技能

这个计划严格遵循了Agent Skills的三层架构设计，包含了技能激活机制，并确保每个组件都符合行数量限制要求。
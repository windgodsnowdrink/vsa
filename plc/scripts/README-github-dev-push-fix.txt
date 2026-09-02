README — 修复「JetBrains IDE 提交 dev 分支失败」
=====================================================

错误原文（Rider / IntelliJ 执行 Commit and Push 或 Fetch 时）：

    git -c diff.mnemonicprefix=false -c core.quotepath=false --no-optional-locks fetch --no-tags origin
    FATAL ERROR: Network error: Software caused connection abort
    fatal: Could not read from remote repository.

    Please make sure you have the correct access rights
    and the repository exists.

根因
----
Windows 端虽然系统代理 / Clash / v2rayN 可能已经打开，但 Git for Windows 的 libcurl
并不会总读取 HTTPS_PROXY 环境变量，更不会读取浏览器 PAC。于是 git fetch / ls-remote
**直接 TCP 连 github.com:443** —— 该端口在你当前所在网络/公司出口被防火墙 / ICP
阻断（在 Linux 沙箱里同样阻断），连接被中途 Reset，表现为 libgit2 报告
Software caused connection abort。IDE 看到 fetch 非 0，就把“提交 + 同步”整个流程
标红成“提交失败”，即使本地 commit 实际已经落盘，甚至 origin/dev 之前已经同步完毕。

验证命令（Windows PowerShell）
------------------------------
    # 直连应当被断（Network error）
    Test-NetConnection github.com -Port 443

    # 经过本地代理 CONNECT 应该是 200 OK
    #   7890 = Clash 默认；10809 = v2rayN 默认 HTTP；请改成你自己的
    Invoke-WebRequest -Uri 'https://github.com' -Proxy 'http://127.0.0.1:7890'

脚本
----
    .\scripts\06-fix-github-dev-push.ps1
        -> 自动探测本机代理（7890/10809/1080/18080），写仓库级 git config
        -> 执行 ls-remote / fetch / push dry-run 三件套，全部 0 退出 = 修复完成

    # 自定义端口
    .\scripts\06-fix-github-dev-push.ps1 -Proxy 'http://127.0.0.1:10809'

    # 不想每次弹 GCM：写入 Fine-grained PAT（推荐）
    .\scripts\06-fix-github-dev-push.ps1 -SetPatRemote

    # 切换到无代理网络（比如公司内网）时清除 proxy
    .\scripts\06-fix-github-dev-push.ps1 -RemoveProxy

手动写也完全等价（仓库级，不影响其他项目）
----------------------------------------------
    git config --local http.proxy  http://127.0.0.1:<你的代理端口>
    git config --local https.proxy http://127.0.0.1:<你的代理端口>
    git config --local http.version HTTP/1.1
    git config --local http.postBuffer 524288000
    git config --global credential.helper manager-core         # 首次启用时执行一次
    git config --global credential.https://github.com.provider github

    git fetch --no-tags origin       # 应当 exit=0

HTTPS vs SSH
------------
如果你更喜欢 SSH：先在 GitHub Settings → SSH Keys 上传本机公钥，然后
    git remote set-url origin git@github.com:windgodsnowdrink/vsa.git
    ssh -T git@github.com
即可永久绕开 HTTPS 凭据 / 代理对 HTTPS 的 CONNECT 限制；SSH 的代理请走
~/.ssh/config (Windows: %USERPROFILE%\.ssh\config):

    Host github.com
      HostName github.com
      User git
      ProxyCommand connect -S 127.0.0.1:7890 %h %p   # SOCKS 或
      # ProxyCommand connect -H 127.0.0.1:7890 %h %p # HTTP CONNECT

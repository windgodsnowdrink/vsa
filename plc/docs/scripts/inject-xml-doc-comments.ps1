<#
.SYNOPSIS
  为 ilspycmd 反编译产物回填标准 XML 文档注释（/// <summary> 等）。

.DESCRIPTION
  数据来源是程序集自带的官方 XML 文档（HslCommunication.xml）。该文件存在若干打包
  损坏点，无法用严格 XML 解析器读取，故改用宽松的正则提取：只取 member 的
  summary / param / returns / remarks 文本，剥掉内层标签，输出始终是合法注释。

  匹配按 ECMA-334 文档注释 ID：类型 T:、方法 M:、属性 P:、字段 F:、事件 E:。
  策略为「宁缺勿错」：重载无法唯一确定时跳过，并计入统计。

.PARAMETER SourceDir
  反编译源码根目录（文件级命名空间，每个类型一个 .cs，目录名即命名空间）。

.PARAMETER XmlPath
  程序集配套的 XML 文档文件。

.EXAMPLE
  pwsh -File .\inject-xml-doc-comments.ps1 -SourceDir ..\hsl-decompiled -XmlPath ..\..\src\plc-gateway\lib\HslCommunication.xml
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory)][string]$SourceDir,
  [Parameter(Mandatory)][string]$XmlPath,
  [string]$AssemblyVersion = "",
  [string]$AssemblyHash = "",
  [string]$GeneratedOn = (Get-Date -Format "yyyy-MM-dd"),
  [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# ---------------------------------------------------------------- 1) 读文档
$raw = [IO.File]::ReadAllText($XmlPath, [Text.Encoding]::UTF8)

function ConvertTo-PlainText([string]$s) {
  if ([string]::IsNullOrWhiteSpace($s)) { return "" }
  $t = $s -replace '<br\s*/?>', "`n" -replace '</para>', "`n"
  $t = $t -replace '<[^>]+>', ''                 # 剥掉内层标签（see / paramref / c ...）
  $t = [System.Net.WebUtility]::HtmlDecode($t)
  $t = ($t -split "`r?`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }) -join "`n"
  return $t.Trim()
}

# id -> @{ Summary; Params=@(); Returns; Remarks; Inherit }
$docs = @{}
foreach ($m in [regex]::Matches($raw, '(?s)<member\s+name="([^"]+)"\s*>(.*?)</member>')) {
  $id = $m.Groups[1].Value
  $body = $m.Groups[2].Value
  $d = @{ Params = @(); Summary = ""; Returns = ""; Remarks = ""; Inherit = $null }
  $s = [regex]::Match($body, '(?s)<summary>(.*?)</summary>')
  if ($s.Success) { $d.Summary = ConvertTo-PlainText $s.Groups[1].Value }
  $r = [regex]::Match($body, '(?s)<returns>(.*?)</returns>')
  if ($r.Success) { $d.Returns = ConvertTo-PlainText $r.Groups[1].Value }
  $rm = [regex]::Match($body, '(?s)<remarks>(.*?)</remarks>')
  if ($rm.Success) { $d.Remarks = ConvertTo-PlainText $rm.Groups[1].Value }
  $ps = @()
  foreach ($p in [regex]::Matches($body, '(?s)<param\s+name="([^"]+)"\s*>(.*?)</param>')) {
    $ps += @{ Name = $p.Groups[1].Value; Text = ConvertTo-PlainText $p.Groups[2].Value }
  }
  $d.Params = $ps
  $inh = [regex]::Match($body, '<inheritdoc\s+cref="([^"]+)"')
  if ($inh.Success) { $d.Inherit = $inh.Groups[1].Value }
  $docs[$id] = $d
}

# 方法重载索引： "类型全名|方法名" -> 文档 ID 列表，避免逐次扫描全部键
$methodIndex = @{}
foreach ($id in $docs.Keys) {
  if (-not $id.StartsWith("M:")) { continue }
  $open = $id.IndexOf("(")
  if ($open -lt 0) { continue }
  $head = $id.Substring(2, $open - 2)          # 去掉 "M:" 与参数表
  $dot = $head.LastIndexOf(".")
  if ($dot -lt 0) { continue }
  $key = $head.Substring(0, $dot) + "|" + $head.Substring($dot + 1)
  if (-not $methodIndex.ContainsKey($key)) { $methodIndex[$key] = New-Object System.Collections.Generic.List[string] }
  $methodIndex[$key].Add($id)
}

function Resolve-Doc([string]$id, [int]$depth = 0) {
  if (-not $id -or $depth -gt 3 -or -not $docs.ContainsKey($id)) { return $null }
  $d = $docs[$id]
  if ($d.Summary -or $d.Params.Count -gt 0) { return $d }
  if ($d.Inherit) {
    $parent = Resolve-Doc $d.Inherit ($depth + 1)
    if ($parent) { return $parent }
  }
  return $d
}

# ------------------------------------------------------- 2) 类型名归一化表
$alias = @{
  "bool"="Boolean"; "byte"="Byte"; "sbyte"="SByte"; "char"="Char"; "decimal"="Decimal"
  "double"="Double"; "float"="Single"; "int"="Int32"; "uint"="UInt32"; "long"="Int64"
  "ulong"="UInt64"; "short"="Int16"; "ushort"="UInt16"; "object"="Object"
  "string"="String"; "void"="Void"; "nint"="IntPtr"; "nuint"="UIntPtr"
}

function Get-SimpleTypeName([string]$typeText) {
  if ([string]::IsNullOrWhiteSpace($typeText)) { return "" }
  $t = $typeText.Trim()
  $t = $t -replace '^\s*(ref|out|in|params|this)\s+', ''
  $parts = $t -split '\s+'
  $t = $parts[0]
  $suffix = ""
  while ($t.EndsWith("[]")) { $suffix += "[]"; $t = $t.Substring(0, $t.Length - 2) }
  $t = $t -replace '\{', '<' -replace '\}', '>'
  $t = $t -replace '<.*$', ''
  if ($t.Contains(".")) { $t = $t.Substring($t.LastIndexOf(".") + 1) }
  $t = $t -replace '`\d+$', ''
  if ($alias.ContainsKey($t)) { $t = $alias[$t] }
  return $t + $suffix
}

function Get-DocLines($doc, [string]$indent) {
  $out = New-Object System.Collections.Generic.List[string]
  if (-not $doc) { return $out }
  if ($doc.Summary) {
    $out.Add("$indent/// <summary>")
    foreach ($l in ($doc.Summary -split "`n")) { $out.Add("$indent/// $l") }
    $out.Add("$indent/// </summary>")
  }
  foreach ($p in $doc.Params) {
    if (-not $p.Text) { continue }
    # 单行标签内不能有换行，否则会写出裸文本破坏语法
    $flat = ($p.Text -replace '\s*\r?\n\s*', ' ').Trim()
    $esc = [System.Security.SecurityElement]::Escape($flat)
    $out.Add("$indent/// <param name=`"$($p.Name)`">$esc</param>")
  }
  if ($doc.Returns) {
    $flat = ($doc.Returns -replace '\s*\r?\n\s*', ' ').Trim()
    $out.Add("$indent/// <returns>$([System.Security.SecurityElement]::Escape($flat))</returns>")
  }
  if ($doc.Remarks) {
    $out.Add("$indent/// <remarks>")
    foreach ($l in ($doc.Remarks -split "`n")) { $out.Add("$indent/// $l") }
    $out.Add("$indent/// </remarks>")
  }
  return $out
}

# ------------------------------------------------------------- 3) 逐文件处理
$files = Get-ChildItem -Path $SourceDir -Recurse -Filter *.cs -File
$stat = [ordered]@{
  Files = $files.Count; TypesHit = 0; TypesMiss = 0
  MembersHit = 0; MembersMiss = 0; Ambiguous = 0
}

$headerLines = @(
  "// -----------------------------------------------------------------------------"
  ("// 反编译产物 | 来源程序集: HslCommunication.dll" + $(if ($AssemblyVersion) { " (v$AssemblyVersion)" } else { "" }))
  $(if ($AssemblyHash) { "// SHA-256: $AssemblyHash" } else { $null })
  "// 反编译工具: ilspycmd 11.0.0.9375 (ICSharpCode.Decompiler)"
  "// 生成日期  : $GeneratedOn"
  "// 注释来源  : 官方 XML 文档 HslCommunication.xml（机械回填，未改写措辞）"
  "// 说明      : 仅供阅读 / 排障 / 协议核对参考；版权归 HslCommunication 原作者所有。"
  "// -----------------------------------------------------------------------------"
) | Where-Object { $_ }

foreach ($file in $files) {
  $ns = $file.Directory.Name
  $typeName = [IO.Path]::GetFileNameWithoutExtension($file.Name)
  if ($typeName -eq "AssemblyInfo") { continue }

  $text = [IO.File]::ReadAllText($file.FullName)
  $hasBom = $text.Length -gt 0 -and $text[0] -eq [char]0xFEFF
  if ($hasBom) { $text = $text.Substring(1) }
  $eol = if ($text -match "`r`n") { "`r`n" } else { "`n" }
  $lines = $text -split "`r?`n"

  $typeFull = "$ns.$typeName"
  $inserts = New-Object System.Collections.Generic.List[object]

  # ---- 3a) 类型级
  $typeDeclIdx = -1
  for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^(public|internal|private|protected|sealed|abstract|static|partial|readonly|unsafe|\s)*\b(class|struct|interface|enum|record)\b') {
      $typeDeclIdx = $i; break
    }
  }
  if ($typeDeclIdx -ge 0) {
    $anchor = $typeDeclIdx
    while ($anchor -gt 0 -and $lines[$anchor - 1].Trim().StartsWith("[")) { $anchor-- }
    $doc = Resolve-Doc "T:$typeFull"
    $block = Get-DocLines $doc ""
    if ($block.Count -gt 0) { $inserts.Add(@{ Index = $anchor; Lines = $block }); $stat.TypesHit++ }
    else { $stat.TypesMiss++ }
  }

  # ---- 3b) 成员级
  for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -notmatch '^\t[^\t]') { continue }
    $t = $line.Trim()
    if ($t -match '^(//|#|\[|\{|\}|else\b|if\b|for\b|foreach\b|while\b|switch\b|return\b|try\b|catch\b|finally\b|using\b|case\b|default\b|break\b|continue\b|throw\b|lock\b|do\b|=>)') { continue }

    $kind = $null; $name = $null; $paramSimple = @()
    if ($t -match '\bevent\s+[\w\.<>\[\],\s]+\s+(\w+)\s*[;{]') { $kind = "E"; $name = $Matches[1] }
    elseif ($t -match '^\s*([\w\.<>\[\],\s]+?)\s+(\w+)\s*\{') { $kind = "P"; $name = $Matches[2] }
    elseif ($t -match '^\s*([\w\.<>\[\],\s]+?)\s+(\w+)\s*[=;]') { $kind = "F"; $name = $Matches[2] }
    elseif ($t -match '^\s*([\w\.<>\[\],\s]+?)\s+(\w+)\s*$' -and
            (($i + 1) -lt $lines.Count) -and ($lines[$i + 1].Trim() -eq "{")) {
      # 属性：大括号换行书写
      $kind = "P"; $name = $Matches[2]
    }
    elseif ($t -match '^\s*([\w\.<>\[\],\s]+?)\s+(\w+)\s*\(') {
      $kind = "M"; $name = $Matches[2]
      if ($name -eq $typeName) { $name = "#ctor" }
      $sig = $t.Substring($t.IndexOf("("))
      $depth = 0; $angle = 0; $buf = ""; $parms = New-Object System.Collections.Generic.List[string]
      foreach ($ch in $sig.ToCharArray()) {
        if ($ch -eq '(') { $depth++; if ($depth -eq 1) { continue } }
        if ($ch -eq ')') { $depth--; if ($depth -eq 0) { break } }
        if ($ch -eq '<') { $angle++ }
        if ($ch -eq '>') { $angle-- }
        if ($ch -eq ',' -and $depth -eq 1 -and $angle -eq 0) { $parms.Add($buf); $buf = ""; continue }
        $buf += $ch
      }
      if ($buf.Trim()) { $parms.Add($buf) }
      $paramSimple = @($parms | ForEach-Object { Get-SimpleTypeName $_ })
    }
    else { continue }
    if (-not $kind -or -not $name) { continue }

    # 已有注释则跳过
    $j = $i - 1
    while ($j -ge 0 -and $lines[$j].Trim().StartsWith("[")) { $j-- }
    if ($j -ge 0 -and $lines[$j].Trim().StartsWith("///")) { continue }

    $candidate = $null
    if ($kind -eq "M") {
      # 文档 ID 的两种形态：无参方法不带括号，泛型方法带 ``N 后缀
      $nameVariants = @($name)
      for ($g = 1; $g -le 3; $g++) { $nameVariants += "$name``$g" }
      $hits = @()
      $noArg = $null
      foreach ($nv in $nameVariants) {
        $k = "$typeFull|$nv"
        if ($methodIndex.ContainsKey($k)) { $hits += $methodIndex[$k] }
        $idNoArg = "M:$typeFull.$nv"
        if (-not $noArg -and $docs.ContainsKey($idNoArg)) { $noArg = $idNoArg }
      }
      if ($paramSimple.Count -eq 0 -and $noArg) { $candidate = $noArg }
      elseif ($hits.Count -eq 1) { $candidate = $hits[0] }
      elseif ($hits.Count -gt 1) {
        $best = @()
        foreach ($h in $hits) {
          $openParen = $h.IndexOf("(")
          if ($openParen -lt 0) { continue }
          $inner = $h.Substring($openParen + 1).TrimEnd(')')
          $xs = @()
          if ($inner.Trim()) {
            $d = 0; $b = ""; $cur = New-Object System.Collections.Generic.List[string]
            foreach ($ch in $inner.ToCharArray()) {
              if ($ch -eq '{' -or $ch -eq '[') { $d++ }
              if ($ch -eq '}' -or $ch -eq ']') { $d-- }
              if ($ch -eq ',' -and $d -eq 0) { $cur.Add($b); $b = ""; continue }
              $b += $ch
            }
            if ($b.Trim()) { $cur.Add($b) }
            $xs = @($cur | ForEach-Object { Get-SimpleTypeName $_ })
          }
          if (($xs -join "|") -eq ($paramSimple -join "|")) { $best += $h }
        }
        if ($best.Count -eq 1) { $candidate = $best[0] }
        elseif ($best.Count -gt 1) { $stat.Ambiguous++ }
      }
    }
    else {
      $id = "$kind`:$typeFull.$name"
      if ($docs.ContainsKey($id)) { $candidate = $id }
    }

    if ($candidate) {
      $block = Get-DocLines (Resolve-Doc $candidate) "`t"
      if ($block.Count -gt 0) { $inserts.Add(@{ Index = $i; Lines = $block }); $stat.MembersHit++ }
      else { $stat.MembersMiss++ }
    }
    else { $stat.MembersMiss++ }
  }

  $byIndex = @{}
  foreach ($ins in $inserts) { $byIndex[$ins.Index] = $ins.Lines }

  $outLines = New-Object System.Collections.Generic.List[string]
  $outLines.AddRange([string[]]$headerLines)
  for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($byIndex.ContainsKey($i)) { foreach ($l in $byIndex[$i]) { $outLines.Add($l) } }
    $outLines.Add($lines[$i])
  }

  if (-not $DryRun) {
    $final = ($outLines -join $eol)
    if ($hasBom) { $final = [char]0xFEFF + $final }
    [IO.File]::WriteAllText($file.FullName, $final, (New-Object System.Text.UTF8Encoding($hasBom)))
  }
}

$stat.GetEnumerator() | ForEach-Object { "{0,-12} {1}" -f $_.Key, $_.Value }

# strip-sources.ps1 - strip #include / #r directives so File-based sources compile.
# Args: $args[0] = source root, $args[1] = output root. Does not modify original sources.
# NOTE: must read/write via .NET File APIs with explicit UTF-8; PowerShell's default
# (GBK on zh-CN) mis-decodes UTF-8 and drops newlines after full-width parens.
$SrcRoot = $args[0]
$OutRoot = $args[1]
$ErrorActionPreference = 'Stop'
$Utf8 = [System.Text.Encoding]::UTF8

if (Test-Path $OutRoot) { Remove-Item $OutRoot -Recurse -Force }
New-Item -ItemType Directory -Path $OutRoot | Out-Null

function Strip-File {
    param($inPath, $outPath)
    $text = [System.IO.File]::ReadAllText($inPath, $Utf8)
    $lines = $text -split "`r?`n"
    $kept = New-Object System.Collections.ArrayList
    foreach ($line in $lines) {
        $t = $line.TrimStart()
        if ($t.StartsWith('#include')) { continue }
        if ($t.StartsWith('#r ')) { continue }
        if ($t.StartsWith('#r"')) { continue }
        [void]$kept.Add($line)
    }
    $out = $kept -join "`n"
    [System.IO.File]::WriteAllText($outPath, $out, $Utf8)
}

$files = [System.IO.Directory]::GetFiles($SrcRoot, '*.cs')
foreach ($f in $files) {
    $name = [System.IO.Path]::GetFileName($f)
    if ($name -eq 'Program.cs') { continue }
    Strip-File $f (Join-Path $OutRoot $name)
}

$slicesDir = Join-Path $SrcRoot 'slices'
if (Test-Path $slicesDir) {
    $sliceFiles = [System.IO.Directory]::GetFiles($slicesDir, '*.cs')
    $destDir = Join-Path $OutRoot 'slices'
    if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir | Out-Null }
    foreach ($f in $sliceFiles) {
        $name = [System.IO.Path]::GetFileName($f)
        Strip-File $f (Join-Path $destDir $name)
    }
}
Write-Host "Stripped sources prepared at $OutRoot"

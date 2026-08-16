# strip-app.ps1 - 剥离 File-based 的 #include / #r 指令，使全部源（含 Program.cs）可作为普通 csproj 编译。
# Args: $args[0] = source root, $args[1] = output root。不修改原始源。
$ErrorActionPreference = 'Stop'
$SrcRoot = [System.IO.Path]::GetFullPath($args[0])
$OutRoot = [System.IO.Path]::GetFullPath($args[1])
Write-Host "STRIP SRC=[$SrcRoot] OUT=[$OutRoot]"

if (Test-Path $OutRoot) { Remove-Item $OutRoot -Recurse -Force }
New-Item -ItemType Directory -Path $OutRoot | Out-Null

function Strip-File($inPath, $outPath) {
    $text = [System.IO.File]::ReadAllText($inPath, [System.Text.Encoding]::UTF8)
    $outLines = New-Object System.Collections.ArrayList
    foreach ($line in ($text -split "`r?`n")) {
        $t = $line.TrimStart()
        $isDirective = $t.StartsWith('#include') -or $t.StartsWith('#r ') -or $t.StartsWith('#r"')
        if (-not $isDirective) {
            [void]$outLines.Add($line)
        }
    }
    [System.IO.File]::WriteAllText($outPath, ($outLines -join "`n"), [System.Text.Encoding]::UTF8)
}

$top = [System.IO.Directory]::GetFiles($SrcRoot, '*.cs')
Write-Host "topCount=$($top.Count)"
foreach ($f in $top) {
    $name = [System.IO.Path]::GetFileName($f)
    Strip-File $f (Join-Path $OutRoot $name)
}

$sd = Join-Path $SrcRoot 'slices'
Write-Host "slicesDirExists=$(Test-Path $sd)"
if (Test-Path $sd) {
    $dd = Join-Path $OutRoot 'slices'
    New-Item -ItemType Directory -Path $dd | Out-Null
    $sl = [System.IO.Directory]::GetFiles($sd, '*.cs')
    Write-Host "sliceCount=$($sl.Count)"
    foreach ($f in $sl) {
        Strip-File $f (Join-Path $dd ([System.IO.Path]::GetFileName($f)))
    }
}
Write-Host "STRIP DONE"

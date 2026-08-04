# PowerShell script to create .appsetting.json and .run.json files for all .cs files in skills directory

$csFiles = Get-ChildItem -Path d:\Trae\vsa\skills -Recurse -File -Filter "*.cs" | Where-Object { $_.Directory.Name -eq "scripts" }

foreach ($file in $csFiles) {
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    $appsettingPath = "$($file.Directory.FullName)\$baseName.appsetting.json"
    $runPath = "$($file.Directory.FullName)\$baseName.run.json"
    
    # Create .appsetting.json file if it doesn't exist
    if (-not (Test-Path -Path $appsettingPath)) {
        Write-Host "Creating $appsettingPath"
        $appsettingContent = @"
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
"@
        Set-Content -Path $appsettingPath -Value $appsettingContent -Encoding UTF8
    }
    
    # Create .run.json file if it doesn't exist
    if (-not (Test-Path -Path $runPath)) {
        Write-Host "Creating $runPath"
        $runContent = @"
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "$baseName",
      "type": "dotnet",
      "request": "launch",
      "program": "$($file.FullName)",
      "args": [
        "--settings",
        "$appsettingPath"
      ],
      "cwd": "d:\\Trae\\vsa",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development"
      },
      "stopAtEntry": false,
      "console": "integratedTerminal"
    }
  ]
}
"@
        Set-Content -Path $runPath -Value $runContent -Encoding UTF8
    }
    
    # Update existing run.json files to use .appsetting.json instead of .settings.json
    if (Test-Path -Path $runPath) {
        $runContent = Get-Content -Path $runPath -Raw
        if ($runContent -match "\\.settings\\.json") {
            Write-Host "Updating $runPath to use .appsetting.json"
            $runContent = $runContent -replace "\\.settings\\.json", ".appsetting.json"
            Set-Content -Path $runPath -Value $runContent -Encoding UTF8
        }
    }
}
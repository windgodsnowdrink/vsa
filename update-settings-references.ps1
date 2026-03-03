# Script to update all references from .appsetting.json to .setting.json in run.json files

# Get all run.json files in the skills directory
$runFiles = Get-ChildItem -Path "d:/Trae/vsa/skills" -Filter "*.run.json" -Recurse -File

foreach ($runFile in $runFiles) {
    Write-Host "Processing $($runFile.FullName)..."
    
    try {
        # Read the content of the run.json file
        $content = Get-Content -Path $runFile.FullName -Raw
        
        # Replace all occurrences of .appsetting.json with .setting.json
        $newContent = $content -replace '\.appsetting\.json', '.setting.json'
        
        # Write the updated content back to the file only if changes were made
        if ($content -ne $newContent) {
            Set-Content -Path $runFile.FullName -Value $newContent
            Write-Host "Updated $($runFile.FullName) successfully."
        }
    } catch {
        Write-Host "Error processing $($runFile.FullName)"
    }
}

Write-Host "All run.json files have been processed."
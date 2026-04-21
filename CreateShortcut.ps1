$appDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$exePath = Join-Path $appDir "bin\Debug\net8.0-windows\VsCodeProfileProjectSearch.exe"

if (-not (Test-Path -LiteralPath $exePath)) {
    Write-Host "App executable not found: $exePath"
    Write-Host "Build the app first with: dotnet build"
    exit 1
}

$desktop = [Environment]::GetFolderPath("Desktop")
$shortcutPath = Join-Path $desktop "VS Code Project Search.lnk"

$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $exePath
$shortcut.WorkingDirectory = Split-Path -Parent $exePath
$shortcut.IconLocation = "$exePath,0"
$shortcut.Description = "Open recent projects across VS Code profile containers"
$shortcut.Save()

Write-Host "Desktop shortcut created: $shortcutPath"

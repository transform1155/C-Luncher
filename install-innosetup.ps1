# Inno Setup 快速安装脚本
# 使用方法: powershell -ExecutionPolicy Bypass -File install-innosetup.ps1

$ErrorActionPreference = "Stop"

Write-Host "Downloading Inno Setup..." -ForegroundColor Yellow

$downloadUrl = "https://files.jrsoftware.org/is/6/innosetup-6.2.2.exe"
$installerPath = Join-Path $env:TEMP "innosetup-installer.exe"

# Download
Invoke-WebRequest -Uri $downloadUrl -OutFile $installerPath -UseBasicParsing

Write-Host "Installing Inno Setup..." -ForegroundColor Yellow

# Silent install
Start-Process -FilePath $installerPath -ArgumentList "/VERYSILENT", "/NORESTART" -Wait -NoNewWindow

# Add to PATH
$installDir = "C:\Program Files (x86)\Inno Setup 6"
if (Test-Path $installDir) {
    $currentPath = [Environment]::GetEnvironmentVariable("Path", "Machine")
    if ($currentPath -notlike "*Inno Setup 6*") {
        [Environment]::SetEnvironmentVariable("Path", $currentPath + ";" + $installDir, "Machine")
        Write-Host "Inno Setup added to PATH" -ForegroundColor Green
    }
    Write-Host "Inno Setup installed at: $installDir" -ForegroundColor Green
    Write-Host "ISCC.exe: $(Join-Path $installDir ISCC.exe)" -ForegroundColor Cyan
} else {
    Write-Host "Warning: Installation directory not found. Install manually." -ForegroundColor Red
}

# Cleanup
Remove-Item $installerPath -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green

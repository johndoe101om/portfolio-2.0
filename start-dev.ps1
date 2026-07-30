# Single-click PowerShell launcher for Portfolio API & Client
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  Starting Portfolio API & Client Services..." -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan

$scriptDir = $PSScriptRoot

# Start Backend API
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$scriptDir/src/Portfolio.Api'; Write-Host 'Starting API...' -ForegroundColor Yellow; dotnet watch run"

# Start Frontend Client
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$scriptDir/src/Portfolio.Client'; Write-Host 'Starting Frontend Client...' -ForegroundColor Yellow; npm run dev"

Write-Host "Backend and Frontend launched successfully!" -ForegroundColor Green

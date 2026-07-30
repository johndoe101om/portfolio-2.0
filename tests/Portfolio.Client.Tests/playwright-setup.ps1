#!/usr/bin/env pwsh
# ============================================================
# Playwright Browser Setup
# Run this once before executing E2E tests:
#   pwsh tests/Portfolio.Client.Tests/playwright-setup.ps1
# ============================================================

Write-Host "Installing Playwright browsers..." -ForegroundColor Cyan

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = $scriptDir

# Build the test project first
dotnet build $projectDir/Portfolio.Client.Tests.csproj

# Find the playwright CLI
$playwrightCli = Get-ChildItem -Path "$env:USERPROFILE/.nuget" -Recurse -Filter "playwright.ps1" -ErrorAction SilentlyContinue |
                 Select-Object -First 1 -ExpandProperty FullName

if (-not $playwrightCli) {
    $playwrightCli = Get-ChildItem -Path "$HOME/.nuget" -Recurse -Filter "playwright.ps1" -ErrorAction SilentlyContinue |
                     Select-Object -First 1 -ExpandProperty FullName
}

if ($playwrightCli) {
    & $playwrightCli install chromium firefox
    Write-Host "Browsers installed successfully." -ForegroundColor Green
} else {
    Write-Host "Could not locate playwright.ps1. Try running:" -ForegroundColor Yellow
    Write-Host "  dotnet tool install --global Microsoft.Playwright.CLI" -ForegroundColor Yellow
    Write-Host "  playwright install" -ForegroundColor Yellow
}

@echo off
echo ========================================================
echo   Starting Portfolio API and Client (Single Click Script)
echo ========================================================

:: Launch ASP.NET API Backend in a new window with live reload (watch)
start "Portfolio API (.NET)" cmd /k "cd /d %~dp0src\Portfolio.Api && dotnet watch run"

:: Launch Vite React/TS Frontend in a new window
start "Portfolio Client (Vite)" cmd /k "cd /d %~dp0src\Portfolio.Client && npm run dev"

echo Both services launched in separate windows!

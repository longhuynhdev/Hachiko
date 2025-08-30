#!/usr/bin/env pwsh

# Hachiko Documentation Build Script (PowerShell)
# This script builds the solution and generates DocFx documentation

param(
    [switch]$Serve,
    [switch]$Help
)

if ($Help) {
    Write-Host "Hachiko Documentation Build Script" -ForegroundColor Cyan
    Write-Host "Usage: ./build-docs.ps1 [-Serve] [-Help]" -ForegroundColor Green
    Write-Host "  -Serve    Automatically serve the documentation after building"
    Write-Host "  -Help     Show this help message"
    exit 0
}

Write-Host "🚀 Building Hachiko Documentation" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# Check if DocFx is installed
$docfxVersion = docfx --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ DocFx is not installed. Installing..." -ForegroundColor Yellow
    dotnet tool install -g docfx
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ DocFx installed successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ Failed to install DocFx" -ForegroundColor Red
        exit 1
    }
}

# Navigate to script directory
Set-Location $PSScriptRoot

Write-Host "📦 Building .NET solution..." -ForegroundColor Blue
dotnet build EC-Project.sln --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ .NET build completed successfully" -ForegroundColor Green

# Navigate to DocFx project directory
Set-Location docfx_project

Write-Host "📚 Generating API metadata..." -ForegroundColor Blue
docfx metadata docfx.json

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ DocFx metadata generation failed!" -ForegroundColor Red
    exit 1
}

Write-Host "🔨 Building documentation site..." -ForegroundColor Blue
docfx build docfx.json

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Documentation generated successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌐 To view locally, run:" -ForegroundColor Cyan
    Write-Host "   cd docfx_project" -ForegroundColor White
    Write-Host "   docfx serve _site" -ForegroundColor White
    Write-Host ""
    Write-Host "📱 Then open: http://localhost:8080" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🚀 To auto-serve after build:" -ForegroundColor Cyan
    Write-Host "   ./build-docs.ps1 -Serve" -ForegroundColor White
} else {
    Write-Host "❌ Documentation generation failed!" -ForegroundColor Red
    exit 1
}

# Auto-serve if requested
if ($Serve) {
    Write-Host "🌐 Starting local server..." -ForegroundColor Magenta
    docfx serve _site --port 8080
}

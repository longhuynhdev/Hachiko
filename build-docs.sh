#!/bin/bash

# Hachiko Documentation Build Script
# This script builds the solution and generates DocFx documentation

set -e  # Exit on any error

echo "🚀 Building Hachiko Documentation"
echo "================================="

# Check if DocFx is installed
if ! command -v docfx &> /dev/null; then
    echo "❌ DocFx is not installed. Installing..."
    dotnet tool install -g docfx
    echo "✅ DocFx installed successfully"
fi

# Navigate to project root
cd "$(dirname "$0")"

echo "📦 Building .NET solution..."
dotnet build EC-Project.sln --configuration Release

if [ $? -ne 0 ]; then
    echo "❌ .NET build failed!"
    exit 1
fi

echo "✅ .NET build completed successfully"

# Navigate to DocFx project directory
cd docfx_project

echo "📚 Generating API metadata..."
docfx metadata docfx.json

if [ $? -ne 0 ]; then
    echo "❌ DocFx metadata generation failed!"
    exit 1
fi

echo "🔨 Building documentation site..."
docfx build docfx.json

if [ $? -eq 0 ]; then
    echo "✅ Documentation generated successfully!"
    echo ""
    echo "🌐 To view locally, run:"
    echo "   cd docfx_project"
    echo "   docfx serve _site"
    echo ""
    echo "📱 Then open: http://localhost:8080"
    echo ""
    echo "🚀 To auto-serve after build:"
    echo "   ./build-docs.sh --serve"
else
    echo "❌ Documentation generation failed!"
    exit 1
fi

# Auto-serve if requested
if [ "$1" = "--serve" ] || [ "$1" = "-s" ]; then
    echo "🌐 Starting local server..."
    docfx serve _site --port 8080
fi

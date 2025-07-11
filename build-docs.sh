#!/bin/bash

# ClickUp .NET SDK Documentation Build Script
# This script builds the documentation locally for development and testing

set -e  # Exit on any error

echo "🔧 Building ClickUp .NET SDK Documentation..."

# Check if DocFX is installed
if ! command -v docfx &> /dev/null; then
    echo "❌ DocFX is not installed. Installing..."
    dotnet tool install -g docfx
    echo "✅ DocFX installed successfully"
fi

# Build the solution first to generate XML documentation
echo "🏗️  Building solution to generate XML documentation..."
dotnet build src/ClickUp.Api.sln --configuration Release --nologo

if [ $? -ne 0 ]; then
    echo "❌ Solution build failed. Please fix build errors first."
    exit 1
fi

echo "✅ Solution built successfully"

# Navigate to DocFX directory
cd docs/docfx

# Generate API metadata
echo "📚 Generating API metadata..."
docfx metadata docfx.json

if [ $? -ne 0 ]; then
    echo "❌ API metadata generation failed."
    exit 1
fi

echo "✅ API metadata generated successfully"

# Build the documentation site
echo "🔨 Building documentation site..."
docfx build docfx.json

if [ $? -ne 0 ]; then
    echo "❌ Documentation build failed."
    exit 1
fi

echo "✅ Documentation built successfully"

# Check if --serve flag is provided
if [[ "$1" == "--serve" || "$1" == "-s" ]]; then
    echo "🌐 Starting local server at http://localhost:8080"
    echo "📝 Press Ctrl+C to stop the server"
    docfx serve _site --port 8080
else
    echo "🎉 Documentation build complete!"
    echo ""
    echo "📁 Output location: docs/docfx/_site/"
    echo "🌐 To serve locally, run: docfx serve docs/docfx/_site --port 8080"
    echo "🔄 To build and serve in one command, run: ./build-docs.sh --serve"
fi
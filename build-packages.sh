#!/bin/bash

# Build and package ClickUp API Client NuGet packages
# This script builds all three NuGet packages for the ClickUp API Client

set -e

echo "🔧 Building ClickUp API Client NuGet packages..."

# Clean previous builds
echo "🧹 Cleaning previous builds..."
cd src
dotnet clean ClickUp.Api.sln --configuration Release
cd ..

# Create output directory
OUTPUT_DIR="./nuget-packages"
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

echo "📦 Building solution..."
cd src
dotnet build ClickUp.Api.sln --configuration Release

echo "📦 Creating NuGet packages..."

# Set package icon to empty if icon.png doesn't exist
ICON_PARAM=""
if [ ! -f "../icon.png" ]; then
    echo "⚠️  Warning: icon.png not found, packages will be created without icon"
    ICON_PARAM="/p:PackageIcon="
fi

# Pack each project
for project in "ClickUp.Api.Client.Models/ClickUp.Api.Client.Models.csproj" \
               "ClickUp.Api.Client.Abstractions/ClickUp.Api.Client.Abstractions.csproj" \
               "ClickUp.Api.Client/ClickUp.Api.Client.csproj"; do
    echo "📦 Packing $project..."
    dotnet pack "$project" --configuration Release --no-build --output "../$OUTPUT_DIR" $ICON_PARAM
done

cd ..

echo "✅ Package creation completed!"
echo "📁 Packages created in: $OUTPUT_DIR"
echo ""
echo "📋 Created packages:"
ls -la "$OUTPUT_DIR"/*.nupkg

echo ""
echo "🎯 To use these packages locally:"
echo "   dotnet add package Stagware.ClickUp.Api.Client --source ./nuget-packages"
echo ""
echo "📝 Note: Add icon.png to the root directory to include an icon in the packages"
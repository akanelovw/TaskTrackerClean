#!/bin/bash
set -e

echo "Cleaning..."
find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

echo "Publishing..."
dotnet publish TaskTracker.Maui/TaskTracker.Maui.csproj \
  -f net10.0-windows10.0.19041.0 \
  -c Release \
  -p:TargetFrameworks=net10.0-windows10.0.19041.0 \
  -p:WindowsPackageType=None \
  -p:SelfContained=true \
  -p:RuntimeIdentifier=win-x64 \
  -p:DisableTransitiveProjectReferences=false \
  -o ./publish/windows

echo "Done! Output: ./publish/windows"
#!/bin/bash

# Define paths
ROOT_DIR=$(pwd)
PUBLISH_DIR="$ROOT_DIR/publish"
BE_PUBLISH_DIR="$PUBLISH_DIR/be"
FE_PUBLISH_DIR="$PUBLISH_DIR/fe"

# Clear old publish directories
echo "Cleaning up old publish directories..."
rm -rf "$BE_PUBLISH_DIR"
rm -rf "$FE_PUBLISH_DIR"

mkdir -p "$BE_PUBLISH_DIR"
mkdir -p "$FE_PUBLISH_DIR"

# Publish Backend
echo "Publishing Backend..."
cd "$ROOT_DIR/App.Trang/App.Trang.Api"
dotnet publish -c Release -o "$BE_PUBLISH_DIR"

# Publish Frontend
echo "Publishing Frontend..."
cd "$ROOT_DIR/App.Angular"
# Uncomment the line below if you want to install dependencies before building
# npm install
npm run build

# Copy Frontend to publish/fe
echo "Copying Frontend files..."
if [ -d "dist/App.Angular/browser" ]; then
    cp -r dist/App.Angular/browser/* "$FE_PUBLISH_DIR/"
elif [ -d "dist/app.angular/browser" ]; then
    cp -r dist/app.angular/browser/* "$FE_PUBLISH_DIR/"
elif [ -d "dist/App.Angular" ]; then
    cp -r dist/App.Angular/* "$FE_PUBLISH_DIR/"
elif [ -d "dist/app.angular" ]; then
    cp -r dist/app.angular/* "$FE_PUBLISH_DIR/"
else
    # Fallback to copy whatever is in dist
    cp -r dist/* "$FE_PUBLISH_DIR/"
fi

echo "Publish completed successfully! Both BE and FE are in the '$PUBLISH_DIR' folder."

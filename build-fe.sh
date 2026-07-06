#!/bin/bash
ROOT_DIR=$(pwd)
PUBLISH_DIR="$ROOT_DIR/publish"
FE_PUBLISH_DIR="$PUBLISH_DIR/fe"

echo "Cleaning up old FE publish directory..."
rm -rf "$FE_PUBLISH_DIR"
mkdir -p "$FE_PUBLISH_DIR"

echo "Publishing Frontend..."
cd "$ROOT_DIR/App.Angular"
npm install
npm run build

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
    cp -r dist/* "$FE_PUBLISH_DIR/"
fi

echo "Frontend publish completed at '$FE_PUBLISH_DIR'"

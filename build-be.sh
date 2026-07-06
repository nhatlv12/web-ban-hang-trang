#!/bin/bash
ROOT_DIR=$(pwd)
PUBLISH_DIR="$ROOT_DIR/publish"
BE_PUBLISH_DIR="$PUBLISH_DIR/be"

echo "Cleaning up old BE publish directory..."
rm -rf "$BE_PUBLISH_DIR"
mkdir -p "$BE_PUBLISH_DIR"

echo "Publishing Backend..."
cd "$ROOT_DIR/App.Trang/App.Trang.Api"
dotnet publish -c Release -o "$BE_PUBLISH_DIR"

echo "Enabling stdout logging for backend..."
sed -i 's/stdoutLogEnabled="false"/stdoutLogEnabled="true"/g' "$BE_PUBLISH_DIR/web.config"
mkdir -p "$BE_PUBLISH_DIR/logs"

echo "Copying custom appsettings.json..."
if [ -f "$ROOT_DIR/appsettings.json" ]; then
    cp "$ROOT_DIR/appsettings.json" "$BE_PUBLISH_DIR/appsettings.json"
fi

echo "Backend publish completed at '$BE_PUBLISH_DIR'"

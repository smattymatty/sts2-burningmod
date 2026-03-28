#!/bin/bash
set -e

# Personal deploy script - change these two paths to match your system
MOD_DIR="/home/mathew/snap/steam/common/.local/share/Steam/steamapps/common/Slay the Spire 2/mods/BurningMod"
GODOT="/home/mathew/Desktop/Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64"

echo "Building..."
./scripts/gen_presets.sh
dotnet build BurningMod.csproj

echo "Exporting PCK..."
"$GODOT" --headless --export-pack "Linux" "$MOD_DIR/BurningMod.pck"

echo "Copying DLL..."
cp .godot/mono/temp/bin/Debug/BurningMod.dll "$MOD_DIR/BurningMod.dll"

echo "Copying manifest..."
cp BurningMod.json "$MOD_DIR/BurningMod.json"

echo "Done!"
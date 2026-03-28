#!/bin/bash
# gen_presets.sh - regenerate export_presets.cfg from actual files

PROJECT_DIR="/home/mathew/sts-2"

# Collect all files to export
FILES=""

# All mod images and localization
for f in $(find "$PROJECT_DIR/BurningMod" -name "*.png" -o -name "*.json"); do
    RESPATH="res://${f#$PROJECT_DIR/}"
    FILES="$FILES\"$RESPATH\", "
done

# Power images
for f in $(find "$PROJECT_DIR/images" -name "*.png"); do
    RESPATH="res://${f#$PROJECT_DIR/}"
    FILES="$FILES\"$RESPATH\", "
done

# mod_manifest.json
FILES="$FILES\"res://mod_manifest.json\", "

# Strip trailing comma and space
FILES="${FILES%, }"

cat > "$PROJECT_DIR/export_presets.cfg" << EOF
[preset.0]
name="Linux"
platform="Linux"
runnable=true
advanced_options=false
dedicated_server=false
custom_features=""
export_filter="resources"
export_files=PackedStringArray($FILES)
include_filter=""
exclude_filter=""
export_path=""
patches=PackedStringArray()
encryption_include_filters=""
encryption_exclude_filters=""
seed=0
encrypt_pck=false
encrypt_directory=false
script_export_mode=2
[preset.0.options]
custom_template/debug=""
custom_template/release=""
debug/export_console_wrapper=1
binary_format/embed_pck=false
texture_format/s3tc_bptc=true
texture_format/etc2_astc=false
shader_baker/enabled=false
binary_format/architecture="x86_64"
ssh_remote_deploy/enabled=false
ssh_remote_deploy/host="user@host_ip"
ssh_remote_deploy/port="22"
ssh_remote_deploy/extra_args_ssh=""
ssh_remote_deploy/extra_args_scp=""
ssh_remote_deploy/run_script="#!/usr/bin/env bash\nexport DISPLAY=:0\nunzip -o -q \"{temp_dir}/{archive_name}\" -d \"{temp_dir}\"\n\"{temp_dir}/{exe_name}\" {cmd_args}"
ssh_remote_deploy/cleanup_script="#!/usr/bin/env bash\npkill -x -f \"{temp_dir}/{exe_name} {cmd_args}\"\nrm -rf \"{temp_dir}\""
dotnet/include_scripts_content=false
dotnet/include_debug_symbols=true
dotnet/embed_build_outputs=false
EOF

echo "export_presets.cfg regenerated with $(echo $FILES | tr ',' '\n' | wc -l) files"
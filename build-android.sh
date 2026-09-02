#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")" && pwd)"
UNITY="${UNITY:-}"
if [[ -z "$UNITY" ]]; then
  for p in /Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity /opt/unity/Editor/Unity "$HOME"/Unity/Hub/Editor/*/Editor/Unity; do
    [[ -x "$p" ]] && UNITY="$p" && break
  done
fi
[[ -n "$UNITY" ]] || { echo "Không tìm thấy Unity Editor. Cài Unity 6 + Android Build Support."; exit 2; }
mkdir -p "$ROOT/Builds"
"$UNITY" -batchmode -quit -projectPath "$ROOT" -executeMethod VuonVietKyThu.Editor.BuildAndroid.PerformBuild -logFile "$ROOT/Builds/unity-build.log"
echo "APK: $ROOT/Builds/VuonVietKyThu-2.0.2-build-ready.apk"

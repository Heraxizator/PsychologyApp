#!/usr/bin/env bash
# Installs the Release package on the running emulator/device, starts it, drives it with random input and fails on a native crash,
# an unhandled exception or a cold start slower than MAX_START_MS. Needs adb on PATH. Usage: tools/android-smoke.sh <apk-dir> [package]
set -u
APK_DIR="${1:?apk directory}"
PKG="${2:-com.subconscious.psychologyapp}"
MAX_START_MS="${MAX_START_MS:-8000}"

APK=$(ls "$APK_DIR"/*-Signed.apk 2>/dev/null | head -1)
[ -n "$APK" ] || APK=$(ls "$APK_DIR"/*.apk | head -1)
echo "installing $APK"
adb install -r "$APK" || exit 1

adb logcat -c
COMPONENT=$(adb shell cmd package resolve-activity --brief "$PKG" | tail -1 | tr -d '\r')
echo "starting $COMPONENT"
adb shell am start -W -n "$COMPONENT" | tee start.txt

TOTAL=$(grep -oE "TotalTime: [0-9]+" start.txt | grep -oE "[0-9]+")
echo "cold start: ${TOTAL:-unknown} ms (limit $MAX_START_MS)"

adb shell monkey -p "$PKG" --throttle 300 --pct-syskeys 0 -s 7 2000 > monkey.txt 2>&1
adb logcat -d > logcat.txt

STATUS=0
if [ -z "$TOTAL" ] || [ "$TOTAL" -ge "$MAX_START_MS" ]; then
  echo "FAIL: cold start missing or slower than ${MAX_START_MS} ms"; STATUS=1
fi
if grep -E "Fatal signal|FATAL EXCEPTION|plt_entry" logcat.txt; then
  echo "FAIL: crash in the log"; STATUS=1
fi
if ! adb shell pidof "$PKG" > /dev/null; then
  echo "FAIL: the app is not running at the end"; STATUS=1
fi
[ "$STATUS" -eq 0 ] && echo "smoke test passed"
exit "$STATUS"

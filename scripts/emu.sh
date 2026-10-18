#!/usr/bin/env bash
# Запускает AVD в фоне и ждёт окончания загрузки Android.
set -euo pipefail
cd "$(dirname "$0")/.."
source scripts/env.sh

running=$(adb devices | awk '/^emulator-/ {print $1; exit}')
if [[ -n "$running" ]]; then
  echo "Эмулятор уже запущен: $running"
  exit 0
fi

log_file="${TMPDIR:-${TEMP:-/tmp}}/psychologyapp-emulator.log"
echo "Запускаю $AVD_NAME (лог: $log_file)"
nohup emulator -avd "$AVD_NAME" -no-snapshot-save -no-boot-anim >"$log_file" 2>&1 &

adb wait-for-device
for _ in $(seq 1 90); do
  if [[ "$(adb shell getprop sys.boot_completed 2>/dev/null | tr -d '\r')" == "1" ]]; then
    echo "Эмулятор готов"
    exit 0
  fi
  sleep 2
done

echo "Эмулятор не загрузился за 3 минуты, см. $log_file" >&2
exit 1

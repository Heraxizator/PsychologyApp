#!/usr/bin/env bash
# Проверяет всё, что нужно для сборки и запуска на эмуляторе, и подсказывает, что сделать.
set -uo pipefail
cd "$(dirname "$0")/.."
source scripts/env.sh

failures=0
ok() { printf '  \033[32m✓\033[0m %s\n' "$1"; }
fail() {
  printf '  \033[31m✗\033[0m %s\n    → %s\n' "$1" "$2"
  failures=$((failures + 1))
}
check() {
  local title=$1 hint=$2
  shift 2
  if "$@" >/dev/null 2>&1; then ok "$title"; else fail "$title" "$hint"; fi
}
dotnet_at_least() {
  local version
  version=$(dotnet --version 2>/dev/null) || return 1
  [[ ${version%%.*} -ge $1 ]]
}
android_workload() { dotnet workload list 2>/dev/null | grep -qE '^(android|maui-android|maui) '; }
java_at_least() {
  local version
  version=$("$JAVA_HOME/bin/java" -version 2>&1 | awk -F'"' '/version/ {print $2}')
  [[ ${version%%.*} -ge $1 ]]
}
accelerated() {
  if is_windows; then emulator -accel-check 2>&1 | grep -q "usable"; else test -r /dev/kvm -a -w /dev/kvm; fi
}
avd_exists() { emulator -list-avds 2>/dev/null | tr -d '\r' | grep -qx "$AVD_NAME"; }

echo "Окружение Android (ANDROID_HOME=$ANDROID_HOME, JAVA_HOME=${JAVA_HOME:-не задан})"
check ".NET SDK >= 10" "установите .NET 10 SDK" dotnet_at_least 10
check "Рабочая нагрузка android" "dotnet workload install maui-android" android_workload
check "JDK >= 17" "установите Microsoft OpenJDK 17+ или задайте PSYCHOLOGY_JAVA_HOME" java_at_least 17
check "platform-tools (adb)" "bash scripts/android-setup.sh" test -d "$ANDROID_HOME/platform-tools"
check "emulator" "bash scripts/android-setup.sh" test -d "$ANDROID_HOME/emulator"
check "platforms;android-36" "bash scripts/android-setup.sh" test -d "$ANDROID_HOME/platforms/android-36"
check "build-tools;36.0.0" "bash scripts/android-setup.sh" test -d "$ANDROID_HOME/build-tools/36.0.0"
check "cmdline-tools" "bash scripts/android-setup.sh" test -d "$ANDROID_HOME/cmdline-tools/latest"
check "$SYSTEM_IMAGE" "bash scripts/android-setup.sh" test -d "$ANDROID_HOME/system-images/android-35/google_apis/x86_64"
if is_windows; then
  check "Ускорение эмулятора (WHPX)" "включите «Платформа низкоуровневой оболочки Windows» в компонентах Windows" accelerated
else
  check "KVM доступен" "добавьте пользователя в группу kvm и перелогиньтесь" accelerated
fi
check "AVD $AVD_NAME" "bash scripts/android-setup.sh" avd_exists

if ((failures > 0)); then
  echo "Проблем: $failures"
  exit 1
fi
echo "Всё готово: bash scripts/emu.sh, затем bash scripts/android-run.sh"

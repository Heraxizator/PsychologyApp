#!/usr/bin/env bash
# Ставит недостающие компоненты Android SDK в каталог пользователя и создаёт AVD. Повторный запуск безопасен.
set -euo pipefail
cd "$(dirname "$0")/.."
source scripts/env.sh

mkdir -p "$ANDROID_HOME"
sdk_root="--sdk_root=$(native_path "$ANDROID_HOME")"
# Лицензии Android SDK принимаются здесь же: без этого sdkmanager ничего не ставит.
yes 2>/dev/null | sdk_tool sdkmanager "$sdk_root" --licenses >/dev/null || true
sdk_tool sdkmanager "$sdk_root" --install "${SDK_PACKAGES[@]}"

if emulator -list-avds 2>/dev/null | tr -d '\r' | grep -qx "$AVD_NAME"; then
  echo "AVD $AVD_NAME уже существует"
else
  echo "no" | sdk_tool avdmanager create avd --name "$AVD_NAME" --package "$SYSTEM_IMAGE" --device "pixel_7"
fi

echo "Готово. Проверка: bash scripts/android-doctor.sh"

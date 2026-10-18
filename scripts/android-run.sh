#!/usr/bin/env bash
# Собирает отладочную копию и запускает её на эмуляторе или телефоне (ANDROID_SERIAL — выбрать устройство).
#
# Ставится только копия com.subconscious.psychologyapp.dev («Psychology DEV»): она живёт рядом с настоящим
# приложением, со своей базой. Настоящий пакет скрипт не ставит никогда: на телефоне разработчика в нём личные записи,
# а пакет с другой подписью Android заменить не даст или потребует удалить вместе с данными.
set -euo pipefail
cd "$(dirname "$0")/.."
source scripts/env.sh

target=$(target_device)
if [[ -z "$target" ]]; then
  echo "Нет подключённого устройства. Запустите: bash scripts/emu.sh" >&2
  exit 1
fi

apk_dir="PsychologyApp.Presentation/bin/Debug/$FRAMEWORK"
apk="$apk_dir/$DEV_APP_ID-Signed.apk"
mapfile -t common < <(android_build_args)
build() {
  rm -f "$apk_dir"/*.apk
  dotnet build "$PROJECT" -f "$FRAMEWORK" -c Debug "${common[@]}" \
    "-p:ApplicationId=$DEV_APP_ID" "-p:ApplicationTitle=Psychology DEV" "$@"
}

build
# Инкрементальная сборка может оставить манифест прошлой сборки с настоящим id: тогда пересобираем приложение заново.
if [[ "$(package_of "$apk")" != "$DEV_APP_ID" ]]; then
  echo "В пакете не $DEV_APP_ID — пересобираю приложение"
  build -t:Rebuild --no-dependencies
fi

package=$(package_of "$apk")
if [[ "$package" != "$DEV_APP_ID" ]]; then
  echo "Отказ: собран пакет '$package' вместо $DEV_APP_ID, ставить его нельзя" >&2
  exit 1
fi

adb -s "$target" install -r "$apk"
activity=$(adb -s "$target" shell cmd package resolve-activity --brief "$DEV_APP_ID" | tail -1 | tr -d '\r')
adb -s "$target" shell am start -n "$activity" >/dev/null
echo "Запущено на $target: $DEV_APP_ID"

#!/usr/bin/env bash
# Релизная сборка для пользователей: AAB для Google Play и RuStore и APK для установки напрямую.
#
# Подпись — ключом выпуска: PsychologyApp.Presentation/AndroidSigning.local.props (образец — *.example рядом, файл
# в .gitignore) или переменные окружения AndroidSigningKeyStore, AndroidSigningKeyAlias, AndroidSigningStorePass,
# AndroidSigningKeyPass. Ключ хранит ответственный за выпуск: без того же ключа установленные приложения не обновятся.
#
# Перед выпуском: поднимите ApplicationDisplayVersion и ApplicationVersion в csproj (2.008 и 2008) и перенесите
# записи из [Unreleased] в раздел [2.008] CHANGELOG.md.
set -euo pipefail
cd "$(dirname "$0")/.."
source scripts/env.sh

die() {
  echo "Отказ: $*" >&2
  exit 1
}

# Релиз собирается только из закоммиченного кода: иначе по коммиту нельзя восстановить, что получили пользователи.
if [[ -n "$(git status --porcelain)" && "${ALLOW_DIRTY:-}" != "1" ]]; then
  die "есть незакоммиченные изменения (git status). Только для пробной сборки: ALLOW_DIRTY=1"
fi

display=$(csproj_property ApplicationDisplayVersion)
code=$(csproj_property ApplicationVersion)
# Схема проекта: версия 2.007 — код 2007. Расхождение значит, что одно из двух забыли поднять.
[[ "$code" == "$((10#${display//./}))" ]] || die "ApplicationVersion $code не соответствует ApplicationDisplayVersion $display"
newest=$(sed -n 's/^## \[\([0-9][^]]*\)\].*/\1/p' CHANGELOG.md | head -1)
[[ "$newest" == "$display" ]] || die "верхний раздел CHANGELOG.md — [$newest], а собирается $display: перенесите [Unreleased] в [$display]"

signing_props="PsychologyApp.Presentation/AndroidSigning.local.props"
if [[ -f "$signing_props" ]]; then
  keystore=$(sed -n 's:.*<AndroidSigningKeyStore>\(.*\)</AndroidSigningKeyStore>.*:\1:p' "$signing_props")
else
  for name in AndroidSigningKeyStore AndroidSigningKeyAlias AndroidSigningStorePass AndroidSigningKeyPass; do
    [[ -n "${!name:-}" ]] || die "не задан $name и нет $signing_props (см. начало скрипта)"
  done
  keystore=$AndroidSigningKeyStore
fi
[[ -n "$keystore" && -f "$(is_windows && cygpath -u "$keystore" || echo "$keystore")" ]] || die "нет файла ключа '$keystore'"

# Те же проверки, что перед коммитом: в релиз не попадает код с падающими тестами.
for tests in "${TEST_PROJECTS[@]}"; do
  dotnet test "$tests" --nologo -v quiet || die "не прошли тесты $tests"
done

out="artifacts/release/$display"
rm -rf "$out"
# Выпуск собирается с нуля: промежуточные файлы прошлых сборок однажды уже дали пакет, падающий при первом запуске
# (устаревшие номера ресурсов, см. drop_stale_abi_intermediates).
rm -rf "$(dirname "$PROJECT")/obj/Release" "$(dirname "$PROJECT")/bin/Release"
mapfile -t common < <(android_build_args)
dotnet publish "$PROJECT" -f "$FRAMEWORK" -c Release "${common[@]}" \
  "-p:AndroidPackageFormats=aab%3Bapk" -o "$out"

apk=$(ls "$out"/*-Signed.apk | head -1)
aab=$(ls "$out"/*-Signed.aab | head -1)
[[ "$(package_of "$apk")" == "$APP_ID" ]] || die "в APK пакет '$(package_of "$apk")' вместо $APP_ID"
# Пакет, подписанный отладочным ключом, нельзя выпускать: магазин его не примет, а поверх установленного он не встанет.
certs=$(MSYS_NO_PATHCONV=1 "$(apksigner_tool)" verify --print-certs "$(native_path "$apk")") || die "подпись APK не проверяется"
grep -q "CN=Android Debug" <<<"$certs" && die "APK подписан отладочным ключом"

echo "Версия $display ($code)"
echo "Google Play / RuStore (AAB): $(pwd)/$aab"
echo "APK для установки напрямую: $(pwd)/$apk"

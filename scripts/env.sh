#!/usr/bin/env bash
# Окружение Android-сборки. Подключается из остальных скриптов: source scripts/env.sh
# Работает в Git Bash на Windows и в bash на Linux/macOS.

# Пути вида /sdcard в командах adb не должны превращаться в C:/Program Files/Git/sdcard.
export MSYS_NO_PATHCONV=1

is_windows() { [[ "${OSTYPE:-}" == msys* || "${OSTYPE:-}" == cygwin* ]]; }

# Путь для dotnet и Windows-программ: C:\..., на остальных системах — как есть.
native_path() { if is_windows; then cygpath -w "$1"; else printf '%s' "$1"; fi; }

# Один SDK для сборки и эмулятора, в каталоге пользователя: SDK Visual Studio лежит в Program Files и без прав
# администратора в него нельзя ничего доставить.
if [[ -z "${ANDROID_HOME:-}" ]]; then
  if is_windows; then ANDROID_HOME="$(cygpath -u "$LOCALAPPDATA")/Android/Sdk"; else ANDROID_HOME="$HOME/Android/Sdk"; fi
fi
export ANDROID_HOME
export ANDROID_SDK_ROOT="$ANDROID_HOME"

# JDK 17+ для .NET Android; PSYCHOLOGY_JAVA_HOME позволяет указать другой.
if [[ -n "${PSYCHOLOGY_JAVA_HOME:-}" ]]; then
  JAVA_HOME="$PSYCHOLOGY_JAVA_HOME"
elif [[ -z "${JAVA_HOME:-}" ]] && is_windows; then
  JAVA_HOME=$(ls -d "/c/Program Files/Microsoft/jdk-"* 2>/dev/null | sort -V | tail -1)
fi
[[ -n "${JAVA_HOME:-}" ]] && JAVA_HOME=$(is_windows && cygpath -u "$JAVA_HOME" || printf '%s' "$JAVA_HOME")
JAVA_HOME=${JAVA_HOME%/}
export JAVA_HOME
export PATH="${JAVA_HOME:+$JAVA_HOME/bin:}$ANDROID_HOME/platform-tools:$ANDROID_HOME/emulator:$ANDROID_HOME/cmdline-tools/latest/bin:$PATH"

export AVD_NAME="${AVD:-Pixel_API35}"
export SYSTEM_IMAGE="system-images;android-35;google_apis;x86_64"
export SDK_PACKAGES=("platform-tools" "emulator" "platforms;android-36" "build-tools;36.0.0" "cmdline-tools;latest" "$SYSTEM_IMAGE")

export PROJECT="PsychologyApp.Presentation/PsychologyApp.Presentation.csproj"
export FRAMEWORK="net10.0-android"
export APP_ID="com.subconscious.psychologyapp"
# Отладочная копия ставится рядом с настоящим приложением и никогда не трогает его данные.
export DEV_APP_ID="$APP_ID.dev"
export TEST_PROJECTS=(
  PsychologyApp.Domain.Tests
  PsychologyApp.Application.Tests
  PsychologyApp.Infrastructure.Tests
  PsychologyApp.Integration.Tests
  PsychologyApp.Presentation.Tests
)

# Свойства MSBuild, общие для всех Android-сборок.
android_build_args() {
  printf '%s\n' "-p:AndroidSdkDirectory=$(native_path "$ANDROID_HOME")"
  [[ -n "${JAVA_HOME:-}" ]] && printf '%s\n' "-p:JavaSdkDirectory=$(native_path "$JAVA_HOME")"
}

# sdkmanager и avdmanager на Windows — .bat; bash не запускает их напрямую, если в пути есть пробелы.
sdk_tool() {
  local tool=$1
  shift
  local candidates=("$ANDROID_HOME/cmdline-tools/latest/bin/$tool")
  # Первый запуск: cmdline-tools ещё нет в SDK пользователя, берём их из Visual Studio.
  candidates+=("/d/Program Files (x86)/Microsoft Visual Studio/Shared/Android/android-sdk/cmdline-tools/latest/bin/$tool"
    "/c/Program Files (x86)/Android/android-sdk/cmdline-tools/latest/bin/$tool")
  local path
  for path in "${candidates[@]}"; do
    if is_windows && [[ -f "$path.bat" ]]; then
      JAVA_HOME="$(native_path "$JAVA_HOME")" cmd /c "$(cygpath -w "$path.bat")" "$@"
      return
    elif [[ -x "$path" ]]; then
      "$path" "$@"
      return
    fi
  done
  echo "Нет $tool: установите Android cmdline-tools (Android Studio или Visual Studio) либо задайте ANDROID_HOME" >&2
  return 1
}

aapt_tool() { ls "$ANDROID_HOME"/build-tools/*/aapt* 2>/dev/null | grep -E '/aapt(\.exe)?$' | sort -V | tail -1; }
apksigner_tool() {
  local dir
  dir=$(ls -d "$ANDROID_HOME"/build-tools/* 2>/dev/null | sort -V | tail -1)
  if is_windows; then echo "$dir/apksigner.bat"; else echo "$dir/apksigner"; fi
}

package_of() { "$(aapt_tool)" dump badging "$1" 2>/dev/null | sed -n "s/^package: name='\([^']*\)'.*/\1/p"; }

# Устройство для установки: ANDROID_SERIAL или первое подключённое.
target_device() { echo "${ANDROID_SERIAL:-$(adb devices | awk 'NR > 1 && $2 == "device" {print $1; exit}')}"; }

# Значение свойства из csproj, например csproj_property ApplicationVersion.
csproj_property() { sed -n "s:.*<$1>\(.*\)</$1>.*:\1:p" "$PROJECT" | head -1; }

# Скриншоты для магазинов

Готовые слайды лежат в `store/screenshots/ru` и `store/screenshots/en`: восемь слайдов 1440×2560 (9:16 подходит для
Google Play и RuStore; Google Play берёт до восьми) и `feature-graphic.png` 1024×500 для Google Play.

Подписи обещают только то, что видно на снимке. Все данные на снимках вымышленные.

## Как переснять

1. Соберите отдельную отладочную копию со своей базой. Ни `.dev`, ни настоящее приложение не затрагиваются:

   ```bash
   source scripts/env.sh; mapfile -t common < <(android_build_args)
   dotnet build "$PROJECT" -f "$FRAMEWORK" -c Debug "${common[@]}" -p:ApplicationId=com.subconscious.psychologyapp.store -p:AndroidDevRid=android-x64 -o out/store
   adb install -r out/store/com.subconscious.psychologyapp.store-Signed.apk
   ```

2. Создайте демо-данные в часовом поясе телефона (у эмулятора это UTC) и положите их в каталог приложения. Debug-сборка
   импортирует файл при запуске (`DemoDataImporter`):

   ```bash
   dotnet run --project tools/PsychologyApp.StoreAssets -- demo demo-ru.json tz=UTC        # или: ... demo-en.json en tz=UTC
   adb push demo-ru.json /data/local/tmp/demo-backup.json
   adb shell run-as com.subconscious.psychologyapp.store mkdir -p files
   adb shell "cat /data/local/tmp/demo-backup.json | run-as com.subconscious.psychologyapp.store sh -c 'cat > files/demo-backup.json'"
   ```

3. Запустите копию (если при первом запуске импорт не прошёл — база ещё не создана, — перезапустите её), пройдите знакомство и выберите язык в настройках. Проверку безопасности закройте кнопкой «Назад», не
   отвечая на вопросы. Снимите экраны (`adb exec-out screencap -p > raw/ru/chat.png`) под именами из `SLIDES` в
   `compose.mjs`:

   | файл | экран |
   | --- | --- |
   | `chat.png` | ИИ-психолог, разговор «Перед презентацией», прокрученный так, чтобы сообщение человека было видно целиком |
   | `breathing.png` | «Квадратное дыхание» → «Пропустить» → «Начать», идёт вдох (не на весь экран: тёмный экран выпадает из светлой ленты) |
   | `practices.png` | вкладка «Практики» |
   | `insights.png` | меню → «Что мне помогает» |
   | `journal.png` | вкладка «Дневник» после выбора 🙂 |
   | `chart.png` | Дневник → карточка недели («Обзор») → «30 дней», без прокрутки |
   | `anchors.png` | меню → «Опорные мысли» |
   | `tests.png` | вкладка «Тесты» |

4. Соберите слайды (нужны Node и Microsoft Edge):

   ```bash
   node tools/store-screenshots/compose.mjs raw/ru store/screenshots/ru ru
   node tools/store-screenshots/compose.mjs raw/en store/screenshots/en en
   ```

Для App Store эти слайды не подходят: там нужны снимки iOS-сборки и размер 1290×2796.

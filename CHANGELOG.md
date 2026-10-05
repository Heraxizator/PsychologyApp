# Changelog

All notable changes to this project are documented in this file.

This project follows a Keep a Changelog style and Semantic Versioning principles for release notes.

## [Unreleased]

### Added
- Offline chat companion (messenger UI): persistent chats, a chat list with swipe actions, a prominent card on the home screen, a typing indicator, day dividers and grouped bubbles. The dialogue is a deterministic, on-device engine; there is no language model and no network call.
- Conversation skills of the companion: capital letters at the start of every sentence, psychoeducation answers ("what is CBT / panic", "why do I feel this", "is this normal", "what helps"), backchannels, laughter and emoji, good news, "how are you", "what can you do", "say that again", "skip this question", typed 0-10 ratings, greeting by time of day, no repeated replies, no parroting of one-sentence messages, and notice of a change of feeling.
- Memory between chats (schema version 10, table `ChatMemory`): the person's name and which practices lowered the tension. The companion addresses the person by name sparingly and offers the practice that helped before.
- Companion profile: tap the face in the chat header to see a drawn, blinking avatar, level of trust, the person's name (editable), statistics (chats, messages, days together, streak), tension over time, what helps, what they talk about most, insights, an honest description of what the companion is, and controls to forget the name or delete all chats.
- Animations: new messages slide in, suggestion chips appear in turn, the send button pulses, chat-list rows reveal in turn, profile numbers count up, bars grow. All honour reduced motion.
- `docs/chat-companion.md` describes the engine, the state, the memory and its limits.

### Quality
- Texts: 685 of the `AppStrings` texts live in `StringsRu.resx` and `StringsEn.resx` (`PsychologyApp.Presentation.Core/Common`); the `AppStrings` API is unchanged. A language is added by adding a `StringsXx.resx` and one `ResourceManager`; a test checks every text exists and is not empty in both languages. Texts with parameters (interpolation, switch) stay in code. `tools/Extract-Strings.ps1` is the one-off migration script.
- `ShellStartupCoordinator`, `DataBackupViewModel` and `StartPhysicsViewModel` take `IUserPreferencesStore` instead of the static `UserPreferences`.
- Mutation testing of the clinical logic: crisis detector and self-harm screening 93%, risk classifier and score calculators 87% (weekly workflow; the build breaks under 50%).
- `SafeAsync.Run` replaces every `async void` handler; `BestEffort.Report` logs the 13 formerly silent "optional" catch blocks; code style is enforced in the build.
- Tests: property-based tests of the chat companion (FsCheck), boundary tests for every questionnaire cut-off (Domain mutation score 58.6% -> 72.6%), upgrade-from-version-1 and idempotent-schema tests; per-layer coverage gates in CI (Application/Domain 80, Infrastructure 75, overall 70).
- CI: warnings are errors, Dependabot, gitleaks and dependency audit (`security.yml`), weekly mutation run (`mutation.yml`), Release emulator smoke test (`android-smoke.yml`, manual/weekly).
- `FireAndForget` records where a background task was started and ignores cancellations; `docs/threat-model.md` states what is and is not protected (the database is not encrypted at rest).

### Fixed
- Reminder taps: the notifications' tap intents carry a random per-install secret and MainActivity (exported as the launcher) accepts a reminder action only with it; package name and action alone, which any app can set, no longer count.
- Quotes are unique by text (schema version 14: duplicates removed, unique index, `INSERT OR IGNORE`), so concurrent seedings cannot insert the same quote twice.
- Release builds now keep a local error log (warnings and errors only, `logs/app-errors.log`, capped at 2 MB; crashes are written straight to disk). Before, Release had no log sink and every `LogError` vanished. The data screen has "Share error log" (the file holds technical errors, not entries; nothing is sent automatically).
- Global error handling: raw exception messages (English, about SQL or entities) are no longer shown to the person; cancellations are not reported; an unobserved background task is logged instead of raising a red toast; a terminating crash is written to disk instead of opening a dialog nobody will see.
- Reminders use inexact alarms (`SetAndAllowWhileIdle`) and the `SCHEDULE_EXACT_ALARM` permission is gone from the manifest: a daily reminder does not need exact delivery and Google Play reserves that permission for alarm/calendar apps.
- Page-visit statistics (written on every screen view, read by nothing but a distinct-page count) are pruned at startup to 90 days; the latest row of every page is kept.
- Measured and not adopted: a minimum confidence for naming a feeling in the chat (a single clear keyword and an accidental match both score 0.5; see docs/companion-understanding.md).
- Safety: an answer other than "never" to the suicide question of the Beck inventory (item 9) or the PHQ-9 (item 9) is now a risk signal on its own: it is recorded like the risk check (so the startup gate and the dashboard react) and the crisis screen opens right after the result, whatever the total.
- Beck inventory scoring takes the highest ticked statement of each group (the standard rule) instead of summing every ticked statement, so ticking several statements no longer inflates the total and the severity band.
- Chat turns are saved atomically: memory counters, the messages and the chat's state/title go to the database in one transaction (`IChatRepository.SaveTurnAsync`), and turns of one chat run one at a time, so a crash or a double send can no longer leave messages without their state or lose a turn; starting a chat from several taps makes one chat.
- Prayer audio cache: HTTPS only, the content type and the first bytes must be audio (an error page answered with 200 is no longer kept as a track), one shared download per track instead of two writers on one file, a bad file is dropped from the cache when it will not play, the folder is capped at 200 MB (least recently used first), and the player stops waiting for a track after 20 s.
- Startup: a failure while seeding quotes (any exception, or the startup deadline) no longer aborts the start.
- Alice: signing in works (the whole Yandex family over HTTPS is allowed, not only alice.yandex.ru).
- Search in quotes and in the body/psychosomatics catalogue ignores case and "ё" and finds other word forms ("тревога" finds "тревоги").
- Chat: links, e-mail addresses and mixed-case names (iPhone, eBay) are no longer capitalised when stored.
- Feedback form: with no developer address configured it said support would receive the message while only opening the share sheet; it now says so and the button reads "Share". Put an address in `AppSettings.ReviewEmailAddress` to send by e-mail.
- Chat companion: input with nothing to understand is no longer answered as a heartfelt story ("1111" used to get "Thank you for sharing, this seems important to you" and move the dialogue on). Bare digits, key mashing, repeated letters, "???" and "!!!" are recognised; Russian typed on the English layout ("ghbdtn") is read as Russian; one or two empty words and hesitation ("ну", "ммм") get an honest reply; the tension question explains its scale when answered with junk. Noise does not count as a turn and does not change what the companion knows.
- Double taps: a page no longer opens twice. `NavigationCoordinator` used to *queue* a second push of the same destination behind the first (so it opened again 350 ms later); it now drops it while the first is opening and for 600 ms after. Commands that start async work (`AsyncCommand<T>` for mood saves, journal share/select, finishing a paper-list session) ignore a second tap while running, instant steps (onboarding Next/Back, technique wizard steps, test answers, Luscher colour pick, option views) are debounced, and only one dialog or sheet can be on screen at a time. Verified on a Samsung A25: a quick double tap on onboarding "Next" skipped a step before the change and moves one step after it; a double tap on a technique card opens it once.
- Release builds no longer abort at random (SIGABRT, `aot-runtime.c: condition plt_entry not met`, on the .NET Timer / TP Gate threads) a few seconds to a minute after opening Prayers or Quotes: profiled AOT (`AndroidEnableProfiledAot`) is switched off, plain AOT stays. Verified on a Samsung A25 (Android 14): the profiled build crashed three times in 2.5 minutes, the plain-AOT build ran over five minutes under load (tabs, scrolling, prayer playback) without a crash; cold start is unchanged (about 1.6 s) and the package grows by about 9 MB.
- Localization tests that change the static language now run in the `Localization` collection, which removes a source of intermittent failures.
- Backup restore: it now runs in a single transaction (all or nothing), skips rows the database already has (importing a file twice changes nothing), keeps the original dates of session results instead of stamping them with the import moment, and no longer creates extra "completed today" practices that inflated counters and the streak. Format version 2 also carries chat memory, favourite quotes, custom techniques, the risk check, the therapy program and escalations; an unreadable, empty, oversized or newer file is rejected before anything is written. The temporary export file is deleted after sharing.
- Streaks now follow the person's own calendar day instead of the UTC date (a practice at 00:30 in Moscow or at 20:00 in New York no longer lands on the wrong day).
- A "red" risk check routes to the crisis hub only for a week; after that the app asks for a new check instead of opening the crisis hub on every launch, and a months-old "green" no longer hides a bad week in the scorecard.
- The weekly scorecard no longer writes a new escalation event every time a dashboard opens (one per kind per week), and the Amber "hold the week" write that was undone on the next read is gone.
- Crisis phrases: more Russian and English wordings (hanging, cutting, jumping, pills, "better off without me", "kms"); journal notes are now checked too and offer the crisis screen. The crisis screen fails closed when the risk state cannot be read.
- Helpline: 8-800-2000-122 is labelled as the children's, teenagers' and parents' helpline; 112 stays the emergency number and findahelpline.com is offered for adult lines.
- Database: every timestamp is written as UTC and read back as UTC; `datetime()` filters that bypassed indexes now compare the stored text directly; `PRAGMA foreign_keys=ON` plus a real `ChatMessages` -> `ChatSessions` foreign key (schema version 13); a database from a newer build is refused instead of being opened; Dapper calls that ignored their cancellation token now honour it; `ChatMemory` is part of the table drop list; the chat list uses one grouped query instead of three subqueries per chat; the deactivate-then-upsert of the therapy program is one transaction.
- The unused `TryProtectDatabaseFile` (it did nothing on Android/iOS and ran `File.Encrypt` on every connection open elsewhere) was removed; the database relies on the app sandbox and is not encrypted at rest.
- Unobserved `FireAndForget` failures with no handler wired are traced instead of being swallowed.
- The local model manifest is pinned to a Hugging Face commit and every file now has a SHA-256.
- Nullable warnings (CS8600/8601/8604/8618/8619/8625) are no longer suppressed in the Presentation project; the 56 real ones were fixed (constructors assign fields before their try block, two unused parameterless view-model constructors removed, dialog titles are `string?`).
- `_build_out/` is no longer tracked; CI also runs for `features/re-design` and quotes the `TargetFrameworks` override.

### Changed
- UI/usability pass over all non-chat screens: 44 dp back button with a screen-reader label, icon tiles on navigation rows (profile, options, crisis hub, practice completion), pinned primary actions on Settings, Feedback, Designer and Donate, a segmented yes/no control in the risk check, pill-sized retake/history actions on test cards, clear button and `Done` key on text fields, left-aligned body text instead of justified, and test history rendered without a nested `CollectionView`.
- Second UI pass: colored profile hero with stats, restructured quote cards (theme pill and favourite on top, author with copy/share below), hero intro and pinned Start button on the somatic screen, icon tiles on test cards, pinned nav bar on theory and created-technique screens, and a lighter pinned action bar on the test result.
- Standardized release documentation and contribution guidelines for cleaner, more professional history going forward.

## [2.002] - 2026-08-14

### Added
- Added `PracticeClinicalDashboardEnricher` and dedicated tests for clinical dashboard enrichment paths.
- Added auto-save behavior in settings with debounce while keeping explicit manual save.
- Added centralized secondary-page tab bar hiding through navigation flow.

### Changed
- Moved and simplified practice dashboard orchestration to reduce thin pass-through layers.
- Improved journal factor chip presentation and note formatting for better readability.
- Updated startup and shell behaviors for safer clinical gate handling and better theme/tab synchronization.
- Aligned bootstrap/service registration so integration/bootstrap paths resolve technique catalog services.
- Updated long-form README with current stack markers where needed.

### Security
- Hardened local SQLite file handling with best-effort OS-level file protection and stricter app data path usage.

### Fixed
- Fixed fail-open behavior in clinical startup gate by switching to explicit fallback behavior on gate failures.
- Fixed silent clinical UI degradation by showing fallback risk status when enrichment fails.
- Fixed mismatch between root-tab and pushed-page tab visibility behavior.

## [2.001] - 2026-08-13

### Changed
- Deepened journal ritual flows and polished crisis/user safety paths.

### Fixed
- Improved note autosave reliability and clarified journal hub check-in behavior.

## [2.000] - 2026-08-12

### Changed
- Refined architecture boundaries by moving clinical and scoring rules further into Domain.


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

### Fixed
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


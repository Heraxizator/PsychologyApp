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
- Localization tests that change the static language now run in the `Localization` collection, which removes a source of intermittent failures.

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


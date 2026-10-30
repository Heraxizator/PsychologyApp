# Quality review — 2026-10-30

This is a cross-layer code and automated-test review, not a claim that every screen or device behaviour has been inspected.

## Findings fixed in this pass

- Journal CSV export left private notes in the cache after sharing. The temporary file is now removed in `finally`, including when sharing fails. A behaviour test executes the real exporter against the existing MAUI test doubles and checks both outcomes.
- A risk assessment dated in the future could suppress periodic checks until that date plus the check interval. Such a timestamp now requests another check. Red-risk routing is preserved. Tests cover both green and red assessments with a fixed clock.

The questionnaire's existing contract is retained: a direct invocation of Next can show a missing-answer hint even when advancing is unavailable.

All five test projects passed: Domain 326, Application 1286, Infrastructure 109, Integration 31, Presentation 878 (2630 total). Presentation includes the existing architecture, resource-parity, contrast and navigation tests. New regressions failed before their respective fixes.

## Review coverage

| Area | Evidence and limits |
| --- | --- |
| Domain | Read questionnaire scoring, trend comparison, streak and active-day calculations, mood reminder policy and risk classification; ran Domain tests. No clinical thresholds changed. |
| Application | Read assessment recency, periodic checks and crisis routing; ran Application tests. Earlier backup and crisis-detection fixes remain covered by their regressions. |
| Infrastructure | Ran repository, encryption, queue-ordering and database integration tests. Earlier migration and restore fixes remain covered. |
| Presentation | Inspected shared command execution, questionnaire validation, journal export, draft and chip storage, speech cancellation, audio download/cache handling, and representative accessibility descriptions. Ran Presentation tests, including architecture, resource parity, contrast and navigation checks. This is sampling, not a screen-by-screen UI audit. |
| Platform | Located Android SDK under Visual Studio on drive D. Android Debug build with an explicit SDK path and the `.dev` application id succeeded with zero warnings and errors. No connected device was found by `adb devices`. |

## Remaining verification

- Exercise onboarding, questionnaire completion, journal edits/export, chat, practices, data restore and erasure on a `.dev` installation.
- Inspect light and dark themes, large text, TalkBack labels/focus, and reduced motion on a device or emulator.
- Check offline dictation with cancellation and page changes on a supported device; unit tests do not establish recognizer behaviour.
- Verify exported files are readable by receiving Android apps, including delayed reads after the share chooser.
- Run Release smoke/AOT/trimming checks and measure performance on a matching ABI before release.
- Clinical texts and translations still require the reviews already called out in CHANGELOG; this code review does not replace them.

No release version was changed. Personal-data exports and tests use synthetic notes only.

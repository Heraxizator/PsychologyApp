# Threat model (one page)

What the app holds is sensitive: mood notes, chat with the companion, test answers (including depression and self-harm items), risk
assessments, a safety plan with contacts. Everything is stored **on the device only**; the app has no account and no server.

## Assets

| Asset | Where it lives |
| --- | --- |
| Chats, mood notes, test results, risk assessments, safety plan, journal | SQLite database in the app's private folder |
| Preferences (language, reminders, onboarding) | Android SharedPreferences |
| Exported backup / specialist summary | A file the person chooses to share; removed from the cache right after the share sheet closes |
| Error log | `logs/app-errors.log`, warnings and errors only, no message text; shared only by the person |
| Downloaded prayer audio | Cache folder, HTTPS only, validated as audio |

## Who could get at it, and how it is handled

| Threat | Handling | Residual risk |
| --- | --- | --- |
| Another app reads the database | Android app sandbox; `allowBackup=false`; cloud backup and device transfer excluded; the database is not encrypted at rest (SQLCipher is a planned hardening, not implemented) | Rooted device or a physical copy of the app data |
| Someone with the unlocked phone | Out of scope (no app lock yet) | Open: an optional PIN/biometric lock is the next step |
| Backup file leaks after sharing | Plain JSON by design (the person decides where it goes); deleted from the cache after sharing | The person's choice of destination |
| Forged intent to MainActivity | Reminder taps need a per-install secret carried only by our own PendingIntents | Navigation only, no data access |
| Malicious content over the network | The app only downloads audio, over HTTPS, size-capped, content-type and magic-byte checked; Alice runs in a WebView restricted to the Yandex family over HTTPS | Third-party site compromise |
| Tampered model download (dormant feature) | Pinned commit and SHA-256 for every file | Feature is not shipped |
| Crash / error log reveals data | Logs carry exception types and technical messages, never chat or journal text | Exception messages may include SQL fragments |
| Network eavesdropping | No traffic except audio and the optional Alice page | – |

## Safety (not security) risks the app handles

Crisis wording in chat and journal notes, the suicide items of the Beck inventory and PHQ-9, and a red risk check each route to the crisis
screen. These rules need review by a clinician before a public release; see the release notes in the README.

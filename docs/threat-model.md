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
| Another app, or a copy of the app data, reads the database | Android app sandbox; `allowBackup=false`; cloud backup and device transfer excluded; the file is encrypted at rest (SQLite3 Multiple Ciphers, ChaCha20-Poly1305) with a random 256-bit key kept in the Android Keystore / iOS Keychain via SecureStorage; an older plain database is converted on first launch, verified row by row, and the plain copy deleted | A rooted device with live access to the unlocked app; loss of the key (clearing Keystore data) makes the file unreadable, there is no recovery by design |
| Someone with the unlocked phone | Out of scope (no app lock yet) | Open: an optional PIN/biometric lock is the next step |
| Backup file leaks after sharing | The export offers a passphrase (PBKDF2-SHA256 600k iterations, AES-256-GCM, authenticated); the unprotected option stays available; the temporary file is deleted from the cache after sharing | A weak passphrase; an unprotected export sent somewhere unsafe |
| Forged intent to MainActivity | Reminder taps need a per-install secret carried only by our own PendingIntents | Navigation only, no data access |
| Malicious content over the network | The app only downloads audio, over HTTPS, size-capped, content-type and magic-byte checked; Alice runs in a WebView restricted to the Yandex family over HTTPS | Third-party site compromise |
| Tampered model download (dormant feature) | Pinned commit and SHA-256 for every file | Feature is not shipped |
| Crash / error log reveals data | Logs carry exception types and technical messages, never chat or journal text | Exception messages may include SQL fragments |
| Network eavesdropping | No traffic except audio and the optional Alice page | – |

## Safety (not security) risks the app handles

Crisis wording in chat and journal notes, the suicide items of the Beck inventory and PHQ-9, and a red risk check each route to the crisis
screen. These rules need review by a clinician before a public release; see the release notes in the README.

# Offline companion and the on-device LLM

The **Talk** card at the top of the practice list opens the companion: the person describes what is going on in free text,
the app recognises the state, answers empathically and launches a fitting practice as a dialogue.

Everything below runs on the device. The user's words are never sent anywhere.

## How the pieces fit

| Piece | Where | Role |
|-------|-------|------|
| `KeywordCrisisDetector` | `Application/Conversation` | Runs on every free-text answer **before** anything else. On a hit the model is never called and the crisis hub opens. |
| `LexiconSituationAnalyzer` | `Application/Conversation/Companion` | RU/EN weighted lexicon -> state (panic, anxiety, anger...), theme, body symptoms, intensity. |
| `TechniqueSuggester` | same | Closed, hand-reviewed state -> practices mapping. **The model never chooses what the app launches.** |
| `ILocalLanguageModel` | same | Port for an on-device model. Optional. |
| `CompanionPromptBuilder` | same | System prompt with guardrails + the last 6 turns. |
| `CompanionReplyGuard` | same | Drops replies with diagnoses, medication talk, lists, questions, URLs, wrong language, "as an AI". |
| `CompanionContent` | same | Scripted wording. Used when there is no model, on timeout, or when the guard rejects a reply. |
| `OnnxGenAiLanguageModel` | `Presentation/Platforms/Android` | ONNX Runtime GenAI implementation. Android only. |

Without a model the companion is fully functional on scripted replies. The model only rewrites the *reflection* sentence(s).

## Installing a model (developer / tester flow)

There is no in-app downloader yet. The runtime looks for a **GenAI-format model folder** (it contains `genai_config.json`,
`model.onnx`, `tokenizer.json`...) in either place:

1. `<app data dir>/llm/`
2. `<external files dir>/llm/` - reachable from a computer:

```bash
adb push <local-model-folder> /sdcard/Android/data/com.subconscious.psychologyapp/files/llm
```

Restart the app. If the folder is valid the companion starts using the model (the first reply after a cold start is slower
while the model loads; opening the Talk screen warms it up).

Requirements: Android 7.0 (API 24) or newer, arm64 device with enough free RAM. On older Android versions the companion silently stays on scripted replies.

### Candidate models (not evaluated by us - test Russian quality yourself)

- A ~1B instruct model in ONNX GenAI int4 (for example a Gemma 3 1B instruct build, roughly 0.8 GB): smallest, weakest Russian.
- A ~3B-4B instruct model in int4 (roughly 2-2.5 GB): noticeably better Russian, needs a recent phone.

Check the licence of whichever model you ship. Quality on emotionally sensitive Russian text is the main open question:
run real conversations before enabling it for users.

## Safety design (why it is built this way)

- Crisis detection is deterministic and precedes the model. The model never sees crisis text.
- The model cannot trigger actions; routing is rule-based.
- Every model reply is filtered; anything suspicious falls back to reviewed text.
- Timeout (25 s) and any exception fall back to scripted text; the conversation never breaks because of the model.
- Prompts forbid diagnoses, medication talk, advice and questions; the guard enforces the same after the fact.

## Costs and platform notes

- `Microsoft.ML.OnnxRuntimeGenAI` is referenced for Android only. Its `onnxruntime-genai.aar` alone is about 21.5 MB, plus the
  ONNX Runtime it depends on; measure the release APK before shipping.
- The libraries declare `minSdk 24`; the app still supports 21. `AndroidManifest.xml` uses `tools:overrideLibrary` and
  `OnnxGenAiLanguageModel` refuses to load below API 24. Raise the app's minSdk to 24 if you would rather drop that override.
- iOS / Mac Catalyst use `NullLanguageModel` (scripted replies).

## Not done yet

- In-app model download with consent, progress, resume and checksum.
- Streaming tokens into the chat bubble (currently the whole reply appears at once).
- Model quality evaluation set (Russian emotional-support dialogues) and a review by a psychologist.
- Conversation memory between sessions.

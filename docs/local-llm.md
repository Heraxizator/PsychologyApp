# Offline companion and the on-device LLM

The **Talk** card at the top of the practice list opens the companion: the person describes what is going on in free text,
the app recognises the state, answers empathically and launches a fitting practice as a dialogue.

Everything below runs on the device. The user's words are never sent anywhere. The only network access in this feature is the
model download the user starts explicitly.

## How the pieces fit

| Piece | Where | Role |
|-------|-------|------|
| `KeywordCrisisDetector` | `Application/Conversation` | Runs on every free-text answer **before** anything else. On a hit the model is never called and the crisis hub opens. |
| `LexiconSituationAnalyzer` | `Application/Conversation/Companion` | RU/EN weighted lexicon -> state (panic, anxiety, anger...), theme, body symptoms, intensity. Ties go to the state named first. |
| `TechniqueSuggester` | same | Closed, hand-reviewed state -> practices mapping. **The model never chooses what the app launches.** |
| `ILocalLanguageModel` | same | Port for an on-device model. Optional. |
| `CompanionPromptBuilder` | same | System prompt + two example exchanges + the last 6 turns. |
| `CompanionReplyGuard` | same | Drops replies with diagnoses, medication talk, advice, dismissive phrases, lists, questions, URLs, informal "ты", wrong language, "as an AI", or no link to what the person wrote. |
| `CompanionContent` | same | Scripted wording. Used when there is no model, on timeout, or when the guard rejects a reply. |
| `LocalModelCatalog` / `HttpLocalModelInstaller` | `Application` / `Infrastructure` | Consent-based, resumable, checksum-verified, atomic model download. |
| `OnnxGenAiLanguageModel` | `Presentation/Platforms/Android` | ONNX Runtime GenAI implementation. Android only. |
| `tools/PsychologyApp.LlmEval` | repo root | Runs the evaluation set through a real model with the app's exact prompt and guard. |

Without a model the companion is fully functional on scripted replies. The model only rewrites the *reflection* sentence(s).

## Current status: the model is English-only

`LocalModelManifest.Languages` lists the languages the model may be used for. It is currently `["en"]`:

- In Russian the companion **never** uses the model and the Settings section is hidden.
- In English the section offers the download and the companion uses the model when installed.

Reason: evaluation on a real model (below). Add `"ru"` only after a Russian-capable model passes the evaluation **and** a native-speaker / psychologist review.

## Evaluation (`tools/PsychologyApp.LlmEval`)

```bash
dotnet run -c Release --project tools/PsychologyApp.LlmEval -- <model-dir> report.md [--temperature 0.3]
```

It runs 24 realistic messages (20 Russian, 4 English, including crisis phrasings; `--limit N` and `--max-tokens N` shorten a run) and writes a markdown report with the analyzer's guess,
the raw model reply, whether the guard accepted it, and timing. Add your own cases to `prompts.json`.
The project is intentionally not in `PsychologyApp.sln` (it restores the native ONNX runtimes).

### Result: Gemma 3 1B instruct (int4, ONNX GenAI, ~0.8 GB), desktop CPU

| Check | Result |
|-------|--------|
| Crisis phrases caught (deterministic detector) | 3/3 (first run 2/3: "I don't want to live anymore" was missed; fixed and covered by tests) |
| Situation analyzer accuracy | 20/21 (first run 18/21; tie-breaking fixed) |
| Russian reply quality | **Not usable.** Ungrammatical or invented content, e.g. "Как вам не спокойно, когда чувствуете, что вас обокрали?", details the person never mentioned |
| English reply quality | Acceptable: short, on topic, no advice |
| Guard acceptance with the strict guard | 9/21; the guard removes unsafe text but cannot make bad Russian good |
| Speed | ~4-8 s per reply on a desktop CPU. **Phone speed was not measured** and will be slower |

### Result: Qwen3-4B (int4, ONNX GenAI, 3.8 GB, community conversion), desktop CPU

| Check | Result |
|-------|--------|
| Russian reply quality | Grammatical and mostly on topic, but not trustworthy: advice ("Просто держите себя в руках"), non-sequiturs ("Злость - это нормально, когда кто-то делает так, как хотите"), invented details, mixing "вы" and "ты", code-switching to English mid-sentence ("Гrief", "nobody") |
| Guard acceptance (first 8 messages, 70 new tokens) | 5/8; rejected replies were mostly correct rejections, accepted ones still contained flaws the guard could not detect |
| Speed | **~30 s per reply** of 70 tokens on a desktop CPU (plus 12-19 s to load). A phone will be several times slower |

Qwen3 also needs its reasoning mode disabled (`/no_think`); the engine does this and the guard strips any `<think>` remains.

### Conclusions

1. **1B models are unusable for Russian emotional support** (Gemma 3 1B).
2. **A 4B model is the first with acceptable Russian grammar, but it is too slow for an interactive chat** (tens of seconds on a desktop, likely minutes on a phone) **and still makes content mistakes** that a rule-based guard cannot catch. It needs 8+ GB RAM and a 4 GB download.
3. Therefore the model stays **English-only** and off by default. The Russian companion runs on scripted replies until a model clears *both* bars: acceptable speed on a real phone, and review of its Russian output by a native speaker / psychologist.
4. Options for a better Russian conversation: improve the scripted companion (more of the person's own words, follow-up questions, more varied reflective listening), an opt-in cloud mode with explicit consent (breaks the "fully offline" promise), or revisit on-device models when faster / better ones exist.
## Installing a model

### From the app (English only for now)

Settings -> **On-device AI** -> Download. The user confirms size and terms first. The download resumes after interruptions,
is verified (size + SHA-256) and installed atomically, so a half-downloaded model is never used. Requires Android 7.0+.

### Manually (developer flow)

The runtime looks for a GenAI-format model folder (`genai_config.json`, `model.onnx`, `tokenizer.json`...) in:

1. `<app data dir>/llm/`
2. `<external files dir>/llm/`, reachable from a computer:

```bash
adb push <local-model-folder> /sdcard/Android/data/com.subconscious.psychologyapp/files/llm
```

Manual installs bypass the language gate in Settings but **not** in the companion: Russian still uses scripted replies.

## Safety design (why it is built this way)

- Crisis detection is deterministic and precedes the model. The model never sees crisis text. Detector tolerates apostrophes, "ё", punctuation and loose wording ("не хочу уже совсем жить").
- The model cannot trigger actions; routing is rule-based.
- Every model reply is filtered; anything suspicious falls back to reviewed text.
- Timeout (25 s) and any exception fall back to scripted text; the conversation never breaks because of the model.
- Prompts forbid diagnoses, medication talk, advice and questions; the guard enforces the same after the fact.
- Known trade-off: the crisis detector favours false positives ("не хочу жить в этом городе" also triggers the help screen).

## Costs and platform notes

- `Microsoft.ML.OnnxRuntimeGenAI` is referenced for Android only. Its `onnxruntime-genai.aar` alone is about 21.5 MB, plus the ONNX Runtime it depends on; measure the release APK before shipping.
- The libraries declare `minSdk 24`; the app still supports 21. `AndroidManifest.xml` uses `tools:overrideLibrary` and `OnnxGenAiLanguageModel` refuses to load below API 24. Raise the app's minSdk to 24 if you would rather drop that override.
- iOS / Mac Catalyst use `NullLanguageModel` (scripted replies).

## Not done yet

- A Russian-capable model that passes evaluation, and a phone benchmark (speed, RAM, battery, thermal).
- Streaming tokens into the chat bubble (currently the whole reply appears at once).
- Review of the scripted wording and the crisis wording by a psychologist.
- Conversation memory between sessions.

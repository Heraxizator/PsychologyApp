# How the companion understands what people write

The messenger companion (`CompanionDialogue`) has to decide which state the person is in (panic, anxiety, anger, guilt...) from free text.
This page records how well that works, what was measured, and what was decided.

## What runs in the app today

A weighted **lexicon** (`LexiconSituationAnalyzer`, RU + EN, with simple negation and a colloquial extension list).
It is instant, offline and predictable. When it recognises nothing the dialogue does not guess: it reflects the person's own words,
asks an open question and, after two unclear messages, offers a choice of feelings as chips.

## Measurements (`AnalyzerDatasetTests`, `tools/PsychologyApp.NluEval`)

Two labelled sets, written by hand (RU + EN, colloquial phrasing):

| Set | Purpose | Lexicon |
|-----|---------|---------|
| `nlu-dataset.json` (106) | development set, the lexicon was tuned on it | 99% (inflated; guards regressions only) |
| `nlu-heldout.json` (53) | written afterwards, never tuned on | **24-28%** (honest estimate for unseen phrasing) |

The first version of the lexicon scored 72% on the development set before it was tuned; on unseen phrasing the realistic figure is
far lower. Word lists cannot cover how people actually write.

### Sentence embeddings (multilingual-e5-small, int8 ONNX, ~118 MB), nearest labelled example

On the held-out set, 100 development messages as prototypes (`--test`):

| Method | Held-out accuracy |
|--------|-------------------|
| Lexicon | 24% |
| Embeddings, nearest example | 42% |
| Lexicon, falling back to embeddings when it recognises nothing | 52% |

With few prototypes per class embeddings alone were weaker than the lexicon (43-68% on the development set depending on the number
of prototypes); the gain comes from combining them. Embedding one message takes ~8 ms on a desktop CPU.

## Decision

**Embeddings are not integrated yet.** Reasons:

- Even the best variant is wrong about half of the time on unseen text; the missing ingredient is labelled data, not the model.
- Shipping needs ONNX Runtime (~15-20 MB extra in the APK) plus a ~118 MB model download (the downloader exists in
  `Infrastructure/LocalModel`), and adds a heavy dependency for a partial gain.
- A wrong guess is costly in this domain ("you seem hurt" to someone who feels guilty). Any embedding-based guess should be
  *proposed and confirmed with one tap*, not assumed.

What would make it worthwhile:

1. **Labelled data.** A few hundred to a thousand messages labelled by a psychologist; train a small classifier on the embeddings
   instead of nearest-example matching, and evaluate on a held-out set that is never tuned on.
2. **Confirm-before-assume UX.** Use the embedding only to pre-select the two most likely chips ("Sounds like anxiety or fear?").
3. Re-run `tools/PsychologyApp.NluEval` and compare with the numbers above.

Until then the low recognition rate is absorbed by the dialogue design (open questions, chips, quoting the person's words).

## How to re-run

```bash
dotnet test PsychologyApp.Application.Tests --filter AnalyzerDataset --logger "console;verbosity=detailed"

dotnet run -c Release --project tools/PsychologyApp.NluEval -- <e5-dir> \
  PsychologyApp.Application.Tests/Conversation/Data/nlu-dataset.json \
  --test PsychologyApp.Application.Tests/Conversation/Data/nlu-heldout.json
```

`<e5-dir>` holds `model_qint8_avx512_vnni.onnx` and `sentencepiece.bpe.model` from `intfloat/multilingual-e5-small` (MIT).
Do not tune the lexicon on the held-out set; replace it with a fresh set first.

## Confidence is not a usable "is this a guess?" signal (measured)

`SituationAnalysis.Confidence` is `top / (top + second + 1)`. A single clear keyword ("anxious", "тревожно") scores exactly 0.5, the same as one
accidental match, so a threshold cannot tell them apart. Measured precision of the named feeling, by minimum confidence:

| Minimum confidence | Development set (named / precision) | Held-out set (named / precision) |
|--------------------|-------------------------------------|----------------------------------|
| 0 | 145 / 99% | 19 / 74% |
| 0.51 | 119 / 99% | 12 / 83% |
| 0.67 | 50 / 100% | 2 / 100% |

Requiring 0.51 raised held-out precision from 74% to 83% but made the dialogue stop naming obvious single-keyword messages
("I feel so anxious about my job"), which broke three existing dialogue tests, so it was not adopted. The held-out problem is mostly
**coverage** (19 of 60 messages named at all), not wrong names: better recall needs a larger lexicon or the embedding fallback
described above, not a confidence cut-off.

## Third lexicon round (2026-10) and the telling-message rule

`LexiconExtendedPhrasings.cs` adds how people describe a state without naming it (body signs, situations, figures of speech).
Measured on `nlu-fresh2.json` (98 messages written *after* the round and never tuned on):

| Lexicon | Final set (never tuned on) |
|---------|----------------------------|
| Before the round | 37% |
| After the round | **41%** |

The two older sets (`nlu-heldout.json` 92%, `nlu-fresh.json` 97%) were read while writing the terms, so those numbers are inflated and only
guard against regressions. The honest figure is the final set: +4 points. Word lists have reached their limit; the rest needs data
(see "What would make it worthwhile" above).

Because most of what is missed is a person *telling* something, a message of eight words or more that names no feeling now gets the
choice of feelings right away (`CompanionDialogue.TellingWords`) instead of an open question, which used to make them tell it again;
shorter unclear messages keep the old two-step behaviour.

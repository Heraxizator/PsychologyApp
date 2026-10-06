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

## A second opinion for what the lexicon misses (2026-10)

`EmotionGuesser` is a naive Bayes classifier over character 3-5-grams, trained at first use (about 25 ms) from the 427 labelled messages in
`Conversation/Companion/Data/emotion-training.json` (the four evaluation sets merged and de-duplicated). It runs only when the lexicon
recognised nothing and the message is a story (at least 8 words, as before), and its answer is never assumed: the chat says "Sounds like X, is that
right? If not, pick what is closer." and puts X first among the feeling chips, so one tap confirms it.

Leave-one-set-out (`EmotionGuesserTests`: train on three sets, ask about the fourth), with the gate of a lead of 20 (natural-log units) and at least
5 words:

| | all four sets pooled (427 messages) |
|---|---|
| Lexicon alone | 355 right (83%) |
| Lexicon, then the guesser's proposal | 379 right (89%) |
| Lexicon misses that get a proposal | 38 of 53, 25 of them right (66%) |
| Small talk wrongly given a feeling | 1 |

On the final set (the only one never read while writing terms): lexicon 41%, lexicon then guesser 63%.

Limits, stated plainly: the training and the test messages were written by the same author in the same style, so real people's phrasing is the open
question; precision of about two in three is why the result is a question and not a conclusion. Replace the training file with messages labelled by a
psychologist and re-run `EmotionGuesserTests`; the gate (`EmotionGuesser.MinMargin`, `MinWords`) should be re-chosen on that data.

### Update: word features, a "no feeling" class, more data (2026-10)

The guesser now also uses word stems and word pairs next to the character n-grams, and "no feeling named" (small talk, questions about the app)
is a class of its own, so it can answer "nothing here" instead of being forced to pick a feeling. The training set grew from 427 to 723 messages
(`nlu-train-extra.json` adds 190 written in other registers: short, formal, colloquial, English). It is regenerated from the labelled files with
`node tools/build-emotion-training.js`; a test fails when a labelled message is missing from the shipped file.

Model comparison, leave-one-set-out over five sets (527 messages; the lexicon alone gets 414 right, 78.6%). Gate: lead of 20, at least 5 words.

| Model | Lexicon then model | Proposals / lexicon misses | Right | Small talk given a feeling |
|---|---|---|---|---|
| Characters only (the first version, extra data) | 87.5% | 67 / 76 | 81% | 7 |
| Characters + words | 87.7% | 69 / 76 | 80% | 7 |
| Words only | 79.1% | 3 / 76 | 100% | 0 |
| Characters + a "no feeling" class | 88.4% | 57 / 76 | 91% | 0 |
| **Characters + words + "no feeling" class (shipped)** | **88.8%** | **60 / 76** | **90%** | **0** |

Per set, the sets written after the lexicon was frozen (never read while writing terms): `nlu-fresh2.json` lexicon 41% -> lexicon then guesser
74%; `nlu-fresh3.json` 59% -> 76%. The model and the gate were chosen on these same folds, so some optimism remains; the authorship caveat above still applies.

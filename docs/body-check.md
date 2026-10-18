# Body check-in

The "Body" page (burger menu) helps notice a feeling through the body: where something is felt, what it is like, and which feelings
may be behind it. It replaces the removed psychosomatics table, which tied illnesses to fixed causes ("an abscess: thoughts of
revenge"). That table had no scientific basis, made people blame themselves or their children for an illness and could delay seeing a
doctor. What people used it for, finding feelings they do not notice yet, is kept, without the harm.

## The flow

1. **Where.** A tap on the front silhouette or on a chip: head, face and jaw, throat, neck and shoulders, chest, stomach, back, arms,
   legs, whole body. The back and the whole body are chips only. The silhouette is skipped by screen readers; the chips carry the same
   choice.
2. **What it is like.** Tightness, heaviness, heat, cold, a lump, trembling, emptiness, fluttering, tingling, pain; strength 0–10, optional: a scale left untouched is saved as not rated.
3. **Doctor first.** Pain, or any sensation at 8 and above, shows a card before the feelings: a body signal can be an illness; if it is
   new or does not pass, see a doctor first (`BodyRules.AsksForDoctor`). A shorter line says the same under every check-in.
4. **What could be behind it.** Up to three feelings as guesses, then all the others and "I don't know"; any number can be chosen,
   "I don't know" excludes the rest. Two open questions ("If this sensation could speak…", "When did it start?") and an optional note.
5. **After saving.** Practices for what was found (`BodyRules.Practices`: always the body scan, then breathing and grounding for fear,
   self-compassion and the thought record for hurt, sadness, shame or guilt, a small step for exhaustion). If the note reads as a
   crisis (`ICrisisDetector`), the page shows the way to crisis help instead of practices.

## Where the guesses come from

`BodyRules.Guesses` adds a weight per place and a weight per sensation and takes the top three. The places follow the bodily maps of
emotions (Nummenmaa, Glerean, Hari, Hietanen, *PNAS* 2014): anxiety and fear in the chest and stomach, anger as heat in the head and
arms, sadness as heaviness in the chest and a lump in the throat, shame as heat in the face. They are offered as guesses for the person
to check, never as an answer. The texts and weights need review by a psychologist before release.

## The person's patterns

`BodyPatterns.Find` groups the check-ins by place and sensation. A pair seen at least twice is a pattern; it shows how often, the
feeling chosen most often in it (not "I don't know") and the part of the day when at least 60% of them fell on one. Up to three
patterns, the most frequent first. They come from the person's own entries, not from a table.

## Data

- `BodyLog` is one JSON document in `AppMetadata` (key `BodyLog`) inside the encrypted database, like the reading state. Notes are the
  person's own words and never leave the phone.
- At most 500 check-ins; the oldest go first.
- An entry can be deleted from the page (with a confirmation).
- A backup carries the log. On restore, entries are joined by id with the ones on the device (`BodyService.Merge`): restoring the same
  file twice adds nothing, and nothing written since is lost. An entry deleted on the device comes back if an older backup that still
  has it is restored.
- Values are stored by name. A place, sensation or feeling this version does not know (written by a newer one, or edited by hand)
  reads as undefined instead of failing the log or the backup; such entries stay in the log and are left out of the list and the
  patterns. A log that cannot be read at all is copied to `BodyLog.unreadable` before the next save replaces it
  (`AppMetadataDocumentRepository`, shared with the reading state and the fear ladder).
- A backup's log is checked at the boundary (`BodyService.Sanitize`): entries without an id or a date are dropped, the rest get a
  list of feelings, a strength within 0..10 and a note within 600 characters.

## Code

| Part | Where |
| --- | --- |
| Model, guesses, doctor rule, practices, patterns | `PsychologyApp.Application/Body/BodyCheck.cs` |
| Saving, crisis check, merge | `PsychologyApp.Application/Body/BodyService.cs` |
| Storage | `PsychologyApp.Infrastructure/Data/Repositories/UserProgress/BodyRepository.cs` |
| Names, silhouette shapes and taps | `PsychologyApp.Presentation.Core/Body` |
| Page | `PsychologyApp.Presentation/Pages/ClinicalCare/BodyCheck`, `Widgets/BodyMap` |

Tests: `PsychologyApp.Application.Tests/Body`, `PsychologyApp.Integration.Tests/Body`, `PsychologyApp.Presentation.Tests/Body`.

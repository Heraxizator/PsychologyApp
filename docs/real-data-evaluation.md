# Evaluating the companion on real messages

Every labelled set in `PsychologyApp.Application.Tests/Conversation/Data` was written by the developers, so the accuracy numbers
(lexicon 76.8% -> 87.7% with the guesser) describe how well the engine reads *our* phrasing. The only honest number comes from messages
written by other people. This is how to get it without compromising anyone's privacy.

## Collecting
- Ask testers (friends, a closed beta group) to write 20-30 messages as if they were using the chat, **or** to paste messages they have
  already written in their own words. Nothing is taken from the app: the chat never leaves the phone and must stay that way.
- Get consent in writing for each person, say what the messages are for (measuring the engine) and that they can be deleted on request.
- Remove names, places, phone numbers, workplaces and anything else that identifies a person before the text is stored.
- Never include messages from real users' chats, even if exported by them, without a separate consent for that.

## Format
JSON arrays in `PsychologyApp.Application.Tests/Conversation/Data/real/*.json` (git-ignored, never committed):

```json
[
  { "lang": "ru", "label": "Anxiety", "text": "..." },
  { "lang": "ru", "label": "Unknown", "text": "small talk, no feeling" },
  { "lang": "en", "label": "Crisis", "text": "a message that must raise the helpline card" },
  { "lang": "ru", "label": "Unknown", "intent": "AsksForAdvice", "text": "..." }
]
```

Labels are `CompanionEmotion` names, `Unknown` for messages with no feeling, `Crisis` for crisis messages. Have a second person label them,
without seeing the engine's answers. `intent` is optional; when present, use a `Utterance` name and label it independently too. Cases with
an intent label are checked against the classifier and reported by intent. The tests read these ignored files directly from the source tree;
they do not copy private examples into build output, where deleted samples could otherwise linger. Test output includes labels and mismatch
counts only; it never prints message text. Consent and anonymisation are still required, and the files stay local and ignored by Git.

## Running
```bash
cd /c/Users/HardWorkerIT/PsychologyApp && dotnet test PsychologyApp.Application.Tests --filter "FullyQualifiedName~RealMessagesTests" --logger "console;verbosity=detailed"
```
The report lists lexicon and guesser accuracy plus intent accuracy. A missed `Crisis` message or a misclassified labeled intent fails the test.
If the local directory has no relevant cases, the test reports that it skipped that evaluation; this is not a quality result.

For multi-turn context, put arrays of dialogues in `PsychologyApp.Application.Tests/Conversation/Data/real/dialogues/*.json`:

```json
[
  {
    "lang": "ru",
    "turns": [
      { "text": "...", "intent": "Statement", "emotion": "Anxiety", "event": "Conflict", "questionPending": true },
      { "text": "...", "intent": "DontKnow", "goal": "vent", "offersPractice": false }
    ]
  }
]
```

Every expected field is optional. Supported labels are `Utterance` names for `intent`; `CompanionEmotion`/`Goal`/`Event` state values
(use `None` to assert that a nullable `goal` or `event` was cleared); `DialogueActionKind` names (or `None`) for `action`; and booleans for
`questionPending` and `offersPractice`. The test runs turns in order,
checks each supplied expectation, and reports mismatches by dialogue and turn number without logging message text. Use independently assigned
labels; do not tune against the same examples and count them as a held-out result.

## Rules
- Do not add lexicon terms from the misses of the same set and then quote its accuracy again; keep part of the data aside (as with `nlu-fresh2.json`).
- Report the number with the size of the set and how it was collected ("84 messages from 6 testers"), not as a bare percentage.

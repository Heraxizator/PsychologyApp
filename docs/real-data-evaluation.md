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
  { "lang": "en", "label": "Crisis", "text": "a message that must raise the helpline card" }
]
```

Labels are `CompanionEmotion` names, `Unknown` for messages with no feeling, `Crisis` for crisis messages. Have a second person label them,
without seeing the engine's answers.

## Running
```bash
cd /c/Users/HardWorkerIT/PsychologyApp && dotnet test PsychologyApp.Application.Tests --filter "FullyQualifiedName~RealMessagesTests" --logger "console;verbosity=detailed"
```
The report lists the lexicon accuracy, the lexicon + guesser accuracy and every miss. A missed `Crisis` message fails the test.

## Rules
- Do not add lexicon terms from the misses of the same set and then quote its accuracy again; keep part of the data aside (as with `nlu-fresh2.json`).
- Report the number with the size of the set and how it was collected ("84 messages from 6 testers"), not as a bare percentage.

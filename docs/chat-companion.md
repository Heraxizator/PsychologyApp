# Chat companion

The chat companion is an offline, on-device dialogue engine behind a messenger-style UI. It does not use a language model and never
touches the network. Why not a model: see [local-llm.md](local-llm.md) (unusable in Russian on a phone) and
[companion-understanding.md](companion-understanding.md) (measured limits of understanding).

## Layers

| Layer | What lives there |
| --- | --- |
| `Application/Chat` | `CompanionDialogue` (the engine), `CompanionState`, `UtteranceClassifier`, wording (`CompanionDialogueContent`, `CompanionActContent`, `CompanionSmallTalk`, `CompanionKnowledge`, `ChatProfileContent`), `ChatService`, `ChatStatistics` |
| `Application/Conversation/Companion` | `LexiconSituationAnalyzer` (feeling, theme, person from text), `TechniqueSuggester`, `CompanionContent` |
| `Infrastructure` | `ChatRepository` (SQLite, Dapper), schema versions 9 (chats, messages) and 10 (`ChatMemory`) |
| `Presentation` | chat list, conversation, companion profile, `CompanionAvatarView`, home card |

## One turn

`ChatService.TakeTurnAsync` loads the chat, merges what is remembered between chats into the state, asks `CompanionDialogue.Respond`,
saves the person's message and the whole reply in one transaction and updates the chat title, feeling and tension.

`CompanionDialogue` decides in this order:

1. Crisis wording is checked first (`KeywordCrisisDetector`). A hit opens the crisis hub and nothing else runs.
2. A typed 0-10 number answers the tension question, exactly like the chip.
2b. Input that is not a message is recognised before anything else is guessed: bare digits ("1111"), key mashing ("asdfgh", "йцукен",
   "qwerty"), one letter repeated, a repeated unit ("sdfsdf"), a long word with no vowel, "???" (means "what?", the last question is
   repeated) and "!!!" (strong feeling, not a smile). The companion says plainly that it could not make it out, does not count it as a
   turn, does not remember it as something said and does not change what it knows (`CompanionState.UnclearStreak`); from the second miss
   in a row it offers the feelings to pick from. While the tension question is open, digits get the scale explained; a lone 0-10 number
   with nothing asked gets "what is that number about?"; a short number (up to three digits) after an open question is an answer.
   Russian typed on the English layout ("ghbdtn" for "привет") is read as Russian when the Russian reading means something to the engine.
   One or two words with no feeling, topic or person, said when nothing was asked, are not thanked as a story.
3. `UtteranceClassifier` says what the person is doing: greeting, thanks, goodbye, yes/no, "I don't know", refusing to talk, asking for
   advice, asking to explain, why, is it normal, backchannel, laughter/emoji, good news, how are you, name, capabilities, "say that
   again", "skip". Social phrases count only when the message carries no feeling ("thanks, but I'm still anxious" is a statement).
4. Anything else is a statement about a situation: quote one sentence back (only when the message has several), name the feeling, ask
   one fitting question at a time, measure tension, summarise now and then, return to the first thing said, offer a practice.
5. If the reply would repeat one of the last eight companion messages, it is drawn again (up to four times).

`CompanionState` is an immutable record serialised to JSON with the chat, so a chat resumes exactly where it stopped.

## Memory between chats

Table `ChatMemory (MemoryKey, MemoryValue)`:

| Key | Meaning |
| --- | --- |
| `name` | how to address the person |
| `tried:<TechniqueId>` | how many times a practice was started from a chat |
| `helped:<TechniqueId>` | how many times the tension dropped after it |

The practice with the most `helped` marks is offered first in the next chat ("Last time X helped you"), except during a panic attack,
where the standard help is used. Deleting all chats keeps the memory; "forget" clears the memory and keeps the chats.

## Companion profile

Opened by tapping the face or the name in the chat header. `ChatService.GetProfileAsync` computes everything from stored chats and the
memory (`ChatStatistics.Compute`), nothing is stored twice:

- chats with something said, messages written, active days, current streak;
- tension at the start and the end of each measured chat (last eight), how many improved, the average change;
- practices with tried / helped counters, the most frequent feelings with their share;
- trust level (0-3) from the number of messages written; it is a friendly progress marker, not a clinical measure;
- short insights that only state what the numbers show.

## Limits, stated plainly

- The engine understands what its lexicon and rules cover. On held-out sentences the analyzer's accuracy is far below its accuracy on the
  tuned set (see companion-understanding.md). Expect misses on unusual wording; the companion then asks which feeling is closest.
- It cannot rephrase a question, only repeat it. "Say that again" repeats the last question with a lead-in.
- Time of day comes from the phone's clock.
- All Russian and English wording, the psychoeducation texts and the advice ("what usually helps") must be reviewed by a psychologist
  before release. They are general and non-diagnostic by design.

## Tests

`Application.Tests/Chat`: classifier, dialogue, small talk, knowledge, statistics, service with an in-memory repository.
`Infrastructure.Tests/Data`: repository, schema, memory. The Presentation project is Android-only, so its view models are not unit tested on
Windows; the logic they show lives in Application on purpose.

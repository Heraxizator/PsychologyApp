# Trauma module

**Status:** psychoeducation and self-help copy, to be reviewed by a clinical psychologist who works with trauma before release.

## What the module is, and what it is not

Post-traumatic stress responds well to trauma-focused therapy: prolonged exposure (PE), cognitive processing therapy (CPT),
cognitive therapy for PTSD, EMDR, written exposure therapy (WET), narrative exposure therapy (NET). Guidelines (APA 2017/2025,
NICE NG116, VA/DoD 2023, ISTSS) recommend these with a therapist. Processing a trauma memory alone, from an app, can flood a
person with no one to help them through it. So the app does **not** run trauma processing. It does what is safe to do alone:

| Need | What the app gives | Where |
| --- | --- | --- |
| Do I have a post-traumatic reaction? | PC-PTSD-5 screen (5 yes/no, positive at ≥ 3) | Tests tab |
| How bad is it, is it getting better? | PCL-5 (20 items, 0–80; "probable" at ≥ 33, elevated 21–32), repeatable every 2 weeks | Tests tab, history |
| A flashback right now | "A memory took over": time and place, feet, three things seen, breath, cold, rating before/after | Crisis help screen, chat, catalogue, articles, test result |
| Recurring nightmares | "Rewrite a nightmare": imagery rehearsal therapy (IRT), start with a milder one | Catalogue ("Heart"), articles |
| Understanding | Articles: start here, the traumatic reaction, flashbacks, nightmares, treatment and choosing a therapist | Articles tab, "Trauma" section |
| The way to a specialist | Which methods work, what to ask, red flags; the specialist summary export | Article "How trauma is treated…", Profile → Data |

The catalogue card "After trauma" (first in "Heart") opens the hub article "After trauma: where to start"; the main page gets no
new block. A "together with a therapist" mode (homework between sessions of PE/CPT/WET) is a possible next step and is not built.

## Trauma work in the chat

The person asked for a chat that "digs up traumas like an AI psychologist". That is deliberately **not** built, and why matters:

- Searching for "hidden" or "repressed" memories with leading questions creates false memories (well documented); our own treatment
  article names it as a red flag in a specialist.
- Reliving an event in detail without a therapist can flood a person (a flashback, dissociation) with no one to notice and stop it.
  The companion is a rule-based engine without a language model; it cannot read a person the way a therapist does.
- A cloud model is ruled out by the first rule of the app (the person's words never leave the phone), and local models do not work
  in Russian on a phone (docs/local-llm.md).

What is built instead rests on a finding of cognitive processing therapy: CPT **without** a written account of the event is as
effective for PTSD as with it (Resick et al., 2008). The work that matters is on what the event changed, not on the memory.

| Part | What it does |
| --- | --- |
| Trauma-informed reply | Words of a hard event (abuse, an assault, an accident, war; `TraumaTerms`, negation respected) are met once with thanks, "you don't have to tell me the details", and choices; "what happened?" is not asked afterwards. A death goes the grief way. |
| What it changed in me (stuck points) | Distress 0–10 first → a theme (blame, safety, trust, control, how I see myself, closeness) → the phrase one tells oneself (examples as chips) → belief 0–10 → two Socratic questions of that theme (for blame: what could you know and do then, who is really responsible) → a fairer wording → belief again → summary. |
| What sets it off (trigger map) | When it got harder (without the event), what the body and mind did, what helped → summary. |
| Stabilising first | Distress of 7 or more, or a flashback in the words at any step, stops the work: back to the present ("A memory took over", grounding). Crisis words open crisis help before anything, as everywhere. "Stop" ends it at any step. |
| Summary for a specialist | Stuck points (phrase, belief before → after, revised wording) and triggers are kept in the chat memory (`trauma_notes`, the encrypted database, at most 10 of each) and go into the summary for a specialist, without details of the event. |
| Entry points | The chat itself, when a hard event is mentioned; "Work it through in the chat" in the article "After trauma: where to start" and on day 4 of the trauma course, which opens a chat on the topic straight at the choices. |

Exposure to the memory itself (PE, WET, EMDR, NET) stays with a therapist.

## Rules

- A positive PC-PTSD-5 or a PCL-5 of 33+ recommends the flashback help, never a processing exercise. The result text says it is
  a screen, not a diagnosis, and points to a trauma specialist.
- The flashback dialogue has its crisis node; words of crisis in any answer stop it, as in every dialogue
  ([chat-companion.md](chat-companion.md)). "Not safe right now" gets the emergency number before anything else.
- The companion treats a described flashback ("накрыло воспоминание", "снова там", "flashback", "reliving") as acute distress:
  the reply names the past as past and offers "A memory took over" first, then grounding. A negated mention ("это не флешбэк")
  does not count. Crisis detection runs before this and wins.
- Nightmare rewriting says to work with a therapist when the nightmare replays the trauma exactly or even a short description
  brings intense horror, and to see a doctor for nightmares with shouting, movement or pauses in breathing.
- Articles that mention danger or self-harm (start here, the traumatic reaction, treatment) carry the crisis-help link (tested).

## Where it lives

- Questionnaires: `Resources/Raw/questionnaires(.en).json` (`pcptsd5`, `pcl5`), bands in `TestScoreBandClassifier`,
  recommendations in `TestScoreRecommendation`, texts in `AppStrings.Scoring`.
- Practices: `TechniqueId.Flashback`, `TechniqueId.Nightmare` in the catalogue; the flashback dialogue in
  `Resources/Raw/conversations/Flashback(.en).json`.
- Articles: `PsychologyApp.Application/Articles` (`ArticleIds.Trauma*`, section `trauma`); article blocks `Tests` and `Article`
  link to the Tests tab and to another article.
- Companion: `FlashbackTerms` in `LexiconExtendedPhrasings`, `SituationAnalysis.IsFlashback`, `TechniqueSuggester`,
  `CompanionDialogueContent.FlashbackBridge`.
- Trauma work in the chat: `CompanionDialogue.Trauma.cs`, wording in `CompanionTraumaContent`, notes in `TraumaNotes`,
  `TraumaTerms` and `SituationAnalysis.MentionsTrauma`; the summary section in `SpecialistSummaryService`.

## Sources

- Resick P. A., Galovski T. E., et al. (2008). A randomized clinical trial to dismantle components of cognitive processing therapy for
  PTSD in female victims of interpersonal violence. *J Consult Clin Psychol* 76(2), 243–258.
- Loftus E. F. (1993). The reality of repressed memories. *American Psychologist* 48(5), 518–537.
- Prins A. et al. (2016). The Primary Care PTSD Screen for DSM-5 (PC-PTSD-5). *J Gen Intern Med* 31, 1206–1211.
- Weathers F. W. et al. (2013). The PTSD Checklist for DSM-5 (PCL-5). National Center for PTSD; cut-off 31–33 (Bovin et al., 2016).
- Krakow B., Zadra A. (2006). Clinical management of chronic nightmares: imagery rehearsal therapy. *Behav Sleep Med* 4, 45–70.
- Sloan D. M., Marx B. P. (2019). *Written Exposure Therapy for PTSD.* APA.
- NICE NG116 (2018). Post-traumatic stress disorder.

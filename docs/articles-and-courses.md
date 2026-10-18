# Articles and courses

**Status:** psychoeducation and self-help copy, to be reviewed by a clinical psychologist (and the "see a doctor" lists by a physician)
before release.

## Why courses

Reading about anxiety or sleep changes little by itself. Guided self-help — short reading, an exercise, a task between sessions and a
measure at the start and the end — has clear effects for anxiety, depression and insomnia (Cuijpers et al.; for insomnia, digital
CBT-I). The Library tab therefore has courses besides articles, and the articles themselves ask for an answer, not only reading.

## What the Library has

The Library is the "Materials" group of the burger menu: For you, Courses, Articles, Prayers, Quotes. Each part opens as its own page
over the current tab (the tab bar stays), so nothing has to be scrolled past to reach the articles.

| Part | What it does | Where |
| --- | --- | --- |
| For you | Up to three picks from what the app already knows on the device: a course in progress, an article not read to the end, recent questionnaire results (PCL-5 / PC-PTSD-5, PHQ-9, ISI, GAD-7 against published cut-offs), three or more hard days in the mood journal | `ReadingRecommender` |
| Courses | Sleep without pills (CBT-I, 7 days, ISI), Anxiety under control (6 days, GAD-7), Out of apathy (behavioural activation, 6 days, PHQ-9), After trauma (5 days, PCL-5) | `CourseContent.Ru/.En` |
| Search | Every word of the query in the title, summary or text; "ё" = "е"; title matches first | `ArticleSearch` |
| Saved | Listed under "For you", below the picks | `ReadingState.Saved` |
| Read marks | An article counts as read once its end is on screen, not on opening | `ArticleReaderPage` |
| Listening | The phone's own speech engine reads the article part by part; only the app's text is spoken, never what the person wrote | `ArticleSpeech`, `ReadAloud` |

## Interactive blocks

`ArticleBlockKind`: `Checklist` ("do I recognise this?", a verdict at a threshold, never a diagnosis), `Quiz` (one answer, then the right
one and why: recalling fixes knowledge better than rereading), `Cycle` (a vicious circle written out step by step, readable by screen
readers and at large text sizes), `Breathe` (the breathing pacer, 4 in / 6 out, one minute, inside the article), `Tests` (opens one
questionnaire by id), `Article` (a link to another article), `Task` (a course day's task).

## Course rules

- One day at a time; the next day opens on the next calendar day, so the task has time to be tried (`CourseRules`, tested).
- Each day ends with a question answered in one's own words. The answer is kept only in the encrypted database and the backup.
  Words that read as a crisis do not close the day: the screen leads to crisis help.
- The measure: the course's questionnaire at the start (a result up to 30 days before the start counts) and the latest one
  (`CourseMeasure`, tested).
- The trauma course does not ask to recall or describe the event; processing a trauma takes a therapist (see trauma-module.md).
  Sleep compression in the sleep course keeps at least six hours in bed and is not for bipolar disorder, epilepsy, pregnancy or
  dangerous sleepiness.

## Storage

`ReadingState` (read, saved, last opened, course progress with answers) is one JSON document in `AppMetadata` (`ReadingRepository`,
source-generated), included in the backup; a backup is restored only when nothing is stored yet, so restoring an old file never erases
newer progress.

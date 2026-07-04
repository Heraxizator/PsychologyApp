using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class TechniqueCatalogContentEn
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("What it is", what),
        new("When it helps", when),
        new("How the practice goes", how),
        new("Example", example)
    ];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "Practice", "Spin",
        [
            "1. Describe the episode that bothers you",
            "2. Name the main feeling",
            "3. Rate the feeling from 0 to 10"
        ],
        "Zivorad Slavinski's Spin helps lower the emotional charge of a painful memory.",
        TechniqueUiKind.Entry, "Technique #1", "26.01.2023", "Spin", "Lower emotional charge in 3 steps", "Episodes", "Zivorad Slavinski", 5, "Autorenew",
        [
            new TechniqueEntrySeed("Episode", "E.g. yesterday's conversation you can't let go"),
            new TechniqueEntrySeed("Key feeling", "Hurt, fear, anger…"),
            new TechniqueEntrySeed("Feeling intensity", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "A simple neutralization technique: you name the episode, the feeling, and its intensity. Often awareness alone reduces the charge.",
            "When a scene or thought loops and won't release anxiety or resentment.",
            "Write the episode and feeling, then rate intensity honestly. Repeat another day if needed.",
            "Episode: \"A colleague ignored my idea\". Feeling: hurt. Rating: 8 → later 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Comparison = new(
        "Practice", "Importance comparison",
        [
            "1. State the problem",
            "2. What mattered in the past in this area?",
            "3. What matters now?",
            "4. What will matter in the future?",
            "5. Compare the three answers and note what shifted"
        ],
        "Comparing importance across time helps place a problem in the context of your life.",
        TechniqueUiKind.Entry, "Technique #2", "26.01.2023", "Importance comparison", "When the problem feels too big", "Importance", "NLP", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("Problem", "What feels unbearable right now?"),
            new TechniqueEntrySeed("Mattered in the past", "E.g. studies, relationships, career"),
            new TechniqueEntrySeed("Matters now", "What takes most of your attention?"),
            new TechniqueEntrySeed("Will matter in the future", "What are you aiming for?"),
            new TechniqueEntrySeed("What changed?", "Does the problem still dominate everything?")
        ],
        Theory(
            "The technique shows that a problem's weight changes over time and rarely stays central forever.",
            "When one difficulty takes the whole screen and feels permanent.",
            "Fill four fields, then compare: other values often shrink the problem.",
            "Problem: \"No job\". Past: studying. Present: job search. Future: stability. Insight: a phase, not all of life."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Polarity = new(
        "Practice", "Polarities",
        [
            "1. Write what you like about the situation",
            "2. Write what you dislike",
            "3. Compare pairs and notice the balance"
        ],
        "The polarity method helps you see both sides and reduce inner tension.",
        TechniqueUiKind.Polarity, "Technique #3", "26.01.2023", "Polarities", "See both sides of a situation", "Aspects", "Zivorad Slavinski", 5, "Contrast",
        null,
        Theory(
            "Every situation has attractive and unpleasant sides. Naming both reduces one-sided fixation.",
            "When you see only negatives or only positives and feel stuck.",
            "Add like/dislike pairs for one theme. Compare lists without judging who is right.",
            "Work: like income, dislike schedule. Home: like coziness, dislike neighbor noise."),
        "What you dislike", "What you like");

    internal static readonly BuiltInTechniqueDefinition Paper = new(
        "Practice", "Sheet of paper",
        [
            "1. Write the thought on paper (or in the field below)",
            "2. Read it aloud once",
            "3. Fold, tear, or symbolically release the sheet"
        ],
        "Writing thoughts on paper reduces their emotional charge.",
        TechniqueUiKind.Paper, "Technique #4", "26.01.2023", "Sheet of paper", "When thoughts keep looping", "Thoughts", "Psyche", 3, "Description",
        [new TechniqueEntrySeed("Thought", "E.g. \"I'll ruin the presentation\"")],
        Theory(
            "Writing moves the thought outside your head. Destroying the sheet symbolizes letting go.",
            "When the same sentence spins for hours and blocks sleep or focus.",
            "Unload the thought, read it, then destroy the sheet — physically or in imagination.",
            "Thought: \"They'll think I'm incompetent\". After writing and burning the sheet, anxiety often fades."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Future = new(
        "Practice", "50 years later",
        [
            "1. State the problem",
            "2. Rate its importance 50 years from now from 0 to 10"
        ],
        "Moving a problem forward in time lowers its subjective importance.",
        TechniqueUiKind.Entry, "Technique #5", "30.01.2023", "50 years later", "Move the problem forward in time", "Importance", "NLP", 2, "Schedule",
        [
            new TechniqueEntrySeed("Problem", "What feels like a disaster today?"),
            new TechniqueEntrySeed("Importance in 50 years", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Imagining an event far in the future often dulls its edge — you see it from a wider perspective.",
            "When a setback feels like the end of the world though it's temporary.",
            "Name the problem and rate how much it will matter in fifty years. The score is usually lower.",
            "Problem: \"I wasn't invited to the meeting\". In 50 years: 1–2 out of 10."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Hack = new(
        "Practice", "White spot",
        [
            "1. Recall the bothering episode",
            "2. Picture it as a white spot without details",
            "3. Hold the spot until tension eases"
        ],
        "Removing details from a memory often lowers its emotional charge.",
        TechniqueUiKind.Entry, "Technique #6", "30.01.2023", "White spot", "Soften a painful memory", "Episodes", "NLP", 3, "BlurOn",
        [new TechniqueEntrySeed("Episode", "Briefly: what happened and what hurts most")],
        Theory(
            "Details fuel the feeling. A white spot removes triggers and gives your nervous system a pause.",
            "When a vivid image from the past hits you with emotion.",
            "Recall the episode, then replace the picture with a plain white spot for 30–60 seconds.",
            "Instead of the offender's face — a white spot. Breathing steadies, mind returns to now."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Experience = new(
        "Practice", "Experience modification",
        [
            "1. Choose an area to work on",
            "2. Describe your attitude toward it",
            "3. Name related beliefs",
            "4. How does attitude change when you think of past experience?",
            "5. Describe a negative experience",
            "6. Rate the negative experience (−10…10)",
            "7. What positive experience could replace it?",
            "8. How would your attitude change with that experience?",
            "9. Rate the new experience (−10…10)"
        ],
        "EMO helps replace a limiting experience with a more supportive one.",
        TechniqueUiKind.Entry, "Technique #7", "08.02.2023", "Experience modification", "Rewrite a limiting experience", "Episodes", "Filip Slavinski", 15, "Psychology",
        [
            new TechniqueEntrySeed("Area", "Relationships, work, health…"),
            new TechniqueEntrySeed("Attitude toward the area", "How do you feel about it now?"),
            new TechniqueEntrySeed("Related beliefs", "E.g. \"I can't succeed\""),
            new TechniqueEntrySeed("Attitude and past experience", "What changes when you recall the past?"),
            new TechniqueEntrySeed("Negative experience", "A specific episode or pattern"),
            new TechniqueEntrySeed("Negative experience rating", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("Positive replacement", "What supportive experience is possible?"),
            new TechniqueEntrySeed("Attitude with the positive", "How would your view of the area change?"),
            new TechniqueEntrySeed("New experience rating", "", EntryFieldKind.RatingNeg10To10)
        ],
        Theory(
            "Beliefs often rest on one vivid negative experience. Conscious replacement can shift your attitude.",
            "When \"it's always been this way\" and change feels impossible.",
            "Go through the fields in order. Compare the two ratings — aim for the second to be higher.",
            "Area: public speaking. Negative: failed exam (−8). Replacement: good talk (+6)."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Copied = new(
        "Practice", "Repeat it",
        [
            "1. Write the wording that bothers you",
            "2. Repeat it aloud as if hearing it for the first time",
            "3. Keep repeating until emotion eases"
        ],
        "Repeating a problem statement \"as for the first time\" lowers emotional intensity.",
        TechniqueUiKind.Copied, "Technique #8", "21.01.2025", "Repeat it", "Hear the problem as if for the first time", "Importance", "NLP", 2, "Repeat",
        [new TechniqueEntrySeed("Wording", "I'm afraid of losing my job")],
        Theory(
            "A familiar phrase loses charge when spoken slowly and neutrally, without drama.",
            "When one thought sounds like a verdict in your head.",
            "Write the phrase, then repeat it 5–10 times with pauses, noticing body sensations.",
            "\"Nobody needs me\" — after seven repeats it often sounds absurd and lighter."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Extend = new(
        "Practice", "Backup plan",
        [
            "1. Name the problem",
            "2. Write three realistic backup options",
            "3. Notice how anxiety shifts when exits exist"
        ],
        "Backup options reduce inflated importance of a single outcome.",
        TechniqueUiKind.Entry, "Technique #9", "21.01.2025", "Backup plan", "Lower the stakes with a backup", "Importance", "NLP", 3, "Backup",
        [
            new TechniqueEntrySeed("Problem", "What if the main plan fails?"),
            new TechniqueEntrySeed("Backup option 1", "E.g. take a freelance project"),
            new TechniqueEntrySeed("Backup option 2", "E.g. train for a related role"),
            new TechniqueEntrySeed("Backup option 3", "E.g. temporarily cut expenses")
        ],
        Theory(
            "When stakes feel all-or-nothing, the mind tightens. Alternatives restore a sense of choice.",
            "Before interviews, exams, or events where failure feels catastrophic.",
            "State the problem and three plausible plan B options — even modest ones.",
            "Problem: \"Won't get the job\". Plan B: internship, referrals, another city."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Resize = new(
        "Practice", "Shrink it",
        [
            "1. Describe what bothers you",
            "2. Picture it as an object in front of you",
            "3. Shrink and move it away until interest fades"
        ],
        "Visual shrinking and distance lower emotional interest in a problem.",
        TechniqueUiKind.Entry, "Technique #10", "22.01.2025", "Shrink it", "Make the problem small and distant", "Importance", "NLP", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("What bothers you", "E.g. a conflict that keeps you awake")],
        Theory(
            "Image size and distance affect significance. A small far object rarely pulls your nerves.",
            "When a problem feels huge and in your face.",
            "Create an image, gradually shrink it and push it to the horizon for 10–20 seconds.",
            "A fight becomes a tiny dot. Breathing eases, thoughts clear."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Check = new(
        "Practice", "Check it",
        [
            "1. Describe what you're looping on",
            "2. Deliberately stop thinking about it for 1–2 minutes",
            "3. Notice what changed in body and mood"
        ],
        "Too much attention inflates importance. A brief pause tests how much the thought controls you.",
        TechniqueUiKind.Entry, "Technique #11", "22.01.2025", "Check it", "See what happens if you let go", "Importance", "NLP", 2, "FactCheck",
        [new TechniqueEntrySeed("What you're looping on", "Which thought keeps returning?")],
        Theory(
            "If a thought were vital for survival, letting go would feel dangerous. The experiment usually shows nothing catastrophic happens.",
            "When you can't stop replaying the same topic.",
            "Name the thought, then gently shift attention to breath or sounds around you.",
            "Thought: \"What if I'm ill\". After a pause, anxiety often drops from 7 to 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Observer = new(
        "Practice", "Observer position",
        [
            "1. Describe the situation that bothers you",
            "2. Write feelings from first person",
            "3. View the situation as a neutral observer",
            "4. Rate tension after shifting position"
        ],
        "Shifting perceptual position helps step back from an emotional peak and see the situation more broadly.",
        TechniqueUiKind.Entry, "Technique #12", "12.06.2026", "Observer position", "Step back from emotions and see the situation from outside", "Episodes", "NLP", 4, "Visibility",
        [
            new TechniqueEntrySeed("Situation", "What happened or what are you replaying in your mind?"),
            new TechniqueEntrySeed("Feelings from first person", "What do you feel while inside the situation?"),
            new TechniqueEntrySeed("What the observer sees", "What would a calm outside observer notice?"),
            new TechniqueEntrySeed("Tension now", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "When you are inside an episode, emotions feel inevitable. The observer position creates distance without denying the experience.",
            "When a situation overwhelms you and rational arguments do not help.",
            "Describe the situation and feelings, then rewrite it in third person — neutrally, without judgment.",
            "Argument: inside — \"they disrespect me\". Observer: \"two people are speaking loudly\". Tension: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Anchor = new(
        "Practice", "Resource anchor",
        [
            "1. Recall a moment when you felt confident or calm",
            "2. Describe body sensations",
            "3. Choose a simple anchor gesture",
            "4. Rate the strength of the resource state"
        ],
        "An anchor links a body gesture to a resource state so you can return to support more quickly.",
        TechniqueUiKind.Entry, "Technique #13", "12.06.2026", "Resource anchor", "Restore support through gesture and memory", "Episodes", "NLP", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Resource memory", "When did you feel strong, calm, or joyful?"),
            new TechniqueEntrySeed("Body sensations", "Where do you feel it? Warmth, lightness, grounding…"),
            new TechniqueEntrySeed("Gesture or anchor", "E.g. clench fist, touch wrist"),
            new TechniqueEntrySeed("State strength", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "The brain quickly links a gesture to a state if you repeat the pair \"memory + gesture\" several times.",
            "Before a stressful event or when inner support feels low.",
            "Recall the resource episode, intensify body sensations, and fix them with a simple gesture.",
            "Resource: successful talk. Body: warmth in chest. Anchor: thumb touch. Strength: 8."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Grounding = new(
        "Practice", "5-4-3-2-1 grounding",
        [
            "1. Name 5 things you can see",
            "2. Name 4 things you can feel with your body",
            "3. Name 3 sounds around you",
            "4. Name 2 smells",
            "5. Name 1 taste or pleasant sensation"
        ],
        "Sensory grounding shifts attention from anxious thoughts to the present moment.",
        TechniqueUiKind.Entry, "Technique #14", "12.06.2026", "5-4-3-2-1 grounding", "Return to the present through the senses", "Episodes", "Psyche", 3, "Nature",
        [
            new TechniqueEntrySeed("5 things I see", "E.g. window, cup, book, lamp, hand"),
            new TechniqueEntrySeed("4 things I feel with my body", "E.g. chair, floor, clothes, air temperature"),
            new TechniqueEntrySeed("3 sounds", "E.g. street, clock, your breath"),
            new TechniqueEntrySeed("2 smells", "E.g. coffee, fresh air"),
            new TechniqueEntrySeed("1 taste or pleasant sensation", "E.g. water in mouth, soft fabric")
        ],
        Theory(
            "Anxiety often pulls you into past or future. Counting through senses brings you back to here and now.",
            "During panic, strong anxiety, dizziness, or feeling detached from reality.",
            "Go through the steps slowly, naming concrete sensations. Breathe evenly between items.",
            "5 see → 4 feel → 3 hear → breathing steadies, thoughts slow down."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Breathing = new(
        "Practice", "Box breathing",
        [
            "1. Rate tension before the practice",
            "2. Inhale for 4 counts",
            "3. Hold for 4 counts",
            "4. Exhale for 4 counts",
            "5. Hold for 4 counts — repeat 4 cycles and rate tension after"
        ],
        "Rhythmic breathing calms the nervous system and reduces bodily tension.",
        TechniqueUiKind.Entry, "Technique #15", "04.07.2026", "Box breathing", "Calm the body through breath rhythm", "Episodes", "Psyche", 3, "Air",
        [
            new TechniqueEntrySeed("Tension before", "", EntryFieldKind.Rating0To10),
            new TechniqueEntrySeed("Notes", "What did you notice in your body or breath during the practice?"),
            new TechniqueEntrySeed("Tension after", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Slow rhythmic breathing lowers sympathetic nervous system activity and helps the body exit fight-or-flight mode.",
            "During anxiety, panic, racing heart, insomnia, or feeling overwhelmed.",
            "Sit comfortably, breathe through your nose: 4 in, 4 hold, 4 out, 4 hold. Repeat 4 cycles.",
            "Anxiety 7/10 → after 4 cycles 4/10. Shoulders drop, pulse slows."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SmallStep = new(
        "Practice", "One small step",
        [
            "1. Describe what feels heavy or blocks you from starting",
            "2. Choose the smallest realistic step for today",
            "3. Write when you will do it",
            "4. Rate your energy after writing the step"
        ],
        "Behavioral activation: a small action restores a sense of movement and control.",
        TechniqueUiKind.Entry, "Technique #16", "04.07.2026", "One small step", "Restore momentum through minimal action", "Episodes", "CBT", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("What feels heavy", "What seems hard, boring, or impossible right now?"),
            new TechniqueEntrySeed("Smallest step today", "E.g. get up, wash face, send one message"),
            new TechniqueEntrySeed("When you will do it", "A specific time or moment in the day"),
            new TechniqueEntrySeed("Energy after", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "In apathy the brain waits for motivation to act. A small step creates motivation through action, not the other way around.",
            "When you have no energy, everything feels pointless, or starting the day is hard.",
            "Pick a 2–5 minute step without a perfect plan. Write it and the time — that is already movement.",
            "Clean apartment → put one cup in the kitchen at 10:00. Energy: 2 → 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition ThoughtRecord = new(
        "Practice", "Thought record",
        [
            "1. Describe the situation",
            "2. Write the automatic thought",
            "3. Rate the emotion",
            "4. Formulate a more balanced alternative thought",
            "5. Rate the emotion again"
        ],
        "A structured CBT record helps you see the link between situation, thought, and emotion.",
        TechniqueUiKind.Entry, "Technique #17", "04.07.2026", "Thought record", "Unpack a thought and reduce emotional charge", "Episodes", "CBT", 7, "EditNote",
        [
            new TechniqueEntrySeed("Situation", "What happened? Where and when?"),
            new TechniqueEntrySeed("Automatic thought", "What did you think in that moment?"),
            new TechniqueEntrySeed("Emotion", "", EntryFieldKind.Rating0To10),
            new TechniqueEntrySeed("Alternative thought", "What else might be true? A more balanced view"),
            new TechniqueEntrySeed("Emotion now", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Automatic thoughts often sound like facts though they are interpretations. Writing them down makes them visible and testable.",
            "After conflict, criticism, failure, or when one thought keeps looping.",
            "Describe situation and thought, rate emotion, then find an alternative — not toxic positivity, but something more accurate.",
            "Situation: no reply to message. Thought: nobody needs me. Alternative: they are busy. Emotion: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SelfCompassion = new(
        "Practice", "Kind words to yourself",
        [
            "1. Write what you are criticizing yourself for",
            "2. Imagine a close friend said this — what would you tell them?",
            "3. Formulate the same supportive phrase for yourself",
            "4. Rate warmth or relief"
        ],
        "Self-compassion reduces harsh self-criticism and helps recover after a mistake.",
        TechniqueUiKind.Entry, "Technique #18", "04.07.2026", "Kind words to yourself", "Soften self-criticism through support", "Episodes", "CBT", 5, "Favorite",
        [
            new TechniqueEntrySeed("What you criticize yourself for", "What do you tell yourself or blame yourself for?"),
            new TechniqueEntrySeed("What you would tell a friend", "How would you support someone close in this situation?"),
            new TechniqueEntrySeed("Phrase to yourself", "The same kindness, directed at you"),
            new TechniqueEntrySeed("Warmth / relief", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Self-criticism rarely motivates — it more often fuels shame and paralysis. Self-compassion does not remove responsibility, but gives support.",
            "After a mistake, comparing yourself to others, or feeling not good enough.",
            "Write the criticism, then answer yourself as you would a friend — with respect and without dismissal.",
            "Criticism: I ruined everything again. To friend: you tried, this is fixable. To self: I can learn from this. Warmth: 3 → 6."),
        null, null);
}

using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.UI.Components;

namespace PsychologyApp.Presentation.Models.Practice.Techniques;

internal static class TechniqueCatalogContentEn
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("What it is", what),
        new("When it helps", when),
        new("How the practice goes", how),
        new("Example", example)
    ];

    internal static readonly TechniqueDefinition Spin = new(
        "Practice", "Spin",
        [
            "1. Describe the episode that bothers you",
            "2. Name the main feeling",
            "3. Rate the feeling from 0 to 10"
        ],
        "Zivorad Slavinski's Spin helps lower the emotional charge of a painful memory.",
        TechniqueUiKind.Entry, "Technique #1", "26.01.2023", "Spin", "Lower emotional charge in 3 steps", "Episodes", "Zivorad Slavinski", 5, "Autorenew",
        [
            new EntryItem { Title = "Episode", Placeholder = "E.g. yesterday's conversation you can't let go" },
            new EntryItem { Title = "Key feeling", Placeholder = "Hurt, fear, anger…" },
            new EntryItem { Title = "Feeling intensity", Kind = EntryFieldKind.Rating0To10 }
        ],
        Theory(
            "A simple neutralization technique: you name the episode, the feeling, and its intensity. Often awareness alone reduces the charge.",
            "When a scene or thought loops and won't release anxiety or resentment.",
            "Write the episode and feeling, then rate intensity honestly. Repeat another day if needed.",
            "Episode: \"A colleague ignored my idea\". Feeling: hurt. Rating: 8 → later 5."),
        null, null);

    internal static readonly TechniqueDefinition Comparison = new(
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
            new EntryItem { Title = "Problem", Placeholder = "What feels unbearable right now?" },
            new EntryItem { Title = "Mattered in the past", Placeholder = "E.g. studies, relationships, career" },
            new EntryItem { Title = "Matters now", Placeholder = "What takes most of your attention?" },
            new EntryItem { Title = "Will matter in the future", Placeholder = "What are you aiming for?" },
            new EntryItem { Title = "What changed?", Placeholder = "Does the problem still dominate everything?" }
        ],
        Theory(
            "The technique shows that a problem's weight changes over time and rarely stays central forever.",
            "When one difficulty takes the whole screen and feels permanent.",
            "Fill four fields, then compare: other values often shrink the problem.",
            "Problem: \"No job\". Past: studying. Present: job search. Future: stability. Insight: a phase, not all of life."),
        null, null);

    internal static readonly TechniqueDefinition Polarity = new(
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

    internal static readonly TechniqueDefinition Paper = new(
        "Practice", "Sheet of paper",
        [
            "1. Write the thought on paper (or in the field below)",
            "2. Read it aloud once",
            "3. Fold, tear, or symbolically release the sheet"
        ],
        "Writing thoughts on paper reduces their emotional charge.",
        TechniqueUiKind.Paper, "Technique #4", "26.01.2023", "Sheet of paper", "When thoughts keep looping", "Thoughts", "Psyche", 3, "Description",
        [new EntryItem { Title = "Thought", Placeholder = "E.g. \"I'll ruin the presentation\"" }],
        Theory(
            "Writing moves the thought outside your head. Destroying the sheet symbolizes letting go.",
            "When the same sentence spins for hours and blocks sleep or focus.",
            "Unload the thought, read it, then destroy the sheet — physically or in imagination.",
            "Thought: \"They'll think I'm incompetent\". After writing and burning the sheet, anxiety often fades."),
        null, null);

    internal static readonly TechniqueDefinition Future = new(
        "Practice", "50 years later",
        [
            "1. State the problem",
            "2. Rate its importance 50 years from now from 0 to 10"
        ],
        "Moving a problem forward in time lowers its subjective importance.",
        TechniqueUiKind.Entry, "Technique #5", "30.01.2023", "50 years later", "Move the problem forward in time", "Importance", "NLP", 2, "Schedule",
        [
            new EntryItem { Title = "Problem", Placeholder = "What feels like a disaster today?" },
            new EntryItem { Title = "Importance in 50 years", Kind = EntryFieldKind.Rating0To10 }
        ],
        Theory(
            "Imagining an event far in the future often dulls its edge — you see it from a wider perspective.",
            "When a setback feels like the end of the world though it's temporary.",
            "Name the problem and rate how much it will matter in fifty years. The score is usually lower.",
            "Problem: \"I wasn't invited to the meeting\". In 50 years: 1–2 out of 10."),
        null, null);

    internal static readonly TechniqueDefinition Hack = new(
        "Practice", "White spot",
        [
            "1. Recall the bothering episode",
            "2. Picture it as a white spot without details",
            "3. Hold the spot until tension eases"
        ],
        "Removing details from a memory often lowers its emotional charge.",
        TechniqueUiKind.Entry, "Technique #6", "30.01.2023", "White spot", "Soften a painful memory", "Episodes", "NLP", 3, "BlurOn",
        [new EntryItem { Title = "Episode", Placeholder = "Briefly: what happened and what hurts most" }],
        Theory(
            "Details fuel the feeling. A white spot removes triggers and gives your nervous system a pause.",
            "When a vivid image from the past hits you with emotion.",
            "Recall the episode, then replace the picture with a plain white spot for 30–60 seconds.",
            "Instead of the offender's face — a white spot. Breathing steadies, mind returns to now."),
        null, null);

    internal static readonly TechniqueDefinition Experience = new(
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
            new EntryItem { Title = "Area", Placeholder = "Relationships, work, health…" },
            new EntryItem { Title = "Attitude toward the area", Placeholder = "How do you feel about it now?" },
            new EntryItem { Title = "Related beliefs", Placeholder = "E.g. \"I can't succeed\"" },
            new EntryItem { Title = "Attitude and past experience", Placeholder = "What changes when you recall the past?" },
            new EntryItem { Title = "Negative experience", Placeholder = "A specific episode or pattern" },
            new EntryItem { Title = "Negative experience rating", Kind = EntryFieldKind.RatingNeg10To10 },
            new EntryItem { Title = "Positive replacement", Placeholder = "What supportive experience is possible?" },
            new EntryItem { Title = "Attitude with the positive", Placeholder = "How would your view of the area change?" },
            new EntryItem { Title = "New experience rating", Kind = EntryFieldKind.RatingNeg10To10 }
        ],
        Theory(
            "Beliefs often rest on one vivid negative experience. Conscious replacement can shift your attitude.",
            "When \"it's always been this way\" and change feels impossible.",
            "Go through the fields in order. Compare the two ratings — aim for the second to be higher.",
            "Area: public speaking. Negative: failed exam (−8). Replacement: good talk (+6)."),
        null, null);

    internal static readonly TechniqueDefinition Copied = new(
        "Practice", "Repeat it",
        [
            "1. Write the wording that bothers you",
            "2. Repeat it aloud as if hearing it for the first time",
            "3. Keep repeating until emotion eases"
        ],
        "Repeating a problem statement \"as for the first time\" lowers emotional intensity.",
        TechniqueUiKind.Copied, "Technique #8", "21.01.2025", "Repeat it", "Hear the problem as if for the first time", "Importance", "NLP", 2, "Repeat",
        [new EntryItem { Title = "Wording", Placeholder = "I'm afraid of losing my job" }],
        Theory(
            "A familiar phrase loses charge when spoken slowly and neutrally, without drama.",
            "When one thought sounds like a verdict in your head.",
            "Write the phrase, then repeat it 5–10 times with pauses, noticing body sensations.",
            "\"Nobody needs me\" — after seven repeats it often sounds absurd and lighter."),
        null, null);

    internal static readonly TechniqueDefinition Extend = new(
        "Practice", "Backup plan",
        [
            "1. Name the problem",
            "2. Write three realistic backup options",
            "3. Notice how anxiety shifts when exits exist"
        ],
        "Backup options reduce inflated importance of a single outcome.",
        TechniqueUiKind.Entry, "Technique #9", "21.01.2025", "Backup plan", "Lower the stakes with a backup", "Importance", "NLP", 3, "Backup",
        [
            new EntryItem { Title = "Problem", Placeholder = "What if the main plan fails?" },
            new EntryItem { Title = "Backup option 1", Placeholder = "E.g. take a freelance project" },
            new EntryItem { Title = "Backup option 2", Placeholder = "E.g. train for a related role" },
            new EntryItem { Title = "Backup option 3", Placeholder = "E.g. temporarily cut expenses" }
        ],
        Theory(
            "When stakes feel all-or-nothing, the mind tightens. Alternatives restore a sense of choice.",
            "Before interviews, exams, or events where failure feels catastrophic.",
            "State the problem and three plausible plan B options — even modest ones.",
            "Problem: \"Won't get the job\". Plan B: internship, referrals, another city."),
        null, null);

    internal static readonly TechniqueDefinition Resize = new(
        "Practice", "Shrink it",
        [
            "1. Describe what bothers you",
            "2. Picture it as an object in front of you",
            "3. Shrink and move it away until interest fades"
        ],
        "Visual shrinking and distance lower emotional interest in a problem.",
        TechniqueUiKind.Entry, "Technique #10", "22.01.2025", "Shrink it", "Make the problem small and distant", "Importance", "NLP", 2, "ZoomOutMap",
        [new EntryItem { Title = "What bothers you", Placeholder = "E.g. a conflict that keeps you awake" }],
        Theory(
            "Image size and distance affect significance. A small far object rarely pulls your nerves.",
            "When a problem feels huge and in your face.",
            "Create an image, gradually shrink it and push it to the horizon for 10–20 seconds.",
            "A fight becomes a tiny dot. Breathing eases, thoughts clear."),
        null, null);

    internal static readonly TechniqueDefinition Check = new(
        "Practice", "Check it",
        [
            "1. Describe what you're looping on",
            "2. Deliberately stop thinking about it for 1–2 minutes",
            "3. Notice what changed in body and mood"
        ],
        "Too much attention inflates importance. A brief pause tests how much the thought controls you.",
        TechniqueUiKind.Entry, "Technique #11", "22.01.2025", "Check it", "See what happens if you let go", "Importance", "NLP", 2, "FactCheck",
        [new EntryItem { Title = "What you're looping on", Placeholder = "Which thought keeps returning?" }],
        Theory(
            "If a thought were vital for survival, letting go would feel dangerous. The experiment usually shows nothing catastrophic happens.",
            "When you can't stop replaying the same topic.",
            "Name the thought, then gently shift attention to breath or sounds around you.",
            "Thought: \"What if I'm ill\". After a pause, anxiety often drops from 7 to 4."),
        null, null);
}

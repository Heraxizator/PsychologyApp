using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.UI.Components;

namespace PsychologyApp.Presentation.Models.Practice.Techniques;

internal static class TechniqueCatalogEnglish
{
    internal static readonly Dictionary<TechniqueId, TechniqueDefinition> Definitions = new()
    {
        [TechniqueId.Spin] = new(
            "Practice", "Spin",
            ["1. Find the episode that bothers you", "2. Identify the key feeling", "3. Rate the feeling on a 10-point scale"],
            "Zivorad Slavinski's Spin is one of the simplest yet most effective techniques. Identify a painful episode and rate the feeling on a 10-point scale.",
            TechniqueUiKind.Entry, "Technique #1", "26.01.2023", "Spin", "Instant neutralization of trauma and shock", "Episodes", "Zivorad Slavinski",
            [new EntryItem { Title = "Episode", Placeholder = "I lost a friend" }, new EntryItem { Title = "Feeling", Placeholder = "Desire for revenge" }]),
        [TechniqueId.Comparison] = new(
            "Practice", "Importance comparison",
            ["1. Name the problem", "2. Identify what mattered in the past", "3. Identify what matters now", "4. Identify what will matter in the future", "5. Compare importance across the three time frames"],
            "Comparing importance helps lower the weight of a problem by contrasting past, present, and future.",
            TechniqueUiKind.Entry, "Technique #2", "26.01.2023", "Importance comparison", "Past, present, and future", "Importance", "NLP",
            [new EntryItem { Title = "Problem", Placeholder = "I have no job" }, new EntryItem { Title = "Past", Placeholder = "Study well" }, new EntryItem { Title = "Present", Placeholder = "Find a job" }, new EntryItem { Title = "Future", Placeholder = "Keep stability" }]),
        [TechniqueId.Polarity] = new(
            "Practice", "Polarities",
            ["1. Find what you like", "2. Find what dissatisfies you", "3. Compare pairs from steps 1 and 2"],
            "The polarity method helps you see opposite aspects of a situation and reduce inner tension.",
            TechniqueUiKind.Polarity, "Technique #3", "26.01.2023", "Polarities", "Working with opposite aspects", "Aspects", "Zivorad Slavinski"),
        [TechniqueId.Paper] = new(
            "Practice", "Sheet of paper",
            ["1. Write thoughts on paper", "2. Fold the sheet or burn it", "3. Notice the relief"],
            "Writing thoughts on paper reduces their emotional charge. Destroying the sheet symbolically releases the experience.",
            TechniqueUiKind.Paper, "Technique #4", "26.01.2023", "Sheet of paper", "Quick clearing of negative thoughts", "Thoughts", "Psyche",
            [new EntryItem { Title = "Negative thought", Placeholder = "Thoughts keep bothering me" }]),
        [TechniqueId.Future] = new(
            "Practice", "50 years later",
            ["1. Formulate the problem", "2. Rate its importance 50 years from now"],
            "The farther and smaller an object appears in imagination, the lower its significance. Moving a problem forward in time reduces its importance.",
            TechniqueUiKind.Entry, "Technique #5", "30.01.2023", "50 years later", "Lowering importance in 10 seconds", "Importance", "NLP",
            [new EntryItem { Title = "Problem", Placeholder = "I was fired" }]),
        [TechniqueId.Hack] = new(
            "Practice", "White spot",
            ["1. Recall the episode that bothers you", "2. Imagine it as a white spot", "3. Hold the spot image instead of details"],
            "If you picture an episode as a white spot without details, the emotional charge often weakens. Repeat until tension drops.",
            TechniqueUiKind.Entry, "Technique #6", "30.01.2023", "White spot", "Neutralizing memories", "Episodes", "NLP",
            [new EntryItem { Title = "Episode", Placeholder = "Recall the episode that bothers you" }]),
        [TechniqueId.Experience] = new(
            "Practice", "Experience modification",
            [
                "1. Choose an area to work on",
                "2. How do you feel about this area?",
                "3. What beliefs are connected to it?",
                "4. How does your attitude change when you think about past experience?",
                "5. Find a negative experience",
                "6. Rate the experience from -10 to 10",
                "7. What positive experience could replace the negative one?",
                "8. How would your attitude change if the negative were replaced?",
                "9. Rate the new experience from -10 to 10",
                "10. Repeat steps 7–9 if needed"
            ],
            "Experience modification (EMO) works with limiting beliefs and behavior patterns by replacing negative experience with positive.",
            TechniqueUiKind.Entry, "Technique #7", "08.02.2023", "Experience modification", "Working through limits, beliefs, and behavior", "Episodes", "Filip Slavinski",
            [
                new EntryItem { Title = "Area to work on", Placeholder = "E.g. relationships, work" },
                new EntryItem { Title = "Attitude toward the area", Placeholder = "How do you feel about this area?" },
                new EntryItem { Title = "Related beliefs", Placeholder = "What beliefs are connected to it?" },
                new EntryItem { Title = "Attitude and past experience", Placeholder = "How does your attitude change when you think about past experience?" },
                new EntryItem { Title = "Negative experience", Placeholder = "Describe the negative experience" },
                new EntryItem { Title = "Negative experience rating", Placeholder = "From -10 to 10" },
                new EntryItem { Title = "Positive replacement", Placeholder = "What positive experience could replace the negative one?" },
                new EntryItem { Title = "Attitude with positive", Placeholder = "How would your attitude change if the negative were replaced?" },
                new EntryItem { Title = "New experience rating", Placeholder = "From -10 to 10" }
            ]),
        [TechniqueId.Copied] = new(
            "Practice", "Repeat it",
            ["1. Name what bothers you", "2. Repeat the phrase as if hearing it for the first time"],
            "Repeating the problem statement 'as for the first time' lowers emotional intensity and helps you see the situation differently.",
            TechniqueUiKind.Copied, "Technique #8", "21.01.2025", "Repeat it", "Simple importance-lowering technique", "Importance", "NLP",
            [new EntryItem { Title = "Wording", Placeholder = "I'm afraid of losing my job" }]),
        [TechniqueId.Extend] = new(
            "Practice", "Backup plan",
            ["1. Name the problem", "2. Find alternatives"],
            "Have backup options: 'What will I do if the goal is not reached?' Alternatives reduce inflated importance.",
            TechniqueUiKind.Entry, "Technique #9", "21.01.2025", "Backup plan", "Lowering importance in 60 seconds", "Importance", "NLP",
            [new EntryItem { Title = "Problem", Placeholder = "Problem" }, new EntryItem { Title = "Alternative 1", Placeholder = "Alternative 1" }, new EntryItem { Title = "Alternative 2", Placeholder = "Alternative 2" }, new EntryItem { Title = "Alternative 3", Placeholder = "Alternative 3" }]),
        [TechniqueId.Resize] = new(
            "Practice", "Shrink it",
            ["1. Find what bothers you", "2. Imagine it as an object", "3. Shrink it and move it away"],
            "Picture the problem small and far away — interest in it often fades. Out of sight, out of mind.",
            TechniqueUiKind.Entry, "Technique #10", "22.01.2025", "Shrink it", "Simple importance-lowering technique", "Importance", "NLP",
            [new EntryItem { Title = "What bothers you", Placeholder = "Describe what bothers you" }]),
        [TechniqueId.Check] = new(
            "Practice", "Check it",
            ["1. Find what bothers you", "2. Consciously stop thinking about it", "3. Observe how your state changes"],
            "Too much attention leads to too much importance. Try thinking less about what bothers you.",
            TechniqueUiKind.Entry, "Technique #11", "22.01.2025", "Check it", "Simple importance-lowering technique", "Importance", "NLP",
            [new EntryItem { Title = "What bothers you", Placeholder = "Describe what bothers you" }])
    };
}

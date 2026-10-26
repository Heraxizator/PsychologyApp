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

    /// <summary>Common topics for a "problem" field: a start to tap and then add a word or two, not a full answer.</summary>
    private static readonly string[] ProblemTopics =
        ["A fight with someone close", "Something at work", "Conflict with my boss", "Studies or an exam", "Health", "Money", "Loneliness", "A failure", "A breakup", "Relationship with my parents", "Children", "Moving", "The future", "Criticism of me"];

    private static readonly string[] BackupOptions =
        ["Ask for help", "Wait and try again", "Find another way", "Lower my demands on myself", "Take a break", "Talk it over with someone close", "See a specialist", "Find side work", "Take a course", "Break the task into steps", "Postpone the decision", "Start small"];

    private static readonly string[] HarshThoughts =
        ["\"I'll ruin everything\"", "\"They'll judge me\"", "\"I can't cope\"", "\"Nobody needs me\"", "\"It's a disaster\"", "\"Nothing will work out\"", "\"I'm worse than others\"", "\"Everything always goes wrong\"", "\"It's my fault\"", "\"Something is wrong with me\"", "\"This will never end\"", "\"I have to be perfect\""];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "Practice", "Spin",
        [
            "1. Describe the episode that bothers you",
            "2. Name the main feeling",
            "3. Rate the feeling from 0 to 10"
        ],
        "Zivorad Slavinski's Spin helps lower the emotional charge of a painful memory.",
        TechniqueUiKind.Entry, "Technique #1", "26.01.2023", "Spin", "Lower emotional charge in 3 steps", "Hard moment", "Zivorad Slavinski", 5, "Autorenew",
        [
            new TechniqueEntrySeed("Episode", "E.g. yesterday's conversation you can't let go", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Key feeling", "Hurt, fear, anger…", Role: EntryFieldRole.Emotion),
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
        TechniqueUiKind.Entry, "Technique #2", "26.01.2023", "Importance comparison", "When the problem feels too big", "Scale of the problem", "NLP", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("Problem", "What feels unbearable right now?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Mattered in the past", "E.g. studies, relationships, career", Suggestions: ["Studies", "Family", "Friends", "Health", "Career", "Hobbies", "Sport", "Love", "Travel", "Freedom", "Recognition", "Home"]),
            new TechniqueEntrySeed("Matters now", "What takes most of your attention?", Suggestions: ["Work", "Family", "Health", "Money", "Relationships", "Rest", "Children", "Friends", "Studies", "Sleep", "Peace of mind", "Self-growth"]),
            new TechniqueEntrySeed("Will matter in the future", "What are you aiming for?", Suggestions: ["Stability", "Family", "Freedom", "Health", "Growth", "People I love", "A home of my own", "Work I love", "Travel", "Calm", "Children", "Savings"]),
            new TechniqueEntrySeed("What changed?", "Does the problem still dominate everything?", Suggestions: ["The problem is only part of life", "More matters than it seemed", "It's a passing stage", "It feels less sharp", "I feel calmer", "I see I've coped before", "Hope appeared", "Nothing has changed yet"])
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
        TechniqueUiKind.Polarity, "Technique #3", "26.01.2023", "Polarities", "See both sides of a situation", "Different sides", "Zivorad Slavinski", 5, "Contrast",
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
        [new TechniqueEntrySeed("Thought", "E.g. \"I'll ruin the presentation\"", Suggestions: HarshThoughts)],
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
        TechniqueUiKind.Entry, "Technique #5", "30.01.2023", "50 years later", "Move the problem forward in time", "Scale of the problem", "NLP", 2, "Schedule",
        [
            new TechniqueEntrySeed("Problem", "What feels like a disaster today?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
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
        TechniqueUiKind.Entry, "Technique #6", "30.01.2023", "White spot", "Soften a painful memory", "Hard moment", "NLP", 3, "BlurOn",
        [new TechniqueEntrySeed("Episode", "Briefly: what happened and what hurts most", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
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
        TechniqueUiKind.Entry, "Technique #7", "08.02.2023", "Experience modification", "Rewrite a limiting experience", "Hard moment", "Filip Slavinski", 15, "Psychology",
        [
            new TechniqueEntrySeed("Area", "Relationships, work, health…", Suggestions: ["Relationships", "Work", "Health", "Money", "Public speaking", "Family", "Studies", "Appearance", "Friendship", "Creativity", "Sport", "Career"]),
            new TechniqueEntrySeed("Attitude toward the area", "How do you feel about it now?", Suggestions: ["Anxiety", "Self-doubt", "Irritation", "Indifference", "Interest", "Tiredness", "Fear", "Shame", "Hope", "Anger", "Confusion", "Calm"]),
            new TechniqueEntrySeed("Related beliefs", "E.g. \"I can't succeed\"", Suggestions: ["\"I can't succeed\"", "\"I'm not good enough\"", "\"They won't accept me\"", "\"Nothing will change\"", "\"I have to be perfect\"", "\"I'm unlucky\"", "\"I don't deserve it\"", "\"Others are better\"", "\"Mistakes are not allowed\"", "\"I'm too old for this\""]),
            new TechniqueEntrySeed("Attitude and past experience", "What changes when you recall the past?", Suggestions: ["It gets heavier", "It gets calmer", "Nothing changes", "Anger comes up", "Sadness comes up", "Fear comes up", "I remember I coped", "I want to avoid it"]),
            new TechniqueEntrySeed("Negative experience", "A specific episode or pattern"),
            new TechniqueEntrySeed("Negative experience rating", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("Positive replacement", "What supportive experience is possible?", Suggestions: ["A time I succeeded", "A time I was supported", "A time I coped on my own", "A time I was praised", "A time I helped someone", "A time I learned something new", "A time I didn't give up", "A time I was accepted as I am"]),
            new TechniqueEntrySeed("Attitude with the positive", "How would your view of the area change?", Suggestions: ["Hope appears", "More calm", "I want to try again", "It gets interesting", "Confidence appears", "It gets lighter", "I feel strength", "Still hard to believe"]),
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
        TechniqueUiKind.Copied, "Technique #8", "21.01.2025", "Repeat it", "Hear the problem as if for the first time", "Scale of the problem", "NLP", 2, "Repeat",
        [new TechniqueEntrySeed("Wording", "I'm afraid of losing my job", Suggestions: HarshThoughts)],
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
        TechniqueUiKind.Entry, "Technique #9", "21.01.2025", "Backup plan", "Lower the stakes with a backup", "Scale of the problem", "NLP", 3, "Backup",
        [
            new TechniqueEntrySeed("Problem", "What if the main plan fails?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Backup option 1", "E.g. take a freelance project", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Backup option 2", "E.g. train for a related role", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Backup option 3", "E.g. temporarily cut expenses", Suggestions: BackupOptions)
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
        TechniqueUiKind.Entry, "Technique #10", "22.01.2025", "Shrink it", "Make the problem small and distant", "Scale of the problem", "NLP", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("What bothers you", "E.g. a conflict that keeps you awake", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
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
        TechniqueUiKind.Entry, "Technique #11", "22.01.2025", "Check it", "See what happens if you let go", "Scale of the problem", "NLP", 2, "FactCheck",
        [new TechniqueEntrySeed("What you're looping on", "Which thought keeps returning?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
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
        TechniqueUiKind.Entry, "Technique #12", "12.06.2026", "Observer position", "Step back from emotions and see the situation from outside", "Hard moment", "NLP", 4, "Visibility",
        [
            new TechniqueEntrySeed("Situation", "What happened or what are you replaying in your mind?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Feelings from first person", "What do you feel while inside the situation?", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("What the observer sees", "What would a calm outside observer notice?", Suggestions: ["Two people arguing", "A tired, upset person", "An ordinary work situation", "Each one defends their side", "In a week this will be forgotten", "People misunderstood each other", "A person trying very hard", "Unpleasant, but not dangerous", "Everyone has their own view", "The person needs rest"]),
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
        TechniqueUiKind.Entry, "Technique #13", "12.06.2026", "Resource anchor", "Restore support through gesture and memory", "Hard moment", "NLP", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Resource memory", "When did you feel strong, calm, or joyful?", Suggestions: ["A talk that went well", "A holiday by the sea", "A hug from someone close", "Finishing something hard", "A walk in nature", "Passing an exam", "The birth of a child", "Winning a competition", "An evening with friends", "Praise from my manager", "First day at a new job", "A morning without rush"]),
            new TechniqueEntrySeed("Body sensations", "Where do you feel it? Warmth, lightness, grounding…", Suggestions: ["Warmth in the chest", "Lightness", "Open shoulders", "Even breathing", "Steady legs", "A smile", "A calm heart", "A straight back", "Energy in my hands", "A relaxed belly"]),
            new TechniqueEntrySeed("Gesture or anchor", "E.g. clench fist, touch wrist", Suggestions: ["Clench a fist", "Touch the wrist", "Join thumb and index finger", "Hand on the heart", "A deep breath", "Squeeze an earlobe", "Open the shoulders", "Stroke my palm"]),
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
        TechniqueUiKind.Entry, "Technique #14", "12.06.2026", "5-4-3-2-1 grounding", "Return to the present through the senses", "Hard moment", "Psyche", 3, "Nature",
        [
            new TechniqueEntrySeed("5 things I see", "E.g. window, cup, book, lamp, hand", Suggestions: ["Window", "Table", "Phone", "Computer", "Laptop", "Monitor", "Keyboard", "Cup", "Lamp", "Hand", "Door", "Book", "Chair", "Wall", "Picture", "Plant", "Clock", "Wardrobe", "Sky", "Car"]),
            new TechniqueEntrySeed("4 things I feel with my body", "E.g. chair, floor, clothes, air temperature", Suggestions: ["Chair", "Floor under my feet", "Clothes", "Coolness", "Warmth", "Phone in my hand", "Back of the chair", "Fabric on my skin", "Breath in my chest", "Wind", "Computer mouse", "Keys", "Pillow", "Watch on my wrist"]),
            new TechniqueEntrySeed("3 sounds", "E.g. street, clock, your breath", Suggestions: ["Street", "Clock", "Breathing", "Voices", "Fridge", "Wind", "Silence", "Computer", "Cars", "Birds", "Music", "Air conditioner", "Footsteps", "Rain"]),
            new TechniqueEntrySeed("2 smells", "E.g. coffee, fresh air", Suggestions: ["Coffee", "Fresh air", "Soap", "Food", "Perfume", "Almost nothing", "Tea", "Bread", "Books", "Flowers", "Hand cream", "Wood"]),
            new TechniqueEntrySeed("1 taste or pleasant sensation", "E.g. water in mouth, soft fabric", Suggestions: ["Water", "Tea", "Soft fabric", "Warm palms", "Fresh mouth", "Coffee", "Mint gum", "Chocolate", "An apple", "A warm blanket"])
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
        TechniqueUiKind.Entry, "Technique #15", "04.07.2026", "Box breathing", "Calm the body through breath rhythm", "Hard moment", "Psyche", 3, "Air",
        [
            new TechniqueEntrySeed("Tension before", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notes", "What did you notice in your body or breath during the practice?", Suggestions: ["Shoulders dropped", "Breathing slowed", "Felt warmer", "Hard to hold the pause", "Thoughts distracted me", "I felt calmer", "Felt dizzy", "Heart slowed down", "Wanted to yawn", "Nothing special"]),
            new TechniqueEntrySeed("Tension after", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
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
        TechniqueUiKind.Entry, "Technique #16", "04.07.2026", "One small step", "Restore momentum through minimal action", "Hard moment", "CBT", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("What feels heavy", "What seems hard, boring, or impossible right now?", Suggestions: ["Tiredness", "No energy", "Too much to do", "Boredom", "Scared to start", "Nothing seems to matter", "Slept badly", "No mood", "Loneliness", "Procrastination", "Headache", "Don't know where to start"], Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Smallest step today", "E.g. get up, wash face, send one message", Suggestions: ["Get out of bed", "Wash my face", "Drink some water", "Send one message", "Go outside for 5 minutes", "Tidy one thing", "Open a window", "Stretch for 2 minutes", "Take a shower", "Eat something", "Call someone close", "Open the file I need", "Write a plan for the day", "Wash one cup"]),
            new TechniqueEntrySeed("When you will do it", "A specific time or moment in the day", Suggestions: ["Right now", "In 15 minutes", "In an hour", "In the morning", "After breakfast", "After lunch", "In the evening", "Before bed", "Tomorrow morning", "At the weekend"]),
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
        TechniqueUiKind.Entry, "Technique #17", "04.07.2026", "Thought record", "Unpack a thought and reduce emotional charge", "Hard moment", "CBT", 7, "EditNote",
        [
            new TechniqueEntrySeed("Situation", "What happened? Where and when?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Automatic thought", "What did you think in that moment?", Suggestions: HarshThoughts),
            new TechniqueEntrySeed("Emotion", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Alternative thought", "What else might be true? A more balanced view", Role: EntryFieldRole.Reframe),
            new TechniqueEntrySeed("Emotion now", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
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
        TechniqueUiKind.Entry, "Technique #18", "04.07.2026", "Kind words to yourself", "Soften self-criticism through support", "Hard moment", "CBT", 5, "Favorite",
        [
            new TechniqueEntrySeed("What you criticize yourself for", "What do you tell yourself or blame yourself for?", Suggestions: ["A mistake at work", "Didn't get it done", "Snapped at someone close", "Comparing myself to others", "Putting things off again", "My appearance", "My weight", "Said the wrong thing", "Not a good enough parent", "Earning too little", "Broke a promise", "Too sensitive"]),
            new TechniqueEntrySeed("What you would tell a friend", "How would you support someone close in this situation?", Role: EntryFieldRole.Kindness),
            new TechniqueEntrySeed("Phrase to yourself", "The same kindness, directed at you", Role: EntryFieldRole.SelfKindness),
            new TechniqueEntrySeed("Warmth / relief", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Self-criticism rarely motivates — it more often fuels shame and paralysis. Self-compassion does not remove responsibility, but gives support.",
            "After a mistake, comparing yourself to others, or feeling not good enough.",
            "Write the criticism, then answer yourself as you would a friend — with respect and without dismissal.",
            "Criticism: I ruined everything again. To friend: you tried, this is fixable. To self: I can learn from this. Warmth: 3 → 6."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BodyScan = new(
        "Practice", "Where is it in the body?",
        [
            "1. Rate your tension now",
            "2. Slowly move your attention through the body from head to toe",
            "3. Find the place where the sensation is strongest",
            "4. Describe it: what it is like, large or small, warm or cold",
            "5. Name the feeling that goes with it",
            "6. Take 5 slow out-breaths as if each one passes through that place, and rate the tension again"
        ],
        "Body scan: notice where a feeling lives, name it, and let the body soften without fighting the sensation.",
        TechniqueUiKind.Entry, "Technique #19", "16.10.2026", "Where is it in the body?", "Notice where a feeling lives in the body and soften the tension", "Hard moment", "Mindfulness", 4, "Accessibility",
        [
            new TechniqueEntrySeed("Tension now", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Where in the body do you feel it?", "For example: chest, throat, shoulders", Suggestions: ["Head", "Forehead", "Eyes", "Jaw", "Throat", "Neck", "Shoulders", "Chest", "Heart", "Stomach", "Back", "Lower back", "Arms", "Hands", "Legs", "The whole body"]),
            new TechniqueEntrySeed("What is the sensation like?", "For example: tightness, heaviness, a lump", Suggestions: ["Tightness", "Heaviness", "A lump", "Pressure", "Tension", "Heat", "Cold", "Trembling", "Tingling", "Throbbing", "Emptiness", "Numbness", "Pain", "Hard to describe"]),
            new TechniqueEntrySeed("What feeling goes with it?", "For example: anxiety, hurt, tiredness", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Tension after", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Feelings echo in the body: anxiety as tightness in the chest, hurt as a lump in the throat, tiredness as heavy shoulders. When we notice the sensation and name the feeling, tension usually eases. This is the core of the body scan in mindfulness programmes.",
            "When you feel anxious or heavy but cannot tell what exactly you feel; when the body is tense after a hard conversation or day. The practice does not explain illnesses and does not replace a doctor: if a sensation is new, strong or does not go away, see a doctor.",
            "Sit comfortably and slowly move your attention from the top of your head to your feet. Stop where the sensation is brightest. Do not change it or fight it: describe it and name the feeling. Then take a few slow out-breaths as if each one passes through that place.",
            "Tension 6. Where: chest. Sensation: tightness, like a fist. Feeling: anxiety before a conversation. After 5 out-breaths the tightness is softer, tension 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Flashback = new(
        "Practice", "A memory took over",
        [
            "1. Rate how strong it is",
            "2. Tell yourself: \"This is a memory. That was then, and now is today\"",
            "3. Name where you are and what time of day it is",
            "4. Feel the support under your feet, clench and release your fists",
            "5. Name three things you can see",
            "6. Five slow out-breaths, cold water, and rate again"
        ],
        "Help with a flashback: come back from the memory to the present through time, place, the body and the senses.",
        TechniqueUiKind.Entry, "Technique #20", "16.10.2026", "A memory took over", "Come back to the present when a hard memory takes over", "Hard moment", "Trauma", 3, "MyLocation",
        [
            new TechniqueEntrySeed("How strong it is", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Where you are and the time of day", "For example: at home, evening", Suggestions: ["At home", "At work", "Outside", "On transport", "Visiting someone", "Morning", "Afternoon", "Evening", "Night"]),
            new TechniqueEntrySeed("Three things you can see", "For example: window, cup, lamp", Suggestions: ["Window", "Table", "Phone", "Cup", "Lamp", "Door", "Chair", "Wall", "Picture", "Plant", "Clock", "Sky"]),
            new TechniqueEntrySeed("How strong it is now", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "A flashback is a memory of trauma that is lived as if it were happening again: with the same feelings and body reactions. The brain mixes up \"then\" and \"now\". Time, place, the body's support and what you see and hear help bring you back to the present.",
            "When a memory of a hard event takes over, with flashes of images and the feeling \"I'm back there\". Frequent flashbacks are a common sign of a post-traumatic reaction, and they respond well to treatment with a trauma specialist.",
            "Do not try to work through the memory: right now the only task is to come back. Tell yourself where you are and what day it is, press your feet down, name what you see, breathe out slowly, use cold.",
            "Strength 8. \"That was then; now it's 2026, I'm at home, it's evening.\" Feet on the floor; I see a window, a cup, a lamp. Cold water. Now 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Nightmare = new(
        "Practice", "Rewrite a nightmare",
        [
            "1. Choose one recurring nightmare, not the hardest one to begin with",
            "2. Describe it briefly, without hard details",
            "3. Decide what you change in it: the ending, the place, yourself, someone nearby",
            "4. Write the new version of the dream in full, to a calm ending",
            "5. Every day for 10 to 20 minutes, imagine the new version in detail",
            "6. After one or two weeks, see how the nightmares have changed"
        ],
        "Imagery rehearsal: a recurring nightmare is rewritten while awake, and the brain starts dreaming the new version.",
        TechniqueUiKind.Entry, "Technique #21", "16.10.2026", "Rewrite a nightmare", "Make a recurring nightmare rarer and weaker", "Sleep", "Trauma", 15, "NightsStay",
        [
            new TechniqueEntrySeed("How hard this nightmare is", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("What the nightmare is about, briefly", "One or two sentences, without hard details", Suggestions: ["Someone is chasing me", "I can't get away", "I'm back where it was frightening", "I'm losing someone close", "I can't scream", "I'm falling", "I'm being attacked", "I'm late and can't make it"]),
            new TechniqueEntrySeed("What you change", "Anything you like: it's your dream", Suggestions: ["The ending", "The place", "I become stronger", "A helper appears", "I can speak and shout", "The danger disappears", "Everything turns safe", "I notice it's a dream"]),
            new TechniqueEntrySeed("The new version of the dream", "Describe it in full, from the start to a calm ending", Suggestions: ["I notice it's a dream", "Someone who protects me appears", "I find myself in a safe place", "I calmly walk away", "It all ends well"]),
            new TechniqueEntrySeed("When I'll rehearse the new version", "Every day, 10 to 20 minutes", Suggestions: ["In the morning", "During the day, on a break", "In the evening, a couple of hours before bed", "Every day at the same time"]),
            new TechniqueEntrySeed("How hard it is now", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Recurring nightmares after trauma or stress are kept going by habit: the brain plays the same plot again and again. In imagery rehearsal therapy the plot is rewritten while awake, and the new version is rehearsed in the mind every day. Research shows that this makes nightmares rarer and weaker and sleep better.",
            "With recurring nightmares that make you afraid to fall asleep. If the nightmare is a direct replay of a trauma and even a short description brings strong horror, do this together with a trauma specialist.",
            "Start with an easier nightmare. Do not write out the frightening parts in detail: a couple of sentences is enough. Change the dream any way you like; the new version does not have to be realistic. Rehearse it during the day, not right before sleep, 10 to 20 minutes a day.",
            "Nightmare: someone is chasing me and I can't get away. Change: I notice it's a dream, turn around, and it's just a shadow; I calmly walk home. I rehearse it every day in the afternoon. After two weeks the nightmare comes less often."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Balloons = new(
        "Practice", "Pop the balloons",
        [
            "1. Rate your tension",
            "2. Pop the balloons with a tap, without rushing",
            "3. With each pop, breathe out slowly",
            "4. Play the round to the end",
            "5. Rate your tension after"
        ],
        "A short game moves attention from anxious thoughts to a simple action and sensations.",
        TechniqueUiKind.Entry, "Technique #22", "26.10.2026", "Pop the balloons", "A short break to let tension out", "Hard moment", "Psyche", 3, "BubbleChart",
        [
            new TechniqueEntrySeed("Tension before", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notes", "What changed while you were popping balloons?", Suggestions: ["I feel lighter", "My mind took a break", "I wanted to smile", "My breathing evened out", "The irritation passed", "My hands relaxed", "Nothing changed", "I want another round"]),
            new TechniqueEntrySeed("Tension after", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "When anxiety or irritation peaks, thoughts are hard to stop by willpower. A simple action that needs your eyes and hands switches the brain for a few minutes and lets tension ease. It is not a treatment but a way through an acute minute.",
            "When irritation, tension or anxiety takes over and you need a short break. If the game only helps you avoid a problem that is worth solving, come back to it once you are calmer.",
            "Don't hurry: you don't have to hit every balloon. Breathe out slowly with each pop. The round ends by itself; there is no endless game.",
            "Tension 7/10 after an argument. A round of 20 balloons, a breath out with each pop. Tension 4/10, and I could calmly think about what to answer."),
        null, null);
}

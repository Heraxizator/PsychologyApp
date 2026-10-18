using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Articles;

public static partial class ArticleContent
{
    private static readonly Course[] CoursesEn =
    [
        new(
            CourseIds.Sleep,
            "Sleep without pills",
            "Seven days of cognitive behavioural therapy for insomnia, the best-studied way to get sleep back",
            "Bedtime",
            "isi",
            "ISI-7",
            [
                new(
                    "How sleep works",
                    4,
                    [
                        P("Sleep can't be \"switched on\" by effort. Two forces prepare it: sleep pressure, which builds all day while you are awake, and the internal clock, which makes us sleepy and alert at the same times each day."),
                        P("In insomnia both forces get knocked off: people go to bed earlier to \"catch up\", lie awake for a long time, nap in the day, get up at different times. Sleep pressure has no time to build, and the clock gets confused."),
                        Cycle("The insomnia loop", "A bad night", "Worry: \"I won't sleep again\"", "Trying hard to fall asleep", "Tension keeps sleep away"),
                        P("This course is built on cognitive behavioural therapy for insomnia (CBT-I). Medical guidelines recommend it first, before sleeping pills: the effect lasts after the course ends."),
                        Warning("When to see a doctor first"),
                        List(
                            "Loud snoring, pauses in breathing during sleep, strong daytime sleepiness",
                            "Unpleasant sensations in the legs in the evening that make you want to move",
                            "Insomnia together with low mood for more than two weeks",
                            "You drive or operate machinery and are very sleepy in the day")
                    ],
                    "Every morning write down: when you went to bed, roughly when you fell asleep, how many times you woke, when you got up and how you slept from 0 to 10. No clock at night, roughly, by feel.",
                    "When do you usually go to bed and get up? What gets in the way of your sleep most?"),
                new(
                    "The bed is for sleep",
                    4,
                    [
                        P("If you often lie awake in bed, worry or look at your phone there, the brain links the bed with being awake. That is why you can nod off in an armchair and be wide awake the moment you lie down."),
                        H("Rules that restore the link"),
                        List(
                            "Go to bed only when you feel sleepy, not just tired",
                            "If you can't sleep for about 20 minutes, get up and do something calm in dim light. Come back when you feel sleepy",
                            "The bed is only for sleep and intimacy: no phone, series or work",
                            "Get up at the same time every day, even after a bad night and at weekends",
                            "Don't nap, and if you really must, no more than 20 minutes and before 3 pm"),
                        Quiz("The night was bad. What is better in the morning?", 1, "A fixed wake-up time is the main anchor of sleep. If you sleep in, there will be less sleep pressure by evening, and the next night will be hard too.", "Sleep in to recover", "Get up at the usual time", "Get up, then have a good nap in the day")
                    ],
                    "Choose a wake-up time you can keep all seven days, and get up at it. If you can't sleep for about 20 minutes, get out of bed.",
                    "Which wake-up time did you choose? What was hard about the \"get up if you can't sleep\" rule?"),
                new(
                    "The sleep window",
                    5,
                    [
                        P("The longer a person lies awake in bed, the lighter and more broken sleep becomes. A technique called sleep compression helps: time in bed is brought closer to how much you actually sleep."),
                        H("How to work out the window"),
                        List(
                            "From your notes of the last few days, work out how long you sleep on average",
                            "Add 30 minutes: this is your time in bed, but not less than six hours",
                            "Count it back from your chosen wake-up time: that gives your bedtime",
                            "Don't go to bed before that time, even if you really want to"),
                        P("The first days you will be sleepy: it means sleep pressure is building. After a week, if you sleep most of the time in bed, the window can be extended by 15 minutes."),
                        Warning("Take care"),
                        P("Don't compress sleep if you have bipolar disorder or epilepsy, if you are pregnant, or if sleepiness makes driving or work dangerous. In these cases, discuss sleep with a doctor.")
                    ],
                    "Work out your sleep window and keep it until the end of the course: no bed before your bedtime, up at the same time.",
                    "What is your sleep window? How did the first night with it go?"),
                new(
                    "An evening that prepares for sleep",
                    4,
                    [
                        P("Sleep comes more easily when body and mind have time to slow down. The hour before bed is the bridge from day to sleep."),
                        H("What helps"),
                        List(
                            "An hour before bed: dim light and calm activities without screens",
                            "Coffee, strong tea and energy drinks no later than 2 pm",
                            "Alcohol helps you fall asleep but makes sleep shallow and wakes you towards morning",
                            "A heavy dinner 2–3 hours before bed",
                            "In the morning, daylight for at least 15 minutes: it sets the internal clock"),
                        H("Relaxing before sleep"),
                        P("Slow breathing with a long out-breath lowers arousal. Try it now."),
                        Breathe("Breathe in for 4, out for 6")
                    ],
                    "Create your own ritual for the last hour before bed and do it every evening. Take the phone out of the bedroom, or at least off the bed.",
                    "What went into your evening ritual? What turned out to be hard?"),
                new(
                    "Thoughts that keep you awake",
                    4,
                    [
                        P("The mind often starts working precisely in bed: during the day there was no time for thoughts. So it helps to give them time earlier."),
                        H("Time for thoughts"),
                        List(
                            "In the evening, a couple of hours before bed, set aside 15 minutes",
                            "Write down everything that worries you, and for each item one next step or \"nothing I can do for now\"",
                            "If the thought comes back in bed, tell yourself: \"I've written this down, I'll deal with it tomorrow\""),
                        Quiz("In bed you think: \"If I don't sleep, I'll fail everything tomorrow.\" What is closer to the truth?", 1, "The thought exaggerates. After a bad night things are harder, but most people still get their tasks done. The stronger the fear about tomorrow, the harder it is to fall asleep.", "It's true, failure is inevitable", "After a bad night it's harder, but I usually manage the main things", "Better not to go to bed at all"),
                        Practice(TechniqueId.Observer, "Observer position")
                    ],
                    "Every evening, set aside 15 minutes to write down what worries you, and don't take those thoughts to bed.",
                    "Which thoughts come most often at night? Did writing them down help?"),
                new(
                    "Stop fighting sleep",
                    4,
                    [
                        P("The harder you try to sleep, the worse it goes. Sleep is like a shy bird: it comes when no one chases it."),
                        H("What helps let go of control"),
                        List(
                            "Put the clock out of sight: counting \"five hours left\" wakes you up",
                            "Allow yourself not to sleep: \"I'll lie here and rest, sleep will come when it comes\"",
                            "A bad night is not a disaster: the body recovers even after incomplete sleep",
                            "Don't cancel plans because of a bad night: that way insomnia doesn't run your life"),
                        P("Compare your notes from the first days with now: how have the time to fall asleep, the number of awakenings and the sleep rating changed?")
                    ],
                    "Keep the clock out of sight at night. After a bad night, live the day as usual.",
                    "What has changed in your sleep over these days, according to your notes?"),
                new(
                    "Summary and plan",
                    4,
                    [
                        P("In a week you have gone through the core of CBT for insomnia. Sleep recovers gradually: many people notice improvement in two to four weeks if they keep to the rules."),
                        H("What to keep doing"),
                        List(
                            "One wake-up time every day",
                            "Go to bed only when sleepy, and get up if you can't sleep",
                            "If you sleep most of the night, extend the window by 15 minutes a week",
                            "The evening ritual and time for thoughts"),
                        H("If insomnia comes back"),
                        P("After stress, illness or travel, sleep can go off track again. That's normal: go back to the rules of this course, and it will recover faster than the first time."),
                        P("Take the ISI-7 index again and compare with your result at the start. If there is no improvement, see a doctor: there are sleep specialists who provide CBT-I."),
                        TestsTab("ISI-7 Index", "isi")
                    ],
                    "Write down your plan for if sleep goes off track again.",
                    "What in the course helped most? What will you keep doing?")
            ]),
        new(
            CourseIds.Anxiety,
            "Anxiety under control",
            "Six days: understand how anxiety works, calm the body, check frightening thoughts and take a first step towards the fear",
            "Psychology",
            "gad7",
            "GAD-7",
            [
                new(
                    "How anxiety works",
                    4,
                    [
                        P("Anxiety is an alarm meant to warn of danger. The trouble starts when it goes off too often, too loudly, and doesn't switch off for a long time."),
                        Cycle("The anxiety loop", "A frightening thought", "Anxiety in the body", "Avoidance or a safety net", "Relief for a minute", "The brain learns: \"it was dangerous\""),
                        P("What keeps anxiety going is not the fear itself but what we do so as not to feel it: avoiding, re-checking, seeking reassurance, going over thoughts. Step by step, the course works through each link of this loop."),
                        P("At the start of the course it is worth taking the GAD-7 scale: at the end you will compare the result."),
                        TestsTab("GAD-7 Scale", "gad7")
                    ],
                    "Three times today, when you feel anxious, notice: what happened, what the thought was, what the body felt and what you did.",
                    "When and where does anxiety show up most often?"),
                new(
                    "Calming the body",
                    4,
                    [
                        P("When anxiety is high, it is hard to argue with thoughts. First it helps to calm the body: a slow out-breath switches on the system that brakes the fight-or-flight response."),
                        Breathe("Breathe in for 4, out for 6"),
                        P("If the anxiety is strong and everything feels unreal, bring your attention back to the present through your senses."),
                        Practice(TechniqueId.Grounding, "5-4-3-2-1 grounding"),
                        P("Important: breathing is not a way to get rid of anxiety at any cost. It helps you get through the peak, not avoid situations.")
                    ],
                    "Twice a day, two minutes of slow breathing, even when calm: that way the skill is there when you need it.",
                    "What did you notice in your body when you breathed slowly?"),
                new(
                    "Checking a frightening thought",
                    5,
                    [
                        P("Anxious thoughts sound like facts: \"I'll embarrass myself\", \"something terrible will happen\". But they are predictions, and they can be checked."),
                        H("Questions to ask the thought"),
                        List(
                            "What facts support this thought, and what facts go against it?",
                            "What happened in similar cases before?",
                            "What is most likely, rather than most frightening?",
                            "If the worst happens, how will I cope with it?",
                            "What would I tell a friend with this thought?"),
                        Quiz("\"I will definitely mess it up.\" Which thinking trap is this?", 1, "It is fortune-telling: the thought treats something that has not happened yet as a fact.", "Mind-reading", "Fortune-telling", "Discounting the positive"),
                        Practice(TechniqueId.ThoughtRecord, "Thought record")
                    ],
                    "Once today, check an anxious thought with a thought record.",
                    "Which thought did you check? How much did you believe it before and after?"),
                new(
                    "Worry time",
                    4,
                    [
                        P("Worry feels useful: \"if I think more, I'll be ready\". In fact, going over things rarely leads to a solution and keeps anxiety high."),
                        Quiz("How do you tell solving from going in circles?", 0, "Solving moves towards a step: what can I do, and when. Going in circles produces new \"what ifs\".", "Solving ends in a step, going in circles in a new \"what if\"", "Going in circles takes longer", "You can't"),
                        H("How to postpone worry"),
                        List(
                            "Choose 15–20 minutes a day, not before bed",
                            "When worry comes at another time, write the topic down in one line and tell yourself: \"I'll think about it in my worry time\"",
                            "In your worry time, go through the list: plan a step for what you can do, let go of what you can't",
                            "Often, by the appointed time, the topic no longer seems important"),
                        Practice(TechniqueId.Observer, "Observer position")
                    ],
                    "Set yourself a worry time and postpone anxious thoughts to it.",
                    "Did you manage to postpone worry? What happened to the topics by the appointed time?"),
                new(
                    "Safety nets and avoidance",
                    4,
                    [
                        P("The strongest thing keeping anxiety going is avoidance. It can be open (I don't go, I don't call) or hidden: safety nets people use to get through a situation."),
                        Check("Do I have safety nets?", 2, "You have several safety nets. They keep the fear going: the brain thinks things turned out fine only thanks to them. Try giving one up in an easy situation.", "You have few safety nets. Also notice what you avoid completely.", "I carry a pill, water or a charged phone \"just in case\"", "I only go to hard places with someone", "I sit close to the exit", "I distract myself so as not to notice the anxiety", "I check or ask again many times"),
                        P("The way out is to meet what you fear gradually and do without safety nets. Then the brain learns from experience that the feared thing doesn't happen or is bearable.")
                    ],
                    "Make a list of three to five situations you avoid, from the easiest to the hardest.",
                    "Which situation from the list are you ready to try first?"),
                new(
                    "The first step and a plan",
                    4,
                    [
                        P("The most effective way to weaken anxiety is exposure: gradually meeting the fear. You don't need to go straight to the most frightening thing: start with an easy step and test a prediction."),
                        List(
                            "Before the step, write down what you fear and how sure you are it will happen",
                            "Take the step without safety nets and stay in the situation until you have tested the prediction",
                            "Afterwards, write down what actually happened"),
                        Ladder("Fear ladder"),
                        H("What to keep doing"),
                        List(
                            "Slow breathing at the peak of anxiety",
                            "Checking thoughts and worry time",
                            "Steps up the fear ladder every few days"),
                        P("Take the GAD-7 scale again and compare with the start. If anxiety is strong or gets in the way of life, see a psychotherapist: CBT for anxiety is well studied."),
                        TestsTab("GAD-7 Scale", "gad7")
                    ],
                    "Take the first step on your fear ladder and write down what happened.",
                    "What did you predict, and what actually happened?")
            ]),
        new(
            CourseIds.Mood,
            "Out of apathy",
            "Six days of behavioural activation: small steps to bring back energy and what brings joy",
            "Cloud",
            "phq9",
            "PHQ-9",
            [
                new(
                    "The inactivity loop",
                    4,
                    [
                        P("When mood is at zero, it is natural to do less: there's no energy, nothing appeals. But the less we do, the less there is in life that brings joy and a sense of \"I managed\", and mood drops even lower."),
                        Cycle("The inactivity loop", "No energy", "I do less", "Less joy and fewer wins", "Even less energy"),
                        P("Behavioural activation breaks this loop from the other side: not waiting until you feel like it, but taking small steps after which energy and the wish gradually return. It is one of the best-studied methods of help for depression."),
                        P("Take the PHQ-9 scale at the start of the course: at the end you will compare the result."),
                        TestsTab("PHQ-9 Scale", "phq9"),
                        Warning("If thoughts of death appear"),
                        P("If you have thoughts that it would be better if you weren't here, don't stay alone with them: this course does not replace help from a specialist."),
                        Crisis("I need help now")
                    ],
                    "Today and tomorrow, notice what you are doing and rate your mood from 0 to 10 several times a day.",
                    "How are your days going now? What have you stopped doing lately?"),
                new(
                    "What lifts your mood",
                    4,
                    [
                        P("Look at your observations: at which moments was your mood slightly higher? Often it is something small: a conversation, a walk, a shower, a task done."),
                        H("Two kinds of useful activities"),
                        List(
                            "Pleasant ones: what brings pleasure, however faint — music, tasty food, sunshine",
                            "\"I managed\" ones: what leaves a sense of accomplishment — washing up, answering an email"),
                        P("In depression, pleasure from pleasant things is weaker than before. That's normal: what matters is doing, not waiting for the old joy.")
                    ],
                    "Make a list of ten activities: five pleasant and five \"I managed\". The smaller and simpler, the better.",
                    "What went into your list? Was there something that lifted your mood a little?"),
                new(
                    "Small steps",
                    4,
                    [
                        P("Big plans are daunting in apathy and don't get done. It helps to break things down: not \"clean the flat\" but \"clear three things off the table\"."),
                        List(
                            "Choose one activity from the list for today",
                            "Break it down to a step that takes five minutes",
                            "Set a time and place: \"after breakfast, in the kitchen\"",
                            "Afterwards, note how your mood changed"),
                        Practice(TechniqueId.SmallStep, "One small step")
                    ],
                    "Every day, do one pleasant activity and one \"I managed\" activity at the time you set.",
                    "What did you manage to do? How did your mood change afterwards?"),
                new(
                    "\"I don't feel like it\"",
                    4,
                    [
                        P("The main obstacle is the thought \"I don't feel like it, I'll do it when I have the energy\". In apathy the wish comes after action, not before it."),
                        Quiz("No energy, and a walk is planned for today. What is better?", 1, "In apathy the wish comes back after action. Even five minutes give body and mood the signal \"I did something\".", "Wait until you feel like it", "Go out for at least five minutes", "Put it off until the weekend"),
                        H("Other obstacle thoughts"),
                        List(
                            "\"It's pointless\": check afterwards — is anything even a little different?",
                            "\"It's too small\": small steps are exactly what the way out is made of",
                            "\"I used to do more\": compare yourself with yesterday, not with your past self"),
                        Practice(TechniqueId.SelfCompassion, "Kind words to yourself")
                    ],
                    "When you don't feel like it, do at least a five-minute version of what you planned.",
                    "Which thought most often stops you from starting? What helped you get round it?"),
                new(
                    "People around you",
                    4,
                    [
                        P("In apathy you want to shut yourself off from everyone. But contact with people is one of the strongest remedies for low mood, even if right now it doesn't bring joy as it used to."),
                        List(
                            "Write to one person you haven't been in touch with for a while",
                            "A short meeting or call is better than none",
                            "You don't have to explain everything: \"things are hard right now, let's just go for a walk\"",
                            "If energy is low, choose people who are easy to be with"),
                        Practice(TechniqueId.SmallStep, "One small step")
                    ],
                    "Get in touch with at least one person in the next two days.",
                    "Who did you contact? How was it?"),
                new(
                    "What matters to me",
                    4,
                    [
                        P("So that the steps don't turn into chores, it helps to link them to what matters to you: people close to you, health, work, creativity, nature."),
                        List(
                            "Choose one area that matters to you",
                            "Think about who you want to be in it",
                            "Choose one small step in that direction for the week"),
                        H("What to keep doing"),
                        List(
                            "Every day one pleasant activity and one \"I managed\"",
                            "Small steps instead of big plans",
                            "Contact with people"),
                        P("Take the PHQ-9 scale again and compare with the start. If the result is high or not changing, see a doctor or psychotherapist: depression is very treatable."),
                        TestsTab("PHQ-9 Scale", "phq9"),
                        Crisis("I need help now")
                    ],
                    "Take one step in an area that matters to you this week.",
                    "What matters to you? Which step did you choose?")
            ]),
        new(
            CourseIds.Trauma,
            "After trauma",
            "Five days on what is safe to do on your own: understand your reaction, cope with flashbacks and nightmares, find a specialist",
            "Healing",
            "pcl5",
            "PCL-5",
            [
                new(
                    "A normal reaction to an abnormal event",
                    4,
                    [
                        P("After a hard event almost everyone has intrusive memories, nightmares, anxiety, numbness, irritability. It is not weakness but the reaction of a brain trying to protect you from it happening again."),
                        P("This course won't process the trauma: that takes therapy with a specialist. It will help you understand what is happening to you, cope with hard moments and arrive at a specialist prepared. You don't need to recall or describe the event itself in this course."),
                        P("Take the PCL-5 scale at the start: at the end you will compare the result."),
                        TestsTab("PCL-5 scale", "pcl5"),
                        Warning("If the danger is ongoing"),
                        P("If you are under threat now, or thoughts of harming yourself have appeared, safety comes first."),
                        Crisis("I need help now")
                    ],
                    "Notice when things get harder during the day, without details of the event: the time, the place, what was around.",
                    "Which of these reactions do you recognise in yourself?"),
                new(
                    "Coming back to the present",
                    4,
                    [
                        P("A flashback is a memory relived as if it were happening now. Time, place, the support of the body and the senses help you come back."),
                        Practice(TechniqueId.Flashback, "A memory took over"),
                        Practice(TechniqueId.Grounding, "5-4-3-2-1 grounding"),
                        H("Your own grounding kit"),
                        List(
                            "A phrase: \"This is a memory. It is a different time now, I am safe\"",
                            "Something to carry with you: a pebble, a ring, something with a scent",
                            "A person you can message",
                            "Something cold: water, ice"),
                        Breathe("The out-breath longer than the in-breath")
                    ],
                    "Put together your grounding kit and try the \"A memory took over\" practice at a calm moment, so you know it in advance.",
                    "What went into your grounding kit?"),
                new(
                    "Sleep and nightmares",
                    4,
                    [
                        P("After trauma sleep often suffers: it is hard to fall asleep, sleep is light, nightmares come. Lack of sleep makes the days more anxious and irritable."),
                        List(
                            "One wake-up time every day",
                            "Light in the morning, a calm evening without the news",
                            "If a nightmare wakes you: light, where am I and what day it is, water, slow out-breaths",
                            "Alcohol strengthens nightmares in the second half of the night"),
                        P("A recurring nightmare can be rewritten: imagery rehearsal makes nightmares less frequent and weaker. Start with a milder one."),
                        Practice(TechniqueId.Nightmare, "Rewrite a nightmare")
                    ],
                    "Keep one wake-up time. If you have recurring nightmares, try rewriting one of them, starting with a milder one.",
                    "How are you sleeping now? What helps after a hard night?"),
                new(
                    "Support and people",
                    4,
                    [
                        P("Trauma often distances people from others and from what used to matter. Support from people close to you is one of the strongest factors in recovery."),
                        List(
                            "You don't have to talk about the event: you can simply be together",
                            "Tell people close to you how to help you at a hard moment",
                            "Gradually bring back what gave you support: walks, movement, things you used to enjoy",
                            "Don't numb feelings with alcohol: it gets in the way of recovery"),
                        P("Avoiding reminders is understandable, but over time it narrows life and keeps the trauma \"hot\". It is better to work on this with a specialist than to force yourself alone."),
                        P("What the event changed in how you think about yourself — blame, \"something is wrong with me\", \"no one can be trusted\" — can be worked through in the chat, without retelling what happened."),
                        TraumaChat("Work it through in the chat"),
                        Practice(TechniqueId.SelfCompassion, "Kind words to yourself")
                    ],
                    "Do one thing that used to give you support, and spend time with someone you trust.",
                    "Who and what gives you support now?"),
                new(
                    "The way to a specialist",
                    5,
                    [
                        P("Post-traumatic symptoms respond well to trauma-focused psychotherapy: prolonged exposure, cognitive therapy, EMDR, written exposure. It usually takes 8–16 sessions."),
                        See(ArticleIds.TraumaTreatment, "How trauma is treated and how to choose a specialist"),
                        H("How to prepare"),
                        List(
                            "Take the PCL-5 scale again and compare with the start",
                            "Export a summary from the app for the specialist: \"Profile → Options → Data & backup\"",
                            "Write down your questions: which method, how many sessions, how progress will be measured",
                            "You don't have to tell everything about the event at the first meeting"),
                        TestsTab("PCL-5 scale", "pcl5"),
                        Crisis("I need help now")
                    ],
                    "Find one or two specialists who work with trauma and book a consultation.",
                    "What in the course was most useful? What do you want to discuss with the specialist?")
            ])
    ];
}

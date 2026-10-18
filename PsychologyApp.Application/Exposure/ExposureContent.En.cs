using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

public static partial class ExposureContent
{
    private static readonly string[] LessonsEn =
    [
        "It was less frightening than I expected",
        "I coped, even while I was anxious",
        "Nobody noticed anything",
        "The anxiety rose and fell on its own",
        "Even if it is awkward, I can get through it",
        "I did not need my safety net"
    ];

    private static readonly ExposureKindContent[] KindsEn =
    [
        new(
            FearKind.Social,
            "People and being judged",
            "Speaking up, calling, meeting people, eating in front of others",
            [
                "Everyone will see that I'm nervous",
                "I'll say something stupid and they'll laugh at me",
                "They'll think badly of me",
                "I'll blush and everyone will see",
                "I'll go blank and won't be able to go on",
                "I'll be rejected"
            ],
            [
                "Phone calls",
                "Presentations and meetings",
                "Meeting and talking to strangers",
                "Eating or writing in front of others",
                "Asking for help or returning things",
                "Saying no"
            ],
            [
                "Rehearsing what I'll say",
                "Avoiding eye contact",
                "Speaking quietly and briefly",
                "Staying next to someone I know",
                "Hiding my hands or face",
                "Leaving early",
                "Looking at my phone",
                "Drinking alcohol for courage"
            ],
            [
                new("Ask a shop assistant where to find something", 25),
                new("Greet a neighbour first", 30),
                new("Call to book a doctor's appointment", 40),
                new("Return an item to a shop", 50),
                new("Ask a question in a meeting or a class", 60),
                new("Start a conversation with a stranger", 65),
                new("Say no to a colleague's request", 70),
                new("Give a short talk in front of a group", 85)
            ]),
        new(
            FearKind.Panic,
            "Panic attacks and body sensations",
            "Pounding heart, short of breath, dizziness",
            [
                "I'll have a heart attack",
                "I'll suffocate",
                "I'll faint",
                "I'll go crazy or lose control",
                "I'll feel ill and nobody will help",
                "I'll embarrass myself in front of everyone"
            ],
            [
                "Public transport and the metro",
                "Queues and crowds",
                "Being far from home alone",
                "Exercise and climbing stairs",
                "Heat and stuffy rooms",
                "Coffee"
            ],
            [
                "Carrying water or a pill",
                "Checking my pulse",
                "Sitting near the exit",
                "Going out only with someone",
                "Distracting myself with my phone",
                "Holding on to a rail or a wall",
                "Watching my breathing",
                "Leaving as soon as it starts"
            ],
            [
                new("Climb three flights of stairs at a brisk pace", 30, ProvokesBodySensations: true),
                new("Drink a cup of coffee", 35, ProvokesBodySensations: true),
                new("Stand in a queue in a shop", 40),
                new("Ride the bus for one stop", 50),
                new("Walk alone 15 minutes from home", 55),
                new("Ride three metro stops without water in my bag", 70),
                new("Sit in the middle of a row at the cinema", 75),
                new("Ride the metro at rush hour", 85)
            ]),
        new(
            FearKind.Specific,
            "A specific fear",
            "Heights, flying, dogs, doctors, blood, lifts",
            [
                "Something terrible will happen",
                "I won't be able to stand the fear",
                "I'll lose control",
                "I'll be hurt or bitten",
                "I'll fall",
                "I'll be trapped and won't get out"
            ],
            [
                "Heights and balconies",
                "Lifts and closed spaces",
                "Animals",
                "Doctors, injections, blood",
                "Flying",
                "Bridges and tunnels"
            ],
            [
                "Closing my eyes or looking away",
                "Holding on to someone",
                "Going only with company",
                "Planning the way out in advance",
                "Distracting myself with music",
                "Taking a sedative"
            ],
            [
                new("Look at photos of what I fear", 20),
                new("Watch a video of it", 30),
                new("Stand near it at a safe distance", 40),
                new("Come closer for a minute", 55),
                new("Stay close for 5 minutes", 65),
                new("Touch it or step in (lift, balcony)", 75),
                new("Do it again alone, without company", 85),
                new("Do it in an unfamiliar place", 90)
            ]),
        new(
            FearKind.Uncertainty,
            "Uncertainty and \"what if something happens\"",
            "Worrying, checking, looking for guarantees",
            [
                "Something bad will happen if I don't check",
                "I can't bear not knowing for sure",
                "Something will happen to someone I love",
                "I'll make a mistake and it will be a disaster",
                "I'll fall seriously ill"
            ],
            [
                "Decisions without full information",
                "Sending messages without re-checking",
                "Not knowing where my loved ones are",
                "New and unpredictable situations",
                "Reading about illnesses"
            ],
            [
                "Checking many times",
                "Searching the internet",
                "Asking others whether everything is fine",
                "Calling loved ones to make sure",
                "Planning everything in detail",
                "Putting decisions off"
            ],
            [
                new("Send a message after reading it once", 25),
                new("Choose a dish in a café within a minute", 30),
                new("Not call a loved one who is 30 minutes late", 50),
                new("Send a work email without re-checking", 55),
                new("Not search the internet about a symptom for a day", 60),
                new("Spend a day off without a plan", 65),
                new("Not ask anyone whether I decided right", 70),
                new("Leave home after checking the door only once", 80)
            ]),
        new(
            FearKind.Other,
            "Something else",
            "Describe your fear in your own words",
            [
                "Something bad will happen",
                "I won't cope",
                "I'll lose control",
                "I'll be judged"
            ],
            [],
            [
                "Avoiding the situation",
                "Going only with someone",
                "Leaving early",
                "Distracting myself with my phone",
                "Planning the way out in advance"
            ],
            [])
    ];
}

namespace PsychologyApp.Presentation.Core.Practice;

/// <summary>
/// One round of "keep the mind busy": a few tasks drawn at random from the list, never the same one twice in a round. The round is
/// short on purpose, so the practice ends and the person comes back to the rating instead of distracting themselves indefinitely.
/// Tasks are numbers 1..<see cref="TaskCount"/>; their texts live in the resources.
/// </summary>
public sealed class BusyMindRound
{
    public const int TaskCount = 12;
    public const int DefaultLength = 5;

    private readonly Queue<int> _unused;

    public BusyMindRound(int length = DefaultLength, Random? random = null)
    {
        Random rng = random ?? Random.Shared;
        int[] order = [.. Enumerable.Range(1, TaskCount)];
        rng.Shuffle(order);
        Length = Math.Clamp(length, 1, TaskCount);
        _unused = new Queue<int>(order);
        Current = _unused.Dequeue();
    }

    public int Length { get; }

    /// <summary>The number of the task on screen.</summary>
    public int Current { get; private set; }

    /// <summary>Which task of the round is on screen, from 1.</summary>
    public int Step { get; private set; } = 1;

    public bool IsOver { get; private set; }

    /// <summary>The task is done: the next one, or the end of the round.</summary>
    public void Next()
    {
        if (IsOver)
        {
            return;
        }

        if (Step >= Length || _unused.Count == 0)
        {
            IsOver = true;
            return;
        }

        Step++;
        Current = _unused.Dequeue();
    }

    /// <summary>Another task in its place, without counting a step; the skipped one is not shown again this round.</summary>
    public bool Skip()
    {
        if (IsOver || _unused.Count == 0)
        {
            return false;
        }

        Current = _unused.Dequeue();
        return true;
    }
}

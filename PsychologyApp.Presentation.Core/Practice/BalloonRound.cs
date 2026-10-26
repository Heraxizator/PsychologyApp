namespace PsychologyApp.Presentation.Core.Practice;

public enum BalloonState
{
    Floating,
    Popped,
    Escaped
}

/// <param name="X">Horizontal centre, 0..1 of the field width.</param>
/// <param name="Y">Vertical centre, 0..1 of the field height; starts below the field and rises.</param>
public sealed class Balloon(double x, double y, double speed, int colorIndex)
{
    public double X { get; } = x;

    public double Y { get; internal set; } = y;

    /// <summary>Share of the field height risen per second.</summary>
    public double Speed { get; } = speed;

    public int ColorIndex { get; } = colorIndex;

    public BalloonState State { get; internal set; } = BalloonState.Floating;

    /// <summary>Seconds since the pop, for the short burst drawn after it.</summary>
    public double BurstAge { get; internal set; }
}

/// <summary>
/// One round of the balloon practice. It is bounded on purpose: a fixed number of balloons, then it stops, so the game never
/// keeps animating (battery, screen readers waiting for an idle screen) and the person comes back to the rating after it.
/// With reduced motion the balloons stand still in a grid and only wait to be popped.
/// </summary>
public sealed class BalloonRound
{
    public const int DefaultCount = 20;
    public const int ColorCount = 6;
    public const double BurstSeconds = 0.35;

    /// <summary>Balloon radius as a share of the field width.</summary>
    public const double RadiusShare = 0.085;

    private const double SpawnInterval = 0.7;
    private const int GridColumns = 4;

    private readonly List<Balloon> _balloons = [];
    private readonly Random _random;
    private readonly bool _still;
    private double _sinceSpawn = SpawnInterval;

    public BalloonRound(int count = DefaultCount, bool reduceMotion = false, Random? random = null)
    {
        Total = Math.Max(1, count);
        _still = reduceMotion;
        _random = random ?? Random.Shared;
        if (_still)
        {
            int rows = (Total + GridColumns - 1) / GridColumns;
            for (int i = 0; i < Total; i++)
            {
                double x = (i % GridColumns + 0.5) / GridColumns;
                double y = (i / GridColumns + 0.5) / rows;
                _balloons.Add(new Balloon(x, y, 0, i % ColorCount));
            }

            Spawned = Total;
        }
    }

    public int Total { get; }

    public int Spawned { get; private set; }

    public int Popped { get; private set; }

    public int Escaped { get; private set; }

    public IReadOnlyList<Balloon> Balloons => _balloons;

    /// <summary>All balloons are released and none is still floating or bursting: the animation can stop.</summary>
    public bool IsOver => Spawned == Total && _balloons.All(balloon => balloon.State == BalloonState.Escaped || (balloon.State == BalloonState.Popped && balloon.BurstAge >= BurstSeconds));

    public void Update(double seconds)
    {
        if (seconds <= 0)
        {
            return;
        }

        foreach (Balloon balloon in _balloons)
        {
            if (balloon.State == BalloonState.Popped)
            {
                balloon.BurstAge += seconds;
            }
            else if (balloon.State == BalloonState.Floating && !_still)
            {
                balloon.Y -= balloon.Speed * seconds;
                if (balloon.Y < -RadiusShare * 2)
                {
                    balloon.State = BalloonState.Escaped;
                    Escaped++;
                }
            }
        }

        if (_still)
        {
            return;
        }

        _sinceSpawn += seconds;
        while (Spawned < Total && _sinceSpawn >= SpawnInterval)
        {
            _sinceSpawn -= SpawnInterval;
            double x = RadiusShare + _random.NextDouble() * (1 - 2 * RadiusShare);
            double speed = 0.16 + _random.NextDouble() * 0.12;
            _balloons.Add(new Balloon(x, 1 + RadiusShare * 2, speed, Spawned % ColorCount));
            Spawned++;
        }
    }

    /// <summary>Pops the topmost floating balloon under a tap; the field size turns the tap into the balloon's own units.</summary>
    public bool TryPop(double pointX, double pointY, double width, double height)
    {
        if (width <= 0 || height <= 0)
        {
            return false;
        }

        // A finger is wider than its point: the target is a little larger than the drawn balloon.
        double reach = RadiusShare * width * 1.25;
        Balloon? hit = _balloons
            .Where(balloon => balloon.State == BalloonState.Floating)
            .LastOrDefault(balloon => Math.Pow(balloon.X * width - pointX, 2) + Math.Pow(balloon.Y * height - pointY, 2) <= reach * reach);
        if (hit is null)
        {
            return false;
        }

        hit.State = BalloonState.Popped;
        Popped++;
        return true;
    }

    /// <summary>For a screen reader, which cannot aim: pops the next floating balloon.</summary>
    public bool PopNext()
    {
        Balloon? next = _balloons.FirstOrDefault(balloon => balloon.State == BalloonState.Floating);
        if (next is null)
        {
            return false;
        }

        next.State = BalloonState.Popped;
        Popped++;
        return true;
    }
}

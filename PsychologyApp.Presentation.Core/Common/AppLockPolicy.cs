namespace PsychologyApp.Presentation.Common;

/// <summary>When the app asks for the phone's unlock (fingerprint, face or the phone's PIN) before showing anything.</summary>
public static class AppLockPolicy
{
    /// <summary>
    /// A short trip out of the app does not lock it again: the share sheet, the file picker, a permission dialog or a quick
    /// call all send the app to the background, and asking each time would make them unusable.
    /// </summary>
    public static readonly TimeSpan RelockAfter = TimeSpan.FromMinutes(1);

    /// <param name="enabled">The person turned the lock on.</param>
    /// <param name="unlockedThisRun">The phone's unlock was already passed since the app started.</param>
    /// <param name="awayFor">
    /// How long the app has been in the background since it was last unlocked; null while it stays in front. Measured on a
    /// clock the phone's owner cannot set back, or turning the clock back would skip the lock.
    /// </param>
    public static bool NeedsUnlock(bool enabled, bool unlockedThisRun, TimeSpan? awayFor)
    {
        if (!enabled)
        {
            return false;
        }

        if (!unlockedThisRun)
        {
            return true;
        }

        return awayFor is TimeSpan away && away >= RelockAfter;
    }
}

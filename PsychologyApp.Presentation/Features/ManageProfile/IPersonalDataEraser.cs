namespace PsychologyApp.Presentation.Features.ManageProfile;

/// <summary>"Delete all my data": everything the person has entered goes, as after a fresh install.</summary>
public interface IPersonalDataEraser
{
    /// <summary>Deletes the data and starts the app anew; returns only if something failed before the restart.</summary>
    Task EraseAllAsync(CancellationToken cancellationToken = default);
}

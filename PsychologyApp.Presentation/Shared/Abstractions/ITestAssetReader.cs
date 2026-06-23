namespace PsychologyApp.Presentation.Shared.Abstractions;

public interface ITestAssetReader
{
    Task<Stream> OpenAsync(string assetPath, CancellationToken cancellationToken = default);
}

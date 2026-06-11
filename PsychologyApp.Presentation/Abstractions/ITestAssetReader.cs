namespace PsychologyApp.Presentation.Abstractions;

public interface ITestAssetReader
{
    Task<Stream> OpenAsync(string assetPath, CancellationToken cancellationToken = default);
}

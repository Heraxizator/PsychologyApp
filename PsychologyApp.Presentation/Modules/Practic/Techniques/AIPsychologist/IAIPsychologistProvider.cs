namespace PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

public interface IAIPsychologistProvider
{
    Task<bool> InitializeAsync();
    Task<string> GenerateResponseAsync(string userMessage);
    bool IsInitialized { get; }
    string ProviderName { get; }
}

namespace PsychologyApp.Presentation.Shared.Common.Localization;

/// <summary>Reloads the language-dependent content (quotes, reasons, tests, techniques) after the language was changed.</summary>
public interface ILanguageContentReloader
{
    /// <summary>Completes when the content of the current language is loaded.</summary>
    Task EnsureReloadedAsync();
}

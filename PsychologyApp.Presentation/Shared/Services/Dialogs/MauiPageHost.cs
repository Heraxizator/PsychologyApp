namespace PsychologyApp.Presentation.Shared.Services.Dialogs;

public sealed class MauiPageHost : IPageHost
{
    public Page? GetActivePage() =>
        Microsoft.Maui.Controls.Shell.Current?.CurrentPage
        ?? Microsoft.Maui.Controls.Application.Current?.MainPage;
}

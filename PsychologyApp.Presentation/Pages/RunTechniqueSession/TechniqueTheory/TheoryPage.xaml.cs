using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryPage : ContentPage
{
    private readonly ITheoryViewModelFactory _theoryViewModelFactory;
    private readonly string _content;
    private readonly TechniqueId? _techniqueId;
    private bool _initialized;

    public TheoryPage(ITheoryViewModelFactory theoryViewModelFactory, string content, TechniqueId? techniqueId = null)
    {
        _theoryViewModelFactory = theoryViewModelFactory;
        _content = content;
        _techniqueId = techniqueId;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        EnsureViewModelAsync().FireAndForget();
    }

    private async Task EnsureViewModelAsync()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        BindingContext = await _theoryViewModelFactory.CreateAsync(this, _content, _techniqueId);
    }
}

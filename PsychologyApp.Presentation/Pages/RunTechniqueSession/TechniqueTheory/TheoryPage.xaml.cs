using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryPage : ContentPage
{
    private readonly ITheoryViewModelFactory _theoryViewModelFactory;
    private readonly string _content;
    private readonly TechniqueId? _techniqueId;
    private TheoryViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;
    private bool _bound;

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
        if (!_bound)
        {
            _bound = true;
            _viewModel = _theoryViewModelFactory.Create(this, _content, _techniqueId);
            BindingContext = _viewModel;
            _animationHelper = new PageAnimationHelper(_viewModel, LoadingProgress, contentView: TheoryContent);
        }

        _animationHelper?.TryRevealAsync();

        if (_viewModel is not null)
        {
            await _viewModel.EnsureInitializedAsync();
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}

using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesPage : ContentPage
{
    private TechniquesViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;

    public TechniquesPage(
        IPageViewModelActivator pageViewModelActivator,
        ITechniquesViewModelFactory techniquesViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, page => techniquesViewModelFactory.Create(page));
        _animationHelper = new PageAnimationHelper(_viewModel, LoadingProgress, TechniquesCollectionView);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
        if (_viewModel is null)
        {
            return;
        }

        if (_viewModel.HasInitialized)
        {
            _viewModel.RefreshOnAppearAsync().FireAndForget();
        }
        else
        {
            _viewModel.EnsureInitializedAsync().FireAndForget();
        }

        _viewModel.TryOpenPendingTechniqueAsync().FireAndForget();
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

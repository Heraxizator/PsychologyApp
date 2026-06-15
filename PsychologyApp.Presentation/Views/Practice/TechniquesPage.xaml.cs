using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Practice;

namespace PsychologyApp.Presentation.Views.Practice;

public partial class TechniquesPage : ContentPage
{
    private TechniquesViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;

    public TechniquesPage(
        IPageViewModelActivator pageViewModelActivator,
        ITechniquesViewModelFactory techniquesViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => techniquesViewModelFactory.Create(nav));
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

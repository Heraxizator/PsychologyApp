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
        PopChatButton();
        _animationHelper?.TryRevealAsync();
        _viewModel?.ChatHero?.RefreshAsync().FireAndForget();
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
        _viewModel.TryOpenPendingJournalAsync().FireAndForget();
    }

    /// <summary>The round chat button grows in once when the tab opens; with reduced motion it is simply there.</summary>
    private void PopChatButton()
    {
        if (!UiAnimations.ShouldAnimate(ChatFab))
        {
            ChatFab.Scale = 1;
            return;
        }

        ChatFab.Scale = 0.6;
        ChatFab.ScaleToAsync(1, 240, Easing.SpringOut).FireAndForget();
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.LoadMoreCustomTechniquesCommand.Execute(null);
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

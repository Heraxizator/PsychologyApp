using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueCreated;

public partial class CreatedPage : ContentPage
{
    private readonly CreatedViewModel _viewModel;
    private PageAnimationHelper? _animationHelper;

    public CreatedPage(
        IPageViewModelActivator pageViewModelActivator,
        ICreatedViewModelFactory createdViewModelFactory,
        long id)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, page => createdViewModelFactory.Create(page, id));
        _animationHelper = new PageAnimationHelper(_viewModel, LoadingProgress, contentView: CreatedContent);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
        _viewModel.EnsureInitializedAsync().FireAndForget();
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

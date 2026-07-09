using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTests.TestHistory;

public partial class TestHistoryPage : ContentPage
{
    private PageAnimationHelper? _animationHelper;

    public TestHistoryPage(
        ITestHistoryViewModelFactory factory,
        string testId,
        string testTitle)
    {
        InitializeComponent();
        TestHistoryViewModel viewModel = factory.Create(this, testId, testTitle);
        BindingContext = viewModel;
        _animationHelper = new PageAnimationHelper(viewModel, LoadingProgress, contentView: HistoryContent);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
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

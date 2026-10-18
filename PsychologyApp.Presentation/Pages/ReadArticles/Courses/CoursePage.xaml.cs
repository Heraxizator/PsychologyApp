using PsychologyApp.Presentation.Features.ReadArticles;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Courses;

public partial class CoursePage : ContentPage
{
    private readonly CourseViewModel _viewModel;

    public CoursePage(ICourseViewModelFactory viewModelFactory, string courseId)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this, courseId);
        BindingContext = _viewModel;
    }

    // A day closed on the day screen, or a test just taken, changes this one.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}

using PsychologyApp.Presentation.Features.ReadArticles;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Courses;

public partial class CourseDayPage : ContentPage
{
    private readonly CourseDayViewModel _viewModel;

    public CourseDayPage(ICourseDayViewModelFactory viewModelFactory, string courseId, int day)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this, courseId, day);
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopListening();
    }
}

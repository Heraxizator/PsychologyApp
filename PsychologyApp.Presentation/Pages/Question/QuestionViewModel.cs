using MvvmHelpers;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;
namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionViewModel : BaseViewModel
{
    public ObservableRangeCollection<Models.Tests.Question> Questions { get; private set; } = [];
    public ICommand ConfirmCommand { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public readonly bool IsSingleAnswer = default!;

    private Func<int, string> Analyzer { get; set; } = default!;
    private readonly IToastService _toastService;
    private readonly IUserProgressService _userProgressService;
    private readonly INavigationService _navigationService;
    private readonly QuestionnaireSubmissionService _submissionService;
    private readonly TestSessionInfo? _session;
    private readonly ITestCatalogService _testCatalogService;

    public QuestionViewModel(
        List<Models.Tests.Question> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        IToastService toastService,
        IDialogService dialogService,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        QuestionnaireSubmissionService submissionService,
        ITestCatalogService testCatalogService,
        TestSessionInfo? session = null)
    {
        BindNavigation(navigationService);
        _toastService = toastService;
        _ = dialogService;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _submissionService = submissionService;
        _testCatalogService = testCatalogService;
        _session = session;

        Analyzer = analyzer;
        IsSingleAnswer = singleAnswer;
        Questions.AddRange(questions);

        ConfirmCommand = new AsyncCommand(CalculateAnswersAsync);
        BackCommand = new AsyncCommand(GoBackAsync);
    }
}

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
    public ObservableRangeCollection<TestQuestion> Questions { get; private set; } = [];
    public ICommand ConfirmCommand { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;
    public bool IsSingleAnswer { get; }

    private Func<int, string> Analyzer { get; set; } = default!;
    private readonly DateTime _startedAtUtc;
    private readonly IToastService _toastService;
    private readonly IUserProgressService _userProgressService;
    private readonly INavigationService _navigationService;
    private readonly QuestionnaireSubmissionService _submissionService;
    private readonly TestRunCoordinator _runCoordinator;
    private readonly TestSessionInfo? _session;
    private readonly ITestCatalogService _testCatalogService;

    public QuestionViewModel(
        List<TestQuestion> questions,
        Func<int, string> analyzer,
        bool singleAnswer,
        IToastService toastService,
        IDialogService dialogService,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        QuestionnaireSubmissionService submissionService,
        TestRunCoordinator runCoordinator,
        ITestCatalogService testCatalogService,
        TestSessionInfo? session = null)
    {
        BindNavigation(navigationService);
        _toastService = toastService;
        _ = dialogService;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _submissionService = submissionService;
        _runCoordinator = runCoordinator;
        _testCatalogService = testCatalogService;
        _session = session;
        _startedAtUtc = DateTime.UtcNow;

        Analyzer = analyzer;
        IsSingleAnswer = singleAnswer;
        Questions.AddRange(questions);

        ConfirmCommand = new AsyncCommand(CalculateAnswersAsync);
        BackCommand = new AsyncCommand(GoBackAsync);
        InitializeWizard();
    }
}

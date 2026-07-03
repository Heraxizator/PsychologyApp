using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace PsychologyApp.Presentation.Pages.RunTests.TestsList;

public partial class TestsListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly TestsListLoader _testsListLoader;
    private readonly ILogger<TestsListViewModel> _logger;

    public ICommand OpenProfileCommand { get; }

    public ObservableCollection<TestItem> TestItemCollection { get; private set; } = [];

    public TestsListViewModel(
        INavigationService navigationService,
        IDatabaseReadySignal databaseReadySignal,
        TestsListLoader testsListLoader,
        ILogger<TestsListViewModel> logger)
    {
        _navigationService = navigationService;
        _databaseReadySignal = databaseReadySignal;
        _testsListLoader = testsListLoader;
        _logger = logger;
        BindNavigation(navigationService);
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        Reload = new AsyncCommand(InitAsync);
    }
}

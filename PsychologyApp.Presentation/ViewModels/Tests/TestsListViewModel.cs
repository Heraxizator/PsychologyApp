using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Tests;

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

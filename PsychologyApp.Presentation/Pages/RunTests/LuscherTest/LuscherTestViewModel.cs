using PsychologyApp.Presentation.Features.RunTests.BaseTest;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

public partial class LuscherTestViewModel : BaseTestViewModel
{
    private readonly LuscherMode _mode;
    private readonly IUserProgressService? _userProgressService;
    private readonly ILuscherResultService _luscherResultService = null!;
    private readonly TechniqueCatalogGateway? _techniqueCatalog;
    private int _lastCoValue;
    private double _lastBkValue;
    private int _passNumber = 1;
    private List<(ColourValue Colour, ColourMeaning Meaning)> _firstPassSelections = [];

    public LuscherMode Mode => _mode;
    public bool IsStandardMode => _mode == LuscherMode.Standard;
    public bool IsBriefMode => _mode == LuscherMode.Brief;

    public ObservableCollection<ResultItem> ResultItems { get; private set; } = [];

    public ICommand BackToListCommand { get; private set; } = default!;

    public LuscherTestViewModel() { }

    public LuscherTestViewModel(
        LuscherMode mode,
        INavigationService navigationService,
        IUserProgressService? userProgressService,
        ILuscherResultService luscherResultService,
        TechniqueCatalogGateway? techniqueCatalog = null)
    {
        _mode = mode;
        _userProgressService = userProgressService;
        _luscherResultService = luscherResultService;
        _techniqueCatalog = techniqueCatalog;
        BindNavigation(navigationService);
        ModuleName = AppStrings.TestsDetectorTitle;
        PageName = PageTitle;

        Restart = new Command(ToRestart);
        BackToListCommand = new AsyncCommand(GoToRootAsync);
        InitializeColorHandlers();
        InitializeRecommendation();

        Init();
    }
}

public class ResultItem
{
    public string? PropertyName { get; set; }
    public string? PropertyValue { get; set; }
    public string? PropertyText { get; set; }
}

using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice;

public partial class TechniquesViewModel
{
    public ICommand ConstructorTapped { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;
    public ICommand StartTodayPracticeCommand { get; private set; } = default!;
    public ICommand RecordMoodCommand { get; private set; } = default!;

    private void WireCommands()
    {
        ConstructorTapped = new AsyncCommand(() => _navigationService.GoToDesignerAsync(-1));
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        StartTodayPracticeCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(_todayTechniqueId));
        RecordMoodCommand = new Command<object?>(parameter =>
        {
            int level = parameter switch
            {
                int value => value,
                string text when int.TryParse(text, out int parsed) => parsed,
                _ => 0
            };

            if (level is >= 1 and <= 5)
            {
                RecordMoodAsync(level).FireAndForget();
            }
        });

        Cancel = new Command(CancelProgress);
        Reload = new AsyncCommand(() => InitializeAsync(showLoadingOverlay: true));
    }
}

using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services.Practice;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class PolarityViewModel
{
    private Task LoadDraftAsync() =>
        _draftCoordinator.LoadAsync(
            polarities,
            item => new Polarity
            {
                Id = item.Id,
                Positive = item.Positive,
                Negative = item.Negative
            },
            hasItems => IsFull = hasItems);

    private Task SaveDraftAsync() =>
        _draftCoordinator.SaveAsync(
            polarities,
            polarity => new PolarityListDraftItem
            {
                Id = polarity.Id,
                Positive = polarity.Positive,
                Negative = polarity.Negative
            });

    private Task CompleteSessionAsync() =>
        _sessionHelper.CompleteAsync(
            TechniqueId.Polarity.ToString(),
            ModuleName,
            PageName,
            _sessionStartedAt);

    private void DeleteItem(Polarity item)
    {
        if (item is null)
        {
            return;
        }

        polarities.Remove(item);
        if (polarities.Count == 0)
        {
            IsFull = false;
        }

        SaveDraftAsync().FireAndForget();
    }

    private void ToAdd(object obj)
    {
        if (string.IsNullOrEmpty(Negative) || string.IsNullOrEmpty(Positive))
        {
            return;
        }

        IsFull = true;
        Polarity item = new()
        {
            Id = AppStrings.PoleNumber(polarities.Count + 1),
            Positive = Positive,
            Negative = Negative
        };
        polarities.Add(item);
        Polarity = item;
        Negative = string.Empty;
        Positive = string.Empty;
        SaveDraftAsync().FireAndForget();
    }
}

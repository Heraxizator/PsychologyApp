using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession.SubViewModels.PaperList;

public partial class PaperListViewModel
{
    private Task LoadDraftAsync() =>
        _draftCoordinator.LoadAsync(
            PapersObservableCollection,
            item => new Paper { Id = item.Id, Text = item.Text },
            SetCollection);

    private Task SaveDraftAsync() =>
        _draftCoordinator.SaveAsync(
            PapersObservableCollection,
            paper => new PaperListDraftItem { Id = paper.Id, Text = paper.Text });

    private Task CompleteSessionAsync() =>
        _sessionHelper.CompleteAsync(
            _techniqueId.ToString(),
            ModuleName,
            PageName,
            _sessionStartedAt);

    private void SetCollection(bool visible) => IsFull = visible;

    private void ToAdd(object obj)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        SetCollection(true);
        PapersObservableCollection.Add(new Paper
        {
            Id = AppStrings.RecordNumber(PapersObservableCollection.Count + 1),
            Text = Text
        });

        if (_clearTextAfterAdd)
        {
            Text = string.Empty;
        }

        SaveDraftAsync().FireAndForget();
    }

    private void DeleteItem(Paper item)
    {
        if (item is null)
        {
            return;
        }

        PapersObservableCollection.Remove(item);
        SetCollection(PapersObservableCollection.Any());
        SaveDraftAsync().FireAndForget();
    }
}

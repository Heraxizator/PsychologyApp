using MobileHelperMaui.Views.TechniquePages.ConstructorPages;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Technique.Constructor;

public class CreatedViewModel : BaseViewModel
{
    private static Task? Initialization;
    private TechniqueService _techniqueService = new();

    public ICommand Remove { get; private set; } = default!;
    public ICommand Edit { get; private set; } = default!;

    public ObservableCollection<Items> Elements { get; private set; } = [];
    private long currentId { get; set; }

    public CreatedViewModel() { }

    public CreatedViewModel(INavigation navigation, long id)
    {
        this.ModuleName = "Практик";
        this.PageName = "Своя техника";

        Finish = new Command(async () => await navigation.PopAsync(false));
        Theory = new Command(ToTheory);
        Remove = new Command(() => ToRemove(navigation));
        Edit = new Command(() => ToEdit(navigation));

        currentId = id;

        Initialization = InitAsync(5000);
    }

    private async void ToEdit(INavigation navigation)
    {
        await navigation.PushAsync(new DesignerPage(currentId), false);
    }

    private async void ToRemove(INavigation navigation)
    {
        bool isConfirmed = await ServiceLocator.Instance.GetService<IDialogService>().AskAsync(null, "Вы уверены, что хотите удалить свою технику", "Да", "Нет");

        if (isConfirmed is true)
        {
            TechniqueDTO item = await _techniqueService.GetTechniqueById(currentId);

            await _techniqueService.DeleteTechnique(item);

            MessagingCenter.Send<object, TechniqueDTO>(this, "remove", item);

            await navigation.PopToRootAsync(false);
        }
    }

    private async Task InitAsync(int cancelTimeout)
    {
        TechniqueDTO item = await _techniqueService.GetTechniqueById(currentId);

        if (item.TechniqueId == -1)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Не удалось загрузить технику");
            return;
        }

        if (string.IsNullOrWhiteSpace(item.Algorithm) is true)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Алгоритм не найден");
            return;
        }

        string[] actions = item.Algorithm.Split('\n');

        foreach (string action in actions)
        {
            Elements.Add(new Items
            {
                Text = action
            });
        }
    }
}

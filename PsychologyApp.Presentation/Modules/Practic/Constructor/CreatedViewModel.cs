using MobileHelperMaui.Views.TechniquePages.ConstructorPages;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Domain.Base.Constants;
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
    private readonly long _techniqueId;
    private readonly TechniqueService _techniqueService = new();

    public ICommand Remove { get; private set; } = default!;
    public ICommand Edit { get; private set; } = default!;

    public CreatedViewModel() { }

    public CreatedViewModel(INavigation navigation, long techniqueId)
    {
        try
        {
            _techniqueId = techniqueId;

            this.ModuleName = "Практик";
            this.PageName = "Своя техника";

            this.Finish = new Command(async () => await navigation.PopAsync(false));
            this.Theory = new Command(ToTheory);
            this.Remove = new Command(() => ToRemove(navigation));
            this.Edit = new Command(() => ToEdit(navigation));

            Task.Run(async () => await InitAsync(Constants.SmallBaseTimeout));
        }
        
        catch (Exception e)
        {
            SetFail();

            Console.WriteLine(e.Message);
        }
    }

    private async void ToEdit(INavigation navigation)
    {
        await navigation.PushAsync(new DesignerPage(_techniqueId), false);
    }

    private async void ToRemove(INavigation navigation)
    {
        try
        {
            bool isConfirmed = await ServiceLocator.Instance.GetService<IDialogService>().AskAsync(null, "Вы уверены, что хотите удалить свою технику", "Да", "Нет");

            if (isConfirmed is true)
            {
                TechniqueDTO techniqueDTO = await _techniqueService.GetTechniqueById(_techniqueId);

                await _techniqueService.DeleteTechnique(techniqueDTO);

                MessagingCenter.Send<object, TechniqueDTO>(this, "remove", techniqueDTO);

                await navigation.PopToRootAsync(false);
            }
        }
        
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetFail();
            });

            Console.WriteLine(e.Message);
        }
    }

    private async Task InitAsync(int cancelTimeout)
    {
        try
        {
            TechniqueDTO techniqueDTO = await _techniqueService.GetTechniqueById(_techniqueId);

            string[] actions = techniqueDTO.Actions?.Split('\n') ?? [];

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (string action in actions)
                {
                    Algorithm.Add(action);
                }
            });
        }
        
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetFail();
            });

            Console.WriteLine(e.Message);
        }
    }
}

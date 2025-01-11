using MobileHelperMaui.Views.TechniquePages;
using MobileHelperMaui.Views.TechniquePages.ConstructorPages;
using MvvmHelpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.Technique.Main;

public class TechniquesViewModel : BaseViewModel
{
    private static Task? Initialization = default;
    private readonly TechniqueService _techniqueService = new();

    public ObservableRangeCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ICommand ConstructorTapped { get; private set; } = default!;

    public TechniquesViewModel() { }

    public TechniquesViewModel(INavigation navigation)
    {
        Title = "Список техник";

        ConstructorTapped = new Command(
            (obj) => navigation.PushAsync(new DesignerPage(-1), false));

        Initialization = InitAsync(navigation, 10000);

        SetObservers(navigation);
    }

    public async Task InitAsync(INavigation navigation, int cancelTimeout)
    {
        IEnumerable<TechniqueItem> staticSource = GetTechniqueItems(navigation);
        Techniques.AddRange(staticSource);

        IEnumerable<TechniqueDTO> dynamicSource = await _techniqueService.GetTechniquesList(int.MaxValue, cancelTimeout);
        Techniques.AddRange(dynamicSource.Select(x => ParseFromDB(navigation, x)));
    }

    private void SetObservers(INavigation navigation)
    {
        MessagingCenter.Subscribe<object, TechniqueDTO>(this, "add", async (sender, item) =>
        {
            Techniques.Clear();

            await InitAsync(navigation, 10000);
        });

        MessagingCenter.Subscribe<object, TechniqueDTO>(this, "remove", async (sender, item) =>
        {
            Techniques.Clear();

            await InitAsync(navigation, 10000);
        });

        MessagingCenter.Subscribe<object, TechniqueDTO>(this, "change", async (sender, item) =>
        {
            Techniques.Clear();

            await InitAsync(navigation, 10000);
        });
    }

    private IEnumerable<TechniqueItem> GetTechniqueItems(INavigation navigation)
    {
        string image = "technique.png";

        List<TechniqueItem> items =
        [
            new TechniqueItem
            {
                Number = "Техника №1",
                Date="26.01.2023",
                Image = image,
                Title = "Крутилка",
                Subtitle = "Метод мгновенной нейтрализации травм и шоков",
                Theme = "Эпизоды",
                Author = "Живорад Славинский",
                Active = true,
                TapCommand = new Command(async () => await navigation.PushAsync(new SpinPage(), false))
            },

            new TechniqueItem
            {
                Number = "Техника №2",
                Date="26.01.2023",
                Image = image,
                Title = "Сравнение важностей",
                Subtitle = "Прошлое, настоящее и будущее",
                Theme = "Важность",
                Author = "НЛП",
                Active = true,
                TapCommand = new Command(async () => await navigation.PushAsync(new ComparisonPage(), false))
            },
            new TechniqueItem
            {
                Number = "Техника №3",
                Date="26.01.2023",
                Image = image,
                Title = "Полярности",
                Subtitle = "Работа с противоположными аспектами",
                Theme = "Аспекты",
                Author = "Живорад Славинский",
                Active = true,
                TapCommand = new Command(async () => await navigation.PushAsync(new PolarityPage(), false))
            },
            new TechniqueItem
            {
                Number = "Техника №4",
                Date="26.01.2023",
                Image = image,
                Title = "Лист бумаги",
                Subtitle = "Быстрое очищение от негативных мыслей",
                Theme = "Мысли",
                Author = "Психика",
                Active= true,
                TapCommand = new Command(async () => await navigation.PushAsync(new PaperPage(), false))
            },
            new TechniqueItem
            {
                Number = "Техника №5",
                Date="30.01.2023",
                Image = image,
                Title = "50 лет спустя",
                Subtitle = "Понижение важности за 10 секунд",
                Theme = "Важность",
                Author = "НЛП",
                Active = true,
                TapCommand = new Command(async () => await navigation.PushAsync(new FuturePage(), false))
            },

            new TechniqueItem
            {
                Number = "Техника №6",
                Date="30.01.2023",
                Image = image,
                Title = "Протокол Руби",
                Subtitle = "Ликвидация любых привязанностей, зависимостей и привычек",
                Theme = "Обработчик",
                Author = "Турбо-Суслик",
                Active = true,
                TapCommand = new Command(async () => await navigation.PushAsync(new HackPage(), false))
            },

            new TechniqueItem
            {
                Number = "Техника №7",
                Date="08.02.2023",
                Image = image,
                Title = "Модификация опыта",
                Subtitle = "Проработка ограничений, убеждений и моделей поведения",
                Theme = "Эпизоды",
                Author = "Филипп Славинский",
                Active = true,
                TapCommand = new Command(async () => await navigation.PushAsync(new ExperiencePage(), false))
            }
        ];

        return items;
    }

    private TechniqueItem ParseFromDB(INavigation navigation, TechniqueDTO item)
    {
        return new TechniqueItem
        {
            Id = item.TechniqueId,
            Number = "Техника №" + (Techniques.Count + 1),
            Date = item.Date,
            Image = item.Image,
            Title = item.Header,
            Subtitle = item.Describtion,
            Theme = item.Subject,
            Author = item.Author,
            Active = true,
            TapCommand = new Command(
                async () => await navigation.PushAsync(new CreatedPage(item.TechniqueId), false))
        };
    }
}
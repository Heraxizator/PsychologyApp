using AutoMapper;
using MobileHelperMaui.Views.TechniquePages;
using MobileHelperMaui.Views.TechniquePages.ConstructorPages;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Repositories;
using PsychologyApp.Infrastructure.Share;
using PsychologyApp.Infrastructure.Uow;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileHelper.ViewModels.TechniqueViewModels
{
    public class TechniquesViewModel : BaseViewModel
    {
        private TechniqueService _service;

        public ObservableCollection<TechniqueItem> Techniques { get; set; }
        public ICommand ConstructorTapped { get; set; }

        public TechniquesViewModel() { }

        public TechniquesViewModel(INavigation navigation)
        {
            this.Title = "Список техник";

            this.Navigation = navigation;

            UnitService();

            this.Techniques = new ObservableCollection<TechniqueItem>();

            this.ConstructorTapped = new Command((object obj) => this.Navigation.PushAsync(new DesignerPage(-1), false));

            InitAsync();

            SetObservers();
        }

        public async void InitAsync()
        {
            IEnumerable<TechniqueItem> source = GetTechniqueItems();

            foreach (TechniqueItem item in source)
            {
                this.Techniques.Add(item);
            }

            IList<TechniqueDTO> list = await this._service.GetTechniquesList(int.MaxValue);

            foreach (TechniqueDTO item in list)
            {
                this.Techniques.Add(ParseFromDB(item));
            }
        }

        private void UnitService()
        {
            ApplicationDbContext context = new();

            UnitOfWork unitOfWork = new(context);

            GenericRepository<Technique> repository = new(context);

            MapperConfiguration configuration = new(cfg =>
            {
                cfg.CreateMap<TechniqueDTO, Technique>();
                cfg.CreateMap<Technique, TechniqueDTO>();
            });

            Mapper mapper = new(configuration);

            this._service = new TechniqueService(repository, unitOfWork, mapper);
        }

        private void SetObservers()
        {
            MessagingCenter.Subscribe<object, TechniqueDTO>(this, "add", (sender, item) => this.Techniques.Add(ParseFromDB(item)));

            MessagingCenter.Subscribe<object, TechniqueDTO>(this, "remove", (sender, item) =>
            {
                this.Techniques.Clear();

                InitAsync();
            });

            MessagingCenter.Subscribe<object, TechniqueDTO>(this, "change", (sender, item) =>
            {
                this.Techniques.Clear();

                InitAsync();
            });
        }

        private IList<TechniqueItem> GetTechniqueItems()
        {
            string image = "technique.png";

            List<TechniqueItem> items = new()
            {
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new SpinPage(), false))
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new ComparisonPage(), false))
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new PolarityPage(), false))
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new PaperPage(), false))
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new FuturePage(), false))
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new HackPage(), false))
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
                    TapCommand = new Command(async () => await this.Navigation.PushAsync(new ExperiencePage(), false))
                }
            };

            return items;
        }

        private TechniqueItem ParseFromDB(TechniqueDTO item)
        {
            return new TechniqueItem
            {
                Id = item.Id,
                Number = "Техника №" + (this.Techniques.Count + 1),
                Date = item.Date,
                Image = item.Image,
                Title = item.Header,
                Subtitle = item.Describtion,
                Theme = item.Subject,
                Author = item.Author,
                TapCommand = new Command(
                    async () => await this.Navigation.PushAsync(new CreatedPage(item.Id), false))
            };
        }

    }
}
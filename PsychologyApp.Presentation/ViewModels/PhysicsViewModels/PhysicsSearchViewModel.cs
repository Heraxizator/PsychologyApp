using AutoMapper;
using MvvmHelpers;
using PsychologyApp.Application.Helpers;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Infrastructure.Repositories;
using PsychologyApp.Infrastructure.Share;
using PsychologyApp.Infrastructure.Uow;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace MobileHelper.ViewModels.PhysicsViewModels
{
    public class PhysicsSearchViewModel : BaseViewModel
    {
        private ReasonService _service;

        public List<ReasonDTO> Reasons { get; set; }
        public ObservableRangeCollection<ReasonDTO> Results { get; set; }

        private Dictionary<char, List<ReasonDTO>> keyValuePairs { get; set; }

        private readonly Task Initialization;

        public PhysicsSearchViewModel(INavigation navigation)
        {
            this.Title = "Поиск";

            InitService();

            this.Reasons = new List<ReasonDTO>();

            this.Results = new ObservableRangeCollection<ReasonDTO>();

            this.keyValuePairs = new Dictionary<char, List<ReasonDTO>>();

            this.Navigation = navigation;

            this.Reload = new Command(async () => await ReloadAsync());

            this.Cancel = new Command(() => SetFail());

            this.Initialization = InitAsync();
        }

        private void ConfigureState()
        {
            if (this.Reasons.Any() && this.Results.Any())
            {
                SetDone();
                return;
            }

            SetFail();
        }

        private async Task ReloadAsync()
        {
            this.Reasons.Clear();

            this.Results.Clear();

            await InitAsync();
        }

        private async Task PrepareReasons()
        {
            await this._service.SaveReasonsIfEmpty();

            IList<ReasonDTO> reasonDTOs = await this._service.GetReasons(1000);

            this.Reasons.AddRange(reasonDTOs);
        }

        private void PreapreDictionary()
        {
            foreach (ReasonDTO reason in this.Reasons)
            {
                char? symvol = reason.Title?.FirstOrDefault();

                if (symvol is null)
                {
                    continue;
                }

                symvol = char.ToLower((char)symvol);

                if (this.keyValuePairs.ContainsKey((char)symvol) is false)
                {
                    this.keyValuePairs.Add((char)symvol, new List<ReasonDTO>());
                }

                this.keyValuePairs[(char)symvol].Add(reason);
            }
        }

        private void PrepareResults()
        {
            IEnumerable<ReasonDTO> source = this.Reasons.Take(100);

            this.Results.AddRange(source);
        }

        private async Task InitAsync()
        {
            await Task.Run(async () =>
            {
                await PrepareReasons();

                PreapreDictionary();

                PrepareResults();
            });
            
            ConfigureState();
        }

        private void InitService()
        {
            ApplicationDbContext context = new();

            UnitOfWork unitOfWork = new(context);

            GenericRepository<PsychologyApp.Domain.Entities.Reason> repository = new(context);

            MapperConfiguration configuration = new(cfg =>
            {
                cfg.CreateMap<ReasonDTO, PsychologyApp.Domain.Entities.Reason>();
                cfg.CreateMap<PsychologyApp.Domain.Entities.Reason, ReasonDTO>();
            });

            Mapper mapper = new(configuration);

            this._service = new ReasonService(repository, unitOfWork, mapper);
        }

        public void ExecuteSearch(string input)
        {
            SetInit();

            this.Results.Clear();

            if (string.IsNullOrEmpty(input))
            {
                ConfigureState();
                return;
            }

            string text = input.ToLower();

            char? symvol = text.FirstOrDefault();

            if (symvol is null)
            {
                ConfigureState();
                return;
            }

            symvol = char.ToLower((char)symvol);

            if (this.keyValuePairs.ContainsKey((char)symvol) is false)
            {
                ConfigureState();
                return;
            }

            IEnumerable<ReasonDTO> range = this.keyValuePairs[(char)symvol];

            IEnumerable<ReasonDTO> source = range
                .Where(x => x.Title?.Length >= text.Length && x.Title.ToLower().Contains(text));

            this.Results.AddRange(source);

            ConfigureState();
        }

        public PhysicsSearchViewModel() { }

        private string _search_text;
        public string SearchText
        {
            get => this._search_text;
            set
            {
                if (this._search_text != value)
                {
                    this._search_text = value;
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }
    }
}

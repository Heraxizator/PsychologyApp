using AutoMapper;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Repositories;
using PsychologyApp.Infrastructure.Share;
using PsychologyApp.Infrastructure.Uow;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MobileHelper.ViewModels.ProfileViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private QuotService _service;

        public ObservableCollection<TechniqueItem> Techniques { get; set; }
        public ObservableCollection<Quots> Quots { get; set; }

        private readonly Task Initialization;

        public UserViewModel(INavigation navigation)
        {
            this.Title = "Профиль";

            this.Navigation = navigation;

            InitService();

            this.Techniques = new ObservableCollection<TechniqueItem>();

            this.Quots = new ObservableCollection<Quots>();

            Initialization = InitAsync();
        }

        public async Task InitAsync()
        {
            SetInit();

            this.Techniques.Add(new TechniqueItem
            {
                Title = "BSFF",
                Subtitle = "Методика депрограммирования подсознания"
            });

            await this._service.SaveQuotFromApi();

            IList<QuotDTO> quotDTOs = await this._service.GetQuotsList(2);

            foreach (QuotDTO quotDTO in quotDTOs)
            {
                this.Quots.Add(new Quots()
                {
                    Text = quotDTO.Text,
                    Author = quotDTO.Title
                });
            }

            SetDone();
        }

        private void InitService()
        {
            ApplicationDbContext context = new();

            UnitOfWork unitOfWork = new(context);

            GenericRepository<Quot> repository = new(context);

            MapperConfiguration configuration = new(cfg => {
                cfg.CreateMap<QuotDTO, Quot>();
                cfg.CreateMap<Quot, QuotDTO>();
            });

            Mapper mapper = new(configuration);

            this._service = new QuotService(repository, unitOfWork, mapper);
        }
    }
}

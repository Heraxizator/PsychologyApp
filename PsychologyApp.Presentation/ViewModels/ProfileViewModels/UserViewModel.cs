using AutoMapper;
using MobileHelperMaui.Services;
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

        public UserViewModel(INavigation navigation)
        {
            this.Title = "Профиль";

            this.Navigation = navigation;

            InitService();

            this.Techniques = new ObservableCollection<TechniqueItem>();

            this.Quots = new ObservableCollection<Quots>();

            InitAsync();
        }

        public async void InitAsync()
        {
            await this._service.SaveQuotsFromApi(1);

            IList<QuotDTO> quotDTOs = await this._service.GetQuotsList(2);

            MapperConfiguration configuration = new(cfg => {
                cfg.CreateMap<QuotDTO, Quots>();
                cfg.CreateMap<Quots, QuotDTO>();
            });

            Mapper mapper = new(configuration);

            IList<Quots> quots = mapper.Map<IList<Quots>>(quotDTOs);

            foreach (Quots quot in quots)
            {
                this.Quots.Add(quot);
            } 
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

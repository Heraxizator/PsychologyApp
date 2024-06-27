using AutoMapper;
using MobileHelperMaui.Views.TechniquePages.ConstructorPages;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Repositories;
using PsychologyApp.Infrastructure.Share;
using PsychologyApp.Infrastructure.Uow;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ServiceLocator;
using PsychologyApp.Presentation.ServiceLocator.Dialog;
using PsychologyApp.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MobileHelper.ViewModels.ConstructorViewModels
{
    public class CreatedViewModel : BaseViewModel
    {
        private TechniqueService _service;

        public ICommand Remove { get; set; }
        public ICommand Edit { get; set; }
        public ObservableCollection<Items> Elements { get; set; }
        private int currentId { get; set; }

        public CreatedViewModel() { }

        [Obsolete]
        public CreatedViewModel(INavigation navigation, int id)
        {
            this.Title = "Техника";

            this.Navigation = navigation;

            InitService();

            this.Finish = new Command(ToFinish);

            this.Theory = new Command(ToTheory);

            this.Remove = new Command(ToRemove);

            this.Edit = new Command(ToEdit);

            this.Elements = new ObservableCollection<Items>();

            this.currentId = id;

            InitAsync();
        }

        private async void ToEdit(object obj)
        {
            await this.Navigation.PushAsync(new DesignerPage(this.currentId), false);
        }

        private new async void ToFinish(object obj)
        {
            _ = await this.Navigation.PopAsync(false);
        }

        [Obsolete]
        private async void ToRemove(object obj)
        {
            bool result = await ServiceLocator.Instance.GetService<IDialogService>().AskAsync(null, "Вы уверены, что хотите удалить свою технику", "Да", "Нет");

            if (result)
            {
                TechniqueDTO item = await this._service.GetTechniqueById(this.currentId);

                await this._service.DeleteTechnique(item);

                MessagingCenter.Send<object, TechniqueDTO>(this, "remove", item);

                _ = this.Navigation.PopToRootAsync(false);
            }
        }

        private async void InitAsync()
        {
            TechniqueDTO item = await this._service.GetTechniqueById(this.currentId);

            if (item == null)
            {
                ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Не удалось загрузить технику");
                return;
            }

            if (string.IsNullOrEmpty(item.Algorithm))
            {
                ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Алгоритм не найден");
                return;
            }

            string[] actions = item.Algorithm.Split('\n');

            foreach (string action in actions)
            {
                this.Elements.Add(new Items
                {
                    Text = action
                });
            }
        }

        private void InitService()
        {
            ApplicationDbContext context = new();

            UnitOfWork unitOfWork = new(context);

            GenericRepository<Technique> repository = new(context);

            MapperConfiguration configuration = new (cfg => {
                cfg.CreateMap<TechniqueDTO, Technique>();
                cfg.CreateMap<Technique, TechniqueDTO>();
            });

            Mapper mapper = new(configuration);

            this._service = new TechniqueService(repository, unitOfWork, mapper);
        }
    }
}

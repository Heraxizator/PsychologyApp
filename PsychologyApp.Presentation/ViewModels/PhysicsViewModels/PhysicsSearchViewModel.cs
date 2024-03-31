using PsychologyApp.Application.Helpers;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static PsychologyApp.Application.Helpers.PsyhosomaticHelper;

namespace MobileHelper.ViewModels.PhysicsViewModels
{
    public class PhysicsSearchViewModel : BaseViewModel
    {
        public List<Reason> Reasons { get; set; }
        public ObservableCollection<Reason> Results { get; set; }
        private bool _empty { get; set; }
        private bool _full { get; set; }
        public enum State
        {
            Empty,
            Full
        }

        public PhysicsSearchViewModel(INavigation navigation)
        {
            this.Title = "Поиск";

            this.Navigation = navigation;

            this.Reasons = new List<Reason>();

            Task.WhenAll
            (
                InitAsync()
            );

            this.Results = new ObservableCollection<Reason>(this.Reasons.Take(100));
        }

        private async Task InitAsync()
        {
            IList<PsychosomaticObject> psychosomaticObjects = await PsyhosomaticHelper.GetPsyhosomaticData();

            foreach (PsychosomaticObject psychosomaticObject in psychosomaticObjects)
            {
                this.Reasons.Add(new Reason() 
                {
                    Header = psychosomaticObject.ProblemText,
                    Describtion = psychosomaticObject.ProblemReason 
                });
            }
        }

        private void SetDefault()
        {
            this.IsEmpty = false;
            this.IsFull = false;
        }

        private void SetState(State state)
        {
            SetDefault();

            switch (state)
            {
                case State.Empty:
                    this.IsEmpty = true;
                    this.IsFull = false;
                    break;

                case State.Full:
                    this.IsEmpty = false;
                    this.IsFull = true;
                    break;
            }
        }

        public void ExecuteSearch(string input)
        {
            this.Results.Clear();

            string text = input.ToLower();

            List<Reason> search = (from x in this.Reasons 
                         where !string.IsNullOrEmpty(x.Header) && x.Header.Contains(text)
                         select x).ToList();

            foreach (Reason item in search)
            {
                this.Results.Add(item);
            }
            
        }

        public PhysicsSearchViewModel() { }

        public bool IsEmpty
        {
            get => this._empty;
            set
            {
                this._empty = value;
                OnPropertyChanged(nameof(this.IsEmpty));
            }
        }

        public bool IsFull
        {
            get => this._full;
            set
            {
                this._full = value;
                OnPropertyChanged(nameof(this.IsFull));
            }
        }
    }
}

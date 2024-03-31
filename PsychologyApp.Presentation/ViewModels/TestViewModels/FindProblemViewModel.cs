using MobileHelperMaui.Views.TestPages;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MobileHelper.ViewModels.TestViewModels
{
    public class FindProblemViewModel : BaseViewModel
    {
        public ICommand Continue { get; set; }
        private Action _nextPageTappedAction {  get; set; }
        public FindProblemViewModel()
        {

        }

        public FindProblemViewModel(INavigation navigation, string? describtion, List<string> algorithm, string? comment, Action action)
        {
            this.Title = "Детектор";

            this.Navigation = navigation;

            this.DescribtionText = describtion;

            this.AlgorithmRows = new ObservableCollection<AlgorithmItem>();

            foreach (string row in algorithm)
            {
                this.AlgorithmRows.Add
                (
                    new AlgorithmItem { Row = row }
                );
            }

            this.CommentText = comment;

            this._nextPageTappedAction = action;

            this.Continue = new Command(ToContinue);
        }

        private void ToContinue(object obj)
        {
            this._nextPageTappedAction.DynamicInvoke();
        }

        #region Public Properties


        private string? _describtion_text;
        public string? DescribtionText
        {
            get => this._describtion_text;
            set
            {
                if (this._describtion_text != value)
                {
                    this._describtion_text = value;
                    OnPropertyChanged(nameof(this.DescribtionText));
                }
            }
        }

        public ObservableCollection<AlgorithmItem> AlgorithmRows { get; set; }

        private string? _comment_text;
        public string? CommentText
        {
            get => this._comment_text;
            set
            {
                if (this._comment_text != value)
                {
                    this._comment_text = value;
                    OnPropertyChanged(nameof(this.CommentText));
                }
            }
        }

        public class AlgorithmItem
        {
            public string? Row { get; set; }
        }

        #endregion
    }
}

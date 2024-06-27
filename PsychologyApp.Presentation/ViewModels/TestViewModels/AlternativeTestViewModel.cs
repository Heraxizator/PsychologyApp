using Microsoft.Maui.Controls.PlatformConfiguration;
using PsychologyApp.Detector.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.ViewModels.TestViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.TestViewModels
{
    public class AlternativeTestViewModel : BaseTestViewModel
    {

        private const string firstInstruction = "Выберите приятный вам цвет";
        private const string secondInstruction = "А теперь выберите неприятный вам цвет";
        
        public AlternativeTestViewModel()
        {

        }

        public AlternativeTestViewModel(INavigation navigation)
        {
            this.Title = "Тест";
            this.Navigation = navigation;

            this.Finish = new Command(ToFinish);
            this.Restart = new Command(ToRestart);
            this.BlackHandler = new Command(ToBlackHandler);
            this.RedHandler = new Command(ToRedHandler);
            this.BlueHandler = new Command(ToBlueHandler);
            this.PurpleHandler = new Command(ToPurpleHandler);
            this.YellowHandler = new Command(ToYellowCommand);
            this.BrownHandler = new Command(ToBrownHandler);
            this.GreenHandler = new Command(ToGreenHandler);
            this.GrayHandler = new Command(ToGrayHandler);

            Init();
        }

        private void ToRestart(object obj)
        {
            Init();
        }

        private void Init()
        {
            this.CurrentInstruction = firstInstruction;

            this._colourSelectedItems.Clear();

            SetColorsVisibility();

            SetStart();
        }

        protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
        {
            if (this._colourSelectedItems.Any() is false)
            {
                this._colourSelectedItems.Add((colourValue, colourMeaningVoted));

                this.CurrentInstruction = secondInstruction;

                this.FirstResult = colourMeaningVoted.Explaination;

                this.FirstColor = Color.FromArgb(colourValue.Code);

                this.FirstName = colourValue.ToString();
            }

            else
            {
                this._colourSelectedItems.Add((colourValue, colourMeaningUnvoted));

                this.SecondResult = colourMeaningUnvoted.Explaination;

                this.SecondColor = Color.FromArgb(colourValue.Code);

                this.SecondName = colourValue.ToString();

                SetFinish();
            }
        }
    }
}

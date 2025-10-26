using PsychologyApp.Detector.Domain.Colour.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.TestViewModels
{
    public abstract class BaseTestViewModel : BaseViewModel
    {

        protected readonly List<(ColourValue, ColourMeaning)> _colourSelectedItems = new();

        public ICommand? Restart { get; set; }
        public ICommand? BlackHandler { get; set; }
        public ICommand? RedHandler { get; set; }
        public ICommand? BlueHandler { get; set; }
        public ICommand? PurpleHandler { get; set; }
        public ICommand? YellowHandler { get; set; }
        public ICommand? BrownHandler { get; set; }
        public ICommand? GreenHandler { get; set; }
        public ICommand? GrayHandler { get; set; }

        protected void SetColorsVisibility()
        {
            this.IsBlack = true;
            this.IsRed = true;
            this.IsBlue = true;
            this.IsPurple = true;
            this.IsYellow = true;
            this.IsBrown = true;
            this.IsGreen = true;
            this.IsGray = true;
        }

        protected void SetStart()
        {
            this.IsStart = true;
            this.IsFinish = false;
        }

        protected void SetFinish()
        {
            this.IsStart = false;
            this.IsFinish = true;
        }

        protected abstract void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted);

        protected void ToGrayHandler(object obj)
        {
            this.IsGray = false;
            SaveResult(ColourValue.Gray, ColourMeaning.GrayVoted, ColourMeaning.GrayUnvoted);
        }

        protected void ToGreenHandler(object obj)
        {
            this.IsGreen = false;
            SaveResult(ColourValue.Green, ColourMeaning.GreenVoted, ColourMeaning.GreenUnvoted);
        }

        protected void ToBrownHandler(object obj)
        {
            this.IsBrown = false;
            SaveResult(ColourValue.Brown, ColourMeaning.BrownVoted, ColourMeaning.BrownUnvoted);
        }

        protected void ToYellowCommand(object obj)
        {
            this.IsYellow = false;
            SaveResult(ColourValue.Yellow, ColourMeaning.YellowVoted, ColourMeaning.YellowUnvoted);
        }

        protected void ToPurpleHandler(object obj)
        {
            this.IsPurple = false;
            SaveResult(ColourValue.Purple, ColourMeaning.PurpleVoted, ColourMeaning.PurpleUnvoted);
        }

        protected void ToBlueHandler(object obj)
        {
            this.IsBlue = false;
            SaveResult(ColourValue.Blue, ColourMeaning.BlueVoted, ColourMeaning.BlueUnvoted);
        }

        protected void ToRedHandler(object obj)
        {
            this.IsRed = false;
            SaveResult(ColourValue.Red, ColourMeaning.RedVoted, ColourMeaning.RedUnvoted);
        }

        protected void ToBlackHandler(object obj)
        {
            this.IsBlack = false;
            SaveResult(ColourValue.Black, ColourMeaning.BlackVoted, ColourMeaning.BlackUnvoted);

        }

        protected string? currentInstruction { get; set; }
        public string? CurrentInstruction
        {
            get => this.currentInstruction;
            set
            {
                if (this.currentInstruction != value)
                {
                    this.currentInstruction = value;
                    OnPropertyChanged(nameof(this.CurrentInstruction));
                }
            }
        }

        protected string? firstResult { get; set; }
        public string? FirstResult
        {
            get => this.firstResult;
            set
            {
                if (this.firstResult != value)
                {
                    this.firstResult = value;
                    OnPropertyChanged(nameof(this.FirstResult));
                }
            }
        }

        protected string? secondResult { get; set; }
        public string? SecondResult
        {
            get => this.secondResult;
            set
            {
                if (this.secondResult != value)
                {
                    this.secondResult = value;
                    OnPropertyChanged(nameof(this.SecondResult));
                }
            }
        }

        protected Color? firstColor { get; set; }
        public Color? FirstColor
        {
            get => this.firstColor;
            set
            {
                if (this.firstColor != value)
                {
                    this.firstColor = value;
                    OnPropertyChanged(nameof(this.FirstColor));
                }
            }
        }

        protected Color? secondColor { get; set; }
        public Color? SecondColor
        {
            get => this.secondColor;
            set
            {
                if (this.secondColor != value)
                {
                    this.secondColor = value;
                    OnPropertyChanged(nameof(this.SecondColor));
                }
            }
        }

        protected string? firstName { get; set; }
        public string? FirstName
        {
            get => this.firstName;
            set
            {
                if (this.firstName != value)
                {
                    this.firstName = value;
                    OnPropertyChanged(nameof(this.FirstName));
                }
            }
        }

        protected string? secondName { get; set; }
        public string? SecondName
        {
            get => this.secondName;
            set
            {
                if (this.secondName != value)
                {
                    this.secondName = value;
                    OnPropertyChanged(nameof(this.SecondName));
                }
            }
        }

        protected bool isStart { get; set; }
        public bool IsStart
        {
            get => this.isStart;
            set
            {
                if (this.isStart != value)
                {
                    this.isStart = value;
                    OnPropertyChanged(nameof(this.IsStart));
                }
            }
        }

        protected bool isFinish { get; set; }
        public bool IsFinish
        {
            get => this.isFinish;
            set
            {
                if (this.isFinish != value)
                {
                    this.isFinish = value;
                    OnPropertyChanged(nameof(this.IsFinish));
                }
            }
        }

        protected bool isBlack { get; set; }
        public bool IsBlack
        {
            get => this.isBlack;
            set
            {
                if (this.isBlack != value)
                {
                    this.isBlack = value;
                    OnPropertyChanged(nameof(this.IsBlack));
                }
            }
        }

        protected bool isRed { get; set; }
        public bool IsRed
        {
            get => this.isRed;
            set
            {
                if (this.isRed != value)
                {
                    this.isRed = value;
                    OnPropertyChanged(nameof(this.IsRed));
                }
            }
        }

        protected bool isBlue { get; set; }
        public bool IsBlue
        {
            get => this.isBlue;
            set
            {
                if (this.isBlue != value)
                {
                    this.isBlue = value;
                    OnPropertyChanged(nameof(this.IsBlue));
                }
            }
        }

        protected bool isPurple { get; set; }
        public bool IsPurple
        {
            get => this.isPurple;
            set
            {
                if (this.isPurple != value)
                {
                    this.isPurple = value;
                    OnPropertyChanged(nameof(this.IsPurple));
                }
            }
        }

        protected bool isYellow { get; set; }
        public bool IsYellow
        {
            get => this.isYellow;
            set
            {
                if (this.isYellow != value)
                {
                    this.isYellow = value;
                    OnPropertyChanged(nameof(this.IsYellow));
                }
            }
        }

        protected bool isBrown { get; set; }
        public bool IsBrown
        {
            get => this.isBrown;
            set
            {
                if (this.isBrown != value)
                {
                    this.isBrown = value;
                    OnPropertyChanged(nameof(this.IsBrown));
                }
            }
        }

        protected bool isGreen { get; set; }
        public bool IsGreen
        {
            get => this.isGreen;
            set
            {
                if (this.isGreen != value)
                {
                    this.isGreen = value;
                    OnPropertyChanged(nameof(this.IsGreen));
                }
            }
        }

        protected bool isGray { get; set; }
        public bool IsGray
        {
            get => this.isGray;
            set
            {
                if (this.isGray != value)
                {
                    this.isGray = value;
                    OnPropertyChanged(nameof(this.IsGray));
                }
            }
        }
    }
}

using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestAnswerOptionView : ContentView
{
    public TestAnswerOptionView()
    {
        InitializeComponent();
        TemplatePressFeedback.Attach(this, new PressFeedbackOptions { HapticOnRelease = true });
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(TestAnswerOptionView),
            string.Empty,
            propertyChanged: OnSemanticsPropertyChanged);

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(TestAnswerOptionView),
            false,
            propertyChanged: OnIsSelectedChanged);

    public static readonly BindableProperty IsSingleChoiceProperty =
        BindableProperty.Create(
            nameof(IsSingleChoice),
            typeof(bool),
            typeof(TestAnswerOptionView),
            true,
            propertyChanged: OnSemanticsPropertyChanged);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TestAnswerOptionView), default);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsSingleChoice
    {
        get => (bool)GetValue(IsSingleChoiceProperty);
        set => SetValue(IsSingleChoiceProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestAnswerOptionView view)
        {
            if (newValue is true)
            {
                UiAnimations.SafePulseAsync(view).FireAndForget();
            }

            UpdateSemantics(view);
        }
    }

    private static void OnSemanticsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestAnswerOptionView view)
        {
            UpdateSemantics(view);
        }
    }

    private static void UpdateSemantics(TestAnswerOptionView view)
    {
        string description = string.IsNullOrWhiteSpace(view.Text) ? AppStrings.TestsAnswerOption : view.Text;
        SemanticProperties.SetDescription(view, description);

        string selectionState = view.IsSelected ? AppStrings.TestsAnswerSelected : AppStrings.TestsAnswerNotSelected;
        string modeHint = view.IsSingleChoice
            ? AppStrings.TestsSingleChoiceHint
            : AppStrings.TestsMultiChoiceHint;
        SemanticProperties.SetHint(view, $"{selectionState}. {modeHint}");
    }

    private async void OnOptionTapped(object? sender, TappedEventArgs e)
    {
        await UiAnimations.SafePulseAsync(this);

        if (TapCommand?.CanExecute(null) == true)
        {
            TapCommand.Execute(null);
        }
    }
}

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
        BindableProperty.Create(nameof(Text), typeof(string), typeof(TestAnswerOptionView), string.Empty);

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(TestAnswerOptionView),
            false,
            propertyChanged: OnIsSelectedChanged);

    public static readonly BindableProperty IsSingleChoiceProperty =
        BindableProperty.Create(nameof(IsSingleChoice), typeof(bool), typeof(TestAnswerOptionView), true);

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
        if (bindable is TestAnswerOptionView view && newValue is true)
        {
            UiAnimations.SafePulseAsync(view).FireAndForget();
        }
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

using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class RetryView : ContentView
{
    public RetryView()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsVisible) && IsVisible)
        {
            UiAnimations.SafeRevealPremiumAsync(this, allowHidden: true).FireAndForget();
        }
    }

    public static readonly BindableProperty FailedTextProperty =
        BindableProperty.Create(nameof(FailedText), typeof(string), typeof(RetryView), string.Empty);

    public string FailedText
    {
        get => (string)GetValue(FailedTextProperty);
        set => SetValue(FailedTextProperty, value);
    }

    public static readonly BindableProperty RetryTextProperty =
        BindableProperty.Create(nameof(RetryText), typeof(string), typeof(RetryView), string.Empty);

    public string RetryText
    {
        get => (string)GetValue(RetryTextProperty);
        set => SetValue(RetryTextProperty, value);
    }

    public static readonly BindableProperty RetryCommandProperty =
        BindableProperty.Create(nameof(RetryCommand), typeof(ICommand), typeof(RetryView), null);

    public ICommand? RetryCommand
    {
        get => (ICommand?)GetValue(RetryCommandProperty);
        set => SetValue(RetryCommandProperty, value);
    }
}

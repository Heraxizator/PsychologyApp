using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class ProgressBarView : ContentView
{
    public ProgressBarView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(ProgressBarView), false);

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly BindableProperty ProgressTextProperty =
        BindableProperty.Create(nameof(ProgressText), typeof(string), typeof(ProgressBarView), string.Empty);

    public string ProgressText
    {
        get => (string)GetValue(ProgressTextProperty);
        set => SetValue(ProgressTextProperty, value);
    }

    public static readonly BindableProperty CancelCommandProperty =
        BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(ProgressBarView), null);

    public ICommand? CancelCommand
    {
        get => (ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
}

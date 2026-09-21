using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// Labelled two-option (yes / no) segmented choice. <see cref="SelectCommand"/> receives "yes" or "no".
/// </summary>
public partial class YesNoSelectorView : ContentView
{
    public YesNoSelectorView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(YesNoSelectorView), string.Empty);

    public static readonly BindableProperty YesTextProperty =
        BindableProperty.Create(nameof(YesText), typeof(string), typeof(YesNoSelectorView), string.Empty);

    public static readonly BindableProperty NoTextProperty =
        BindableProperty.Create(nameof(NoText), typeof(string), typeof(YesNoSelectorView), string.Empty);

    public static readonly BindableProperty IsYesProperty =
        BindableProperty.Create(nameof(IsYes), typeof(bool), typeof(YesNoSelectorView), false);

    public static readonly BindableProperty IsNoProperty =
        BindableProperty.Create(nameof(IsNo), typeof(bool), typeof(YesNoSelectorView), false);

    public static readonly BindableProperty SelectCommandProperty =
        BindableProperty.Create(nameof(SelectCommand), typeof(ICommand), typeof(YesNoSelectorView), null);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public string YesText
    {
        get => (string)GetValue(YesTextProperty);
        set => SetValue(YesTextProperty, value);
    }

    public string NoText
    {
        get => (string)GetValue(NoTextProperty);
        set => SetValue(NoTextProperty, value);
    }

    public bool IsYes
    {
        get => (bool)GetValue(IsYesProperty);
        set => SetValue(IsYesProperty, value);
    }

    public bool IsNo
    {
        get => (bool)GetValue(IsNoProperty);
        set => SetValue(IsNoProperty, value);
    }

    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }
}

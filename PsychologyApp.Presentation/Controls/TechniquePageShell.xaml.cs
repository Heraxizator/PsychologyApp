using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Controls;

public partial class TechniquePageShell : ContentView
{
    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public static readonly BindableProperty AlgorithmProperty =
        BindableProperty.Create(nameof(Algorithm), typeof(IEnumerable), typeof(TechniquePageShell));

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(TechniquePageShell));

    public static readonly BindableProperty TheoryCommandProperty =
        BindableProperty.Create(nameof(TheoryCommand), typeof(ICommand), typeof(TechniquePageShell));

    public static readonly BindableProperty FinishCommandProperty =
        BindableProperty.Create(nameof(FinishCommand), typeof(ICommand), typeof(TechniquePageShell));

    public static readonly BindableProperty BodyContentProperty =
        BindableProperty.Create(nameof(BodyContent), typeof(View), typeof(TechniquePageShell));

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public IEnumerable? Algorithm
    {
        get => (IEnumerable?)GetValue(AlgorithmProperty);
        set => SetValue(AlgorithmProperty, value);
    }

    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public ICommand? TheoryCommand
    {
        get => (ICommand?)GetValue(TheoryCommandProperty);
        set => SetValue(TheoryCommandProperty, value);
    }

    public ICommand? FinishCommand
    {
        get => (ICommand?)GetValue(FinishCommandProperty);
        set => SetValue(FinishCommandProperty, value);
    }

    public View? BodyContent
    {
        get => (View?)GetValue(BodyContentProperty);
        set => SetValue(BodyContentProperty, value);
    }

    public TechniquePageShell() => InitializeComponent();
}

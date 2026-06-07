using System.Collections;
using System.Windows.Input;
using PsychologyApp.Presentation.Infrastructure;

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
        BindableProperty.Create(
            nameof(BodyContent),
            typeof(View),
            typeof(TechniquePageShell),
            propertyChanged: OnBodyContentChanged);

    public static readonly BindableProperty TheoryTextProperty =
        BindableProperty.Create(nameof(TheoryText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public static readonly BindableProperty AlgorithmTitleTextProperty =
        BindableProperty.Create(nameof(AlgorithmTitleText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public static readonly BindableProperty FinishTextProperty =
        BindableProperty.Create(nameof(FinishText), typeof(string), typeof(TechniquePageShell), string.Empty);

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

    public string TheoryText
    {
        get => (string)GetValue(TheoryTextProperty);
        set => SetValue(TheoryTextProperty, value);
    }

    public string AlgorithmTitleText
    {
        get => (string)GetValue(AlgorithmTitleTextProperty);
        set => SetValue(AlgorithmTitleTextProperty, value);
    }

    public string FinishText
    {
        get => (string)GetValue(FinishTextProperty);
        set => SetValue(FinishTextProperty, value);
    }

    public TechniquePageShell()
    {
        InitializeComponent();
        ApplyLocalization();
        UserPreferences.Changed += ApplyLocalization;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        SyncBodyBindingContext();
    }

    private static void OnBodyContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TechniquePageShell shell)
        {
            return;
        }

        shell.UpdateBodyHost(oldValue as View, newValue as View);
    }

    private void UpdateBodyHost(View? oldBody, View? newBody)
    {
        if (oldBody is not null)
        {
            BodyHost.Children.Remove(oldBody);
        }

        if (newBody is null)
        {
            return;
        }

        newBody.BindingContext = BindingContext;
        BodyHost.Children.Add(newBody);
    }

    private void SyncBodyBindingContext()
    {
        if (BodyContent is not null)
        {
            BodyContent.BindingContext = BindingContext;
        }
    }

    private void ApplyLocalization()
    {
        TheoryText = AppStrings.TechniqueTheory;
        AlgorithmTitleText = AppStrings.TechniqueAlgorithm;
        FinishText = AppStrings.TechniqueFinish;
    }
}

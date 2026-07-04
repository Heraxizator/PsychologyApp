using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class FilterChipView : ContentView
{
    public FilterChipView() => InitializeComponent();

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(FilterChipView), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool),
            typeof(FilterChipView),
            false,
            propertyChanged: OnIsSelectedChanged);

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(FilterChipView));

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FilterChipView));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not FilterChipView chip || newValue is not true || oldValue is true)
        {
            return;
        }

        UiAnimations.SafePulseAsync(chip).FireAndForget();
    }
}

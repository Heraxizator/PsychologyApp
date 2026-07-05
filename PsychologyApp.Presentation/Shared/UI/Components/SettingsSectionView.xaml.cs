namespace PsychologyApp.Presentation.Shared.UI.Components;

[ContentProperty(nameof(SectionContent))]
public partial class SettingsSectionView : ContentView
{
    public SettingsSectionView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(SettingsSectionView), string.Empty);

    public static readonly BindableProperty SubtitleTextProperty =
        BindableProperty.Create(nameof(SubtitleText), typeof(string), typeof(SettingsSectionView), string.Empty);

    public static readonly BindableProperty SectionContentProperty =
        BindableProperty.Create(nameof(SectionContent), typeof(View), typeof(SettingsSectionView), null);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string SubtitleText
    {
        get => (string)GetValue(SubtitleTextProperty);
        set => SetValue(SubtitleTextProperty, value);
    }

    public View? SectionContent
    {
        get => (View?)GetValue(SectionContentProperty);
        set => SetValue(SectionContentProperty, value);
    }
}

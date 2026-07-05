using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class LuscherHistoryDetailView : ContentView
{
    public LuscherHistoryDetailView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty HasStandardDetailProperty =
        BindableProperty.Create(nameof(HasStandardDetail), typeof(bool), typeof(LuscherHistoryDetailView), false);

    public static readonly BindableProperty HasBriefDetailProperty =
        BindableProperty.Create(nameof(HasBriefDetail), typeof(bool), typeof(LuscherHistoryDetailView), false);

    public static readonly BindableProperty FirstPassTitleProperty =
        BindableProperty.Create(nameof(FirstPassTitle), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty SecondPassTitleProperty =
        BindableProperty.Create(nameof(SecondPassTitle), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BkTextProperty =
        BindableProperty.Create(nameof(BkText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty StandardDetailProperty =
        BindableProperty.Create(
            nameof(StandardDetail),
            typeof(LuscherStandardResultDetail),
            typeof(LuscherHistoryDetailView),
            null,
            propertyChanged: OnStandardDetailChanged);

    public static readonly BindableProperty BriefDetailProperty =
        BindableProperty.Create(
            nameof(BriefDetail),
            typeof(LuscherBriefResultDetail),
            typeof(LuscherHistoryDetailView),
            null,
            propertyChanged: OnBriefDetailChanged);

    public static readonly BindableProperty StandardFirstPassColorsProperty =
        BindableProperty.Create(
            nameof(StandardFirstPassColors),
            typeof(IReadOnlyList<LuscherColorDisplayItem>),
            typeof(LuscherHistoryDetailView),
            Array.Empty<LuscherColorDisplayItem>());

    public static readonly BindableProperty StandardSecondPassColorsProperty =
        BindableProperty.Create(
            nameof(StandardSecondPassColors),
            typeof(IReadOnlyList<LuscherColorDisplayItem>),
            typeof(LuscherHistoryDetailView),
            Array.Empty<LuscherColorDisplayItem>());

    public static readonly BindableProperty BriefFirstRoleLabelProperty =
        BindableProperty.Create(nameof(BriefFirstRoleLabel), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefSecondRoleLabelProperty =
        BindableProperty.Create(nameof(BriefSecondRoleLabel), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefFirstColorProperty =
        BindableProperty.Create(nameof(BriefFirstColor), typeof(Color), typeof(LuscherHistoryDetailView), Colors.Transparent);

    public static readonly BindableProperty BriefSecondColorProperty =
        BindableProperty.Create(nameof(BriefSecondColor), typeof(Color), typeof(LuscherHistoryDetailView), Colors.Transparent);

    public static readonly BindableProperty BriefFirstNameProperty =
        BindableProperty.Create(nameof(BriefFirstName), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefSecondNameProperty =
        BindableProperty.Create(nameof(BriefSecondName), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefFirstTextProperty =
        BindableProperty.Create(nameof(BriefFirstText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefSecondTextProperty =
        BindableProperty.Create(nameof(BriefSecondText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public bool HasStandardDetail
    {
        get => (bool)GetValue(HasStandardDetailProperty);
        set => SetValue(HasStandardDetailProperty, value);
    }

    public bool HasBriefDetail
    {
        get => (bool)GetValue(HasBriefDetailProperty);
        set => SetValue(HasBriefDetailProperty, value);
    }

    public string FirstPassTitle
    {
        get => (string)GetValue(FirstPassTitleProperty);
        set => SetValue(FirstPassTitleProperty, value);
    }

    public string SecondPassTitle
    {
        get => (string)GetValue(SecondPassTitleProperty);
        set => SetValue(SecondPassTitleProperty, value);
    }

    public string BkText
    {
        get => (string)GetValue(BkTextProperty);
        set => SetValue(BkTextProperty, value);
    }

    public LuscherStandardResultDetail? StandardDetail
    {
        get => (LuscherStandardResultDetail?)GetValue(StandardDetailProperty);
        set => SetValue(StandardDetailProperty, value);
    }

    public LuscherBriefResultDetail? BriefDetail
    {
        get => (LuscherBriefResultDetail?)GetValue(BriefDetailProperty);
        set => SetValue(BriefDetailProperty, value);
    }

    public IReadOnlyList<LuscherColorDisplayItem> StandardFirstPassColors
    {
        get => (IReadOnlyList<LuscherColorDisplayItem>)GetValue(StandardFirstPassColorsProperty);
        set => SetValue(StandardFirstPassColorsProperty, value);
    }

    public IReadOnlyList<LuscherColorDisplayItem> StandardSecondPassColors
    {
        get => (IReadOnlyList<LuscherColorDisplayItem>)GetValue(StandardSecondPassColorsProperty);
        set => SetValue(StandardSecondPassColorsProperty, value);
    }

    public string BriefFirstRoleLabel
    {
        get => (string)GetValue(BriefFirstRoleLabelProperty);
        set => SetValue(BriefFirstRoleLabelProperty, value);
    }

    public string BriefSecondRoleLabel
    {
        get => (string)GetValue(BriefSecondRoleLabelProperty);
        set => SetValue(BriefSecondRoleLabelProperty, value);
    }

    public Color BriefFirstColor
    {
        get => (Color)GetValue(BriefFirstColorProperty);
        set => SetValue(BriefFirstColorProperty, value);
    }

    public Color BriefSecondColor
    {
        get => (Color)GetValue(BriefSecondColorProperty);
        set => SetValue(BriefSecondColorProperty, value);
    }

    public string BriefFirstName
    {
        get => (string)GetValue(BriefFirstNameProperty);
        set => SetValue(BriefFirstNameProperty, value);
    }

    public string BriefSecondName
    {
        get => (string)GetValue(BriefSecondNameProperty);
        set => SetValue(BriefSecondNameProperty, value);
    }

    public string BriefFirstText
    {
        get => (string)GetValue(BriefFirstTextProperty);
        set => SetValue(BriefFirstTextProperty, value);
    }

    public string BriefSecondText
    {
        get => (string)GetValue(BriefSecondTextProperty);
        set => SetValue(BriefSecondTextProperty, value);
    }

    private static void OnStandardDetailChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not LuscherHistoryDetailView view || newValue is not LuscherStandardResultDetail detail)
        {
            return;
        }

        view.StandardFirstPassColors = LuscherColorDisplayFactory.FromStandardPass(detail.FirstPassColors);
        view.StandardSecondPassColors = LuscherColorDisplayFactory.FromStandardPass(detail.Colors);
    }

    private static void OnBriefDetailChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not LuscherHistoryDetailView view || newValue is not LuscherBriefResultDetail detail)
        {
            return;
        }

        LuscherColorDisplayItem first = LuscherColorDisplayFactory.FromBriefColor(detail.First);
        LuscherColorDisplayItem second = LuscherColorDisplayFactory.FromBriefColor(detail.Second);
        view.BriefFirstColor = first.MauiColor;
        view.BriefFirstName = first.Name;
        view.BriefFirstText = detail.First?.Text ?? string.Empty;
        view.BriefSecondColor = second.MauiColor;
        view.BriefSecondName = second.Name;
        view.BriefSecondText = detail.Second?.Text ?? string.Empty;
    }
}

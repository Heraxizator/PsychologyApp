using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.DailyQuoteHero;

public partial class DailyQuoteHeroView : ContentView
{
    public DailyQuoteHeroView()
    {
        InitializeComponent();
        LocalizedContentView.SubscribePreferences(this, ApplyLocalization);
        ApplyLocalization();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        AttachIconPressFeedback(FavoriteActionBorder);
        AttachIconPressFeedback(CopyActionBorder);
        AttachIconPressFeedback(ShareActionBorder);
    }

    private static void AttachIconPressFeedback(Border border) =>
        VisualElementPressFeedback.Attach(border, new PressFeedbackOptions { HapticOnRelease = true });

    private void ApplyLocalization()
    {
        FavoriteHint = AppStrings.QuoteAddFavoriteHint;
        CopyHint = AppStrings.QuoteCopyHint;
        ShareHint = AppStrings.QuoteShareHint;
        UpdateAuthorLine();
    }

    private void UpdateAuthorLine()
    {
        AuthorLine = string.IsNullOrWhiteSpace(AuthorText)
            ? string.Empty
            : $"— {AuthorText}";
    }

    public static readonly BindableProperty CaptionTextProperty =
        BindableProperty.Create(nameof(CaptionText), typeof(string), typeof(DailyQuoteHeroView), string.Empty);

    public string CaptionText
    {
        get => (string)GetValue(CaptionTextProperty);
        set => SetValue(CaptionTextProperty, value);
    }

    public static readonly BindableProperty QuoteTextProperty =
        BindableProperty.Create(nameof(QuoteText), typeof(string), typeof(DailyQuoteHeroView), string.Empty);

    public string QuoteText
    {
        get => (string)GetValue(QuoteTextProperty);
        set => SetValue(QuoteTextProperty, value);
    }

    public static readonly BindableProperty AuthorTextProperty =
        BindableProperty.Create(
            nameof(AuthorText),
            typeof(string),
            typeof(DailyQuoteHeroView),
            string.Empty,
            propertyChanged: OnAuthorTextChanged);

    public string AuthorText
    {
        get => (string)GetValue(AuthorTextProperty);
        set => SetValue(AuthorTextProperty, value);
    }

    public static readonly BindableProperty AuthorLineProperty =
        BindableProperty.Create(nameof(AuthorLine), typeof(string), typeof(DailyQuoteHeroView), string.Empty);

    public string AuthorLine
    {
        get => (string)GetValue(AuthorLineProperty);
        private set => SetValue(AuthorLineProperty, value);
    }

    public static readonly BindableProperty IsFavouriteProperty =
        BindableProperty.Create(
            nameof(IsFavourite),
            typeof(bool),
            typeof(DailyQuoteHeroView),
            false,
            propertyChanged: OnIsFavouriteChanged);

    public bool IsFavourite
    {
        get => (bool)GetValue(IsFavouriteProperty);
        set => SetValue(IsFavouriteProperty, value);
    }

    public static readonly BindableProperty LikeCommandProperty =
        BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(DailyQuoteHeroView), null);

    public ICommand? LikeCommand
    {
        get => (ICommand?)GetValue(LikeCommandProperty);
        set => SetValue(LikeCommandProperty, value);
    }

    public static readonly BindableProperty ShareCommandProperty =
        BindableProperty.Create(nameof(ShareCommand), typeof(ICommand), typeof(DailyQuoteHeroView), null);

    public ICommand? ShareCommand
    {
        get => (ICommand?)GetValue(ShareCommandProperty);
        set => SetValue(ShareCommandProperty, value);
    }

    public static readonly BindableProperty CopyCommandProperty =
        BindableProperty.Create(nameof(CopyCommand), typeof(ICommand), typeof(DailyQuoteHeroView), null);

    public ICommand? CopyCommand
    {
        get => (ICommand?)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }

    public static readonly BindableProperty FavoriteHintProperty =
        BindableProperty.Create(nameof(FavoriteHint), typeof(string), typeof(DailyQuoteHeroView), string.Empty);

    public string FavoriteHint
    {
        get => (string)GetValue(FavoriteHintProperty);
        set => SetValue(FavoriteHintProperty, value);
    }

    public static readonly BindableProperty CopyHintProperty =
        BindableProperty.Create(nameof(CopyHint), typeof(string), typeof(DailyQuoteHeroView), string.Empty);

    public string CopyHint
    {
        get => (string)GetValue(CopyHintProperty);
        set => SetValue(CopyHintProperty, value);
    }

    public static readonly BindableProperty ShareHintProperty =
        BindableProperty.Create(nameof(ShareHint), typeof(string), typeof(DailyQuoteHeroView), string.Empty);

    public string ShareHint
    {
        get => (string)GetValue(ShareHintProperty);
        set => SetValue(ShareHintProperty, value);
    }

    private static void OnAuthorTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DailyQuoteHeroView view)
        {
            view.UpdateAuthorLine();
        }
    }

    private static void OnIsFavouriteChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DailyQuoteHeroView view)
        {
            FavoriteIconAnimator.PulseIfFavoriteChanged(
                oldValue is true,
                newValue is true,
                view.FavoriteActionBorder);
        }
    }
}

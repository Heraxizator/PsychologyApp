using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.QuoteCard;

public partial class QuoteCardView : ContentView
{
    public QuoteCardView()
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
        DefaultAuthorText = AppStrings.ProverbLabel;
        FavoriteHint = AppStrings.QuoteAddFavoriteHint;
        CopyHint = AppStrings.QuoteCopyHint;
        ShareHint = AppStrings.QuoteShareHint;
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(QuoteCardView), string.Empty);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty AuthorProperty =
        BindableProperty.Create(nameof(Author), typeof(string), typeof(QuoteCardView), string.Empty);

    public string Author
    {
        get => (string)GetValue(AuthorProperty);
        set => SetValue(AuthorProperty, value);
    }

    public static readonly BindableProperty IsFavouriteProperty =
        BindableProperty.Create(nameof(IsFavourite), typeof(bool), typeof(QuoteCardView), false);

    public bool IsFavourite
    {
        get => (bool)GetValue(IsFavouriteProperty);
        set => SetValue(IsFavouriteProperty, value);
    }

    public static readonly BindableProperty LikeCommandProperty =
        BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(QuoteCardView), null);

    public ICommand? LikeCommand
    {
        get => (ICommand?)GetValue(LikeCommandProperty);
        set => SetValue(LikeCommandProperty, value);
    }

    public static readonly BindableProperty CopyCommandProperty =
        BindableProperty.Create(nameof(CopyCommand), typeof(ICommand), typeof(QuoteCardView), null);

    public ICommand? CopyCommand
    {
        get => (ICommand?)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }

    public static readonly BindableProperty ShareCommandProperty =
        BindableProperty.Create(nameof(ShareCommand), typeof(ICommand), typeof(QuoteCardView), null);

    public ICommand? ShareCommand
    {
        get => (ICommand?)GetValue(ShareCommandProperty);
        set => SetValue(ShareCommandProperty, value);
    }

    public static readonly BindableProperty DefaultAuthorTextProperty =
        BindableProperty.Create(nameof(DefaultAuthorText), typeof(string), typeof(QuoteCardView), string.Empty);

    public string DefaultAuthorText
    {
        get => (string)GetValue(DefaultAuthorTextProperty);
        set => SetValue(DefaultAuthorTextProperty, value);
    }

    public static readonly BindableProperty FavoriteHintProperty =
        BindableProperty.Create(nameof(FavoriteHint), typeof(string), typeof(QuoteCardView), string.Empty);

    public string FavoriteHint
    {
        get => (string)GetValue(FavoriteHintProperty);
        set => SetValue(FavoriteHintProperty, value);
    }

    public static readonly BindableProperty CopyHintProperty =
        BindableProperty.Create(nameof(CopyHint), typeof(string), typeof(QuoteCardView), string.Empty);

    public string CopyHint
    {
        get => (string)GetValue(CopyHintProperty);
        set => SetValue(CopyHintProperty, value);
    }

    public static readonly BindableProperty ShareHintProperty =
        BindableProperty.Create(nameof(ShareHint), typeof(string), typeof(QuoteCardView), string.Empty);

    public string ShareHint
    {
        get => (string)GetValue(ShareHintProperty);
        set => SetValue(ShareHintProperty, value);
    }
}

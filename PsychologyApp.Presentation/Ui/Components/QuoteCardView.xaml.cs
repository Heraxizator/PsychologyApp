using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class QuoteCardView : ContentView
{
    public QuoteCardView()
    {
        InitializeComponent();
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
        BindableProperty.Create(nameof(IsFavourite), typeof(bool), typeof(QuoteCardView), false, BindingMode.TwoWay);

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

    public static readonly BindableProperty ShareCommandProperty =
        BindableProperty.Create(nameof(ShareCommand), typeof(ICommand), typeof(QuoteCardView), null);

    public ICommand? ShareCommand
    {
        get => (ICommand?)GetValue(ShareCommandProperty);
        set => SetValue(ShareCommandProperty, value);
    }

    public static readonly BindableProperty CopyCommandProperty =
        BindableProperty.Create(nameof(CopyCommand), typeof(ICommand), typeof(QuoteCardView), null);

    public ICommand? CopyCommand
    {
        get => (ICommand?)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }
}

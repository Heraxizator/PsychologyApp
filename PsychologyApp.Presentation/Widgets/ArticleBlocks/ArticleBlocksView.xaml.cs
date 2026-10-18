namespace PsychologyApp.Presentation.Widgets.ArticleBlocks;

/// <summary>The blocks of an article or a course day: text, links, checklists, questions, loops, breathing and the day's task.</summary>
public partial class ArticleBlocksView : ContentView
{
    public static readonly BindableProperty BlocksProperty =
        BindableProperty.Create(nameof(Blocks), typeof(IReadOnlyList<ArticleBlockItem>), typeof(ArticleBlocksView), Array.Empty<ArticleBlockItem>());

    public ArticleBlocksView()
    {
        InitializeComponent();
    }

    public IReadOnlyList<ArticleBlockItem> Blocks
    {
        get => (IReadOnlyList<ArticleBlockItem>)GetValue(BlocksProperty);
        set => SetValue(BlocksProperty, value);
    }
}

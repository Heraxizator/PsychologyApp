using PsychologyApp.Application.Articles;

namespace PsychologyApp.Presentation.Widgets.ArticleBlocks;

/// <summary>One template per kind of block, so a paragraph does not carry a hidden checklist or breathing circle.</summary>
public sealed class ArticleBlockTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Paragraph { get; set; }
    public DataTemplate? Heading { get; set; }
    public DataTemplate? Bullets { get; set; }
    public DataTemplate? Warning { get; set; }
    public DataTemplate? Link { get; set; }
    public DataTemplate? Checklist { get; set; }
    public DataTemplate? Quiz { get; set; }
    public DataTemplate? Cycle { get; set; }
    public DataTemplate? Breathe { get; set; }
    public DataTemplate? Task { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        ArticleBlockItem block = (ArticleBlockItem)item;
        DataTemplate? template = block.Kind switch
        {
            ArticleBlockKind.Heading => Heading,
            ArticleBlockKind.Bullets => Bullets,
            ArticleBlockKind.Warning => Warning,
            ArticleBlockKind.Checklist => Checklist,
            ArticleBlockKind.Quiz => Quiz,
            ArticleBlockKind.Cycle => Cycle,
            ArticleBlockKind.Breathe => Breathe,
            ArticleBlockKind.Task => Task,
            _ when block.IsLink => Link,
            _ => Paragraph
        };
        return template ?? Paragraph!;
    }
}

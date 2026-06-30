namespace PsychologyApp.Presentation.Widgets.Test;

public partial class QuestionnaireResultDetailView : ContentView
{
    public QuestionnaireResultDetailView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty QuestionsProperty =
        BindableProperty.Create(
            nameof(Questions),
            typeof(IReadOnlyList<QuestionnaireResultQuestion>),
            typeof(QuestionnaireResultDetailView),
            Array.Empty<QuestionnaireResultQuestion>());

    public static readonly BindableProperty DurationTextProperty =
        BindableProperty.Create(nameof(DurationText), typeof(string), typeof(QuestionnaireResultDetailView), string.Empty);

    public static readonly BindableProperty HasDurationProperty =
        BindableProperty.Create(nameof(HasDuration), typeof(bool), typeof(QuestionnaireResultDetailView), false);

    public IReadOnlyList<QuestionnaireResultQuestion> Questions
    {
        get => (IReadOnlyList<QuestionnaireResultQuestion>)GetValue(QuestionsProperty);
        set => SetValue(QuestionsProperty, value);
    }

    public string DurationText
    {
        get => (string)GetValue(DurationTextProperty);
        set => SetValue(DurationTextProperty, value);
    }

    public bool HasDuration
    {
        get => (bool)GetValue(HasDurationProperty);
        set => SetValue(HasDurationProperty, value);
    }
}

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestFlowMetaStripView : ContentView
{
    public TestFlowMetaStripView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DurationTextProperty =
        BindableProperty.Create(nameof(DurationText), typeof(string), typeof(TestFlowMetaStripView), string.Empty, propertyChanged: OnMetaChanged);

    public static readonly BindableProperty QuestionCountTextProperty =
        BindableProperty.Create(nameof(QuestionCountText), typeof(string), typeof(TestFlowMetaStripView), string.Empty, propertyChanged: OnMetaChanged);

    public static readonly BindableProperty HasDurationProperty =
        BindableProperty.Create(nameof(HasDuration), typeof(bool), typeof(TestFlowMetaStripView), false);

    public static readonly BindableProperty HasQuestionCountProperty =
        BindableProperty.Create(nameof(HasQuestionCount), typeof(bool), typeof(TestFlowMetaStripView), false);

    public static readonly BindableProperty HasMetaProperty =
        BindableProperty.Create(nameof(HasMeta), typeof(bool), typeof(TestFlowMetaStripView), false);

    public string DurationText
    {
        get => (string)GetValue(DurationTextProperty);
        set => SetValue(DurationTextProperty, value);
    }

    public string QuestionCountText
    {
        get => (string)GetValue(QuestionCountTextProperty);
        set => SetValue(QuestionCountTextProperty, value);
    }

    public bool HasDuration
    {
        get => (bool)GetValue(HasDurationProperty);
        set => SetValue(HasDurationProperty, value);
    }

    public bool HasQuestionCount
    {
        get => (bool)GetValue(HasQuestionCountProperty);
        set => SetValue(HasQuestionCountProperty, value);
    }

    public bool HasMeta
    {
        get => (bool)GetValue(HasMetaProperty);
        set => SetValue(HasMetaProperty, value);
    }

    private static void OnMetaChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TestFlowMetaStripView view)
        {
            return;
        }

        view.HasDuration = !string.IsNullOrWhiteSpace(view.DurationText);
        view.HasQuestionCount = !string.IsNullOrWhiteSpace(view.QuestionCountText);
        view.HasMeta = view.HasDuration || view.HasQuestionCount;
    }
}

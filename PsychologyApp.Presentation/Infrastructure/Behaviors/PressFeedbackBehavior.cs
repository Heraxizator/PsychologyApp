namespace PsychologyApp.Presentation.Infrastructure.Behaviors;

public sealed class PressFeedbackBehavior : Behavior<View>
{
    public static readonly BindableProperty TargetNameProperty =
        BindableProperty.Create(
            nameof(TargetName),
            typeof(string),
            typeof(PressFeedbackBehavior),
            string.Empty);

    public string TargetName
    {
        get => (string)GetValue(TargetNameProperty);
        set => SetValue(TargetNameProperty, value);
    }

    public static readonly BindableProperty AttachToTappableLabelsProperty =
        BindableProperty.Create(
            nameof(AttachToTappableLabels),
            typeof(bool),
            typeof(PressFeedbackBehavior),
            false);

    public bool AttachToTappableLabels
    {
        get => (bool)GetValue(AttachToTappableLabelsProperty);
        set => SetValue(AttachToTappableLabelsProperty, value);
    }

    private View? _attachedView;
    private readonly HashSet<VisualElement> _attachedTargets = [];

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        _attachedView = bindable;
        bindable.Loaded += OnLoaded;
        bindable.HandlerChanged += OnHandlerChanged;
        TryAttach(bindable);
    }

    protected override void OnDetachingFrom(View bindable)
    {
        bindable.Loaded -= OnLoaded;
        bindable.HandlerChanged -= OnHandlerChanged;
        _attachedView = null;
        _attachedTargets.Clear();
        base.OnDetachingFrom(bindable);
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (sender is View view)
        {
            TryAttach(view);
        }
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is View view)
        {
            TryAttach(view);
        }
    }

    private void TryAttach(View bindable)
    {
        if (AttachToTappableLabels)
        {
            foreach (Label label in bindable.GetVisualTreeDescendants().OfType<Label>())
            {
                if (label.GestureRecognizers.OfType<TapGestureRecognizer>().Any()
                    && _attachedTargets.Add(label))
                {
                    VisualElementPressFeedback.Attach(label);
                }
            }

            return;
        }

        VisualElement? target = ResolveTarget(bindable);
        if (target is not null && _attachedTargets.Add(target))
        {
            VisualElementPressFeedback.Attach(target);
        }
    }

    private VisualElement? ResolveTarget(View bindable)
    {
        if (!string.IsNullOrWhiteSpace(TargetName))
        {
            foreach (VisualElement element in bindable.GetVisualTreeDescendants().OfType<VisualElement>())
            {
                if (element.StyleId == TargetName || element.AutomationId == TargetName)
                {
                    return element;
                }
            }
        }

        return bindable;
    }
}

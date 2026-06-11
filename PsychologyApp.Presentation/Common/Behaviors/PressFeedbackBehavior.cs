namespace PsychologyApp.Presentation.Common.Behaviors;

public sealed class PressFeedbackBehavior : Behavior<View>
{
    public static readonly BindableProperty TargetNameProperty =
        BindableProperty.Create(
            nameof(TargetName),
            typeof(string),
            typeof(PressFeedbackBehavior),
            string.Empty);

    public static readonly BindableProperty AttachToTappableLabelsProperty =
        BindableProperty.Create(
            nameof(AttachToTappableLabels),
            typeof(bool),
            typeof(PressFeedbackBehavior),
            false);

    public static readonly BindableProperty AttachToAllTapTargetsProperty =
        BindableProperty.Create(
            nameof(AttachToAllTapTargets),
            typeof(bool),
            typeof(PressFeedbackBehavior),
            false);

    public static readonly BindableProperty HapticOnReleaseProperty =
        BindableProperty.Create(
            nameof(HapticOnRelease),
            typeof(bool),
            typeof(PressFeedbackBehavior),
            false);

    public string TargetName
    {
        get => (string)GetValue(TargetNameProperty);
        set => SetValue(TargetNameProperty, value);
    }

    public bool AttachToTappableLabels
    {
        get => (bool)GetValue(AttachToTappableLabelsProperty);
        set => SetValue(AttachToTappableLabelsProperty, value);
    }

    public bool AttachToAllTapTargets
    {
        get => (bool)GetValue(AttachToAllTapTargetsProperty);
        set => SetValue(AttachToAllTapTargetsProperty, value);
    }

    public bool HapticOnRelease
    {
        get => (bool)GetValue(HapticOnReleaseProperty);
        set => SetValue(HapticOnReleaseProperty, value);
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
        foreach (VisualElement target in _attachedTargets)
        {
            VisualElementPressFeedback.Detach(target);
        }

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
        if (AttachToAllTapTargets)
        {
            AttachAllTapTargets(bindable);
            return;
        }

        if (AttachToTappableLabels)
        {
            foreach (Label label in bindable.GetVisualTreeDescendants().OfType<Label>())
            {
                if (PressFeedbackTapScanner.HasCommandTap(label)
                    && _attachedTargets.Add(label))
                {
                    VisualElementPressFeedback.Attach(label, CreateOptions());
                }
            }

            return;
        }

        VisualElement? target = ResolveTarget(bindable);
        if (target is not null && _attachedTargets.Add(target))
        {
            VisualElementPressFeedback.Attach(target, CreateOptions());
        }
    }

    private void AttachAllTapTargets(View bindable)
    {
        foreach (View target in PressFeedbackTapScanner.FindTapTargets(bindable))
        {
            if (!_attachedTargets.Add(target))
            {
                continue;
            }

            VisualElementPressFeedback.Attach(target, CreateOptions());
        }
    }

    private PressFeedbackOptions CreateOptions() => new()
    {
        HapticOnRelease = HapticOnRelease
    };

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

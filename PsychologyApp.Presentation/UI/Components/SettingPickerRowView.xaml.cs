using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.UI.Converters;
using System.Collections;

namespace PsychologyApp.Presentation.UI.Components;

public partial class SettingPickerRowView : ContentView
{
    private static readonly PreferenceLabelConverter LabelConverter = new();
    private bool _isSyncingPicker;

    public SettingPickerRowView()
    {
        InitializeComponent();
        ApplyItemDisplayBinding();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        VisualElementPressFeedback.Attach(SettingLabel);
        ApplyItemDisplayBinding();
        SyncPickerFromViewModel();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        SyncPickerFromViewModel();
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(ItemsSource))
        {
            ApplyItemDisplayBinding();
            SyncPickerFromViewModel();
            return;
        }

        if (propertyName is nameof(SelectedItem))
        {
            SyncPickerFromViewModel();
        }
    }

    private void ApplyItemDisplayBinding()
    {
        if (SettingPicker is null)
        {
            return;
        }

        SettingPicker.ItemDisplayBinding = new Binding(".")
        {
            Converter = LabelConverter,
            ConverterParameter = LabelKind
        };
    }

    private void SyncPickerFromViewModel()
    {
        if (SettingPicker is null)
        {
            return;
        }

        _isSyncingPicker = true;
        try
        {
            object? selected = SelectedItem;
            IList? items = ItemsSource;

            if (items is null || items.Count == 0)
            {
                SettingPicker.SelectedIndex = -1;
                return;
            }

            if (selected is null)
            {
                SettingPicker.SelectedIndex = -1;
                return;
            }

            string selectedKey = NormalizeKey(selected);

            for (int i = 0; i < items.Count; i++)
            {
                if (KeysMatch(items[i], selectedKey))
                {
                    SettingPicker.SelectedIndex = i;
                    return;
                }
            }

            SettingPicker.SelectedIndex = -1;
        }
        finally
        {
            _isSyncingPicker = false;
        }
    }

    private void OnPickerSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isSyncingPicker || SettingPicker.SelectedIndex < 0)
        {
            return;
        }

        object? selected = SettingPicker.SelectedItem;
        if (KeysMatch(SelectedItem, selected))
        {
            return;
        }

        SelectedItem = selected;
    }

    private bool KeysMatch(object? left, object? right) =>
        string.Equals(NormalizeKey(left), NormalizeKey(right), StringComparison.Ordinal);

    private string NormalizeKey(object? value) =>
        LabelKind switch
        {
            PreferenceLabelKind.Language => UserPreferences.ParseLanguageKey(value?.ToString() ?? string.Empty),
            PreferenceLabelKind.Theme => UserPreferences.ParseThemeKey(value?.ToString() ?? string.Empty),
            PreferenceLabelKind.Color => UserPreferences.ParseColorKey(value?.ToString() ?? string.Empty),
            PreferenceLabelKind.Form => UserPreferences.ParseFormKey(value?.ToString() ?? string.Empty),
            PreferenceLabelKind.Size => UserPreferences.ParseSizeKey(value?.ToString() ?? string.Empty),
            _ => value?.ToString() ?? string.Empty
        };

    public static readonly BindableProperty LabelTextProperty =
        BindableProperty.Create(nameof(LabelText), typeof(string), typeof(SettingPickerRowView), string.Empty);

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly BindableProperty PickerTitleProperty =
        BindableProperty.Create(nameof(PickerTitle), typeof(string), typeof(SettingPickerRowView), string.Empty);

    public string PickerTitle
    {
        get => (string)GetValue(PickerTitleProperty);
        set => SetValue(PickerTitleProperty, value);
    }

    public static readonly BindableProperty LabelKindProperty =
        BindableProperty.Create(
            nameof(LabelKind),
            typeof(PreferenceLabelKind),
            typeof(SettingPickerRowView),
            PreferenceLabelKind.Language,
            propertyChanged: static (bindable, _, _) => ((SettingPickerRowView)bindable).ApplyItemDisplayBinding());

    public PreferenceLabelKind LabelKind
    {
        get => (PreferenceLabelKind)GetValue(LabelKindProperty);
        set => SetValue(LabelKindProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(SettingPickerRowView), null);

    public IList? ItemsSource
    {
        get => (IList?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SettingPickerRowView), null, BindingMode.TwoWay);

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
}

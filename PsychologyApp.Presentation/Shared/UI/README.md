# Shared UI layer

Design tokens, reusable components, and layouts used by `Pages/` and `Widgets/`.

## Structure

```
Resources/Styles/
  Colors.xaml       — palette (Primary, Secondary, Surface*, InputBackground*)
  Typography.xaml   — TextPrimaryLight/Dark, SectionTitleStyle, BodyStyle, Nav*, UiCornerRadius
  Components.xaml   — CardFrameStyle, PrimaryAction* (uses DynamicResource UiCornerRadiusShape)
Shared/UI/Components/   — generic ContentView building blocks
Shared/ViewModels/      — BaseViewModel hierarchy (not XAML)
Widgets/                — feature-specific composite controls
```

Global XAML alias (add on pages as needed):

```xml
xmlns:ui="clr-namespace:PsychologyApp.Presentation.Shared.UI.Components"
```

`App.xaml` merges `Typography.xaml` and `Components.xaml` after `Colors.xaml`.

## Components

| Component | Use when |
|-----------|----------|
| `NavigationBarSimpleView` | Back arrow + centered title; bind `BackCommand` |
| `NavigationBarExtendedView` | Simple nav + one trailing text action (`ExtensionText` / `ExtensionCommand`) |
| `AlgorithmBoxView` | Card with algorithm steps: `BodySource` (strings) or `ItemsSource` + `ItemTemplate` |
| `TextBoxView` | Titled info block (header + body) |
| `TextEntryView` / `TextEditorView` | Single field with label |
| `EntryBoxView` | List of labeled entries from `BodySource` |
| `ButtonView` | Action button; `TapCommand`. Use `Variant="Primary"` and `IsCompact="True"` for compact header actions |
| `TextItemView` | List row with title + body (tests, simple lists) |
| `SettingsLinkCardView` | Tappable settings menu card: `Title`, `Subtitle`, `TapCommand` |
| `SettingPickerRowView` | Settings row with label + picker: `LabelText`, `PickerTitle`, `ItemsSource`, `SelectedItem` |
| `SettingSwitchRowView` | Settings row with label + switch: `LabelText`, `IsToggled` |
| `ProgressBarView` | Loading + optional `CancelCommand` |
| `RetryView` | Error overlay: `FailedText`, `RetryText`, `RetryCommand` |

Feature-specific cards live under `Widgets/` (`QuoteCardView`, `TechniqueListCardView`, `TestListCardView`, …).

## Rules for new UI

1. **Before writing inline XAML**, search `Shared/UI/Components/` and `Widgets/`.
2. **Add a generic component** only when the same pattern repeats ≥3 times; otherwise use a widget slice.
3. **Use typography tokens** (`SectionTitleStyle`, `BodyStyle`) and `AppThemeBinding` for text.
4. **Bind commands** on components — not page-level gesture wrappers.
5. **Press feedback:** `ButtonView` and widget cards attach `VisualElementPressFeedback`; pages use `PressFeedbackHost.AttachToPage` via `PageRegistry`.
6. **Run `dotnet build`** after adding or migrating a component.

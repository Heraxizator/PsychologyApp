# UI layer (Presentation)

Design tokens, reusable components, and layouts used by module pages.

## Structure

```
Resources/Styles/
  Colors.xaml       — palette (Primary, Secondary, Surface*, InputBackground*)
  Typography.xaml   — TextPrimaryLight/Dark, SectionTitleStyle, BodyStyle, Nav*, UiCornerRadius
  Components.xaml   — CardFrameStyle, PrimaryAction* (uses DynamicResource UiCornerRadiusShape)
UI/Components/      — ContentView + ControlTemplate building blocks
Controls/           — TechniquePageShell and other layouts
Ui/Techniques/Bodies/ — technique-specific form bodies
```

Global XAML alias (add on pages as needed):

```xml
xmlns:ui="clr-namespace:PsychologyApp.Presentation.UI.Components"
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
| `TechniqueListCardView` | Technique list item card with number, date, image, theme, author |
| `QuoteCardView` | Quote card with like/copy/share actions |
| `ListEntryCardView` | Simple two-line card: `Title`, `Body` (technique body lists) |
| `ProgressBarView` | Loading + optional `CancelCommand` |
| `RetryView` | Error overlay: `FailedText`, `RetryText`, `RetryCommand` |

## Rules for new UI

1. **Before writing inline XAML**, search `UI/Components/` for an existing block.
2. **Add a new component** only when the same pattern repeats ≥3 times.
3. **Use typography tokens** (`SectionTitleStyle`, `BodyStyle`) and `AppThemeBinding` for text (`TextPrimaryLight` / `TextPrimaryDark`) — avoid hardcoded `black`/`white`.
4. **Bind commands** on components (`TapCommand`, `RetryCommand`) — not page-level gesture wrappers.
5. **Run `dotnet build`** after adding or migrating a component.

## Adoption matrix

| Page / layout | Components used |
|---------------|-----------------|
| `TechniquesPage` | `ButtonView` (compact primary), `TechniqueListCardView` |
| `OptionsPage` | `NavigationBarSimpleView`, `SettingsLinkCardView` ×4 |
| `SettingsPage` | `NavigationBarSimpleView`, `SettingPickerRowView`, `SettingSwitchRowView` |
| `QuotePage` | `QuoteCardView`, `ProgressBarView`, `RetryView` |
| `UserPage` | `NavigationBarSimpleView`, `TextItemView`, `ProgressBarView`, `RetryView` |
| `PhysicsSearchPage` | `NavigationBarSimpleView`, `TextEntryView`, `ProgressBarView`, `RetryView` |
| `StartPhysicsPage` | `ProgressBarView`, `TextBoxView`, `AlgorithmBoxView`, `ButtonView`, `RetryView` |
| `MusicPlayerPage` | `ProgressBarView`, `TextBoxView`, `RetryView` |
| `TestsListPage` | `TextItemView` |
| `TechniquePageShell` | `NavigationBarExtendedView`, `AlgorithmBoxView`, `ButtonView` |
| `PaperFormBody` / `CopiedFormBody` | `TextEntryView`, `ButtonView`, `ListEntryCardView` |
| Constructor, Physics, Tests, Donate, Info, Theory | `NavigationBarSimpleView`, `TextBoxView`, `ButtonView`, etc. |

## Conventions

- Use `{StaticResource Secondary}` for accent fills — do not declare local `Accent` colors in components.
- Prefer `SectionTitleStyle` / `BodyStyle` over inline `FontSize` / `#DE000000`.
- Module pages should not duplicate nav bars, settings rows, or list cards; compose `ui:` components instead.

## Example

```xml
<ui:NavigationBarSimpleView TitleText="Опросник" BackCommand="{Binding Finish}" />
<ui:SettingsLinkCardView Title="Настройки" Subtitle="Тема и цвет" TapCommand="{Binding OpenSettings}" />
<ui:ButtonView Variant="Primary" IsCompact="True" BodyText="СОЗДАТЬ" TapCommand="{Binding Create}" />
<ui:RetryView IsVisible="{Binding IsFail}" FailedText="Ошибка" RetryText="Повторить?" RetryCommand="{Binding Reload}" />
```

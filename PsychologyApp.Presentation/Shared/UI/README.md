# Shared UI layer

Design tokens, reusable components, and layouts used by `Pages/` and `Widgets/`.

## Structure

```
Resources/Styles/
  Colors.xaml       — palette (Primary, Secondary, Surface*, PageBackground, BrandCardShadowLight/Dark)
  Typography.xaml   — TextPrimaryLight/Dark, SectionTitleStyle, BodyStyle, CaptionStyle, Nav*, UiCornerRadius
  Components.xaml   — CardFrameStyle, BrandCardStyle (theme-aware shadows), PrimaryAction* (min height 48)
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
| `ButtonView` | Action button; `TapCommand`. Use `Variant="Primary"` (default) or `Variant="Secondary"`; `IsCompact="True"` for compact header actions. Attaches `PressFeedbackBehavior`. |
| `EmptyStateView` | Centered empty list/content: `TitleText`, `BodyText`, optional `IconName`, optional `ActionText` + `ActionCommand` |
| `SectionHeaderView` | Section title + optional `SubtitleText` + optional compact `ActionText` / `ActionCommand` |
| `ListEntryCardView` | Tappable list card: title (`ListRowTitleStyle`) + body (`CaptionStyle`) |
| `TextItemView` | List row with title + body (tests, simple lists) |
| `SettingsLinkCardView` | Tappable settings menu card: `Title`, `Subtitle`, `TapCommand` |
| `SettingPickerRowView` | Settings row with label + picker: `LabelText`, `PickerTitle`, `ItemsSource`, `SelectedItem` |
| `SettingSwitchRowView` | Settings row with label + switch: `LabelText`, `IsToggled` |
| `ProgressBarView` | Loading + optional `CancelCommand` |
| `RetryView` | Error overlay: `FailedText`, `RetryText`, `RetryCommand` (icon + press feedback) |

Feature-specific cards live under `Widgets/` (`QuoteCardView`, `TechniqueListCardView`, `TestListCardView`, …).

Test flow widgets (`Widgets/Test/`):

| Widget | Use when |
|--------|----------|
| `TestAnswerOptionView` | Tappable questionnaire answer row (single/multi) |
| `TestProgressHeaderView` | Step dots + label (`Step`, `StepCount`, `StepLabel`) |
| `TestResultHeroView` | Result screen score headline + interpretation + trend badge |
| `TestResultMetricCardView` | Named metric card (Lüscher standard results) |
| `LuscherColorResultView` | Brief Lüscher color swatch + interpretation block |
| `TestFlowMetaStripView` | Duration / question-count chips on test intro |

Onboarding widgets (`Widgets/Onboarding/`):

| Widget | Use when |
|--------|----------|
| `OnboardingStepIndicatorView` | Progress dots + optional `StepLabel` (`ShowStepLabel`) |
| `OnboardingWelcomeHeroView` | Welcome step branding (logo halo, name, tagline, copy) |
| `OnboardingValueStripView` | Welcome trust pills (offline, no judgment, on device) |
| `OnboardingOverviewBannerView` | Overview step tab-icon strip + lead line |
| `OnboardingModuleRowView` | App section overview row (accent bar + icon + title + hint) |
| `OnboardingConcernOptionView` | Tappable concern picker row (icon + title + empathy subtitle) |
| `OnboardingFinishHeaderView` | Finish step celebration check circle |
| `OnboardingRecommendationPreviewView` | Finish step practice preview (read-only) |

## Rules for new UI

1. **Before writing inline XAML**, search `Shared/UI/Components/` and `Widgets/`.
2. **Add a generic component** only when the same pattern repeats ≥3 times; otherwise use a widget slice.
3. **Use typography tokens** (`SectionTitleStyle`, `BodyStyle`) and `AppThemeBinding` for text.
4. **Bind commands** on components — not page-level gesture wrappers.
5. **Press feedback:** `ButtonView`, `SettingsLinkCardView`, `RetryView`, and widget cards attach `PressFeedbackBehavior` / `VisualElementPressFeedback`; pages use `PressFeedbackHost.AttachToPage` via `PageRegistry`.
6. **Card shadows:** use `BrandCardShadowLight` / `BrandCardShadowDark` via `AppThemeBinding` in styles — not a single hard-coded shadow color.
7. **Run `dotnet build`** after adding or migrating a component.

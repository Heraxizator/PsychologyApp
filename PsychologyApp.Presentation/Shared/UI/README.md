# Shared UI layer

Design tokens, reusable components, and layouts used by `Pages/` and `Widgets/`.

## Structure

```
Resources/Styles/
  Colors.xaml       — palette (Primary, Secondary, Surface*, PageBackground, BrandCardShadowLight/Dark)
  Typography.xaml   — TextPrimaryLight/Dark, SectionTitleStyle, BodyStyle, CaptionStyle, Nav*, UiCornerRadius
  Components.xaml   — CardFrameStyle, BrandCardStyle, EmptyStateIconHaloStyle, PrimaryAction* (min height 48)
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
| `NavigationBarExtendedView` | Simple nav + one trailing text action; min height 48, bottom divider, press feedback |
| `AlgorithmBoxView` | Card with algorithm steps: `BodySource` (strings) or `ItemsSource` + `ItemTemplate` |
| `TextBoxView` | Titled info block (header + body) |
| `TextEntryView` / `TextEditorView` | Single field with label |
| `EntryBoxView` | List of labeled entries from `BodySource` |
| `ButtonView` | Action button; `TapCommand`. Use `Variant="Primary"` (default) or `Variant="Secondary"`; `IsCompact="True"` for compact header actions. Attaches `PressFeedbackBehavior`. |
| `EmptyStateView` | Centered empty list/content: `TitleText`, `BodyText`, optional `IconName` (halo), optional `ActionText` + `ActionCommand`. Auto `EmptyStateRevealBehavior` on appear. |
| `CompletionCelebrationView` | Full-screen celebration moment: icon halo, title/body, optional streak (`StreakValueText`, `HasStreak`), primary + secondary actions. Used by `PracticeCompletionPage`. |
| `MetricTileView` | Profile-style stat: `ValueText` + `LabelText`, optional `TapCommand` |
| `SectionHeaderView` | Section title + optional `SubtitleText` + optional compact `ActionText` / `ActionCommand` |
| `ListEntryCardView` | Tappable list card: title (`ListRowTitleStyle`) + body (`CaptionStyle`) |
| `TextItemView` | List row with title + body (tests, simple lists) |
| `SettingsLinkCardView` | Tappable settings menu card: `Title`, `Subtitle`, `TapCommand` |
| `SettingPickerRowView` | Settings row with label + picker. Set `LabelKind` (`Language`, `Theme`, … or `PassThrough` for raw labels like reminder hours). Default: `PassThrough`. |
| `SettingSwitchRowView` | Settings row with label + switch: `LabelText`, `IsToggled` (`BodyStyle`, `OnColor=Primary`) |
| `ProgressBarView` | Loading + optional `CancelCommand` (cancel row hidden when command is null) |
| `RetryView` | Error overlay: `FailedText`, `RetryText`, `RetryCommand` (icon + press feedback) |

Feature-specific cards live under `Widgets/` (`QuoteCardView`, `TechniqueListCardView`, `TestListCardView`, …). All list-card widgets attach `PressFeedbackBehavior AttachToAllTapTargets="True"`. High-touch widgets: `MoodStripView` (mood chips + `MoodChipSelectedStyle`), `TodayPracticeRowView`, `PhysicsReasonCardView`.

### Empty state icons (by screen)

| Screen | `IconName` |
|--------|------------|
| Techniques | `SelfImprovement` |
| Tests list | `Assignment` |
| Quotes (empty / all read) | `FormatQuote` / `DoneAll` |
| Music search | `SearchOff` |
| Profile favorites | `FavoriteBorder` |
| Test history | `History` |

Test flow widgets (`Widgets/Test/`):

| Widget | Use when |
|--------|----------|
| `TestAnswerOptionView` | Tappable questionnaire answer row (single/multi) |
| `TestProgressHeaderView` | Step dots + label (`Step`, `StepCount`, `StepLabel`) |
| `TestResultHeroView` | Result screen: check icon halo, score headline, interpretation, trend badge (`TrendKind` colors) |
| `TestScoreTrendChartView` | Score-over-time line chart: `ChartPoints`, `Title`. On `TestResultPage` when 2+ results; also `TestHistoryPage`. |
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
5. **Press feedback:** `ButtonView`, `SettingsLinkCardView`, `RetryView`, `FilterChipView`, `MetricTileView`, list-card widgets, `MoodStripView`, `TodayPracticeRowView`, and `PhysicsReasonCardView` attach `PressFeedbackBehavior`; pages use `PressFeedbackHost.AttachToPage` via `PageRegistry`.
6. **Settings pickers:** always set `LabelKind` on `SettingPickerRowView`; use `PassThrough` when `ItemsSource` already contains display strings (e.g. `07:00`…`22:00`).
7. **Empty states:** prefer `EmptyStateView` with `IconName`; reveal animation is built in via `EmptyStateRevealBehavior`.
8. **Card shadows:** use `BrandCardShadowLight` / `BrandCardShadowDark` via `AppThemeBinding` in styles — not a single hard-coded shadow color.
9. **Run `dotnet build`** after adding or migrating a component.

## Emotional moments

| Flow | Screen / widget |
|------|-----------------|
| Practice completed | `PracticeCompletionPage` → `CompletionCelebrationView` (replaces system alert). Primary: back to techniques list; secondary: home tab. |
| Test result | `TestResultPage` → `TestResultHeroView` + `TestScoreTrendChartView` when history has 2+ scored results. |

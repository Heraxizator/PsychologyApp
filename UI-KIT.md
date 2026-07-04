# UI Kit

Design tokens and reusable components live under `PsychologyApp.Presentation/Resources/Styles/` and `PsychologyApp.Presentation/Shared/UI/Components/`.

## Card styles

| Style | Use when |
|-------|----------|
| `BrandCardStyle` | Settings sections, forms, profile blocks |
| `ListCardItemStyle` | Default list rows (music, quotes, techniques) |
| `HeroCardStyle` | Featured content: daily quote, today practice |
| `TestFlatCardStyle` | Flat test options without list shadow |

## Typography

| Style | Use when |
|-------|----------|
| `PageTitleStyle` / `SectionTitleStyle` | Screen and section headers |
| `BodyStyle` / `CaptionStyle` | Default copy |
| `HeroQuoteStyle` / `HeroCaptionStyle` | Daily quote hero and featured captions |
| `QuoteDisplayStyle` | Quote cards in feeds |

Font sizes and quote hero sizes scale via `UserPreferences.ApplyTypography`. Accent colors via `UserPreferences.ApplyAccentColor`.

## Layout tokens

Use `Typography.xaml` spacing tokens instead of inline margins:

- `CardBottomMargin` — bottom spacing between cards
- `FieldTopMargin` — gap above form fields
- `SectionTopMargin` — gap before stacked sections
- `SubtleDividerStyle` — horizontal rules (profile, settings)

## Behaviors checklist

- Tappable control or card → `PressFeedbackBehavior`
- List item appearing in a feed → `ListItemRevealBehavior`
- Hero block on first appear → `RevealOnAppearingBehavior`
- Empty state block → `EmptyStateRevealBehavior` (via `EmptyStateView`)

## Components

Prefer kit components over raw MAUI controls:

- Actions: `ButtonView` (`Primary` / `Secondary`, `IsCompact`)
- Inputs: `TextEntryView`, `SettingPickerRowView`, `SettingSwitchRowView`
- Chrome: `NavigationBarSimpleView`, `SectionHeaderView`, `EmptyStateView`
- Filters: `FilterChipView`, `FilterChipTabBarView`

Set `AccessibilityLabel` / `AccessibilityHint` on `ButtonView` when button text alone is ambiguous.

## Token catalog

Required keys are listed in `PsychologyApp.Presentation.Core/Common/UiTokenCatalog.cs` and covered by `UiTokensTests`.

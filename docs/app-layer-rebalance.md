# App-wide layer rebalance

Boundary map after Phase 15 rebalance (practice, recommendations, tests finish, content providers).

## Practice

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `EntryFieldKind`, `OnboardingConcernKeys`, `TechniqueId`, `TechniqueUiKind` |
| **Application** | `BuiltInTechniqueDefinition`, catalog content (`Practice/Catalog`), `ITechniqueCatalogService`, `ITechniqueRecommendationService`, `SomaticTechniqueRecommendation` (keyword map) |
| **Presentation** | `TechniqueCatalogGateway`, `TechniqueDefinitionMapper`, `TechniqueEntryMapper`, session ViewModels, MAUI widgets (`EntryItem`) |

Language-scoped catalog cache invalidates via `LanguageContentReloader` alongside quot/reason/test catalogs.

## Recommendations

| Layer | Responsibility |
|-------|----------------|
| **Application** | `TechniqueRecommendationService` — onboarding concern → technique, somatic query → techniques |
| **Presentation** | Consumers inject `ITechniqueRecommendationService` / `TodayRecommendationResolver`; no `OnboardingRecommendation` static class |

## Tests (finish)

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `LuscherScoring` (formulas only), `TestTrendEvaluator`, `QuestionnaireScoreCalculator` |
| **Application** | `TestSessionInfo`, `ILuscherResultService`, `TestScoreRecommendation`, catalog/scoring/trend services |
| **Presentation** | `LuscherStrings` + `LuscherInterpretationsRu` (prose), `TestRunCoordinator`, loaders, navigation |

## Content providers

| Layer | Responsibility |
|-------|----------------|
| **Application** | `ReasonTsvParser`, `QuoteJsonEntry`, `QuotSeed` mapping inputs |
| **Presentation** | `MauiReasonContentProvider`, `MauiQuotContentProvider`, `MauiTestCatalogProvider` — asset I/O only |

## User progress

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `StreakCalculator` |
| **Application** | `UserProgressService` delegates streak to domain calculator |

## Stays in Presentation

Coordinators, loaders, `AppStrings`, XAML, MAUI platform channels, `MusicPlaybackPresenter`, `PrayerCatalog`.

## Phase 16 (closure)

Rebalance from Phase 15 is complete. Remaining optional work is deferred:

| Deferred | Notes |
|----------|-------|
| JSON technique catalog | Move built-in catalog to `Resources/Raw/techniques.{ru,en}.json` |
| `ReasonSearchRanker` | Pure ranking logic could move to Domain |

### Practice reminders (Android)

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `PracticeReminderPolicy` — when to schedule next fire (enabled, onboarding, practiced today, hour) |
| **Application** | `IUserProgressService.GetLastTechniqueCompletionDateAsync` (existing) |
| **Presentation** | `PracticeReminderCoordinator`, `AndroidPracticeReminderScheduler`, prefs in `UserPreferencesState`, Settings UI |

Android-only local notifications: once per day at user-chosen hour, only if no technique completion today. Tap opens recommended technique via `SetPendingTechnique` / `OpenPendingTechniqueIfNeeded`.

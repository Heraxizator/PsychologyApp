# Architecture

Psychology App uses a **modular monolith**: Clean Architecture layers at the solution level, Feature-Sliced Design (FSD) inside Presentation.

## Solution layers

| Project | Role |
|---------|------|
| `PsychologyApp.Domain` | Entities, value objects, domain rules |
| `PsychologyApp.Application` | Use cases, DTOs, ports (`Abstractions/`) |
| `PsychologyApp.Infrastructure` | SQLite, Dapper, repository implementations |
| `PsychologyApp.Bootstrap` | Composition root: `AddPsychologyAppCore()` |
| `PsychologyApp.Presentation.Core` | Shared strings, chart layout math (`Charts/`), small models without MAUI |
| `PsychologyApp.Presentation` | MAUI UI (FSD slices) |

Dependency flow:

```
Presentation → Application → Domain
Presentation → Bootstrap → Application + Infrastructure
Infrastructure → Application → Domain
```

ViewModels and Features must not reference `PsychologyApp.Infrastructure`.

### Presentation.Core folders

| Folder | Contents |
|--------|----------|
| `Common/` | `AppStrings`, navigation helpers, cross-cutting presentation utilities |
| `Charts/` | MAUI-free chart layout math (`TrendLineChartLayout`, `TrendChartPoint`) — rendering lives in `Presentation/Shared/UI/Drawing/` |
| `Navigation/` | Shared navigation coordination |
| `Models/` | Small presentation models without UI framework |

Presentation copy for charts (sparse subtitles via `AppStrings.ResolveChartSubtitle`) lives in `Presentation.Core/Common/AppStrings.cs`; layout math stays in `Presentation.Core/Charts/`.

## Presentation (FSD)

```
PsychologyApp.Presentation/
├── App/              Routes, DI composition, Shell, MauiProgram
├── Features/         Business logic: loaders, coordinators, factories
├── Pages/            Screens: Page + ViewModel (+ XAML), grouped by feature slice
├── Widgets/          Composite reusable UI
├── Entities/         Presentation models and item factories
└── Shared/           UI kit, navigation ports, cross-cutting infra only
```

### Page folder layout

All screens live under `Pages/{Slice}/` — one of the eight canonical slices listed below. Legacy flat folders (e.g. `Pages/Question/`, `Pages/ReviewForm/`) have been removed; do not add new top-level folders under `Pages/`. Session sub-ViewModels live under `Pages/{Slice}/{Screen}/SubViewModels/`.

### GlobalUsings policy

`GlobalUsings.cs` must not import feature slice namespaces or `PsychologyApp.Presentation.App`. Cross-slice dependencies must use explicit `using` lines and, where applicable, import only `Features/{Slice}/Index` for documented cross-slice types.

### Feature slices

| Slice | Pages | Application | Domain |
|-------|-------|-------------|--------|
| **RunTests** | TestsList, TestHistory, FindProblem, Question, StandardTest, AlternativeTest, LuscherTest, TestResult | `Tests/*`, `Models/Tests/` | `Tests/`, `Colour/` |
| **RunTechniqueSession** | Techniques, TechniqueSession, TechniqueCreated, TechniqueDesigner, TechniqueTheory, PracticeCompletion | `Practice/`, `Technique/`, `Recommendations/` | `Practice/`, `Technique/` |
| **ManageProfile** | ProfileUser, ProfileOptions, ProfileSettings, ProfileInfo, ProfileDonate | `UserProgress/`, `Statistic/` | `UserProgress/`, `Statistic/` |
| **ManageQuotes** | QuoteFeed | `Quot/` | `Quot/` |
| **SearchPhysics** | StartPhysics, PhysicsSearch | `Reason/` | `Reason/` |
| **Onboarding** | Onboarding | `Recommendations/` | `Practice/OnboardingConcernKeys` |
| **PlayMusic** | MusicPlayer | — | — |
| **SendReviewForm** | ReviewForm | — | — |

Each slice under `Features/{Slice}/` exposes `Index/{Slice}PublicApi.cs` as the slice entry marker.

### Dependency rules (enforced by tests)

1. **Features** must not import other feature slices (`Features.*` cross-imports forbidden).
2. **Shared** must not import `Pages`, `Widgets`, `Features`, `Entities`, or `App` (except `Shared.Lib` navigation ports).
3. **Pages** must live under `Pages/{Slice}/` and must not import other feature slices.
4. ViewModels use `INavigationService`; no direct `PopAsync` in page code-behind.

### Allowed cross-slice imports

Some screens intentionally compose multiple slices:

| Source slice | May import |
|--------------|------------|
| ManageProfile | ManageQuotes.Index (profile quotes), RunTechniqueSession.Index (featured techniques) |
| RunTests | RunTechniqueSession.Index (test result technique recommendations) |
| SearchPhysics | RunTechniqueSession.Index (technique suggestions in search results) |

All other cross-feature imports are forbidden and enforced by architecture tests.

### What belongs in Shared

- `Shared/UI/Components/` — generic UI building blocks
- `Shared/Common/` — infrastructure helpers (AsyncCommand, exception handler, preferences)
- `Shared/Common/Localization/` — `LanguageContentReloader` (cross-feature language switch)
- `Shared/Services/` — toasts, dialogs, notifications
- `Shared/Platform/` — MAUI content providers (Reason, Quot, Test catalog)
- `Shared/Navigation/` — navigation coordinator, page activator
- `Shared/Lib/` — shared ports usable from Shared layer (e.g. `IShellTabNavigator`, `INavigateToTheory`)

Feature-specific logic (loaders, mappers, presenters) belongs in `Features/{Slice}/`, not Shared.

## Backend feature folders

Application and Domain group code by feature area:

```
Application/
├── Abstractions/     Persistence, Integration, Analytics, Startup ports
├── Tests/
├── Practice/
├── Technique/
├── Quot/
├── Reason/
├── Statistic/
├── UserProgress/
├── Recommendations/
└── Models/           Read models (Tests/, Practice/, Quot/)
```

Infrastructure keeps a single SQLite database and shared repository base classes.

## Adding a new feature

1. **Domain** — entities/rules in `PsychologyApp.Domain/{Area}/`
2. **Application** — service + port interfaces in `PsychologyApp.Application/{Area}/`
3. **Infrastructure** — repository if persisted
4. **Presentation**
   - `Features/{Slice}/Index/{Slice}PublicApi.cs`
   - `Features/{Slice}/DependencyInjection/{Slice}FeatureServiceCollectionExtensions.cs`
   - `Pages/{Slice}/` — Page + ViewModel
   - Register in `PresentationServiceCollectionExtensions.AddPsychologyAppPresentation()`
5. **Tests** — unit tests per layer; architecture tests must pass

## Composition root

Startup order in `MauiProgram.cs`:

1. `AddPsychologyAppCore()` — Infrastructure + Application
2. Platform content providers (Reason, Quot, Tests, Practice catalog)
3. `AddPsychologyAppPresentation()` — Shared + feature slices

## Presentation lifecycle

List/detail screens follow a shared async lifecycle:

1. **Lazy init** — constructors wire commands only; pages call `EnsureInitializedAsync()` from `OnAppearing` (not `InitAsync().FireAndForget()` in the ctor).
2. **Gate + timeout** — init is serialized with `SemaphoreSlim` and cancelled via `OperationCancellation` CTS from `appsettings.json` timeouts.
3. **Overlay cancel** — bind `ProgressBarView.CancelCommand="{Binding Cancel}"` (not ad-hoc `TapGestureRecognizer`). Cancel must abort the CTS / bump a load generation, not only dismiss the overlay.
4. **Lightweight reappear** — tabs that are already initialized refresh only dashboard/header state on reappear (e.g. Practice streak/mood/today), not a full catalog reload.
5. **Collections** — prefer in-place `ReplaceRange` / mutate existing groups over assigning new `ObservableCollection` instances on every refresh.
6. **Search** — debounce with CTS; expose an in-flight filtering flag for inline progress; surface failures with toast/`AppStrings`, not silent empty lists.
7. **CollectionView sizing** — use `ItemSizingStrategy="MeasureFirstItem"` only for uniform rows (e.g. truncated technique cards). Variable-height lists (Quotes, Physics expandable cards, Tests with optional meta/result rows) must keep the default / `MeasureAllItems`.
8. **CollectionView.Footer** — when idle, collapse height (`HeightRequest=0`), do not rely on child `IsVisible=false` alone; MAUI often reserves footer space.
9. **List reveals** — `ListItemRevealBehavior` may run inside `CollectionView` for the first `LiteRevealMaxIndex` items only; deeper rows skip animation to protect scroll performance.

## UI components

See [PsychologyApp.Presentation/Shared/UI/README.md](PsychologyApp.Presentation/Shared/UI/README.md).

### EmptyStateView contract

- Action pill (`EmptyStateActionPillStyle`) is visible only when both `ActionText` and `ActionCommand` are set (`HasAction`).
- Icon halo is visible only when `IconName` resolves via `MaterialIconNames.TryResolve` (`HasIcon`); invalid strings must not reserve a tinted square.
- Reveal animation lives on the control (`EmptyStateRevealBehavior`); pages must not attach a second copy.
- Canonical empty-state icon strings live in `Shared/UI/Components/MaterialIconNames.cs`.

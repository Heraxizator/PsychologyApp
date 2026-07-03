# Architecture

Psychology App uses a **modular monolith**: Clean Architecture layers at the solution level, Feature-Sliced Design (FSD) inside Presentation.

## Solution layers

| Project | Role |
|---------|------|
| `PsychologyApp.Domain` | Entities, value objects, domain rules |
| `PsychologyApp.Application` | Use cases, DTOs, ports (`Abstractions/`) |
| `PsychologyApp.Infrastructure` | SQLite, Dapper, repository implementations |
| `PsychologyApp.Bootstrap` | Composition root: `AddPsychologyAppCore()` |
| `PsychologyApp.Presentation.Core` | Shared strings and small models without MAUI |
| `PsychologyApp.Presentation` | MAUI UI (FSD slices) |

Dependency flow:

```
Presentation → Application → Domain
Presentation → Bootstrap → Application + Infrastructure
Infrastructure → Application → Domain
```

ViewModels and Features must not reference `PsychologyApp.Infrastructure`.

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

## UI components

See [PsychologyApp.Presentation/Shared/UI/README.md](PsychologyApp.Presentation/Shared/UI/README.md).

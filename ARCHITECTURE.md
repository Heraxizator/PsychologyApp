# PsychologyApp Architecture



## Layers



| Layer | Project | Responsibility |

|-------|---------|----------------|

| Domain | `PsychologyApp.Domain` | Entities, value objects, domain constants |

| Application | `PsychologyApp.Application` | Use cases, DTOs, ports (interfaces), DI registration |

| Infrastructure | `PsychologyApp.Infrastructure` | SQLite/Dapper, HTTP clients, port implementations |

| Bootstrap | `PsychologyApp.Bootstrap` | `AddPsychologyAppCore()` — wires Application + Infrastructure |

| Presentation | `PsychologyApp.Presentation` | MAUI UI, platform adapters, ViewModels |

| Tests | `PsychologyApp.*.Tests` | Unit tests per layer |



## Dependency rules



```

Presentation → Application, Bootstrap

Bootstrap → Application, Infrastructure

Infrastructure → Application, Domain

Application → Domain

```



ViewModels must not reference Infrastructure types. `MauiProgram` registers `AddPsychologyAppCore()`, `AddPsychologyAppPresentation()`, and platform services (`IReasonContentProvider`).



## Data access



- SQLite file: `%LocalApplicationData%/mentalfire3.db`

- `SchemaVersion` table + incremental migrations in `SqliteSchema` (v2: index on `Statistics.PageName`)

- Repositories implement `PsychologyApp.Application.Abstractions.Persistence.*`

- Dapper with parameterized SQL (`EntitySqlMaps`)

- WAL + `synchronous=NORMAL` + `busy_timeout` PRAGMAs



## DI and UI composition



- `MauiProgram` — MAUI host, fonts, handlers, logging; sets `MauiServiceProvider.Current` once after `Build()` for pre-handler startup only

- `MauiServiceProvider` — static resolver **only** for `AppShell` and `GlobalExceptionHandler` (MAUI startup before `MauiContext`); all other code uses constructor injection

- `IPageFactory` / `MauiPageFactory` — single entry point for constructing pages with constructor-injected dependencies

- `IPageViewModelActivator` + `ActivateViewModel` — Shell and module pages bind VMs via factories

- `Services/Factories/*` — ViewModel factories split by feature (Profile, Techniques, Physics, Motivator, Tests, technique session)
- `IFormViewModelFactory` — new `FormViewModel` per open (fixes stale `MessageText` from singleton capture)
- `IDonateViewModelFactory` — `DonateViewModel` with `INavigationService` + `AsyncCommand`

- `INavigationService` / `MauiNavigationService` — centralized imperative navigation; ViewModels do not use `IPageFactory`
- `IProfilePageFactory`, `ITestPageFactory`, `ITechniquePageFactory` — feature page factories; `MauiPageFactory` is a thin facade for Shell and navigation

- `ITechniqueMessenger` / `TechniqueMessengerService` — technique list updates (constructor CRUD)

### Factory layers (intentional)

Multiple factory types are deliberate, not accidental duplication:

| Layer | Examples | Why separate |
|-------|----------|--------------|
| Page factories | `IPageFactory`, `IProfilePageFactory`, `ITestPageFactory` | Pages need different ctor dependencies; Shell uses one facade |
| ViewModel factories | `IUserViewModelFactory`, `ITechniqueViewModelFactory`, … | Fresh VM per navigation scope; avoids capturing stale `INavigation` in singletons |
| Session factory | `ITechniqueViewModelFactory` | Selects `PaperListViewModel` / `PolarityViewModel` / `TechniqueSessionViewModel` by `TechniqueId` |

Collapsing into a single mega-factory would couple unrelated features. Current split matches feature boundaries and is covered by Presentation tests.

### Presentation layout

| Folder | Namespaces |
|--------|------------|
| `Modules/Practice` | `Views.*`, `ViewModels.Practice.Constructor`, `Technique.Main`, technique session UI |
| `Modules/Tests` | `Views.Tests`, `ViewModels.Tests` |
| `Modules/Physics` | `Views.Physics`, `ViewModels.Physics` |
| `Modules/Profile` | `Views.Profile`, `ViewModels.Profile`, `QuoteItem` |
| `Modules/Motivator` | `Views.Motivator`, `ViewModels.Motivator` |
| `Modules/Clean` | `Views.Clean`, `ViewModels.Clean` — audio player tab |
| `Modules/Review` | `Views.Review`, `ViewModels.Review` — feedback form (`FormPage` from Options) |

### Folder-to-namespace map (physical path may differ)

| Physical path | Namespace | Notes |
|---------------|-----------|-------|
| `Modules/Practice/Collection/TechniquesPage` | `Views.TechniquesPage` | Shell home tab |
| `Modules/Practice/Collection/TechniquesViewModel` | `Technique.Main` | Legacy; target `ViewModels.Practice` |
| `Modules/Practice/TechniqueItem.cs` | `Presentation.Technique` | List DTO for techniques |
| `Modules/Profile/QuoteItem.cs` | `Modules.Profile` | Profile quote card model |
| `Modules/Profile/InfoPage` | `Views.About` | Historical name |
| `Modules/BaseViewModel.cs` | `ViewModels` | Shared VM base |
| `Ui/Components/` | `UI.Components` | Disk folder `Ui/`, namespace `UI` (case) |
| `Services/Dialogs`, `Services/Toasts` | `Services.Dialogs`, `Services.Toasts` | Not Service Locator — normal DI services |

Do not physically move `Modules/` → `Views/` in bulk without updating XAML `x:Class`, DI registrations, and Shell routes. Prefer new files in target namespaces; migrate incrementally.

Per-technique legacy `*Page.xaml` (Spin, Hack, …) removed; bodies live in `Ui/Techniques/Bodies`.

### UI layer (tokens → components → pages)

- **Tokens:** `Resources/Styles/Typography.xaml`, `Components.xaml` (merged in `App.xaml` after `Colors.xaml`)
- **Components:** `UI/Components/` — namespace `PsychologyApp.Presentation.UI.Components` (`xmlns:ui` on pages)
- **Layouts:** `Controls/TechniquePageShell.xaml` for practice sessions
- **Catalog:** see `PsychologyApp.Presentation/UI/README.md`

Dead controls removed: `LocalFrame`, `LocalEditor`, `LocalEntry`, `ExtendedLabel`.



## Techniques UI



- `TechniqueCatalog` — single registry: session content, list metadata (`ListTitle`, `ListSubtitle`, …), `TechniqueUiKind`

- `TechniqueListCatalog.BuiltIn` — projection from `TechniqueCatalog` (no duplicate titles)

- `TechniqueSessionPage` + `TechniquePageShell` — one page for all built-in techniques

- `TechniqueBodyFactory` + `Ui/Techniques/Bodies/*` — `Entry`, `Polarity`, `Paper`, `Copied`, `None` body templates

- `ITechniqueViewModelFactory` — `TechniqueSessionViewModel` for simple kinds; `PaperListViewModel` (Paper + Copied, `clearTextAfterAdd` flag) and `PolarityViewModel` for list-based bodies

- `TechniqueSessionBehavior` — analytics on page disappear



## Tests module (`Modules/Tests`, namespaces `Views.Tests` / `ViewModels.Tests`)



- `TestCatalogLoader` — all tests from `Resources/Raw/tests/*.json` (beck, luscher, questionnaires)

- `TestScoreAnalyzers` — scoring functions referenced by `analyzerId` in JSON

- `TestsListViewModel` — orchestration only; `ITestsListViewModelFactory` + activator on `TestsListPage`



## Analytics



- `IPageAnalyticsService` / `PageAnalyticsService` persist visit duration via `IStatisticService` (`TimeProvider` for duration)

- `Statistics.DateTime` stored as ISO-8601 UTC text via Dapper `Iso8601DateTimeHandler`



## AOT / trimming (Android Release)



- `PublishTrimmed` + `RunAOTCompilation` for Release Android

- `TrimmerRootAssembly` for `PsychologyApp.Presentation`, `CommunityToolkit.Maui`, `System.Text.Json`, Dapper (Infrastructure csproj)



## Domain model (light DDD)

- **Entities** with factory methods and behavior: `Technique`, `Quot`, `Statistic`
- **Value objects** in `Domain.Colour`: `ColourValue`, `ColourMeaning`
- **Domain services** (static): `LuscherScoring` — CO/BK psychometric rules for the Lüscher test
- Repository ports live in Application (hexagonal), not in Domain
- Aggregates, domain events, and CQRS are intentionally not used at current scale

## Reason content

- `IReasonContentProvider.GetPageAsync(page, pageSize)` — paginated slices; `CachedReasonContentProvider` and `MauiReasonContentProvider` cache full content once

## Presentation MVVM conventions (phase 3–4)

- Page code-behind does not call `PopAsync` / `PopToRootAsync`; back navigation uses `Finish`, `BackCommand`, or `INavigationService` (`GoBackAsync`, `GoToRootAsync`)
- Async UI work uses `AsyncCommand` or `.FireAndForget()` on fire-and-forget tasks (not bare `_ = task` or `Command(async () => …)`)
- `TechniquesPage` toolbar opens profile via `OpenProfileCommand` on `TechniquesViewModel`

## Naming conventions (phase 4)

- **Physical folders:** `Modules/{Area}/` groups feature code; `Ui/` for reusable components; `Services/` for navigation and factories
- **Target namespaces:** `PsychologyApp.Presentation.Views.{Area}` for pages, `PsychologyApp.Presentation.ViewModels.{Area}` for ViewModels (folder path may differ until a later physical move)
- **Backend vs UI terms (intentional split — do not unify):**
  - Domain/Application/Infrastructure/DB: `Quot`, `IQuotService`, `IQuotRepository`, table `Quots`, `IsReaded`, `QuotDTO`
  - Presentation UI: `QuotePage`, `QuoteViewModel`, `QuoteItem`, `QuoteCardView`
  - Mapping happens in ViewModels (`QuotDTO` → `QuoteItem`); renaming backend types would break SQL schema and API contracts
- **Resolved renames:** `Practice` (not Practic), `Physics` (not Physic), `Clean` (not Cleaner)
- **Legacy namespace debt:** `Technique.*`, `Modules.Practice.*` in Constructor — migrating to `ViewModels.Practice.Constructor`

## Settings, localization, and `UserPreferences`

- Keys: `Language`, `Theme`, `Color`, `Form`, `Size`, `IsBold` in `Presentation/Infrastructure/UserPreferences.cs`
- Stored values use stable keys (`ru`/`en`, `light`/`dark`, `blue`/`red`/…) — legacy Russian/English display strings are normalized on load
- `AppStrings` provides localized UI text for Shell, Options/Settings, Tests, Physics, technique shell, and startup errors
- Embedded quotes: `IQuotContentProvider` → `MauiQuotContentProvider` reads `Resources/Raw/quotes/quotes.{ru|en}.json`; `QuotService.LoadSingleAsync` picks a random seed and persists to SQLite (no external API)
- Localized content assets: `ContentAssets.Localized()` selects `*.en.json` / `Psyhosomatic.en.txt` when language is English
- `ApplyAll()` at startup and on save: culture, `UserAppTheme`, accent colors, typography, corner-radius shapes, Shell/status-bar chrome; raises `UserPreferences.Changed`
- `SettingsViewModel` loads prefs in ctor; `Finish` saves, calls `ApplyAll()`, shows confirmation, navigates back
- `OptionsViewModel.OpenSettingsPageCommand` → `GoToSettingsAsync()`

## Cross-cutting



- `GlobalExceptionHandler` — `ILogger` + `IDialogService` on unhandled errors

- CI: solution build, tests (including `PsychologyApp.Domain.Tests` and `PsychologyApp.Presentation.Tests`), iOS + Android Debug; coverage gate fails if summary is missing or below 60%

- Release Android trim smoke recommended on device


# PsychologyApp Architecture



## Layers



| Layer | Project | Responsibility |

|-------|---------|----------------|

| Domain | `PsychologyApp.Domain` | Entities, value objects, domain constants |

| Application | `PsychologyApp.Application` | Use cases, DTOs, ports (interfaces), DI registration |

| Infrastructure | `PsychologyApp.Infrastructure` | SQLite/Dapper, HTTP clients, port implementations |

| Bootstrap | `PsychologyApp.Bootstrap` | `AddPsychologyAppCore()` — wires Application + Infrastructure |

| Presentation | `PsychologyApp.Presentation` | MAUI UI, platform adapters, ViewModels |

| Tests | `PsychologyApp.*.Tests`, `PsychologyApp.Testing` | Unit tests per layer; shared in-memory SQLite helpers; integration tests via Bootstrap |



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

- `SchemaVersion` table + incremental migrations in `SqliteSchema` (current **v5**: `Techniques.Description`, `TechniqueDTO.Algorithm`; v4→v5 renames `Describtion` column)

- Repositories implement `PsychologyApp.Application.Abstractions.Persistence.*`

- Dapper with parameterized SQL (`EntitySqlMaps`)

- WAL + `synchronous=NORMAL` + `busy_timeout` PRAGMAs



## DI and UI composition



- `MauiProgram` — MAUI host, fonts, handlers, logging

- `GlobalExceptionHandler` — singleton registered in DI; `App` calls `Attach(this)` for unhandled exceptions and `AsyncCommand` errors

- `IPageFactory` / `MauiPageFactory` — single entry point for constructing pages with constructor-injected dependencies

- `IPageViewModelActivator` + `ActivateViewModel` — Shell and module pages bind VMs via factories

- `Services/Factories/*` — ViewModel factories split by feature; `ViewModelFactoryBase.ResolveNavigation` centralizes `NavigationContext.From(navigation)`
- `TechniqueListBuilder`, `TodayRecommendationResolver`, `PracticeDashboardLoader`, `TechniqueDashboardApplier`, `TechniquesListInitializer`, `DesignerTechniqueOperations`, `TechniqueSessionCompletionService`, `ListTechniqueSessionHelper`, `EntryDraftCoordinator`, `ListSessionDraftCoordinator`, `CustomTechniqueSessionOperations`, `TestRetakeOperations`, `TestHistoryLoader`, `TestsListLoader`, `QuestionnaireSubmissionService`, `LuscherTestSubmissionService`, `FindProblemContentLoader`, `ProfileStatsLoader`, `ProfileQuotesPresenter`, `ProfileQuotesLoader`, `ProfilePracticeHistoryLoader`, `ProfileFeaturedTechniquesBuilder`, `UserProfileRefreshCoordinator`, `QuoteFeedCoordinator`, `QuoteFeedLoader`, `QuoteItemCommandsFactory`, `MusicPlaylistPresenter`, `MusicPlaybackPresenter`, `MusicPlaylistViewApplier`, `PhysicsSearchCoordinator`, `PhysicsSearchSession`, `PhysicsSearchUiState`, `PhysicsSearchUiBinder`, `PhysicsReasonItemFactory`, `SettingsPreferencesPresenter` — extracted from god ViewModels; `PhysicsSearchCoordinator` runs debounce and in-memory search without `Task.Run`
- `IPageHost` / `MauiPageHost` — dialog host without `Application.Current`
- `IUserPreferencesStore`, `IDatabaseReadySignal` (`DatabaseReadySignal` — singleton readiness gate with `SignalReady` / `SignalFailed`), `IAudioPlaybackService` — DI ports replacing static hooks where wired
- `IUserProgressService.GetLastPracticeDatesAsync` / `GetSessionDraftKeysAsync` — batch SQLite reads for the practice techniques list (replaces per-technique N+1)
- Shell tabs (`QuotePage`, `TechniquesPage`, `UserPage`, `PhysicsSearchPage`) call `EnsureInitializedAsync` on first appear; `QuoteFeedCoordinator` / `ProfileQuotesLoader` registered as transient
- ViewModels depend on `INavigationService` only (not MAUI `INavigation`); factories resolve navigation context
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

**Practice session flow:** `PaperListViewModel`, `PolarityViewModel`, and entry `TechniqueSessionViewModel` finish via `ListTechniqueSessionHelper` (per-VM instance with navigation-scoped deps; wraps `TechniqueSessionCompletionService` for progress record, draft cleanup, and `PracticeCompletionNavigator`). Entry techniques delegate debounced draft persistence to `EntryDraftCoordinator`. Paper/Polarity list drafts use `ListSessionDraftCoordinator` (typed wrappers per technique). Custom technique sessions use `CustomTechniqueSessionOperations` for load/remove/mark-complete; finish still records progress through `TechniqueSessionCompletionService` (without deleting a session draft).

**Tests module flow:** Luscher standard/brief results persist through `LuscherTestSubmissionService`. Find-problem page reloads localized copy through `FindProblemContentLoader`. `TestsListLoader` (singleton with injected `IUserProgressService` + `ITestCatalogService`) builds catalog rows with latest-result metadata; `TestsListViewModel` only passes navigation and selection handler. `TestHistoryLoader` loads history entries; retake from history or result pages goes through `TestRetakeOperations` (root navigation + `TestItem.StartAsync`).

### Presentation layout

Physical folders mirror namespaces (`{Folder}/{Area}/…` → `PsychologyApp.Presentation.{Namespace}`):

| Folder | Role |
|--------|------|
| `Views/{Area}/` | MAUI pages (`*Page.xaml` + code-behind) |
| `ViewModels/{Area}/` | ViewModels; `ViewModels/BaseViewModel.cs` — shared base |
| `Models/{Area}/` | Presentation DTOs, JSON catalog loaders, technique registry |
| `UI/Components/`, `UI/Techniques/` | Reusable XAML (`UI.Components`, `UI.Techniques`) |
| `Controls/` | Composite layouts (`TechniquePageShell`) |
| `Common/` | Shared MAUI helpers (namespace `PsychologyApp.Presentation.Common`); subfolders: `Preferences/`, `Localization/`, `Animation/`, `PressFeedback/`, `Formatting/`, `Infrastructure/`, `Behaviors/` |
| `PsychologyApp.Presentation.Core` | net10.0 library: catalogs, test models, `AppStrings`, `EntryDraft` — referenced by Presentation and tests |
| `Platform/` | MAUI content adapters (`MauiReasonContentProvider`, …) |
| `Platforms/` | MAUI multi-target entry points (Android, iOS, MacCatalyst) |
| `Services/` | Navigation, page/VM factories, dialogs, toasts, `Services/Practice` messenger |

Feature areas under `Views` / `ViewModels` / `Models`: `Practice` (incl. `Techniques`, `Constructor`), `Tests`, `Physics`, `Profile`, `Motivator`, `Clean`, `Review`, `Onboarding`.

### Key paths

| Path | Namespace | Notes |
|------|-----------|-------|
| `Views/Practice/TechniquesPage` | `Views.Practice` | Shell home tab |
| `Views/Practice/Techniques/*` | `Views.Practice.Techniques` | Session, theory |
| `Views/Practice/Constructor/*` | `Views.Practice.Constructor` | User technique builder |
| `Models/Practice/Techniques/TechniqueCatalog*` | `Models.Practice.Techniques` | Registry + list metadata |
| `UI/Techniques/TechniqueBodyFactory.cs` | `UI.Techniques` | Session body templates |
| `Common/Behaviors/TechniqueSessionBehavior.cs` | `Common.Behaviors` | Session analytics |
| `Services/Practice/*` | `Services.Practice` | `ITechniqueMessenger`, session completion/draft coordinators |
| `Services/Tests/*` | `Services.Tests` | `TestRetakeOperations`, `TestHistoryLoader`, `TestsListLoader`, catalog services |
| `Models/Tests/*` | `Models.Tests` | Test catalog JSON, `Question`/`Answer` |
| `UI/Components/` | `UI.Components` | Reusable XAML components |

Per-technique legacy `*Page.xaml` (Spin, Hack, …) removed; bodies live in `UI/Techniques/Bodies`.

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

- `TechniqueBodyFactory` + `UI/Techniques/Bodies/*` — `Entry`, `Polarity`, `Paper`, `Copied`, `None` body templates

- `ITechniqueViewModelFactory` — `TechniqueSessionViewModel` for simple kinds; `PaperListViewModel` (Paper + Copied, `clearTextAfterAdd` flag) and `PolarityViewModel` for list-based bodies

- `TechniqueSessionBehavior` — analytics on page disappear



## Tests module (`Views/Tests`, `ViewModels/Tests`, `Models/Tests`)



- `ITestAssetReader` / `MauiTestAssetReader` — opens localized JSON from `Resources/Raw/tests/`

- `TestCatalogParser` + `TestDefinition` (Core) — pure JSON parsing; `CachedTestCatalogService` caches catalog per language and invalidates with `ContentCacheInvalidator`

- `TestItemFactory` — maps `TestDefinition` to UI `TestItem` (navigation commands, question cloning)

- `TestScoreAnalyzers` / `TestScoreRecommendation` — scoring and technique recommendation by `analyzerId`

- `LuscherTestViewModel` + `LuscherColorGridView` — shared Lüscher UI (`LuscherMode.Standard` / `Brief`)

- `TestsListViewModel` — loads catalog via `ITestCatalogService`, enriches with `IUserProgressService`; `ITestsListViewModelFactory` + activator on `TestsListPage`



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
- `IReasonSearchService.LoadReasonsAsync` — loads via `IReasonContentProvider.LoadReasonsAsync` (full in-memory cache); `Search` ranks in Application layer
- `NavigationContext` + `Func<NavigationContext, INavigationService>` — unified navigation resolution in ViewModel factories

## Presentation MVVM conventions (phase 3–4)

- Page code-behind does not call `PopAsync` / `PopToRootAsync`; back navigation uses `Finish`, `BackCommand`, or `INavigationService` (`GoBackAsync`, `GoToRootAsync`)
- Async UI work uses `AsyncCommand` or `.FireAndForget()` on fire-and-forget tasks (not bare `_ = task` or `Command(async () => …)`)
- `TechniquesPage` toolbar opens profile via `OpenProfileCommand` on `TechniquesViewModel`

## Naming conventions (phase 4)

- **Physical folders:** `Views/`, `ViewModels/`, `Models/` per feature area; `UI/` for reusable components; `Services/` for navigation and factories
- **Target namespaces:** `PsychologyApp.Presentation.Views.{Area}` for pages, `PsychologyApp.Presentation.ViewModels.{Area}` for ViewModels (folder path may differ until a later physical move)
- **Backend vs UI terms (intentional split — do not unify):**
  - Domain/Application/Infrastructure/DB: `Quot`, `IQuotService`, `IQuotRepository`, table `Quots`, `IsReaded`, `QuotDTO`
  - Presentation UI: `QuotePage`, `QuoteViewModel`, `QuoteItem`, `QuoteCardView`
  - Mapping happens in ViewModels (`QuotDTO` → `QuoteItem`); renaming backend types would break SQL schema and API contracts
- **Resolved renames:** `Practice` (not Practic), `Physics` (not Physic), `Clean` (not Cleaner)
- **Presentation layout:** physical folders and namespaces are aligned (`Views.*`, `ViewModels.*`, `Models.*`)

## Settings, localization, and `UserPreferences`

- Keys: `Language`, `Theme`, `Color`, `Form`, `Size`, `IsBold` in `Presentation/Common/UserPreferences.cs`
- MAUI port adapters: `Presentation/Platform/MauiReasonContentProvider`, `MauiQuotContentProvider`
- Shell startup: `IShellStartupCoordinator` (DB init + onboarding)
- Stored values use stable keys (`ru`/`en`, `light`/`dark`, `blue`/`red`/…) — legacy Russian/English display strings are normalized on load
- `AppStrings` provides localized UI text for Shell, Options/Settings, Tests, Physics, technique shell, and startup errors
- Embedded quotes: `IQuotContentProvider` → `MauiQuotContentProvider` reads `Resources/Raw/quotes/quotes.{ru|en}.json`; `QuotService.LoadSingleAsync` picks a random seed and persists to SQLite (no external API)
- Localized content assets: `ContentAssets.Localized()` selects `*.en.json` / `Psyhosomatic.en.txt` when language is English
- `ApplyAll()` at startup and on save: culture, `UserAppTheme`, accent colors, typography, corner-radius shapes, Shell/status-bar chrome; raises `UserPreferences.Changed`
- `SettingsViewModel` loads prefs in ctor; `Finish` saves, calls `ApplyAll()`, shows confirmation, navigates back
- `OptionsViewModel.OpenSettingsPageCommand` → `GoToSettingsAsync()`

## Cross-cutting



- `GlobalExceptionHandler` — `ILogger` + `IDialogService` on unhandled errors

- CI: Windows — full solution build, tests (Domain, Application, Infrastructure, Integration, Presentation), iOS + Android Debug; coverage gate fails if summary is missing or below 60%. Linux — `dotnet build` + `dotnet test` (catches case-sensitive path issues such as `UI/`)

## Test projects

| Project | Scope |
|---------|--------|
| `PsychologyApp.Domain.Tests` | Domain entities and value objects |
| `PsychologyApp.Application.Tests` | Services with mocked ports |
| `PsychologyApp.Infrastructure.Tests` | Repositories, SQLite schema, HTTP clients |
| `PsychologyApp.Integration.Tests` | `AddPsychologyAppCore()` + in-memory SQLite end-to-end (service → repository → DB) |
| `PsychologyApp.Presentation.Core` | Catalogs, test JSON models, `AppStrings`, `EntryDraft` (net10.0, no MAUI) |
| `PsychologyApp.Presentation.Tests` | ViewModels and MAUI helpers via linked sources + `ProjectReference` to Core |
| `PsychologyApp.Testing` | Shared `SharedMemoryConnectionFactory`, `IntegrationTestServiceCollection`, `FakeReasonContentProvider` |

- Release Android trim smoke recommended on device


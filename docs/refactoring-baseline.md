# Refactoring baseline (2026-06-12, Phase 13 complete)

## Test suite

```powershell
dotnet test PsychologyApp.Presentation.Tests
dotnet test PsychologyApp.Application.Tests
dotnet test PsychologyApp.Domain.Tests
dotnet test PsychologyApp.Infrastructure.Tests
dotnet test PsychologyApp.Integration.Tests
```

| Project | Tests (approx.) |
|---------|-----------------|
| Domain | 48 |
| Application | 57 |
| Infrastructure | 29 |
| Integration | 5 |
| Presentation | 218 |
| **Total** | **~356** |

Application layer coverage **~67%** (target ≥65%).

## Slimmed ViewModels (orchestration file lines)

### Phase 1 (top-5)

| ViewModel | Lines (main file) | Partials |
|-----------|-------------------|----------|
| UserViewModel.cs | ~149 | Labels, Quotes |
| QuoteViewModel.cs | ~168 | Labels, Feed |
| MusicPlayerViewModel.cs | ~156 | Labels, Properties |
| TechniquesViewModel.cs | ~153 | Labels, Dashboard |
| PhysicsSearchViewModel.cs | ~94 | Labels, Init, Search |

### Phase 2 (secondary VMs)

| ViewModel | Lines (main file) | Partials |
|-----------|-------------------|----------|
| DesignerViewModel.cs | ~145 | Labels, Fields |
| SettingsViewModel.cs | ~65 | Labels, Preferences |
| BaseTestViewModel.cs | ~20 | Handlers, Results, ColorVisibility |
| LuscherTestViewModel.cs | ~175 | Labels |

### Phase 3 (practice sessions + BaseViewModel)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| BaseViewModel.cs | ~74 | Technique, LoadingState |
| TechniqueSessionViewModel.cs | ~73 | EntryDraftCoordinator, TechniqueSessionCompletionService |
| CreatedViewModel.cs | ~154 | Labels; CustomTechniqueSessionOperations, TechniqueSessionCompletionService |
| FormViewModel.cs | ~50 | Labels, Send |
| PaperListViewModel.cs | ~168 | TechniqueSessionCompletionService |
| PolarityViewModel.cs | ~197 | TechniqueSessionCompletionService |

### Phase 4 (list drafts + tests + onboarding)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| PaperListViewModel.cs | ~120 | Labels; ListSessionDraftCoordinator |
| PolarityViewModel.cs | ~145 | Labels; ListSessionDraftCoordinator |
| LuscherTestViewModel.cs | ~50 | Labels, Init, Results |
| TestHistoryViewModel.cs | ~85 | Labels; TestHistoryLoader, TestRetakeOperations |
| TestsListViewModel.cs | ~70 | Labels; TestsListLoader |
| TestResultViewModel.cs | ~95 | Labels; TestRetakeOperations |
| QuestionViewModel.cs | ~95 | Labels |
| OnboardingViewModel.cs | ~75 | Labels, Flow |

### Phase 5 (top-4 services + secondary partials)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| QuoteViewModel.cs | ~99 | Labels, Feed, Init; QuoteFeedLoader |
| MusicPlayerViewModel.cs | ~71 | Labels, Properties, Playback |
| TechniquesViewModel.cs | ~122 | Labels, Dashboard, Init; TechniquesListInitializer |
| UserViewModel.cs | ~105 | Labels, Quotes, Refresh; UserProfileRefreshCoordinator |
| CreatedViewModel.cs | ~74 | Labels, Session |
| FindProblemViewModel.cs | ~100 | Labels, Reload; `AlgorithmRow` → Models/Tests |
| TheoryViewModel.cs | ~73 | Labels |

### Phase 6 (list sessions + questionnaire + content partials)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| PaperListViewModel.cs | ~52 | Labels, Session; ListTechniqueSessionHelper |
| PolarityViewModel.cs | ~49 | Labels, Session, Fields; ListTechniqueSessionHelper |
| DesignerViewModel.cs | ~59 | Labels, Fields, Init, Save |
| QuestionViewModel.cs | ~54 | Labels, Results; QuestionnaireSubmissionService |
| FindProblemViewModel.cs | ~64 | Labels, Content, Reload |
| TheoryViewModel.cs | ~33 | Labels, Content |
| TechniqueSessionViewModel.cs | ~48 | Session |
| TechniquesViewModel.cs | ~105 | Labels, Dashboard, Init, Messenger |

### Phase 7 (settings + profile + test partials + session helper)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| SettingsViewModel.cs | ~44 | Labels, Properties, Collections, Apply |
| UserViewModel.cs | ~85 | Labels, Quotes, Refresh, Commands |
| QuoteViewModel.cs | ~75 | Labels, Feed, Init |
| TestResultViewModel.cs | ~54 | Labels, Recommendation, Commands |
| TestHistoryViewModel.cs | ~58 | Labels, Load |
| TechniquesViewModel.cs | ~79 | Labels, Dashboard, Init, Messenger, Commands |
| TechniqueSessionViewModel.cs | ~49 | Session; ListTechniqueSessionHelper |
| OnboardingViewModel.cs | ~44 | Labels, Flow, Steps |

### Phase 8 (physics/profile collections + luscher service + helper ctor)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| PhysicsSearchViewModel.cs | ~55 | Labels, Init, Search, State |
| UserViewModel.cs | ~55 | Labels, Quotes, Refresh, Commands, Collections, Stats |
| TechniquesViewModel.cs | ~55 | Labels, Dashboard, Init, Messenger, Commands, Collections |
| LuscherTestViewModel.cs | ~45 | Labels, Init, Results; LuscherTestSubmissionService |
| BaseTestViewModel.cs | ~20 | Handlers, Results.Properties, ColorVisibility |
| Paper/Polarity/TechniqueSession | — | Session; ListTechniqueSessionHelper (ctor-scoped deps) |

### Phase 9 (find-problem loader + oversized partials)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| FindProblemViewModel.cs | ~65 | Labels, Content, Reload; FindProblemContentLoader |
| LuscherTestViewModel.cs | ~45 | Labels, Init, Results.Labels, Results.Scoring; LuscherTestSubmissionService |
| QuoteViewModel.cs | ~75 | Labels, Feed, Init, Properties |
| PhysicsSearchViewModel.cs | ~55 | Labels, Init, Search, State, Properties |
| PaperListViewModel.cs | ~50 | Labels, Session, Fields; ListTechniqueSessionHelper |
| BaseViewModel.cs | ~74 | Technique, LoadingState, LoadingState.Properties |
| CreatedViewModel.cs | ~50 | Labels, Session, Commands |

### Phase 10 (oversized partials + TestsListLoader ctor)

| ViewModel | Lines (main file) | Partials / services |
|-----------|-------------------|---------------------|
| TestsListViewModel.cs | ~35 | Labels, Load; TestsListLoader (ctor-injected progress + catalog) |
| QuoteViewModel.cs | ~75 | Labels, Feed, Init, Init.Load, Properties |
| MusicPlayerViewModel.cs | ~55 | Labels, Properties, Playback, Playback.Playlist |
| BaseTestViewModel.cs | ~20 | Handlers, Results.Properties, Results.Colors, ColorVisibility.Properties |
| FormViewModel.cs | ~45 | Labels, Send, Send.Channels |
| TechniquesViewModel.cs | ~55 | Labels, Dashboard, Dashboard.Properties, Init, Messenger, Commands, Collections |

All listed ViewModels use **`UiThread.RunAsync`** (no direct `MainThread` in ViewModels).

### Phase 11 (post-review improvements)

| Area | Change |
|------|--------|
| Dead code | Removed unused `ReviewSent*` / `Review*Error` strings; slimmed `UserViewModel` / `TechniqueSessionViewModel` ctors |
| Domain | `LuscherScoring.CalculateBk` divide-by-zero guard |
| Performance | `PhysicsSearchCoordinator` — in-process search + debounce without `Task.Run`; `MusicAudioCache` — shared `HttpClient` with timeout and size guard; `InputFocusHelper` — sync border styling |
| Architecture | `DatabaseReadySignal` — single readiness gate (`SignalReady` / `SignalFailed`); removed static `AppReadiness` |
| Logging | `ListSessionDraftCoordinator` passes `ILogger` into `SessionDraftStore.LoadAsync` |
| Tests | `PhysicsSearchCoordinatorTests`, `MusicAudioCacheTests`, Luscher denominator-zero case, `TestDatabaseReady` helper |

### Phase 12 (performance balanced)

| Area | Change |
|------|--------|
| Batch SQLite | `GetLastPracticeDatesAsync` + `GetSessionDraftKeysAsync` — 2 queries instead of 28 for technique list |
| Technique list | `TechniqueListBuilder.BuildStaticItemsAsync` uses batch lookups |
| Parallel loads | `Task.WhenAll` in `ProfileStatsLoader`, `UserProfileRefreshCoordinator`, `TechniquesListInitializer` |
| Search | `ReasonSearchService` single-pass match scoring |
| Tests | batch repository tests, `BuildStaticItemsAsync_UsesBatchProgressQueries`, subtitle rank case |

### Phase 13 (shell startup & stateful services)

| Area | Change |
|------|--------|
| Lazy init | Shell tabs: `EnsureInitializedAsync` on first `OnAppearing` (Quote, Techniques, User, PhysicsSearch); no ctor `InitAsync` |
| Stateful services | `QuoteFeedCoordinator`, `ProfileQuotesLoader` → Transient; factories use `Func<>` for fresh instances |
| Dev schema | `SqliteSchema.DropTablesSql` extended to v5 tables |
| Tests | lazy-init ctor tests, fresh coordinator isolation, `DropAllTablesAsync_RemovesV5Data` |

## New services

### Phase 1
- `ProfileQuotesLoader`, `QuoteItemCommandsFactory`
- `MusicPlaybackPresenter`, `MusicPlaylistViewApplier`
- `PhysicsSearchSession`, `PhysicsSearchUiBinder`, `PhysicsReasonItemFactory`
- `TechniqueDashboardApplier`

### Phase 2
- `DesignerTechniqueOperations` — load/build/add/update custom techniques for constructor

### Phase 3
- `TechniqueSessionCompletionService` — record progress, delete session draft, post-completion navigation
- `EntryDraftCoordinator` — debounced entry-field draft load/save for entry techniques
- `CustomTechniqueSessionOperations` — load algorithm, remove, mark completed for custom technique sessions

### Phase 4
- `ListSessionDraftCoordinator` (+ `PaperListDraftCoordinator`, `PolarityListDraftCoordinator`) — list-based session draft load/save
- `TestRetakeOperations` — shared retake flow (root + start test)
- `TestHistoryLoader` — history entries + localized title
- `TestsListLoader` — catalog item building with latest result metadata (progress + catalog injected via ctor)

### Phase 5
- `QuoteFeedLoader` — seed, reset known quotes, fetch/map feed items
- `TechniquesListInitializer` — catalog + dashboard snapshot for techniques list init
- `UserProfileRefreshCoordinator` — stats/history load with generation guard + quote reload policy

### Phase 6
- `ListTechniqueSessionHelper` — shared list-technique session completion wrapper
- `QuestionnaireSubmissionService` — validate answers, score, save result, recommendation mapping

### Phase 8
- `LuscherTestSubmissionService` — Luscher standard/brief result JSON + persistence

### Phase 9
- `FindProblemContentLoader` — localized reload of test description/algorithm/comment by test id

## ViewModels with dedicated tests

- Top VMs: `TopViewModelsTests`, `SlimmingServicesTests`
- Constructor: `DesignerViewModelTests`, `DesignerTechniqueOperationsTests`
- Practice sessions: `PracticeSessionServicesTests`, `TestModuleOperationsTests`, `ListSessionDraftCoordinatorTests`
- Tests module: `LuscherTestViewModelTests`, `TestHistoryViewModelTests`, `TestsListViewModelTests`, `TestResultViewModelTests`, `QuestionViewModelTests`
- Extracted services: `ExtractedServicesTests`, `ProfileStatsLoaderTests`, `Phase5LoaderTests`, `Phase6LoaderTests`, `LuscherTestSubmissionServiceTests`, `FindProblemContentLoaderTests`, `QuoteViewModelTests`, `MusicPlayerViewModelTests`, `PhysicsSearchCoordinatorTests`, `MusicAudioCacheTests`

## Architecture notes

- ViewModels use `UiThread.RunAsync` instead of direct `MainThread`
- Quote load/cancel in `ProfileQuotesLoader`; share/copy/like in `QuoteItemCommandsFactory`; feed init/load-more in `QuoteFeedLoader`
- Custom technique CRUD in `DesignerTechniqueOperations`; custom session remove/complete in `CustomTechniqueSessionOperations`
- Standard technique session completion in `TechniqueSessionCompletionService` (Created finish); Paper, Polarity, and entry techniques finish via `ListTechniqueSessionHelper` (wraps completion + navigation)
- Entry technique drafts in `EntryDraftCoordinator`; list technique drafts in `ListSessionDraftCoordinator` (with draft-restore logging)
- `IDatabaseReadySignal` / `DatabaseReadySignal` — single startup readiness gate; `ShellStartupCoordinator` signals ready/failed after DB init
- Physics reason search runs in-process in `PhysicsSearchCoordinator` (no thread-pool hop for in-memory `Contains` search)
- Technique list progress badges load via batch SQLite queries (`GetLastPracticeDatesAsync`, `GetSessionDraftKeysAsync`)
- Shell tab ViewModels defer data load to `EnsureInitializedAsync` on first `OnAppearing` (see `TestsListPage` pattern)
- `QuoteFeedCoordinator` and `ProfileQuotesLoader` are transient per ViewModel scope
- Test list/history loading and retake in `TestsListLoader`, `TestHistoryLoader`, `TestRetakeOperations`; questionnaire scoring in `QuestionnaireSubmissionService`; Luscher persistence in `LuscherTestSubmissionService`; find-problem reload in `FindProblemContentLoader`
- Profile dashboard refresh orchestration in `UserProfileRefreshCoordinator`; techniques list init in `TechniquesListInitializer`
- Large ViewModels split into partial classes (Labels, domain-specific partials)
- All ViewModel factories use `ViewModelFactoryBase.ResolveNavigation` where navigation is resolved

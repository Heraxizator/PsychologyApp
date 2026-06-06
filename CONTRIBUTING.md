# Contributing

## Configuration

Set review email and API URLs in `PsychologyApp.Presentation/appsettings.json` (`AppSettings` section). See `appsettings.Development.json.example`. Do not commit personal email addresses.

Namespaces are unified under `PsychologyApp.Presentation.Views` / `ViewModels` (see `ARCHITECTURE.md`).

## Build

```bash
dotnet restore PsychologyApp.sln
dotnet build PsychologyApp.Presentation/PsychologyApp.Presentation.csproj -f net10.0-ios
dotnet test PsychologyApp.sln
```

## PR checklist

- [ ] No `new` for application services in Presentation
- [ ] No direct `PsychologyApp.Infrastructure` usings in ViewModels (except `MauiServiceProvider`)
- [ ] SQL uses parameters (`@name`), not string interpolation
- [ ] Unit tests added or updated for changed behavior
- [ ] `dotnet build` and `dotnet test` pass locally

## Naming

- Namespaces: `PsychologyApp.{Layer}.{Feature}`
- Interfaces in `Application.Abstractions`

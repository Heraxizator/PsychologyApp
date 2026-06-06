# Psychology App

MAUI-приложение по психологии и самопомощи: техники, тесты, психосоматика, мотиватор, профиль.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/maui)

**Актуальная архитектура:** [ARCHITECTURE.md](ARCHITECTURE.md)  
**UI-компоненты:** [PsychologyApp.Presentation/Ui/README.md](PsychologyApp.Presentation/Ui/README.md)  
**Участие в разработке:** [CONTRIBUTING.md](CONTRIBUTING.md)

## Стек

| Слой | Технологии |
|------|------------|
| UI | .NET MAUI 10, MVVM, `Ui/Components` |
| Application | Use cases, DTOs, порты (`Abstractions`) |
| Infrastructure | SQLite, Dapper, HTTP (Forismatic API) |
| Tests | xUnit — Domain, Application, Infrastructure, Presentation |

## Структура решения

```
PsychologyApp.Domain          — сущности
PsychologyApp.Application     — сервисы, DTO, интерфейсы репозиториев
PsychologyApp.Infrastructure  — SQLite/Dapper, API-клиенты
PsychologyApp.Bootstrap       — AddPsychologyAppCore()
PsychologyApp.Presentation    — MAUI UI, ViewModels, навигация
PsychologyApp.*.Tests         — unit-тесты по слоям
```

Presentation: `Modules/` (страницы), `Services/` (навигация, фабрики), `Ui/Components/` (переиспользуемый XAML), `Controls/TechniquePageShell`.

## Установка и запуск

```bash
dotnet restore PsychologyApp.sln
dotnet build PsychologyApp.sln
```

Конфигурация: `PsychologyApp.Presentation/appsettings.json` (см. `appsettings.Development.json.example`).

Запуск на платформе (пример):

```bash
dotnet build PsychologyApp.Presentation/PsychologyApp.Presentation.csproj -f net10.0-android
```

## Тестирование

```bash
dotnet test PsychologyApp.sln
```

## Принципы

- ViewModels не ссылаются на Infrastructure
- Навигация через `INavigationService`, не `PopAsync` в code-behind
- Новый UI — сначала `xmlns:ui` компоненты из `Ui/Components`
- SQL только с параметрами (`@name`)

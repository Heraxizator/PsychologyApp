# Psychology App

MAUI-приложение по психологии и самопомощи: техники, тесты, психосоматика, мотиватор, профиль.

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/maui)

**Актуальная архитектура:** [ARCHITECTURE.md](ARCHITECTURE.md)  
**UI-компоненты:** [PsychologyApp.Presentation/Shared/UI/README.md](PsychologyApp.Presentation/Shared/UI/README.md)

## Стек

| Слой | Технологии |
|------|------------|
| UI | .NET MAUI 10, MVVM, `UI/Components` |
| Application | Use cases, DTOs, порты (`Abstractions`) |
| Infrastructure | SQLite, Dapper, локальные цитаты |
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

Presentation (FSD): `Features/`, `Pages/` (по feature-слайсам), `Widgets/`, `Entities/`, `Shared/UI/Components/`, `App/` (навигация, DI).

## Установка и запуск

```bash
dotnet restore PsychologyApp.sln
dotnet build PsychologyApp.sln
```

Конфигурация: `PsychologyApp.Presentation/appsettings.json` (см. `appsettings.Development.json.example`). Цитаты хранятся локально в SQLite; внешний API не используется.

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
- Новый UI — сначала `xmlns:ui` компоненты из `UI/Components`
- SQL только с параметрами (`@name`)

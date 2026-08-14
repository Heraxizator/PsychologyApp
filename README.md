<div align="center">

# 🧠 Psychology App

> **Clean Architecture • SOLID Principles • Enterprise Patterns**

<table>
<tr>
<td>

### 🎯 Architecture
- Clean Architecture
- MVVM Pattern
- Repository Pattern
- DDD Approach

</td>
<td>

### 🛠️ Stack
- .NET 10.0
- MAUI 10.0
- Dapper + Microsoft.Data.Sqlite
- SQLite

</td>
<td>

### 📱 Platforms
- Android 5.0+
- iOS 11.0+
- macOS 13.1+
- Windows 10+

</td>
</tr>
</table>

```ascii
╔══════════════════════════════════════════════════════════════╗
║  4-Layer Architecture │ Async/Await │ Dependency Injection  ║
║  Dapper + SQLite │ MVVM + Data Binding │ Unit Testing       ║
╚══════════════════════════════════════════════════════════════╝
```

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/maui)
[![Dapper](https://img.shields.io/badge/Dapper-2.x-1E88E5?style=flat-square)](https://github.com/DapperLib/Dapper)
[![SQLite](https://img.shields.io/badge/SQLite-3.x-003B57?style=flat-square&logo=sqlite)](https://www.sqlite.org/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

</div>

---

## 🔗 Project Meta

- Release notes: [`CHANGELOG.md`](CHANGELOG.md)
- Contribution guide: [`CONTRIBUTING.md`](CONTRIBUTING.md)

---

## 📋 Содержание

- [Описание проекта](#-описание-проекта)
- [Архитектура](#-архитектура)
- [Технологический стек](#-технологический-стек)
- [Структура проекта](#-структура-проекта)
- [Слои приложения](#-слои-приложения)
- [Паттерны проектирования](#-паттерны-проектирования)
- [MVVM Implementation](#-mvvm-implementation)
- [Dependency Injection](#-dependency-injection)
- [Data Persistence](#-data-persistence)
- [Custom Controls](#-custom-controls)
- [Navigation System](#-navigation-system)
- [Примеры кода](#-примеры-кода)
- [Установка и запуск](#-установка-и-запуск)
- [Тестирование](#-тестирование)
- [Лицензия](#-лицензия)

---

## 📖 Описание проекта

Psychology App — кросс-платформенное мобильное приложение, демонстрирующее современный подход к разработке на .NET MAUI. Проект построен с использованием Clean Architecture, SOLID принципов и enterprise-level паттернов проектирования.

### Технические характеристики

- **Архитектурный стиль**: Clean Architecture с четким разделением на 4 слоя
- **Паттерн представления**: MVVM (Model-View-ViewModel)
- **Dependency Management**: Custom Service Locator с поддержкой scoped dependencies
- **Персистентность**: Dapper + SQLite
- **Платформы**: Android (API 21+), iOS (11.0+), MacCatalyst (13.1+)
- **UI Framework**: .NET MAUI с XAML разметкой
- **Принципы**: SOLID, DRY, KISS, YAGNI, GRASP

### Ключевые технические особенности

- ✅ Полная офлайн-работа с локальной SQLite базой данных
- ✅ Repository + Unit of Work паттерны для data access layer
- ✅ DTO маппинг между слоями
- ✅ Value Objects и Entity в Domain Layer
- ✅ Custom Controls с platform-specific renderers
- ✅ Асинхронная обработка данных (async/await)
- ✅ Модульная архитектура с независимыми feature modules
- ✅ Specification Pattern для бизнес-правил

---

## 🏗️ Архитектура

Проект реализует **Clean Architecture** (Onion Architecture) с соблюдением принципа инверсии зависимостей. Внутренние слои не зависят от внешних.

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│         MAUI Views, ViewModels, Custom Controls             │
│         Зависимости: Application Layer                       │
└────────────────────────────┬────────────────────────────────┘
                             │ IService interfaces
┌────────────────────────────▼────────────────────────────────┐
│                   Application Layer                          │
│    Business Logic, Services, DTOs, Mappers                  │
│    Зависимости: Domain + Infrastructure interfaces          │
└────────────────────────────┬────────────────────────────────┘
                             │ IRepository interfaces
┌────────────────────────────▼────────────────────────────────┐
│                  Infrastructure Layer                        │
│    EF Core, Repositories, DbContext, External APIs          │
│    Зависимости: Domain Layer                                 │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│                     Domain Layer                             │
│    Entities, Value Objects, Specifications                  │
│    Зависимости: нет (ядро системы)                          │
└─────────────────────────────────────────────────────────────┘
```

### Принцип Dependency Rule

**Domain Layer** (ядро) не имеет зависимостей — содержит только бизнес-логику и доменные модели.

**Infrastructure Layer** зависит только от Domain — реализует паттерны доступа к данным.

**Application Layer** оркестрирует бизнес-логику, используя Domain модели и Infrastructure через интерфейсы.

**Presentation Layer** знает только об Application сервисах через интерфейсы.

### Cross-Cutting Concerns

```
ServiceLocator (DI Container)
     │
     ├─ ViewModels Registration
     ├─ Services Registration  
     ├─ Repositories Registration
     └─ DbContext Registration

Navigation Service
     │
     └─ Shell-based Navigation

Exception Handling
     │
     ├─ Domain Exceptions
     ├─ Application Exceptions
     └─ Infrastructure Exceptions
```

---

## 💻 Технологический стек

### Core Framework

| Технология | Версия | Использование |
|-----------|--------|--------------|
| **.NET** | 9.0 | Runtime, BCL, Language features (C# 12) |
| **.NET MAUI** | 9.0.22 | Cross-platform UI framework, Shell navigation |
| **C# 12** | Latest | Pattern matching, record types, init-only setters |

### Data Access Layer

| Технология | Версия | Использование |
|-----------|--------|--------------|
| **Entity Framework Core** | 9.0.0 | ORM, DbContext, Migrations, Change Tracking |
| **SQLite** | 9.0.0 | Embedded relational database (Microsoft.EntityFrameworkCore.Sqlite) |
| **EF Core Design** | 9.0.0 | Design-time tools for migrations |

**EF Core Features Used:**
- Code-First approach with Fluent API
- Async operations (ToListAsync, FirstOrDefaultAsync)
- LINQ queries
- Migration management
- Lazy/Eager loading with Include()

### UI Framework

| Технология | Версия | Использование |
|-----------|--------|--------------|
| **XAML** | - | Declarative UI markup |
| **CommunityToolkit.Maui** | 10.0.0 | Animations, Behaviors, Converters |
| **MauiIcons.Material** | 4.0.0 | Material Design icon library |
| **MvvmHelpers** | 1.6.2 | BaseViewModel, ObservableRangeCollection |

**MAUI Features Used:**
- Shell navigation with routing
- Data Binding (OneWay, TwoWay, OneWayToSource)
- MVVM pattern with INotifyPropertyChanged
- Custom Controls and Handlers
- Platform-specific implementations
- ResourceDictionary for theming

### Testing

| Технология | Версия | Использование |
|-----------|--------|--------------|
| **xUnit** | Latest | Unit testing framework |
| **Moq** | Latest | Mocking framework (planned) |
| **FluentAssertions** | Latest | Assertion library (planned) |

### Development Tools

- **IDE**: Visual Studio 2022 (17.8+) / JetBrains Rider
- **Version Control**: Git
- **Package Manager**: NuGet
- **Build System**: MSBuild / dotnet CLI
- **Workloads**: 
  - .NET Multi-platform App UI development
  - Mobile development with .NET

### Supported Platforms

| Платформа | Target Framework | Min Version | Architecture |
|-----------|-----------------|-------------|--------------|
| **Android** | net9.0-android | 5.0 (API 21) | arm64-v8a, armeabi-v7a, x86_64 |
| **iOS** | net9.0-ios | 11.0+ | arm64, x86_64 (simulator) |
| **MacCatalyst** | net9.0-maccatalyst | 13.1+ | x86_64, arm64 (Apple Silicon) |
| **Windows** | net9.0-windows10.0.19041.0 | Windows 10 (1809+) | x64, x86, arm64 |

### NuGet Packages

```xml
<ItemGroup>
  <!-- MAUI Core -->
  <PackageReference Include="Microsoft.Maui.Controls" Version="9.0.22" />
  <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="9.0.22" />
  
  <!-- Entity Framework -->
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
  
  <!-- UI Libraries -->
  <PackageReference Include="CommunityToolkit.Maui" Version="10.0.0" />
  <PackageReference Include="MauiIcons.Material" Version="4.0.0" />
  
  <!-- MVVM Helpers -->
  <PackageReference Include="Refractored.MvvmHelpers" Version="1.6.2" />
  
  <!-- Testing (Test project) -->
  <PackageReference Include="xunit" Version="2.4.2" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
</ItemGroup>
```

---

## 📁 Структура проекта

```
PsychologyApp/
│
├── 📂 PsychologyApp.Presentation/      # Слой представления
│   ├── 📂 Modules/                     # Модули приложения
│   │   ├── 📂 Practic/                # Практик - техники
│   │   ├── 📂 Tester/                 # Детектор - тесты
│   │   ├── 📂 Physic/                 # Соматик - психосоматика
│   │   ├── 📂 Cleaner/                # Очиститель - аудио
│   │   ├── 📂 Motivator/              # Мотиватор - цитаты
│   │   ├── 📂 Profile/                # Профиль пользователя
│   │   └── 📂 Reviewer/               # Отзывы
│   ├── 📂 Templates/                   # Переиспользуемые компоненты
│   ├── 📂 Controls/                    # Кастомные контролы
│   ├── 📂 ServiceLocator/             # Dependency Injection
│   ├── 📂 Resources/                   # Ресурсы приложения
│   │   ├── 📂 Fonts/                  # Шрифты (Roboto)
│   │   ├── 📂 Images/                 # Изображения
│   │   ├── 📂 Styles/                 # XAML стили
│   │   └── 📂 Raw/                    # Сырые данные
│   └── 📄 MauiProgram.cs              # Точка входа
│
├── 📂 PsychologyApp.Application/       # Слой приложения
│   ├── 📂 Technique/                   # Сервисы техник
│   ├── 📂 Quot/                        # Сервисы цитат
│   ├── 📂 Reason/                      # Сервисы причин
│   ├── 📂 Statistic/                   # Сервисы статистики
│   └── 📂 Base/                        # Базовые интерфейсы
│
├── 📂 PsychologyApp.Infrastructure/    # Слой инфраструктуры
│   ├── 📂 Data/                        # Работа с данными
│   │   ├── 📂 Context/                # EF Core контекст
│   │   └── 📂 Repositories/           # Репозитории
│   ├── 📂 API/                         # API интеграции
│   └── 📂 Extensions/                  # Расширения
│
├── 📂 PsychologyApp.Domain/            # Доменный слой
│   ├── 📂 Technique/                   # Сущность Техника
│   ├── 📂 Quot/                        # Сущность Цитата
│   ├── 📂 Reason/                      # Сущность Причина
│   ├── 📂 Statistic/                   # Сущность Статистика
│   ├── 📂 Colour/                      # Value Objects
│   └── 📂 Base/                        # Базовые классы
│
├── 📂 PsychologyApp.Tests/             # Тесты
│   └── 📂 Domain/                      # Тесты доменной логики
│
├── 📄 PsychologyApp.sln               # Solution файл
└── 📄 README.md                        # Этот файл
```

---

## 🛠️ Установка и запуск

### Предварительные требования

Перед началом работы убедитесь, что у вас установлено:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (17.8 или новее) с рабочими нагрузками:
  - **.NET Multi-platform App UI development**
  - **Mobile development with .NET**
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Шаги установки

1. **Клонируйте репозиторий**

```bash
git clone https://github.com/yourusername/PsychologyApp.git
cd PsychologyApp
```

2. **Откройте решение**

Откройте файл `PsychologyApp.sln` в Visual Studio 2022

3. **Восстановите NuGet пакеты**

```bash
dotnet restore
```

4. **Выберите целевую платформу**

В Visual Studio выберите целевую платформу в панели инструментов:
- `net9.0-android` для Android
- `net9.0-ios` для iOS
- `net9.0-maccatalyst` для MacCatalyst

5. **Запустите приложение**

Нажмите `F5` или кнопку "Run" для сборки и запуска приложения

### Сборка для релиза

#### Android

```bash
dotnet publish -f net9.0-android -c Release
```

APK файл будет находиться в:
```
PsychologyApp.Presentation/bin/Release/net9.0-android/publish/
```

#### iOS

```bash
dotnet publish -f net9.0-ios -c Release
```

#### MacCatalyst

```bash
dotnet publish -f net9.0-maccatalyst -c Release
```

---

## 🧩 Слои приложения

### 1️⃣ Domain Layer (`PsychologyApp.Domain`)

**Назначение**: Ядро приложения, содержит бизнес-логику и доменные модели без внешних зависимостей.

**Структура:**

```
Domain/
├── Base/
│   ├── Entity.cs                    # Базовый класс для всех сущностей
│   ├── ValueObject.cs               # Базовый класс для Value Objects
│   ├── Specification.cs             # Specification Pattern
│   └── Constants/                   # Доменные константы
├── Technique/
│   └── Technique.cs                 # Сущность "Техника"
├── Quot/
│   └── Quot.cs                      # Сущность "Цитата"
├── Reason/
│   └── Reason.cs                    # Сущность "Причина"
├── Statistic/
│   └── Statistic.cs                 # Сущность "Статистика"
└── Colour/
    ├── ValueObjects/                # Value Objects для цветов
    ├── Enums/                       # Перечисления
    └── Exceptions/                  # Доменные исключения
```

**Ключевые концепции:**

- **Entity**: Объекты с идентичностью, имеют уникальный ID
- **Value Object**: Неизменяемые объекты, определяемые своими значениями
- **Specification**: Инкапсулирует бизнес-правила в переиспользуемых объектах
- **Domain Exceptions**: Специфичные для домена исключения

### 2️⃣ Infrastructure Layer (`PsychologyApp.Infrastructure`)

**Назначение**: Реализация технических деталей доступа к данным и внешним сервисам.

**Структура:**

```
Infrastructure/
├── Data/
│   ├── Context/
│   │   ├── ApplicationDbContext.cs        # EF Core DbContext
│   │   └── ApplicationDbContextFactory.cs # Design-time factory
│   └── Repositories/
│       ├── IRepository.cs                  # Generic repository interface
│       ├── Repository.cs                   # Generic repository implementation
│       ├── IUnitOfWork.cs                  # Unit of Work interface
│       ├── TechniqueRepository.cs          # Specialized repository
│       ├── QuotRepository.cs
│       └── ReasonRepository.cs
├── API/
│   ├── Base/
│   │   └── ApiClient.cs                    # HTTP client wrapper
│   └── Quots/
│       ├── IQuotApiService.cs              # API service interface
│       └── QuotApiService.cs               # API implementation
└── Extensions/
    └── ReasonExtension.cs                  # Extension methods
```

**Технологии:**

- **Entity Framework Core 9.0**: ORM для работы с SQLite
- **Repository Pattern**: Абстракция над EF Core
- **Unit of Work**: Управление транзакциями

### 3️⃣ Application Layer (`PsychologyApp.Application`)

**Назначение**: Оркестрация бизнес-логики, координация между UI и Infrastructure.

**Структура:**

```
Application/
├── Base/
│   └── IAppService.cs                # Базовый интерфейс для сервисов
├── Technique/
│   ├── ITechniqueService.cs          # Interface
│   ├── TechniqueService.cs           # Implementation
│   ├── TechniqueDTO.cs               # Data Transfer Object
│   ├── TechniqueMapper.cs            # Entity ↔ DTO mapping
│   └── Exceptions/
│       └── TechniqueNotFoundException.cs
├── Quot/
│   ├── IQuotService.cs
│   ├── QuotService.cs
│   ├── QuotDTO.cs
│   ├── QuotMapper.cs
│   └── Exceptions/
├── Reason/
│   ├── IReasonService.cs
│   ├── ReasonService.cs
│   ├── ReasonDTO.cs
│   └── ReasonMapper.cs
└── Statistic/
    ├── IStatisticService.cs
    ├── StatisticService.cs
    ├── StatisticDTO.cs
    └── StatisticMapper.cs
```

**Паттерны:**

- **Service Layer**: Инкапсулирует бизнес-логику
- **DTO Pattern**: Разделяет доменные модели и модели представления
- **Mapper Pattern**: Конвертация между Entity и DTO

### 4️⃣ Presentation Layer (`PsychologyApp.Presentation`)

**Назначение**: UI и взаимодействие с пользователем на базе MVVM.

**Структура:**

```
Presentation/
├── Modules/                          # Feature-based организация
│   ├── BaseViewModel.cs              # Базовый класс для ViewModels
│   ├── MainViewModel.cs              # Главная ViewModel
│   ├── Practic/                      # Модуль техник
│   │   ├── Collection/
│   │   ├── Constructor/
│   │   └── Techniques/
│   ├── Tester/                       # Модуль тестирования
│   ├── Physic/                       # Модуль психосоматики
│   ├── Cleaner/                      # Аудиоплеер
│   ├── Motivator/                    # Цитаты
│   ├── Profile/                      # Профиль
│   └── Reviewer/                     # Отзывы
├── Controls/                         # Custom UI Controls
│   ├── ExtendedLabel.cs
│   ├── LocalEditor.cs
│   ├── LocalEntry.cs
│   └── LocalFrame.cs
├── Templates/                        # Переиспользуемые UI компоненты
├── ServiceLocator/                   # Dependency Injection
│   ├── ServiceLocator.cs
│   ├── Dialog/
│   │   ├── IDialogService.cs
│   │   └── DialogService.cs
│   └── Toast/
│       ├── IToastService.cs
│       └── ToastService.cs
├── Resources/                        # Ресурсы приложения
│   ├── Styles/                       # XAML стили
│   ├── Fonts/                        # Шрифты
│   ├── Images/                       # Изображения
│   └── Raw/                          # Данные
└── MauiProgram.cs                    # Точка входа, регистрация сервисов
```

**MVVM реализация:**

- **Views (XAML)**: Декларативная UI разметка
- **ViewModels**: Presentation Logic, data binding
- **Models**: Application Services (через DI)

---

## 🎨 Паттерны проектирования

### Архитектурные паттерны

#### Clean Architecture

Строгое разделение на слои с соблюдением Dependency Rule:

```
Domain (Core) ← Infrastructure ← Application ← Presentation
```

Каждый слой имеет четко определенную ответственность и зависит только от внутренних слоев.

#### MVVM (Model-View-ViewModel)

```
View (XAML) ←→ ViewModel ←→ Model (Services)
      ↓              ↓
  Data Binding   INotifyPropertyChanged
```

**Преимущества:**
- Separation of Concerns
- Testability (ViewModels можно тестировать без UI)
- Data Binding для автоматического обновления UI

#### Repository Pattern

Абстракция над механизмом доступа к данным:

```csharp
// Generic repository interface
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Implementation with EF Core
public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}
```

#### Unit of Work Pattern

Управление транзакциями и координация работы нескольких репозиториев:

```csharp
public interface IUnitOfWork : IDisposable
{
    ITechniqueRepository Techniques { get; }
    IQuotRepository Quots { get; }
    IReasonRepository Reasons { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
```

### Доменные паттерны

#### Entity Pattern

Объекты с уникальной идентичностью:

```csharp
// Base Entity
public abstract class Entity
{
    public long Id { get; protected set; }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
            
        if (ReferenceEquals(this, other))
            return true;
            
        return Id == other.Id;
    }
    
    public override int GetHashCode() => Id.GetHashCode();
}

// Domain Entity
public class Technique : Entity
{
    public long TechniqueId { get; private set; }
    public string Number { get; private set; }
    public string Date { get; private set; }
    public string Header { get; private set; }
    public string Description { get; private set; }
    
    // Factory method
    public static Technique Create(
        long id, 
        string number, 
        string date, 
        string header, 
        string description)
    {
        return new Technique
        {
            TechniqueId = id,
            Number = number,
            Date = date,
            Header = header,
            Description = description
        };
    }
}
```

#### Value Object Pattern

Неизменяемые объекты, определяемые значениями:

```csharp
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
            
        var valueObject = (ValueObject)obj;
        return GetEqualityComponents()
            .SequenceEqual(valueObject.GetEqualityComponents());
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
    }
}

// Usage example
public class Colour : ValueObject
{
    public string Value { get; private set; }
    
    private Colour(string value)
    {
        Value = value;
    }
    
    public static Colour Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Color value cannot be empty");
            
        return new Colour(value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

#### Specification Pattern

Инкапсуляция бизнес-правил:

```csharp
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    
    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }
    
    public Specification<T> And(Specification<T> specification)
    {
        return new AndSpecification<T>(this, specification);
    }
    
    public Specification<T> Or(Specification<T> specification)
    {
        return new OrSpecification<T>(this, specification);
    }
}

// Example usage
public class CompletedTechniqueSpecification : Specification<Technique>
{
    public override Expression<Func<Technique, bool>> ToExpression()
    {
        return technique => technique.IsCompleted == true;
    }
}
```

### Application Layer Patterns

#### Service Layer Pattern

```csharp
// Service interface
public interface ITechniqueService : IAppService
{
    Task<IEnumerable<TechniqueDTO>> GetAllTechniquesAsync();
    Task<TechniqueDTO?> GetTechniqueByIdAsync(long id);
    Task<TechniqueDTO> CreateTechniqueAsync(TechniqueDTO dto);
    Task UpdateTechniqueAsync(TechniqueDTO dto);
    Task DeleteTechniqueAsync(long id);
}

// Service implementation
public class TechniqueService : ITechniqueService
{
    private readonly IRepository<Technique> _repository;
    
    public TechniqueService(IRepository<Technique> repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<TechniqueDTO>> GetAllTechniquesAsync()
    {
        var techniques = await _repository.GetAllAsync();
        return techniques.Select(TechniqueMapper.ToDTO);
    }
    
    public async Task<TechniqueDTO?> GetTechniqueByIdAsync(long id)
    {
        var technique = await _repository.GetByIdAsync(id);
        
        if (technique == null)
            throw new TechniqueNotFoundException(id);
            
        return TechniqueMapper.ToDTO(technique);
    }
}
```

#### Mapper Pattern

```csharp
public static class TechniqueMapper
{
    // Entity to DTO
    public static TechniqueDTO ToDTO(Technique technique)
    {
        return new TechniqueDTO
        {
            Id = technique.TechniqueId,
            Number = technique.Number,
            Date = technique.Date,
            Header = technique.Header,
            Description = technique.Description,
            Subject = technique.Subject,
            Author = technique.Author,
            Algorithm = technique.Algorithm,
            Image = technique.Image
        };
    }
    
    // DTO to Entity
    public static Technique ToEntity(TechniqueDTO dto)
    {
        return Technique.Create(
            id: dto.Id,
            number: dto.Number,
            date: dto.Date,
            header: dto.Header,
            description: dto.Description,
            subject: dto.Subject,
            author: dto.Author,
            algorithm: dto.Algorithm,
            image: dto.Image
        );
    }
}
```

### SOLID Principles

#### Single Responsibility Principle (SRP)

Каждый класс отвечает только за одну задачу:

```csharp
// ❌ Bad: God Object
public class TechniqueManager
{
    public void SaveToDatabase() { }
    public void SendEmail() { }
    public void GenerateReport() { }
    public void ValidateData() { }
}

// ✅ Good: Separated responsibilities
public class TechniqueRepository { /* Data access */ }
public class EmailService { /* Email sending */ }
public class ReportGenerator { /* Report generation */ }
public class TechniqueValidator { /* Validation */ }
```

#### Open/Closed Principle (OCP)

Открыт для расширения, закрыт для модификации:

```csharp
// Strategy pattern for extensibility
public interface IExportStrategy
{
    Task ExportAsync(IEnumerable<TechniqueDTO> data);
}

public class JsonExportStrategy : IExportStrategy
{
    public async Task ExportAsync(IEnumerable<TechniqueDTO> data)
    {
        // JSON export implementation
    }
}

public class CsvExportStrategy : IExportStrategy
{
    public async Task ExportAsync(IEnumerable<TechniqueDTO> data)
    {
        // CSV export implementation
    }
}
```

#### Liskov Substitution Principle (LSP)

Подтипы должны быть заменяемы базовыми типами:

```csharp
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Derived ViewModels can be used wherever BaseViewModel is expected
public class TechniqueViewModel : BaseViewModel { }
public class QuotViewModel : BaseViewModel { }
```

#### Interface Segregation Principle (ISP)

Клиенты не должны зависеть от интерфейсов, которые они не используют:

```csharp
// ❌ Bad: Fat interface
public interface IRepository
{
    Task Add();
    Task Update();
    Task Delete();
    Task BulkInsert();
    Task BulkUpdate();
    Task BulkDelete();
}

// ✅ Good: Segregated interfaces
public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
}

public interface IWriteRepository<T>
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

#### Dependency Inversion Principle (DIP)

Зависимость от абстракций, а не от конкретных реализаций:

```csharp
// ❌ Bad: Direct dependency on concrete class
public class TechniqueViewModel
{
    private readonly TechniqueService _service = new TechniqueService();
}

// ✅ Good: Dependency on abstraction
public class TechniqueViewModel
{
    private readonly ITechniqueService _service;
    
    public TechniqueViewModel(ITechniqueService service)
    {
        _service = service; // Injected through constructor
    }
}
```

---

## 🔷 MVVM Implementation

### BaseViewModel

Базовый класс для всех ViewModels с поддержкой `INotifyPropertyChanged`:

```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }
    
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    protected bool SetProperty<T>(
        ref T backingStore, 
        T value,
        [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;
            
        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

### ViewModel с Commands

```csharp
public class TechniqueCollectionViewModel : BaseViewModel
{
    private readonly ITechniqueService _techniqueService;
    private ObservableCollection<TechniqueDTO> _techniques;
    
    public ObservableCollection<TechniqueDTO> Techniques
    {
        get => _techniques;
        set => SetProperty(ref _techniques, value);
    }
    
    public ICommand LoadTechniquesCommand { get; }
    public ICommand SelectTechniqueCommand { get; }
    public ICommand DeleteTechniqueCommand { get; }
    
    public TechniqueCollectionViewModel(ITechniqueService techniqueService)
    {
        _techniqueService = techniqueService;
        _techniques = new ObservableCollection<TechniqueDTO>();
        
        LoadTechniquesCommand = new Command(async () => await LoadTechniquesAsync());
        SelectTechniqueCommand = new Command<TechniqueDTO>(async (technique) => 
            await OnTechniqueSelectedAsync(technique));
        DeleteTechniqueCommand = new Command<long>(async (id) => 
            await DeleteTechniqueAsync(id));
    }
    
    private async Task LoadTechniquesAsync()
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            
            var techniques = await _techniqueService.GetAllTechniquesAsync();
            Techniques.Clear();
            
            foreach (var technique in techniques)
            {
                Techniques.Add(technique);
            }
        }
        catch (Exception ex)
        {
            // Handle exception
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task OnTechniqueSelectedAsync(TechniqueDTO technique)
    {
        if (technique == null) return;
        
        await Shell.Current.GoToAsync($"techniqueDetail?id={technique.Id}");
    }
    
    private async Task DeleteTechniqueAsync(long id)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Confirm", 
            "Delete this technique?", 
            "Yes", 
            "No");
            
        if (!confirm) return;
        
        await _techniqueService.DeleteTechniqueAsync(id);
        await LoadTechniquesAsync();
    }
}
```

### Data Binding в XAML

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="PsychologyApp.Presentation.Modules.Practic.CollectionPage"
             Title="{Binding Title}">
    
    <Grid>
        <!-- Loading Indicator -->
        <ActivityIndicator IsRunning="{Binding IsBusy}"
                          IsVisible="{Binding IsBusy}"
                          VerticalOptions="Center"
                          HorizontalOptions="Center"/>
        
        <!-- Collection View -->
        <CollectionView ItemsSource="{Binding Techniques}"
                       SelectionMode="Single"
                       SelectedItem="{Binding SelectedTechnique}"
                       SelectionChangedCommand="{Binding SelectTechniqueCommand}"
                       SelectionChangedCommandParameter="{Binding SelectedTechnique}">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <Grid Padding="10">
                        <Frame>
                            <StackLayout>
                                <Label Text="{Binding Header}" 
                                      FontSize="18" 
                                      FontAttributes="Bold"/>
                                <Label Text="{Binding Description}" 
                                      FontSize="14"/>
                            </StackLayout>
                        </Frame>
                    </Grid>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </Grid>
</ContentPage>
```

---

## 🔌 Dependency Injection

### Custom Service Locator

```csharp
public class ServiceLocator
{
    private static ServiceLocator? _instance;
    private readonly Dictionary<Type, object> _services;
    private readonly Dictionary<Type, Func<object>> _factories;
    
    public static ServiceLocator Instance => _instance ??= new ServiceLocator();
    
    private ServiceLocator()
    {
        _services = new Dictionary<Type, object>();
        _factories = new Dictionary<Type, Func<object>>();
    }
    
    // Register singleton
    public void Register<T>(T implementation) where T : class
    {
        _services[typeof(T)] = implementation;
    }
    
    // Register transient (factory)
    public void Register<T>(Func<T> factory) where T : class
    {
        _factories[typeof(T)] = () => factory();
    }
    
    // Resolve service
    public T Resolve<T>() where T : class
    {
        var type = typeof(T);
        
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }
        
        if (_factories.TryGetValue(type, out var factory))
        {
            return (T)factory();
        }
        
        throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
    }
    
    // Check if service is registered
    public bool IsRegistered<T>() where T : class
    {
        return _services.ContainsKey(typeof(T)) || _factories.ContainsKey(typeof(T));
    }
}
```

### Service Registration в MauiProgram.cs

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
            });
        
        // Register DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Filename={Path.Combine(FileSystem.AppDataDirectory, "psychology.db")}"));
        
        // Register Repositories
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<ITechniqueRepository, TechniqueRepository>();
        builder.Services.AddScoped<IQuotRepository, QuotRepository>();
        builder.Services.AddScoped<IReasonRepository, ReasonRepository>();
        
        // Register Services
        builder.Services.AddScoped<ITechniqueService, TechniqueService>();
        builder.Services.AddScoped<IQuotService, QuotService>();
        builder.Services.AddScoped<IReasonService, ReasonService>();
        builder.Services.AddScoped<IStatisticService, StatisticService>();
        
        // Register ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<TechniqueCollectionViewModel>();
        builder.Services.AddTransient<QuoteViewModel>();
        
        // Register Platform Services
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IToastService, ToastService>();
        
        return builder.Build();
    }
}
```

### Constructor Injection в Pages

```csharp
public partial class TechniqueCollectionPage : ContentPage
{
    public TechniqueCollectionPage(TechniqueCollectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is TechniqueCollectionViewModel vm)
        {
            vm.LoadTechniquesCommand.Execute(null);
        }
    }
}
```

---

## 💾 Data Persistence

### Entity Framework Core Configuration

#### DbContext Setup

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Technique> Techniques { get; set; }
    public DbSet<Quot> Quots { get; set; }
    public DbSet<Reason> Reasons { get; set; }
    public DbSet<Statistic> Statistics { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Technique entity
        modelBuilder.Entity<Technique>(entity =>
        {
            entity.ToTable("Techniques");
            entity.HasKey(e => e.TechniqueId);
            
            entity.Property(e => e.Number)
                .IsRequired()
                .HasMaxLength(10);
                
            entity.Property(e => e.Header)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
        });
        
        // Configure Quot entity
        modelBuilder.Entity<Quot>(entity =>
        {
            entity.ToTable("Quots");
            entity.HasKey(e => e.QuotId);
            
            entity.Property(e => e.Text)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Author)
                .HasMaxLength(100);
        });
        
        // Seed initial data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Technique>().HasData(
            Technique.Create(1, "001", "2024-01-01", "Sample Technique", "Description")
        );
    }
}
```

#### Design-Time DbContext Factory

```csharp
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite("Data Source=psychology.db");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
```

### Repository Implementation

```csharp
public class TechniqueRepository : Repository<Technique>, ITechniqueRepository
{
    public TechniqueRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Technique>> GetCompletedTechniquesAsync()
    {
        return await _dbSet
            .Where(t => t.IsCompleted)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Technique>> GetTechniquesByAuthorAsync(string author)
    {
        return await _dbSet
            .Where(t => t.Author == author)
            .ToListAsync();
    }
    
    public async Task<Technique?> GetTechniqueWithStatisticsAsync(long id)
    {
        return await _dbSet
            .Include(t => t.Statistics)
            .FirstOrDefaultAsync(t => t.TechniqueId == id);
    }
}
```

### Async Data Operations

```csharp
public class TechniqueService : ITechniqueService
{
    private readonly ITechniqueRepository _repository;
    
    public async Task<TechniqueDTO> CreateTechniqueAsync(TechniqueDTO dto)
    {
        var technique = TechniqueMapper.ToEntity(dto);
        
        await _repository.AddAsync(technique);
        await _repository.SaveChangesAsync();
        
        return TechniqueMapper.ToDTO(technique);
    }
    
    public async Task<IEnumerable<TechniqueDTO>> SearchTechniquesAsync(string searchTerm)
    {
        var techniques = await _repository.FindAsync(t => 
            t.Header.Contains(searchTerm) || 
            t.Description.Contains(searchTerm));
            
        return techniques.Select(TechniqueMapper.ToDTO);
    }
}
```

---

## 🎛️ Custom Controls

### ExtendedLabel

Label с дополнительными возможностями:

```csharp
public class ExtendedLabel : Label
{
    public static readonly BindableProperty MaxLinesProperty =
        BindableProperty.Create(
            nameof(MaxLines),
            typeof(int),
            typeof(ExtendedLabel),
            default(int));
    
    public int MaxLines
    {
        get => (int)GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }
    
    public static readonly BindableProperty IsHtmlProperty =
        BindableProperty.Create(
            nameof(IsHtml),
            typeof(bool),
            typeof(ExtendedLabel),
            false);
    
    public bool IsHtml
    {
        get => (bool)GetValue(IsHtmlProperty);
        set => SetValue(IsHtmlProperty, value);
    }
}
```

### LocalEntry

Entry с локализованным placeholder:

```csharp
public class LocalEntry : Entry
{
    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(
            nameof(BorderColor),
            typeof(Color),
            typeof(LocalEntry),
            Colors.Gray);
    
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }
    
    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(
            nameof(CornerRadius),
            typeof(double),
            typeof(LocalEntry),
            5.0);
    
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
}
```

### LocalFrame

Frame с расширенными возможностями стилизации:

```csharp
public class LocalFrame : Frame
{
    public static readonly BindableProperty ElevationProperty =
        BindableProperty.Create(
            nameof(Elevation),
            typeof(float),
            typeof(LocalFrame),
            4.0f);
    
    public float Elevation
    {
        get => (float)GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }
    
    public static readonly BindableProperty RippleColorProperty =
        BindableProperty.Create(
            nameof(RippleColor),
            typeof(Color),
            typeof(LocalFrame),
            Colors.LightGray);
    
    public Color RippleColor
    {
        get => (Color)GetValue(RippleColorProperty);
        set => SetValue(RippleColorProperty, value);
    }
}
```

---

## 🧭 Navigation System

### Shell-Based Navigation

#### AppShell Configuration

```csharp
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes
        Routing.RegisterRoute("techniqueDetail", typeof(TechniqueDetailPage));
        Routing.RegisterRoute("techniqueEdit", typeof(TechniqueEditPage));
        Routing.RegisterRoute("quotDetail", typeof(QuotDetailPage));
        Routing.RegisterRoute("testDetail", typeof(TestDetailPage));
    }
}
```

#### AppShell XAML

```xml
<Shell xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       xmlns:local="clr-namespace:PsychologyApp.Presentation.Modules"
       x:Class="PsychologyApp.Presentation.AppShell">
    
    <TabBar>
        <ShellContent Title="Практик"
                     Icon="practic_icon.png"
                     Route="practic"
                     ContentTemplate="{DataTemplate local:PracticPage}"/>
        
        <ShellContent Title="Детектор"
                     Icon="tester_icon.png"
                     Route="tester"
                     ContentTemplate="{DataTemplate local:TesterPage}"/>
        
        <ShellContent Title="Соматик"
                     Icon="physic_icon.png"
                     Route="physic"
                     ContentTemplate="{DataTemplate local:PhysicPage}"/>
        
        <ShellContent Title="Мотиватор"
                     Icon="motivator_icon.png"
                     Route="motivator"
                     ContentTemplate="{DataTemplate local:MotivatorPage}"/>
        
        <ShellContent Title="Профиль"
                     Icon="profile_icon.png"
                     Route="profile"
                     ContentTemplate="{DataTemplate local:ProfilePage}"/>
    </TabBar>
</Shell>
```

### Navigation Service

```csharp
public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, Dictionary<string, object> parameters);
    Task GoBackAsync();
    Task PopToRootAsync();
}

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }
    
    public async Task NavigateToAsync(string route, Dictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }
    
    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
    
    public async Task PopToRootAsync()
    {
        await Shell.Current.GoToAsync("//");
    }
}
```

### Passing Parameters

```csharp
// In ViewModel - Navigation with parameters
public async Task NavigateToDetailAsync(long techniqueId)
{
    var parameters = new Dictionary<string, object>
    {
        { "techniqueId", techniqueId }
    };
    
    await Shell.Current.GoToAsync("techniqueDetail", parameters);
}

// In Detail Page - Receiving parameters
[QueryProperty(nameof(TechniqueId), "techniqueId")]
public partial class TechniqueDetailPage : ContentPage
{
    private long _techniqueId;
    
    public long TechniqueId
    {
        get => _techniqueId;
        set
        {
            _techniqueId = value;
            LoadTechnique(value);
        }
    }
    
    private async void LoadTechnique(long id)
    {
        if (BindingContext is TechniqueDetailViewModel vm)
        {
            await vm.LoadTechniqueAsync(id);
        }
    }
}
```

---

## 📝 Примеры кода

### Complete Feature Module Example

Полный пример реализации модуля "Techniques":

#### 1. Domain Entity

```csharp
// Domain/Technique/Technique.cs
public class Technique : Entity
{
    public long TechniqueId { get; private set; }
    public string Number { get; private set; }
    public string Date { get; private set; }
    public string Header { get; private set; }
    public string Description { get; private set; }
    public string Subject { get; private set; }
    public string Author { get; private set; }
    public string Algorithm { get; private set; }
    public string Image { get; private set; }
    public bool IsCompleted { get; private set; }
    
    private Technique() { }
    
    public static Technique Create(
        long id,
        string number,
        string date,
        string header,
        string description,
        string subject = "",
        string author = "",
        string algorithm = "",
        string image = "")
    {
        if (string.IsNullOrWhiteSpace(header))
            throw new ArgumentException("Header cannot be empty", nameof(header));
        
        return new Technique
        {
            TechniqueId = id,
            Number = number,
            Date = date,
            Header = header,
            Description = description,
            Subject = subject,
            Author = author,
            Algorithm = algorithm,
            Image = image,
            IsCompleted = false
        };
    }
    
    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }
    
    public void Update(string header, string description, string algorithm)
    {
        if (string.IsNullOrWhiteSpace(header))
            throw new ArgumentException("Header cannot be empty", nameof(header));
        
        Header = header;
        Description = description;
        Algorithm = algorithm;
    }
}
```

#### 2. Repository Interface & Implementation

```csharp
// Infrastructure/Data/Repositories/ITechniqueRepository.cs
public interface ITechniqueRepository : IRepository<Technique>
{
    Task<IEnumerable<Technique>> GetCompletedTechniquesAsync();
    Task<IEnumerable<Technique>> GetTechniquesByAuthorAsync(string author);
    Task<Technique?> GetByNumberAsync(string number);
}

// Infrastructure/Data/Repositories/TechniqueRepository.cs
public class TechniqueRepository : Repository<Technique>, ITechniqueRepository
{
    public TechniqueRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Technique>> GetCompletedTechniquesAsync()
    {
        return await _dbSet
            .Where(t => t.IsCompleted)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Technique>> GetTechniquesByAuthorAsync(string author)
    {
        return await _dbSet
            .Where(t => t.Author.Contains(author))
            .ToListAsync();
    }
    
    public async Task<Technique?> GetByNumberAsync(string number)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Number == number);
    }
}
```

#### 3. Service Layer

```csharp
// Application/Technique/ITechniqueService.cs
public interface ITechniqueService : IAppService
{
    Task<IEnumerable<TechniqueDTO>> GetAllTechniquesAsync();
    Task<TechniqueDTO?> GetTechniqueByIdAsync(long id);
    Task<TechniqueDTO> CreateTechniqueAsync(TechniqueDTO dto);
    Task UpdateTechniqueAsync(TechniqueDTO dto);
    Task DeleteTechniqueAsync(long id);
    Task MarkAsCompletedAsync(long id);
}

// Application/Technique/TechniqueService.cs
public class TechniqueService : ITechniqueService
{
    private readonly ITechniqueRepository _repository;
    
    public TechniqueService(ITechniqueRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<TechniqueDTO>> GetAllTechniquesAsync()
    {
        var techniques = await _repository.GetAllAsync();
        return techniques.Select(TechniqueMapper.ToDTO);
    }
    
    public async Task<TechniqueDTO?> GetTechniqueByIdAsync(long id)
    {
        var technique = await _repository.GetByIdAsync(id);
        
        if (technique == null)
            throw new TechniqueNotFoundException(id);
        
        return TechniqueMapper.ToDTO(technique);
    }
    
    public async Task<TechniqueDTO> CreateTechniqueAsync(TechniqueDTO dto)
    {
        var technique = TechniqueMapper.ToEntity(dto);
        await _repository.AddAsync(technique);
        return TechniqueMapper.ToDTO(technique);
    }
    
    public async Task UpdateTechniqueAsync(TechniqueDTO dto)
    {
        var technique = await _repository.GetByIdAsync(dto.Id);
        
        if (technique == null)
            throw new TechniqueNotFoundException(dto.Id);
        
        technique.Update(dto.Header, dto.Description, dto.Algorithm);
        await _repository.UpdateAsync(technique);
    }
    
    public async Task DeleteTechniqueAsync(long id)
    {
        var technique = await _repository.GetByIdAsync(id);
        
        if (technique == null)
            throw new TechniqueNotFoundException(id);
        
        await _repository.DeleteAsync(technique);
    }
    
    public async Task MarkAsCompletedAsync(long id)
    {
        var technique = await _repository.GetByIdAsync(id);
        
        if (technique == null)
            throw new TechniqueNotFoundException(id);
        
        technique.MarkAsCompleted();
        await _repository.UpdateAsync(technique);
    }
}
```

#### 4. ViewModel

```csharp
// Presentation/Modules/Practic/Collection/TechniqueCollectionViewModel.cs
public class TechniqueCollectionViewModel : BaseViewModel
{
    private readonly ITechniqueService _techniqueService;
    private ObservableCollection<TechniqueDTO> _techniques;
    private TechniqueDTO? _selectedTechnique;
    
    public ObservableCollection<TechniqueDTO> Techniques
    {
        get => _techniques;
        set => SetProperty(ref _techniques, value);
    }
    
    public TechniqueDTO? SelectedTechnique
    {
        get => _selectedTechnique;
        set => SetProperty(ref _selectedTechnique, value);
    }
    
    public ICommand LoadTechniquesCommand { get; }
    public ICommand SelectTechniqueCommand { get; }
    public ICommand AddTechniqueCommand { get; }
    public ICommand RefreshCommand { get; }
    
    public TechniqueCollectionViewModel(ITechniqueService techniqueService)
    {
        _techniqueService = techniqueService;
        _techniques = new ObservableCollection<TechniqueDTO>();
        
        Title = "Techniques";
        
        LoadTechniquesCommand = new Command(async () => await LoadTechniquesAsync());
        SelectTechniqueCommand = new Command<TechniqueDTO>(async (t) => await OnTechniqueSelected(t));
        AddTechniqueCommand = new Command(async () => await OnAddTechnique());
        RefreshCommand = new Command(async () => await RefreshTechniquesAsync());
    }
    
    private async Task LoadTechniquesAsync()
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            
            var techniques = await _techniqueService.GetAllTechniquesAsync();
            
            Techniques.Clear();
            foreach (var technique in techniques)
            {
                Techniques.Add(technique);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to load techniques: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private async Task OnTechniqueSelected(TechniqueDTO technique)
    {
        if (technique == null) return;
        
        await Shell.Current.GoToAsync($"techniqueDetail?id={technique.Id}");
    }
    
    private async Task OnAddTechnique()
    {
        await Shell.Current.GoToAsync("techniqueEdit");
    }
    
    private async Task RefreshTechniquesAsync()
    {
        await LoadTechniquesAsync();
    }
}
```

#### 5. View (XAML)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:PsychologyApp.Presentation.Modules.Practic"
             x:Class="PsychologyApp.Presentation.Modules.Practic.TechniqueCollectionPage"
             x:DataType="vm:TechniqueCollectionViewModel"
             Title="{Binding Title}">
    
    <Grid RowDefinitions="Auto,*">
        
        <!-- Toolbar -->
        <Grid Grid.Row="0" Padding="10" BackgroundColor="{StaticResource Primary}">
            <Button Text="Add Technique"
                   Command="{Binding AddTechniqueCommand}"
                   HorizontalOptions="End"/>
        </Grid>
        
        <!-- Content -->
        <RefreshView Grid.Row="1"
                    IsRefreshing="{Binding IsBusy}"
                    Command="{Binding RefreshCommand}">
            
            <CollectionView ItemsSource="{Binding Techniques}"
                           SelectionMode="Single"
                           SelectedItem="{Binding SelectedTechnique}"
                           SelectionChangedCommand="{Binding SelectTechniqueCommand}"
                           SelectionChangedCommandParameter="{Binding SelectedTechnique}">
                
                <CollectionView.EmptyView>
                    <StackLayout Padding="20" VerticalOptions="Center">
                        <Label Text="No techniques found"
                              FontSize="18"
                              HorizontalOptions="Center"/>
                        <Label Text="Add your first technique to get started"
                              FontSize="14"
                              HorizontalOptions="Center"
                              Margin="0,10,0,0"/>
                    </StackLayout>
                </CollectionView.EmptyView>
                
                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="vm:TechniqueDTO">
                        <Grid Padding="10">
                            <Frame CornerRadius="10"
                                  HasShadow="True"
                                  Padding="15">
                                
                                <Grid ColumnDefinitions="Auto,*,Auto">
                                    
                                    <!-- Icon -->
                                    <Image Grid.Column="0"
                                          Source="{Binding Image}"
                                          WidthRequest="50"
                                          HeightRequest="50"
                                          Aspect="AspectFit"
                                          Margin="0,0,15,0"/>
                                    
                                    <!-- Content -->
                                    <StackLayout Grid.Column="1" Spacing="5">
                                        <Label Text="{Binding Header}"
                                              FontSize="18"
                                              FontAttributes="Bold"/>
                                        <Label Text="{Binding Description}"
                                              FontSize="14"
                                              MaxLines="2"
                                              LineBreakMode="TailTruncation"/>
                                        <Label Text="{Binding Author}"
                                              FontSize="12"
                                              TextColor="Gray"/>
                                    </StackLayout>
                                    
                                    <!-- Status -->
                                    <Image Grid.Column="2"
                                          Source="check_icon.png"
                                          WidthRequest="24"
                                          HeightRequest="24"
                                          IsVisible="{Binding IsCompleted}"/>
                                    
                                </Grid>
                            </Frame>
                        </Grid>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>
        </RefreshView>
        
    </Grid>
</ContentPage>
```

---

## 🧪 Тестирование

Проект включает модульные тесты для доменной логики.

### Запуск тестов

```bash
dotnet test
```

### Структура тестов

```
PsychologyApp.Tests/
└── Domain/
    └── Entities/
        ├── TechniqueTests.cs
        ├── QuotTests.cs
        └── ReasonTests.cs
```

### Используемые инструменты

- **xUnit** — фреймворк для тестирования
- **Moq** — мокирование зависимостей
- **FluentAssertions** — выразительные проверки

### Примеры тестов

```csharp
[Fact]
public void Technique_Create_ShouldCreateValidTechnique()
{
    // Arrange & Act
    var technique = Technique.Create(
        id: 1,
        number: "001",
        date: "2024-01-01",
        header: "Test",
        description: "Description",
        subject: "Subject",
        author: "Author",
        algorithm: "Algorithm",
        image: "image.png"
    );
    
    // Assert
    technique.Should().NotBeNull();
    technique.TechniqueId.Should().Be(1);
}
```

---

## 🔧 Technical Roadmap

### Version 1.3 (Current)

**Architecture:**
- ✅ Clean Architecture implementation (4 layers)
- ✅ MVVM pattern with data binding
- ✅ Repository Pattern with EF Core
- ✅ Service Layer with DTOs
- ✅ Custom Service Locator for DI

**Infrastructure:**
- ✅ SQLite database with EF Core 9.0
- ✅ Generic Repository implementation
- ✅ Async/await throughout the stack
- ✅ Unit of Work pattern

**Presentation:**
- ✅ Shell-based navigation
- ✅ Custom Controls (ExtendedLabel, LocalEntry, LocalFrame)
- ✅ XAML styling and theming
- ✅ Platform-specific implementations (Android, iOS, MacCatalyst)

**Testing:**
- ✅ xUnit test framework
- ✅ Domain entity tests

### Version 1.4 (In Progress)

**Architecture Improvements:**
- [ ] Implement CQRS pattern for complex operations
- [ ] Add MediatR for command/query handling
- [ ] Introduce Domain Events
- [ ] Implement specification pattern for queries

**Infrastructure:**
- [ ] Migration to .NET 10
- [ ] Add Redis caching layer
- [ ] Implement data synchronization service
- [ ] Add background job processing (Hangfire)

**Testing:**
- [ ] Increase test coverage to 80%
- [ ] Add integration tests
- [ ] Add UI tests with Appium
- [ ] Performance testing

**Platform:**
- [ ] Windows 11 support
- [ ] Platform-specific optimizations

### Version 2.0 (Planned)

**Architecture:**
- [ ] Microservices architecture (backend)
- [ ] Event-driven architecture with message bus
- [ ] API Gateway implementation
- [ ] GraphQL API

**Cloud Integration:**
- [ ] Azure/AWS cloud deployment
- [ ] Blob storage for media files
- [ ] Cloud database (Azure SQL/Cosmos DB)
- [ ] Authentication with OAuth 2.0 / OpenID Connect

**Advanced Features:**
- [ ] Real-time synchronization (SignalR)
- [ ] Offline-first architecture with sync conflicts resolution
- [ ] Machine Learning recommendations (ML.NET)
- [ ] Blazor WebAssembly web version

**DevOps:**
- [ ] CI/CD pipeline (GitHub Actions / Azure DevOps)
- [ ] Automated testing in pipeline
- [ ] Containerization (Docker)
- [ ] Kubernetes orchestration

---

## 🤝 Contributing

### Development Guidelines

**Code Style:**
- Follow C# coding conventions
- Use async/await for asynchronous operations
- Implement interfaces for all public services
- Write XML documentation for public APIs

**Architecture Rules:**
- Domain layer must have no external dependencies
- Use DTOs for data transfer between layers
- All database access must go through repositories
- ViewModels should not reference Entity Framework or database entities

**Pull Request Process:**

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Follow existing architecture patterns
4. Write unit tests for new functionality
5. Update documentation if needed
6. Commit with meaningful messages
7. Push to your fork
8. Open a Pull Request

**Testing Requirements:**
- Unit tests for Domain entities
- Unit tests for Application services
- Integration tests for Repositories
- Minimum 70% code coverage for new code

---

## 📄 Лицензия

Этот проект распространяется под лицензией MIT. Подробности в файле [LICENSE](LICENSE).

```
MIT License

Copyright (c) 2024 Psychology App

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🔗 Links

**Application:**
- [RU Store](https://www.rustore.ru/catalog/app/com.subconscious.psychologyapp)
- [Google Play](https://play.google.com/store/apps/details?id=com.subconscious.psychologyapp)
- [Demo Video](https://clck.ru/37HzYu)

**Development:**
- [Issues](../../issues)
- [Pull Requests](../../pulls)
- [Project Board](../../projects)

---

## 📚 Additional Resources

**Technologies:**
- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

**Patterns:**
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [MVVM Pattern](https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm)
- [Specification Pattern](https://martinfowler.com/apsupp/spec.pdf)

---

<div align="center">

**Built with .NET MAUI and Clean Architecture**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-Cross--Platform-512BD4)](https://dotnet.microsoft.com/apps/maui)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)](https://docs.microsoft.com/en-us/ef/core/)

[⬆ Back to Top](#psychology-app)

</div>


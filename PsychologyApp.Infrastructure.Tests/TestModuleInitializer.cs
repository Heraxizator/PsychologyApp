using System.Runtime.CompilerServices;
using PsychologyApp.Infrastructure.Data.Context;

namespace PsychologyApp.Infrastructure.Tests;

internal static class TestModuleInitializer
{
    // Tests open SqliteConnection directly, before any app type has run.
    [ModuleInitializer]
    internal static void Init() => SqliteProvider.EnsureInitialized();
}

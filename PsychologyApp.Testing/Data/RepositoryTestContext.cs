using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;

namespace PsychologyApp.Testing.Data;

public static class RepositoryTestContext
{
    public static IOptions<AppSettings> Settings { get; } =
        Options.Create(new AppSettings { DbCommandTimeoutSeconds = 30 });
}

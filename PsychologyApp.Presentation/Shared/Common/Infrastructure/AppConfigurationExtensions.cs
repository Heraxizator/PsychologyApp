using System.Reflection;
using Microsoft.Extensions.Configuration;
using PsychologyApp.Application.Configuration;

namespace PsychologyApp.Presentation.Shared.Common;

public static class AppConfigurationExtensions
{
    public static MauiAppBuilder AddPsychologyAppConfiguration(this MauiAppBuilder builder)
    {
        Stream? stream = typeof(AppConfigurationExtensions).Assembly
            .GetManifestResourceStream("PsychologyApp.Presentation.appsettings.json");

        if (stream is not null)
        {
            builder.Configuration.AddJsonStream(stream);
        }

        return builder;
    }
}

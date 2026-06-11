using System.Reflection;
using Microsoft.Extensions.Configuration;
using PsychologyApp.Application.Configuration;

namespace PsychologyApp.Presentation.Common;

public static class AppConfigurationExtensions
{
    public static MauiAppBuilder AddPsychologyAppConfiguration(this MauiAppBuilder builder)
    {
        Stream? stream = typeof(AppConfigurationExtensions).Assembly
            .GetManifestResourceStream("PsychologyApp.Presentation.appsettings.json");

        if (stream is not null)
        {
            builder.Configuration.AddJsonStream(stream);
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));
        }

        return builder;
    }
}

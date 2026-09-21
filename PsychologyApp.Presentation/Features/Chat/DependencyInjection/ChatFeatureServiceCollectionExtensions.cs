using PsychologyApp.Application.Chat;
using PsychologyApp.Presentation.Features.Chat.Index;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.Chat.DependencyInjection;

public static class ChatFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddChatFeature(this IServiceCollection services)
    {
        services.AddSingleton<IChatLanguageProvider, AppChatLanguageProvider>();
        services.AddSingleton<IChatViewModelFactory, ChatViewModelFactory>();
        services.AddSingleton<IChatHeroFactory>(sp => sp.GetRequiredService<IChatViewModelFactory>());
        services.AddSingleton<IChatPageFactory, ChatPageFactory>();
        return services;
    }
}

/// <summary>The chat speaks the language the app is currently set to.</summary>
public sealed class AppChatLanguageProvider : IChatLanguageProvider
{
    public bool IsEnglish => AppStrings.IsEnglish(AppStrings.Language);
}

using PsychologyApp.Presentation.Features.SendReviewForm;

namespace PsychologyApp.Presentation.App.Providers;

public static class SendReviewFormFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddSendReviewFormFeature(this IServiceCollection services)
    {
        services.AddSingleton<IReviewPageFactory, ReviewPageFactory>();
        services.AddSingleton<IFormViewModelFactory, FormViewModelFactory>();
        return services;
    }
}

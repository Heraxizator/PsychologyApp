using PsychologyApp.Presentation.Features.SendReviewForm;

namespace PsychologyApp.Presentation.Features.SendReviewForm.DependencyInjection;

public static class SendReviewFormFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddSendReviewFormFeature(this IServiceCollection services)
    {
        services.AddSingleton<IReviewPageFactory, ReviewPageFactory>();
        services.AddSingleton<IFormViewModelFactory, FormViewModelFactory>();
        return services;
    }
}

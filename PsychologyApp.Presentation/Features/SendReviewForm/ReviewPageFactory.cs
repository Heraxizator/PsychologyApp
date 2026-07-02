using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.SendReviewForm.ReviewForm;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.SendReviewForm;

public interface IReviewPageFactory
{
    FormPage CreateFormPage();
}

public interface IFormViewModelFactory
{
    FormViewModel Create(ContentPage page);
}

public sealed class FormViewModelFactory(
    IDialogService dialogService,
    IOptions<AppSettings> settings,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IFormViewModelFactory
{
    public FormViewModel Create(ContentPage page) =>
        new(dialogService, settings, ResolveNavigation(navigationServiceFactory, page));
}

public sealed class ReviewPageFactory(IFormViewModelFactory formViewModelFactory) : IReviewPageFactory
{
    public FormPage CreateFormPage() =>
        new(formViewModelFactory);
}

using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Pages.ReviewForm;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.SendReviewForm;

public interface IReviewPageFactory
{
    FormPage CreateFormPage();
}

public interface IFormViewModelFactory
{
    FormViewModel Create();
}

public sealed class FormViewModelFactory(IDialogService dialogService, IOptions<AppSettings> settings) : IFormViewModelFactory
{
    public FormViewModel Create() => new(dialogService, settings);
}

public sealed class ReviewPageFactory(IFormViewModelFactory formViewModelFactory) : IReviewPageFactory
{
    public FormPage CreateFormPage() =>
        new(formViewModelFactory.Create());
}

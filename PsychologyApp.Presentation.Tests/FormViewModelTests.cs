using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.ViewModels.Review;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class FormViewModelTests
{
    [Fact]
    public async Task Send_EmptyMessage_DoesNotOpenDialog()
    {
        var dialog = new Mock<IDialogService>();
        var viewModel = CreateViewModel(dialog.Object, recipient: "+123");

        viewModel.Send.Execute(null);
        await Task.Delay(50);

        dialog.Verify(d => d.ShowAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Send_MissingRecipient_ShowsConfigurationMessage()
    {
        var dialog = new Mock<IDialogService>();
        var viewModel = CreateViewModel(dialog.Object, recipient: string.Empty);
        viewModel.MessageText = "Feedback text";

        viewModel.Send.Execute(null);
        await Task.Delay(50);

        dialog.Verify(
            d => d.ShowAsync(null, "Получатель SMS не настроен"),
            Times.Once);
    }

    private static FormViewModel CreateViewModel(IDialogService dialogService, string recipient) =>
        new(
            dialogService,
            Options.Create(new AppSettings { ReviewSmsRecipient = recipient }));
}

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
        var viewModel = CreateViewModel(dialog.Object, email: "a@b.com");

        viewModel.Send.Execute(null);
        await Task.Delay(50);

        dialog.Verify(d => d.ShowAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("a@b.com", "", FeedbackChannel.Email)]
    [InlineData("", "+123", FeedbackChannel.Sms)]
    [InlineData("", "", FeedbackChannel.Share)]
    [InlineData("a@b.com", "+123", FeedbackChannel.Email)]
    public void ResolveChannel_PrefersEmailThenSmsThenShare(string email, string sms, FeedbackChannel expected)
    {
        var settings = new AppSettings
        {
            ReviewEmailAddress = email,
            ReviewSmsRecipient = sms
        };

        Assert.Equal(expected, FormViewModel.ResolveChannel(settings));
    }

    private static FormViewModel CreateViewModel(IDialogService dialogService, string email = "", string sms = "") =>
        new(
            dialogService,
            Options.Create(new AppSettings
            {
                ReviewEmailAddress = email,
                ReviewSmsRecipient = sms
            }));
}

using Moq;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Presentation.Pages.Chat.ChatList;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>
/// The ready first lines on the empty chat list are sent as the person's first message. Each must be one the chat
/// understands from a first line, in both languages; reworded into something it does not read, the person would get a
/// vague "tell me more" instead of an answer to what they picked.
/// </summary>
[Collection("Localization")]
public sealed class ChatOpenersTests
{
    [Theory]
    [InlineData("ru")]
    [InlineData("en")]
    public void Every_offered_first_line_is_understood_from_the_first_message(string language)
    {
        AppStrings.LanguageOverride = language;
        try
        {
            ChatListViewModel viewModel = new(
                Mock.Of<IChatService>(), Mock.Of<INavigationService>(), Mock.Of<IChatLanguageProvider>(), Mock.Of<IDialogService>(), TimeProvider.System);
            bool english = language == "en";

            string[] notUnderstood = [.. viewModel.Openers
                .Select(opener => opener.Text)
                .Where(text =>
                {
                    CompanionDialogue dialogue = new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(5));
                    CompanionReply opened = dialogue.Open(new CompanionState(), null);
                    return dialogue.Respond(opened.State, new CompanionInput.FreeText(text)).Emotion == CompanionEmotion.Unknown;
                })];

            Assert.Equal(4, viewModel.Openers.Count);
            Assert.Empty(notUnderstood);
        }
        finally
        {
            AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
        }
    }
}

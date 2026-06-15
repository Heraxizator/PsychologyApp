using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Models.Profile;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Services.Profile;

public sealed class ProfileQuotesPresenter
{
    public IReadOnlyList<QuoteItem> MapFavorites(
        IEnumerable<QuotDTO> quotDTOs,
        ICommand openQuotesTabCommand,
        Func<string, string, ICommand> shareCommandFactory,
        Func<string, string, ICommand> copyCommandFactory)
    {
        List<QuoteItem> items = [];

        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (string.IsNullOrEmpty(quotDTO.Text) || string.IsNullOrEmpty(quotDTO.Title))
            {
                continue;
            }

            items.Add(new QuoteItem
            {
                Id = quotDTO.QuotId,
                Text = quotDTO.Text,
                Author = quotDTO.Title,
                IsFavourite = quotDTO.IsFavourite,
                ShareCommand = shareCommandFactory(quotDTO.Text, quotDTO.Title),
                CopyCommand = copyCommandFactory(quotDTO.Text, quotDTO.Title),
                OpenQuotesTabCommand = openQuotesTabCommand
            });
        }

        return items;
    }
}

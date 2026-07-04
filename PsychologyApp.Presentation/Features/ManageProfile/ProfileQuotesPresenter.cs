using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfileQuotesPresenter
{
    public IReadOnlyList<QuoteItem> MapFavorites(
        IEnumerable<QuotDTO> quotDTOs,
        QuoteItemCommandsFactory commandsFactory,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        ICommand openQuotesTabCommand)
    {
        List<QuoteItem> items = [];

        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (string.IsNullOrEmpty(quotDTO.Text) || string.IsNullOrEmpty(quotDTO.Title))
            {
                continue;
            }

            QuoteItem item = commandsFactory.CreateQuoteItem(quotDTO, refreshBindingAsync, onFail);
            item.OpenQuotesTabCommand = openQuotesTabCommand;
            items.Add(item);
        }

        return items;
    }
}

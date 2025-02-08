
using MvvmHelpers;
using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Toast;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.Modules.Motivator;

public class QuoteViewModel : BaseViewModel
{
    public ObservableRangeCollection<QuoteItem> QuotesObservableCollection { get; set; } = [];

    private readonly QuotService _quotService = new();

    private static Task Initialiation = default!;

    public QuoteViewModel(INavigation navigation) 
    { 
        Navigation = navigation;

        SetInit();

        Initialiation = InitAsync();

        SetDone();
    }

    private async Task InitAsync()
    {
        try
        {
            await LoadQuotesAsync();

            await FillCollAsync();
        }
        
        catch(Exception e)
        {
            SetDone();
        }
    }

    private async Task FillCollAsync()
    {
        await AddItemsInCollAsync(20);
    }

    private async Task AddItemsInCollAsync(int count)
    {
        IEnumerable<QuotDTO> quotDTOs = await _quotService.GetAllAsync(count, 5000);

        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (QuotesObservableCollection.Any(x => x.Text == quotDTO.Text) is true)
            {
                continue;
            }

            QuotesObservableCollection.Add(new QuoteItem
            {
                Id = quotDTO.QuotId,
                Text = quotDTO.Text!,
                Author = quotDTO.Title!,
                IsFavourite = quotDTO.IsFavourite,
                IsReaded = quotDTO.IsReaded,

                LikeCommand = new Command(async (object quoteItem) =>
                {
                    QuoteItem? item = quoteItem as QuoteItem;

                    if (item is null)
                    {
                        return;
                    }

                    await MarkAsFavouriteAsync(item.Id, !item.IsFavourite);
                }),

                ShareCommand = new Command(async () =>
                {
                    await Share.Default.RequestAsync(new ShareTextRequest
                    {
                        Text = $"{quotDTO.Text} ({quotDTO.Title ?? "Пословица"})",
                        Title = "Цитата"
                    });
                }),

                CopyCommand = new Command(async (object quoteItem) =>
                {
                    QuoteItem? item = quoteItem as QuoteItem;

                    if (item is null)
                    {
                        return;
                    }

                    await Clipboard.Default.SetTextAsync($"{item.Text} ({item.Author})");
                }),
            });
        }
    }

    public async Task AddFreshQuotsAsync()
    {
        try
        {
            await LoadQuotesAsync();

            await AddItemsInCollAsync(1);
        }
        
        catch (Exception e)
        {
            SetDone();
        }
    }

    private async Task MarkAsFavouriteAsync(long quoteId, bool isFavourite)
    {
        try
        {
            QuoteItem? quoteItem = QuotesObservableCollection.FirstOrDefault(x => x.Id == quoteId);

            if (quoteItem is not null)
            {
                int quoteIndex = QuotesObservableCollection.IndexOf(quoteItem);

                QuotesObservableCollection[quoteIndex].IsFavourite = isFavourite;
            }

            await _quotService.MarkAsFavouriteAsync(quoteId, isFavourite, 5000);
        }
        
        catch (Exception e)
        {
            SetDone();
        }
    }

    private async Task LoadQuotesAsync()
    {
        try
        {
            await QuotHandler.GetQuotsFromApi(5000);
        }
        
        catch (Exception e)
        {
            SetDone();
        }
    }
}

public class QuoteItem
{
    public long Id { get; set; } = default!;
    public string Text { get; set; } = default!;
    public string Author { get; set; } = default!;
    public bool IsReaded { get; set; } = default!;
    public bool IsFavourite { get; set; } = default!;
    public bool IsShown { get; set; } = default!;
    public ICommand ShareCommand { get; set; } = default!;
    public ICommand LikeCommand { get; set; } = default!;
    public ICommand CopyCommand { get; set; } = default!;
}
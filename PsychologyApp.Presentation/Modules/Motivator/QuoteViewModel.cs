using MvvmHelpers;
using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Infrastructure.API.Quots;
using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Toast;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.Modules.Motivator;

public class QuoteViewModel : BaseViewModel
{
    public ObservableRangeCollection<QuoteItem> QuotesObservableCollection { get; set; } = [];

    private readonly IQuotService _quotService = new QuotService();

    public QuoteViewModel(INavigation navigation) 
    { 
        try
        {
            Navigation = navigation;

            Task.Run(async () => await InitAsync(Constants.MiddleBaseTimeout));
        }
        
        catch (Exception e)
        {
            SetFail();
            Console.WriteLine(e.Message);
        }
    }

    private async Task InitAsync(int cancelTimeout)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetInit();
            });

            await LoadQuotesAsync(cancelTimeout);

            await FillCollAsync(cancelTimeout);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
            });
        }
        
        catch(Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetFail();
            });

            Console.WriteLine(e.Message);
        }
    }

    private async Task FillCollAsync(int cancelTimeout)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            this.QuotesObservableCollection.Clear();
        });

        await AddItemsInCollAsync(20, cancelTimeout);
    }

    private async Task AddItemsInCollAsync(int count, int cancelTimeout)
    {
        IEnumerable<QuotDTO> quotDTOs = await _quotService.GetAllAsync(count, cancelTimeout);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
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

                        await MarkAsFavouriteAsync(item.Id, !item.IsFavourite, cancelTimeout);
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
        });
    }

    public async Task AddFreshQuotsAsync(int cancelTimeout)
    {
        try
        {
            await LoadQuotesAsync(cancelTimeout);

            await AddItemsInCollAsync(1, cancelTimeout);
        }
        
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
            });

            Console.WriteLine(e.Message);
        }
    }

    private async Task MarkAsFavouriteAsync(long quoteId, bool isFavourite, int cancelTimeout)
    {
        try
        {
            QuoteItem? quoteItem = QuotesObservableCollection.FirstOrDefault(x => x.Id == quoteId);

            if (quoteItem is null)
            {
                return;
            }

            int index = QuotesObservableCollection.IndexOf(quoteItem);

            quoteItem.IsFavourite = !quoteItem.IsFavourite;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                QuotesObservableCollection[index] = quoteItem;
            });
            
            await _quotService.MarkAsFavouriteAsync(quoteId, isFavourite, cancelTimeout);
        }
        
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
            });
            
            Console.WriteLine(e.Message);
        }
    }

    private Task LoadQuotesAsync(int cancelTimeout)
    {
        return _quotService.LoadSingleAsync(cancelTimeout);
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
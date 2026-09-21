using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>An <see cref="ObservableCollection{T}"/> that can take many items with a single change notification, so the UI lays out once instead of once per item.</summary>
public sealed class RangeObservableCollection<T> : ObservableCollection<T>
{
    public void AddRange(IReadOnlyCollection<T> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        foreach (T item in items)
        {
            Items.Add(item);
        }

        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

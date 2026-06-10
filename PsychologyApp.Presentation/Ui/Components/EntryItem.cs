using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.UI.Components;

public class EntryItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _placeholder = string.Empty;
    private string _text = string.Empty;

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public string Placeholder
    {
        get => _placeholder;
        set => SetField(ref _placeholder, value);
    }

    public string Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.UI.Components;

public class EntryItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _placeholder = string.Empty;
    private string _text = string.Empty;
    private EntryFieldKind _kind = EntryFieldKind.Text;

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

    public EntryFieldKind Kind
    {
        get => _kind;
        set => SetField(ref _kind, value);
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

    private void SetField(ref EntryFieldKind field, EntryFieldKind value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public EntryItem CloneEmpty() => new()
    {
        Title = Title,
        Placeholder = Placeholder,
        Kind = Kind,
        Text = string.Empty
    };
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.UI.Components;

public class EntryItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _placeholder = string.Empty;
    private string _text = string.Empty;
    private EntryFieldKind _kind = EntryFieldKind.Text;
    private string _recentText = string.Empty;
    private IReadOnlyList<string> _assistSuggestions = [];

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

    /// <summary>Typical short answers offered as chips under the field.</summary>
    public IReadOnlyList<string> Suggestions { get; init; } = [];

    /// <summary>What the field is about: decides which extra help the field shows.</summary>
    public EntryFieldRole Role { get; init; }

    /// <summary>The problem the person worked on last time, offered as a one-tap chip while this field is empty.</summary>
    public string RecentText
    {
        get => _recentText;
        set => SetField(ref _recentText, value);
    }

    /// <summary>Ready phrases picked for what the person wrote earlier in the same practice (balanced thoughts, kind words).</summary>
    public IReadOnlyList<string> AssistSuggestions
    {
        get => _assistSuggestions;
        set
        {
            if (ReferenceEquals(_assistSuggestions, value))
            {
                return;
            }

            _assistSuggestions = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AssistSuggestions)));
        }
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
        Suggestions = Suggestions,
        Role = Role,
        Text = string.Empty
    };
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;

/// <summary>One step of a practice being written, in its own field.</summary>
public sealed class DesignerStep : INotifyPropertyChanged
{
    private string _text = string.Empty;

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value)
            {
                return;
            }

            _text = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    }

    public required ICommand RemoveCommand { get; init; }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class DesignerViewModel
{
    private bool _syncingSteps;

    /// <summary>
    /// The steps one field each, a "+" for the next: one field of lines hid that every line became a step. <see cref="Actions"/> stays
    /// the stored form, the non-empty steps one per line, as <c>CustomTechniqueSessionOperations</c> splits it.
    /// </summary>
    public ObservableCollection<DesignerStep> Steps { get; } = [];

    public ICommand AddStepCommand => _addStepCommand ??= new Command(() => AddStep(string.Empty));

    private ICommand? _addStepCommand;

    private void AddStep(string text)
    {
        DesignerStep step = null!;
        step = new DesignerStep { Text = text, RemoveCommand = new Command(() => RemoveStep(step)) };
        step.PropertyChanged += OnStepChanged;
        Steps.Add(step);
    }

    private void RemoveStep(DesignerStep step)
    {
        step.PropertyChanged -= OnStepChanged;
        Steps.Remove(step);
        if (Steps.Count == 0)
        {
            AddStep(string.Empty);
        }

        WriteStepsToActions();
    }

    private void OnStepChanged(object? sender, PropertyChangedEventArgs e) => WriteStepsToActions();

    private void WriteStepsToActions()
    {
        _syncingSteps = true;
        try
        {
            string[] lines = [.. Steps.Select(step => step.Text.Trim()).Where(text => text.Length > 0)];
            Actions = lines.Length == 0 ? null : string.Join('\n', lines);
        }
        finally
        {
            _syncingSteps = false;
        }
    }

    /// <summary>Actions set from outside (a loaded practice): its lines become the step fields, and one empty field when there are none.</summary>
    private void ReadStepsFromActions()
    {
        if (_syncingSteps)
        {
            return;
        }

        foreach (DesignerStep step in Steps)
        {
            step.PropertyChanged -= OnStepChanged;
        }

        Steps.Clear();
        foreach (string line in (Actions ?? string.Empty).Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0))
        {
            AddStep(line);
        }

        if (Steps.Count == 0)
        {
            AddStep(string.Empty);
        }
    }
}


namespace PsychologyApp.Presentation.Modules.Tests;

public class Question
{
    public int Number { get; set; }
    public string? Context { get; set; }
    public List<Answer> Answers { get; set; } = [];
}

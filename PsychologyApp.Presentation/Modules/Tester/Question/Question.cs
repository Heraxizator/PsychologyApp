
namespace PsychologyApp.Presentation.Modules.Tester;

public class Question
{
    public int Number { get; set; }
    public string? Context { get; set; }
    public List<Answer> Answers { get; set; } = [];
}

using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Application.Models.Tests;

public class Answer
{
    public int Number { get; set; }
    public int Ball { get; set; }
    public string? Text { get; set; }
    public bool Selected { get; set; }
}

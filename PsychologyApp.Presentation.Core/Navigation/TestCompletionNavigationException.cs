namespace PsychologyApp.Presentation.Shared.Navigation;

/// <summary>
/// Thrown when an operation succeeded but navigation to the next screen failed after retries.
/// </summary>
public sealed class TestCompletionNavigationException : Exception
{
    public TestCompletionNavigationException()
        : this(null)
    {
    }

    public TestCompletionNavigationException(Exception? innerException)
        : base("Test result was saved but navigation to the result screen failed.", innerException)
    {
    }
}

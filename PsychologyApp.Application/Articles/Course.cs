namespace PsychologyApp.Application.Articles;

/// <param name="Task">What to try before the next day: guided self-help works through practice between readings, not reading alone.</param>
/// <param name="Reflection">A question the person answers in their own words when they close the day.</param>
public sealed record CourseDay(string Title, int Minutes, IReadOnlyList<ArticleBlock> Blocks, string Task, string Reflection);

/// <param name="Id">Stable: progress is stored by it.</param>
/// <param name="TestId">The questionnaire taken at the start and the end, so the person sees whether the course helped.</param>
/// <param name="TestName">How the questionnaire is called on the Tests tab.</param>
public sealed record Course(string Id, string Title, string Summary, string Icon, string TestId, string TestName, IReadOnlyList<CourseDay> Days);

public static class CourseIds
{
    public const string Sleep = "sleep";
    public const string Anxiety = "anxiety";
    public const string Mood = "mood";
    public const string Trauma = "trauma";
}

public static partial class ArticleContent
{
    public static IReadOnlyList<Course> Courses(bool english) => english ? CoursesEn : CoursesRu;

    public static Course? FindCourse(string id, bool english) => Courses(english).FirstOrDefault(course => course.Id == id);
}

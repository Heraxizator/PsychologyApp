using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class QuestionnaireDetailReader(IQuestionnaireResultDetailService detailService)
{
    public QuestionnaireResultDetail? TryParse(string? detailJson) => detailService.TryParse(detailJson);
}

using PsychologyApp.Domain.Entities;
using ReasonEntity = PsychologyApp.Domain.Entities.Reason;

namespace PsychologyApp.Application.Reason;

public static class ReasonTsvParser
{
    public static IReadOnlyList<ReasonEntity> ParseLines(IEnumerable<string> lines)
    {
        List<ReasonEntity> reasons = [];

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] cols = line.Split('\t');
            if (cols.Length < 3)
            {
                continue;
            }

            string problemText = cols[0];
            string problemReason = cols[1];
            string problemSolution = cols[2];

            if (string.IsNullOrWhiteSpace(problemText) ||
                string.IsNullOrWhiteSpace(problemReason) ||
                string.IsNullOrWhiteSpace(problemSolution))
            {
                continue;
            }

            reasons.Add(ReasonEntity.Create(problemText, problemReason, problemSolution));
        }

        return reasons;
    }
}

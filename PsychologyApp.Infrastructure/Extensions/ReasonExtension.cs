using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Helpers;

public static class ReasonExtension
{
    public static async Task<IEnumerable<Reason>> LoadReasonsAsync(CancellationToken cancellationToken)
    {
        return await Task.Run(async () =>
        {
            List<Reason> reasons = new();

            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Psyhosomatic.txt");
            using StreamReader streamReader = new(fileStream);

            string? line = streamReader?.ReadLine();

            while ((line = streamReader?.ReadLine()) != null && streamReader?.EndOfStream is false)
            {
                string[] cols = line.Split('\t');

                string? problemText = cols[0].ToString();

                string? problemReason = cols[1].ToString();

                string? problemSolution = cols[2].ToString();

                if (string.IsNullOrWhiteSpace(problemText) is true || string.IsNullOrWhiteSpace(problemReason) is true || string.IsNullOrWhiteSpace(problemSolution) is true)
                {
                    continue;
                }

                reasons.Add(Reason.Create(problemText, problemReason, problemSolution));
            }

            return reasons;
        }, cancellationToken);
    }
}

using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Helpers;

public static class ReasonHelper
{
    public static async Task<bool> SavePsyhosomaticData(int cancelTimeout = 15000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);
        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        if ((await Database.ReasonRepository.GetAllAsync()).Any() is true)
        {
            return true;
        }

        return await Task.Run(async () =>
        {
            List<Reason> psychosomaticObjects = new();

            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Psyhosomatic.txt");

            using StreamReader streamReader = new(fileStream);

            string? line = streamReader?.ReadLine();

            while ((line = streamReader?.ReadLine()) != null && streamReader?.EndOfStream is false)
            {
                string[] cols = line.Split('\t');

                string? problemText = cols[0].ToString();

                string? problemReason = cols[1].ToString();

                string? problemSolution = cols[2].ToString();

                try
                {
                    Reason reason = Reason.Create(problemText, problemReason, problemSolution);

                    psychosomaticObjects.Add(reason);
                }

                catch (Exception) { }
            }

            return await Database.ReasonRepository.AddRangeAsync(psychosomaticObjects);
        });
    }
}

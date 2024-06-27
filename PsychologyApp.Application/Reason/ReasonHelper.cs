using PsychologyApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Helpers
{
    public class ReasonHelper
    {
        public async Task<IList<ReasonDTO>> GetPsyhosomaticData()
        {
            return await Task.Run(async () =>
            {
                List<ReasonDTO> psychosomaticObjects = new();

                using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Psyhosomatic.txt");

                using StreamReader streamReader = new(fileStream);

                string? line = streamReader?.ReadLine();

                while ((line = streamReader?.ReadLine()) != null && streamReader?.EndOfStream is false)
                {
                    string[] cols = line.Split('\t');

                    string? problemText = cols[0].ToString();

                    string? problemReason = cols[1].ToString();

                    string? problemSolution = cols[2].ToString();

                    ReasonDTO psychosomaticObject = new()
                    {
                        Title = problemText,
                        Subtitle = problemReason,
                        Solution = problemSolution
                    };

                    psychosomaticObjects.Add(psychosomaticObject);
                }

                return psychosomaticObjects;
            });
        }
    }
}

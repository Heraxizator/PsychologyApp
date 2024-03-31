using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Application.Helpers
{
    public static class PsyhosomaticHelper
    {
        public static async Task<IList<PsychosomaticObject>> GetPsyhosomaticData()
        {
            List<PsychosomaticObject> psychosomaticObjects = new();

            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Psyhosomatic.txt");

            using StreamReader streamReader = new(fileStream);

            string? line = streamReader?.ReadLine();

            while ((line = streamReader?.ReadLine()) != null) 
            {
                string[] cols = line.Split('\t');

                string? problemText = cols[0].ToString();

                string? problemReason = cols[1].ToString();

                string? problemSolution = cols[2].ToString();

                PsychosomaticObject psychosomaticObject = new()
                {
                    ProblemText = problemText,
                    ProblemReason = problemReason,
                    ProblemSolution = problemSolution
                };

                psychosomaticObjects.Add(psychosomaticObject);
            }

            return psychosomaticObjects;
        }

        #region Objects

        public class PsychosomaticObject
        {
            public string? ProblemText { get; set; }
            public string? ProblemReason { get; set; }
            public string? ProblemSolution { get; set; }
        }

        #endregion
    }
}

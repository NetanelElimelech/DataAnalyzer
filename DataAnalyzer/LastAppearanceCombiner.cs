using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    class LastAppearanceCombiner : CustomArray
    {
        public LastAppearanceCombiner(string fileContent, int maxNumber)
        {
            int[][] intArrayFromFile = CreateIntArrayFromString(fileContent);

            int[] lastAppearanceArray = new int[maxNumber];

            int[][] draws = SeparateToNumbers(CreateDrawsNumberWonArray(intArrayFromFile, maxNumber));

            for (int i = 0; i < maxNumber; i++)
            {
                lastAppearanceArray[i] = draws[i][0];
            }

            view = new DataView(Tables.PopulateDataTable(maxNumber, lastAppearanceArray, Tables.ETableType.lastAppearance, new string[] { "Number", "Last appeared in" }));
            view.Sort = "Number ASC";
        }
    }
}

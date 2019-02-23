﻿using System.Data;

namespace DataAnalyzer
{
    class RatingCombiner : CustomArray
    {
        public RatingCombiner(string fileContent, int maxNumber)
        {
            int[][] intArrayFromFile = CropArray(CreateIntArrayFromString(fileContent));
            Tables.allNumbersCount = Calculators.CalculateTotalNumbersCount(intArrayFromFile[0].Length - 2, intArrayFromFile.Length);

            int[] numbersCounted = Calculators.CountEverySingleNumber(maxNumber, intArrayFromFile);

            view = new DataView(Tables.PopulateDataTable(maxNumber, numbersCounted,
                Tables.ETableType.rate, new string[] { "Number", "Count", "Rate" }));
            view.Sort = "Count DESC";
        }
    }
}

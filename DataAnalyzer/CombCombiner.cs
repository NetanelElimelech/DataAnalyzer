using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    class CombCombiner : CustomArray
    {
        int[][] combinationsToCheckArray;
        int[][] comparedArrayFiltered;

        public CombCombiner(int maxNumber, int innerArraySize, string fileContent)
        {
            combinationsToCheckArray = new int[maxNumber - 2][];
            for (int i = 0; i < combinationsToCheckArray.Length; i++)
            {
                combinationsToCheckArray[i] = new int[innerArraySize];
            }

            combinationsToCheckArray = BuildConsequentComb(maxNumber, innerArraySize);
            int[][] intArrayFromFile = CropArray(CreateIntArrayFromString(fileContent));
            int[][] comparedArray = CompareArrays(EPurpose.statistics, combinationsToCheckArray, intArrayFromFile, combinationsToCheckArray.Length, intArrayFromFile.Length, innerArraySize);
            comparedArrayFiltered = ReduceArrayByPushingOutNulls(comparedArray);
        }

        public string GetCombinationsToBeCheckedAsString()
        {
            return GetArrayAsString(combinationsToCheckArray, "");
        }

        public string GetComparedCombinationsAsString()
        {
            return GetArrayAsString(comparedArrayFiltered, "- ");
        }
    }
}

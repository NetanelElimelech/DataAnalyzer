using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    class AvgSpanCombiner : CustomArray
    {
        int lowerStepsLimit;
        int upperStepsLimit;
        int[][] drawsIntArray;
        int[][] jumpsArray;

        public AvgSpanCombiner(string fileContent, int maxNumber, string lowerStepsLimitString, string upperStepsLimitString)
        {
            int[][] intArrayFromFile = CreateIntArrayFromString(fileContent);
            string[] drawsStringArray = CreateInitialDrawsArray(intArrayFromFile, maxNumber);
            List<int>[] drawsListArray = CreateInitialDrawsList(intArrayFromFile, maxNumber);
            //drawsIntArray = SeparateToNumbers(drawsStringArray);
            jumpsArray = CalculateJumps(maxNumber, drawsListArray, GetLowerStepsLimit(lowerStepsLimitString), GetUpperStepsLimit(upperStepsLimitString));

            //GetLowerStepsLimit(lowerStepsLimitString);
            //GetUpperStepsLimit(upperStepsLimitString);
        }

        public int GetLowerStepsLimit(string lowerStepsLimitString)
        {
            bool lowerStepsLimitProvided = int.TryParse(lowerStepsLimitString, out lowerStepsLimit);

            if (lowerStepsLimitProvided == false)
            {
                lowerStepsLimit = 0;
            }

            return lowerStepsLimit;
        }

        public int GetUpperStepsLimit(string upperStepsLimitString)
        {
            bool upperStepsLimitProvided = int.TryParse(upperStepsLimitString, out upperStepsLimit);

            if (upperStepsLimitProvided == false)
            {
                upperStepsLimit = int.MaxValue;
            }

            return upperStepsLimit;
        }

        public int GetLowerStepsLimit()
        {
            return lowerStepsLimit;
        }

        public int GetUpperStepsLimit()
        {
            return upperStepsLimit;
        }

        public int[][] GetDrawsIntArray()
        {
            return drawsIntArray;
        }

        public int[][] GetJumpsArray()
        {
            return jumpsArray;
        }

        int[][] CalculateJumps(int maxNumber, List<int>[] drawsListArray, int lowerStepsLimit, int upperStepsLimit)
        {
            jumpsArray = new int[maxNumber][];
            int jump = 0;

            for (int i = 0; i < drawsListArray.Length; i++)
            {
                jumpsArray[i] = new int[drawsListArray[i].Count - 1];

                for (int j = 0; j < jumpsArray[i].Length; j++)
                {
                    jump = drawsListArray[i][j] - drawsListArray[i][j + 1];
                    if (jump >= lowerStepsLimit && jump <= upperStepsLimit)
                    {
                        jumpsArray[i][j] = jump;
                    }
                }
            }
            return jumpsArray;
        }
    }
}

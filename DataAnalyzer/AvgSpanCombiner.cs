﻿using System;
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
        readonly int[][] drawsArray;
        readonly int[][] intArrayFromFile;

        public AvgSpanCombiner(string fileContent, int maxNumber, string lowerStepsLimitString, string upperStepsLimitString)
        {
            intArrayFromFile = CreateIntArrayFromString(fileContent);
            drawsArray = SeparateToNumbers(CreateDrawsNumberWonArray(intArrayFromFile, maxNumber));
            GetLowerStepsLimit(lowerStepsLimitString);
            GetUpperStepsLimit(upperStepsLimitString);
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

        public int[][] GetInitArrayFromFile()
        {
            return intArrayFromFile;
        }

        public int[][] GetDrawsIntArray()
        {
            return drawsArray;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    class DrawsCombiner : CustomArray
    {
        readonly string[] drawsStringArray;
        readonly string[] drawsArrayToBeDisplayed;

        public DrawsCombiner(string fileContent, int maxNumber)
        {
            drawsArrayToBeDisplayed = new string[maxNumber];
            int[][] intArrayFromFile = CreateIntArrayFromString(fileContent);
            drawsStringArray = CreateInitialDrawsArray(intArrayFromFile, maxNumber);

            drawsArrayToBeDisplayed = GetDrawsArrayToBeDisplayed(drawsStringArray);
        }

        internal string[] GetDrawsStringArray()
        {
            return drawsArrayToBeDisplayed;
        }

        internal string[] GetDrawsArrayToBeDisplayed(string[] inputArray)
        {
            for (int i = 0; i < inputArray.Length - 1; i++)
            {
                drawsArrayToBeDisplayed[i] = ($"{i + 1} appears in:\n{inputArray[i]}\n");
            }

            return drawsArrayToBeDisplayed;
        }
    }
}

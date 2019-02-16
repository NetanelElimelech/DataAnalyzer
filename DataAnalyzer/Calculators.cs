using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalyzer
{
    static class Calculators
    {        
        public static double CalculatePercentRate(double firstNumber, double secondNumber)
        {
            return Math.Round(firstNumber / secondNumber * 100, 2);
        }

        public static int[] CountEverySingleNumber(int arraySize, int[][] inputArray)
        {
            int[] outputArray = new int[arraySize];
            for (int i = 0; i < arraySize; i++) //Checks every number from 1 to maximal possible number
            {
                for (int j = 0; j < inputArray.Length; j++) //Goes through all the sequences
                {
                    for (int k = 0; k < inputArray[j].Length; k++) //Goes through all the positions (numbers) in the sequence
                    {
                        if (inputArray[j][k] == i + 1)
                        {
                            outputArray[i]++;
                        }
                    }
                }
            }
            return outputArray;
        }

        public static IEnumerable<IEnumerable<int>> Combinations(this IEnumerable<int> elements, int k)
        {
            return k == 0 ? new[] { new int[0] } :
              elements.SelectMany((e, i) => elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static int CalculateTotalNumbersCount(int combinationSize, int outerArraySize)
        {
            return combinationSize * outerArraySize;
        }
    }
}
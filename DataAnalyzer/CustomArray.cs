using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace DataAnalyzer
{
    abstract class CustomArray
    {
        public const string SEPARATE_TO_LINES = @"(?=\n)";
        public const string SEPARATE_TO_NUMBERS = @"(?=\t)";
        static string[] separatedNumbersArray;

        protected DataView view;

        internal DataView GetView()
        {
            return view;
        }

        public enum EPurpose { control, statistics }

        public static int[][] CreateIntArrayFromString(string fileContent)
        {
            string[] stringArray = SeparateToLines(fileContent);
            int[][] intArray = SeparateToNumbers(stringArray);
            return intArray;
        }

        public static string[] SeparateToLines(string initStringFromFile)
        {
            return Regex.Split(initStringFromFile, SEPARATE_TO_LINES);
        }

        public static int[][] SeparateToNumbers(string[] inputArray)
        {
            int[][] outputArray = new int[inputArray.Length][];

            for (int i = 0; i < inputArray.Length; i++)
            {
                separatedNumbersArray = Regex.Split(inputArray[i], SEPARATE_TO_NUMBERS);

                outputArray[i] = ParseStringArray(separatedNumbersArray);
            }
            return outputArray;
        }

    public static int[] ParseStringArray(string[] inputArray)
        {
            int[] outputArray = new int[inputArray.Length];

            for (int i = 0; i < outputArray.Length; i++)
            {
                bool success = int.TryParse(inputArray[i], out int parsedNumber);

                if (success)
                {
                    outputArray[i] = int.Parse(inputArray[i]);
                }

                else
                {
                    string errorNaN = $"This entry isn't a number: {inputArray[i]}";
                }
            }
            return outputArray;
        }

        protected static string[] CreateInitialDrawsArray(int[][] inputArray, int maxNumber)
        {
            string[] outputArray = new string[maxNumber];

            for (int i = 0; i < outputArray.Length; i++) //Check every number from 1 to maximal number
            {
                for (int j = 0; j < inputArray.Length; j++) //Go through all the sequences
                {
                    for (int k = 0; k < inputArray[j].Length - 1; k++) //Go through all the positions (numbers) in the sequence
                    {
                        if (inputArray[j][k] == i + 1)
                        {
                            outputArray[i] += "\t" + inputArray[j][0].ToString();
                        }
                    }
                }
            }
            return outputArray;
        }

        protected string[] CreateDrawsNumberWonArray(int[][] inputArray, int maxNumber)
        {
            string[] inWhichDrawNumberWonArray = new string[maxNumber];

            for (int i = 0; i < inWhichDrawNumberWonArray.Length; i++) //Check every number from 1 to maximal number
            {
                for (int j = 0; j < inputArray.Length; j++) //Go through all the sequences
                {
                    for (int k = 0; k < inputArray[j].Length; k++) //Go through all the positions (numbers) in the sequence
                    {
                        if (inputArray[j][k] == i + 1)
                        {
                            inWhichDrawNumberWonArray[i] += inputArray[j][0].ToString() + "\t";
                        }
                    }
                }
            }
            return inWhichDrawNumberWonArray;
        }

        public static int[][] CreateCombinationsArray(IEnumerable<IEnumerable<int>> collection, int innerOutputArraySize)
        {
            int iteration = 0;
            int[][] outputArray = new int[GetArrayLength(collection)][];

            for (int i = 0; i < outputArray.Length; i++)
            {
                outputArray[i] = new int[innerOutputArraySize];
            }

            foreach (var subArray in collection)
            {
                int innerIteration = 0;
                foreach (var item in subArray)
                {
                    outputArray[iteration][innerIteration] = item;
                    innerIteration++;
                }
                iteration++;
            }

            return outputArray;
        }

        public static int GetArrayLength<T>(IEnumerable<T> collection)
        {
            int size = 0;

            foreach (var item in collection)
            {
                size++;
            }
            return size;
        }

        public static int GetFilteredArrayLength(int[][] inputArray)
        {
            int size = 0;
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (inputArray[i] is null)
                {
                    //then do nothing
                }

                else if (inputArray[i][0] == 0)
                {
                    inputArray[i] = null;
                }

                else
                {
                    size++;
                }
            }
            return size;
        }

        //Reduce array size
        public static int[][] ReduceArrayByPushingOutNulls(int[][] inputArray)
        {
            int[][] outputArray = new int[GetFilteredArrayLength(inputArray)][];
            int iteration = 0;
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (inputArray[i] is null)
                {
                    //then do nothing
                }

                else
                {
                    outputArray[iteration] = inputArray[i];
                    iteration++;
                }
            }
            return outputArray;
        }

        public static int[][] CompareArrays(int[][] outerLoopArray, int[][] innerLoopArray, int outputArraySize, int combFilter)
        {
            int[][] outputArray = new int[outputArraySize][];

            for (int i = 0; i < outputArray.Length; i++)
            {
                outputArray[i] = new int[outerLoopArray[i].Length];
            }

            if (innerLoopArray.Length == 0)
            {
                for (int i = 0; i < outputArray.Length; i++)
                {
                    outputArray[i] = outerLoopArray[i];
                }
            }

            else
            {
                for (int i = 0; i < outerLoopArray.Length; i++)
                {
                    for (int j = 0; j < innerLoopArray.Length; j++)
                    {
                        int test = 0;

                        foreach (int item in innerLoopArray[j])
                        {
                            if (Array.Exists(outerLoopArray[i], control => control == item))
                            {
                                test++;
                            }
                        }
                        if (test == combFilter)
                        {
                            outputArray[i] = null;
                            break;
                        }

                        else
                        {
                            outputArray[i] = outerLoopArray[i];
                        }
                    }
                }
            }

            return outputArray;
        }

        public static int[][] CompareArrays(EPurpose purpose, int[][] outerLoopArray, int[][] innerLoopArray, int outputArraySize, int breakhere, int combFilter)
        {
            int[][] outputArray = new int[outputArraySize][];

            for (int i = 0; i < outputArray.Length; i++)
            {
                outputArray[i] = new int[combFilter + 1];
            }

            if (breakhere > innerLoopArray.Length)
            {
                breakhere = innerLoopArray.Length;
            }

            for (int i = 0; i < outerLoopArray.Length; i++)
            {
                for (int j = 0; j < breakhere; j++)
                {
                    int test = 0;

                    foreach (int item in outerLoopArray[i])
                    {
                        if (Array.Exists(innerLoopArray[j], control => control == item))
                        {
                            test++;

                            if (test == combFilter)
                            {
                                if (purpose == EPurpose.control)
                                {
                                    outputArray[i] = outerLoopArray[i];
                                    break;
                                }

                                else if (purpose == EPurpose.statistics)
                                {
                                    for (int k = 0; k < outerLoopArray[i].Length; k++)
                                    {
                                        outputArray[i][k] = outerLoopArray[i][k];
                                    }
                                    outputArray[i][outputArray[i].Length - 1]++;
                                }
                            }
                        }
                    }
                }
            }
            return outputArray;
        }

        public static string[][] CreatePartialCombArray(int[][] inputArray)
        {
            string[][] outputArray = new string[inputArray.Length][];
            for (int i = 0; i < outputArray.Length; i++)
            {
                outputArray[i] = new string[2];
            }

            for (int i = 0; i < inputArray.Length; i++)
            {
                string combination = "";

                for (int j = 0; j < inputArray[i].Length; j++)
                {
                    if (j == (inputArray[i].Length - 1))
                    {
                        outputArray[i][1] = inputArray[i][j].ToString();
                    }
                    else
                    {
                        combination += $"{inputArray[i][j].ToString()} ";
                    }
                    outputArray[i][0] = combination;
                }
            }
            return outputArray;
        }

        public static int[][] RemoveEvensOrOddsOnlyComb(int[][] inputArray)
        {
            int[][] outputArray = new int[inputArray.Length][];

            for (int i = 0; i < outputArray.Length; i++)
            {
                int evens = 0;
                int odds = 0;
                if (inputArray[i] != null)
                {
                    foreach (var item in inputArray[i])
                    {
                        if (item % 2 == 0)
                        {
                            evens++;
                        }

                        else
                        {
                            odds++;
                        }
                    }

                    if (evens == 6 || odds == 6)
                    {
                        outputArray[i] = null;
                    }

                    else
                    {
                        outputArray[i] = inputArray[i];
                    }
                }
            }

            return outputArray;
        }
        //TODO: Push out odds-only and evens-only combinations
        //TODO: In Version 2 the data should be DB-based
        //TODO: Working with one single file for all
    }
}
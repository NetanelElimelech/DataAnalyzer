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

        public enum EPurpose { control, combinations, statistics }

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

            for (int i = 0; i < outputArray.Length; i++) //Check every number from 1 to maximal possible number
            {
                for (int j = 0; j < inputArray.Length; j++) //Go through all the sequences
                {
                    for (int k = 0; k < inputArray[j].Length; k++) //Go through all the positions (numbers) in the sequence
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

            for (int i = 0; i < inWhichDrawNumberWonArray.Length; i++) //Check every number from 1 to maximal possible number
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
        public static int[][] ReduceArrayByPullingOutNulls(int[][] inputArray)
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

        public static int[][] CompareArrays(int[][] outerLoopArray, int[][] innerLoopArray, int outputArraySize, int combFilter, EPurpose purpose)
        {
            int[][] outputArray = new int[outputArraySize][];

            for (int i = 0; i < outputArray.Length; i++)
            {
                outputArray[i] = new int[combFilter + 1];
            }

            for (int i = 0; i < outerLoopArray.Length; i++)
            {
                for (int j = 0; j < innerLoopArray.Length; j++)
                {
                    if (purpose == EPurpose.combinations)
                    {
                        if (Array.Exists(outerLoopArray[i], control1 => control1 == innerLoopArray[j][0])
                        && Array.Exists(outerLoopArray[i], control2 => control2 == innerLoopArray[j][1])
                        && Array.Exists(outerLoopArray[i], control3 => control3 == innerLoopArray[j][2])
                        && Array.Exists(outerLoopArray[i], control4 => control4 == innerLoopArray[j][3])
                        )
                        {
                            outputArray[i] = null;
                            break;
                        }

                        else
                        {
                            outputArray[i] = outerLoopArray[i];
                        }
                    }

                    else
                    {
                        if (Array.Exists(innerLoopArray[j], control1 => control1 == outerLoopArray[i][0])
                        && Array.Exists(innerLoopArray[j], control2 => control2 == outerLoopArray[i][1])
                        && Array.Exists(innerLoopArray[j], control3 => control3 == outerLoopArray[i][2])
                        && Array.Exists(innerLoopArray[j], control4 => control4 == outerLoopArray[i][3])
                        )
                        {
                            if (purpose == EPurpose.control)
                            {
                                outputArray[i] = outerLoopArray[i];
                                break;
                            }

                            else if (purpose == EPurpose.statistics)
                            {
                                //if ()
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
            return outputArray;
        }

        //public static int[][] CompareArrays(int[][] outerLoopArray,
        //    int[][] innerLoopArray, int outputArraySize, int outputInnerArraySize, int combFilter, EPurpose purpose)
        //{
        //    int[][] outputArray = new int[outputArraySize][];
        //    for (int i = 0; i < outputArray.Length; i++)
        //    {
        //        outputArray[i] = new int[outputInnerArraySize];
        //    }
        //    int test = 0;
        //    for (int i = 0; i < outerLoopArray.Length; i++)
        //    {
        //        for (int j = 0; j < innerLoopArray.Length; j++)
        //        {
        //            test = 0;
        //            if (innerLoopArray[j] is null == false)
        //            {
        //                foreach (int item in innerLoopArray[j])
        //                {
        //                    if (Array.Exists(outerLoopArray[i], control => control == item))
        //                    {
        //                        test++;
        //                    }
        //                }
        //            }
        //            if (purpose == EPurpose.control || purpose == EPurpose.statistics)
        //            {
        //                if (outputArray[j] is null == false && test == combFilter)
        //                {
        //                    for (int k = 0; k < innerLoopArray[j].Length; k++)
        //                    {
        //                        outputArray[j][k] = innerLoopArray[j][k];
        //                    }
        //                    if (purpose == EPurpose.statistics)
        //                    {
        //                        outputArray[j][outputInnerArraySize - 1]++;
        //                        //break;
        //                    }
        //                }
        //                else
        //                {
        //                    outputArray[j] = null;
        //                    //break;
        //                }
        //            }
        //            else if (purpose == EPurpose.combinations)
        //            {
        //                if (test == combFilter)
        //                {
        //                    outputArray[i] = null;
        //                    break;
        //                }
        //                else
        //                {
        //                    outputArray[i] = outerLoopArray[i];
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return outputArray;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace DataAnalyzer
{
    public partial class AdvancedCombiner : Window
    {
        public AdvancedCombiner()
        {
            InitializeComponent();
        }

        private string FetchFileFromUrl()
        {
            string fileContent = null;
            try
            {
                var webRequest = WebRequest.Create(@"https://raw.githubusercontent.com/NetanelElimelech/DataAnalyzer/master/DataAnalyzer/allDraws.txt");

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    fileContent = reader.ReadToEnd();
                    reader.Close();
                }
            }

            catch
            {
                CustomMessageBox customMessageBox = new CustomMessageBox();
                customMessageBox.Show();
                customMessageBox.Title = "File not found";
            }

            return fileContent;
        }

        private void CombineButtonADV_Click(object sender, RoutedEventArgs e)
        {
            outputTextBoxADV.Clear();
            CombineAndDisplay();
        }

        void CombineAndDisplay()
        {
            Display(JoinNumbers(FilterInclude(Combine())));
        }

        int[][] FilterInclude(int[][] combArrayUnfiltered)
        {
            List<int> includedNumbersList = GetNumbersToInclude();

            if (includedNumbersList.Count != 0)
            {
                int[][] tempArrayInclude = ForceInclude(combArrayUnfiltered, includedNumbersList);
                return tempArrayInclude;
            }

            else
            {
                return combArrayUnfiltered;
            }
        }

        void FilterExclude(string[] combStringArrayUnfiltered)
        {
            string[] numbersToExlude = CustomArray.SeparateToLines(excludeTextBoxADV.Text);
        }

        string[] JoinNumbers(int[][] combIntArray)
        {
            string[] combStringArray = new string[combIntArray.Length];

            for (int i = 0; i < combIntArray.Length; i++)
            {
                string combination = " ";
                for (int j = 0; j < combIntArray[i].Length; j++)
                {
                    combination += $"{combIntArray[i][j].ToString()} ";
                }

                combStringArray[i] = combination;
            }
            return combStringArray;
        }

        void Display(string[] combinations)
        {
            int count = 0;

            for (int i = 0; i < combinations.Length; i++)
            {
                //For: 1 7 8 9 10 11 12 13 15 16 17 18 22 23 25 27 28 29 30 35

                //if (!combinations[i].Contains("7 8 9")
                //&& !combinations[i].Contains("8 9 10")
                //&& !combinations[i].Contains("9 10 11")
                //&& !combinations[i].Contains("10 11 12")
                //&& !combinations[i].Contains("11 12 13")
                //&& !combinations[i].Contains("15 16 17")
                //&& !combinations[i].Contains("16 17 18")
                //&& !combinations[i].Contains("25 26 27")
                //&& !combinations[i].Contains("26 27 28")
                //&& !combinations[i].Contains("27 28 29")
                //&& !combinations[i].Contains("28 29 30")
                //&& !combinations[i].Contains("1 15")
                //&& !combinations[i].Contains("1 16")
                //&& !combinations[i].Contains("1 17")
                //&& !combinations[i].Contains("1 18")
                //&& !combinations[i].Contains("1 20")
                //&& !combinations[i].Contains("1 22")
                //&& !combinations[i].Contains("1 23")
                //&& !combinations[i].Contains("1 25")
                //&& !combinations[i].Contains("1 26")
                //&& !combinations[i].Contains("1 27")
                //&& !combinations[i].Contains("1 28")
                //&& !combinations[i].Contains("1 29")
                //&& !combinations[i].Contains("1 30")
                //&& !combinations[i].Contains("1 35")
                //&& !combinations[i].Contains("7 20")
                //&& !combinations[i].Contains("7 22")
                //&& !combinations[i].Contains("7 23")
                //&& !combinations[i].Contains("7 25")
                //&& !combinations[i].Contains("7 26")
                //&& !combinations[i].Contains("7 27")
                //&& !combinations[i].Contains("7 28")
                //&& !combinations[i].Contains("7 29")
                //&& !combinations[i].Contains("7 30")
                //&& !combinations[i].Contains("7 35")
                //&& !combinations[i].Contains("8 20")
                //&& !combinations[i].Contains("8 22")
                //&& !combinations[i].Contains("8 23")
                //&& !combinations[i].Contains("8 25")
                //&& !combinations[i].Contains("8 26")
                //&& !combinations[i].Contains("8 27")
                //&& !combinations[i].Contains("8 28")
                //&& !combinations[i].Contains("8 29")
                //&& !combinations[i].Contains("8 30")
                //&& !combinations[i].Contains("8 35")
                //&& !combinations[i].Contains("9 20")
                //&& !combinations[i].Contains("9 22")
                //&& !combinations[i].Contains("9 23")
                //&& !combinations[i].Contains("9 25")
                //&& !combinations[i].Contains("9 26")
                //&& !combinations[i].Contains("9 27")
                //&& !combinations[i].Contains("9 28")
                //&& !combinations[i].Contains("9 29")
                //&& !combinations[i].Contains("9 30")
                //&& !combinations[i].Contains("9 35")
                //&& !combinations[i].Contains("10 20")
                //&& !combinations[i].Contains("10 22")
                //&& !combinations[i].Contains("10 23")
                //&& !combinations[i].Contains("10 25")
                //&& !combinations[i].Contains("10 26")
                //&& !combinations[i].Contains("10 27")
                //&& !combinations[i].Contains("10 28")
                //&& !combinations[i].Contains("10 29")
                //&& !combinations[i].Contains("10 30")
                //&& !combinations[i].Contains("10 35")
                //&& !combinations[i].Contains("11 25")
                //&& !combinations[i].Contains("11 26")
                //&& !combinations[i].Contains("11 27")
                //&& !combinations[i].Contains("11 28")
                //&& !combinations[i].Contains("11 29")
                //&& !combinations[i].Contains("11 30")
                //&& !combinations[i].Contains("11 35")
                //&& !combinations[i].Contains("12 25")
                //&& !combinations[i].Contains("12 26")
                //&& !combinations[i].Contains("12 27")
                //&& !combinations[i].Contains("12 28")
                //&& !combinations[i].Contains("12 29")
                //&& !combinations[i].Contains("12 30")
                //&& !combinations[i].Contains("12 35")
                //&& !combinations[i].Contains("13 25")
                //&& !combinations[i].Contains("13 26")
                //&& !combinations[i].Contains("13 27")
                //&& !combinations[i].Contains("13 28")
                //&& !combinations[i].Contains("13 29")
                //&& !combinations[i].Contains("13 30")
                //&& !combinations[i].Contains("13 35")
                //&& !combinations[i].Contains("15 28")
                //&& !combinations[i].Contains("15 29")
                //&& !combinations[i].Contains("15 30")
                //&& !combinations[i].Contains("15 35")
                //&& !combinations[i].Contains("16 35")
                //&& !combinations[i].Contains("17 35")
                //&& !combinations[i].Contains("18 35")
                //&& !combinations[i].Contains("19 35")
                //)
                //{
                    outputTextBoxADV.AppendText($"{combinations[i]}\n");
                    count++;
                //}
            }
            CombinationsCount.Content = $"Count: {count}";
        }

        int[][] Combine()
        {
            string fileContent = FetchFileFromUrl();
            int[][] wonDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            List<Tuple<int, int>> tupleList = GetControlCombsLengthAndAmount(wonDrawsArray);

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBoxADV.Text, @"(?=\s)"));
            var combinationsSix = chosenNumbers.Combinations(6);
            int[][] tempCombinationsSixArrayInt = CustomArray.CreateCombinationsArray(combinationsSix, 6);
            //int[][] combinationsArray = new int[tempCombinationsSixArrayInt.Length][];

            for (int i = 0; i < tupleList.Count; i++)
            {
                tempCombinationsSixArrayInt = CreateCombinationsArrayToBeDisplayed(chosenNumbers, tempCombinationsSixArrayInt, tupleList, i, wonDrawsArray);
            }

            return tempCombinationsSixArrayInt;
        }

        List<Tuple<int, int>> GetControlCombsLengthAndAmount(int[][] wonDrawsArray)
        {
            List<Tuple<int, int>> tupleList = new List<Tuple<int, int>>();

            Dictionary<CheckBox, bool?> checkBoxesADV = new Dictionary<CheckBox, bool?>()
            {
                { TwosCheckBoxADV, TwosCheckBoxADV.IsChecked },
                { ThreesCheckBoxADV, ThreesCheckBoxADV.IsChecked },
                { FoursCheckBoxADV, FoursCheckBoxADV.IsChecked },
                { FivesCheckBoxADV, FivesCheckBoxADV.IsChecked },
                { SixsCheckBoxADV, SixsCheckBoxADV.IsChecked },
            };

            foreach (KeyValuePair<CheckBox, bool?> pair in checkBoxesADV)
            {
                if (pair.Value == true)
                {
                    if (pair.Key == TwosCheckBoxADV)
                        tupleList.Add(Tuple.Create(2, HowManyDrawsConsider(TwosHowManyDrawsTextBox.Text, wonDrawsArray)));
                    else if (pair.Key == ThreesCheckBoxADV)
                        tupleList.Add(Tuple.Create(3, HowManyDrawsConsider(ThreesHowManyDrawsTextBox.Text, wonDrawsArray)));
                    else if (pair.Key == FoursCheckBoxADV)
                        tupleList.Add(Tuple.Create(4, HowManyDrawsConsider(FoursHowManyDrawsTextBox.Text, wonDrawsArray)));
                    else if (pair.Key == FivesCheckBoxADV)
                        tupleList.Add(Tuple.Create(5, HowManyDrawsConsider(FivesHowManyDrawsTextBox.Text, wonDrawsArray)));
                    else if (pair.Key == SixsCheckBoxADV)
                        tupleList.Add(Tuple.Create(6, HowManyDrawsConsider(SixsHowManyDrawsTextBox.Text, wonDrawsArray)));
                }
            }

            return tupleList;
        }

        int HowManyDrawsConsider(string stringToParse, int[][] wonDrawsArray)
        {
            int howManyDrawsConsider = 0;

            bool success = int.TryParse(stringToParse, out int parsedValue);

            if (success)
            {
                howManyDrawsConsider = parsedValue;
            }

            else
            {
                howManyDrawsConsider = wonDrawsArray.Length;
            }

            return howManyDrawsConsider;
        }

        int[][] CreateCombinationsArrayToBeDisplayed(int[] chosenNumbers, int[][] tempCombArray, List<Tuple<int, int>> tupleList, int index, int[][] wonDrawsArray)
        {
            // Build combinations of five, four, three numbers
            var combinationsShort = chosenNumbers.Combinations(tupleList[index].Item1);

            // Create control array of five, four, three numbers
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsShort, tupleList[index].Item1);

            // Create temp array of draws
            //int[][] controlDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.control, tempControlArrayInt, wonDrawsArray, tempControlArrayInt.Length, tupleList[index].Item2, tupleList[index].Item1);
            //Filter array
            int[][] finalControlArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(tempControlArray);

            //Create temporary combinations array to be filtered
            int[][] tempCombinationsArray = CustomArray.CompareArrays(tempCombArray, finalControlArrayFiltered, tempCombArray.Length, tupleList[index].Item1);

            //Prepare final combinations array
            int[][] finalCombinationsArrayFiltered;

            if (EvensOddsCheckBoxADV.IsChecked == true)
            {
                //Remove evens-only or odds-only combinations
                int[][] combinationsArrayWithoutEvensOnlyOrOddsOnly = CustomArray.RemoveEvensOrOddsOnlyComb(tempCombinationsArray);
                //Pull out nulls
                finalCombinationsArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(combinationsArrayWithoutEvensOnlyOrOddsOnly);
            }

            else
            {
                //Pull out nulls
                finalCombinationsArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(tempCombinationsArray);
            }

            return finalCombinationsArrayFiltered;
        }

        List<int> GetNumbersToInclude()
        {
            List<int> includedList = new List<int>();
            if (int.TryParse(Include1TextBox.Text, out int parsedValue) && parsedValue >= 1 && parsedValue <= 37)
                includedList.Add(parsedValue);
            if (int.TryParse(Include2TextBox.Text, out parsedValue) && parsedValue >= 1 && parsedValue <= 37)
                includedList.Add(parsedValue);
            if (int.TryParse(Include3TextBox.Text, out parsedValue) && parsedValue >= 1 && parsedValue <= 37)
                includedList.Add(parsedValue);
            if (int.TryParse(Include4TextBox.Text, out parsedValue) && parsedValue >= 1 && parsedValue <= 37)
                includedList.Add(parsedValue);
            if (int.TryParse(Include5TextBox.Text, out parsedValue) && parsedValue >= 1 && parsedValue <= 37)
                includedList.Add(parsedValue);

            return includedList;
        }

        private void ClearCombinationsButtonADV_Click(object sender, RoutedEventArgs e)
        {
            outputTextBoxADV.Clear();
        }

        int[][] ForceInclude(int[][] combArrayUnfiltered, List<int> includedList)
        {
            int[][] tempArrayInclude = new int[combArrayUnfiltered.Length][];

            for (int i = 0; i < combArrayUnfiltered.Length; i++)
            {
                int test = 0;
                foreach (int item in includedList)
                {
                    if (Array.Exists(combArrayUnfiltered[i], control => control == item))
                    {
                        test++;
                    }

                    if (test == includedList.Count)
                    {
                        tempArrayInclude[i] = combArrayUnfiltered[i];
                    }

                    else
                    {
                        tempArrayInclude[i] = null;
                    }
                }
            }
            return CustomArray.ReduceArrayByPushingOutNulls(tempArrayInclude);
        }
    }
}
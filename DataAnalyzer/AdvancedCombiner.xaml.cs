using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            string fileContent = FetchFileFromUrl();
            int[][] wonDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            List<Tuple<int, int>> tupleList = GetControlCombsLengthAndAmount(wonDrawsArray);

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBoxADV.Text, @"(?=\s)"));
            var combinationsSix = chosenNumbers.Combinations(6);
            int[][] tempCombinationsSixArrayInt = CustomArray.CreateCombinationsArray(combinationsSix, 6);

            for (int i = 0; i < tupleList.Count; i++)
            {
                tempCombinationsSixArrayInt = CreateCombinationsArrayToBeDisplayed(chosenNumbers, tempCombinationsSixArrayInt, tupleList, i, wonDrawsArray);
            }

            //Display
            int count = 0;
            for (int i = 0; i < tempCombinationsSixArrayInt.Length; i++)
            {
                string combination = "";
                for (int j = 0; j < tempCombinationsSixArrayInt[i].Length; j++)
                {
                    combination += $"{tempCombinationsSixArrayInt[i][j].ToString()} ";
                }

                //For: 1 2 8 10 11 12 14 18 22 25 26 27 29 31 33 35

                if (combination.Contains("11")
                && !combination.Contains("11 25")
                && !combination.Contains("11 26")
                && !combination.Contains("11 27")
                && !combination.Contains("11 29")
                && !combination.Contains("11 31")
                && !combination.Contains("11 33")
                && !combination.Contains("11 35")
                && !combination.Contains("12 25")
                && !combination.Contains("12 26")
                && !combination.Contains("12 27")
                && !combination.Contains("12 29")
                && !combination.Contains("12 31")
                && !combination.Contains("12 33")
                && !combination.Contains("12 35")
                && !combination.Contains("14 27")
                && !combination.Contains("14 29")
                && !combination.Contains("14 31")
                && !combination.Contains("14 33")
                && !combination.Contains("14 35")
                && !(combination.Contains("10") && combination.Contains("11") && combination.Contains("12"))
                && !(combination.Contains("25") && combination.Contains("26") && combination.Contains("27"))
                )
                {
                    count++;
                    outputTextBoxADV.AppendText($"{combination}\n");
                }
            }
            CombinationsCount.Content = $"Count: {count}";
            count = 0;
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

        private void ClearCombinationsButtonADV_Click(object sender, RoutedEventArgs e)
        {
            outputTextBoxADV.Clear();
        }
    }
}
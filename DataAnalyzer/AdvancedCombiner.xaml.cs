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
            Combine();
        }

        void Combine()
        {
            string fileContent = FetchFileFromUrl();

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
                        tupleList.Add(Tuple.Create(2, int.Parse(TwosHowManyDrawsTextBox.Text)));
                    else if (pair.Key == ThreesCheckBoxADV)
                        tupleList.Add(Tuple.Create(3, int.Parse(ThreesHowManyDrawsTextBox.Text)));
                    else if (pair.Key == FoursCheckBoxADV)
                        tupleList.Add(Tuple.Create(4, int.Parse(FoursHowManyDrawsTextBox.Text)));
                    else if (pair.Key == FivesCheckBoxADV)
                        tupleList.Add(Tuple.Create(5, int.Parse(FivesHowManyDrawsTextBox.Text)));
                    else if (pair.Key == SixsCheckBoxADV)
                        tupleList.Add(Tuple.Create(6, int.Parse(SixsHowManyDrawsTextBox.Text)));
                }
            }

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBoxADV.Text, @"(?=\s)"));
            var combinationsSix = chosenNumbers.Combinations(6);
            int[][] tempCombinationsSixArrayInt = CustomArray.CreateCombinationsArray(combinationsSix, 6);

            for (int i = 0; i < tupleList.Count; i++)
            {
                tempCombinationsSixArrayInt = CreateCombinationsArrayToBeDisplayed(chosenNumbers, tempCombinationsSixArrayInt, tupleList, i, fileContent);
            }

            //Display
            for (int i = 0; i < tempCombinationsSixArrayInt.Length; i++)
            {
                string combination = "";
                for (int j = 0; j < tempCombinationsSixArrayInt[i].Length; j++)
                {
                    combination += $"{tempCombinationsSixArrayInt[i][j].ToString()} ";
                }

                //For: 1 2 6 8 11 14 15 16 18 20 27 29 30 31 34 36 37

                //if (!combination.Contains("29 30 31")
                //&& !combination.Contains("11 37")
                //)
                //{
                    outputTextBoxADV.AppendText($"{combination}\n");
                //}
            }
            CombinationsCount.Content = $"Count: {tempCombinationsSixArrayInt.Length}";
        }

        int[][] CreateCombinationsArrayToBeDisplayed(int[] chosenNumbers, int[][] tempCombArray, List<Tuple<int, int>> tupleList, int index, string fileContent)
        {
            
            // Build combinations of five, four, three numbers
            var combinationsShort = chosenNumbers.Combinations(tupleList[index].Item1);

            // Create control array of five, four, three numbers
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsShort, tupleList[index].Item1);

            // Create temp array of draws
            int[][] controlDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.control, tempControlArrayInt, controlDrawsArray, tempControlArrayInt.Length, tupleList[index].Item2, tupleList[index].Item1);
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
                //Push out nulls
                finalCombinationsArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(combinationsArrayWithoutEvensOnlyOrOddsOnly);
            }

            else
            {
                //Push out nulls
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

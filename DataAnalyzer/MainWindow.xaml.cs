using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using static System.Windows.Visibility;

namespace DataAnalyzer
{
    //=================================//
    //Prefix E marks enums
    //Names of CONSTANTS are written in CAPITALS
    //=================================//

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            inputTextBox.Text = FetchFileFromUrl();
            FetchFileFromUrl();

            //ADV
            DataBinding dataBinding = new DataBinding();
            TwosCheckBoxADV.DataContext = dataBinding;
            IgnoreLastCheckBoxADV.DataContext = dataBinding;
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

        private string GetFileContentAsString()
        {
            string fileContent = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                fileContent = File.ReadAllText(openFileDialog.FileName);
            }

            return fileContent;
        }

        private void DisplayFileContent()
        {
            inputTextBox.Clear();
            outputTextBox.Clear();
            inputTextBox.Text = FetchFileFromUrl();
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayFileContent();
        }

        private void PrepareGUIforTextBoxes()
        {
            dataGridView.Visibility = Hidden;
            outputTextBox.Visibility = Visible;
            outputTextBox.Clear();
        }

        private void PrepareGUIforTableView()
        {
            outputTextBox.Visibility = Hidden;
            dataGridView.Visibility = Visible;
            outputTextBox.Clear();
        }

        private void CalculateRateButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTableView();
            DisplayFileContent();
            string fileContent = FetchFileFromUrl();
            int maxNumber = GetMaxNumber();

            dataGridView.ItemsSource = new RatingCombiner(fileContent, maxNumber).GetView();
        }

        private void CalculateAvgSpanButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();
            DisplayFileContent();

            string fileContent = FetchFileFromUrl();
            int maxNumber = GetMaxNumber();

            string lowerStepsLimitString = lowerStepsLimitTextBox.Text;
            string upperStepsLimitString = upperStepsLimitTextBox.Text;

            int[][] jumpsArray = new AvgSpanCombiner(fileContent, maxNumber, lowerStepsLimitString, upperStepsLimitString).GetJumpsArray();

            for (int i = 0; i < jumpsArray.Length; i++)
            {
                outputTextBox.AppendText($"{i + 1} appears every:\n");
                for (int j = 0; j < jumpsArray[i].Length; j++)
                {
                    outputTextBox.AppendText($"{jumpsArray[i][j]} ");
                }
                outputTextBox.AppendText("\n");

                IEnumerable<double> query = jumpsArray[i].Select(avg => jumpsArray[i].Average());

                outputTextBox.AppendText($"AVG span = {Math.Round(query.First(), 2)}\n");
            }
        }

        private void ShowDrawsButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();
            DisplayFileContent();

            string fileContent = FetchFileFromUrl();
            int maxNumber = GetMaxNumber();

            string[] drawsArrayToBeDisplayed = new DrawsCombiner(fileContent, maxNumber).GetDrawsStringArray();

            for (int i = 0; i < drawsArrayToBeDisplayed.Length - 1; i++)
            {
                outputTextBox.AppendText($"{drawsArrayToBeDisplayed[i]}\n");
            }
        }

        private void LastAppearanceButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTableView();
            DisplayFileContent();
            string fileContent = FetchFileFromUrl();
            int maxNumber = GetMaxNumber();

            dataGridView.ItemsSource = new LastAppearanceCombiner(fileContent, maxNumber).GetView();

        }

        private int GetMaxNumber()
        {
            bool maxNumberIsInt = int.TryParse(maxNumberTextBox.Text, out int maxNumber);

            if (!maxNumberIsInt)
            {
                const string message = "Please provide the maximal possible number";
                const string caption = "No max. number provided";
                var result = MessageBox.Show(message, caption, MessageBoxButton.OK);
                maxNumberTextBox.Focus();
                return 0;
            }

            else
            {
                return maxNumber;
            }
        }

        private void CheckCombinationButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayFileContent();
            string[] drawsArray = CustomArray.SeparateToLines(GetFileContentAsString());

            string sequenceToCheck = "";
            bool sequenceAlreadyWon = false;
            bool valueIsInteger = false;
            int maxNumber = GetMaxNumber();

            GetMaxNumber();

            Dictionary<TextBox, string> textBoxes = new Dictionary<TextBox, string>()
            {
                { num1TextBox, num1TextBox.Text },
                { num2TextBox, num2TextBox.Text },
                { num3TextBox, num3TextBox.Text },
                { num4TextBox, num4TextBox.Text },
                { num5TextBox, num5TextBox.Text },
                { num6TextBox, num6TextBox.Text },
            };

            foreach (KeyValuePair<TextBox, string> pair in textBoxes)
            {
                valueIsInteger = int.TryParse(pair.Value, out int parsedNumber);

                if (!valueIsInteger || parsedNumber <= 0 || (parsedNumber > maxNumber && maxNumber != 0))
                {
                    const string MESSAGE = "Some entries are invalid";
                    const string CAPTION = "Invalid number";
                    var result = MessageBox.Show(MESSAGE, CAPTION, MessageBoxButton.OK);
                    pair.Key.Focus();
                    break;
                }

                else if (pair.Key != num6TextBox)
                {
                    sequenceToCheck += (pair.Value + "\t");
                }

                else
                {
                    sequenceToCheck += (pair.Value);
                }
            }

            if (valueIsInteger)
            {
                foreach (string item in drawsArray)
                {
                    sequenceAlreadyWon = item.Contains(sequenceToCheck);

                    if (sequenceAlreadyWon)
                    {
                        const string MESSAGE = "This sequence of numbers already won";
                        const string CAPTION = "Combination already won in the past";
                        var result = MessageBox.Show(MESSAGE, CAPTION, MessageBoxButton.OK);
                        break;
                    }
                }

                if (!sequenceAlreadyWon)
                {
                    const string MESSAGE = "Nothing found";
                    const string CAPTION = "Nothing found";
                    var resultNothingFound = MessageBox.Show(MESSAGE, CAPTION, MessageBoxButton.OK);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<TextBox, string> textBoxes = new Dictionary<TextBox, string>()
            {
                { num1TextBox, "" },
                { num2TextBox, "" },
                { num3TextBox, "" },
                { num4TextBox, "" },
                { num5TextBox, "" },
                { num6TextBox, "" }
            };

            foreach (var pair in textBoxes)
            {
                pair.Key.Text = pair.Value;
            }
        }

        private int GetComboBoxValue(int value)
        {
            int comboBoxValue;
            switch (value)
            {
                case 0:
                    comboBoxValue = 2;
                    break;
                case 1:
                    comboBoxValue = 3;
                    break;
                case 2:
                    comboBoxValue = 4;
                    break;
                case 3:
                    comboBoxValue = 5;
                    break;
                case 4:
                    comboBoxValue = 6;
                    break;
                default:
                    comboBoxValue = 4;
                    break;
            }
            return comboBoxValue;
        }

        int HowManyDrawsConsider(int[][] drawsArray)
        {
            int howManyDrawsConsider = 0;

            bool success = int.TryParse(HowManyDrawsTextBox.Text, out int parsedValue);

            if (success)
            {
                howManyDrawsConsider = parsedValue;
            }

            else
            {
                howManyDrawsConsider = drawsArray.Length;
                HowManyDrawsTextBox.Text = drawsArray.Length.ToString();
            }

            return howManyDrawsConsider;
        }

        int[][] CreateCombinationsArrayToBeDisplayed(int combFilter, string fileContent)
        {
            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBox.Text, @"(?=\s)"));

            // Combine numbers
            var combinationsSix = chosenNumbers.Combinations(6);

            // Build combinations of five, four, three numbers
            var combinationsShort = chosenNumbers.Combinations(combFilter);

            // Create temporary combinations array
            int[][] tempCombinationsSixArrayInt = CustomArray.CreateCombinationsArray(combinationsSix, 6);

            // Create control array of five, four, three numbers
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsShort, combFilter);

            // Create temp array of draws
            int[][] controlDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            // Create final array of draws
            int howManyDrawsConsider = HowManyDrawsConsider(controlDrawsArray);

            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.control, outerLoopArray: tempControlArrayInt, innerLoopArray: controlDrawsArray, tempControlArrayInt.Length, startIteration: 0, howManyDrawsConsider, combFilter);
            //Filter array
            int[][] finalControlArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(tempControlArray);

            //Create temporary combinations array to be filtered
            int[][] tempCombinationsArray = CustomArray.CompareArrays(tempCombinationsSixArrayInt, finalControlArrayFiltered, tempCombinationsSixArrayInt.Length, combFilter);

            //Prepare final combinations array
            int[][] finalCombinationsArrayFiltered;

            if (EvensOddsCheckBox.IsChecked == true)
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

        private void PartialCombinationsButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTableView();

            int combFilter = GetComboBoxValue(FilterComboBox.SelectedIndex);

            string fileContent = FetchFileFromUrl();
            inputTextBox.Text = fileContent;

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBox.Text, @"(?=\s)"));

            // Build combinations of five, four, three numbers
            var combinationsFive = chosenNumbers.Combinations(combFilter);

            // Create control array of five
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsFive, combFilter);

            // Create final array of five
            int[][] controlDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            int howManyDrawsConsider = HowManyDrawsConsider(controlDrawsArray);
            //Compare
            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.statistics, outerLoopArray: tempControlArrayInt, innerLoopArray: controlDrawsArray, tempControlArrayInt.Length, startIteration: 0, howManyDrawsConsider, combFilter);

            //Filter array
            int[][] finalControlArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(tempControlArray);

            //Create string array to be displayed via TableView
            string[][] partialCombArray = CustomArray.CreatePartialCombArray(finalControlArrayFiltered);

            //Display
            DataView view = new DataView(Tables.PopulateDataTable(partialCombArray, Tables.ETableType.partial, new string[] { "Combination", "Count" }));
            view.Sort = "Combination ASC";
            dataGridView.ItemsSource = view;
        }

        private void CheckConsequentCombinationsButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();

            string fileContent = FetchFileFromUrl();
            CombCombiner combCombiner = new CombCombiner(GetMaxNumber(), GetComboBoxValue(ConsequentComboBox.SelectedIndex), fileContent);

            inputTextBox.Text = combCombiner.GetCombinationsToBeCheckedAsString();
            outputTextBox.Text = combCombiner.GetComparedCombinationsAsString();
        }

        //==========================
        // A D V
        //==========================

        private void CombineButtonADV_Click(object sender, RoutedEventArgs e)
        {
            if (TwosCheckBoxADV.IsChecked == false)
            {
                IgnoreLastCheckBoxADV.IsChecked = false;
            }

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
                outputTextBoxADV.AppendText($"{combinations[i]}\n");
                count++;
            }
            CombinationsCount.Content = $"Count: {count}";
        }

        int[][] Combine()
        {
            string fileContent = FetchFileFromUrl();
            int[][] wonDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            List<Tuple<int, int>> tupleList = GetControlCombsLengthAndAmount(wonDrawsArray);

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBox.Text, @"(?=\s)"));
            var combinationsSix = chosenNumbers.Combinations(6);
            int[][] tempCombinationsSixArrayInt = CustomArray.CreateCombinationsArray(combinationsSix, 6);

            for (int i = 0; i < tupleList.Count; i++)
            {
                if (IgnoreLastCheckBoxADV.IsChecked == true)
                {
                    tempCombinationsSixArrayInt = CreateCombinationsArrayToBeDisplayed(chosenNumbers, tempCombinationsSixArrayInt, tupleList, i, startIteration: 1, wonDrawsArray);
                }

                else
                {
                    tempCombinationsSixArrayInt = CreateCombinationsArrayToBeDisplayed(chosenNumbers, tempCombinationsSixArrayInt, tupleList, i, startIteration: 0, wonDrawsArray);
                }
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

        int[][] CreateCombinationsArrayToBeDisplayed(int[] chosenNumbers, int[][] tempCombArray, List<Tuple<int, int>> tupleList, int index, int startIteration, int[][] wonDrawsArray)
        {
            // Build combinations of five, four, three numbers
            var combinationsShort = chosenNumbers.Combinations(tupleList[index].Item1);

            // Create control array of five, four, three numbers
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsShort, tupleList[index].Item1);

            // Create temp array of draws
            //int[][] controlDrawsArray = CustomArray.CropArray(CustomArray.CreateIntArrayFromString(fileContent));

            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.control, tempControlArrayInt, wonDrawsArray, tempControlArrayInt.Length, startIteration, tupleList[index].Item2, tupleList[index].Item1);
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

            Dictionary<TextBox, string> textBoxes = new Dictionary<TextBox, string>()
            {
                { Include1TextBox, "" },
                { Include2TextBox, "" },
                { Include3TextBox, "" },
                { Include4TextBox, "" },
                { Include5TextBox, "" }
            };

            foreach (var pair in textBoxes)
            {
                pair.Key.Text = pair.Value;
            }
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
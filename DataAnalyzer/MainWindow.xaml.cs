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
using static System.Windows.Visibility;

namespace DataAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

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
        }

        private string FetchFileFromUrl()
        {
            string fileContent = null;
            try
            {
                var webRequest = WebRequest.Create(@"https://raw.githubusercontent.com/NetanelElimelech/_DataAnalyzer---OOP/master/allDraws.txt");

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    fileContent = reader.ReadToEnd();
                    inputTextBox.AppendText(fileContent);
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
            //inputTextBox.Text = GetFileContentAsString();
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayFileContent();
        }

        private void PrepareGUIforTextBoxes()
        {
            dataGridView.Visibility = Hidden;
            outputTextBox.Visibility = Visible;
            inputTextBox.Clear();
            outputTextBox.Clear();
        }

        private void PrepareGUIforTableView()
        {
            outputTextBox.Visibility = Hidden;
            dataGridView.Visibility = Visible;
            inputTextBox.Clear();
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

            AvgSpanCombiner avgSpanCombiner = new AvgSpanCombiner(fileContent, maxNumber, lowerStepsLimitString, upperStepsLimitString);

            int[][] jumpsArray = new int[maxNumber][];
            double avg = 0;
            int jump = 0;

            int[][] drawsArray = avgSpanCombiner.GetDrawsIntArray();

            for (int i = 0; i < drawsArray.Length; i++)
            {
                avg = 0;
                jumpsArray[i] = new int[drawsArray[i].Length - 1];
                outputTextBox.AppendText($"{i + 1} appears every:\n");

                for (int j = 0; j < jumpsArray[i].Length; j++)
                {
                    jump = drawsArray[i][j] - drawsArray[i][j + 1];
                    if (jump >= avgSpanCombiner.GetLowerStepsLimit() && jump <= avgSpanCombiner.GetUpperStepsLimit())
                    {
                        jumpsArray[i][j] = jump;
                        avg = (avg + jumpsArray[i][j]);
                        outputTextBox.AppendText($"{jumpsArray[i][j]} ");
                    }
                }
                outputTextBox.AppendText($"\nAVG = {Math.Round(avg / (drawsArray[i].Length), 2)}\n\n");
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

        private void CheckSequenceButton_Click(object sender, RoutedEventArgs e)
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
                    const string message = "Some entries are invalid";
                    const string caption = "Invalid number";
                    var result = MessageBox.Show(message, caption, MessageBoxButton.OK);
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
                        const string message = "This sequence of numbers already won";
                        const string caption = "Sequence already won in the past";
                        var result = MessageBox.Show(message, caption, MessageBoxButton.OK);
                        break;
                    }
                }

                if (!sequenceAlreadyWon)
                {
                    const string messageNothingFound = "Nothing found";
                    const string captionNothingFound = "Nothing found";
                    var resultNothingFound = MessageBox.Show(messageNothingFound, captionNothingFound, MessageBoxButton.OK);
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
                { num6TextBox, "" },
            };

            foreach (var pair in textBoxes)
            {
                pair.Key.Text = pair.Value;
            }
        }

        private int GetCombFilterValue()
        {
            int combFilter;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    combFilter = 2;
                    break;
                case 1:
                    combFilter = 3;
                    break;
                case 2:
                    combFilter = 4;
                    break;
                case 3:
                    combFilter = 5;
                    break;
                case 4:
                    combFilter = 6;
                    break;
                default:
                    combFilter = 4;
                    break;
            }
            return combFilter;
        }

        int HowManyDrawsConsider(int maxDrawsToConsider)
        {
            int howManyDrawsConsider = 0;

            bool success = int.TryParse(HowManyDrawsTextBox.Text, out int parsedValue);

            if (success)
            {
                howManyDrawsConsider = parsedValue;
            }

            else
            {
                howManyDrawsConsider = maxDrawsToConsider;
            }

            return howManyDrawsConsider;
        }

        private void CombineButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();

            int combFilter = GetCombFilterValue();

            //string fileContent = GetFileContentAsString();
            string fileContent = FetchFileFromUrl();
            inputTextBox.Text = fileContent;

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
            int[][] tempControlDrawsArray = CustomArray.CreateIntArrayFromString(fileContent);
            int[][] controlDrawsArray = RemoveFirstAndLastElementsFromArray(tempControlDrawsArray);

            int[][] RemoveFirstAndLastElementsFromArray(int[][] inputArray)
            {
                int[][] outputArray = new int[inputArray.Length][];
                for (int i = 0; i < inputArray.Length; i++)
                {
                    outputArray[i] = new int[6];
                    for (int j = 0; j < inputArray[i].Length - 2; j++)
                    {
                        outputArray[i][j] = inputArray[i][j + 1];
                    }
                }
                return outputArray;
            }
            //// Create final array of draws
            //int[][] controlDrawsArray = CustomArray.CreateIntArrayFromString(fileContent); //2 5 8 9 10 11 15 16 22 28 30 32

            int howManyDrawsConsider = HowManyDrawsConsider(controlDrawsArray.Length);

            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.control, tempControlArrayInt, controlDrawsArray, tempControlArrayInt.Length, howManyDrawsConsider, combFilter);
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

            //Display
            for (int i = 0; i < finalCombinationsArrayFiltered.Length; i++)
            {
                string combination = "";
                for (int j = 0; j < finalCombinationsArrayFiltered[i].Length; j++)
                {
                    combination += $"{finalCombinationsArrayFiltered[i][j].ToString()} ";
                }

                //For: 2 5 8 11 14 18 20 24 26 29 30 36
                //if (
                //   !(combination.Contains("35") && combination.Contains("36"))
                ////&& !(combination.Contains("2") && combination.Contains("29"))
                //&& !(combination.Contains("2") && combination.Contains("30"))
                ////&& !(combination.Contains("29") && combination.Contains("30"))
                ////&& !(combination.Contains("11") && combination.Contains("19"))
                ////&& !(combination.Contains("14") && combination.Contains("15"))
                ////&& !(combination.Contains("15") && combination.Contains("16"))
                ////&& !(combination.Contains("18") && combination.Contains("19"))
                ////&& !(combination.Contains("29") && combination.Contains("30"))
                ////&& !(combination.Contains("29") && combination.Contains("32"))
                ////&& !(combination.Contains("30") && combination.Contains("32"))
                //&& !(combination.Contains("11") && combination.Contains("18") && combination.Contains("35"))
                //&& !(combination.Contains("11") && combination.Contains("26") && combination.Contains("36"))
                //&& !(combination.Contains("8") && combination.Contains("14") && combination.Contains("18"))
                //&& !(combination.Contains("8") && combination.Contains("15") && combination.Contains("20"))
                //&& !(combination.Contains("5") && combination.Contains("8") && combination.Contains("14"))
                ////&& !(combination.Contains("2") && combination.Contains("23") && combination.Contains("29"))
                ////&& !(combination.Contains("2") && combination.Contains("23") && combination.Contains("30"))
                ////&& !(combination.Contains("2") && combination.Contains("23") && combination.Contains("32"))
                ////&& !(combination.Contains("16") && combination.Contains("29") && combination.Contains("30"))
                ////&& !(combination.Contains("16") && combination.Contains("29") && combination.Contains("32"))
                ////&& !(combination.Contains("8") && combination.Contains("15") && combination.Contains("25"))
                ////&& !(combination.Contains("5") && combination.Contains("15") && combination.Contains("25"))
                ////&& !(combination.Contains("5") && combination.Contains("8") && combination.Contains("15"))
                ////&& !(combination.Contains("5") && combination.Contains("8") && combination.Contains("25"))
                ////&& !(combination.Contains("2") && combination.Contains("5") && combination.Contains("8"))
                ////&& !(combination.Contains("2 19"))
                ////&& !(combination.Contains("2 21"))
                ////&& !(combination.Contains("5 23"))
                ////&& !(combination.Contains("5 25"))
                //&& !(combination.LastIndexOf("20") == 12)
                //&& !(combination.LastIndexOf("20") == 13)
                //&& !(combination.LastIndexOf("20") == 14)
                //)
                //{
                outputTextBox.AppendText($"{combination}\n");
                //}
            }
        }

        private void ClearCombinationsButton_Click(object sender, RoutedEventArgs e)
        {
            outputTextBox.Clear();
            chosenNumbersTextBox.Clear();
        }

        private void PartialCombinationsButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTableView();

            int combFilter = GetCombFilterValue();

            string fileContent = GetFileContentAsString();
            inputTextBox.Text = fileContent;

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBox.Text, @"(?=\s)"));

            // Build combinations of five, four, three numbers
            var combinationsFive = chosenNumbers.Combinations(combFilter);

            // Create control array of five
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsFive, combFilter);

            // Create final array of five
            int[][] controlDrawsArray = CustomArray.CreateIntArrayFromString(fileContent);

            int howManyDrawsConsider = HowManyDrawsConsider(controlDrawsArray.Length);
            //Compare
            int[][] tempControlArray = CustomArray.CompareArrays(CustomArray.EPurpose.statistics, tempControlArrayInt, controlDrawsArray, tempControlArrayInt.Length, howManyDrawsConsider, combFilter);

            //Filter array
            int[][] finalControlArrayFiltered = CustomArray.ReduceArrayByPushingOutNulls(tempControlArray);

            //Create string array to be displayed via TableView
            string[][] partialCombArray = CustomArray.CreatePartialCombArray(finalControlArrayFiltered);

            //Display
            DataView view = new DataView(Tables.PopulateDataTable(partialCombArray, Tables.ETableType.partial, new string[] { "Combination", "Count" }));
            view.Sort = "Combination ASC";
            dataGridView.ItemsSource = view;
        }
    }
}
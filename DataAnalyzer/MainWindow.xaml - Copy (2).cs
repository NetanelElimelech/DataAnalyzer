using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
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
            inputTextBox.Text = GetFileContentAsString();
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayFileContent();
        }

        private void PrepareGUIforTextBoxes()
        {
            dataGridViewRates.Visibility = Hidden;
            outputTextBox.Visibility = Visible;
            inputTextBox.Clear();
            outputTextBox.Clear();
        }

        private void PrepareGUIforTableView()
        {
            outputTextBox.Visibility = Hidden;
            dataGridViewRates.Visibility = Visible;
            inputTextBox.Clear();
            outputTextBox.Clear();
        }

        private void CalculateRateButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTableView();

            string fileContent = GetFileContentAsString();
            int maxNumber = GetMaxNumber();
            inputTextBox.Text = fileContent;

            dataGridViewRates.ItemsSource = new RatingCombiner(fileContent, maxNumber).GetView();
        }

        private string[] CreateDrawsNumberWonArray(int[][] inputArray)
        {
            drawLabel.Content = "Draw #";
            GetMaxNumber();

            string[] inWhichDrawNumberWonArray = new string[GetMaxNumber()];

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

        private void CalculateAvgSpanButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();

            string fileContent = GetFileContentAsString();
            int maxNumber = GetMaxNumber();
            inputTextBox.Text = fileContent;

            string lowerStepsLimitString = lowerStepsLimitTextBox.Text;
            string upperStepsLimitString = upperStepsLimitTextBox.Text;

            AvgSpanCombiner avgSpanCombiner = new AvgSpanCombiner(fileContent, maxNumber, lowerStepsLimitString, upperStepsLimitString);

            int[][] jumpsArray = new int[maxNumber][];
            double avg = 0;
            int jump = 0;

            int[][] intArrayFromFile = avgSpanCombiner.GetInitArrayFromFile();
            int[][] drawsArray = avgSpanCombiner.GetDrawsIntArray();

            for (int i = 0; i < drawsArray.Length; i++)
            {
                avg = 0;
                jumpsArray[i] = new int[drawsArray[i].Length - 2];
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

            drawLabel.Content = "Draw #";
            string fileContent = GetFileContentAsString();
            int maxNumber = GetMaxNumber();

            inputTextBox.Text = fileContent;

            string[] drawsArray = new DrawsCombiner(fileContent, maxNumber).GetDrawsStringArray();

            //TODO: First put the data in array and then print it
            for (int i = 0; i < drawsArray.Length; i++)
            {
                outputTextBox.AppendText($"{i + 1} appears in:\n{drawsArray[i]}\n");
            }
        }

        private void LastAppearanceButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTableView();
            drawLabel.Content = "Draw #";

            string fileContent = GetFileContentAsString();
            int maxNumber = GetMaxNumber();

            inputTextBox.Text = fileContent;

            dataGridViewRates.ItemsSource = new LastAppearanceCombiner(fileContent, maxNumber).GetView();

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

        private void CombineButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();

            string fileContent = GetFileContentAsString();
            inputTextBox.Text = fileContent;

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBox.Text, @"(?=\s)"));

            // Combine numbers
            var combinationsSix = chosenNumbers.Combinations(6);
            
            // Build combinations of five, four, three numbers
            var combinationsShort = chosenNumbers.Combinations(4);

            // Create temporary combinations array
            int[][] tempCombinationsSixArrayInt = CustomArray.CreateCombinationsArray(combinationsSix);

            // Create control array of five, four, three numbers
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsShort);

            // Create final array of five, four, three numbers
            int[][] controlDrawsArray = CustomArray.CreateIntArrayFromString(fileContent);
            int[][] tempControlArray = CustomArray.CompareArrays(tempControlArrayInt, controlDrawsArray, tempControlArrayInt.Length, CustomArray.EPurpose.control);

            //Filter array
            int[][] finalControlArrayFiltered = CustomArray.ReduceArrayByPullingOutNulls(tempControlArray);

            //Create temporary combinations array to be filtered
            int[][] tempCombinationsArray = CustomArray.CompareArrays(tempCombinationsSixArrayInt, finalControlArrayFiltered, tempCombinationsSixArrayInt.Length, CustomArray.EPurpose.combinations);

            //Display
            int[][] finalCombinationsArrayFiltered = CustomArray.ReduceArrayByPullingOutNulls(tempCombinationsArray);

            for (int i = 0; i < finalCombinationsArrayFiltered.Length; i++)
            {
                string combination = "";
                for (int j = 0; j < finalCombinationsArrayFiltered[i].Length; j++)
                {
                    combination += $"{finalCombinationsArrayFiltered[i][j].ToString()} ";
                }
                if (
                                   !(combination.Contains("5") && combination.Contains("6"))
                                && !(combination.Contains("2") && combination.Contains("30"))
                                && !(combination.Contains("2") && combination.Contains("32"))
                                && !(combination.Contains("11") && combination.Contains("18"))
                                && !(combination.Contains("11") && combination.Contains("19"))
                                && !(combination.Contains("14") && combination.Contains("15"))
                                && !(combination.Contains("15") && combination.Contains("16"))
                                && !(combination.Contains("18") && combination.Contains("19"))
                                && !(combination.Contains("29") && combination.Contains("30"))
                                && !(combination.Contains("29") && combination.Contains("32"))
                                && !(combination.Contains("30") && combination.Contains("32"))
                                && !(combination.Contains("11") && combination.Contains("18") && combination.Contains("25"))
                                && !(combination.Contains("11") && combination.Contains("19") && combination.Contains("25"))
                                && !(combination.Contains("2") && combination.Contains("16") && combination.Contains("29"))
                                && !(combination.Contains("2") && combination.Contains("16") && combination.Contains("30"))
                                && !(combination.Contains("2") && combination.Contains("16") && combination.Contains("32"))
                                && !(combination.Contains("2") && combination.Contains("29") && combination.Contains("30"))
                                && !(combination.Contains("2") && combination.Contains("30") && combination.Contains("32"))
                                && !(combination.Contains("2") && combination.Contains("16") && combination.Contains("23"))
                                && !(combination.Contains("2") && combination.Contains("23") && combination.Contains("29"))
                                && !(combination.Contains("2") && combination.Contains("23") && combination.Contains("30"))
                                && !(combination.Contains("2") && combination.Contains("23") && combination.Contains("32"))
                                && !(combination.Contains("16") && combination.Contains("29") && combination.Contains("30"))
                                && !(combination.Contains("16") && combination.Contains("29") && combination.Contains("32"))
                                && !(combination.Contains("8") && combination.Contains("15") && combination.Contains("25"))
                                && !(combination.Contains("5") && combination.Contains("15") && combination.Contains("25"))
                                && !(combination.Contains("5") && combination.Contains("8") && combination.Contains("15"))
                                && !(combination.Contains("5") && combination.Contains("8") && combination.Contains("25"))
                                && !(combination.Contains("2") && combination.Contains("5") && combination.Contains("8"))
                                && !(combination.Contains("2 19"))
                                && !(combination.Contains("2 21"))
                                && !(combination.Contains("5 23"))
                                && !(combination.Contains("5 25"))
                                && !(combination.LastIndexOf("16") == 12)
                                && !(combination.LastIndexOf("16") == 13)
                                && !(combination.LastIndexOf("18") == 12)
                                && !(combination.LastIndexOf("18") == 13)
                                && !(combination.LastIndexOf("19") == 12)
                                && !(combination.LastIndexOf("19") == 13)
                                && !(combination.LastIndexOf("21") == 12)
                                && !(combination.LastIndexOf("21") == 13)
                                && !(combination.LastIndexOf("21") == 14)
                                && !(combination.LastIndexOf("23") == 12)
                                && !(combination.LastIndexOf("23") == 13)
                                && !(combination.LastIndexOf("23") == 14)
                                && !(combination.LastIndexOf("25") == 12)
                                && !(combination.LastIndexOf("25") == 13)
                                && !(combination.LastIndexOf("25") == 14)
                                )
                {
                    outputTextBox.AppendText($"{combination}\n");
                }
            }
        }

        private void ClearCombinationsButton_Click(object sender, RoutedEventArgs e)
        {
            outputTextBox.Clear();
            chosenNumbersTextBox.Clear();
        }

        private void PartialCombinationsButton_Click(object sender, RoutedEventArgs e)
        {
            PrepareGUIforTextBoxes();

            string fileContent = GetFileContentAsString();
            inputTextBox.Text = fileContent;

            int[] chosenNumbers = CustomArray.ParseStringArray(Regex.Split(chosenNumbersTextBox.Text, @"(?=\s)"));

            // Build combinations of five, four, three numbers
            var combinationsFive = chosenNumbers.Combinations(4);

            // Create control array of five
            int[][] tempControlArrayInt = CustomArray.CreateCombinationsArray(combinationsFive);

            // Create final array of five
            int[][] controlDrawsArray = CustomArray.CreateIntArrayFromString(fileContent);

            //Compare
            int[][] tempControlArray = CustomArray.CompareArrays(tempControlArrayInt, controlDrawsArray, tempControlArrayInt.Length, CustomArray.EPurpose.statistics);

            //Filter array
            int[][] finalControlArrayFiltered = CustomArray.ReduceArrayByPullingOutNulls(tempControlArray);

            //Display
            for (int i = 0; i < finalControlArrayFiltered.Length; i++)
            {
                string combination = "";

                for (int j = 0; j < finalControlArrayFiltered[i].Length; j++)
                {
                    if (finalControlArrayFiltered[i][j] == 0)
                    {
                        combination += "- ";
                    }
                    else
                    {
                        combination += $"{finalControlArrayFiltered[i][j].ToString()} ";
                    }
                }
                outputTextBox.AppendText($"{combination}\n");
            }
        }
    }
}
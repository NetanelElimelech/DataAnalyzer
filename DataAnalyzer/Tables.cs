using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    class Tables
    {
        public enum ETableType { rate, lastAppearance, partial }
        public static int allNumbersCount = 0;

        public static DataTable CreateTable(ETableType tableType, string[] columnNames)
        {
            DataTable table = new DataTable();

            DataColumn column;

            column = new DataColumn();
            if (tableType == ETableType.partial)
            {
                column.DataType = Type.GetType("System.String");
            }
            else
            {
                column.DataType = Type.GetType("System.Int32");
            }
            column.ColumnName = columnNames[0];
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = columnNames[1];
            table.Columns.Add(column);

            if (tableType == ETableType.rate)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = columnNames[2];
                table.Columns.Add(column);
            }
            return table;
        }

        public static DataTable PopulateDataTable(int maxNumber, int[] inputArray, ETableType tableType, string[] columnNames)
        {
            DataTable table = CreateTable(tableType, columnNames);
            DataRow row;

            for (int i = 0; i < maxNumber; i++)
            {
                if (inputArray[i] != 0)
                {
                    row = table.NewRow();
                    row[columnNames[0]] = i + 1;
                    row[columnNames[1]] = inputArray[i];
                    if (tableType == ETableType.rate)
                    {
                        row[columnNames[2]] = $"{Calculators.CalculatePercentRate(inputArray[i], allNumbersCount)}%";
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public static DataTable PopulateDataTable(string[][] inputArray, ETableType tableType, string[] columnNames)
        {
            DataTable table = CreateTable(tableType, columnNames);
            DataRow row;

            for (int i = 0; i < inputArray.Length; i++)
            {
                    row = table.NewRow();
                    row[columnNames[0]] = inputArray[i][0];
                    row[columnNames[1]] = int.Parse(inputArray[i][1]);
                    table.Rows.Add(row);
            }
            return table;
        }
    }
}
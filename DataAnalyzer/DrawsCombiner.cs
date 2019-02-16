using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    class DrawsCombiner : CustomArray
    {
        readonly string[] drawsArray;

        public DrawsCombiner(string fileContent, int maxNumber)
        {
            int[][] intArrayFromFile = CreateIntArrayFromString(fileContent);
            drawsArray = CreateInitialDrawsArray(intArrayFromFile, maxNumber);
        }

        internal string[] GetDrawsStringArray()
        {
            return drawsArray;
        }
    }
}

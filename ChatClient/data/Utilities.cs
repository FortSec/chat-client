using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Utils
{
    static class Utilities
    {
        public static string Margin_EditOneVector(string toEdit, double valueToAdd, string vector)
        {
            double[] splitted = Array.ConvertAll(toEdit.Split(","), s => double.Parse(s));
            IDictionary<string, int> vectorDispatcher = new Dictionary<string, int>();
            vectorDispatcher.Add("top", 0);
            vectorDispatcher.Add("right", 1);
            vectorDispatcher.Add("bottom", 2);
            vectorDispatcher.Add("left", 3);
            splitted[vectorDispatcher[vector]] += valueToAdd;
            return string.Join(",", splitted);
        }
    }
}

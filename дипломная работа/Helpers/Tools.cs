using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace дипломная_работа.Helpers
{
    public class Tools
    {
        static public void SetNumberDecimalSeparator(string NumberDecimalSeparator=".")
        {
            CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo newCulture = new CultureInfo(curCulture.Name);
            newCulture.NumberFormat.NumberDecimalSeparator = NumberDecimalSeparator;
            Thread.CurrentThread.CurrentCulture = newCulture;
        } 
    }
}
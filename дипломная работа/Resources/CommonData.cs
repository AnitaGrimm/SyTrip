using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using дипломная_работа.Helpers;
using дипломная_работа.Model;

namespace дипломная_работа.Resources
{
    public class CommonData
    {
        public static ObservableCollection<Country> Countries;
        public static List<string> NameOfTowns;
        public static List<Airport> Airports;
        public static List<Town> Towns;
        public static List<string> NameOfTowns_rus;
        public static Town UserTown;
        public static string AmadeusAPIapikey = "tTQv0BJmAp5x4t2nhiNGbzJ5kudPpIC0";
        public static CurrencyConverter CurrencyConverter = new CurrencyConverter();
    }
}

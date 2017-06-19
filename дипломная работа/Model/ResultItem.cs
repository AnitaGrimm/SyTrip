using Google.Apis.QPXExpress.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using дипломная_работа.Helpers;
using дипломная_работа.Resources;

namespace дипломная_работа.Model
{
    public class ResultItem
    {
        public TripOption tripOpt;
        public SliceInfo ArrivalInfo;
        public SliceInfo DepartureInfo;
        public double ArrivalCost = 0;
        public Airport ArrivalPlace;
        public DateTime ArrivalDate;
        public DateTime DepatureDate;
        public Airport DeparturePlace;
        public Town Town = null;
        public Airport Airport = null;
        public int DaysCount = 0;
        public double DayCost = 0;
        public List<AmadeusAPI.Room> rooms = new List<AmadeusAPI.Room>();
        
        public override string ToString()
        {
            string res = "";
            string nl = Environment.NewLine;
            if (ArrivalDate != new DateTime())
                res += "Arrival Date: " + ArrivalDate.ToShortDateString() + nl;
            if (DaysCount!=0)
                res += "Days Count: " + DaysCount + nl;
            if (DepatureDate != new DateTime())
                res += "Departure Date: " + DepatureDate.ToShortDateString() + nl;
            res += "Town: " + Town + nl;
            res += "Hotels: " + nl;
            
            //Зесь должно быть остальное...
            return res;
        }
    }
}

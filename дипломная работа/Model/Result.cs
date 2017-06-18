using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.QPXExpress.v1.Data;
using дипломная_работа.Resources;
using дипломная_работа.Helpers;
using System.Text.RegularExpressions;
using System.Threading;

namespace дипломная_работа.Model
{
    public class Result
    {
        public List<ResultItem> Route;
        public DateTime BeginDate;
        public DateTime EndDate;
        private TripOption currenttrip;
        private Querry querry;
        private List<Stop> order;
        public bool IsMinDays = false, IsMinCost = false, IsMinDaysCost = false;
        double minHotelCost=0, maxHotelCost=0;

        public Result(TripOption currenttrip, Querry querry, List<Stop> order)
        {
            Route = new List<ResultItem>();
            this.querry = querry;
            this.order = order;
            this.currenttrip = currenttrip;
            var town = -1;
            for (int i=0; i<order.Count; i++)
            {
                ResultItem res = new ResultItem();
                res.Town = order[i].Town;
                if (town != -1)
                {
                    res.ArrivalCost = ParseRub(currenttrip.SaleTotal);
                    res.tripOpt = currenttrip;
                    res.ArrivalDate = Computer.ParseDateTime(currenttrip.Slice[town].Segment[0].Leg[0].ArrivalTime);
                    res.ArrivalPlace = CommonData.Airports.Where(a => a.Code == currenttrip.Slice[town].Segment[0].Leg[0].Destination).FirstOrDefault();
                    res.ArrivalInfo = currenttrip.Slice[town];
                }
                if (town + 1 < order.Count - 1)
                {
                    res.DepatureDate = Computer.ParseDateTime(currenttrip.Slice[town + 1].Segment[0].Leg[0].DepartureTime);
                    res.DepartureInfo = currenttrip.Slice[town + 1];
                    res.DeparturePlace = CommonData.Airports.Where(a => a.Code == currenttrip.Slice[town+1].Segment[0].Leg[0].Origin).FirstOrDefault();
                }
                town++;
                Route.Add(res);
            }
            this.BeginDate = Route.First().DepatureDate;
            this.EndDate = Route.Last().ArrivalDate;
        }
       
        public void SetHotelsForItems(List<DateRangeForHotel> dates)
        {
            Parallel.ForEach(Route, (item) => item.SetRooms(dates));
            //foreach(var item in Route)
            //{
            //    item.SetRooms(dates);
            //}
            minHotelCost = GetMinHotelCost();
            maxHotelCost = GetMaxHotelCost();
        }
        private double ParseRub(string saleTotal)
        {
            var rx = new Regex("[^0-9]*(?<val>[0-9]*(.[0-9])?)");
            var match = rx.Match(saleTotal);
            return double.Parse(match.Groups["val"].Value);
        }
        public double GetCost()
        {
            double cost = minHotelCost;
            foreach(var item in Route)
            {
                cost += item.ArrivalCost;
            }
            return cost;
        }
        public double GetMinHotelCost()
        {
            double minCost = 0;
            CurrencyConverter cc = CommonData.CurrencyConverter;
            foreach (var item in Route)
            {
                try
                {
                    minCost += item.rooms.Select(roomset => roomset.Sum(room => cc.getRub(room.total_amount).amount)).Min();
                }
                catch
                {

                }
            }
            return minCost;
        }
        public double GetMaxHotelCost()
        {
            double maxCost = 0;
            CurrencyConverter cc = CommonData.CurrencyConverter;
            foreach (var item in Route)
            {
                try
                {
                    maxCost += item.rooms.Select(roomset => roomset.Sum(room => cc.getRub(room.total_amount).amount)).Max();
                }
                catch { }
            }
            return maxCost;
        }
        public string GetDescription()
        {
            string s = "";
            foreach (var val in Route)
            {
                s += val.Town.Name_rus + "(" + (val.ArrivalCost != 0 ? (val.ArrivalCost.ToString() + ", ") : "") + (val.ArrivalDate != (new DateTime()) ? val.ArrivalDate.ToShortDateString() : "") + ((val.ArrivalDate != (new DateTime())) && (val.DepatureDate != (new DateTime())) ? " - " : "") + (val.DepatureDate != (new DateTime()) ? val.DepatureDate.ToShortDateString() : "") + ") ";
            }
            s += "Взрослых: " + querry.AdultsCount + " пенс:" + querry.CeniorsCount + " детей(2-11):" + querry.ChildrenCount + " дети(0-1): с местом - " + querry.InfantSeatCount + ", на руках -" + querry.InfantLapCount + Environment.NewLine;
            return s;
        }
        public override string ToString()
        {
            string s = "";
            foreach(var val in Route)
            {
                s += val.Town.Name_rus+"(" + (val.ArrivalCost!=0? (val.ArrivalCost.ToString()+", "):"") + (val.ArrivalDate!=(new DateTime())?val.ArrivalDate.ToShortDateString():"")+ ((val.ArrivalDate != (new DateTime()))&& (val.DepatureDate!= (new DateTime())) ? " - ":"") + (val.DepatureDate != (new DateTime()) ? val.DepatureDate.ToShortDateString():"") + ") ";
            }
            var ticketsCost = GetCost();
            s += Environment.NewLine + "Стоимость отелей: "  + string.Format("{0:f2}", minHotelCost) + " - " + string.Format("{0:f2}", maxHotelCost) + " RUB" + Environment.NewLine;
            s += "Итого: " + string.Format("{0:f2}",ticketsCost + minHotelCost) + " - " + string.Format("{0:f2}",ticketsCost + maxHotelCost) + " RUB" + Environment.NewLine;
            if (IsMinDays)
                s += "(Макс. кол-во дней)";
            if (IsMinCost)
                s += "(Мин. стоимость)";
            if (IsMinDaysCost)
                s += "(Мин. стоимость на день)";
            return s;
        }
    }
}

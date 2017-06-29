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
        public TripOption currenttrip;
        private Querry querry;
        private List<Stop> order;
        public double HotelCost =0;
        public double FlightCostTotal = 0;
        public double AverageHotelRating = 0;
        public string Class;
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
                FlightCostTotal = ParseRub(currenttrip.SaleTotal);
                if(FlightCostTotal==0)
                {
                    FlightCostTotal = -1;
                }
                if (town != -1)
                {
                    res.tripOpt = currenttrip;
                    try
                    {
                        res.ArrivalDate = Computer.ParseDateTime(currenttrip.Slice[town].Segment.Last().Leg[0].ArrivalTime);
                        res.ArrivalPlace = CommonData.Airports.Where(a => a.Code == currenttrip.Slice[town].Segment.Last().Leg[0].Destination).FirstOrDefault();
                    }
                    catch { }
                    res.ArrivalInfo = currenttrip.Slice[town];
                }
                if (town + 1 < order.Count - 1)
                {
                    try
                    {
                        res.DepatureDate = Computer.ParseDateTime(currenttrip.Slice[town + 1].Segment.First().Leg[0].DepartureTime);
                        res.DeparturePlace = CommonData.Airports.Where(a => a.Code == currenttrip.Slice[town + 1].Segment.First().Leg[0].Origin).FirstOrDefault();
                    }
                    catch { }
                    res.DepartureInfo = currenttrip.Slice[town + 1];
                }
                town++;
                Route.Add(res);
            }
            this.BeginDate = Route.First().DepatureDate;
            this.EndDate = Route.Last().ArrivalDate;
        }
        public Result(Result res)
        {
            BeginDate = res.BeginDate;
            EndDate = res.EndDate;
            currenttrip = res.currenttrip;
            querry = res.querry;
            order = res.order;
            HotelCost = res.HotelCost;
            FlightCostTotal = res.FlightCostTotal;
            AverageHotelRating = res.AverageHotelRating;
            Route = res.Route.Select(x => new ResultItem() { ArrivalDate = x.ArrivalDate, ArrivalInfo = x.ArrivalInfo, ArrivalPlace = x.ArrivalPlace, DayCost = x.DayCost, DaysCount = x.DaysCount, DepartureInfo = x.DepartureInfo, DeparturePlace = x.DeparturePlace, DepatureDate = x.DepatureDate, rooms = x.rooms, Town = x.Town, tripOpt = x.tripOpt }).ToList();
        }
        
        private double ParseRub(string saleTotal)
        {
            Tools.SetNumberDecimalSeparator();
            var rx = new Regex("[^0-9]*(?<val>[0-9]*(.[0-9])?)");
            var match = rx.Match(saleTotal);
            return double.Parse(match.Groups["val"].Value);
        }
        public double GetCost()
        {
            return HotelCost + FlightCostTotal;
        }
        public double GetHotelCost()
        {
            double minCost = 0;
            CurrencyConverter cc = CommonData.CurrencyConverter;
            foreach (var item in Route)
            {
                try
                {
                    minCost += item.rooms.Sum(room => cc.getRub(room.total_amount).amount);
                }
                catch
                {

                }
            }
            return minCost;
        }
        
        public string GetDescription()
        {
            string s = "";
            foreach (var val in Route)
            {
                s += val.Town.Name_rus + "(" + (val.ArrivalDate != (new DateTime()) ? val.ArrivalDate.ToShortDateString() : "") + ((val.ArrivalDate != (new DateTime())) && (val.DepatureDate != (new DateTime())) ? " - " : "") + (val.DepatureDate != (new DateTime()) ? val.DepatureDate.ToShortDateString() : "") + ") ";
            }
            s += "Взрослых: " + querry.AdultsCount + " пенс:" + querry.CeniorsCount + " детей(2-11):" + querry.ChildrenCount + " дети(0-1): с местом - " + querry.InfantSeatCount + ", на руках -" + querry.InfantLapCount + Environment.NewLine;
            return s;
        }
        public override string ToString()
        {
            string s = "";
            foreach(var val in Route)
            {
                s += val.Town.Name_rus+"(" + (val.ArrivalDate!=(new DateTime())?val.ArrivalDate.ToShortDateString():"")+ ((val.ArrivalDate != (new DateTime()))&& (val.DepatureDate!= (new DateTime())) ? " - ":"") + (val.DepatureDate != (new DateTime()) ? val.DepatureDate.ToShortDateString():"") + ") ";
            }
            var ticketsCost = GetCost();
            s += Environment.NewLine + "Стоимость отелей: "  + string.Format("{0:f2}", HotelCost) + " RUB" + Environment.NewLine;
            s += "Итого: " + string.Format("{0:f2}",ticketsCost + HotelCost) + " RUB" + Environment.NewLine;
            return s;
        }

        public double GetAverageHotelRating()
        {
            var a = Route.Where(r => r.AverageHotelRating!=0)?.ToList();
            if (a != null && a.Count != 0)
                return a.Average(r => r.AverageHotelRating);
            return 0;
        }
    }
}

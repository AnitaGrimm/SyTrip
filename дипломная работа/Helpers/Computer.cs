using Google.Apis.QPXExpress.v1;
using Google.Apis.QPXExpress.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using дипломная_работа.Model;
using дипломная_работа.Resources;

namespace дипломная_работа.Helpers
{
    class Computer
    {
        int servicecount = 0;
        public Computer() { }
        List<QPXExpressService> service = new List<Google.Apis.QPXExpress.v1.QPXExpressService>(){
            new QPXExpressService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyB7fe5nAryZH3YY2XWGrklZehJdx-g6C1U",
            ApplicationName = "Daimto QPX Express Sample",
        }),
        new QPXExpressService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyAsC20UoxAGKrk5tCBlth__Xbo6CHNUuOc",
            ApplicationName = "Daimto QPX Express Sample",
        }),
        new QPXExpressService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCD5lsCVSDXP5BeECQVmc-IIkPPnVLcJCA",
            ApplicationName = "Daimto QPX Express Sample",
        }),
        new QPXExpressService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyBiiWbU4hT_GIhOCw-TRSp63gHaerGai-0",
            ApplicationName = "Daimto QPX Express Sample",
        }),
        new QPXExpressService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyDYs6bL5N98l_KgMyAnkgop4xtzPADVA9I",
            ApplicationName = "Daimto QPX Express Sample",
        }),
        new QPXExpressService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyDuOJxymb3TqDopvo_eGrk2EOga-_sWz-g",
            ApplicationName = "Daimto QPX Express Sample",
        })
        };
        WebClient wb = new WebClient();
        Random rnd = new Random();

        public List<Result> DoCompute(Querry Querry, ProgressBar pb)
        {
            TripsSearchRequest request = new TripsSearchRequest();
            request.Request = new TripOptionsRequest();
            request.Request.Passengers = new PassengerCounts { AdultCount = Querry.AdultsCount, ChildCount = Querry.ChildrenCount, SeniorCount = Querry.CeniorsCount, InfantInLapCount = Querry.InfantLapCount, InfantInSeatCount = Querry.InfantSeatCount };
            request.Request.MaxPrice = "RUB"+Querry.Budget.ToString();

            List<List<Stop>> Order = new List<List<Stop>>();
            var slices = GetList(Querry, ref Order);
            var result = new List<Result>();
            int i = 0;
            var val = 100 / (double)(slices.Count+2);
            foreach(var slice in slices)
            {
                var prov = true;
                while (prov)
                {
                    prov = false;
                    request.Request.Slice = slice;
                    var arr = request.Request.Slice.ToArray();
                    Array.Sort(request.Request.Slice.Select(x => ParseDate(x.Date)).ToArray(), arr);
                    request.Request.Slice = arr.ToList();
                    try
                    {
                        var res = service[servicecount].Trips.Search(request).Execute();
                        if (res.Trips.TripOption != null)
                        {
                            result.AddRange(GetResult(Order[i], Querry, res));
                        }
                    }
                    catch
                    {
                        if (servicecount < service.Count)
                        {
                            servicecount = (servicecount + 1);
                            prov = true;
                        }
                        else
                            break;
                    }
                }
                i++;
                pb.Dispatcher.BeginInvoke(new Action(() => { pb.Value += val; }));
                if (servicecount >= service.Count)
                    break;
            }
            var dates = getDates(result, Querry);
            Result[] resarray = result.ToArray();
            Array.Sort(resarray.Select(x => x.GetCost()).ToArray(), resarray);
            resarray = result.Take(200).ToArray();
            pb.Dispatcher.BeginInvoke(new Action(() => { pb.Value += val; }));
            Parallel.ForEach(resarray, (res) => res.SetHotelsForItems(dates));
            pb.Dispatcher.BeginInvoke(new Action(() => { pb.Value += val; }));
            return resarray.ToList();
        }
        public List<DateRangeForHotel> getDates(List<Result> results, Querry querry)
        {
            List<DateRangeForHotel> dates = new List<DateRangeForHotel>();
            Parallel.ForEach(results, (result) =>
            {
                foreach (var item in result.Route)
                {
                    var newdatetime = new DateTime();
                    if (item.DepatureDate != newdatetime && item.ArrivalDate != newdatetime)
                    {
                        if (dates.Where(date => date.IsDateAndTownApproach(item.ArrivalDate, item.DepatureDate, item.Town)).FirstOrDefault() == null)
                            dates.Add(new DateRangeForHotel(item.ArrivalDate, item.DepatureDate, item.Town, querry.AdultsCount, querry.ChildrenCount, querry.RoomCount, querry.IsOneHotel));
                    }
                }
            });
            return dates;
        }
        private List<Result> GetResult(List<Stop> Order, Querry Querry,  TripsSearchResponse tripsSearchResponse)
        {
            return GetResultsOneType(Order, Querry, tripsSearchResponse);
        }
        private List<Result> GetResultsOneType(List<Stop> Order, Querry Querry, TripsSearchResponse tripsSearchResponse)
        {
            List<Result> result = new List<Result>();
            var infos = tripsSearchResponse.Trips.TripOption;
            foreach (var item in infos)
             {
                 var duration = ParseDateTime(item.Slice[item.Slice.Count - 1].Segment[0].Leg[0].ArrivalTime) - ParseDateTime(item.Slice[0].Segment[0].Leg[0].DepartureTime);
                 if (duration.TotalDays <= Querry.MaxDayCount)
                 {
                     var res = new Result(item, Querry, Order);
                     var cost = res.GetCost();
                     if (cost <= Querry.Budget)
                         result.Add(res);
                 }
             }
            return result;
        }
        private bool IsAllowed(TripOption trip, Town Ori, Town Dest, Stop stop, TripOption lasttrip)
        {
            var TripOri = trip.Slice[0].Segment[0].Leg[0].Origin;
            var TripDest = trip.Slice[0].Segment[0].Leg[0].Destination;
            var TripOri1 = trip.Slice[1].Segment[0].Leg[0].Origin;
            var TripDest1 = trip.Slice[1].Segment[0].Leg[0].Destination;
            if (lasttrip != null)
            {
                var mindepDate = ParseDateTime(lasttrip.Slice[0].Segment[0].Leg[0].ArrivalTime);
                var depDate = ParseDateTime(trip.Slice[0].Segment[0].Leg[0].DepartureTime);
                var mindepDate1 = ParseDateTime(lasttrip.Slice[1].Segment[0].Leg[0].ArrivalTime);
                var depDate1 = ParseDateTime(trip.Slice[1].Segment[0].Leg[0].DepartureTime);
                return CompareCode(TripOri, Ori.Code) && CompareCode(TripDest, Dest.Code) && depDate >= mindepDate || CompareCode(TripOri1, Ori.Code) && CompareCode(TripDest1, Dest.Code) && depDate1 >= mindepDate1;
            }
            else
                return CompareCode(TripOri,Ori.Code) && CompareCode(TripDest, Dest.Code) || CompareCode(TripOri1, Ori.Code) && CompareCode(TripDest1, Dest.Code);
        }
        private bool CompareCode(string code1, string code2)
        {
            var A1 = CommonData.Airports?.Where(x => x.Code == code1);
            var A2 = CommonData.Airports?.Where(x => x.Code == code2);
            if (A1 != null && A1.Count() != 0)
                code1 = A1.First().Town_code;
            if (A2 != null && A2.Count() != 0)
                code2 = A2.First().Town_code;
            return code1 == code2;
        }
        public static DateTime ParseDate(string arrivalTime)
        {
            var rx = new Regex("" +
                "(?<year>[0-9]{4})-(?<month>[0-9]{2})-(?<day>[0-9]{2})"
                );
            var match = rx.Match(arrivalTime);
            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            int day = int.Parse(match.Groups["day"].Value);
            return new DateTime(year, month, day);
        }
        public static DateTime ParseDateTime(string arrivalTime)
        {
            var rx = new Regex("" +
                "(?<year>[0-9]{4})-(?<month>[0-9]{2})-(?<day>[0-9]{2})T(?<hour>[0-9]{2}):(?<min>[0-9]{2})(?<belt_sign>[+-])(?<belt_hour>[0-9]{2}):(?<belt_min>[0-9]{2})"
                );
            var match = rx.Match(arrivalTime);
            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            int day = int.Parse(match.Groups["day"].Value);
            int hour = int.Parse(match.Groups["hour"].Value);
            int minute = int.Parse(match.Groups["min"].Value);
            int belthour = int.Parse(match.Groups["belt_hour"].Value);
            int beltmin = int.Parse(match.Groups["belt_min"].Value);
            bool belt_sign = match.Groups["belt_sign"].Value == "+";
            var date = new DateTime(year, month, day, hour,minute,0);
            if (belt_sign)
            {
                date = date.Subtract(new TimeSpan(belthour, beltmin, 0));
            }
            else
            {
                date = date.AddHours(belthour);
                date = date.AddMinutes(beltmin);
            }
            date = date.AddHours(3);
            return date;
        }
        private List<List<SliceInput>> GetList(Querry Querry, ref List<List<Stop>> Order)
        {
            var result = new List<List<SliceInput>>();
            for (int i = 0; i < Math.Pow(10, Querry.Count); i++)
                if (IsAllowed(i, Querry))//+
                {
                    var item = new List<Stop>() { new Stop { Town = Querry.NativeTown, Period = new Interval(0, 0) } };
                    var k = i;
                    while (k > 0)
                    {
                        var n = k % 10;
                        k /= 10;
                        item.Add(Querry[n - 1]);
                    }
                    item.Add(new Stop { Town = Querry.NativeTown, Period = new Interval(0, 0) });
                    result.AddRange(GetListForOneType(i, Querry, ref Order, item));
                    //Order.Add(item);
                }
            return result;
        }
        private bool IsAllowed(int i, Querry Querry)
        {
            int k = 0;
            List<int> values = Querry.Select(x => { k += 1; return k; }).ToList();
            while (i > 0)
            {
                var val = i % 10;
                i /= 10;
                if (values.IndexOf(val) == -1)
                    return false;
                values.Remove(val);
            }
            return values.Count == 0;
        }
        private List<SliceInput> GetEthalon(int l, Querry Querry, List<int> numdays, DateTime DateOfBegining)
        {
            var resultEthalon = new List<SliceInput>();
            var result = new List<SliceInput>();
            int th = -1, pr = -1, i = l;
            List<int> order = new List<int>();
            DateTime date = DateOfBegining;
            SliceInput res;
            while (i > 0)
            {
                pr = th;
                th = i % 10;
                order.Add(th);
                i /= 10;
                res = new SliceInput();
                res.MaxStops = 1;
                if (pr == -1)
                {
                    res.Origin = Querry.NativeTown.Code;
                    res.Destination = Querry[th - 1].Town.Code;
                    res.Date = date.Year + "-" + (date.Month < 10 ? "0" : "") + date.Month + "-" + (date.Day < 10 ? "0" : "") + date.Day;
                }
                else
                {
                    res.Origin = Querry[pr - 1].Town.Code;
                    res.Destination = Querry[th - 1].Town.Code;
                    date = date.AddDays((double)numdays[pr - 1]);
                    res.Date = date.Year + "-" + (date.Month < 10 ? "0" : "") + date.Month + "-" + (date.Day < 10 ? "0" : "") + date.Day;
                }
                resultEthalon.Add(res);
            }
            res = new SliceInput();
            res.MaxStops = 1;
            res.Origin = Querry[th - 1].Town.Code;
            res.Destination = Querry.NativeTown.Code;
            date = date.AddDays((double)numdays[th - 1] + 1);
            res.Date = date.Year + "-" + (date.Month < 10 ? "0" : "") + date.Month + "-" + (date.Day < 10 ? "0" : "") + date.Day;
            resultEthalon.Add(res);
            return resultEthalon;
        }
        private List<List<SliceInput>> GetListForOneType(int i, Querry Querry, ref List<List<Stop>> Order, List<Stop> orderItem)
        {
            var buff = 0;
            List<int> vals = new List<int>();
            List<string> numbers1 = new List<string>();
            List<List<int>> days = new List<List<int>>();
            var sumkomb = Querry.Count != 0 ? 1 : 0;
            foreach (var item in Querry)
            {
                vals.Add(item.Period.From- item.Period.To + 1);
                sumkomb *= vals.Last();
            }

            Random rnd = new Random();
            do
            {
                var buff1 = 0;
                List<int> VALS = new List<int>();
                foreach (var item in Querry)
                    VALS.Add(rnd.Next(item.Period.To, item.Period.From + 1));
                var strbuff = String.Join("-",VALS)+";";
                for (int j = 0; j < numbers1.Count; j++)
                {
                    if (strbuff == numbers1[j])
                    {
                        buff1 = 1;
                    }
                }
                if (buff1 == 0)
                {
                    days.Add(VALS);
                    buff = buff + 1;
                    numbers1.Add(strbuff);
                }
            }
            while (buff < sumkomb);
            var result = new List<List<SliceInput>>();
            var sub = Querry.DateOfBigiinning2.Subtract(Querry.DateOfBigiinning1).TotalDays;
            foreach (var numdays in days)
            {
                for(int d=0; d<=sub;d++)
                    result.Add(GetEthalon(i, Querry, numdays, Querry.DateOfBigiinning1.AddDays(d)));
                Order.Add(orderItem);
            }
            return result.Take(5).ToList();
        }
    }
    public class DateRangeForHotel
    {
        public DateTime From { get; private set; }
        public Town Town { get; private set; }
        public DateTime To { get; private set; }
        public List<List<AmadeusAPI.Room>> Rooms { get; private set; }
        public int AdultsCount { get; private set; }
        public int ChildrenCount { get; private set; }
        public int RoomsCount { get; private set; }
        public bool IsOneHotel { get; private set; }
        public DateRangeForHotel(DateTime From, DateTime To, Town Town, int AdultsCount, int ChildrenCount, int RoomsCount, bool IsOneHotel)
        {
            this.From = new DateTime(From.Year, From.Month, From.Day);
            this.To = new DateTime(To.Year, To.Month, To.Day);
            this.Town = Town;
            this.AdultsCount = AdultsCount;
            this.ChildrenCount = ChildrenCount;
            this.RoomsCount = RoomsCount;
            this.IsOneHotel = IsOneHotel;
            Rooms = MakeResults();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is DateRangeForHotel))
                return false;
            DateRangeForHotel objtocomp = obj as DateRangeForHotel;
            return From == objtocomp.From && To == objtocomp.To && Town == objtocomp.Town;
            
        }
        public double GetDays()
        {
            return To.Subtract(From).TotalDays;
        }
        public bool IncludeNights()
        {
            if(GetDays()>=1)
                return true;
            return false;
        }
        List<List<AmadeusAPI.Room>>  MakeResults()
        {
            AmadeusAPI.Quadrat quadr = new AmadeusAPI.Quadrat(50, new AmadeusAPI.Location { longitude = Town.Coordinates.longitude, latitude = Town.Coordinates.latitude });
            AmadeusAPI.Querry q = new AmadeusAPI.Querry() { apikey = CommonData.AmadeusAPIapikey, all_rooms = false, location = Town.Code, check_in = From, check_out = To, north_east_corner = quadr.north_east_corner, south_west_corner = quadr.south_west_corner };
            var AllResults = AmadeusAPI.Response.GetResponse(q).results;
            var results = new List<List<AmadeusAPI.Room>>();
            List<AmadeusAPI.Room> way = new List<AmadeusAPI.Room>();
            if (IsOneHotel)
            {
                foreach (var hotel in AllResults)
                {
                    List<List<AmadeusAPI.Room>> tempres = new List<List<AmadeusAPI.Room>>();
                    FindResults(ref tempres, hotel.rooms, way, AdultsCount+ChildrenCount, RoomsCount);
                    results.AddRange(tempres);
                }
            }
            else
            {
                List<AmadeusAPI.Room> items = AllResults.SelectMany(res => res.rooms).ToList();
                FindResults(ref results, items, way, AdultsCount + ChildrenCount, RoomsCount);
            }
            return results;
        }
        public void FindResults(ref List<List<AmadeusAPI.Room>> result, List<AmadeusAPI.Room> items, List<AmadeusAPI.Room> way, int peopleCount, int roomsCount)
        {
            foreach (var it in items)
                if (way.IndexOf(it) == -1)
                    if (roomsCount <= 1)
                    {
                        if (peopleCount <= it.room_type_info.number_of_beds)
                        {
                            var r = new List<AmadeusAPI.Room>(way);
                            r.Add(it);
                            result.Add(r);
                        }
                    }
                    else
                    {
                        var r = new List<AmadeusAPI.Room>(way);
                        r.Add(it);
                        FindResults(ref result, items, r, peopleCount - it.room_type_info.number_of_beds, roomsCount - 1);
                    }
        }
        public bool IsDateAndTownApproach( DateTime dateFrom, DateTime dateTo, Town Town)
        {
            if (Town.Code != this.Town.Code)
                return false;
            return From.Year == dateFrom.Year && From.Month == dateFrom.Month && From.Day == dateFrom.Day && To.Day == dateTo.Day && To.Month == dateTo.Month && To.Year == dateTo.Year;
        }
    }
}

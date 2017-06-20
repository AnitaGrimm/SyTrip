using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using дипломная_работа.Model;
using дипломная_работа.Resources;

namespace дипломная_работа.Helpers
{
    public partial class AmadeusAPI
    {
        public static BitmapImage GetRating(double rating)
        {
            return new BitmapImage(new Uri(@"..\Resources\Stars\" + ((int)Math.Round(rating)) + ".gif", UriKind.Relative));
        }
        public class Result
        {
            public string property_code { get; set; }
            public string property_name { get; set; }
            public Location location { get; set; }
            public Address address { get; set; }
            public Price total_price { get; set; }
            public Price min_daily_rate { get; set; }
            public List<Award> awards { get; set; }
            public List<HotelImage> images { get; set; }
            public List<Room> rooms { get; set; }
            public List<Amenity> amenities { get; set; }
            public List<Contact> contacts { get; set; }
            public double rating { get; set; }
            
        }
        public class Award
        {
            public string provider { get; set; }
            public string rating { get; set; }
            public static Award Parse(string s)
            {
                Regex rx = new Regex("([\n"+@"\s"+"\r]*)\"provider\""+@"\s"+"*:"+@"\s"+"*\"(?<provider>[^\"]*)\",(["+@"\s"+"\n\r]*)\"rating\""+@"\s"+"*:"+@"\s"+"*\"(?<rating>[^\"]*)\"(["+@"\s"+"\n\r]*)");
                var match = rx.Match(s);
                try
                {
                    return new Award { provider = match.Groups["provider"].Value, rating = match.Groups["rating"].Value };
                }
                catch
                {
                    return new Award();
                }
            }
            public static List<Award> ParseAwards(string s)
            {
                List<Award> results = new List<Award>();
                Regex rx = new Regex(@"{(?<award>[^}]*)}([,\s\r\n]*)");
                var matches = rx.Matches(s);
                foreach (Match match in matches)
                {
                    var award = Parse(match.Groups["award"].Value);
                    if (award != new Award())
                        results.Add(award);
                }
                return results;
            }
        }
        public class HotelImage
        {
            public string category { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string url { get; set; }
            public BitmapImage GetImage()
            {
                try
                {
                    return new BitmapImage(new Uri(url));
                }
                catch { return new BitmapImage(new Uri(@"..\Resources\image_not_found.jpg", UriKind.Relative)); }
            }
            public static HotelImage Parse(string s)
            {
                Regex rx = new Regex("([\n\r"+@"\s"+"]*)\"category\""+@"\s"+"*:"+@"\s"+"*\"(?<category>[^\"]*)\",([\n"+@"\s"+"\r]*)\"width\""+@"\s"+"*:"+@"\s"+"*(?<width>[0-9]+),([\n\r"+@"\s"+"]*)\"height\""+@"\s"+"*:"+@"\s"+"*(?<height>[0-9]+),([\n\r"+@"\s"+"]*)\"url\""+@"\s"+"*:"+@"\s"+"*\"(?<url>[^\"]*)\"([\r\n"+@"\s"+"]*)");
                var match = rx.Match(s);
                try
                {
                    return new HotelImage { category = match.Groups["category"].Value, width = int.Parse(match.Groups["width"].Value), height = int.Parse(match.Groups["height"].Value), url = match.Groups["url"].Value };
                }
                catch
                {
                    return new HotelImage();
                }
            }
            public static List<HotelImage> ParseHotelImages(string s)
            {
                List<HotelImage> results = new List<HotelImage>();
                if (String.IsNullOrWhiteSpace(s))
                    return results;
                Regex rx = new Regex(@"{(?<image>[^}]*)}([,\s\r\n]*)");
                var matches = rx.Matches(s);
                foreach (Match match in matches)
                {
                    var image = Parse(match.Groups["image"].Value);
                    if (image != new HotelImage())
                        results.Add(image);
                }
                return results;
            }
        }
        public class Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public static Location Parse(string s)
            {
                Regex rx = new Regex("([\n"+@"\s"+"\r]*)\"latitude\""+@"\s"+"*:"+@"\s"+"*(?<latitude>[0-9.-]+),([\n"+@"\s"+"\r]*)\"longitude\""+@"\s"+"*:"+@"\s"+"*(?<longitude>[0-9.-]+)([\n"+@"\s"+"\r]*)");
                var match = rx.Match(s);
                try
                {
                    return new Location { latitude = double.Parse(match.Groups["latitude"].Value), longitude = double.Parse(match.Groups["longitude"].Value) };
                }
                catch
                {
                    return new Location();
                }
            }
            public override string ToString()
            {
                return latitude.ToString() +","+ longitude.ToString();
            }
            public double getDistance(Location location2)
            {
                return 2*Math.Asin(Math.Sqrt(Math.Sin((location2.latitude-latitude)/2)* Math.Sin((location2.latitude - latitude) / 2)+Math.Cos(latitude)*Math.Cos(location2.latitude) * Math.Sin((longitude - location2.longitude) / 2) * Math.Sin((longitude - location2.longitude) / 2))) * 6371;
            }
        }
        public class Address
        {
            public string line1 { get; set; }
            public string city { get; set; }
            public string region { get; set; }
            public string postal_code { get; set; }
            public string country { get; set; }
            public static Address Parse(string s)
            {
                //([\s\n\r]*)"line1"\s*:\s*"(?<line1>[^"]*)",([\s\n\r]*)"city"\s*:\s*"(?<city>[^"]*)",([\s\n\r]*)"postal_code"\s*:\s"(?<postal_code>[^']*)([\s\n\r]*)
                Regex rx = new Regex("(["+@"\s"+"\n\r]*)\"line1\""+@"\s"+"*:"+@"\s"+"*\"(?<line1>[^\"]*)\",(["+@"\s"+"\n\r]*)\"city\""+@"\s"+"*:"+@"\s"+"*\"(?<city>[^\"]*)\",(["+@"\s"+"\n\r]*)\"postal_code\""+@"\s"+"*:"+@"\s"+"\"(?<postal_code>[^']*)(["+@"\s"+"\n\r]*)");
                var match = rx.Match(s);
                try
                {
                    var a = new Address();
                    a.line1 = match.Groups["line1"].Value;
                    a.city = match.Groups["city"].Value;
                    a.postal_code = match.Groups["postal_code"].Value;
                    a.country = match.Groups["country"].Value;
                    return a;
                }
                catch
                {
                    return new Address();
                }
            }
        }
        public class Price
        {
            public double amount { get; set; }
            public string currency { get; set; }
            public static Price Parse(string s)
            {
                Regex rx= new Regex("\"amount\""+@"\s"+"*:"+@"\s"+"\"(?<amount>[^\"]*)\",([\n\r"+@"\s"+"]*)\"currency\""+@"\s"+"*:"+@"\s"+"*\"(?<currency>[^\"]*)\"");
                Match match = rx.Match(s);
                try
                {
                    return new Price { amount = double.Parse(match.Groups["amount"].Value), currency = match.Groups["currency"].Value };
                }
                catch
                {
                    return new Price();
                }
            }
        }
        public class Rate
        {
            public DateTime starting_date { get; set; }
            public DateTime end_date { get; set; }
            public string currency_code { get; set; }
            public double price { get; set; }
            public static List<Rate> ParseRates(ref string s)
            {
                List<Rate> result = new List<Rate>();
                Regex rx = new Regex(@"{([" + @"\s" + "\n\r]*)\"start_date\"" + @"\s" + "*:" + @"\s" + "*\"(?<start_date>[^\"]*)\"," + @"\s" + "*\"end_date\"" + @"\s" + "*:" + @"\s" + "*\"(?<end_date>[^\"]*)\",([" + @"\s" + "\n\r]*)\"currency_code\"" + @"\s" + "*:" + @"\s" + "*\"(?<currency_code>[^\"]*)\",([" + @"\s" + "\n\r]*)\"price\"" + @"\s" + "*:" + @"\s" + "*(?<price>[0-9.]+)([" + @"\s" + "\r\n]*)}");
                var matches = rx.Matches(s);
                foreach (Match match in matches)
                {
                    try
                    {
                        s = s.Replace(match.Value, result.Count.ToString());
                        result.Add(new Rate { currency_code = match.Groups["currency_code"].Value, end_date = ParseDate(match.Groups["end_date"].Value), price = double.Parse(match.Groups["price"].Value), starting_date = ParseDate(match.Groups["start_date"].Value) });
                    }
                    catch { }
                }
                return result;
            }
            public static List<Rate> GetRates(string s, List<Rate> rates)
            {
                List<Rate> result = new List<Rate>();
                Regex rx = new Regex("(?<rate>[0-9]+)");
                var matches = rx.Matches(s);
                foreach(Match match in matches)
                    try
                    {
                        result.Add(rates[int.Parse(match.Groups["rate"].Value)]);
                    }
                    catch
                    {

                    }
                return result;
            }
        }
        static DateTime ParseDate(string s)
        {
            Regex rx = new Regex("(?<year>[0-9]{4})-(?<month>[0-9]{1,2})-(?<day>[0-9]{1,2})");
            var match = rx.Match(s);
            try
            {
                return new DateTime(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value), int.Parse(match.Groups["day"].Value),0,0,0);
            }
            catch
            {
                return new DateTime(0, 0, 0, 0, 0, 0);
            }
        }
        public class RoomTypeInfo
        {
            public string room_type { get; set; }
            public string bed_type { get; set; }
            public int number_of_beds { get; set; }
            public static RoomTypeInfo Parse(string s)
            {
                Regex rx = new Regex("\"room_type\""+@"\s"+"*:"+@"\s"+"*\"(?<room_type>[^\"]*)\",(["+@"\s"+"\r\n]*)\"bed_type\""+@"\s"+"*:"+@"\s"+"*\"(?<bed_type>[^\"]*)\",["+@"\s"+"\r\n]*\"number_of_beds\""+@"\s"+"*:"+@"\s"+"*\"(?<number_of_beds>[^\"]*)\"");
                Match match = rx.Match(s);
                try
                {
                    return new RoomTypeInfo { bed_type = match.Groups["bed_type"].Value, room_type = match.Groups["room_type"].Value, number_of_beds = int.Parse(match.Groups["number_of_beds"].Value) };
                }
                catch
                {
                    return new RoomTypeInfo();
                }
            }
        }
        public class Room
        {
            public Result hotel { get; set; }
            public string booking_code { get; set; }
            public string room_type_code { get; set; }
            public string rate_plan_code { get; set; }
            public Price total_amount { get; set; }
            public List<Rate> rates { get; set; }
            public string description { get; set; }
            public RoomTypeInfo room_type_info { get; set; }
            public string rate_type_code { get; set; }
            public static Room Parse(string s)
            {
                Regex rx = new Regex("([\n\r" + @"\s" + "]*)\"booking_code\"" + @"\s" + "*:" + @"\s" + "*\"(?<booking_code>[^\"]*)\",([\n\r" + @"\s" + "]*)\"room_type_code\"" + @"\s" + "*:" + @"\s" + "*\"(?<room_type_code>[^\"]*)\",([\n\r" + @"\s" + "]*)\"rate_plan_code\"" + @"\s" + "*:" + @"\s" + "*\"(?<rate_plan_code>[^\"]*)\",([" + @"\s" + "\r\n]*)\"total_amount\"" + @"\s" + "*:" + @"\s" + "*{(?<price>[^}]*)},([" + @"\s" + "\n\r]*)\"rates\"" + @"\s" + "*:" + @"\s" + @"*\[(?<rates>[^\]]*)\],([" + @"\s" + "\n\r]*)\"descriptions\"" + @"\s" + "*:" + @"\s" + @"*\[(?<descriptions>[^\]]*)\],([\n\r" + @"\s" + "]*)\"room_type_info\"" + @"\s" + "*:" + @"\s" + "*{(?<room_type_info>[^}]*)},([\n\r" + @"\s" + "]*)\"rate_type_code\"" + @"\s" + "*:" + @"\s" + "*\"(?<rate_type_code>[^\"]*)\"([\n\r" + @"\s" + "]*)");
                var match = rx.Match(s);
                try
                {
                    return new Room { booking_code = match.Groups["booking_code"].Value, room_type_code = match.Groups["room_type_code"].Value, rate_plan_code = match.Groups["rate_plan_code"].Value, total_amount = Price.Parse(match.Groups["price"].Value), rate_type_code = match.Groups["rate_type_code"].Value, room_type_info = RoomTypeInfo.Parse(match.Groups["room_type_info"].Value), description = match.Groups["descriptions"].Value/*, rates = Rate.ParseRates(ref match.Groups["rates"].Value)*/ };
                }
                catch
                {
                    return new Room();
                }
            }
            public static List<Room> ParseRooms(string s, List<Rate> allRates)
            {
                List<Room> results = new List<Room>();
                Regex rx = new Regex("{([\n\r"+@"\s"+"]*)\"booking_code\""+@"\s"+"*:"+@"\s"+"*\"(?<booking_code>[^\"]*)\",([\n\r"+@"\s"+"]*)\"room_type_code\""+@"\s"+"*:"+@"\s"+"*\"(?<room_type_code>[^\"]*)\",([\n\r"+@"\s"+"]*)\"rate_plan_code\""+@"\s"+":"+@"\s"+"\"(?<rate_plan_code>[^\"]*)\",([\n\r"+@"\s"+"]*)\"total_amount\""+@"\s"+"*:"+@"\s"+"*{(?<total_amount>[^}]*)},([\n\r"+@"\s"+"]*)\"rates\""+@"\s"+":"+@"\s"+"{(?<rates>[^}]*)},([\n\r"+@"\s"+"]*)\"descriptions\""+@"\s"+"*:"+@"\s"+"(?<descriptions>[^}]*)},([\n\r"+@"\s"+"]*)\"room_type_info\""+@"\s"+"*:"+@"\s"+"*{(?<room_type_info>[^}]*)},([\n\r"+@"\s"+"]*)\"rate_type_code\""+@"\s"+"*:"+@"\s"+"*\"(?<rate_type_code>[^\"]*)\"([\n\r"+@"\s"+"]*)}");
                var matches = rx.Matches(s);
                foreach (Match match in matches)
                {
                    try
                    {
                        results.Add(new Room { booking_code = match.Groups["booking_code"].Value, room_type_code = match.Groups["room_type_code"].Value, rate_plan_code = match.Groups["rate_plan_code"].Value, total_amount = Price.Parse(match.Groups["total_amount"].Value), rate_type_code = match.Groups["rate_type_code"].Value, room_type_info = RoomTypeInfo.Parse(match.Groups["room_type_info"].Value), description = match.Groups["descriptions"].Value, rates = Rate.GetRates(match.Groups["rates"].Value, allRates) });
                    }
                    catch
                    {

                    }
                }
                return results;
            }
            public override bool Equals(object obj)
            {
                return (obj as Room).booking_code == booking_code;
            }
        }
        public class Amenity
        {
            public string amenity { get; set; }
            public int ota_code { get; set; }
            public string decrtiption { get; set; }
            public static Amenity Parse(string s)
            {
                Regex rx = new Regex(@"\s" + "*\"amenity\"" + @"\s" + ":" + @"\s" + "*\"(?<amenity>[^\"]*)\",([" + @"\s" + "\n\r]*)\"ota_code\"" + @"\s" + "*:" + @"\s" + "*(?<ota_code>[0-9]+),([" + @"\s" + "\n\r]*)\"description\"" + @"\s" + "*:" + @"\s" + "*\"(?<description>[^\"]*)\"([/s/r/n]*)");
                var match = rx.Match(s);
                try
                {
                    return new Amenity { amenity = match.Groups["amenity"].Value, decrtiption = match.Groups["description"].Value, ota_code = int.Parse(match.Groups["ota_code"].Value) };
                }
                catch
                {
                    return new Amenity();
                }
            }
            public static List<Amenity> ParseAmenities(string s)
            {
                List<Amenity> results = new List<Amenity>();
                Regex rx = new Regex(@"{(?<amenity>[^}]*)}([,\s\r\n]*)");
                var matches = rx.Matches(s);
                foreach(Match match in matches)
                {
                    var amenity = Parse(match.Groups["amenity"].Value);
                    if (amenity != new Amenity())
                        results.Add(amenity);
                }
                return results;
            }
        }
        public class Contact
        {
            public string type { get; set; }
            public string detail { get; set; }
            public static Contact Parse(string s)
            {
                Regex rx = new Regex("(["+@"\s"+"\n\r]*)\"type\""+@"\s"+":"+@"\s"+"*\"(?<type>[^\"]*)\",([\n\r"+@"\s"+"]*)\"detail\""+@"\s"+"*:"+@"\s"+"\"(?<detail>[^\"]*)\"([\r"+@"\s"+"\n]*)");
                var match = rx.Match(s);
                try
                {
                    return new Contact { type = match.Groups["type"].Value, detail = match.Groups["detail"].Value };
                }
                catch
                {
                    return new Contact();
                }
            }
            public static List<Contact> ParseContacts(string s)
            {
                List<Contact> results = new List<Contact>();
                Regex rx = new Regex(@"{(?<contact>[^}]*)}([,\s\r\n]*)");
                var matches = rx.Matches(s);
                foreach (Match match in matches)
                {
                    var contact = Parse(match.Groups["contact"].Value);
                    if (contact != new Contact())
                        results.Add(contact);
                }
                return results;
            }
        }
        public class Quadrat
        {
            public Location south_west_corner { get; private set; } = null;
            public Location north_east_corner { get; private set; } = null;
            double radius { get; set; }
            Location townCoord { get; set; }
            public Quadrat(double radius, Location townCoord)
            {
                double radlon = 360 * radius / 40000.0;
                double radlat = 360 * radius / 40000.0 / Math.Abs(Math.Cos(townCoord.latitude));
                south_west_corner = new Location { latitude = townCoord.latitude - radlat, longitude = townCoord.longitude - radlon };
                north_east_corner = new Location { latitude = townCoord.latitude + radlat, longitude = townCoord.longitude + radlon };
            }

        }
        public class Querry
        {
            string InitialURL = @"https://api.sandbox.amadeus.com/v1.2/hotels/search-box?apikey=";
            public string apikey { get; set; } = "";
            public string location { get; set; } = "";
            public DateTime check_in { get; set; } = DateTime.MinValue;
            public DateTime check_out { get; set; } = DateTime.MinValue;
            public int radius { get; set; } = 50;
            public Location south_west_corner { set; get; } = null;
            public Location north_east_corner { set; get; } = null;
            public string lang { get; set; } = "EN";
            public string currency { get; set; } = "USD";
            public string chain { get; set; } = "";
            public double max_rate { get; set; } = 500;
            public int number_of_results { get; set; } = 100;
            public bool all_rooms { get; set; } = true;
            public bool show_sold_out { get; set; } = false;
            public string amenity { get; set; } = "";
            public string GetQuerryURL()
            {
                string url = InitialURL+apikey;
                if (south_west_corner != null && north_east_corner != null)
                    url += "&south_west_corner=" + south_west_corner + "&north_east_corner=" + north_east_corner;
                if (check_in != DateTime.MinValue)
                    url += "&check_in=" + GetDate(check_in);
                if (check_out != DateTime.MinValue)
                    url += "&check_out=" + GetDate(check_out);
                if (lang != "" && lang!=null)
                    url += "&lang=" + lang;
                if (currency != "" && currency!=null)
                    url += "&currency=" + currency;
                if (chain != "" && chain!=null)
                    url += "&chain=" + chain;
                if (max_rate >= 0)
                    url += "&max_rate=" + max_rate;
                if (number_of_results >=0)
                    url += "&number_of_results=" + number_of_results;
                url += "&all_rooms=" + (all_rooms?"true":"false");
                url += "&show_sold_out=" + (show_sold_out ? "true" : "false");
                if (amenity != "" && amenity!=null)
                    url += "&amenity=" + amenity;
                return url;
            }
            
        }
        public static string GetDate(DateTime date)
        {
            return date.Year.ToString() + "-" + (date.Month < 10 ? "0" : "") + date.Month.ToString() + "-" + (date.Day < 10 ? "0" : "") + date.Day.ToString();
        }
        public class Response
        {
            public List<Result> results { get; set; }
            public static Response GetResponse(Querry querry,Town town)
            {
                WebClient wc = new WebClient();
                
                string response = Encoding.UTF8.GetString(wc.DownloadData(querry.GetQuerryURL()));
                return new Response { results = GetResult(response, town) };
            }
            static List<Result> GetResult(string response,Town town)
            {
                var townLocation = new Location() { latitude = town.Coordinates.latitude, longitude = town.Coordinates.longitude };
                List<Rate> allrates = Rate.ParseRates(ref response);
                Regex ratesRX = new Regex("\"rates\"" + @"\s*:\s*\[[^\]]*\]");
                var rates = ratesRX.Matches(response);
                foreach (Match rate in rates)
                    response = response.Replace(rate.Value, rate.Value.Replace('[', '{').Replace(']', '}'));
                Regex descriptRX = new Regex("\"descriptions\""+@"\s*:\s*\[[^\]]*\]");
                var descriptions = descriptRX.Matches(response);
                foreach (Match descroption in descriptions)
                    response = response.Replace(descroption.Value, descroption.Value.Replace('[', '{').Replace(']', '}'));
                List<Result> results = new List<Result>();
                Regex rx = new Regex("{([" + @"\s" + "\r\n]*)\"property_code\"" + @"\s" + "*:" + @"\s" + "*\"(?<property_code>[^\"]*)\",([" + @"\s" + "\r\n]*)\"property_name\"" + @"\s" + "*:" + @"\s" + "*\"(?<property_name>[^\"]*)\",([" + @"\s" + "\r\n]*)\"location\"" + @"\s" + "*:" + @"\s" + "*{(?<location>[^}]*)},([" + @"\s" + "\r\n]*)\"address\"" + @"\s" + "*:" + @"\s" + "*{(?<address>[^}]*)},([" + @"\s" + "\r\n]*)\"total_price\"" + @"\s" + "*:" + @"\s" + "*{(?<total_price>[^}]*)},([" + @"\s" + "\r\n]*)\"min_daily_rate\"" + @"\s" + "*:" + @"\s" + "*{(?<min_daily_rate>[^}]*)},([" + @"\s" + "\r\n]*)\"contacts\"" + @"\s" + "*:" + @"\s" + @"*\[(?<contacts>[^\]]*)\],([" + @"\s" + "\r\n]*)\"amenities\"" + @"\s" + "*:" + @"\s" + @"*\[(?<amenities>[^\]]*)\],([" + @"\s" + "\r\n]*)\"awards\"" + @"\s" + "*:" + @"\s" + @"*\[(?<awards>[^\]]*)\],([" + @"\s" + "\r\n]*)\"images\"" + @"\s" + "*:" + @"\s" + @"*\[(?<images>[^\]]*)\],([" + @"\s" + "\r\n]*)\"rooms\"" + @"\s" + "*:" + @"\s" + @"*\[(?<rooms>[^\]]*)],([" + @"\s" + "\r\n]*)\"_links\"" + @"\s" + "*:" + @"\s" + "{(?<links>[^}]*)}([" + @"\s" + "\r\n]*)}([" + @"\s" + "\r\n]*)}");
                var matches = rx.Matches(response);
                double varfortryparse;
                foreach (Match match in matches)
                    try
                    {
                        var res = new Result() { property_code = match.Groups["property_code"].Value, property_name = match.Groups["property_name"].Value, location = Location.Parse(match.Groups["location"].Value), address = Address.Parse(match.Groups["address"].Value), total_price = Price.Parse(match.Groups["total_price"].Value), min_daily_rate = Price.Parse(match.Groups["min_daily_rate"].Value), contacts = Contact.ParseContacts(match.Groups["contacts"].Value), amenities = Amenity.ParseAmenities(match.Groups["amenities"].Value), awards = Award.ParseAwards(match.Groups["awards"].Value), images = HotelImage.ParseHotelImages(match.Groups["images"].Value), rooms = Room.ParseRooms(match.Groups["rooms"].Value, allrates) };
                        res.rating = (res.awards.Sum(x=> { return double.TryParse(x.rating, out varfortryparse) ? double.Parse(x.rating): 0; }));
                        foreach (var room in res.rooms)
                            room.hotel = res;
                        results.Add(res);
                    }
                    catch { }
                return results;
            }
        }
    }
}

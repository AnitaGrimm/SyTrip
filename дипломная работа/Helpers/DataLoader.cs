using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using дипломная_работа.Model;
using дипломная_работа.Resources;

namespace дипломная_работа.Helpers
{
    static class DataLoader
    {
        #region helpers
        static DbGeography ConvertLatLonToDbGeography(string longitude, string latitude)
        {
            if (latitude == "" || longitude == "")
                return null;
            var point = string.Format("POINT({1} {0})", latitude, longitude);
            return DbGeography.FromText(point);
        }
        #endregion
        public static List<Country> Load(ref ProgressBar pb)
        {
            var WebClient = new WebClient();
            var Countries = LoadCountries(WebClient, ref pb);
            var Towns = LoadTowns(WebClient, ref pb);
            var Airports = LoadAirports(WebClient, ref pb);
            CommonData.Airports = Airports;
            return CompareData(Countries, Towns, Airports, ref pb);
        }

        static List<Country> LoadCountries(WebClient wb,  ref ProgressBar pb)
        {
            if (!WebTools.ConnectionAvailable("http://www.google.com"))
                return null;
            List<Country> Countries = new List<Country>();
            string s = Encoding.UTF8.GetString(wb.DownloadData("http://api.travelpayouts.com/data/countries.json"));
            var rx = new Regex("{\"code\":\"(?<code>[^\"]*)\",\"name\":\"(?<name>[^\"]*)\",\"currency\":((\"(?<currency>[^\"]*)\")|(?<currency>null)),\"name_translations\":{(?<translations>[^}]*)}}");
            var rus = new Regex("\"ru\":\"(?<ru>[^\"]*)\"");
            var matches = rx.Matches(s);
            double pathofstatus = 100 / 5.0 / matches.Count;
            foreach (Match match in matches)
            {
                Country Country = new Country();
                Country.Code = match.Groups["code"].Value;
                Country.Name = match.Groups["name"].Value;
                var rusmatch = rus.Match(match.Groups["translations"].Value);
                if (rusmatch.Success)
                    Country.Name_rus = rusmatch.Groups["ru"].Value;
                Countries.Add(Country);
                var prbar = pb;
                pb.Dispatcher.Invoke(new Action(() => { prbar.Value += pathofstatus; }));
            }
            return Countries;
        }
         static List<Town> LoadTowns(WebClient wb, ref ProgressBar pb)
        {
            if (!WebTools.ConnectionAvailable("http://www.google.com"))
                return null;
            List<Town> Towns = new List<Town>();
            string s = Encoding.UTF8.GetString(wb.DownloadData("http://api.travelpayouts.com/data/cities.json"));
            var rx = new Regex("{\"code\":\"(?<code>[^\"]*)\",\"name\":\"(?<name>[^\"]*)\",\"coordinates\":(({\"lon\":(?<lon>[^,]*),\"lat\":(?<lat>[^}]*)})|(null)),\"time_zone\":\"(?<time_zone>[^\"]*)\",\"name_translations\":{(?<translations>[^}]*)},\"country_code\":\"(?<country_code>[^\"]*)\"}");
            var matches = rx.Matches(s);
            var rus = new Regex("\"ru\":\"(?<ru>[^\"]*)\"");
            double pathofstatus = 100 / 5.0 / matches.Count;
            foreach (Match match in matches)
            {
                Town Town = new Town();
                Town.Country_code = match.Groups["country_code"].Value;
                Town.Code = match.Groups["code"].Value;
                Town.Time_zone = match.Groups["time_zone"].Value;
                Town.Name = match.Groups["name"].Value;
                Town.Coordinates = new Coordinates() { longitude = double.Parse(match.Groups["lon"].Value==""?"0": match.Groups["lon"].Value), latitude = double.Parse(match.Groups["lat"].Value==""?"0": match.Groups["lat"].Value) };
                var rusmatch = rus.Match(match.Groups["translations"].Value);
                if (rusmatch.Success)
                    Town.Name_rus = rusmatch.Groups["ru"].Value;
                Towns.Add(Town);
                var prbar = pb;
                pb.Dispatcher.Invoke(new Action(() => { prbar.Value += pathofstatus; }));
            }
            return Towns;
        }
        static List<Airport> LoadAirports(WebClient wb, ref ProgressBar pb)
        {
            if (!WebTools.ConnectionAvailable("http://www.google.com"))
                return null;
            //{\"code\":\"(?<code>[^\"]*)\",\"name\":\"(?<name>[^\"]*)\",\"coordinates\":{\"lon\":(?<lon>[^,]*),\"lat\":(?<lat>[^}]*)},\"time_zone\":\"(?<time_zone>[^\"]*)\",\"name_translations\":{(?<translations>[^}]*)},\"Country_code\":\"(?<Country_code>[^\"]*)\",\"Town_code\":\"(?<Town_code>[^\"]*)\"}
            //{"code":"(?<code>[^"]*)","name":"(?<name>[^"]*)","coordinates":{
            List<Airport> Airports = new List<Airport>();
            string s = Encoding.UTF8.GetString(wb.DownloadData("http://api.travelpayouts.com/data/airports.json"));
            var rx = new Regex("{\"code\":\"(?<code>[^\"]*)\",\"name\":\"(?<name>[^\"]*)\",\"coordinates\":(({\"lon\":(?<lon>[^,]*),\"lat\":(?<lat>[^}]*)})|(null)),\"time_zone\":\"(?<time_zone>[^\"]*)\",\"name_translations\":{(?<translations>[^}]*)},\"country_code\":\"(?<country_code>[^\"]*)\",\"city_code\":\"(?<city_code>[^\"]*)\"}");
            var matches = rx.Matches(s);
            var rus = new Regex("\"ru\":\"(?<ru>[^\"]*)\"");
            double pathofstatus = 100 / 5.0 / matches.Count;
            foreach (Match match in matches)
            {
                Airport airport = new Airport();
                airport.Town_code = match.Groups["city_code"].Value;
                airport.Code = match.Groups["code"].Value;
                airport.Country_code = match.Groups["country_code"].Value;
                airport.Name = match.Groups["name"].Value;
                airport.Coordinates = new Coordinates() { longitude = double.Parse(match.Groups["lon"].Value == "" ? "0" : match.Groups["lon"].Value), latitude = double.Parse(match.Groups["lat"].Value == "" ? "0" : match.Groups["lat"].Value) };
                var rusmatch = rus.Match(match.Groups["translations"].Value);
                if (rusmatch.Success && rusmatch.Groups["ru"].Value != "")
                    airport.Name_rus = rusmatch.Groups["ru"].Value;
                Airports.Add(airport);
                var prbar = pb;
                pb.Dispatcher.Invoke(new Action(() => { prbar.Value += pathofstatus; }));
            }
            return Airports;
        }
        static List<Country> CompareData(List<Country> Countries, List<Town> Towns, List<Airport> Airports,  ref ProgressBar pb)
        {
            foreach(var Country in Countries)
            {
                Country.Towns = Towns.Where(Town => Town.Country_code == Country.Code)?.ToList()?? new List<Town>();
                foreach(var Town in Country.Towns)
                {
                    Town.Airports = Airports.Where(Airport => Airport.Town_code == Town.Code)?.ToList() ?? new List<Airport>();
                }
            }
            return Countries;
        }
        public static BitmapImage getLogoAirport(int Width, int Height, string IATA)
        {
            return new BitmapImage(new Uri(String.Format(@"http://pics.avs.io/{0}/{1}/{2}.png",Width,Height, IATA), UriKind.Absolute));
        }
    }
}

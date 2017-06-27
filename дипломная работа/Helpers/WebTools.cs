using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using дипломная_работа.Model;
using static дипломная_работа.Helpers.AmadeusAPI;

namespace дипломная_работа.Helpers
{
    static class WebTools
    {
        public static bool ConnectionAvailable(string strServer = "www.google.com")
        {
            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create(strServer);
                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();
                if (HttpStatusCode.OK == rspFP.StatusCode)
                {
                    // HTTP = 200 - Интернет безусловно есть!
                    rspFP.Close();
                    return true;
                }
                else
                {
                    // сервер вернул отрицательный ответ, возможно что инета нет
                    rspFP.Close();
                    return false;
                }
            }
            catch (WebException)
            {
                // Ошибка, значит интернета у нас нет. Плачем :'(
                return false;
            }
        }
        public static Town GetUserLocation()
        {
            WebClient wb = new WebClient();
            string s = Encoding.UTF8.GetString(wb.DownloadData("http://www.travelpayouts.com/whereami?locale=ru&callback=useriata"));
            var rx = new Regex("{\"iata\":\"(?<iata>[^\"]*)\",\"name\":\"(?<name>[^\"]*)\",\"country_name\":\"(?<country_name>[^\"]+)\",\"coordinates\":\"(?<lot>[^:\"]*):(?<lon>[^:\"]*)\"}");
            var match = rx.Match(s);
            Town location = new Town();
            location.Code = match.Groups["iata"].Value;
            location.Name = match.Groups["name"].Value;
            return location;
        }
    }
    public class CurrencyConverter
    {
        //"cny":8.24394,"eur":57.1578,"mzn":1.49643,"nio":1.97342,"usd":51.1388,"hrk":7.48953
        public List<Price> ValuesToRub { get; private set; } = new List<Price>();
        public CurrencyConverter()
        {
            Tools.SetNumberDecimalSeparator();
            if (!WebTools.ConnectionAvailable("http://www.google.com"))
            {
                return;
            }
            var wb = new WebClient();
            string s = Encoding.UTF8.GetString(wb.DownloadData("http://yasen.aviasales.ru/adaptors/currency.json"));
            Regex rx = new Regex("\"(?<currency>[^\":,]{3})\":" + @"\s" + "(?<val>[0-9.]+)");
            var matches = rx.Matches(s);
            foreach (Match match in matches)
            {
                try
                {
                    ValuesToRub.Add(new Price { currency = match.Groups["currency"].Value.ToUpper(), amount = double.Parse(match.Groups["val"].Value) });
                }
                catch { }
            }
        }
        public Price getRub(Price price)
        {
            var currency = ValuesToRub.Where(val => val.currency == price.currency).FirstOrDefault();
            if (currency == null)
                return price;
            else return new Price { currency = "RUB", amount = price.amount * currency.amount };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using дипломная_работа.Helpers;

namespace дипломная_работа.Model
{
    public class Hotel
    {
        public string Name;
        public string Code;
        public string City_code;
        public double min_daily_rate;
        public string Address;
        public string Phone;
        public string awards;
        public string Amenities;
        public Image miniImage;
        public Image originalImage;
        public Hotel(AmadeusAPI.Result hotelInfo)
        {
        }
    }
}

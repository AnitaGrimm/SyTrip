using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace дипломная_работа.Model
{
    public class Town
    {
        public string Name;
        public string Name_rus;
        public string Code;
        public string Country_code;
        public Coordinates Coordinates;
        public string Time_zone;
        public List<Airport> Airports = new List<Airport>();
        //public List<Hotel> Hotels = new List<Hotel>();
        public Town()
        {

        }
        public override string ToString()
        {
            return Name_rus == "" ? Name : Name_rus;
        }
    }
    public class Coordinates
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }
}
